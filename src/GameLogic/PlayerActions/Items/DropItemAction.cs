// <copyright file="DropItemAction.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.GameLogic.PlayerActions.Items
{
    using MUnique.OpenMU.DataModel.Entities;
    using MUnique.OpenMU.GameLogic.Views.Inventory;
    using MUnique.OpenMU.Pathfinding;

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
        /// <param name="target">The target coordinates.</param>
        public void DropItem(Player player, byte slot, Point target)
        {
            Item item = player.Inventory.GetItem(slot);

            if (player.CurrentMap.Terrain.WalkMap[target.X, target.Y] && item != null)
            {
                this.DropItem(player, item, target);
            }
            else
            {
                player.ViewPlugIns.GetPlugIn<IItemDropResultPlugIn>()?.ItemDropResult(slot, false);
            }
        }

        private void DropItem(Player player, Item item, Point target)
        {
            var droppedItem = new DroppedItem(item, target, player.CurrentMap, player);
            player.CurrentMap.Add(droppedItem);
            player.Inventory.RemoveItem(item);
            player.ViewPlugIns.GetPlugIn<IItemDropResultPlugIn>()?.ItemDropResult(item.ItemSlot, true);
        }
    }
}
