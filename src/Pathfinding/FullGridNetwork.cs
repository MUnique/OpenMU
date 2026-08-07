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
    private Node[] _nodes = Array.Empty<Node>();

    private int _size;

    /// <summary>
    /// Initializes a new instance of the <see cref="FullGridNetwork"/> class.
    /// </summary>
    /// <param name="allowDiagonals">If set to <c>true</c>, diagonal traveling is allowed.</param>
    public FullGridNetwork(bool allowDiagonals)
        : base(allowDiagonals)
    {
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
        var size = grid.GetUpperBound(0) + 1;
        if (size != this._size)
        {
            this._size = size;
            this._nodes = new Node[size * size];
        }

        foreach (var node in this._nodes.Where(n => n != null))
        {
            node.Status = NodeStatus.Undefined;
        }

        return base.Prepare(start, end, grid, includeSafezone);
    }

    private int GetIndexOfPoint(Point position)
    {
        return position.Y * this._size + position.X;
    }
}