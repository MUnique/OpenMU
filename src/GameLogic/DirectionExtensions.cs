// <copyright file="DirectionExtensions.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.GameLogic;

using MUnique.OpenMU.Pathfinding;

/// <summary>
/// Extension methods which have to do with handling <see cref="Direction"/>s.
/// </summary>
public static class DirectionExtensions
{
    /// <summary>
    /// Calculates the target point based on the origin and the specified direction.
    /// </summary>
    /// <param name="origin">From point.</param>
    /// <param name="direction">The direction.</param>
    /// <returns>The target point based on the origin and the specified direction.</returns>
    /// <exception cref="ArgumentException">direction.</exception>
    public static Point CalculateTargetPoint(this Point origin, Direction direction)
    {
        return direction switch
        {
            Direction.Undefined => origin,
            Direction.North => new Point((byte)(origin.X - 1), (byte)(origin.Y + 1)),
            Direction.South => new Point((byte)(origin.X + 1), (byte)(origin.Y - 1)),
            Direction.East => new Point((byte)(origin.X + 1), (byte)(origin.Y + 1)),
            Direction.West => new Point((byte)(origin.X - 1), (byte)(origin.Y - 1)),
            Direction.NorthEast => new Point(origin.X, (byte)(origin.Y + 1)),
            Direction.NorthWest => new Point((byte)(origin.X - 1), origin.Y),
            Direction.SouthEast => new Point((byte)(origin.X + 1), origin.Y),
            Direction.SouthWest => new Point(origin.X, (byte)(origin.Y - 1)),
            _ => throw new ArgumentException($"Direction value {direction} is not defined", nameof(direction))
        };
    }

    /// <summary>
    /// Gets the direction from one point to another.
    /// </summary>
    /// <param name="from">The origin point.</param>
    /// <param name="to">The target point.</param>
    /// <returns>The direction from the origin to the target.</returns>
    public static Direction GetDirectionTo(this Point from, Point to)
    {
        if (from == to)
        {
            return Direction.Undefined;
        }

        double angle = Math.Atan2(to.Y - from.Y, to.X - from.X);
        angle += Math.PI;
        angle /= Math.PI / 4;
        int halfQuarter = Convert.ToInt32(angle);
        halfQuarter %= 8;

        return halfQuarter switch
        {
            7 => Direction.North,
            0 => Direction.NorthWest,
            1 => Direction.West,
            2 => Direction.SouthWest,
            3 => Direction.South,
            4 => Direction.SouthEast,
            5 => Direction.East,
            6 => Direction.NorthEast,
            _ => Direction.Undefined
        };
    }
}