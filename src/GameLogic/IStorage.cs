// <copyright file="IStorage.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.GameLogic;

using MUnique.OpenMU.PlugIns;
using Nito.AsyncEx;

/// <summary>
/// The types of item storages.
/// </summary>
public enum Storages
{
    /// <summary>
    /// The inventory storage.
    /// </summary>
    Inventory,

    /// <summary>
    /// The trade storage.
    /// </summary>
    Trade,

    /// <summary>
    /// The vault storage.
    /// </summary>
    Vault,

    /// <summary>
    /// The chaos machine storage.
    /// </summary>
    ChaosMachine,

    /// <summary>
    /// The personal store storage.
    /// </summary>
    PersonalStore,

    /// <summary>
    /// The pet trainer storage.
    /// </summary>
    PetTrainer,

    /// <summary>
    /// The storage of the refinery of the elphis npc.
    /// </summary>
    Refinery,

    /// <summary>
    /// The storage of the smelting dialog of the osbourne npc.
    /// </summary>
    Smelting,

    /// <summary>
    /// The storage of the item restore dialog of the jerridon npc.
    /// </summary>
    ItemRestore,

    /// <summary>
    /// The storage of the chaos card master dialog.
    /// </summary>
    ChaosCardMaster,

    /// <summary>
    /// The storage of the cherry blossom spirit dialog.
    /// </summary>
    CherryBlossomSpirit,

    /// <summary>
    /// The storage of the seed crafting dialog.
    /// </summary>
    SeedCrafting,

    /// <summary>
    /// The storage of the seed sphere crafting dialog.
    /// </summary>
    SeedSphereCrafting,

    /// <summary>
    /// The storage of the seed mount crafting dialog.
    /// </summary>
    SeedMountCrafting,

    /// <summary>
    /// The storage of the seed unmount crafting dialog.
    /// </summary>
    SeedUnmountCrafting,
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
    AsyncLock StoreLock { get; }
}