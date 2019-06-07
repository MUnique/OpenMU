// <copyright file="IStorage.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.GameLogic
{
    using System;
    using System.Collections.Generic;

    using MUnique.OpenMU.DataModel.Entities;

    /// <summary>
    /// The types of item storages.
    /// </summary>
    public enum Storages
    {
        /// <summary>
        /// The inventory storage.
        /// </summary>
        Inventory = 0,

        /// <summary>
        /// The trade storage.
        /// </summary>
        Trade = 1,

        /// <summary>
        /// The vault storage.
        /// </summary>
        Vault = 2,

        /// <summary>
        /// The chaos machine storage.
        /// </summary>
        ChaosMachine = 3,

        /// <summary>
        /// The personal store storage.
        /// </summary>
        PersonalStore = 4,
    }

    /// <summary>
    /// An object which handles the storage with its logic.
    /// </summary>
    public interface IStorage
    {
        /// <summary>
        /// Gets the underlying item storage.
        /// </summary>
        ItemStorage ItemStorage { get; }

        /// <summary>
        /// Gets an enumeration of all items.
        /// </summary>
        IEnumerable<Item> Items { get; }

        /// <summary>
        /// Gets an enumeration of all free item slot indexes.
        /// </summary>
        IEnumerable<byte> FreeSlots { get; }

        /// <summary>
        /// Gets the extensions of an inventory.
        /// TODO: Use or remove it?
        ///       -> Use it. Current Storage implementation has an error... e.g. you could place a 2x2 item on the last row of the regular inventory.
        /// </summary>
        IEnumerable<IStorage> Extensions { get; }

        /// <summary>
        /// Adds the item to the storage.
        /// </summary>
        /// <param name="slot">The slot where the items should be put in.</param>
        /// <param name="item">The item.</param>
        /// <returns>True, if successful.</returns>
        bool AddItem(byte slot, Item item);

        /// <summary>
        /// Adds the item to the next free slot of the storage.
        /// </summary>
        /// <param name="item">The item.</param>
        /// <returns>True, if successful.</returns>
        bool AddItem(Item item);

        /// <summary>
        /// Tries to add money to itemStorage.
        /// </summary>
        /// <param name="value">The value which should be added.</param>
        /// <returns><c>True</c>, if the money can be add to itemStorage; Otherwise, <c>false</c>.</returns>
        bool TryAddMoney(int value);

        /// <summary>
        /// Tries to remove money from itemStorage.
        /// </summary>
        /// <param name="value">The value which should be added.</param>
        /// <returns><c>True</c>, if had enought money to be remove; Otherwise, <c>false</c>.</returns>
        bool TryRemoveMoney(int value);

        /// <summary>
        /// Returns the index of a slot in which the item would fit.
        /// </summary>
        /// <param name="item">The item.</param>
        /// <returns>The index of a slot in which the item would fit.</returns>
        int CheckInvSpace(Item item);

        /// <summary>
        /// Checks if the items of this Storage will fit into another storage, and adds them if possible.
        /// </summary>
        /// <param name="anotherStorage">The other storage.</param>
        /// <returns>If it was successful.</returns>
        /// <remarks>Helpful for the trade function where all items of the trade partner has to be added to the own inventory.</remarks>
        bool TryTakeAll(IStorage anotherStorage);

        /// <summary>
        /// Gets the item from the specified slot.
        /// </summary>
        /// <param name="inventorySlot">The inventory slot.</param>
        /// <returns>The item from the specified slot.</returns>
        Item GetItem(byte inventorySlot);

        /// <summary>
        /// Removes the item from this storage.
        /// </summary>
        /// <param name="item">The item which should be removed.</param>
        void RemoveItem(Item item);

        /// <summary>
        /// Clears this storage from all of its items.
        /// </summary>
        void Clear();
    }

    /// <summary>
    /// Interface for the inventory storage, which may have equipped items.
    /// </summary>
    /// <seealso cref="MUnique.OpenMU.GameLogic.IStorage" />
    public interface IInventoryStorage : IStorage
    {
        /// <summary>
        /// Occurs when the equipped items changed.
        /// </summary>
        event EventHandler<ItemEventArgs> EquippedItemsChanged;

        /// <summary>
        /// Gets all items which are in the wearable slots.
        /// </summary>
        IEnumerable<Item> EquippedItems { get; }
    }

    /// <summary>
    /// The interface for a player shop storage. A shop can be opened by a player, and other players can buy the items of this shop.
    /// </summary>
    public interface IShopStorage : IStorage
    {
        /// <summary>
        /// Gets or sets a value indicating whether the store is opened for other players.
        /// </summary>
        /// <value>
        ///   <c>true</c> if the store is opened for other players; otherwise, <c>false</c>.
        /// </value>
        bool StoreOpen { get; set; }

        /// <summary>
        /// Gets or sets the name of the store.
        /// </summary>
        string StoreName { get; set; }

        /// <summary>
        /// Gets the store lock.
        /// </summary>
        object StoreLock { get; }
    }
}
