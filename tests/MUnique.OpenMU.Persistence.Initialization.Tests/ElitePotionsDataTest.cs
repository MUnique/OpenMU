// <copyright file="ElitePotionsDataTest.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.Persistence.Initialization.Tests;

using Microsoft.Extensions.Logging.Abstractions;
using MUnique.OpenMU.DataModel.Configuration;
using MUnique.OpenMU.DataModel.Configuration.Items;
using MUnique.OpenMU.Persistence.Initialization.Updates;
using MUnique.OpenMU.Persistence.InMemory;

/// <summary>
/// Tests the data of the elite potions of the in-game shop.
/// </summary>
[TestFixture]
internal class ElitePotionsDataTest
{
    private static readonly short[] PotionNumbers = [70, 71, 94, 133];

    /// <summary>
    /// The packages of the in-game shop which only contain elite potions, for WCoin (C) and (P).
    /// </summary>
    private static readonly int[] PotionPackages = [4, 5, 94, 95, 96, 97];

    /// <summary>
    /// The gladiator packages, which contain an elite SD potion besides time-limited items.
    /// </summary>
    private static readonly int[] GladiatorPackages = [162, 163];

    /// <summary>
    /// Tests that a new season 6 database contains the potions, and that their packages are on sale.
    /// </summary>
    [Test]
    public async Task NewDatabaseContainsPotionsAsync()
    {
        var (context, gameConfiguration) = await CreateSeason6ConfigurationAsync().ConfigureAwait(false);
        using var _ = context;

        AssertPotionData(gameConfiguration);
    }

    /// <summary>
    /// Tests that the update adds the potions to a database which was created before they existed,
    /// assigns them to the products of the in-game shop and puts their packages on sale.
    /// Applying it twice doesn't add the potions twice.
    /// </summary>
    [Test]
    public async Task UpdateAddsPotionsToExistingDatabaseAsync()
    {
        var (context, gameConfiguration) = await CreateSeason6ConfigurationAsync().ConfigureAwait(false);
        using var _ = context;
        foreach (var package in gameConfiguration.CashShopConfiguration!.Packages.Where(p => PotionPackages.Contains(p.PackageSequence)))
        {
            package.IsForSale = false;
        }

        foreach (var product in gameConfiguration.CashShopConfiguration.Packages.SelectMany(p => p.Products).Where(p => p.ItemDefinition is { } item && IsPotion(item)))
        {
            product.ItemDefinition = null;
        }

        gameConfiguration.Items.Where(IsPotion).ToList().ForEach(item => gameConfiguration.Items.Remove(item));

        var update = new AddElitePotionsUpdatePlugIn();
        await update.ApplyUpdateAsync(context, gameConfiguration).ConfigureAwait(false);
        await update.ApplyUpdateAsync(context, gameConfiguration).ConfigureAwait(false);

        AssertPotionData(gameConfiguration);
    }

    /// <summary>
    /// Tests that the update doesn't assign the potions to the products of a custom in-game shop script,
    /// because its product sequences may identify other products, and doesn't put its packages on sale.
    /// </summary>
    [Test]
    public async Task UpdateDoesNotAssignPotionsToCustomScriptAsync()
    {
        var (context, gameConfiguration) = await CreateSeason6ConfigurationAsync().ConfigureAwait(false);
        using var _ = context;
        var configuration = gameConfiguration.CashShopConfiguration!;
        configuration.ScriptYearId++;
        foreach (var package in configuration.Packages.Where(p => PotionPackages.Contains(p.PackageSequence)))
        {
            package.IsForSale = false;
        }

        foreach (var product in configuration.Packages.SelectMany(p => p.Products).Where(p => p.ItemDefinition is { } item && IsPotion(item)))
        {
            product.ItemDefinition = null;
        }

        gameConfiguration.Items.Where(IsPotion).ToList().ForEach(item => gameConfiguration.Items.Remove(item));

        await new AddElitePotionsUpdatePlugIn().ApplyUpdateAsync(context, gameConfiguration).ConfigureAwait(false);

        Assert.That(gameConfiguration.Items.Count(IsPotion), Is.EqualTo(PotionNumbers.Length));
        Assert.That(configuration.Packages.SelectMany(p => p.Products), Has.None.Matches<CashShopProduct>(p => p.ItemDefinition is { } item && IsPotion(item)));
        Assert.That(configuration.Packages.Where(p => PotionPackages.Contains(p.PackageSequence)), Has.None.Matches<CashShopPackage>(p => p.IsForSale));
    }

    private static bool IsPotion(ItemDefinition item) => item.Group == 14 && PotionNumbers.Contains(item.Number);

    private static void AssertPotionData(GameConfiguration gameConfiguration)
    {
        var potions = gameConfiguration.Items.Where(IsPotion).ToList();
        Assert.That(potions.Select(item => item.Number), Is.EquivalentTo(PotionNumbers));
        Assert.That(potions, Has.All.Matches<ItemDefinition>(item => item.Durability == 50 && !item.IsTradable && !item.IsDroppable && !item.IsSellableToNpc && item.Name.ValueInNeutralLanguage.Length > 0));

        var packages = gameConfiguration.CashShopConfiguration!.Packages.Where(p => PotionPackages.Contains(p.PackageSequence)).ToList();
        Assert.That(packages, Has.Count.EqualTo(PotionPackages.Length));
        Assert.That(packages, Has.All.Matches<CashShopPackage>(p => p.IsForSale));
        Assert.That(packages.SelectMany(p => p.Products), Has.All.Matches<CashShopProduct>(product => product.ItemDefinition is { } item && IsPotion(item)));

        var gladiatorPotions = gameConfiguration.CashShopConfiguration.Packages
            .Where(p => GladiatorPackages.Contains(p.PackageSequence))
            .SelectMany(p => p.Products)
            .Where(p => p.ProductSequence == 116)
            .ToList();
        Assert.That(gladiatorPotions, Has.Count.EqualTo(GladiatorPackages.Length));
        Assert.That(gladiatorPotions, Has.All.Matches<CashShopProduct>(product => product.ItemDefinition is { Group: 14, Number: 133 }));
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
