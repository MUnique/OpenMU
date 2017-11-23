// <copyright file="PathInfo.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.Pathfinding.PreCalculation
{
    using System.Runtime.InteropServices;

    /// <summary>
    /// Information about which is the <see cref="NextStep"/> to reach the <see cref="PointCombination.End"/> from the <see cref="PointCombination.Start"/>.
    /// </summary>
    [StructLayout(LayoutKind.Sequential, Pack = 1, Size = 6)]
    public struct PathInfo
    {
        /// <summary>
        /// The start/end point combination which acts like a key for the next step.
        /// </summary>
        public PointCombination Combination;

        /// <summary>
        /// The next step to get one step closer to the <see cref="PointCombination.End"/>.
        /// </summary>
        public Point NextStep;

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
    }
}
