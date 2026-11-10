// <copyright file="InventoryUsedItemExtensions.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.GameLogic;

using MUnique.OpenMU.DataModel.Entities;
using MUnique.OpenMU.GameLogic.Attributes;

/// <summary>
/// Extensions for the items which are used inside the inventory, see <see cref="InventoryItemUseConfiguration"/>.
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
    /// <param name="gameContext">The game context.</param>
    /// <returns><c>true</c>, if the item is used inside the inventory; otherwise, <c>false</c>.</returns>
    public static bool IsUsedInsideInventory(this Item item, IGameContext gameContext)
    {
        return item.Definition is { } definition
               && InventoryItemUseFeaturePlugIn.GetActiveConfiguration(gameContext) is { } configuration
               && configuration.Contains(definition);
    }

    /// <summary>
    /// Determines whether the item is used inside the inventory, and currently in use.
    /// </summary>
    /// <param name="item">The item.</param>
    /// <param name="gameContext">The game context.</param>
    /// <returns><c>true</c>, if the item is in use inside the inventory; otherwise, <c>false</c>.</returns>
    public static bool IsInUseInsideInventory(this Item item, IGameContext gameContext)
    {
        return item.HasInUseState() && item.IsUsedInsideInventory(gameContext);
    }

    /// <summary>
    /// Determines whether the item has the state of an item which is in use inside the inventory:
    /// it has the <see cref="InUseDurability"/> and it isn't equipped.
    /// </summary>
    /// <param name="item">The item.</param>
    /// <returns><c>true</c>, if the item has the state of an item which is in use inside the inventory; otherwise, <c>false</c>.</returns>
    internal static bool HasInUseState(this Item item)
    {
        return (int)item.Durability == InUseDurability
               && (item.ItemSlot < InventoryConstants.FirstEquippableItemSlotIndex || item.ItemSlot > InventoryConstants.LastEquippableItemSlotIndex);
    }

    /// <summary>
    /// Updates the power-ups of an item which is used inside the inventory:
    /// they're removed, and added again when the item is in use.
    /// </summary>
    /// <param name="attributes">The attribute system of the player.</param>
    /// <param name="item">The item.</param>
    /// <param name="gameContext">The game context.</param>
    internal static void UpdateInventoryUsedItemPowerUps(this ItemAwareAttributeSystem attributes, Item item, IGameContext gameContext)
    {
        attributes.RemoveInventoryUsedItemPowerUps(item);
        if (item.IsInUseInsideInventory(gameContext) && gameContext.ItemPowerUpFactory is { } factory)
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
