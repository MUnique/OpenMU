// <copyright file="IAreaOfInterestManager.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.GameLogic
{
    using System.Collections.Generic;
    using MUnique.OpenMU.GameLogic.Views;

    /// <summary>
    /// A manager of an area of interest.
    /// </summary>
    public interface IAreaOfInterestManager
    {
        /// <summary>
        /// Adds the object to the area of interest.
        /// </summary>
        /// <param name="obj">The object.</param>
        void AddObject(ILocateable obj);

        /// <summary>
        /// Removes the object from the area of interest.
        /// </summary>
        /// <param name="obj">The object.</param>
        void RemoveObject(ILocateable obj);

        /// <summary>
        /// Moves the object.
        /// </summary>
        /// <param name="obj">The object.</param>
        /// <param name="newX">The new x.</param>
        /// <param name="newY">The new y.</param>
        /// <param name="moveLock">The move lock.</param>
        /// <param name="moveType">Type of the move.</param>
        void MoveObject(ILocateable obj, byte newX, byte newY, object moveLock, MoveType moveType);

        /// <summary>
        /// Gets the object in range.
        /// </summary>
        /// <param name="x">The x.</param>
        /// <param name="y">The y.</param>
        /// <param name="range">The range.</param>
        /// <param name="rangeType">Type of the range.</param>
        /// <returns>The objects in range.</returns>
        IEnumerable<ILocateable> GetInRange(int x, int y, int range, RangeType rangeType);
    }
}
