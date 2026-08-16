// <copyright file="BotBuffHandler.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.GameLogic.Bots;

using System.Runtime.CompilerServices;
using MUnique.OpenMU.DataModel.Configuration;
using MUnique.OpenMU.GameLogic.NPC;
using MUnique.OpenMU.GameLogic.Offline;
using MUnique.OpenMU.GameLogic.PlayerActions;
using MUnique.OpenMU.GameLogic.PlayerActions.Quests;
using MUnique.OpenMU.Pathfinding;

/// <summary>
/// Lets a bot claim the free buff a dialog NPC offers new players (e.g. the Elf Soldier, which grants
/// defense and damage): when the server has such an NPC configured with a buff the bot qualifies for,
/// it walks up to it and requests the buff like a real player would. The visit reuses the regular player
/// actions (<see cref="TalkNpcAction"/>, <see cref="NpcBuffRequestAction"/>, <see cref="CloseNpcDialogAction"/>),
/// and while the dialog is open the player state pauses the combat AI - the bot visibly "visits" the NPC.
/// </summary>
internal static class BotBuffHandler
{
    private static readonly TalkNpcAction TalkAction = new();
    private static readonly NpcBuffRequestAction BuffRequestAction = new();
    private static readonly CloseNpcDialogAction CloseAction = new();

    /// <summary>
    /// The configured buff offers (which dialog NPC grants which buff), computed once per game configuration:
    /// this data does not change at runtime, so re-scanning every configured monster for every bot on every
    /// check would be pure waste. Weakly keyed by configuration, so a configuration which gets garbage
    /// collected (e.g. a thrown-away test context) is dropped together with its cached offers.
    /// </summary>
    private static readonly ConditionalWeakTable<GameConfiguration, BuffOffer[]> BuffOffersByConfiguration = new();

    /// <summary>
    /// This NPC defines a buff dialog, i.e. it offers a buff to the player.
    /// </summary>
    /// <param name="definition">The NPC definition.</param>
    /// <returns>True, if the NPC offers at least one buff.</returns>
    public static bool IsBuffNpc(MonsterDefinition definition)
    {
        return definition is { NpcWindow: NpcWindow.NpcDialog, Buffs.Count: > 0 };
    }

    /// <summary>
    /// Gets the buff NPC definition the game data actually configures for the given bot to receive,
    /// or null, if the server does not have one (the Elf Soldier exists only in Season Six data) or
    /// the bot does not qualify for any of its buffs. The per-NPC data comes pre-filtered from the
    /// configuration cache (see <see cref="BuffOffersByConfiguration"/>); only the checks which depend on
    /// the individual player - the level range and the active effect - run on every call.
    /// </summary>
    /// <param name="player">The bot player.</param>
    /// <returns>The buff and its NPC definition, or null.</returns>
    public static (MonsterDefinition Npc, Buff Buff)? TryGetApplicableBuff(OfflinePlayer player)
    {
        foreach (var offer in GetBuffOffers(player.GameContext.Configuration))
        {
            if (offer.Buff.MinimumLevel.HasValue && player.Level < offer.Buff.MinimumLevel.Value)
            {
                continue;
            }

            if (offer.Buff.MaximumLevel.HasValue && player.Level > offer.Buff.MaximumLevel.Value)
            {
                continue;
            }

            if (HasEffect(player, offer.EffectDefinition))
            {
                continue;
            }

            return (offer.Npc, offer.Buff);
        }

        return null;
    }

    /// <summary>
    /// Determines whether the player is already under the given magic effect.
    /// </summary>
    /// <param name="player">The bot player.</param>
    /// <param name="effectDef">The magic effect.</param>
    /// <returns>True, if the effect is active.</returns>
    public static bool HasEffect(OfflinePlayer player, MagicEffectDefinition effectDef)
    {
        if (player.MagicEffectList.ActiveEffects is not { } effects)
        {
            return false;
        }

        // Eager snapshot, like the other readers of ActiveEffects (see MagicEffectsList): the list is
        // mutated by the effect expiry timers, and a lazy enumeration from this helper tick raced them regularly at scale.
        // A torn read simply counts as "active"; the next tick then skips the trip for a moment and re-evaluates.
        try
        {
            return effects.Values.ToArray().Any(e => e?.Definition == effectDef);
        }
        catch (InvalidOperationException ex)
        {
            player.Logger.LogDebug(ex, "Bot '{Name}' encountered a concurrent modification while inspecting active effects.", player.Name);
            return true;
        }
    }

