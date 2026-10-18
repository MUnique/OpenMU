// <copyright file="HappyHourPlugIn.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.GameLogic.PlugIns.PeriodicTasks;

using System.Collections.Concurrent;
using System.Runtime.InteropServices;
using MUnique.OpenMU.AttributeSystem;
using MUnique.OpenMU.GameLogic.Attributes;
using MUnique.OpenMU.GameLogic.Views;
using MUnique.OpenMU.PlugIns;

/// <summary>
/// This plugin enables Happy Hour feature.
/// </summary>
[PlugIn]
[Display(Name = nameof(PlugInResources.HappyHourPlugIn_Name), Description = nameof(PlugInResources.HappyHourPlugIn_Description), ResourceType = typeof(PlugInResources))]
[Guid("6542E452-9780-45B8-85AE-4036422E9A6E")]
public class HappyHourPlugIn : PeriodicTaskBasePlugIn<HappyHourConfiguration, PeriodicTaskGameServerState>, ISupportDefaultCustomConfiguration, IPlayerStateChangedPlugIn
{
    /// <summary>
    /// The happy hour extra multipliers by game context: one element per context, so
    /// concurrent runs in different contexts never share a value.
    /// </summary>
    private readonly ConcurrentDictionary<IGameContext, SimpleElement> _happyHourExtraMultipliers = new();

    /// <inheritdoc />
    public object CreateDefaultConfig() => HappyHourConfiguration.Default;

    /// <inheritdoc />
    public async ValueTask PlayerStateChangedAsync(Player player, State previousState, State currentState)
    {
        try
        {
            // The element is added on every server, so players joining mid-event get the
            // currently active multiplier right away. On servers without the event it stays
            // neutral (1.0); only the event run mutates its own server's element.
            var multiplier = this.GetMultiplierElement(player.GameContext);
            if (currentState.IsDisconnectedOrFinished())
            {
                player.Attributes?.RemoveElement(multiplier, Stats.ExperienceRate);
                player.Attributes?.RemoveElement(multiplier, Stats.MasterExperienceRate);
            }

            if (previousState != PlayerState.CharacterSelection || currentState != PlayerState.EnteredWorld)
            {
                return;
            }

            player.Attributes?.AddElement(multiplier, Stats.ExperienceRate);
            player.Attributes?.AddElement(multiplier, Stats.MasterExperienceRate);
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

    /// <summary>
    /// Gets the multiplier element of the specified game context, creating a neutral one on demand.
    /// </summary>
    /// <param name="gameContext">The game context.</param>
    internal SimpleElement GetMultiplierElement(IGameContext gameContext)
    {
        return this._happyHourExtraMultipliers.GetOrAdd(gameContext, _ => new SimpleElement(1.0f, AggregateType.Multiplicate));
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
        this.GetMultiplierElement(state.Context).Value = this.Configuration?.ExperienceMultiplier ?? 1;

        return ValueTask.CompletedTask;
    }

    /// <inheritdoc/>
    protected override ValueTask OnFinishedAsync(PeriodicTaskGameServerState state)
    {
        this.GetMultiplierElement(state.Context).Value = 1;

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

        if (!this.IsPlayerOnMap(player))
        {
            return;
        }

        try
        {
            var message = configuration.StartMessage.GetTranslation(player.Culture) is { Length: > 0 } translation ? translation : player.GetLocalizedMessage(nameof(PlayerMessage.HappyHourEventHasBeenStarted));
            await player.InvokeViewPlugInAsync<IShowMessagePlugIn>(p => p.ShowMessageAsync(message, Interfaces.MessageType.GoldenCenter)).ConfigureAwait(false);
        }
        catch (Exception ex)
        {
            player.Logger.LogDebug(ex, "Unexpected error sending start message.");
        }
    }
}
