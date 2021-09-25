// <copyright file="MiniGameMapCreationPolicy.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.DataModel.Configuration
{
    /// <summary>
    /// Defines how a mini game map is created.
    /// </summary>
    public enum MiniGameMapCreationPolicy
    {
        /// <summary>
        /// One map is created per party and game level.
        /// </summary>
        OnePerParty,

        /// <summary>
        /// One map is created for each player.
        /// </summary>
        OnePerPlayer,

        /// <summary>
        /// One map is created and shared for all players.
        /// </summary>
        Shared,
    }
}