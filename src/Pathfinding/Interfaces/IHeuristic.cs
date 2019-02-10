// <copyright file="IHeuristic.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.Pathfinding.Interfaces
{
    /// <summary>
    /// Describes a heuristic which detemines a heuristic distance from the current location to the target location.
    /// As better the heuristic as faster an optimal path can be found.
    /// </summary>
    public interface IHeuristic
    {
        /// <summary>
        /// Gets or sets the heuristic estimate multiply value.
        /// </summary>
        int HeuristicEstimateMultiplier { get; set; }

        /// <summary>
        /// Calculates the heuristic distance from the location to the target.
        /// </summary>
        /// <param name="location">The location position.</param>
        /// <param name="target">The target position.</param>
        /// <returns>The heuristic distance from the location to the target.</returns>
        int CalculateHeuristicDistance(Point location, Point target);
    }
}
