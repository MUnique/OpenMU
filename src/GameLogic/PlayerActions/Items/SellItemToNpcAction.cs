// <copyright file="SellItemToNpcAction.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.GameLogic.PlayerActions.Items
{
    using log4net;
    using MUnique.OpenMU.DataModel.Entities;
    using MUnique.OpenMU.GameLogic;
    using MUnique.OpenMU.GameLogic.PlugIns;
    using MUnique.OpenMU.GameLogic.Views.Inventory;

    /// <summary>
    /// Action to sell an item to a npc merchant.
    /// </summary>
    public class SellItemToNpcAction
    {
        /// <summary>
        /// The logger of this class.
        /// </summary>
        private static readonly ILog Log = LogManager.GetLogger(typeof(SellItemToNpcAction));

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
                Log.WarnFormat("Player {0} requested to sell item at slot {1}, but item wasn't found.", player, slot);
                player.ViewPlugIns.GetPlugIn<IItemSoldToNpcPlugIn>()?.ItemSoldToNpc(false);
                return;
            }

            if (player.OpenedNpc?.Definition.MerchantStore == null)
            {
                Log.WarnFormat("Player {0} requested to sell item at slot {1} to an npc, but no npc merchant store is currently opened.", player, slot);
                player.ViewPlugIns.GetPlugIn<IItemSoldToNpcPlugIn>()?.ItemSoldToNpc(false);
                return;
            }

            this.SellItem(player, item);
        }

        private void SellItem(Player player, Item item)
        {
            var sellingPrice = (int)this.itemPriceCalculator.CalculateSellingPrice(item);
            Log.DebugFormat("Calculated selling price {0} for item {1}", sellingPrice, item);
            if (player.TryAddMoney(sellingPrice))
            {
                Log.DebugFormat("Sold Item {0} for price: {1}", item, sellingPrice);
                player.Inventory.RemoveItem(item);
                player.ViewPlugIns.GetPlugIn<IItemSoldToNpcPlugIn>()?.ItemSoldToNpc(true);

                player.GameContext.PlugInManager.GetPlugInPoint<IItemSoldToMerchantPlugIn>()?.ItemSold(player, item, player.OpenedNpc);
            }
        }
    }
}
