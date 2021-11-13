// <copyright file="ShortDictionary{T}.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.GameLogic;

using System.Diagnostics.CodeAnalysis;

/// <summary>
/// An extremely fast dictionary with <see cref="ushort"/> as key. It is using an array with a fixed size internally.
/// </summary>
/// <typeparam name="T">The type of the value.</typeparam>
public class ShortDictionary<T> : IDictionary<ushort, T>
    where T : class
{
    private const ushort NullValue = 0xFFFF;

    private readonly IList<T?> _list;

    private readonly ushort[] _mapping;

    private readonly Queue<ushort> _freeKeys;

    private int _count;

    /// <summary>
    /// Initializes a new instance of the <see cref="ShortDictionary{T}"/> class.
    /// </summary>
    public ShortDictionary()
    {
        this._list = new List<T?>();
        this._mapping = new ushort[0x10000];
        for (int i = 0; i < this._mapping.Length; ++i)
        {
            this._mapping[i] = NullValue;
        }

        this._freeKeys = new Queue<ushort>();
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
            for (int i = 0; i < this._mapping.Length; ++i)
            {
                if (this._mapping[i] != NullValue)
                {
                    keys.Add(this._mapping[i]);
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
            for (int i = 0; i < this._mapping.Length; ++i)
            {
                if (this._mapping[i] != NullValue)
                {
                    result.Add(this._list[this._mapping[i]]!);
                }
            }

            return result;
        }
    }

    /// <inheritdoc/>
    public int Count => this._count;

    /// <inheritdoc/>
    public bool IsReadOnly => false;

    /// <inheritdoc/>
    public T this[ushort key]
    {
        get
        {
            if (this.ContainsKey(key))
            {
                return this._list[this._mapping[key]]!;
            }

            throw new KeyNotFoundException();
        }

        set
        {
            if (this.ContainsKey(key))
            {
                this._list[this._mapping[key]] = value;
            }
            else
            {
                throw new KeyNotFoundException();
            }
        }
    }

    /// <inheritdoc/>
    public void Add(KeyValuePair<ushort, T> item)
    {
        this.Add(item.Key, item.Value);
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
            if (this._freeKeys.Count > 0)
            {
                newIndex = this._freeKeys.Dequeue();
            }
            else
            {
                newIndex = (ushort)this._list.Count;
            }

            this._list.Add(value);
            ++this._count;
            this._mapping[key] = newIndex;
        }
    }

    /// <inheritdoc/>
    public bool ContainsKey(ushort key)
    {
        return this._mapping[key] != NullValue;
    }

    /// <inheritdoc/>
    public bool Remove(KeyValuePair<ushort, T> item)
    {
        return this.Remove(item.Key);
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

            this._freeKeys.Enqueue(this._mapping[key]);
            this._list[this._mapping[key]] = default(T);
            this._mapping[key] = NullValue;
            --this._count;
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
            value = this._list[this._mapping[key]]!;
            return true;
        }

        return false;
    }

    /// <inheritdoc/>
    public void Clear()
    {
        lock (this.SyncRoot)
        {
            for (int i = 0; i < this._mapping.Length; ++i)
            {
                this._mapping[i] = NullValue;
            }

            this._list.Clear();
            this._freeKeys.Clear();
            this._count = 0;
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
    public IEnumerator<KeyValuePair<ushort, T>> GetEnumerator()
    {
        for (ushort i = 0; i < this._mapping.Length; ++i)
        {
            if (this._mapping[i] != NullValue)
            {
                yield return new KeyValuePair<ushort, T>(i, this._list[this._mapping[i]]!);
            }
        }
    }

    /// <inheritdoc/>
    System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator()
    {
        return this._list.GetEnumerator();
    }

    private void TryClearQueue()
    {
        if (this._count == 0)
        {
            this._freeKeys.Clear();
        }
    }
}