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
        /// <exception cref="ArgumentException">direction</exception>
        public static Point CalculateTargetPoint(this Point origin, Direction direction)
        {
            switch (direction)
            {
                case Direction.Undefined:
                    return origin;
                case Direction.South:
                    return new Point((byte)(origin.X + 1), origin.Y);
                case Direction.North:
                    return new Point((byte)(origin.X - 1), origin.Y);
                case Direction.East:
                    return new Point(origin.X, (byte)(origin.Y - 1));
                case Direction.West:
                    return new Point(origin.X, (byte)(origin.Y + 1));
                case Direction.SouthEast:
                    return new Point((byte)(origin.X + 1), (byte)(origin.Y - 1));
                case Direction.SouthWest:
                    return new Point((byte)(origin.X + 1), (byte)(origin.Y + 1));
                case Direction.NorthEast:
                    return new Point((byte)(origin.X - 1), (byte)(origin.Y - 1));
                case Direction.NorthWest:
                    return new Point((byte)(origin.X - 1), (byte)(origin.Y + 1));
            }

            throw new ArgumentException($"Direction value {direction} is not defined", nameof(direction));
        }

        /// <summary>
        /// Negates the specified direction.
        /// </summary>
        /// <param name="direction">The direction.</param>
        /// <returns>The negative direction of the specified diection.</returns>
        public static Direction Negate(this Direction direction)
        {
            switch (direction)
            {
                case Direction.Undefined:
                    return Direction.Undefined;
                case Direction.South:
                    return Direction.North;
                case Direction.North:
                    return Direction.South;
                case Direction.East:
                    return Direction.West;
                case Direction.West:
                    return Direction.East;
                case Direction.SouthEast:
                    return Direction.NorthWest;
                case Direction.SouthWest:
                    return Direction.NorthEast;
                case Direction.NorthEast:
                    return Direction.SouthWest;
                case Direction.NorthWest:
                    return Direction.SouthEast;
            }

            throw new ArgumentException($"Direction value {direction} is not defined", nameof(direction));
        }

        /// <summary>
        /// Rotates the direction by 90 degree to the right.
        /// </summary>
        /// <param name="direction">The direction which should get rotated to the right.</param>
        /// <returns>The rotated direction.</returns>
        public static Direction RotateRight(this Direction direction)
        {
            switch (direction)
            {
                case Direction.Undefined:
                    return Direction.Undefined;
                case Direction.South:
                    return Direction.West;
                case Direction.North:
                    return Direction.East;
                case Direction.East:
                    return Direction.South;
                case Direction.West:
                    return Direction.North;
                case Direction.SouthEast:
                    return Direction.SouthWest;
                case Direction.SouthWest:
                    return Direction.NorthWest;
                case Direction.NorthEast:
                    return Direction.SouthEast;
                case Direction.NorthWest:
                    return Direction.NorthEast;
            }

            throw new ArgumentException($"Direction value {direction} is not defined", nameof(direction));
        }

        /// <summary>
        /// Rotates the direction by 90 degree to the left.
        /// </summary>
        /// <param name="direction">The direction which should get rotated to the left.</param>
        /// <returns>The rotated direction.</returns>
        public static Direction RotateLeft(this Direction direction)
        {
            switch (direction)
            {
                case Direction.Undefined:
                    return Direction.Undefined;
                case Direction.South:
                    return Direction.East;
                case Direction.North:
                    return Direction.West;
                case Direction.East:
                    return Direction.North;
                case Direction.West:
                    return Direction.South;
                case Direction.SouthEast:
                    return Direction.NorthEast;
                case Direction.SouthWest:
                    return Direction.SouthEast;
                case Direction.NorthEast:
                    return Direction.NorthWest;
                case Direction.NorthWest:
                    return Direction.SouthWest;
            }

            throw new ArgumentException($"Direction value {direction} is not defined", nameof(direction));
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
                    return Direction.East;
                }

                return Direction.West;
            }

            if (to.Y == from.Y)
            {
                if (to.X > from.X)
                {
                    return Direction.South;
                }

                return Direction.North;
            }

            if (to.X > from.X)
            {
                if (to.Y > from.Y)
                {
                    return Direction.SouthEast;
                }

                return Direction.SouthWest;
            }

            if (to.Y > from.Y)
            {
                return Direction.NorthEast;
            }

            return Direction.NorthWest;
        }
    }
}