    /// <summary>
    /// Finds the position of a live buff NPC on the map, so the bot can walk to it and open its dialog.
    /// </summary>
    /// <param name="player">The bot player, whose level governs qualification.</param>
    /// <param name="map">The game map.</param>
    /// <returns>The position of a qualifying buff NPC, or null, if there is none.</returns>
    public static Point? FindBuffNpcPosition(OfflinePlayer player, GameMap map)
    {
        // The bot qualifies for the buff of one specific NPC definition; look for a live instance of
        // exactly that dialog NPC on the map (the Elf Soldiers on the maps share the definition).
        if (TryGetApplicableBuff(player) is not { } applicable)
        {
            return null;
        }

        return map.GetNpcsInRange(new Point(128, 128), 256)
            .Where(n => n.Definition == applicable.Npc)
            .SelectRandom()
            ?.Position;
    }

    /// <summary>
    /// Opens the dialog of the buff NPC standing near the given position, requests its buff and closes
    /// the dialog again.
    /// </summary>
    /// <param name="player">The bot player.</param>
    /// <param name="map">The game map.</param>
    /// <param name="npcPosition">The position of the buff NPC.</param>
    /// <returns>True, if the buff was requested; false, if no qualifying NPC is there.</returns>
    public static async ValueTask<bool> TryRequestBuffAsync(OfflinePlayer player, GameMap map, Point npcPosition)
    {
        // Re-confirm at claim time: the bot only gets the buff as long as it still qualifies (the
        // effect might have been granted in the meantime, or the bot leveled out of the range).
        if (TryGetApplicableBuff(player) is not { } applicable)
        {
            return false;
        }

        var npc = map.GetNpcsInRange(npcPosition, 4)
            .FirstOrDefault(n => n.Definition == applicable.Npc);
        if (npc is null)
        {
            player.Logger.LogDebug("Bot '{Name}' found no buff NPC near {Position} and gives up the trip.", player.Name, npcPosition);
            return false;
        }

        await TalkAction.TalkToNpcAsync(player, npc).ConfigureAwait(false);
        if (player.OpenedNpc is null)
        {
            player.Logger.LogDebug("Bot '{Name}' could not open the dialog of '{Npc}'.", player.Name, npc.Definition.Designation);
            return false;
        }

        try
        {
            await BuffRequestAction.RequestBuffAsync(player).ConfigureAwait(false);
            player.Logger.LogDebug("Bot '{Name}' requested the buff of '{Npc}'.", player.Name, npc.Definition.Designation);
        }
        finally
        {
            await CloseAction.CloseNpcDialogAsync(player).ConfigureAwait(false);
        }

        return true;
    }

    /// <summary>
    /// Gets the buff offers the given game configuration defines, computing them once per configuration.
    /// </summary>
    /// <param name="configuration">The game configuration.</param>
    /// <returns>The buff offers.</returns>
    private static BuffOffer[] GetBuffOffers(GameConfiguration configuration)
    {
        return BuffOffersByConfiguration.GetValue(configuration, static c => CreateBuffOffers(c).ToArray());
    }

    /// <summary>
    /// Enumerates the buff offers of a configuration, dropping every buff which could not be applied to a
    /// character anyway (see <see cref="IsApplicable"/>). The filter is configuration-static and therefore
    /// folded into the cache instead of being repeated per bot per check.
    /// </summary>
    /// <param name="configuration">The game configuration.</param>
    /// <returns>An enumeration of the applicable buff offers.</returns>
    private static IEnumerable<BuffOffer> CreateBuffOffers(GameConfiguration configuration)
    {
        foreach (var monster in configuration.Monsters.Where(IsBuffNpc))
        {
            foreach (var buff in monster.Buffs)
            {
                if (IsApplicable(buff) is not { } effectDef)
                {
                    continue;
                }

                yield return new BuffOffer(monster, buff, effectDef);
            }
        }
    }

    /// <summary>
    /// Determines whether the given buff could be applied to a character at all, matching the acceptance
    /// criteria of <see cref="NpcBuffRequestAction"/>: the effect needs a duration and at least one power-up
    /// which targets an attribute. Returns the (non-null) effect definition in that case.
    /// </summary>
    /// <param name="buff">The buff.</param>
    /// <returns>The effect definition, or null, if the buff can not be applied.</returns>
    private static MagicEffectDefinition? IsApplicable(Buff buff)
    {
        if (buff.MagicEffectDefinition is not { Duration: not null } effectDef)
        {
            return null;
        }

        // A buff with no duration can not be applied.
        if (effectDef.Duration?.ConstantValue.Value <= 0)
        {
            return null;
        }

        return effectDef.PowerUpDefinitions.Any(def => def.Boost is not null && def.TargetAttribute is not null)
            ? effectDef
            : null;
    }

    /// <summary>
    /// A single configured buff offer: the dialog NPC, the buff itself and its (non-null) effect definition.
    /// </summary>
    private sealed record BuffOffer(MonsterDefinition Npc, Buff Buff, MagicEffectDefinition EffectDefinition);
}