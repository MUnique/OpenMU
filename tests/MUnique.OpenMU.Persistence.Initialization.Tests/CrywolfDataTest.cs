// <copyright file="CrywolfDataTest.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.Persistence.Initialization.Tests;

using Microsoft.Extensions.Logging.Abstractions;
using MUnique.OpenMU.DataModel.Configuration;
using MUnique.OpenMU.GameLogic;
using MUnique.OpenMU.GameLogic.Crywolf;
using MUnique.OpenMU.Persistence.Initialization.Updates;
using MUnique.OpenMU.Persistence.InMemory;

/// <summary>
/// Tests the data of the crywolf event.
/// </summary>
[TestFixture]
internal class CrywolfDataTest
{
    private const short CrywolfNumber = 34;

    /// <summary>
    /// Tests that a new database contains the monsters of the army and their spawns,
    /// and that the statue and the altars are always on the map.
    /// </summary>
    [Test]
    public async Task NewDatabaseContainsEventDataAsync()
    {
        var contextProvider = new InMemoryPersistenceContextProvider();
        var gameConfiguration = await CreateConfigurationAsync(contextProvider).ConfigureAwait(false);

        AssertEventData(gameConfiguration);
    }

    /// <summary>
    /// Tests that the update adds the event data to an existing database, and that applying it twice doesn't change anything.
    /// </summary>
    [Test]
    public async Task UpdateAddsEventDataAsync()
    {
        var contextProvider = new InMemoryPersistenceContextProvider();
        var gameConfiguration = await CreateConfigurationAsync(contextProvider).ConfigureAwait(false);
        var map = GetMap(gameConfiguration);
        foreach (var spawn in map.MonsterSpawns.Where(spawn => spawn.SpawnTrigger == SpawnTrigger.OnceAtWaveStart).ToList())
        {
            map.MonsterSpawns.Remove(spawn);
        }

        foreach (var spawn in map.MonsterSpawns.Where(spawn => spawn.MonsterDefinition!.Number is >= 204 and <= 209))
        {
            spawn.SpawnTrigger = SpawnTrigger.OnceAtEventStart;
            spawn.MonsterDefinition!.ObjectKind = NpcObjectKind.Monster;
        }

        gameConfiguration.WarpList.Remove(gameConfiguration.WarpList.Single(warp => warp.Index == 26));
        map.TerrainVariants.Clear();

        var update = new AddCrywolfEventUpdatePlugIn();
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

    private static GameMapDefinition GetMap(GameConfiguration gameConfiguration)
    {
        return gameConfiguration.Maps.Single(map => map.Number == CrywolfNumber);
    }

    private static void AssertEventData(GameConfiguration gameConfiguration)
    {
        var definition = new CrywolfEventDefinition();
        var map = GetMap(gameConfiguration);
        foreach (var number in new short[] { 340, 341, 344, 345, 348, 349 })
        {
            Assert.That(gameConfiguration.Monsters.Where(monster => monster.Number == number).ToList(), Has.Count.EqualTo(1), $"Monster {number}");
        }

        var eventSpawns = map.MonsterSpawns.Where(spawn => spawn.SpawnTrigger == SpawnTrigger.OnceAtWaveStart).ToList();
        Assert.That(eventSpawns, Has.Count.EqualTo(76));

        foreach (var group in definition.MonsterGroups)
        {
            var groupSpawns = eventSpawns.Where(spawn => spawn.WaveNumber == group.WaveNumber).ToList();
            Assert.That(groupSpawns.Count(spawn => spawn.MonsterDefinition!.Number == definition.DarkElfNumber), Is.EqualTo(1), $"The leader of the group {group.WaveNumber}");
        }

        var balgass = eventSpawns.Single(spawn => spawn.WaveNumber == definition.BalgassWaveNumber);
        Assert.That(balgass.MonsterDefinition!.Number, Is.EqualTo(349));

        foreach (var ballista in definition.Ballistas)
        {
            Assert.That(eventSpawns.Count(spawn => spawn.X1 == ballista.X && spawn.Y1 == ballista.Y && spawn.MonsterDefinition!.Number == 348), Is.EqualTo(1), $"The ballista at {ballista}");
        }

        var statueAndAltars = map.MonsterSpawns.Where(spawn => spawn.MonsterDefinition!.Number is >= 204 and <= 209).ToList();
        Assert.That(statueAndAltars, Has.Count.EqualTo(6));
        Assert.That(statueAndAltars.Select(spawn => spawn.SpawnTrigger), Is.All.EqualTo(SpawnTrigger.Automatic));
        Assert.That(statueAndAltars.Select(spawn => spawn.MonsterDefinition!.ObjectKind), Is.All.EqualTo(NpcObjectKind.PassiveNpc));

        var warp = gameConfiguration.WarpList.Single(warp => warp.Index == 26);
        Assert.That(warp.Gate?.Map, Is.SameAs(map));
        Assert.That(warp.LevelRequirement, Is.EqualTo(190));

        Assert.That(map.TerrainVariants.Select(variant => variant.Number), Is.EquivalentTo(new short[] { 1, 2 }));
        AssertTerrainSwitch(map);
    }

    /// <summary>
    /// Asserts that switching to the terrain of an occupation state and back results in exactly the terrain of the respective file.
    /// </summary>
    private static void AssertTerrainSwitch(GameMapDefinition map)
    {
        var terrain = new GameMapTerrain(map.TerrainData);
        foreach (var variant in map.TerrainVariants)
        {
            Assert.That(variant.TerrainData, Is.Not.Null.And.Not.EqualTo(map.TerrainData), $"Terrain variant {variant}");
            terrain.LoadTerrainData(variant.TerrainData);
            Assert.That(terrain.AttributeMap, Is.EqualTo(new GameMapTerrain(variant.TerrainData).AttributeMap), $"Terrain variant {variant}");
            terrain.LoadTerrainData(map.TerrainData);
            Assert.That(terrain.AttributeMap, Is.EqualTo(new GameMapTerrain(map.TerrainData).AttributeMap), $"Normal terrain after {variant}");
        }
    }
}
