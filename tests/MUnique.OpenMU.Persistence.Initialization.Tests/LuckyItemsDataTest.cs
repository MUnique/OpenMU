// <copyright file="LuckyItemsDataTest.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.Persistence.Initialization.Tests;

using Microsoft.Extensions.Logging.Abstractions;
using MUnique.OpenMU.DataModel.Configuration;
using MUnique.OpenMU.DataModel.Configuration.Items;
using MUnique.OpenMU.GameLogic.Attributes;
using MUnique.OpenMU.Persistence.Initialization.Updates;
using MUnique.OpenMU.Persistence.Initialization.VersionSeasonSix.Items;
using MUnique.OpenMU.Persistence.InMemory;

/// <summary>
/// Tests the data of the lucky items: the lucky armors, the lucky item tickets, the jewels of extension and elevation,
/// and the lucky item exchange of David.
/// </summary>
[TestFixture]
internal class LuckyItemsDataTest
{
    private const short DavidNumber = 579;

    private const byte TicketExchangeNumber = 51;

    /// <summary>
    /// The packages of the in-game shop which contain the lucky item tickets.
    /// </summary>
    private static readonly int[] TicketPackages = [360, 361, 362, 363, 364, 365, 366, 367, 368, 369];

    /// <summary>
    /// Tests that a new season 6 database contains the lucky items, David's exchange and the ticket packages on sale.
    /// </summary>
    [Test]
    public async ValueTask NewDatabaseContainsLuckyItemsAsync()
    {
        var (context, gameConfiguration) = await CreateSeason6ConfigurationAsync().ConfigureAwait(false);
        using var _ = context;

        AssertLuckyItemData(gameConfiguration);
    }

    /// <summary>
    /// Tests that the update adds the lucky items, David's exchange and the ticket products to a database which was
    /// created before they existed. Applying it twice doesn't add anything twice.
    /// </summary>
    [Test]
    public async ValueTask UpdateAddsLuckyItemsToExistingDatabaseAsync()
    {
        var (context, gameConfiguration) = await CreateSeason6ConfigurationAsync().ConfigureAwait(false);
        using var _ = context;
        RemoveLuckyItems(gameConfiguration);

        var update = new AddLuckyItemsUpdatePlugIn();
        await update.ApplyUpdateAsync(context, gameConfiguration).ConfigureAwait(false);
        await update.ApplyUpdateAsync(context, gameConfiguration).ConfigureAwait(false);

        AssertLuckyItemData(gameConfiguration);
    }

    /// <summary>
    /// Tests that the update repairs a database in which only some of the lucky items are missing, and David has his
    /// window but no exchange yet.
    /// </summary>
    [Test]
    public async ValueTask UpdateRepairsPartialDataAsync()
    {
        var (context, gameConfiguration) = await CreateSeason6ConfigurationAsync().ConfigureAwait(false);
        using var _ = context;
        RemoveLuckyItems(gameConfiguration);
        var ticketsOnly = new LuckyItems(context, gameConfiguration).CreateItems().Where(item => item.Group == 13).ToList();
        ItemRules.Apply(ticketsOnly);
        gameConfiguration.Items.Where(item => LuckyItems.IsLuckyItem(item) && item.Group != 13).ToList().ForEach(item => gameConfiguration.Items.Remove(item));
        gameConfiguration.Monsters.Single(m => m.Number == DavidNumber).NpcWindow = NpcWindow.CombineLuckyItem;

        await new AddLuckyItemsUpdatePlugIn().ApplyUpdateAsync(context, gameConfiguration).ConfigureAwait(false);

        AssertLuckyItemData(gameConfiguration);
        Assert.That(gameConfiguration.Items.Where(item => item.Group == 13 && LuckyItems.IsLuckyItem(item)), Is.EquivalentTo(ticketsOnly));
    }

    /// <summary>
    /// Tests that the update doesn't assign the tickets to the products of a custom in-game shop script,
    /// because its product sequences may identify other products. The items and David's exchange are still added.
    /// </summary>
    [Test]
    public async ValueTask UpdateDoesNotAssignTicketsToCustomScriptAsync()
    {
        var (context, gameConfiguration) = await CreateSeason6ConfigurationAsync().ConfigureAwait(false);
        using var _ = context;
        RemoveLuckyItems(gameConfiguration);
        gameConfiguration.CashShopConfiguration!.ScriptYearId++;

        await new AddLuckyItemsUpdatePlugIn().ApplyUpdateAsync(context, gameConfiguration).ConfigureAwait(false);

        Assert.That(gameConfiguration.Items.Count(LuckyItems.IsLuckyItem), Is.EqualTo(65));
        Assert.That(gameConfiguration.Monsters.Single(m => m.Number == DavidNumber).ItemCraftings.Any(c => c.Number == TicketExchangeNumber), Is.True);
        var packages = gameConfiguration.CashShopConfiguration.Packages.Where(p => TicketPackages.Contains(p.PackageSequence)).ToList();
        Assert.That(packages.SelectMany(p => p.Products), Has.All.Matches<CashShopProduct>(p => p.ItemDefinition is null));
        Assert.That(packages, Has.None.Matches<CashShopPackage>(p => p.IsForSale));
    }

