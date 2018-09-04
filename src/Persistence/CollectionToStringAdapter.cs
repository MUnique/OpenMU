// <copyright file="CollectionToStringAdapter.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.Persistence
{
    using System;
    using System.Collections;
    using System.Collections.Generic;
    using System.ComponentModel;

    /// <summary>
    /// This is a adapter which adapts a collection to a string value.
    /// </summary>
    /// <typeparam name="T">The type of objects which are contained in the collection.</typeparam>
    public class CollectionToStringAdapter<T> : ICollection<T>
        where T : struct
    {
        /// <summary>
        /// The inner collection, holding the values.
        /// </summary>
        private readonly IList<T> innerCollection = new List<T>();

        /// <summary>
        /// The action which updates the string.
        /// </summary>
        private readonly Action<string> updateAction;

        /// <summary>
        /// Initializes a new instance of the <see cref="CollectionToStringAdapter{T}" /> class.
        /// </summary>
        /// <param name="rawString">The raw string.</param>
        /// <param name="updateAction">The update action.</param>
        public CollectionToStringAdapter(string rawString, Action<string> updateAction)
            : this(rawString, updateAction, ";")
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="CollectionToStringAdapter{T}" /> class.
        /// </summary>
        /// <param name="rawString">The raw string.</param>
        /// <param name="updateAction">The update action.</param>
        /// <param name="itemSeparator">The item separator.</param>
        public CollectionToStringAdapter(string rawString, Action<string> updateAction, string itemSeparator)
        {
            this.ItemSeparator = itemSeparator;
            this.ReadValuesFromString(rawString);
            this.updateAction = updateAction;
        }

        /// <summary>
        /// Gets the item separator.
        /// </summary>
        public string ItemSeparator { get; }

        /// <inheritdoc />
        public int Count => this.innerCollection.Count;

        /// <inheritdoc/>
        public bool IsReadOnly => this.innerCollection.IsReadOnly;

        /// <inheritdoc />
        public void Add(T item)
        {
            this.innerCollection.Add(item);
            this.UpdateString();
        }

        /// <inheritdoc />
        public void Clear()
        {
            this.innerCollection.Clear();
            this.UpdateString();
        }

        /// <inheritdoc />
        public bool Contains(T item) => this.innerCollection.Contains(item);

        /// <inheritdoc />
        public void CopyTo(T[] array, int arrayIndex) => this.innerCollection.CopyTo(array, arrayIndex);

        /// <inheritdoc />
        public IEnumerator<T> GetEnumerator() => this.innerCollection.GetEnumerator();

        /// <inheritdoc />
        public bool Remove(T item)
        {
            if (this.innerCollection.Remove(item))
            {
                this.UpdateString();
                return true;
            }

            return false;
        }

        /// <inheritdoc />
        IEnumerator IEnumerable.GetEnumerator() => this.innerCollection.GetEnumerator();

        private void UpdateString()
        {
            var result = string.Join(this.ItemSeparator, this.innerCollection);
            this.updateAction(result);
        }

        private void ReadValuesFromString(string rawString)
        {
            if (rawString == null)
            {
                return;
            }

            var itemStrings = rawString.Split(new[] { this.ItemSeparator }, StringSplitOptions.RemoveEmptyEntries);
            if (itemStrings.Length == 0)
            {
                return;
            }

            var converter = TypeDescriptor.GetConverter(typeof(T));
            foreach (var itemString in itemStrings)
            {
                var converted = converter.ConvertFromInvariantString(itemString);
                if (converted != null)
                {
                    this.innerCollection.Add((T)converted);
                }
            }
        }
    }
}
