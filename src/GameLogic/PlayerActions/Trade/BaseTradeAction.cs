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
    /// <param name="expectedPartner">The peer which must still own this half, or null to leave peer ownership unchecked for existing rollback callers.</param>
    /// <param name="startResult">Optional owned result start after reset and before this half publishes its available state. Completions must be observed outside the scope.</param>
    /// <returns>Whether this half was cleaned up.</returns>
    protected async ValueTask<bool> CancelTradeAsync(ITrader trader, bool checkState = true, ITrader? expectedPartner = null, Action? startResult = null)
    {
        CancelledTrades.Add(1);
        await using (var context = await trader.PlayerState.TryBeginAdvanceToAsync(PlayerState.EnteredWorld).ConfigureAwait(false))
        {
            // Only a trade is left here: a cancel must never revive a dead or map-changing life.
            context.Allowed &= trader.PlayerState.CurrentState == PlayerState.TradeRequested
                || trader.PlayerState.CurrentState == PlayerState.TradeOpened
                || trader.PlayerState.CurrentState == PlayerState.TradeButtonPressed;
            if ((checkState && !context.Allowed)
                || (expectedPartner is not null && !ReferenceEquals(trader.TradingPartner, expectedPartner)))
            {
                context.Allowed = false;
                return false;
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
            }

            // Still inside the state exclusion: the reset sees the backup which marks the trade's own items.
            this.ResetTradeState(trader);
            startResult?.Invoke();
        }

        return true;
    }

    /// <summary>
    /// Resets the trade.
    /// </summary>
    /// <param name="trader">The trader.</param>
    protected void ResetTradeState(ITrader trader)
    {
        trader.TradingPartner = null;
        if (trader.BackupInventory is not null)
        {
            // Without a backup, the trade never owned the temporary storage: it holds items which another
            // window (e.g. the chaos machine) left there, and they are returned instead of cleared.
            trader.TemporaryStorage!.Clear();
            trader.BackupInventory = null;
        }
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
}
