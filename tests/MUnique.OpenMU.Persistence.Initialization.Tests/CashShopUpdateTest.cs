// <copyright file="CashShopUpdateTest.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.Persistence.Initialization.Tests;

using Microsoft.Extensions.Logging.Abstractions;
using MUnique.OpenMU.DataModel.Configuration;
using MUnique.OpenMU.Persistence.Initialization.Updates;
using MUnique.OpenMU.Persistence.InMemory;

/// <summary>
/// Tests the initialization of the cash shop configuration for new and existing season 6 databases.
/// </summary>
[TestFixture]
internal class CashShopUpdateTest
{
    /// <summary>
    /// The packages of the items which delivery plugins apply: the Summoner and Rage Fighter
    /// Character Cards, the Magic Backpack and the Vault Expansion Certificate, for WCoin (C) and (P).
    /// </summary>
    private static readonly int[] DeliveryPackages = [126, 127, 258, 259, 260, 261, 263, 264];

    /// <summary>
    /// Tests that a freshly initialized database contains the cash shop configuration
    /// which matches the cash shop script 512.2012.084 of the game client.
    /// </summary>
    [Test]
    public async Task FreshDatabaseContainsCashShopAsync()
    {
        var (context, gameConfiguration) = await CreateSeason6ConfigurationAsync().ConfigureAwait(false);
        using var _ = context;

        var configuration = gameConfiguration.CashShopConfiguration;
        Assert.That(configuration, Is.Not.Null);
        Assert.That(configuration!.ScriptSaleZone, Is.EqualTo(512));
        Assert.That(configuration.ScriptYear, Is.EqualTo(2012));
        Assert.That(configuration.ScriptYearId, Is.EqualTo(84));
        Assert.That(configuration.Packages, Has.Count.EqualTo(205));
        Assert.That(configuration.Packages.Select(p => p.PackageSequence), Is.Unique);
        Assert.That(configuration.Packages, Has.All.Matches<CashShopPackage>(p => p.Products.Select(product => product.PriceSequence).Distinct().Count() == p.Products.Count));
        Assert.That(
            configuration.Packages.Where(p => p.IsForSale).SelectMany(p => p.Products),
            Has.All.Matches<CashShopProduct>(product => product.ItemDefinition is not null && product.Duration == TimeSpan.Zero));
        Assert.That(configuration.Packages.Count(p => p.IsForSale), Is.EqualTo(21));
        Assert.That(configuration.Packages.Where(p => DeliveryPackages.Contains(p.PackageSequence)), Has.All.Matches<CashShopPackage>(p => p.IsForSale));

        var seal = configuration.Packages.Single(p => p.PackageSequence == 122);
        Assert.That(seal.Name, Is.EqualTo("Seal of Wealth"));
        Assert.That(seal.CoinType, Is.EqualTo(CashShopCoinType.WCoinC));
        Assert.That(seal.IsBundle, Is.False);
        Assert.That(seal.Products.Select(p => p.Duration), Is.EquivalentTo(new[] { TimeSpan.FromDays(1), TimeSpan.FromDays(3), TimeSpan.FromDays(7), TimeSpan.FromDays(30) }));
    }

    /// <summary>
    /// Tests that the update adds the cash shop configuration to an existing database
    /// which doesn't have it yet, and that applying it twice doesn't change anything.
    /// </summary>
    [Test]
    public async Task UpdateAddsCashShopToExistingDatabaseAsync()
    {
        var (context, gameConfiguration) = await CreateSeason6ConfigurationAsync().ConfigureAwait(false);
        using var _ = context;
        gameConfiguration.CashShopConfiguration = null;

        var update = new AddCashShopUpdatePlugIn();
        await update.ApplyUpdateAsync(context, gameConfiguration).ConfigureAwait(false);
        var configuration = gameConfiguration.CashShopConfiguration;
        await update.ApplyUpdateAsync(context, gameConfiguration).ConfigureAwait(false);

        Assert.That(configuration, Is.Not.Null);
        Assert.That(gameConfiguration.CashShopConfiguration, Is.SameAs(configuration));
        Assert.That(configuration!.Packages, Has.Count.EqualTo(205));
    }

    /// <summary>
    /// Tests that the update assigns the items of the delivery plugins to the products of a configuration
    /// which was created without these items, and puts their
    /// packages on sale. Applying it twice doesn't add the items twice.
    /// </summary>
    [Test]
    public async Task UpdateAssignsDeliveryItemsToExistingCatalogAsync()
    {
        var (context, gameConfiguration) = await CreateSeason6ConfigurationAsync().ConfigureAwait(false);
        using var _ = context;
        var configuration = gameConfiguration.CashShopConfiguration!;
        var deliveryItems = gameConfiguration.Items.Where(IsDeliveryItem).ToList();
        foreach (var package in configuration.Packages.Where(p => DeliveryPackages.Contains(p.PackageSequence)))
        {
            package.IsForSale = false;
            package.Products.ToList().ForEach(product => product.ItemDefinition = null);
        }

        deliveryItems.ForEach(item => gameConfiguration.Items.Remove(item));

        var update = new AddCashShopUpdatePlugIn();
        await update.ApplyUpdateAsync(context, gameConfiguration).ConfigureAwait(false);
        await update.ApplyUpdateAsync(context, gameConfiguration).ConfigureAwait(false);

        Assert.That(gameConfiguration.Items.Count(IsDeliveryItem), Is.EqualTo(4));
        Assert.That(configuration.Packages.Count(p => p.IsForSale), Is.EqualTo(21));
        Assert.That(
            configuration.Packages.Where(p => DeliveryPackages.Contains(p.PackageSequence)).SelectMany(p => p.Products),
            Has.All.Matches<CashShopProduct>(product => product.ItemDefinition is { } item && IsDeliveryItem(item)));
    }

    private static bool IsDeliveryItem(DataModel.Configuration.Items.ItemDefinition item)
    {
        return item.Group == 14 && item.Number is 91 or 162 or 163 or 169;
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
