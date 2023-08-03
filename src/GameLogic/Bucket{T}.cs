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
            await eventHandler(item).ConfigureAwait(false);
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
            await eventHandler(item).ConfigureAwait(false);
        }

        return result;
    }

    /// <inheritdoc/>
    public IEnumerator<T> GetEnumerator()
    {
        return new LockingEnumerator<T>(this._locker, this._innerList);
    }

    /// <inheritdoc/>
    IEnumerator IEnumerable.GetEnumerator()
    {
        return this.GetEnumerator();
    }

    private sealed class LockingEnumerator<TEnumerated> : IEnumerator<TEnumerated>
    {
        private readonly AsyncReaderWriterLock _locker;
        private readonly IEnumerable<TEnumerated> _enumerable;
        private IEnumerator<TEnumerated>? _enumerator;
        private IDisposable? _lockRelease;

        public LockingEnumerator(AsyncReaderWriterLock locker, IEnumerable<TEnumerated> enumerable)
        {
            this._locker = locker;
            this._enumerable = enumerable;
        }

        private IEnumerator<TEnumerated> Enumerator
        {
            get
            {
                if (this._enumerator is { })
                {
                    return this._enumerator;
                }

                this._lockRelease = this._locker.ReaderLock();
                return this._enumerator ??= this._enumerable.GetEnumerator();
            }
        }

        public TEnumerated Current => this.Enumerator.Current;

        object IEnumerator.Current => this.Enumerator.Current!;

        public bool MoveNext()
        {
            return this.Enumerator.MoveNext();
        }

        public void Reset()
        {
            this.Enumerator.Reset();
        }

        public void Dispose()
        {
            this._enumerator?.Dispose();
            this._lockRelease?.Dispose();
        }
    }
}