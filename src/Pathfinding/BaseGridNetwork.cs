// <copyright file="BaseGridNetwork.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.Pathfinding;

/// <summary>
/// Base class for grid networks.
/// </summary>
public abstract class BaseGridNetwork : INetwork
{
    /// <summary>
    /// The grid node value of an unreachable grid coordinate.
    /// </summary>
    private const byte UnreachableGridNodeValue = 0;

    /// <summary>
    /// The bit flag which marks a safezone node.
    /// </summary>
    private const byte SafezoneBitFlag = 0b1000_0000;

    /// <summary>
    /// The bit mask for the cost of a node.
    /// </summary>
    private const byte CostBitMask = 0b0111_1111;

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

    private readonly int _numberOfDirections;
    private ushort _gridWidth;
    private ushort _gridHeight;

    /// <summary>
    /// The two-dimensional grid.
    /// For each coordinate it contains the cost of traveling to it from a neighbor coordinate.
    /// The value of 0 means, that the coordinate is unreachable.
    /// </summary>
    private byte[,]? _grid;

    /// <summary>
    /// A flag, if safezone nodes should be included in the network.
    /// </summary>
    private bool _includeSafezone;

    /// <summary>
    /// Initializes a new instance of the <see cref="BaseGridNetwork"/> class.
    /// </summary>
    /// <param name="allowDiagonals">If set to <c>true</c>, diagonal traveling is allowed.</param>
    protected BaseGridNetwork(bool allowDiagonals)
    {
        this._numberOfDirections = allowDiagonals ? 8 : 4;
    }

    /// <inheritdoc/>
    public virtual bool Prepare(Point start, Point end, byte[,] grid, bool includeSafezone)
    {
        this._grid = grid;
        this._gridWidth = (ushort)(grid.GetUpperBound(0) + 1);
        this._gridHeight = (ushort)(grid.GetUpperBound(1) + 1);
        this._includeSafezone = includeSafezone;
        return true;
    }

    /// <inheritdoc/>
    /// <remarks>
    /// Not sure, if the implementation should really filter out nodes based on their status and cost.
    /// </remarks>
    public IEnumerable<Node> GetPossibleNextNodes(Node node)
    {
        var grid = this._grid ?? throw new InvalidOperationException("Call Prepare before");

        // ReSharper disable once TooWideLocalVariableScope performance improvement
        byte newX;

        // ReSharper disable once TooWideLocalVariableScope performance improvement
        byte newY;

        for (int i = 0; i < this._numberOfDirections; i++)
        {
            newX = (byte)(node.X + DirectionOffsets[i, 0]);
            newY = (byte)(node.Y + DirectionOffsets[i, 1]);

            if (!this._includeSafezone && (grid[newX, newY] & SafezoneBitFlag) > 0)
            {
                continue;
            }

            var costToNode = grid[newX, newY] & CostBitMask;
            if (newX >= this._gridWidth || newY >= this._gridHeight || costToNode == UnreachableGridNodeValue)
            {
                continue;
            }

            var newPoint = new Point(newX, newY);
            var newNode = this.GetNodeAt(newPoint);
            if (newNode is null || newNode.Status == NodeStatus.Closed)
            {
                continue;
            }

            var newG = node.CostUntilNow + this._grid[newNode.X, newNode.Y];

            if (newNode.Status == NodeStatus.Open && newNode.CostUntilNow <= newG)
            {
                // The current node has less cost than the previous? then skip this node
                continue;
            }

            newNode.CostUntilNow = newG;

            yield return newNode;
        }
    }

    /// <inheritdoc />
    public abstract Node? GetNodeAt(Point position);
}