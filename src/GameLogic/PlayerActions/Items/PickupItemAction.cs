// <copyright file="PickupItemAction.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.GameLogic.PlayerActions.Items
{
    using MUnique.OpenMU.DataModel.Entities;
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
            var droppedItem = player.CurrentMap.GetDrop(dropId);
            if (droppedItem == null)
            {
                player.ViewPlugIns.GetPlugIn<IItemPickUpFailedPlugIn>()?.ItemPickUpFailed(ItemPickFailReason.General);
                return;
            }

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
        }

        private bool TryPickupItem(Player player, DroppedItem droppedItem, out Item stackTarget)
        {
            stackTarget = null;
            if (!player.Alive)
            {
                return false;
            }

            var dist = (int)player.GetDistanceTo(droppedItem);
            if (dist > 3)
            {
                return false;
            }

            int slot = player.Inventory.CheckInvSpace(droppedItem.Item);
            if (slot < InventoryConstants.EquippableSlotsCount)
            {
                return false;
            }

            return droppedItem.TryPickUpBy(player, out stackTarget);
        }
    }
}
