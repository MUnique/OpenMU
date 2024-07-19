// <copyright file="ItemRepairAction.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.GameLogic.PlayerActions.Items;

using MUnique.OpenMU.GameLogic.Views;
using MUnique.OpenMU.GameLogic.Views.Inventory;
using MUnique.OpenMU.Interfaces;

/// <summary>
/// Action to repair an item from the inventory.
/// </summary>
public class ItemRepairAction
{
    /// <summary>
    /// Repairs the item of the specified inventory slot.
    /// </summary>
    /// <param name="player">The player.</param>
    /// <param name="slot">The inventory slot.</param>
    public async ValueTask RepairItemAsync(Player player, byte slot)
    {
        using var loggerScope = player.Logger.BeginScope(this.GetType());
        if (slot == InventoryConstants.PetSlot && player.OpenedNpc is null)
        {
            player.Logger.LogWarning("Cheater Warning: Player tried to repair pet slot, without opened NPC. Character: [{0}], Account: [{1}]", player.SelectedCharacter?.Name, player.Account?.LoginName);
            return;
        }

        var item = player.Inventory?.GetItem(slot);
        if (item is null)
        {
            await player.InvokeViewPlugInAsync<IShowMessagePlugIn>(p => p.ShowMessageAsync("No Item there to repair.", MessageType.BlueNormal)).ConfigureAwait(false);
            player.Logger.LogWarning("RepairItem: Player {0}, Itemslot {1} not filled", player.SelectedCharacter?.Name, slot);
            return;
        }

        if ((byte)item.Durability == item.GetMaximumDurabilityOfOnePiece())
        {
            return;
        }

        if (IsMoneySufficient(player, item))
        {
            item.Durability = item.GetMaximumDurabilityOfOnePiece();
            await player.InvokeViewPlugInAsync<IItemDurabilityChangedPlugIn>(p => p.ItemDurabilityChangedAsync(item, false)).ConfigureAwait(false);
        }
        else
        {
            await player.InvokeViewPlugInAsync<IShowMessagePlugIn>(p => p.ShowMessageAsync("You don't have enough money to repair.", MessageType.BlueNormal)).ConfigureAwait(false);
        }
    }

    /// <summary>
    /// Repairs all equipped items.
    /// </summary>
    /// <param name="player">The player.</param>
    /// <remarks>
    /// The client calculates a sum based on all items in the inventory, even these which are not equipped.
    /// However, it should really just repair the equipped ones.
    /// </remarks>
    public async ValueTask RepairAllItemsAsync(Player player)
    {
        if (player.OpenedNpc is null)
        {
            // probably cheater
            player.Logger.LogWarning("Cheater Warning: Player tried to repair all items, without opened NPC. Character: [{0}], Account: [{1}]", player.SelectedCharacter?.Name, player.Account?.LoginName);
        }

        // TODO: Check if NPC is able to repair all items. Maybe specified by npc dialog type
        for (byte i = InventoryConstants.FirstEquippableItemSlotIndex; i <= InventoryConstants.LastEquippableItemSlotIndex; i++)
        {
            if (i == InventoryConstants.PetSlot)
            {
                // Pets are repaired due pet trainer
                continue;
            }

            var item = player.Inventory?.GetItem(i);
            if (item is null)
            {
                continue;
            }

            if ((int)item.Durability == item.GetMaximumDurabilityOfOnePiece())
            {
                continue;
            }

            if (IsMoneySufficient(player, item))
            {
                item.Durability = item.GetMaximumDurabilityOfOnePiece();
                await player.InvokeViewPlugInAsync<IItemDurabilityChangedPlugIn>(p => p.ItemDurabilityChangedAsync(item, false)).ConfigureAwait(false);
            }
            else
            {
                await player.InvokeViewPlugInAsync<IShowMessagePlugIn>(p => p.ShowMessageAsync("You don't have enough money to repair.", MessageType.BlueNormal)).ConfigureAwait(false);
                break;
            }
        }
    }

    private static bool IsMoneySufficient(Player player, Item item)
    {
        var priceCalculator = new ItemPriceCalculator();
        var price = priceCalculator.CalculateRepairPrice(item, player.OpenedNpc != null);
        return player.TryRemoveMoney((int)price);
    }
}