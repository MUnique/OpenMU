// <copyright file="MoneyDistributionTest.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.Tests;

using Microsoft.Extensions.Logging.Abstractions;
using MUnique.OpenMU.AttributeSystem;
using MUnique.OpenMU.GameLogic;
using MUnique.OpenMU.GameLogic.Attributes;
using MUnique.OpenMU.Pathfinding;
using NUnit.Framework;

/// <summary>
/// Tests for the distribution of a money drop between the players which earned it.
/// </summary>
[TestFixture]
public class MoneyDistributionTest
{
    private static readonly Point DropPosition = new(100, 100);

    /// <summary>
    /// Tests that the money is split proportionally to the experience each player gained,
    /// because the money amount is derived from that experience.
    /// </summary>
    [Test]
    public async Task SharesFollowExperienceProportionsAsync()
    {
        var (first, second) = await CreateTwoPlayersAsync().ConfigureAwait(false);

        var shares = MoneyDistribution.CreateShares(1000, [new ExperienceShare(first, 800), new ExperienceShare(second, 200)]);

        Assert.That(shares.Count, Is.EqualTo(2));
        Assert.That(shares[0].Amount, Is.EqualTo(800u));
        Assert.That(shares[1].Amount, Is.EqualTo(200u));
    }

    /// <summary>
    /// Tests that the remainder of the integer division is not lost, so the shares always add up
    /// to the dropped amount.
    /// </summary>
    [Test]
    public async Task SharesAddUpToTheDroppedAmountAsync()
    {
        var (first, second) = await CreateTwoPlayersAsync().ConfigureAwait(false);
        var third = await PlayerTestHelper.CreatePlayerAsync(first.GameContext).ConfigureAwait(false);

        var shares = MoneyDistribution.CreateShares(10, [new ExperienceShare(first, 1), new ExperienceShare(second, 1), new ExperienceShare(third, 1)]);

        Assert.That(shares.Sum(s => (long)s.Amount), Is.EqualTo(10));
    }

    /// <summary>
    /// Tests that the units lost to the integer division are spread over the shares which were cut
    /// the most, instead of always landing on the same player - which would favour them over many kills.
    /// </summary>
    [Test]
    public async Task RemainderIsSpreadInsteadOfAlwaysGoingToTheSamePlayerAsync()
    {
        var (first, second) = await CreateTwoPlayersAsync().ConfigureAwait(false);
        var third = await PlayerTestHelper.CreatePlayerAsync(first.GameContext).ConfigureAwait(false);

        // 52 split between three equal contributors: 17 each leaves a remainder of 1.
        var shares = MoneyDistribution.CreateShares(52, [new ExperienceShare(first, 9), new ExperienceShare(second, 9), new ExperienceShare(third, 9)]);

        Assert.That(shares.Sum(s => (long)s.Amount), Is.EqualTo(52));
        Assert.That(shares.Max(s => s.Amount) - shares.Min(s => s.Amount), Is.LessThanOrEqualTo(1u));
    }

    /// <summary>
    /// Tests that a player who gained no experience from the kill gets no money from it.
    /// </summary>
    [Test]
    public async Task PlayerWithoutExperienceGetsNoShareAsync()
    {
        var (first, second) = await CreateTwoPlayersAsync().ConfigureAwait(false);

        var shares = MoneyDistribution.CreateShares(8, [new ExperienceShare(first, 0), new ExperienceShare(second, 1)]);

        Assert.That(shares[0].Amount, Is.EqualTo(0u));
        Assert.That(shares[1].Amount, Is.EqualTo(8u));
    }

    /// <summary>
    /// Tests that a player without a party gets the money multiplied by their money rate.
    /// </summary>
    [Test]
    public async Task SoloPickUpAppliesMoneyRateAsync()
    {
        var player = await PlayerTestHelper.CreatePlayerAsync().ConfigureAwait(false);
        player.GameContext.Configuration.MaximumInventoryMoney = int.MaxValue;
        player.Money = 0;
        SetMoneyRate(player, 2.0f);

        var money = new DroppedMoney(100, DropPosition, player.CurrentMap!);

        Assert.That(await money.TryPickUpByAsync(player).ConfigureAwait(false), Is.True);
        Assert.That(player.Money, Is.EqualTo(200));
    }

