// <copyright file="BotMarketPlannerTest.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.Tests;

using Moq;
using MUnique.OpenMU.DataModel;
using MUnique.OpenMU.DataModel.Configuration;
using MUnique.OpenMU.DataModel.Configuration.Items;
using MUnique.OpenMU.DataModel.Entities;
using MUnique.OpenMU.GameLogic;
using MUnique.OpenMU.GameLogic.Bots;
using MUnique.OpenMU.Pathfinding;

/// <summary>
/// Tests the decisions of a market shift of a bot (<see cref="BotMarketPlanner"/>): where it puts its
/// stall, how long it stays and when it leaves.
/// </summary>
[TestFixture]
public class BotMarketPlannerTest
{
    private static readonly Point Arrival = new(128, 145);

    private static readonly DateTime Now = new(2026, 11, 5, 12, 0, 0, DateTimeKind.Utc);

    /// <summary>
    /// A stall keeps its distance from the stalls of the other merchants, so that every store can be clicked.
    /// </summary>
    [Test]
    public void StallKeepsItsDistanceFromTheOtherStalls()
    {
        var terrain = CreateSafezoneTerrain();
        var taken = new List<Point> { Arrival };

        for (var i = 0; i < 50; i++)
        {
            var stall = BotMarketPlanner.FindStall(terrain, Arrival, taken, Random.Shared.Next);
            Assert.That(stall, Is.Not.Null);
            Assert.That(taken.All(t => t.EuclideanDistanceTo(stall!.Value) >= BotMarketPlanner.StallSpacing), Is.True);
            taken.Add(stall!.Value);
        }
    }

    /// <summary>
    /// A stall is always a walkable safezone tile near the arrival point - a store outside the safezone
    /// would be in the middle of the monsters.
    /// </summary>
    [Test]
    public void StallIsInTheSafezoneNearTheArrival()
    {
        var terrain = CreateSafezoneTerrain();
        var outside = new Point(Arrival.X, (byte)(Arrival.Y + 2));
        terrain.SafezoneMap[outside.X, outside.Y] = false;

        for (var i = 0; i < 200; i++)
        {
            var stall = BotMarketPlanner.FindStall(terrain, Arrival, [], Random.Shared.Next)!.Value;
            Assert.That(stall, Is.Not.EqualTo(outside));
            Assert.That(terrain.SafezoneMap[stall.X, stall.Y], Is.True);
            Assert.That(stall.EuclideanDistanceTo(Arrival), Is.LessThanOrEqualTo(BotMarketPlanner.StallSearchRange));
        }
    }

    /// <summary>
    /// A market without a free tile has no stall: the bot doesn't go there at all.
    /// </summary>
    [Test]
    public void FullMarketHasNoStall()
    {
        var terrain = CreateSafezoneTerrain();
        Array.Clear(terrain.WalkMap);
        terrain.WalkMap[Arrival.X, Arrival.Y] = true;

        Assert.That(BotMarketPlanner.FindStall(terrain, Arrival, [Arrival], Random.Shared.Next), Is.Null);
    }

    /// <summary>
    /// The length of a shift is random between the minimum and the maximum, both included.
    /// </summary>
    [Test]
    public void ShiftDurationIsBetweenMinimumAndMaximum()
    {
        var configuration = new BotMerchantConfiguration();

        Assert.That(BotMarketPlanner.DrawShiftDuration(configuration, (min, _) => min), Is.EqualTo(TimeSpan.FromMinutes(30)));
        Assert.That(BotMarketPlanner.DrawShiftDuration(configuration, (_, max) => max - 1), Is.EqualTo(TimeSpan.FromMinutes(90)));
    }

    /// <summary>
    /// A maximum below the minimum doesn't break the draw: the shift then lasts the minimum.
    /// </summary>
    [Test]
    public void ShiftDurationWithMaximumBelowMinimumIsTheMinimum()
    {
        var configuration = new BotMerchantConfiguration { MaximumShiftDuration = TimeSpan.FromMinutes(10) };

        Assert.That(BotMarketPlanner.DrawShiftDuration(configuration, (_, max) => max - 1), Is.EqualTo(TimeSpan.FromMinutes(30)));
    }

