// <copyright file="LogoutReselectionTradeTests.cs" company="MUnique">
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
/// A player who logs out of the world during a trade, or while a trade request reaches it, and selects a character
/// again can trade in both directions in the same session; no item or money is lost.
/// </summary>
[TestFixture]
public class LogoutReselectionTradeTests
{
    /// <summary>
    /// After a logout during a trade, the peer's cancel while the owner is at the character selection and the
    /// reselection of the character, trades work in both directions; items and money are conserved and the logout
    /// doesn't bring the owner back into the world.
    /// </summary>
    /// <param name="phase">The ordinary trade state in which logout is requested.</param>
    [TestCase("requested")]
    [TestCase("opened")]
    [TestCase("pressed")]
    public async Task LogoutDuringTradeKeepsTradingAfterReselectionAsync(string phase)
    {
        var owner = await PlayerTestHelper.CreatePlayerAsync().ConfigureAwait(false);
        Player? peer = null;
        try
        {
            peer = await PlayerTestHelper.CreatePlayerAsync(owner.GameContext).ConfigureAwait(false);
            var character = PrepareOwner(owner, peer);
            var item = CreateItem(owner);
            Assert.That(await owner.Inventory!.AddItemAsync(20, item).ConfigureAwait(false), Is.True);
            var tradePeer = peer;
            Assert.That(await RunAsync(owner, () => new TradeRequestAction().RequestTradeAsync(owner, tradePeer)).ConfigureAwait(false), Is.True);
            if (phase != "requested")
            {
                await RunAsync(tradePeer, () => new TradeAcceptAction().HandleTradeAcceptAsync(tradePeer, true)).ConfigureAwait(false);
                await RunAsync(owner, async () =>
                {
                    await new MoveItemAction().MoveItemAsync(owner, 20, Storages.Inventory, 0, Storages.Trade).ConfigureAwait(false);
                    await new TradeMoneyAction().TradeMoneyAsync(owner, 60).ConfigureAwait(false);
                    if (phase == "pressed")
                    {
                        await new TradeButtonAction().TradeButtonChangedAsync(owner, TradeButtonState.Checked).ConfigureAwait(false);
                    }
                }).ConfigureAwait(false);
                Assert.That(owner.PlayerState.CurrentState, Is.EqualTo(phase == "pressed" ? PlayerState.TradeButtonPressed : PlayerState.TradeOpened));
            }

            await RunAsync(owner, () => new LogoutAction().LogoutAsync(owner, LogoutType.BackToCharacterSelection)).ConfigureAwait(false);
            Assert.That(owner.PlayerState.CurrentState, Is.EqualTo(PlayerState.Authenticated));
            await AssertPeerCancelAndReselectionAsync(owner, tradePeer, character, item).ConfigureAwait(false);
            await AssertTradesWorkInBothDirectionsAsync(owner, tradePeer, item).ConfigureAwait(false);
        }
        finally
        {
            try
            {
                await owner.DisposeAsync().ConfigureAwait(false);
            }
            finally
            {
                if (peer is not null)
                {
                    await peer.DisposeAsync().ConfigureAwait(false);
                }
            }
        }
    }

