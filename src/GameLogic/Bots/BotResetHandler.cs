// <copyright file="BotResetHandler.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.GameLogic.Bots;

using MUnique.OpenMU.GameLogic.Attributes;
using MUnique.OpenMU.GameLogic.Offline;
using MUnique.OpenMU.GameLogic.PlugIns;
using MUnique.OpenMU.GameLogic.Resets;

/// <summary>
/// Performs character resets for bots on servers where the <see cref="ResetFeaturePlugIn"/> is enabled,
/// and provides the reset-aware effective level used by the bot logic. Everything is driven by the
/// server's actual <see cref="ResetConfiguration"/> (and <see cref="ResetProgressionCalculator"/>), so
/// bots follow the same reset rules as the human players of that particular server; when the feature
/// is disabled, none of this changes bot behavior at all.
/// </summary>
/// <remarks>
/// The reset itself mirrors the effect of <see cref="ResetCharacterAction"/>, with two deliberate
/// differences for the connection-less bot ghosts: the costs (zen, reset items) are skipped by default,
/// because bots don't take part in the economy the costs are balanced for (see
/// <see cref="BotConfiguration.BotsPayResetCosts"/>), and the <see cref="ResetConfiguration.LogOut"/>
/// step is replaced by continuing in place - a bot has no client to log out.
/// </remarks>
internal static class BotResetHandler
{
    /// <summary>
    /// Gets the server's reset configuration, or null when the reset feature is not enabled.
    /// </summary>
    /// <param name="gameContext">The game context.</param>
    /// <returns>The reset configuration, or null.</returns>
    public static ResetConfiguration? GetResetConfiguration(IGameContext gameContext)
        => gameContext.FeaturePlugIns.GetPlugIn<ResetFeaturePlugIn>()?.Configuration;

    /// <summary>
    /// Gets the player's effective level for bot decisions: on a reset server a freshly reset
    /// character is back at the configured <see cref="ResetConfiguration.LevelAfterReset"/> but
    /// fights with the accumulated power of all its resets, so the
    /// plain level would misjudge it everywhere (target safety, map choice, party matching). Each
    /// reset counts as the level span it took (<see cref="ResetConfiguration.RequiredLevel"/>).
    /// Master levels count on top (like the game's own total level), so an evolved master keeps
    /// being judged stronger than a plain level-capped character.
    /// Without the reset feature this is the character level plus the master level.
    /// </summary>
    /// <param name="player">The player.</param>
    /// <returns>The effective level.</returns>
    public static int GetEffectiveLevel(Player player)
    {
        var level = (int)(player.Attributes?[Stats.Level] ?? 1)
                    + (int)(player.Attributes?[Stats.MasterLevel] ?? 0f);
        if (GetResetConfiguration(player.GameContext) is not { } configuration)
        {
            return level;
        }

        var resets = (int)(player.Attributes?[Stats.Resets] ?? 0f);
        return (resets * Math.Max(0, configuration.RequiredLevel)) + level;
    }

    /// <summary>
    /// Determines whether the bot is currently eligible for a reset: the reset feature is enabled,
    /// the required level is reached and the reset limit (if any) is not yet exhausted.
    /// </summary>
    /// <param name="player">The bot player.</param>
    /// <param name="configuration">The reset configuration.</param>
    /// <returns>True, if the bot can reset now.</returns>
    public static bool IsResetDue(Player player, ResetConfiguration configuration)
    {
        if (player.Attributes is not { } attributes)
        {
            return false;
        }

        if (player.Level < configuration.RequiredLevel)
        {
            return false;
        }

        var nextResetCount = (int)attributes[Stats.Resets] + 1;
        return configuration.ResetLimit is not > 0 || nextResetCount <= configuration.ResetLimit;
    }

