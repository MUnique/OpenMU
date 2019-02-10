// <copyright file="IndexedLinkedList{T}.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.Pathfinding.Interfaces
{
    using System.Collections.Generic;

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
        private readonly LinkedList<T> innerList;
        private readonly IComparer<T> comparer;
        private readonly IIndexer<T> indexer;

        /// <summary>
        /// The index helps to find a better starting point to insert new nodes.
        /// </summary>
        private readonly IDictionary<int, LinkedListNode<T>> index;

        /// <summary>
        /// Initializes a new instance of the <see cref="IndexedLinkedList{T}"/> class.
        /// </summary>
        /// <param name="comparer">A comparer for the type which should be conained in the heap. It is used to determine the correct position in the heap.</param>
        /// <param name="indexer">The indexer, which returns an index value which is used as key.</param>
        public IndexedLinkedList(IComparer<T> comparer, IIndexer<T> indexer)
        {
            this.innerList = new LinkedList<T>();
            this.comparer = comparer;
            this.indexer = indexer;
            this.index = new Dictionary<int, LinkedListNode<T>>();
        }

        /// <inheritdoc/>
        public int Count => this.innerList.Count;

        /// <inheritdoc/>
        public void Push(T item)
        {
            int indexValue = this.indexer.GetIndexValue(item);
            if (this.innerList.First == null)
            {
                var addedNode = this.innerList.AddFirst(item);
                this.index.Add(indexValue, addedNode);
                return;
            }

            var inIndex = true;
            if (!this.index.TryGetValue(indexValue, out LinkedListNode<T> node))
            {
                node = this.innerList.First;
                inIndex = false;
            }

            while (node != null && this.comparer.Compare(item, node.Value) > 0)
            {
                node = node.Next;
            }

            if (node != null)
            {
                if (inIndex)
                {
                    this.innerList.AddAfter(node, item);
                }
                else
                {
                    var addedNode = this.innerList.AddAfter(node, item);
                    this.index.Add(indexValue, addedNode);
                }
            }
            else
            {
                var addedNode = this.innerList.AddLast(item);
                if (!inIndex)
                {
                    this.index.Add(indexValue, addedNode);
                }
            }
        }

        /// <inheritdoc/>
        public T Pop()
        {
            var value = this.innerList.First.Value;
            this.index.Remove(this.indexer.GetIndexValue(value)); // first value is probably always in the index...
            this.innerList.RemoveFirst();
            return value;
        }

        /// <inheritdoc/>
        public T Peek()
        {
            return this.innerList.First.Value;
        }

        /// <inheritdoc/>
        public void Clear()
        {
            this.innerList.Clear();
            this.index.Clear();
        }
    }
}
