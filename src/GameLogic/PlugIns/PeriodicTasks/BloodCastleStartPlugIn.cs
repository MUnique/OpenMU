// <copyright file="BloodCastleStartPlugIn.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.GameLogic.PlugIns.PeriodicTasks;

using System.Runtime.InteropServices;
using MUnique.OpenMU.GameLogic.MiniGames;
using MUnique.OpenMU.PlugIns;

/// <summary>
/// This plugin enables the start of the blood castle.
/// </summary>
[PlugIn(nameof(BloodCastleStartPlugIn), "Blood Castle event")]
[Guid("95E68C14-AD87-4B3C-AF46-45B8F1C3BC2A")]
public sealed class BloodCastleStartPlugIn : PeriodicTaskBasePlugIn<BloodCastleStartConfiguration, BloodCastleGameServerState>, ISupportDefaultCustomConfiguration, IPeriodicMiniGameStartPlugIn
{
    /// <summary>
    /// Gets the key under which the strategy is getting registered.
    /// </summary>
    public MiniGameType Key => MiniGameType.BloodCastle;

    /// <inheritdoc />
    public object CreateDefaultConfig()
    {
        return BloodCastleStartConfiguration.Default;
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
    protected override ValueTask OnPrepareEventAsync(BloodCastleGameServerState state)
    {
        // nothing to do
        return ValueTask.CompletedTask;
    }

    /// <inheritdoc />
    protected override BloodCastleGameServerState CreateState(IGameContext gameContext)
    {
        return new BloodCastleGameServerState(gameContext);
    }

    /// <inheritdoc />
    protected override ValueTask OnPreparedAsync(BloodCastleGameServerState state)
    {
        // We keep it simple and don't send a message here.
        return ValueTask.CompletedTask;
    }

    /// <inheritdoc />
    protected override async ValueTask OnStartedAsync(BloodCastleGameServerState state)
    {
        var bloodCastleDefinitions = state.Context.Configuration.MiniGameDefinitions
            .Where(d => d is { Type: MiniGameType.BloodCastle, MapCreationPolicy: MiniGameMapCreationPolicy.Shared });
        var enterDuration = TimeSpan.Zero;
        foreach (var bloodCastleDefinition in bloodCastleDefinitions)
        {
            // If the blood castle map does not have the Shared creation policy, we skip the time plugin.
            if (bloodCastleDefinition.MapCreationPolicy != MiniGameMapCreationPolicy.Shared)
            {
                continue;
            }

            // we're causing that the event context gets created.
            _ = await state.Context.GetMiniGameAsync(bloodCastleDefinition, null!).ConfigureAwait(false);
            enterDuration = bloodCastleDefinition.EnterDuration;
        }

        _ = this.SendOpenedNotificationsAsync(state, enterDuration);
    }

    /// <inheritdoc />
    protected override ValueTask OnFinishedAsync(BloodCastleGameServerState state)
    {
        // nothing to do; The mini game will clean up itself.
        return ValueTask.CompletedTask;
    }

    private async Task SendOpenedNotificationsAsync(BloodCastleGameServerState state, TimeSpan enterDuration)
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
            state.Context.LoggerFactory.CreateLogger<BloodCastleStartPlugIn>().LogError(ex, "Error when sending the notifications");
        }
    }
}
