// <copyright file="FieldMonsterSpawnsUpdatePlugInBase.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.Persistence.Initialization.Updates;

using MUnique.OpenMU.DataModel.Configuration;
using MUnique.OpenMU.Persistence.Initialization.Version075.Maps;

/// <summary>
/// Brings the automatic monster spawns of the field maps to the Season 6 data, which all versions use.
/// </summary>
/// <remarks>
/// Version 1 removed the monster spawns which the initialization data of version 0.75 defined twice in
/// Dungeon and Lost Tower: Dungeon had each of its monster spawns twice (the spawn numbers 361 to 616 repeat
/// 101 to 359), so it had twice the intended monsters, and Lost Tower had two monster spawns twice.
/// Traps are kept, also where several of them stand on the same spot.
/// Version 2 brings the spawns of the field maps to the Season 6 data in all versions, because no better data
/// of the older versions is known and it keeps the monster levels of the maps consistent across the versions:
/// Devias and Noria get the layout of smaller areas and single spots instead of quarters of the map (the spiders,
/// which Season 6 added to Noria, only in Season 6), and the other field maps get the corrections of
/// <see cref="SpawnCorrections"/>: missing spawns are added, surplus ones removed and spawns with wrong
/// coordinates corrected.
/// </remarks>
public abstract class FieldMonsterSpawnsUpdatePlugInBase : UpdatePlugInBase
{
    /// <summary>
    /// The plug-in name.
    /// </summary>
    internal const string PlugInName = "Season 6 monster spawns of the field maps";

    /// <summary>
    /// The plug-in description.
    /// </summary>
    internal const string PlugInDescription = "Brings the monster spawns of the field maps to the Season 6 data, which this version uses as well: Devias and Noria get the layout of smaller areas and single spots instead of quarters of the map, without the spiders of Season 6. Missing spawns of Dungeon, Lost Tower and Icarus are added and surplus ones removed. Also removes the duplicate monster spawns of Dungeon (254) and Lost Tower (2). Customized spawns of the field monsters of Devias and Noria are overwritten.";

    private const short FirstDungeonDuplicateNumber = 361;

    private const short LastDungeonDuplicateNumber = 616;

    private static readonly short[] LostTowerDuplicateNumbers = [338, 536];

    /// <inheritdoc />
    public override string Name => PlugInName;

    /// <inheritdoc />
    public override string Description => PlugInDescription;

    /// <inheritdoc />
    public override bool IsMandatory => false;

    /// <inheritdoc />
    public override int Version => 2;

    /// <inheritdoc />
    public override DateTime CreatedAt => new(2026, 11, 11, 12, 0, 0, DateTimeKind.Utc);

    /// <inheritdoc />
    public override DateTime UpdatedAt => new(2026, 11, 16, 12, 0, 0, DateTimeKind.Utc);

    /// <summary>
    /// Gets the corrections of the monster spawns of the maps which all versions have.
    /// </summary>
    internal static IReadOnlyList<FieldMonsterSpawnCorrection> CommonSpawnCorrections { get; } =
    [
        new(
            Dungeon.Number,
            [
                new(769, 8, 30, 30, 122, 122, 1), // Poison Bull
                new(770, 8, 40, 40, 98, 98, 1), // Poison Bull
            ],
            [101, 107, 108, 109, 110]),
        new(
            LostTower.Number,
            [
                new(698, 39, 203, 203, 50, 50, 1), // Poison Shadow
                new(699, 39, 236, 236, 127, 127, 1), // Poison Shadow
            ],
            [101, 102, 103, 104, 105, 106]),
    ];

    /// <summary>
    /// Gets the corrections of the monster spawns of Icarus, which exists since version 0.95d.
    /// </summary>
    internal static FieldMonsterSpawnCorrection IcarusSpawnCorrection { get; } =
        new(
            Version095d.Maps.Icarus.Number,
            [
                new(154, 75, 32, 32, 231, 231, 1), // Great Drakan
                new(164, 70, 84, 84, 36, 36, 1), // Queen Rainer
                new(165, 70, 90, 90, 48, 48, 1), // Queen Rainer
                new(166, 70, 95, 95, 56, 56, 1), // Queen Rainer
                new(167, 70, 74, 74, 84, 84, 1), // Queen Rainer
                new(168, 70, 74, 74, 72, 72, 1), // Queen Rainer
                new(169, 70, 57, 57, 71, 71, 1), // Queen Rainer
                new(170, 73, 28, 28, 67, 67, 1), // Drakan
                new(171, 71, 50, 50, 33, 33, 1), // Mega Crust
                new(172, 71, 59, 59, 27, 27, 1), // Mega Crust
                new(173, 71, 74, 74, 38, 38, 1), // Mega Crust
                new(174, 69, 36, 36, 29, 29, 1), // Alquamos
                new(175, 69, 37, 37, 41, 41, 1), // Alquamos
                new(176, 69, 26, 26, 41, 41, 1), // Alquamos
                new(177, 69, 21, 21, 24, 24, 1), // Alquamos
                new(178, 75, 45, 45, 173, 173, 1), // Great Drakan
                new(179, 75, 59, 59, 163, 163, 1), // Great Drakan
                new(180, 75, 52, 52, 168, 168, 1), // Great Drakan
                new(181, 69, 21, 21, 36, 36, 1), // Alquamos
                new(182, 70, 64, 64, 84, 84, 1), // Queen Rainer
                new(183, 70, 69, 69, 77, 77, 1), // Queen Rainer
            ],
            [103, 105, 119, 132]);

    /// <summary>
    /// Gets the corrections of the monster spawns of the field maps of this version, besides the layouts of
    /// Devias and Noria.
    /// </summary>
    internal virtual IReadOnlyList<FieldMonsterSpawnCorrection> SpawnCorrections => CommonSpawnCorrections;

