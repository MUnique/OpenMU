// <copyright file="IAreaOfInterestManager.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.GameLogic;

using MUnique.OpenMU.GameLogic.Views.World;
using MUnique.OpenMU.Pathfinding;
using Nito.AsyncEx;

/// <summary>
/// A manager of an area of interest.
/// </summary>
public interface IAreaOfInterestManager
{
    /// <summary>
    /// Adds the object to the area of interest.
    /// </summary>
    /// <param name="obj">The object.</param>
    ValueTask AddObjectAsync(ILocateable obj);

    /// <summary>
    /// Removes the object from the area of interest.
    /// </summary>
    /// <param name="obj">The object.</param>
    ValueTask RemoveObjectAsync(ILocateable obj);

    /// <summary>
    /// Moves the object.
    /// </summary>
    /// <param name="obj">The object.</param>
    /// <param name="target">The new coordinates.</param>
    /// <param name="moveLock">The move lock.</param>
    /// <param name="moveType">Type of the move.</param>
    /// <param name="notifyMovedObject">
    /// If set to <c>true</c>, the moved object is notified about its own move as well.
    /// Pass <c>false</c> when the move only confirms a position the object already reported itself.
    /// </param>
    ValueTask MoveObjectAsync(ILocateable obj, Point target, AsyncLock moveLock, MoveType moveType, bool notifyMovedObject = true);

    /// <summary>
    /// Gets the object in range.
    /// </summary>
    /// <param name="point">The point at which the objects are searched in the specified range.</param>
    /// <param name="range">The range.</param>
    /// <returns>The objects in range.</returns>
    IEnumerable<ILocateable> GetInRange(Point point, int range);
}