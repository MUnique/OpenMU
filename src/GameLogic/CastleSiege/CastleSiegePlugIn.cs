// <copyright file="CastleSiegePlugIn.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.GameLogic.CastleSiege;

using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Threading;
using MUnique.OpenMU.DataModel.Configuration;
using MUnique.OpenMU.GameLogic.PlugIns;
using MUnique.OpenMU.PlugIns;

/// <summary>
/// Drives the weekly Castle Siege state cycle.
/// </summary>
[PlugIn]
[Display(Name = nameof(PlugInResources.CastleSiegePlugIn_Name), Description = nameof(PlugInResources.CastleSiegePlugIn_Description), ResourceType = typeof(PlugInResources))]
[Guid("B7F62FA9-59E6-49E9-B499-0358A14957CF")]
public class CastleSiegePlugIn : IPeriodicTaskPlugIn, IObjectAddedToMapPlugIn, IObjectRemovedFromMapPlugIn
{
    private static readonly TimeSpan NpcSaveInterval = TimeSpan.FromMinutes(2);

    private readonly ConditionalWeakTable<IGameContext, CastleSiegeContext> _contexts = new();
    private readonly TimeProvider _timeProvider;

    private int _forceRequestVersion;
    private int _forcedState = (int)CastleSiegeState.Start;

    /// <summary>
    /// Initializes a new instance of the <see cref="CastleSiegePlugIn"/> class.
    /// </summary>
    public CastleSiegePlugIn()
        : this(TimeProvider.System)
    {
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="CastleSiegePlugIn"/> class.
    /// </summary>
    /// <param name="timeProvider">The time provider.</param>
    internal CastleSiegePlugIn(TimeProvider timeProvider)
    {
        this._timeProvider = timeProvider;
    }

    /// <inheritdoc />
    public ValueTask ExecuteTaskAsync(GameContext gameContext) => this.ExecuteTaskAsync((IGameContext)gameContext);

    /// <inheritdoc />
    public void ForceStart() => this.ForceState(CastleSiegeState.Start);

    /// <inheritdoc />
    public async ValueTask ObjectAddedToMapAsync(GameMap map, ILocateable addedObject)
    {
        if (addedObject is Player player
            && this.GetContext(player.GameContext) is { } context)
        {
            context.TrackPlayer(player, map);
            await SynchronizePlayerAsync(player, map, context).ConfigureAwait(false);
        }
    }

    /// <inheritdoc />
    public ValueTask ObjectRemovedFromMapAsync(GameMap map, ILocateable removedObject)
    {
        if (removedObject is Player player
            && this.GetContext(player.GameContext) is { } context)
        {
            context.UntrackPlayer(player);
        }

        return ValueTask.CompletedTask;
    }

    /// <summary>
    /// Forces all Castle Siege contexts to enter a state on their next timer tick.
    /// </summary>
    /// <param name="state">The state to enter.</param>
    public void ForceState(CastleSiegeState state)
    {
        Volatile.Write(ref this._forcedState, (int)state);
        Interlocked.Increment(ref this._forceRequestVersion);
    }

    /// <summary>
    /// Gets the Castle Siege runtime context for a game context, if it has already been initialized.
    /// </summary>
    /// <param name="gameContext">The game context.</param>
    /// <returns>The Castle Siege context, or <see langword="null"/>.</returns>
    public CastleSiegeContext? GetContext(IGameContext gameContext)
    {
        return this._contexts.TryGetValue(gameContext, out var context) ? context : null;
    }

    /// <summary>
    /// Executes the periodic task against an abstract game context.
    /// </summary>
    /// <param name="gameContext">The game context.</param>
    internal async ValueTask ExecuteTaskAsync(IGameContext gameContext)
    {
        var configuration = gameContext.Configuration.CastleSiegeConfiguration;
        if (configuration is not { Enabled: true })
        {
            return;
        }

        var logger = gameContext.LoggerFactory.CreateLogger(this.GetType().Name);
        using var scope = logger.BeginScope(gameContext);
        CastleSiegeContext context;
        try
        {
            context = this._contexts.GetValue(gameContext, key => new CastleSiegeContext(key, configuration));
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "The Castle Siege context could not be created.");
            return;
        }

        if (!await context.ExecutionLock.WaitAsync(0).ConfigureAwait(false))
        {
            return;
        }

        try
        {
            var utcNow = this._timeProvider.GetUtcNow().UtcDateTime;
            if (!context.IsInitialized)
            {
                await context.InitializeAsync(utcNow).ConfigureAwait(false);
                this.ConfigureNotifications(context, utcNow);
                await this.OnEnterStateAsync(context, true).ConfigureAwait(false);
                await this.BroadcastStateUpdateAsync(context).ConfigureAwait(false);
                logger.LogInformation(
                    "Castle Siege initialized in state {state}; the state ends at {stateEndUtc}.",
                    context.CurrentState,
                    context.StateEndTimeUtc);
            }

            var forceRequestVersion = Volatile.Read(ref this._forceRequestVersion);
            if (context.LastForceRequestVersion != forceRequestVersion)
            {
                context.LastForceRequestVersion = forceRequestVersion;
                var forcedState = (CastleSiegeState)Volatile.Read(ref this._forcedState);
                await this.ChangeStateAsync(context, context.Schedule.CreatePeriod(forcedState, utcNow), logger).ConfigureAwait(false);
            }

            await this.AdvanceExpiredStatesAsync(context, utcNow, logger).ConfigureAwait(false);
            await this.OnTickAsync(context, utcNow).ConfigureAwait(false);
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Unexpected error while executing the Castle Siege state machine.");
        }
        finally
        {
            context.ExecutionLock.Release();
        }
    }

