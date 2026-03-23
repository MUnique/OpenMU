// <copyright file="InventoryPlacementService.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.Web.ItemEditor;

using MUnique.OpenMU.DataModel;
using MUnique.OpenMU.DataModel.Entities;

/// <summary>
/// Service for calculating inventory item placement, slot availability, and positioning.
/// </summary>
public sealed class InventoryPlacementService
{
    private readonly StorageType _storageType;
    private readonly byte _numberOfExtensions;

    /// <summary>
    /// Initializes a new instance of the <see cref="InventoryPlacementService"/> class.
    /// </summary>
    /// <param name="storageType">The type of storage (inventory, vault, etc.).</param>
    /// <param name="numberOfExtensions">The number of inventory extensions.</param>
    public InventoryPlacementService(StorageType storageType, byte numberOfExtensions)
    {
        this._storageType = storageType;
        this._numberOfExtensions = numberOfExtensions;
    }

    /// <summary>
    /// Finds the first available slot where the item can be placed.
    /// </summary>
    /// <param name="item">The item to place.</param>
    /// <param name="existingItems">The existing items in the storage.</param>
    /// <returns>The first available slot, or null if no slot is available.</returns>
    public byte? FindFreeSlot(Item item, IEnumerable<Item> existingItems)
    {
        if (item.Definition is null)
        {
            return this.FindFirstEmptySlot(existingItems);
        }

        var occupiedSlots = existingItems.SelectMany(this.GetItemSlots).ToHashSet();
        var maxSlot = this.GetMaxSlot();
        var boxOffset = this.GetBoxOffset();
        var rowSize = this.GetRowSize();
        var itemWidth = item.Definition.Width;
        var itemHeight = item.Definition.Height;

        for (byte slot = boxOffset; slot < maxSlot; slot++)
        {
            var column = (slot - boxOffset) % rowSize;
            if (column + itemWidth > rowSize)
            {
                continue;
            }

            bool canPlace = true;
            for (byte row = 0; row < itemHeight && canPlace; row++)
            {
                for (byte col = 0; col < itemWidth && canPlace; col++)
                {
                    var currentSlot = (byte)(slot + (row * rowSize) + col);
                    if (currentSlot >= maxSlot || occupiedSlots.Contains(currentSlot))
                    {
                        canPlace = false;
                    }
                }
            }

            if (canPlace)
            {
                return slot;
            }
        }

        return null;
    }

    private byte GetMaxSlot()
    {
        return this._storageType switch
        {
            StorageType.Vault => InventoryConstants.WarehouseSize,
            StorageType.VaultExtension => InventoryConstants.WarehouseSize,
            StorageType.Inventory => InventoryConstants.GetInventorySize(this._numberOfExtensions),
            StorageType.InventoryExtension => (byte)(InventoryConstants.RowsOfOneExtension * InventoryConstants.RowSize),
            StorageType.PersonalStore => InventoryConstants.StoreSize,
            StorageType.Merchant => InventoryConstants.WarehouseSize,
            _ => byte.MaxValue,
        };
    }

    private byte GetBoxOffset()
    {
        return this._storageType == StorageType.Inventory ? InventoryConstants.EquippableSlotsCount : (byte)0;
    }

    private byte? FindFirstEmptySlot(IEnumerable<Item> existingItems)
    {
        var occupiedSlots = existingItems.SelectMany(this.GetItemSlots).ToHashSet();
        var maxSlot = this.GetMaxSlot();
        var boxOffset = this.GetBoxOffset();

        for (byte slot = boxOffset; slot < maxSlot; slot++)
        {
            if (!occupiedSlots.Contains(slot))
            {
                return slot;
            }
        }

        return null;
    }

    private byte GetRowSize()
    {
        return (byte)InventoryConstants.RowSize;
    }

    private HashSet<byte> GetAllOccupiedSlots(IEnumerable<Item> existingItems)
    {
        return existingItems
            .SelectMany(this.GetItemSlots)
            .ToHashSet();
    }

    private IEnumerable<byte> GetItemSlots(Item item)
    {
        if (item.Definition is null)
        {
            yield break;
        }

        var startSlot = item.ItemSlot;
        var width = item.Definition.Width;
        var height = item.Definition.Height;
        var boxOffset = this.GetBoxOffset();
        var rowSize = this.GetRowSize();

        if (startSlot < boxOffset)
        {
            yield return startSlot;
            yield break;
        }

        for (byte row = 0; row < height; row++)
        {
            for (byte col = 0; col < width; col++)
            {
                yield return (byte)(startSlot + (row * rowSize) + col);
            }
        }
    }
}
