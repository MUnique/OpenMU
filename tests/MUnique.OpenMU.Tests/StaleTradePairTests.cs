// <copyright file="StaleTradePairTests.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.Tests;

using Moq;
using MUnique.OpenMU.DataModel.Configuration.Items;
using MUnique.OpenMU.DataModel.Entities;
using MUnique.OpenMU.GameLogic;
using MUnique.OpenMU.GameLogic.PlayerActions;
using MUnique.OpenMU.GameLogic.PlayerActions.Character;
using MUnique.OpenMU.GameLogic.PlayerActions.Items;
using MUnique.OpenMU.GameLogic.PlayerActions.Trade;
using MUnique.OpenMU.GameLogic.Views.Login;
using MUnique.OpenMU.GameLogic.Views.Trade;

/// <summary>
/// A player who logged out of a trade and selected its character again can trade with someone else, while the old
/// partner still holds its half of the old trade or request. The old half must neither move items or money of the
/// new trade nor break the new pair; two players which request each other at the same time don't end in a pair which
/// neither of them was shown.
/// This exercises action ordering with an already-resolved target and a response from the requesting peer;
/// it does not assert the normal client UI sequence.
/// </summary>
[TestFixture]
public class StaleTradePairTests
{
    /// <summary>
    /// The old partner of an opened trade presses its trade button after the owner opened a new trade, offered an
    /// item and money and pressed its own button. The old trade is a late invitation: it reaches the owner while its
    /// logout is at the map removal, after the logout closed the trades of the owner. Every item exists exactly once, at its owner after the new trade,
    /// and the money of the new trade goes to the new partner only.
    /// </summary>
    [Test]
    public async Task StaleOpenedPartnerCannotFinishNewTradeAsync()
    {
        var owner = await PlayerTestHelper.CreatePlayerAsync().ConfigureAwait(false);
        Player? oldPeer = null;
        Player? newPeer = null;
        try
        {
            oldPeer = await PlayerTestHelper.CreatePlayerAsync(owner.GameContext).ConfigureAwait(false);
            newPeer = await PlayerTestHelper.CreatePlayerAsync(owner.GameContext).ConfigureAwait(false);
            var (p, d) = (oldPeer, newPeer);
            var character = PrepareOwner(owner);
            var removalRelease = new TaskCompletionSource(TaskCreationOptions.RunContinuationsAsynchronously);
            var removalEntered = HoldAtMapRemoval(owner, removalRelease);
            var logout = Task.CompletedTask;
            var oldAnswer = Task.CompletedTask;
            var oldTradeShown = new TaskCompletionSource(TaskCreationOptions.RunContinuationsAsynchronously);
            Mock.Get(p.ViewPlugIns.GetPlugIn<IShowTradeRequestAnswerPlugIn>()!)
                .Setup(v => v.ShowTradeRequestAnswerAsync(true))
                .Returns(() =>
                {
                    oldTradeShown.TrySetResult();
                    return ValueTask.CompletedTask;
                });
            owner.Money = 200;
            p.Money = 100;
            d.Money = 300;
            var x = await AddItemAsync(p).ConfigureAwait(false);
            var y = await AddItemAsync(owner).ConfigureAwait(false);
            var z = await AddItemAsync(d).ConfigureAwait(false);
            try
            {
                // The old trade: the old peer requests and opens it while the owner's logout is at its map removal (the
                // owner is still selected and alive there), offers its item after the logout, and the owner selects its
                // character again. The old peer uses its own packet funnel; the owner's funnel is held by the logout.
                logout = RunAsync(owner, () => new LogoutAction().LogoutAsync(owner, LogoutType.BackToCharacterSelection));
                Assert.That(await Task.WhenAny(removalEntered, logout).WaitAsync(TimeSpan.FromSeconds(5)).ConfigureAwait(false), Is.SameAs(removalEntered), "setup: the logout reaches its map removal");
                Assert.That(await RunAsync(p, () => new TradeRequestAction().RequestTradeAsync(p, owner)).WaitAsync(TimeSpan.FromSeconds(5)).ConfigureAwait(false), Is.True, "the late invitation is accepted");
                oldAnswer = RunAsync(p, () => new TradeAcceptAction().HandleTradeAcceptAsync(p, true));
                Assert.That(await Task.WhenAny(oldTradeShown.Task, oldAnswer).WaitAsync(TimeSpan.FromSeconds(5)).ConfigureAwait(false), Is.SameAs(oldTradeShown.Task), "setup: the old trade opens for the old peer during the logout");
                removalRelease.TrySetResult();
                await logout.WaitAsync(TimeSpan.FromSeconds(5)).ConfigureAwait(false);
                await oldAnswer.WaitAsync(TimeSpan.FromSeconds(5)).ConfigureAwait(false);
                Assert.Multiple(() =>
                {
                    Assert.That(owner.PlayerState.CurrentState, Is.EqualTo(PlayerState.Authenticated));
                    Assert.That(p.PlayerState.CurrentState, Is.EqualTo(PlayerState.TradeOpened), "the opened half survives the logout");
                    Assert.That(p.TradingPartner, Is.SameAs(owner));
                    Assert.That(p.BackupInventory, Is.Not.Null);
                });
                await RunAsync(p, () => new MoveItemAction().MoveItemAsync(p, 20, Storages.Inventory, 0, Storages.Trade)).ConfigureAwait(false);
                Assert.That(p.TemporaryStorage!.Items, Does.Contain(x), "the old peer offers its item in its half");
                await ReselectAsync(owner, character).ConfigureAwait(false);
                Assert.That(p.PlayerState.CurrentState, Is.EqualTo(PlayerState.TradeOpened), "The old peer keeps its half of the old trade.");

                // The new trade: opened, both offer, only the owner presses its button.
                Assert.That(await RunAsync(owner, () => new TradeRequestAction().RequestTradeAsync(owner, d)).ConfigureAwait(false), Is.True);
                await RunAsync(d, () => new TradeAcceptAction().HandleTradeAcceptAsync(d, true)).ConfigureAwait(false);
                Assert.That(owner.PlayerState.CurrentState, Is.EqualTo(PlayerState.TradeOpened));
                Assert.That(d.PlayerState.CurrentState, Is.EqualTo(PlayerState.TradeOpened));
                await RunAsync(owner, async () =>
                {
                    await new MoveItemAction().MoveItemAsync(owner, 20, Storages.Inventory, 0, Storages.Trade).ConfigureAwait(false);
                    await new TradeMoneyAction().TradeMoneyAsync(owner, 60).ConfigureAwait(false);
                }).ConfigureAwait(false);
                await RunAsync(d, () => new MoveItemAction().MoveItemAsync(d, 20, Storages.Inventory, 0, Storages.Trade)).ConfigureAwait(false);
                await RunAsync(owner, () => new TradeButtonAction().TradeButtonChangedAsync(owner, TradeButtonState.Checked)).ConfigureAwait(false);
                Assert.That(owner.PlayerState.CurrentState, Is.EqualTo(PlayerState.TradeButtonPressed));

                // The old peer's button packet, then the new peer completes the new trade.
                await RunAsync(p, () => new TradeButtonAction().TradeButtonChangedAsync(p, TradeButtonState.Checked)).ConfigureAwait(false);
                TestContext.Out.WriteLine($"After old peer's button: owner {owner.PlayerState.CurrentState}, old peer {p.PlayerState.CurrentState}, new peer {d.PlayerState.CurrentState}");
                await RunAsync(d, () => new TradeButtonAction().TradeButtonChangedAsync(d, TradeButtonState.Checked)).ConfigureAwait(false);
                TestContext.Out.WriteLine($"After new peer's button: owner {owner.PlayerState.CurrentState}, old peer {p.PlayerState.CurrentState}, new peer {d.PlayerState.CurrentState}");
            }
            finally
            {
                removalRelease.TrySetResult();
                await WhenEndedAsync(logout, oldAnswer).ConfigureAwait(false);
                await EndTradesAsync(d, p, owner).ConfigureAwait(false);
            }

            var players = new[] { owner, p, d };
            TestContext.Out.WriteLine($"Counts X {Count(players, x)}, Y {Count(players, y)}, Z {Count(players, z)}; money owner {owner.Money}, old peer {p.Money}, new peer {d.Money}");
            Assert.Multiple(() =>
            {
                Assert.That(Count(players, x), Is.EqualTo(1), "old peer's item exists once");
                Assert.That(Count(players, y), Is.EqualTo(1), "owner's item exists once");
                Assert.That(Count(players, z), Is.EqualTo(1), "new peer's item exists once");
                Assert.That(p.Inventory!.Items, Does.Contain(x), "the old peer keeps its item");
                Assert.That(d.Inventory!.Items, Does.Contain(y), "the owner's item goes to the new peer");
                Assert.That(owner.Inventory!.Items, Does.Contain(z), "the new peer's item goes to the owner");
                Assert.That(owner.Money + p.Money + d.Money, Is.EqualTo(600), "money is conserved");
                Assert.That(owner.Money, Is.EqualTo(140));
                Assert.That(p.Money, Is.EqualTo(100));
                Assert.That(d.Money, Is.EqualTo(360));
                foreach (var player in players)
                {
                    Assert.That(player.PlayerState.CurrentState, Is.EqualTo(PlayerState.EnteredWorld));
                    Assert.That(player.TradingPartner, Is.Null);
                    Assert.That(player.BackupInventory, Is.Null);
                }
            });
        }
        finally
        {
            await DisposeAllAsync(owner, oldPeer, newPeer).ConfigureAwait(false);
        }
    }

