// <copyright file="KanturuTowerEntry.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.GameLogic.MiniGames.Kanturu;

using MUnique.OpenMU.DataModel.Configuration;
using MUnique.OpenMU.Pathfinding;

/// <summary>
/// Tower entry without a running event game, e.g. after a server restart while the
/// Tower of Refinement window is still open.
/// </summary>
public static class KanturuTowerEntry
{
    /// <summary>
    /// Gets the remaining tower window, unless a running event game owns entry itself.
    /// Games which already ended are ignored: they are tearing down and must not block
    /// the tower.
    /// </summary>
    /// <param name="player">The player which tries to enter.</param>
    /// <param name="definition">The mini game definition.</param>
    /// <returns>The remaining open window, or <c>null</c> when tower entry doesn't apply.</returns>
    public static TimeSpan? GetRemainingTowerWindow(Player player, MiniGameDefinition definition)
    {
        if (KanturuTowerWindow.GetOpenUntilUtc(player.GameContext) is not { } until || until <= DateTime.UtcNow)
        {
            return null;
        }

        if (GetLiveGame(player, definition) is KanturuContext { TowerMode: false })
        {
            return null;
        }

        return until - DateTime.UtcNow;
    }

    /// <summary>
    /// Makes sure a tower game exists for the player to enter: a running one is reused,
    /// a stale empty one is replaced, otherwise a new one is created without the event phases.
    /// </summary>
    /// <param name="player">The player which tries to enter.</param>
    /// <param name="definition">The mini game definition.</param>
    /// <returns><c>true</c> when tower entry applies; otherwise, <c>false</c>.</returns>
    public static async ValueTask<bool> EnsureTowerGameAsync(Player player, MiniGameDefinition definition)
    {
        try
        {
            return await EnsureTowerGameCoreAsync(player, definition).ConfigureAwait(false);
        }
        catch (Exception ex)
        {
            // Never break entering: without a tower game the generic entry below
            // answers with a clean rejection instead.
            player.Logger.LogError(ex, "Failed to ensure the Kanturu tower game.");
            return false;
        }
    }

    /// <summary>
    /// Creates the transient definition for a tower-only game. It keys identically to
    /// the event definition, but its timers host the tower for the remaining window
    /// instead of running the event. It's never persisted.
    /// </summary>
    /// <param name="source">The mini game definition.</param>
    /// <param name="remainingWindow">The remaining tower window.</param>
    /// <returns>The tower game definition.</returns>
    public static MiniGameDefinition CreateTowerDefinition(MiniGameDefinition source, TimeSpan remainingWindow)
    {
        return new TowerMiniGameDefinition(source, remainingWindow);
    }

    /// <summary>
    /// Gets where tower entrants arrive: the Nightmare zone entry, where the transition
    /// from the Maya fight leads. It's <c>null</c> when no transition is configured.
    /// </summary>
    /// <param name="definition">The event definition.</param>
    /// <returns>The tower entry point, if configured.</returns>
    internal static Point? GetTowerEntryPoint(KanturuEventDefinition definition)
    {
        if (definition.Phases.FirstOrDefault(phase => phase.Kind == KanturuPhaseKind.Transition)?.Transition is { } transition)
        {
            return new Point(transition.EntryPointX, transition.EntryPointY);
        }

        return null;
    }

    private static async ValueTask<bool> EnsureTowerGameCoreAsync(Player player, MiniGameDefinition definition)
    {
        if (GetRemainingTowerWindow(player, definition) is not { } remaining)
        {
            return false;
        }

        if (player.GameContext.MiniGames.TryGetRunningMiniGame(definition, null) is KanturuContext tower)
        {
            if (!tower.TowerMode && tower.State is not (MiniGameState.Ended or MiniGameState.Disposed))
            {
                return false; // A live event owns entry; never break it.
            }

            // A running tower is reused. Anything else found here is tearing down;
            // dispose it so the creation below starts fresh instead of reusing a dead map.
            if (tower.TowerMode && (tower.State is MiniGameState.Open or MiniGameState.Playing || tower.PlayerCount > 0))
            {
                return true;
            }

            await tower.DisposeAsync().ConfigureAwait(false);
        }

        var game = await player.GameContext.MiniGames.GetOrCreateAsync(CreateTowerDefinition(definition, remaining), player).ConfigureAwait(false);
        return game is KanturuContext;
    }

    private static MiniGameContext? GetLiveGame(Player player, MiniGameDefinition definition)
    {
        var game = player.GameContext.MiniGames.TryGetRunningMiniGame(definition, null);
        return game is { IsDisposed: false, IsDisposing: false }
            && game.State is not (MiniGameState.Ended or MiniGameState.Disposed)
            ? game
            : null;
    }

    /// <summary>
    /// A mini game definition which carries the tower timers. The collections stay
    /// empty: a tower game spawns no waves, grants no rewards and runs no change events.
    /// </summary>
    private sealed class TowerMiniGameDefinition : MiniGameDefinition
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="TowerMiniGameDefinition"/> class.
        /// </summary>
        /// <param name="source">The mini game definition to take the key and capacity from.</param>
        /// <param name="remainingWindow">The remaining tower window.</param>
        public TowerMiniGameDefinition(MiniGameDefinition source, TimeSpan remainingWindow)
        {
            this.Type = source.Type;
            this.Name = source.Name;
            this.Description = source.Description;
            this.GameLevel = source.GameLevel;
            this.MapCreationPolicy = source.MapCreationPolicy;
            this.Entrance = source.Entrance;
            this.MaximumPlayerCount = source.MaximumPlayerCount;
            this.AllowParty = source.AllowParty;
            this.SaveRankingStatistics = false;
            this.EnterDuration = TimeSpan.Zero;
            this.GameDuration = remainingWindow;
            this.ExitDuration = TimeSpan.Zero;
            this.Rewards = new List<MiniGameReward>();
            this.SpawnWaves = new List<MiniGameSpawnWave>();
            this.ChangeEvents = new List<MiniGameChangeEvent>();
        }
    }
}
