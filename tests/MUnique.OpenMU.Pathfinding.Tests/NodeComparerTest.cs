// <copyright file="NodeComparerTest.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.Pathfinding.Tests
{
    using MUnique.OpenMU.Pathfinding;
    using NUnit.Framework;

    /// <summary>
    /// Tests the <see cref="NodeComparer"/>.
    /// </summary>
    [TestFixture]
    internal class NodeComparerTest
    {
        /// <summary>
        /// Tests if <see cref="NodeComparer.Compare(Node, Node)"/> does return the
        /// correct value when comparing two different nodes with different costs.
        /// </summary>
        [Test]
        public void Compare()
        {
            var node1 = new Node { PredictedTotalCost = 1 };
            var node2 = new Node { PredictedTotalCost = 2 };
            var comparer = new NodeComparer();
            Assert.That(comparer.Compare(node1, node2), Is.Negative);
        }
    }
}