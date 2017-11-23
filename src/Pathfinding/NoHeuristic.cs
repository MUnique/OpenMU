// <copyright file="NoHeuristic.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.Pathfinding
{
    /// <summary>
    /// A heuristic which does not calculate a heuristic distance.
    /// </summary>
    /// <remarks>Using this equals the Dijkstra algorithm.</remarks>
    internal class NoHeuristic : IHeuristic
    {
        /// <inheritdoc/>
        public int HeuristicEstimateMultiplier { get; set; }

        /// <inheritdoc/>
        public int CalculateHeuristicDistance(Point location, Point target)
        {
            return 0;
        }
    }
}
