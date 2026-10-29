// <copyright file="SimpleItemCraftingRateTest.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.Tests;

using Microsoft.Extensions.Logging.Abstractions;
using MUnique.OpenMU.DataModel.Configuration;
using MUnique.OpenMU.DataModel.Configuration.Items;
using MUnique.OpenMU.DataModel.Entities;
using MUnique.OpenMU.GameLogic;
using MUnique.OpenMU.GameLogic.PlayerActions.Items;
using MUnique.OpenMU.Persistence.Initialization.VersionSeasonSix;
using MUnique.OpenMU.Persistence.InMemory;
using MUnique.OpenMU.PlugIns;

/// <summary>
/// Tests the success rate of the <see cref="SimpleItemCraftingHandler"/> when it's derived from the price of the items.
/// </summary>
[TestFixture]
public class SimpleItemCraftingRateTest
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
    /// Tests that items worth more than 255 times the price divisor give the maximum success rate.
    /// The rate is a byte, and the division result used to be cast to it, so such a price wrapped around
    /// to a small rate instead.
    /// </summary>
    [Test]
    public async ValueTask ExpensiveItemsGiveTheMaximumRateAsync()
    {
        var player = await PlayerTestHelper.CreatePlayerAsync(this._gameContext).ConfigureAwait(false);
        var priceCalculator = new ItemPriceCalculator();

        // An item whose price wraps around to less than 100 percent, when it's cast to a byte.
        Item? item = null;
        foreach (var definition in this._gameConfiguration.Items.Where(d => d.Width == 1 && d.Height == 1 && !d.IsAmmunition))
        {
            var candidate = player.PersistenceContext.CreateNew<Item>();
            candidate.Definition = definition;
            candidate.Durability = definition.Durability;
            var price = priceCalculator.CalculateFinalOldBuyingPrice(candidate);
            if (price > byte.MaxValue && price % 256 < 100)
            {
                item = candidate;
                break;
            }
        }

        Assert.That(item, Is.Not.Null, "The test needs an item which is worth more than 255 zen.");
        await player.TemporaryStorage!.AddItemAsync(item!).ConfigureAwait(false);

        var settings = new Persistence.BasicModel.SimpleCraftingSettings { NpcPriceDivisor = 1 };
        var requiredItem = new Persistence.BasicModel.ItemCraftingRequiredItem { MinimumAmount = 1, MaximumAmount = 1, MaximumItemLevel = 15 };
        requiredItem.PossibleItems.Add(item!.Definition!);
        settings.RequiredItems.Add(requiredItem);

        var result = new SimpleItemCraftingHandler(settings).TryGetRequiredItems(player, out _, out var successRate);

        Assert.That(result, Is.Null);
        Assert.That(successRate, Is.EqualTo(100));
    }
}
