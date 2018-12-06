// <copyright file="IGuildView.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.GameLogic.Views
{
    using System.Collections.Generic;
    using MUnique.OpenMU.Interfaces;

    /// <summary>
    /// Guild create error detail code.
    /// </summary>
    public enum GuildCreateErrorDetail : byte
    {
        /// <summary>
        /// No error occured.
        /// </summary>
        None = 0,

        /// <summary>
        /// The guild already exists.
        /// </summary>
        GuildAlreadyExist = 0xB3
    }

    /// <summary>
    /// Type of the guild kick success.
    /// </summary>
    public enum GuildKickSuccess : byte
    {
        /// <summary>
        /// Kicking failed. Player stays at guild.
        /// </summary>
        Failed = 0,

        /// <summary>
        /// Kicking succeeded. Player left the guild.
        /// </summary>
        KickSucceeded = 1,

        /// <summary>
        /// Kicking succeeded and guild got disbanded.
        /// </summary>
        GuildDisband = 4
    }

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

    /// <summary>
    /// The guild view.
    /// </summary>
    public interface IGuildView
    {
        /// <summary>
        /// A Player the left his guild.
        /// </summary>
        /// <param name="player">The player who left his guild.</param>
        void PlayerLeftGuild(Player player);

        /// <summary>
        /// Shows the guild create result.
        /// </summary>
        /// <param name="errorDetail">The error detail.</param>
        void ShowGuildCreateResult(GuildCreateErrorDetail errorDetail);

        /// <summary>
        /// Shows the guild information of a previously requested guild.
        /// </summary>
        /// <param name="guildInfo">The guild information.</param>
        void ShowGuildInfo(byte[] guildInfo);

        /// <summary>
        /// Assigns the players to their guilds.
        /// </summary>
        /// <param name="guildPlayers">The players of a guild.</param>
        /// <param name="appearsNew">If set to <c>true</c>,the player just appeared in the view. Otherwise, the players were already in the view, and freshly assigned to the guild.</param>
        void AssignPlayersToGuild(ICollection<Player> guildPlayers, bool appearsNew);

        /// <summary>
        /// Assigns the specified player to its guild.
        /// </summary>
        /// <param name="guildPlayer">The player of a guild.</param>
        /// <param name="appearsNew">If set to <c>true</c>,the player just appeared in the view. Otherwise, the players were already in the view, and freshly assigned to the guild.</param>
        void AssignPlayerToGuild(Player guildPlayer, bool appearsNew);

        /// <summary>
        /// Shows the result of the kick request.
        /// </summary>
        /// <param name="successCode">The success code.</param>
        void GuildKickResult(GuildKickSuccess successCode);

        /// <summary>
        /// Shows the guild list.
        /// </summary>
        /// <param name="players">The players of the guild.</param>
        void ShowGuildList(IEnumerable<GuildListEntry> players);

        /// <summary>
        /// Shows the guild join request.
        /// </summary>
        /// <param name="requester">The requester.</param>
        void ShowGuildJoinRequest(Player requester);

        /// <summary>
        /// Shows the guild creation dialog.
        /// </summary>
        void ShowGuildCreationDialog();

        /// <summary>
        /// Shows the guild join response from the guild master.
        /// </summary>
        /// <param name="response">The response.</param>
        void GuildJoinResponse(GuildRequestAnswerResult response);
    }
}
