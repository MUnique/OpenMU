// <copyright file="IInventoryView.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.GameLogic.Views
{
    using MUnique.OpenMU.DataModel.Entities;

    /// <summary>
    /// The inventory view.
    /// </summary>
    public interface IInventoryView
    {
        /// <summary>
        /// An item got moved.
        /// </summary>
        /// <param name="item">The item.</param>
        /// <param name="toSlot">The new slot index.</param>
        /// <param name="storage">The new storage.</param>
        void ItemMoved(Item item, byte toSlot, Storages storage);

        /// <summary>
        /// Updates the money value.
        /// </summary>
        void UpdateMoney();

        /// <summary>
        /// Item has been upgraded.
        /// </summary>
        /// <param name="item">The item.</param>
        void ItemUpgraded(Item item);

        /// <summary>
        /// Updates the inventory list.
        /// </summary>
        void UpdateInventoryList();

        /// <summary>
        /// Notifies the client that an item got consumed (or not).
        /// </summary>
        /// <param name="inventorySlot">The inventory slot.</param>
        /// <param name="success">If set to <c>true</c> the item got consumed; otherwise not.</param>
        void ItemConsumed(byte inventorySlot, bool success);

        /// <summary>
        /// Notifies the client that the durability of the item changed.
        /// </summary>
        /// <param name="item">The item.</param>
        void ItemDurabilityChanged(Item item);

        /// <summary>
        /// Notifies the client that a new item appears in the inventory.
        /// </summary>
        /// <param name="newItem">The new item.</param>
        void ItemAppear(Item newItem);

        /// <summary>
        /// Notifies the client that the item could not be bought from the npc.
        /// </summary>
        void BuyNpcItemFailed();

        /// <summary>
        /// Notifies the client that the item got bought from the npc.
        /// </summary>
        /// <param name="newItem">The new item.</param>
        void NpcItemBought(Item newItem);

        /// <summary>
        /// Notifies the client that the item got sold to the npc (or not).
        /// </summary>
        /// <param name="success">If set to <c>true</c> the item has been sold; otherwise not.</param>
        void ItemSoldToNpc(bool success);

        /// <summary>
        /// Notifies the client that an item got dropped from the inventory (or not).
        /// </summary>
        /// <param name="slot">The slot from which the item has been dropped (or not).</param>
        /// <param name="success">If set to <c>true</c> the item has been dropped; otherwise not.</param>
        void ItemDropResult(byte slot, bool success);

        /// <summary>
        /// Notifies the client that his item from the player shop has been sold to another player.
        /// </summary>
        /// <param name="slot">The slot of the item.</param>
        /// <param name="buyer">The buyer.</param>
        void ItemSoldByPlayerShop(byte slot, Player buyer);

        /// <summary>
        /// Notifies the client that an item has been bought from another players shop.
        /// </summary>
        /// <param name="item">The item.</param>
        void ItemBoughtFromPlayerShop(Item item);
    }
}
