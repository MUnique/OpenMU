// <copyright file="IStorage.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.GameLogic;

using MUnique.OpenMU.DataModel.Configuration.Items;
using MUnique.OpenMU.PlugIns;
using Nito.AsyncEx;

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
    /// </summary>
    IEnumerable<IStorage> Extensions { get; }

    /// <summary>
    /// Adds the item to the storage or its <see cref="Extensions"/>.
    /// </summary>
    /// <param name="slot">The slot where the items should be put in.</param>
    /// <param name="item">The item.</param>
    /// <returns>True, if successful.</returns>
    ValueTask<bool> AddItemAsync(byte slot, Item item);

    /// <summary>
    /// Adds the item to the next free slot of the storage or its <see cref="Extensions"/>.
    /// </summary>
    /// <param name="item">The item.</param>
    /// <returns>True, if successful.</returns>
    ValueTask<bool> AddItemAsync(Item item);

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
    byte? CheckInvSpace(Item item);

    /// <summary>
    /// Determines whether an item of the specified definition would find a place in this storage (including
    /// extensions) after the specified items are removed from it.
    /// </summary>
    /// <param name="definition">The item definition.</param>
    /// <param name="removedItems">The items of this storage which are removed before; others are ignored.</param>
    /// <returns><c>True</c>, if an item of the definition would fit; Otherwise, <c>false</c>.</returns>
    /// <remarks>
    /// By default, e.g. for storages of plug-in assemblies, it only checks whether the item fits now, without the
    /// cells which the removed items free. That may refuse an item which would fit, but never accepts one which
    /// finds no place. <see cref="Storage"/> also counts the freed cells.
    /// </remarks>
    bool FitsAfterRemoval(ItemDefinition definition, ICollection<Item> removedItems) => this.CheckInvSpace(new TemporaryItem { Definition = definition }) is not null;

    /// <summary>
    /// Determines whether all the items would fit into this storage (including extensions),
    /// after the other items were removed, without changing the storage.
    /// </summary>
    /// <param name="items">The items which would be added, in this order, each at the slot which <see cref="CheckInvSpace"/> would find.</param>
    /// <param name="removedItems">The items of this storage which would be removed before.</param>
    /// <returns><c>true</c>, if all the items would fit; otherwise, <c>false</c>, also when an item has no definition.</returns>
    /// <remarks>
    /// By default, e.g. for storages of plug-in assemblies, a single item is checked with <see cref="FitsAfterRemoval"/>,
    /// and more than one item are refused, because it isn't known whether they fit together. That may refuse items
    /// which would fit, but never accepts one which finds no place. <see cref="Storage"/> places them together.
    /// </remarks>
    bool CanAddItems(IEnumerable<Item> items, ICollection<Item>? removedItems = null)
    {
        var firstItems = items.Take(2).ToList();
        return firstItems.Count switch
        {
            0 => true,
            1 => firstItems[0].Definition is { } definition && this.FitsAfterRemoval(definition, removedItems ?? []),
            _ => false,
        };
    }

    /// <summary>
    /// Checks if the items of another storage will fit into this storage (including extensions), and adds them if possible.
    /// </summary>
    /// <param name="anotherStorage">The other storage.</param>
    /// <returns>If it was successful.</returns>
    /// <remarks>Helpful for the trade function where all items of the trade partner has to be added to the own inventory.</remarks>
    ValueTask<bool> TryTakeAllAsync(IStorage anotherStorage);

    /// <summary>
    /// Gets the item from the specified slot.
    /// </summary>
    /// <param name="inventorySlot">The inventory slot.</param>
    /// <returns>The item from the specified slot.</returns>
    Item? GetItem(byte inventorySlot);

    /// <summary>
    /// Finds items that matches the given definition.
    /// </summary>
    /// <param name="definition">The item definition to be searched.</param>
    /// <returns>The items with the same definition.</returns>
    IEnumerable<Item> FindItemsByDefinition(ItemDefinition definition);

    /// <summary>
    /// Removes the item from this storage.
    /// </summary>
    /// <param name="item">The item which should be removed.</param>
    ValueTask RemoveItemAsync(Item item);

    /// <summary>
    /// Clears this storage from all of its items.
    /// </summary>
    void Clear();

    /// <summary>
    /// Determines whether the slot belongs to this storage, or not.
    /// Extensions are not considered here, so that this function can be used to determine to which extension
    /// a slot belongs to.
    /// </summary>
    /// <param name="slot">The slot.</param>
    /// <returns>
    ///   <c>true</c> if the slot belongs to this storage; otherwise, <c>false</c>.
    /// </returns>
    bool ContainsSlot(byte slot);
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
    event AsyncEventHandler<ItemEventArgs> EquippedItemsChanged;

    /// <summary>
    /// Gets all items which are in the wearable slots.
    /// </summary>
    IEnumerable<Item> EquippedItems { get; }

    /// <summary>
    /// Gets equipped ammunition item.
    /// </summary>
    Item? EquippedAmmunitionItem { get; }

    /// <summary>
    /// Gets this instance as <see cref="InventoryStorage"/>.
    /// </summary>
    /// <returns>The inventory as <see cref="InventoryStorage"/>.</returns>
    InventoryStorage? AsInventoryStorage => this as InventoryStorage;
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
    /// Gets the store lock.
    /// </summary>
    AsyncLock StoreLock { get; }
}