    /// <summary>
    /// The old partner of a pending request answers it after the owner logged out, selected its character again and
    /// has a new pending request with another player. The answer ends only the old half; the new pair stays usable.
    /// The old request is a late invitation of the old partner, which reaches the owner while its logout is at the
    /// map removal, after the logout closed the trades of the owner.
    /// </summary>
    /// <param name="accept">Whether the old partner accepts rather than declines.</param>
    [TestCase(true)]
    [TestCase(false)]
    public async Task StaleRequestAnswerKeepsNewPairAsync(bool accept)
    {
        var owner = await PlayerTestHelper.CreatePlayerAsync().ConfigureAwait(false);
        Player? oldPeer = null;
        Player? newPeer = null;
        try
        {
            oldPeer = await PlayerTestHelper.CreatePlayerAsync(owner.GameContext).ConfigureAwait(false);
            newPeer = await PlayerTestHelper.CreatePlayerAsync(owner.GameContext).ConfigureAwait(false);
            var (p, c) = (oldPeer, newPeer);
            var character = PrepareOwner(owner);
            var removalRelease = new TaskCompletionSource(TaskCreationOptions.RunContinuationsAsynchronously);
            var removalEntered = HoldAtMapRemoval(owner, removalRelease);
            var logout = Task.CompletedTask;
            owner.Money = 200;
            p.Money = 100;
            c.Money = 300;
            var y = await AddItemAsync(owner).ConfigureAwait(false);
            try
            {
                logout = RunAsync(owner, () => new LogoutAction().LogoutAsync(owner, LogoutType.BackToCharacterSelection));
                Assert.That(await Task.WhenAny(removalEntered, logout).WaitAsync(TimeSpan.FromSeconds(5)).ConfigureAwait(false), Is.SameAs(removalEntered), "setup: the logout reaches its map removal");
                Assert.That(await RunAsync(p, () => new TradeRequestAction().RequestTradeAsync(p, owner)).WaitAsync(TimeSpan.FromSeconds(5)).ConfigureAwait(false), Is.True, "the late invitation is accepted");
                removalRelease.TrySetResult();
                await logout.WaitAsync(TimeSpan.FromSeconds(5)).ConfigureAwait(false);
                Assert.Multiple(() =>
                {
                    Assert.That(owner.PlayerState.CurrentState, Is.EqualTo(PlayerState.Authenticated));
                    Assert.That(p.PlayerState.CurrentState, Is.EqualTo(PlayerState.TradeRequested), "the requested half survives the logout");
                    Assert.That(p.TradingPartner, Is.SameAs(owner));
                });
                await ReselectAsync(owner, character).ConfigureAwait(false);
                Assert.That(p.PlayerState.CurrentState, Is.EqualTo(PlayerState.TradeRequested), "The old peer keeps its half of the old request.");
                Assert.That(await RunAsync(owner, () => new TradeRequestAction().RequestTradeAsync(owner, c)).ConfigureAwait(false), Is.True);

                // The old peer answers its old request; it didn't cancel before.
                await RunAsync(p, () => new TradeAcceptAction().HandleTradeAcceptAsync(p, accept)).ConfigureAwait(false);
                TestContext.Out.WriteLine($"After old answer: owner {owner.PlayerState.CurrentState}, old peer {p.PlayerState.CurrentState}, new peer {c.PlayerState.CurrentState}");
                Assert.Multiple(() =>
                {
                    Assert.That(p.PlayerState.CurrentState, Is.EqualTo(PlayerState.EnteredWorld), "the old half ends");
                    Assert.That(p.TradingPartner, Is.Null);
                    Assert.That(p.BackupInventory, Is.Null);
                    Assert.That(owner.PlayerState.CurrentState, Is.EqualTo(PlayerState.TradeRequested), "the new pair is kept");
                    Assert.That(c.PlayerState.CurrentState, Is.EqualTo(PlayerState.TradeRequested));
                    Assert.That(owner.TradingPartner, Is.SameAs(c));
                    Assert.That(c.TradingPartner, Is.SameAs(owner));
                });

                // The new pair opens and ends normally.
                await RunAsync(c, () => new TradeAcceptAction().HandleTradeAcceptAsync(c, true)).ConfigureAwait(false);
                Assert.Multiple(() =>
                {
                    Assert.That(owner.PlayerState.CurrentState, Is.EqualTo(PlayerState.TradeOpened));
                    Assert.That(c.PlayerState.CurrentState, Is.EqualTo(PlayerState.TradeOpened));
                    Assert.That(owner.BackupInventory, Is.Not.Null);
                    Assert.That(c.BackupInventory, Is.Not.Null);
                });
                await RunAsync(owner, () => new TradeCancelAction().CancelTradeAsync(owner)).ConfigureAwait(false);
            }
            finally
            {
                removalRelease.TrySetResult();
                await WhenEndedAsync(logout).ConfigureAwait(false);
                await EndTradesAsync(c, p, owner).ConfigureAwait(false);
            }

            var players = new[] { owner, p, c };
            Assert.Multiple(() =>
            {
                Assert.That(Count(players, y), Is.EqualTo(1));
                Assert.That(owner.Inventory!.Items, Does.Contain(y));
                Assert.That(owner.Money, Is.EqualTo(200));
                Assert.That(p.Money, Is.EqualTo(100));
                Assert.That(c.Money, Is.EqualTo(300));
                foreach (var player in players)
                {
                    Assert.That(player.PlayerState.CurrentState, Is.EqualTo(PlayerState.EnteredWorld));
                    Assert.That(player.TradingPartner, Is.Null);
                    Assert.That(player.BackupInventory, Is.Null);
                }
            });
        }
        finally
        {
            await DisposeAllAsync(owner, oldPeer, newPeer).ConfigureAwait(false);
        }
    }

