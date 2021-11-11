// <copyright file="MaximumDistanceOfXorYHeuristic.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.Pathfinding;

/// <summary>
/// A heuristic which takes the maximum distance value between the x-axis or the y-axis.
/// </summary>
internal class MaximumDistanceOfXorYHeuristic : IHeuristic
{
    /// <inheritdoc/>
    public int HeuristicEstimateMultiplier { get; set; }

    /// <inheritdoc/>
    public int CalculateHeuristicDistance(Point location, Point target)
    {
        return this.HeuristicEstimateMultiplier * Math.Max(Math.Abs(location.X - target.X), Math.Abs(location.Y - target.Y));
    }
}