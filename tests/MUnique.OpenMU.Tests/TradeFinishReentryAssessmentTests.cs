// <copyright file="TradeFinishReentryAssessmentTests.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.Tests;

using System.Collections.ObjectModel;
using System.Diagnostics.Metrics;
using System.Threading;
using Microsoft.Extensions.Logging.Abstractions;
using Moq;
using MUnique.OpenMU.DataModel;
using MUnique.OpenMU.DataModel.Configuration;
using MUnique.OpenMU.DataModel.Configuration.Items;
using MUnique.OpenMU.DataModel.Entities;
using MUnique.OpenMU.GameLogic;
using MUnique.OpenMU.GameLogic.Attributes;
using MUnique.OpenMU.GameLogic.PlayerActions.Items;
using MUnique.OpenMU.GameLogic.PlayerActions.Trade;
using MUnique.OpenMU.GameLogic.Views.Trade;
using MUnique.OpenMU.GameLogic.Views.World;
using MUnique.OpenMU.Persistence;
using MUnique.OpenMU.Persistence.InMemory;
using MUnique.OpenMU.PlugIns;

/// <summary>Review155 causal assessment on unchanged production, with ordinary completion controls.</summary>
[TestFixture]
[NonParallelizable]
public class TradeFinishReentryAssessmentTests
{
    /// <summary>
    /// A peer withdraws after the unlocked confirmation check, before its state exclusion is acquired. Like the packet
    /// order in which the withdrawal came first, nothing is exchanged and the trade stays open; fresh confirmations
    /// complete it with the offer which both then confirmed.
    /// </summary>
    /// <param name="withdraw">Whether the independent peer sends its uncheck packet.</param>
    /// <param name="takeBack">Whether the peer then also takes its offered item back, which resets both confirmations.</param>
    [TestCase(false, false)]
    [TestCase(true, false)]
    [TestCase(true, true)]
    public async Task ConfirmationMustStillHoldAtLockedFinishAsync(bool withdraw, bool takeBack)
    {
        await using var fixture = await Fixture.CreateAsync().ConfigureAwait(false);
        var a = fixture.Players.OrderBy(p => p.PlayerState.TransitionLockId).First();
        var b = fixture.Players.Single(p => !ReferenceEquals(p, a));
        var aMoney = a.Money;
        var bMoney = b.Money;
        var x = await AddItemAsync(a, 20).ConfigureAwait(false);
        var y = await AddItemAsync(b, 20).ConfigureAwait(false);
        await OpenAsync(a, b).ConfigureAwait(false);
        await OfferAsync(a, 20, 60).ConfigureAwait(false);
        await OfferAsync(b, 20, 25).ConfigureAwait(false);
        await ButtonAsync(b, TradeButtonState.Checked).ConfigureAwait(false);
        var aFinished = Mock.Get(a.ViewPlugIns.GetPlugIn<ITradeFinishedPlugIn>()!);
        var bFinished = Mock.Get(b.ViewPlugIns.GetPlugIn<ITradeFinishedPlugIn>()!);
        var bButton = Mock.Get(b.ViewPlugIns.GetPlugIn<IChangeTradeButtonStatePlugIn>()!);
        bButton.Invocations.Clear();
        var entered = Signal();
        var release = Signal();
        var armed = 1;
        a.PlayerState.StateChanges += async change =>
        {
            if (change.NextState == PlayerState.EnteredWorld && Interlocked.Exchange(ref armed, 0) == 1)
            {
                entered.TrySetResult();
                await release.Task.ConfigureAwait(false);
            }
        };
        var finish = ButtonAsync(a, TradeButtonState.Checked);
        var peerPacket = Task.CompletedTask;
        try
        {
            await entered.Task.WaitAsync(TimeSpan.FromSeconds(5)).ConfigureAwait(false);
            if (withdraw)
            {
                await ButtonAsync(b, TradeButtonState.Unchecked).WaitAsync(TimeSpan.FromSeconds(5)).ConfigureAwait(false);
                Assert.That(b.PlayerState.CurrentState, Is.EqualTo(PlayerState.TradeOpened));
            }

            if (takeBack)
            {
                // Same B packet funnel. Resetting A's confirmation needs A's state exclusion, which the finish holds.
                peerPacket = RunAsync(b, () => new MoveItemAction().MoveItemAsync(b, 0, Storages.Trade, 20, Storages.Inventory));
            }

            release.TrySetResult();
            await Task.WhenAll(finish, peerPacket).WaitAsync(TimeSpan.FromSeconds(5)).ConfigureAwait(false);
            AssertConserved(fixture.Players, x, y);
            if (!withdraw)
            {
                AssertFree(a);
                AssertFree(b);
                Assert.That(a.Inventory!.Items, Does.Contain(y));
                Assert.That(b.Inventory!.Items, Does.Contain(x));
                Assert.That(a.Money, Is.EqualTo(aMoney - 35));
                Assert.That(b.Money, Is.EqualTo(bMoney + 35));
                aFinished.Verify(v => v.TradeFinishedAsync(TradeResult.Success), Times.Once);
                bFinished.Verify(v => v.TradeFinishedAsync(TradeResult.Success), Times.Once);
                return;
            }

            // Nothing was exchanged and the trade is still open with the withdrawn confirmation.
            Assert.Multiple(() =>
            {
                Assert.That(a.Inventory!.Items, Does.Not.Contain(y), "Unconfirmed exchange after peer withdrew before its lock.");
                Assert.That(b.Inventory!.Items, Does.Not.Contain(x));
                Assert.That(a.TemporaryStorage!.Items, Does.Contain(x));
                Assert.That(takeBack ? b.Inventory!.Items : b.TemporaryStorage!.Items, Does.Contain(y));
                Assert.That(a.PlayerState.CurrentState, Is.EqualTo(takeBack ? PlayerState.TradeOpened : PlayerState.TradeButtonPressed));
                Assert.That(b.PlayerState.CurrentState, Is.EqualTo(PlayerState.TradeOpened));
                Assert.That(a.TradingPartner, Is.SameAs(b));
                Assert.That(b.TradingPartner, Is.SameAs(a));
                Assert.That(a.BackupInventory, Is.Not.Null);
                Assert.That(b.BackupInventory, Is.Not.Null);
                Assert.That(a.TradingMoney, Is.EqualTo(60));
                Assert.That(b.TradingMoney, Is.EqualTo(25));
                Assert.That(a.Money, Is.EqualTo(aMoney - 60));
                Assert.That(b.Money, Is.EqualTo(bMoney - 25));
            });
            aFinished.Verify(v => v.TradeFinishedAsync(It.IsAny<TradeResult>()), Times.Never);
            bFinished.Verify(v => v.TradeFinishedAsync(It.IsAny<TradeResult>()), Times.Never);

            if (!takeBack)
            {
                // As in the order where the withdrawal came first, B learns that A still confirms.
                bButton.Verify(v => v.ChangeTradeButtonStateAsync(TradeButtonState.Checked), Times.Once);
            }

            if (takeBack)
            {
                await ButtonAsync(a, TradeButtonState.Checked).ConfigureAwait(false);
            }

            await ButtonAsync(b, TradeButtonState.Checked).ConfigureAwait(false);
            AssertFree(a);
            AssertFree(b);
            Assert.That(b.Inventory!.Items, Does.Contain(x));
            Assert.That(a.Inventory!.Items, takeBack ? Does.Not.Contain(y) : Does.Contain(y));
            Assert.That(b.Inventory!.Items, takeBack ? Does.Contain(y) : Does.Not.Contain(y));
            Assert.That(a.Money, Is.EqualTo(aMoney - 35));
            Assert.That(b.Money, Is.EqualTo(bMoney + 35));
            aFinished.Verify(v => v.TradeFinishedAsync(TradeResult.Success), Times.Once);
            bFinished.Verify(v => v.TradeFinishedAsync(TradeResult.Success), Times.Once);
            aFinished.Verify(v => v.TradeFinishedAsync(TradeResult.Cancelled), Times.Never);
            bFinished.Verify(v => v.TradeFinishedAsync(TradeResult.Cancelled), Times.Never);
            AssertConserved(fixture.Players, x, y);
        }
        finally
        {
            release.TrySetResult();
            await Task.WhenAll(finish, peerPacket).WaitAsync(TimeSpan.FromSeconds(5)).ConfigureAwait(false);
        }
    }

