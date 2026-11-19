// <copyright file="SocketCountTablesTest.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.Persistence.Initialization.Tests;

using Microsoft.Extensions.Logging.Abstractions;
using MUnique.OpenMU.DataModel.Configuration;
using MUnique.OpenMU.DataModel.Configuration.Items;
using MUnique.OpenMU.Persistence.Initialization.Updates;
using MUnique.OpenMU.Persistence.InMemory;

/// <summary>
/// Tests for the tables of the number of sockets of dropped socket items.
/// </summary>
[TestFixture]
internal class SocketCountTablesTest
{
    private const int MaximumSocketCount = 5;

    /// <summary>
    /// Tests that a new season 6 database has the tables, and that every socket item refers to the one of its kind.
    /// </summary>
    [Test]
    public async Task NewDatabaseHasTablesAsync()
    {
        var (context, gameConfiguration) = await CreateSeason6ConfigurationAsync().ConfigureAwait(false);
        using var _ = context;

        AssertSocketCountTables(gameConfiguration);
    }

    /// <summary>
    /// Tests that the update adds the tables to an existing database, and that applying it twice changes nothing.
    /// </summary>
    [Test]
    public async Task UpdateAddsTablesAsync()
    {
        var (context, gameConfiguration) = await CreateSeason6ConfigurationAsync().ConfigureAwait(false);
        using var _ = context;
        foreach (var socketItem in GetSocketItems(gameConfiguration))
        {
            // The state before the tables: three sockets, five for the Stinger Bow.
            socketItem.SocketCountTable = null;
            socketItem.MaximumSockets = socketItem is { Group: 4, Number: 23 } ? 5 : 3;
        }

        gameConfiguration.WeightedValueTables.Clear();

        var update = new AddSocketCountTablesUpdatePlugIn();
        await update.ApplyUpdateAsync(context, gameConfiguration).ConfigureAwait(false);
        await update.ApplyUpdateAsync(context, gameConfiguration).ConfigureAwait(false);

        AssertSocketCountTables(gameConfiguration);
    }

    /// <summary>
    /// Tests that the update keeps a socket count table which a socket item already has.
    /// </summary>
    [Test]
    public async Task UpdateKeepsExistingTableAsync()
    {
        var (context, gameConfiguration) = await CreateSeason6ConfigurationAsync().ConfigureAwait(false);
        using var _ = context;
        var ownTable = context.CreateNew<WeightedValueTable>();
        gameConfiguration.WeightedValueTables.Add(ownTable);
        var socketItem = GetSocketItems(gameConfiguration).First();
        socketItem.SocketCountTable = ownTable;

        await new AddSocketCountTablesUpdatePlugIn().ApplyUpdateAsync(context, gameConfiguration).ConfigureAwait(false);

        Assert.That(socketItem.SocketCountTable, Is.SameAs(ownTable));
    }

    private static void AssertSocketCountTables(GameConfiguration gameConfiguration)
    {
        Assert.That(gameConfiguration.WeightedValueTables, Has.Count.EqualTo(2));
        var socketItems = GetSocketItems(gameConfiguration);
        Assert.That(socketItems, Is.Not.Empty);
        Assert.Multiple(() =>
        {
            foreach (var socketItem in socketItems)
            {
                var name = socketItem.Name.ValueInNeutralLanguage;
                Assert.That(socketItem.MaximumSockets, Is.EqualTo(MaximumSocketCount), name);
                Assert.That(socketItem.SocketCountTable, Is.Not.Null, name);
                var isTwoHanded = socketItem is { Group: 4, Number: 23 }; // Stinger Bow, the only two-handed socket item
                var weights = socketItem.SocketCountTable!.Values.ToDictionary(v => v.Value, v => v.Weight);
                Assert.That(weights.Keys, Is.EquivalentTo(new[] { 1, 2, 3, 4, 5 }), name);
                Assert.That(weights[1], Is.GreaterThan(weights[2]), name);
                Assert.That(weights[2], Is.GreaterThan(weights[3]), name);
                if (isTwoHanded)
                {
                    Assert.That(weights[5], Is.GreaterThan(0), name);
                }
                else
                {
                    Assert.That(weights[4], Is.Zero, name);
                    Assert.That(weights[5], Is.Zero, name);
                }
            }
        });
    }

    private static List<ItemDefinition> GetSocketItems(GameConfiguration gameConfiguration)
    {
        return gameConfiguration.Items.Where(item => item.MaximumSockets > 0).ToList();
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
