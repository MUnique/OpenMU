﻿// <copyright file="Storage.cs" company="MUnique">
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

        var lastSlot = numberOfSlots + slotOffset;
        this.ItemStorage.Items
            .Where(item => item.ItemSlot <= lastSlot && item.ItemSlot >= slotOffset)
            .ForEach(item =>
            {
                if (!this.AddItemInternal((byte)(item.ItemSlot - slotOffset), item))
                {
                    throw new ArgumentException("item did not fit into the storage");
                }
            });
    }

    /// <inheritdoc/>
    public ItemStorage ItemStorage { get; }

    /// <inheritdoc/>
    public IEnumerable<Item> Items
    {
        get
        {
            return this.ItemArray.Where(i => i is not null).Select(item => item!);
        }
    }

    /// <inheritdoc/>
    public IEnumerable<byte> FreeSlots
    {
        get
        {
            for (int x = 0; x < InventoryConstants.RowSize; x++)
            {
                for (int y = 0; y < this._rows; y++)
                {
                    if (!this._usedSlots[x, y])
                    {
                        yield return this.GetSlot(x, y);
                    }
                }
            }
        }
    }

    /// <inheritdoc/>
    public IEnumerable<IStorage> Extensions { get; set; } = Enumerable.Empty<IStorage>();

    /// <summary>
    /// Gets the item array with the <see cref="Item.ItemSlot"/> minus <see cref="_slotOffset"/> as index.
    /// </summary>
    protected Item?[] ItemArray { get; }

    /// <inheritdoc/>
    public virtual bool AddItem(byte slot, Item item)
    {
        var result = this.AddItemInternal((byte)(slot - this._slotOffset), item);
        if (result)
        {
            this.ItemStorage.Items.Add(item);
            item.ItemSlot = slot;
        }

        return result;
    }

    /// <inheritdoc/>
    public bool AddItem(Item item)
    {
        var freeSlot = this.CheckInvSpace(item);
        if (freeSlot is null)
        {
            return false;
        }

        return this.AddItem((byte)freeSlot, item);
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
    public int? CheckInvSpace(Item item)
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
                    return (row * InventoryConstants.RowSize) + column + this._boxOffset + this._slotOffset;
                }
            }
        }

        return null;
    }

    /// <inheritdoc/>
    public bool TryTakeAll(IStorage anotherStorage)
    {
        // TODO: this should be a all-or-nothing action...
        foreach (var item in anotherStorage.Items)
        {
            if (!this.AddItem(item))
            {
                return false;
            }
        }

        return true;
    }

    /// <inheritdoc/>
    public Item? GetItem(byte inventorySlot)
    {
        var index = inventorySlot - this._slotOffset;
        if (index < this.ItemArray.Length)
        {
            return this.ItemArray[index];
        }

        return null;
    }

    /// <inheritdoc/>
    public virtual void RemoveItem(Item item)
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

    /// <inheritdoc/>
    public virtual void Clear()
    {
        this.ItemStorage.Items.Clear();
        this.ItemArray.ClearToDefaults();
        this._usedSlots.ClearToDefaults();
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