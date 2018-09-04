// <copyright file="CollectionAdapter.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.Persistence
{
    using System;
    using System.Collections;
    using System.Collections.Generic;
    using System.Collections.Specialized;
    using System.Linq;

    /// <summary>
    /// A collection adapter which adapts a <see cref="ICollection{TEfCore}"/> to an <see cref="ICollection{TClass}"/>,
    /// if <typeparamref name="TEfCore" /> inherits from <typeparamref name="TClass"/>.
    /// </summary>
    /// <typeparam name="TClass">The type of the class.</typeparam>
    /// <typeparam name="TEfCore">The type of the ef core.</typeparam>
    /// <seealso cref="System.Collections.Generic.ICollection{TClass}" />
    public class CollectionAdapter<TClass, TEfCore> : ICollection<TClass>, INotifyCollectionChanged
        where TEfCore : TClass
    {
        /// <summary>
        /// The raw collection which is the actually mapped collection by entity framework.
        /// </summary>
        private readonly ICollection<TEfCore> rawCollection;

        /// <summary>
        /// Initializes a new instance of the <see cref="CollectionAdapter{TClass, TEfCore}"/> class.
        /// </summary>
        /// <param name="rawCollection">The raw collection which is the actually mapped collection by entity framework.</param>
        public CollectionAdapter(ICollection<TEfCore> rawCollection)
        {
            this.rawCollection = rawCollection;
        }

        /// <inheritdoc/>
        public event NotifyCollectionChangedEventHandler CollectionChanged;

        /// <inheritdoc />
        public int Count => this.rawCollection.Count;

        /// <inheritdoc />
        public bool IsReadOnly => this.rawCollection.IsReadOnly;

        /// <inheritdoc />
        public IEnumerator<TClass> GetEnumerator()
        {
            return this.rawCollection.OfType<TClass>().GetEnumerator();
        }

        /// <summary>
        /// Returns an enumerator that iterates through a collection.
        /// </summary>
        /// <returns>
        /// An <see cref="T:System.Collections.IEnumerator" /> object that can be used to iterate through the collection.
        /// </returns>
        IEnumerator IEnumerable.GetEnumerator()
        {
            return this.rawCollection.GetEnumerator();
        }

        /// <inheritdoc />
        public void Add(TClass item)
        {
            if (!(item is TEfCore))
            {
                throw new ArgumentException($"The item needs to be of type {typeof(TEfCore)}.", nameof(item));
            }

            this.rawCollection.Add((TEfCore)item);
            this.CollectionChanged?.Invoke(this, new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Add, new List<TEfCore> { (TEfCore)item }));
        }

        /// <inheritdoc />
        public void Clear()
        {
            var items = this.rawCollection.ToList();
            this.rawCollection.Clear();
            this.CollectionChanged?.Invoke(this, new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Reset, items));
        }

        /// <inheritdoc />
        public bool Contains(TClass item)
        {
            if (!(item is TEfCore))
            {
                return false;
            }

            return this.rawCollection.Contains((TEfCore)item);
        }

        /// <inheritdoc />
        public void CopyTo(TClass[] array, int arrayIndex)
        {
            int i = 0;
            foreach (var item in this.rawCollection)
            {
                array[arrayIndex + i] = item;
                i++;
            }
        }

        /// <inheritdoc />
        public bool Remove(TClass item)
        {
            if (!(item is TEfCore))
            {
                return false;
            }

            if (this.rawCollection.Remove((TEfCore)item))
            {
                this.CollectionChanged?.Invoke(this, new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Remove, new List<TEfCore> { (TEfCore)item }));
                return true;
            }

            return false;
        }
    }
}
