// <copyright file="StorageViewModel.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.Web.ItemEditor;

/// <summary>
/// View Model for the <see cref="ItemStorage"/>.
/// </summary>
public class StorageViewModel
{
    /// <summary>
    /// Initializes a new instance of the <see cref="StorageViewModel" /> class.
    /// </summary>
    /// <param name="storage">The storage.</param>
    /// <param name="storageType">Type of the storage.</param>
    /// <param name="rows">The rows.</param>
    /// <param name="startIndex">The start index.</param>
    /// <param name="endIndex">The end index.</param>
    /// <param name="emptyRowsToNextStorage">The empty rows to next storage.</param>
    /// <param name="emptyRowsToPreviousStorage">The empty rows to previous storage.</param>
    public StorageViewModel(ItemStorage storage, StorageType storageType, int rows, int startIndex, int endIndex, byte? emptyRowsToNextStorage = null, byte? emptyRowsToPreviousStorage = null)
    {
        if (rows is not (4 or 8 or 15))
        {
            throw new ArgumentException($"Unsupported number of rows ({rows}) instead of 4, 8 or 15.", nameof(rows));
        }

        this.Storage = storage;
        this.StorageType = storageType;
        this.Rows = rows;
        this.StartIndex = startIndex;
        this.EndIndex = endIndex;
        this.EmptyRowsToNextStorage = emptyRowsToNextStorage;
        this.EmptyRowsToPreviousStorage = emptyRowsToPreviousStorage;
    }

    /// <summary>
    /// Gets the storage.
    /// </summary>
    public ItemStorage Storage { get; }

    /// <summary>
    /// Gets the type of the storage.
    /// </summary>
    public StorageType StorageType { get; }

    /// <summary>
    /// Gets the items.
    /// </summary>
    public IEnumerable<ItemViewModel> Items => this.Storage.Items.Where(this.IsIncluded).Select(item => new ItemViewModel(this, item));

    /// <summary>
    /// Gets the rows.
    /// </summary>
    public int Rows { get; }

    /// <summary>
    /// Gets the start index.
    /// </summary>
    public int StartIndex { get; }

    /// <summary>
    /// Gets the end index.
    /// </summary>
    public int EndIndex { get; }

    /// <summary>
    /// Gets the empty rows to next storage.
    /// </summary>
    public byte? EmptyRowsToNextStorage { get; }

    /// <summary>
    /// Gets the empty rows to previous storage.
    /// </summary>
    public byte? EmptyRowsToPreviousStorage { get; }

    /// <summary>
    /// Determines whether the item can be placed at the specified position of this storage box,
    /// so that it fits into the box and doesn't overlap with other items of it.
    /// </summary>
    /// <param name="item">The item.</param>
    /// <param name="column">The column of the top left corner of the item.</param>
    /// <param name="row">The row of the top left corner of the item.</param>
    /// <returns><c>true</c>, if the item can be placed there; Otherwise, <c>false</c>.</returns>
    public bool CanPlace(Item item, int column, int row)
    {
        var width = GetWidth(item);
        var height = GetHeight(item);
        if (column < 0 || row < 0 || column + width > InventoryConstants.RowSize || row + height > this.Rows)
        {
            return false;
        }

        var startSlot = this.StartIndex + (row * InventoryConstants.RowSize) + column;
        if (startSlot > this.EndIndex || startSlot > byte.MaxValue)
        {
            return false;
        }

        return !this.Storage.Items
            .Where(other => other != item && this.IsIncluded(other))
            .Any(other => Overlaps(other, column, row, width, height));

        bool Overlaps(Item other, int left, int top, int w, int h)
        {
            var otherIndex = other.ItemSlot - this.StartIndex;
            var otherColumn = otherIndex % InventoryConstants.RowSize;
            var otherRow = otherIndex / InventoryConstants.RowSize;
            return left < otherColumn + GetWidth(other)
                   && otherColumn < left + w
                   && top < otherRow + GetHeight(other)
                   && otherRow < top + h;
        }
    }

    /// <summary>
    /// Moves the item to the specified position of this storage box, if it can be placed there.
    /// The item may come from another box of the same storage, e.g. from an inventory extension.
    /// </summary>
    /// <param name="item">The item.</param>
    /// <param name="column">The column of the top left corner of the item.</param>
    /// <param name="row">The row of the top left corner of the item.</param>
    /// <returns><c>true</c>, if the item was moved; Otherwise, <c>false</c>.</returns>
    public bool TryPlace(Item item, int column, int row)
    {
        if (!this.Storage.Items.Contains(item) || !this.CanPlace(item, column, row))
        {
            return false;
        }

        item.ItemSlot = (byte)(this.StartIndex + (row * InventoryConstants.RowSize) + column);
        return true;
    }

    /// <summary>
    /// Determines whether the specified item is included in the shown storage box.
    /// </summary>
    /// <param name="item">The item.</param>
    /// <returns>
    ///   <c>true</c> if the specified item is included; otherwise, <c>false</c>.
    /// </returns>
    public bool IsIncluded(Item item)
    {
        return item.ItemSlot >= this.StartIndex && item.ItemSlot <= this.EndIndex;
    }

    private static int GetWidth(Item item) => Math.Max(item.Definition?.Width ?? 1, (byte)1);

    private static int GetHeight(Item item) => Math.Max(item.Definition?.Height ?? 1, (byte)1);
}
