// <copyright file="EnterGateExport.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.Web.Shared.Components.MapEditor;

using MUnique.OpenMU.DataModel.Configuration;

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
