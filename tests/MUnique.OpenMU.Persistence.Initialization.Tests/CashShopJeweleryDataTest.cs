// <copyright file="CashShopJeweleryDataTest.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.Persistence.Initialization.Tests;

using Microsoft.Extensions.Logging.Abstractions;
using MUnique.OpenMU.AttributeSystem;
using MUnique.OpenMU.DataModel.Configuration;
using MUnique.OpenMU.DataModel.Configuration.Items;
using MUnique.OpenMU.GameLogic.Attributes;
using MUnique.OpenMU.Persistence.Initialization.Updates;
using MUnique.OpenMU.Persistence.InMemory;

/// <summary>
/// Tests the data of the rings and necklaces of the in-game shop.
/// </summary>
[TestFixture]
internal class CashShopJeweleryDataTest
{
    private static readonly short[] RingNumbers = [107, 109, 110, 111, 112];

    private static readonly short[] NecklaceNumbers = [113, 114, 115];

    /// <summary>
    /// The packages of the in-game shop which contain the items, for WCoin (C) and (P).
    /// The Gladiator Package (162 and 163) contains the Lethal Wizard's Ring and the Ruby Necklace.
    /// </summary>
    private static readonly int[] JeweleryPackages = [14, 15, 16, 17, 18, 19, 20, 21, 22, 23, 24, 25, 26, 27, 147, 148, 162, 163];

    /// <summary>
    /// Tests that a new season 6 database contains the items.
    /// </summary>
    [Test]
    public async Task NewDatabaseContainsItemsAsync()
    {
        var (context, gameConfiguration) = await CreateSeason6ConfigurationAsync().ConfigureAwait(false);
        using var _ = context;

        AssertItemData(gameConfiguration);
    }

    /// <summary>
    /// Tests that the update adds the items to a database which was created before they existed,
    /// and assigns them to the products of the in-game shop. Applying it twice doesn't add the items twice.
    /// </summary>
    [Test]
    public async Task UpdateAddsItemsToExistingDatabaseAsync()
    {
        var (context, gameConfiguration) = await CreateSeason6ConfigurationAsync().ConfigureAwait(false);
        using var _ = context;
        foreach (var product in gameConfiguration.CashShopConfiguration!.Packages.SelectMany(p => p.Products).Where(p => p.ItemDefinition is { } item && IsJewelery(item)))
        {
            product.ItemDefinition = null;
        }

        gameConfiguration.Items.Where(IsJewelery).ToList().ForEach(item => gameConfiguration.Items.Remove(item));

        var update = new AddCashShopJeweleryUpdatePlugIn();
        await update.ApplyUpdateAsync(context, gameConfiguration).ConfigureAwait(false);
        await update.ApplyUpdateAsync(context, gameConfiguration).ConfigureAwait(false);

        AssertItemData(gameConfiguration);
    }

    /// <summary>
    /// Tests that the update doesn't assign the items to the products of a custom in-game shop script,
    /// because its product sequences may identify other products.
    /// </summary>
    [Test]
    public async Task UpdateDoesNotAssignItemsToCustomScriptAsync()
    {
        var (context, gameConfiguration) = await CreateSeason6ConfigurationAsync().ConfigureAwait(false);
        using var _ = context;
        var configuration = gameConfiguration.CashShopConfiguration!;
        configuration.ScriptYearId++;
        foreach (var product in configuration.Packages.SelectMany(p => p.Products).Where(p => p.ItemDefinition is { } item && IsJewelery(item)))
        {
            product.ItemDefinition = null;
        }

        gameConfiguration.Items.Where(IsJewelery).ToList().ForEach(item => gameConfiguration.Items.Remove(item));

        await new AddCashShopJeweleryUpdatePlugIn().ApplyUpdateAsync(context, gameConfiguration).ConfigureAwait(false);

        Assert.That(gameConfiguration.Items.Count(IsJewelery), Is.EqualTo(RingNumbers.Length + NecklaceNumbers.Length));
        Assert.That(configuration.Packages.SelectMany(p => p.Products), Has.None.Matches<CashShopProduct>(p => p.ItemDefinition is { } item && IsJewelery(item)));
    }

