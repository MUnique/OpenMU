// <copyright file="Point.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.Pathfinding
{
    using System;
    using System.Runtime.InteropServices;

    /// <summary>
    /// Defines a coordinate on a map.
    /// </summary>
    [StructLayout(LayoutKind.Sequential, Pack = 1, Size = 2)]
    public struct Point : IEquatable<Point>
    {
        private readonly byte x;
        private readonly byte y;

        /// <summary>
        /// Initializes a new instance of the <see cref="Point"/> struct.
        /// </summary>
        /// <param name="x">The x coordinate.</param>
        /// <param name="y">The y coordinate.</param>
        public Point(byte x, byte y)
        {
            this.x = x;
            this.y = y;
        }

        /// <summary>
        /// Gets the x coordinate.
        /// </summary>
        public byte X => this.x;

        /// <summary>
        /// Gets the y coordinate.
        /// </summary>
        public byte Y => this.y;

        /// <summary>
        /// Implements the operator ==.
        /// </summary>
        /// <param name="point1">The point1.</param>
        /// <param name="point2">The point2.</param>
        /// <returns>
        /// The result of the operator.
        /// </returns>
        public static bool operator ==(Point point1, Point point2)
        {
            return point1.Equals(point2);
        }

        /// <summary>
        /// Implements the operator !=.
        /// </summary>
        /// <param name="point1">The point1.</param>
        /// <param name="point2">The point2.</param>
        /// <returns>
        /// The result of the operator.
        /// </returns>
        public static bool operator !=(Point point1, Point point2)
        {
            return !point1.Equals(point2);
        }

        /// <summary>
        /// Gets the euclidean distance between this point and another point.
        /// </summary>
        /// <param name="otherPoint">The other point.</param>
        /// <returns>The distance between this point and another point.</returns>
        public double EuclideanDistanceTo(Point otherPoint)
        {
            return Math.Sqrt(Math.Pow(Math.Abs(this.X - otherPoint.X), 2) + Math.Pow(Math.Abs(this.Y - otherPoint.Y), 2));
        }

        /// <inheritdoc />
        public bool Equals(Point otherPoint)
        {
            return otherPoint.X == this.X && otherPoint.Y == this.Y;
        }

        /// <inheritdoc/>
        public override bool Equals(object obj)
        {
            if (!(obj is Point))
            {
                return false;
            }

            return this.Equals((Point)obj);
        }

        /// <inheritdoc/>
        public override int GetHashCode()
        {
            return (this.x * 0x100) + this.y;
        }

        /// <inheritdoc/>
        public override string ToString()
        {
            return $"{this.x}, {this.y}";
        }
    }
}
