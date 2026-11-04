// <copyright file="ExitGate.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.DataModel.Configuration;

using MUnique.OpenMU.Annotations;

/// <summary>
/// Defines a gate through which a player enters a map.
/// </summary>
[Cloneable]
public partial class ExitGate : Gate
{
    /// <summary>
    /// Gets or sets the direction to which the player looks when he enters the map.
    /// </summary>
    public Direction Direction { get; set; }

    /// <summary>
    /// Gets or sets the map which will be entered.
    /// </summary>
    [Required]
    public virtual GameMapDefinition? Map { get; set; }

    /// <summary>
    /// Gets or sets a value indicating whether this instance is a spawn gate.
    /// If it's not a spawn gate, it's a target of an <see cref="EnterGate"/>.
    /// </summary>
    /// <value>
    ///   <c>true</c> if this instance is spawn gate; otherwise, <c>false</c>.
    /// </value>
    public bool IsSpawnGate { get; set; }

    /// <inheritdoc />
    public override string ToString()
    {
        return $"{this.Map?.Name} @ {base.ToString()}";
    }
}