    /// <summary>
    /// Resets the bot character, mirroring the effect of <see cref="ResetCharacterAction"/>: the reset
    /// count goes up, the level drops to <see cref="ResetConfiguration.LevelAfterReset"/>, the stats and
    /// level-up points follow the server's configuration, and with <see cref="ResetConfiguration.MoveHome"/>
    /// the bot warps back to its class home town. Afterwards the regular level-up progression is fired,
    /// so the bot immediately re-invests the granted point pool according to its class build - until that
    /// runs, the damage-based target safety keeps the temporarily weak bot away from monsters it can no
    /// longer handle.
    /// </summary>
    /// <param name="player">The bot player.</param>
    /// <param name="configuration">The reset configuration.</param>
    /// <param name="payCosts">Whether the configured costs (zen, reset items) are consumed like for a human player.</param>
    /// <returns>True, if the reset was performed.</returns>
    public static async ValueTask<bool> TryResetAsync(OfflinePlayer player, ResetConfiguration configuration, bool payCosts)
    {
        if (player.Attributes is not { } attributes
            || player.SelectedCharacter is not { CharacterClass: not null } character
            || !IsResetDue(player, configuration))
        {
            return false;
        }

        var resetProgression = ResetProgressionCalculator.Calculate(
            (int)attributes[Stats.Resets],
            (int)attributes[Stats.PointsPerReset],
            configuration);

        if (payCosts && !await TryConsumeCostsAsync(player, configuration, resetProgression).ConfigureAwait(false))
        {
            return false;
        }

        attributes[Stats.Resets] = resetProgression.NextResetCount;
        attributes[Stats.Level] = configuration.LevelAfterReset;
        character.Experience = 0;

        if (configuration.ResetStats)
        {
            character.CharacterClass!.StatAttributes
                .Where(s => s.IncreasableByPlayer)
                .ForEach(s => attributes[s.Attribute] = s.BaseValue);
        }

        if (configuration.ReplacePointsPerReset)
        {
            character.LevelUpPoints = resetProgression.TotalPointsAfterReset;
        }
        else
        {
            character.LevelUpPoints += resetProgression.PointsForReset;
        }

        player.Logger.LogInformation(
            "Bot '{Name}' performed reset {ResetCount} and got {Points} points to invest.",
            player.Name,
            resetProgression.NextResetCount,
            character.LevelUpPoints);

        if (configuration.MoveHome)
        {
            await MoveHomeAsync(player).ConfigureAwait(false);
        }

        // No LogOut for the connection-less ghost - instead re-run the level-up progression, which
        // invests the whole granted point pool and re-checks the learnable skills (queued into the
        // bot's AI tick by BotSkillProgressionPlugIn, exactly like for an earned level-up).
        player.GameContext.PlugInManager.GetPlugInPoint<ICharacterLevelUpPlugIn>()?.CharacterLeveledUp(player);

        try
        {
            // Persist right away instead of waiting for the periodic save - losing a whole performed
            // reset to a crash within that window would hurt far more than ordinary hunting progress.
            await player.SaveProgressAsync().ConfigureAwait(false);
        }
        catch (Exception ex)
        {
            player.Logger.LogWarning(ex, "Couldn't save bot '{Name}' right after its reset; the periodic save will retry.", player.Name);
        }

        return true;
    }

    /// <summary>
    /// Consumes the configured reset costs like <see cref="ResetCharacterAction"/> does for a human
    /// player: the required zen and the required amount of the configured reset item. Only used when
    /// <see cref="BotConfiguration.BotsPayResetCosts"/> is enabled.
    /// </summary>
    private static async ValueTask<bool> TryConsumeCostsAsync(OfflinePlayer player, ResetConfiguration configuration, ResetProgression resetProgression)
    {
        IList<Item> requiredItems = [];
        if (resetProgression.RequiredItemAmount > 0 && configuration.RequiredResetItem is { } requiredDefinition)
        {
            requiredItems = player.Inventory?.Items
                .Where(item => item.Definition is { } definition
                               && definition.Group == requiredDefinition.Group
                               && definition.Number == requiredDefinition.Number)
                .Take(resetProgression.RequiredItemAmount)
                .ToList() ?? [];
            if (requiredItems.Count < resetProgression.RequiredItemAmount)
            {
                return false;
            }
        }

        if (player.Money < resetProgression.RequiredZen
            || (resetProgression.RequiredZen > 0 && !player.TryRemoveMoney(resetProgression.RequiredZen)))
        {
            return false;
        }

        foreach (var item in requiredItems)
        {
            await player.DestroyInventoryItemAsync(item).ConfigureAwait(false);
        }

        return true;
    }

    /// <summary>
    /// Warps the bot to a spawn gate of its class home map, the live-ghost equivalent of the
    /// position rewrite <see cref="ResetCharacterAction"/> performs before logging a player out.
    /// </summary>
    private static async ValueTask MoveHomeAsync(OfflinePlayer player)
    {
        var homeMapDefinition = player.SelectedCharacter?.CharacterClass?.HomeMap;
        if (homeMapDefinition is null
            || await player.GameContext.GetMapAsync((ushort)homeMapDefinition.Number).ConfigureAwait(false) is not { } homeMap
            || homeMap.Definition.ExitGates.Where(g => g.IsSpawnGate).SelectRandom() is not { } spawnGate)
        {
            return;
        }

        await player.WarpToAsync(spawnGate).ConfigureAwait(false);
    }
}
