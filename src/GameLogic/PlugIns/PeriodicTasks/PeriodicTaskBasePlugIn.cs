// <copyright file="PeriodicTaskBasePlugIn.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.GameLogic.PlugIns.PeriodicTasks;

using System.Collections.Concurrent;
using System.Threading;
using MUnique.OpenMU.GameLogic;
using MUnique.OpenMU.GameLogic.PlugIns;
using MUnique.OpenMU.PlugIns;

/// <summary>
/// Base class for periodic task plugins.
/// </summary>
/// <typeparam name="TConfiguration">Configuration type.</typeparam>
/// <typeparam name="TState">State type.</typeparam>
public abstract class PeriodicTaskBasePlugIn<TConfiguration, TState> : IPeriodicTaskPlugIn, ISupportCustomConfiguration<TConfiguration>, IScheduledPlugIn
    where TConfiguration : PeriodicTaskConfiguration
    where TState : PeriodicTaskGameServerState
{
    private static readonly ConcurrentDictionary<Type, ConcurrentDictionary<IGameContext, TState>> States = new();

    private long _forceGeneration;

    /// <summary>
    /// Gets or sets configuration for periodic invasion.
    /// </summary>
    public TConfiguration? Configuration { get; set; }

    /// <inheritdoc />
    public DateTime? GetNextStartUtc(DateTime utcNow, TimeZoneInfo serverTimeZone)
    {
        return this.Configuration?.GetNextStartUtc(utcNow, serverTimeZone);
    }

    /// <summary>
    /// Forces the task to start on the next start check of the game server which
    /// owns this plug-in instance: every game server runs its own instance, so a
    /// force only ever reaches the server on which it was requested.
    /// </summary>
    public void ForceStart()
    {
        // A generation counter instead of a boolean, so that a forced start which
        // arrives while a run is being prepared isn't lost by a concurrent consume.
        Interlocked.Increment(ref this._forceGeneration);
    }

    /// <inheritdoc />
    public virtual async ValueTask ExecuteTaskAsync(GameContext gameContext)
    {
        var logger = gameContext.LoggerFactory.CreateLogger(this.GetType().Name);
        using var scope = logger.BeginScope(gameContext);

        var state = this.GetStateByGameContext(gameContext);

        // A manually forced start bypasses a future NextRunUtc, which is otherwise set to
        // the full task duration on every start and would swallow forced starts until it
        // elapses. It's limited to NotStarted (a fresh start) and Started (promptly
        // finishing a stuck previous run, so the start that follows can proceed), so that
        // a Prepared run still waits out its PreStartMessageDelay before starting.
        var forceGeneration = Volatile.Read(ref this._forceGeneration);
        var isForcedStartPending = IsForcedStartPending(state, forceGeneration);
        if (state.NextRunUtc > DateTime.UtcNow && !isForcedStartPending)
        {
            return;
        }

        var configuration = this.Configuration;

        if (configuration is null && this is ISupportDefaultCustomConfiguration defaultConfigSupporter)
        {
            logger.LogWarning("{description} ({gameContext}):configuration is not set. Using default configuration.", state.Description, gameContext);
            this.Configuration = configuration = defaultConfigSupporter.CreateDefaultConfig() as TConfiguration;
        }

        if (configuration is null)
        {
            logger.LogError("{description} ({gameContext}):no configuration available; can't execute task plugin.", state.Description, gameContext);
            return;
        }

        switch (state.State)
        {
            case PeriodicTaskState.NotStarted:
                {
                    // The scope is enforced here in the base flow rather than only in
                    // IsItTimeToStart, so overrides of the start predicate can't bypass
                    // it by accident. Only new runs are gated: a run which already
                    // started always finishes, so removing a server from the list
                    // mid-run still cleans up instead of freezing.
                    if (!this.IsEnabledOnContext(gameContext))
                    {
                        // Observing the force counts as consuming it for this server: it must
                        // not fire retroactively when the server is added back to the list later.
                        state.ConsumedForceGeneration = forceGeneration;
                        return;
                    }

                    if (!this.IsItTimeToStart(gameContext))
                    {
                        return;
                    }

                    if (this.IsPreviousEventStillRunning(state))
                    {
                        state.ConsumedForceGeneration = forceGeneration;
                        return;
                    }

                    state.ConsumedForceGeneration = forceGeneration;
                    state.NextRunUtc = DateTime.UtcNow.Add(configuration.PreStartMessageDelay);
                    await this.OnPrepareEventAsync(state).ConfigureAwait(false);
                    state.State = PeriodicTaskState.Prepared;
                    await this.OnPreparedAsync(state).ConfigureAwait(false);

                    if (!string.IsNullOrWhiteSpace(state.Description))
                    {
                        logger.LogDebug("{description} ({gameContext}): event prepared", state.Description, gameContext);
                    }

                    break;
                }

            case PeriodicTaskState.Prepared:
                {
                    state.NextRunUtc = DateTime.UtcNow.Add(configuration.TaskDuration);
                    state.State = PeriodicTaskState.Started;
                    state.LastRunUtc = DateTime.UtcNow;

                    await this.OnStartedAsync(state).ConfigureAwait(false);

                    if (!string.IsNullOrWhiteSpace(state.Description))
                    {
                        logger.LogDebug("{description} ({gameContext}): event started", state.Description, gameContext);
                    }

                    break;
                }

            case PeriodicTaskState.Started:
                {
                    state.State = PeriodicTaskState.NotStarted;

                    await this.OnFinishedAsync(state).ConfigureAwait(false);

                    if (!string.IsNullOrWhiteSpace(state.Description))
                    {
                        logger.LogDebug("{description} ({gameContext}): event finished", state.Description, gameContext);
                    }

                    break;
                }

            default:
                throw new NotImplementedException("Unknown state.");
        }
    }

    /// <summary>
    /// Determines whether this task runs on the game server which owns the specified context.
    /// Contexts without a game server (e.g. in tests) always run the task.
    /// </summary>
    /// <param name="gameContext">The game context.</param>
    /// <returns><c>true</c> if the task runs in this context; otherwise, <c>false</c>.</returns>
    protected bool IsEnabledOnContext(IGameContext gameContext)
    {
        return gameContext is not IGameServerContext serverContext
            || this.Configuration?.IsEnabledOnServer(serverContext.Id) != false;
    }

    /// <summary>
    /// Gets a value indicating whether the start condition of the task is met: a pending
    /// forced start or the configured timetable. The server scope is intentionally not
    /// part of this predicate; the base flow enforces it before calling it, so overrides
    /// keep working unchanged.
    /// </summary>
    /// <param name="gameContext">The game context.</param>
    /// <returns>
    ///   <c>true</c> if it's the right time to start the task; otherwise, <c>false</c>.
    /// </returns>
    protected virtual bool IsItTimeToStart(IGameContext gameContext)
    {
        // The state lookup creates the per-context state on first use; the per-state
        // force consumption keeps a forced start pending until this server starts.
        var state = this.GetStateByGameContext(gameContext);
        return IsForcedStartPending(state, Volatile.Read(ref this._forceGeneration))
            || (this.Configuration?.IsItTimeToStart(gameContext.ServerTimeZone) ?? false);
    }

    /// <summary>
    /// Determines whether the previous event run is still within its configured task duration.
    /// Prevents a new event from starting before the previous one has fully elapsed.
    /// </summary>
    /// <param name="state">The current task state.</param>
    /// <returns><c>true</c> if the previous event duration has not elapsed yet; otherwise <c>false</c>.</returns>
    protected virtual bool IsPreviousEventStillRunning(TState state)
        => state.LastRunUtc != DateTime.MinValue && state.LastRunUtc.Add(this.Configuration?.TaskDuration ?? TimeSpan.Zero) > DateTime.UtcNow;

    /// <summary>
    /// Called when the task should be prepared before starting it.
    /// </summary>
    /// <param name="state">The state.</param>
    protected abstract ValueTask OnPrepareEventAsync(TState state);

    /// <summary>
    /// Creates the state for the given context.
    /// </summary>
    /// <param name="gameContext">The game context.</param>
    /// <returns>The created state object.</returns>
    protected abstract TState CreateState(IGameContext gameContext);

    /// <summary>
    /// Get a unique state per GameContext.
    /// </summary>
    /// <param name="gameContext">GameContext.</param>
    protected TState GetStateByGameContext(IGameContext gameContext)
    {
        var type = this.GetType();

        var statesPerType = States.GetOrAdd(type, newType => new());

        return statesPerType.GetOrAdd(gameContext, _ => this.CreateState(gameContext));
    }

    /// <summary>
    /// Calls after the state changed to Prepared.
    /// </summary>
    /// <param name="state">The state.</param>
    protected abstract ValueTask OnPreparedAsync(TState state);

    /// <summary>
    /// Calls after the state changed to Started.
    /// </summary>
    /// <param name="state">State.</param>
    protected abstract ValueTask OnStartedAsync(TState state);

    /// <summary>
    /// Calls after the state changed to Finished.
    /// </summary>
    /// <param name="state">State.</param>
    protected abstract ValueTask OnFinishedAsync(TState state);

    /// <summary>
    /// Determines whether a forced start is pending for the specified state.
    /// </summary>
    /// <param name="state">The state.</param>
    /// <param name="forceGeneration">The currently requested forced-start generation.</param>
    /// <returns><c>true</c> if the state hasn't consumed the requested generation yet and may start; otherwise, <c>false</c>.</returns>
    private static bool IsForcedStartPending(TState state, long forceGeneration)
    {
        return state.ConsumedForceGeneration < forceGeneration
            && state.State is PeriodicTaskState.NotStarted or PeriodicTaskState.Started;
    }
}