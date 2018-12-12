// <copyright file="PickupItemAction.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.GameLogic.PlayerActions.Items
{
    using DataModel.Entities;
    using MUnique.OpenMU.GameLogic.Views;

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
                player.PlayerView.InventoryView.ItemPickUpFailed(ItemPickFailReason.General);
                return;
            }

            if (this.TryPickupItem(player, droppedItem, out var stackTarget))
            {
                if (stackTarget != null)
                {
                    player.PlayerView.InventoryView.ItemPickUpFailed(ItemPickFailReason.ItemStacked);
                    player.PlayerView.InventoryView.ItemDurabilityChanged(stackTarget, false);
                }
                else
                {
                    player.PlayerView.InventoryView.ItemAppear(droppedItem.Item);
                }
            }
            else
            {
                player.PlayerView.InventoryView.ItemPickUpFailed(ItemPickFailReason.General);
            }
        }

        private bool TryPickupItem(Player player, DroppedItem droppedItem, out Item stackTarget)
        {
            stackTarget = null;
            if (!player.Alive)
            {
                return false;
            }

            double dist = player.GetDistanceTo(droppedItem);
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
