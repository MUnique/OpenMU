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
[StructLayout(LayoutKind.Sequential, Pack = 1, Size = 2)]
public record struct Point(byte X, byte Y)
{
    /// <summary>
    /// Gets the euclidean distance between this point and another point.
    /// </summary>
    /// <param name="otherPoint">The other point.</param>
    /// <returns>The distance between this point and another point.</returns>
    public double EuclideanDistanceTo(Point otherPoint)
    {
        return Math.Sqrt(Math.Pow(Math.Abs(this.X - otherPoint.X), 2) + Math.Pow(Math.Abs(this.Y - otherPoint.Y), 2));
    }

    /// <inheritdoc/>
    public override string ToString()
    {
        return $"{this.X}, {this.Y}";
    }
}