    /// <summary>Partial transfer cannot be committed by rechecking while the first packet is about to roll back.</summary>
    /// <param name="reenter">Whether the peer attempts uncheck, inventory rearrangement and recheck.</param>
    /// <param name="reverse">Whether the other fixture player initiates the failed finish.</param>
    [TestCase(false, false)]
    [TestCase(true, false)]
    [TestCase(false, true)]
    [TestCase(true, true)]
    public async Task FailedFitMustNotCommitHalfAppliedTransferAsync(bool reenter, bool reverse)
    {
        await using var fixture = await Fixture.CreateAsync().ConfigureAwait(false);
        // Establish one stable order before choosing which peer initiates.
        Assert.That(fixture.Players[0].PlayerState.TransitionLockId, Is.LessThan(fixture.Players[1].PlayerState.TransitionLockId));
        var (a, b) = reverse ? (fixture.Players[1], fixture.Players[0]) : (fixture.Players[0], fixture.Players[1]);
        var aMoney = a.Money;
        var bMoney = b.Money;
        var x = await AddItemAsync(a, 20, 2).ConfigureAwait(false);
        Item? y = null;
        for (byte slot = InventoryConstants.EquippableSlotsCount; slot < InventoryConstants.FirstExtensionItemSlotIndex; slot++)
        {
            if (slot == 22)
            {
                continue;
            }

            var item = await AddItemAsync(b, slot).ConfigureAwait(false);
            if (slot == 20)
            {
                y = item;
            }
        }

        Assert.That(y, Is.Not.Null);
        var originalBItems = b.Inventory!.Items.ToArray();
        await OpenAsync(a, b).ConfigureAwait(false);
        await OfferAsync(a, 20, 60).ConfigureAwait(false);
        await OfferAsync(b, 20, 25).ConfigureAwait(false);
        await ButtonAsync(b, TradeButtonState.Checked).ConfigureAwait(false);
        var entered = Signal();
        using var release = new ManualResetEventSlim();
        var armed = 1;
        using var listener = new MeterListener();
        listener.InstrumentPublished = (instrument, observer) =>
        {
            if (instrument.Name == "CancelledTradesCount" && instrument.Meter.Name == BaseTradeAction.MeterName)
            {
                observer.EnableMeasurementEvents(instrument);
            }
        };
        listener.SetMeasurementEventCallback<int>((_, _, _, _) =>
        {
            if (Interlocked.Exchange(ref armed, 0) == 1)
            {
                entered.TrySetResult();
                Assert.That(release.Wait(TimeSpan.FromSeconds(10)), Is.True, "Actual first rollback counter, before either owned restore.");
            }
        });
        listener.Start();
        var finish = ButtonAsync(a, TradeButtonState.Checked);
        var peerPacket = Task.CompletedTask;
        try
        {
            await entered.Task.WaitAsync(TimeSpan.FromSeconds(5)).ConfigureAwait(false);
            Assert.Multiple(() =>
            {
                Assert.That(a.Inventory!.Items, Does.Contain(y));
                Assert.That(b.TemporaryStorage!.Items, Is.Empty);
                Assert.That(a.TemporaryStorage!.Items, Does.Contain(x));
                Assert.That(a.PlayerState.CurrentState, Is.EqualTo(PlayerState.TradeButtonPressed));
                Assert.That(b.PlayerState.CurrentState, Is.EqualTo(PlayerState.TradeButtonPressed));
                Assert.That(a.TradingMoney, Is.EqualTo(60));
                Assert.That(b.TradingMoney, Is.EqualTo(25));
            });
            var blocked = false;
            if (reenter)
            {
                var attempted = new TaskCompletionSource<bool>(TaskCreationOptions.RunContinuationsAsynchronously);
                peerPacket = RunAsync(b, async () =>
                {
                    // Same B packet funnel. The first action must queue at the held state exclusion.
                    var uncheck = new TradeButtonAction().TradeButtonChangedAsync(b, TradeButtonState.Unchecked);
                    attempted.TrySetResult(!uncheck.IsCompleted);
                    await uncheck.ConfigureAwait(false);
                    await new MoveItemAction().MoveItemAsync(b, 21, Storages.Inventory, 22, Storages.Inventory).ConfigureAwait(false);
                    await new TradeButtonAction().TradeButtonChangedAsync(b, TradeButtonState.Checked).ConfigureAwait(false);
                });
                blocked = await attempted.Task.WaitAsync(TimeSpan.FromSeconds(5)).ConfigureAwait(false);
                if (!blocked)
                {
                    // Exact unchanged baseline permits all three packets before rollback; retain its causal schedule.
                    // Fixed code cannot reach this phase until rollback releases both exclusions.
                    await peerPacket.WaitAsync(TimeSpan.FromSeconds(5)).ConfigureAwait(false);
                }
            }

            release.Set();
            await Task.WhenAll(finish, peerPacket).WaitAsync(TimeSpan.FromSeconds(5)).ConfigureAwait(false);
            TestContext.Out.WriteLine($"reenter={reenter}; A money={a.Money}; B money={b.Money}; Y detach from B={((RecordingContext)b.PersistenceContext).Detached.Contains(y!)}; Y attach to A={((RecordingContext)a.PersistenceContext).Transferred.Contains(y!)}; X detach from A={((RecordingContext)a.PersistenceContext).Detached.Contains(x)}; X attach to B={((RecordingContext)b.PersistenceContext).Transferred.Contains(x)}");
            AssertConserved(fixture.Players, x, y!);
            AssertFree(a);
            AssertFree(b);
            Assert.Multiple(() =>
            {
                if (reenter)
                {
                    Assert.That(blocked, Is.True, "Peer uncheck must queue until both rollback halves have restored resources.");
                }

                Assert.That(a.Inventory!.Items, Does.Contain(x), "Failed fit must roll back original owner, not permit a second finish on partial state.");
                Assert.That(b.Inventory!.Items, Is.EquivalentTo(originalBItems));
                Assert.That(a.Money, Is.EqualTo(aMoney));
                Assert.That(b.Money, Is.EqualTo(bMoney));
                Assert.That(((RecordingContext)b.PersistenceContext).Detached, Does.Not.Contain(y));
                Assert.That(((RecordingContext)a.PersistenceContext).Transferred, Does.Not.Contain(y));
                Mock.Get(a.ViewPlugIns.GetPlugIn<ITradeFinishedPlugIn>()!).Verify(v => v.TradeFinishedAsync(TradeResult.FailedByFullInventory), Times.Once);
                Mock.Get(b.ViewPlugIns.GetPlugIn<ITradeFinishedPlugIn>()!).Verify(v => v.TradeFinishedAsync(TradeResult.FailedByFullInventory), Times.Once);
                Mock.Get(a.ViewPlugIns.GetPlugIn<ITradeFinishedPlugIn>()!).Verify(v => v.TradeFinishedAsync(TradeResult.Success), Times.Never);
                Mock.Get(b.ViewPlugIns.GetPlugIn<ITradeFinishedPlugIn>()!).Verify(v => v.TradeFinishedAsync(TradeResult.Success), Times.Never);
            });
            // Both packet funnels are free. Rearrange for an ordinary fresh retry, never a finish on partial state.
            if (!reenter)
            {
                await RunAsync(b, () => new MoveItemAction().MoveItemAsync(b, 21, Storages.Inventory, 22, Storages.Inventory)).ConfigureAwait(false);
            }

            await OpenAsync(a, b).ConfigureAwait(false);
            await OfferAsync(a, x.ItemSlot, 60).ConfigureAwait(false);
            await OfferAsync(b, y!.ItemSlot, 25).ConfigureAwait(false);
            await ButtonAsync(b, TradeButtonState.Checked).ConfigureAwait(false);
            await ButtonAsync(a, TradeButtonState.Checked).ConfigureAwait(false);
            AssertFree(a);
            AssertFree(b);
            Assert.That(a.Inventory!.Items, Does.Contain(y));
            Assert.That(b.Inventory!.Items, Does.Contain(x));
            Assert.That(a.Money, Is.EqualTo(aMoney - 35));
            Assert.That(b.Money, Is.EqualTo(bMoney + 35));
            Assert.That(((RecordingContext)b.PersistenceContext).Detached, Does.Contain(y));
            Assert.That(((RecordingContext)a.PersistenceContext).Transferred, Does.Contain(y));
            AssertConserved(fixture.Players, x, y);
        }
        finally
        {
            release.Set();
            await Task.WhenAll(finish, peerPacket).WaitAsync(TimeSpan.FromSeconds(5)).ConfigureAwait(false);
        }
    }

