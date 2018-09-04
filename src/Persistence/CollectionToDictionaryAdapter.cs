// <copyright file="CollectionToDictionaryAdapter.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.Persistence
{
    using System.Collections;
    using System.Collections.Generic;
    using System.Linq;

    /// <summary>
    /// A adapter which adapts from a mapped collection of join entities to a dictionary interface.
    /// </summary>
    /// <typeparam name="TKey">The type of the key.</typeparam>
    /// <typeparam name="TClass">The type of the class.</typeparam>
    /// <typeparam name="TPersistent">The persistent type of <typeparamref name="TClass"/>.</typeparam>
    /// <typeparam name="TJoin">The type of the join.</typeparam>
    /// <seealso cref="System.Collections.Generic.IDictionary{TKey, TClass}" />
    public class CollectionToDictionaryAdapter<TKey, TClass, TPersistent, TJoin> : IDictionary<TKey, TClass>
        where TJoin : IDictionaryEntity<TKey, TPersistent>, new()
        where TPersistent : class, TClass, IIdentifiable
        where TClass : class
    {
        private readonly ICollection<TJoin> rawCollection;

        /// <summary>
        /// Initializes a new instance of the <see cref="CollectionToDictionaryAdapter{TKey, TClass, TEfCore, TJoin}"/> class.
        /// </summary>
        /// <param name="rawCollection">The raw collection.</param>
        public CollectionToDictionaryAdapter(ICollection<TJoin> rawCollection)
        {
            this.rawCollection = rawCollection;
        }

        /// <inheritdoc/>
        public ICollection<TKey> Keys => this.rawCollection.Select(join => join.Key).ToList();

        /// <inheritdoc/>
        public ICollection<TClass> Values => this.rawCollection.Select(join => join.Value).OfType<TClass>().ToList();

        /// <inheritdoc/>
        public int Count => this.rawCollection.Count;

        /// <inheritdoc/>
        public bool IsReadOnly => this.rawCollection.IsReadOnly;

        /// <inheritdoc/>
        public TClass this[TKey key]
        {
            get
            {
                this.TryGetValue(key, out TClass value);
                return value;
            }

            set => this.Add(key, value);
        }

        /// <inheritdoc/>
        public IEnumerator<KeyValuePair<TKey, TClass>> GetEnumerator()
        {
            foreach (var item in this.rawCollection)
            {
                yield return new KeyValuePair<TKey, TClass>(item.Key, item.Value);
            }
        }

        /// <inheritdoc/>
        IEnumerator IEnumerable.GetEnumerator()
        {
            return this.GetEnumerator();
        }

        /// <inheritdoc/>
        public void Add(KeyValuePair<TKey, TClass> item)
        {
            this.Add(item.Key, item.Value);
        }

        /// <inheritdoc/>
        public void Clear()
        {
            this.rawCollection.Clear();
        }

        /// <inheritdoc/>
        public bool Contains(KeyValuePair<TKey, TClass> item)
        {
            return this.ContainsKey(item.Key);
        }

        /// <inheritdoc/>
        public void CopyTo(KeyValuePair<TKey, TClass>[] array, int arrayIndex)
        {
            var keyValuePairs = this.ToList();
            keyValuePairs.CopyTo(array, arrayIndex);
        }

        /// <inheritdoc/>
        public bool Remove(KeyValuePair<TKey, TClass> item)
        {
            return this.Remove(item.Key);
        }

        /// <inheritdoc/>
        public bool ContainsKey(TKey key)
        {
            return this.rawCollection.Any(join => object.Equals(join.Key, key));
        }

        /// <inheritdoc/>
        public void Add(TKey key, TClass value)
        {
            this.rawCollection.Add(new TJoin { Key = key, Value = (TPersistent)value, ValueId = ((TPersistent)value).Id });
        }

        /// <inheritdoc/>
        public bool Remove(TKey key)
        {
            var joinItem = this.rawCollection.FirstOrDefault(join => object.Equals(join.Key, key));
            return this.rawCollection.Remove(joinItem);
        }

        /// <inheritdoc/>
        public bool TryGetValue(TKey key, out TClass value)
        {
            value = this.rawCollection.FirstOrDefault(join => object.Equals(join.Key, key))?.Value;
            return value != null;
        }
    }
}
