// <copyright file="RentalItemRulesDataTest.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.Persistence.Initialization.Tests;

using Microsoft.Extensions.Logging.Abstractions;
using MUnique.OpenMU.DataModel.Configuration;
using MUnique.OpenMU.DataModel.Configuration.Items;
using MUnique.OpenMU.DataModel.Entities;
using MUnique.OpenMU.GameLogic;
using MUnique.OpenMU.Persistence.Initialization.Updates;
using MUnique.OpenMU.Persistence.Initialization.VersionSeasonSix.Items;
using MUnique.OpenMU.Persistence.InMemory;

/// <summary>
/// Tests the data which the items rented in the cash shop need for their expiration:
/// <see cref="ItemDefinition.IsSellableToNpcWhenExpired"/> and the small fixed selling prices which the client shows.
/// </summary>
[TestFixture]
internal class RentalItemRulesDataTest
{
    /// <summary>
    /// The selling prices which the client shows (<c>ItemValue</c> of the client), by item.
    /// </summary>
    private static readonly Dictionary<(byte Group, short Number), long> ClientSellingPrices = new()
    {
        { (12, 130), 80 }, { (12, 131), 80 }, { (12, 132), 80 }, { (12, 133), 80 }, { (12, 134), 80 }, { (12, 135), 80 },
        { (13, 64), 1000 }, { (13, 65), 1000 }, { (13, 76), 1000 }, { (13, 80), 1000 },
        { (13, 109), 1000 }, { (13, 110), 1000 }, { (13, 111), 1000 }, { (13, 112), 1000 },
        { (13, 113), 1000 }, { (13, 114), 1000 }, { (13, 115), 1000 },
        { (13, 122), 2000 }, { (13, 123), 2000 },
    };

    /// <summary>
    /// Tests that a new season 6 database has the flag and the prices.
    /// </summary>
    [Test]
    public async Task NewDatabaseHasRulesAndPricesAsync()
    {
        var (context, gameConfiguration) = await CreateSeason6ConfigurationAsync().ConfigureAwait(false);
        using var _ = context;

        AssertRulesAndPrices(context, gameConfiguration);
    }

    /// <summary>
    /// Tests that the update sets the flag and the prices on a database which was created before they existed,
    /// where the items had the price of the accessories. Applying it twice gives the same result.
    /// </summary>
    [Test]
    public async Task UpdateSetsRulesAndPricesOnExistingDatabaseAsync()
    {
        var (context, gameConfiguration) = await CreateSeason6ConfigurationAsync().ConfigureAwait(false);
        using var _ = context;
        var accessories = gameConfiguration.ItemPriceDefinitions.Single(definition => definition.Name.ValueInNeutralLanguage == "Accessories");
        foreach (var item in gameConfiguration.Items.Where(item => ClientSellingPrices.ContainsKey((item.Group, item.Number)) || item.IsSellableToNpcWhenExpired))
        {
            item.IsSellableToNpcWhenExpired = false;
            if (ClientSellingPrices.ContainsKey((item.Group, item.Number)))
            {
                item.PriceDefinition = accessories;
            }
        }

        gameConfiguration.ItemPriceDefinitions.Where(definition => definition.Name.ValueInNeutralLanguage.StartsWith("Rental items") || definition.Name.ValueInNeutralLanguage == "Small wings and capes")
            .ToList().ForEach(definition => gameConfiguration.ItemPriceDefinitions.Remove(definition));

        var update = new AddRentalItemRulesUpdatePlugIn();
        await update.ApplyUpdateAsync(context, gameConfiguration).ConfigureAwait(false);
        await update.ApplyUpdateAsync(context, gameConfiguration).ConfigureAwait(false);

        AssertRulesAndPrices(context, gameConfiguration);
        Assert.That(gameConfiguration.ItemPriceDefinitions.Count(definition => definition.Name.ValueInNeutralLanguage == "Rental items"), Is.EqualTo(1));
    }

    /// <summary>
    /// Tests that the update keeps a price definition which a server owner assigned to a rented item.
    /// </summary>
    [Test]
    public async Task UpdateKeepsCustomPriceAsync()
    {
        var (context, gameConfiguration) = await CreateSeason6ConfigurationAsync().ConfigureAwait(false);
        using var _ = context;
        var custom = context.CreateNew<ItemPriceDefinition>();
        custom.Name = "Custom";
        custom.BasePriceFormula = "123456";
        gameConfiguration.ItemPriceDefinitions.Add(custom);
        var panda = gameConfiguration.Items.Single(item => item is { Group: 13, Number: 80 });
        panda.PriceDefinition = custom;

        await new AddRentalItemRulesUpdatePlugIn().ApplyUpdateAsync(context, gameConfiguration).ConfigureAwait(false);

        Assert.That(panda.PriceDefinition, Is.SameAs(custom));
    }

    private static void AssertRulesAndPrices(IContext context, GameConfiguration gameConfiguration)
    {
        Assert.That(
            gameConfiguration.Items.Where(item => item.IsSellableToNpcWhenExpired).Select(item => (item.Group, item.Number)),
            Is.EquivalentTo(ItemRules.SellableWhenExpired));

        var calculator = new ItemPriceCalculator();
        foreach (var ((group, number), expectedPrice) in ClientSellingPrices)
        {
            var definition = gameConfiguration.Items.Single(item => item.Group == group && item.Number == number);
            var item = context.CreateNew<Item>();
            item.Definition = definition;

            item.Durability = definition.Durability;
            Assert.That(calculator.CalculateSellingPrice(item), Is.EqualTo(expectedPrice), $"{definition.Name} at full durability");

            // The price doesn't depend on the durability, like the client shows it.
            item.Durability = 1;
            Assert.That(calculator.CalculateSellingPrice(item, 1), Is.EqualTo(expectedPrice), $"{definition.Name} at durability 1");
        }
    }

    private static async Task<(IContext Context, GameConfiguration GameConfiguration)> CreateSeason6ConfigurationAsync()
    {
        var contextProvider = new InMemoryPersistenceContextProvider();
        var dataInitialization = new VersionSeasonSix.DataInitialization(contextProvider, new NullLoggerFactory());
        await dataInitialization.CreateInitialDataAsync(1, true).ConfigureAwait(false);

        var context = contextProvider.CreateNewContext();
        var gameConfiguration = (await context.GetAsync<GameConfiguration>().ConfigureAwait(false)).First();
        return (context, gameConfiguration);
    }
}
