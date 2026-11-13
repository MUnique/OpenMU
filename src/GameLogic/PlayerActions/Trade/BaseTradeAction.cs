// <copyright file="BaseTradeAction.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.GameLogic.PlayerActions.Trade;

using System.Diagnostics.Metrics;
using MUnique.OpenMU.GameLogic.Views;
using MUnique.OpenMU.GameLogic.Views.Trade;
using MUnique.OpenMU.Interfaces;

/// <summary>
/// Action to cancel a trade.
/// </summary>
public class BaseTradeAction
{
    private static readonly Meter TradeMeter = new(MeterName);

    /// <summary>
    /// Gets the name of the meter of this class.
    /// </summary>
    internal static string MeterName => typeof(BaseTradeAction).Namespace ?? nameof(BaseTradeAction);

    /// <summary>
    /// Gets the counter for finished trades.
    /// </summary>
    protected static Counter<int> FinishedTrades { get; } = TradeMeter.CreateCounter<int>("FinishedTradesCount");

    /// <summary>
    /// Gets the counter for cancelled trades.
    /// </summary>
    private static Counter<int> CancelledTrades { get; } = TradeMeter.CreateCounter<int>("CancelledTradesCount");

    /// <summary>
    /// Cancels the matching trade and starts its notification before publishing the available state.
    /// </summary>
    /// <param name="trader">The trader.</param>
    /// <param name="expectedPartner">The partner whose trade may be cancelled.</param>
    /// <param name="onNotificationStarted">Captures the notification for observation after both cancellation scopes end; must not block.</param>
    internal ValueTask CancelTradeAndStartNotificationAsync(ITrader trader, ITrader? expectedPartner, Action<ValueTask> onNotificationStarted)
        => this.CancelTradeCoreAsync(trader, true, expectedPartner, onNotificationStarted);

    /// <summary>
    /// Cancels the trade.
    /// </summary>
    /// <param name="trader">The trader.</param>
    /// <param name="checkState">If set to <c>true</c>, the player state is checked. Otherwise, it's ignored.</param>
    /// <param name="expectedPartner">The partner whose trade may be cancelled, or <c>null</c> to omit the identity check.</param>
    protected ValueTask CancelTradeAsync(ITrader trader, bool checkState = true, ITrader? expectedPartner = null)
        => this.CancelTradeCoreAsync(trader, checkState, expectedPartner, null);

    /// <summary>
    /// Resets the trade.
    /// </summary>
    /// <param name="trader">The trader.</param>
    protected void ResetTradeState(ITrader trader)
    {
        trader.TradingPartner = null;
        trader.BackupInventory = null;
        trader.TemporaryStorage!.Clear();
    }

    /// <summary>
    /// Sends the message to the trader.
    /// </summary>
    /// <param name="trader">The trader.</param>
    /// <param name="messageKey">The key of the message in <see cref="PlayerMessage"/>.</param>
    protected async ValueTask SendMessageAsync(ITrader trader, string messageKey)
    {
        if (trader is Player player)
        {
            try
            {
                await player.ShowLocalizedBlueMessageAsync(messageKey).ConfigureAwait(false);
            }
            catch (Exception ex)
            {
                trader.Logger.LogError(ex, "Error sending a message");
            }
        }
    }

    private async ValueTask CancelTradeCoreAsync(ITrader trader, bool checkState, ITrader? expectedPartner, Action<ValueTask>? onNotificationStarted)
    {
        CancelledTrades.Add(1);
        await using (var context = await trader.PlayerState.TryBeginAdvanceToAsync(PlayerState.EnteredWorld).ConfigureAwait(false))
        {
            context.Allowed &= trader.PlayerState.CurrentState == PlayerState.TradeRequested
                || trader.PlayerState.CurrentState == PlayerState.TradeOpened
                || trader.PlayerState.CurrentState == PlayerState.TradeButtonPressed;
            if ((checkState && !context.Allowed)
                || (expectedPartner is not null && !ReferenceEquals(trader.TradingPartner, expectedPartner)))
            {
                context.Allowed = false;
                return;
            }

            trader.TradingMoney = 0;
            if (trader.BackupInventory != null)
            {
                trader.Inventory!.Clear();
                trader.BackupInventory.RestoreItemStates();
                foreach (var item in trader.BackupInventory.Items)
                {
                    try
                    {
                        if (!await trader.Inventory.AddItemAsync(item.ItemSlot, item).ConfigureAwait(false)
                            && !await trader.Inventory.AddItemAsync(item).ConfigureAwait(false))
                        {
                            trader.Logger.LogError(
                                "Failed to restore item {item} during trade cancellation for {trader}.",
                                item,
                                trader.Name);
                        }
                    }
                    catch (Exception ex)
                    {
                        trader.Logger.LogError(
                            ex,
                            "Error restoring item {item} during trade cancellation for {trader}.",
                            item,
                            trader.Name);
                    }
                }

                trader.Inventory.ItemStorage.Money = trader.BackupInventory.Money;
                trader.BackupInventory = null;
            }

            this.ResetTradeState(trader);
            if (onNotificationStarted is not null)
            {
                // The concrete remote view queues its result first, before a replacement can publish its window.
                // Only capture here: awaiting a reentrant view while holding state exclusion would deadlock.
                onNotificationStarted(trader.InvokeViewPlugInAsync<ITradeFinishedPlugIn>(p => p.TradeFinishedAsync(TradeResult.Cancelled)));
            }
        }
    }
}
