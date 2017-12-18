// <copyright file="TradeTest.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.Tests
{
    using System.Collections.Generic;
    using System.Linq;
    using MUnique.OpenMU.DataModel.Configuration;
    using MUnique.OpenMU.DataModel.Configuration.Items;
    using MUnique.OpenMU.DataModel.Entities;
    using MUnique.OpenMU.GameLogic;
    using MUnique.OpenMU.GameLogic.PlayerActions.Items;
    using MUnique.OpenMU.GameLogic.PlayerActions.Trade;
    using MUnique.OpenMU.GameLogic.Views;
    using MUnique.OpenMU.Persistence;
    using NUnit.Framework;
    using Rhino.Mocks;

    /// <summary>
    /// Tests for the trade actions.
    /// </summary>
    [TestFixture]
    public class TradeTest
    {
        /// <summary>
        /// Tests the trade request.
        /// </summary>
        [Test]
        public void TestTradeRequest()
        {
            var player = this.CreateTrader(PlayerState.EnteredWorld); // The player which will send the trade request
            var tradePartner = this.CreateTrader(PlayerState.EnteredWorld); // The player which will receive the trade request

            tradePartner.TradeView.Expect(view => view.ShowTradeRequest(player)).Repeat.Once();
            var packetHandler = new TradeRequestAction();
            var success = packetHandler.RequestTrade(player, tradePartner);
            Assert.AreEqual(true, success);
            Assert.AreSame(tradePartner, player.TradingPartner);
            Assert.AreSame(player, tradePartner.TradingPartner);
            Assert.AreEqual(PlayerState.TradeRequested, player.PlayerState.CurrentState);
            Assert.AreEqual(PlayerState.TradeRequested, tradePartner.PlayerState.CurrentState);
            tradePartner.TradeView.VerifyAllExpectations();
        }

        /// <summary>
        /// Tests the trade response.
        /// </summary>
        [Test]
        public void TestTradeResponse()
        {
            var requester = this.CreateTrader(PlayerState.TradeRequested);
            var responder = this.CreateTrader(PlayerState.TradeRequested);
            requester.TradingPartner = responder;
            responder.TradingPartner = requester;
            requester.TradeView.Expect(view => view.ShowTradeRequestAnswer(true));
            var responseHandler = new TradeAcceptAction();
            responseHandler.HandleTradeAccept(responder, true);
            Assert.AreEqual(requester.PlayerState.CurrentState, PlayerState.TradeOpened);
            Assert.AreEqual(responder.PlayerState.CurrentState, PlayerState.TradeOpened);
            requester.TradeView.VerifyAllExpectations();
        }

        /// <summary>
        /// Tests the cancellation of a trade.
        /// </summary>
        [Test]
        public void TradeCancelTest()
        {
            var trader1 = this.CreateTrader(PlayerState.TradeOpened);
            var trader2 = this.CreateTrader(PlayerState.TradeOpened);
            trader1.TradingPartner = trader2;
            trader2.TradingPartner = trader1;
            trader1.TradeView.Expect(view => view.TradeFinished(TradeResult.Cancelled));
            trader2.TradeView.Expect(view => view.TradeFinished(TradeResult.Cancelled));
            var cancelTrader = new TradeCancelAction();
            cancelTrader.CancelTrade(trader1);
            Assert.AreEqual(PlayerState.EnteredWorld, trader1.PlayerState.CurrentState);
            Assert.AreEqual(PlayerState.EnteredWorld, trader2.PlayerState.CurrentState);
            trader1.TradeView.VerifyAllExpectations();
            trader2.TradeView.VerifyAllExpectations();
        }

        /// <summary>
        /// Tests the finishing of a trade.
        /// </summary>
        [Test]
        public void TradeFinishTest()
        {
            var trader1 = this.CreateTrader(PlayerState.TradeOpened);
            var trader2 = this.CreateTrader(PlayerState.TradeOpened);
            trader1.TradingPartner = trader2;
            trader2.TradingPartner = trader1;
            trader1.TradeView.Expect(view => view.TradeFinished(TradeResult.Success));
            trader2.TradeView.Expect(view => view.TradeFinished(TradeResult.Success));

            var gameContext = MockRepository.GenerateStub<IGameContext>();
            gameContext.Stub(c => c.RepositoryManager).Return(new TestRepositoryManager());
            var tradeButtonHandler = new TradeButtonAction(gameContext);
            tradeButtonHandler.TradeButtonChanged(trader1, TradeButtonState.Unchecked);
            Assert.AreEqual(trader1.PlayerState.CurrentState, PlayerState.TradeOpened);
            tradeButtonHandler.TradeButtonChanged(trader1, TradeButtonState.Checked);
            Assert.AreEqual(trader1.PlayerState.CurrentState, PlayerState.TradeButtonPressed);
            Assert.AreEqual(trader2.PlayerState.CurrentState, PlayerState.TradeOpened);
            tradeButtonHandler.TradeButtonChanged(trader2, TradeButtonState.Checked);
            Assert.AreEqual(trader1.PlayerState.CurrentState, PlayerState.EnteredWorld);
            Assert.AreEqual(trader2.PlayerState.CurrentState, PlayerState.EnteredWorld);
            trader1.TradeView.VerifyAllExpectations();
            trader2.TradeView.VerifyAllExpectations();
        }

        /// <summary>
        /// Tests a trade of items.
        /// </summary>
        [Test]
        public void TradeItems()
        {
            var trader1 = TestHelper.GetPlayer();
            var trader2 = TestHelper.GetPlayer();
            var tradeRequestAction = new TradeRequestAction();
            var tradeResponseAction = new TradeAcceptAction();

            var item1 = this.GetItem();
            var item2 = this.GetItem();
            trader1.Inventory.AddItem(20, item1);
            trader1.Inventory.AddItem(21, item2);
            tradeRequestAction.RequestTrade(trader1, trader2);
            tradeResponseAction.HandleTradeAccept(trader2, true);
            var itemMoveAction = new MoveItemAction();
            itemMoveAction.MoveItem(trader1, 20, Storages.Inventory, 0, Storages.Trade);
            itemMoveAction.MoveItem(trader1, 21, Storages.Inventory, 2, Storages.Trade);
            Assert.That(trader1.TemporaryStorage.Items.First(), Is.SameAs(item1));

            var gameContext = MockRepository.GenerateStub<IGameContext>();
            gameContext.Stub(c => c.RepositoryManager).Return(new TestRepositoryManager());
            var tradeButtonHandler = new TradeButtonAction(gameContext);
            tradeButtonHandler.TradeButtonChanged(trader1, TradeButtonState.Checked);
            tradeButtonHandler.TradeButtonChanged(trader2, TradeButtonState.Checked);
            Assert.That(trader1.Inventory.ItemStorage.Items, Is.Empty);
            Assert.That(trader2.Inventory.ItemStorage.Items.First(), Is.SameAs(item1));
        }

        private Item GetItem()
        {
            var item = MockRepository.GenerateStub<Item>();
            item.Definition = new ItemDefinition { Width = 1, Height = 1 };
            item.Stub(i => i.ItemOptions).Return(new List<ItemOptionLink>());
            return item;
        }

        private ITrader CreateTrader(State playerState)
        {
            var trader = MockRepository.GenerateStub<ITrader>();
            trader.Stub(t => t.PlayerState).Return(new StateMachine(playerState));
            var inventory = MockRepository.GenerateStub<IStorage>();
            trader.Stub(t => t.Inventory).Return(inventory);
            inventory.Stub(i => i.ItemStorage).Return(MockRepository.GenerateStub<ItemStorage>());
            inventory.ItemStorage.Stub(i => i.Items).Return(new List<Item>());
            trader.BackupInventory = new BackupItemStorage(inventory.ItemStorage) { Items = new List<Item>() };
            trader.Stub(t => t.TemporaryStorage).Return(MockRepository.GenerateStub<IStorage>());
            trader.TemporaryStorage.Stub(t => t.Items).Return(new List<Item>());
            trader.Stub(t => t.TradeView).Return(MockRepository.GenerateMock<ITradeView>());
            return trader;
        }

        //// TODO: Test fail scenarios

        private class TestRepositoryManager : BaseRepositoryManager
        {
            public override IContext CreateNewAccountContext(GameConfiguration gameConfiguration)
            {
                return MockRepository.GenerateStub<IContext>();
            }
        }
    }
}
