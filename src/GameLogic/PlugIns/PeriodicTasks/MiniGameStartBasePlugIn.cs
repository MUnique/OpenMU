// <copyright file="MiniGameStartBasePlugIn.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.GameLogic.PlugIns.PeriodicTasks;

using System.Collections.Concurrent;
using MUnique.OpenMU.GameLogic.MiniGames;
using MUnique.OpenMU.PlugIns;

/// <summary>
/// This base plugin allows you to implement mini game starts.
/// </summary>
/// <typeparam name="TConfiguration">Implementation of the mini game configuration class.</typeparam>
/// <typeparam name="TGameState">Implementation of the mini game state class.</typeparam>
public abstract class MiniGameStartBasePlugIn<TConfiguration, TGameState> : PeriodicTaskBasePlugIn<TConfiguration, TGameState>, ISupportDefaultCustomConfiguration, IPeriodicMiniGameStartPlugIn
    where TConfiguration : MiniGameStartConfiguration
    where TGameState : PeriodicTaskGameServerState
{
    private static readonly ConcurrentDictionary<(Type PlugInType, IGameContext GameContext), EntranceAnnouncement> Announcements = new();

    /// <inheritdoc />
    public abstract MiniGameType Key { get; }

    /// <inheritdoc />
    public abstract object CreateDefaultConfig();

    /// <inheritdoc />
    public bool IsEventActive(IGameContext gameContext)
    {
        return gameContext.MiniGames.GetRunningMiniGames(this.Key).Any(IsActive);
    }

    /// <inheritdoc />
    public async ValueTask DisposeRunningGamesAsync(IGameContext gameContext)
    {
        var logger = gameContext.LoggerFactory.CreateLogger(this.GetType());
        foreach (var game in gameContext.MiniGames.GetRunningMiniGames(this.Key))
        {
            try
            {
                // Disposing warps all remaining players to the safezone and unregisters
                // the game, so that the next start creates fresh instances.
                await game.DisposeAsync().ConfigureAwait(false);
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "Error when disposing the running game {game}", game);
            }
        }
    }

    /// <inheritdoc />
    public async ValueTask<TimeSpan?> GetDurationUntilNextStartAsync(IGameContext gameContext, MiniGameDefinition miniGameDefinition)
    {
        var state = this.GetStateByGameContext(gameContext);
        if (state.State == PeriodicTaskState.Prepared)
        {
            // That's not totally correct, but should be sufficient.
            return this.Configuration?.PreStartMessageDelay;
        }

        // Entering is allowed while a live game of this definition is open. The lookup
        // must not create a game: creating on read would spawn ghost lobbies on every
        // entrance query.
        if (gameContext.MiniGames.TryGetRunningMiniGame(miniGameDefinition, null) is { State: MiniGameState.Open })
        {
            return TimeSpan.Zero;
        }

        var timeNow = new TimeOnly(DateTime.UtcNow.TimeOfDay.Ticks);
        var nextRun = this.Configuration?.Timetable.Where(time => time > timeNow).Order().FirstOrDefault();
        return nextRun - timeNow;
    }

    /// <inheritdoc />
    public override async ValueTask ExecuteTaskAsync(GameContext gameContext)
    {
        var state = this.GetStateByGameContext(gameContext);
        if (state.State == PeriodicTaskState.Started)
        {
            // Finish promptly instead of lingering in Started until NextRunUtc elapses.
            // Entering is gated by the live game instances (see
            // GetDurationUntilNextStartAsync), so nothing depends on the lingering state -
            // and it would block forced restarts behind the task duration.
            state.State = PeriodicTaskState.NotStarted;
            await this.OnFinishedAsync(state).ConfigureAwait(false);
        }

        await base.ExecuteTaskAsync(gameContext).ConfigureAwait(false);
        await this.AnnounceEntranceAsync(gameContext).ConfigureAwait(false);
    }

    /// <inheritdoc />
    public ValueTask<MiniGameContext?> GetMiniGameContextAsync(IGameContext gameContext, MiniGameDefinition miniGameDefinition)
    {
        return ValueTask.FromResult(gameContext.MiniGames.TryGetRunningMiniGame(miniGameDefinition, null));
    }

    /// <inheritdoc />
    protected override ValueTask OnPrepareEventAsync(TGameState state)
    {
        // nothing to do
        return ValueTask.CompletedTask;
    }

    /// <inheritdoc />
    protected override ValueTask OnPreparedAsync(TGameState state)
    {
        // We keep it simple and don't send a message here.
        return ValueTask.CompletedTask;
    }

    /// <inheritdoc />
    protected override bool IsPreviousEventStillRunning(TGameState state)
    {
        // A previous run only blocks a new start while its games are actually still
        // around. Once they ended and cleaned up, a new event may start even if the
        // configured task duration hasn't fully elapsed yet - e.g. when a game master
        // manually starts the next run with a start command.
        return base.IsPreviousEventStillRunning(state) && this.IsEventActive(state.Context);
    }

    /// <inheritdoc />
    protected override async ValueTask OnStartedAsync(TGameState state)
    {
        var miniGameDefinitions = state.Context.Configuration.MiniGameDefinitions
            .Where(d => d.Type == this.Key && d.MapCreationPolicy == MiniGameMapCreationPolicy.Shared);

        await this.DisposeStaleGamesAsync(state.Context).ConfigureAwait(false);

        var enterDuration = TimeSpan.Zero;
        foreach (var miniGameDefinition in miniGameDefinitions)
        {
            // we're causing that the event context gets created.
            MiniGameContext game;
            for (int attempt = 0; ; attempt++)
            {
                game = await state.Context.MiniGames.GetOrCreateAsync(miniGameDefinition, null!).ConfigureAwait(false);
                if (game is { IsDisposed: false, IsDisposing: false, State: MiniGameState.Open } || attempt >= 2)
                {
                    break;
                }

                // A stale instance slipped through (e.g. caught mid-dispose): dispose it,
                // so that the next attempt creates a fresh one.
                await game.DisposeAsync().ConfigureAwait(false);
            }

            enterDuration = miniGameDefinition.EnterDuration;
        }

        // Announce the full minutes immediately, like the former notification loop did
        // on start. The periodic tick takes over from here and announces each following
        // minute when it begins.
        var initialMinutes = (int)enterDuration.TotalMinutes;
        if (initialMinutes >= 1)
        {
            Announcements[(this.GetType(), state.Context)] = new EntranceAnnouncement { LastAnnouncedMinutes = initialMinutes, OpenAnnounced = true };
            if (this.Configuration?.EntranceOpenedMessage is { } openMessage)
            {
                await state.Context.SendGlobalNotificationAsync(string.Format(openMessage, initialMinutes)).ConfigureAwait(false);
            }
        }
    }

    /// <inheritdoc />
    protected override ValueTask OnFinishedAsync(TGameState state)
    {
        // nothing to do; The mini game will clean up itself.
        return ValueTask.CompletedTask;
    }

    /// <summary>
    /// Disposes previously started games which already ended, so that starting a new event
    /// always creates fresh game instances instead of reusing a stale one.
    /// </summary>
    /// <param name="gameContext">The game context.</param>
    private async ValueTask DisposeStaleGamesAsync(IGameContext gameContext)
    {
        foreach (var game in gameContext.MiniGames.GetRunningMiniGames(this.Key))
        {
            if (game.State is MiniGameState.Ended or MiniGameState.Disposed)
            {
                await game.DisposeAsync().ConfigureAwait(false);
            }
        }
    }

    /// <summary>
    /// Announces the entrance countdown on every periodic tick, derived from the live
    /// game state instead of a sleeping timer loop. Because nothing runs in the
    /// background, there is nothing to skip, cancel, or supersede: skipped and restarted
    /// runs are observed as a closed entrance on the next tick, at the latest.
    /// </summary>
    /// <param name="gameContext">The game context.</param>
    private async ValueTask AnnounceEntranceAsync(IGameContext gameContext)
    {
        var key = (this.GetType(), gameContext);
        var maxRemaining = gameContext.MiniGames.GetRunningMiniGames(this.Key)
            .Where(game => game.State == MiniGameState.Open)
            .Select(game => (TimeSpan?)(game.EnterEndsAtUtc - DateTime.UtcNow))
            .Max();
        if (maxRemaining is { } remaining)
        {
            var minutesLeft = (int)Math.Ceiling(remaining.TotalMinutes);
            if (minutesLeft >= 1
                && Announcements.TryGetValue(key, out var announcement)
                && announcement.LastAnnouncedMinutes != minutesLeft)
            {
                announcement.LastAnnouncedMinutes = minutesLeft;
                if (this.Configuration?.EntranceOpenedMessage is { } openMessage)
                {
                    await gameContext.SendGlobalNotificationAsync(string.Format(openMessage, minutesLeft)).ConfigureAwait(false);
                }
            }

            return;
        }

        // No open entrance left. If this run announced its opening before, announce the
        // closing exactly once - no matter if the entrance closed naturally, was skipped,
        // or the whole run was restarted.
        if (Announcements.TryGetValue(key, out var finished) && finished.OpenAnnounced && !finished.CloseAnnounced)
        {
            finished.CloseAnnounced = true;
            if (this.Configuration?.EntranceClosedMessage is { } closedMessage)
            {
                await gameContext.SendGlobalNotificationAsync(closedMessage).ConfigureAwait(false);
            }

            Announcements.TryRemove(key, out _);
        }
    }

    private static bool IsActive(MiniGameContext game)
    {
        return !game.IsDisposed && !game.IsDisposing
            && game.State is MiniGameState.Open or MiniGameState.Closed or MiniGameState.Playing;
    }

    /// <summary>
    /// Tracks the entrance announcements of one run: which minute was announced last, and
    /// whether the opening and the closing have been announced.
    /// </summary>
    private sealed class EntranceAnnouncement
    {
        /// <summary>
        /// Gets or sets the last announced remaining minutes.
        /// </summary>
        public int LastAnnouncedMinutes { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether the opening has been announced.
        /// </summary>
        public bool OpenAnnounced { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether the closing has been announced.
        /// </summary>
        public bool CloseAnnounced { get; set; }
    }
}
