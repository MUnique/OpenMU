// <copyright file="PathResultNode.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.Pathfinding
{
    /// <summary>
    /// A path finder node.
    /// </summary>
    public struct PathResultNode
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="PathResultNode" /> struct.
        /// </summary>
        /// <param name="point">The point.</param>
        /// <param name="previousPoint">The previous point.</param>
        public PathResultNode(Point point, Point previousPoint)
        {
            this.ThisPoint = point;
            this.PreviousPoint = previousPoint;
        }

        /// <summary>
        /// Gets the previous point.
        /// </summary>
        public Point PreviousPoint { get; }

        /// <summary>
        /// Gets the x coordinate.
        /// </summary>
        public byte X => this.ThisPoint.X;

        /// <summary>
        /// Gets the y coordinate.
        /// </summary>
        public byte Y => this.ThisPoint.Y;

        private Point ThisPoint { get; }
    }
}
