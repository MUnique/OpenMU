// <copyright file="BinaryMinHeapTest.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.Pathfinding.Tests
{
    using System;
    using System.Diagnostics;
    using System.Linq;
    using MUnique.OpenMU.Pathfinding;
    using NUnit.Framework;

    /// <summary>
    /// Tests the <see cref="BinaryMinHeap{T}"/>.
    /// </summary>
    [TestFixture]
    internal class BinaryMinHeapTest
    {
        /// <summary>
        /// Tests if the heap pops the node with the lowest cost.
        /// </summary>
        [Test]
        public void MinHeapWithNodes()
        {
            var heap = new BinaryMinHeap<Node>(new NodeComparer());
            foreach (var number in Enumerable.Range(1, 10).Reverse())
            {
                heap.Push(new Node { PredictedTotalCost = number });
            }

            var minimumNumber = heap.Pop().PredictedTotalCost;
            Assert.That(minimumNumber, Is.EqualTo(1));
        }

        /// <summary>
        /// Tests if the heap pops the lowest value.
        /// </summary>
        [Test]
        public void MinHeapWithNumbers()
        {
            var heap = new BinaryMinHeap<int>();
            foreach (var number in Enumerable.Range(1, 10).Reverse())
            {
                heap.Push(number);
            }

            var minimumNumber = heap.Pop();
            Assert.That(minimumNumber, Is.EqualTo(1));
        }

        /// <summary>
        /// Compares the performance between <see cref="BinaryMinHeap{T}"/> and <see cref="IndexedLinkedList{T}"/>.
        /// </summary>
        [Test]
        public void PerformanceComparison()
        {
            var simpleHeap = new BinaryMinHeap<Node>(new NodeComparer());
            var indexedHeap = new IndexedLinkedList<Node>(new NodeComparer(), new NodeIndexer());
            var count = 1000;
            this.PushNodes(simpleHeap, count);
            this.PushNodes(indexedHeap, count);
            var pushSimple = this.PushNodes(simpleHeap, count);
            var pushIndexed = this.PushNodes(indexedHeap, count);
            this.PopNodes(simpleHeap, count);
            this.PopNodes(indexedHeap, count);
            var popSimple = this.PopNodes(simpleHeap, count);
            var popIndexed = this.PopNodes(indexedHeap, count);

            Assert.Pass($"(PUSH) BinaryMinHeap: {pushSimple.Elapsed}, IndexedLinkedList: {pushIndexed.Elapsed}\r\n(POP) BinaryMinHeap: {popSimple.Elapsed}, IndexedLinkedList: {popIndexed.Elapsed}");
        }

        private Stopwatch PopNodes(IPriorityQueue<Node> queue, int count)
        {
            var stopwatch = new Stopwatch();
            stopwatch.Start();
            for (int i = 0; i < count; i++)
            {
                queue.Pop();
            }

            stopwatch.Stop();
            return stopwatch;
        }

        private Stopwatch PushNodes(IPriorityQueue<Node> queue, int count)
        {
            var stopwatch = new Stopwatch();
            stopwatch.Start();
            var randomizer = new Random(1);
            foreach (var unused in Enumerable.Range(1, count))
            {
                var node = new Node { PredictedTotalCost = randomizer.Next(1, count) };
                queue.Push(node);
            }

            stopwatch.Stop();
            return stopwatch;
        }
    }
}
