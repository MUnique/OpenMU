// <copyright file="LocateableExtensions.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.GameLogic
{
    using System;

    /// <summary>
    /// Extensions for mu objects.
    /// </summary>
    public static class LocateableExtensions
    {
        /// <summary>
        /// Gets the distance to another object.
        /// </summary>
        /// <param name="objectFrom">The object from which the distance is calculated.</param>
        /// <param name="objectTo">The object to which the distance is calculated.</param>
        /// <returns>The distance between this and another object.</returns>
        public static double GetDistanceTo(this ILocateable objectFrom, ILocateable objectTo)
        {
            return objectFrom.GetDistanceTo(objectTo.X, objectTo.Y);
        }

        /// <summary>
        /// Gets the distance to the specified coordinates.
        /// </summary>
        /// <param name="objectFrom">The object from which the distance is calculated.</param>
        /// <param name="x">The x coordinate of the point to which the distance is calculated.</param>
        /// <param name="y">The y coordinate of the point to which the distance is calculated.</param>
        /// <returns>The distance between this and the specified coordinates.</returns>
        public static double GetDistanceTo(this ILocateable objectFrom, byte x, byte y)
        {
            return
                    Math.Sqrt(
                        Math.Pow(objectFrom.X - x, 2) +
                        Math.Pow(objectFrom.Y - y, 2));
        }

        /// <summary>
        /// Determines whether the specified coordinate is in the specified range of the object.
        /// </summary>
        /// <param name="obj">The object.</param>
        /// <param name="x">The x coordinate.</param>
        /// <param name="y">The y coorindate.</param>
        /// <param name="range">The maximum range.</param>
        /// <returns><c>True</c>, if rhe specified coordinate is in the specified range of the object; Otherwise, <c>false</c>.</returns>
        public static bool IsInRange(this ILocateable obj, int x, int y, int range)
        {
            int xdiff;
            int ydiff;
            if (x < obj.X)
            {
                xdiff = obj.X - x;
            }
            else if (x > obj.X)
            {
                xdiff = x - obj.X;
            }
            else
            {
                xdiff = 0;
            }

            if (xdiff > range)
            {
                return false;
            }

            if (y < obj.Y)
            {
                ydiff = obj.Y - y;
            }
            else if (y > obj.Y)
            {
                ydiff = y - obj.Y;
            }
            else
            {
                ydiff = 0;
            }

            return (xdiff < range) && (ydiff < range);
        }

        /// <summary>
        /// Gets the direction to another object.
        /// </summary>
        /// <param name="objectFrom">The object from which the direction is calculated.</param>
        /// <param name="objectTo">The object to which the direction is calculated.</param>
        /// <returns>The direction between this and another object.</returns>
        public static byte GetDirectionTo(this ILocateable objectFrom, ILocateable objectTo)
        {
            byte dir = 0;
            if ((objectFrom.X < objectTo.X) && (objectFrom.Y < objectTo.Y))
            {
                dir = 0;
            }
            else if ((objectFrom.X == objectTo.X) && (objectFrom.Y < objectTo.Y))
            {
                dir = 1;
            }
            else if ((objectFrom.X > objectTo.X) && (objectFrom.Y < objectTo.Y))
            {
                dir = 2;
            }
            else if ((objectFrom.X > objectTo.X) && (objectFrom.Y == objectTo.Y))
            {
                dir = 3;
            }
            else if ((objectFrom.X > objectTo.X) && (objectFrom.Y > objectTo.Y))
            {
                dir = 4;
            }
            else if ((objectFrom.X == objectTo.X) && (objectFrom.Y > objectTo.Y))
            {
                dir = 5;
            }
            else if ((objectFrom.X < objectTo.X) && (objectFrom.Y > objectTo.Y))
            {
                dir = 6;
            }
            else if ((objectFrom.X < objectTo.X) && (objectFrom.Y == objectTo.Y))
            {
                dir = 7;
            }

            return dir;
        }

        /// <summary>
        /// Determines whether the object is at the safezone of his current map.
        /// </summary>
        /// <param name="obj">The object.</param>
        /// <returns>True, if it is on the safezone of his current map; Otherwise, false.</returns>
        public static bool IsAtSafezone(this ILocateable obj)
        {
            var map = obj.CurrentMap;
            if (map == null)
            {
                return true;
            }

            return map.Terrain.SafezoneMap[obj.X, obj.Y];
        }
    }
}
