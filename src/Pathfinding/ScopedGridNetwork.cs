// <copyright file="ScopedGridNetwork.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.Pathfinding;

/// <summary>
/// Network which is built of a two-dimensional grid of nodes where
/// each coordinate has a fixed cost to reach it from any direction.
/// The network proves the nodes of a smaller scope of the grid.
/// </summary>
public sealed class ScopedGridNetwork : BaseGridNetwork
{
    private readonly Node?[] _gridNodes;
    private readonly byte _maximumSegmentSideLength;
    private readonly byte _minimumSegmentSideLength;
    private int _bitsPerCoordinate;
    private byte _actualSegmentSideLength;

    /// <summary>
    /// The offset of the <see cref="_gridNodes"/> in which the calculation takes place.
    /// </summary>
    private Point _segmentOffset;

    /// <summary>
    /// Initializes a new instance of the <see cref="ScopedGridNetwork" /> class.
    /// </summary>
    /// <param name="allowDiagonals">If set to <c>true</c>, diagonal traveling is allowed.</param>
    /// <param name="maximumSegmentSideLength">Maximum Length of the segment side. Should be a power of 2.</param>
    /// <param name="minimumSegmentSideLength">Minimum length of the segment side. Should be a power of 2.</param>
    public ScopedGridNetwork(bool allowDiagonals = true, byte maximumSegmentSideLength = 16, byte minimumSegmentSideLength = 8)
    : base(allowDiagonals)
    {
        this._gridNodes = new Node[maximumSegmentSideLength * maximumSegmentSideLength];
        this._maximumSegmentSideLength = maximumSegmentSideLength;
        this._minimumSegmentSideLength = minimumSegmentSideLength;
    }

    /// <inheritdoc/>
    public override Node? GetNodeAt(Point position)
    {
        var nodeIndex = this.GetIndexOfPoint(position.X, position.Y);
        if (nodeIndex < 0 || nodeIndex >= this._actualSegmentSideLength * this._actualSegmentSideLength)
        {
            return null;
        }

        var node = this._gridNodes[nodeIndex];
        if (node is null)
        {
            node = new Node { Position = position };
            this._gridNodes[nodeIndex] = node;
        }

        return node;
    }

    /// <inheritdoc/>
    public override bool Prepare(Point start, Point end, byte[,] grid, bool includeSafezone)
    {
        var diffX = Math.Abs(end.X - start.X);
        var diffY = Math.Abs(end.Y - start.Y);
        if (diffX > this._maximumSegmentSideLength || diffY > this._maximumSegmentSideLength)
        {
            return false;
        }

        this._actualSegmentSideLength = this._minimumSegmentSideLength;
        while ((diffX > this._actualSegmentSideLength || diffY > this._actualSegmentSideLength)
               && this._actualSegmentSideLength < this._maximumSegmentSideLength)
        {
            this._actualSegmentSideLength *= 2;
        }

        this._bitsPerCoordinate = (int)Math.Log(this._actualSegmentSideLength, 2);
        var avg = (start / 2) + (end / 2);

        var offsetX = GetOffset(avg.X, grid.GetUpperBound(0) + 1);
        var offsetY = GetOffset(avg.Y, grid.GetUpperBound(1) + 1);
        this._segmentOffset = new(offsetX, offsetY);

        var maxX = offsetX + this._actualSegmentSideLength;
        var maxY = offsetY + this._actualSegmentSideLength;

        for (byte x = offsetX; x < maxX; ++x)
        {
            for (byte y = offsetY; y < maxY; ++y)
            {
                var i = this.GetIndexOfPoint(x, y);
                var node = this._gridNodes[i];
                if (node is not null)
                {
                    node.Status = NodeStatus.Undefined;
                    node.Position = new(x, y);
                }
            }
        }

        return base.Prepare(start, end, grid, includeSafezone);

        byte GetOffset(byte avgValue, int gridSize)
        {
            var offset = (byte)Math.Max(avgValue - (this._actualSegmentSideLength / 2), 0);
            offset = (byte)Math.Min(offset, gridSize - this._actualSegmentSideLength);
            return offset;
        }
    }

    private int GetIndexOfPoint(int x, int y)
    {
        y -= this._segmentOffset.Y;
        x -= this._segmentOffset.X;

        if (x < 0 || y < 0)
        {
            return -1;
        }

        return (y << this._bitsPerCoordinate) + x;
    }
}