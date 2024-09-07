// <copyright file="FullGridNetwork.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.Pathfinding;

/// <summary>
/// Network which is built of a two-dimensional grid of nodes where
/// each coordinate has a fixed cost to reach it from any direction.
/// The network provides the nodes of the whole grid.
/// </summary>
public class FullGridNetwork : BaseGridNetwork
{
    private readonly Node[] _nodes;

    /// <summary>
    /// Initializes a new instance of the <see cref="FullGridNetwork"/> class.
    /// </summary>
    /// <param name="allowDiagonals">If set to <c>true</c>, diagonal traveling is allowed.</param>
    public FullGridNetwork(bool allowDiagonals)
        :base(allowDiagonals)
    {
        this._nodes = new Node[0x10000];
    }

    /// <inheritdoc/>
    public override Node GetNodeAt(Point position)
    {
        var nodeIndex = this.GetIndexOfPoint(position);
        var node = this._nodes[nodeIndex];
        if (node is null)
        {
            node = new Node { Position = position };
            this._nodes[nodeIndex] = node;
        }

        return node;
    }

    /// <inheritdoc/>
    public override bool Prepare(Point start, Point end, byte[,] grid, bool includeSafezone)
    {
        foreach (var node in this._nodes.Where(n => n != null))
        {
            node.Status = NodeStatus.Undefined;
        }

        return base.Prepare(start, end, grid, includeSafezone);
    }

    private int GetIndexOfPoint(Point position)
    {
        return (position.Y << 8) + position.X;
    }
}