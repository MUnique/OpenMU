// <copyright file="OldVersionNpcSpawnsUpdateTest.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.Persistence.Initialization.Tests;

using MUnique.OpenMU.DataModel.Configuration;
using MUnique.OpenMU.Persistence.Initialization.Updates;

/// <summary>
/// Tests the NPC spawns of 0.75 and 0.95d and the <see cref="NpcSpawnsUpdatePlugIn075"/> and
/// <see cref="NpcSpawnsUpdatePlugIn095D"/>, which bring them to existing databases.
/// </summary>
[TestFixture]
internal class OldVersionNpcSpawnsUpdateTest
{
    /// <summary>
    /// The NPC spawns which an older 0.75 or 0.95d database still has.
    /// </summary>
    private static readonly (byte MapNumber, short SpawnNumber, short NpcNumber, byte X, byte Y, Direction Direction)[] OldSpawns075 =
    [
        (0, 9, 247, 114, 125, Direction.SouthEast),
        (3, 1, 253, 169, 109, Direction.SouthEast),
        (3, 2, 253, 193, 110, Direction.South),
        (4, 1, 253, 207, 76, Direction.SouthWest),
        (4, 2, 240, 201, 76, Direction.SouthEast),
        (6, 1, 240, 58, 140, Direction.SouthWest),
        (7, 1, 240, 23, 17, Direction.SouthWest),
    ];

    /// <summary>
    /// The Golden Archer of Lorencia, which an older 0.95d database still has.
    /// </summary>
    private static readonly (byte MapNumber, short SpawnNumber, short NpcNumber, byte X, byte Y, Direction Direction) OldGoldenArcher095d =
        (0, 15, 236, 175, 120, Direction.SouthEast);

    /// <summary>
    /// Tests that the update brings the NPC spawns of an older 0.75 database to the state of a new database,
    /// and that applying it a second time doesn't change anything.
    /// </summary>
    [Test]
    public async Task UpdateBringsExisting075DatabaseToNewDataAsync()
    {
        var (context, gameConfiguration) = await TestGameConfiguration.Create075Async().ConfigureAwait(false);
        using var _ = context;
        await AssertUpdateAsync(context, gameConfiguration, new NpcSpawnsUpdatePlugIn075(), OldSpawns075).ConfigureAwait(false);
    }

    /// <summary>
    /// Tests that the update brings the NPC spawns of an older 0.95d database to the state of a new database,
    /// and that applying it a second time doesn't change anything.
    /// </summary>
    [Test]
    public async Task UpdateBringsExisting095dDatabaseToNewDataAsync()
    {
        var (context, gameConfiguration) = await TestGameConfiguration.Create095dAsync().ConfigureAwait(false);
        using var _ = context;
        await AssertUpdateAsync(context, gameConfiguration, new NpcSpawnsUpdatePlugIn095D(), [.. OldSpawns075, OldGoldenArcher095d]).ConfigureAwait(false);
    }

    /// <summary>
    /// Tests that Season 6, which takes the Golden Archer of Devias from 0.95d, has it exactly once.
    /// </summary>
    [Test]
    public async Task SeasonSixHasOneGoldenArcherInDeviasAsync()
    {
        var (context, gameConfiguration) = await TestGameConfiguration.CreateSeason6Async().ConfigureAwait(false);
        using var _ = context;

        var archers = gameConfiguration.Maps.Where(m => m.Number is 0 or 2)
            .SelectMany(m => m.MonsterSpawns)
            .Where(s => s.MonsterDefinition?.Number == 236)
            .Select(s => (s.GameMap!.Number, s.X1, s.Y1, s.Direction))
            .ToList();
        Assert.That(archers, Is.EqualTo(new[] { ((short)2, (byte)226, (byte)66, Direction.SouthWest) }));
    }

    private static async Task AssertUpdateAsync(
        IContext context,
        GameConfiguration gameConfiguration,
        NpcSpawnsUpdatePlugInBase update,
        (byte MapNumber, short SpawnNumber, short NpcNumber, byte X, byte Y, Direction Direction)[] oldSpawns)
    {
        foreach (var (mapNumber, spawnNumber, _) in update.RemovedSpawns)
        {
            Assert.That(FindSpawn(gameConfiguration, mapNumber, spawnNumber), Is.Null, $"map {mapNumber}, spawn {spawnNumber}");
        }

        var expected = Snapshot(gameConfiguration);

        // The state of a database which was initialized before: other positions and directions,
        // without the added NPCs and with the removed ones.
        foreach (var correction in update.Corrections)
        {
            var spawn = FindSpawn(gameConfiguration, correction.MapNumber, correction.SpawnNumber)!;
            Assert.That(
                (spawn.MonsterDefinition!.Number, spawn.X1, spawn.X2, spawn.Y1, spawn.Y2, spawn.Direction),
                Is.EqualTo((correction.NpcNumber, correction.X, correction.X, correction.Y, correction.Y, correction.Direction)),
                $"map {correction.MapNumber}, spawn {correction.SpawnNumber}");
            if (correction.IsAdded)
            {
                spawn.GameMap!.MonsterSpawns.Remove(spawn);
                continue;
            }

            spawn.X1 = spawn.X2 = 1;
            spawn.Y1 = spawn.Y2 = 1;
            spawn.Direction = Direction.Undefined;
        }

        foreach (var (mapNumber, spawnNumber, npcNumber, x, y, direction) in oldSpawns)
        {
            AddSpawn(context, gameConfiguration, mapNumber, spawnNumber, npcNumber, x, y, direction);
        }

        Assert.That(Snapshot(gameConfiguration), Is.Not.EquivalentTo(expected));

        await update.ApplyUpdateAsync(context, gameConfiguration).ConfigureAwait(false);
        Assert.That(Snapshot(gameConfiguration), Is.EquivalentTo(expected));

        await update.ApplyUpdateAsync(context, gameConfiguration).ConfigureAwait(false);
        Assert.That(Snapshot(gameConfiguration), Is.EquivalentTo(expected));
    }

    private static List<(Guid Id, short Npc, byte X1, byte X2, byte Y1, byte Y2, Direction Direction, SpawnTrigger Trigger, short Quantity)> Snapshot(GameConfiguration gameConfiguration)
    {
        return gameConfiguration.Maps
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

    private static void AddSpawn(IContext context, GameConfiguration gameConfiguration, byte mapNumber, short spawnNumber, short npcNumber, byte x, byte y, Direction direction)
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
    }
}
