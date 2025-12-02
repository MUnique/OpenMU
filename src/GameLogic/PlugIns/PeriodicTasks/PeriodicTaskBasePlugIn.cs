// <copyright file="PeriodicTaskBasePlugIn.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.GameLogic.PlugIns.PeriodicTasks;

using System.Collections.Concurrent;
using MUnique.OpenMU.GameLogic;
using MUnique.OpenMU.GameLogic.PlugIns;
using MUnique.OpenMU.PlugIns;

/// <summary>
/// Base class for periodic task plugins.
/// </summary>
/// <typeparam name="TConfiguration">Configuration type.</typeparam>
/// <typeparam name="TState">State type.</typeparam>
public abstract class PeriodicTaskBasePlugIn<TConfiguration, TState> : IPeriodicTaskPlugIn, ISupportCustomConfiguration<TConfiguration>
    where TConfiguration : PeriodicTaskConfiguration
    where TState : PeriodicTaskGameServerState
{
    private static readonly ConcurrentDictionary<Type, ConcurrentDictionary<IGameContext, TState>> States = new();

    private bool _isStartForced = false;

    /// <summary>
    /// Gets or sets configuration for periodic invasion.
    /// </summary>
    public TConfiguration? Configuration { get; set; }

    /// <summary>
    /// Forces to start the task on the next start check.
    /// </summary>
    public void ForceStart()
    {
        this._isStartForced = true;
    }

    /// <inheritdoc />
    public async ValueTask ExecuteTaskAsync(GameContext gameContext)
    {
        var logger = gameContext.LoggerFactory.CreateLogger(this.GetType().Name);
        using var scope = logger.BeginScope(gameContext);

        var state = this.GetStateByGameContext(gameContext);

        if (state.NextRunUtc > DateTime.UtcNow)
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
                    if (!this.IsItTimeToStart(gameContext))
                    {
                        return;
                    }

                    this._isStartForced = false;
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
    /// Gets a value indicating whether if it's the right time to start the task.
    /// </summary>
    /// <param name="gameContext">The game context.</param>
    /// <returns>
    ///   <c>true</c> if it's the right time to start the task; otherwise, <c>false</c>.
    /// </returns>
    protected virtual bool IsItTimeToStart(IGameContext gameContext)
    {
        return this._isStartForced || (this.Configuration?.IsItTimeToStart() ?? false);
    }

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
}