    /// <summary>
    /// Two players request a trade with each other at the same time, each through its own packet funnel. Each attempt
    /// ends with either one visible pair (one request succeeded) or both players free; never with a pair which both
    /// requests refused, because then neither player was shown the request. The order of the tasks isn't forced; a
    /// green run doesn't prove that the contended interleaving occurred.
    /// </summary>
    [Test]
    public async Task MutualRequestsDontLeaveHiddenPairAsync()
    {
        var a = await PlayerTestHelper.CreatePlayerAsync().ConfigureAwait(false);
        Player? other = null;
        try
        {
            other = await PlayerTestHelper.CreatePlayerAsync(a.GameContext).ConfigureAwait(false);
            var b = other;
            int visible = 0, refused = 0, hidden = 0;
            for (var attempt = 0; attempt < 32; attempt++)
            {
                Task<bool> fromA;
                Task<bool> fromB;
                if (attempt % 2 == 0)
                {
                    fromA = RunAsync(a, () => new TradeRequestAction().RequestTradeAsync(a, b));
                    fromB = RunAsync(b, () => new TradeRequestAction().RequestTradeAsync(b, a));
                }
                else
                {
                    fromB = RunAsync(b, () => new TradeRequestAction().RequestTradeAsync(b, a));
                    fromA = RunAsync(a, () => new TradeRequestAction().RequestTradeAsync(a, b));
                }

                var results = await Task.WhenAll(fromA, fromB).WaitAsync(TimeSpan.FromSeconds(5)).ConfigureAwait(false);
                var pair = a.PlayerState.CurrentState == PlayerState.TradeRequested && b.PlayerState.CurrentState == PlayerState.TradeRequested
                    && ReferenceEquals(a.TradingPartner, b) && ReferenceEquals(b.TradingPartner, a);
                var free = IsFree(a) && IsFree(b);
                var successes = results.Count(r => r);
                if (successes == 1 && pair)
                {
                    visible++;
                }
                else if (successes == 0 && free)
                {
                    refused++;
                }
                else
                {
                    hidden++;
                    TestContext.Out.WriteLine($"attempt {attempt}: A to B {results[0]}, B to A {results[1]}; A {a.PlayerState.CurrentState}/{a.TradingPartner is not null}, B {b.PlayerState.CurrentState}/{b.TradingPartner is not null}");
                }

                await EndTradesAsync(a, b).ConfigureAwait(false);
                Assert.That(IsFree(a) && IsFree(b), Is.True, $"attempt {attempt}: the ordinary cancel ends every state");
            }

            TestContext.Out.WriteLine($"visible pairs {visible}, both refused {refused}, hidden pairs {hidden}");
            Assert.That(hidden, Is.Zero, "a pair which both requests refused stays hidden from both players");
        }
        finally
        {
            await DisposeAllAsync(a, other).ConfigureAwait(false);
        }
    }

