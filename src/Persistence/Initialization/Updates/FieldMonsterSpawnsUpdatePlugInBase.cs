// <copyright file="FieldMonsterSpawnsUpdatePlugInBase.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.Persistence.Initialization.Updates;

using MUnique.OpenMU.DataModel.Configuration;
using MUnique.OpenMU.Persistence.Initialization.Version075.Maps;

/// <summary>
/// Removes the monster spawns which the initialization data of version 0.75 defined twice in
/// Dungeon and Lost Tower. These maps are the same in all versions.
/// </summary>
/// <remarks>
/// Dungeon had each of its monster spawns twice (the spawn numbers 361 to 616 repeat 101 to 359),
/// so it had twice the intended monsters. Lost Tower had two monster spawns twice.
/// Only these spawns are removed; traps are kept, also where several of them stand on the same spot.
/// </remarks>
public abstract class FieldMonsterSpawnsUpdatePlugInBase : UpdatePlugInBase
{
    /// <summary>
    /// The plug-in name.
    /// </summary>
    internal const string PlugInName = "Remove duplicate monster spawns of Dungeon and Lost Tower";

    /// <summary>
    /// The plug-in description.
    /// </summary>
    internal const string PlugInDescription = "Removes the monster spawns which are defined twice: 254 in Dungeon, which therefore had twice the intended monsters, and 2 in Lost Tower. Other spawns are kept.";

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
    public override DateTime CreatedAt => new(2026, 11, 11, 12, 0, 0, DateTimeKind.Utc);

    /// <inheritdoc />
    protected override ValueTask ApplyAsync(IContext context, GameConfiguration gameConfiguration)
    {
        var dungeonDuplicates = Enumerable.Range(FirstDungeonDuplicateNumber, LastDungeonDuplicateNumber - FirstDungeonDuplicateNumber + 1).Select(number => (short)number);
        RemoveSpawns(gameConfiguration, Dungeon.Number, dungeonDuplicates);
        RemoveSpawns(gameConfiguration, LostTower.Number, LostTowerDuplicateNumbers);
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
