// <copyright file="Bucket{T}.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.GameLogic;

using System.Collections;
using Nito.AsyncEx;
using MUnique.OpenMU.PlugIns;

/// <summary>
/// A bucket, which can be observed for added and removed items.
/// </summary>
/// <typeparam name="T">The type which should be hold by this bucket.</typeparam>
public sealed class Bucket<T> : IEnumerable<T>
{
    private readonly List<T> _innerList;

    private readonly AsyncReaderWriterLock _locker = new ();

    /// <summary>
    /// Initializes a new instance of the <see cref="Bucket{T}"/> class.
    /// </summary>
    /// <param name="capacity">The initial capacity.</param>
    public Bucket(int capacity)
    {
        this._innerList = new List<T>(capacity);
    }

    /// <summary>
    /// Occurs when an item has been added.
    /// </summary>
    public event AsyncEventHandler<T>? ItemAdded;

    /// <summary>
    /// Occurs when an item has been removed.
    /// </summary>
    public event AsyncEventHandler<T>? ItemRemoved;

    /// <summary>
    /// Gets the count.
    /// </summary>
    public int Count => this._innerList.Count;

    /// <summary>
    /// Adds the specified item.
    /// </summary>
    /// <param name="item">The item.</param>
    public async ValueTask AddAsync(T item)
    {
        using (await this._locker.WriterLockAsync())
        {
            this._innerList.Add(item);
        }

        if (this.ItemAdded is { } eventHandler)
        {
            await eventHandler(item);
        }
    }

    /// <summary>
    /// Removes the specified item.
    /// </summary>
    /// <param name="item">The item.</param>
    /// <returns>The success.</returns>
    public async ValueTask<bool> RemoveAsync(T item)
    {
        bool result;
        using (await this._locker.WriterLockAsync())
        {
            result = this._innerList.Remove(item);
        }

        if (result && this.ItemRemoved is { } eventHandler)
        {
            await eventHandler(item);
        }

        return result;
    }

    /// <inheritdoc/>
    public IEnumerator<T> GetEnumerator()
    {
        return new LockingEnumerator<T>(this._locker, this._innerList.GetEnumerator());
    }

    /// <inheritdoc/>
    IEnumerator IEnumerable.GetEnumerator()
    {
        return this.GetEnumerator();
    }

    private sealed class LockingEnumerator<TEnumerated> : IEnumerator<TEnumerated>
    {
        private readonly IDisposable _lockRelease;
        private readonly IEnumerator<TEnumerated> _enumerator;

        public LockingEnumerator(AsyncReaderWriterLock locker, IEnumerator<TEnumerated> innerEnumerator)
        {
            this._lockRelease = locker.ReaderLock();
            this._enumerator = innerEnumerator;
        }

        public TEnumerated Current => this._enumerator.Current;

        object IEnumerator.Current => this.Current!;

        public bool MoveNext()
        {
            return this._enumerator.MoveNext();
        }

        public void Reset()
        {
            this._enumerator.Reset();
        }

        public void Dispose()
        {
            this._enumerator.Dispose();
            this._lockRelease.Dispose();
        }
    }
}