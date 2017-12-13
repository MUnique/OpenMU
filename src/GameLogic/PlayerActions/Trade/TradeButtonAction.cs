// <copyright file="TradeButtonAction.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.GameLogic.PlayerActions.Trade
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using log4net;
    using MUnique.OpenMU.DataModel.Entities;
    using MUnique.OpenMU.GameLogic.Views;
    using MUnique.OpenMU.Persistence;

    /// <summary>
    /// Action to change the trade button state.
    /// </summary>
    public class TradeButtonAction : BaseTradeAction
    {
        private static readonly ILog Log = LogManager.GetLogger(typeof(TradeButtonAction));

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

            var tradingPartner = trader.TradingPartner;

            if (trader.PlayerState.CurrentState == PlayerState.TradeButtonPressed
                && tradingPartner != null
                && tradingPartner.PlayerState.CurrentState == PlayerState.TradeButtonPressed)
            {
                TradeResult result = this.InternalFinishTrade(trader, tradingPartner);
                if (result != TradeResult.Success)
                {
                    this.CancelTrade(tradingPartner);
                    this.CancelTrade(trader);
                    Log.Warn($"Cancelled the trade because of unfinished state. trader: {trader.Name}, partner:{trader.TradingPartner.Name}");
                }

                trader.TradeView.TradeFinished(result);
                tradingPartner.TradeView.TradeFinished(result);
            }
            else
            {
                (trader.TradingPartner as Player)?.PlayerView.TradeView.ChangeTradeButtonState(TradeButtonState.Checked);
            }
        }

        private static bool TryAddItemsOfTradingPartner(ITrader trader)
        {
            if (trader.TradingPartner.TemporaryStorage.Items.Any())
            {
                return trader.Inventory.TryTakeAll(trader.TradingPartner.TemporaryStorage);
            }

            return true;
        }

        private TradeResult InternalFinishTrade(ITrader trader, ITrader tradingPartner)
        {
            using (var context = trader.PlayerState.TryBeginAdvanceTo(PlayerState.EnteredWorld))
            using (var partnerContext = tradingPartner.PlayerState.TryBeginAdvanceTo(PlayerState.EnteredWorld))
            using (var itemContext = this.gameContext.RepositoryManager.CreateNewAccountContext(this.gameContext.Configuration))
            using (this.gameContext.RepositoryManager.UseContext(itemContext))
            {
                if (!context.Allowed || !partnerContext.Allowed)
                {
                    context.Allowed = false;
                    partnerContext.Allowed = false;
                    Log.Error($"Unexpected player states. {trader.Name}:{trader.PlayerState}, {tradingPartner.Name}:{tradingPartner.PlayerState}");
                    return TradeResult.Cancelled;
                }

                var traderItems = trader.TemporaryStorage.Items.ToList();
                var tradePartnerItems = tradingPartner.TemporaryStorage.Items.ToList();
                this.AttachItemsToPersistenceContext(traderItems, itemContext);
                this.AttachItemsToPersistenceContext(tradePartnerItems, itemContext);

                if (!TryAddItemsOfTradingPartner(trader) || !TryAddItemsOfTradingPartner(tradingPartner))
                {
                    this.SendMessage(trader, "Inventory is full.");
                    this.SendMessage(tradingPartner, "Inventory is full.");
                    return TradeResult.FailedByFullInventory;
                }

                try
                {
                    this.DetachItemsFromPersistenceContext(traderItems, trader.PersistenceContext);
                    this.DetachItemsFromPersistenceContext(tradePartnerItems, trader.TradingPartner.PersistenceContext);
                    itemContext.SaveChanges();
                    trader.Money += trader.TradingPartner.TradingMoney;
                    trader.TradingPartner.Money += trader.TradingMoney;
                    (trader.TradingPartner as Player)?.PlayerView.TradeView.ChangeTradeButtonState(TradeButtonState.Checked);
                    this.ResetTradeState(trader.TradingPartner);
                    this.ResetTradeState(trader);
                    return TradeResult.Success;
                }
                catch (Exception exception)
                {
                    this.SendMessage(trader, "An unexpected error occured during closing the trade.");
                    this.SendMessage(tradingPartner, "An unexpected error occured during closing the trade.");
                    context.Allowed = false;
                    partnerContext.Allowed = false;
                    Log.Error($"An unexpected error occured during closing the trade. trader: {trader.Name}, partner:{tradingPartner.Name}", exception);
                    return TradeResult.Cancelled;
                }
            }
        }

        private void AttachItemsToPersistenceContext(IEnumerable<Item> items, IContext itemContext)
        {
            foreach (var item in items)
            {
                itemContext.Attach(item);
            }
        }

        private void DetachItemsFromPersistenceContext(IEnumerable<Item> items, IContext itemContext)
        {
            foreach (var item in items)
            {
                itemContext.Detach(item);
            }
        }
    }
}
