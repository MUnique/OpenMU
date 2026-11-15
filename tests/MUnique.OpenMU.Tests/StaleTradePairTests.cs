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
/// </summary>
[TestFixture]
public class StaleTradePairTests
{
    /// <summary>
    /// The old partner of an opened trade presses its trade button after the owner opened a new trade, offered an
    /// item and money and pressed its own button. Every item exists exactly once, at its owner after the new trade,
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
            owner.Money = 200;
            p.Money = 100;
            d.Money = 300;
            var x = await AddItemAsync(p).ConfigureAwait(false);
            var y = await AddItemAsync(owner).ConfigureAwait(false);
            var z = await AddItemAsync(d).ConfigureAwait(false);
            try
            {
                // The old trade: opened, the old peer offers its item, then the owner logs out and selects its character again.
                Assert.That(await RunAsync(owner, () => new TradeRequestAction().RequestTradeAsync(owner, p)).ConfigureAwait(false), Is.True);
                await RunAsync(p, () => new TradeAcceptAction().HandleTradeAcceptAsync(p, true)).ConfigureAwait(false);
                Assert.That(p.PlayerState.CurrentState, Is.EqualTo(PlayerState.TradeOpened));
                await RunAsync(p, () => new MoveItemAction().MoveItemAsync(p, 20, Storages.Inventory, 0, Storages.Trade)).ConfigureAwait(false);
                await LogoutAndReselectAsync(owner, character).ConfigureAwait(false);
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
            owner.Money = 200;
            p.Money = 100;
            c.Money = 300;
            var y = await AddItemAsync(owner).ConfigureAwait(false);
            try
            {
                Assert.That(await RunAsync(owner, () => new TradeRequestAction().RequestTradeAsync(owner, p)).ConfigureAwait(false), Is.True);
                await LogoutAndReselectAsync(owner, character).ConfigureAwait(false);
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

    private static async Task LogoutAndReselectAsync(Player owner, Character character)
    {
        await RunAsync(owner, () => new LogoutAction().LogoutAsync(owner, LogoutType.BackToCharacterSelection)).ConfigureAwait(false);
        Assert.That(owner.PlayerState.CurrentState, Is.EqualTo(PlayerState.Authenticated));
        await RunAsync(owner, () => new RequestCharacterListAction().RequestCharacterListAsync(owner)).ConfigureAwait(false);
        await RunAsync(owner, () => new SelectCharacterAction().SelectCharacterAsync(owner, character.Name)).ConfigureAwait(false);
        Assert.That(owner.PlayerState.CurrentState, Is.EqualTo(PlayerState.EnteredWorld));
    }

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