    /// <summary>A stale half created by a real late logout invitation must not cancel a successor pair at lifecycle entry.</summary>
    /// <param name="opened">Whether the obsolete invitation was opened before logout completed.</param>
    /// <param name="trigger">Lifecycle cleanup, explicit cancel baseline witness, or no-cleanup control.</param>
    [TestCase(false, "death")]
    [TestCase(false, "logout")]
    [TestCase(false, "placement")]
    [TestCase(false, "cancel")]
    [TestCase(false, "none")]
    [TestCase(true, "death")]
    [TestCase(true, "logout")]
    [TestCase(true, "placement")]
    [TestCase(true, "cancel")]
    [TestCase(true, "none")]
    public async Task StaleLifecycleEntryPreservesSuccessorTradeAsync(bool opened, string trigger)
    {
        var owner = await PlayerTestHelper.CreatePlayerAsync().ConfigureAwait(false);
        Player? stale = null;
        Player? successor = null;
        var release = new TaskCompletionSource(TaskCreationOptions.RunContinuationsAsynchronously);
        var logout = Task.CompletedTask;
        var opening = Task.CompletedTask;
        try
        {
            stale = await PlayerTestHelper.CreatePlayerAsync(owner.GameContext).ConfigureAwait(false);
            successor = await PlayerTestHelper.CreatePlayerAsync(owner.GameContext).ConfigureAwait(false);
            var (a, b, c) = (stale, owner, successor);
            var staleCharacter = a.SelectedCharacter!;
            var character = PrepareOwner(b);
            var removalEntered = HoldAtMapRemoval(b, release);
            b.Money = 200;
            a.Money = 100;
            c.Money = 300;
            var x = await AddItemAsync(a).ConfigureAwait(false);
            var y = await AddItemAsync(b).ConfigureAwait(false);
            var z = await AddItemAsync(c).ConfigureAwait(false);
            logout = RunAsync(b, () => new LogoutAction().LogoutAsync(b, LogoutType.BackToCharacterSelection));
            Assert.That(await Task.WhenAny(removalEntered, logout).WaitAsync(TimeSpan.FromSeconds(5)).ConfigureAwait(false), Is.SameAs(removalEntered));
            Assert.That(await RunAsync(a, () => new TradeRequestAction().RequestTradeAsync(a, b)).WaitAsync(TimeSpan.FromSeconds(5)).ConfigureAwait(false), Is.True);
            if (opened)
            {
                var firstOpened = new TaskCompletionSource(TaskCreationOptions.RunContinuationsAsynchronously);
                Mock.Get(a.ViewPlugIns.GetPlugIn<IShowTradeRequestAnswerPlugIn>()!).Setup(v => v.ShowTradeRequestAnswerAsync(true)).Returns(() =>
                {
                    firstOpened.TrySetResult();
                    return ValueTask.CompletedTask;
                });
                opening = RunAsync(a, () => new TradeAcceptAction().HandleTradeAcceptAsync(a, true));
                await firstOpened.Task.WaitAsync(TimeSpan.FromSeconds(5)).ConfigureAwait(false);
            }

            release.TrySetResult();
            await logout.WaitAsync(TimeSpan.FromSeconds(5)).ConfigureAwait(false);
            await opening.WaitAsync(TimeSpan.FromSeconds(5)).ConfigureAwait(false);
            await ReselectAsync(b, character).ConfigureAwait(false);
            Assert.That(a.PlayerState.CurrentState, Is.EqualTo(opened ? PlayerState.TradeOpened : PlayerState.TradeRequested));
            Assert.That(a.TradingPartner, Is.SameAs(b));
            Assert.That(await RunAsync(b, () => new TradeRequestAction().RequestTradeAsync(b, c)).ConfigureAwait(false), Is.True);
            await RunAsync(c, () => new TradeAcceptAction().HandleTradeAcceptAsync(c, true)).ConfigureAwait(false);
            await RunAsync(b, async () =>
            {
                await new MoveItemAction().MoveItemAsync(b, 20, Storages.Inventory, 0, Storages.Trade).ConfigureAwait(false);
                await new TradeMoneyAction().TradeMoneyAsync(b, 60).ConfigureAwait(false);
            }).ConfigureAwait(false);
            await RunAsync(c, async () =>
            {
                await new MoveItemAction().MoveItemAsync(c, 20, Storages.Inventory, 0, Storages.Trade).ConfigureAwait(false);
                await new TradeMoneyAction().TradeMoneyAsync(c, 25).ConfigureAwait(false);
            }).ConfigureAwait(false);
            var bBackup = b.BackupInventory;
            var cBackup = c.BackupInventory;
            var bFinished = Mock.Get(b.ViewPlugIns.GetPlugIn<ITradeFinishedPlugIn>()!);
            var cFinished = Mock.Get(c.ViewPlugIns.GetPlugIn<ITradeFinishedPlugIn>()!);
            bFinished.Invocations.Clear();
            cFinished.Invocations.Clear();
            Assert.Multiple(() =>
            {
                Assert.That(b.TradingPartner, Is.SameAs(c));
                Assert.That(c.TradingPartner, Is.SameAs(b));
                Assert.That(b.TemporaryStorage!.Items, Does.Contain(y));
                Assert.That(c.TemporaryStorage!.Items, Does.Contain(z));
            });
            if (trigger == "death")
            {
                await a.KillInstantlyAsync().AsTask().WaitAsync(TimeSpan.FromSeconds(5)).ConfigureAwait(false);
            }
            else if (trigger == "logout")
            {
                await RunAsync(a, () => new LogoutAction().LogoutAsync(a, LogoutType.BackToCharacterSelection)).WaitAsync(TimeSpan.FromSeconds(5)).ConfigureAwait(false);
            }
            else if (trigger == "placement")
            {
                await RunAsync(a, () => a.RespawnAtAsync(new MUnique.OpenMU.DataModel.Configuration.ExitGate { Map = a.CurrentMap!.Definition, X1 = 1, X2 = 1, Y1 = 1, Y2 = 1 })).WaitAsync(TimeSpan.FromSeconds(5)).ConfigureAwait(false);
            }
            else if (trigger == "cancel")
            {
                await RunAsync(a, () => new TradeCancelAction().CancelTradeAsync(a)).WaitAsync(TimeSpan.FromSeconds(5)).ConfigureAwait(false);
            }
            else
            {
                // Control: keep the stale half inert while the successor pair completes normally.
            }

            Assert.Multiple(() =>
            {
                Assert.That(b.PlayerState.CurrentState, Is.EqualTo(PlayerState.TradeOpened), "Stale lifecycle cleanup must not leave the successor peer behind.");
                Assert.That(c.PlayerState.CurrentState, Is.EqualTo(PlayerState.TradeOpened));
                Assert.That(b.TradingPartner, Is.SameAs(c));
                Assert.That(c.TradingPartner, Is.SameAs(b));
                Assert.That(b.BackupInventory, Is.SameAs(bBackup));
                Assert.That(c.BackupInventory, Is.SameAs(cBackup));
                Assert.That(b.TradingMoney, Is.EqualTo(60));
                Assert.That(c.TradingMoney, Is.EqualTo(25));
                Assert.That(b.TemporaryStorage!.Items, Does.Contain(y));
                Assert.That(c.TemporaryStorage!.Items, Does.Contain(z));
                Assert.That(Count(new[] { a, b, c }, x), Is.EqualTo(1));
                Assert.That(Count(new[] { a, b, c }, y), Is.EqualTo(1));
                Assert.That(Count(new[] { a, b, c }, z), Is.EqualTo(1));
                bFinished.Verify(v => v.TradeFinishedAsync(TradeResult.Cancelled), Times.Never);
                cFinished.Verify(v => v.TradeFinishedAsync(TradeResult.Cancelled), Times.Never);
            });
            await RunAsync(b, () => new TradeButtonAction().TradeButtonChangedAsync(b, TradeButtonState.Checked)).ConfigureAwait(false);
            await RunAsync(c, () => new TradeButtonAction().TradeButtonChangedAsync(c, TradeButtonState.Checked)).ConfigureAwait(false);
            Assert.Multiple(() =>
            {
                Assert.That(b.Inventory!.Items, Does.Contain(z));
                Assert.That(c.Inventory!.Items, Does.Contain(y));
                Assert.That(b.Money, Is.EqualTo(165));
                Assert.That(c.Money, Is.EqualTo(335));
                Assert.That(staleCharacter.Inventory!.Money, Is.EqualTo(100));
                bFinished.Verify(v => v.TradeFinishedAsync(TradeResult.Success), Times.Once);
                cFinished.Verify(v => v.TradeFinishedAsync(TradeResult.Success), Times.Once);
            });
        }
        finally
        {
            release.TrySetResult();
            await WhenEndedAsync(logout).ConfigureAwait(false);
            await WhenEndedAsync(opening).ConfigureAwait(false);
            if (stale is not null && successor is not null)
            {
                await EndTradesAsync(successor, stale, owner).ConfigureAwait(false);
            }

            await DisposeAllAsync(owner, stale, successor).ConfigureAwait(false);
        }
    }

