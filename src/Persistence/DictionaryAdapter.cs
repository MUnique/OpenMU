// <copyright file="DictionaryAdapter.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.Persistence
{
    using System;
    using System.Collections;
    using System.Collections.Generic;
    using System.Linq;

    /// <summary>
    /// An adapter which adapts a <see cref="IDictionary{TKey, TPersistent}"/> to a <see cref="IDictionary{TKey, TClass}"/>.
    /// </summary>
    /// <typeparam name="TKey">The type of the key.</typeparam>
    /// <typeparam name="TClass">The type of the base class.</typeparam>
    /// <typeparam name="TPersistent">The type of the persistent class.</typeparam>
    /// <seealso cref="System.Collections.Generic.IDictionary{TKey, TClass}" />
    public class DictionaryAdapter<TKey, TClass, TPersistent> : IDictionary<TKey, TClass>
        where TPersistent : class, TClass, IIdentifiable
        where TClass : class
    {
        private readonly IDictionary<TKey, TPersistent> innerDictionary;

        /// <summary>
        /// Initializes a new instance of the <see cref="DictionaryAdapter{TKey, TClass, TPersistent}"/> class.
        /// </summary>
        /// <param name="innerDictionary">The inner dictionary which will get adapted.</param>
        public DictionaryAdapter(IDictionary<TKey, TPersistent> innerDictionary)
        {
            this.innerDictionary = innerDictionary;
        }

        /// <inheritdoc/>
        public ICollection<TKey> Keys => this.innerDictionary.Select(join => join.Key).ToList();

        /// <inheritdoc/>
        public ICollection<TClass> Values => this.innerDictionary.Select(join => join.Value).OfType<TClass>().ToList();

        /// <inheritdoc/>
        public int Count => this.innerDictionary.Count;

        /// <inheritdoc/>
        public bool IsReadOnly => this.innerDictionary.IsReadOnly;

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
            foreach (var item in this.innerDictionary)
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
            this.innerDictionary.Clear();
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
            return this.innerDictionary.Any(join => object.Equals(join.Key, key));
        }

        /// <inheritdoc/>
        public void Add(TKey key, TClass value)
        {
            if (value is TPersistent persistentValue)
            {
                this.innerDictionary.Add(key, persistentValue);
            }
            else
            {
                throw new ArgumentException($"{nameof(value)} is not of type {typeof(TPersistent)}");
            }
        }

        /// <inheritdoc/>
        public bool Remove(TKey key)
        {
            return this.innerDictionary.Remove(key);
        }

        /// <inheritdoc/>
        public bool TryGetValue(TKey key, out TClass value)
        {
            if (this.innerDictionary.TryGetValue(key, out var persistentValue))
            {
                value = persistentValue;
                return true;
            }

            value = null;
            return false;
        }
    }
}
