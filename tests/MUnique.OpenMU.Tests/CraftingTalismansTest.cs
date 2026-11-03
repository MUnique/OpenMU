// <copyright file="CraftingTalismansTest.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.Tests;

using Microsoft.Extensions.Logging.Abstractions;
using MUnique.OpenMU.DataModel.Configuration;
using MUnique.OpenMU.DataModel.Configuration.ItemCrafting;
using MUnique.OpenMU.DataModel.Configuration.Items;
using MUnique.OpenMU.DataModel.Entities;
using MUnique.OpenMU.GameLogic;
using MUnique.OpenMU.GameLogic.PlayerActions.Items;
using MUnique.OpenMU.GameLogic.Views.NPC;
using MUnique.OpenMU.Persistence.Initialization.VersionSeasonSix;
using MUnique.OpenMU.Persistence.InMemory;
using MUnique.OpenMU.PlugIns;

/// <summary>
/// Tests the Talisman of Luck and the Talisman of Chaos Assembly in the craftings.
/// </summary>
[TestFixture]
public class CraftingTalismansTest
{
    private GameConfiguration _gameConfiguration = null!;

    private IGameContext _gameContext = null!;

    /// <summary>
    /// Sets up the test environment by initializing the season 6 configuration data.
    /// </summary>
    [OneTimeSetUp]
    public async ValueTask SetupAsync()
    {
        var contextProvider = new InMemoryPersistenceContextProvider();
        await new DataInitialization(contextProvider, new NullLoggerFactory()).CreateInitialDataAsync(0, false).ConfigureAwait(false);
        this._gameConfiguration = (await contextProvider.CreateNewConfigurationContext().GetAsync<GameConfiguration>().ConfigureAwait(false)).First();

        var mapInitializer = new MapInitializer(this._gameConfiguration, new NullLogger<MapInitializer>(), NullDropGenerator.Instance, null);
        var gameContext = new GameContext(this._gameConfiguration, contextProvider, mapInitializer, new NullLoggerFactory(), new PlugInManager(null, new NullLoggerFactory(), null, null), NullDropGenerator.Instance, new ConfigurationChangeMediator());
        mapInitializer.PlugInManager = gameContext.PlugInManager;
        mapInitializer.PathFinderPool = gameContext.PathFinderPool;
        this._gameContext = gameContext;
    }

    /// <summary>
    /// Tests that the Talisman of Luck increases the success rate of the +10 item upgrade by its durability.
    /// </summary>
    /// <param name="talismanDurabilities">The durabilities of the talismans which are put in.</param>
    /// <returns>The success rate.</returns>
    [TestCase(new byte[0], ExpectedResult = 60)]
    [TestCase(new byte[] { 1 }, ExpectedResult = 61)]
    [TestCase(new byte[] { 5, 2 }, ExpectedResult = 67)]
    [TestCase(new byte[] { 10 }, ExpectedResult = 70)]
    public async ValueTask<int> TalismanOfLuckIncreasesRateAsync(byte[] talismanDurabilities)
    {
        var player = await this.CreatePlayerWithUpgradeItemsAsync(talismanDurabilities).ConfigureAwait(false);

        var result = this.GetUpgradeHandler().TryGetRequiredItems(player, out _, out var successRate);

        Assert.That(result, Is.Null);
        return successRate;
    }

    /// <summary>
    /// Tests that talismans of luck with a total durability above 10 are refused.
    /// </summary>
    [Test]
    public async ValueTask TooManyTalismansOfLuckAreRefusedAsync()
    {
        var player = await this.CreatePlayerWithUpgradeItemsAsync([10, 1]).ConfigureAwait(false);

        var result = this.GetUpgradeHandler().TryGetRequiredItems(player, out _, out _);

        Assert.That(result, Is.EqualTo(CraftingResult.TooManyItems));
    }