    private static bool IsFree(Player player)
        => player.PlayerState.CurrentState == PlayerState.EnteredWorld && player.TradingPartner is null;

    private static Character PrepareOwner(Player owner)
    {
        owner.GameContext.Configuration.MaximumInventoryMoney = 1_000_000;
        var character = owner.SelectedCharacter!;
        character.Name = "StaleA";
        Mock.Get(owner.Account!).Setup(a => a.Characters).Returns(new List<Character> { character });
        return character;
    }

    /// <summary>
    /// Holds the removal of the owner from its map, which a logout does after it closed the trades of the owner and
    /// before it restores the inventory; the owner is still selected and alive there.
    /// </summary>
    private static Task HoldAtMapRemoval(Player owner, TaskCompletionSource release)
    {
        var entered = new TaskCompletionSource(TaskCreationOptions.RunContinuationsAsynchronously);
        owner.CurrentMap!.ObjectRemoved += async removal =>
        {
            if (ReferenceEquals(removal.Object, owner))
            {
                entered.TrySetResult();
                await release.Task.ConfigureAwait(false);
            }
        };

        return entered.Task;
    }

    private static async Task ReselectAsync(Player owner, Character character)
    {
        await RunAsync(owner, () => new RequestCharacterListAction().RequestCharacterListAsync(owner)).ConfigureAwait(false);
        await RunAsync(owner, () => new SelectCharacterAction().SelectCharacterAsync(owner, character.Name)).ConfigureAwait(false);
        Assert.That(owner.PlayerState.CurrentState, Is.EqualTo(PlayerState.EnteredWorld));
    }

