// <copyright file="MapImportMapPreview.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.Web.Shared.Components.MapEditor;

using MUnique.OpenMU.DataModel.Configuration;

/// <summary>
/// The importable content of one map.
/// </summary>
/// <param name="Map">The target map.</param>
/// <param name="SpawnCounts">The number of importable spawn areas per kind.</param>
/// <param name="ExitGateCount">The number of importable exit gates.</param>
/// <param name="EnterGateCount">The number of importable enter gates.</param>
public sealed record MapImportMapPreview(
    GameMapDefinition Map,
    IReadOnlyDictionary<MapObjectKind, int> SpawnCounts,
    int ExitGateCount,
    int EnterGateCount)
{
    /// <summary>
    /// Gets a value indicating whether this map has any importable records.
    /// </summary>
    public bool HasChanges =>
        this.SpawnCounts.Values.Any(count => count > 0) || this.ExitGateCount > 0 || this.EnterGateCount > 0;

    /// <summary>
    /// Gets the number of importable spawn areas of the specified kind.
    /// </summary>
    /// <param name="kind">The kind.</param>
    /// <returns>The number of importable spawn areas.</returns>
    public int GetSpawnCount(MapObjectKind kind) => this.SpawnCounts.TryGetValue(kind, out var count) ? count : 0;

    /// <summary>
    /// Gets the number of importable objects of the specified kind. For <see cref="MapObjectKind.Gates"/>,
    /// this is the sum of exit and enter gates.
    /// </summary>
    /// <param name="kind">The kind.</param>
    /// <returns>The number of importable objects.</returns>
    public int GetObjectCount(MapObjectKind kind) =>
        kind == MapObjectKind.Gates ? this.ExitGateCount + this.EnterGateCount : this.GetSpawnCount(kind);
}
