// <copyright file="GridNetwork.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.Pathfinding;

/// <summary>
/// Network which is built of a two-dimensional grid of nodes where
/// each coordinate has a fixed cost to reach it from any direction.
/// </summary>
/// <seealso cref="MUnique.OpenMU.Pathfinding.INetwork" />
public class GridNetwork : INetwork
{
    /// <summary>
    /// The grid node value of an unreachable grid coordinate.
    /// </summary>
    private const byte UnreachableGridNodeValue = 0;

    private static readonly sbyte[,] DirectionOffsets =
    {
        { 0, -1 },
        { 1, 0 },
        { 0, 1 },
        { -1, 0 },
        { 1, -1 },
        { 1, 1 },
        { -1, 1 },
        { -1, -1 },
    };

    /// <summary>
    /// The two-dimensional grid.
    /// For each coordinate it contains the cost of traveling to it from a neighbor coordinate.
    /// The value of 0 means, that the coordinate is unreachable.
    /// </summary>
    private readonly byte[,] _grid;
    private readonly ushort _gridWidth;
    private readonly ushort _gridHeight;
    private readonly int _heightLog2;
    private readonly Node[] _calculationGrid;
    private readonly int _numberOfDirections;

    /// <summary>
    /// Initializes a new instance of the <see cref="GridNetwork"/> class.
    /// </summary>
    /// <param name="grid">
    ///     The two-dimensional grid.
    ///     For each coordinate it contains the cost of traveling to it from a neighbor coordinate.
    ///     The value of 0 means, that the coordinate is unreachable, <see cref="UnreachableGridNodeValue"/>.
    /// </param>
    /// <param name="allowDiagonals">if set to <c>true</c> [allow diagonals].</param>
    public GridNetwork(byte[,] grid, bool allowDiagonals)
    {
        this._grid = grid;
        this._gridWidth = (ushort)(grid.GetUpperBound(0) + 1);
        this._gridHeight = (ushort)(grid.GetUpperBound(1) + 1);
        this._calculationGrid = new Node[this._gridWidth * this._gridHeight];
        this._heightLog2 = (int)Math.Log(this._gridHeight, 2);
        this._numberOfDirections = allowDiagonals ? 8 : 4;
    }

    /// <inheritdoc/>
    public Node GetNodeAt(Point position)
    {
        var nodeIndex = this.GetIndexOfPoint(position);
        var node = this._calculationGrid[nodeIndex];
        if (node is null)
        {
            node = new Node { Position = position };
            this._calculationGrid[nodeIndex] = node;
        }

        return node;
    }

    /// <inheritdoc/>
    /// <remarks>
    /// Not sure, if the implementation should really filter out nodes based on their status and cost.
    /// </remarks>
    public IEnumerable<Node> GetPossibleNextNodes(Node node)
    {
        // ReSharper disable once TooWideLocalVariableScope performance improvement
        byte newX;

        // ReSharper disable once TooWideLocalVariableScope performance improvement
        byte newY;

        for (int i = 0; i < this._numberOfDirections; i++)
        {
            newX = (byte)(node.X + DirectionOffsets[i, 0]);
            newY = (byte)(node.Y + DirectionOffsets[i, 1]);

            if (newX >= this._gridWidth || newY >= this._gridHeight || this._grid[newX, newY] == UnreachableGridNodeValue)
            {
                continue;
            }

            var newPoint = new Point(newX, newY);
            var newNode = this.GetNodeAt(newPoint);
            if (newNode.Status == NodeStatus.Closed)
            {
                continue;
            }

            var newG = this.CalculateCost(node, newNode);

            if (newNode.Status == NodeStatus.Open && newNode.CostUntilNow <= newG)
            {
                // The current node has less cost than the previous? then skip this node
                continue;
            }

            newNode.CostUntilNow = newG;

            yield return newNode;
        }
    }

    /// <inheritdoc/>
    public void ResetStatus()
    {
        foreach (var node in this._calculationGrid.Where(n => n != null))
        {
            node.Status = NodeStatus.Undefined;
        }
    }

    private int GetIndexOfPoint(Point position)
    {
        return (position.Y << this._heightLog2) + position.X;
    }

    private int CalculateCost(Node fromNode, Node toNode)
    {
        return fromNode.CostUntilNow + this._grid[toNode.X, toNode.Y];
    }
}