// <copyright file="MapImportTarget.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.Web.Shared.Components.MapEditor;

using MUnique.OpenMU.DataModel.Configuration;

/// <summary>
/// The validated changes of one target map.
/// </summary>
internal sealed class MapImportTarget
{
    /// <summary>
    /// Initializes a new instance of the <see cref="MapImportTarget"/> class.
    /// </summary>
    /// <param name="map">The target map.</param>
    public MapImportTarget(GameMapDefinition map)
    {
        this.Map = map;
    }

    /// <summary>
    /// Gets the target map.
    /// </summary>
    public GameMapDefinition Map { get; }

    /// <summary>
    /// Gets the spawn areas to create.
    /// </summary>
    public List<PlannedSpawn> Spawns { get; } = new List<PlannedSpawn>();

    /// <summary>
    /// Gets the exit gates to update or create.
    /// </summary>
    public List<ExitGateExport> ExitGates { get; } = new List<ExitGateExport>();

    /// <summary>
    /// Gets the enter gates to create.
    /// </summary>
    public List<EnterGateExport> EnterGates { get; } = new List<EnterGateExport>();
}
