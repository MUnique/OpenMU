// <copyright file="TradeContinuationAssessmentTests.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.Tests;

using System.Diagnostics.Metrics;
using System.Threading;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Logging.Abstractions;
using Moq;
using MUnique.OpenMU.DataModel;
using MUnique.OpenMU.DataModel.Configuration;
using MUnique.OpenMU.DataModel.Configuration.Items;
using MUnique.OpenMU.DataModel.Entities;
using MUnique.OpenMU.GameLogic;
using MUnique.OpenMU.GameLogic.PlayerActions;
using MUnique.OpenMU.GameLogic.PlayerActions.Character;
using MUnique.OpenMU.GameLogic.PlayerActions.Items;
using MUnique.OpenMU.GameLogic.PlayerActions.Trade;
using MUnique.OpenMU.GameLogic.Views.Login;
using MUnique.OpenMU.GameLogic.Views.Trade;
using MUnique.OpenMU.Persistence;
using MUnique.OpenMU.Persistence.InMemory;
using MUnique.OpenMU.PlugIns;

/// <summary>Review152 assessment: production funnels, actual failure boundaries and successor conservation.</summary>
[TestFixture]
public class TradeContinuationAssessmentTests
{
    /// <summary>A peer packet queues during its progress save, but can establish a successor after the save throws.</summary>
    /// <param name="successor">Whether the queued peer packet cancels and then starts a new real trade.</param>
    [TestCase(false)]
    [TestCase(true)]
    public async Task OpeningSaveFailurePreservesSuccessorAsync(bool successor)
    {
        var saveEntered = Signal();
        var saveRelease = Signal();
        var catchEntered = Signal();
        using var catchRelease = new ManualResetEventSlim();
        var failure = new InvalidOperationException("Review152 peer progress save failure.");
        var logger = new Mock<ILogger>();
        logger.Setup(l => l.IsEnabled(It.IsAny<LogLevel>())).Returns(true);
        logger.Setup(l => l.Log(It.IsAny<LogLevel>(), It.IsAny<EventId>(), It.IsAny<It.IsAnyType>(), It.IsAny<Exception?>(), It.IsAny<Func<It.IsAnyType, Exception?, string>>()))
            .Callback(new InvocationAction(invocation =>
            {
                if (ReferenceEquals(invocation.Arguments[3], failure))
                {
                    catchEntered.TrySetResult();
                    Assert.That(catchRelease.Wait(TimeSpan.FromSeconds(10)), Is.True, "Bounded real error-log publication barrier.");
                }
            }));
        await using var fixture = await Fixture.CreateAsync(logger.Object).ConfigureAwait(false);
        var (a, b, c) = (fixture.Players[0], fixture.Players[1], fixture.Players[2]);
        var x = await AddItemAsync(a, 20).ConfigureAwait(false);
        var y = await AddItemAsync(b, 20).ConfigureAwait(false);
        var z = await AddItemAsync(c, 20).ConfigureAwait(false);
        var oldAnswer = Task.CompletedTask;
        var peerPacket = Task.CompletedTask;
        var peerQueued = Signal();
        var peerEntered = Signal();
        try
        {
            Assert.That(await RequestAsync(b, a).ConfigureAwait(false), Is.True);
            ((SavingContext)b.PersistenceContext).BeforeSave = async () =>
            {
                saveEntered.TrySetResult();
                await saveRelease.Task.ConfigureAwait(false);
                throw failure;
            };
            oldAnswer = RunAsync(a, () => new TradeAcceptAction().HandleTradeAcceptAsync(a, true));
            await saveEntered.Task.WaitAsync(TimeSpan.FromSeconds(5)).ConfigureAwait(false);
            Mock.Get(a.ViewPlugIns.GetPlugIn<IShowTradeRequestAnswerPlugIn>()!).Verify(v => v.ShowTradeRequestAnswerAsync(true), Times.Once);
            peerPacket = Task.Run(async () =>
            {
                peerQueued.TrySetResult();
                await b.RunPersistenceExclusiveAsync(async () =>
                {
                    peerEntered.TrySetResult();
                    if (successor)
                    {
                        await new TradeCancelAction().CancelTradeAsync(b).ConfigureAwait(false);
                    }
                }).ConfigureAwait(false);
            });
            await peerQueued.Task.WaitAsync(TimeSpan.FromSeconds(5)).ConfigureAwait(false);
            Assert.That(peerEntered.Task.IsCompleted, Is.False, "Peer's actual progress save owns its packet funnel.");
            saveRelease.TrySetResult();
            await catchEntered.Task.WaitAsync(TimeSpan.FromSeconds(5)).ConfigureAwait(false);
            await peerPacket.WaitAsync(TimeSpan.FromSeconds(5)).ConfigureAwait(false);
            if (successor)
            {
                await OpenAsync(b, c).ConfigureAwait(false);
                await OfferAsync(b, 20, 60).ConfigureAwait(false);
                await OfferAsync(c, 20, 25).ConfigureAwait(false);
            }

            var bBackup = b.BackupInventory;
            var cBackup = c.BackupInventory;
            var bTerminals = Mock.Get(b.ViewPlugIns.GetPlugIn<ITradeFinishedPlugIn>()!).Invocations.Count;
            var aTerminals = Mock.Get(a.ViewPlugIns.GetPlugIn<ITradeFinishedPlugIn>()!).Invocations.Count;
            catchRelease.Set();
            await oldAnswer.WaitAsync(TimeSpan.FromSeconds(5)).ConfigureAwait(false);
            if (successor)
            {
                Assert.Multiple(() =>
                {
                    Assert.That(b.TradingPartner, Is.SameAs(c));
                    Assert.That(c.TradingPartner, Is.SameAs(b));
                    Assert.That(b.PlayerState.CurrentState, Is.EqualTo(PlayerState.TradeOpened));
                    Assert.That(c.PlayerState.CurrentState, Is.EqualTo(PlayerState.TradeOpened));
                    Assert.That(b.BackupInventory, Is.SameAs(bBackup));
                    Assert.That(c.BackupInventory, Is.SameAs(cBackup));
                    Assert.That(b.TemporaryStorage!.Items, Does.Contain(y));
                    Assert.That(c.TemporaryStorage!.Items, Does.Contain(z));
                    Assert.That(b.TradingMoney, Is.EqualTo(60));
                    Assert.That(c.TradingMoney, Is.EqualTo(25));
                    Assert.That(Mock.Get(b.ViewPlugIns.GetPlugIn<ITradeFinishedPlugIn>()!).Invocations.Count, Is.EqualTo(bTerminals));
                    Assert.That(Mock.Get(a.ViewPlugIns.GetPlugIn<ITradeFinishedPlugIn>()!).Invocations.Count, Is.EqualTo(aTerminals));
                });
                await FinishAsync(b, c).ConfigureAwait(false);
                Assert.Multiple(() =>
                {
                    Assert.That(b.Inventory!.Items, Does.Contain(z));
                    Assert.That(c.Inventory!.Items, Does.Contain(y));
                    Assert.That(b.Money, Is.EqualTo(265));
                    Assert.That(c.Money, Is.EqualTo(435));
                });
            }
            else
            {
                AssertFree(a);
                AssertFree(b);
                Mock.Get(a.ViewPlugIns.GetPlugIn<ITradeFinishedPlugIn>()!).Verify(v => v.TradeFinishedAsync(TradeResult.Cancelled), Times.Once);
                Mock.Get(b.ViewPlugIns.GetPlugIn<ITradeFinishedPlugIn>()!).Verify(v => v.TradeFinishedAsync(TradeResult.Cancelled), Times.Once);
                await OpenAsync(b, c).ConfigureAwait(false);
                await RunAsync(b, () => new TradeCancelAction().CancelTradeAsync(b)).ConfigureAwait(false);
            }

            Assert.Multiple(() =>
            {
                Assert.That(Count(fixture.Players, x), Is.EqualTo(1));
                Assert.That(Count(fixture.Players, y), Is.EqualTo(1));
                Assert.That(Count(fixture.Players, z), Is.EqualTo(1));
                Assert.That(OwnedMoney(fixture.Players), Is.EqualTo(900));
            });
        }
        finally
        {
            saveRelease.TrySetResult();
            catchRelease.Set();
            await Task.WhenAll(oldAnswer, peerPacket).WaitAsync(TimeSpan.FromSeconds(5)).ConfigureAwait(false);
        }
    }

