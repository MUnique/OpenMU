// <copyright file="TradeTest.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.Tests;

using System.Threading;
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
        Mock.Get(tradePartner.ViewPlugIns.GetPlugIn<IShowTradeRequestPlugIn>()!).Verify(view => view!.ShowTradeRequestAsync(player), Times.Once);
    }

    /// <summary>
    /// A pending trade request keeps its pair: further requests which involve one of its players are refused and
    /// don't replace a partner; the third player stays untouched. After the cancel, a new request succeeds.
    /// </summary>
    [Test]
    public async Task PendingRequestKeepsItsPairAsync()
    {
        var a = await PlayerTestHelper.CreatePlayerAsync().ConfigureAwait(false);
        try
        {
            var b = await PlayerTestHelper.CreatePlayerAsync(a.GameContext).ConfigureAwait(false);
            try
            {
                var c = await PlayerTestHelper.CreatePlayerAsync(a.GameContext).ConfigureAwait(false);
                try
                {
                    Assert.That(await RequestAsync(a, b).ConfigureAwait(false), Is.True);
                    var busyRequester = await RequestAsync(a, c).ConfigureAwait(false);
                    var busyRequestedAsRequester = await RequestAsync(b, c).ConfigureAwait(false);
                    var busyRequesterAsRequested = await RequestAsync(c, a).ConfigureAwait(false);
                    var busyRequested = await RequestAsync(c, b).ConfigureAwait(false);
                    Assert.Multiple(() =>
                    {
                        Assert.That(busyRequester, Is.False, "busy requester");
                        Assert.That(busyRequestedAsRequester, Is.False, "busy requested player as requester");
                        Assert.That(busyRequesterAsRequested, Is.False, "busy requester as requested player");
                        Assert.That(busyRequested, Is.False, "busy requested player");
                    });
                    AssertPair(a, b);
                    AssertFree(c);

                    await a.RunPersistenceExclusiveAsync(() => new TradeCancelAction().CancelTradeAsync(a)).ConfigureAwait(false);
                    AssertFree(a);
                    AssertFree(b);
                    Assert.That(await RequestAsync(a, c).ConfigureAwait(false), Is.True);
                    AssertPair(a, c);
                    AssertFree(b);
                }
                finally
                {
                    await c.DisposeAsync().ConfigureAwait(false);
                }
            }
            finally
            {
                await b.DisposeAsync().ConfigureAwait(false);
            }
        }
        finally
        {
            await a.DisposeAsync().ConfigureAwait(false);
        }
    }

    /// <summary>
    /// Two requests which involve the same player (C to A and A to B) are started through the packet funnels of their
    /// requesters at the same time. Exactly one pair results, and the refused request is rolled back. The tasks aren't
    /// forced to interleave in a specific order; each attempt alternates which one starts first.
    /// </summary>
    [Test]
    public async Task CrossingRequestsResultInOnePairAsync()
    {
        var a = await PlayerTestHelper.CreatePlayerAsync().ConfigureAwait(false);
        try
        {
            var b = await PlayerTestHelper.CreatePlayerAsync(a.GameContext).ConfigureAwait(false);
            try
            {
                var c = await PlayerTestHelper.CreatePlayerAsync(a.GameContext).ConfigureAwait(false);
                try
                {
                    for (var attempt = 0; attempt < 8; attempt++)
                    {
                        Task<bool> fromC;
                        Task<bool> fromA;
                        if (attempt % 2 == 0)
                        {
                            fromC = RequestAsync(c, a);
                            fromA = RequestAsync(a, b);
                        }
                        else
                        {
                            fromA = RequestAsync(a, b);
                            fromC = RequestAsync(c, a);
                        }

                        var results = await Task.WhenAll(fromC, fromA).WaitAsync(TimeSpan.FromSeconds(5)).ConfigureAwait(false);
                        Assert.That(results.Count(r => r), Is.EqualTo(1), $"attempt {attempt}: C to A {results[0]}, A to B {results[1]}");
                        if (results[0])
                        {
                            AssertPair(c, a);
                            AssertFree(b);
                        }
                        else
                        {
                            AssertPair(a, b);
                            AssertFree(c);
                        }

                        await a.RunPersistenceExclusiveAsync(() => new TradeCancelAction().CancelTradeAsync(a)).ConfigureAwait(false);
                        AssertFree(a);
                        AssertFree(b);
                        AssertFree(c);
                    }
                }
                finally
                {
                    await c.DisposeAsync().ConfigureAwait(false);
                }
            }
            finally
            {
                await b.DisposeAsync().ConfigureAwait(false);
            }
        }
        finally
        {
            await a.DisposeAsync().ConfigureAwait(false);
        }
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
        Mock.Get(requester.ViewPlugIns.GetPlugIn<IShowTradeRequestAnswerPlugIn>()!).Verify(view => view!.ShowTradeRequestAnswerAsync(true), Times.Once);
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

        Mock.Get(trader1.ViewPlugIns.GetPlugIn<ITradeFinishedPlugIn>()!).Verify(view => view!.TradeFinishedAsync(TradeResult.Cancelled), Times.Once);
        Mock.Get(trader2.ViewPlugIns.GetPlugIn<ITradeFinishedPlugIn>()!).Verify(view => view!.TradeFinishedAsync(TradeResult.Cancelled), Times.Once);
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
        Mock.Get(trader1.ViewPlugIns.GetPlugIn<ITradeFinishedPlugIn>()!).Verify(view => view!.TradeFinishedAsync(TradeResult.Success), Times.Once);
        Mock.Get(trader2.ViewPlugIns.GetPlugIn<ITradeFinishedPlugIn>()!).Verify(view => view!.TradeFinishedAsync(TradeResult.Success), Times.Once);
    }

    /// <summary>
    /// Tests a trade of items.
    /// </summary>
    [Test]
    public async ValueTask TradeItemsAsync()
    {
        var trader1 = await PlayerTestHelper.CreatePlayerAsync().ConfigureAwait(false);
        var trader2 = await PlayerTestHelper.CreatePlayerAsync().ConfigureAwait(false);
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
        var trader1 = await PlayerTestHelper.CreatePlayerAsync().ConfigureAwait(false);
        var trader2 = await PlayerTestHelper.CreatePlayerAsync().ConfigureAwait(false);
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

    /// <summary>
    /// Tests that a trade cancellation does not abort restoring all valid items when one backup item is invalid.
    /// </summary>
    [Test]
    public async ValueTask TradeCancelIgnoresInvalidBackupItemsAsync()
    {
        var trader1 = this.CreateTrader(PlayerState.TradeOpened);
        var trader2 = this.CreateTrader(PlayerState.TradeOpened);
        Mock.Get(trader1).Setup(t => t.Logger).Returns(NullLogger.Instance);
        Mock.Get(trader2).Setup(t => t.Logger).Returns(NullLogger.Instance);
        trader1.TradingPartner = trader2;
        trader2.TradingPartner = trader1;

        var brokenItem = new Mock<Item>();
        brokenItem.SetupAllProperties();
        brokenItem.Object.ItemSlot = 20;

        var validItem = new Mock<Item>();
        validItem.SetupAllProperties();
        validItem.Object.ItemSlot = 21;

        trader1.BackupInventory = new BackupItemStorage(trader1.Inventory!.ItemStorage)
        {
            Items = new List<Item> { brokenItem.Object, validItem.Object },
        };

        var inventoryMock = Mock.Get(trader1.Inventory!);
        inventoryMock.Setup(i => i.AddItemAsync(20, brokenItem.Object)).Throws(new InvalidOperationException("broken test item"));
        inventoryMock.Setup(i => i.AddItemAsync(21, validItem.Object)).Returns(new ValueTask<bool>(true));

        var cancelAction = new TradeCancelAction();
        await cancelAction.CancelTradeAsync(trader1).ConfigureAwait(false);

        Assert.That(trader1.PlayerState.CurrentState, Is.EqualTo(PlayerState.EnteredWorld));
        Assert.That(trader2.PlayerState.CurrentState, Is.EqualTo(PlayerState.EnteredWorld));
        inventoryMock.Verify(i => i.AddItemAsync(21, validItem.Object), Times.Once);
        Assert.That(trader1.BackupInventory, Is.Null);
    }

    /// <summary>
    /// A trade already persisted to the trade context must never be rolled back because a player
    /// persistence context cannot track the transferred item after the commit.
    /// </summary>
    [Test]
    public async ValueTask CommittedTradeDoesNotRestoreOriginalInventoriesAfterTrackingConflictAsync()
    {
        var trader1 = this.CreateTrader(PlayerState.TradeOpened);
        var trader2 = this.CreateTrader(PlayerState.TradeOpened);
        trader1.TradingPartner = trader2;
        trader2.TradingPartner = trader1;
        trader1.Money = 1000;
        trader2.Money = 1000;
        trader1.TradingMoney = 100;
        trader2.TradingMoney = 200;

        var offered = this.GetItem();
        offered.Definition = new ItemDefinition { Width = 1, Height = 1, IsTradable = true };
        Mock.Get(trader1.TemporaryStorage!).Setup(s => s.Items).Returns(new List<Item> { offered });
        Mock.Get(trader1.Inventory!).Setup(s => s.TryTakeAllAsync(It.IsAny<IStorage>())).Returns(new ValueTask<bool>(true));
        Mock.Get(trader2.Inventory!).Setup(s => s.TryTakeAllAsync(It.IsAny<IStorage>())).Returns(new ValueTask<bool>(true));

        var tradeContext = new Mock<IContext>();
        tradeContext.Setup(c => c.SaveChangesAsync(It.IsAny<CancellationToken>())).Returns(new ValueTask<bool>(true));
        var provider = new Mock<IPersistenceContextProvider>();
        provider.Setup(p => p.CreateNewTradeContext()).Returns(tradeContext.Object);
        var gameContext = new Mock<IGameContext>();
        gameContext.Setup(c => c.PersistenceContextProvider).Returns(provider.Object);
        gameContext.Setup(c => c.PlugInManager).Returns(new PlugInManager(null, new NullLoggerFactory(), null, null));
        Mock.Get(trader1).Setup(t => t.GameContext).Returns(gameContext.Object);
        Mock.Get(trader2).Setup(t => t.GameContext).Returns(gameContext.Object);
        Mock.Get(trader2.PersistenceContext).Setup(c => c.AttachWithoutParent(offered))
            .Throws(new InvalidOperationException("An item with the same Id is already tracked."));

        var action = new TradeButtonAction();
        await action.TradeButtonChangedAsync(trader1, TradeButtonState.Checked).ConfigureAwait(false);
        await action.TradeButtonChangedAsync(trader2, TradeButtonState.Checked).ConfigureAwait(false);

        tradeContext.Verify(c => c.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Once);
        Mock.Get(trader1.ViewPlugIns.GetPlugIn<ITradeFinishedPlugIn>()!)
            .Verify(v => v!.TradeFinishedAsync(TradeResult.Success), Times.Once);
        Mock.Get(trader2.ViewPlugIns.GetPlugIn<ITradeFinishedPlugIn>()!)
            .Verify(v => v!.TradeFinishedAsync(TradeResult.Success), Times.Once);
        Assert.That(trader1.TradingPartner, Is.Null);
        Assert.That(trader2.TradingPartner, Is.Null);
        Assert.That(trader1.Money, Is.EqualTo(1200));
        Assert.That(trader2.Money, Is.EqualTo(1100));
        Mock.Get(trader1.Inventory!).Verify(i => i.Clear(), Times.Never);
        Mock.Get(trader2.Inventory!).Verify(i => i.Clear(), Times.Never);
    }

    /// <summary>
    /// An unsuccessful trade database save must still cancel safely, before any money
    /// is credited or committed-trade results are reported.
    /// </summary>
    [TestCase(false)]
    [TestCase(true)]
    public async ValueTask TradeSaveFailureDoesNotReportSuccessAsync(bool throwsException)
    {
        var trader1 = this.CreateTrader(PlayerState.TradeOpened);
        var trader2 = this.CreateTrader(PlayerState.TradeOpened);
        trader1.TradingPartner = trader2;
        trader2.TradingPartner = trader1;
        trader1.Money = 1000;
        trader2.Money = 1000;
        trader1.TradingMoney = 100;
        trader2.TradingMoney = 200;

        var offered = this.GetItem();
        offered.Definition = new ItemDefinition { Width = 1, Height = 1, IsTradable = true };
        Mock.Get(trader1.TemporaryStorage!).Setup(s => s.Items).Returns(new List<Item> { offered });
        Mock.Get(trader1.Inventory!).Setup(s => s.TryTakeAllAsync(It.IsAny<IStorage>())).Returns(new ValueTask<bool>(true));
        Mock.Get(trader2.Inventory!).Setup(s => s.TryTakeAllAsync(It.IsAny<IStorage>())).Returns(new ValueTask<bool>(true));
        var tradeContext = new Mock<IContext>();
        if (throwsException)
        {
            tradeContext.Setup(c => c.SaveChangesAsync(It.IsAny<CancellationToken>()))
                .Throws(new InvalidOperationException("Trade persistence failed before commit."));
        }
        else
        {
            tradeContext.Setup(c => c.SaveChangesAsync(It.IsAny<CancellationToken>()))
                .Returns(new ValueTask<bool>(false));
        }
        var provider = new Mock<IPersistenceContextProvider>();
        provider.Setup(p => p.CreateNewTradeContext()).Returns(tradeContext.Object);
        var gameContext = new Mock<IGameContext>();
        gameContext.Setup(c => c.PersistenceContextProvider).Returns(provider.Object);
        gameContext.Setup(c => c.PlugInManager).Returns(new PlugInManager(null, new NullLoggerFactory(), null, null));
        Mock.Get(trader1).Setup(t => t.GameContext).Returns(gameContext.Object);
        Mock.Get(trader2).Setup(t => t.GameContext).Returns(gameContext.Object);

        var action = new TradeButtonAction();
        await action.TradeButtonChangedAsync(trader1, TradeButtonState.Checked).ConfigureAwait(false);
        await action.TradeButtonChangedAsync(trader2, TradeButtonState.Checked).ConfigureAwait(false);

        tradeContext.Verify(c => c.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Once);
        Mock.Get(trader1.ViewPlugIns.GetPlugIn<ITradeFinishedPlugIn>()!)
            .Verify(v => v!.TradeFinishedAsync(TradeResult.Cancelled), Times.Once);
        Assert.That(trader1.Money, Is.EqualTo(1000));
        Assert.That(trader2.Money, Is.EqualTo(1000));
        Assert.That(trader1.TradingPartner, Is.Null);
        Assert.That(trader2.TradingPartner, Is.Null);
    }

    private static Task<bool> RequestAsync(Player requester, Player partner)
        => Task.Run(async () => await requester.RunPersistenceExclusiveAsync(() => new TradeRequestAction().RequestTradeAsync(requester, partner)).ConfigureAwait(false));

    private static void AssertPair(Player requester, Player partner)
    {
        Assert.Multiple(() =>
        {
            Assert.That(requester.PlayerState.CurrentState, Is.EqualTo(PlayerState.TradeRequested));
            Assert.That(partner.PlayerState.CurrentState, Is.EqualTo(PlayerState.TradeRequested));
            Assert.That(requester.TradingPartner, Is.SameAs(partner));
            Assert.That(partner.TradingPartner, Is.SameAs(requester));
        });
    }

    private static void AssertFree(Player player)
    {
        Assert.Multiple(() =>
        {
            Assert.That(player.PlayerState.CurrentState, Is.EqualTo(PlayerState.EnteredWorld));
            Assert.That(player.TradingPartner, Is.Null);
        });
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
