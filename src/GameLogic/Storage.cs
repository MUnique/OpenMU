// <copyright file="Storage.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.GameLogic;

/// <summary>
/// This class wraps the access to the IItemStorage of an character.
/// </summary>
public class Storage : IStorage
{
    private readonly bool[,] _usedSlots;

    private readonly int _rows;
    private readonly int _slotOffset;

    private readonly int _boxOffset;

    /// <summary>
    /// Initializes a new instance of the <see cref="Storage"/> class.
    /// </summary>
    /// <param name="numberOfSlots">The number of slots.</param>
    /// <param name="itemStorage">The item storage.</param>
    public Storage(int numberOfSlots, ItemStorage itemStorage)
        : this(numberOfSlots, 0, 0, itemStorage)
    {
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="Storage" /> class.
    /// </summary>
    /// <param name="numberOfSlots">The number of slots.</param>
    /// <param name="boxOffset">The box offset, which is the first index where the 2-dimensional box starts.</param>
    /// <param name="slotOffset">The slot offset, which means at which <see cref="Item.ItemSlot" /> this instance starts.</param>
    /// <param name="itemStorage">The item storage.</param>
    public Storage(int numberOfSlots, int boxOffset, int slotOffset, ItemStorage itemStorage)
    {
        this.ItemArray = new Item?[numberOfSlots];
        this._rows = (numberOfSlots - boxOffset) / InventoryConstants.RowSize;
        this._usedSlots = new bool[this._rows, InventoryConstants.RowSize];
        this.ItemStorage = itemStorage;
        this._slotOffset = slotOffset;
        this._boxOffset = boxOffset;

        var lastSlot = numberOfSlots + slotOffset - 1;
        List<Item>? unfittingItems = null;
        this.ItemStorage.Items
            .Where(item => item.ItemSlot <= lastSlot && item.ItemSlot >= slotOffset)
            .Where(item => item.Definition is not null)
            .ForEach(item =>
            {
                if (!this.AddItemInternal((byte)(item.ItemSlot - slotOffset), item))
                {
                    (unfittingItems ??= new()).Add(item);
                }
            });

        if (unfittingItems is null)
        {
            return;
        }

        // we first try to add them.
        for (var index = unfittingItems.Count - 1; index >= 0; index--)
        {
            var item = unfittingItems[index];
            var freeSlot = this.CheckInvSpace(item);

            if (freeSlot is not null && this.ContainsSlot(freeSlot.Value) && this.AddItem(freeSlot.Value, item))
            {
                unfittingItems.RemoveAt(index);
            }
        }

        if (unfittingItems.Count > 0)
        {
            throw new ArgumentException($"Some items do not fit into the storage: {string.Join(';', unfittingItems)}");
        }
    }

    /// <inheritdoc/>
    public ItemStorage ItemStorage { get; }

    /// <inheritdoc/>
    public IEnumerable<Item> Items
    {
        get { return this.ItemArray.Where(i => i is not null).Select(item => item!).Concat(this.Extensions.SelectMany(ext => ext.Items)); }
    }

    /// <inheritdoc/>
    public IEnumerable<byte> FreeSlots
    {
        get
        {
            for (int row = 0; row < this._rows; row++)
            {
                for (int column = 0; column < InventoryConstants.RowSize; column++)
                {

                    if (!this._usedSlots[row, column])
                    {
                        yield return this.GetSlot(column, row);
                    }
                }
            }

            foreach (var extensionSlot in this.Extensions.SelectMany(e => e.FreeSlots))
            {
                yield return extensionSlot;
            }
        }
    }

    /// <inheritdoc/>
    public IEnumerable<IStorage> Extensions { get; protected set; } = [];

    /// <summary>
    /// Gets the item array with the <see cref="Item.ItemSlot"/> minus <see cref="_slotOffset"/> as index.
    /// </summary>
    protected Item?[] ItemArray { get; }

    /// <inheritdoc/>
    public virtual async ValueTask<bool> AddItemAsync(byte slot, Item item)
    {
        if (this.ContainsSlot(slot))
        {
            return this.AddItem(slot, item);
        }

        if (this.Extensions.FirstOrDefault(ext => ext.ContainsSlot(slot)) is { } extension)
        {
            return await extension.AddItemAsync(slot, item).ConfigureAwait(false);
        }

        return false;
    }

    /// <inheritdoc/>
    public async ValueTask<bool> AddItemAsync(Item item)
    {
        var freeSlot = this.CheckInvSpace(item);
        if (freeSlot is null)
        {
            return false;
        }

        if (this.ContainsSlot((byte)freeSlot))
        {
            return this.AddItem((byte)freeSlot, item);
        }

        foreach (var extension in this.Extensions)
        {
            if (await extension.AddItemAsync(item).ConfigureAwait(false))
            {
                return true;
            }
        }

        return false;
    }

    /// <inheritdoc/>
    public bool TryAddMoney(int value)
    {
        if (this.ItemStorage.Money + value < 0)
        {
            return false;
        }

        this.ItemStorage.Money = this.ItemStorage.Money + value;
        return true;
    }

    /// <inheritdoc/>
    public bool TryRemoveMoney(int value)
    {
        if (this.ItemStorage.Money - value < 0)
        {
            return false;
        }

        this.ItemStorage.Money = this.ItemStorage.Money - value;
        return true;
    }

    /// <inheritdoc/>
    public byte? CheckInvSpace(Item item)
    {
        item.ThrowNotInitializedProperty(item.Definition is null, nameof(item.Definition));

        // Find free Space in the Inventory and return the slot where the Item can be placed
        // Check every slot if it fits
        for (byte row = 0; row < this._rows; row++)
        {
            for (byte column = 0; column < InventoryConstants.RowSize; column++)
            {
                if (this.FitsInside(row, column, item.Definition.Width, item.Definition.Height))
                {
                    return (byte)((row * InventoryConstants.RowSize) + column + this._boxOffset + this._slotOffset);
                }
            }
        }

        foreach (var extension in this.Extensions)
        {
            if (extension.CheckInvSpace(item) is { } slot)
            {
                return slot;
            }
        }

        return null;
    }

    /// <inheritdoc/>
    public async ValueTask<bool> TryTakeAllAsync(IStorage anotherStorage)
    {
        var result = true;

        var itemsWithSlots = anotherStorage.Items.Select(item => (item.ItemSlot, Item: item)).ToList();
        try
        {
            foreach (var item in itemsWithSlots)
            {
                await anotherStorage.RemoveItemAsync(item.Item).ConfigureAwait(false);
                if (!await this.AddItemAsync(item.Item).ConfigureAwait(false))
                {
                    result = false;
                }
            }
        }
        finally
        {
            if (!result)
            {
                foreach (var item in itemsWithSlots)
                {
                    await this.RemoveItemAsync(item.Item).ConfigureAwait(false);
                    await anotherStorage.AddItemAsync(item.ItemSlot, item.Item).ConfigureAwait(false);
                }
            }
        }

        return result;
    }

    /// <inheritdoc/>
    public Item? GetItem(byte inventorySlot)
    {
        if (this.ContainsSlot(inventorySlot))
        {
            var index = inventorySlot - this._slotOffset;
            if (index < this.ItemArray.Length)
            {
                return this.ItemArray[index];
            }
        }

        var extension = this.Extensions.FirstOrDefault(ext => ext.ContainsSlot(inventorySlot));
        return extension?.GetItem(inventorySlot);
    }

    /// <inheritdoc/>
    public virtual async ValueTask RemoveItemAsync(Item item)
    {
        if (this.ContainsSlot(item.ItemSlot))
        {
            var slot = item.ItemSlot - this._slotOffset;
            this.ItemArray[slot] = null;
            if (slot >= this._boxOffset)
            {
                var columnIndex = this.GetColumnIndex(slot);
                var rowIndex = this.GetRowIndex(slot);
                this.SetItemUsedSlots(item, columnIndex, rowIndex, false);
            }

            this.ItemStorage.Items.Remove(item);
        }
        else if (this.Extensions.FirstOrDefault(ext => ext.ContainsSlot(item.ItemSlot)) is { } extension)
        {
            await extension.RemoveItemAsync(item).ConfigureAwait(false);
        }
        else
        {
            // ignore it.
        }
    }

    /// <inheritdoc/>
    public virtual void Clear()
    {
        this.ItemStorage.Items.Clear();
        this.ItemArray.ClearToDefaults();
        this._usedSlots.ClearToDefaults();

        foreach (var extension in this.Extensions)
        {
            extension.Clear();
        }
    }

    /// <summary>
    /// Determines whether the slot belongs to this storage, or not.
    /// Extensions are not considered here.
    /// </summary>
    /// <param name="slot">The slot.</param>
    /// <returns>
    ///   <c>true</c> if the slot belongs to this storage; otherwise, <c>false</c>.
    /// </returns>
    public bool ContainsSlot(byte slot)
    {
        if (slot < this._slotOffset)
        {
            return false;
        }

        if (slot >= this._slotOffset + this.ItemArray.Length)
        {
            return false;
        }

        return true;
    }

    /// <summary>
    /// Adds the item to the internal data structures.
    /// </summary>
    /// <param name="slot">The slot.</param>
    /// <param name="item">The item.</param>
    /// <returns><c>True</c>, if successful; Otherwise, <c>false</c>.</returns>
    protected bool AddItemInternal(byte slot, Item item)
    {
        item.ThrowNotInitializedProperty(item.Definition is null, nameof(item.Definition));

        if (this.ItemArray[slot] != null)
        {
            return false;
        }

        if (slot < this._boxOffset)
        {
            if (this.ItemArray[slot] is null)
            {
                this.ItemArray[slot] = item;
                return true;
            }

            return false;
        }

        var columnIndex = this.GetColumnIndex(slot);
        var rowIndex = this.GetRowIndex(slot);
        var itemDef = item.Definition;
        if (!this.FitsInside((byte)rowIndex, (byte)columnIndex, itemDef.Width, itemDef.Height))
        {
            return false;
        }

        this.ItemArray[slot] = item;
        this.SetItemUsedSlots(item, columnIndex, rowIndex);
        return true;
    }

    private bool AddItem(byte slot, Item item)
    {
        var result = this.AddItemInternal((byte)(slot - this._slotOffset), item);
        if (result)
        {
            this.ItemStorage.Items.Add(item);
            item.ItemSlot = slot;
        }

        return result;
    }

    private byte GetSlot(int column, int row)
    {
        byte result = (byte)(this._slotOffset + this._boxOffset + column + (row * InventoryConstants.RowSize));
        return result;
    }

    private void SetItemUsedSlots(Item item, int columnIndex, int rowIndex, bool used = true)
    {
        for (int r = rowIndex; r < rowIndex + item.Definition!.Height; r++)
        {
            for (int c = columnIndex; c < columnIndex + item.Definition.Width; c++)
            {
                if (r < this._rows && c < InventoryConstants.RowSize)
                {
                    this._usedSlots[r, c] = used;
                }
            }
        }
    }

    private int GetColumnIndex(int slot)
    {
        return (slot - this._boxOffset) % InventoryConstants.RowSize;
    }

    private int GetRowIndex(int slot)
    {
        return (slot - this._boxOffset) / InventoryConstants.RowSize;
    }

    private bool FitsInside(byte row, byte column, byte x, byte y)
    {
        int rowCount = this._usedSlots.GetLength(0);
        int columnCount = this._usedSlots.GetLength(1);
        if ((rowCount >= row + y) && (columnCount >= column + x))
        {
            for (byte currentRow = row; currentRow < row + y; currentRow++)
            {
                for (byte currentColumn = column; currentColumn < column + x; currentColumn++)
                {
                    if (this._usedSlots[currentRow, currentColumn])
                    {
                        return false;
                    }
                }
            }
        }
        else
        {
            return false;
        }

        return true;
    }
}