    /// <summary>A genuine full-inventory result publishes World before the old rollback, permitting a successor.</summary>
    /// <param name="successor">Whether the released peer establishes its new pair before the finisher's transition completes.</param>
    [TestCase(false)]
    [TestCase(true)]
    public async Task FailedFinishPreservesSuccessorAsync(bool successor)
    {
        await using var fixture = await Fixture.CreateAsync().ConfigureAwait(false);
        var (a, b, c) = (fixture.Players[0], fixture.Players[1], fixture.Players[2]);
        var y = await AddItemAsync(b, 20).ConfigureAwait(false);
        var q = await AddItemAsync(b, 21).ConfigureAwait(false);
        var z = await AddItemAsync(c, 20).ConfigureAwait(false);
        for (byte slot = InventoryConstants.EquippableSlotsCount; slot < InventoryConstants.FirstExtensionItemSlotIndex; slot++)
        {
            await AddItemAsync(a, slot).ConfigureAwait(false);
        }

        await OpenAsync(a, b).ConfigureAwait(false);
        await OfferAsync(b, 20, 0).ConfigureAwait(false);
        await RunAsync(b, () => new TradeButtonAction().TradeButtonChangedAsync(b, TradeButtonState.Checked)).ConfigureAwait(false);
        var transitionEntered = Signal();
        var transitionRelease = Signal();
        var armed = 1;
        a.PlayerState.StateChanged += async change =>
        {
            if (change.CurrentStateState == PlayerState.EnteredWorld && Interlocked.Exchange(ref armed, 0) == 1)
            {
                transitionEntered.TrySetResult();
                await transitionRelease.Task.ConfigureAwait(false);
            }
        };
        var finish = RunAsync(a, () => new TradeButtonAction().TradeButtonChangedAsync(a, TradeButtonState.Checked));
        try
        {
            await transitionEntered.Task.WaitAsync(TimeSpan.FromSeconds(5)).ConfigureAwait(false);
            Assert.That(b.PlayerState.CurrentState, Is.EqualTo(PlayerState.EnteredWorld), "Partner context already disposed before finisher publishes World.");
            if (successor)
            {
                await OpenAsync(b, c).ConfigureAwait(false);
                await OfferAsync(b, 21, 60).ConfigureAwait(false);
                await OfferAsync(c, 20, 25).ConfigureAwait(false);
            }

            var backup = b.BackupInventory;
            var bFailureFeedbackAtSuccessorAdmission = Mock.Get(b.ViewPlugIns.GetPlugIn<ITradeFinishedPlugIn>()!).Invocations.Count(i => i.Arguments[0] is TradeResult.FailedByFullInventory);
            transitionRelease.TrySetResult();
            await finish.WaitAsync(TimeSpan.FromSeconds(5)).ConfigureAwait(false);
            if (successor)
            {
                Assert.Multiple(() =>
                {
                    Assert.That(b.PlayerState.CurrentState, Is.EqualTo(PlayerState.TradeOpened));
                    Assert.That(c.PlayerState.CurrentState, Is.EqualTo(PlayerState.TradeOpened));
                    Assert.That(b.TradingPartner, Is.SameAs(c));
                    Assert.That(c.TradingPartner, Is.SameAs(b));
                    Assert.That(b.BackupInventory, Is.SameAs(backup));
                    Assert.That(b.TemporaryStorage!.Items, Does.Contain(q));
                    Assert.That(c.TemporaryStorage!.Items, Does.Contain(z));
                    Assert.That(b.TradingMoney, Is.EqualTo(60));
                    Assert.That(c.TradingMoney, Is.EqualTo(25));
                    Assert.That(bFailureFeedbackAtSuccessorAdmission, Is.EqualTo(1), "Old failure must start before the restored half becomes available to its successor.");
                    Assert.That(Mock.Get(b.ViewPlugIns.GetPlugIn<ITradeFinishedPlugIn>()!).Invocations.Count(i => i.Arguments[0] is TradeResult.FailedByFullInventory), Is.EqualTo(bFailureFeedbackAtSuccessorAdmission), "No late old failure may enter the successor window.");
                });
                await FinishAsync(b, c).ConfigureAwait(false);
            }
            else
            {
                AssertFree(a);
                AssertFree(b);
                Assert.That(b.Inventory!.Items, Does.Contain(y));
                Mock.Get(a.ViewPlugIns.GetPlugIn<ITradeFinishedPlugIn>()!).Verify(v => v.TradeFinishedAsync(TradeResult.FailedByFullInventory), Times.Once);
                Mock.Get(b.ViewPlugIns.GetPlugIn<ITradeFinishedPlugIn>()!).Verify(v => v.TradeFinishedAsync(TradeResult.FailedByFullInventory), Times.Once);
            }

            Assert.Multiple(() =>
            {
                Assert.That(Count(fixture.Players, y), Is.EqualTo(1));
                Assert.That(Count(fixture.Players, q), Is.EqualTo(1));
                Assert.That(Count(fixture.Players, z), Is.EqualTo(1));
                Assert.That(OwnedMoney(fixture.Players), Is.EqualTo(900));
            });
        }
        finally
        {
            transitionRelease.TrySetResult();
            await finish.WaitAsync(TimeSpan.FromSeconds(5)).ConfigureAwait(false);
        }
    }