    /// <summary>
    /// A storage fault after the first half of the item exchange already happened must not leave both traders in a
    /// half-exchanged trade without a result: both halves are restored from their own backups and both traders get
    /// the cancellation, while the fault itself stays observable to the packet handler.
    /// </summary>
    /// <param name="fault">Whether the receiving storage of the second half fails once.</param>
    [TestCase(false)]
    [TestCase(true)]
    public async Task StorageFaultDuringExchangeRestoresBothHalvesAsync(bool fault)
    {
        var bItems = new FailingOnceItems();
        await using var fixture = await Fixture.CreateAsync(bItems).ConfigureAwait(false);
        var a = fixture.Players[0];
        var b = fixture.Players[1];
        var aMoney = a.Money;
        var bMoney = b.Money;
        var x = await AddItemAsync(a, 20).ConfigureAwait(false);
        var y = await AddItemAsync(b, 20).ConfigureAwait(false);
        await OpenAsync(a, b).ConfigureAwait(false);
        await OfferAsync(a, 20, 60).ConfigureAwait(false);
        await OfferAsync(b, 20, 25).ConfigureAwait(false);
        var aStorage = a.SelectedCharacter!.Inventory!;
        var bStorage = b.SelectedCharacter!.Inventory!;
        Assert.That(bStorage.Items, Is.SameAs(bItems));
        var aFinished = Mock.Get(a.ViewPlugIns.GetPlugIn<ITradeFinishedPlugIn>()!);
        var bFinished = Mock.Get(b.ViewPlugIns.GetPlugIn<ITradeFinishedPlugIn>()!);
        await ButtonAsync(b, TradeButtonState.Checked).ConfigureAwait(false);
        bItems.Armed = fault;
        var finish = ButtonAsync(a, TradeButtonState.Checked);
        if (fault)
        {
            var error = Assert.ThrowsAsync<InvalidOperationException>(async () => await finish.WaitAsync(TimeSpan.FromSeconds(5)).ConfigureAwait(false));
            Assert.That(error!.Message, Is.EqualTo(FailingOnceItems.Message));
            Assert.That(bItems.Faults, Is.EqualTo(1), "The first half (y into A's inventory) was applied before the fault.");
        }
        else
        {
            await finish.WaitAsync(TimeSpan.FromSeconds(5)).ConfigureAwait(false);
        }

        AssertFree(a);
        AssertFree(b);
        AssertConserved(fixture.Players, x, y);
        Assert.Multiple(() =>
        {
            Assert.That(a.Inventory!.Items, Does.Contain(fault ? x : y));
            Assert.That(b.Inventory!.Items, Does.Contain(fault ? y : x));
            Assert.That(a.Money, Is.EqualTo(fault ? aMoney : aMoney - 35));
            Assert.That(b.Money, Is.EqualTo(fault ? bMoney : bMoney + 35));

            // The persisted item collections agree with the live inventories.
            Assert.That(aStorage.Items, Is.EquivalentTo(a.Inventory.Items));
            Assert.That(bStorage.Items, Is.EquivalentTo(b.Inventory.Items));
        });
        var expected = fault ? TradeResult.Cancelled : TradeResult.Success;
        aFinished.Verify(v => v.TradeFinishedAsync(expected), Times.Once);
        bFinished.Verify(v => v.TradeFinishedAsync(expected), Times.Once);
        aFinished.Verify(v => v.TradeFinishedAsync(It.IsAny<TradeResult>()), Times.Once);
        bFinished.Verify(v => v.TradeFinishedAsync(It.IsAny<TradeResult>()), Times.Once);

        if (fault)
        {
            // Both are free for an ordinary new trade, which then completes.
            await OpenAsync(a, b).ConfigureAwait(false);
            await OfferAsync(a, x.ItemSlot, 60).ConfigureAwait(false);
            await OfferAsync(b, y.ItemSlot, 25).ConfigureAwait(false);
            await ButtonAsync(b, TradeButtonState.Checked).ConfigureAwait(false);
            await ButtonAsync(a, TradeButtonState.Checked).ConfigureAwait(false);
            AssertFree(a);
            AssertFree(b);
            Assert.That(a.Inventory!.Items, Does.Contain(y));
            Assert.That(b.Inventory!.Items, Does.Contain(x));
            Assert.That(a.Money, Is.EqualTo(aMoney - 35));
            Assert.That(b.Money, Is.EqualTo(bMoney + 35));
            Assert.That(bStorage.Items, Is.EquivalentTo(b.Inventory.Items));
            AssertConserved(fixture.Players, x, y);
        }
    }

