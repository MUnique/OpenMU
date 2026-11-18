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
    [Display(ResourceType = typeof(PlugInResources), Name = nameof(PlugInResources.InventoryUsedItemIdentifier_Group_Name), Description = nameof(PlugInResources.InventoryUsedItemIdentifier_Group_Description))]
    public byte Group { get; set; }

    /// <summary>
    /// Gets or sets the number of the item definition.
    /// </summary>
    [Display(ResourceType = typeof(PlugInResources), Name = nameof(PlugInResources.InventoryUsedItemIdentifier_Number_Name), Description = nameof(PlugInResources.InventoryUsedItemIdentifier_Number_Description))]
    public short Number { get; set; }

    /// <summary>
    /// Gets or sets the exclusive group of the item definition.
    /// Only one item of the same exclusive group can be in use at the same time,
    /// e.g. either the Hawk or the Goat Figurine. With 0, the item belongs to no group,
    /// so only another item of the same definition is excluded.
    /// </summary>
    [Display(ResourceType = typeof(PlugInResources), Name = nameof(PlugInResources.InventoryUsedItemIdentifier_ExclusiveGroup_Name), Description = nameof(PlugInResources.InventoryUsedItemIdentifier_ExclusiveGroup_Description))]
    public byte ExclusiveGroup { get; set; }

    /// <inheritdoc />
    public override string ToString()
    {
        return $"{this.Group}/{this.Number}";
    }
}
