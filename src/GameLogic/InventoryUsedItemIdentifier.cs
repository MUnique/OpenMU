// <copyright file="InventoryUsedItemIdentifier.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.GameLogic;

/// <summary>
/// Identifies an item definition which is used inside the inventory.
/// </summary>
public class InventoryUsedItemIdentifier
{
    /// <summary>
    /// Gets or sets the group of the item definition.
    /// </summary>
    public byte Group { get; set; }

    /// <summary>
    /// Gets or sets the number of the item definition.
    /// </summary>
    public short Number { get; set; }

    /// <inheritdoc />
    public override string ToString()
    {
        return $"{this.Group}/{this.Number}";
    }
}
