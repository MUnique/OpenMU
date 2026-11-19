// <copyright file="NpcSpawnsUpdatePlugInBase.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.Persistence.Initialization.Updates;

using MUnique.OpenMU.DataModel.Configuration;

/// <summary>
/// Brings the NPC spawns of a game version to the Season 6 data: removes the spawns which the version doesn't have,
/// corrects positions and directions and adds missing spawns.
/// </summary>
/// <remarks>
/// Spawns are identified by their number, like in the map initializers, which define the same values.
/// A spawn is only removed or changed when it still has the expected NPC, so a spawn which the owner reused
/// for another NPC is kept.
/// </remarks>
public abstract class NpcSpawnsUpdatePlugInBase : UpdatePlugInBase
{
    /// <inheritdoc />
    public override bool IsMandatory => false;

    /// <summary>
    /// Gets the NPC spawns which the game version doesn't have.
    /// </summary>
    internal abstract IReadOnlyList<(byte MapNumber, short SpawnNumber, short NpcNumber)> RemovedSpawns { get; }

    /// <summary>
    /// Gets the NPC spawns which get the Season 6 position and direction, or which are added when they're missing.
    /// </summary>
    internal abstract IReadOnlyList<NpcSpawnCorrection> Corrections { get; }

    /// <inheritdoc />
    protected override ValueTask ApplyAsync(IContext context, GameConfiguration gameConfiguration)
    {
        foreach (var (mapNumber, spawnNumber, npcNumber) in this.RemovedSpawns)
        {
            if (FindMap(gameConfiguration, mapNumber) is { } map
                && FindSpawn(map, spawnNumber) is { } spawn
                && spawn.MonsterDefinition?.Number == npcNumber)
            {
                map.MonsterSpawns.Remove(spawn);
            }
        }

        foreach (var correction in this.Corrections)
        {
            if (FindMap(gameConfiguration, correction.MapNumber) is not { } map)
            {
                continue;
            }

            var spawn = FindSpawn(map, correction.SpawnNumber);
            if (spawn is null)
            {
                if (!correction.IsAdded
                    || gameConfiguration.Monsters.FirstOrDefault(m => m.Number == correction.NpcNumber) is not { } npc)
                {
                    continue;
                }

                spawn = context.CreateNew<MonsterSpawnArea>();
                spawn.SetGuid(correction.MapNumber, correction.SpawnNumber);
                spawn.GameMap = map;
                spawn.MonsterDefinition = npc;
                spawn.Quantity = 1;
                spawn.SpawnTrigger = SpawnTrigger.Automatic;
                map.MonsterSpawns.Add(spawn);
            }

            // Only where the spawn still has its NPC; a spawn which the owner reused for another NPC is kept.
            if (spawn.MonsterDefinition?.Number != correction.NpcNumber)
            {
                continue;
            }

            spawn.X1 = spawn.X2 = correction.X;
            spawn.Y1 = spawn.Y2 = correction.Y;
            spawn.Direction = correction.Direction;
        }

        return ValueTask.CompletedTask;
    }

    private static GameMapDefinition? FindMap(GameConfiguration gameConfiguration, byte mapNumber)
        => gameConfiguration.Maps.FirstOrDefault(map => map.Number == mapNumber);

    private static MonsterSpawnArea? FindSpawn(GameMapDefinition map, short spawnNumber)
    {
        var id = GuidHelper.CreateGuid<MonsterSpawnArea>(map.Number, spawnNumber);
        return map.MonsterSpawns.FirstOrDefault(s => s.GetId() == id);
    }
}