    /// <summary>Opening a successor before failure rollback must not discard the original trade's escrow.</summary>
    /// <param name="oldEscrow">Escrow committed to the original trade before its inventory fit fails.</param>
    [TestCase(0u)]
    [TestCase(60u)]
    public async Task FailedFitKeepsOriginalEscrowBeforeSuccessorOpenAsync(uint oldEscrow)
    {
        await using var fixture = await Fixture.CreateAsync().ConfigureAwait(false);
        var (a, b, c) = (fixture.Players[0], fixture.Players[1], fixture.Players[2]);
        var y = await AddItemAsync(b, 20).ConfigureAwait(false);
        for (byte slot = InventoryConstants.EquippableSlotsCount; slot < InventoryConstants.FirstExtensionItemSlotIndex; slot++)
        {
            await AddItemAsync(a, slot).ConfigureAwait(false);
        }

        await OpenAsync(a, b).ConfigureAwait(false);
        await OfferAsync(b, 20, oldEscrow).ConfigureAwait(false);
        await RunAsync(b, () => new TradeButtonAction().TradeButtonChangedAsync(b, TradeButtonState.Checked)).ConfigureAwait(false);
        var worldEntered = Signal();
        var worldRelease = Signal();
        var armed = 1;
        a.PlayerState.StateChanged += async change =>
        {
            if (change.CurrentStateState == PlayerState.EnteredWorld && Interlocked.Exchange(ref armed, 0) == 1)
            {
                worldEntered.TrySetResult();
                await worldRelease.Task.ConfigureAwait(false);
            }
        };
        var finish = RunAsync(a, () => new TradeButtonAction().TradeButtonChangedAsync(a, TradeButtonState.Checked));
        try
        {
            await worldEntered.Task.WaitAsync(TimeSpan.FromSeconds(5)).ConfigureAwait(false);
            Assert.Multiple(() =>
            {
                Assert.That(b.PlayerState.CurrentState, Is.EqualTo(PlayerState.EnteredWorld));
                Assert.That(b.Money, Is.EqualTo(300), "World publication occurs during rollback only after escrow restoration.");
                Assert.That(b.TradingMoney, Is.Zero);
            });
            await OpenAsync(b, c).ConfigureAwait(false);
            Assert.Multiple(() =>
            {
                Assert.That(b.Money + b.TradingMoney, Is.EqualTo(300), "New opening must not forget escrow of an old trade whose rollback has not run.");
                Assert.That(OwnedMoney(fixture.Players), Is.EqualTo(900));
                Assert.That(Count(fixture.Players, y), Is.EqualTo(1));
            });
        }
        finally
        {
            worldRelease.TrySetResult();
            await finish.WaitAsync(TimeSpan.FromSeconds(5)).ConfigureAwait(false);
        }
    }

