// <copyright file="TradeTest.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.Tests;

using Microsoft.Extensions.Logging.Abstractions;
using Moq;
using MUnique.OpenMU.DataModel;
using MUnique.OpenMU.DataModel.Configuration.Items;
using MUnique.OpenMU.DataModel.Entities;
using MUnique.OpenMU.GameLogic;
using MUnique.OpenMU.GameLogic.PlayerActions.Items;
using MUnique.OpenMU.GameLogic.PlayerActions.Trade;
using MUnique.OpenMU.GameLogic.Views.Trade;
using MUnique.OpenMU.Persistence;
using MUnique.OpenMU.Persistence.InMemory;
using MUnique.OpenMU.PlugIns;

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
    public async ValueTask TestTradeRequestAsync()
    {
        var player = this.CreateTrader(PlayerState.EnteredWorld); // The player which will send the trade request
        var tradePartner = this.CreateTrader(PlayerState.EnteredWorld); // The player which will receive the trade request

        var packetHandler = new TradeRequestAction();
        var success = await packetHandler.RequestTradeAsync(player, tradePartner).ConfigureAwait(false);
        Assert.AreEqual(true, success);
        Assert.AreSame(tradePartner, player.TradingPartner);
        Assert.AreSame(player, tradePartner.TradingPartner);
        Assert.AreEqual(PlayerState.TradeRequested, player.PlayerState.CurrentState);
        Assert.AreEqual(PlayerState.TradeRequested, tradePartner.PlayerState.CurrentState);
        Mock.Get(tradePartner.ViewPlugIns.GetPlugIn<IShowTradeRequestPlugIn>()).Verify(view => view!.ShowTradeRequestAsync(player), Times.Once);
    }

    /// <summary>
    /// Tests the trade response.
    /// </summary>
    [Test]
    public async ValueTask TestTradeResponseAsync()
    {
        var requester = this.CreateTrader(PlayerState.TradeRequested);
        var responder = this.CreateTrader(PlayerState.TradeRequested);
        requester.TradingPartner = responder;
        responder.TradingPartner = requester;
        var responseHandler = new TradeAcceptAction();
        await responseHandler.HandleTradeAcceptAsync(responder, true).ConfigureAwait(false);
        Assert.AreEqual(requester.PlayerState.CurrentState, PlayerState.TradeOpened);
        Assert.AreEqual(responder.PlayerState.CurrentState, PlayerState.TradeOpened);
        Mock.Get(requester.ViewPlugIns.GetPlugIn<IShowTradeRequestAnswerPlugIn>()).Verify(view => view!.ShowTradeRequestAnswerAsync(true), Times.Once);
    }

    /// <summary>
    /// Tests the cancellation of a trade.
    /// </summary>
    [Test]
    public async ValueTask TradeCancelTestAsync()
    {
        var trader1 = this.CreateTrader(PlayerState.TradeOpened);
        var trader2 = this.CreateTrader(PlayerState.TradeOpened);
        trader1.TradingPartner = trader2;
        trader2.TradingPartner = trader1;
        var cancelTrader = new TradeCancelAction();
        await cancelTrader.CancelTradeAsync(trader1).ConfigureAwait(false);
        Assert.AreEqual(PlayerState.EnteredWorld, trader1.PlayerState.CurrentState);
        Assert.AreEqual(PlayerState.EnteredWorld, trader2.PlayerState.CurrentState);

        Mock.Get(trader1.ViewPlugIns.GetPlugIn<ITradeFinishedPlugIn>()).Verify(view => view!.TradeFinishedAsync(TradeResult.Cancelled), Times.Once);
        Mock.Get(trader2.ViewPlugIns.GetPlugIn<ITradeFinishedPlugIn>()).Verify(view => view!.TradeFinishedAsync(TradeResult.Cancelled), Times.Once);
    }

    /// <summary>
    /// Tests the finishing of a trade.
    /// </summary>
    [Test]
    public async ValueTask TradeFinishTestAsync()
    {
        var trader1 = this.CreateTrader(PlayerState.TradeOpened);
        var trader2 = this.CreateTrader(PlayerState.TradeOpened);
        trader1.TradingPartner = trader2;
        trader2.TradingPartner = trader1;

        var gameContext = new Mock<IGameContext>();
        gameContext.Setup(c => c.PlugInManager).Returns(new PlugInManager(null, new NullLoggerFactory(), null, null));
        gameContext.Setup(c => c.PersistenceContextProvider).Returns(new InMemoryPersistenceContextProvider());

        Mock.Get(trader1).Setup(m => m.GameContext).Returns(gameContext.Object);
        Mock.Get(trader2).Setup(m => m.GameContext).Returns(gameContext.Object);

        var tradeButtonHandler = new TradeButtonAction();
        await tradeButtonHandler.TradeButtonChangedAsync(trader1, TradeButtonState.Unchecked).ConfigureAwait(false);
        Assert.AreEqual(trader1.PlayerState.CurrentState, PlayerState.TradeOpened);
        await tradeButtonHandler.TradeButtonChangedAsync(trader1, TradeButtonState.Checked).ConfigureAwait(false);
        Assert.AreEqual(trader1.PlayerState.CurrentState, PlayerState.TradeButtonPressed);
        Assert.AreEqual(trader2.PlayerState.CurrentState, PlayerState.TradeOpened);
        await tradeButtonHandler.TradeButtonChangedAsync(trader2, TradeButtonState.Checked).ConfigureAwait(false);
        Assert.AreEqual(trader1.PlayerState.CurrentState, PlayerState.EnteredWorld);
        Assert.AreEqual(trader2.PlayerState.CurrentState, PlayerState.EnteredWorld);
        Mock.Get(trader1.ViewPlugIns.GetPlugIn<ITradeFinishedPlugIn>()).Verify(view => view!.TradeFinishedAsync(TradeResult.Success), Times.Once);
        Mock.Get(trader2.ViewPlugIns.GetPlugIn<ITradeFinishedPlugIn>()).Verify(view => view!.TradeFinishedAsync(TradeResult.Success), Times.Once);
    }

    /// <summary>
    /// Tests a trade of items.
    /// </summary>
    [Test]
    public async ValueTask TradeItemsAsync()
    {
        var trader1 = await TestHelper.CreatePlayerAsync().ConfigureAwait(false);
        var trader2 = await TestHelper.CreatePlayerAsync().ConfigureAwait(false);
        var tradeRequestAction = new TradeRequestAction();
        var tradeResponseAction = new TradeAcceptAction();

        var item1 = this.GetItem();
        var item2 = this.GetItem();
        await trader1.Inventory!.AddItemAsync(20, item1).ConfigureAwait(false);
        await trader1.Inventory.AddItemAsync(21, item2).ConfigureAwait(false);
        await tradeRequestAction.RequestTradeAsync(trader1, trader2).ConfigureAwait(false);
        await tradeResponseAction.HandleTradeAcceptAsync(trader2, true).ConfigureAwait(false);
        var itemMoveAction = new MoveItemAction();
        await itemMoveAction.MoveItemAsync(trader1, 20, Storages.Inventory, 0, Storages.Trade).ConfigureAwait(false);
        await itemMoveAction.MoveItemAsync(trader1, 21, Storages.Inventory, 2, Storages.Trade).ConfigureAwait(false);
        Assert.That(trader1.TemporaryStorage!.Items.First(), Is.SameAs(item1));

        var tradeButtonHandler = new TradeButtonAction();
        await tradeButtonHandler.TradeButtonChangedAsync(trader1, TradeButtonState.Checked).ConfigureAwait(false);
        await tradeButtonHandler.TradeButtonChangedAsync(trader2, TradeButtonState.Checked).ConfigureAwait(false);
        Assert.That(trader1.Inventory.ItemStorage.Items, Is.Empty);
        Assert.That(trader2.Inventory!.ItemStorage.Items.First(), Is.SameAs(item1));
    }

    /// <summary>
    /// Tests a trade of items, when it failes due to missing inventory space.
    /// </summary>
    [Test]
    public async ValueTask TradeFailedItemsNotFitAsync()
    {
        var trader1 = await TestHelper.CreatePlayerAsync().ConfigureAwait(false);
        var trader2 = await TestHelper.CreatePlayerAsync().ConfigureAwait(false);
        var tradeRequestAction = new TradeRequestAction();
        var tradeResponseAction = new TradeAcceptAction();

        // Fill up inventory of the receiving player
        for (byte i = (byte)(InventoryConstants.LastEquippableItemSlotIndex + 1); i < 64 + InventoryConstants.LastEquippableItemSlotIndex; i++)
        {
            var item = this.GetItem();
            await trader2.Inventory!.AddItemAsync(i, item).ConfigureAwait(false);
        }

        // Create items which should be traded.
        var item1 = this.GetItem();
        var item2 = this.GetItem();
        await trader1.Inventory!.AddItemAsync(20, item1).ConfigureAwait(false);
        await trader1.Inventory.AddItemAsync(21, item2).ConfigureAwait(false);

        // Set up the trade
        await tradeRequestAction.RequestTradeAsync(trader1, trader2).ConfigureAwait(false);
        await tradeResponseAction.HandleTradeAcceptAsync(trader2, true).ConfigureAwait(false);
        var itemMoveAction = new MoveItemAction();
        await itemMoveAction.MoveItemAsync(trader1, 20, Storages.Inventory, 0, Storages.Trade).ConfigureAwait(false);
        await itemMoveAction.MoveItemAsync(trader1, 21, Storages.Inventory, 2, Storages.Trade).ConfigureAwait(false);
        Assert.That(trader1.TemporaryStorage!.Items.First(), Is.SameAs(item1));

        // Accept the trade on both ends
        var tradeButtonHandler = new TradeButtonAction();
        await tradeButtonHandler.TradeButtonChangedAsync(trader1, TradeButtonState.Checked).ConfigureAwait(false);
        await tradeButtonHandler.TradeButtonChangedAsync(trader2, TradeButtonState.Checked).ConfigureAwait(false);

        // Check result
        Assert.That(trader1.Inventory.ItemStorage.Items, Is.Not.Empty);
        Assert.That(trader1.Inventory!.ItemStorage.Items.First(), Is.SameAs(item1));
        Assert.That(trader1.Inventory!.ItemStorage.Items.Last(), Is.SameAs(item2));
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

        var contextMock = new Mock<IPlayerContext>();
        trader.Setup(t => t.PersistenceContext).Returns(contextMock.Object);
        return trader.Object;
    }

    //// TODO: Test fail scenarios
}