    /// <summary>
    /// An ordinary trade request which reaches a player while its logout removes it from the map (after any cleanup
    /// at the start of the logout) doesn't block trades after the reselection of the character.
    /// </summary>
    [Test]
    public async Task RequestDuringLogoutKeepsTradingAfterReselectionAsync()
    {
        var owner = await PlayerTestHelper.CreatePlayerAsync().ConfigureAwait(false);
        Player? peer = null;
        var removalRelease = new TaskCompletionSource(TaskCreationOptions.RunContinuationsAsynchronously);
        var logout = Task.CompletedTask;
        try
        {
            peer = await PlayerTestHelper.CreatePlayerAsync(owner.GameContext).ConfigureAwait(false);
            var character = PrepareOwner(owner, peer);
            var item = CreateItem(owner);
            Assert.That(await owner.Inventory!.AddItemAsync(20, item).ConfigureAwait(false), Is.True);
            var map = owner.CurrentMap!;
            var removalEntered = new TaskCompletionSource(TaskCreationOptions.RunContinuationsAsynchronously);
            map.ObjectRemoved += async removal =>
            {
                if (ReferenceEquals(removal.Object, owner))
                {
                    removalEntered.TrySetResult();
                    await removalRelease.Task.ConfigureAwait(false);
                }
            };

            var tradePeer = peer;
            logout = RunAsync(owner, () => new LogoutAction().LogoutAsync(owner, LogoutType.BackToCharacterSelection));
            await removalEntered.Task.WaitAsync(TimeSpan.FromSeconds(5)).ConfigureAwait(false);

            // The request uses only the peer's packet funnel; the logout holds the owner's.
            var lateRequest = await RunAsync(tradePeer, () => new TradeRequestAction().RequestTradeAsync(tradePeer, owner)).WaitAsync(TimeSpan.FromSeconds(5)).ConfigureAwait(false);
            TestContext.Out.WriteLine($"Request during logout: {lateRequest}, owner state {owner.PlayerState.CurrentState}");
            removalRelease.TrySetResult();
            await logout.WaitAsync(TimeSpan.FromSeconds(5)).ConfigureAwait(false);
            Assert.That(owner.PlayerState.CurrentState, Is.EqualTo(PlayerState.Authenticated));
            await AssertPeerCancelAndReselectionAsync(owner, tradePeer, character, item).ConfigureAwait(false);
            await AssertTradesWorkInBothDirectionsAsync(owner, tradePeer, item).ConfigureAwait(false);
        }
        finally
        {
            removalRelease.TrySetResult();
            try
            {
                await logout.WaitAsync(TimeSpan.FromSeconds(5)).ConfigureAwait(false);
            }
            finally
            {
                try
                {
                    await owner.DisposeAsync().ConfigureAwait(false);
                }
                finally
                {
                    if (peer is not null)
                    {
                        await peer.DisposeAsync().ConfigureAwait(false);
                    }
                }
            }
        }
    }

    private static Character PrepareOwner(Player owner, Player peer)
    {
        owner.GameContext.Configuration.MaximumInventoryMoney = 1_000_000;
        var character = owner.SelectedCharacter!;
        character.Name = "LogoutA";
        Mock.Get(owner.Account!).Setup(a => a.Characters).Returns(new List<Character> { character });
        owner.Money = 200;
        peer.Money = 300;
        return character;
    }

    private static async Task AssertPeerCancelAndReselectionAsync(Player owner, Player peer, Character character, Item item)
    {
        // The peer leaves the request or trade with its ordinary cancel while the owner is at the character selection.
        await RunAsync(peer, () => new TradeCancelAction().CancelTradeAsync(peer)).ConfigureAwait(false);
        Assert.Multiple(() =>
        {
            Assert.That(peer.PlayerState.CurrentState, Is.EqualTo(PlayerState.EnteredWorld));
            Assert.That(peer.TradingPartner, Is.Null);
            Assert.That(peer.BackupInventory, Is.Null);
            Assert.That(peer.Money, Is.EqualTo(300));
            Assert.That(owner.PlayerState.CurrentState, Is.EqualTo(PlayerState.Authenticated), "The peer's cancel doesn't bring the logged-out owner back.");
            Assert.That(owner.SelectedCharacter, Is.Null);
            Assert.That(character.Inventory!.Items.Count(i => ReferenceEquals(i, item)), Is.EqualTo(1));
            Assert.That(character.Inventory.Money, Is.EqualTo(200));
        });

        await RunAsync(owner, () => new RequestCharacterListAction().RequestCharacterListAsync(owner)).ConfigureAwait(false);
        await RunAsync(owner, () => new SelectCharacterAction().SelectCharacterAsync(owner, character.Name)).ConfigureAwait(false);
        Assert.Multiple(() =>
        {
            Assert.That(owner.PlayerState.CurrentState, Is.EqualTo(PlayerState.EnteredWorld));
            Assert.That(owner.IsAlive, Is.True);
            Assert.That(owner.Inventory!.Items.Count(i => ReferenceEquals(i, item)), Is.EqualTo(1));
            Assert.That(owner.Money, Is.EqualTo(200));
        });
    }

