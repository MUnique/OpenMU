// <copyright file="BuyRequestAction.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.GameLogic.PlayerActions.PlayerStore
{
    using System.Linq;
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
        private readonly CloseStoreAction closeStoreAction = new CloseStoreAction();

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
                player.ViewPlugIns.GetPlugIn<IShowMessagePlugIn>()?.ShowMessage("Player's Store not open.", MessageType.BlueNormal); // Code: 3
                return;
            }

            if (slot < InventoryConstants.FirstStoreItemSlotIndex)
            {
                Log.WarnFormat("Store Slot too low: {0}, possible hacker", slot);
                return;
            }

            var item = requestedPlayer.ShopStorage.GetItem(slot);
            if (item?.StorePrice == null)
            {
                Log.DebugFormat("Item unavailable, Slot {0}", slot);
                player.ViewPlugIns.GetPlugIn<IShowMessagePlugIn>()?.ShowMessage("Item unavailable.", MessageType.BlueNormal); // Code 5?
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

            bool itemSold = false;
            lock (requestedPlayer.ShopStorage.StoreLock)
            {
                if (!requestedPlayer.ShopStorage.StoreOpen)
                {
                    Log.DebugFormat("Store not open anymore, Character {0}", requestedPlayer.SelectedCharacter.Name);
                    player.ViewPlugIns.GetPlugIn<IShowMessagePlugIn>()?.ShowMessage("Player's Store not open anymore.", MessageType.BlueNormal);
                    return;
                }

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
                        using (var itemContext = requestedPlayer.GameContext.PersistenceContextProvider.CreateNewTradeContext())
                        {
                            itemContext.Attach(item);
                            requestedPlayer.ShopStorage.RemoveItem(item);
                            requestedPlayer.ViewPlugIns.GetPlugIn<IUpdateMoneyPlugIn>()?.UpdateMoney();
                            requestedPlayer.ViewPlugIns.GetPlugIn<IItemSoldByPlayerShopPlugIn>()?.ItemSoldByPlayerShop(slot, player);
                            requestedPlayer.ViewPlugIns.GetPlugIn<Views.Inventory.IItemRemovedPlugIn>()?.RemoveItem(slot);
                            item.ItemSlot = (byte)freeslot;
                            item.StorePrice = null;
                            player.Inventory.AddItem(item);
                            requestedPlayer.PersistenceContext.Detach(item);
                            itemContext.SaveChanges();
                            player.PersistenceContext.Attach(item);
                            player.ViewPlugIns.GetPlugIn<IItemBoughtFromPlayerShopPlugIn>()?.ItemBoughtFromPlayerShop(item);
                            player.ViewPlugIns.GetPlugIn<IUpdateMoneyPlugIn>()?.UpdateMoney();
                            itemSold = true;

                            player.GameContext.PlugInManager.GetPlugInPoint<IItemSoldToOtherPlayerPlugIn>()?.ItemSold(requestedPlayer, item, player);
                        }
                    }
                    else
                    {
                        player.ViewPlugIns.GetPlugIn<IShowMessagePlugIn>()?.ShowMessage("The inventory of the seller is full.", MessageType.BlueNormal);
                        player.TryAddMoney(itemPrice);
                    }
                }
            }

            if (itemSold)
            {
                if (requestedPlayer.ShopStorage.Items.Any())
                {
                    // this update may be sent to other players as well which are currently looking at the store
                    player.ViewPlugIns.GetPlugIn<Views.PlayerShop.IShowShopItemListPlugIn>()?.ShowShopItemList(requestedPlayer, true);
                }
                else
                {
                    this.closeStoreAction.CloseStore(requestedPlayer);
                }
            }
        }
    }
}
