// <copyright file="RingBuffer.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.AdminPanel
{
    using System.Collections.Generic;
    using MUnique.OpenMU.GameLogic;

    /// <summary>
    /// A ring buffer of <typeparamref name="T"/>.
    /// </summary>
    /// <typeparam name="T">The type of items which should be kept inside this instance.</typeparam>
    internal class RingBuffer<T>
    {
        private readonly T[] array;

        private int currentStartIndex;

        private int count;

        /// <summary>
        /// Initializes a new instance of the <see cref="RingBuffer{T}"/> class.
        /// </summary>
        /// <param name="size">The size (capacity) of the buffer.</param>
        public RingBuffer(int size)
        {
            this.array = new T[size];
        }

        /// <summary>
        /// Gets the size (capacity) of the buffer.
        /// </summary>
        public int Size => this.array.Length;

        /// <summary>
        /// Gets the enumerable.
        /// </summary>
        /// <returns>The enumerable.</returns>
        public IEnumerable<T> GetEnumerable()
        {
            lock (this.array)
            {
                for (int i = 0; i < this.array.Length; i++)
                {
                    var index = (i + this.currentStartIndex) % this.array.Length;
                    if (this.array[index] is { } notNull)
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
            lock (this.array)
            {
                this.currentStartIndex = this.count >= this.Size ? (this.count - 1) % this.Size : 0;
                var nextIndex = this.count % this.Size;
                this.array[nextIndex] = item;
                this.count++;
            }
        }

        /// <summary>
        /// Clears this instance.
        /// </summary>
        public void Clear()
        {
            lock (this.array)
            {
                this.array.ClearToDefaults();
                this.currentStartIndex = 0;
                this.count = 0;
            }
        }
    }
}