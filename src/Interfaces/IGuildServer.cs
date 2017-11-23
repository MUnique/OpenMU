// <copyright file="IGuildServer.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.Interfaces
{
    using System;
    using System.Collections.Generic;
    using MUnique.OpenMU.DataModel.Entities;

    /// <summary>
    /// Interface for the guild server.
    /// </summary>
    public interface IGuildServer
    {
        /// <summary>
        /// Notifies the guild server that a guild message was sent and maybe needs to be forwarded to the game servers.
        /// </summary>
        /// <param name="guildId">The guild id.</param>
        /// <param name="sender">The sender.</param>
        /// <param name="message">The message.</param>
        void GuildMessage(Guid guildId, string sender, string message);

        /// <summary>
        /// Notifies the guild server that an alliance message was sent and maybe needs to be forwarded to the game servers.
        /// </summary>
        /// <param name="guildId">The guild id.</param>
        /// <param name="sender">The sender.</param>
        /// <param name="message">The message.</param>
        void AllianceMessage(Guid guildId, string sender, string message);

        /// <summary>
        /// Checks if the guild with the specified name exists.
        /// </summary>
        /// <param name="guildName">Name of the guild.</param>
        /// <returns>True, if the guild exists; False, otherwise.</returns>
        bool GuildExists(string guildName);

        /// <summary>
        /// Gets the guild by the guild identifier.
        /// </summary>
        /// <param name="guildId">The guild identifier.</param>
        /// <returns>The guild.</returns>
        Guild GetGuild(Guid guildId);

        /// <summary>
        /// Creates the guild.
        /// </summary>
        /// <param name="name">The name.</param>
        /// <param name="masterName">Name of the master.</param>
        /// <param name="masterId">The master identifier.</param>
        /// <param name="logo">The logo.</param>
        /// <param name="shortGuildId">The short guild identifier.</param>
        /// <param name="masterGuildMemberInfo">The master guild member information.</param>
        /// <returns>
        /// The identifier of the created guild.
        /// </returns>
        Guild CreateGuild(string name, string masterName, Guid masterId, byte[] logo, out ushort shortGuildId, out GuildMemberInfo masterGuildMemberInfo);

        /// <summary>
        /// Updates the guild.
        /// </summary>
        /// <param name="guild">The guild.</param>
        /// <returns>The success.</returns>
        bool UpdateGuild(Guild guild);

        /// <summary>
        /// Deletes the guild with the specified id.
        /// </summary>
        /// <param name="guildId">The guild identifier.</param>
        /// <returns>The success.</returns>
        bool DeleteGuild(Guid guildId);

        /// <summary>
        /// Creates the guild member.
        /// </summary>
        /// <param name="guildId">The guild identifier.</param>
        /// <param name="characterId">The identifier.</param>
        /// <param name="characterName">The name.</param>
        /// <param name="role">The role of the member.</param>
        /// <returns>The created guild member info.</returns>
        GuildMemberInfo CreateGuildMember(Guid guildId, Guid characterId, string characterName, GuildPosition role);

        /// <summary>
        /// Updates the guild member.
        /// </summary>
        /// <param name="guildMember">The guild member.</param>
        /// <returns>The success.</returns>
        bool UpdateGuildMember(GuildMemberInfo guildMember);

        /// <summary>
        /// Deletes the guild member.
        /// </summary>
        /// <param name="guildMember">The guild member.</param>
        /// <returns>The success.</returns>
        bool DeleteGuildMember(GuildMemberInfo guildMember);

        /// <summary>
        /// Notifies the guild server that a guild member entered the game.
        /// </summary>
        /// <param name="guildId">The guild identifier.</param>
        /// <param name="guildMemberName">Name of the guild member.</param>
        /// <param name="serverId">The identifier of the server on which the guild member entered.</param>
        /// <returns>The short guild identifier.</returns>
        ushort GuildMemberEnterGame(Guid guildId, string guildMemberName, byte serverId);

        /// <summary>
        /// Notifies the guild server that a guild member left the game.
        /// </summary>
        /// <param name="guildId">The guild identifier.</param>
        /// <param name="guildMemberName">Name of the guild member.</param>
        /// <param name="serverId">The identifier of the server from which the guild member left.</param>
        void GuildMemberLeaveGame(Guid guildId, string guildMemberName, byte serverId);

        /// <summary>
        /// Gets the guild member list.
        /// </summary>
        /// <param name="guildId">The guild identifier.</param>
        /// <returns>The guild member list.</returns>
        IEnumerable<GuildListEntry> GetGuildList(Guid guildId);

        /// <summary>
        /// Kicks a guild member from a guild.
        /// </summary>
        /// <param name="guildId">The guild identifier.</param>
        /// <param name="playerName">Name of the player which is getting kicked.</param>
        void KickPlayer(Guid guildId, string playerName);
    }

    /// <summary>
    /// The guild list entry.
    /// </summary>
    public class GuildListEntry
    {
        /// <summary>
        /// Gets or sets the name of the player.
        /// </summary>
        public string PlayerName { get; set; }

        /// <summary>
        /// Gets or sets the server identifier on which the player is playing.
        /// </summary>
        public byte ServerId { get; set; }

        /// <summary>
        /// Gets or sets the players position in the guild.
        /// </summary>
        public GuildPosition PlayerPosition { get; set; }
    }
}
