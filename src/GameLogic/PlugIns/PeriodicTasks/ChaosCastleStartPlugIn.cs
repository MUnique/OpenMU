// <copyright file="ChaosCastleStartPlugIn.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.GameLogic.PlugIns.PeriodicTasks;

using System.Runtime.InteropServices;
using MUnique.OpenMU.GameLogic.MiniGames;
using MUnique.OpenMU.PlugIns;

/// <summary>
/// This plugin enables the start of the chaos castle.
/// </summary>
[PlugIn(nameof(ChaosCastleStartPlugIn), "Chaos Castle event")]
[Guid("3AD96A70-ED24-4979-80B8-169E461E548F")]
public sealed class ChaosCastleStartPlugIn : PeriodicTaskBasePlugIn<ChaosCastleStartConfiguration, ChaosCastleGameServerState>, ISupportDefaultCustomConfiguration, IPeriodicMiniGameStartPlugIn
{
    /// <summary>
    /// Gets the key under which the strategy is getting registered.
    /// </summary>
    public MiniGameType Key => MiniGameType.ChaosCastle;

    /// <inheritdoc />
    public object CreateDefaultConfig()
    {
        return ChaosCastleStartConfiguration.Default;
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

        if (state.State == PeriodicTaskState.Started
            && await state.Context.GetMiniGameAsync(miniGameDefinition, null!).ConfigureAwait(false) is { State: MiniGameState.Open })
        {
            return TimeSpan.Zero;
        }

        var timeNow = new TimeOnly(DateTime.UtcNow.TimeOfDay.Ticks);
        var nextRun = this.Configuration?.Timetable.Where(time => time > timeNow).Order().FirstOrDefault();
        return nextRun - timeNow;
    }

    /// <inheritdoc />
    public async ValueTask<MiniGameContext?> GetMiniGameContextAsync(IGameContext gameContext, MiniGameDefinition miniGameDefinition)
    {
        var state = this.GetStateByGameContext(gameContext);
        if (state.State == PeriodicTaskState.Started)
        {
            return await state.Context.GetMiniGameAsync(miniGameDefinition, null!).ConfigureAwait(false);
        }

        return null;
    }

    /// <inheritdoc />
    protected override ValueTask OnPrepareEventAsync(ChaosCastleGameServerState state)
    {
        // nothing to do
        return ValueTask.CompletedTask;
    }

    /// <inheritdoc />
    protected override ChaosCastleGameServerState CreateState(IGameContext gameContext)
    {
        return new ChaosCastleGameServerState(gameContext);
    }

    /// <inheritdoc />
    protected override ValueTask OnPreparedAsync(ChaosCastleGameServerState state)
    {
        // We keep it simple and don't send a message here.
        return ValueTask.CompletedTask;
    }

    /// <inheritdoc />
    protected override async ValueTask OnStartedAsync(ChaosCastleGameServerState state)
    {
        var chaosCastleDefinitions = state.Context.Configuration.MiniGameDefinitions
            .Where(d => d is { Type: MiniGameType.ChaosCastle, MapCreationPolicy: MiniGameMapCreationPolicy.Shared });
        var enterDuration = TimeSpan.Zero;
        foreach (var chaosCastleDefinition in chaosCastleDefinitions)
        {
            // we're causing that the event context gets created.
            _ = await state.Context.GetMiniGameAsync(chaosCastleDefinition, null!).ConfigureAwait(false);
            enterDuration = chaosCastleDefinition.EnterDuration;
        }

        _ = this.SendOpenedNotificationsAsync(state, enterDuration);
    }

    /// <inheritdoc />
    protected override ValueTask OnFinishedAsync(ChaosCastleGameServerState state)
    {
        // nothing to do; The mini game will clean up itself.
        return ValueTask.CompletedTask;
    }

    private async Task SendOpenedNotificationsAsync(ChaosCastleGameServerState state, TimeSpan enterDuration)
    {
        try
        {
            if (enterDuration <= TimeSpan.Zero)
            {
                return;
            }

            for (var remainingMinutes = (int)enterDuration.TotalMinutes; remainingMinutes > 0; remainingMinutes--)
            {
                if (this.Configuration?.EntranceOpenedMessage is { } openMessage)
                {
                    await state.Context.SendGlobalNotificationAsync(string.Format(openMessage, remainingMinutes)).ConfigureAwait(false);
                }

                await Task.Delay(TimeSpan.FromMinutes(1)).ConfigureAwait(false);
            }

            var remainingSeconds = TimeSpan.FromSeconds(enterDuration.Seconds);
            if (remainingSeconds > TimeSpan.Zero)
            {
                await Task.Delay(remainingSeconds).ConfigureAwait(false);
            }

            if (this.Configuration?.EntranceClosedMessage is { } closedMessage)
            {
                await state.Context.SendGlobalNotificationAsync(closedMessage).ConfigureAwait(false);
            }
        }
        catch (Exception ex)
        {
            state.Context.LoggerFactory.CreateLogger<ChaosCastleStartPlugIn>().LogError(ex, "Error when sending the notifications");
        }
    }
}
