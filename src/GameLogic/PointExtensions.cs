// <copyright file="PointExtensions.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.GameLogic;

using MUnique.OpenMU.Pathfinding;

/// <summary>
/// Extensions for <see cref="Point"/>.
/// </summary>
public static class PointExtensions
{
    /// <summary>
    /// Determines whether if the point is within bounds of the specified rectangle.
    /// The border points of the rectangle are inclusive.
    /// </summary>
    /// <param name="point">The point.</param>
    /// <param name="rectangle">The rectangle.</param>
    /// <returns><c>true</c> if the point is within bounds of the specified rectangle; otherwise, <c>false</c>.</returns>
    public static bool IsWithinBoundsOf(this Point point, Rectangle rectangle)
    {
        return point.X >= rectangle.X1
               && point.X <= rectangle.X2
               && point.Y >= rectangle.Y1
               && point.Y <= rectangle.Y2;
    }
}