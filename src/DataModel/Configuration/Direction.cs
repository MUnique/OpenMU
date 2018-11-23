// <copyright file="Direction.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.DataModel.Configuration
{
    /// <summary>
    /// Direction where an object is looking at.
    /// </summary>
    /// <remarks>
    /// The directions are named/valued after how they look like due the game client.
    /// That means, when a character looks to south, it looks straight downwards. Because the map is rotated on the game client, that's actually a corner.
    /// Since we use the value 0 as 'Undefined' and the original game client uses value 0 as 'West', this has to be considered when communicating with it.
    /// </remarks>
    public enum Direction
    {
        /// <summary>
        /// The undefined direction.
        /// </summary>
        Undefined = 0,

        /// <summary>
        /// The direction looking to the west.
        /// </summary>
        West = 1,

        /// <summary>
        /// The direction looking to the south east.
        /// </summary>
        SouthWest = 2,

        /// <summary>
        /// The direction looking to the south.
        /// </summary>
        South = 3,

        /// <summary>
        /// The direction looking to the south west.
        /// </summary>
        SouthEast = 4,

        /// <summary>
        /// The direction looking to the east.
        /// </summary>
        East = 5,

        /// <summary>
        /// The direction looking to the north east.
        /// </summary>
        NorthEast = 6,

        /// <summary>
        /// The direction looking to the north.
        /// </summary>
        North = 7,

        /// <summary>
        /// The direction looking to the north west.
        /// </summary>
        NorthWest = 8,
    }
}