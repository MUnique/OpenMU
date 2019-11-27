// <copyright file="GuildRequestAnswerResult.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.GameLogic.Views.Guild
{
    /// <summary>
    /// Guild join request answer result.
    /// </summary>
    public enum GuildRequestAnswerResult
    {
        /// <summary>
        /// Refused by the guild master.
        /// </summary>
        Refused,

        /// <summary>
        /// Accepted by the guild master.
        /// </summary>
        Accepted,

        /// <summary>
        /// The guild is full.
        /// </summary>
        GuildFull,

        /// <summary>
        /// The guild master is disconnected.
        /// </summary>
        Disconnected,

        /// <summary>
        /// The requested player is not the guild master of its guild.
        /// </summary>
        NotTheGuildMaster,

        /// <summary>
        /// The player already has a guild.
        /// </summary>
        AlreadyHaveGuild,

        /// <summary>
        /// The guild master or the requesting player is busy, e.g. by another request or by an ongoing guild war.
        /// </summary>
        GuildMasterOrRequesterIsBusy,

        /// <summary>
        /// The requesting player needs a minimum level of 6.
        /// </summary>
        MinimumLevel6,
    }
}