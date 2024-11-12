// <copyright file="TradeCancelAction.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.GameLogic.PlayerActions.Trade;

using MUnique.OpenMU.GameLogic.Views.Trade;

/// <summary>
/// Action to cancel the trade.
/// </summary>
public class TradeCancelAction : BaseTradeAction
{
    /// <summary>
    /// Cancels the trade.
    /// </summary>
    /// <param name="trader">The trader.</param>
    public async ValueTask CancelTradeAsync(ITrader trader)
    {
        using var loggerScope = (trader as Player)?.Logger.BeginScope(this.GetType());
        var tradingPartner = trader.TradingPartner;
        if (tradingPartner != null)
        {
            await base.CancelTradeAsync(tradingPartner).ConfigureAwait(false);
            await base.CancelTradeAsync(trader).ConfigureAwait(false);
            await trader.InvokeViewPlugInAsync<ITradeFinishedPlugIn>(p => p.TradeFinishedAsync(TradeResult.Cancelled)).ConfigureAwait(false);
            await tradingPartner.InvokeViewPlugInAsync<ITradeFinishedPlugIn>(p => p.TradeFinishedAsync(TradeResult.Cancelled)).ConfigureAwait(false);
        }
        else
        {
            trader.Logger.LogWarning($"Trader {trader?.Name} invoked CancelTrade, but it probably wasn't in a trade (TradingPartner = null).");
        }
    }
}