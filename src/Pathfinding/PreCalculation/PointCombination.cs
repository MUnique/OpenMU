// <copyright file="PointCombination.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.Pathfinding.PreCalculation
{
    using System;
    using System.Runtime.InteropServices;

    /// <summary>
    /// A combination of the start and end point, which acts like a key for the next step to reach the end point.
    /// </summary>
    [StructLayout(LayoutKind.Sequential, Pack = 1, Size = 4)]
    public struct PointCombination : IEquatable<PointCombination>
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="PointCombination"/> struct.
        /// </summary>
        /// <param name="start">The start point.</param>
        /// <param name="end">The end point.</param>
        public PointCombination(Point start, Point end)
        {
            this.Start = start;
            this.End = end;
        }

        /// <summary>
        /// Gets the start point.
        /// </summary>
        public Point Start { get; }

        /// <summary>
        /// Gets the end point.
        /// </summary>
        public Point End { get; }

        /// <inheritdoc />
        public override bool Equals(object? obj)
        {
            if (obj is PointCombination other)
            {
                return this.Equals(other);
            }

            return base.Equals(obj);
        }

        /// <inheritdoc/>
        public bool Equals(PointCombination other)
        {
            return other.Start == this.Start && other.End == this.End;
        }

        /// <inheritdoc />
        public override int GetHashCode()
        {
            return HashCode.Combine(this.Start, this.End);
        }
    }
}
