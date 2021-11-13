// <copyright file="Bucket{T}.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.GameLogic;

using System.Collections;
using System.Threading;

/// <summary>
/// A bucket, which can be observed for added and removed items.
/// </summary>
/// <typeparam name="T">The type which should be hold by this bucket.</typeparam>
public sealed class Bucket<T> : IEnumerable<T>, IDisposable
{
    private readonly List<T> _innerList;

    private readonly ReaderWriterLockSlim _locker = new ();

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
    public event EventHandler<BucketItemEventArgs<T>>? ItemAdded;

    /// <summary>
    /// Occurs when an item has been removed.
    /// </summary>
    public event EventHandler<BucketItemEventArgs<T>>? ItemRemoved;

    /// <summary>
    /// Gets the count.
    /// </summary>
    public int Count => this._innerList.Count;

    /// <summary>
    /// Adds the specified item.
    /// </summary>
    /// <param name="item">The item.</param>
    public void Add(T item)
    {
        this._locker.EnterWriteLock();
        try
        {
            this._innerList.Add(item);
        }
        finally
        {
            this._locker.ExitWriteLock();
        }

        this.ItemAdded?.Invoke(this, new BucketItemEventArgs<T>(item));
    }

    /// <summary>
    /// Removes the specified item.
    /// </summary>
    /// <param name="item">The item.</param>
    /// <returns>The success.</returns>
    public bool Remove(T item)
    {
        bool result;
        this._locker.EnterWriteLock();
        try
        {
            result = this._innerList.Remove(item);
        }
        finally
        {
            this._locker.ExitWriteLock();
        }

        if (result)
        {
            this.ItemRemoved?.Invoke(this, new BucketItemEventArgs<T>(item));
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

    /// <inheritdoc/>
    public void Dispose()
    {
        this._locker.Dispose();
    }

    private sealed class LockingEnumerator<TEnumerated> : IEnumerator<TEnumerated>
    {
        private readonly ReaderWriterLockSlim _locker;
        private readonly IEnumerator<TEnumerated> _enumerator;

        public LockingEnumerator(ReaderWriterLockSlim locker, IEnumerator<TEnumerated> innerEnumerator)
        {
            this._locker = locker;

            locker.EnterReadLock();
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
            this._locker.ExitReadLock();
        }
    }
}