// <copyright file="CrywolfItemIdentifier.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.GameLogic.Crywolf;

using MUnique.OpenMU.DataModel.Configuration.Items;

/// <summary>
/// Identifies an item definition by its group and number, e.g. a jewel which drops less often while the crywolf fortress is occupied.
/// </summary>
public class CrywolfItemIdentifier
{
    /// <summary>
    /// Gets or sets the group of the item.
    /// </summary>
    public byte Group { get; set; }

    /// <summary>
    /// Gets or sets the number of the item.
    /// </summary>
    public short Number { get; set; }

    /// <summary>
    /// Determines whether the item identifier matches the specified item definition.
    /// </summary>
    /// <param name="definition">The item definition.</param>
    /// <returns><c>true</c>, if it matches; otherwise, <c>false</c>.</returns>
    public bool Matches(ItemDefinition definition)
    {
        return definition.Group == this.Group && definition.Number == this.Number;
    }

    /// <inheritdoc />
    public override string ToString()
    {
        return $"{this.Group},{this.Number}";
    }
}
