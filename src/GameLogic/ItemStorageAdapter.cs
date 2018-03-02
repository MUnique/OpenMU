// <copyright file="ItemStorageAdapter.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.GameLogic
{
    using System.Collections;
    using System.Collections.Generic;
    using System.Linq;
    using MUnique.OpenMU.DataModel.Entities;

    /// <summary>
    /// A wrapper for another <see cref="ItemStorage"/>.
    /// Required to split one item storage into more than one storage spaces, e.g. Inventory and Personal Store which use the same ItemStorage.
    /// </summary>
    /// <seealso cref="MUnique.OpenMU.DataModel.Entities.ItemStorage" />
    public class ItemStorageAdapter : ItemStorage
    {
        private readonly CollectionAdapter adapter;

        /// <summary>
        /// Initializes a new instance of the <see cref="ItemStorageAdapter"/> class.
        /// </summary>
        /// <param name="actualStorage">The actual storage.</param>
        /// <param name="firstItemSlot">The first item slot.</param>
        /// <param name="itemSlotCount">The item slot count.</param>
        public ItemStorageAdapter(ItemStorage actualStorage, byte firstItemSlot, byte itemSlotCount)
        {
            this.adapter = new CollectionAdapter(actualStorage.Items, firstItemSlot, itemSlotCount);
            this.ActualStorage = actualStorage;
        }

        /// <inheritdoc />
        public override ICollection<Item> Items => this.adapter;

        /// <summary>
        /// Gets the actual storage which is wrapped by this instance.
        /// </summary>
        public ItemStorage ActualStorage { get; }

        /// <summary>
        /// A collection adapter which just returns items between certain item slots.
        /// </summary>
        private class CollectionAdapter : ICollection<Item>
        {
            private readonly ICollection<Item> actualCollection;
            private readonly byte firstItemSlot;
            private readonly byte itemSlotCount;

            public CollectionAdapter(ICollection<Item> actualCollection, byte firstItemSlot, byte itemSlotCount)
            {
                this.actualCollection = actualCollection;
                this.firstItemSlot = firstItemSlot;
                this.itemSlotCount = itemSlotCount;
            }

            /// <inheritdoc />
            public int Count => this.actualCollection.Count(i => this.IsSlotOfThisStorage(i.ItemSlot));

            /// <inheritdoc />
            public bool IsReadOnly => false;

            /// <inheritdoc />
            public IEnumerator<Item> GetEnumerator()
            {
                return this.actualCollection.Where(item => this.IsSlotOfThisStorage(item.ItemSlot)).GetEnumerator();
            }

            /// <inheritdoc />
            IEnumerator IEnumerable.GetEnumerator()
            {
                return this.GetEnumerator();
            }

            /// <inheritdoc />
            public void Add(Item item)
            {
                this.actualCollection.Add(item);
            }

            /// <inheritdoc />
            public void Clear()
            {
                var itemsToRemove = this.ToList();
                itemsToRemove.ForEach(item => this.actualCollection.Remove(item));
            }

            /// <inheritdoc />
            public bool Contains(Item item) => item != null && this.IsSlotOfThisStorage(item.ItemSlot) && this.actualCollection.Contains(item);

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
                    return this.actualCollection.Remove(item);
                }

                return false;
            }

            private bool IsSlotOfThisStorage(byte itemSlot)
            {
                return itemSlot >= this.firstItemSlot && itemSlot < this.firstItemSlot + this.itemSlotCount;
            }
        }
    }
}