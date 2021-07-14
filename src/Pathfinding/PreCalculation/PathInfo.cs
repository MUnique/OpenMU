// <copyright file="PathInfo.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.Pathfinding.PreCalculation
{
    using System;
    using System.Runtime.InteropServices;

    /// <summary>
    /// Information about which is the <see cref="NextStep"/> to reach the <see cref="PointCombination.End"/> from the <see cref="PointCombination.Start"/>.
    /// </summary>
    [StructLayout(LayoutKind.Sequential, Pack = 1, Size = 6)]
    public struct PathInfo : IEquatable<PathInfo>
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="PathInfo"/> struct.
        /// </summary>
        /// <param name="combination">The combination.</param>
        /// <param name="nextStep">The next step.</param>
        public PathInfo(PointCombination combination, Point nextStep)
        {
            this.Combination = combination;
            this.NextStep = nextStep;
        }

        /// <summary>
        /// Gets the start/end point combination which acts like a key for the next step.
        /// </summary>
        public PointCombination Combination { get; }

        /// <summary>
        /// Gets the next step to get one step closer to the <see cref="PointCombination.End"/>.
        /// </summary>
        public Point NextStep { get; }

        /// <inheritdoc />
        public override bool Equals(object? obj)
        {
            if (obj is PathInfo other)
            {
                return this.Equals(other);
            }

            return base.Equals(obj);
        }

        /// <inheritdoc/>
        public bool Equals(PathInfo other)
        {
            return this.Combination.Equals(other.Combination) && this.NextStep == other.NextStep;
        }

        /// <inheritdoc />
        public override int GetHashCode()
        {
            return HashCode.Combine(this.Combination, this.NextStep);
        }
    }
}
