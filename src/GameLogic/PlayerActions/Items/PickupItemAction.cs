// <copyright file="PickupItemAction.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.GameLogic.PlayerActions.Items
{
    /// <summary>
    /// Action to pick up an item from the floor.
    /// </summary>
    public class PickupItemAction
    {
        private readonly IGameContext gameContext;

        /// <summary>
        /// Initializes a new instance of the <see cref="PickupItemAction"/> class.
        /// </summary>
        /// <param name="gameContext">The game context.</param>
        public PickupItemAction(IGameContext gameContext)
        {
            this.gameContext = gameContext;
        }

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
                return;
            }

            this.PickupItem(player, droppedItem);
        }

        private void PickupItem(Player player, DroppedItem droppedItem)
        {
            // Check the Distance Player <> Item
            double dist = player.GetDistanceTo(droppedItem);
            if (dist > 3)
            {
                return;
            }

            // Check Inv Space
            int slot = player.Inventory.CheckInvSpace(droppedItem.Item);
            if (slot < 12)
            {
                return;
            }

            var success = droppedItem.TryPickUpBy(player);
            if (success)
            {
                player.PlayerView.InventoryView.ItemAppear(droppedItem.Item);
            }

            // todo: "Obtained message" ?? guess not
        }
    }
}
