// <copyright file="TradeTest.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.Tests
{
    using System.Collections.Generic;
    using System.Linq;
    using Microsoft.Extensions.Logging.Abstractions;
    using Moq;
    using MUnique.OpenMU.DataModel.Configuration.Items;
    using MUnique.OpenMU.DataModel.Entities;
    using MUnique.OpenMU.GameLogic;
    using MUnique.OpenMU.GameLogic.PlayerActions.Items;
    using MUnique.OpenMU.GameLogic.PlayerActions.Trade;
    using MUnique.OpenMU.GameLogic.Views.Trade;
    using MUnique.OpenMU.Persistence.InMemory;
    using MUnique.OpenMU.PlugIns;
    using NUnit.Framework;

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

            var packetHandler = new TradeRequestAction();
            var success = packetHandler.RequestTrade(player, tradePartner);
            Assert.AreEqual(true, success);
            Assert.AreSame(tradePartner, player.TradingPartner);
            Assert.AreSame(player, tradePartner.TradingPartner);
            Assert.AreEqual(PlayerState.TradeRequested, player.PlayerState.CurrentState);
            Assert.AreEqual(PlayerState.TradeRequested, tradePartner.PlayerState.CurrentState);
            Mock.Get(tradePartner.ViewPlugIns.GetPlugIn<IShowTradeRequestPlugIn>()).Verify(view => view!.ShowTradeRequest(player), Times.Once);
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
            var responseHandler = new TradeAcceptAction();
            responseHandler.HandleTradeAccept(responder, true);
            Assert.AreEqual(requester.PlayerState.CurrentState, PlayerState.TradeOpened);
            Assert.AreEqual(responder.PlayerState.CurrentState, PlayerState.TradeOpened);
            Mock.Get(requester.ViewPlugIns.GetPlugIn<IShowTradeRequestAnswerPlugIn>()).Verify(view => view!.ShowTradeRequestAnswer(true), Times.Once);
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
            var cancelTrader = new TradeCancelAction();
            cancelTrader.CancelTrade(trader1);
            Assert.AreEqual(PlayerState.EnteredWorld, trader1.PlayerState.CurrentState);
            Assert.AreEqual(PlayerState.EnteredWorld, trader2.PlayerState.CurrentState);

            Mock.Get(trader1.ViewPlugIns.GetPlugIn<ITradeFinishedPlugIn>()).Verify(view => view!.TradeFinished(TradeResult.Cancelled), Times.Once);
            Mock.Get(trader2.ViewPlugIns.GetPlugIn<ITradeFinishedPlugIn>()).Verify(view => view!.TradeFinished(TradeResult.Cancelled), Times.Once);
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

            var gameContext = new Mock<IGameContext>();
            gameContext.Setup(c => c.PlugInManager).Returns(new PlugInManager(null, new NullLoggerFactory(), null));
            gameContext.Setup(c => c.PersistenceContextProvider).Returns(new InMemoryPersistenceContextProvider());

            Mock.Get(trader1).Setup(m => m.GameContext).Returns(gameContext.Object);
            Mock.Get(trader2).Setup(m => m.GameContext).Returns(gameContext.Object);

            var tradeButtonHandler = new TradeButtonAction();
            tradeButtonHandler.TradeButtonChanged(trader1, TradeButtonState.Unchecked);
            Assert.AreEqual(trader1.PlayerState.CurrentState, PlayerState.TradeOpened);
            tradeButtonHandler.TradeButtonChanged(trader1, TradeButtonState.Checked);
            Assert.AreEqual(trader1.PlayerState.CurrentState, PlayerState.TradeButtonPressed);
            Assert.AreEqual(trader2.PlayerState.CurrentState, PlayerState.TradeOpened);
            tradeButtonHandler.TradeButtonChanged(trader2, TradeButtonState.Checked);
            Assert.AreEqual(trader1.PlayerState.CurrentState, PlayerState.EnteredWorld);
            Assert.AreEqual(trader2.PlayerState.CurrentState, PlayerState.EnteredWorld);
            Mock.Get(trader1.ViewPlugIns.GetPlugIn<ITradeFinishedPlugIn>()).Verify(view => view!.TradeFinished(TradeResult.Success), Times.Once);
            Mock.Get(trader2.ViewPlugIns.GetPlugIn<ITradeFinishedPlugIn>()).Verify(view => view!.TradeFinished(TradeResult.Success), Times.Once);
        }

        /// <summary>
        /// Tests a trade of items.
        /// </summary>
        [Test]
        public void TradeItems()
        {
            var trader1 = TestHelper.CreatePlayer();
            var trader2 = TestHelper.CreatePlayer();
            var tradeRequestAction = new TradeRequestAction();
            var tradeResponseAction = new TradeAcceptAction();

            var item1 = this.GetItem();
            var item2 = this.GetItem();
            trader1.Inventory!.AddItem(20, item1);
            trader1.Inventory.AddItem(21, item2);
            tradeRequestAction.RequestTrade(trader1, trader2);
            tradeResponseAction.HandleTradeAccept(trader2, true);
            var itemMoveAction = new MoveItemAction();
            itemMoveAction.MoveItem(trader1, 20, Storages.Inventory, 0, Storages.Trade);
            itemMoveAction.MoveItem(trader1, 21, Storages.Inventory, 2, Storages.Trade);
            Assert.That(trader1.TemporaryStorage!.Items.First(), Is.SameAs(item1));

            var tradeButtonHandler = new TradeButtonAction();
            tradeButtonHandler.TradeButtonChanged(trader1, TradeButtonState.Checked);
            tradeButtonHandler.TradeButtonChanged(trader2, TradeButtonState.Checked);
            Assert.That(trader1.Inventory.ItemStorage.Items, Is.Empty);
            Assert.That(trader2.Inventory!.ItemStorage.Items.First(), Is.SameAs(item1));
        }

        private Item GetItem()
        {
            var item = new Mock<Item>();
            item.SetupAllProperties();
            item.Object.Definition = new ItemDefinition { Width = 1, Height = 1 };
            item.Setup(i => i.ItemOptions).Returns(new List<ItemOptionLink>());
            return item.Object;
        }

        private ITrader CreateTrader(State playerState)
        {
            var trader = new Mock<ITrader>();
            trader.SetupAllProperties();
            trader.Setup(t => t.PlayerState).Returns(new StateMachine(playerState));
            var inventory = new Mock<IInventoryStorage>();
            var itemStorage = new Mock<ItemStorage>();
            itemStorage.Setup(i => i.Items).Returns(new List<Item>());
            inventory.Setup(i => i.ItemStorage).Returns(itemStorage.Object);
            trader.Setup(t => t.Inventory).Returns(inventory.Object);
            trader.Object.BackupInventory = new BackupItemStorage(itemStorage.Object) { Items = new List<Item>() };
            var temporaryStorage = new Mock<IStorage>();
            temporaryStorage.Setup(t => t.Items).Returns(new List<Item>());
            trader.Setup(t => t.TemporaryStorage).Returns(temporaryStorage.Object);
            trader.Setup(t => t.ViewPlugIns).Returns(new MockViewPlugInContainer());
            return trader.Object;
        }

        //// TODO: Test fail scenarios
    }
}
