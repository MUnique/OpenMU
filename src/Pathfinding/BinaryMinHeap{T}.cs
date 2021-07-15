// <copyright file="BinaryMinHeap{T}.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.Pathfinding
{
    using System;
    using System.Collections.Generic;
    using System.Runtime.CompilerServices;

    /// <summary>
    /// A binary min heap which implements <see cref="IPriorityQueue{T}"/>.
    /// Objects with the lowest index values appear at the top of the heap, and will be retrieved when calling <see cref="Pop"/>.
    /// Please note: This class is not thread safe! Push/Pop should not be executed by two different threads at the same time!.
    /// </summary>
    /// <remarks>
    /// This class contains some optimizations which do not make the code nicer. However,
    /// this data structure is THE bottleneck of the pathfinding algorithm, so optimization is worth it.
    /// </remarks>
    /// <typeparam name="T">The type which should be contained in the heap.</typeparam>
    public class BinaryMinHeap<T> : IPriorityQueue<T>
    {
        private readonly List<T> innerList = new ();
        private readonly IComparer<T> elementComparer;

        /// <summary>Reused variable to reduce stack allocations.</summary>
        private int i;

        /// <summary>Reused variable to reduce stack allocations.</summary>
        private int parentIndex;

        /// <summary>Reused variable to reduce stack allocations.</summary>
        private int left;

        /// <summary>Reused variable to reduce stack allocations.</summary>
        private int right;

        /// <summary>
        /// Initializes a new instance of the <see cref="BinaryMinHeap{T}"/> class.
        /// </summary>
        public BinaryMinHeap()
        {
            this.elementComparer = Comparer<T>.Default;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="BinaryMinHeap{T}"/> class.
        /// </summary>
        /// <param name="comparer">The comparer.</param>
        public BinaryMinHeap(IComparer<T> comparer)
        {
            this.elementComparer = comparer;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="BinaryMinHeap{T}"/> class.
        /// </summary>
        /// <param name="comparer">The comparer.</param>
        /// <param name="capacity">The capacity.</param>
        public BinaryMinHeap(IComparer<T> comparer, int capacity)
        {
            this.elementComparer = comparer;
            this.innerList.Capacity = capacity;
        }

        /// <inheritdoc/>
        public int Count => this.innerList.Count;

        /// <inheritdoc/>
        public void Push(T item)
        {
            this.i = this.innerList.Count;
            this.innerList.Add(item);
            do
            {
                if (this.i == 0)
                {
                    break;
                }

                this.parentIndex = unchecked(this.i - 1) >> 1;
                if (this.OnCompareWithElementOfI(this.parentIndex) < 0)
                {
                    this.SwitchElementsParentWithI();
                    this.i = this.parentIndex;
                }
                else
                {
                    break;
                }
            }
            while (true);
        }

        /// <inheritdoc/>
        public T Pop()
        {
            if (this.Count == 0)
            {
                throw new InvalidOperationException("Heap is empty");
            }

            var result = this.innerList[0];
            this.i = 0;
            this.innerList[0] = this.innerList[^1];
            this.innerList.RemoveAt(this.innerList.Count - 1);
            do
            {
                this.parentIndex = this.i;
                this.left = unchecked((this.i << 1) + 1);
                this.right = unchecked((this.i << 1) + 2);
                if (this.innerList.Count > this.left && this.OnCompareWithElementOfI(this.left) > 0)
                {
                    this.i = this.left;
                }

                if (this.innerList.Count > this.right && this.OnCompareWithElementOfI(this.right) > 0)
                {
                    this.i = this.right;
                }

                if (this.i == this.parentIndex)
                {
                    break;
                }

                this.SwitchElementsParentWithI();
            }
            while (true);

            return result;
        }

        /// <summary>
        /// Get the smallest object without removing it.
        /// </summary>
        /// <returns>The smallest object.</returns>
        public T Peek()
        {
            if (this.innerList.Count > 0)
            {
                return this.innerList[0];
            }

            throw new InvalidOperationException("Heap is empty");
        }

        /// <inheritdoc/>
        public void Clear()
        {
            this.innerList.Clear();
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private void SwitchElementsParentWithI()
        {
            T h = this.innerList[this.i];
            this.innerList[this.i] = this.innerList[this.parentIndex];
            this.innerList[this.parentIndex] = h;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private int OnCompareWithElementOfI(int j)
        {
            return this.elementComparer.Compare(this.innerList[this.i], this.innerList[j]);
        }
    }
}
