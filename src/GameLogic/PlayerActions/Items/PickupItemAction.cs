// <copyright file="PickupItemAction.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.GameLogic.PlayerActions.Items;

using MUnique.OpenMU.GameLogic.Views.Inventory;

/// <summary>
/// Action to pick up an item from the floor.
/// </summary>
public class PickupItemAction
{
    /// <summary>
    /// Pickups the item.
    /// </summary>
    /// <param name="player">The player.</param>
    /// <param name="dropId">The drop identifier.</param>
    public async ValueTask PickupItemAsync(Player player, ushort dropId)
    {
        var droppedLocateable = player.CurrentMap?.GetDrop(dropId);

        switch (droppedLocateable)
        {
            case DroppedMoney droppedMoney:
                if (!await this.TryPickupMoneyAsync(player, droppedMoney))
                {
                    await player.InvokeViewPlugInAsync<IItemPickUpFailedPlugIn>(p => p.ItemPickUpFailedAsync(ItemPickFailReason.General)).ConfigureAwait(false);
                }

                break;
            case DroppedItem droppedItem:
            {
                var (success, stackTarget) = await this.TryPickupItemAsync(player, droppedItem);
                if (success)
                {
                    if (stackTarget != null)
                    {
                        await player.InvokeViewPlugInAsync<IItemPickUpFailedPlugIn>(p => p.ItemPickUpFailedAsync(ItemPickFailReason.ItemStacked)).ConfigureAwait(false);
                        await player.InvokeViewPlugInAsync<IItemDurabilityChangedPlugIn>(p => p.ItemDurabilityChangedAsync(stackTarget, false)).ConfigureAwait(false);
                    }
                    else
                    {
                        await player.InvokeViewPlugInAsync<IItemAppearPlugIn>(p => p.ItemAppearAsync(droppedItem.Item)).ConfigureAwait(false);
                    }
                }
                else
                {
                    await player.InvokeViewPlugInAsync<IItemPickUpFailedPlugIn>(p => p.ItemPickUpFailedAsync(ItemPickFailReason.General)).ConfigureAwait(false);
                }

                break;
            }

            default:
                await player.InvokeViewPlugInAsync<IItemPickUpFailedPlugIn>(p => p.ItemPickUpFailedAsync(ItemPickFailReason.General)).ConfigureAwait(false);
                break;
        }

        if (droppedLocateable is not null)
        {
            await player.OnPickedUpItemAsync(droppedLocateable).ConfigureAwait(false);
        }
    }

    private bool CanPickup(Player player, ILocateable droppedLocateable)
    {
        if (!player.IsAlive)
        {
            return false;
        }

        var dist = (int)player.GetDistanceTo(droppedLocateable);
        if (dist > 3)
        {
            return false;
        }

        return true;
    }

    private async ValueTask<(bool Success, Item? StackTarget)> TryPickupItemAsync(Player player, DroppedItem droppedItem)
    {
        if (!this.CanPickup(player, droppedItem))
        {
            return (false, null);
        }

        var slot = player.Inventory?.CheckInvSpace(droppedItem.Item);
        if (slot < InventoryConstants.EquippableSlotsCount)
        {
            return (false, null);
        }

        return await droppedItem.TryPickUpByAsync(player);
    }

    private async ValueTask<bool> TryPickupMoneyAsync(Player player, DroppedMoney droppedMoney)
    {
        return this.CanPickup(player, droppedMoney) && await droppedMoney.TryPickUpByAsync(player);
    }
}