// <copyright file="ShortDictionary{T}.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

using System.Diagnostics.CodeAnalysis;

namespace MUnique.OpenMU.GameLogic.DataStructures
{
    using System;
    using System.Collections.Generic;
    using System.Linq;

    /// <summary>
    /// An extremely fast dictionary with <see cref="ushort"/> as key. It is using an array with a fixed size internally.
    /// </summary>
    /// <typeparam name="T">The type of the value.</typeparam>
    public class ShortDictionary<T> : IDictionary<ushort, T>
        where T : class
    {
        private const ushort NullValue = 0xFFFF;

        private readonly IList<T?> list;

        private readonly ushort[] mapping;

        private readonly Queue<ushort> freeKeys;

        private int count;

        /// <summary>
        /// Initializes a new instance of the <see cref="ShortDictionary{T}"/> class.
        /// </summary>
        public ShortDictionary()
        {
            this.list = new List<T?>();
            this.mapping = new ushort[0x10000];
            for (int i = 0; i < this.mapping.Length; ++i)
            {
                this.mapping[i] = NullValue;
            }

            this.freeKeys = new Queue<ushort>();
            this.SyncRoot = new object();
        }

        /// <summary>
        /// Gets or sets the synchronize root.
        /// </summary>
        public object SyncRoot { get; set; }

        /// <inheritdoc/>
        public ICollection<ushort> Keys
        {
            get
            {
                var keys = new List<ushort>();
                for (int i = 0; i < this.mapping.Length; ++i)
                {
                    if (this.mapping[i] != NullValue)
                    {
                        keys.Add(this.mapping[i]);
                    }
                }

                return keys;
            }
        }

        /// <inheritdoc/>
        public ICollection<T> Values
        {
            get
            {
                var result = new List<T>();
                for (int i = 0; i < this.mapping.Length; ++i)
                {
                    if (this.mapping[i] != NullValue)
                    {
                        result.Add(this.list[this.mapping[i]]!);
                    }
                }

                return result;
            }
        }

        /// <inheritdoc/>
        public int Count => this.count;

        /// <inheritdoc/>
        public bool IsReadOnly => false;

        /// <inheritdoc/>
        public T this[ushort key]
        {
            get
            {
                if (this.ContainsKey(key))
                {
                    return this.list[this.mapping[key]]!;
                }

                throw new KeyNotFoundException();
            }

            set
            {
                if (this.ContainsKey(key))
                {
                    this.list[this.mapping[key]] = value;
                }
                else
                {
                    throw new KeyNotFoundException();
                }
            }
        }

        /// <inheritdoc/>
        public void Add(ushort key, T value)
        {
            lock (this.SyncRoot)
            {
                if (key == NullValue)
                {
                    throw new ArgumentException($"Key {NullValue} is not supported.");
                }

                if (this.ContainsKey(key))
                {
                    throw new ArgumentException($"Key {key} already exists.");
                }

                ushort newIndex;
                if (this.freeKeys.Count > 0)
                {
                    newIndex = this.freeKeys.Dequeue();
                }
                else
                {
                    newIndex = (ushort)this.list.Count;
                }

                this.list.Add(value);
                ++this.count;
                this.mapping[key] = newIndex;
            }
        }

        /// <inheritdoc/>
        public bool ContainsKey(ushort key)
        {
            return this.mapping[key] != NullValue;
        }

        /// <inheritdoc/>
        public bool Remove(ushort key)
        {
            lock (this.SyncRoot)
            {
                if (!this.ContainsKey(key))
                {
                    return false;
                }

                this.freeKeys.Enqueue(this.mapping[key]);
                this.list[this.mapping[key]] = default(T);
                this.mapping[key] = NullValue;
                --this.count;
            }

            this.TryClearQueue();
            return true;
        }

        /// <inheritdoc/>
        public bool TryGetValue(ushort key, [MaybeNullWhen(false)] out T value)
        {
            value = default;
            if (this.ContainsKey(key))
            {
                value = this.list[this.mapping[key]]!;
                return true;
            }

            return false;
        }

        /// <inheritdoc/>
        public void Add(KeyValuePair<ushort, T> item)
        {
            this.Add(item.Key, item.Value);
        }

        /// <inheritdoc/>
        public void Clear()
        {
            lock (this.SyncRoot)
            {
                for (int i = 0; i < this.mapping.Length; ++i)
                {
                    this.mapping[i] = NullValue;
                }

                this.list.Clear();
                this.freeKeys.Clear();
                this.count = 0;
            }
        }

        /// <inheritdoc/>
        public bool Contains(KeyValuePair<ushort, T> item)
        {
            return this.ContainsKey(item.Key);
        }

        /// <inheritdoc/>
        public void CopyTo(KeyValuePair<ushort, T>[] array, int arrayIndex)
        {
            IList<T> values;
            IList<ushort> keys;
            lock (this.SyncRoot)
            {
                values = this.Values.ToList();
                keys = this.Keys.ToList();
            }

            int j = arrayIndex;
            for (ushort i = 0; i < keys.Count; ++i)
            {
                array[j] = new KeyValuePair<ushort, T>(keys[i], values[i]);
                ++j;
            }
        }

        /// <inheritdoc/>
        public bool Remove(KeyValuePair<ushort, T> item)
        {
            return this.Remove(item.Key);
        }

        /// <inheritdoc/>
        public IEnumerator<KeyValuePair<ushort, T>> GetEnumerator()
        {
            for (ushort i = 0; i < this.mapping.Length; ++i)
            {
                if (this.mapping[i] != NullValue)
                {
                    yield return new KeyValuePair<ushort, T>(i, this.list[this.mapping[i]]!);
                }
            }
        }

        /// <inheritdoc/>
        System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator()
        {
            return this.list.GetEnumerator();
        }

        private void TryClearQueue()
        {
            if (this.count == 0)
            {
                this.freeKeys.Clear();
            }
        }
    }
}
