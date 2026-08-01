// <copyright file="ExitGateExport.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.Web.Shared.Components.MapEditor;

using MUnique.OpenMU.DataModel.Configuration;

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