    /// <summary>
    /// Broadcasts a state update to players on the Castle Siege map.
    /// </summary>
    /// <param name="context">The Castle Siege context.</param>
    /// <remarks>The packet-backed implementation is added with the Castle Siege network view phase.</remarks>
    protected virtual ValueTask BroadcastStateUpdateAsync(CastleSiegeContext context) => ValueTask.CompletedTask;

    private static TimeSpan? GetNotificationInterval(CastleSiegeState state)
    {
        return state switch
        {
            CastleSiegeState.RegisterGuild or CastleSiegeState.RegisterMark or CastleSiegeState.Start => TimeSpan.FromMinutes(30),
            CastleSiegeState.Idle3 or CastleSiegeState.Notify => TimeSpan.FromMinutes(120),
            _ => null,
        };
    }

    private static DateTime GetNextInterval(DateTime stateStartUtc, DateTime utcNow, TimeSpan interval)
    {
        if (utcNow < stateStartUtc)
        {
            return stateStartUtc + interval;
        }

        var elapsedTicks = utcNow.Ticks - stateStartUtc.Ticks;
        var completedIntervals = elapsedTicks / interval.Ticks;
        return stateStartUtc.AddTicks((completedIntervals + 1) * interval.Ticks);
    }

    private static async ValueTask SynchronizePlayerAsync(
        Player player,
        GameMap map,
        CastleSiegeContext context)
    {
        if (context.CurrentState is not (CastleSiegeState.Ready or CastleSiegeState.Start)
            || context.Configuration.CastleSiegeMapDefinition?.Number != map.Definition.Number)
        {
            return;
        }

        await context.ExecutionLock.WaitAsync().ConfigureAwait(false);
        try
        {
            await context.NpcController.SynchronizePlayerAsync(player).ConfigureAwait(false);
        }
        finally
        {
            context.ExecutionLock.Release();
        }
    }

    private async ValueTask AdvanceExpiredStatesAsync(CastleSiegeContext context, DateTime utcNow, ILogger logger)
    {
        var maximumTransitions = context.Schedule.Count + 1;
        for (var transition = 0; context.StateEndTimeUtc <= utcNow && transition < maximumTransitions; transition++)
        {
            var nextState = context.Schedule.GetNextState(context.CurrentState);
            var nextPeriod = context.Schedule.CreatePeriod(nextState, context.StateEndTimeUtc);
            await this.ChangeStateAsync(context, nextPeriod, logger).ConfigureAwait(false);
        }
    }

    private async ValueTask ChangeStateAsync(CastleSiegeContext context, CastleSiegeStatePeriod period, ILogger logger)
    {
        var previousState = context.CurrentState;
        await this.OnExitStateAsync().ConfigureAwait(false);

        context.SetPeriod(period);
        this.ConfigureNotifications(context, period.StartUtc);
        await this.OnEnterStateAsync(context, false).ConfigureAwait(false);
        await context.SaveOwnerAsync().ConfigureAwait(false);
        await this.BroadcastStateUpdateAsync(context).ConfigureAwait(false);

        logger.LogInformation(
            "Castle Siege changed from state {previousState} to {state}; the state ends at {stateEndUtc}.",
            previousState,
            context.CurrentState,
            context.StateEndTimeUtc);

        if (period.State == CastleSiegeState.EndCycle)
        {
            var registrationPeriod = context.Schedule.CreateEarlyStartPeriod(CastleSiegeState.RegisterGuild, period.StartUtc);
            await this.ChangeStateAsync(context, registrationPeriod, logger).ConfigureAwait(false);
        }
    }

