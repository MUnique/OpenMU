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

/// <summary>
/// Serializable representation of an <see cref="ExitGate"/>.
/// </summary>
public sealed record ExitGateExport
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

    /// <summary>Gets or sets a value indicating whether this gate is a spawn gate.</summary>
    public bool IsSpawnGate { get; set; }
}

/// <summary>
/// Serializable representation of an <see cref="EnterGate"/>.
/// </summary>
public sealed record EnterGateExport
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

    /// <summary>Gets or sets the level requirement.</summary>
    public short LevelRequirement { get; set; }

    /// <summary>Gets or sets the gate number.</summary>
    public short Number { get; set; }

    /// <summary>
    /// Gets or sets the original <see cref="ExitGate"/> id this enter gate targets.
    /// </summary>
    public Guid? TargetGateId { get; set; }
}
