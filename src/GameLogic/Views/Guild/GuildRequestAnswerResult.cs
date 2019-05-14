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
        /// The guild is full.
        /// </summary>
        GuildFull = 2,

        /// <summary>
        /// The guild master is disconnected.
        /// </summary>
        Disconnected = 3,

        /// <summary>
        /// The requested player is not the guild master of its guild.
        /// </summary>
        NotTheGuildMaster = 4,

        /// <summary>
        /// The player already has a guild.
        /// </summary>
        AlreadyHaveGuild = 5,

        /// <summary>
        /// The guild master or the requesting player is busy, e.g. by another request or by an ongoing guild war.
        /// </summary>
        GuildMasterOrRequesterIsBusy = 6,

        /// <summary>
        /// The requesting player needs a minimum level of 6.
        /// </summary>
        MinimumLevel6 = 7,
    }
}