// <copyright file="TradeCancelAction.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.GameLogic.PlayerActions.Trade;

using System.Runtime.ExceptionServices;

/// <summary>
/// Action to cancel the trade.
/// </summary>
public class TradeCancelAction : BaseTradeAction
{
    /// <summary>
    /// Cancels the trade.
    /// </summary>
    /// <param name="trader">The trader.</param>
    public ValueTask CancelTradeAsync(ITrader trader) => this.CancelTradeAsync(trader, trader.TradingPartner);

    /// <summary>Cancels only the captured pair, preserving replacement trades and their notifications.</summary>
    /// <param name="trader">The trader.</param>
    /// <param name="expectedPartner">The captured partner, or null for a request without a partner.</param>
    internal async ValueTask CancelTradeAsync(ITrader trader, ITrader? expectedPartner)
    {
        using var loggerScope = (trader as Player)?.Logger.BeginScope(this.GetType());
        var tradingPartner = expectedPartner;
        var traderNotification = ValueTask.CompletedTask;
        var partnerNotification = ValueTask.CompletedTask;
        Exception? error = null;
        if (tradingPartner != null)
        {
            await RunAndCaptureErrorAsync(() => this.CancelTradeAndStartNotificationAsync(tradingPartner, trader, n => partnerNotification = n)).ConfigureAwait(false);
            await RunAndCaptureErrorAsync(() => this.CancelTradeAndStartNotificationAsync(trader, tradingPartner, n => traderNotification = n)).ConfigureAwait(false);
        }
        else if (trader.PlayerState.CurrentState == PlayerState.TradeRequested
                 || trader.PlayerState.CurrentState == PlayerState.TradeOpened
                 || trader.PlayerState.CurrentState == PlayerState.TradeButtonPressed)
        {
            await RunAndCaptureErrorAsync(() => this.CancelTradeAndStartNotificationAsync(trader, null, n => traderNotification = n)).ConfigureAwait(false);
        }
        else
        {
            trader.Logger.LogWarning($"Trader {trader.Name} invoked CancelTrade, but it probably wasn't in a trade (TradingPartner = null).");
        }

        // Always finish both cleanups and observe each started notification, including after another operation failed.
        await RunAndCaptureErrorAsync(() => traderNotification).ConfigureAwait(false);
        await RunAndCaptureErrorAsync(() => partnerNotification).ConfigureAwait(false);
        if (error is not null)
        {
            ExceptionDispatchInfo.Capture(error).Throw();
        }

        async ValueTask RunAndCaptureErrorAsync(Func<ValueTask> operation)
        {
            try
            {
                await operation().ConfigureAwait(false);
            }
            catch (Exception ex)
            {
                if (error is null)
                {
                    error = ex;
                }
                else
                {
                    trader.Logger.LogError(ex, "Additional error completing trade cancellation for {trader}.", trader.Name);
                }
            }
        }
    }
}
