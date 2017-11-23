// <copyright file="EuclideanHeuristic.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.Pathfinding
{
    /// <summary>
    /// A heuristic which takes the maximum distance value between the x-axis or the y-axis.
    /// </summary>
    internal class EuclideanHeuristic : IHeuristic
    {
        /// <inheritdoc/>
        public int HeuristicEstimateMultiplier { get; set; }

        /// <inheritdoc/>
        public int CalculateHeuristicDistance(Point location, Point target)
        {
            var distance = location.EuclideanDistanceTo(target);
            return (int)(this.HeuristicEstimateMultiplier * distance);
        }
    }
}
