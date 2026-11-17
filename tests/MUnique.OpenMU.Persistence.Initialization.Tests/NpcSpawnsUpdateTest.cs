// <copyright file="NpcSpawnsUpdateTest.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.Persistence.Initialization.Tests;

using MUnique.OpenMU.DataModel.Configuration;
using MUnique.OpenMU.Persistence.Initialization.Updates;
using MUnique.OpenMU.Persistence.Initialization.VersionSeasonSix.Maps;

/// <summary>
/// Tests the Season 6 NPC spawns - positions, looking directions, added and removed NPCs - and the
/// <see cref="NpcSpawnsUpdatePlugInSeason6"/>, which brings them to existing databases.
/// </summary>
[TestFixture]
internal class NpcSpawnsUpdateTest
{
    /// <summary>
    /// The NPC spawns of 0.75 and 0.95d which Season 6 doesn't have, as an older Season 6 database still has them.
    /// </summary>
    private static readonly (byte MapNumber, short SpawnNumber, short NpcNumber, byte X, byte Y, Direction Direction)[] OldSpawns =
    [
        (Lorencia.Number, 9, 247, 114, 125, Direction.SouthEast),
        (Lorencia.Number, 15, 236, 175, 120, Direction.SouthEast),
        (Noria.Number, 1, 253, 169, 109, Direction.SouthEast),
        (Noria.Number, 2, 253, 193, 110, Direction.South),
        (LostTower.Number, 1, 253, 207, 76, Direction.SouthWest),
        (LostTower.Number, 2, 240, 201, 76, Direction.SouthEast),
        (Arena.Number, 1, 240, 58, 140, Direction.SouthWest),
    ];

    /// <summary>
    /// Tests that a new Season 6 database contains the NPCs at their Season 6 positions and directions,
    /// without the NPCs of 0.75 and 0.95d which Season 6 doesn't have.
    /// </summary>
    [Test]
    public async Task NewDatabaseContainsSeasonSixNpcSpawnsAsync()
    {
        var (context, gameConfiguration) = await TestGameConfiguration.CreateSeason6Async().ConfigureAwait(false);
        using var _ = context;

        foreach (var correction in NpcSpawnsUpdatePlugInSeason6.Corrections)
        {
            var spawn = FindSpawn(gameConfiguration, correction.MapNumber, correction.SpawnNumber);
            Assert.That(spawn, Is.Not.Null, $"map {correction.MapNumber}, spawn {correction.SpawnNumber}");
            Assert.That(
                (spawn!.MonsterDefinition!.Number, spawn.X1, spawn.X2, spawn.Y1, spawn.Y2, spawn.Direction),
                Is.EqualTo((correction.NpcNumber, correction.X, correction.X, correction.Y, correction.Y, correction.Direction)),
                $"map {correction.MapNumber}, spawn {correction.SpawnNumber}");
        }

        foreach (var (mapNumber, spawnNumber, _) in NpcSpawnsUpdatePlugInSeason6.RemovedSpawns)
        {
            Assert.That(FindSpawn(gameConfiguration, mapNumber, spawnNumber), Is.Null, $"map {mapNumber}, spawn {spawnNumber}");
        }
    }

    /// <summary>
    /// Tests that the update brings the NPC spawns of an older database to the state of a new database,
    /// and that applying it a second time doesn't change anything.
    /// </summary>
    [Test]
    public async Task UpdateBringsExistingDatabaseToSeasonSixDataAsync()
    {
        var (context, gameConfiguration) = await TestGameConfiguration.CreateSeason6Async().ConfigureAwait(false);
        using var _ = context;
        var expected = Snapshot(gameConfiguration);

        // The state of a database which was initialized before: other positions and directions,
        // without the added NPCs and with the NPCs of 0.75 and 0.95d.
        foreach (var correction in NpcSpawnsUpdatePlugInSeason6.Corrections)
        {
            var spawn = FindSpawn(gameConfiguration, correction.MapNumber, correction.SpawnNumber)!;
            if (correction.IsAdded)
            {
                spawn.GameMap!.MonsterSpawns.Remove(spawn);
                continue;
            }

            spawn.X1 = spawn.X2 = 1;
            spawn.Y1 = spawn.Y2 = 1;
            spawn.Direction = Direction.Undefined;
        }

        foreach (var (mapNumber, spawnNumber, npcNumber, x, y, direction) in OldSpawns)
        {
            AddSpawn(context, gameConfiguration, mapNumber, spawnNumber, npcNumber, x, y, direction);
        }

        Assert.That(Snapshot(gameConfiguration), Is.Not.EquivalentTo(expected));

        var update = new NpcSpawnsUpdatePlugInSeason6();
        await update.ApplyUpdateAsync(context, gameConfiguration).ConfigureAwait(false);
        Assert.That(Snapshot(gameConfiguration), Is.EquivalentTo(expected));

        await update.ApplyUpdateAsync(context, gameConfiguration).ConfigureAwait(false);
        Assert.That(Snapshot(gameConfiguration), Is.EquivalentTo(expected));
    }

