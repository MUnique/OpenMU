// <copyright file="PathResultNode.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.Pathfinding
{
    using System;

    /// <summary>
    /// A path finder node.
    /// </summary>
    public readonly struct PathResultNode : IEquatable<PathResultNode>
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

        /// <inheritdoc />
        public override bool Equals(object? obj)
        {
            if (obj is PathResultNode other)
            {
                return this.Equals(other);
            }

            return base.Equals(obj);
        }

        /// <inheritdoc/>
        public bool Equals(PathResultNode other)
        {
            return this.ThisPoint == other.ThisPoint && this.PreviousPoint == other.PreviousPoint;
        }

        /// <inheritdoc />
        public override int GetHashCode()
        {
            return HashCode.Combine(this.PreviousPoint, this.ThisPoint);
        }
    }
}
