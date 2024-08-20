// <copyright file="MiniGameStartBasePlugIn.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.GameLogic.PlugIns.PeriodicTasks;

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
    /// <inheritdoc />
    public abstract MiniGameType Key { get; }

    /// <inheritdoc />
    public abstract object CreateDefaultConfig();

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
    protected override async ValueTask OnStartedAsync(TGameState state)
    {
        var miniGameDefinitions = state.Context.Configuration.MiniGameDefinitions
            .Where(d => d.Type == this.Key && d.MapCreationPolicy == MiniGameMapCreationPolicy.Shared);

        var enterDuration = TimeSpan.Zero;
        foreach (var miniGameDefinition in miniGameDefinitions)
        {
            // we're causing that the event context gets created.
            _ = await state.Context.GetMiniGameAsync(miniGameDefinition, null!).ConfigureAwait(false);
            enterDuration = miniGameDefinition.EnterDuration;
        }

        _ = this.SendOpenedNotificationsAsync(state, enterDuration);
    }

    /// <inheritdoc />
    protected override ValueTask OnFinishedAsync(TGameState state)
    {
        // nothing to do; The mini game will clean up itself.
        return ValueTask.CompletedTask;
    }

    private async Task SendOpenedNotificationsAsync(TGameState state, TimeSpan enterDuration)
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
            state.Context.LoggerFactory.CreateLogger<MiniGameStartBasePlugIn<TConfiguration, TGameState>>().LogError(ex, "Error when sending the notifications");
        }
    }
}