    /// <summary>
    /// Tests that the update keeps spawns which the server owner reused for another NPC.
    /// </summary>
    [Test]
    public async Task UpdateKeepsSpawnsWithAnotherNpcAsync()
    {
        var (context, gameConfiguration) = await TestGameConfiguration.CreateSeason6Async().ConfigureAwait(false);
        using var _ = context;
        const short otherNpcNumber = 251;

        // A spawn number of a removed NPC and one of a corrected NPC, both used for another NPC.
        var reused = AddSpawn(context, gameConfiguration, Noria.Number, 1, otherNpcNumber, 170, 110, Direction.South);
        var correction = NpcSpawnsUpdatePlugInSeason6.Corrections.First(c => !c.IsAdded);
        var changed = FindSpawn(gameConfiguration, correction.MapNumber, correction.SpawnNumber)!;
        changed.MonsterDefinition = gameConfiguration.Monsters.Single(m => m.Number == otherNpcNumber);
        changed.X1 = changed.X2 = 1;

        await new NpcSpawnsUpdatePlugInSeason6().ApplyUpdateAsync(context, gameConfiguration).ConfigureAwait(false);

        Assert.That(FindSpawn(gameConfiguration, Noria.Number, 1), Is.SameAs(reused));
        Assert.That((reused.X1, reused.Y1, reused.Direction), Is.EqualTo(((byte)170, (byte)110, Direction.South)));
        Assert.That(changed.X1, Is.EqualTo(1));
    }

    /// <summary>
    /// Gets the NPC spawns of the maps which the update changes, to compare two states of the configuration.
    /// </summary>
    private static List<(Guid Id, short Npc, byte X1, byte X2, byte Y1, byte Y2, Direction Direction, SpawnTrigger Trigger, short Quantity)> Snapshot(GameConfiguration gameConfiguration)
    {
        var mapNumbers = NpcSpawnsUpdatePlugInSeason6.Corrections.Select(c => (short)c.MapNumber)
            .Concat(NpcSpawnsUpdatePlugInSeason6.RemovedSpawns.Select(r => (short)r.MapNumber))
            .ToHashSet();
        return gameConfiguration.Maps
            .Where(m => mapNumbers.Contains(m.Number))
            .SelectMany(m => m.MonsterSpawns)
            .Where(s => s.MonsterDefinition!.ObjectKind != NpcObjectKind.Monster)
            .Select(s => (s.GetId(), s.MonsterDefinition!.Number, s.X1, s.X2, s.Y1, s.Y2, s.Direction, s.SpawnTrigger, s.Quantity))
            .ToList();
    }

    private static MonsterSpawnArea? FindSpawn(GameConfiguration gameConfiguration, byte mapNumber, short spawnNumber)
    {
        var id = GuidHelper.CreateGuid<MonsterSpawnArea>(mapNumber, spawnNumber);
        return gameConfiguration.Maps.Single(m => m.Number == mapNumber).MonsterSpawns.FirstOrDefault(s => s.GetId() == id);
    }

    private static MonsterSpawnArea AddSpawn(IContext context, GameConfiguration gameConfiguration, byte mapNumber, short spawnNumber, short npcNumber, byte x, byte y, Direction direction)
    {
        var map = gameConfiguration.Maps.Single(m => m.Number == mapNumber);
        var area = context.CreateNew<MonsterSpawnArea>();
        area.SetGuid(mapNumber, spawnNumber);
        area.GameMap = map;
        area.MonsterDefinition = gameConfiguration.Monsters.Single(m => m.Number == npcNumber);
        area.SpawnTrigger = SpawnTrigger.Automatic;
        area.Quantity = 1;
        area.X1 = area.X2 = x;
        area.Y1 = area.Y2 = y;
        area.Direction = direction;
        map.MonsterSpawns.Add(area);
        return area;
    }
}
