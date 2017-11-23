// <copyright file="ILocateable.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.GameLogic
{
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
        /// Gets or sets the x coordinate on the map.
        /// </summary>
        byte X { get; set; }

        /// <summary>
        /// Gets or sets the y coordinate on the map.
        /// </summary>
        byte Y { get; set; }
    }

    /// <summary>
    /// Interface for objects which have bucket information.
    /// </summary>
    public interface IHasBucketInformation
    {
        /// <summary>
        /// Gets or sets the current bucket where this instance currently is in.
        /// This is helpful for other objects to determine if the observation should
        /// be continued or not.
        /// </summary>
        Bucket<ILocateable> CurrentBucket { get; set; }
    }
}
