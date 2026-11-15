// <copyright file="FieldMonsterSpawnsUpdateTest.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.Persistence.Initialization.Tests;

using MUnique.OpenMU.DataModel.Configuration;
using MUnique.OpenMU.Persistence.Initialization.Updates;
using MUnique.OpenMU.Persistence.Initialization.VersionSeasonSix.Maps;

/// <summary>
/// Tests the Season 6 monster spawn layout of Devias, Noria and Elvenland, and the
/// <see cref="FieldMonsterSpawnsUpdatePlugInBase"/> variants, which also remove the duplicate spawns of Dungeon and Lost Tower.
/// </summary>
[TestFixture]
internal class FieldMonsterSpawnsUpdateTest
{
    private const short EliteYetiNumber = 20;

    private const short GoblinNumber = 26;

    private const short GolemNumber = 7;

    private const short MarlonNumber = 229;

    private const short DeathKnightNumber = 40;

    private const short ShadowNumber = 36;

    private const short MeteoriteTrapNumber = 103;

    private static readonly (byte MapNumber, IReadOnlyList<FieldMonsterSpawn> Layout)[] Layouts =
    [
        (Devias.Number, Devias.MonsterSpawnLayout),
        (Noria.Number, Noria.MonsterSpawnLayout),
        (Elvenland.Number, Elvenland.MonsterSpawnLayout),
    ];

    /// <summary>
    /// Tests that a new Season 6 database contains the layout and none of the 0.75 spawns of these maps.
    /// </summary>
    [Test]
    public async Task NewDatabaseContainsLayoutAsync()
    {
        var (context, gameConfiguration) = await TestGameConfiguration.CreateSeason6Async().ConfigureAwait(false);
        using var _ = context;

        AssertLayouts(gameConfiguration);
    }

    /// <summary>
    /// Tests that the update replaces the 0.75 spawns of an existing database with the layout, keeps
    /// the spawns of other monsters and NPCs, and that applying it twice doesn't change anything.
    /// </summary>
    [Test]
    public async Task UpdateReplacesSpawnsOfExistingDatabaseAsync()
    {
        var (context, gameConfiguration) = await TestGameConfiguration.CreateSeason6Async().ConfigureAwait(false);
        using var _ = context;
        var devias = gameConfiguration.Maps.Single(m => m.Number == Devias.Number);
        var noria = gameConfiguration.Maps.Single(m => m.Number == Noria.Number);
        RemoveLayout(noria, Noria.MonsterSpawnLayout);
        RemoveLayout(devias, Devias.MonsterSpawnLayout);

        // The state of a database which was initialized before: monsters spread over quarters of the map.
        var oldEliteYeti = AddSpawn(context, gameConfiguration, devias, 103, EliteYetiNumber, 0, 128, 128, 245, 200);
        var oldGoblin = AddSpawn(context, gameConfiguration, noria, 21, GoblinNumber, 128, 251, 0, 128, 155);
        var customGolem = AddSpawn(context, gameConfiguration, noria, 999, GolemNumber, 10, 20, 10, 20, 5);
        var npcCount = devias.MonsterSpawns.Count(s => s.MonsterDefinition?.Number == MarlonNumber);

        var update = new FieldMonsterSpawnsUpdatePlugInSeason6();
        await update.ApplyUpdateAsync(context, gameConfiguration).ConfigureAwait(false);
        var spawnCounts = gameConfiguration.Maps.ToDictionary(m => m.GetId(), m => m.MonsterSpawns.Count);
        await update.ApplyUpdateAsync(context, gameConfiguration).ConfigureAwait(false);

        AssertLayouts(gameConfiguration);
        Assert.That(gameConfiguration.Maps.ToDictionary(m => m.GetId(), m => m.MonsterSpawns.Count), Is.EqualTo(spawnCounts));
        Assert.That(devias.MonsterSpawns, Does.Not.Contain(oldEliteYeti));
        Assert.That(noria.MonsterSpawns, Does.Not.Contain(oldGoblin));
        Assert.That(noria.MonsterSpawns, Does.Contain(customGolem));
        Assert.That(devias.MonsterSpawns.Count(s => s.MonsterDefinition?.Number == MarlonNumber), Is.EqualTo(npcCount).And.GreaterThan(0));
    }

