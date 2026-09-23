// <copyright file="DoppelgangerDataTest.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.Persistence.Initialization.Tests;

using Microsoft.Extensions.Logging.Abstractions;
using MUnique.OpenMU.DataModel.Configuration;
using MUnique.OpenMU.Persistence.Initialization.Updates;
using MUnique.OpenMU.Persistence.InMemory;

/// <summary>
/// Tests the data of the doppelganger event.
/// </summary>
[TestFixture]
internal class DoppelgangerDataTest
{
    private const short LugardNumber = 540;
    private const short ElvenlandNumber = 51;
    private static readonly short[] EventMapNumbers = [65, 66, 67, 68];

    /// <summary>
    /// Tests that a new season 6 database contains the doppelganger event.
    /// </summary>
    [Test]
    public async Task NewDatabaseContainsEventAsync()
    {
        var gameConfiguration = await CreateSeason6ConfigurationAsync().ConfigureAwait(false);

        AssertEventData(gameConfiguration);
    }

    /// <summary>
    /// Tests that the update adds the doppelganger event to a database which was created
    /// before it existed, and that applying it twice doesn't duplicate anything.
    /// </summary>
    [Test]
    public async Task UpdateAddsEventToExistingDatabaseAsync()
    {
        var contextProvider = new InMemoryPersistenceContextProvider();
        var dataInitialization = new VersionSeasonSix.DataInitialization(contextProvider, new NullLoggerFactory());
        await dataInitialization.CreateInitialDataAsync(1, true).ConfigureAwait(false);

        using var context = contextProvider.CreateNewContext();
        var gameConfiguration = (await context.GetAsync<GameConfiguration>().ConfigureAwait(false)).First();

        // Revert the database to the state before the doppelganger event existed.
        foreach (var definition in gameConfiguration.MiniGameDefinitions.Where(d => d.Type == MiniGameType.Doppelganger).ToList())
        {
            gameConfiguration.MiniGameDefinitions.Remove(definition);
        }

        gameConfiguration.Items.Remove(gameConfiguration.Items.Single(item => item is { Group: 14, Number: 111 }));
        gameConfiguration.Monsters.Single(monster => monster.Number == LugardNumber).NpcWindow = NpcWindow.Undefined;
        foreach (var map in gameConfiguration.Maps.Where(map => EventMapNumbers.Contains(map.Number)))
        {
            map.SafezoneMap = map;
        }

        var update = new AddDoppelgangerDataUpdatePlugIn();
        await update.ApplyUpdateAsync(context, gameConfiguration).ConfigureAwait(false);
        await update.ApplyUpdateAsync(context, gameConfiguration).ConfigureAwait(false);

        AssertEventData(gameConfiguration);
    }

    private static async Task<GameConfiguration> CreateSeason6ConfigurationAsync()
    {
        var contextProvider = new InMemoryPersistenceContextProvider();
        var dataInitialization = new VersionSeasonSix.DataInitialization(contextProvider, new NullLoggerFactory());
        await dataInitialization.CreateInitialDataAsync(1, true).ConfigureAwait(false);

        using var context = contextProvider.CreateNewContext();
        return (await context.GetAsync<GameConfiguration>().ConfigureAwait(false)).First();
    }

    private static void AssertEventData(GameConfiguration gameConfiguration)
    {
        var ticket = gameConfiguration.Items.Single(item => item is { Group: 14, Number: 111 });
        var definitions = gameConfiguration.MiniGameDefinitions.Where(d => d.Type == MiniGameType.Doppelganger).ToList();

        Assert.That(definitions.Select(d => (short)d.GameLevel), Is.EquivalentTo(new short[] { 1, 2, 3, 4 }));
        Assert.That(definitions.Select(d => d.Entrance?.Map?.Number), Is.EquivalentTo(EventMapNumbers.Select(n => (short?)n)));
        Assert.That(definitions, Has.All.Matches<MiniGameDefinition>(d => d.MapCreationPolicy == MiniGameMapCreationPolicy.OnePerParty));
        Assert.That(definitions, Has.All.Matches<MiniGameDefinition>(d => d.TicketItem == ticket));
        Assert.That(gameConfiguration.Monsters.Single(monster => monster.Number == LugardNumber).NpcWindow, Is.EqualTo(NpcWindow.LugardDoppelgangerEntry));
        Assert.That(
            gameConfiguration.Maps.Where(map => EventMapNumbers.Contains(map.Number)).Select(map => map.SafezoneMap?.Number),
            Has.All.EqualTo(ElvenlandNumber));
    }
}
