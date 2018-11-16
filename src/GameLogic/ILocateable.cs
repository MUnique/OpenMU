// <copyright file="ILocateable.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.GameLogic
{
    using MUnique.OpenMU.Pathfinding;

    /// <summary>
    /// Interface for an object which has a location on a map.
    /// </summary>
    public interface ILocateable : IIdentifiable
    {
        /// <summary>
        /// Gets the current map on which the object currently is.
        /// </summary>
        GameMap CurrentMap { get; }

        /// <summary>
        /// Gets or sets the coordinates on the map.
        /// </summary>
        Point Position { get; set; }
    }

    /// <summary>
    /// Interface for objects which have bucket information.
    /// </summary>
    public interface IHasBucketInformation
    {
        /// <summary>
        /// Gets or sets the current bucket where this instance currently moves at.
        /// This is helpful for other objects to determine if the observation should
        /// be continued or not.
        /// </summary>
        Bucket<ILocateable> NewBucket { get; set; }

        /// <summary>
        /// Gets or sets the bucket where this instance currently moves away.
        /// This is helpful for other objects to determine if the observation should
        /// be continued or not.
        /// </summary>
        Bucket<ILocateable> OldBucket { get; set; }
    }
}