    /// <summary>
    /// A state-changing veto against leaving the trade keeps the vetoed half in its trade with its own escrow, while the
    /// other half is restored and released. Once the veto permits leaving, the vetoed trader's ordinary cancel
    /// restores its own escrow, without touching the released partner.
    /// </summary>
    /// <param name="veto">Whether the finisher's transition back to the world is vetoed.</param>
    [TestCase(false)]
    [TestCase(true)]
    public async Task VetoedHalfKeepsItsEscrowUntilItsCancelIsAllowedAsync(bool veto)
    {
        await using var fixture = await Fixture.CreateAsync().ConfigureAwait(false);
        var a = fixture.Players[0];
        var b = fixture.Players[1];
        var aMoney = a.Money;
        var bMoney = b.Money;
        var x = await AddItemAsync(a, 20).ConfigureAwait(false);
        var y = await AddItemAsync(b, 20).ConfigureAwait(false);
        await OpenAsync(a, b).ConfigureAwait(false);
        await OfferAsync(a, 20, 60).ConfigureAwait(false);
        await OfferAsync(b, 20, 25).ConfigureAwait(false);
        await ButtonAsync(b, TradeButtonState.Checked).ConfigureAwait(false);
        var vetoing = veto;
        a.PlayerState.StateChanges += change =>
        {
            if (Volatile.Read(ref vetoing) && change.NextState == PlayerState.EnteredWorld)
            {
                change.Cancel = true;
            }

            return ValueTask.CompletedTask;
        };
        var aFinished = Mock.Get(a.ViewPlugIns.GetPlugIn<ITradeFinishedPlugIn>()!);
        var bFinished = Mock.Get(b.ViewPlugIns.GetPlugIn<ITradeFinishedPlugIn>()!);
        await ButtonAsync(a, TradeButtonState.Checked).WaitAsync(TimeSpan.FromSeconds(5)).ConfigureAwait(false);
        AssertConserved(fixture.Players, x, y);
        AssertFree(b);
        if (!veto)
        {
            AssertFree(a);
            Assert.That(a.Inventory!.Items, Does.Contain(y));
            Assert.That(b.Inventory!.Items, Does.Contain(x));
            aFinished.Verify(v => v.TradeFinishedAsync(TradeResult.Success), Times.Once);
            bFinished.Verify(v => v.TradeFinishedAsync(TradeResult.Success), Times.Once);
            return;
        }

        Assert.Multiple(() =>
        {
            Assert.That(b.Inventory!.Items, Does.Contain(y));
            Assert.That(b.Money, Is.EqualTo(bMoney));
            Assert.That(a.PlayerState.CurrentState, Is.EqualTo(PlayerState.TradeButtonPressed));
            Assert.That(a.TradingPartner, Is.SameAs(b));
            Assert.That(a.TemporaryStorage!.Items, Does.Contain(x));
            Assert.That(a.TradingMoney, Is.EqualTo(60));
            Assert.That(a.BackupInventory, Is.Not.Null);
        });
        bFinished.Verify(v => v.TradeFinishedAsync(TradeResult.Cancelled), Times.Once);
        aFinished.Verify(v => v.TradeFinishedAsync(It.IsAny<TradeResult>()), Times.Never);

        // While vetoed, the cancel can't leave the trade either; afterwards it restores only A's own escrow.
        await RunAsync(a, () => new TradeCancelAction().CancelTradeAsync(a)).WaitAsync(TimeSpan.FromSeconds(5)).ConfigureAwait(false);
        Assert.That(a.PlayerState.CurrentState, Is.EqualTo(PlayerState.TradeButtonPressed));
        Assert.That(a.TemporaryStorage!.Items, Does.Contain(x));
        Volatile.Write(ref vetoing, false);
        await RunAsync(a, () => new TradeCancelAction().CancelTradeAsync(a)).WaitAsync(TimeSpan.FromSeconds(5)).ConfigureAwait(false);
        AssertFree(a);
        AssertFree(b);
        Assert.That(a.Inventory!.Items, Does.Contain(x));
        Assert.That(a.Money, Is.EqualTo(aMoney));
        Assert.That(b.Money, Is.EqualTo(bMoney));
        aFinished.Verify(v => v.TradeFinishedAsync(TradeResult.Cancelled), Times.Once);
        bFinished.Verify(v => v.TradeFinishedAsync(TradeResult.Cancelled), Times.Once);
        AssertConserved(fixture.Players, x, y);
    }