    /// <summary>
    /// Tests that the pick up clamp is calculated on the amount *after* the money rate was applied.
    /// Clamping the raw amount would let the player exceed the maximum inventory money.
    /// </summary>
    [Test]
    public async Task SoloPickUpClampsAfterApplyingMoneyRateAsync()
    {
        var player = await PlayerTestHelper.CreatePlayerAsync().ConfigureAwait(false);
        player.GameContext.Configuration.MaximumInventoryMoney = 150;
        player.GameContext.Configuration.ClampMoneyOnPickup = true;
        player.Money = 0;
        SetMoneyRate(player, 2.0f);

        var money = new DroppedMoney(100, DropPosition, player.CurrentMap!);

        Assert.That(await money.TryPickUpByAsync(player).ConfigureAwait(false), Is.True);
        Assert.That(player.Money, Is.EqualTo(150));
    }

    /// <summary>
    /// Tests that every party member receives exactly the share which was reserved for them.
    /// </summary>
    [Test]
    public async Task PartyPickUpPaysReservedSharesAsync()
    {
        var (first, second) = await CreateTwoPlayersAsync().ConfigureAwait(false);
        await CreatePartyAsync(first, second).ConfigureAwait(false);

        var money = new DroppedMoney(1000, DropPosition, first.CurrentMap!, [new MoneyShare(first, 800), new MoneyShare(second, 200)]);

        Assert.That(await money.TryPickUpByAsync(first).ConfigureAwait(false), Is.True);
        Assert.That(first.Money, Is.EqualTo(800));
        Assert.That(second.Money, Is.EqualTo(200));
    }

    /// <summary>
    /// Tests that the money rate of a party member is applied exactly once, on their own share.
    /// It used to be applied twice: once for the killer when the drop was created, and once for
    /// the receiver when it was picked up.
    /// </summary>
    [Test]
    public async Task PartyPickUpAppliesEachMembersOwnRateOnceAsync()
    {
        var (first, second) = await CreateTwoPlayersAsync().ConfigureAwait(false);
        await CreatePartyAsync(first, second).ConfigureAwait(false);
        SetMoneyRate(first, 2.0f);

        var money = new DroppedMoney(200, DropPosition, first.CurrentMap!, [new MoneyShare(first, 100), new MoneyShare(second, 100)]);

        Assert.That(await money.TryPickUpByAsync(first).ConfigureAwait(false), Is.True);
        Assert.That(first.Money, Is.EqualTo(200));
        Assert.That(second.Money, Is.EqualTo(100));
    }

    /// <summary>
    /// Tests that the share of a player who is not eligible anymore - because they left the party,
    /// logged out or went to another map - is given to the remaining members instead of being lost.
    /// </summary>
    [Test]
    public async Task PartyPickUpRedistributesShareOfIneligiblePlayerAsync()
    {
        var (first, second) = await CreateTwoPlayersAsync().ConfigureAwait(false);
        await CreatePartyAsync(first).ConfigureAwait(false);

        // 'second' never joined the party, so it can't receive its share.
        var money = new DroppedMoney(200, DropPosition, first.CurrentMap!, [new MoneyShare(first, 100), new MoneyShare(second, 100)]);

        Assert.That(await money.TryPickUpByAsync(first).ConfigureAwait(false), Is.True);
        Assert.That(first.Money, Is.EqualTo(200));
        Assert.That(second.Money, Is.EqualTo(0));
    }

