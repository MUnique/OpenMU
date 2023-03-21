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

    /// <summary>
    /// Gets or sets configuration for periodic invasion.
    /// </summary>
    public TConfiguration? Configuration { get; set; }

    /// <inheritdoc />
    public async ValueTask ExecuteTaskAsync(GameContext gameContext)
    {
        var logger = gameContext.LoggerFactory.CreateLogger(this.GetType().Name);

        var state = this.GetStateByGameContext(gameContext);

        if (state.NextRunUtc > DateTime.UtcNow)
        {
            return;
        }

        var configuration = this.Configuration;

        if (configuration is null && this is ISupportDefaultCustomConfiguration defaultConfigSupporter)
        {
            logger.LogWarning("configuration is not set. Using default configuration.");
            this.Configuration = configuration = defaultConfigSupporter.CreateDefaultConfig() as TConfiguration;
        }

        if (configuration is null)
        {
            logger.LogError("no configuration available; can't execute task plugin.");
            return;
        }

        switch (state.State)
        {
            case PeriodicTaskState.NotStarted:
                {
                    if (!configuration.IsItTimeToStart())
                    {
                        return;
                    }

                    state.NextRunUtc = DateTime.UtcNow.Add(configuration.PreStartMessageDelay);
                    await this.OnPrepareEventAsync(state).ConfigureAwait(false);
                    state.State = PeriodicTaskState.Prepared;
                    await this.OnPreparedAsync(state).ConfigureAwait(false);

                    logger.LogInformation($"{state.Description}: event prepared");

                    break;
                }

            case PeriodicTaskState.Prepared:
                {
                    state.NextRunUtc = DateTime.UtcNow.Add(configuration.TaskDuration);
                    state.State = PeriodicTaskState.Started;

                    await this.OnStartedAsync(state).ConfigureAwait(false);

                    logger.LogInformation($"{state.Description}: event started");

                    break;
                }

            case PeriodicTaskState.Started:
                {
                    state.State = PeriodicTaskState.NotStarted;

                    await this.OnFinishedAsync(state).ConfigureAwait(false);

                    logger.LogInformation($"{state.Description}: event finished");

                    break;
                }

            default:
                throw new NotImplementedException("Unknown state.");
        }
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
