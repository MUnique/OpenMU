// <copyright file="MapExport.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.Web.Shared.Components.MapEditor;

using MUnique.OpenMU.DataModel.Configuration;

/// <summary>
/// Serializable representation of the exported content of one map.
/// </summary>
public sealed record MapExport
{
    /// <summary>
    /// Gets or sets the <see cref="GameMapDefinition.Number"/> of the map.
    /// </summary>
    public required short MapNumber { get; set; }

    /// <summary>Gets or sets the exported spawn areas.</summary>
    public ICollection<SpawnExport> Spawns { get; set; } = new List<SpawnExport>();

    /// <summary>Gets or sets the exported exit gates.</summary>
    public ICollection<ExitGateExport> ExitGates { get; set; } = new List<ExitGateExport>();

    /// <summary>Gets or sets the exported enter gates.</summary>
    public ICollection<EnterGateExport> EnterGates { get; set; } = new List<EnterGateExport>();
}
