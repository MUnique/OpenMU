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
    public new void CancelTrade(ITrader trader)
    {
        using var loggerScope = (trader as Player)?.Logger.BeginScope(this.GetType());
        var tradingPartner = trader.TradingPartner;
        if (tradingPartner != null)
        {
            base.CancelTrade(tradingPartner);
            base.CancelTrade(trader!);
            trader!.ViewPlugIns.GetPlugIn<ITradeFinishedPlugIn>()?.TradeFinished(TradeResult.Cancelled);
            tradingPartner.ViewPlugIns.GetPlugIn<ITradeFinishedPlugIn>()?.TradeFinished(TradeResult.Cancelled);
        }
        else
        {
            (trader as Player)?.Logger.LogWarning($"Trader {trader?.Name} invoked CancelTrade, but it probably wasn't in a trade (TradingPartner = null).");
        }
    }
}