    /// <summary>
    /// Tests that the update restores the values of a layout spawn which was changed on an existing database.
    /// </summary>
    [Test]
    public async Task UpdateRestoresChangedLayoutSpawnAsync()
    {
        var (context, gameConfiguration) = await TestGameConfiguration.CreateSeason6Async().ConfigureAwait(false);
        using var _ = context;
        var elvenland = gameConfiguration.Maps.Single(m => m.Number == Elvenland.Number);
        var changed = elvenland.MonsterSpawns.Single(s => s.GetId() == GuidHelper.CreateGuid<MonsterSpawnArea>(Elvenland.Number, Elvenland.MonsterSpawnLayout[0].Number));
        changed.Quantity = 99;
        changed.X1 = 0;

        await new FieldMonsterSpawnsUpdatePlugInSeason6().ApplyUpdateAsync(context, gameConfiguration).ConfigureAwait(false);

        AssertLayouts(gameConfiguration);
    }

    /// <summary>
    /// Tests that the 0.95d data keeps the monster spawns of version 0.75 on Devias and Noria.
    /// </summary>
    [Test]
    public async Task Version095dKeepsItsSpawnsAsync()
    {
        var (context, gameConfiguration) = await TestGameConfiguration.Create095dAsync().ConfigureAwait(false);
        using var _ = context;

        foreach (var (mapNumber, layout) in Layouts.Where(l => l.MapNumber != Elvenland.Number))
        {
            var map = gameConfiguration.Maps.Single(m => m.Number == mapNumber);
            var layoutIds = layout.Select(spawn => GuidHelper.CreateGuid<MonsterSpawnArea>(mapNumber, spawn.Number)).ToHashSet();
            Assert.That(map.MonsterSpawns.Any(s => layoutIds.Contains(s.GetId())), Is.False, $"map {mapNumber}");
            Assert.That(map.MonsterSpawns.Any(s => s.MonsterDefinition?.ObjectKind == NpcObjectKind.Monster), Is.True, $"map {mapNumber}");
        }
    }

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

    private static void AssertLayouts(GameConfiguration gameConfiguration)
    {
        foreach (var (mapNumber, layout) in Layouts)
        {
            var map = gameConfiguration.Maps.Single(m => m.Number == mapNumber);
            var layoutMonsters = layout.Select(spawn => spawn.MonsterNumber).ToHashSet();
            var expected = layout.Select(spawn => (GuidHelper.CreateGuid<MonsterSpawnArea>(mapNumber, spawn.Number), spawn.MonsterNumber, spawn.X1, spawn.X2, spawn.Y1, spawn.Y2, spawn.Quantity));
            var actual = map.MonsterSpawns
                .Where(s => s.SpawnTrigger == SpawnTrigger.Automatic && layoutMonsters.Contains(s.MonsterDefinition!.Number))
                .Select(s => (s.GetId(), s.MonsterDefinition!.Number, s.X1, s.X2, s.Y1, s.Y2, s.Quantity));
            Assert.That(actual, Is.EquivalentTo(expected), $"map {mapNumber}");
        }
    }

    private static void RemoveLayout(GameMapDefinition map, IReadOnlyList<FieldMonsterSpawn> layout)
    {
        var layoutIds = layout.Select(spawn => GuidHelper.CreateGuid<MonsterSpawnArea>(map.Number, spawn.Number)).ToHashSet();
        var layoutSpawns = map.MonsterSpawns.Where(s => layoutIds.Contains(s.GetId())).ToList();
        foreach (var spawn in layoutSpawns)
        {
            map.MonsterSpawns.Remove(spawn);
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
