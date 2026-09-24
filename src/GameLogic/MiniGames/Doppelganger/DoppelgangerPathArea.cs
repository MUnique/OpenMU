// <copyright file="DoppelgangerPathArea.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.GameLogic.MiniGames.Doppelganger;

using MUnique.OpenMU.Pathfinding;

/// <summary>
/// A rectangular area of the path, which the monsters of the doppelganger event walk
/// along towards the magic circle.
/// </summary>
/// <remarks>
/// The areas of a path overlap. The lower bounds are exclusive and the upper bounds are inclusive.
/// </remarks>
public class DoppelgangerPathArea
{
    /// <summary>
    /// Initializes a new instance of the <see cref="DoppelgangerPathArea"/> class.
    /// </summary>
    public DoppelgangerPathArea()
    {
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="DoppelgangerPathArea"/> class.
    /// </summary>
    /// <param name="x1">The lower x coordinate (exclusive).</param>
    /// <param name="y1">The lower y coordinate (exclusive).</param>
    /// <param name="x2">The upper x coordinate (inclusive).</param>
    /// <param name="y2">The upper y coordinate (inclusive).</param>
    public DoppelgangerPathArea(byte x1, byte y1, byte x2, byte y2)
    {
        this.X1 = x1;
        this.Y1 = y1;
        this.X2 = x2;
        this.Y2 = y2;
    }

    /// <summary>
    /// Gets or sets the lower x coordinate (exclusive).
    /// </summary>
    public byte X1 { get; set; }

    /// <summary>
    /// Gets or sets the lower y coordinate (exclusive).
    /// </summary>
    public byte Y1 { get; set; }

    /// <summary>
    /// Gets or sets the upper x coordinate (inclusive).
    /// </summary>
    public byte X2 { get; set; }

    /// <summary>
    /// Gets or sets the upper y coordinate (inclusive).
    /// </summary>
    public byte Y2 { get; set; }

    /// <summary>
    /// Determines whether the area contains the specified point.
    /// </summary>
    /// <param name="point">The point.</param>
    /// <returns><c>true</c>, if the area contains the point; otherwise, <c>false</c>.</returns>
    public bool Contains(Point point)
    {
        return point.X > this.X1 && point.X <= this.X2
            && point.Y > this.Y1 && point.Y <= this.Y2;
    }

    /// <inheritdoc />
    public override string ToString() => $"({this.X1}, {this.Y1}) - ({this.X2}, {this.Y2})";
}
