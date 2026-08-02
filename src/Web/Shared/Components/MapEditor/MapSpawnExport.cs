// <copyright file="MapSpawnExport.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.Web.Shared.Components.MapEditor;

using MUnique.OpenMU.DataModel.Configuration;

/// <summary>
/// DTO for exporting/importing spawn areas, enter gates, and exit gates of a single map.
/// </summary>
public sealed record MapSpawnExport
{
    /// <summary>
    /// Gets or sets the format version for forward compatibility.
    /// </summary>
    public string FormatVersion { get; set; } = "1.0";

    /// <summary>
    /// Gets or sets the exported monster spawn areas.
    /// </summary>
    public ICollection<SpawnExport> Spawns { get; set; } = new List<SpawnExport>();

    /// <summary>
    /// Gets or sets the exported exit gates.
    /// </summary>
    public ICollection<ExitGateExport> ExitGates { get; set; } = new List<ExitGateExport>();

    /// <summary>
    /// Gets or sets the exported enter gates.
    /// </summary>
    public ICollection<EnterGateExport> EnterGates { get; set; } = new List<EnterGateExport>();
}