    /// <summary>
    /// A lethal hit which is already reserved, but whose death didn't close the trade yet, makes that trader
    /// unavailable: the confirmed trade isn't exchanged, but cancelled and both traders are restored at once, without
    /// depending on the other flow to close it. The death then finds no trade left. A nonlethal hit at the same point
    /// doesn't prevent the exchange.
    /// </summary>
    /// <param name="lethal">Whether the paused hit is lethal.</param>
    /// <param name="finisherDies">Whether the hit trader is the finisher (otherwise its already confirmed partner).</param>
    [TestCase(false, false)]
    [TestCase(true, false)]
    [TestCase(true, true)]
    public async Task ReservedDeathPreventsTradeExchangeAsync(bool lethal, bool finisherDies)
    {
        await using var fixture = await Fixture.CreateAsync().ConfigureAwait(false);
        var a = fixture.Players[0];
        var b = fixture.Players[1];
        var dying = finisherDies ? a : b;
        var attacker = await PlayerTestHelper.CreatePlayerAsync(fixture.Game).ConfigureAwait(false);
        var release = Signal();
        var hit = Task.CompletedTask;
        try
        {
            var aMoney = a.Money;
            var bMoney = b.Money;
            var x = await AddItemAsync(a, 20).ConfigureAwait(false);
            var y = await AddItemAsync(b, 20).ConfigureAwait(false);
            await OpenAsync(a, b).ConfigureAwait(false);
            await OfferAsync(a, 20, 60).ConfigureAwait(false);
            await OfferAsync(b, 20, 25).ConfigureAwait(false);
            await ButtonAsync(b, TradeButtonState.Checked).ConfigureAwait(false);
            dying.CurrentMap!.Definition.ExitGates.Add(new ExitGate { Map = dying.CurrentMap.Definition, IsSpawnGate = true, X1 = 1, X2 = 1, Y1 = 1, Y2 = 1 });
            dying.Attributes![Stats.CurrentHealth] = lethal ? 10 : 100;
            var entered = Signal();
            var holds = 0;
            Mock.Get(dying.ViewPlugIns.GetPlugIn<IShowHitPlugIn>()!).Setup(v => v.ShowHitAsync(dying, It.IsAny<HitInfo>())).Returns(async () =>
            {
                if (Interlocked.Increment(ref holds) == 1)
                {
                    entered.TrySetResult();
                    await release.Task.ConfigureAwait(false);
                }
            });
            var aFinished = Mock.Get(a.ViewPlugIns.GetPlugIn<ITradeFinishedPlugIn>()!);
            var bFinished = Mock.Get(b.ViewPlugIns.GetPlugIn<ITradeFinishedPlugIn>()!);

            // The attacker uses only its own packet funnel; the finish uses A's funnel.
            hit = Task.Run(async () => await attacker.RunPersistenceExclusiveAsync(() => dying.ApplyPoisonDamageAsync(attacker, 10)).ConfigureAwait(false));
            await entered.Task.WaitAsync(TimeSpan.FromSeconds(5)).ConfigureAwait(false);
            Assert.That(dying.IsAlive, Is.EqualTo(!lethal));
            await ButtonAsync(a, TradeButtonState.Checked).WaitAsync(TimeSpan.FromSeconds(5)).ConfigureAwait(false);
            if (!lethal)
            {
                AssertFree(a);
                AssertFree(b);
                Assert.That(a.Inventory!.Items, Does.Contain(y));
                Assert.That(b.Inventory!.Items, Does.Contain(x));
                aFinished.Verify(v => v.TradeFinishedAsync(TradeResult.Success), Times.Once);
                bFinished.Verify(v => v.TradeFinishedAsync(TradeResult.Success), Times.Once);
            }
            else
            {
                Assert.Multiple(() =>
                {
                    Assert.That(a.Inventory!.Items, Does.Not.Contain(y), "A trader with a reserved death exchanged items.");
                    Assert.That(b.Inventory!.Items, Does.Not.Contain(x));
                    Assert.That(a.Inventory!.Items, Does.Contain(x));
                    Assert.That(b.Inventory!.Items, Does.Contain(y));
                    Assert.That(a.TradingPartner, Is.Null);
                    Assert.That(b.TradingPartner, Is.Null);
                    Assert.That(a.Money, Is.EqualTo(aMoney));
                    Assert.That(b.Money, Is.EqualTo(bMoney));
                    Assert.That(dying.PlayerState.CurrentState, Is.EqualTo(PlayerState.EnteredWorld), "The reserved death hasn't reached its state yet.");
                });
                aFinished.Verify(v => v.TradeFinishedAsync(TradeResult.Cancelled), Times.Once);
                bFinished.Verify(v => v.TradeFinishedAsync(TradeResult.Cancelled), Times.Once);
            }

            release.TrySetResult();
            await hit.WaitAsync(TimeSpan.FromSeconds(5)).ConfigureAwait(false);
            AssertConserved(fixture.Players, x, y);
            if (!lethal)
            {
                Assert.That(dying.IsAlive, Is.True);
                Assert.That(dying.PlayerState.CurrentState, Is.EqualTo(PlayerState.EnteredWorld));
                return;
            }

            // The death found no trade left to close; nothing was restored twice.
            var survivor = finisherDies ? b : a;
            Assert.Multiple(() =>
            {
                Assert.That(dying.PlayerState.CurrentState, Is.EqualTo(PlayerState.Dead));
                Assert.That(survivor.PlayerState.CurrentState, Is.EqualTo(PlayerState.EnteredWorld));
                Assert.That(a.TradingPartner, Is.Null);
                Assert.That(b.TradingPartner, Is.Null);
                Assert.That(a.BackupInventory, Is.Null);
                Assert.That(b.BackupInventory, Is.Null);
                Assert.That(a.Inventory!.Items, Does.Contain(x));
                Assert.That(b.Inventory!.Items, Does.Contain(y));
                Assert.That(a.Money, Is.EqualTo(aMoney));
                Assert.That(b.Money, Is.EqualTo(bMoney));
            });
            aFinished.Verify(v => v.TradeFinishedAsync(TradeResult.Cancelled), Times.Once);
            bFinished.Verify(v => v.TradeFinishedAsync(TradeResult.Cancelled), Times.Once);
            aFinished.Verify(v => v.TradeFinishedAsync(TradeResult.Success), Times.Never);
            bFinished.Verify(v => v.TradeFinishedAsync(TradeResult.Success), Times.Never);
        }
        finally
        {
            release.TrySetResult();
            try
            {
                await hit.WaitAsync(TimeSpan.FromSeconds(5)).ConfigureAwait(false);
            }
            finally
            {
                await attacker.DisposeAsync().ConfigureAwait(false);
            }
        }
    }

