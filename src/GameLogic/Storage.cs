// <copyright file="Storage.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.GameLogic
{
    using System;
    using System.Collections.Generic;
    using System.Linq;

    using MUnique.OpenMU.DataModel.Entities;

    /// <summary>
    /// This class wraps the access to the IItemStorage of an character.
    /// </summary>
    public class Storage : IStorage
    {
        private readonly Item[] itemArray;

        private readonly int firstEquippableSlot;

        private readonly int lastEquippableSlot;

        private readonly int firstStorageSlot; // 12

        private readonly bool[,] usedSlots;

        private readonly int rows;

        private readonly ItemStorage itemStorage;

        /// <summary>
        /// Initializes a new instance of the <see cref="Storage"/> class.
        /// </summary>
        /// <param name="firstEquippableSlot">The first equippable slot.</param>
        /// <param name="lastEquippableSlot">The last equippable slot.</param>
        /// <param name="numberOfSlots">The number of slots.</param>
        /// <param name="itemStorage">The item storage.</param>
        public Storage(int firstEquippableSlot, int lastEquippableSlot, int numberOfSlots, ItemStorage itemStorage)
        {
            this.firstEquippableSlot = firstEquippableSlot;
            this.lastEquippableSlot = lastEquippableSlot;
            this.itemArray = new Item[numberOfSlots];
            this.firstStorageSlot = lastEquippableSlot > 0 ? lastEquippableSlot + 1 : 0;
            this.rows = (numberOfSlots - this.firstStorageSlot) / InventoryConstants.RowSize;
            this.usedSlots = new bool[this.rows, InventoryConstants.RowSize];
            this.itemStorage = itemStorage;
            this.itemStorage?.Items
                .Where(item => item.ItemSlot <= lastEquippableSlot + numberOfSlots)
                .ForEach(item =>
                {
                    if (!this.AddItemInternal(item.ItemSlot, item))
                    {
                        throw new ArgumentException("item did not fit into the storage");
                    }
                });
        }

        /// <inheritdoc/>
        public event EventHandler<ItemEventArgs> EquippedItemsChanged;

        /// <inheritdoc/>
        public ItemStorage ItemStorage
        {
            get
            {
                return this.itemStorage;
            }
        }

        /// <inheritdoc/>
        public IEnumerable<Item> Items
        {
            get
            {
                return this.itemArray.Where(i => i != null);
            }
        }

        /// <inheritdoc/>
        public IEnumerable<Item> EquippedItems
        {
            get
            {
                for (int i = this.firstEquippableSlot; i <= this.lastEquippableSlot; i++)
                {
                    if (this.itemArray[i] != null)
                    {
                        yield return this.itemArray[i];
                    }
                }
            }
        }

        /// <inheritdoc/>
        public IEnumerable<byte> FreeSlots
        {
            get
            {
                for (int x = 0; x < InventoryConstants.RowSize; x++)
                {
                    for (int y = 0; y < this.rows; y++)
                    {
                        if (!this.usedSlots[x, y])
                        {
                            yield return this.GetSlot(x, y);
                        }
                    }
                }
            }
        }

        /// <inheritdoc/>
        public IEnumerable<IStorage> Extensions { get; set; }

        /// <inheritdoc/>
        public virtual bool AddItem(byte slot, Item item)
        {
            var result = this.AddItemInternal(slot, item);
            if (result)
            {
                this.itemStorage.Items.Add(item);
                this.SetItemSlot(item, slot);
                var onEquippedItemsChanged = this.EquippedItemsChanged;
                if (onEquippedItemsChanged != null && this.IsWearingSlot(slot))
                {
                    onEquippedItemsChanged(this, new ItemEventArgs(item));
                }
            }

            return result;
        }

        /// <inheritdoc/>
        public bool AddItem(Item item)
        {
            var freeSlot = (this as IStorage).CheckInvSpace(item);
            if (freeSlot < 0)
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

           this.itemStorage.Money = checked(this.ItemStorage.Money + value);
            return true;
        }

        /// <inheritdoc/>
        public bool TryRemoveMoney(int value)
        {
            if (this.ItemStorage.Money - value < 0)
            {
                return false;
            }

            this.itemStorage.Money = checked(this.ItemStorage.Money - value);
            return true;
        }

        /// <inheritdoc/>
        public int CheckInvSpace(Item item)
        {
            if (item.Definition.Number == 0x0F && item.Definition.Group == 0xE)
            {
                // zen: index 0x0F, kind 0x0E
                return 0xFF;
            }

            // Find free Space in the Inventory and return the slot where the Item can be placed
            // Check every slot if it fits
            for (byte row = 0; row < this.rows; row++)
            {
                for (byte column = 0; column < InventoryConstants.RowSize; column++)
                {
                    if (this.FitsInside(row, column, item.Definition.Width, item.Definition.Height))
                    {
                        return (row * InventoryConstants.RowSize) + column + this.firstStorageSlot;
                    }
                }
            }

            return -1;
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
        public Item GetItem(byte inventorySlot)
        {
            if (inventorySlot < this.itemArray.Length)
            {
                return this.itemArray[inventorySlot - this.firstEquippableSlot];
            }

            return null;
        }

        /// <inheritdoc/>
        public void RemoveItem(Item item)
        {
            this.itemArray[item.ItemSlot - this.firstEquippableSlot] = null;
            if (!this.IsWearingSlot(item.ItemSlot))
            {
                var columnIndex = this.GetColumnIndex(item.ItemSlot);
                var rowIndex = this.GetRowIndex(item.ItemSlot);
                this.SetItemUsedSlots(item, columnIndex, rowIndex, false);
            }

            this.itemStorage.Items.Remove(item);
            var onEquippedItemsChanged = this.EquippedItemsChanged;
            if (onEquippedItemsChanged != null && this.IsWearingSlot(item.ItemSlot))
            {
                onEquippedItemsChanged.Invoke(this, new ItemEventArgs(item));
            }
        }

        /// <inheritdoc/>
        public virtual void Clear()
        {
            this.itemStorage.Items.Clear();
            this.itemArray.ClearToDefaults();
            this.usedSlots.ClearToDefaults();
        }

        /// <summary>
        /// Sets the item slot at the item. Can be overwritten if there is some kind of slot offset.
        /// </summary>
        /// <param name="item">The item.</param>
        /// <param name="slot">The slot.</param>
        protected virtual void SetItemSlot(Item item, byte slot)
        {
            item.ItemSlot = slot;
        }

        private byte GetSlot(int column, int row)
        {
            byte result = (byte)(this.firstStorageSlot + column + (row * InventoryConstants.RowSize));
            return result;
        }

        private bool AddItemInternal(byte slot, Item item)
        {
            if (this.itemArray[slot] != null)
            {
                return false;
            }

            if (this.IsWearingSlot(slot))
            {
                this.itemArray[slot] = item;
                return true;
            }

            var itemDef = item.Definition;
            var columnIndex = this.GetColumnIndex(slot);
            var rowIndex = this.GetRowIndex(slot);
            if (!this.FitsInside((byte)rowIndex, (byte)columnIndex, itemDef.Width, itemDef.Height))
            {
                return false;
            }

            this.itemArray[slot] = item;
            this.SetItemUsedSlots(item, columnIndex, rowIndex);
            return true;
        }

        private void SetItemUsedSlots(Item item, int columnIndex, int rowIndex, bool used = true)
        {
            for (int r = rowIndex; r < rowIndex + item.Definition.Height; r++)
            {
                for (int c = columnIndex; c < columnIndex + item.Definition.Width; c++)
                {
                    if (r < this.rows && c < InventoryConstants.RowSize)
                    {
                        this.usedSlots[r, c] = used;
                    }
                }
            }
        }

        private int GetColumnIndex(int slot)
        {
            return (slot - this.firstStorageSlot) % InventoryConstants.RowSize;
        }

        private int GetRowIndex(int slot)
        {
            return (slot - this.firstStorageSlot) / InventoryConstants.RowSize;
        }

        private bool IsWearingSlot(int slot)
        {
            if (this.firstEquippableSlot == this.lastEquippableSlot)
            {
                return false;
            }

            return this.firstEquippableSlot <= slot && this.lastEquippableSlot >= slot;
        }

        private bool FitsInside(byte row, byte column, byte x, byte y)
        {
            int rowCount = this.usedSlots.GetLength(0);
            int columnCount = this.usedSlots.GetLength(1);
            if ((rowCount >= row + y) && (columnCount >= column + x))
            {
                for (byte currentRow = row; currentRow < row + y; currentRow++)
                {
                    for (byte currentColumn = column; currentColumn < column + x; currentColumn++)
                    {
                        if (this.usedSlots[currentRow, currentColumn])
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
}
