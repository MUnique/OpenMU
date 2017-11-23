// <copyright file="BuyNpcItemAction.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.GameLogic.PlayerActions.Items
{
    using System.Linq;
    using MUnique.OpenMU.DataModel.Entities;
    using MUnique.OpenMU.Interfaces;

    /// <summary>
    /// Action to buy items from a Monster merchant.
    /// </summary>
    public class BuyNpcItemAction
    {
        private readonly IGameContext gameContext;
        private readonly ItemPriceCalculator priceCalculator;

        /// <summary>
        /// Initializes a new instance of the <see cref="BuyNpcItemAction"/> class.
        /// </summary>
        /// <param name="gameContext">The game context.</param>
        public BuyNpcItemAction(IGameContext gameContext)
        {
            this.gameContext = gameContext;
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
                player.PlayerView.InventoryView.BuyNpcItemFailed();
                return;
            }

            var npcDefinition = player.OpenedNpc.Definition;
            if (npcDefinition.MerchantStore == null || npcDefinition.MerchantStore.Items.Count == 0)
            {
                player.PlayerView.InventoryView.BuyNpcItemFailed();
                return;
            }

            Item storeItem = npcDefinition.MerchantStore.Items.FirstOrDefault(i => i.ItemSlot == slot);
            if (storeItem == null)
            {
                player.PlayerView.ShowMessage("Item Unknown", MessageType.BlueNormal);
                player.PlayerView.InventoryView.BuyNpcItemFailed();
                return;
            }

            // Inventory Update:
            int toSlot = player.Inventory.CheckInvSpace(storeItem);
            if (toSlot == -1)
            {
                player.PlayerView.ShowMessage("Inventory Full", MessageType.BlueNormal);
                player.PlayerView.InventoryView.BuyNpcItemFailed();
                return;
            }

            var price = this.priceCalculator.CalculateSellingPrice(storeItem);
            if (!player.TryRemoveMoney((int)price))
            {
                player.PlayerView.ShowMessage("You don't have enough Money", MessageType.BlueNormal);
                player.PlayerView.InventoryView.BuyNpcItemFailed();
                return;
            }

            var newItem = new Item();
            newItem.AssignValues(storeItem);
            newItem.ItemSlot = (byte)toSlot;
            player.PlayerView.InventoryView.NpcItemBought(newItem);
            player.Inventory.AddItem(newItem.ItemSlot, newItem);
            player.PlayerView.InventoryView.UpdateMoney();
        }
    }
}