    private static TaskCompletionSource Signal() => new(TaskCreationOptions.RunContinuationsAsynchronously);

    private static Task RunAsync(Player player, Func<ValueTask> action) => Task.Run(async () => await player.RunPersistenceExclusiveAsync(action).ConfigureAwait(false));

    private static Task ButtonAsync(Player player, TradeButtonState state) => RunAsync(player, () => new TradeButtonAction().TradeButtonChangedAsync(player, state));

    private static async Task OpenAsync(Player a, Player b)
    {
        Assert.That(await a.RunPersistenceExclusiveAsync(() => new TradeRequestAction().RequestTradeAsync(a, b)).ConfigureAwait(false), Is.True);
        await RunAsync(b, () => new TradeAcceptAction().HandleTradeAcceptAsync(b, true)).ConfigureAwait(false);
        Assert.That(a.PlayerState.CurrentState, Is.EqualTo(PlayerState.TradeOpened));
        Assert.That(b.PlayerState.CurrentState, Is.EqualTo(PlayerState.TradeOpened));
    }

    private static async Task OfferAsync(Player player, byte slot, uint money)
    {
        await RunAsync(player, async () =>
        {
            await new MoveItemAction().MoveItemAsync(player, slot, Storages.Inventory, 0, Storages.Trade).ConfigureAwait(false);
            await new TradeMoneyAction().TradeMoneyAsync(player, money).ConfigureAwait(false);
        }).ConfigureAwait(false);
    }

