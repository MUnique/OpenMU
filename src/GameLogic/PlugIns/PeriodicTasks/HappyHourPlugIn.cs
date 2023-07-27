// <copyright file="HappyHourPlugIn.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.GameLogic.PlugIns.InvasionEvents;

using System.Runtime.InteropServices;
using MUnique.OpenMU.GameLogic.PlugIns.PeriodicTasks;
using MUnique.OpenMU.GameLogic.Views;
using MUnique.OpenMU.PlugIns;

/// <summary>
/// This plugin enables Happy Hour feature.
/// </summary>
[PlugIn(nameof(HappyHourPlugIn), "Handle Happy Hour event")]
[Guid("6542E452-9780-45B8-85AE-4036422E9A6E")]
public class HappyHourPlugIn : PeriodicTaskBasePlugIn<HappyHourConfiguration, HappyHourGameServerState>, ISupportDefaultCustomConfiguration, IObjectAddedToMapPlugIn
{
    /// <inheritdoc />
    public object CreateDefaultConfig() => HappyHourConfiguration.Default;

    /// <inheritdoc />
    [System.Diagnostics.CodeAnalysis.SuppressMessage("Usage", "VSTHRD100:Avoid async void methods", Justification = "Catching all Exceptions.")]
    public virtual async void ObjectAddedToMap(GameMap map, ILocateable addedObject)
    {
        try
        {
            if (addedObject is Player player)
            {
                var state = this.GetStateByGameContext(player.GameContext);
                var isEnabled = state.State != PeriodicTaskState.NotStarted;
                if (!isEnabled)
                {
                    return;
                }

                await this.TrySendStartMessageAsync(player).ConfigureAwait(false);
            }
        }
        catch
        {
            // must be catched because it's an async void method.
        }
    }

    /// <inheritdoc/>
    protected override ValueTask OnPrepareEventAsync(HappyHourGameServerState state)
    {
        return ValueTask.CompletedTask;
    }

    /// <inheritdoc/>
    protected override async ValueTask OnPreparedAsync(HappyHourGameServerState state)
    {
        await state.Context.ForEachPlayerAsync(this.TrySendStartMessageAsync).ConfigureAwait(false);
    }

    /// <inheritdoc/>
    protected override HappyHourGameServerState CreateState(IGameContext gameContext)
    {
        var expMult = this.Configuration?.ExperienceMultiplier ?? 1;

        return new HappyHourGameServerState(gameContext) { OldExperienceRate = gameContext.Configuration.ExperienceRate, NewExperienceRate = gameContext.Configuration.ExperienceRate * expMult };
    }

    /// <inheritdoc/>
    protected override ValueTask OnFinishedAsync(HappyHourGameServerState state)
    {
        state.Context.Configuration.ExperienceRate = state.OldExperienceRate;

        return ValueTask.CompletedTask;
    }

    /// <inheritdoc/>
    protected override ValueTask OnStartedAsync(HappyHourGameServerState state)
    {
        state.Context.Configuration.ExperienceRate = state.NewExperienceRate;

        return ValueTask.CompletedTask;
    }

    /// <summary>
    /// Returns true if the player stays on the map.
    /// </summary>
    /// <param name="player">The player.</param>
    protected bool IsPlayerOnMap(Player player)
    {
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

        var message = configuration.Message ?? "Happy Hour event has been started!";

        if (!this.IsPlayerOnMap(player))
        {
            return;
        }

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
