// <copyright file="RingBuffer.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.Web.Shared;

using MUnique.OpenMU.GameLogic;

/// <summary>
/// A ring buffer of <typeparamref name="T"/>.
/// </summary>
/// <typeparam name="T">The type of items which should be kept inside this instance.</typeparam>
internal class RingBuffer<T>
{
    private readonly T[] _array;

    private int _currentStartIndex;

    private int _count;

    /// <summary>
    /// Initializes a new instance of the <see cref="RingBuffer{T}"/> class.
    /// </summary>
    /// <param name="size">The size (capacity) of the buffer.</param>
    public RingBuffer(int size)
    {
        this._array = new T[size];
    }

    /// <summary>
    /// Gets the size (capacity) of the buffer.
    /// </summary>
    public int Size => this._array.Length;

    /// <summary>
    /// Gets the enumerable.
    /// </summary>
    /// <returns>The enumerable.</returns>
    public IEnumerable<T> GetEnumerable()
    {
        lock (this._array)
        {
            for (int i = 0; i < this._array.Length; i++)
            {
                var index = (i + this._currentStartIndex) % this._array.Length;
                if (this._array[index] is { } notNull)
                {
                    yield return notNull;
                }
                else
                {
                    yield break;
                }
            }
        }
    }

    /// <summary>
    /// Adds the specified item.
    /// </summary>
    /// <param name="item">The item.</param>
    public void Add(T item)
    {
        lock (this._array)
        {
            this._currentStartIndex = this._count >= this.Size ? (this._count - 1) % this.Size : 0;
            var nextIndex = this._count % this.Size;
            this._array[nextIndex] = item;
            this._count++;
        }
    }

    /// <summary>
    /// Clears this instance.
    /// </summary>
    public void Clear()
    {
        lock (this._array)
        {
            this._array.ClearToDefaults();
            this._currentStartIndex = 0;
            this._count = 0;
        }
    }
}