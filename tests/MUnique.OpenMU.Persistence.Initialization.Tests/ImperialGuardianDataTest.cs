// <copyright file="ImperialGuardianDataTest.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.Persistence.Initialization.Tests;

using Microsoft.Extensions.Logging.Abstractions;
using MUnique.OpenMU.DataModel.Configuration;
using MUnique.OpenMU.Persistence.Initialization.Updates;
using MUnique.OpenMU.Persistence.InMemory;

/// <summary>
/// Tests the data of the imperial guardian event.
/// </summary>
[TestFixture]
internal class ImperialGuardianDataTest
{
    private const short JerintNumber = 522;
    private const short DeviasNumber = 2;
    private static readonly short[] EventMapNumbers = [69, 70, 71, 72];

    /// <summary>
    /// The maps of the days (1 = monday, ..., 7 = sunday) with the number of their zones.
    /// </summary>
    private static readonly (byte Day, short MapNumber, int ZoneCount)[] Days =
    [
        (1, 69, 3), (2, 70, 3), (3, 71, 3), (4, 69, 3), (5, 70, 3), (6, 71, 3), (7, 72, 4),
    ];

    /// <summary>
    /// Tests that a new season 6 database contains the imperial guardian event.
    /// </summary>
    [Test]
    public async Task NewDatabaseContainsEventAsync()
    {
        var contextProvider = new InMemoryPersistenceContextProvider();
        var gameConfiguration = await CreateConfigurationAsync(contextProvider).ConfigureAwait(false);

        AssertEventData(gameConfiguration);
    }

    /// <summary>
    /// Tests that the update adds the imperial guardian event to a database which was created
    /// before it existed, and that applying it twice doesn't duplicate anything.
    /// </summary>
    [Test]
    public async Task UpdateAddsEventToExistingDatabaseAsync()
    {
        var contextProvider = new InMemoryPersistenceContextProvider();
        var gameConfiguration = await CreateConfigurationAsync(contextProvider).ConfigureAwait(false);

        // Revert the database to the state before the event existed.
        foreach (var definition in gameConfiguration.MiniGameDefinitions.Where(d => d.Type == MiniGameType.ImperialGuardian).ToList())
        {
            gameConfiguration.MiniGameDefinitions.Remove(definition);
        }

        gameConfiguration.Monsters.Single(monster => monster.Number == JerintNumber).NpcWindow = NpcWindow.Undefined;
        var scrapDropGroup = gameConfiguration.DropItemGroups.Single(group => group.PossibleItems.Any(item => item is { Group: 14, Number: 101 }));
        gameConfiguration.DropItemGroups.Remove(scrapDropGroup);
        foreach (var map in gameConfiguration.Maps)
        {
            map.DropItemGroups.Remove(scrapDropGroup);
        }

        foreach (var map in gameConfiguration.Maps.Where(map => EventMapNumbers.Contains(map.Number)))
        {
            map.SafezoneMap = map;
            foreach (var spawn in map.MonsterSpawns)
            {
                spawn.SpawnTrigger = SpawnTrigger.OnceAtEventStart;
                spawn.WaveNumber = 0;
            }
        }

        var update = new AddImperialGuardianDataUpdatePlugIn();
        await update.ApplyUpdateAsync(contextProvider.CreateNewContext(), gameConfiguration).ConfigureAwait(false);
        await update.ApplyUpdateAsync(contextProvider.CreateNewContext(), gameConfiguration).ConfigureAwait(false);

        AssertEventData(gameConfiguration);
    }

    private static async Task<GameConfiguration> CreateConfigurationAsync(InMemoryPersistenceContextProvider contextProvider)
    {
        var dataInitialization = new VersionSeasonSix.DataInitialization(contextProvider, new NullLoggerFactory());
        await dataInitialization.CreateInitialDataAsync(1, true).ConfigureAwait(false);
        using var context = contextProvider.CreateNewContext();
        return (await context.GetAsync<GameConfiguration>().ConfigureAwait(false)).First();
    }

    private static void AssertEventData(GameConfiguration gameConfiguration)
    {
        var definitions = gameConfiguration.MiniGameDefinitions.Where(d => d.Type == MiniGameType.ImperialGuardian).ToList();
        Assert.That(definitions, Has.Count.EqualTo(Days.Length));

        foreach (var (day, mapNumber, zoneCount) in Days)
        {
            var definition = definitions.Single(d => d.GameLevel == day);
            Assert.That(definition.Entrance?.Map?.Number, Is.EqualTo(mapNumber), $"Map of day {day}");
            Assert.That(definition.TicketItem, Is.Null, "The ticket is checked by the enter action.");
            Assert.That(definition.MapCreationPolicy, Is.EqualTo(MiniGameMapCreationPolicy.OnePerParty));

            var map = gameConfiguration.Maps.Single(m => m.Number == mapNumber);
            var waves = map.MonsterSpawns.Where(spawn => spawn.WaveNumber / 10 == day).Select(spawn => spawn.WaveNumber % 10).Distinct().ToList();
            Assert.That(waves, Is.EquivalentTo(Enumerable.Range(0, zoneCount)), $"Zones of day {day}");
            foreach (var zone in waves)
            {
                var zoneSpawns = map.MonsterSpawns.Where(spawn => spawn.WaveNumber == (day * 10) + zone).ToList();
                Assert.That(zoneSpawns.Any(spawn => spawn.MonsterDefinition!.Number is >= 504 and <= 521), Is.True, $"Monsters of day {day}, zone {zone}");
                Assert.That(zoneSpawns.Any(spawn => spawn.MonsterDefinition!.Number is 525 or 528), Is.True, $"Gate of day {day}, zone {zone}");
            }
        }

        foreach (var map in gameConfiguration.Maps.Where(map => EventMapNumbers.Contains(map.Number)))
        {
            Assert.That(map.MonsterSpawns.Select(spawn => spawn.SpawnTrigger), Is.All.EqualTo(SpawnTrigger.OnceAtWaveStart), $"Spawns of map {map.Number}");
            Assert.That(map.SafezoneMap?.Number, Is.EqualTo(DeviasNumber), $"Safezone of map {map.Number}");
        }

        Assert.That(gameConfiguration.Monsters.Single(monster => monster.Number == JerintNumber).NpcWindow, Is.EqualTo(NpcWindow.JerintGaionEvententry));

        var scrapDropGroups = gameConfiguration.DropItemGroups.Where(group => group.PossibleItems.Any(item => item is { Group: 14, Number: 101 })).ToList();
        Assert.That(scrapDropGroups, Has.Count.EqualTo(1));
        Assert.That(gameConfiguration.Maps.Where(map => map.DropItemGroups.Contains(scrapDropGroups[0])), Is.Not.Empty);
    }
}
