// <copyright file="HappyHourPlugIn.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.GameLogic.PlugIns.PeriodicTasks;

using System.Runtime.InteropServices;
using MUnique.OpenMU.AttributeSystem;
using MUnique.OpenMU.GameLogic.Attributes;
using MUnique.OpenMU.GameLogic.Views;
using MUnique.OpenMU.PlugIns;

/// <summary>
/// This plugin enables Happy Hour feature.
/// </summary>
[PlugIn(nameof(HappyHourPlugIn), "Handle Happy Hour event")]
[Guid("6542E452-9780-45B8-85AE-4036422E9A6E")]
public class HappyHourPlugIn : PeriodicTaskBasePlugIn<HappyHourConfiguration, PeriodicTaskGameServerState>, ISupportDefaultCustomConfiguration, IPlayerStateChangedPlugIn
{
    /// <summary>
    /// The happy hour extra multiplier which gets added to the players <see cref="Stats.ExperienceRate"/>
    /// and <see cref="Stats.MasterExperienceRate"/>.
    /// </summary>
    private readonly SimpleElement _happyHourExtraMultiplier = new(1.0f, AggregateType.Multiplicate);

    /// <inheritdoc />
    public object CreateDefaultConfig() => HappyHourConfiguration.Default;

    /// <inheritdoc />
    public async ValueTask PlayerStateChangedAsync(Player player, State previousState, State currentState)
    {
        try
        {
            if (currentState.IsDisconnectedOrFinished())
            {
                player.Attributes?.RemoveElement(this._happyHourExtraMultiplier, Stats.ExperienceRate);
                player.Attributes?.RemoveElement(this._happyHourExtraMultiplier, Stats.MasterExperienceRate);
            }

            if (previousState != PlayerState.CharacterSelection || currentState != PlayerState.EnteredWorld)
            {
                return;
            }

            player.Attributes?.AddElement(this._happyHourExtraMultiplier, Stats.ExperienceRate);
            player.Attributes?.AddElement(this._happyHourExtraMultiplier, Stats.MasterExperienceRate);
            var state = this.GetStateByGameContext(player.GameContext);
            var isEnabled = state.State != PeriodicTaskState.NotStarted;
            if (!isEnabled)
            {
                return;
            }

            await this.TrySendStartMessageAsync(player).ConfigureAwait(false);
        }
        catch (Exception ex)
        {
            player.Logger.LogDebug(ex, "Unexpected error handling player state change.");
        }
    }

    /// <inheritdoc/>
    protected override PeriodicTaskGameServerState CreateState(IGameContext gameContext)
    {
        return new PeriodicTaskGameServerState(gameContext);
    }

    /// <inheritdoc/>
    protected override ValueTask OnPrepareEventAsync(PeriodicTaskGameServerState state)
    {
        return ValueTask.CompletedTask;
    }

    /// <inheritdoc/>
    protected override async ValueTask OnPreparedAsync(PeriodicTaskGameServerState state)
    {
        await state.Context.ForEachPlayerAsync(this.TrySendStartMessageAsync).ConfigureAwait(false);
    }

    /// <inheritdoc/>
    protected override ValueTask OnStartedAsync(PeriodicTaskGameServerState state)
    {
        this._happyHourExtraMultiplier.Value = this.Configuration?.ExperienceMultiplier ?? 1;

        return ValueTask.CompletedTask;
    }

    /// <inheritdoc/>
    protected override ValueTask OnFinishedAsync(PeriodicTaskGameServerState state)
    {
        this._happyHourExtraMultiplier.Value = 1;

        return ValueTask.CompletedTask;
    }

    /// <summary>
    /// Returns true if the player stays on the map.
    /// </summary>
    /// <param name="player">The player.</param>
    protected bool IsPlayerOnMap(Player player)
    {
        return player.CurrentMap is not null
               && !player.PlayerState.CurrentState.IsDisconnectedOrFinished();
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