    /// <summary>
    /// A shift ends when its time is up, even before the store was opened.
    /// </summary>
    /// <returns>The task.</returns>
    [Test]
    public async ValueTask ShiftEndsWhenTheTimeIsUpAsync()
    {
        var bot = await PlayerTestHelper.CreatePlayerAsync().ConfigureAwait(false);
        var shift = CreateShift();

        var reason = BotMarketPlanner.GetEndReason(bot, shift, new BotMerchantConfiguration(), shift.UntilUtc);

        Assert.That(reason, Is.EqualTo(BotMarketPlanner.EndReason.TimeIsUp));
    }

    /// <summary>
    /// On the way to the stall, the store is still closed and empty shelves don't count yet.
    /// </summary>
    /// <returns>The task.</returns>
    [Test]
    public async ValueTask ShiftGoesOnWhileOnTheWayAsync()
    {
        var bot = await PlayerTestHelper.CreatePlayerAsync().ConfigureAwait(false);
        var shift = CreateShift();

        var reason = BotMarketPlanner.GetEndReason(bot, shift, new BotMerchantConfiguration(), Now.AddMinutes(1));

        Assert.That(reason, Is.EqualTo(BotMarketPlanner.EndReason.None));
    }

    /// <summary>
    /// The game closes a store after its last item was sold - the shift ends with it.
    /// </summary>
    /// <returns>The task.</returns>
    [Test]
    public async ValueTask ShiftEndsWhenTheGameClosedTheStoreAsync()
    {
        var bot = await PlayerTestHelper.CreatePlayerAsync().ConfigureAwait(false);
        var shift = CreateShift();
        shift.IsStoreOpened = true;

        var reason = BotMarketPlanner.GetEndReason(bot, shift, new BotMerchantConfiguration(), Now.AddMinutes(1));

        Assert.That(reason, Is.EqualTo(BotMarketPlanner.EndReason.StoreClosed));
    }

    /// <summary>
    /// By default, a bot stays as long as it has anything to sell - a store of two items mustn't close
    /// after the first sale.
    /// </summary>
    /// <returns>The task.</returns>
    [Test]
    public async ValueTask ShiftGoesOnWithTheLastGoodAsync()
    {
        var bot = await PlayerTestHelper.CreatePlayerAsync().ConfigureAwait(false);
        await AddPricedGoodsAsync(bot, 1).ConfigureAwait(false);
        bot.ShopStorage!.StoreOpen = true;
        var shift = CreateShift();
        shift.IsStoreOpened = true;

        var reason = BotMarketPlanner.GetEndReason(bot, shift, new BotMerchantConfiguration(), Now.AddMinutes(1));

        Assert.That(reason, Is.EqualTo(BotMarketPlanner.EndReason.None));
    }

    /// <summary>
    /// With a higher minimum, a bot which sold nearly everything leaves early.
    /// </summary>
    /// <returns>The task.</returns>
    [Test]
    public async ValueTask ShiftEndsBelowTheMinimumOfGoodsAsync()
    {
        var bot = await PlayerTestHelper.CreatePlayerAsync().ConfigureAwait(false);
        await AddPricedGoodsAsync(bot, 1).ConfigureAwait(false);
        bot.ShopStorage!.StoreOpen = true;
        var shift = CreateShift();
        shift.IsStoreOpened = true;

        var reason = BotMarketPlanner.GetEndReason(bot, shift, new BotMerchantConfiguration { MinimumGoodsToStay = 2 }, Now.AddMinutes(1));

        Assert.That(reason, Is.EqualTo(BotMarketPlanner.EndReason.SoldOut));
    }

    /// <summary>
    /// A bot which doesn't sell anything for the configured time leaves and makes room for another one.
    /// </summary>
    /// <returns>The task.</returns>
    [Test]
    public async ValueTask ShiftEndsAfterALongTimeWithoutSalesAsync()
    {
        var bot = await PlayerTestHelper.CreatePlayerAsync().ConfigureAwait(false);
        await AddPricedGoodsAsync(bot, 3).ConfigureAwait(false);
        bot.ShopStorage!.StoreOpen = true;
        var shift = CreateShift();
        shift.IsStoreOpened = true;
        var configuration = new BotMerchantConfiguration();

        Assert.That(BotMarketPlanner.GetEndReason(bot, shift, configuration, Now.AddMinutes(29)), Is.EqualTo(BotMarketPlanner.EndReason.None));
        Assert.That(BotMarketPlanner.GetEndReason(bot, shift, configuration, Now.AddMinutes(30)), Is.EqualTo(BotMarketPlanner.EndReason.NoSales));

        shift.LastSaleUtc = Now.AddMinutes(20);
        Assert.That(BotMarketPlanner.GetEndReason(bot, shift, configuration, Now.AddMinutes(30)), Is.EqualTo(BotMarketPlanner.EndReason.None));

        configuration.LeaveWithoutSalesAfter = TimeSpan.Zero;
        Assert.That(BotMarketPlanner.GetEndReason(bot, shift, configuration, Now.AddMinutes(55)), Is.EqualTo(BotMarketPlanner.EndReason.None));
    }

