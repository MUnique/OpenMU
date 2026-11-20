// <copyright file="InventoryUsedItemExtensions.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.GameLogic;

using MUnique.OpenMU.DataModel.Entities;
using MUnique.OpenMU.GameLogic.Attributes;

/// <summary>
/// Extensions for the items which are used inside the inventory, see <see cref="InventoryItemUseConfiguration"/>.
/// Like in the game client, the durability of such an item tells whether it's in use,
/// see <see cref="InventoryItemUseConfiguration.InUseDurability"/> and <see cref="InventoryItemUseConfiguration.NotInUseDurability"/>.
/// While it's in use, its power-ups apply, although it isn't equipped.
/// </summary>
public static class InventoryUsedItemExtensions
{
    /// <summary>
    /// Determines whether the item is one which is used inside the inventory.
    /// </summary>
    /// <param name="item">The item.</param>
    /// <param name="gameContext">The game context.</param>
    /// <returns><c>true</c>, if the item is used inside the inventory; otherwise, <c>false</c>.</returns>
    public static bool IsUsedInsideInventory(this Item item, IGameContext gameContext)
    {
        return item.GetInventoryItemUseConfiguration(gameContext) is not null;
    }

    /// <summary>
    /// Determines whether the item is used inside the inventory, and currently in use.
    /// </summary>
    /// <param name="item">The item.</param>
    /// <param name="gameContext">The game context.</param>
    /// <returns><c>true</c>, if the item is in use inside the inventory; otherwise, <c>false</c>.</returns>
    public static bool IsInUseInsideInventory(this Item item, IGameContext gameContext)
    {
        return item.GetInventoryItemUseConfiguration(gameContext) is { } configuration
               && item.HasInUseState(configuration);
    }

    /// <summary>
    /// Gets the configuration of the items which are used inside the inventory, if the item is one of them.
    /// </summary>
    /// <param name="item">The item.</param>
    /// <param name="gameContext">The game context.</param>
    /// <returns>The configuration, if the item is used inside the inventory; otherwise, <c>null</c>.</returns>
    internal static InventoryItemUseConfiguration? GetInventoryItemUseConfiguration(this Item item, IGameContext gameContext)
    {
        if (item.Definition is { } definition
            && InventoryItemUseFeaturePlugIn.GetActiveConfiguration(gameContext) is { } configuration
            && configuration.Contains(definition))
        {
            return configuration;
        }

        return null;
    }

    /// <summary>
    /// Updates the power-ups of an item which is used inside the inventory:
    /// they're added when the item is in use and don't exist yet, and removed when it isn't in use.
    /// Existing power-ups are kept, because removing them could lower the current health, mana etc. of the player.
    /// </summary>
    /// <param name="attributes">The attribute system of the player.</param>
    /// <param name="item">The item.</param>
    /// <param name="gameContext">The game context.</param>
    internal static void UpdateInventoryUsedItemPowerUps(this ItemAwareAttributeSystem attributes, Item item, IGameContext gameContext)
    {
        if (!item.IsInUseInsideInventory(gameContext))
        {
            attributes.RemoveInventoryUsedItemPowerUps(item);
            return;
        }

        if (!attributes.ItemPowerUps.ContainsKey(item) && gameContext.ItemPowerUpFactory is { } factory)
        {
            attributes.ItemPowerUps.Add(item, factory.GetPowerUps(item, attributes, true).ToList());
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

    /// <summary>
    /// Determines whether the item has the state of an item which is in use inside the inventory:
    /// it has the <see cref="InventoryItemUseConfiguration.InUseDurability"/> and it isn't equipped.
    /// </summary>
    /// <param name="item">The item.</param>
    /// <param name="configuration">The configuration.</param>
    /// <returns><c>true</c>, if the item has the state of an item which is in use inside the inventory; otherwise, <c>false</c>.</returns>
    private static bool HasInUseState(this Item item, InventoryItemUseConfiguration configuration)
    {
        return (int)item.Durability == configuration.InUseDurability
               && (item.ItemSlot < InventoryConstants.FirstEquippableItemSlotIndex || item.ItemSlot > InventoryConstants.LastEquippableItemSlotIndex);
    }
}
