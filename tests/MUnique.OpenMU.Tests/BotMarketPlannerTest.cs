// <copyright file="BotMarketPlannerTest.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.Tests;

using Microsoft.Extensions.Logging.Abstractions;
using Moq;
using MUnique.OpenMU.DataModel;
using MUnique.OpenMU.DataModel.Configuration;
using MUnique.OpenMU.DataModel.Configuration.Items;
using MUnique.OpenMU.DataModel.Entities;
using MUnique.OpenMU.GameLogic;
using MUnique.OpenMU.GameLogic.Bots;
using MUnique.OpenMU.Pathfinding;
using MUnique.OpenMU.PlugIns;

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
    /// A stall keeps away from the walls, so that no store stands against one.
    /// </summary>
    [Test]
    public void StallKeepsAwayFromTheWalls()
    {
        var terrain = CreateSafezoneTerrain();
        for (var y = 0; y <= byte.MaxValue; y++)
        {
            terrain.WalkMap[Arrival.X + 4, y] = false;
        }

        for (var i = 0; i < 200; i++)
        {
            var stall = BotMarketPlanner.FindStall(terrain, Arrival, [], Random.Shared.Next)!.Value;
            Assert.That(stall.X, Is.LessThanOrEqualTo(Arrival.X + 4 - BotMarketPlanner.StallMargin - 1));
        }
    }

    /// <summary>
    /// On the market, the stalls spread over the whole safezone, not only around the arrival point.
    /// </summary>
    [Test]
    public void MarketStallsSpreadOverTheWholeSafezone()
    {
        var terrain = CreateSafezoneTerrain();

        var stalls = Enumerable.Range(0, 50).Select(_ => BotMarketPlanner.FindStall(terrain, Arrival, [], Random.Shared.Next, null)!.Value).ToList();

        Assert.That(stalls.Any(s => s.EuclideanDistanceTo(Arrival) > BotMarketPlanner.StallSearchRange), Is.True);
    }

    /// <summary>
    /// A safezone behind a wall, which the bot can't walk to from the arrival point, gets no stall.
    /// </summary>
    [Test]
    public void SafezoneBehindAWallGetsNoStall()
    {
        var terrain = CreateSafezoneTerrain();
        const int wallX = 160;
        for (var y = 0; y <= byte.MaxValue; y++)
        {
            terrain.WalkMap[wallX, y] = false;
        }

        for (var i = 0; i < 50; i++)
        {
            var stall = BotMarketPlanner.FindStall(terrain, Arrival, [], Random.Shared.Next, null)!.Value;
            Assert.That(stall.X, Is.LessThan(wallX));
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
    /// About the configured share of the shifts takes place in the cities, the rest on the market.
    /// </summary>
    [Test]
    public void ShareOfTheShiftsIsInTheCities()
    {
        var configuration = new BotMerchantConfiguration();
        var cities = configuration.GetCityMapNumbers();
        const int draws = 10_000;

        var maps = Enumerable.Range(0, draws).Select(_ => BotMarketPlanner.DrawShiftMap(configuration, cities, Random.Shared.Next)).ToList();

        Assert.That(cities, Is.EqualTo(new short[] { 0, 2, 3, 51 }));
        Assert.That(maps.Count(m => m == configuration.MarketMapNumber), Is.InRange(7_500, 8_500));
        Assert.That(maps.Where(m => m != configuration.MarketMapNumber).Distinct(), Is.EquivalentTo(cities));
    }

    /// <summary>
    /// Without a share for the cities, or without usable cities, every shift is on the market.
    /// </summary>
    [Test]
    public void WithoutCitiesEveryShiftIsOnTheMarket()
    {
        var noShare = new BotMerchantConfiguration { CityMerchantsPercent = 0 };
        var noCities = new BotMerchantConfiguration { CityMerchantsPercent = 100, CityMapNumbers = "x, " };

        Assert.That(BotMarketPlanner.DrawShiftMap(noShare, noShare.GetCityMapNumbers(), (_, max) => max - 1), Is.EqualTo(noShare.MarketMapNumber));
        Assert.That(noCities.GetCityMapNumbers(), Is.Empty);
        Assert.That(BotMarketPlanner.DrawShiftMap(noCities, noCities.GetCityMapNumbers(), (min, _) => min), Is.EqualTo(noCities.MarketMapNumber));
    }

    /// <summary>
    /// No two open stores get the same name while there are free names - the default names suffice
    /// for the maximum number of merchants.
    /// </summary>
    [Test]
    public void StoreNamesDontRepeat()
    {
        var configuration = new BotMerchantConfiguration();
        var storeNames = configuration.GetStoreNames();
        var taken = new List<string>();

        for (var i = 0; i < configuration.MaximumMerchants; i++)
        {
            taken.Add(BotMarketPlanner.DrawStoreName(storeNames, taken, "Bot", Random.Shared.Next));
        }

        Assert.That(taken, Is.Unique);
        Assert.That(taken, Has.None.EqualTo("Bot"));
    }

    /// <summary>
    /// When all store names are taken, the store gets the name of the bot.
    /// </summary>
    [Test]
    public void StoreGetsTheBotNameWhenAllNamesAreTaken()
    {
        Assert.That(BotMarketPlanner.DrawStoreName(["Rare finds"], ["rare finds"], "Raven", Random.Shared.Next), Is.EqualTo("Raven"));
    }

    /// <summary>
    /// A bot may leave a party of bots for a market shift, but never a party with a player.
    /// </summary>
    /// <returns>The task.</returns>
    [Test]
    public async ValueTask OnlyAPartyOfBotsIsLeftForTheMarketAsync()
    {
        var bot = await PlayerTestHelper.CreatePlayerAsync().ConfigureAwait(false);
        var otherBot = await PlayerTestHelper.CreatePlayerAsync().ConfigureAwait(false);
        var player = await PlayerTestHelper.CreatePlayerAsync().ConfigureAwait(false);
        SetIsBot(bot, true);
        SetIsBot(otherBot, true);
        SetIsBot(player, false);

        Assert.That(BotMarketPlanner.HasOnlyBots([bot, otherBot]), Is.True);
        Assert.That(BotMarketPlanner.HasOnlyBots([bot, player]), Is.False);
    }

    /// <summary>
    /// A player may join the party of a bot after the bot was picked for a shift: the bot then stays in the
    /// party, while it leaves a party of bots.
    /// </summary>
    /// <returns>The task.</returns>
    [Test]
    public async ValueTask PartyWhichAPlayerJoinedIsNotLeftAsync()
    {
        var gameContext = GameContextTestHelper.CreateGameContext();
        var bot = await PlayerTestHelper.CreateOfflineLevelingPlayerAsync(gameContext).ConfigureAwait(false);
        var otherBot = await PlayerTestHelper.CreatePlayerAsync(gameContext).ConfigureAwait(false);
        var player = await PlayerTestHelper.CreatePlayerAsync(gameContext).ConfigureAwait(false);
        SetIsBot(bot, true);
        SetIsBot(otherBot, true);
        SetIsBot(player, false);
        var party = new Party(new PartyManager(5, new NullLogger<Party>()), 5, new NullLogger<Party>());
        await party.AddAsync(bot).ConfigureAwait(false);
        await party.AddAsync(otherBot).ConfigureAwait(false);
        await party.AddAsync(player).ConfigureAwait(false);

        Assert.That(await BotMerchantPlugIn.LeavePartyAsync(bot).ConfigureAwait(false), Is.False);
        Assert.That(bot.Party, Is.SameAs(party));

        await party.KickMySelfAsync(player).ConfigureAwait(false);
        Assert.That(await BotMerchantPlugIn.LeavePartyAsync(bot).ConfigureAwait(false), Is.True);
        Assert.That(bot.Party, Is.Null);
    }

    /// <summary>
    /// After a server start, the bots go to the market a few at a time.
    /// </summary>
    [Test]
    public void FewBotsStartAShiftAtATime()
    {
        var configuration = new BotMerchantConfiguration();

        Assert.That(configuration.NewMerchantsPerCheck, Is.LessThan(configuration.MaximumMerchants));
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

    /// <summary>
    /// A bot whose store the game doesn't open ends its shift like any other one, so it waits for its
    /// next shift instead of coming back right away to fail again.
    /// </summary>
    /// <returns>The task.</returns>
    [Test]
    public async ValueTask FailedStoreOpeningWaitsForTheNextShiftAsync()
    {
        var gameContext = GameContextTestHelper.CreateGameContext([new PlugInConfiguration { TypeId = typeof(BotMerchantPlugIn).GUID, IsActive = true }]);
        var configuration = new BotMerchantConfiguration();
        gameContext.FeaturePlugIns.GetPlugIn<BotMerchantPlugIn>()!.Configuration = configuration;
        var bot = await PlayerTestHelper.CreateOfflineLevelingPlayerAsync(gameContext).ConfigureAwait(false);
        Assert.That(await bot.ShopStorage!.AddItemAsync(InventoryConstants.FirstStoreItemSlotIndex, CreateItem()).ConfigureAwait(false), Is.True);
        var shift = CreateShift();
        bot.MarketShift = shift;
        var before = DateTime.UtcNow;

        await new BotNavigator(bot).OpenMarketStoreAsync(shift).ConfigureAwait(false);
        await bot.DrainPendingBotActionsAsync().ConfigureAwait(false);

        Assert.That(bot.ShopStorage.StoreOpen, Is.False);
        Assert.That(bot.MarketShift, Is.Null);
        Assert.That(bot.NextMarketShiftUtc, Is.GreaterThanOrEqualTo(before + configuration.TimeBetweenShifts));
    }

    /// <summary>
    /// The income of a sale is removed in the next tick of the bot. A bot which logs out before that
    /// tick removes it on the way out - otherwise the logout would save the income with the bot.
    /// </summary>
    /// <returns>The task.</returns>
    [Test]
    public async ValueTask IncomeIsRemovedWhenTheBotLogsOutRightAfterASaleAsync()
    {
        var gameContext = GameContextTestHelper.CreateGameContext();
        var buyer = await PlayerTestHelper.CreatePlayerAsync(gameContext).ConfigureAwait(false);
        var template = await PlayerTestHelper.CreatePlayerAsync(gameContext).ConfigureAwait(false);
        var bot = new BotPlayer(gameContext) { Account = template.Account };
        await bot.PlayerState.TryAdvanceToAsync(PlayerState.LoginScreen).ConfigureAwait(false);
        await bot.PlayerState.TryAdvanceToAsync(PlayerState.Authenticated).ConfigureAwait(false);
        await bot.PlayerState.TryAdvanceToAsync(PlayerState.CharacterSelection).ConfigureAwait(false);
        await bot.SetSelectedCharacterAsync(template.SelectedCharacter!).ConfigureAwait(false);
        var character = bot.SelectedCharacter!;
        var inventory = bot.Inventory!;
        var shopStorage = bot.ShopStorage!;

        await AddPricedGoodsAsync(bot, 2).ConfigureAwait(false);
        bot.Money = 1000;
        var shift = CreateShift();
        shift.MoneyAtOpen = bot.Money;
        shift.ItemsAtOpen = inventory.Items.ToHashSet();
        shift.IsStoreOpened = true;
        shopStorage.StoreOpen = true;
        bot.MarketShift = shift;

        bot.Money += 810_000;
        var paidJewel = CreateItem();
        Assert.That(await inventory.AddItemAsync(paidJewel).ConfigureAwait(false), Is.True);
        new BotMerchantPlugIn().ItemSold(bot, shopStorage.Items.First(), buyer);
        Assert.That(bot.PendingBotActions, Is.Not.Empty);

        await bot.DisconnectAsync().ConfigureAwait(false);

        Assert.That(character.Inventory!.Money, Is.EqualTo(1000));
        Assert.That(inventory.Items, Does.Not.Contain(paidJewel));
        Assert.That(shopStorage.StoreOpen, Is.False);
        Assert.That(bot.MarketShift, Is.Null);
    }

    /// <summary>
    /// The shifts of a good without a buyer are forgotten when it wasn't counted for a long time - it left
    /// the stores without a sale, e.g. a retired good which went to an NPC -, so the counts don't pile up.
    /// </summary>
    [Test]
    public void OldUnsoldShiftsAreForgotten()
    {
        var plugIn = new BotMerchantPlugIn();
        var configuration = new BotMerchantConfiguration();
        var retired = new MUnique.OpenMU.Persistence.BasicModel.Item { Id = Guid.NewGuid() };
        var inStore = new MUnique.OpenMU.Persistence.BasicModel.Item { Id = Guid.NewGuid() };
        for (var i = 0; i < configuration.UnsoldShiftsUntilNpc; i++)
        {
            plugIn.CountUnsoldShift(retired, Now);
        }

        plugIn.CountUnsoldShift(inStore, Now.AddHours(20));

        plugIn.ForgetOldUnsoldShifts(configuration, Now.AddHours(23));
        Assert.That(plugIn.UnsoldGoodCount, Is.EqualTo(2));
        Assert.That(plugIn.IsRetired(retired, configuration), Is.True);

        plugIn.ForgetOldUnsoldShifts(configuration, Now.AddHours(25));
        Assert.That(plugIn.UnsoldGoodCount, Is.EqualTo(1));
        Assert.That(plugIn.IsRetired(retired, configuration), Is.False);
        Assert.That(plugIn.CountUnsoldShift(inStore, Now.AddHours(25)), Is.EqualTo(2));
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
    /// Gives the player an account which is a bot account or not.
    /// </summary>
    private static void SetIsBot(Player player, bool isBot)
    {
        var account = new Mock<Account>();
        account.SetupAllProperties();
        account.Object.IsBot = isBot;
        player.Account = account.Object;
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