    /// <summary>
    /// Tests that the pick up clamp is honoured for each party member separately. A member at the
    /// money limit used to silently lose their share, because the clamp was only applied for solo pick ups.
    /// </summary>
    [Test]
    public async Task PartyPickUpHonoursClampPerMemberAsync()
    {
        var (first, second) = await CreateTwoPlayersAsync().ConfigureAwait(false);
        await CreatePartyAsync(first, second).ConfigureAwait(false);
        first.GameContext.Configuration.MaximumInventoryMoney = 150;
        first.GameContext.Configuration.ClampMoneyOnPickup = true;
        first.Money = 100;
        second.Money = 0;

        var money = new DroppedMoney(200, DropPosition, first.CurrentMap!, [new MoneyShare(first, 100), new MoneyShare(second, 100)]);

        Assert.That(await money.TryPickUpByAsync(first).ConfigureAwait(false), Is.True);
        Assert.That(first.Money, Is.EqualTo(150));
        Assert.That(second.Money, Is.EqualTo(100));
    }

    /// <summary>
    /// Tests that money without reserved shares - for example the fixed amount of an item box -
    /// is still split equally between the party members.
    /// </summary>
    [Test]
    public async Task MoneyWithoutSharesIsSplitEquallyAsync()
    {
        var (first, second) = await CreateTwoPlayersAsync().ConfigureAwait(false);
        await CreatePartyAsync(first, second).ConfigureAwait(false);

        var money = new DroppedMoney(200, DropPosition, first.CurrentMap!);

        Assert.That(await money.TryPickUpByAsync(first).ConfigureAwait(false), Is.True);
        Assert.That(first.Money, Is.EqualTo(100));
        Assert.That(second.Money, Is.EqualTo(100));
    }

    /// <summary>
    /// Tests that the drop is not consumed when no party member could take anything, so it stays
    /// available instead of being lost.
    /// </summary>
    [Test]
    public async Task PartyPickUpKeepsMoneyAvailableWhenNobodyCanTakeItAsync()
    {
        var (first, second) = await CreateTwoPlayersAsync().ConfigureAwait(false);
        await CreatePartyAsync(first, second).ConfigureAwait(false);
        first.GameContext.Configuration.MaximumInventoryMoney = 100;
        first.Money = 100;
        second.Money = 100;

        var money = new DroppedMoney(200, DropPosition, first.CurrentMap!, [new MoneyShare(first, 100), new MoneyShare(second, 100)]);

        Assert.That(await money.TryPickUpByAsync(first).ConfigureAwait(false), Is.False);
        Assert.That(first.Money, Is.EqualTo(100));
        Assert.That(second.Money, Is.EqualTo(100));
    }

    private static void SetMoneyRate(Player player, float rate)
    {
        player.Attributes!.AddElement(new SimpleElement(rate, AggregateType.Multiplicate), Stats.MoneyAmountRate);
        Assert.That(
            player.Attributes[Stats.MoneyAmountRate],
            Is.EqualTo(rate).Within(0.001f),
            "test setup: the money rate was not applied as expected");
    }

    private static async ValueTask<Party> CreatePartyAsync(params Player[] members)
    {
        var party = new Party(new PartyManager(5, new NullLogger<Party>()), 5, new NullLogger<Party>());
        foreach (var member in members)
        {
            await party.AddAsync(member).ConfigureAwait(false);
        }

        return party;
    }

    private static async ValueTask<(Player First, Player Second)> CreateTwoPlayersAsync()
    {
        var first = await PlayerTestHelper.CreatePlayerAsync().ConfigureAwait(false);
        first.GameContext.Configuration.MaximumInventoryMoney = int.MaxValue;
        first.Money = 0;

        var second = await PlayerTestHelper.CreatePlayerAsync(first.GameContext).ConfigureAwait(false);
        second.Money = 0;

        Assert.That(first.CurrentMap, Is.Not.Null, "test setup: the players need a map");
        Assert.That(second.CurrentMap, Is.SameAs(first.CurrentMap), "test setup: the players need to be on the same map");
        Assert.That(first.IsAtSafezone(), Is.False, "test setup: the players must not be at a safezone");

        return (first, second);
    }
}
