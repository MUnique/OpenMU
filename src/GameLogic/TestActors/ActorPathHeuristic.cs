// <copyright file="ActorPathHeuristic.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.GameLogic.TestActors;

using MUnique.OpenMU.Pathfinding;

/// <summary>
/// The straight-line estimate which steers the actor's path finder towards its target.
/// </summary>
/// <remarks>
/// The path finder searches the whole map for an actor (see
/// <see cref="ScriptedIntelligence"/>), so it needs a heuristic to stay cheap - without one it
/// expands the map in all directions. The engine's own Euclidean heuristic is <c>internal</c>, so
/// this is the same two lines in our assembly.
/// </remarks>
public sealed class ActorPathHeuristic : IHeuristic
{
    /// <inheritdoc />
    public int HeuristicEstimateMultiplier { get; set; }

    /// <inheritdoc />
    public int CalculateHeuristicDistance(Point location, Point target)
        => (int)(this.HeuristicEstimateMultiplier * location.EuclideanDistanceTo(target));
}
