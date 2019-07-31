// <copyright file="MoveType.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.GameLogic.Views.World
{
    /// <summary>
    /// Specifies the move type of objects.
    /// </summary>
    public enum MoveType
    {
        /// <summary>
        /// Moving by walking.
        /// </summary>
        Walk,

        /// <summary>
        /// Moving by instantly moving the object on the map, without animations.
        /// </summary>
        Instant,
    }
}