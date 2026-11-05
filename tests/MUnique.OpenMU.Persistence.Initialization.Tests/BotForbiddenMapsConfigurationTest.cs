// <copyright file="BotForbiddenMapsConfigurationTest.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.Persistence.Initialization.Tests;

using Microsoft.Extensions.Logging.Abstractions;
using MUnique.OpenMU.DataModel.Configuration;
using MUnique.OpenMU.GameLogic.Bots;
using MUnique.OpenMU.Persistence.InMemory;
using MUnique.OpenMU.PlugIns;

/// <summary>
/// Tests that the maps forbidden for bots (<see cref="BotConfiguration.MapsForbiddenForBots"/>) survive the
/// plugin configuration's way through the database, the way the admin panel and the bot plugin store it:
/// as references to the configured maps, resolved back to them when loaded.
/// </summary>
[TestFixture]
internal class BotForbiddenMapsConfigurationTest
{
    private const short ArenaNumber = 6;
    private const short CrywolfNumber = 34;

    /// <summary>
    /// Tests that the forbidden maps are stored as references and come back as the configured maps.
    /// </summary>
    /// <returns>The task.</returns>
    [Test]
    public async Task ForbiddenMapsAreStoredAsReferencesAsync()
    {
        var contextProvider = new InMemoryPersistenceContextProvider();
        await new VersionSeasonSix.DataInitialization(contextProvider, new NullLoggerFactory()).CreateInitialDataAsync(1, true).ConfigureAwait(false);
        using var context = contextProvider.CreateNewContext();
        var gameConfiguration = (await context.GetAsync<GameConfiguration>().ConfigureAwait(false)).First();
        var dataSource = new GameConfigurationDataSource(new NullLogger<GameConfigurationDataSource>(), contextProvider);
        await dataSource.GetOwnerAsync(gameConfiguration.GetId()).ConfigureAwait(false);
        var referenceHandler = new ByDataSourceReferenceHandler(dataSource);

        var arena = gameConfiguration.Maps.Single(m => m.Number == ArenaNumber);
        var crywolf = gameConfiguration.Maps.Single(m => m.Number == CrywolfNumber);
        var configuration = new BotConfiguration();
        configuration.MapsForbiddenForBots.Add(arena);
        configuration.MapsForbiddenForBots.Add(crywolf);

        var plugInConfiguration = context.CreateNew<PlugInConfiguration>();
        plugInConfiguration.SetConfiguration(configuration, referenceHandler);
        var loaded = plugInConfiguration.GetConfiguration<BotConfiguration>(referenceHandler);

        Assert.That(plugInConfiguration.CustomConfiguration, Does.Not.Contain("MonsterSpawns"), "a reference, not a copy of the map");
        Assert.That(loaded, Is.Not.Null);
        Assert.That(loaded!.MapsForbiddenForBots.Select(m => m.Number), Is.EquivalentTo(new[] { ArenaNumber, CrywolfNumber }));
        Assert.That(loaded.IsMapForbidden(arena), Is.True);
        Assert.That(loaded.IsMapForbidden(gameConfiguration.Maps.Single(m => m.Number == 0)), Is.False);
    }

    /// <summary>
    /// Tests that a new configuration forbids no map - bots may go everywhere unless the server says
    /// otherwise.
    /// </summary>
    [Test]
    public void NoMapIsForbiddenByDefault()
    {
        var configuration = new BotConfiguration();

        Assert.That(configuration.MapsForbiddenForBots, Is.Empty);
        Assert.That(configuration.IsMapForbidden(null), Is.False);
    }
}
