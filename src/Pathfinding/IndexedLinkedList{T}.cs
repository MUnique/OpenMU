// <copyright file="IndexedLinkedList{T}.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.Pathfinding;

/// <summary>
/// An indexed linked list which implements <see cref="IPriorityQueue{T}"/>.
/// Objects with the lowest index values appears at the first place of the list, and will be retrieved when calling <see cref="Pop"/>.
/// </summary>
/// <remarks>
/// This list is a bit slower (~ 3x) at the <see cref="Push"/> operation,
/// but a lot faster (10x) at the <see cref="Pop"/> operation than
/// the <see cref="BinaryMinHeap{T}"/>.
/// </remarks>
/// <typeparam name="T">The type which should be contained in the heap.</typeparam>
internal class IndexedLinkedList<T> : IPriorityQueue<T>
{
    private readonly LinkedList<T> _innerList;
    private readonly IComparer<T> _comparer;
    private readonly IIndexer<T> _indexer;

    /// <summary>
    /// The index helps to find a better starting point to insert new nodes.
    /// </summary>
    private readonly IDictionary<int, LinkedListNode<T>> _index;

    /// <summary>
    /// Initializes a new instance of the <see cref="IndexedLinkedList{T}"/> class.
    /// </summary>
    /// <param name="comparer">A comparer for the type which should be conained in the heap. It is used to determine the correct position in the heap.</param>
    /// <param name="indexer">The indexer, which returns an index value which is used as key.</param>
    public IndexedLinkedList(IComparer<T> comparer, IIndexer<T> indexer)
    {
        this._innerList = new LinkedList<T>();
        this._comparer = comparer;
        this._indexer = indexer;
        this._index = new Dictionary<int, LinkedListNode<T>>();
    }

    /// <inheritdoc/>
    public int Count => this._innerList.Count;

    /// <inheritdoc/>
    public void Push(T item)
    {
        int indexValue = this._indexer.GetIndexValue(item);
        if (this._innerList.First is null)
        {
            var addedNode = this._innerList.AddFirst(item);
            this._index.Add(indexValue, addedNode);
            return;
        }

        var inIndex = true;
        if (!this._index.TryGetValue(indexValue, out var node))
        {
            node = this._innerList.First;
            inIndex = false;
        }

        while (node != null && this._comparer.Compare(item, node.Value) > 0)
        {
            node = node.Next;
        }

        if (node != null)
        {
            if (inIndex)
            {
                this._innerList.AddAfter(node, item);
            }
            else
            {
                var addedNode = this._innerList.AddAfter(node, item);
                this._index.Add(indexValue, addedNode);
            }
        }
        else
        {
            var addedNode = this._innerList.AddLast(item);
            if (!inIndex)
            {
                this._index.Add(indexValue, addedNode);
            }
        }
    }

    /// <inheritdoc/>
    public T Pop()
    {
        if (this._innerList.First is { } first)
        {
            var value = first.Value;
            this._index.Remove(this._indexer.GetIndexValue(value)); // first value is probably always in the index...
            this._innerList.RemoveFirst();
            return value;
        }

        throw new InvalidOperationException("List is empty");
    }

    /// <inheritdoc/>
    public T Peek()
    {
        if (this._innerList.First is { } first)
        {
            return first.Value;
        }

        throw new InvalidOperationException("List is empty");
    }

    /// <inheritdoc/>
    public void Clear()
    {
        this._innerList.Clear();
        this._index.Clear();
    }
}