    /// <summary>
    /// Only goods with a price count: the game doesn't open a store with an item without one.
    /// </summary>
    /// <returns>The task.</returns>
    [Test]
    public async ValueTask OnlyPricedGoodsCountAsync()
    {
        var bot = await PlayerTestHelper.CreatePlayerAsync().ConfigureAwait(false);
        await AddPricedGoodsAsync(bot, 2).ConfigureAwait(false);
        var unpriced = CreateItem();
        await bot.ShopStorage!.AddItemAsync((byte)(InventoryConstants.FirstStoreItemSlotIndex + 10), unpriced).ConfigureAwait(false);

        Assert.That(BotMarketPlanner.CountPricedGoods(bot), Is.EqualTo(2));
    }

    /// <summary>
    /// The bot buys nothing, so its income is removed: the zen above what it had when it opened the store,
    /// and the items paid for its goods. What it had before stays.
    /// </summary>
    /// <returns>The task.</returns>
    [Test]
    public async ValueTask IncomeIsRemovedAsync()
    {
        var bot = await PlayerTestHelper.CreatePlayerAsync().ConfigureAwait(false);
        bot.Money = 1000;
        var ownJewel = CreateItem();
        Assert.That(await bot.Inventory!.AddItemAsync(ownJewel).ConfigureAwait(false), Is.True);
        var shift = CreateShift();
        shift.MoneyAtOpen = bot.Money;
        shift.ItemsAtOpen = bot.Inventory.Items.ToHashSet();
        shift.IsStoreOpened = true;

        bot.Money += 500_000;
        var paidJewel = CreateItem();
        Assert.That(await bot.Inventory.AddItemAsync(paidJewel).ConfigureAwait(false), Is.True);
        await BotMerchandise.RemoveIncomeAsync(bot, shift).ConfigureAwait(false);

        Assert.That(bot.Money, Is.EqualTo(1000));
        Assert.That(bot.Inventory.Items, Does.Contain(ownJewel));
        Assert.That(bot.Inventory.Items, Does.Not.Contain(paidJewel));
    }

    private static BotMarketShift CreateShift()
        => new(new GameMapDefinition { Number = BotMerchantConfiguration.DefaultMarketMapNumber }, Arrival, "Good stuff", Now, Now.AddMinutes(60));

    private static async ValueTask AddPricedGoodsAsync(Player bot, int count)
    {
        for (var i = 0; i < count; i++)
        {
            var item = CreateItem();
            item.StorePrice = 1000;
            Assert.That(await bot.ShopStorage!.AddItemAsync((byte)(InventoryConstants.FirstStoreItemSlotIndex + (i * 2)), item).ConfigureAwait(false), Is.True);
        }
    }

    private static Item CreateItem()
    {
        var definitionMock = new Mock<ItemDefinition>();
        definitionMock.SetupAllProperties();
        definitionMock.Setup(d => d.PossibleItemOptions).Returns(new List<ItemOptionDefinition>());
        definitionMock.Setup(d => d.PossibleItemSetGroups).Returns(new List<ItemSetGroup>());
        var definition = definitionMock.Object;
        definition.Width = 1;
        definition.Height = 1;

        var itemMock = new Mock<Item>();
        itemMock.SetupAllProperties();
        itemMock.Setup(i => i.ItemOptions).Returns(new List<ItemOptionLink>());
        itemMock.Setup(i => i.ItemSetGroups).Returns(new List<ItemOfItemSet>());
        var item = itemMock.Object;
        item.Definition = definition;
        return item;
    }

    /// <summary>
    /// Creates a terrain on which every tile is walkable and in the safezone, like the middle of a town.
    /// </summary>
    private static GameMapTerrain CreateSafezoneTerrain()
    {
        var terrain = new GameMapTerrain((byte[]?)null);
        for (var x = 0; x <= byte.MaxValue; x++)
        {
            for (var y = 0; y <= byte.MaxValue; y++)
            {
                terrain.WalkMap[x, y] = true;
                terrain.SafezoneMap[x, y] = true;
            }
        }

        return terrain;
    }
}