    private static void RemoveLuckyItems(GameConfiguration gameConfiguration)
    {
        foreach (var package in gameConfiguration.CashShopConfiguration!.Packages.Where(p => TicketPackages.Contains(p.PackageSequence)))
        {
            package.IsForSale = false;
            package.Products.ToList().ForEach(product => product.ItemDefinition = null);
        }

        var david = gameConfiguration.Monsters.Single(m => m.Number == DavidNumber);
        david.NpcWindow = NpcWindow.Undefined;
        david.ItemCraftings.Clear();
        gameConfiguration.Items.Where(LuckyItems.IsLuckyItem).ToList().ForEach(item => gameConfiguration.Items.Remove(item));
    }

    private static void AssertLuckyItemData(GameConfiguration gameConfiguration)
    {
        var armors = gameConfiguration.Items.Where(LuckyItems.IsLuckyArmor).ToList();
        var tickets = gameConfiguration.Items.Where(item => item.Group == 13 && LuckyItems.IsLuckyItem(item)).ToList();
        var jewels = gameConfiguration.Items.Where(item => item.Group == 14 && LuckyItems.IsLuckyItem(item)).ToList();
        Assert.Multiple(() =>
        {
            Assert.That(armors, Has.Count.EqualTo(53));
            Assert.That(tickets.Select(t => t.Number), Is.EquivalentTo(Enumerable.Range(135, 10).Select(n => (short)n)));
            Assert.That(jewels.Select(j => j.Number), Is.EquivalentTo(new short[] { 160, 161 }));
            Assert.That(armors, Has.All.Matches<ItemDefinition>(item =>
                item.Durability == byte.MaxValue
                && item.MaximumItemLevel == 0
                && !item.DropsFromMonsters
                && !item.ExcellentRaisesLevel
                && !item.IsTradable && !item.IsDroppable && !item.IsStorable && !item.IsPersonalStoreSellable && !item.IsRepairable
                && item.QualifiedCharacters.Count > 0
                && item.ItemSlot is not null
                && item.StatProfile is not null
                && item.Requirements.Any(r => r.Attribute == Stats.Level && r.MinimumValue is 1 or 150)
                && item.BasePowerUpAttributes.Any(p => p.TargetAttribute == Stats.DefenseBase)));
            Assert.That(tickets.Concat(jewels), Has.All.Matches<ItemDefinition>(item =>
                item.Durability == 1 && !item.DropsFromMonsters && !item.IsDroppable && !item.IsSellableToNpc && !item.IsPersonalStoreSellable && item.IsTradable));
            Assert.That(gameConfiguration.Items.GroupBy(item => (item.Group, item.Number)).Where(g => g.Count() > 1).Select(g => g.Key), Is.Empty);

            var david = gameConfiguration.Monsters.Single(m => m.Number == DavidNumber);
            Assert.That(david.NpcWindow, Is.EqualTo(NpcWindow.CombineLuckyItem));
            var exchange = david.ItemCraftings.Single(c => c.Number == TicketExchangeNumber).SimpleCraftingSettings!;
            Assert.That(exchange.RequiredItems.SelectMany(r => r.PossibleItems), Is.EquivalentTo(tickets));
            Assert.That(exchange.ResultItems.Select(r => r.ItemDefinition), Is.EquivalentTo(armors));

            var packages = gameConfiguration.CashShopConfiguration!.Packages.Where(p => TicketPackages.Contains(p.PackageSequence)).ToList();
            Assert.That(packages, Has.Count.EqualTo(TicketPackages.Length));
            Assert.That(packages, Has.All.Matches<CashShopPackage>(p => p.IsForSale));
            Assert.That(packages.SelectMany(p => p.Products).Select(p => p.ItemDefinition), Is.EquivalentTo(tickets));
        });
    }

    private static async ValueTask<(IContext Context, GameConfiguration GameConfiguration)> CreateSeason6ConfigurationAsync()
    {
        var contextProvider = new InMemoryPersistenceContextProvider();
        var dataInitialization = new VersionSeasonSix.DataInitialization(contextProvider, new NullLoggerFactory());
        await dataInitialization.CreateInitialDataAsync(1, true).ConfigureAwait(false);

        var context = contextProvider.CreateNewContext();
        var gameConfiguration = (await context.GetAsync<GameConfiguration>().ConfigureAwait(false)).First();
        return (context, gameConfiguration);
    }
}
