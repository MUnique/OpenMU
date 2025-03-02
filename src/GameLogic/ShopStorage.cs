// <copyright file="ShopStorage.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.GameLogic;

using Nito.AsyncEx;

/// <summary>
/// The storage of the personal store of a player.
/// </summary>
public class ShopStorage : Storage, IShopStorage
{
    private readonly Character _character;

    /// <summary>
    /// Initializes a new instance of the <see cref="ShopStorage"/> class.
    /// </summary>
    /// <param name="character">The character.</param>
    public ShopStorage(Character character)
        : base(InventoryConstants.StoreSize, 0, InventoryConstants.FirstStoreItemSlotIndex, new ItemStorageAdapter(character.Inventory ?? throw Error.NotInitializedProperty(character, nameof(Character.Inventory)), InventoryConstants.FirstStoreItemSlotIndex, InventoryConstants.StoreSize))
    {
        this.StoreLock = new AsyncLock();
        this._character = character;
    }

    /// <inheritdoc/>
    public AsyncLock StoreLock { get; }

    /// <inheritdoc/>
    public bool StoreOpen
    {
        get => this._character.IsStoreOpened;
        set => this._character.IsStoreOpened = value;
    }

    /// <inheritdoc/>
    public override async ValueTask<bool> AddItemAsync(byte slot, Item item)
    {
        if (this.StoreOpen)
        {
            return false;
        }

        return await base.AddItemAsync(slot, item).ConfigureAwait(false);
    }
}