    private static bool IsJewelery(ItemDefinition item) => item.Group == 13 && (RingNumbers.Contains(item.Number) || NecklaceNumbers.Contains(item.Number));

    private static void AssertItemData(GameConfiguration gameConfiguration)
    {
        var items = gameConfiguration.Items.Where(IsJewelery).ToList();
        Assert.That(items.Select(item => item.Number), Is.EquivalentTo(RingNumbers.Concat(NecklaceNumbers)));
        Assert.That(items, Has.All.Matches<ItemDefinition>(item =>
            item.Durability > 0
            && item.MaximumItemLevel == 0
            && !item.DropsFromMonsters
            && !item.IsTradable
            && item.IsSellableToNpc
            && !item.IsRepairable
            && item.StatProfile is not null
            && item.PossibleItemOptions.Count == 0
            && item.QualifiedCharacters.Count == gameConfiguration.CharacterClasses.Count
            && item.Name.ValueInNeutralLanguage.Length > 0));
        Assert.That(items.Where(item => RingNumbers.Contains(item.Number)), Has.All.Matches<ItemDefinition>(item => item.ItemSlot!.ItemSlots.Contains(10)));
        Assert.That(items.Where(item => NecklaceNumbers.Contains(item.Number)), Has.All.Matches<ItemDefinition>(item => item.ItemSlot!.ItemSlots.Contains(9)));

        Assert.That(PowerUp(items, 107, Stats.AttackSpeedAny), Is.EqualTo(10));
        Assert.That(PowerUp(items, 107, Stats.MaximumWizBaseDmg), Is.EqualTo(1.15f));
        Assert.That(PowerUp(items, 109, Stats.MaximumMana), Is.EqualTo(1.04f));
        Assert.That(PowerUp(items, 110, Stats.MaximumHealth), Is.EqualTo(1.04f));
        Assert.That(PowerUp(items, 111, Stats.MoneyAmountRate), Is.EqualTo(1.3f));
        Assert.That(PowerUp(items, 112, Stats.ArmorDamageDecrease), Is.EqualTo(0.04f));
        Assert.That(PowerUp(items, 113, Stats.ExcellentDamageChance), Is.EqualTo(0.1f));
        Assert.That(PowerUp(items, 114, Stats.AttackSpeedAny), Is.EqualTo(7));
        Assert.That(PowerUp(items, 115, Stats.ManaAfterMonsterKillMultiplier), Is.EqualTo(1f / 8f));
        Assert.That(items.Where(item => item.Number != 107), Has.All.Matches<ItemDefinition>(item => PowerUp(item, Stats.HealthRecoveryMultiplier) == 0.03f));

        var packages = gameConfiguration.CashShopConfiguration!.Packages.Where(p => JeweleryPackages.Contains(p.PackageSequence)).ToList();
        Assert.That(packages, Has.Count.EqualTo(JeweleryPackages.Length));

        // Per coin type: two products in each of the eight packages of a single item, and two in the Gladiator Package.
        Assert.That(packages.SelectMany(p => p.Products).Count(p => p.ItemDefinition is { } item && IsJewelery(item)), Is.EqualTo(36));

        // The products are time-limited, which isn't supported yet.
        Assert.That(packages, Has.None.Matches<CashShopPackage>(p => p.IsForSale));
    }

    private static float? PowerUp(IEnumerable<ItemDefinition> items, short number, AttributeDefinition attribute)
    {
        return PowerUp(items.Single(item => item.Number == number), attribute);
    }

    private static float? PowerUp(ItemDefinition item, AttributeDefinition attribute)
    {
        return item.BasePowerUpAttributes.SingleOrDefault(powerUp => powerUp.TargetAttribute == attribute)?.BaseValue;
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
