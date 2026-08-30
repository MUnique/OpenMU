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
public class CastleSiegePlugIn : IPeriodicTaskPlugIn, IObjectAddedToMapPlugIn, IObjectRemovedFromMapPlugIn, IPlayerStateChangedPlugIn
{
    private static readonly TimeSpan EconomySaveInterval = TimeSpan.FromSeconds(30);
    private static readonly TimeSpan NpcSaveInterval = TimeSpan.FromMinutes(2);
    private static readonly TimeSpan ParticipantUpdateInterval = TimeSpan.FromSeconds(5);

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
            await this.SynchronizePlayerAsync(player, map, context).ConfigureAwait(false);
        }
    }

    /// <inheritdoc />
    public async ValueTask ObjectRemovedFromMapAsync(GameMap map, ILocateable removedObject)
    {
        if (removedObject is not Player player
            || this.GetContext(player.GameContext) is not { } context)
        {
            return;
        }

        context.UntrackPlayer(player);
        if (context.Configuration.CastleSiegeMapDefinition?.Number != map.Definition.Number)
        {
            return;
        }

        CastleSiegeParticipantTracker.StopTracking(
            context,
            player,
            this._timeProvider.GetUtcNow().UtcDateTime);
        await context.ClearPlayerJoinSideAsync(player).ConfigureAwait(false);
    }

    /// <inheritdoc />
    public async ValueTask PlayerStateChangedAsync(Player player, State previousState, State currentState)
    {
        if (currentState == PlayerState.EnteredWorld
            && player.CurrentMap is { } map
            && this.GetContext(player.GameContext) is { } context)
        {
            await this.SynchronizePlayerAsync(player, map, context).ConfigureAwait(false);
        }
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
    /// Persists accumulated Castle Siege economy changes when the batch interval is due.
    /// </summary>
    /// <param name="context">The Castle Siege context.</param>
    /// <param name="utcNow">The current UTC time.</param>
    internal static async ValueTask PersistEconomyIfDueAsync(CastleSiegeContext context, DateTime utcNow)
    {
        if (!context.IsEconomyPersistencePending || context.NextEconomySaveUtc > utcNow)
        {
            return;
        }

        await context.SaveOwnerAsync().ConfigureAwait(false);
        context.NextEconomySaveUtc = utcNow + EconomySaveInterval;
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
                context.NextEconomySaveUtc = utcNow + EconomySaveInterval;
                this.ConfigureNotifications(context, utcNow);
                await this.OnEnterStateAsync(context, true).ConfigureAwait(false);
                await CastleSiegeEconomyNotifier.BroadcastTaxRatesAsync(context).ConfigureAwait(false);
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

    private async ValueTask SynchronizePlayerAsync(
        Player player,
        GameMap map,
        CastleSiegeContext context)
    {
        await CastleSiegeEconomyNotifier.SynchronizePlayerAsync(context, player).ConfigureAwait(false);
        if (context.CurrentState is not (CastleSiegeState.Ready or CastleSiegeState.Start)
            || context.Configuration.CastleSiegeMapDefinition?.Number != map.Definition.Number)
        {
            return;
        }

        await context.NpcController.SynchronizePlayerAsync(player).ConfigureAwait(false);
        if (context.CurrentState is CastleSiegeState.Ready or CastleSiegeState.Start
            && context.Configuration.CastleSiegeMapDefinition?.Number == player.CurrentMap?.Definition.Number)
        {
            await context.SynchronizePlayerJoinSideAsync(player).ConfigureAwait(false);
            CastleSiegeParticipantTracker.StartTracking(
                context,
                player,
                this._timeProvider.GetUtcNow().UtcDateTime);
            if (context.CurrentState == CastleSiegeState.Start)
            {
                await CastleSiegeSwitchMechanics.SynchronizePlayerAsync(context, player).ConfigureAwait(false);
            }
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
        if (previousState == CastleSiegeState.Start)
        {
            await CastleSiegeParticipantTracker.TrackAsync(context, period.StartUtc).ConfigureAwait(false);
        }

        await this.OnExitStateAsync().ConfigureAwait(false);

        context.SetPeriod(period);
        this.ConfigureNotifications(context, period.StartUtc);
        await this.OnEnterStateAsync(context, false).ConfigureAwait(false);
        if (period.State != CastleSiegeState.End)
        {
            await context.SaveOwnerAsync().ConfigureAwait(false);
        }

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
            case CastleSiegeState.Idle1:
            case CastleSiegeState.Idle2:
            case CastleSiegeState.Idle3:
                break;
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
            case CastleSiegeState.Notify:
                await CastleSiegeGuildSelector.SelectGuildsAsync(context).ConfigureAwait(false);
                await this.SendStateNotificationAsync(context).ConfigureAwait(false);
                break;
            case CastleSiegeState.Ready:
                if (context.FinalGuildList.IsEmpty)
                {
                    await CastleSiegeGuildSelector.SelectGuildsAsync(context).ConfigureAwait(false);
                }

                await context.NpcController.PrepareAsync().ConfigureAwait(false);
                await context.NpcController.CloseGatesAsync().ConfigureAwait(false);
                await context.SetPlayerJoinSideAsync().ConfigureAwait(false);
                break;
            case CastleSiegeState.Start:
                if (context.FinalGuildList.IsEmpty)
                {
                    await CastleSiegeGuildSelector.SelectGuildsAsync(context).ConfigureAwait(false);
                }

                if (!isStartup)
                {
                    context.ParticipantTracking.Clear();
                }

                context.CrownUser = null;
                context.PreviousCrownUser = null;
                Array.Clear(context.SwitchUsers);
                context.CrownAccumulatedTime = TimeSpan.Zero;
                context.IsCrownAvailable = false;
                context.LastBroadcastSwitchInfos.Clear();
                context.LastBroadcastCrownAvailability = null;
                await context.NpcController.PrepareAsync().ConfigureAwait(false);
                await context.NpcController.CloseGatesAsync().ConfigureAwait(false);
                await context.NpcController.SpawnMachinesAsync().ConfigureAwait(false);
                await context.SetPlayerJoinSideAsync().ConfigureAwait(false);
                var utcNow = this._timeProvider.GetUtcNow().UtcDateTime;
                context.LastCrownUpdateUtc = utcNow;
                await CastleSiegeParticipantTracker.TrackAsync(context, utcNow).ConfigureAwait(false);
                context.NextParticipantUpdateUtc = utcNow + ParticipantUpdateInterval;
                break;
            case CastleSiegeState.End:
                await CastleSiegeCrownMechanics.CheckResultAsync(context).ConfigureAwait(false);
                if (!isStartup)
                {
                    await CastleSiegeParticipantTracker.AwardRewardsAsync(context).ConfigureAwait(false);
                    context.ParticipantTracking.Clear();
                }

                await context.SaveNpcStatesAsync().ConfigureAwait(false);
                await context.NpcController.DespawnMachinesAsync().ConfigureAwait(false);
                break;
            case CastleSiegeState.EndCycle:
                await context.ClearRegistrationsAsync().ConfigureAwait(false);
                context.FinalGuildList.Clear();
                await context.SaveFinalGuildListAsync().ConfigureAwait(false);
                await context.SetPlayerJoinSideAsync().ConfigureAwait(false);
                context.ClearPlayerJoinSides();
                context.ParticipantTracking.Clear();
                await context.NpcController.DespawnAllAsync().ConfigureAwait(false);
                context.MiddleOwnerGuildId = null;
                context.CrownUser = null;
                context.PreviousCrownUser = null;
                Array.Clear(context.SwitchUsers);
                context.CrownAccumulatedTime = TimeSpan.Zero;
                context.IsCrownAvailable = false;
                context.LastCrownUpdateUtc = DateTime.MinValue;
                context.LastBroadcastSwitchInfos.Clear();
                context.LastBroadcastCrownAvailability = null;
                context.NextParticipantUpdateUtc = DateTime.MaxValue;
                break;
        }

        context.NextNpcSaveUtc = this._timeProvider.GetUtcNow().UtcDateTime + NpcSaveInterval;
    }

    private ValueTask OnExitStateAsync() => ValueTask.CompletedTask;

    private async ValueTask OnTickAsync(CastleSiegeContext context, DateTime utcNow)
    {
        await PersistEconomyIfDueAsync(context, utcNow).ConfigureAwait(false);

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

        if (context.CurrentState == CastleSiegeState.Start
            && context.NextParticipantUpdateUtc <= utcNow)
        {
            await context.SetPlayerJoinSideAsync().ConfigureAwait(false);
            await CastleSiegeParticipantTracker
                .TrackAsync(context, utcNow)
                .ConfigureAwait(false);
            context.NextParticipantUpdateUtc = GetNextInterval(
                context.StateStartTimeUtc,
                utcNow,
                ParticipantUpdateInterval);
        }

        if (context.CurrentState == CastleSiegeState.Start)
        {
            await CastleSiegeSwitchMechanics.SendSwitchInfoAsync(context).ConfigureAwait(false);
            await CastleSiegeCrownMechanics.CheckMiddleWinnerAsync(context, utcNow).ConfigureAwait(false);
        }

        if (!context.IsEventRunning && context.NextNpcSaveUtc <= utcNow)
        {
            await context.SaveNpcStatesAsync().ConfigureAwait(false);
            context.NextNpcSaveUtc = utcNow + NpcSaveInterval;
        }
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
