// <copyright file="TradeButtonAction.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.GameLogic.PlayerActions.Trade
{
    using MUnique.OpenMU.GameLogic.Views;

    /// <summary>
    /// Action to change the trade button state.
    /// </summary>
    public class TradeButtonAction : BaseTradeAction
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="TradeButtonAction"/> class.
        /// </summary>
        public TradeButtonAction()
        {
        }

        /// <summary>
        /// Tries to change the trade button change to the new <paramref name="tradeButtonState"/>.
        /// </summary>
        /// <param name="trader">The trader.</param>
        /// <param name="tradeButtonState">The new state of the trade button.</param>
        public void TradeButtonChanged(ITrader trader, TradeButtonState tradeButtonState)
        {
            var success = (tradeButtonState == TradeButtonState.Checked && trader.PlayerState.TryAdvanceTo(PlayerState.TradeButtonPressed))
                || (tradeButtonState == TradeButtonState.Unchecked && trader.PlayerState.TryAdvanceTo(PlayerState.TradeOpened));
            if (!success)
            {
                return;
            }

            if (trader.PlayerState.CurrentState == PlayerState.TradeButtonPressed
                && trader.TradingPartner != null
                && trader.TradingPartner.PlayerState.CurrentState == PlayerState.TradeButtonPressed)
            {
                if (!TryAddItemsOfTradingPartner(trader) || !TryAddItemsOfTradingPartner(trader.TradingPartner))
                {
                    this.SendMessage(trader, "Inventory is full.");
                    var tradingPartner = trader.TradingPartner as ITrader;
                    this.SendMessage(tradingPartner, "Inventory is full.");
                    this.CancelTrade(trader.TradingPartner);
                    this.CancelTrade(trader);
                }
                else
                {
                    this.FinishTrade(trader.TradingPartner);
                    this.FinishTrade(trader);
                }
            }
        }

        /// <summary>
        /// Finishes the trade.
        /// </summary>
        /// <param name="trader">The trader.</param>
        internal void FinishTrade(ITrader trader)
        {
            using (var context = trader.PlayerState.TryBeginAdvanceTo(PlayerState.EnteredWorld))
            {
                if (!context.Allowed)
                {
                    return;
                }

                trader.Money += trader.TradingPartner.TradingMoney;
                this.ResetTrade(trader);
            }
        }

        private static bool TryAddItemsOfTradingPartner(ITrader trader)
        {
            return trader.Inventory.TryTakeAll(trader.TradingPartner.TemporaryStorage);
        }
    }
}
