// <copyright file="FieldMonsterSpawnsUpdateTest.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.Persistence.Initialization.Tests;

using MUnique.OpenMU.DataModel.Configuration;
using MUnique.OpenMU.Persistence.Initialization.Updates;

/// <summary>
/// Tests that the monster spawns of Dungeon and Lost Tower are defined only once, and the
/// <see cref="FieldMonsterSpawnsUpdatePlugInBase"/> variants which remove the duplicates from existing databases.
/// </summary>
[TestFixture]
internal class FieldMonsterSpawnsUpdateTest
{
    private const short DeathKnightNumber = 40;

    private const short ShadowNumber = 36;

    private const short MeteoriteTrapNumber = 103;

    /// <summary>
    /// Tests that new databases of all versions contain each monster spawn of Dungeon and Lost Tower only once.
    /// </summary>
    /// <param name="version">The game version.</param>
    [TestCase("0.75")]
    [TestCase("0.95d")]
    [TestCase("Season 6")]
    public async Task NewDatabaseHasNoDuplicateSpawnsAsync(string version)
    {
        var (context, gameConfiguration) = await CreateAsync(version).ConfigureAwait(false);
        using var _ = context;

        AssertNoDuplicateSpawns(gameConfiguration);
    }

    /// <summary>
    /// Tests that the update of each version removes the duplicate spawns of Dungeon and Lost Tower from an
    /// existing database, keeps the traps, and that applying it twice doesn't change anything.
    /// </summary>
    /// <param name="version">The game version.</param>
    [TestCase("0.75")]
    [TestCase("0.95d")]
    [TestCase("Season 6")]
    public async Task UpdateRemovesDuplicateSpawnsAsync(string version)
    {
        var (context, gameConfiguration) = await CreateAsync(version).ConfigureAwait(false);
        using var _ = context;
        var dungeon = gameConfiguration.Maps.Single(m => m.Number == Version075.Maps.Dungeon.Number);
        var lostTower = gameConfiguration.Maps.Single(m => m.Number == Version075.Maps.LostTower.Number);

        // The state of a database which was initialized before: spawns 361 to 616 repeat 101 to 359.
        var originals = dungeon.MonsterSpawns.Where(s => s.MonsterDefinition?.ObjectKind == NpcObjectKind.Monster).ToList();
        var duplicateNumbers = Enumerable.Range(361, 256).Where(n => n is not 400 and not 500).Select(n => (short)n);
        foreach (var (original, number) in originals.Zip(duplicateNumbers))
        {
            AddSpawn(context, gameConfiguration, dungeon, number, original.MonsterDefinition!.Number, original.X1, original.X2, original.Y1, original.Y2, original.Quantity);
        }

        AddSpawn(context, gameConfiguration, lostTower, 338, DeathKnightNumber, 6, 6, 98, 98, 1);
        AddSpawn(context, gameConfiguration, lostTower, 536, ShadowNumber, 195, 195, 40, 40, 1);
        var lostTowerCount = lostTower.MonsterSpawns.Count;
        var trapCount = lostTower.MonsterSpawns.Count(s => s.MonsterDefinition?.Number == MeteoriteTrapNumber);

        FieldMonsterSpawnsUpdatePlugInBase update = version switch
        {
            "0.75" => new FieldMonsterSpawnsUpdatePlugIn075(),
            "0.95d" => new FieldMonsterSpawnsUpdatePlugIn095D(),
            _ => new FieldMonsterSpawnsUpdatePlugInSeason6(),
        };
        for (var i = 0; i < 2; i++)
        {
            await update.ApplyUpdateAsync(context, gameConfiguration).ConfigureAwait(false);
        }

        AssertNoDuplicateSpawns(gameConfiguration);
        Assert.That(dungeon.MonsterSpawns.Where(s => s.MonsterDefinition?.ObjectKind == NpcObjectKind.Monster), Is.EquivalentTo(originals));
        Assert.That(lostTower.MonsterSpawns, Has.Count.EqualTo(lostTowerCount - 2));
        Assert.That(lostTower.MonsterSpawns.Count(s => s.MonsterDefinition?.Number == MeteoriteTrapNumber), Is.EqualTo(trapCount).And.GreaterThan(0));
    }

    private static Task<(IContext Context, GameConfiguration GameConfiguration)> CreateAsync(string version)
    {
        return version switch
        {
            "0.75" => TestGameConfiguration.Create075Async(),
            "0.95d" => TestGameConfiguration.Create095dAsync(),
            _ => TestGameConfiguration.CreateSeason6Async(),
        };
    }

    private static void AssertNoDuplicateSpawns(GameConfiguration gameConfiguration)
    {
        foreach (var mapNumber in new[] { Version075.Maps.Dungeon.Number, Version075.Maps.LostTower.Number })
        {
            var duplicates = gameConfiguration.Maps.Single(m => m.Number == mapNumber).MonsterSpawns
                .Where(s => s.MonsterDefinition?.ObjectKind == NpcObjectKind.Monster)
                .GroupBy(s => (s.MonsterDefinition!.Number, s.X1, s.X2, s.Y1, s.Y2))
                .Where(group => group.Count() > 1)
                .Select(group => group.Key);
            Assert.That(duplicates, Is.Empty, $"map {mapNumber}");
        }
    }

    private static MonsterSpawnArea AddSpawn(IContext context, GameConfiguration gameConfiguration, GameMapDefinition map, short number, short monsterNumber, byte x1, byte x2, byte y1, byte y2, short quantity)
    {
        var area = context.CreateNew<MonsterSpawnArea>();
        area.SetGuid(map.Number, number);
        area.GameMap = map;
        area.MonsterDefinition = gameConfiguration.Monsters.Single(m => m.Number == monsterNumber);
        area.SpawnTrigger = SpawnTrigger.Automatic;
        area.X1 = x1;
        area.X2 = x2;
        area.Y1 = y1;
        area.Y2 = y2;
        area.Quantity = quantity;
        map.MonsterSpawns.Add(area);
        return area;
    }
}
