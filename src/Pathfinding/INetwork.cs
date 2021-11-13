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
    /// <returns>The node at the specified position.</returns>
    Node GetNodeAt(Point position);

    /// <summary>
    /// Gets the nodes which can be reached by the specified node.
    /// </summary>
    /// <param name="node">The node.</param>
    /// <returns>The nodes which can be reached by the specified node.</returns>
    IEnumerable<Node> GetPossibleNextNodes(Node node);

    /// <summary>
    /// Resets the status of all nodes of the network.
    /// Needed to be called before any new path is being searched.
    /// </summary>
    void ResetStatus();
}