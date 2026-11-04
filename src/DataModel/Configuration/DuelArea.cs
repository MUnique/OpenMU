// <copyright file="DuelArea.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.DataModel.Configuration;

using MUnique.OpenMU.Annotations;

/// <summary>
/// Defines an area where a duel can take place.
/// </summary>
[Cloneable]
public partial class DuelArea
{
    /// <summary>
    /// Gets or sets the index of the area.
    /// </summary>
    public short Index { get; set; }

    /// <summary>
    /// Gets or sets the first player gate to which the player is teleported when he enters the duel.
    /// </summary>
    public virtual ExitGate? FirstPlayerGate { get; set; }

    /// <summary>
    /// Gets or sets the second player gate to which the player is teleported when he enters the duel.
    /// </summary>
    public virtual ExitGate? SecondPlayerGate { get; set; }

    /// <summary>
    /// Gets or sets the gate to which a spectator is teleported when he enters the duel.
    /// </summary>
    public virtual ExitGate? SpectatorsGate { get; set; }

    /// <inheritdoc />
    public override string ToString()
    {
        return $"{this.Index} - {this.FirstPlayerGate} - {this.SecondPlayerGate}";
    }
}