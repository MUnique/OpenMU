// <copyright file="InventoryItemUseConfiguration.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.GameLogic;

using MUnique.OpenMU.DataModel.Configuration.Items;

/// <summary>
/// The configuration of the items which are used inside the inventory, like the figurines and charms.
/// </summary>
public class InventoryItemUseConfiguration
{
    /// <summary>
    /// Gets or sets the items which are used inside the inventory instead of being equipped.
    /// </summary>
    /// <remarks>
    /// By default, these are the figurines (group 13, number 128 and 129), the charms (130 to 133)
    /// and the worn horseshoe (134), which the game client requests to use with a right click in the inventory.
    /// </remarks>
    public IList<InventoryUsedItemIdentifier> Items { get; set; } = CreateDefaultItems();

    /// <summary>
    /// Determines whether the item definition belongs to an item which is used inside the inventory.
    /// </summary>
    /// <param name="definition">The item definition.</param>
    /// <returns><c>true</c>, if items of this definition are used inside the inventory; otherwise, <c>false</c>.</returns>
    public bool Contains(ItemDefinition definition)
    {
        return this.Items.Any(id => id.Group == definition.Group && id.Number == definition.Number);
    }

    private static IList<InventoryUsedItemIdentifier> CreateDefaultItems()
    {
        return Enumerable.Range(128, 7)
            .Select(number => new InventoryUsedItemIdentifier { Group = 13, Number = (short)number })
            .ToList();
    }
}
