// <copyright file="BuyRequestAction.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.GameLogic.PlayerActions.PlayerStore
{
    using log4net;
    using MUnique.OpenMU.GameLogic.PlugIns;
    using MUnique.OpenMU.GameLogic.Views;
    using MUnique.OpenMU.GameLogic.Views.Inventory;
    using MUnique.OpenMU.Interfaces;

    /// <summary>
    /// Action to buy an item from another player shop.
    /// </summary>
    public class BuyRequestAction
    {
        private static readonly ILog Log = LogManager.GetLogger(typeof(BuyRequestAction));

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
                Log.DebugFormat("Store not open, Character {0}", requestedPlayer.SelectedCharacter.Name);
                player.ViewPlugIns.GetPlugIn<IShowMessagePlugIn>()?.ShowMessage("Player's Store not open.", MessageType.BlueNormal);
                return;
            }

            if (slot < InventoryConstants.FirstStoreItemSlotIndex)
            {
                Log.WarnFormat("Store Slot too low: {0}, possible hacker", slot);
                return;
            }

            var item = requestedPlayer.ShopStorage.GetItem(slot);
            if (item == null || !item.StorePrice.HasValue)
            {
                Log.DebugFormat("Item unavailable, Slot {0}", slot);
                player.ViewPlugIns.GetPlugIn<IShowMessagePlugIn>()?.ShowMessage("Item unavailable.", MessageType.BlueNormal);
                return;
            }

            var itemPrice = item.StorePrice.Value;

            if (player.Money < itemPrice)
            {
                player.ViewPlugIns.GetPlugIn<IShowMessagePlugIn>()?.ShowMessage("Not enough Zen.", MessageType.BlueNormal);
                return;
            }

            // Check Inv Space
            int freeslot = player.Inventory.CheckInvSpace(item);
            if (freeslot == -1)
            {
                player.ViewPlugIns.GetPlugIn<IShowMessagePlugIn>()?.ShowMessage("Not enough Space in your Inventory.", MessageType.BlueNormal);
                return;
            }

            lock (requestedPlayer.ShopStorage.StoreLock)
            {
                item = requestedPlayer.ShopStorage.GetItem(slot);
                if (item == null)
                {
                    player.ViewPlugIns.GetPlugIn<IShowMessagePlugIn>()?.ShowMessage("Sorry, Item was sold in the meantime.", MessageType.BlueNormal);
                    return;
                }

                Log.DebugFormat("BuyRequest, Item Price: {0}", itemPrice);
                if (player.TryRemoveMoney(itemPrice))
                {
                    if (requestedPlayer.TryAddMoney(itemPrice))
                    {
                        requestedPlayer.ShopStorage.RemoveItem(item);

                        requestedPlayer.ViewPlugIns.GetPlugIn<IUpdateMoneyPlugIn>()?.UpdateMoney();
                        requestedPlayer.ViewPlugIns.GetPlugIn<IItemSoldByPlayerShopPlugIn>()?.ItemSoldByPlayerShop(slot, player);
                        item.ItemSlot = (byte)freeslot;
                        item.StorePrice = null;
                        player.Inventory.AddItem(item.ItemSlot, item);

                        player.ViewPlugIns.GetPlugIn<IItemBoughtFromPlayerShopPlugIn>()?.ItemBoughtFromPlayerShop(item);
                        player.ViewPlugIns.GetPlugIn<IUpdateMoneyPlugIn>()?.UpdateMoney();
                        player.GameContext.PlugInManager.GetPlugInPoint<IItemSoldToOtherPlayerPlugIn>()?.ItemSold(requestedPlayer, item, player);
                    }
                    else
                    {
                        player.ViewPlugIns.GetPlugIn<IShowMessagePlugIn>()?.ShowMessage("The inventory of the seller is full.", MessageType.BlueNormal);
                        player.TryAddMoney(itemPrice);
                    }
                }
            }
        }
    }
}