    /// <summary>Waits bounded for the given actions, whatever their outcome, so that cleanup doesn't overlap them.</summary>
    private static Task WhenEndedAsync(params Task[] tasks)
        => Task.WhenAll(tasks.Select(t => t.ContinueWith(_ => { }, TaskScheduler.Default))).WaitAsync(TimeSpan.FromSeconds(5));

    /// <summary>
    /// Ends every trade state through the ordinary cancel of each player, so that a failed assertion doesn't leave a
    /// trade behind. Bounded: one cancel per player and round, at most three rounds.
    /// </summary>
    private static async Task EndTradesAsync(params Player[] players)
    {
        for (var round = 0; round < 3; round++)
        {
            foreach (var player in players)
            {
                var state = player.PlayerState.CurrentState;
                if (state == PlayerState.TradeRequested || state == PlayerState.TradeOpened || state == PlayerState.TradeButtonPressed)
                {
                    await RunAsync(player, () => new TradeCancelAction().CancelTradeAsync(player)).WaitAsync(TimeSpan.FromSeconds(5)).ConfigureAwait(false);
                }
            }
        }
    }

    private static async Task DisposeAllAsync(params Player?[] players)
    {
        var errors = new List<Exception>();
        foreach (var player in players)
        {
            if (player is null)
            {
                continue;
            }

            try
            {
                await player.DisposeAsync().ConfigureAwait(false);
            }
            catch (Exception ex)
            {
                errors.Add(ex);
            }
        }

        if (errors.Count > 0)
        {
            throw new AggregateException(errors);
        }
    }

    private static int Count(IEnumerable<Player> players, Item item)
        => players.Sum(p => (p.Inventory?.Items.Count(i => ReferenceEquals(i, item)) ?? 0)
            + (p.TemporaryStorage?.Items.Count(i => ReferenceEquals(i, item)) ?? 0));

    private static async Task<Item> AddItemAsync(Player owner)
    {
        var item = owner.PersistenceContext.CreateNew<Item>();
        item.Definition = new ItemDefinition { Width = 1, Height = 1, IsTradable = true };
        Assert.That(await owner.Inventory!.AddItemAsync(20, item).ConfigureAwait(false), Is.True);
        return item;
    }

    private static Task RunAsync(Player player, Func<ValueTask> action)
        => Task.Run(async () => await player.RunPersistenceExclusiveAsync(action).ConfigureAwait(false));

    private static Task<T> RunAsync<T>(Player player, Func<ValueTask<T>> action)
        => Task.Run(async () => await player.RunPersistenceExclusiveAsync(action).ConfigureAwait(false));
}
