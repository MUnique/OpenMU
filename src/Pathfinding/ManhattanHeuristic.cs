// <copyright file="ManhattanHeuristic.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.Pathfinding
{
    using System;
    using Interfaces;

    /// <summary>
    /// The manhattan (because of how the buildings are placed) heuristic.
    /// </summary>
    internal class ManhattanHeuristic : IHeuristic
    {
        /// <inheritdoc/>
        public int HeuristicEstimateMultiplier { get; set; }

        /// <inheritdoc/>
        public int CalculateHeuristicDistance(Point location, Point target)
        {
            return this.HeuristicEstimateMultiplier * (Math.Abs(location.X - target.X) + Math.Abs(location.Y - target.Y));
        }
    }
}
