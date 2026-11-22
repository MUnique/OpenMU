// <copyright file="PickupMoneyRefusalTest.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.Tests;

using Moq;
using MUnique.OpenMU.GameLogic;
using MUnique.OpenMU.GameLogic.PlayerActions.Items;
using MUnique.OpenMU.GameLogic.Views;
using MUnique.OpenMU.GameLogic.Views.Inventory;
using MUnique.OpenMU.Interfaces;
using MUnique.OpenMU.Pathfinding;

/// <summary>
/// Tests the reply of <see cref="PickupItemAction"/> when a pile of money is refused.
/// </summary>
[TestFixture]
internal class PickupMoneyRefusalTest
{
    private const uint PileAmount = 408967;

    /// <summary>
    /// Tests that a pile which doesn't fit into the inventory money anymore is refused with the general
    /// failure, never with <see cref="ItemPickFailReason.MaximumInventoryMoneyReached"/>, whose code the client
    /// reads as the money update, and that a message tells the player why.
    /// </summary>
    [Test]
    public async ValueTask MoneyLimitRepliesTheGeneralFailureAndAMessageAsync()
    {
        var player = await CreatePlayerAtTheMoneyLimitAsync().ConfigureAwait(false);
        var money = await DropMoneyAsync(player, player.Position).ConfigureAwait(false);

        await new PickupItemAction().PickupItemAsync(player, money.Id).ConfigureAwait(false);

        var failedView = Mock.Get(player.ViewPlugIns.GetPlugIn<IItemPickUpFailedPlugIn>()!);
        failedView.Verify(p => p.ItemPickUpFailedAsync(ItemPickFailReason.General), Times.Once);
        failedView.Verify(p => p.ItemPickUpFailedAsync(ItemPickFailReason.MaximumInventoryMoneyReached), Times.Never);
        var messageView = Mock.Get(player.ViewPlugIns.GetPlugIn<IShowMessagePlugIn>()!);
        messageView.Verify(p => p.ShowMessageAsync(It.IsAny<string>(), MessageType.BlueNormal), Times.Once);
        Assert.That(player.Money, Is.EqualTo(int.MaxValue - 4899));
    }

    /// <summary>
    /// Tests that a pile out of reach is refused with the general failure and without the message of the
    /// money limit, even when the inventory money is at the limit.
    /// </summary>
    [Test]
    public async ValueTask PileOutOfReachRepliesTheGeneralFailureWithoutMessageAsync()
    {
        var player = await CreatePlayerAtTheMoneyLimitAsync().ConfigureAwait(false);
        var farAway = new Point((byte)(player.Position.X + 10), player.Position.Y);
        var money = await DropMoneyAsync(player, farAway).ConfigureAwait(false);

        await new PickupItemAction().PickupItemAsync(player, money.Id).ConfigureAwait(false);

        var failedView = Mock.Get(player.ViewPlugIns.GetPlugIn<IItemPickUpFailedPlugIn>()!);
        failedView.Verify(p => p.ItemPickUpFailedAsync(ItemPickFailReason.General), Times.Once);
        var messageView = Mock.Get(player.ViewPlugIns.GetPlugIn<IShowMessagePlugIn>()!);
        messageView.Verify(p => p.ShowMessageAsync(It.IsAny<string>(), It.IsAny<MessageType>()), Times.Never);
    }

    private static async ValueTask<Player> CreatePlayerAtTheMoneyLimitAsync()
    {
        var player = await PlayerTestHelper.CreatePlayerAsync().ConfigureAwait(false);
        player.GameContext.Configuration.MaximumInventoryMoney = int.MaxValue;
        player.GameContext.Configuration.ClampMoneyOnPickup = false;
        player.Money = int.MaxValue - 4899;
        Assert.That(player.CurrentMap, Is.Not.Null, "test setup: the player needs a map");
        Assert.That(player.IsAlive, Is.True, "test setup: the player must be alive");
        return player;
    }

    private static async ValueTask<DroppedMoney> DropMoneyAsync(Player player, Point position)
    {
        var money = new DroppedMoney(PileAmount, position, player.CurrentMap!);
        await player.CurrentMap!.AddAsync(money).ConfigureAwait(false);
        Assert.That(player.CurrentMap.GetDrop(money.Id), Is.SameAs(money), "test setup: the money needs to be on the map");
        return money;
    }
}