    /// <summary>
    /// Gets the automatic monster spawns of Noria of this version.
    /// </summary>
    internal virtual IReadOnlyList<FieldMonsterSpawn> NoriaMonsterSpawnLayout => Noria.MonsterSpawnLayout;

    /// <inheritdoc />
    protected override ValueTask ApplyAsync(IContext context, GameConfiguration gameConfiguration)
    {
        var dungeonDuplicates = Enumerable.Range(FirstDungeonDuplicateNumber, LastDungeonDuplicateNumber - FirstDungeonDuplicateNumber + 1).Select(number => (short)number);
        RemoveSpawns(gameConfiguration, Dungeon.Number, dungeonDuplicates);
        RemoveSpawns(gameConfiguration, LostTower.Number, LostTowerDuplicateNumbers);
        ApplyLayout(context, gameConfiguration, Devias.Number, Devias.MonsterSpawnLayout);
        ApplyLayout(context, gameConfiguration, Noria.Number, this.NoriaMonsterSpawnLayout);
        foreach (var correction in this.SpawnCorrections)
        {
            RemoveSpawns(gameConfiguration, correction.MapNumber, correction.RemovedSpawnNumbers);
            ApplySpawns(context, gameConfiguration, correction.MapNumber, correction.Spawns);
        }

        this.ApplyVersionSpecificChanges(context, gameConfiguration);
        return ValueTask.CompletedTask;
    }

    /// <summary>
    /// Applies the changes to the monster spawns which only belong to the game version.
    /// </summary>
    /// <param name="context">The context.</param>
    /// <param name="gameConfiguration">The game configuration.</param>
    protected virtual void ApplyVersionSpecificChanges(IContext context, GameConfiguration gameConfiguration)
    {
    }

    /// <summary>
    /// Ensures that the automatic spawns of the monsters of the layout on the map are exactly the ones of the layout.
    /// </summary>
    /// <param name="context">The context.</param>
    /// <param name="gameConfiguration">The game configuration.</param>
    /// <param name="mapNumber">The number of the map.</param>
    /// <param name="layout">The spawns of the layout.</param>
    private protected static void ApplyLayout(IContext context, GameConfiguration gameConfiguration, byte mapNumber, IReadOnlyList<FieldMonsterSpawn> layout)
    {
        if (gameConfiguration.Maps.FirstOrDefault(map => map.Number == mapNumber) is not { } map)
        {
            return;
        }

        ApplySpawns(context, gameConfiguration, mapNumber, layout);
        var layoutIds = layout.Select(spawn => GuidHelper.CreateGuid<MonsterSpawnArea>(mapNumber, spawn.Number)).ToHashSet();
        var layoutMonsters = layout.Select(spawn => spawn.MonsterNumber).ToHashSet();
        var replacedSpawns = map.MonsterSpawns
            .Where(s => s.SpawnTrigger == SpawnTrigger.Automatic
                        && s.MonsterDefinition is { } monster
                        && layoutMonsters.Contains(monster.Number)
                        && !layoutIds.Contains(s.GetId()))
            .ToList();
        foreach (var spawn in replacedSpawns)
        {
            map.MonsterSpawns.Remove(spawn);
        }
    }

    /// <summary>
    /// Adds the spawns to the map, or updates them in place when they exist already.
    /// </summary>
    /// <param name="context">The context.</param>
    /// <param name="gameConfiguration">The game configuration.</param>
    /// <param name="mapNumber">The number of the map.</param>
    /// <param name="spawns">The spawns.</param>
    private static void ApplySpawns(IContext context, GameConfiguration gameConfiguration, byte mapNumber, IReadOnlyList<FieldMonsterSpawn> spawns)
    {
        if (gameConfiguration.Maps.FirstOrDefault(map => map.Number == mapNumber) is not { } map)
        {
            return;
        }

        foreach (var spawn in spawns)
        {
            var monster = gameConfiguration.Monsters.FirstOrDefault(m => m.Number == spawn.MonsterNumber)
                          ?? throw new InvalidOperationException($"The spawns of map {mapNumber} need monster {spawn.MonsterNumber}, which is missing in the game configuration.");
            var id = GuidHelper.CreateGuid<MonsterSpawnArea>(mapNumber, spawn.Number);

            // Updated in place when it's already there, so applying the update again doesn't duplicate anything.
            var area = map.MonsterSpawns.FirstOrDefault(s => s.GetId() == id);
            if (area is null)
            {
                area = context.CreateNew<MonsterSpawnArea>();
                area.SetGuid(id);
                map.MonsterSpawns.Add(area);
            }

            area.GameMap = map;
            area.MonsterDefinition = monster;
            area.Quantity = spawn.Quantity;
            area.Direction = Direction.Undefined;
            area.SpawnTrigger = SpawnTrigger.Automatic;
            area.X1 = spawn.X1;
            area.X2 = spawn.X2;
            area.Y1 = spawn.Y1;
            area.Y2 = spawn.Y2;
            area.WaveNumber = 0;
        }
    }

    private static void RemoveSpawns(GameConfiguration gameConfiguration, byte mapNumber, IEnumerable<short> spawnNumbers)
    {
        if (gameConfiguration.Maps.FirstOrDefault(map => map.Number == mapNumber) is not { } map)
        {
            return;
        }

        var ids = spawnNumbers.Select(number => GuidHelper.CreateGuid<MonsterSpawnArea>(mapNumber, number)).ToHashSet();
        foreach (var spawn in map.MonsterSpawns.Where(s => ids.Contains(s.GetId())).ToList())
        {
            map.MonsterSpawns.Remove(spawn);
        }
    }
}
