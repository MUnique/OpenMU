// <copyright file="TalismanOfResurrectionDataTest.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.Persistence.Initialization.Tests;

using Microsoft.Extensions.Logging.Abstractions;
using MUnique.OpenMU.DataModel.Configuration;
using MUnique.OpenMU.DataModel.Configuration.Items;
using MUnique.OpenMU.Persistence.Initialization.Updates;
using MUnique.OpenMU.Persistence.InMemory;

/// <summary>
/// Tests the data of the Talisman of Resurrection.
/// </summary>
[TestFixture]
internal class TalismanOfResurrectionDataTest
{
    private static readonly short[] TalismanNumbers = [69];

    /// <summary>
    /// The packages of the in-game shop which contain the talisman, for WCoin (C) and (P).
    /// </summary>
    private static readonly int[] TalismanPackages = [70, 71];

    /// <summary>
    /// Tests that a new season 6 database contains the talisman, and that its packages are on sale.
    /// </summary>
    [Test]
    public async Task NewDatabaseContainsTalismanAsync()
    {
        var (context, gameConfiguration) = await CreateSeason6ConfigurationAsync().ConfigureAwait(false);
        using var _ = context;

        AssertTalismanData(gameConfiguration);
    }

    /// <summary>
    /// Tests that the update adds the talisman to a database which was created before it existed,
    /// assigns it to the products of the in-game shop and puts its packages on sale.
    /// Applying it twice doesn't add the talisman twice.
    /// </summary>
    [Test]
    public async Task UpdateAddsTalismanToExistingDatabaseAsync()
    {
        var (context, gameConfiguration) = await CreateSeason6ConfigurationAsync().ConfigureAwait(false);
        using var _ = context;
        foreach (var package in gameConfiguration.CashShopConfiguration!.Packages.Where(p => TalismanPackages.Contains(p.PackageSequence)))
        {
            package.IsForSale = false;
            package.Products.ToList().ForEach(product => product.ItemDefinition = null);
        }

        gameConfiguration.Items.Where(IsTalisman).ToList().ForEach(item => gameConfiguration.Items.Remove(item));

        var update = new AddTalismanOfResurrectionUpdatePlugIn();
        await update.ApplyUpdateAsync(context, gameConfiguration).ConfigureAwait(false);
        await update.ApplyUpdateAsync(context, gameConfiguration).ConfigureAwait(false);

        AssertTalismanData(gameConfiguration);
    }

    /// <summary>
    /// Tests that the update doesn't assign the talisman to the products of a custom in-game shop script,
    /// because its product sequences may identify other products, and doesn't put its packages on sale.
    /// </summary>
    [Test]
    public async Task UpdateDoesNotAssignTalismanToCustomScriptAsync()
    {
        var (context, gameConfiguration) = await CreateSeason6ConfigurationAsync().ConfigureAwait(false);
        using var _ = context;
        var configuration = gameConfiguration.CashShopConfiguration!;
        configuration.ScriptYearId++;
        foreach (var package in configuration.Packages.Where(p => TalismanPackages.Contains(p.PackageSequence)))
        {
            package.IsForSale = false;
            package.Products.ToList().ForEach(product => product.ItemDefinition = null);
        }

        gameConfiguration.Items.Where(IsTalisman).ToList().ForEach(item => gameConfiguration.Items.Remove(item));

        await new AddTalismanOfResurrectionUpdatePlugIn().ApplyUpdateAsync(context, gameConfiguration).ConfigureAwait(false);

        Assert.That(gameConfiguration.Items.Count(IsTalisman), Is.EqualTo(TalismanNumbers.Length));
        Assert.That(configuration.Packages.SelectMany(p => p.Products), Has.None.Matches<CashShopProduct>(p => p.ItemDefinition is { } item && IsTalisman(item)));
        Assert.That(configuration.Packages.Where(p => TalismanPackages.Contains(p.PackageSequence)), Has.None.Matches<CashShopPackage>(p => p.IsForSale));
    }

    private static bool IsTalisman(ItemDefinition item) => item.Group == 13 && TalismanNumbers.Contains(item.Number);

    private static void AssertTalismanData(GameConfiguration gameConfiguration)
    {
        var talismans = gameConfiguration.Items.Where(IsTalisman).ToList();
        Assert.That(talismans.Select(item => item.Number), Is.EquivalentTo(TalismanNumbers));
        Assert.That(talismans, Has.All.Matches<ItemDefinition>(item => item.Durability == 1 && !item.DropsFromMonsters && !item.IsTradable && !item.IsSellableToNpc && item.Name.ValueInNeutralLanguage.Length > 0));

        var packages = gameConfiguration.CashShopConfiguration!.Packages.Where(p => TalismanPackages.Contains(p.PackageSequence)).ToList();
        Assert.That(packages, Has.Count.EqualTo(TalismanPackages.Length));
        Assert.That(packages, Has.All.Matches<CashShopPackage>(p => p.IsForSale));
        Assert.That(packages.SelectMany(p => p.Products), Has.All.Matches<CashShopProduct>(product => product.ItemDefinition is { } item && IsTalisman(item)));
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
