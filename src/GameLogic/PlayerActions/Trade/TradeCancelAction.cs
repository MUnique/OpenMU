// <copyright file="TradeCancelAction.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.GameLogic.PlayerActions.Trade
{
    using Interfaces;
    using log4net;
    using MUnique.OpenMU.GameLogic.Views;

    /// <summary>
    /// Action to cancel the trade.
    /// </summary>
    public class TradeCancelAction : BaseTradeAction
    {
        private static ILog Log { get; } = LogManager.GetLogger(typeof(TradeCancelAction));

        /// <summary>
        /// Cancels the trade.
        /// </summary>
        /// <param name="trader">The trader.</param>
        public new void CancelTrade(ITrader trader)
        {
            var tradingPartner = trader?.TradingPartner;
            if (tradingPartner != null)
            {
                base.CancelTrade(tradingPartner);
                base.CancelTrade(trader);
                trader.TradeView.TradeFinished(TradeResult.Cancelled);
                tradingPartner.TradeView.TradeFinished(TradeResult.Cancelled);
            }
            else
            {
                Log.Warn($"Trader {trader?.Name} invoked CancelTrade, but it probably wasn't in a trade (TradingPartner = null).");
            }
        }
    }
}
