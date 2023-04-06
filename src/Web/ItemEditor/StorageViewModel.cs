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
    /// Initializes a new instance of the <see cref="StorageViewModel"/> class.
    /// </summary>
    /// <param name="storage">The storage.</param>
    /// <param name="rows">The rows.</param>
    /// <param name="startIndex">The start index.</param>
    /// <param name="endIndex">The end index.</param>
    /// <exception cref="System.ArgumentException">Unsupported number of rows ({rows}) instead of 4, 8 or 15. - rows</exception>
    public StorageViewModel(ItemStorage storage, int rows, int startIndex, int endIndex)
    {
        if (rows is not (4 or 8 or 15))
        {
            throw new ArgumentException($"Unsupported number of rows ({rows}) instead of 4, 8 or 15.", nameof(rows));
        }

        this.Storage = storage;
        this.Rows = rows;
        this.StartIndex = startIndex;
        this.EndIndex = endIndex;
    }

    /// <summary>
    /// Gets the storage.
    /// </summary>
    public ItemStorage Storage { get; }

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
}