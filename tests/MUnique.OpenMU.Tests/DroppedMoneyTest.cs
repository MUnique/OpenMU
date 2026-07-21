// <copyright file="DroppedMoneyTest.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.Tests;

using MUnique.OpenMU.GameLogic;
using MUnique.OpenMU.Pathfinding;
using NUnit.Framework;

/// <summary>
/// Tests for the <see cref="DroppedMoney"/>.
/// </summary>
[TestFixture]
public class DroppedMoneyTest
{
    private const uint DroppedAmount = 1000;

    /// <summary>
    /// Tests that a failed pick up doesn't consume the drop, so it's still available for the next player.
    /// A player at the maximum inventory money can't take it, and used to claim it nevertheless - which left
    /// it lying on the map, unpickable for everyone until it expired.
    /// </summary>
    [Test]
    public async Task FailedPickUpKeepsMoneyAvailableAsync()
    {
        var player = await PlayerTestHelper.CreatePlayerAsync().ConfigureAwait(false);
        player.GameContext.Configuration.MaximumInventoryMoney = int.MaxValue;
        var maximumMoney = player.GameContext.Configuration.MaximumInventoryMoney;
        player.Money = maximumMoney;

        var money = new DroppedMoney(DroppedAmount, new Point(100, 100), player.CurrentMap!);

        Assert.That(await money.TryPickUpByAsync(player).ConfigureAwait(false), Is.False);
        Assert.That(player.Money, Is.EqualTo(maximumMoney));

        var otherPlayer = await PlayerTestHelper.CreatePlayerAsync(player.GameContext).ConfigureAwait(false);
        otherPlayer.Money = 0;

        Assert.That(await money.TryPickUpByAsync(otherPlayer).ConfigureAwait(false), Is.True);
        Assert.That(otherPlayer.Money, Is.EqualTo((int)DroppedAmount));
    }

    /// <summary>
    /// Tests that a successful pick up still consumes the drop, so it can't be picked up twice.
    /// </summary>
    [Test]
    public async Task SuccessfulPickUpConsumesMoneyAsync()
    {
        var player = await PlayerTestHelper.CreatePlayerAsync().ConfigureAwait(false);
        player.GameContext.Configuration.MaximumInventoryMoney = int.MaxValue;
        player.Money = 0;

        var money = new DroppedMoney(DroppedAmount, new Point(100, 100), player.CurrentMap!);

        Assert.That(await money.TryPickUpByAsync(player).ConfigureAwait(false), Is.True);
        Assert.That(player.Money, Is.EqualTo((int)DroppedAmount));

        var otherPlayer = await PlayerTestHelper.CreatePlayerAsync(player.GameContext).ConfigureAwait(false);
        otherPlayer.Money = 0;

        Assert.That(await money.TryPickUpByAsync(otherPlayer).ConfigureAwait(false), Is.False);
        Assert.That(otherPlayer.Money, Is.EqualTo(0));
    }
}
