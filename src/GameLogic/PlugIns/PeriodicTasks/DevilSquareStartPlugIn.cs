// <copyright file="DevilSquareStartPlugIn.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.GameLogic.PlugIns.PeriodicTasks;

using System.Runtime.InteropServices;
using MUnique.OpenMU.GameLogic.MiniGames;
using MUnique.OpenMU.PlugIns;

/// <summary>
/// This plugin enables the start of the devil square.
/// </summary>
[PlugIn(nameof(DevilSquareStartPlugIn), "Devil Square event")]
[Guid("61C61A58-211E-4D6A-9EA1-D25E0C4A47C5")]
public sealed class DevilSquareStartPlugIn : PeriodicTaskBasePlugIn<DevilSquareStartConfiguration, DevilSquareGameServerState>, ISupportDefaultCustomConfiguration, IPeriodicMiniGameStartPlugIn
{
    /// <summary>
    /// Gets the key under which the strategy is getting registered.
    /// </summary>
    public MiniGameType Key => MiniGameType.DevilSquare;

    /// <inheritdoc />
    public object CreateDefaultConfig()
    {
        return DevilSquareStartConfiguration.Default;
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
    protected override ValueTask OnPrepareEventAsync(DevilSquareGameServerState state)
    {
        // nothing to do
        return ValueTask.CompletedTask;
    }

    /// <inheritdoc />
    protected override DevilSquareGameServerState CreateState(IGameContext gameContext)
    {
        return new DevilSquareGameServerState(gameContext);
    }

    /// <inheritdoc />
    protected override ValueTask OnPreparedAsync(DevilSquareGameServerState state)
    {
        // We keep it simple and don't send a message here.
        return ValueTask.CompletedTask;
    }

    /// <inheritdoc />
    protected override async ValueTask OnStartedAsync(DevilSquareGameServerState state)
    {
        var devilSquareDefinitions = state.Context.Configuration.MiniGameDefinitions
            .Where(d => d is { Type: MiniGameType.DevilSquare, MapCreationPolicy: MiniGameMapCreationPolicy.Shared });
        var enterDuration = TimeSpan.Zero;
        foreach (var devilSquareDefinition in devilSquareDefinitions)
        {
            // If the devil square map does not have the Shared creation policy, we skip the time plugin.
            if (devilSquareDefinition.MapCreationPolicy != MiniGameMapCreationPolicy.Shared)
            {
                continue;
            }

            // we're causing that the event context gets created.
            _ = await state.Context.GetMiniGameAsync(devilSquareDefinition, null!).ConfigureAwait(false);
            enterDuration = devilSquareDefinition.EnterDuration;
        }

        _ = this.SendOpenedNotificationsAsync(state, enterDuration);
    }

    /// <inheritdoc />
    protected override ValueTask OnFinishedAsync(DevilSquareGameServerState state)
    {
        // nothing to do; The mini game will clean up itself.
        return ValueTask.CompletedTask;
    }

    private async Task SendOpenedNotificationsAsync(DevilSquareGameServerState state, TimeSpan enterDuration)
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
            state.Context.LoggerFactory.CreateLogger<DevilSquareStartPlugIn>().LogError(ex, "Error when sending the notifications");
        }
    }
}
