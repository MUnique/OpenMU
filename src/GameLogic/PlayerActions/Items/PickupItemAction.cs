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
    public void PickupItem(Player player, ushort dropId)
    {
        var droppedLocateable = player.CurrentMap?.GetDrop(dropId);

        switch (droppedLocateable)
        {
            case DroppedMoney droppedMoney:
                if (!this.TryPickupMoney(player, droppedMoney))
                {
                    player.ViewPlugIns.GetPlugIn<IItemPickUpFailedPlugIn>()?.ItemPickUpFailed(ItemPickFailReason.General);
                }

                break;
            case DroppedItem droppedItem:
            {
                if (this.TryPickupItem(player, droppedItem, out var stackTarget))
                {
                    if (stackTarget != null)
                    {
                        player.ViewPlugIns.GetPlugIn<IItemPickUpFailedPlugIn>()?.ItemPickUpFailed(ItemPickFailReason.ItemStacked);
                        player.ViewPlugIns.GetPlugIn<IItemDurabilityChangedPlugIn>()?.ItemDurabilityChanged(stackTarget, false);
                    }
                    else
                    {
                        player.ViewPlugIns.GetPlugIn<IItemAppearPlugIn>()?.ItemAppear(droppedItem.Item);
                    }
                }
                else
                {
                    player.ViewPlugIns.GetPlugIn<IItemPickUpFailedPlugIn>()?.ItemPickUpFailed(ItemPickFailReason.General);
                }

                break;
            }

            default:
                player.ViewPlugIns.GetPlugIn<IItemPickUpFailedPlugIn>()?.ItemPickUpFailed(ItemPickFailReason.General);
                break;
        }

        if (droppedLocateable is not null)
        {
            player.OnPickedUpItem(droppedLocateable);
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

    private bool TryPickupItem(Player player, DroppedItem droppedItem, out Item? stackTarget)
    {
        stackTarget = null;
        if (!this.CanPickup(player, droppedItem))
        {
            return false;
        }

        var slot = player.Inventory?.CheckInvSpace(droppedItem.Item);
        if (slot < InventoryConstants.EquippableSlotsCount)
        {
            return false;
        }

        return droppedItem.TryPickUpBy(player, out stackTarget);
    }

    private bool TryPickupMoney(Player player, DroppedMoney droppedMoney)
    {
        return this.CanPickup(player, droppedMoney) && droppedMoney.TryPickUpBy(player);
    }
}