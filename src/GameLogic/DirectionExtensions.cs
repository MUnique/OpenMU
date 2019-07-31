// <copyright file="DirectionExtensions.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.GameLogic
{
    using System;
    using MUnique.OpenMU.DataModel.Configuration;
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
            switch (direction)
            {
                case Direction.Undefined:
                    return origin;
                case Direction.North:
                    return new Point((byte)(origin.X - 1), (byte)(origin.Y + 1));
                case Direction.South:
                    return new Point((byte)(origin.X + 1), (byte)(origin.Y - 1));
                case Direction.East:
                    return new Point((byte)(origin.X + 1), (byte)(origin.Y + 1));
                case Direction.West:
                    return new Point((byte)(origin.X - 1), (byte)(origin.Y - 1));
                case Direction.NorthEast:
                    return new Point(origin.X, (byte)(origin.Y + 1));
                case Direction.NorthWest:
                    return new Point((byte)(origin.X - 1), origin.Y);
                case Direction.SouthEast:
                    return new Point((byte)(origin.X + 1), origin.Y);
                case Direction.SouthWest:
                    return new Point(origin.X, (byte)(origin.Y - 1));
                default: throw new ArgumentException($"Direction value {direction} is not defined", nameof(direction));
            }
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

            if (to.X == from.X)
            {
                if (to.Y > from.Y)
                {
                    return Direction.NorthEast;
                }

                return Direction.SouthWest;
            }

            if (to.Y == from.Y)
            {
                if (to.X > from.X)
                {
                    return Direction.SouthEast;
                }

                return Direction.NorthWest;
            }

            if (to.X > from.X)
            {
                if (to.Y > from.Y)
                {
                    return Direction.East;
                }

                return Direction.North;
            }

            if (to.Y > from.Y)
            {
                return Direction.South;
            }

            return Direction.West;
        }
    }
}