// <copyright file="SpawnExport.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.Web.Shared.Components.MapEditor;

using MUnique.OpenMU.DataModel.Configuration;

/// <summary>
/// Serializable representation of a <see cref="MonsterSpawnArea"/>.
/// </summary>
public sealed record SpawnExport
{
    /// <summary>Gets or sets the original entity id.</summary>
    public Guid Id { get; set; }

    /// <summary>Gets or sets the top-left X coordinate.</summary>
    public byte X1 { get; set; }

    /// <summary>Gets or sets the top-left Y coordinate.</summary>
    public byte Y1 { get; set; }

    /// <summary>Gets or sets the bottom-right X coordinate.</summary>
    public byte X2 { get; set; }

    /// <summary>Gets or sets the bottom-right Y coordinate.</summary>
    public byte Y2 { get; set; }

    /// <summary>Gets or sets the direction.</summary>
    public Direction Direction { get; set; }

    /// <summary>Gets or sets the spawn quantity.</summary>
    public short Quantity { get; set; }

    /// <summary>Gets or sets the spawn trigger.</summary>
    public SpawnTrigger SpawnTrigger { get; set; }

    /// <summary>Gets or sets the wave number.</summary>
    public byte WaveNumber { get; set; }

    /// <summary>Gets or sets the maximum health override.</summary>
    public int? MaximumHealthOverride { get; set; }

    /// <summary>
    /// Gets or sets the <see cref="MonsterDefinition.Number"/> of the referenced monster definition.
    /// </summary>
    public int MonsterNumber { get; set; }
}
