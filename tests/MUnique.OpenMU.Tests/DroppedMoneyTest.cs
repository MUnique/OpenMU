// <copyright file="DroppedMoneyTest.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.Tests;

using Microsoft.Extensions.Logging.Abstractions;
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

    /// <summary>
    /// Tests that money earned by one party can still be picked up by a stranger's party - money has no
    /// owner - and is then split equally between the picking party, because there is no experience of theirs
    /// to follow. The earner, who is not in the picking party, receives nothing.
    /// </summary>
    [Test]
    public async Task StrangerPartySplitsPickedUpMoneyEquallyAsync()
    {
        var earner = await PlayerTestHelper.CreatePlayerAsync().ConfigureAwait(false);
        var gameContext = earner.GameContext;
        gameContext.Configuration.MaximumInventoryMoney = int.MaxValue;

        var strangerParty = CreateParty();
        var stranger1 = await AddPartyMemberAsync(gameContext, strangerParty).ConfigureAwait(false);
        var stranger2 = await AddPartyMemberAsync(gameContext, strangerParty).ConfigureAwait(false);

        // The money was earned by 'earner', who is in neither of the picking party's members.
        var shares = new[] { new MoneyShare(earner, DroppedAmount) };
        var money = new DroppedMoney(DroppedAmount, new Point(100, 100), stranger1.CurrentMap!, shares);

        Assert.That(await money.TryPickUpByAsync(stranger1).ConfigureAwait(false), Is.True);
        Assert.That(earner.Money, Is.EqualTo(0));
        Assert.That(stranger1.Money + stranger2.Money, Is.EqualTo((int)DroppedAmount));
        Assert.That(stranger1.Money, Is.EqualTo((int)DroppedAmount / 2));
        Assert.That(stranger2.Money, Is.EqualTo((int)DroppedAmount / 2));
    }

    /// <summary>
    /// Tests that when the earning party picks its own money up, it follows the recorded shares
    /// (proportional to the experience each member gained) instead of being split equally.
    /// </summary>
    [Test]
    public async Task EarningPartyReceivesMoneyByExperienceShareAsync()
    {
        var big = await PlayerTestHelper.CreatePlayerAsync().ConfigureAwait(false);
        var gameContext = big.GameContext;
        gameContext.Configuration.MaximumInventoryMoney = int.MaxValue;

        var party = CreateParty();
        await party.AddAsync(big).ConfigureAwait(false);
        var small = await AddPartyMemberAsync(gameContext, party).ConfigureAwait(false);

        var shares = new[]
        {
            new MoneyShare(big, 800),
            new MoneyShare(small, 200),
        };
        var money = new DroppedMoney(DroppedAmount, new Point(100, 100), big.CurrentMap!, shares);

        Assert.That(await money.TryPickUpByAsync(big).ConfigureAwait(false), Is.True);
        Assert.That(big.Money, Is.EqualTo(800));
        Assert.That(small.Money, Is.EqualTo(200));
    }

    private static Party CreateParty()
    {
        // The party manager of the lightweight test context has a maximum size of zero, which would
        // reject every member. A stand-alone party with a real size is enough for the money distribution.
        var partyManager = new PartyManager(5, new NullLogger<Party>());
        return new Party(partyManager, 5, new NullLogger<Party>());
    }

    private static async ValueTask<Player> AddPartyMemberAsync(IGameContext gameContext, Party party)
    {
        // The test helper already enters the player into the world, so it shares the live map of the
        // context with the other members created from the same context - which is what the money
        // distribution's eligibility check compares.
        var player = await PlayerTestHelper.CreatePlayerAsync(gameContext).ConfigureAwait(false);
        await party.AddAsync(player).ConfigureAwait(false);
        return player;
    }
}
