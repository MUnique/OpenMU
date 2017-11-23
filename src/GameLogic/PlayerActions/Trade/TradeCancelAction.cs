// <copyright file="TradeCancelAction.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.GameLogic.PlayerActions.Trade
{
    using log4net;

    /// <summary>
    /// Action to cancel the trade.
    /// </summary>
    public class TradeCancelAction : BaseTradeAction
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="TradeCancelAction"/> class.
        /// </summary>
        public TradeCancelAction()
        {
        }

        private static ILog Log { get; } = LogManager.GetLogger(typeof(TradeCancelAction));

        /// <summary>
        /// Cancels the trade.
        /// </summary>
        /// <param name="trader">The trader.</param>
        public new void CancelTrade(ITrader trader)
        {
            if (trader != null && trader.TradingPartner != null)
            {
                base.CancelTrade(trader.TradingPartner);
                base.CancelTrade(trader);
            }
            else
            {
                Log.Warn($"Trader {trader.Name} invoked CancelTrade, but it probably wasn't in a trade (TradingPartner = null).");
            }
        }
    }
}
