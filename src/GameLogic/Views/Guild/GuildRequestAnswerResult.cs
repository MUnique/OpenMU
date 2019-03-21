// <copyright file="GuildRequestAnswerResult.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.GameLogic.Views.Guild
{
    /// <summary>
    /// Guild join request answer result.
    /// </summary>
    public enum GuildRequestAnswerResult : byte
    {
        /// <summary>
        /// Refused by the guild master.
        /// </summary>
        Refused = 0,

        /// <summary>
        /// Accepted by the guild master.
        /// </summary>
        Accepted = 1,

        /// <summary>
        /// The player already has a guild.
        /// </summary>
        AlreadyHaveGuild = 5
    }
}