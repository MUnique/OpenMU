// <copyright file="TradeButtonAction.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.GameLogic.PlayerActions.Trade
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using MUnique.OpenMU.DataModel.Entities;
    using MUnique.OpenMU.GameLogic.Views;
    using MUnique.OpenMU.Persistence;

    /// <summary>
    /// Action to change the trade button state.
    /// </summary>
    public class TradeButtonAction : BaseTradeAction
    {
        private readonly IGameContext gameContext;

        /// <summary>
        /// Initializes a new instance of the <see cref="TradeButtonAction" /> class.
        /// </summary>
        /// <param name="gameContext">The game context.</param>
        public TradeButtonAction(IGameContext gameContext)
        {
            this.gameContext = gameContext;
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
                bool tradeFinished = false;
                using (var context = trader.PlayerState.TryBeginAdvanceTo(PlayerState.EnteredWorld))
                using (var partnerContext = trader.PlayerState.TryBeginAdvanceTo(PlayerState.EnteredWorld))
                using (var itemContext = this.gameContext.RepositoryManager.CreateNewAccountContext(this.gameContext.Configuration))
                using (this.gameContext.RepositoryManager.UseContext(itemContext))
                {
                    if (!context.Allowed || !partnerContext.Allowed)
                    {
                        context.Allowed = false;
                        partnerContext.Allowed = false;
                        return;
                    }

                    var traderItems = trader.TemporaryStorage.Items.ToList();
                    var tradePartnerItems = trader.TradingPartner.TemporaryStorage.Items.ToList();
                    this.AttachItemsToTradeContext(traderItems, itemContext);
                    this.AttachItemsToTradeContext(tradePartnerItems, itemContext);

                    if (!TryAddItemsOfTradingPartner(trader) || !TryAddItemsOfTradingPartner(trader.TradingPartner))
                    {
                        this.SendMessage(trader, "Inventory is full.");
                        this.SendMessage(trader.TradingPartner, "Inventory is full.");
                    }
                    else
                    {
                        try
                        {
                            this.DetachItemsFromTradeContext(traderItems, trader.PersistenceContext);
                            this.DetachItemsFromTradeContext(tradePartnerItems, trader.TradingPartner.PersistenceContext);
                            itemContext.SaveChanges();
                            trader.Money += trader.TradingPartner.TradingMoney;
                            trader.TradingPartner.Money += trader.TradingMoney;
                            this.ResetTradeState(trader.TradingPartner);
                            this.ResetTradeState(trader);
                            tradeFinished = true;
                        }
                        catch (Exception exception)
                        {
                            // TODO: Log exception
                            this.SendMessage(trader, "An unexpected error occured during closing the trade.");
                            this.SendMessage(trader.TradingPartner, "An unexpected error occured during closing the trade.");
                            context.Allowed = false;
                            context.Allowed = false;
                        }
                    }
                }

                if (!tradeFinished)
                {
                    this.CancelTrade(trader.TradingPartner);
                    this.CancelTrade(trader);
                }
            }
        }

        private void AttachItemsToTradeContext(IEnumerable<Item> items, IContext itemContext)
        {
            foreach (var item in items)
            {
                itemContext.Attach(item);
            }
        }

        private void DetachItemsFromTradeContext(IEnumerable<Item> items, IContext itemContext)
        {
            foreach (var item in items)
            {
                itemContext.Detach(item);
            }
        }

        private static bool TryAddItemsOfTradingPartner(ITrader trader)
        {
            return trader.Inventory.TryTakeAll(trader.TradingPartner.TemporaryStorage);
        }
    }
}