    private static async Task<Item> AddItemAsync(Player player, byte slot, byte width = 1)
    {
        var item = player.PersistenceContext.CreateNew<Item>();
        item.Definition = new ItemDefinition { Width = width, Height = 1, Durability = 9, IsTradable = true };
        item.Level = 3;
        item.Durability = 7;
        Assert.That(await player.Inventory!.AddItemAsync(slot, item).ConfigureAwait(false), Is.True);
        return item;
    }

    private static void AssertFree(Player player)
    {
        Assert.That(player.PlayerState.CurrentState, Is.EqualTo(PlayerState.EnteredWorld));
        Assert.That(player.TradingPartner, Is.Null);
        Assert.That(player.BackupInventory, Is.Null);
    }

    private static void AssertConserved(Player[] players, params Item[] items)
    {
        Assert.That(players.Sum(p => p.Money + (p.BackupInventory is null ? 0 : p.TradingMoney)), Is.EqualTo(500));
        foreach (var item in items)
        {
            Assert.That(item.GetId(), Is.Not.EqualTo(Guid.Empty));
            Assert.That(players.Sum(p => p.Inventory!.Items.Count(i => i.GetId() == item.GetId()) + p.TemporaryStorage!.Items.Count(i => i.GetId() == item.GetId())), Is.EqualTo(1));
            Assert.That(item.Level, Is.EqualTo(3));
            Assert.That(item.Durability, Is.EqualTo(7));
        }
    }

