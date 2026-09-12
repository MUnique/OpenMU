// <copyright file="BinaryMinHeap{T}.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.Pathfinding;

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
    private readonly List<T> _innerList = new();
    private readonly IComparer<T> _elementComparer;

    /// <summary>
    /// Initializes a new instance of the <see cref="BinaryMinHeap{T}"/> class.
    /// </summary>
    public BinaryMinHeap()
    {
        this._elementComparer = Comparer<T>.Default;
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="BinaryMinHeap{T}"/> class.
    /// </summary>
    /// <param name="comparer">The comparer.</param>
    public BinaryMinHeap(IComparer<T> comparer)
    {
        this._elementComparer = comparer;
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="BinaryMinHeap{T}"/> class.
    /// </summary>
    /// <param name="comparer">The comparer.</param>
    /// <param name="capacity">The capacity.</param>
    public BinaryMinHeap(IComparer<T> comparer, int capacity)
    {
        this._elementComparer = comparer;
        this._innerList.Capacity = capacity;
    }

    /// <inheritdoc/>
    public int Count => this._innerList.Count;

    /// <inheritdoc/>
    public void Push(T item)
    {
        var index = this._innerList.Count;
        this._innerList.Add(item);
        while (index > 0)
        {
            var parentIndex = unchecked(index - 1) >> 1;
            if (this.Compare(index, parentIndex) >= 0)
            {
                break;
            }

            this.Swap(index, parentIndex);
            index = parentIndex;
        }
    }

    /// <inheritdoc/>
    public T Pop()
    {
        if (this.Count == 0)
        {
            throw new InvalidOperationException("Heap is empty");
        }

        var result = this._innerList[0];
        var index = 0;
        this._innerList[0] = this._innerList[^1];
        this._innerList.RemoveAt(this._innerList.Count - 1);
        while (true)
        {
            var smallest = index;
            var left = unchecked((index << 1) + 1);
            var right = unchecked((index << 1) + 2);
            if (this._innerList.Count > left && this.Compare(index, left) > 0)
            {
                index = left;
            }

            if (this._innerList.Count > right && this.Compare(index, right) > 0)
            {
                index = right;
            }

            if (index == smallest)
            {
                break;
            }

            this.Swap(smallest, index);
        }

        return result;
    }

    /// <summary>
    /// Get the smallest object without removing it.
    /// </summary>
    /// <returns>The smallest object.</returns>
    public T Peek()
    {
        if (this._innerList.Count > 0)
        {
            return this._innerList[0];
        }

        throw new InvalidOperationException("Heap is empty");
    }

    /// <inheritdoc/>
    public void Clear()
    {
        this._innerList.Clear();
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private void Swap(int i, int j)
    {
        (this._innerList[i], this._innerList[j]) = (this._innerList[j], this._innerList[i]);
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private int Compare(int i, int j)
    {
        return this._elementComparer.Compare(this._innerList[i], this._innerList[j]);
    }
}