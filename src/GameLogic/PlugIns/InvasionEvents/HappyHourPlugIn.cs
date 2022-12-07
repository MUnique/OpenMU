// <copyright file="HappyHourPlugIn.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.GameLogic.PlugIns.InvasionEvents;

using System.Collections.Concurrent;
using System.Runtime.InteropServices;
using MUnique.OpenMU.GameLogic.NPC;
using MUnique.OpenMU.GameLogic.Views;
using MUnique.OpenMU.PlugIns;

public class HappyHourConfiguration : PeriodicInvasionConfiguration
{
    /// <summary>
    /// Gets the default configuration.
    /// </summary>
    public static HappyHourConfiguration Default => new()
    {
        EventDuration = TimeSpan.FromHours(1),
        PreStartMessageDelay = TimeSpan.FromSeconds(0),
        Message = "Happy Hour event started!",
        Timetable = GenerateTimeSequence(TimeSpan.FromHours(6), new TimeOnly(0, 5)).ToList(), // Every 6 hours,
        ExperienceMultiplier = 1.5f,
    };

    public float ExperienceMultiplier { get; set; } = 1.5f;
}

/// <summary>
/// This plugin enables Happy Hour feature.
/// </summary>
[PlugIn(nameof(HappyHourPlugIn), "Handle Happy Hour event")]
[Guid("6542E452-9780-45B8-85AE-4036422E9A6E")]
public class HappyHourPlugIn : IPeriodicTaskPlugIn, IObjectAddedToMapPlugIn, ISupportCustomConfiguration<HappyHourConfiguration>
{
    /// <summary>
    /// Game server state per event.
    /// </summary>
    protected class GameServerState
    {
        public GameServerState(IGameContext ctx)
        {
            this.Context = ctx;
        }

        public IGameContext Context { get; private set; }

        public DateTime NextRunUtc { get; set; } = DateTime.UtcNow;

        public InvasionEventState State { get; set; } = InvasionEventState.NotStarted;

        public float NewExperienceRate { get; set; }

        public float OldExperienceRate { get; set; }
    }

    private static readonly ConcurrentDictionary<Type, ConcurrentDictionary<IGameContext, GameServerState>> _states = new();

    /// <summary>
    /// Gets or sets configuration for periodic invasion.
    /// </summary>
    public HappyHourConfiguration? Configuration { get; set; }

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

        if (configuration is null)
        {
            logger.LogWarning("configuration is not set.");
            return;
        }

        switch (state.State)
        {
            case InvasionEventState.NotStarted:
                {
                    if (!configuration.IsItTimeToStartInvasion())
                    {
                        return;
                    }

                    state.NextRunUtc = DateTime.UtcNow.Add(configuration.PreStartMessageDelay);
                    state.OldExperienceRate = gameContext.ExperienceRate;
                    state.NewExperienceRate = gameContext.ExperienceRate * configuration.ExperienceMultiplier;
                    state.State = InvasionEventState.Prepared;

                    await this.OnPreparedAsync(state).ConfigureAwait(false);

                    logger.LogInformation($"event initialized");

                    break;
                }

            case InvasionEventState.Prepared:
                {
                    state.NextRunUtc = DateTime.UtcNow.Add(configuration.EventDuration);
                    state.State = InvasionEventState.Started;

                    await this.OnStartedAsync(state).ConfigureAwait(false);

                    logger.LogInformation($"event started with EXP rate = {state.NewExperienceRate}");

                    break;
                }

            case InvasionEventState.Started:
                {
                    state.State = InvasionEventState.NotStarted;

                    await this.OnFinishedAsync(state).ConfigureAwait(false);

                    logger.LogInformation($"event finished");

                    break;
                }

            default:
                throw new NotImplementedException("Unknown state.");
        }
    }

    /// <inheritdoc />
    [System.Diagnostics.CodeAnalysis.SuppressMessage("Usage", "VSTHRD100:Avoid async void methods", Justification = "Catching all Exceptions.")]
    public virtual async void ObjectAddedToMap(GameMap map, ILocateable addedObject)
    {
    }

    /// <summary>
    /// Get a unique state per GameContext.
    /// </summary>
    /// <param name="gameContext">GameContext.</param>
    protected GameServerState GetStateByGameContext(IGameContext gameContext)
    {
        var type = this.GetType();

        var statesPerType = _states.GetOrAdd(type, newType => new() { });

        return statesPerType.GetOrAdd(gameContext, gameCtx => new GameServerState(gameContext));
    }

    /// <summary>
    /// Returns true if the player stays on the map.
    /// </summary>
    /// <param name="player">The player.</param>
    protected bool IsPlayerOnMap(Player player)
    {
        var state = this.GetStateByGameContext(player.GameContext);

        return player.CurrentMap is { } map
            && player.PlayerState.CurrentState != PlayerState.Disconnected
            && player.PlayerState.CurrentState != PlayerState.Finished;
    }

    /// <summary>
    /// Send a golden message to player's client.
    /// </summary>
    /// <param name="player">The player.</param>
    protected async Task TrySendStartMessageAsync(Player player)
    {
        var configuration = this.Configuration;

        if (configuration is null)
        {
            return;
        }

        var message = configuration.Message ?? "Happy Hour event started!";

        if (this.IsPlayerOnMap(player))
        {
            try
            {
                await player.InvokeViewPlugInAsync<IShowMessagePlugIn>(p => p.ShowMessageAsync(message, Interfaces.MessageType.GoldenCenter)).ConfigureAwait(false);
            }
            catch (Exception ex)
            {
                player.Logger.LogDebug(ex, "Unexpected error sending start message.");
            }
        }
    }

    /// <summary>
    /// Calls after the state changed to Prepared.
    /// </summary>
    /// <param name="state">State.</param>
    protected virtual async ValueTask OnPreparedAsync(GameServerState state)
    {
        await state.Context.ForEachPlayerAsync(p => this.TrySendStartMessageAsync(p)).ConfigureAwait(false);
    }

    /// <summary>
    /// Calls after the state changed to Started.
    /// </summary>
    /// <param name="state">State.</param>
    protected virtual async ValueTask OnStartedAsync(GameServerState state)
    {
        state.Context.Configuration.ExperienceRate = state.NewExperienceRate;
    }

    /// <summary>
    /// Calls after the state changed to NotStarted.
    /// </summary>
    /// <param name="state">The state.</param>
    protected virtual async ValueTask OnFinishedAsync(GameServerState state)
    {
        state.Context.Configuration.ExperienceRate = state.OldExperienceRate;
    }
}
