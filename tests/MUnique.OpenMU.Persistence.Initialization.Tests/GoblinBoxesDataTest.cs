// <copyright file="GoblinBoxesDataTest.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.Persistence.Initialization.Tests;

using Microsoft.Extensions.Logging.Abstractions;
using MUnique.OpenMU.DataModel.Configuration;
using MUnique.OpenMU.DataModel.Configuration.Items;
using MUnique.OpenMU.Persistence.Initialization.Updates;
using MUnique.OpenMU.Persistence.InMemory;

/// <summary>
/// Tests the data of the keys and boxes of the goblin combination.
/// </summary>
[TestFixture]
internal class GoblinBoxesDataTest
{
    private const byte CraftingNumber = 47;

    private static readonly short[] ItemNumbers = [112, 113, 121, 122, 123, 124];

    /// <summary>
    /// The packages of the in-game shop which contain the keys.
    /// </summary>
    private static readonly int[] KeyPackages = [12, 13];

    /// <summary>
    /// Tests that a new season 6 database contains the keys, boxes and combinations, and that the packages of the keys are on sale.
    /// </summary>
    [Test]
    public async Task NewDatabaseContainsGoblinBoxesAsync()
    {
        var (context, gameConfiguration) = await CreateSeason6ConfigurationAsync().ConfigureAwait(false);
        using var _ = context;

        AssertData(gameConfiguration);
    }

    /// <summary>
    /// Tests that the update adds the keys, boxes and combinations to a database which was created before they existed,
    /// assigns the keys to the products of the in-game shop and puts their packages on sale.
    /// Applying it twice doesn't add anything twice.
    /// </summary>
    [Test]
    public async Task UpdateAddsGoblinBoxesToExistingDatabaseAsync()
    {
        var (context, gameConfiguration) = await CreateSeason6ConfigurationAsync().ConfigureAwait(false);
        using var _ = context;
        foreach (var package in gameConfiguration.CashShopConfiguration!.Packages.Where(p => KeyPackages.Contains(p.PackageSequence)))
        {
            package.IsForSale = false;
            package.Products.ToList().ForEach(product => product.ItemDefinition = null);
        }

        var chaosGoblin = GetChaosGoblin(gameConfiguration);
        chaosGoblin.ItemCraftings.Where(c => c.Number == CraftingNumber).ToList().ForEach(c => chaosGoblin.ItemCraftings.Remove(c));
        gameConfiguration.Items.Where(IsGoblinBoxItem).ToList().ForEach(item => gameConfiguration.Items.Remove(item));

        var update = new AddGoblinBoxesUpdatePlugIn();
        await update.ApplyUpdateAsync(context, gameConfiguration).ConfigureAwait(false);
        await update.ApplyUpdateAsync(context, gameConfiguration).ConfigureAwait(false);

        AssertData(gameConfiguration);
    }

    /// <summary>
    /// Tests that the update doesn't assign the keys to the products of a custom in-game shop script,
    /// because its product sequences may identify other products, and doesn't put its packages on sale.
    /// </summary>
    [Test]
    public async Task UpdateDoesNotAssignKeysToCustomScriptAsync()
    {
        var (context, gameConfiguration) = await CreateSeason6ConfigurationAsync().ConfigureAwait(false);
        using var _ = context;
        var configuration = gameConfiguration.CashShopConfiguration!;
        configuration.ScriptYearId++;
        foreach (var package in configuration.Packages.Where(p => KeyPackages.Contains(p.PackageSequence)))
        {
            package.IsForSale = false;
            package.Products.ToList().ForEach(product => product.ItemDefinition = null);
        }

        gameConfiguration.Items.Where(IsGoblinBoxItem).ToList().ForEach(item => gameConfiguration.Items.Remove(item));

        await new AddGoblinBoxesUpdatePlugIn().ApplyUpdateAsync(context, gameConfiguration).ConfigureAwait(false);

        Assert.That(gameConfiguration.Items.Count(IsGoblinBoxItem), Is.EqualTo(ItemNumbers.Length));
        Assert.That(configuration.Packages.SelectMany(p => p.Products), Has.None.Matches<CashShopProduct>(p => p.ItemDefinition is { } item && IsGoblinBoxItem(item)));
        Assert.That(configuration.Packages.Where(p => KeyPackages.Contains(p.PackageSequence)), Has.None.Matches<CashShopPackage>(p => p.IsForSale));
    }