    /// <summary>
    /// Tests that the Talisman of Luck respects the maximum success rate of a crafting.
    /// </summary>
    [Test]
    public async ValueTask TalismanOfLuckRespectsMaximumRateAsync()
    {
        var player = await PlayerTestHelper.CreatePlayerAsync(this._gameContext).ConfigureAwait(false);
        await AddItemAsync(player, this.TalismanOfLuck, 0, 10).ConfigureAwait(false);
        var settings = new Persistence.BasicModel.SimpleCraftingSettings { SuccessPercent = 85, MaximumSuccessPercent = 90 };
        settings.RequiredItems.Add(CreateTalismanRequirement(this.TalismanOfLuck, 10, 1, MixResult.Disappear));

        var result = new SimpleItemCraftingHandler(settings).TryGetRequiredItems(player, out _, out var successRate);

        Assert.That(result, Is.Null);
        Assert.That(successRate, Is.EqualTo(90));
    }

    /// <summary>
    /// Tests that the Talisman of Luck is added on top of a rate which is based on the price of the items,
    /// and that its own price doesn't count to that rate.
    /// </summary>
    [Test]
    public async ValueTask TalismanOfLuckAddsToPriceBasedRateAsync()
    {
        var player = await PlayerTestHelper.CreatePlayerAsync(this._gameContext).ConfigureAwait(false);
        var item = await AddItemAsync(player, this._gameConfiguration.Items.First(i => i.Group == 0), 9, 20).ConfigureAwait(false);
        await AddItemAsync(player, this.TalismanOfLuck, 0, 5).ConfigureAwait(false);
        var price = new ItemPriceCalculator().CalculateFinalOldBuyingPrice(item);
        var settings = new Persistence.BasicModel.SimpleCraftingSettings { NpcPriceDivisor = (int)Math.Max(1, price / 50) };
        var requiredItem = new Persistence.BasicModel.ItemCraftingRequiredItem { MinimumAmount = 1, MaximumAmount = 1, MaximumItemLevel = 15 };
        requiredItem.PossibleItems.Add(item.Definition!);
        settings.RequiredItems.Add(requiredItem);
        settings.RequiredItems.Add(CreateTalismanRequirement(this.TalismanOfLuck, 10, 1, MixResult.Disappear));
        var expectedRate = Math.Min(100, (price / settings.NpcPriceDivisor) + 5);

        var result = new SimpleItemCraftingHandler(settings).TryGetRequiredItems(player, out _, out var successRate);

        Assert.That(result, Is.Null);
        Assert.That(successRate, Is.EqualTo(expectedRate));
    }

    /// <summary>
    /// Tests that the Talisman of Chaos Assembly keeps the item of a failed item upgrade, and is used up itself.
    /// </summary>
    [Test]
    public async ValueTask TalismanOfChaosAssemblyProtectsItemAsync()
    {
        var player = await PlayerTestHelper.CreatePlayerAsync(this._gameContext).ConfigureAwait(false);
        var item = await AddItemAsync(player, this._gameConfiguration.Items.First(i => i.Group == 0), 9, 20).ConfigureAwait(false);
        var talisman = await AddItemAsync(player, this.TalismanOfChaosAssembly, 0, 1).ConfigureAwait(false);
        var settings = CreateFailingUpgradeSettings(item.Definition!);
        settings.RequiredItems.Add(CreateTalismanRequirement(this.TalismanOfChaosAssembly, 1, 0, MixResult.DisappearAndProtectReferencedItems));

        var (result, _) = await new SimpleItemCraftingHandler(settings).DoMixAsync(player, 0).ConfigureAwait(false);

        Assert.That(result, Is.EqualTo(CraftingResult.Failed));
        Assert.That(player.TemporaryStorage!.Items, Does.Contain(item));
        Assert.That(player.TemporaryStorage.Items, Does.Not.Contain(talisman));
        Assert.That(item.Level, Is.EqualTo(9));
    }

