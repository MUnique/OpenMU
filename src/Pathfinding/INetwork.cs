// <copyright file="INetwork.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.Pathfinding;

/// <summary>
/// Interface of a network which can be used by the <see cref="PathFinder"/>.
/// </summary>
public interface INetwork
{
    /// <summary>
    /// Gets the node at the specified position.
    /// </summary>
    /// <param name="position">The position.</param>
    /// <returns>The node at the specified position, or null if there is no node at this position.</returns>
    Node? GetNodeAt(Point position);

    /// <summary>
    /// Gets the nodes which can be reached by the specified node.
    /// </summary>
    /// <param name="node">The node.</param>
    /// <returns>The nodes which can be reached by the specified node.</returns>
    IEnumerable<Node> GetPossibleNextNodes(Node node);

    /// <summary>
    /// Prepares the network for the next path finding.
    /// Resets the status of all nodes of the network.
    /// Needed to be called before any new path is being searched.
    /// </summary>
    /// <param name="start">The start point.</param>
    /// <param name="end">The end point.</param>
    /// <param name="grid">
    /// The two-dimensional grid.
    /// For each coordinate it contains the cost of traveling to it from a neighbor coordinate.
    /// The value of 0 means, that the coordinate is unreachable, <see cref="BaseGridNetwork.UnreachableGridNodeValue" />.
    /// If the highest bit of a value is set, it means it's a coordinate of a safezone.
    /// </param>
    /// <param name="includeSafezone">If set to <c>true</c>, safezone nodes should be included in the search.</param>
    /// <returns>
    /// If the preparations were successful and the pathfinding can proceed.
    /// </returns>
    bool Prepare(Point start, Point end, byte[,] grid, bool includeSafezone);
}