    private static bool IsGoblinBoxItem(ItemDefinition item) => item.Group == 14 && ItemNumbers.Contains(item.Number);

    private static MonsterDefinition GetChaosGoblin(GameConfiguration gameConfiguration) => gameConfiguration.Monsters.First(m => m.NpcWindow == NpcWindow.ChaosMachine);

    private static ItemDefinition GetItem(GameConfiguration gameConfiguration, short number) => gameConfiguration.Items.Single(item => item.Group == 14 && item.Number == number);

    private static void AssertData(GameConfiguration gameConfiguration)
    {
        var items = gameConfiguration.Items.Where(IsGoblinBoxItem).ToList();
        Assert.That(items.Select(item => item.Number), Is.EquivalentTo(ItemNumbers));
        Assert.That(items, Has.All.Matches<ItemDefinition>(item => item.Name.ValueInNeutralLanguage.Length > 0 && !item.IsRepairable && !item.DropsFromMonsters));

        // The keys and sealed boxes can't be dropped; the opened boxes are opened by dropping them.
        Assert.That(items.Where(item => item.Number is 112 or 113 or 121 or 122), Has.All.Matches<ItemDefinition>(item => !item.IsDroppable));
        Assert.That(items.Where(item => item.Number is 112 or 113 or 123 or 124), Has.All.Matches<ItemDefinition>(item => !item.IsTradable && !item.IsSellableToNpc));
        foreach (var box in items.Where(item => item.Number is 123 or 124))
        {
            Assert.That(box.IsDroppable, Is.True);
            Assert.That(box.DropItems, Has.Count.EqualTo(3));
            Assert.That(box.DropItems.Where(group => group.ItemType != SpecialItemType.Money), Has.All.Matches<ItemDropItemGroup>(group => group.PossibleItems.Count > 0));
            Assert.That(box.DropItems.Single(group => group.ItemType == SpecialItemType.Money).Chance, Is.EqualTo(1.0));
        }

        var craftings = GetChaosGoblin(gameConfiguration).ItemCraftings.Where(c => c.Number == CraftingNumber).ToList();
        Assert.That(craftings, Has.Count.EqualTo(2));
        AssertCrafting(craftings, GetItem(gameConfiguration, 113), GetItem(gameConfiguration, 121), GetItem(gameConfiguration, 123));
        AssertCrafting(craftings, GetItem(gameConfiguration, 112), GetItem(gameConfiguration, 122), GetItem(gameConfiguration, 124));

        var packages = gameConfiguration.CashShopConfiguration!.Packages.Where(p => KeyPackages.Contains(p.PackageSequence)).ToList();
        Assert.That(packages, Has.Count.EqualTo(KeyPackages.Length));
        Assert.That(packages, Has.All.Matches<CashShopPackage>(p => p.IsForSale));
        Assert.That(packages.SelectMany(p => p.Products), Has.All.Matches<CashShopProduct>(product => product.ItemDefinition is { } item && item.Number is 112 or 113));
    }

    private static void AssertCrafting(IEnumerable<DataModel.Configuration.ItemCrafting.ItemCrafting> craftings, ItemDefinition key, ItemDefinition sealedBox, ItemDefinition result)
    {
        var crafting = craftings.Single(c => c.SimpleCraftingSettings!.ResultItems.Single().ItemDefinition == result);
        var settings = crafting.SimpleCraftingSettings!;
        Assert.That(settings.SuccessPercent, Is.EqualTo(100));
        Assert.That(settings.Money, Is.Zero);
        Assert.That(settings.RequiredItems.Select(required => required.PossibleItems.Single()), Is.EquivalentTo(new[] { key, sealedBox }));
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
