// <copyright file="ResetFruitsDataTest.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.Persistence.Initialization.Tests;

using Microsoft.Extensions.Logging.Abstractions;
using MUnique.OpenMU.DataModel.Configuration;
using MUnique.OpenMU.DataModel.Configuration.Items;
using MUnique.OpenMU.Persistence.Initialization.Updates;
using MUnique.OpenMU.Persistence.InMemory;

/// <summary>
/// Tests the data of the reset fruits of the in-game shop.
/// </summary>
[TestFixture]
internal class ResetFruitsDataTest
{
    private static readonly short[] FruitNumbers = [54, 55, 56, 57, 58];

    /// <summary>
    /// The packages of the in-game shop which contain the fruits, for WCoin (C) and (P).
    /// </summary>
    private static readonly int[] FruitPackages = [106, 107, 108, 109, 110, 111, 112, 113, 114, 115];

    /// <summary>
    /// Tests that a new season 6 database contains the fruits, and that their packages are on sale.
    /// </summary>
    [Test]
    public async Task NewDatabaseContainsFruitsAsync()
    {
        var (context, gameConfiguration) = await CreateSeason6ConfigurationAsync().ConfigureAwait(false);
        using var _ = context;

        AssertFruitData(gameConfiguration);
    }

    /// <summary>
    /// Tests that the update adds the fruits to a database which was created before they existed,
    /// assigns them to the products of the in-game shop and puts their packages on sale.
    /// Applying it twice doesn't add the fruits twice.
    /// </summary>
    [Test]
    public async Task UpdateAddsFruitsToExistingDatabaseAsync()
    {
        var (context, gameConfiguration) = await CreateSeason6ConfigurationAsync().ConfigureAwait(false);
        using var _ = context;
        foreach (var package in gameConfiguration.CashShopConfiguration!.Packages.Where(p => FruitPackages.Contains(p.PackageSequence)))
        {
            package.IsForSale = false;
            package.Products.ToList().ForEach(product => (product.ItemDefinition, product.Quantity) = (null, 1));
        }

        gameConfiguration.Items.Where(IsFruit).ToList().ForEach(item => gameConfiguration.Items.Remove(item));

        var update = new AddResetFruitsUpdatePlugIn();
        await update.ApplyUpdateAsync(context, gameConfiguration).ConfigureAwait(false);
        await update.ApplyUpdateAsync(context, gameConfiguration).ConfigureAwait(false);

        AssertFruitData(gameConfiguration);
    }

    /// <summary>
    /// Tests that the update applies the durability, the rules and the product quantities to fruits which
    /// already exist, e.g. because the cash shop update created them without these in the same run.
    /// </summary>
    [Test]
    public async Task UpdateAppliesRulesToExistingFruitsAsync()
    {
        var (context, gameConfiguration) = await CreateSeason6ConfigurationAsync().ConfigureAwait(false);
        using var _ = context;
        foreach (var fruit in gameConfiguration.Items.Where(IsFruit))
        {
            (fruit.Durability, fruit.IsTradable, fruit.IsDroppable, fruit.IsSellableToNpc, fruit.IsPersonalStoreSellable) = (1, true, true, true, true);
        }

        foreach (var package in gameConfiguration.CashShopConfiguration!.Packages.Where(p => FruitPackages.Contains(p.PackageSequence)))
        {
            package.IsForSale = false;
            package.Products.ToList().ForEach(product => (product.ItemDefinition, product.Quantity) = (null, 1));
        }

        await new AddResetFruitsUpdatePlugIn().ApplyUpdateAsync(context, gameConfiguration).ConfigureAwait(false);

        AssertFruitData(gameConfiguration);
        Assert.That(gameConfiguration.Items.Where(IsFruit), Has.All.Matches<ItemDefinition>(item => !item.IsDroppable && !item.IsPersonalStoreSellable));
    }

    private static bool IsFruit(ItemDefinition item) => item.Group == 13 && FruitNumbers.Contains(item.Number);

    private static void AssertFruitData(GameConfiguration gameConfiguration)
    {
        var fruits = gameConfiguration.Items.Where(IsFruit).ToList();
        Assert.That(fruits.Select(item => item.Number), Is.EquivalentTo(FruitNumbers));
        Assert.That(fruits, Has.All.Matches<ItemDefinition>(item => item.Durability == 100 && !item.IsTradable && !item.IsSellableToNpc && item.Name.ValueInNeutralLanguage.Length > 0));

        var packages = gameConfiguration.CashShopConfiguration!.Packages.Where(p => FruitPackages.Contains(p.PackageSequence)).ToList();
        Assert.That(packages, Has.Count.EqualTo(FruitPackages.Length));
        Assert.That(packages, Has.All.Matches<CashShopPackage>(p => p.IsForSale));
        Assert.That(packages.SelectMany(p => p.Products), Has.All.Matches<CashShopProduct>(product => product.ItemDefinition is { } item && IsFruit(item) && product.Quantity == 100));
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
