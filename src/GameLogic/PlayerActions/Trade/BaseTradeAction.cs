// <copyright file="BaseTradeAction.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.GameLogic.PlayerActions.Trade;

using System.Diagnostics.Metrics;
using MUnique.OpenMU.GameLogic.Views;
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
    /// Cancels the trade.
    /// </summary>
    /// <param name="trader">The trader.</param>
    /// <param name="checkState">If set to <c>true</c>, the player state is checked. Otherwise, it's ignored.</param>
    protected async ValueTask CancelTradeAsync(ITrader trader, bool checkState = true)
    {
        CancelledTrades.Add(1);
        await using (var context = await trader.PlayerState.TryBeginAdvanceToAsync(PlayerState.EnteredWorld).ConfigureAwait(false))
        {
            if (checkState && !context.Allowed)
            {
                return;
            }

            trader.TradingMoney = 0;
            if (trader.BackupInventory != null)
            {
                trader.Inventory!.Clear();
                trader.BackupInventory.RestoreItemStates();
                foreach (var item in trader.BackupInventory.Items)
                {
                    await trader.Inventory.AddItemAsync(item.ItemSlot, item).ConfigureAwait(false);
                }

                trader.Inventory.ItemStorage.Money = trader.BackupInventory.Money;
                trader.BackupInventory = null;
            }
        }

        this.ResetTradeState(trader);
    }

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
    /// <param name="message">The message.</param>
    protected async ValueTask SendMessageAsync(ITrader trader, string message)
    {
        if (trader is IWorldObserver observer)
        {
            try
            {
                await observer.InvokeViewPlugInAsync<IShowMessagePlugIn>(p => p.ShowMessageAsync(message, MessageType.BlueNormal)).ConfigureAwait(false);
            }
            catch (Exception ex)
            {
                trader.Logger.LogError(ex, "Error sending a message");
            }
        }
    }
}