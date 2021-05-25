// <copyright file="GuildWarType.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.GameLogic.GuildWar
{
    /// <summary>
    /// Defines the type of a guild war.
    /// </summary>
    public enum GuildWarType
    {
        /// <summary>
        /// A normal guild war, where two parties fight against each other on a normal game map.
        /// </summary>
        Normal,

        /// <summary>
        /// A battle soccer match, where two parties play against each other on a <see cref="SoccerGameMap"/>.
        /// </summary>
        Soccer,
    }
}