    /// <summary>
    /// Tests that the item of a failed item upgrade disappears without the Talisman of Chaos Assembly.
    /// </summary>
    [Test]
    public async ValueTask ItemDisappearsWithoutTalismanOfChaosAssemblyAsync()
    {
        var player = await PlayerTestHelper.CreatePlayerAsync(this._gameContext).ConfigureAwait(false);
        var item = await AddItemAsync(player, this._gameConfiguration.Items.First(i => i.Group == 0), 9, 20).ConfigureAwait(false);
        var settings = CreateFailingUpgradeSettings(item.Definition!);
        settings.RequiredItems.Add(CreateTalismanRequirement(this.TalismanOfChaosAssembly, 1, 0, MixResult.DisappearAndProtectReferencedItems));

        var (result, _) = await new SimpleItemCraftingHandler(settings).DoMixAsync(player, 0).ConfigureAwait(false);

        Assert.That(result, Is.EqualTo(CraftingResult.Failed));
        Assert.That(player.TemporaryStorage!.Items, Does.Not.Contain(item));
    }

    private ItemDefinition TalismanOfLuck => this._gameConfiguration.Items.Single(i => i.Group == 14 && i.Number == 53);

    private ItemDefinition TalismanOfChaosAssembly => this._gameConfiguration.Items.Single(i => i.Group == 14 && i.Number == 96);

    private static SimpleCraftingSettings CreateFailingUpgradeSettings(ItemDefinition definition)
    {
        var settings = new Persistence.BasicModel.SimpleCraftingSettings { SuccessPercent = 0 };
        var requiredItem = new Persistence.BasicModel.ItemCraftingRequiredItem
        {
            MinimumAmount = 1,
            MaximumAmount = 1,
            MinimumItemLevel = 9,
            MaximumItemLevel = 9,
            Reference = 1,
            SuccessResult = MixResult.StaysAsIs,
            FailResult = MixResult.Disappear,
        };
        requiredItem.PossibleItems.Add(definition);
        settings.RequiredItems.Add(requiredItem);
        return settings;
    }

    private static ItemCraftingRequiredItem CreateTalismanRequirement(ItemDefinition talisman, byte maximumAmount, byte addPercentage, MixResult failResult)
    {
        var requirement = new Persistence.BasicModel.ItemCraftingRequiredItem
        {
            MinimumAmount = 0,
            MaximumAmount = maximumAmount,
            AddPercentage = addPercentage,
            SuccessResult = MixResult.Disappear,
            FailResult = failResult,
        };
        requirement.PossibleItems.Add(talisman);
        return requirement;
    }

    private static async ValueTask<Item> AddItemAsync(Player player, ItemDefinition definition, byte level, byte durability)
    {
        var item = player.PersistenceContext.CreateNew<Item>();
        item.Definition = definition;
        item.Level = level;
        item.Durability = durability;
        Assert.That(await player.TemporaryStorage!.AddItemAsync(item).ConfigureAwait(false), Is.True);
        return item;
    }

    private SimpleItemCraftingHandler GetUpgradeHandler()
    {
        var crafting = this._gameConfiguration.Monsters.First(m => m.NpcWindow == NpcWindow.ChaosMachine).ItemCraftings.Single(c => c.Number == 3);
        return new SimpleItemCraftingHandler(crafting.SimpleCraftingSettings!);
    }

    private async ValueTask<Player> CreatePlayerWithUpgradeItemsAsync(byte[] talismanDurabilities)
    {
        var player = await PlayerTestHelper.CreatePlayerAsync(this._gameContext).ConfigureAwait(false);
        await AddItemAsync(player, this._gameConfiguration.Items.First(i => i.Group == 0), 9, 20).ConfigureAwait(false);
        await AddItemAsync(player, this._gameConfiguration.Items.Single(i => i.Group == 12 && i.Number == 15), 0, 1).ConfigureAwait(false);
        await AddItemAsync(player, this._gameConfiguration.Items.Single(i => i.Group == 14 && i.Number == 13), 0, 1).ConfigureAwait(false);
        await AddItemAsync(player, this._gameConfiguration.Items.Single(i => i.Group == 14 && i.Number == 14), 0, 1).ConfigureAwait(false);
        foreach (var durability in talismanDurabilities)
        {
            await AddItemAsync(player, this.TalismanOfLuck, 0, durability).ConfigureAwait(false);
        }

        return player;
    }
}
