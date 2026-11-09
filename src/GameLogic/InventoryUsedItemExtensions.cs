// <copyright file="InventoryUsedItemExtensions.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.GameLogic;

using MUnique.OpenMU.DataModel.Entities;
using MUnique.OpenMU.GameLogic.Attributes;

/// <summary>
/// Extensions for the items which are used inside the inventory, see <see cref="ItemConstants.ItemsUsedInsideInventory"/>.
/// Like in the game client, the durability of such an item tells whether it's in use:
/// <see cref="InUseDurability"/> when it is, <see cref="NotInUseDurability"/> when it isn't.
/// While it's in use, its power-ups apply, although it isn't equipped.
/// </summary>
public static class InventoryUsedItemExtensions
{
    /// <summary>
    /// The durability of an item which is in use. It's the value which the game client expects.
    /// </summary>
    internal const byte InUseDurability = 254;

    /// <summary>
    /// The durability of an item which isn't in use. It's the value which the game client expects.
    /// </summary>
    internal const byte NotInUseDurability = 255;

    /// <summary>
    /// Determines whether the item is one which is used inside the inventory.
    /// </summary>
    /// <param name="item">The item.</param>
    /// <returns><c>true</c>, if the item is used inside the inventory; otherwise, <c>false</c>.</returns>
    public static bool IsUsedInsideInventory(this Item item)
    {
        return item.Definition is { } definition
               && ItemConstants.ItemsUsedInsideInventory.Any(id => id.Group == definition.Group && id.Number == definition.Number);
    }

    /// <summary>
    /// Determines whether the item is used inside the inventory, and currently in use.
    /// </summary>
    /// <param name="item">The item.</param>
    /// <returns><c>true</c>, if the item is in use inside the inventory; otherwise, <c>false</c>.</returns>
    public static bool IsInUseInsideInventory(this Item item)
    {
        return (int)item.Durability == InUseDurability
               && (item.ItemSlot < InventoryConstants.FirstEquippableItemSlotIndex || item.ItemSlot > InventoryConstants.LastEquippableItemSlotIndex)
               && item.IsUsedInsideInventory();
    }

    /// <summary>
    /// Updates the power-ups of an item which is used inside the inventory:
    /// they're removed, and added again when the item is in use.
    /// </summary>
    /// <param name="attributes">The attribute system of the player.</param>
    /// <param name="item">The item.</param>
    /// <param name="factory">The item power up factory.</param>
    internal static void UpdateInventoryUsedItemPowerUps(this ItemAwareAttributeSystem attributes, Item item, IItemPowerUpFactory factory)
    {
        attributes.RemoveInventoryUsedItemPowerUps(item);
        if (item.IsInUseInsideInventory())
        {
            attributes.ItemPowerUps.Add(item, factory.GetPowerUps(item, attributes).ToList());
        }
    }

    /// <summary>
    /// Removes the power-ups of an item which is used inside the inventory, e.g. when it leaves the inventory.
    /// </summary>
    /// <param name="attributes">The attribute system of the player.</param>
    /// <param name="item">The item.</param>
    internal static void RemoveInventoryUsedItemPowerUps(this ItemAwareAttributeSystem attributes, Item item)
    {
        if (attributes.ItemPowerUps.Remove(item, out var powerUps))
        {
            foreach (var powerUp in powerUps)
            {
                powerUp.Dispose();
            }
        }
    }
}
