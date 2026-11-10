// <copyright file="FigurinesAndCharmsDataTest.cs" company="MUnique">
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
/// Tests the data of the figurines and charms, which are used inside the inventory.
/// </summary>
[TestFixture]
internal class FigurinesAndCharmsDataTest
{
    private static readonly short[] Numbers = [128, 129, 130, 131, 132, 133];

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
    /// Tests that the update adds the items to a database which was created before they existed.
    /// Applying it twice doesn't add the items twice.
    /// </summary>
    [Test]
    public async Task UpdateAddsItemsToExistingDatabaseAsync()
    {
        var (context, gameConfiguration) = await CreateSeason6ConfigurationAsync().ConfigureAwait(false);
        using var _ = context;
        gameConfiguration.Items.Where(IsFigurineOrCharm).ToList().ForEach(item => gameConfiguration.Items.Remove(item));

        var update = new AddFigurinesAndCharmsUpdatePlugIn();
        await update.ApplyUpdateAsync(context, gameConfiguration).ConfigureAwait(false);
        await update.ApplyUpdateAsync(context, gameConfiguration).ConfigureAwait(false);

        AssertItemData(gameConfiguration);
    }

    /// <summary>
    /// Tests that the update links the products of the cash shop to the items,
    /// which have no item definition in a database which was created before the items existed.
    /// </summary>
    [Test]
    public async Task UpdateLinksCashShopProductsAsync()
    {
        var (context, gameConfiguration) = await CreateSeason6ConfigurationAsync().ConfigureAwait(false);
        using var _ = context;
        var products = gameConfiguration.CashShopConfiguration!.Packages
            .SelectMany(package => package.Products)
            .Where(product => product.ItemDefinition is { } definition && IsFigurineOrCharm(definition))
            .ToList();
        Assert.That(products.Select(product => product.ProductSequence).Distinct(), Is.EquivalentTo(new[] { 459, 460, 461, 462 }));
        products.ForEach(product => product.ItemDefinition = null);
        gameConfiguration.Items.Where(IsFigurineOrCharm).ToList().ForEach(item => gameConfiguration.Items.Remove(item));

        await new AddFigurinesAndCharmsUpdatePlugIn().ApplyUpdateAsync(context, gameConfiguration).ConfigureAwait(false);

        Assert.That(products, Has.All.Matches<CashShopProduct>(product => product.ItemDefinition is { Group: 13 } definition && gameConfiguration.Items.Contains(definition)));
        Assert.That(products.Where(product => product.ProductSequence == 459).Select(product => product.ItemDefinition!.Number), Has.All.EqualTo(128));
        Assert.That(products.Where(product => product.ProductSequence == 462).Select(product => product.ItemDefinition!.Number), Has.All.EqualTo(132));
    }

    private static bool IsFigurineOrCharm(ItemDefinition item) => item.Group == 13 && Numbers.Contains(item.Number);

    private static void AssertItemData(GameConfiguration gameConfiguration)
    {
        var items = gameConfiguration.Items.Where(IsFigurineOrCharm).ToList();
        Assert.That(items.Select(item => item.Number), Is.EquivalentTo(Numbers));
        Assert.That(items, Has.All.Matches<ItemDefinition>(item =>
            item.Durability == 1
            && item.ItemSlot is null
            && item.Name.ValueInNeutralLanguage.Length > 0));

        Assert.That(PowerUp(items, 128, Stats.CriticalDamageBonus), Is.EqualTo(10));
        Assert.That(PowerUp(items, 129, Stats.ExcellentDamageBonus), Is.EqualTo(10));
        Assert.That(PowerUp(items, 130, Stats.MaximumHealth), Is.EqualTo(50));
        Assert.That(PowerUp(items, 131, Stats.MaximumMana), Is.EqualTo(50));
        Assert.That(PowerUp(items, 132, Stats.MaximumHealth), Is.EqualTo(150));
        Assert.That(PowerUp(items, 132, Stats.MaximumShield), Is.EqualTo(50));
        Assert.That(PowerUp(items, 133, Stats.MaximumMana), Is.EqualTo(150));
        Assert.That(PowerUp(items, 133, Stats.MaximumAbility), Is.EqualTo(50));
    }

    private static float? PowerUp(IEnumerable<ItemDefinition> items, short number, AttributeDefinition attribute)
    {
        return items.Single(item => item.Number == number).BasePowerUpAttributes
            .SingleOrDefault(powerUp => powerUp.TargetAttribute == attribute)?.BaseValue;
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
