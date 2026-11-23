// <copyright file="WornHorseshoeDataTest.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.Persistence.Initialization.Tests;

using Microsoft.Extensions.Logging.Abstractions;
using MUnique.OpenMU.DataModel.Configuration;
using MUnique.OpenMU.DataModel.Configuration.Items;
using MUnique.OpenMU.GameLogic.Attributes;
using MUnique.OpenMU.Persistence.Initialization.Updates;
using MUnique.OpenMU.Persistence.InMemory;

/// <summary>
/// Tests the data of the worn horseshoe (13/134), which increases the item drop rate while it's used inside the inventory.
/// </summary>
[TestFixture]
internal class WornHorseshoeDataTest
{
    /// <summary>
    /// Tests that a new season 6 database contains the item.
    /// </summary>
    [Test]
    public async Task NewDatabaseContainsItemAsync()
    {
        var (context, gameConfiguration) = await CreateSeason6ConfigurationAsync().ConfigureAwait(false);
        using var _ = context;

        AssertItemData(gameConfiguration);
    }

    /// <summary>
    /// Tests that the update adds the item to a database which was created before it existed,
    /// and that applying it twice doesn't add the item twice.
    /// </summary>
    [Test]
    public async Task UpdateAddsItemToExistingDatabaseAsync()
    {
        var (context, gameConfiguration) = await CreateSeason6ConfigurationAsync().ConfigureAwait(false);
        using var _ = context;
        gameConfiguration.Items.Remove(gameConfiguration.Items.Single(IsWornHorseshoe));

        var update = new AddWornHorseshoeUpdatePlugIn();
        await update.ApplyUpdateAsync(context, gameConfiguration).ConfigureAwait(false);
        await update.ApplyUpdateAsync(context, gameConfiguration).ConfigureAwait(false);

        AssertItemData(gameConfiguration);
    }

    /// <summary>
    /// Tests that the update links the products of the cash shop to the item, which have no item definition
    /// in a database which was created before the item existed. Their packages stay not for sale.
    /// </summary>
    [Test]
    public async Task UpdateLinksCashShopProductsAsync()
    {
        var (context, gameConfiguration) = await CreateSeason6ConfigurationAsync().ConfigureAwait(false);
        using var _ = context;
        var packages = gameConfiguration.CashShopConfiguration!.Packages
            .Where(package => package.Products.Any(product => product.ProductSequence == AddWornHorseshoeUpdatePlugIn.ProductSequence))
            .ToList();
        var products = packages.SelectMany(package => package.Products).ToList();
        Assert.That(products, Has.Count.EqualTo(6));
        Assert.That(products, Has.All.Matches<CashShopProduct>(product => product.ItemDefinition is { } definition && IsWornHorseshoe(definition)));
        Assert.That(packages, Has.None.Matches<CashShopPackage>(package => package.IsForSale));
        products.ForEach(product => product.ItemDefinition = null);
        gameConfiguration.Items.Remove(gameConfiguration.Items.Single(IsWornHorseshoe));

        await new AddWornHorseshoeUpdatePlugIn().ApplyUpdateAsync(context, gameConfiguration).ConfigureAwait(false);

        var horseshoe = gameConfiguration.Items.Single(IsWornHorseshoe);
        Assert.That(products, Has.All.Matches<CashShopProduct>(product => product.ItemDefinition == horseshoe));
        Assert.That(packages, Has.None.Matches<CashShopPackage>(package => package.IsForSale));
    }

    /// <summary>
    /// Tests that the older cash shop update, which creates the item as well, works on a database
    /// without the item drop rate attribute, which the update of the seals adds otherwise.
    /// </summary>
    [Test]
    public async Task CashShopUpdateAddsMissingDropRateAttributeAsync()
    {
        var (context, gameConfiguration) = await CreateSeason6ConfigurationAsync().ConfigureAwait(false);
        using var _ = context;
        gameConfiguration.Items.Remove(gameConfiguration.Items.Single(IsWornHorseshoe));
        gameConfiguration.Attributes.Remove(gameConfiguration.Attributes.Single(a => a.Id == Stats.BonusItemDropRate.Id));

        await new AddCashShopUpdatePlugIn().ApplyUpdateAsync(context, gameConfiguration).ConfigureAwait(false);

        var dropRate = gameConfiguration.Attributes.Single(a => a.Id == Stats.BonusItemDropRate.Id);
        var horseshoe = gameConfiguration.Items.Single(IsWornHorseshoe);
        Assert.That(horseshoe.BasePowerUpAttributes.Select(p => p.TargetAttribute), Has.Member(dropRate));
    }

    private static bool IsWornHorseshoe(ItemDefinition item) => item is { Group: 13, Number: 134 };

    private static void AssertItemData(GameConfiguration gameConfiguration)
    {
        var horseshoe = gameConfiguration.Items.Single(IsWornHorseshoe);
        Assert.That(horseshoe.Name.ValueInNeutralLanguage, Is.EqualTo("Worn Horseshoe"));
        Assert.That(horseshoe.Durability, Is.EqualTo(1));
        Assert.That(horseshoe.ItemSlot, Is.Null);
        Assert.That(horseshoe.Width, Is.EqualTo(1));
        Assert.That(horseshoe.Height, Is.EqualTo(1));
        var powerUp = horseshoe.BasePowerUpAttributes.Single();
        Assert.That(powerUp.TargetAttribute, Is.EqualTo(Stats.BonusItemDropRate));
        Assert.That(powerUp.BaseValue, Is.EqualTo(0.2f));
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
