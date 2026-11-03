// <copyright file="CraftingTalismansDataTest.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.Persistence.Initialization.Tests;

using Microsoft.Extensions.Logging.Abstractions;
using MUnique.OpenMU.DataModel.Configuration;
using MUnique.OpenMU.DataModel.Configuration.ItemCrafting;
using MUnique.OpenMU.DataModel.Configuration.Items;
using MUnique.OpenMU.Persistence.Initialization.Updates;
using MUnique.OpenMU.Persistence.InMemory;

/// <summary>
/// Tests the data of the Talisman of Luck and the Talisman of Chaos Assembly.
/// </summary>
[TestFixture]
internal class CraftingTalismansDataTest
{
    private const byte Group = 14;
    private const short LuckNumber = 53;
    private const short ChaosAssemblyNumber = 96;

    /// <summary>
    /// The craftings of the chaos machine which accept the Talisman of Luck.
    /// </summary>
    private static readonly byte[] LuckCraftings = [3, 4, 22, 23, 49, 50, 6, 7, 11, 24, 25, 26, 27, 30, 31, 32, 38, 39];

    /// <summary>
    /// The craftings of the chaos machine which accept the Talisman of Chaos Assembly.
    /// </summary>
    private static readonly byte[] ChaosAssemblyCraftings = [3, 4, 22, 23, 49, 50];

    /// <summary>
    /// The packages of the in-game shop which contain the Talisman of Luck.
    /// </summary>
    private static readonly int[] LuckPackages = [98, 99, 389];

    /// <summary>
    /// Tests that a new season 6 database contains the talismans, their crafting requirements, and that their packages are on sale.
    /// </summary>
    [Test]
    public async Task NewDatabaseContainsTalismansAsync()
    {
        var (context, gameConfiguration) = await CreateSeason6ConfigurationAsync().ConfigureAwait(false);
        using var _ = context;

        AssertTalismanData(gameConfiguration);
    }

    /// <summary>
    /// Tests that the update adds the talismans to a database which was created before they existed,
    /// adds them to the craftings, assigns them to the products of the in-game shop and puts their packages on sale.
    /// Applying it twice doesn't add anything twice.
    /// </summary>
    [Test]
    public async Task UpdateAddsTalismansToExistingDatabaseAsync()
    {
        var (context, gameConfiguration) = await CreateSeason6ConfigurationAsync().ConfigureAwait(false);
        using var _ = context;
        foreach (var package in gameConfiguration.CashShopConfiguration!.Packages.Where(p => LuckPackages.Contains(p.PackageSequence)))
        {
            package.IsForSale = false;
            package.Products.ToList().ForEach(product => (product.ItemDefinition, product.Quantity) = (null, 1));
        }

        foreach (var settings in gameConfiguration.Monsters.SelectMany(m => m.ItemCraftings).Select(c => c.SimpleCraftingSettings).OfType<SimpleCraftingSettings>())
        {
            settings.RequiredItems.Where(r => r.PossibleItems.Any(IsTalisman)).ToList().ForEach(r => settings.RequiredItems.Remove(r));
        }

        gameConfiguration.Items.Where(IsTalisman).ToList().ForEach(item => gameConfiguration.Items.Remove(item));

        var update = new AddCraftingTalismansUpdatePlugIn();
        await update.ApplyUpdateAsync(context, gameConfiguration).ConfigureAwait(false);
        await update.ApplyUpdateAsync(context, gameConfiguration).ConfigureAwait(false);

        AssertTalismanData(gameConfiguration);
    }

    private static bool IsTalisman(ItemDefinition item) => item.Group == Group && item.Number is LuckNumber or ChaosAssemblyNumber;

    private static void AssertTalismanData(GameConfiguration gameConfiguration)
    {
        var luck = gameConfiguration.Items.Single(item => item.Group == Group && item.Number == LuckNumber);
        var chaosAssembly = gameConfiguration.Items.Single(item => item.Group == Group && item.Number == ChaosAssemblyNumber);
        Assert.That(luck.Durability, Is.EqualTo(10));
        Assert.That(chaosAssembly.Durability, Is.EqualTo(1));
        Assert.That(new[] { luck, chaosAssembly }, Has.All.Matches<ItemDefinition>(item => !item.IsTradable && !item.IsSellableToNpc && !item.IsPersonalStoreSellable && item.Name.ValueInNeutralLanguage.Length > 0));
        Assert.That(luck.IsDroppable, Is.False);
        Assert.That(chaosAssembly.IsDroppable, Is.True);

        var chaosMachine = gameConfiguration.Monsters.First(m => m.NpcWindow == NpcWindow.ChaosMachine).ItemCraftings;
        var petTrainer = gameConfiguration.Monsters.First(m => m.NpcWindow == NpcWindow.PetTrainer).ItemCraftings;
        var luckSettings = chaosMachine.Where(c => LuckCraftings.Contains(c.Number))
            .Concat(petTrainer.Where(c => c.Number is 13 or 14))
            .Select(c => c.SimpleCraftingSettings!)
            .ToList();
        Assert.That(luckSettings, Has.Count.EqualTo(LuckCraftings.Length + 2));
        foreach (var settings in luckSettings)
        {
            var requirement = settings.RequiredItems.Single(r => r.PossibleItems.Contains(luck));
            Assert.That((requirement.MinimumAmount, requirement.MaximumAmount, requirement.AddPercentage), Is.EqualTo(((byte)0, (byte)10, (byte)1)));
        }

        foreach (var crafting in chaosMachine)
        {
            var requirements = crafting.SimpleCraftingSettings?.RequiredItems.Where(r => r.PossibleItems.Contains(chaosAssembly)).ToList() ?? [];
            if (ChaosAssemblyCraftings.Contains(crafting.Number))
            {
                Assert.That(requirements, Has.Count.EqualTo(1));
                Assert.That(requirements[0].FailResult, Is.EqualTo(MixResult.DisappearAndProtectReferencedItems));
                Assert.That(requirements[0].MaximumAmount, Is.EqualTo(1));
            }
            else
            {
                Assert.That(requirements, Is.Empty);
            }
        }

        var packages = gameConfiguration.CashShopConfiguration!.Packages.Where(p => LuckPackages.Contains(p.PackageSequence)).ToList();
        Assert.That(packages, Has.Count.EqualTo(LuckPackages.Length));
        Assert.That(packages, Has.All.Matches<CashShopPackage>(p => p.IsForSale));
        Assert.That(
            packages.SelectMany(p => p.Products).Select(p => (p.ItemDefinition, p.PriceSequence, p.Quantity)),
            Is.EquivalentTo(new[] { (luck, 37, 1), (luck, 38, 5), (luck, 37, 1), (luck, 38, 5), (luck, 583, 10) }));
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
