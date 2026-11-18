// <copyright file="InventoryItemUseConfiguration.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.GameLogic;

using MUnique.OpenMU.DataModel.Composition;
using MUnique.OpenMU.DataModel.Configuration.Items;

/// <summary>
/// The configuration of the items which are used inside the inventory, like the figurines and charms.
/// </summary>
public class InventoryItemUseConfiguration
{
    /// <summary>
    /// The default of <see cref="InUseDurability"/>, which is the value the game client expects.
    /// </summary>
    private const byte DefaultInUseDurability = 254;

    /// <summary>
    /// The default of <see cref="NotInUseDurability"/>, which is the value the game client expects.
    /// </summary>
    private const byte DefaultNotInUseDurability = 255;

    /// <summary>
    /// Gets or sets the items which are used inside the inventory instead of being equipped.
    /// </summary>
    /// <remarks>
    /// By default, these are the figurines (group 13, number 128 and 129), the charms (130 to 133)
    /// and the worn horseshoe (134), which the game client requests to use with a right click in the inventory.
    /// The figurines, the oak charms and the maple charms each form an exclusive group,
    /// see <see cref="InventoryUsedItemIdentifier.ExclusiveGroup"/>.
    /// </remarks>
    [MemberOfAggregate]
    [ScaffoldColumn(true)]
    [Display(ResourceType = typeof(PlugInResources), Name = nameof(PlugInResources.InventoryItemUseConfiguration_Items_Name), Description = nameof(PlugInResources.InventoryItemUseConfiguration_Items_Description))]
    public ICollection<InventoryUsedItemIdentifier> Items { get; set; } = CreateDefaultItems();

    /// <summary>
    /// Gets or sets the durability of an item which is in use.
    /// </summary>
    /// <remarks>
    /// Like in the game client, the durability of such an item tells whether it's in use,
    /// so this value has to match the client.
    /// </remarks>
    [Display(ResourceType = typeof(PlugInResources), Name = nameof(PlugInResources.InventoryItemUseConfiguration_InUseDurability_Name), Description = nameof(PlugInResources.InventoryItemUseConfiguration_InUseDurability_Description))]
    public byte InUseDurability { get; set; } = DefaultInUseDurability;

    /// <summary>
    /// Gets or sets the durability of an item which isn't in use.
    /// </summary>
    /// <remarks>
    /// Like in the game client, the durability of such an item tells whether it's in use,
    /// so this value has to match the client. A new item starts with it.
    /// </remarks>
    [Display(ResourceType = typeof(PlugInResources), Name = nameof(PlugInResources.InventoryItemUseConfiguration_NotInUseDurability_Name), Description = nameof(PlugInResources.InventoryItemUseConfiguration_NotInUseDurability_Description))]
    public byte NotInUseDurability { get; set; } = DefaultNotInUseDurability;

    /// <summary>
    /// Determines whether the item definition belongs to an item which is used inside the inventory.
    /// </summary>
    /// <param name="definition">The item definition.</param>
    /// <returns><c>true</c>, if items of this definition are used inside the inventory; otherwise, <c>false</c>.</returns>
    public bool Contains(ItemDefinition definition)
    {
        return this.Items.Any(id => id.Group == definition.Group && id.Number == definition.Number);
    }

    /// <summary>
    /// Determines whether items of the two definitions exclude each other, so that only one of them can be in use
    /// at the same time. That's the case for the same definition, and for definitions of the same exclusive group.
    /// </summary>
    /// <param name="definition">The item definition.</param>
    /// <param name="otherDefinition">The other item definition.</param>
    /// <returns><c>true</c>, if only one of the items can be in use at the same time; otherwise, <c>false</c>.</returns>
    public bool AreExclusive(ItemDefinition definition, ItemDefinition otherDefinition)
    {
        if (definition == otherDefinition)
        {
            return true;
        }

        var group = this.GetExclusiveGroup(definition);
        return group != 0 && group == this.GetExclusiveGroup(otherDefinition);
    }

    private static ICollection<InventoryUsedItemIdentifier> CreateDefaultItems()
    {
        const byte figurines = 1;
        const byte oakCharms = 2;
        const byte mapleCharms = 3;
        return
        [
            CreateItem(128, figurines), // Hawk Figurine
            CreateItem(129, figurines), // Goat Figurine
            CreateItem(130, oakCharms), // Oak Charm
            CreateItem(131, mapleCharms), // Maple Charm
            CreateItem(132, oakCharms), // Golden Oak Charm
            CreateItem(133, mapleCharms), // Golden Maple Charm
            CreateItem(134, 0), // Worn Horseshoe
        ];

        static InventoryUsedItemIdentifier CreateItem(short number, byte exclusiveGroup)
        {
            return new InventoryUsedItemIdentifier { Group = 13, Number = number, ExclusiveGroup = exclusiveGroup };
        }
    }

    private byte GetExclusiveGroup(ItemDefinition definition)
    {
        return this.Items.FirstOrDefault(id => id.Group == definition.Group && id.Number == definition.Number)?.ExclusiveGroup ?? 0;
    }
}
