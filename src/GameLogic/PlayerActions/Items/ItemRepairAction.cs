// <copyright file="ItemRepairAction.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.GameLogic.PlayerActions.Items
{
    using MUnique.OpenMU.DataModel.Entities;
    using MUnique.OpenMU.GameLogic.Views;
    using MUnique.OpenMU.GameLogic.Views.Inventory;
    using MUnique.OpenMU.Interfaces;

    /// <summary>
    /// Action to repair an item from the inventory.
    /// </summary>
    public class ItemRepairAction
    {
        private static readonly log4net.ILog Log = log4net.LogManager.GetLogger(typeof(ItemRepairAction));

        /// <summary>
        /// Repairs the item of the specified inventory slot.
        /// </summary>
        /// <param name="player">The player.</param>
        /// <param name="slot">The inventory slot.</param>
        public void RepairItem(Player player, byte slot)
        {
            if (slot == InventoryConstants.PetSlot && player.OpenedNpc == null)
            {
                Log.WarnFormat("Cheater Warning: Player tried to repair pet slot, without opened NPC. Character: [{0}], Account: [{1}]", player.SelectedCharacter.Name, player.Account.LoginName);
                return;
            }

            Item item = player.Inventory.GetItem(slot);
            if (item == null)
            {
                player.ViewPlugIns.GetPlugIn<IShowMessagePlugIn>()?.ShowMessage("No Item there to repair.", MessageType.BlueNormal);
                Log.WarnFormat("RepairItem: Player {0}, Itemslot {1} not filled", player.SelectedCharacter.Name, slot);
                return;
            }

            if (item.Durability == item.GetMaximumDurabilityOfOnePiece())
            {
                return;
            }

            if (IsMoneySufficient(player, item))
            {
                item.Durability = item.GetMaximumDurabilityOfOnePiece();
                player.ViewPlugIns.GetPlugIn<IItemDurabilityChangedPlugIn>()?.ItemDurabilityChanged(item, false);
            }
        }

        /// <summary>
        /// Repairs all equipped items.
        /// </summary>
        /// <param name="player">The player.</param>
        /// <remarks>
        /// The client calculates a sum based on all items in the inventory, even these which are not equipped.
        /// However, it should really just repair the equipped ones.
        /// </remarks>
        public void RepairAllItems(Player player)
        {
            if (player.OpenedNpc == null)
            {
                // probably cheater
                Log.WarnFormat("Cheater Warning: Player tried to repair all items, without opened NPC. Character: [{0}], Account: [{1}]", player.SelectedCharacter.Name, player.Account.LoginName);
            }

            // TODO: Check if NPC is able to repair all items. Maybe specified by npc dialog type
            for (byte i = InventoryConstants.FirstEquippableItemSlotIndex; i <= InventoryConstants.LastEquippableItemSlotIndex; i++)
            {
                if (i == InventoryConstants.PetSlot)
                {
                    // Pets are repaired due pet trainer
                    continue;
                }

                Item item = player.Inventory.GetItem(i);
                if (item == null)
                {
                    continue;
                }

                if (item.Durability == item.GetMaximumDurabilityOfOnePiece())
                {
                    continue;
                }

                if (IsMoneySufficient(player, item))
                {
                    item.Durability = item.GetMaximumDurabilityOfOnePiece();
                    player.ViewPlugIns.GetPlugIn<IItemDurabilityChangedPlugIn>()?.ItemDurabilityChanged(item, false);
                }
            }
        }

        private static bool IsMoneySufficient(Player player, Item item)
        {
            var priceCalculator = new ItemPriceCalculator();
            var price = priceCalculator.CalculateRepairPrice(item, player.OpenedNpc != null);
            return player.TryRemoveMoney((int)price);
        }
    }
}