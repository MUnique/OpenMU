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

    /// <summary>
    /// Determines whether an item can be placed at the top-left slot of a storage section.
    /// </summary>
    /// <param name="item">The item to move.</param>
    /// <param name="slot">The top-left destination slot.</param>
    /// <param name="existingItems">All items belonging to the storage.</param>
    /// <param name="startIndex">The first slot displayed by the target section.</param>
    /// <param name="rows">The number of rows in the target section.</param>
    /// <returns><c>true</c> when the entire item rectangle fits; otherwise, <c>false</c>.</returns>
    public bool CanMoveItem(Item item, int slot, IEnumerable<Item> existingItems, int startIndex, int rows)
    {
        var relativeSlot = slot - startIndex;
        if (relativeSlot < 0)
        {
            return false;
        }

        return this.CanMoveItem(
            item,
            relativeSlot % InventoryConstants.RowSize,
            relativeSlot / InventoryConstants.RowSize,
            existingItems,
            startIndex,
            rows);
    }

    /// <summary>
    /// Determines whether an item can be placed at the specified column and row of a storage section.
    /// </summary>
    /// <param name="item">The item to move.</param>
    /// <param name="column">The zero-based destination column.</param>
    /// <param name="row">The zero-based destination row.</param>
    /// <param name="existingItems">All items belonging to the storage.</param>
    /// <param name="startIndex">The first slot displayed by the target section.</param>
    /// <param name="rows">The number of rows in the target section.</param>
    /// <returns><c>true</c> when the entire item rectangle fits; otherwise, <c>false</c>.</returns>
    public bool CanMoveItem(Item item, int column, int row, IEnumerable<Item> existingItems, int startIndex, int rows)
    {
        var width = item.Definition?.Width ?? 1;
        var height = item.Definition?.Height ?? 1;
        var sectionSize = rows * InventoryConstants.RowSize;
        if (rows <= 0 || column < 0 || row < 0 || row >= rows || column + width > InventoryConstants.RowSize || row + height > rows || width == 0 || height == 0)
        {
            return false;
        }

        var relativeSlot = (row * InventoryConstants.RowSize) + column;
        var occupiedSlots = existingItems
            .Where(existing => !ReferenceEquals(existing, item)
                && existing.ItemSlot >= startIndex
                && existing.ItemSlot < startIndex + sectionSize)
            .SelectMany(existing => this.GetItemSlotsInSection(existing, startIndex))
            .ToHashSet();

        for (var rowOffset = 0; rowOffset < height; rowOffset++)
        {
            for (var columnOffset = 0; columnOffset < width; columnOffset++)
            {
                var targetCell = relativeSlot + (rowOffset * InventoryConstants.RowSize) + columnOffset;
                if (occupiedSlots.Contains(targetCell))
                {
                    return false;
                }
            }
        }

        return true;
    }

    /// <summary>
    /// Moves an item to a cell in the specified storage section when its entire rectangle fits.
    /// </summary>
    /// <param name="item">The item to move.</param>
    /// <param name="slot">The top-left destination slot.</param>
    /// <param name="existingItems">All items belonging to the storage.</param>
    /// <param name="startIndex">The first slot displayed by the target section.</param>
    /// <param name="rows">The number of rows in the target section.</param>
    /// <returns><c>true</c> when the item was moved; otherwise, <c>false</c>.</returns>
    public bool TryMoveItem(Item item, byte slot, IEnumerable<Item> existingItems, int startIndex, int rows)
    {
        if (!this.CanMoveItem(item, slot, existingItems, startIndex, rows))
        {
            return false;
        }

        item.ItemSlot = slot;
        return true;
    }

    /// <summary>
    /// Moves an item to the specified column and row when its entire rectangle fits.
    /// </summary>
    /// <param name="item">The item to move.</param>
    /// <param name="column">The zero-based destination column.</param>
    /// <param name="row">The zero-based destination row.</param>
    /// <param name="existingItems">All items belonging to the storage.</param>
    /// <param name="startIndex">The first slot displayed by the target section.</param>
    /// <param name="rows">The number of rows in the target section.</param>
    /// <returns><c>true</c> when the item was moved; otherwise, <c>false</c>.</returns>
    public bool TryMoveItem(Item item, int column, int row, IEnumerable<Item> existingItems, int startIndex, int rows)
    {
        if (!this.CanMoveItem(item, column, row, existingItems, startIndex, rows))
        {
            return false;
        }

        item.ItemSlot = (byte)(startIndex + (row * InventoryConstants.RowSize) + column);
        return true;
    }

    /// <summary>
    /// Calculates the item top-left coordinates from the cell currently under the pointer.
    /// </summary>
    /// <param name="hoveredSlot">The slot currently under the pointer.</param>
    /// <param name="startIndex">The first slot displayed by the target section.</param>
    /// <param name="columnOffset">The grabbed column within the dragged item.</param>
    /// <param name="rowOffset">The grabbed row within the dragged item.</param>
    /// <returns>The zero-based item top-left column and row within the target section.</returns>
    public static (int Column, int Row) GetDropPosition(int hoveredSlot, int startIndex, byte columnOffset, byte rowOffset)
    {
        var relativeSlot = hoveredSlot - startIndex;
        return (
            (relativeSlot % InventoryConstants.RowSize) - columnOffset,
            (relativeSlot / InventoryConstants.RowSize) - rowOffset);
    }

    /// <summary>
    /// Determines whether an item can be equipped into a compatible empty equipment slot.
    /// </summary>
    /// <param name="item">The item to equip.</param>
    /// <param name="slot">The target equipment slot.</param>
    /// <param name="existingItems">All items belonging to the storage.</param>
    /// <returns><c>true</c> when the item can be equipped; otherwise, <c>false</c>.</returns>
    public bool CanEquipItem(Item item, byte slot, IEnumerable<Item> existingItems)
    {
        return slot < InventoryConstants.EquippableSlotsCount
            && item.Definition?.ItemSlot?.ItemSlots.Contains(slot) == true
            && existingItems.All(existing => ReferenceEquals(existing, item) || existing.ItemSlot != slot);
    }

    /// <summary>
    /// Moves an item to a compatible empty equipment slot.
    /// </summary>
    /// <param name="item">The item to equip.</param>
    /// <param name="slot">The target equipment slot.</param>
    /// <param name="existingItems">All items belonging to the storage.</param>
    /// <returns><c>true</c> when the item was equipped; otherwise, <c>false</c>.</returns>
    public bool TryEquipItem(Item item, byte slot, IEnumerable<Item> existingItems)
    {
        if (!this.CanEquipItem(item, slot, existingItems))
        {
            return false;
        }

        item.ItemSlot = slot;
        return true;
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

    private IEnumerable<int> GetItemSlotsInSection(Item item, int startIndex)
    {
        var relativeSlot = item.ItemSlot - startIndex;
        var column = relativeSlot % InventoryConstants.RowSize;
        var row = relativeSlot / InventoryConstants.RowSize;
        var width = item.Definition?.Width ?? 1;
        var height = item.Definition?.Height ?? 1;

        for (var rowOffset = 0; rowOffset < height; rowOffset++)
        {
            for (var columnOffset = 0; columnOffset < width; columnOffset++)
            {
                yield return ((row + rowOffset) * InventoryConstants.RowSize) + column + columnOffset;
            }
        }
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
