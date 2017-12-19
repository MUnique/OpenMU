// <copyright file="DropItemAction.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.GameLogic.PlayerActions.Items
{
    using MUnique.OpenMU.DataModel.Entities;

    /// <summary>
    /// Action to drop an item from the inventory to the floor.
    /// </summary>
    public class DropItemAction
    {
        /// <summary>
        /// Drops the item.
        /// </summary>
        /// <param name="player">The player.</param>
        /// <param name="slot">The slot.</param>
        /// <param name="x">The x coordinate of the point where the item should be dropped.</param>
        /// <param name="y">The y coordinate of the point where the item should be dropped.</param>
        public void DropItem(Player player, byte slot, byte x, byte y)
        {
            Item item = player.Inventory.GetItem(slot);

            if (player.CurrentMap.Terrain.WalkMap[x, y] && item != null)
            {
                this.DropItem(player, item, x, y);
            }
            else
            {
                player.PlayerView.InventoryView.ItemDropResult(slot, false);
            }
        }

        private void DropItem(Player player, Item item, byte x, byte y)
        {
            var droppedItem = new DroppedItem(item, x, y, player.CurrentMap, player);
            player.CurrentMap.Add(droppedItem);
            player.Inventory.RemoveItem(item);
            player.PlayerView.InventoryView.ItemDropResult(item.ItemSlot, true);
            player.PersistenceContext.Detach(item);
        }
    }
}
