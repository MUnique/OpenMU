// <copyright file="Node.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.Pathfinding
{
    /// <summary>
    /// The status of a node.
    /// </summary>
    public enum NodeStatus : byte
    {
        /// <summary>
        /// The status is undefined.
        /// </summary>
        Undefined,

        /// <summary>
        /// The node is on the open list.
        /// </summary>
        Open,

        /// <summary>
        /// The node is on the closed list.
        /// </summary>
        Closed,
    }

    /// <summary>
    /// A node of the path network.
    /// </summary>
    public class Node
    {
        /// <summary>
        /// Gets or sets the predicted total cost (F) to reach the destination.
        /// </summary>
        /// <remarks>F = G + H.</remarks>
        public int PredictedTotalCost { get; set; }

        /// <summary>
        /// Gets or sets the cost which came up so far (G) to reach this node.
        /// </summary>
        /// <remarks>G.</remarks>
        public int CostUntilNow { get; set; }

        /// <summary>
        /// Gets or sets the position of this node.
        /// </summary>
        public Point Position { get; set; }

        /// <summary>
        /// Gets the x coordinate of this node.
        /// </summary>
        public byte X => this.Position.X;

        /// <summary>
        /// Gets the y coordinate of this node.
        /// </summary>
        public byte Y => this.Position.Y;

        /// <summary>
        /// Gets or sets the previous node.
        /// </summary>
        public Node PreviousNode { get; set; }

        /// <summary>
        /// Gets or sets the status.
        /// </summary>
        public NodeStatus Status { get; set; }
    }
}
