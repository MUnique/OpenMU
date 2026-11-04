// <copyright file="PathResultNode.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.Pathfinding;

/// <summary>
/// A path finder node.
/// </summary>
/// <param name="Point">The point.</param>
/// <param name="PreviousPoint">The previous point.</param>
public record struct PathResultNode(Point Point, Point PreviousPoint)
{
    /// <summary>
    /// Gets the x coordinate.
    /// </summary>
    public byte X => this.Point.X;

    /// <summary>
    /// Gets the y coordinate.
    /// </summary>
    public byte Y => this.Point.Y;
}