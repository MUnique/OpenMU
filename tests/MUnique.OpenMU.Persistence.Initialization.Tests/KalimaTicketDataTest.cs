// <copyright file="KalimaTicketDataTest.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.Persistence.Initialization.Tests;

using Microsoft.Extensions.Logging.Abstractions;
using MUnique.OpenMU.DataModel.Configuration;
using MUnique.OpenMU.DataModel.Configuration.Items;
using MUnique.OpenMU.Persistence.Initialization.Updates;
using MUnique.OpenMU.Persistence.InMemory;

/// <summary>
/// Tests the data of the kalima ticket.
/// </summary>
[TestFixture]
internal class KalimaTicketDataTest
{
    private static readonly short[] TicketNumbers = [48];

    /// <summary>
    /// The packages of the in-game shop which contain the ticket, for WCoin (C) and (P).
    /// </summary>
    private static readonly int[] TicketPackages = [56, 57];

    /// <summary>
    /// Tests that a new season 6 database contains the ticket, and that its packages are on sale.
    /// </summary>
    [Test]
    public async Task NewDatabaseContainsTicketAsync()
    {
        var (context, gameConfiguration) = await CreateSeason6ConfigurationAsync().ConfigureAwait(false);
        using var _ = context;

        AssertTicketData(gameConfiguration);
    }

    /// <summary>
    /// Tests that the update adds the ticket to a database which was created before it existed,
    /// assigns it to the products of the in-game shop and puts its packages on sale.
    /// Applying it twice doesn't add the ticket twice.
    /// </summary>
    [Test]
    public async Task UpdateAddsTicketToExistingDatabaseAsync()
    {
        var (context, gameConfiguration) = await CreateSeason6ConfigurationAsync().ConfigureAwait(false);
        using var _ = context;
        foreach (var package in gameConfiguration.CashShopConfiguration!.Packages.Where(p => TicketPackages.Contains(p.PackageSequence)))
        {
            package.IsForSale = false;
            package.Products.ToList().ForEach(product => product.ItemDefinition = null);
        }

        gameConfiguration.Items.Where(IsTicket).ToList().ForEach(item => gameConfiguration.Items.Remove(item));

        var update = new AddKalimaTicketUpdatePlugIn();
        await update.ApplyUpdateAsync(context, gameConfiguration).ConfigureAwait(false);
        await update.ApplyUpdateAsync(context, gameConfiguration).ConfigureAwait(false);

        AssertTicketData(gameConfiguration);
    }

    private static bool IsTicket(ItemDefinition item) => item.Group == 13 && TicketNumbers.Contains(item.Number);

    private static void AssertTicketData(GameConfiguration gameConfiguration)
    {
        var tickets = gameConfiguration.Items.Where(IsTicket).ToList();
        Assert.That(tickets.Select(item => item.Number), Is.EquivalentTo(TicketNumbers));
        Assert.That(tickets, Has.All.Matches<ItemDefinition>(item => item.Durability == 10 && !item.DropsFromMonsters && !item.IsTradable && !item.IsSellableToNpc && item.Name.ValueInNeutralLanguage.Length > 0));

        var packages = gameConfiguration.CashShopConfiguration!.Packages.Where(p => TicketPackages.Contains(p.PackageSequence)).ToList();
        Assert.That(packages, Has.Count.EqualTo(TicketPackages.Length));
        Assert.That(packages, Has.All.Matches<CashShopPackage>(p => p.IsForSale));
        Assert.That(packages.SelectMany(p => p.Products), Has.All.Matches<CashShopProduct>(product => product.ItemDefinition is { } item && IsTicket(item)));
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
