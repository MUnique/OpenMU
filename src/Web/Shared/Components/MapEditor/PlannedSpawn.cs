// <copyright file="PlannedSpawn.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.Web.Shared.Components.MapEditor;

using MUnique.OpenMU.DataModel.Configuration;

/// <summary>
/// A spawn area of an import file with its resolved monster definition.
/// </summary>
/// <param name="Export">The exported spawn area.</param>
/// <param name="Definition">The resolved monster definition.</param>
internal sealed record PlannedSpawn(SpawnExport Export, MonsterDefinition Definition)
{
    /// <summary>
    /// Gets the kind of the spawn area.
    /// </summary>
    public MapObjectKind Kind => this.Export.Kind;
}
