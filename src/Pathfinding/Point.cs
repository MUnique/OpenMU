// <copyright file="Point.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.Pathfinding;

using System.Runtime.InteropServices;

/// <summary>
/// Defines a coordinate on a map.
/// </summary>
/// <param name="X">The x coordinate.</param>
/// <param name="Y">The y coordinate.</param>
[StructLayout(LayoutKind.Sequential, Pack = 1, Size = 4)]
public record struct Point(ushort X, ushort Y)
{
    /// <summary>
    /// Implements the Addition operator between two points.
    /// </summary>
    /// <param name="a">The first point.</param>
    /// <param name="b">The second point.</param>
    /// <returns>
    /// The result of the operator.
    /// </returns>
    public static Point operator +(Point a, Point b) => new((ushort)(a.X + b.X), (ushort)(a.Y + b.Y));

    /// <summary>
    /// Implements the Subtraction operator between two points.
    /// </summary>
    /// <param name="a">The first point.</param>
    /// <param name="b">The second point.</param>
    /// <returns>
    /// The result of the operator.
    /// </returns>
    public static Point operator -(Point a, Point b) => new((ushort)(a.X - b.X), (ushort)(a.Y - b.Y));

    /// <summary>
    /// Implements the Subtraction operator between a point and an integer.
    /// </summary>
    /// <param name="a">The first point.</param>
    /// <param name="d">The divisor.</param>
    /// <returns>
    /// The result of the operator.
    /// </returns>
    public static Point operator /(Point a, int d) => new((ushort)(a.X / d), (ushort)(a.Y / d));

    /// <summary>
    /// Gets the euclidean distance between this point and another point.
    /// </summary>
    /// <param name="otherPoint">The other point.</param>
    /// <returns>The distance between this point and another point.</returns>
    public double EuclideanDistanceTo(Point otherPoint)
    {
        var dx = (int)this.X - otherPoint.X;
        var dy = (int)this.Y - otherPoint.Y;
        return Math.Sqrt((dx * dx) + (dy * dy));
    }

    /// <inheritdoc/>
    public override string ToString()
    {
        return $"{this.X}, {this.Y}";
    }
}
