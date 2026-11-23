// <copyright file="MapImportPlanner.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.Web.Shared.Components.MapEditor;

using System.Threading;
using MUnique.OpenMU.DataModel.Configuration;
using MUnique.OpenMU.Persistence;

/// <summary>
/// Validates an import file and builds a preview without changing anything.
/// </summary>
public sealed class MapImportPlanner
{
    private readonly MapExportFileReader _reader;

    /// <summary>
    /// Initializes a new instance of the <see cref="MapImportPlanner"/> class.
    /// </summary>
    /// <param name="reader">The reader which parses the import file.</param>
    public MapImportPlanner(MapExportFileReader reader)
    {
        this._reader = reader;
    }

    /// <summary>
    /// Creates the import preview for the given JSON content.
    /// </summary>
    /// <param name="json">The JSON content.</param>
    /// <param name="targetMaps">The maps which are available as import target.</param>
    /// <param name="includeGates">Whether gates may be imported. They are only supported for all maps at once.</param>
    /// <param name="context">The persistence context used to resolve monster definitions.</param>
    /// <param name="allMaps">
    /// All maps of the database in their regular order. It is used to tell apart maps which share the same
    /// number (e.g. the Devil Squares), when <paramref name="targetMaps"/> is a subset of them.
    /// When <see langword="null"/>, <paramref name="targetMaps"/> is used.
    /// </param>
    /// <param name="cancellationToken">The cancellation token.</param>
    /// <returns>The preview of the importable changes.</returns>
    public async ValueTask<MapImportPreview> CreatePreviewAsync(
        string json,
        IReadOnlyCollection<GameMapDefinition> targetMaps,
        bool includeGates,
        IContext context,
        IReadOnlyCollection<GameMapDefinition>? allMaps = null,
        CancellationToken cancellationToken = default)
    {
        if (!this._reader.TryRead(json, out var file, out var error))
        {
            return new MapImportPreview { Error = error };
        }

        var monsters = await context.GetAsync<MonsterDefinition>(cancellationToken).ConfigureAwait(false);
        var monstersByNumber = monsters
            .GroupBy(monster => (int)monster.Number)
            .ToDictionary(group => group.Key, group => group.First());

        var declaredKinds = new HashSet<MapObjectKind>(file.Kinds);
        if (!includeGates)
        {
            declaredKinds.Remove(MapObjectKind.Gates);
        }

        var warnings = new List<MapImportWarning>();
        if (!TryResolveTargets(file, targetMaps, allMaps ?? targetMaps, warnings, out var resolvedMaps, out var targetError))
        {
            return new MapImportPreview { Error = targetError };
        }

        var includeGateData = includeGates && declaredKinds.Contains(MapObjectKind.Gates);
        var exitGateKeys = includeGateData
            ? resolvedMaps.SelectMany(pair => pair.Export.ExitGates).Select(gate => gate.Key).ToHashSet()
            : new HashSet<int>();

        var targets = new List<MapImportTarget>();
        foreach (var (export, map) in resolvedMaps)
        {
            var target = new MapImportTarget(map);
            AddSpawns(target, export, monstersByNumber, declaredKinds, warnings);
            if (includeGateData)
            {
                AddGates(target, export, exitGateKeys, warnings);
            }

            targets.Add(target);
        }

        var maps = targets
            .Select(target => new MapImportMapPreview(
                target.Map,
                target.Spawns
                    .GroupBy(spawn => spawn.Kind)
                    .ToDictionary(group => group.Key, group => group.Count()),
                target.ExitGates.Count,
                target.EnterGates.Count))
            .ToList();

        return new MapImportPreview(targets)
        {
            DeclaredKinds = declaredKinds.OrderBy(kind => kind).ToList(),
            Maps = maps,
            Warnings = warnings,
        };
    }

