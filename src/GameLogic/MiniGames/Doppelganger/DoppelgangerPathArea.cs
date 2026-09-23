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
/// The areas of a path overlap. As in the original server, the lower bounds are exclusive
/// and the upper bounds are inclusive.
/// </remarks>
/// <param name="X1">The lower x coordinate (exclusive).</param>
/// <param name="Y1">The lower y coordinate (exclusive).</param>
/// <param name="X2">The upper x coordinate (inclusive).</param>
/// <param name="Y2">The upper y coordinate (inclusive).</param>
public record DoppelgangerPathArea(byte X1, byte Y1, byte X2, byte Y2)
{
    /// <summary>
    /// Gets the center of the area, which is the target of the monsters walking to this area.
    /// </summary>
    public Point Center => new((byte)(this.X1 + ((this.X2 - this.X1) / 2)), (byte)(this.Y1 + ((this.Y2 - this.Y1) / 2)));

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
}