    /// <summary>The null-result own-half cleanup must not act on an independently completed or replaced half.</summary>
    /// <param name="scenario">Untouched stale half, completed half, or successor request before the old cleanup.</param>
    [TestCase("none")]
    [TestCase("cleaned")]
    [TestCase("successor")]
    public async Task NullFinishCleanupOwnsOriginalPeerAsync(string scenario)
    {
        await using var fixture = await Fixture.CreateAsync(playerCount: 4).ConfigureAwait(false);
        var (a, b, c, d) = (fixture.Players[0], fixture.Players[1], fixture.Players[2], fixture.Players[3]);
        var x = await AddItemAsync(a, 20).ConfigureAwait(false);
        var y = await AddItemAsync(b, 20).ConfigureAwait(false);
        var z = await AddItemAsync(c, 20).ConfigureAwait(false);
        await CreateStaleOpenedHalfAsync(a, b).ConfigureAwait(false);
        await OpenAsync(b, c).ConfigureAwait(false);
        await OfferAsync(b, 20, 60).ConfigureAwait(false);
        await OfferAsync(c, 20, 25).ConfigureAwait(false);
        await RunAsync(b, () => new TradeButtonAction().TradeButtonChangedAsync(b, TradeButtonState.Checked)).ConfigureAwait(false);
        var cleanupEntered = Signal();
        using var cleanupRelease = new ManualResetEventSlim();
        using var listener = new MeterListener();
        var armed = 1;
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
                cleanupEntered.TrySetResult();
                Assert.That(cleanupRelease.Wait(TimeSpan.FromSeconds(10)), Is.True, "Bounded actual cancellation-counter publication before domain cleanup.");
            }
        });
        listener.Start();
        var oldPress = RunAsync(a, () => new TradeButtonAction().TradeButtonChangedAsync(a, TradeButtonState.Checked));
        try
        {
            await cleanupEntered.Task.WaitAsync(TimeSpan.FromSeconds(5)).ConfigureAwait(false);
            Assert.That(a.PlayerState.CurrentState, Is.EqualTo(PlayerState.TradeButtonPressed));
            Assert.That(a.TradingPartner, Is.SameAs(b));
            Assert.That(b.TradingPartner, Is.SameAs(c));
            if (scenario != "none")
            {
                // Actual independent forced-placement route, not another packet inside A's occupied funnel.
                await a.RespawnAtAsync(new ExitGate { Map = a.CurrentMap!.Definition, X1 = 1, X2 = 1, Y1 = 1, Y2 = 1 }).AsTask().WaitAsync(TimeSpan.FromSeconds(5)).ConfigureAwait(false);
                AssertFree(a);
                Mock.Get(a.ViewPlugIns.GetPlugIn<ITradeFinishedPlugIn>()!).Verify(v => v.TradeFinishedAsync(TradeResult.Cancelled), Times.Once);
            }

            if (scenario == "successor")
            {
                Assert.That(await RequestAsync(d, a).ConfigureAwait(false), Is.True);
            }

            cleanupRelease.Set();
            await oldPress.WaitAsync(TimeSpan.FromSeconds(5)).ConfigureAwait(false);
            if (scenario == "successor")
            {
                Assert.Multiple(() =>
                {
                    Assert.That(a.PlayerState.CurrentState, Is.EqualTo(PlayerState.TradeRequested));
                    Assert.That(d.PlayerState.CurrentState, Is.EqualTo(PlayerState.TradeRequested));
                    Assert.That(a.TradingPartner, Is.SameAs(d));
                    Assert.That(d.TradingPartner, Is.SameAs(a));
                });
            }
            else
            {
                AssertFree(a);
            }

            Assert.Multiple(() =>
            {
                Mock.Get(a.ViewPlugIns.GetPlugIn<ITradeFinishedPlugIn>()!).Verify(v => v.TradeFinishedAsync(TradeResult.Cancelled), Times.Once);
                Mock.Get(b.ViewPlugIns.GetPlugIn<ITradeFinishedPlugIn>()!).Verify(v => v.TradeFinishedAsync(TradeResult.Cancelled), Times.Never);
                Mock.Get(c.ViewPlugIns.GetPlugIn<ITradeFinishedPlugIn>()!).Verify(v => v.TradeFinishedAsync(TradeResult.Cancelled), Times.Never);
                Assert.That(b.TradingPartner, Is.SameAs(c));
                Assert.That(c.TradingPartner, Is.SameAs(b));
                Assert.That(b.TemporaryStorage!.Items, Does.Contain(y));
                Assert.That(c.TemporaryStorage!.Items, Does.Contain(z));
                Assert.That(b.TradingMoney, Is.EqualTo(60));
                Assert.That(c.TradingMoney, Is.EqualTo(25));
                Assert.That(Count(fixture.Players, x), Is.EqualTo(1));
                Assert.That(Count(fixture.Players, y), Is.EqualTo(1));
                Assert.That(Count(fixture.Players, z), Is.EqualTo(1));
                Assert.That(OwnedMoney(fixture.Players), Is.EqualTo(1400));
            });
            await RunAsync(c, () => new TradeButtonAction().TradeButtonChangedAsync(c, TradeButtonState.Checked)).ConfigureAwait(false);
            AssertFree(b);
            AssertFree(c);
        }
        finally
        {
            cleanupRelease.Set();
            await oldPress.WaitAsync(TimeSpan.FromSeconds(5)).ConfigureAwait(false);
        }
    }

    /// <summary>Reversing the initiating button must not reverse acquisition order for the same participants.</summary>
    /// <param name="reverseFirst">Whether B initiates the first completion rather than A.</param>
    [TestCase(false)]
    [TestCase(true)]
    public async Task FinishAcquisitionOrderDoesNotFollowInitiatorAsync(bool reverseFirst)
    {
        await using var fixture = await Fixture.CreateAsync().ConfigureAwait(false);
        var (a, b) = (fixture.Players[0], fixture.Players[1]);
        var x = await AddItemAsync(a, 20).ConfigureAwait(false);
        var y = await AddItemAsync(b, 20).ConfigureAwait(false);
        var acquired = new List<Player>();
        foreach (var player in new[] { a, b })
        {
            player.PlayerState.StateChanges += change =>
            {
                if (change.NextState == PlayerState.EnteredWorld && player.PlayerState.CurrentState == PlayerState.TradeButtonPressed)
                {
                    acquired.Add(player);
                }

                return ValueTask.CompletedTask;
            };
        }

        await OpenAsync(a, b).ConfigureAwait(false);
        await OfferAsync(a, x.ItemSlot, 60).ConfigureAwait(false);
        await OfferAsync(b, y.ItemSlot, 25).ConfigureAwait(false);
        if (reverseFirst)
        {
            await FinishAsync(a, b).ConfigureAwait(false);
        }
        else
        {
            await FinishAsync(b, a).ConfigureAwait(false);
        }

        var firstOrder = acquired.ToArray();
        Assert.That(firstOrder, Has.Length.EqualTo(2));
        acquired.Clear();
        await OpenAsync(a, b).ConfigureAwait(false);
        await OfferAsync(a, y.ItemSlot, 25).ConfigureAwait(false);
        await OfferAsync(b, x.ItemSlot, 60).ConfigureAwait(false);
        if (reverseFirst)
        {
            await FinishAsync(b, a).ConfigureAwait(false);
        }
        else
        {
            await FinishAsync(a, b).ConfigureAwait(false);
        }

        Assert.Multiple(() =>
        {
            Assert.That(acquired, Has.Count.EqualTo(2));
            Assert.That(acquired[0], Is.SameAs(firstOrder[0]), "The same state exclusion must be acquired first in both initiating orientations.");
            Assert.That(acquired[1], Is.SameAs(firstOrder[1]));
            Assert.That(a.Inventory!.Items, Does.Contain(x));
            Assert.That(b.Inventory!.Items, Does.Contain(y));
            Assert.That(Count(fixture.Players, x), Is.EqualTo(1));
            Assert.That(Count(fixture.Players, y), Is.EqualTo(1));
            Assert.That(a.Money, Is.EqualTo(200));
            Assert.That(b.Money, Is.EqualTo(300));
            Assert.That(OwnedMoney(fixture.Players), Is.EqualTo(900));
            Mock.Get(a.ViewPlugIns.GetPlugIn<ITradeFinishedPlugIn>()!).Verify(v => v.TradeFinishedAsync(TradeResult.Success), Times.Exactly(2));
            Mock.Get(b.ViewPlugIns.GetPlugIn<ITradeFinishedPlugIn>()!).Verify(v => v.TradeFinishedAsync(TradeResult.Success), Times.Exactly(2));
        });
    }

    /// <summary>Both ordinary simultaneous button funnels complete once without a bug-dependent barrier.</summary>
    /// <param name="reverseLaunch">Whether B's packet task is launched first.</param>
    [TestCase(false)]
    [TestCase(true)]
    public async Task ConcurrentButtonsCompleteExactlyOnceAsync(bool reverseLaunch)
    {
        await using var fixture = await Fixture.CreateAsync().ConfigureAwait(false);
        var (a, b) = (fixture.Players[0], fixture.Players[1]);
        var x = await AddItemAsync(a, 20).ConfigureAwait(false);
        var y = await AddItemAsync(b, 20).ConfigureAwait(false);
        await OpenAsync(a, b).ConfigureAwait(false);
        await OfferAsync(a, 20, 60).ConfigureAwait(false);
        await OfferAsync(b, 20, 25).ConfigureAwait(false);
        var pressed = 0;
        var bothPressed = Signal();
        foreach (var player in new[] { a, b })
        {
            player.PlayerState.StateChanged += async change =>
            {
                if (change.CurrentStateState == PlayerState.TradeButtonPressed)
                {
                    if (Interlocked.Increment(ref pressed) == 2)
                    {
                        bothPressed.TrySetResult();
                    }

                    await bothPressed.Task.ConfigureAwait(false);
                }
            };
        }

        var first = reverseLaunch ? b : a;
        var second = reverseLaunch ? a : b;
        var firstPacket = RunAsync(first, () => new TradeButtonAction().TradeButtonChangedAsync(first, TradeButtonState.Checked));
        var secondPacket = RunAsync(second, () => new TradeButtonAction().TradeButtonChangedAsync(second, TradeButtonState.Checked));
        await Task.WhenAll(firstPacket, secondPacket).WaitAsync(TimeSpan.FromSeconds(5)).ConfigureAwait(false);
        AssertFree(a);
        AssertFree(b);
        Assert.Multiple(() =>
        {
            Assert.That(pressed, Is.EqualTo(2));
            Assert.That(a.Inventory!.Items, Does.Contain(y));
            Assert.That(b.Inventory!.Items, Does.Contain(x));
            Assert.That(Count(fixture.Players, x), Is.EqualTo(1));
            Assert.That(Count(fixture.Players, y), Is.EqualTo(1));
            Assert.That(a.Money, Is.EqualTo(165));
            Assert.That(b.Money, Is.EqualTo(335));
            Assert.That(OwnedMoney(fixture.Players), Is.EqualTo(900));
            Mock.Get(a.ViewPlugIns.GetPlugIn<ITradeFinishedPlugIn>()!).Verify(v => v.TradeFinishedAsync(TradeResult.Success), Times.Once);
            Mock.Get(b.ViewPlugIns.GetPlugIn<ITradeFinishedPlugIn>()!).Verify(v => v.TradeFinishedAsync(TradeResult.Success), Times.Once);
            Mock.Get(a.ViewPlugIns.GetPlugIn<ITradeFinishedPlugIn>()!).Verify(v => v.TradeFinishedAsync(TradeResult.Cancelled), Times.Never);
            Mock.Get(b.ViewPlugIns.GetPlugIn<ITradeFinishedPlugIn>()!).Verify(v => v.TradeFinishedAsync(TradeResult.Cancelled), Times.Never);
        });
    }

    /// <summary>Both rollback halves finish while the standard view wrapper observes and logs synchronous/asynchronous callback errors.</summary>
    /// <param name="synchronous">Whether the first view throws directly rather than returning a failed task.</param>
    [TestCase(false)]
    [TestCase(true)]
    public async Task FailedRollbackObservesBothViewErrorsAsync(bool synchronous)
    {
        var firstFailure = new InvalidOperationException("First rollback view failure.");
        var secondFailure = new InvalidOperationException("Second rollback view failure.");
        var logged = new System.Collections.Concurrent.ConcurrentQueue<Exception>();
        var logger = new Mock<ILogger>();
        logger.Setup(l => l.IsEnabled(It.IsAny<LogLevel>())).Returns(true);
        logger.Setup(l => l.Log(It.IsAny<LogLevel>(), It.IsAny<EventId>(), It.IsAny<It.IsAnyType>(), It.IsAny<Exception?>(), It.IsAny<Func<It.IsAnyType, Exception?, string>>()))
            .Callback(new InvocationAction(invocation =>
            {
                if (invocation.Arguments[3] is Exception error
                    && (ReferenceEquals(error, firstFailure) || ReferenceEquals(error, secondFailure)))
                {
                    logged.Enqueue(error);
                }
            }));
        await using var fixture = await Fixture.CreateAsync(logger.Object).ConfigureAwait(false);
        var (a, b) = (fixture.Players[0], fixture.Players[1]);
        var y = await AddItemAsync(b, 20).ConfigureAwait(false);
        for (byte slot = InventoryConstants.EquippableSlotsCount; slot < InventoryConstants.FirstExtensionItemSlotIndex; slot++)
        {
            await AddItemAsync(a, slot).ConfigureAwait(false);
        }

        await OpenAsync(a, b).ConfigureAwait(false);
        await OfferAsync(b, 20, 60).ConfigureAwait(false);
        await RunAsync(b, () => new TradeButtonAction().TradeButtonChangedAsync(b, TradeButtonState.Checked)).ConfigureAwait(false);
        var aView = Mock.Get(a.ViewPlugIns.GetPlugIn<ITradeFinishedPlugIn>()!);
        var bView = Mock.Get(b.ViewPlugIns.GetPlugIn<ITradeFinishedPlugIn>()!);
        if (synchronous)
        {
            aView.Setup(v => v.TradeFinishedAsync(TradeResult.FailedByFullInventory)).Throws(firstFailure);
        }
        else
        {
            aView.Setup(v => v.TradeFinishedAsync(TradeResult.FailedByFullInventory)).Returns(ValueTask.FromException(firstFailure));
        }

        bView.Setup(v => v.TradeFinishedAsync(TradeResult.FailedByFullInventory)).Returns(ValueTask.FromException(secondFailure));
        var finish = RunAsync(a, () => new TradeButtonAction().TradeButtonChangedAsync(a, TradeButtonState.Checked));
        await finish.WaitAsync(TimeSpan.FromSeconds(5)).ConfigureAwait(false);
        AssertFree(a);
        AssertFree(b);
        Assert.Multiple(() =>
        {
            Assert.That(logged.Count(error => ReferenceEquals(error, firstFailure)), Is.EqualTo(1));
            Assert.That(logged.Count(error => ReferenceEquals(error, secondFailure)), Is.EqualTo(1));
            aView.Verify(v => v.TradeFinishedAsync(TradeResult.FailedByFullInventory), Times.Once);
            bView.Verify(v => v.TradeFinishedAsync(TradeResult.FailedByFullInventory), Times.Once);
            Assert.That(b.Inventory!.Items, Does.Contain(y));
            Assert.That(b.Money, Is.EqualTo(300));
            Assert.That(b.TradingMoney, Is.Zero);
            Assert.That(Count(fixture.Players, y), Is.EqualTo(1));
            Assert.That(OwnedMoney(fixture.Players), Is.EqualTo(900));
        });
    }

    /// <summary>An actual cleanup instrumentation error must not prevent the other half's guarded rollback.</summary>
    [Test]
    public async Task FailedRollbackStillAttemptsOtherHalfAsync()
    {
        await using var fixture = await Fixture.CreateAsync().ConfigureAwait(false);
        var (a, b, c) = (fixture.Players[0], fixture.Players[1], fixture.Players[2]);
        var y = await AddItemAsync(b, 20).ConfigureAwait(false);
        for (byte slot = InventoryConstants.EquippableSlotsCount; slot < InventoryConstants.FirstExtensionItemSlotIndex; slot++)
        {
            await AddItemAsync(a, slot).ConfigureAwait(false);
        }

        await OpenAsync(a, b).ConfigureAwait(false);
        await OfferAsync(b, 20, 60).ConfigureAwait(false);
        await RunAsync(b, () => new TradeButtonAction().TradeButtonChangedAsync(b, TradeButtonState.Checked)).ConfigureAwait(false);
        var ownedBackup = b.BackupInventory;
        var failure = new InvalidOperationException("First cleanup instrumentation failed.");
        var cleanupCalls = 0;
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
            if (Interlocked.Increment(ref cleanupCalls) == 1)
            {
                throw failure;
            }
        });
        listener.Start();
        var observed = Assert.ThrowsAsync<InvalidOperationException>(async () => await RunAsync(a, () => new TradeButtonAction().TradeButtonChangedAsync(a, TradeButtonState.Checked)).WaitAsync(TimeSpan.FromSeconds(5)).ConfigureAwait(false));
        AssertFree(a);
        Assert.Multiple(() =>
        {
            Assert.That(observed, Is.SameAs(failure));
            Assert.That(cleanupCalls, Is.EqualTo(2));
            Assert.That(b.PlayerState.CurrentState, Is.EqualTo(PlayerState.TradeButtonPressed));
            Assert.That(b.BackupInventory, Is.SameAs(ownedBackup));
            Assert.That(b.TemporaryStorage!.Items, Does.Contain(y));
            Assert.That(b.Money, Is.EqualTo(240));
            Assert.That(b.TradingMoney, Is.EqualTo(60));
            Assert.That(OwnedMoney(fixture.Players), Is.EqualTo(900));
            Mock.Get(a.ViewPlugIns.GetPlugIn<ITradeFinishedPlugIn>()!).Verify(v => v.TradeFinishedAsync(TradeResult.FailedByFullInventory), Times.Once);
            Mock.Get(b.ViewPlugIns.GetPlugIn<ITradeFinishedPlugIn>()!).Verify(v => v.TradeFinishedAsync(TradeResult.FailedByFullInventory), Times.Never);
        });
        await RunAsync(b, () => new TradeCancelAction().CancelTradeAsync(b)).ConfigureAwait(false);
        Assert.That(b.Money, Is.EqualTo(300));
        await OpenAsync(b, c).ConfigureAwait(false);
        await RunAsync(b, () => new TradeCancelAction().CancelTradeAsync(b)).ConfigureAwait(false);
        Assert.That(OwnedMoney(fixture.Players), Is.EqualTo(900));
    }

    private static async Task CreateStaleOpenedHalfAsync(Player a, Player b)
    {
        var character = b.SelectedCharacter!;
        character.Name = "Review152B";
        Mock.Get(b.Account!).Setup(account => account.Characters).Returns(new List<Character> { character });
        var removalEntered = Signal();
        var removalRelease = Signal();
        var firstAccepted = Signal();
        var armed = 1;
        b.CurrentMap!.ObjectRemoved += async removal =>
        {
            if (ReferenceEquals(removal.Object, b) && Interlocked.Exchange(ref armed, 0) == 1)
            {
                removalEntered.TrySetResult();
                await removalRelease.Task.ConfigureAwait(false);
            }
        };
        Mock.Get(a.ViewPlugIns.GetPlugIn<IShowTradeRequestAnswerPlugIn>()!).Setup(v => v.ShowTradeRequestAnswerAsync(true)).Returns(() =>
        {
            firstAccepted.TrySetResult();
            return ValueTask.CompletedTask;
        });
        var logout = RunAsync(b, () => new LogoutAction().LogoutAsync(b, LogoutType.BackToCharacterSelection));
        var opening = Task.CompletedTask;
        try
        {
            await removalEntered.Task.WaitAsync(TimeSpan.FromSeconds(5)).ConfigureAwait(false);
            Assert.That(await RequestAsync(a, b).ConfigureAwait(false), Is.True);
            opening = RunAsync(a, () => new TradeAcceptAction().HandleTradeAcceptAsync(a, true));
            await firstAccepted.Task.WaitAsync(TimeSpan.FromSeconds(5)).ConfigureAwait(false);
        }
        finally
        {
            removalRelease.TrySetResult();
            await Task.WhenAll(logout, opening).WaitAsync(TimeSpan.FromSeconds(5)).ConfigureAwait(false);
        }

        await RunAsync(b, () => new RequestCharacterListAction().RequestCharacterListAsync(b)).ConfigureAwait(false);
        await RunAsync(b, () => new SelectCharacterAction().SelectCharacterAsync(b, character.Name)).ConfigureAwait(false);
        Assert.That(b.PlayerState.CurrentState, Is.EqualTo(PlayerState.EnteredWorld));
        Assert.That(a.PlayerState.CurrentState, Is.EqualTo(PlayerState.TradeOpened));
        Assert.That(a.TradingPartner, Is.SameAs(b));
    }

    private static TaskCompletionSource Signal() => new(TaskCreationOptions.RunContinuationsAsynchronously);

    private static Task RunAsync(Player player, Func<ValueTask> action) => Task.Run(async () => await player.RunPersistenceExclusiveAsync(action).ConfigureAwait(false));

    private static Task<bool> RequestAsync(Player a, Player b) => Task.Run(async () => await a.RunPersistenceExclusiveAsync(() => new TradeRequestAction().RequestTradeAsync(a, b)).ConfigureAwait(false));

    private static async Task OpenAsync(Player requester, Player accepter)
    {
        Assert.That(await RequestAsync(requester, accepter).ConfigureAwait(false), Is.True);
        await RunAsync(accepter, () => new TradeAcceptAction().HandleTradeAcceptAsync(accepter, true)).ConfigureAwait(false);
        Assert.That(requester.PlayerState.CurrentState, Is.EqualTo(PlayerState.TradeOpened));
        Assert.That(accepter.PlayerState.CurrentState, Is.EqualTo(PlayerState.TradeOpened));
    }

    private static async Task OfferAsync(Player player, byte slot, uint money)
    {
        await RunAsync(player, async () =>
        {
            await new MoveItemAction().MoveItemAsync(player, slot, Storages.Inventory, 0, Storages.Trade).ConfigureAwait(false);
            await new TradeMoneyAction().TradeMoneyAsync(player, money).ConfigureAwait(false);
        }).ConfigureAwait(false);
    }

    private static async Task FinishAsync(Player a, Player b)
    {
        await RunAsync(a, () => new TradeButtonAction().TradeButtonChangedAsync(a, TradeButtonState.Checked)).ConfigureAwait(false);
        await RunAsync(b, () => new TradeButtonAction().TradeButtonChangedAsync(b, TradeButtonState.Checked)).ConfigureAwait(false);
    }

    private static async Task<Item> AddItemAsync(Player player, byte slot)
    {
        var item = player.PersistenceContext.CreateNew<Item>();
        item.Definition = new ItemDefinition { Width = 1, Height = 1, Durability = 9, IsTradable = true };
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

    // Successful reset deliberately leaves TradingMoney as an inactive value; only a live backup owns escrow.
    private static int OwnedMoney(IEnumerable<Player> players) => players.Sum(p => p.Money + (p.BackupInventory is null ? 0 : p.TradingMoney));

    private static int Count(IEnumerable<Player> players, Item item) => players.Sum(p => p.Inventory!.Items.Count(i => ReferenceEquals(i, item)) + p.TemporaryStorage!.Items.Count(i => ReferenceEquals(i, item)));

    private sealed class SavingProvider(GameConfiguration configuration) : InMemoryPersistenceContextProvider, IPersistenceContextProvider
    {
        IPlayerContext IPersistenceContextProvider.CreateNewPlayerContext(GameConfiguration actualConfiguration)
        {
            Assert.That(actualConfiguration, Is.SameAs(configuration));
            return new SavingContext((InMemoryRepositoryProvider)this.RepositoryProvider);
        }
    }

    private sealed class SavingContext(InMemoryRepositoryProvider provider) : PlayerInMemoryContext(provider), IPlayerContext
    {
        public Func<ValueTask>? BeforeSave { get; set; }

        async ValueTask<bool> IContext.SaveChangesAsync(CancellationToken cancellationToken)
        {
            if (this.BeforeSave is { } before)
            {
                this.BeforeSave = null;
                await before().ConfigureAwait(false);
            }

            return await base.SaveChangesAsync(cancellationToken).ConfigureAwait(false);
        }
    }

    private sealed class Fixture(GameContext game, Player[] players) : IAsyncDisposable
    {
        public Player[] Players { get; } = players;

        public static async Task<Fixture> CreateAsync(ILogger? logger = null, int playerCount = 3)
        {
            var configuration = PlayerTestHelper.CreateGameConfiguration();
            configuration.MaximumInventoryMoney = 1_000_000;
            var factory = new Mock<ILoggerFactory>();
            factory.Setup(f => f.CreateLogger(It.IsAny<string>())).Returns(logger ?? NullLogger.Instance);
            var initializer = new MapInitializer(configuration, NullLogger<MapInitializer>.Instance, NullDropGenerator.Instance, null);
            var game = new GameContext(configuration, new SavingProvider(configuration), initializer, factory.Object, new PlugInManager(null, factory.Object, null, null), NullDropGenerator.Instance, new ConfigurationChangeMediator());
            initializer.PlugInManager = game.PlugInManager;
            initializer.PathFinderPool = game.PathFinderPool;
            var players = new Player[playerCount];
            for (var i = 0; i < players.Length; i++)
            {
                players[i] = await PlayerTestHelper.CreatePlayerAsync(game).ConfigureAwait(false);
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
