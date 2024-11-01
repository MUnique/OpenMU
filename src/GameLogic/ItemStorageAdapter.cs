// <copyright file="ItemStorageAdapter.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.GameLogic;

using System.Collections;

/// <summary>
/// A wrapper for another <see cref="ItemStorage"/>.
/// Required to split one item storage into more than one storage spaces, e.g. Inventory and Personal Store which use the same ItemStorage.
/// </summary>
/// <seealso cref="ItemStorage"/>
public class ItemStorageAdapter : ItemStorage
{
    private readonly CollectionAdapter _adapter;

    /// <summary>
    /// Initializes a new instance of the <see cref="ItemStorageAdapter"/> class.
    /// </summary>
    /// <param name="actualStorage">The actual storage.</param>
    /// <param name="firstItemSlot">The first item slot.</param>
    /// <param name="itemSlotCount">The item slot count.</param>
    public ItemStorageAdapter(ItemStorage actualStorage, byte firstItemSlot, byte itemSlotCount)
    {
        this._adapter = new CollectionAdapter(actualStorage.Items, firstItemSlot, itemSlotCount);
        this.ActualStorage = actualStorage;
    }

    /// <inheritdoc />
    public override ICollection<Item> Items => this._adapter;

    /// <summary>
    /// Gets the actual storage which is wrapped by this instance.
    /// </summary>
    public ItemStorage ActualStorage { get; }

    /// <summary>
    /// A collection adapter which just returns items between certain item slots.
    /// </summary>
    private class CollectionAdapter : ICollection<Item>
    {
        private readonly ICollection<Item> _actualCollection;
        private readonly byte _firstItemSlot;
        private readonly byte _itemSlotCount;

        public CollectionAdapter(ICollection<Item> actualCollection, byte firstItemSlot, byte itemSlotCount)
        {
            this._actualCollection = actualCollection;
            this._firstItemSlot = firstItemSlot;
            this._itemSlotCount = itemSlotCount;
        }

        /// <inheritdoc />
        public int Count => this._actualCollection.Count(i => this.IsSlotOfThisStorage(i.ItemSlot));

        /// <inheritdoc />
        public bool IsReadOnly => false;

        /// <inheritdoc />
        public IEnumerator<Item> GetEnumerator()
        {
            return this._actualCollection.Where(item => this.IsSlotOfThisStorage(item.ItemSlot)).GetEnumerator();
        }

        /// <inheritdoc />
        IEnumerator IEnumerable.GetEnumerator()
        {
            return this.GetEnumerator();
        }

        /// <inheritdoc />
        public void Add(Item item)
        {
            this._actualCollection.Add(item);
        }

        /// <inheritdoc />
        public void Clear()
        {
            var itemsToRemove = this.ToList();
            itemsToRemove.ForEach(item => this._actualCollection.Remove(item));
        }

        /// <inheritdoc />
        public bool Contains(Item item) => item is { } && this.IsSlotOfThisStorage(item.ItemSlot) && this._actualCollection.Contains(item);

        /// <inheritdoc />
        public void CopyTo(Item[] array, int arrayIndex)
        {
            var i = arrayIndex;
            foreach (var item in this)
            {
                array[i] = item;
                i++;
            }
        }

        /// <inheritdoc />
        public bool Remove(Item item)
        {
            if (this.Contains(item))
            {
                return this._actualCollection.Remove(item);
            }

            return false;
        }

        private bool IsSlotOfThisStorage(byte itemSlot)
        {
            return itemSlot >= this._firstItemSlot && itemSlot < this._firstItemSlot + this._itemSlotCount;
        }
    }
}