    private sealed class RecordingProvider(GameConfiguration configuration) : InMemoryPersistenceContextProvider, IPersistenceContextProvider
    {
        IPlayerContext IPersistenceContextProvider.CreateNewPlayerContext(GameConfiguration actualConfiguration)
        {
            Assert.That(actualConfiguration, Is.SameAs(configuration));
            return new RecordingContext((InMemoryRepositoryProvider)this.RepositoryProvider);
        }
    }

    private sealed class RecordingContext(InMemoryRepositoryProvider provider) : PlayerInMemoryContext(provider), IPlayerContext
    {
        public List<object> Detached { get; } = new();

        public List<object> Transferred { get; } = new();

        bool IContext.Detach(object item)
        {
            this.Detached.Add(item);
            return base.Detach(item);
        }

        void IContext.AttachWithoutParent(object item)
        {
            this.Transferred.Add(item);
            base.Attach(item);
        }
    }

    /// <summary>The persisted item collection of a storage, which fails on its first addition once armed.</summary>
    private sealed class FailingOnceItems : Collection<Item>
    {
        public const string Message = "Injected storage fault.";

        public bool Armed { get; set; }

        public int Faults { get; private set; }

        protected override void InsertItem(int index, Item item)
        {
            if (this.Armed)
            {
                this.Armed = false;
                this.Faults++;
                throw new InvalidOperationException(Message);
            }

            base.InsertItem(index, item);
        }
    }

    private sealed class Fixture(GameContext game, Player[] players) : IAsyncDisposable
    {
        public Player[] Players { get; } = players;

        public GameContext Game => game;

        public static async Task<Fixture> CreateAsync(ICollection<Item>? secondInventory = null)
        {
            var configuration = PlayerTestHelper.CreateGameConfiguration();
            configuration.MaximumInventoryMoney = 1_000_000;
            var factory = NullLoggerFactory.Instance;
            var initializer = new MapInitializer(configuration, NullLogger<MapInitializer>.Instance, NullDropGenerator.Instance, null);
            var game = new GameContext(configuration, new RecordingProvider(configuration), initializer, factory, new PlugInManager(null, factory, null, null), NullDropGenerator.Instance, new ConfigurationChangeMediator());
            initializer.PlugInManager = game.PlugInManager;
            initializer.PathFinderPool = game.PathFinderPool;
            var players = new Player[2];
            for (var i = 0; i < players.Length; i++)
            {
                var character = PlayerTestHelper.CreateCharacter(game);
                if (i == 1 && secondInventory is not null)
                {
                    Mock.Get(character.Inventory!).Setup(storage => storage.Items).Returns(secondInventory);
                }

                players[i] = await PlayerTestHelper.CreatePlayerAsync(game, character).ConfigureAwait(false);
                players[i].Money = 200 + (i * 100);
            }

            return new Fixture(game, players);
        }

        public async ValueTask DisposeAsync()
        {
            foreach (var player in this.Players)
            {
                if (player.TradingPartner is not null)
                {
                    await RunAsync(player, () => new TradeCancelAction().CancelTradeAsync(player)).WaitAsync(TimeSpan.FromSeconds(5)).ConfigureAwait(false);
                }
            }

            foreach (var player in this.Players)
            {
                await player.DisposeAsync().ConfigureAwait(false);
            }

            game.Dispose();
        }
    }
}
