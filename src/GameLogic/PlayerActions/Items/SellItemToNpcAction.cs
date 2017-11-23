// <copyright file="SellItemToNpcAction.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.GameLogic.PlayerActions.Items
{
    using MUnique.OpenMU.DataModel.Entities;
    using MUnique.OpenMU.GameLogic;

    /// <summary>
    /// Action to sell an item to a npc merchant.
    /// </summary>
    public class SellItemToNpcAction
    {
        private readonly ItemPriceCalculator itemPriceCalculator;

        /// <summary>
        /// Initializes a new instance of the <see cref="SellItemToNpcAction"/> class.
        /// </summary>
        public SellItemToNpcAction()
        {
             this.itemPriceCalculator = new ItemPriceCalculator(); // TODO: DI? Calculator into gameContext?
        }

        /// <summary>
        /// Sells the item of the specified slot to the npc merchant.
        /// </summary>
        /// <param name="player">The player.</param>
        /// <param name="slot">The slot.</param>
        public void SellItem(Player player, byte slot)
        {
            Item item = player.Inventory.GetItem(slot);
            if (item == null)
            {
                player.PlayerView.InventoryView.ItemSoldToNpc(false);
                return;
            }

            this.SellItem(player, item);
        }

        private void SellItem(Player player, Item item)
        {
            if (player.TryAddMoney(this.GetNPCPrice(item)))
            {
                player.Inventory.RemoveItem(item);
                player.PlayerView.InventoryView.ItemSoldToNpc(true);
            }
        }

        private int GetNPCPrice(Item item)
        {
            return (int)this.itemPriceCalculator.CalculateSellingPrice(item);
        }
    }
}
