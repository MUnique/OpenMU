// <copyright file="BuyNpcItemAction.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.GameLogic.PlayerActions.Items
{
    using System.Linq;
    using MUnique.OpenMU.DataModel.Entities;
    using MUnique.OpenMU.GameLogic.PlugIns;
    using MUnique.OpenMU.GameLogic.Views;
    using MUnique.OpenMU.GameLogic.Views.Inventory;
    using MUnique.OpenMU.Interfaces;

    /// <summary>
    /// Action to buy items from a Monster merchant.
    /// </summary>
    public class BuyNpcItemAction
    {
        private readonly ItemPriceCalculator priceCalculator;

        /// <summary>
        /// Initializes a new instance of the <see cref="BuyNpcItemAction"/> class.
        /// </summary>
        public BuyNpcItemAction()
        {
            this.priceCalculator = new ItemPriceCalculator();
        }

        /// <summary>
        /// Buys the item of the specified slot from the <see cref="Player.OpenedNpc"/> merchant store.
        /// </summary>
        /// <param name="player">The player who buys the item.</param>
        /// <param name="slot">The slot of the item.</param>
        public void BuyItem(Player player, byte slot)
        {
            if (player.OpenedNpc == null)
            {
                player.ViewPlugIns.GetPlugIn<IBuyNpcItemFailedPlugIn>()?.BuyNpcItemFailed();
                return;
            }

            var npcDefinition = player.OpenedNpc.Definition;
            if (npcDefinition.MerchantStore == null || npcDefinition.MerchantStore.Items.Count == 0)
            {
                player.ViewPlugIns.GetPlugIn<IBuyNpcItemFailedPlugIn>()?.BuyNpcItemFailed();
                return;
            }

            Item storeItem = npcDefinition.MerchantStore.Items.FirstOrDefault(i => i.ItemSlot == slot);
            if (storeItem == null)
            {
                player.ViewPlugIns.GetPlugIn<IShowMessagePlugIn>()?.ShowMessage("Item Unknown", MessageType.BlueNormal);
                player.ViewPlugIns.GetPlugIn<IBuyNpcItemFailedPlugIn>()?.BuyNpcItemFailed();
                return;
            }

            // Inventory Update:
            if (storeItem.IsStackable() && player.Inventory.Items.FirstOrDefault(item => storeItem.CanCompletelyStackOn(item)) is Item targetItem)
            {
                if (!this.CheckMoney(player, storeItem))
                {
                    return;
                }

                targetItem.Durability += storeItem.Durability;
                player.ViewPlugIns.GetPlugIn<IItemDurabilityChangedPlugIn>()?.ItemDurabilityChanged(targetItem, false);
                player.ViewPlugIns.GetPlugIn<IBuyNpcItemFailedPlugIn>()?.BuyNpcItemFailed();
            }
            else
            {
                int toSlot = player.Inventory.CheckInvSpace(storeItem);
                if (toSlot == -1)
                {
                    player.ViewPlugIns.GetPlugIn<IShowMessagePlugIn>()?.ShowMessage("Inventory Full", MessageType.BlueNormal);
                    player.ViewPlugIns.GetPlugIn<IBuyNpcItemFailedPlugIn>()?.BuyNpcItemFailed();
                    return;
                }

                if (!this.CheckMoney(player, storeItem))
                {
                    return;
                }

                var newItem = player.PersistenceContext.CreateNew<Item>();
                newItem.AssignValues(storeItem);
                newItem.ItemSlot = (byte)toSlot;
                player.ViewPlugIns.GetPlugIn<INpcItemBoughtPlugIn>()?.NpcItemBought(newItem);
                player.Inventory.AddItem(newItem);
                player.GameContext.PlugInManager.GetPlugInPoint<IItemBoughtFromMerchantPlugIn>()?.ItemBought(player, newItem, storeItem, player.OpenedNpc);
            }

            player.ViewPlugIns.GetPlugIn<IUpdateMoneyPlugIn>()?.UpdateMoney();
        }

        private bool CheckMoney(Player player, Item item)
        {
            var price = this.priceCalculator.CalculateBuyingPrice(item);
            if (!player.TryRemoveMoney((int)price))
            {
                player.ViewPlugIns.GetPlugIn<IShowMessagePlugIn>()?.ShowMessage("You don't have enough Money", MessageType.BlueNormal);
                player.ViewPlugIns.GetPlugIn<IBuyNpcItemFailedPlugIn>()?.BuyNpcItemFailed();
                return false;
            }

            return true;
        }
    }
}
