// <copyright file="MapExportBuilder.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.Web.Shared.Components.MapEditor;

using MUnique.OpenMU.DataModel.Configuration;

/// <summary>
/// Builds an export file from the domain objects of the selected maps and kinds.
/// </summary>
public sealed class MapExportBuilder
{
    /// <summary>
    /// Builds the export file.
    /// </summary>
    /// <param name="maps">The maps to export.</param>
    /// <param name="kinds">The kinds to export.</param>
    /// <returns>The export file.</returns>
    public MapExportFile Build(IReadOnlyCollection<GameMapDefinition> maps, IReadOnlySet<MapObjectKind> kinds)
    {
        var file = new MapExportFile
        {
            Kinds = kinds.OrderBy(kind => kind).ToList(),
        };

        var exitGateKeys = AssignExitGateKeys(maps, kinds);
        foreach (var map in maps)
        {
            file.Maps.Add(BuildMap(map, kinds, exitGateKeys));
        }

        return file;
    }

    private static Dictionary<ExitGate, int> AssignExitGateKeys(IReadOnlyCollection<GameMapDefinition> maps, IReadOnlySet<MapObjectKind> kinds)
    {
        var result = new Dictionary<ExitGate, int>();
        if (!kinds.Contains(MapObjectKind.Gates))
        {
            return result;
        }

        var nextKey = 1;
        foreach (var gate in maps.SelectMany(map => map.ExitGates))
        {
            result[gate] = nextKey++;
        }

        return result;
    }

    private static MapExport BuildMap(GameMapDefinition map, IReadOnlySet<MapObjectKind> kinds, IReadOnlyDictionary<ExitGate, int> exitGateKeys)
    {
        var export = new MapExport
        {
            MapNumber = map.Number,
        };

        foreach (var spawn in map.MonsterSpawns)
        {
            var kind = MapObjectKindClassifier.Classify(spawn);
            if (!kinds.Contains(kind))
            {
                continue;
            }

            export.Spawns.Add(new SpawnExport
            {
                Kind = kind,
                X1 = spawn.X1,
                Y1 = spawn.Y1,
                X2 = spawn.X2,
                Y2 = spawn.Y2,
                Direction = spawn.Direction,
                Quantity = spawn.Quantity,
                SpawnTrigger = spawn.SpawnTrigger,
                WaveNumber = spawn.WaveNumber,
                MaximumHealthOverride = spawn.MaximumHealthOverride,
                MonsterNumber = spawn.MonsterDefinition?.Number ?? 0,
            });
        }

        if (!kinds.Contains(MapObjectKind.Gates))
        {
            return export;
        }

        foreach (var gate in map.ExitGates)
        {
            export.ExitGates.Add(new ExitGateExport
            {
                Key = exitGateKeys[gate],
                X1 = gate.X1,
                Y1 = gate.Y1,
                X2 = gate.X2,
                Y2 = gate.Y2,
                Direction = gate.Direction,
                IsSpawnGate = gate.IsSpawnGate,
            });
        }

        foreach (var gate in map.EnterGates)
        {
            export.EnterGates.Add(new EnterGateExport
            {
                X1 = gate.X1,
                Y1 = gate.Y1,
                X2 = gate.X2,
                Y2 = gate.Y2,
                LevelRequirement = gate.LevelRequirement,
                Number = gate.Number,
                TargetGateKey = gate.TargetGate is { } targetGate && exitGateKeys.TryGetValue(targetGate, out var key)
                    ? key
                    : null,
            });
        }

        return export;
    }
}
