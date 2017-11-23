// <copyright file="BuyRequestAction.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.GameLogic.PlayerActions.PlayerStore
{
    using log4net;
    using MUnique.OpenMU.Interfaces;

    /// <summary>
    /// Action to buy an item from another player shop.
    /// </summary>
    public class BuyRequestAction
    {
        private static ILog log = LogManager.GetLogger(typeof(BuyRequestAction));

        /// <summary>
        /// Buys the item from another player shop.
        /// </summary>
        /// <param name="player">The player.</param>
        /// <param name="requestedPlayer">The requested player.</param>
        /// <param name="slot">The slot.</param>
        public void BuyItem(Player player, Player requestedPlayer, byte slot)
        {
            if (!requestedPlayer.ShopStorage.StoreOpen)
            {
                log.DebugFormat("Store not open, Character {0}", requestedPlayer.SelectedCharacter.Name);
                player.PlayerView.ShowMessage("Player's Store not open.", MessageType.BlueNormal);
                return;
            }

            if (slot < InventoryConstants.FirstStoreItemSlotIndex)
            {
                log.WarnFormat("Store Slot too low: {0}, possible hacker", slot);
                return;
            }

            var item = requestedPlayer.ShopStorage.GetItem(slot);
            if (item == null)
            {
                log.DebugFormat("Item unavailable, Slot {0}", slot);
                player.PlayerView.ShowMessage("Item unavailable.", MessageType.BlueNormal);
                return;
            }

            if (player.SelectedCharacter.Money < requestedPlayer.ShopStorage.StorePrices[slot - InventoryConstants.FirstStoreItemSlotIndex])
            {
                player.PlayerView.ShowMessage("Not enough Zen.", MessageType.BlueNormal);
                return;
            }

            // Check Inv Space
            int freeslot = player.Inventory.CheckInvSpace(item);
            if (freeslot == -1)
            {
                player.PlayerView.ShowMessage("Not enough Space in your Inventory.", MessageType.BlueNormal);
                return;
            }

            lock (requestedPlayer.ShopStorage.StoreLock)
            {
                item = requestedPlayer.ShopStorage.GetItem(slot);
                if (item == null)
                {
                    player.PlayerView.ShowMessage("Sorry, Item was sold in the meantime.", MessageType.BlueNormal);
                    return;
                }

                int price = (int)requestedPlayer.ShopStorage.StorePrices[slot - InventoryConstants.FirstStoreItemSlotIndex];

                // Remove Item, Add Money
                log.DebugFormat("BuyRequest, Item Price: {0}", price);
                player.ShopStorage.RemoveItem(item);
                requestedPlayer.TryAddMoney(price);
                requestedPlayer.PlayerView.InventoryView.UpdateMoney();
                requestedPlayer.PlayerView.InventoryView.ItemSoldByPlayerShop(slot, player);
                item.ItemSlot = (byte)freeslot;
                player.Inventory.AddItem(item.ItemSlot, item);

                // Add Item, Remove Money
                player.PlayerView.InventoryView.ItemBoughtFromPlayerShop(item);
                player.TryRemoveMoney(price);
                player.PlayerView.InventoryView.UpdateMoney();
            }
        }
    }
}
