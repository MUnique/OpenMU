// <copyright file="BotMiniGameHandler.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.GameLogic.Bots;

using MUnique.OpenMU.DataModel.Configuration;
using MUnique.OpenMU.GameLogic.Attributes;
using MUnique.OpenMU.GameLogic.MiniGames;
using MUnique.OpenMU.GameLogic.Offline;
using MUnique.OpenMU.GameLogic.PlayerActions.MiniGames;

/// <summary>
/// Lets server-side bots take part in the mini game events (Blood Castle, Devil Square, Chaos
/// Castle) - but only ever in the wake of a human: when a real player who leads a party with bots
/// enters an event with their own ticket, the party's bots follow them in. Bots never enter on
/// their own, and they don't need tickets - the leader's entry is what legitimizes the visit.
/// Each bot is checked against the same entry rules a player faces (the event level bracket,
/// the master class requirement, the player killer restriction); a bot which does not qualify
/// says goodbye and leaves the party to go back to its own hunting life, like a player who cannot
/// join the run. Inside, the bot's routine switches to the event mode of the
/// <see cref="BotNavigator"/>; death and the event's end need no special handling, because the
/// engine respawns a dead bot at the map's safezone (exactly like a player, which removes it from
/// the event) and the event itself warps the remaining participants out when it ends.
/// </summary>
internal static class BotMiniGameHandler
{
    /// <summary>
    /// Takes the snapshot of the bots which would follow the given player into a mini game.
    /// Must be called BEFORE the player actually enters: entering an event which disallows
    /// parties (Chaos Castle) kicks the entering player out of its party, and with it the
    /// knowledge of who was going to follow.
    /// </summary>
    /// <param name="player">The player about to enter a mini game.</param>
    /// <returns>The party bots to bring along; empty when the player is a bot itself, has no party or is not its master.</returns>
    internal static IReadOnlyList<OfflinePlayer> SnapshotPartyBots(Player player)
    {
        if (player is OfflinePlayer
            || player.Party is not { } party
            || !ReferenceEquals(party.PartyMaster, player))
        {
            return [];
        }

        return party.PartyList
            .OfType<OfflinePlayer>()
            .Where(bot => bot.Account?.IsBot == true)
            .ToList();
    }

    /// <summary>
    /// Brings the party bots of a player who just successfully entered a mini game along into it.
    /// Each bot's entry is queued into its own MuHelper tick (see <see cref="OfflinePlayer.PendingBotActions"/>),
    /// because warping and effect-clearing mutate the bot's state.
    /// </summary>
    /// <param name="leader">The party leader who entered the mini game.</param>
    /// <param name="bots">The snapshot taken by <see cref="SnapshotPartyBots"/> before the entry.</param>
    /// <param name="definition">The definition of the entered mini game.</param>
    /// <param name="miniGame">The mini game instance the leader entered.</param>
    internal static void BringPartyBotsAlong(Player leader, IReadOnlyList<OfflinePlayer> bots, MiniGameDefinition definition, MiniGameContext miniGame)
    {
        foreach (var bot in bots)
        {
            bot.PendingBotActions.Enqueue(() => TryEnterAsync(bot, leader, definition, miniGame));
        }
    }

    /// <summary>
    /// Determines whether the bot passes the same entry restrictions <c>EnterMiniGameAction</c>
    /// checks for a player: the event's character level bracket, the master class requirement and
    /// the player killer restriction. Pure decision logic - exposed for unit tests.
    /// </summary>
    /// <param name="bot">The bot which wants to follow its leader in.</param>
    /// <param name="definition">The mini game definition.</param>
    /// <param name="reason">The human-readable reason when the bot does not qualify.</param>
    internal static bool IsEligible(Player bot, MiniGameDefinition definition, out string reason)
    {
        var level = (int)(bot.Attributes?[Stats.Level] ?? 0);

        // The special characters (Magic Gladiator, Dark Lord, Rage Fighter, Summoner) enter the events in
        // their own level bracket - the same distinction EnterMiniGameAction makes for a player. Judging
        // them by the regular bracket kicked a qualified Magic Gladiator out of its leader's party as
        // "below the minimum" (and would have let it in past its own maximum).
        var isSpecialCharacter = bot.SelectedCharacter?.IsSpecialCharacter() == true;
        var minimumLevel = isSpecialCharacter ? definition.MinimumSpecialCharacterLevel : definition.MinimumCharacterLevel;
        var maximumLevel = isSpecialCharacter ? definition.MaximumSpecialCharacterLevel : definition.MaximumCharacterLevel;

        if (level < minimumLevel)
        {
            reason = $"level {level} is below the minimum of {minimumLevel}";
            return false;
        }

        if (level > maximumLevel)
        {
            reason = $"level {level} is above the maximum of {maximumLevel}";
            return false;
        }

        if (definition.RequiresMasterClass && bot.SelectedCharacter?.CharacterClass?.IsMasterClass is not true)
        {
            reason = "it has not evolved into a master class yet";
            return false;
        }

        if (!definition.ArePlayerKillersAllowedToEnter && bot.SelectedCharacter?.State >= HeroState.PlayerKiller1stStage)
        {
            reason = "player killers cannot enter";
            return false;
        }

        reason = string.Empty;
        return true;
    }

    private static async ValueTask TryEnterAsync(OfflinePlayer bot, Player leader, MiniGameDefinition definition, MiniGameContext miniGame)
    {
        if (bot.PlayerState.CurrentState != PlayerState.EnteredWorld
            || !bot.IsAlive
            || bot.CurrentMiniGame is not null)
        {
            return;
        }

        if (!IsEligible(bot, definition, out var reason))
        {
            // Like a player who cannot join the run: the bot says goodbye and goes back to its
            // own hunting life instead of waiting at the gate.
            bot.Logger.LogInformation(
                "Bot '{Name}' cannot follow '{Leader}' into {Event} ({Reason}) and leaves the party.",
                bot.Name,
                leader.Name,
                definition.Name,
                reason);
            if (bot.Party is { } party)
            {
                await party.KickMySelfAsync(bot).ConfigureAwait(false);
            }

            return;
        }

        if (definition.Entrance is not { } entrance)
        {
            return;
        }

        var enterResult = await miniGame.TryEnterAsync(bot).ConfigureAwait(false);
        if (enterResult != EnterResult.Success)
        {
            // Full or already closed - not the bot's fault; it stays in the party and waits for
            // the leader outside, hunting normally.
            bot.Logger.LogInformation(
                "Bot '{Name}' could not follow '{Leader}' into {Event}: {Result}.",
                bot.Name,
                leader.Name,
                definition.Name,
                enterResult);
            return;
        }

        // Mirror of the player entry flow in EnterMiniGameAction, without ticket and entrance fee.
        if (!definition.AllowParty && bot.Party is { } noPartyEventParty)
        {
            await noPartyEventParty.KickMySelfAsync(bot).ConfigureAwait(false);
        }

        await bot.MagicEffectList.ClearEffectsAfterDeathAsync().ConfigureAwait(false);
        await bot.RemoveSummonAsync().ConfigureAwait(false);
        await bot.WarpToAsync(entrance).ConfigureAwait(false);
        bot.Logger.LogInformation(
            "Bot '{Name}' follows '{Leader}' into {Event}.",
            bot.Name,
            leader.Name,
            definition.Name);
    }
}
