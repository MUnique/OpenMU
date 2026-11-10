// <copyright file="UseInventoryItemAction.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.GameLogic.PlayerActions.Items;

using MUnique.OpenMU.GameLogic.Views.Inventory;

/// <summary>
/// The action to start or stop using an item inside the inventory, like a figurine or a charm,
/// see <see cref="InventoryItemUseConfiguration"/>.
/// While such an item is in use, its power-ups apply.
/// </summary>
public class UseInventoryItemAction
{
    /// <summary>
    /// Starts or stops using the item of the inventory slot.
    /// </summary>
    /// <param name="player">The player.</param>
    /// <param name="slot">The inventory slot of the item.</param>
    /// <param name="requestedDurability">The requested durability, which tells whether the item should be in use,
    /// see <see cref="InventoryUsedItemExtensions"/>.</param>
    public async ValueTask SetItemInUseAsync(Player player, byte slot, byte requestedDurability)
    {
        if (player.PlayerState.CurrentState != PlayerState.EnteredWorld
            || player.Inventory?.GetItem(slot) is not { } item
            || slot <= InventoryConstants.LastEquippableItemSlotIndex
            || !item.IsUsedInsideInventory(player.GameContext)
            || player.Attributes is not { } attributes)
        {
            return;
        }

        if (requestedDurability is not (InventoryUsedItemExtensions.InUseDurability or InventoryUsedItemExtensions.NotInUseDurability))
        {
            player.Logger.LogWarning("Player {player} requested durability {requestedDurability} for {item}, which isn't possible.", player, requestedDurability, item);
            await player.InvokeViewPlugInAsync<IInventoryItemUseChangedPlugIn>(p => p.InventoryItemUseChangedAsync(item)).ConfigureAwait(false);
            return;
        }

        if (requestedDurability == InventoryUsedItemExtensions.InUseDurability
            && player.Inventory.Items.Any(other => other != item && other.Definition == item.Definition && other.IsInUseInsideInventory(player.GameContext)))
        {
            await player.ShowLocalizedBlueMessageAsync(nameof(PlayerMessage.SameItemAlreadyInUse)).ConfigureAwait(false);
            await player.InvokeViewPlugInAsync<IInventoryItemUseChangedPlugIn>(p => p.InventoryItemUseChangedAsync(item)).ConfigureAwait(false);
            return;
        }

        item.Durability = requestedDurability;
        attributes.UpdateInventoryUsedItemPowerUps(item, player.GameContext);
        await player.InvokeViewPlugInAsync<IInventoryItemUseChangedPlugIn>(p => p.InventoryItemUseChangedAsync(item)).ConfigureAwait(false);
    }
}
