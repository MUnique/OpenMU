// <copyright file="Direction.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.DataModel.Configuration
{
    /// <summary>
    /// Direction where an object is looking at.
    /// </summary>
    /// <remarks>
    /// Direction Matrix (P = Player), similar to how it's shown in the web live map:
    ///     Y | - | y | +
    ///   X   |   |   |
    /// -------------------
    ///   -   | 4 | 5 | 6
    ///   x   | 3 | P | 7
    ///   +   | 2 | 1 | 8
    /// </remarks>
    public enum Direction
    {
        /// <summary>
        /// The undefined direction.
        /// </summary>
        Undefined = 0,

        /// <summary>
        /// The direction looking to the south.
        /// </summary>
        South = 1,

        /// <summary>
        /// The direction looking to the south east.
        /// </summary>
        SouthEast = 2,

        /// <summary>
        /// The direction looking to the east.
        /// </summary>
        East = 3,

        /// <summary>
        /// The direction looking to the north east.
        /// </summary>
        NorthEast = 4,

        /// <summary>
        /// The direction looking to the north.
        /// </summary>
        North = 5,

        /// <summary>
        /// The direction looking to the north west.
        /// </summary>
        NorthWest = 6,

        /// <summary>
        /// The direction looking to the west.
        /// </summary>
        West = 7,

        /// <summary>
        /// The direction looking to the south west.
        /// </summary>
        SouthWest = 8,
    }
}