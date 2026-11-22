// <copyright file="MapImportApplier.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.Web.Shared.Components.MapEditor;

using MUnique.OpenMU.DataModel.Configuration;
using MUnique.OpenMU.Persistence;

/// <summary>
/// Applies a validated import preview to the persistence context.
/// </summary>
public sealed class MapImportApplier
{
    /// <summary>
    /// Applies the selected kinds of the preview.
    /// </summary>
    /// <param name="preview">The validated preview.</param>
    /// <param name="selectedKinds">The kinds which the user selected for import.</param>
    /// <param name="context">The persistence context.</param>
    /// <returns>A task representing the asynchronous operation.</returns>
    public async ValueTask ApplyAsync(
        MapImportPreview preview,
        IReadOnlySet<MapObjectKind> selectedKinds,
        IContext context)
    {
        var kinds = new HashSet<MapObjectKind>(selectedKinds);
        kinds.IntersectWith(preview.DeclaredKinds);

        var targets = preview.GetTargets();
        foreach (var target in targets)
        {
            foreach (var kind in kinds.Where(kind => kind != MapObjectKind.Gates))
            {
                await ReplaceSpawnsAsync(target, kind, context).ConfigureAwait(false);
            }
        }

        if (!kinds.Contains(MapObjectKind.Gates))
        {
            return;
        }

        // Exit gates can be referenced from outside the map, e.g. by warp entries, duel areas
        // and mini games. They are matched by their position and updated in place, so those
        // references stay intact. Enter gates are only referenced by their map, so they can be
        // replaced safely.
        var exitGatesByKey = new Dictionary<int, ExitGate>();
        foreach (var target in targets)
        {
            ApplyExitGates(target, exitGatesByKey, context);
        }

        foreach (var target in targets)
        {
            await ApplyEnterGatesAsync(target, exitGatesByKey, context).ConfigureAwait(false);
        }
    }

    private static async ValueTask ReplaceSpawnsAsync(
        MapImportTarget target,
        MapObjectKind kind,
        IContext context)
    {
        var existingSpawns = target.Map.MonsterSpawns
            .Where(spawn => MapObjectKindClassifier.Classify(spawn) == kind)
            .ToList();

        foreach (var spawn in existingSpawns)
        {
            target.Map.MonsterSpawns.Remove(spawn);
            await context.DeleteAsync(spawn).ConfigureAwait(false);
        }

        foreach (var planned in target.Spawns.Where(spawn => spawn.Kind == kind))
        {
            var export = planned.Export;
            var spawn = context.CreateNew<MonsterSpawnArea>();
            spawn.GameMap = target.Map;
            spawn.MonsterDefinition = planned.Definition;
            spawn.X1 = export.X1;
            spawn.Y1 = export.Y1;
            spawn.X2 = export.X2;
            spawn.Y2 = export.Y2;
            spawn.Direction = export.Direction;
            spawn.Quantity = export.Quantity;
            spawn.SpawnTrigger = export.SpawnTrigger;
            spawn.WaveNumber = export.WaveNumber;
            spawn.MaximumHealthOverride = export.MaximumHealthOverride;
            target.Map.MonsterSpawns.Add(spawn);
        }
    }

    private static void ApplyExitGates(
        MapImportTarget target,
        IDictionary<int, ExitGate> exitGatesByKey,
        IContext context)
    {
        var claimedGates = new HashSet<ExitGate>();
        foreach (var export in target.ExitGates)
        {
            var gate = target.Map.ExitGates.FirstOrDefault(existing =>
                !claimedGates.Contains(existing)
                && existing.X1 == export.X1
                && existing.Y1 == export.Y1
                && existing.X2 == export.X2
                && existing.Y2 == export.Y2);

            if (gate is null)
            {
                gate = context.CreateNew<ExitGate>();
                gate.Map = target.Map;
                gate.X1 = export.X1;
                gate.Y1 = export.Y1;
                gate.X2 = export.X2;
                gate.Y2 = export.Y2;
                target.Map.ExitGates.Add(gate);
            }

            gate.Direction = export.Direction;
            gate.IsSpawnGate = export.IsSpawnGate;
            claimedGates.Add(gate);
            exitGatesByKey[export.Key] = gate;
        }
    }

    private static async ValueTask ApplyEnterGatesAsync(
        MapImportTarget target,
        IReadOnlyDictionary<int, ExitGate> exitGatesByKey,
        IContext context)
    {
        foreach (var gate in target.Map.EnterGates.ToList())
        {
            target.Map.EnterGates.Remove(gate);
            await context.DeleteAsync(gate).ConfigureAwait(false);
        }

        foreach (var export in target.EnterGates)
        {
            // The planner only accepts enter gates with a resolvable target. This guard
            // keeps a gate without target from being created, which would violate [Required].
            if (export.TargetGateKey is not { } key || !exitGatesByKey.TryGetValue(key, out var targetGate))
            {
                continue;
            }

            var gate = context.CreateNew<EnterGate>();
            gate.X1 = export.X1;
            gate.Y1 = export.Y1;
            gate.X2 = export.X2;
            gate.Y2 = export.Y2;
            gate.LevelRequirement = export.LevelRequirement;
            gate.Number = export.Number;
            gate.TargetGate = targetGate;
            target.Map.EnterGates.Add(gate);
        }
    }
}