    private void ConfigureNotifications(CastleSiegeContext context, DateTime utcNow)
    {
        context.NextNotificationUtc = GetNotificationInterval(context.CurrentState) is { } interval
            ? GetNextInterval(context.StateStartTimeUtc, utcNow, interval)
            : DateTime.MaxValue;
        context.SentReadyCountdownMinutes.Clear();
    }

    private async ValueTask OnEnterStateAsync(CastleSiegeContext context, bool isStartup)
    {
        switch (context.CurrentState)
        {
            case CastleSiegeState.RegisterGuild:
                if (!isStartup)
                {
                    await context.LoadAsync().ConfigureAwait(false);
                }

                await this.SendStateNotificationAsync(context).ConfigureAwait(false);
                break;
            case CastleSiegeState.RegisterMark:
                await this.SendStateNotificationAsync(context).ConfigureAwait(false);
                break;
            case CastleSiegeState.Ready:
                await context.NpcController.PrepareAsync().ConfigureAwait(false);
                await context.NpcController.CloseGatesAsync().ConfigureAwait(false);
                break;
            case CastleSiegeState.Start:
                await context.NpcController.PrepareAsync().ConfigureAwait(false);
                await context.NpcController.CloseGatesAsync().ConfigureAwait(false);
                await context.NpcController.SpawnMachinesAsync().ConfigureAwait(false);
                break;
            case CastleSiegeState.End:
                await context.SaveNpcStatesAsync().ConfigureAwait(false);
                await context.NpcController.DespawnMachinesAsync().ConfigureAwait(false);
                break;
            case CastleSiegeState.EndCycle:
                await context.ClearRegistrationsAsync().ConfigureAwait(false);
                context.FinalGuildList.Clear();
                context.ParticipantTracking.Clear();
                await context.NpcController.DespawnAllAsync().ConfigureAwait(false);
                context.MiddleOwnerGuildId = null;
                context.CrownUser = null;
                Array.Clear(context.SwitchUsers);
                context.CrownAccumulatedTime = TimeSpan.Zero;
                context.IsCrownAvailable = false;
                break;
            default:
                // The remaining states do not require an entry action.
                break;
        }

        context.NextNpcSaveUtc = this._timeProvider.GetUtcNow().UtcDateTime + NpcSaveInterval;
    }

    private ValueTask OnExitStateAsync() => ValueTask.CompletedTask;

    private async ValueTask OnTickAsync(CastleSiegeContext context, DateTime utcNow)
    {
        if (context.NextNotificationUtc <= utcNow)
        {
            await this.SendStateNotificationAsync(context).ConfigureAwait(false);
            if (GetNotificationInterval(context.CurrentState) is { } interval)
            {
                context.NextNotificationUtc = GetNextInterval(context.StateStartTimeUtc, utcNow, interval);
            }
        }

        if (context.CurrentState == CastleSiegeState.Ready)
        {
            await this.SendReadyCountdownIfDueAsync(context, utcNow).ConfigureAwait(false);
        }

        if (!context.IsEventRunning && context.NextNpcSaveUtc <= utcNow)
        {
            await context.SaveNpcStatesAsync().ConfigureAwait(false);
            context.NextNpcSaveUtc = utcNow + NpcSaveInterval;
        }

        // Start-state Crown, switch, mini-map and participant ticks are implemented by their dedicated phases.
    }

    private async ValueTask SendReadyCountdownIfDueAsync(CastleSiegeContext context, DateTime utcNow)
    {
        var remaining = context.GetRemainingTime(utcNow);
        if (remaining <= TimeSpan.Zero)
        {
            return;
        }

        var remainingMinutes = (int)Math.Ceiling(remaining.TotalMinutes);
        if (remainingMinutes != 30 && remainingMinutes is not (>= 1 and <= 5))
        {
            return;
        }

        if (context.SentReadyCountdownMinutes.Add(remainingMinutes))
        {
            await context.GameContext.SendGlobalNotificationAsync($"Castle Siege starts in {remainingMinutes} minute(s).").ConfigureAwait(false);
        }
    }

    private ValueTask SendStateNotificationAsync(CastleSiegeContext context)
    {
        var message = context.CurrentState switch
        {
            CastleSiegeState.RegisterGuild => "Castle Siege guild registration is open.",
            CastleSiegeState.RegisterMark => "Castle Siege mark registration is open.",
            CastleSiegeState.Idle3 or CastleSiegeState.Notify => "Castle Siege preparations are in progress.",
            CastleSiegeState.Start => "Castle Siege is in progress.",
            _ => string.Empty,
        };

        return string.IsNullOrEmpty(message)
            ? ValueTask.CompletedTask
            : context.GameContext.SendGlobalNotificationAsync(message);
    }
}