    private static async Task AssertTradesWorkInBothDirectionsAsync(Player owner, Player peer, Item item)
    {
        // Peer to owner: requested, opened by the owner, cancelled by the owner.
        Assert.That(await RunAsync(peer, () => new TradeRequestAction().RequestTradeAsync(peer, owner)).ConfigureAwait(false), Is.True, "The peer can request a trade with the reselected owner.");
        Assert.Multiple(() =>
        {
            Assert.That(owner.PlayerState.CurrentState, Is.EqualTo(PlayerState.TradeRequested));
            Assert.That(peer.PlayerState.CurrentState, Is.EqualTo(PlayerState.TradeRequested));
            Assert.That(owner.TradingPartner, Is.SameAs(peer));
            Assert.That(peer.TradingPartner, Is.SameAs(owner));
        });
        await RunAsync(owner, () => new TradeAcceptAction().HandleTradeAcceptAsync(owner, true)).ConfigureAwait(false);
        Assert.Multiple(() =>
        {
            Assert.That(owner.PlayerState.CurrentState, Is.EqualTo(PlayerState.TradeOpened));
            Assert.That(peer.PlayerState.CurrentState, Is.EqualTo(PlayerState.TradeOpened));
            Assert.That(owner.TradingMoney, Is.Zero);
            Assert.That(owner.BackupInventory, Is.Not.Null);
        });
        await RunAsync(owner, () => new TradeCancelAction().CancelTradeAsync(owner)).ConfigureAwait(false);
        AssertTradeEnded(owner, peer, item);

        // Owner to peer: requested by the owner, declined by the peer.
        Assert.That(await RunAsync(owner, () => new TradeRequestAction().RequestTradeAsync(owner, peer)).ConfigureAwait(false), Is.True, "The reselected owner can request a trade.");
        Assert.Multiple(() =>
        {
            Assert.That(owner.PlayerState.CurrentState, Is.EqualTo(PlayerState.TradeRequested));
            Assert.That(peer.PlayerState.CurrentState, Is.EqualTo(PlayerState.TradeRequested));
            Assert.That(owner.TradingPartner, Is.SameAs(peer));
            Assert.That(peer.TradingPartner, Is.SameAs(owner));
        });
        await RunAsync(peer, () => new TradeAcceptAction().HandleTradeAcceptAsync(peer, false)).ConfigureAwait(false);
        AssertTradeEnded(owner, peer, item);
    }

    private static void AssertTradeEnded(Player owner, Player peer, Item item)
    {
        Assert.Multiple(() =>
        {
            Assert.That(owner.PlayerState.CurrentState, Is.EqualTo(PlayerState.EnteredWorld));
            Assert.That(peer.PlayerState.CurrentState, Is.EqualTo(PlayerState.EnteredWorld));
            Assert.That(owner.TradingPartner, Is.Null);
            Assert.That(peer.TradingPartner, Is.Null);
            Assert.That(owner.BackupInventory, Is.Null);
            Assert.That(peer.BackupInventory, Is.Null);
            Assert.That(owner.Money, Is.EqualTo(200));
            Assert.That(peer.Money, Is.EqualTo(300));
            Assert.That(owner.Inventory!.Items.Count(i => ReferenceEquals(i, item)), Is.EqualTo(1));
            Assert.That(peer.Inventory!.Items.Any(i => ReferenceEquals(i, item)), Is.False);
        });
    }

    private static Task RunAsync(Player player, Func<ValueTask> action)
        => Task.Run(async () => await player.RunPersistenceExclusiveAsync(action).ConfigureAwait(false));

    private static Task<T> RunAsync<T>(Player player, Func<ValueTask<T>> action)
        => Task.Run(async () => await player.RunPersistenceExclusiveAsync(action).ConfigureAwait(false));

    private static Item CreateItem(Player owner)
    {
        var item = owner.PersistenceContext.CreateNew<Item>();
        item.Definition = new ItemDefinition { Width = 1, Height = 1, IsTradable = true };
        return item;
    }
}