    /// <summary>
    /// Pairs the maps of the file with the target maps. The map number is not unique (e.g. all Devil Squares
    /// share one number), so maps with the same number are paired by their order of occurrence.
    /// </summary>
    private static bool TryResolveTargets(
        MapExportFile file,
        IReadOnlyCollection<GameMapDefinition> targetMaps,
        IReadOnlyCollection<GameMapDefinition> allMaps,
        ICollection<MapImportWarning> warnings,
        out List<(MapExport Export, GameMapDefinition Map)> resolvedMaps,
        out MapImportError error)
    {
        resolvedMaps = new List<(MapExport Export, GameMapDefinition Map)>();
        error = default;

        if (targetMaps.Count == 0)
        {
            error = MapImportError.MapNotFound;
            return false;
        }

        if (targetMaps.Count == 1)
        {
            var targetMap = targetMaps.First();
            if (file.Maps.Count == 1)
            {
                // A single-map file can be imported into the currently selected map, even if
                // it was exported from another map number.
                resolvedMaps.Add((file.Maps.First(), targetMap));
                return true;
            }

            var occurrence = allMaps.Where(map => map.Number == targetMap.Number).ToList().IndexOf(targetMap);
            var candidates = file.Maps.Where(export => export.MapNumber == targetMap.Number).ToList();
            var index = Math.Max(0, occurrence);
            if (index >= candidates.Count)
            {
                error = MapImportError.MapNotFound;
                return false;
            }

            resolvedMaps.Add((candidates[index], targetMap));
            return true;
        }

        var targetsByNumber = targetMaps
            .GroupBy(map => map.Number)
            .ToDictionary(group => group.Key, group => group.ToList());
        var occurrences = new Dictionary<short, int>();
        foreach (var export in file.Maps)
        {
            occurrences.TryGetValue(export.MapNumber, out var index);
            occurrences[export.MapNumber] = index + 1;

            if (targetsByNumber.TryGetValue(export.MapNumber, out var targets) && index < targets.Count)
            {
                resolvedMaps.Add((export, targets[index]));
            }
            else
            {
                warnings.Add(new MapImportWarning(MapImportWarningKind.UnknownMapNumber, export.MapNumber));
            }
        }

        return true;
    }

    private static void AddSpawns(
        MapImportTarget target,
        MapExport export,
        IReadOnlyDictionary<int, MonsterDefinition> monstersByNumber,
        IReadOnlySet<MapObjectKind> declaredKinds,
        ICollection<MapImportWarning> warnings)
    {
        foreach (var spawnExport in export.Spawns)
        {
            if (!monstersByNumber.TryGetValue(spawnExport.MonsterNumber, out var monsterDefinition))
            {
                warnings.Add(new MapImportWarning(
                    MapImportWarningKind.UnknownMonsterNumber,
                    target.Map.Number,
                    spawnExport.MonsterNumber));
                continue;
            }

            var actualKind = MapObjectKindClassifier.Classify(monsterDefinition);
            if (actualKind != spawnExport.Kind)
            {
                warnings.Add(new MapImportWarning(
                    MapImportWarningKind.KindMismatch,
                    target.Map.Number,
                    spawnExport.MonsterNumber,
                    spawnExport.Kind,
                    actualKind));
                continue;
            }

            if (!declaredKinds.Contains(actualKind))
            {
                warnings.Add(new MapImportWarning(
                    MapImportWarningKind.KindNotDeclared,
                    target.Map.Number,
                    spawnExport.MonsterNumber,
                    ActualKind: actualKind));
                continue;
            }

            target.Spawns.Add(new PlannedSpawn(spawnExport, monsterDefinition));
        }
    }

    private static void AddGates(
        MapImportTarget target,
        MapExport export,
        IReadOnlySet<int> exitGateKeys,
        ICollection<MapImportWarning> warnings)
    {
        target.ExitGates.AddRange(export.ExitGates);

        foreach (var enterGate in export.EnterGates)
        {
            if (enterGate.TargetGateKey is { } key && exitGateKeys.Contains(key))
            {
                target.EnterGates.Add(enterGate);
            }
            else
            {
                warnings.Add(new MapImportWarning(
                    MapImportWarningKind.UnresolvedGateTarget,
                    target.Map.Number,
                    GateKey: enterGate.TargetGateKey));
            }
        }
    }
}
