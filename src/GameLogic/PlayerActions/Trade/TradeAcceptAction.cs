// <copyright file="TradeAcceptAction.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.GameLogic.PlayerActions.Trade
{
    /// <summary>
    /// Action to accept the trade.
    /// </summary>
    public class TradeAcceptAction : BaseTradeAction
    {
        /// <summary>
        /// Handles the trade request answer.
        /// </summary>
        /// <param name="tradeAccepter">The trader which answers the trade request.</param>
        /// <param name="accept">Indicates if the trader accepted the trade request.</param>
        public void HandleTradeAccept(ITrader tradeAccepter, bool accept)
        {
            var tradePartner = tradeAccepter.TradingPartner;
            if (accept && tradePartner != null)
            {
                if (!tradeAccepter.PlayerState.TryAdvanceTo(PlayerState.TradeOpened) || !tradePartner.PlayerState.TryAdvanceTo(PlayerState.TradeOpened))
                {
                    ////Something bad happend here...
                    this.CancelTrade(tradeAccepter);
                    this.CancelTrade(tradePartner);

                    return;
                }

                this.OpenTrade(tradeAccepter);
                this.OpenTrade(tradePartner);
            }
            else
            {
                if (tradePartner != null)
                {
                    this.CancelTrade(tradePartner);
                }

                this.CancelTrade(tradeAccepter);
            }
        }

        /// <summary>
        /// Opens the trade.
        /// </summary>
        /// <param name="trader">Trader for which the trade should be opened.</param>
        internal void OpenTrade(ITrader trader)
        {
            trader.BackupInventory = new BackupItemStorage(trader.Inventory.ItemStorage);
            trader.TradingMoney = 0;

            if (trader is Player player)
            {
                player.PlayerView.TradeView.TradeOpened();
            }
        }
    }
}
