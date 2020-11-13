// <copyright file="GridNetwork.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.Pathfinding
{
    using System;
    using System.Collections.Generic;
    using System.Linq;

    /// <summary>
    /// Network which is built of a two-dimensional grid of nodes where
    /// each coordinate has a fixed cost to reach it from any direction.
    /// </summary>
    /// <seealso cref="MUnique.OpenMU.Pathfinding.INetwork" />
    public class GridNetwork : INetwork
    {
        /// <summary>
        /// The grid node value of an unreachable grid coordinate.
        /// </summary>
        private const byte UnreachableGridNodeValue = 0;

        private static readonly sbyte[,] DirectionOffsets =
        {
            { 0, -1 },
            { 1, 0 },
            { 0, 1 },
            { -1, 0 },
            { 1, -1 },
            { 1, 1 },
            { -1, 1 },
            { -1, -1 },
        };

        /// <summary>
        /// The two-dimensional grid.
        /// For each coordinate it contains the cost of traveling to it from a neighbor coordinate.
        /// The value of 0 means, that the coordinate is unreachable.
        /// </summary>
        private readonly byte[,] grid;
        private readonly ushort gridWidth;
        private readonly ushort gridHeight;
        private readonly int heightLog2;
        private readonly Node[] calculationGrid;
        private readonly int numberOfDirections;

        /// <summary>
        /// Initializes a new instance of the <see cref="GridNetwork"/> class.
        /// </summary>
        /// <param name="grid">
        ///     The two-dimensional grid.
        ///     For each coordinate it contains the cost of traveling to it from a neighbor coordinate.
        ///     The value of 0 means, that the coordinate is unreachable, <see cref="UnreachableGridNodeValue"/>.
        /// </param>
        /// <param name="allowDiagonals">if set to <c>true</c> [allow diagonals].</param>
        public GridNetwork(byte[,] grid, bool allowDiagonals)
        {
            this.grid = grid;
            this.gridWidth = (ushort)(grid.GetUpperBound(0) + 1);
            this.gridHeight = (ushort)(grid.GetUpperBound(1) + 1);
            this.calculationGrid = new Node[this.gridWidth * this.gridHeight];
            this.heightLog2 = (int)Math.Log(this.gridHeight, 2);
            this.numberOfDirections = allowDiagonals ? 8 : 4;
        }

        /// <inheritdoc/>
        public Node GetNodeAt(Point position)
        {
            var nodeIndex = this.GetIndexOfPoint(position);
            var node = this.calculationGrid[nodeIndex];
            if (node is null)
            {
                node = new Node { Position = position };
                this.calculationGrid[nodeIndex] = node;
            }

            return node;
        }

        /// <inheritdoc/>
        /// <remarks>
        /// Not sure, if the implementation should really filter out nodes based on their status and cost.
        /// </remarks>
        public IEnumerable<Node> GetPossibleNextNodes(Node node)
        {
            // ReSharper disable once TooWideLocalVariableScope performance improvement
            byte newX;

            // ReSharper disable once TooWideLocalVariableScope performance improvement
            byte newY;

            for (int i = 0; i < this.numberOfDirections; i++)
            {
                newX = (byte)(node.X + DirectionOffsets[i, 0]);
                newY = (byte)(node.Y + DirectionOffsets[i, 1]);

                if (newX >= this.gridWidth || newY >= this.gridHeight || this.grid[newX, newY] == UnreachableGridNodeValue)
                {
                    continue;
                }

                var newPoint = new Point(newX, newY);
                var newNode = this.GetNodeAt(newPoint);
                if (newNode.Status == NodeStatus.Closed)
                {
                    continue;
                }

                var newG = this.CalculateCost(node, newNode);

                if (newNode.Status == NodeStatus.Open && newNode.CostUntilNow <= newG)
                {
                    // The current node has less cost than the previous? then skip this node
                    continue;
                }

                newNode.CostUntilNow = newG;

                yield return newNode;
            }
        }

        /// <inheritdoc/>
        public void ResetStatus()
        {
            foreach (var node in this.calculationGrid.Where(n => n != null))
            {
                node.Status = NodeStatus.Undefined;
            }
        }

        private int GetIndexOfPoint(Point position)
        {
            return (position.Y << this.heightLog2) + position.X;
        }

        private int CalculateCost(Node fromNode, Node toNode)
        {
            return fromNode.CostUntilNow + this.grid[toNode.X, toNode.Y];
        }
    }
}
