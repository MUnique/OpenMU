// <copyright file="LocateableExtensions.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.GameLogic;

using MUnique.OpenMU.GameLogic.Attributes;
using MUnique.OpenMU.Pathfinding;

/// <summary>
/// Extensions for mu objects.
/// </summary>
public static class LocateableExtensions
{
    /// <summary>
    /// Filters out inactive (non-alive) locateables.
    /// </summary>
    /// <typeparam name="T">Type of elements.</typeparam>
    /// <param name="locateables">The locateables.</param>
    /// <returns>
    /// All active locateables of the given enumeration.
    /// </returns>
    public static IEnumerable<T> WhereActive<T>(this IEnumerable<T> locateables)
        where T : ILocateable
    {
        return locateables.Where(l => l.IsActive());
    }

    /// <summary>
    /// Filters out invisible locateables.
    /// </summary>
    /// <typeparam name="T">Type of elements.</typeparam>
    /// <param name="locateables">The locateables.</param>
    /// <returns>
    /// All visible locateables of the given enumeration.
    /// </returns>
    public static IEnumerable<T> WhereNotInvisible<T>(this IEnumerable<T> locateables)
        where T : IAttackable
    {
        return locateables.Where(l => l.Attributes[Stats.IsInvisible] == 0);
    }

    /// <summary>
    /// Determines whether this instance is active (alive).
    /// </summary>
    /// <param name="locateable">The locateable.</param>
    /// <returns>
    ///   <c>true</c> if the specified locateable is active; otherwise, <c>false</c>.
    /// </returns>
    public static bool IsActive(this ILocateable locateable)
    {
        return locateable is not IAttackable attackable || (attackable.IsAlive && !attackable.IsTeleporting);
    }

    /// <summary>
    /// Gets the distance to another object.
    /// </summary>
    /// <param name="objectFrom">The object from which the distance is calculated.</param>
    /// <param name="objectTo">The object to which the distance is calculated.</param>
    /// <returns>The distance between this and another object.</returns>
    public static double GetDistanceTo(this ILocateable objectFrom, ILocateable objectTo)
    {
        return objectFrom.GetDistanceTo(objectTo.Position);
    }

    /// <summary>
    /// Gets the distance to another point.
    /// </summary>
    /// <param name="objectFrom">The object from which the distance is calculated.</param>
    /// <param name="objectToPosition">The point to which the distance is calculated.</param>
    /// <returns>The distance between this and another object.</returns>
    public static double GetDistanceTo(this ILocateable objectFrom, Point objectToPosition)
    {
        return objectFrom.Position.EuclideanDistanceTo(objectToPosition);
    }

    /// <summary>
    /// Determines whether the specified coordinates are in the specified range of the object.
    /// </summary>
    /// <param name="obj">The object.</param>
    /// <param name="point">The coordinates.</param>
    /// <param name="range">The maximum range.</param>
    /// <returns><c>True</c>, if the specified coordinate is in the specified range of the object; Otherwise, <c>false</c>.</returns>
    public static bool IsInRange(this ILocateable obj, Point point, int range) => obj.IsInRange(point.X, point.Y, range);

    /// <summary>
    /// Determines whether the specified coordinates are in the specified range of the object.
    /// </summary>
    /// <param name="obj">The object.</param>
    /// <param name="obj2">The second object.</param>
    /// <param name="range">The maximum range.</param>
    /// <returns><c>True</c>, if the specified coordinate is in the specified range of the object; Otherwise, <c>false</c>.</returns>
    public static bool IsInRange(this ILocateable obj, ILocateable obj2, int range) => obj.IsInRange(obj2.Position, range);

    /// <summary>
    /// Determines whether the specified coordinate is in the specified range of the object.
    /// </summary>
    /// <param name="locatable">The object.</param>
    /// <param name="x">The x coordinate.</param>
    /// <param name="y">The y coordinate.</param>
    /// <param name="range">The maximum range.</param>
    /// <returns><c>True</c>, if the specified coordinate is in the specified range of the object; Otherwise, <c>false</c>.</returns>
    public static bool IsInRange(this ILocateable locatable, int x, int y, int range)
    {
        int xdiff;
        int ydiff;
        var point = locatable.Position;
        if (x < point.X)
        {
            xdiff = point.X - x;
        }
        else if (x > point.X)
        {
            xdiff = x - point.X;
        }
        else
        {
            xdiff = 0;
        }

        if (xdiff > range)
        {
            return false;
        }

        if (y < point.Y)
        {
            ydiff = point.Y - y;
        }
        else if (y > point.Y)
        {
            ydiff = y - point.Y;
        }
        else
        {
            ydiff = 0;
        }

        return ydiff <= range;
    }

    /// <summary>
    /// Gets the direction to another object.
    /// </summary>
    /// <param name="objectFrom">The object from which the direction is calculated.</param>
    /// <param name="objectTo">The object to which the direction is calculated.</param>
    /// <returns>The direction between this and another object.</returns>
    /// <remarks>
    ///       The returned values differ a bit, so we first have to analyze which function is correct.
    /// </remarks>
    public static Direction GetDirectionTo(this ILocateable objectFrom, ILocateable objectTo) => objectFrom.Position.GetDirectionTo(objectTo.Position);

    /// <summary>
    /// Determines whether the object is at the safezone of his current map.
    /// </summary>
    /// <param name="obj">The object.</param>
    /// <returns>True, if it is on the safezone of his current map; Otherwise, false.</returns>
    public static bool IsAtSafezone(this ILocateable obj)
    {
        var map = obj.CurrentMap;
        if (map?.Terrain?.SafezoneMap is null)
        {
            return true;
        }

        return map.Terrain.SafezoneMap[obj.Position.X, obj.Position.Y];
    }
}