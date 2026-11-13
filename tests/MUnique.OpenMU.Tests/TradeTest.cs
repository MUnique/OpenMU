// <copyright file="TradeTest.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.Tests;

using System.Collections.Concurrent;
using System.IO.Pipelines;
using System.Threading;
using Microsoft.Extensions.Logging.Abstractions;
using Moq;
using MUnique.OpenMU.DataModel;
using MUnique.OpenMU.DataModel.Configuration;
using MUnique.OpenMU.DataModel.Configuration.Items;
using MUnique.OpenMU.DataModel.Entities;
using MUnique.OpenMU.GameLogic;
using MUnique.OpenMU.GameLogic.Attributes;
using MUnique.OpenMU.GameLogic.PlayerActions;
using MUnique.OpenMU.GameLogic.PlayerActions.Character;
using MUnique.OpenMU.GameLogic.PlayerActions.Items;
using MUnique.OpenMU.GameLogic.PlayerActions.Trade;
using MUnique.OpenMU.GameLogic.Views.Trade;
using MUnique.OpenMU.GameLogic.Views.Login;
using MUnique.OpenMU.GameLogic.Views;
using MUnique.OpenMU.Network;
using MUnique.OpenMU.GameServer.MessageHandler.Trade;
using MUnique.OpenMU.Persistence;
using MUnique.OpenMU.Persistence.InMemory;
using MUnique.OpenMU.PlugIns;
using Nito.AsyncEx;
using ServerPackets = MUnique.OpenMU.Network.Packets.ServerToClient;

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

    /// <summary>Logout cancels the current trade before character teardown and permits the same connection to trade again.</summary>
    /// <param name="phase">The ordinary trade state in which logout is requested.</param>
    [TestCase("requested")]
    [TestCase("opened")]
    [TestCase("pressed")]
    public async Task LogoutTradeCanReselectAndTradeAgainAsync(string phase)
    {
        var owner = await PlayerTestHelper.CreatePlayerAsync().ConfigureAwait(false);
        var peer = await PlayerTestHelper.CreatePlayerAsync(owner.GameContext).ConfigureAwait(false);
        owner.GameContext.Configuration.MaximumInventoryMoney = 1_000_000;
        var character = owner.SelectedCharacter!;
        character.Name = "LogoutA";
        Mock.Get(owner.Account!).Setup(a => a.Characters).Returns(new List<Character> { character });
        owner.Money = 200;
        peer.Money = 300;
        var inventory = owner.Inventory!;
        var item = this.GetItem();
        item.Definition = new ItemDefinition { Width = 1, Height = 1, IsTradable = true };
        Assert.That(await inventory.AddItemAsync(20, item).ConfigureAwait(false), Is.True);
        try
        {
            Assert.That(await Task.Run(async () => await owner.RunPersistenceExclusiveAsync(
                () => new TradeRequestAction().RequestTradeAsync(owner, peer)).ConfigureAwait(false)).ConfigureAwait(false), Is.True);
            if (phase != "requested")
            {
                await Task.Run(async () => await peer.RunPersistenceExclusiveAsync(
                    () => new TradeAcceptAction().HandleTradeAcceptAsync(peer, true)).ConfigureAwait(false)).ConfigureAwait(false);
                await Task.Run(async () => await owner.RunPersistenceExclusiveAsync(async () =>
                {
                    await new MoveItemAction().MoveItemAsync(owner, 20, Storages.Inventory, 0, Storages.Trade).ConfigureAwait(false);
                    await new TradeMoneyAction().TradeMoneyAsync(owner, 60).ConfigureAwait(false);
                    if (phase == "pressed")
                    {
                        await new TradeButtonAction().TradeButtonChangedAsync(owner, TradeButtonState.Checked).ConfigureAwait(false);
                    }
                }).ConfigureAwait(false)).ConfigureAwait(false);
            }

            await Task.Run(async () => await owner.RunPersistenceExclusiveAsync(
                () => new LogoutAction().LogoutAsync(owner, LogoutType.BackToCharacterSelection)).ConfigureAwait(false)).ConfigureAwait(false);
            // The peer's later cancel is a separate packet; it must not be needed to clean the logged-out player.
            await Task.Run(async () => await peer.RunPersistenceExclusiveAsync(
                () => new TradeCancelAction().CancelTradeAsync(peer)).ConfigureAwait(false)).ConfigureAwait(false);
            var stateAfterLogout = owner.PlayerState.CurrentState;
            var characterAfterLogout = owner.SelectedCharacter;
            var partnerAfterLogout = owner.TradingPartner;
            var moneyAfterLogout = owner.TradingMoney;
            var peerAfterLogout = peer.TradingPartner;
            await owner.RunPersistenceExclusiveAsync(() => new RequestCharacterListAction().RequestCharacterListAsync(owner)).ConfigureAwait(false);
            await owner.RunPersistenceExclusiveAsync(() => new SelectCharacterAction().SelectCharacterAsync(owner, character.Name)).ConfigureAwait(false);
            Assert.That(owner.PlayerState.CurrentState, Is.EqualTo(PlayerState.EnteredWorld));
            var requested = await Task.Run(async () => await peer.RunPersistenceExclusiveAsync(
                () => new TradeRequestAction().RequestTradeAsync(peer, owner)).ConfigureAwait(false)).ConfigureAwait(false);
            Assert.Multiple(() =>
            {
                Assert.That(stateAfterLogout, Is.EqualTo(PlayerState.Authenticated));
                Assert.That(characterAfterLogout, Is.Null);
                Assert.That(partnerAfterLogout, Is.Null);
                Assert.That(moneyAfterLogout, Is.Zero);
                Assert.That(owner.BackupInventory, Is.Null);
                Assert.That(peerAfterLogout, Is.Null);
                Assert.That(peer.BackupInventory, Is.Null);
                Assert.That(inventory.ItemStorage.Money, Is.EqualTo(200));
                Assert.That(inventory.Items.Count(i => ReferenceEquals(i, item)), Is.EqualTo(1));
                Assert.That(peer.Money, Is.EqualTo(300));
                Assert.That(requested, Is.True, "Same-session trade must work after actual logout and character reselection.");
            });
            await owner.RunPersistenceExclusiveAsync(() => new TradeAcceptAction().HandleTradeAcceptAsync(owner, true)).ConfigureAwait(false);
            Assert.That(owner.PlayerState.CurrentState, Is.EqualTo(PlayerState.TradeOpened));
            await owner.RunPersistenceExclusiveAsync(() => new TradeCancelAction().CancelTradeAsync(owner)).ConfigureAwait(false);
            Assert.That(owner.Money, Is.EqualTo(200));
            Assert.That(owner.Inventory!.Items.Count(i => ReferenceEquals(i, item)), Is.EqualTo(1));
        }
        finally
        {
            await owner.DisposeAsync().ConfigureAwait(false);
            await peer.DisposeAsync().ConfigureAwait(false);
        }
    }

    /// <summary>The real accepter funnel may resume a failed opening after peer logout or death; that lifecycle owns cleanup.</summary>
    /// <param name="logout">Whether the peer leaves to character selection rather than dying.</param>
    [TestCase(false)]
    [TestCase(true)]
    public async Task InterruptedAcceptanceIsCleanedByPeerLifecycleAsync(bool logout)
    {
        var seed = await PlayerTestHelper.CreatePlayerAsync().ConfigureAwait(false);
        var configuration = seed.GameContext.Configuration;
        var provider = new PausingContextProvider(configuration);
        var initializer = new MapInitializer(configuration, NullLogger<MapInitializer>.Instance, NullDropGenerator.Instance, null);
        using var game = new GameContext(configuration, provider, initializer, new NullLoggerFactory(), new PlugInManager(null, new NullLoggerFactory(), null, null), NullDropGenerator.Instance, new ConfigurationChangeMediator());
        initializer.PlugInManager = game.PlugInManager;
        initializer.PathFinderPool = game.PathFinderPool;
        var accepter = await PlayerTestHelper.CreatePlayerAsync(game).ConfigureAwait(false);
        var peer = await PlayerTestHelper.CreatePlayerAsync(game).ConfigureAwait(false);
        var attacker = await PlayerTestHelper.CreatePlayerAsync(game).ConfigureAwait(false);
        peer.Attributes![Stats.CurrentHealth] = 10;
        var entered = new TaskCompletionSource(TaskCreationOptions.RunContinuationsAsynchronously);
        var release = new TaskCompletionSource(TaskCreationOptions.RunContinuationsAsynchronously);
        ((PausingPlayerContext)accepter.PersistenceContext).BeforeSave = async () =>
        {
            entered.TrySetResult();
            await release.Task.ConfigureAwait(false);
        };
        Assert.That(await peer.RunPersistenceExclusiveAsync(() => new TradeRequestAction().RequestTradeAsync(peer, accepter)).ConfigureAwait(false), Is.True);
        var acceptance = Task.Run(async () => await accepter.RunPersistenceExclusiveAsync(
            () => new TradeAcceptAction().HandleTradeAcceptAsync(accepter, true)).ConfigureAwait(false));
        try
        {
            await entered.Task.WaitAsync(TimeSpan.FromSeconds(5)).ConfigureAwait(false);
            if (logout)
            {
                await Task.Run(async () => await peer.RunPersistenceExclusiveAsync(
                    () => new LogoutAction().LogoutAsync(peer, LogoutType.BackToCharacterSelection)).ConfigureAwait(false)).WaitAsync(TimeSpan.FromSeconds(5)).ConfigureAwait(false);
            }
            else
            {
                await Task.Run(async () => await attacker.RunPersistenceExclusiveAsync(
                    () => peer.ApplyPoisonDamageAsync(attacker, 10)).ConfigureAwait(false)).WaitAsync(TimeSpan.FromSeconds(5)).ConfigureAwait(false);
            }

            release.TrySetResult();
            await acceptance.WaitAsync(TimeSpan.FromSeconds(5)).ConfigureAwait(false);
            Assert.Multiple(() =>
            {
                Assert.That(accepter.PlayerState.CurrentState, Is.EqualTo(PlayerState.EnteredWorld));
                Assert.That(accepter.TradingPartner, Is.Null);
                Assert.That(accepter.BackupInventory, Is.Null);
                Assert.That(peer.TradingPartner, Is.Null);
                Assert.That(peer.BackupInventory, Is.Null);
            });
            Assert.That(await attacker.RunPersistenceExclusiveAsync(() => new TradeRequestAction().RequestTradeAsync(attacker, accepter)).ConfigureAwait(false), Is.True);
            await attacker.RunPersistenceExclusiveAsync(() => new TradeCancelAction().CancelTradeAsync(attacker)).ConfigureAwait(false);
        }
        finally
        {
            release.TrySetResult();
            await acceptance.WaitAsync(TimeSpan.FromSeconds(5)).ConfigureAwait(false);
            await accepter.DisposeAsync().ConfigureAwait(false);
            await peer.DisposeAsync().ConfigureAwait(false);
            await attacker.DisposeAsync().ConfigureAwait(false);
            await seed.DisposeAsync().ConfigureAwait(false);
        }
    }

    /// <summary>The actual handler resolves self observation but must reject self trade without publishing state or views.</summary>
    /// <param name="self">Whether the malformed request targets its own connection's player.</param>
    [TestCase(false)]
    [TestCase(true)]
    public async Task HandlerRejectsSelfTradeWithoutMutationAsync(bool self)
    {
        var player = await PlayerTestHelper.CreatePlayerAsync().ConfigureAwait(false);
        var peer = await PlayerTestHelper.CreatePlayerAsync(player.GameContext).ConfigureAwait(false);
        var target = self ? player : peer;
        Assert.That(await player.GetObservingPlayerWithIdAsync(target.Id).ConfigureAwait(false), Is.SameAs(target));
        var packet = CreateTradeRequestPacket(target.Id);
        try
        {
            await player.RunPersistenceExclusiveAsync(() => new TradeRequestHandlerPlugIn().HandlePacketAsync(player, packet)).ConfigureAwait(false);
            Assert.Multiple(() =>
            {
                Assert.That(player.PlayerState.CurrentState, Is.EqualTo(self ? PlayerState.EnteredWorld : PlayerState.TradeRequested));
                Assert.That(player.TradingPartner, Is.SameAs(self ? null : peer));
                Assert.That(player.BackupInventory, Is.Null);
                Assert.That(player.TradingMoney, Is.Zero);
            });
            Mock.Get(target.ViewPlugIns.GetPlugIn<IShowTradeRequestPlugIn>()!).Verify(v => v.ShowTradeRequestAsync(player), self ? Times.Never() : Times.Once());
            if (!self)
            {
                await peer.RunPersistenceExclusiveAsync(() => new TradeAcceptAction().HandleTradeAcceptAsync(peer, true)).ConfigureAwait(false);
                await peer.RunPersistenceExclusiveAsync(() => new TradeCancelAction().CancelTradeAsync(peer)).ConfigureAwait(false);
            }
        }
        finally
        {
            await player.DisposeAsync().ConfigureAwait(false);
            await peer.DisposeAsync().ConfigureAwait(false);
        }
    }

    /// <summary>The production packet funnel queues an early OK until acceptance finished both openings.</summary>
    /// <param name="earlyOk">Whether the OK packet arrives during the peer's persistence save.</param>
    [TestCase(false)]
    [TestCase(true)]
    public async Task TradeOkFunnelWaitsForAcceptanceAsync(bool earlyOk)
    {
        var seed = await PlayerTestHelper.CreatePlayerAsync().ConfigureAwait(false);
        var configuration = seed.GameContext.Configuration;
        var provider = new PausingContextProvider(configuration);
        var initializer = new MapInitializer(configuration, NullLogger<MapInitializer>.Instance, NullDropGenerator.Instance, null);
        using var game = new GameContext(configuration, provider, initializer, new NullLoggerFactory(), new PlugInManager(null, new NullLoggerFactory(), null, null), NullDropGenerator.Instance, new ConfigurationChangeMediator());
        initializer.PlugInManager = game.PlugInManager;
        initializer.PathFinderPool = game.PathFinderPool;
        var target = await PlayerTestHelper.CreatePlayerAsync(game).ConfigureAwait(false);
        var peer = await PlayerTestHelper.CreatePlayerAsync(game).ConfigureAwait(false);
        var item = this.GetItem();
        Assert.That(await target.Inventory!.AddItemAsync(item).ConfigureAwait(false), Is.True);
        target.Inventory.ItemStorage.Money = 100;
        peer.Inventory!.ItemStorage.Money = 200;
        var entered = new TaskCompletionSource(TaskCreationOptions.RunContinuationsAsynchronously);
        var release = new TaskCompletionSource(TaskCreationOptions.RunContinuationsAsynchronously);
        ((PausingPlayerContext)peer.PersistenceContext).BeforeSave = async () =>
        {
            entered.TrySetResult();
            await release.Task.ConfigureAwait(false);
        };
        Assert.That(await new TradeRequestAction().RequestTradeAsync(peer, target).ConfigureAwait(false), Is.True);
        // Independent request contexts, not descendants of the lock-owning save callback.
        var opening = Task.Run(async () => await target.RunPersistenceExclusiveAsync(
            () => new TradeAcceptAction().HandleTradeAcceptAsync(target, true)).ConfigureAwait(false));
        Task confirmation = Task.CompletedTask;
        try
        {
            await entered.Task.WaitAsync(TimeSpan.FromSeconds(5)).ConfigureAwait(false);
            Assert.That(target.BackupInventory, Is.Not.Null);
            if (earlyOk)
            {
                var attempted = new TaskCompletionSource(TaskCreationOptions.RunContinuationsAsynchronously);
                confirmation = Task.Run(async () =>
                {
                    var queued = target.RunPersistenceExclusiveAsync(
                        () => new TradeButtonAction().TradeButtonChangedAsync(target, TradeButtonState.Checked));
                    attempted.TrySetResult();
                    await queued.ConfigureAwait(false);
                });
                await attempted.Task.WaitAsync(TimeSpan.FromSeconds(5)).ConfigureAwait(false);
                Assert.That(confirmation.IsCompleted, Is.False);
                Assert.That(target.PlayerState.CurrentState, Is.EqualTo(PlayerState.TradeOpened));
            }

            release.TrySetResult();
            await Task.WhenAll(opening, confirmation).WaitAsync(TimeSpan.FromSeconds(5)).ConfigureAwait(false);
            if (!earlyOk)
            {
                await target.RunPersistenceExclusiveAsync(
                    () => new TradeButtonAction().TradeButtonChangedAsync(target, TradeButtonState.Checked)).ConfigureAwait(false);
            }

            Assert.Multiple(() =>
            {
                Assert.That(target.PlayerState.CurrentState, Is.EqualTo(PlayerState.TradeButtonPressed));
                Assert.That(peer.PlayerState.CurrentState, Is.EqualTo(PlayerState.TradeOpened));
                Assert.That(target.BackupInventory!.Money, Is.EqualTo(100));
                Assert.That(target.BackupInventory.Items, Does.Contain(item));
                Assert.That(peer.BackupInventory, Is.Not.Null);
                Assert.That(peer.BackupInventory!.Money, Is.EqualTo(200));
            });
            Mock.Get(peer.ViewPlugIns.GetPlugIn<IShowTradeRequestAnswerPlugIn>()!).Verify(v => v.ShowTradeRequestAnswerAsync(true), Times.Once);
            await peer.RunPersistenceExclusiveAsync(() => new TradeCancelAction().CancelTradeAsync(peer)).ConfigureAwait(false);
            Assert.That(target.BackupInventory, Is.Null);
            Assert.That(peer.BackupInventory, Is.Null);
            var targetStorage = target.Inventory.ItemStorage;
            var peerStorage = peer.Inventory.ItemStorage;
            targetStorage.Money = 333;
            peerStorage.Money = 444;
            await target.RemoveFromGameAsync().ConfigureAwait(false);
            await peer.RemoveFromGameAsync().ConfigureAwait(false);
            Assert.That(targetStorage.Money, Is.EqualTo(333));
            Assert.That(peerStorage.Money, Is.EqualTo(444));
            Assert.That(targetStorage.Items.Count(i => ReferenceEquals(i, item)), Is.EqualTo(1));
        }
        finally
        {
            release.TrySetResult();
            await Task.WhenAll(opening, confirmation).WaitAsync(TimeSpan.FromSeconds(5)).ConfigureAwait(false);
            await target.DisposeAsync().ConfigureAwait(false);
            await peer.DisposeAsync().ConfigureAwait(false);
            await seed.DisposeAsync().ConfigureAwait(false);
        }
    }

    /// <summary>A delayed old cancel must not close a replacement trade opened through another participant's funnel.</summary>
    /// <param name="replace">Whether the already-cleared partner opens another trade before notification resumes.</param>
    [TestCase(false)]
    [TestCase(true)]
    public async Task DelayedCancelDoesNotCloseReplacementTradeAsync(bool replace)
    {
        var owner = await PlayerTestHelper.CreatePlayerAsync().ConfigureAwait(false);
        var partner = await PlayerTestHelper.CreatePlayerAsync(owner.GameContext).ConfigureAwait(false);
        var replacement = await PlayerTestHelper.CreatePlayerAsync(owner.GameContext).ConfigureAwait(false);
        owner.GameContext.Configuration.MaximumInventoryMoney = 1_000_000;
        owner.SelectedCharacter!.Name = "TraderA";
        partner.SelectedCharacter!.Name = "TraderB";
        replacement.SelectedCharacter!.Name = "TraderC";
        foreach (var player in new[] { owner, partner, replacement })
        {
            player.Attributes![Stats.CurrentHealth] = 100;
        }

        owner.Inventory!.ItemStorage.Money = 100;
        partner.Inventory!.ItemStorage.Money = 200;
        replacement.Inventory!.ItemStorage.Money = 300;
        var item = this.GetItem();
        item.Definition = new ItemDefinition { Width = 1, Height = 1, IsTradable = true };
        Assert.That(await partner.Inventory.AddItemAsync(20, item).ConfigureAwait(false), Is.True);
        var events = new ConcurrentQueue<string>();
        var partnerWindow = false;
        var partnerCancelled = 0;
        Mock.Get(partner.ViewPlugIns.GetPlugIn<IShowTradeRequestAnswerPlugIn>()!).Setup(v => v.ShowTradeRequestAnswerAsync(true)).Returns(() =>
        {
            partnerWindow = true;
            events.Enqueue(ReferenceEquals(partner.TradingPartner, replacement) ? "B:open:C" : "B:open:A");
            return ValueTask.CompletedTask;
        });
        Mock.Get(partner.ViewPlugIns.GetPlugIn<ITradeFinishedPlugIn>()!).Setup(v => v.TradeFinishedAsync(TradeResult.Cancelled)).Returns(() =>
        {
            Assert.That(partner.IsApplyingHitOnCurrentThread, Is.False);
            Interlocked.Increment(ref partnerCancelled);
            partnerWindow = false;
            events.Enqueue("B:cancel");
            return ValueTask.CompletedTask;
        });
        Assert.That(await Task.Run(async () => await owner.RunPersistenceExclusiveAsync(
            () => new TradeRequestAction().RequestTradeAsync(owner, partner)).ConfigureAwait(false)).ConfigureAwait(false), Is.True);
        await Task.Run(async () => await partner.RunPersistenceExclusiveAsync(
            () => new TradeAcceptAction().HandleTradeAcceptAsync(partner, true)).ConfigureAwait(false)).ConfigureAwait(false);
        var notifyingOwner = new TaskCompletionSource(TaskCreationOptions.RunContinuationsAsynchronously);
        var releaseOwnerView = new TaskCompletionSource(TaskCreationOptions.RunContinuationsAsynchronously);
        Mock.Get(owner.ViewPlugIns.GetPlugIn<ITradeFinishedPlugIn>()!).Setup(v => v.TradeFinishedAsync(TradeResult.Cancelled)).Returns(async () =>
        {
            // Real TradeFinished sends its result first, then awaits inventory/money views.
            Assert.That(owner.IsApplyingHitOnCurrentThread, Is.False);
            events.Enqueue("A:cancel");
            notifyingOwner.TrySetResult();
            await releaseOwnerView.Task.ConfigureAwait(false);
        });
        var cancellation = Task.Run(async () => await owner.RunPersistenceExclusiveAsync(
            () => new TradeCancelAction().CancelTradeAsync(owner)).ConfigureAwait(false));
        try
        {
            await notifyingOwner.Task.WaitAsync(TimeSpan.FromSeconds(5)).ConfigureAwait(false);
            var cancelsBeforeReplacement = partnerCancelled;
            BackupItemStorage? newBackup = null;
            if (replace)
            {
                // These requests start in independent coordinator contexts, not in A's held funnel.
                Assert.That(await Task.Run(async () => await replacement.RunPersistenceExclusiveAsync(
                    () => new TradeRequestAction().RequestTradeAsync(replacement, partner)).ConfigureAwait(false)).WaitAsync(TimeSpan.FromSeconds(5)).ConfigureAwait(false), Is.True);
                await Task.Run(async () => await partner.RunPersistenceExclusiveAsync(
                    () => new TradeAcceptAction().HandleTradeAcceptAsync(partner, true)).ConfigureAwait(false)).WaitAsync(TimeSpan.FromSeconds(5)).ConfigureAwait(false);
                newBackup = partner.BackupInventory;
                await Task.Run(async () => await partner.RunPersistenceExclusiveAsync(async () =>
                {
                    await new MoveItemAction().MoveItemAsync(partner, 20, Storages.Inventory, 0, Storages.Trade).ConfigureAwait(false);
                    await new TradeMoneyAction().TradeMoneyAsync(partner, 60).ConfigureAwait(false);
                }).ConfigureAwait(false)).WaitAsync(TimeSpan.FromSeconds(5)).ConfigureAwait(false);
                Assert.That(cancellation.IsCompleted, Is.False);
            }

            releaseOwnerView.TrySetResult();
            await cancellation.WaitAsync(TimeSpan.FromSeconds(5)).ConfigureAwait(false);
            Assert.Multiple(() =>
            {
                Assert.That(owner.PlayerState.CurrentState, Is.EqualTo(PlayerState.EnteredWorld));
                Assert.That(owner.TradingPartner, Is.Null);
                Assert.That(owner.BackupInventory, Is.Null);
                Assert.That(owner.Inventory.ItemStorage.Money, Is.EqualTo(100));
                Assert.That(partnerWindow, Is.EqualTo(replace));
                if (replace)
                {
                    Assert.That(partnerCancelled, Is.EqualTo(cancelsBeforeReplacement));
                    Assert.That(partner.PlayerState.CurrentState, Is.EqualTo(PlayerState.TradeOpened));
                    Assert.That(replacement.PlayerState.CurrentState, Is.EqualTo(PlayerState.TradeOpened));
                    Assert.That(partner.TradingPartner, Is.SameAs(replacement));
                    Assert.That(replacement.TradingPartner, Is.SameAs(partner));
                    Assert.That(partner.BackupInventory, Is.SameAs(newBackup));
                    Assert.That(newBackup, Is.Not.Null);
                    Assert.That(partner.Inventory.ItemStorage.Money, Is.EqualTo(140));
                    Assert.That(partner.TradingMoney, Is.EqualTo(60));
                    Assert.That(partner.TemporaryStorage!.Items.Single(), Is.SameAs(item));
                    Assert.That(events.Last(), Is.EqualTo("B:open:C"));
                }
                else
                {
                    Assert.That(partnerCancelled, Is.EqualTo(1));
                    Assert.That(partner.PlayerState.CurrentState, Is.EqualTo(PlayerState.EnteredWorld));
                    Assert.That(partner.TradingPartner, Is.Null);
                    Assert.That(partner.BackupInventory, Is.Null);
                }
            });
            if (replace)
            {
                await Task.Run(async () => await replacement.RunPersistenceExclusiveAsync(
                    () => new TradeCancelAction().CancelTradeAsync(replacement)).ConfigureAwait(false)).ConfigureAwait(false);
            }

            Assert.That(partner.Inventory.ItemStorage.Money, Is.EqualTo(200));
            Assert.That(replacement.Inventory.ItemStorage.Money, Is.EqualTo(300));
            Assert.That(partner.Inventory.ItemStorage.Items.Count(i => ReferenceEquals(i, item)), Is.EqualTo(1));
            Assert.That(partner.TemporaryStorage!.Items, Is.Empty);
        }
        finally
        {
            releaseOwnerView.TrySetResult();
            await cancellation.WaitAsync(TimeSpan.FromSeconds(5)).ConfigureAwait(false);
            if (replacement.TradingPartner is not null)
            {
                await replacement.RunPersistenceExclusiveAsync(() => new TradeCancelAction().CancelTradeAsync(replacement)).ConfigureAwait(false);
            }

            await owner.DisposeAsync().ConfigureAwait(false);
            await partner.DisposeAsync().ConfigureAwait(false);
            await replacement.DisposeAsync().ConfigureAwait(false);
        }
    }

    /// <summary>A skipped cancellation must not initiate a notification against the current window.</summary>
    /// <param name="invalidState">Whether cancellation fails its state check instead of its expected-partner check.</param>
    [TestCase(false)]
    [TestCase(true)]
    public async Task SkippedCancellationDoesNotNotifyAsync(bool invalidState)
    {
        var state = invalidState ? PlayerState.EnteredWorld : PlayerState.TradeOpened;
        var trader = this.CreateTrader(state);
        var currentPartner = this.CreateTrader(PlayerState.TradeOpened);
        var oldPartner = this.CreateTrader(PlayerState.EnteredWorld);
        trader.TradingPartner = currentPartner;
        currentPartner.TradingPartner = trader;
        var backup = trader.BackupInventory;
        var windowOpen = true;
        Mock.Get(trader.ViewPlugIns.GetPlugIn<ITradeFinishedPlugIn>()!).Setup(v => v.TradeFinishedAsync(TradeResult.Cancelled)).Returns(() =>
        {
            windowOpen = false;
            return ValueTask.CompletedTask;
        });
        var started = false;
        var notification = ValueTask.CompletedTask;
        await new BaseTradeAction().CancelTradeAndStartNotificationAsync(trader, invalidState ? currentPartner : oldPartner, n =>
        {
            started = true;
            notification = n;
        }).ConfigureAwait(false);
        await notification.ConfigureAwait(false);
        Assert.Multiple(() =>
        {
            Assert.That(started, Is.False);
            Assert.That(windowOpen, Is.True);
            Assert.That(trader.TradingPartner, Is.SameAs(currentPartner));
            Assert.That(trader.BackupInventory, Is.SameAs(backup));
            Assert.That(trader.PlayerState.CurrentState, Is.SameAs(state));
        });
    }

    /// <summary>Async notification reentry can acquire the just-cancelled participant's state after its scope ends.</summary>
    [Test]
    public async Task CancelNotificationCanReenterTradeAsync()
    {
        var owner = await PlayerTestHelper.CreatePlayerAsync().ConfigureAwait(false);
        var partner = await PlayerTestHelper.CreatePlayerAsync(owner.GameContext).ConfigureAwait(false);
        var replacement = await PlayerTestHelper.CreatePlayerAsync(owner.GameContext).ConfigureAwait(false);
        foreach (var player in new[] { owner, partner, replacement })
        {
            player.Attributes![Stats.CurrentHealth] = 100;
        }

        Assert.That(await owner.RunPersistenceExclusiveAsync(() => new TradeRequestAction().RequestTradeAsync(owner, partner)).ConfigureAwait(false), Is.True);
        await partner.RunPersistenceExclusiveAsync(() => new TradeAcceptAction().HandleTradeAcceptAsync(partner, true)).ConfigureAwait(false);
        var notifications = 0;
        var notificationUnderHit = false;
        var windowOpen = true;
        Mock.Get(partner.ViewPlugIns.GetPlugIn<IShowTradeRequestAnswerPlugIn>()!).Setup(v => v.ShowTradeRequestAnswerAsync(true)).Returns(() =>
        {
            windowOpen = true;
            return ValueTask.CompletedTask;
        });
        Mock.Get(partner.ViewPlugIns.GetPlugIn<ITradeFinishedPlugIn>()!).Setup(v => v.TradeFinishedAsync(TradeResult.Cancelled)).Returns(async () =>
        {
            notificationUnderHit |= partner.IsApplyingHitOnCurrentThread;
            windowOpen = false;
            if (Interlocked.Increment(ref notifications) == 1)
            {
                // Intentional view reentry, not the independent packet-interleaving reproduction above.
                Assert.That(await replacement.RunPersistenceExclusiveAsync(() => new TradeRequestAction().RequestTradeAsync(replacement, partner)).ConfigureAwait(false), Is.True);
                await partner.RunPersistenceExclusiveAsync(() => new TradeAcceptAction().HandleTradeAcceptAsync(partner, true)).ConfigureAwait(false);
            }
        });
        try
        {
            await owner.RunPersistenceExclusiveAsync(() => new TradeCancelAction().CancelTradeAsync(owner)).AsTask().WaitAsync(TimeSpan.FromSeconds(5)).ConfigureAwait(false);
            Assert.Multiple(() =>
            {
                Assert.That(notifications, Is.EqualTo(1));
                Assert.That(notificationUnderHit, Is.False);
                Assert.That(windowOpen, Is.True);
                Assert.That(owner.TradingPartner, Is.Null);
                Assert.That(partner.TradingPartner, Is.SameAs(replacement));
                Assert.That(replacement.TradingPartner, Is.SameAs(partner));
                Assert.That(partner.PlayerState.CurrentState, Is.EqualTo(PlayerState.TradeOpened));
                Assert.That(partner.BackupInventory, Is.Not.Null);
            });
        }
        finally
        {
            if (replacement.TradingPartner is not null)
            {
                await replacement.RunPersistenceExclusiveAsync(() => new TradeCancelAction().CancelTradeAsync(replacement)).ConfigureAwait(false);
            }

            await owner.DisposeAsync().ConfigureAwait(false);
            await partner.DisposeAsync().ConfigureAwait(false);
            await replacement.DisposeAsync().ConfigureAwait(false);
        }
    }

    /// <summary>Either participant's rollback or notification failure must not skip other cleanup or leak a started notification.</summary>
    /// <param name="failPartner">Whether the partner fails rather than the initiating trader.</param>
    /// <param name="notificationFails">Whether notification resolution fails rather than inventory rollback.</param>
    [TestCase(false, false)]
    [TestCase(true, false)]
    [TestCase(false, true)]
    [TestCase(true, true)]
    public async Task CancelFailureStillFinishesOtherSideAsync(bool failPartner, bool notificationFails)
    {
        var trader = this.CreateTrader(PlayerState.TradeOpened);
        var partner = this.CreateTrader(PlayerState.TradeOpened);
        trader.TradingPartner = partner;
        partner.TradingPartner = trader;
        Mock.Get(trader).Setup(t => t.Logger).Returns(NullLogger.Instance);
        Mock.Get(partner).Setup(t => t.Logger).Returns(NullLogger.Instance);
        var failing = failPartner ? partner : trader;
        var successful = failPartner ? trader : partner;
        var failure = new InvalidOperationException("Expected cancellation failure.");
        var failureStarted = new TaskCompletionSource(TaskCreationOptions.RunContinuationsAsynchronously);
        if (notificationFails)
        {
            var views = new Mock<ICustomPlugInContainer<IViewPlugIn>>();
            views.Setup(v => v.GetPlugIn<ITradeFinishedPlugIn>()).Callback(() => failureStarted.TrySetResult()).Throws(failure);
            Mock.Get(failing).Setup(t => t.ViewPlugIns).Returns(views.Object);
        }
        else
        {
            Mock.Get(failing.Inventory!).Setup(i => i.Clear()).Callback(() => failureStarted.TrySetResult()).Throws(failure);
        }

        var notificationStarted = new TaskCompletionSource(TaskCreationOptions.RunContinuationsAsynchronously);
        var releaseNotification = new TaskCompletionSource(TaskCreationOptions.RunContinuationsAsynchronously);
        Mock.Get(successful.ViewPlugIns.GetPlugIn<ITradeFinishedPlugIn>()!).Setup(v => v.TradeFinishedAsync(TradeResult.Cancelled)).Returns(async () =>
        {
            notificationStarted.TrySetResult();
            await releaseNotification.Task.ConfigureAwait(false);
        });
        var cancellation = new TradeCancelAction().CancelTradeAsync(trader).AsTask();
        try
        {
            await Task.WhenAll(notificationStarted.Task, failureStarted.Task).WaitAsync(TimeSpan.FromSeconds(5)).ConfigureAwait(false);
            Assert.Multiple(() =>
            {
                Assert.That(cancellation.IsCompleted, Is.False);
                Assert.That(successful.PlayerState.CurrentState, Is.EqualTo(PlayerState.EnteredWorld));
                Assert.That(successful.TradingPartner, Is.Null);
                Assert.That(successful.BackupInventory, Is.Null);
                if (notificationFails)
                {
                    Assert.That(failing.TradingPartner, Is.Null);
                    Assert.That(failing.BackupInventory, Is.Null);
                }
            });
        }
        finally
        {
            releaseNotification.TrySetResult();
            var observed = Assert.ThrowsAsync<InvalidOperationException>(async () => await cancellation.WaitAsync(TimeSpan.FromSeconds(5)).ConfigureAwait(false));
            Assert.That(observed, Is.SameAs(failure));
        }
    }

    /// <summary>A later notification error is observed without replacing the rollback error.</summary>
    [Test]
    public async Task CancelRollbackErrorSurvivesOtherNotificationErrorAsync()
    {
        var trader = this.CreateTrader(PlayerState.TradeOpened);
        var partner = this.CreateTrader(PlayerState.TradeOpened);
        trader.TradingPartner = partner;
        partner.TradingPartner = trader;
        var logger = new Mock<Microsoft.Extensions.Logging.ILogger>();
        Mock.Get(trader).Setup(t => t.Logger).Returns(logger.Object);
        var rollbackError = new InvalidOperationException("Rollback failure.");
        var notificationError = new InvalidOperationException("Notification failure.");
        Mock.Get(trader.Inventory!).Setup(i => i.Clear()).Throws(rollbackError);
        var views = new Mock<ICustomPlugInContainer<IViewPlugIn>>();
        views.Setup(v => v.GetPlugIn<ITradeFinishedPlugIn>()).Throws(notificationError);
        Mock.Get(partner).Setup(t => t.ViewPlugIns).Returns(views.Object);

        var observed = Assert.ThrowsAsync<InvalidOperationException>(async () => await new TradeCancelAction().CancelTradeAsync(trader).ConfigureAwait(false));
        Assert.Multiple(() =>
        {
            Assert.That(observed, Is.SameAs(rollbackError));
            Assert.That(partner.TradingPartner, Is.Null);
            Assert.That(partner.BackupInventory, Is.Null);
            Assert.That(logger.Invocations.Any(i => i.Arguments.Any(a => ReferenceEquals(a, notificationError))), Is.True);
        });
    }

    /// <summary>The concrete remote view's first generated send enters OutputLock FIFO even while another write holds it.</summary>
    [Test]
    public async Task ContendedTradeFinishedSendPrecedesReplacementAnswerAsync()
    {
        var pipe = new Pipe();
        var outputLock = new AsyncLock();
        var connection = new Mock<IConnection>();
        connection.SetupGet(c => c.Connected).Returns(true);
        connection.SetupGet(c => c.Output).Returns(pipe.Writer);
        connection.SetupGet(c => c.OutputLock).Returns(outputLock);
        IDisposable? held = await outputLock.LockAsync().ConfigureAwait(false);
        var cancellation = Task.CompletedTask;
        var opening = Task.CompletedTask;
        try
        {
            cancellation = ServerPackets.ConnectionExtensions.SendTradeFinishedAsync(connection.Object, ServerPackets.TradeFinished.TradeResult.Cancelled).AsTask();
            opening = ServerPackets.ConnectionExtensions.SendTradeRequestAnswerAsync(connection.Object, true, "TraderC", 1, 0).AsTask();
            Assert.That(cancellation.IsCompleted, Is.False);
            Assert.That(opening.IsCompleted, Is.False);
            held.Dispose();
            held = null;
            await Task.WhenAll(cancellation, opening).WaitAsync(TimeSpan.FromSeconds(5)).ConfigureAwait(false);
            var read = await pipe.Reader.ReadAsync().ConfigureAwait(false);
            var bytes = System.Buffers.BuffersExtensions.ToArray(read.Buffer);
            pipe.Reader.AdvanceTo(read.Buffer.End);
            Assert.Multiple(() =>
            {
                Assert.That(bytes, Has.Length.EqualTo(ServerPackets.TradeFinishedRef.Length + ServerPackets.TradeRequestAnswerRef.Length));
                Assert.That(bytes[2], Is.EqualTo(ServerPackets.TradeFinishedRef.Code));
                Assert.That(bytes[3], Is.EqualTo((byte)ServerPackets.TradeFinished.TradeResult.Cancelled));
                Assert.That(bytes[ServerPackets.TradeFinishedRef.Length + 2], Is.EqualTo(ServerPackets.TradeRequestAnswerRef.Code));
            });
        }
        finally
        {
            held?.Dispose();
            await Task.WhenAll(cancellation, opening).WaitAsync(TimeSpan.FromSeconds(5)).ConfigureAwait(false);
            await pipe.Writer.CompleteAsync().ConfigureAwait(false);
            await pipe.Reader.CompleteAsync().ConfigureAwait(false);
        }
    }

    /// <summary>
    /// A cancelled acceptance cannot recreate inventory backups across any awaited preparation step.
    /// </summary>
    /// <param name="pause">The awaited step which is paused.</param>
    [TestCase("save")]
    [TestCase("temporary-return")]
    [TestCase("accepted-view")]
    public async Task CancelledAcceptanceCannotRestoreStaleInventoryAsync(string pause)
    {
        var seed = await PlayerTestHelper.CreatePlayerAsync().ConfigureAwait(false);
        var configuration = seed.GameContext.Configuration;
        var provider = new PausingContextProvider(configuration);
        var mapInitializer = new MapInitializer(configuration, NullLogger<MapInitializer>.Instance, NullDropGenerator.Instance, null);
        using var game = new GameContext(configuration, provider, mapInitializer, new NullLoggerFactory(), new PlugInManager(null, new NullLoggerFactory(), null, null), NullDropGenerator.Instance, new ConfigurationChangeMediator());
        mapInitializer.PlugInManager = game.PlugInManager;
        mapInitializer.PathFinderPool = game.PathFinderPool;
        var entered = new TaskCompletionSource(TaskCreationOptions.RunContinuationsAsynchronously);
        var release = new TaskCompletionSource(TaskCreationOptions.RunContinuationsAsynchronously);
        async ValueTask PauseAsync()
        {
            entered.TrySetResult();
            await release.Task.ConfigureAwait(false);
        }

        var pendingStorage = pause == "temporary-return" ? new PausingStorage(PauseAsync) : null;
        var target = new TradeAcceptancePlayer(game, pendingStorage);
        await PlayerTestHelper.EnterWorldAsync(target, PlayerTestHelper.CreateCharacter(game)).ConfigureAwait(false);
        var peer = await PlayerTestHelper.CreatePlayerAsync(game).ConfigureAwait(false);
        target.Attributes![Stats.CurrentHealth] = 10;
        var inventory = target.Inventory!.ItemStorage;
        inventory.Money = 1234;

        Item? returnedItem = null;
        if (pause == "save")
        {
            ((PausingPlayerContext)target.PersistenceContext).BeforeSave = PauseAsync;
        }
        else if (pause == "temporary-return")
        {
            returnedItem = this.GetItem();
            Assert.That(await pendingStorage!.AddItemAsync(0, returnedItem).ConfigureAwait(false), Is.True);
        }
        else
        {
            Mock.Get(target.ViewPlugIns.GetPlugIn<IShowTradeRequestAnswerPlugIn>()!)
                .Setup(v => v.ShowTradeRequestAnswerAsync(true)).Returns(PauseAsync);
        }

        Assert.That(await new TradeRequestAction().RequestTradeAsync(peer, target).ConfigureAwait(false), Is.True);
        var dead = new TaskCompletionSource(TaskCreationOptions.RunContinuationsAsynchronously);
        target.PlayerState.StateChanged += args =>
        {
            if (args.CurrentStateState == PlayerState.Dead)
            {
                dead.TrySetResult();
            }

            return ValueTask.CompletedTask;
        };
        var acceptance = new TradeAcceptAction().HandleTradeAcceptAsync(target, true).AsTask();
        Task? death = null;
        try
        {
            await entered.Task.WaitAsync(TimeSpan.FromSeconds(5)).ConfigureAwait(false);
            death = target.ApplyPoisonDamageAsync(peer, 10).AsTask();
            await dead.Task.WaitAsync(TimeSpan.FromSeconds(5)).ConfigureAwait(false);
            Assert.That(target.TradingPartner, Is.Null);
            Assert.That(peer.TradingPartner, Is.Null);
            release.TrySetResult();
            await Task.WhenAll(acceptance, death).WaitAsync(TimeSpan.FromSeconds(5)).ConfigureAwait(false);
            var staleBackup = target.BackupInventory;
            var stalePeerBackup = peer.BackupInventory;
            await target.RespawnAfterDeathAsync(new ExitGate { Map = target.CurrentMap!.Definition, X1 = 1, X2 = 1, Y1 = 1, Y2 = 1 }).ConfigureAwait(false);
            var gainedItem = this.GetItem();
            Assert.That(await target.Inventory.AddItemAsync(gainedItem).ConfigureAwait(false), Is.True);
            inventory.Money = 9876;
            await target.RemoveFromGameAsync().ConfigureAwait(false);
            Assert.Multiple(() =>
            {
                Assert.That(inventory.Money, Is.EqualTo(9876));
                Assert.That(inventory.Items, Does.Contain(gainedItem));
                Assert.That(staleBackup, Is.Null);
                Assert.That(stalePeerBackup, Is.Null);
                Assert.That(target.BackupInventory, Is.Null);
                if (returnedItem is not null)
                {
                    Assert.That(inventory.Items.Count(i => ReferenceEquals(i, returnedItem)), Is.EqualTo(1));
                }
            });
            Mock.Get(peer.ViewPlugIns.GetPlugIn<IShowTradeRequestAnswerPlugIn>()!)
                .Verify(v => v.ShowTradeRequestAnswerAsync(true), Times.Never);
            Mock.Get(target.ViewPlugIns.GetPlugIn<IShowTradeRequestAnswerPlugIn>()!)
                .Verify(v => v.ShowTradeRequestAnswerAsync(true), pause == "accepted-view" ? Times.Once : Times.Never);
        }
        finally
        {
            release.TrySetResult();
            await acceptance.WaitAsync(TimeSpan.FromSeconds(5)).ConfigureAwait(false);
            if (death is not null)
            {
                await death.WaitAsync(TimeSpan.FromSeconds(5)).ConfigureAwait(false);
            }

            await target.DisposeAsync().ConfigureAwait(false);
            await peer.DisposeAsync().ConfigureAwait(false);
            await seed.DisposeAsync().ConfigureAwait(false);
        }
    }

    /// <summary>
    /// Items left by a previous window return before the trade snapshot, and survive cancellation.
    /// </summary>
    [Test]
    public async Task TemporaryItemsReturnBeforeTradeBackupAsync()
    {
        var target = await PlayerTestHelper.CreatePlayerAsync().ConfigureAwait(false);
        var peer = await PlayerTestHelper.CreatePlayerAsync(target.GameContext).ConfigureAwait(false);
        var item = this.GetItem();
        Assert.That(await target.TemporaryStorage!.AddItemAsync(0, item).ConfigureAwait(false), Is.True);
        Assert.That(await new TradeRequestAction().RequestTradeAsync(peer, target).ConfigureAwait(false), Is.True);
        await new TradeAcceptAction().HandleTradeAcceptAsync(target, true).ConfigureAwait(false);
        Assert.Multiple(() =>
        {
            Assert.That(target.PlayerState.CurrentState, Is.EqualTo(PlayerState.TradeOpened));
            Assert.That(target.TemporaryStorage.Items, Is.Empty);
            Assert.That(target.BackupInventory!.Items, Does.Contain(item));
        });
        await new TradeCancelAction().CancelTradeAsync(target).ConfigureAwait(false);
        var inventory = target.Inventory!.ItemStorage;
        inventory.Money = 4321;
        await target.RemoveFromGameAsync().ConfigureAwait(false);
        Assert.That(inventory.Items.Count(i => ReferenceEquals(i, item)), Is.EqualTo(1));
        Assert.That(inventory.Money, Is.EqualTo(4321));
        await target.DisposeAsync().ConfigureAwait(false);
        await peer.DisposeAsync().ConfigureAwait(false);
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

    private static byte[] CreateTradeRequestPacket(ushort targetId)
    {
        var packet = new byte[MUnique.OpenMU.Network.Packets.ClientToServer.TradeRequestRef.Length];
        var request = new MUnique.OpenMU.Network.Packets.ClientToServer.TradeRequestRef(packet);
        request.PlayerId = targetId;
        return packet;
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

    private sealed class TradeAcceptancePlayer(IGameContext context, IStorage? pendingStorage) : Player(context), ITrader
    {
        IStorage? ITrader.TemporaryStorage => pendingStorage ?? this.TemporaryStorage;

        protected override ICustomPlugInContainer<IViewPlugIn> CreateViewPlugInContainer() => new MockViewPlugInContainer();
    }

    private sealed class PausingContextProvider(GameConfiguration expectedConfiguration) : InMemoryPersistenceContextProvider, IPersistenceContextProvider
    {
        IPlayerContext IPersistenceContextProvider.CreateNewPlayerContext(GameConfiguration configuration)
        {
            Assert.That(configuration, Is.SameAs(expectedConfiguration));
            return new PausingPlayerContext((InMemoryRepositoryProvider)this.RepositoryProvider);
        }
    }

    private sealed class PausingPlayerContext(InMemoryRepositoryProvider provider) : PlayerInMemoryContext(provider), IPlayerContext
    {
        public Func<ValueTask>? BeforeSave { get; set; }

        async ValueTask<bool> IContext.SaveChangesAsync(CancellationToken cancellationToken)
        {
            if (this.BeforeSave is { } beforeSave)
            {
                this.BeforeSave = null;
                await beforeSave().ConfigureAwait(false);
            }

            return await base.SaveChangesAsync(cancellationToken).ConfigureAwait(false);
        }
    }

    private sealed class PausingStorage(Func<ValueTask> pause) : Storage(InventoryConstants.TemporaryStorageSize, new TemporaryItemStorage())
    {
        public override async ValueTask RemoveItemAsync(Item item)
        {
            await base.RemoveItemAsync(item).ConfigureAwait(false);
            await pause().ConfigureAwait(false);
        }
    }

    //// TODO: Test fail scenarios
}
