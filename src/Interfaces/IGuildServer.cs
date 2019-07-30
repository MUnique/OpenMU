// <copyright file="IGuildServer.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.Interfaces
{
    using System;
    using System.Collections.Generic;

    /// <summary>
    /// Interface for the guild server.
    /// </summary>
    /// <remarks>
    /// A little note about the guild id:
    ///   The original GMO server uses an 32-bit integer in all of its messages. However, actually it's only using (or used?) 16 bits of it for created guilds (see struct SDHP_GUILDCREATED).
    ///   Some people may remember the "guildbug" on GMO - I guess the keys exceeded these 16 bits and somehow caused a crash... but after restart of the servers it started working again.
    /// </remarks>
    public interface IGuildServer
    {
        /// <summary>
        /// Notifies the guild server that a guild message was sent and maybe needs to be forwarded to the game servers.
        /// </summary>
        /// <param name="guildId">The guild id.</param>
        /// <param name="sender">The sender.</param>
        /// <param name="message">The message.</param>
        void GuildMessage(uint guildId, string sender, string message);

        /// <summary>
        /// Notifies the guild server that an alliance message was sent and maybe needs to be forwarded to the game servers.
        /// </summary>
        /// <param name="guildId">The guild id.</param>
        /// <param name="sender">The sender.</param>
        /// <param name="message">The message.</param>
        void AllianceMessage(uint guildId, string sender, string message);

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
        Guild GetGuild(uint guildId);

        /// <summary>
        /// Creates the guild and sets the guild master online at the guild server. A separate call to <see cref="PlayerEnteredGame"/> is not required.
        /// </summary>
        /// <param name="name">The name.</param>
        /// <param name="masterName">Name of the master.</param>
        /// <param name="masterId">The master identifier.</param>
        /// <param name="logo">The logo.</param>
        /// <param name="serverId">The identifier of the server on which the guild is getting created.</param>
        /// <returns>The guild member status of the creator (guild master).</returns>
        GuildMemberStatus CreateGuild(string name, string masterName, Guid masterId, byte[] logo, byte serverId);

        /// <summary>
        /// Creates the guild member and sets it online at the guild server. A separate call to <see cref="PlayerEnteredGame"/> is not required.
        /// </summary>
        /// <param name="guildId">The guild identifier.</param>
        /// <param name="characterId">The identifier.</param>
        /// <param name="characterName">The name.</param>
        /// <param name="role">The role of the member.</param>
        /// <param name="serverId">The identifier of the server on which the guild member is getting created.</param>
        /// <returns>The created guild member info.</returns>
        GuildMemberStatus CreateGuildMember(uint guildId, Guid characterId, string characterName, GuildPosition role, byte serverId);

        /// <summary>
        /// Updates the guild member position.
        /// </summary>
        /// <param name="guildId">The guild identifier.</param>
        /// <param name="characterId">The id of the character.</param>
        /// <param name="role">The role.</param>
        void ChangeGuildMemberPosition(uint guildId, Guid characterId, GuildPosition role);

        /// <summary>
        /// Notifies the guild server that a player (potential guild member) entered the game.
        /// </summary>
        /// <param name="characterId">The character identifier.</param>
        /// <param name="characterName">Name of the character.</param>
        /// <param name="serverId">The identifier of the server on which the guild member entered.</param>
        /// <returns>The guild member status if it's a guild member; Otherwise, null.</returns>
        GuildMemberStatus PlayerEnteredGame(Guid characterId, string characterName, byte serverId);

        /// <summary>
        /// Notifies the guild server that a guild member left the game.
        /// </summary>
        /// <param name="guildId">The guild identifier.</param>
        /// <param name="guildMemberId">The identifier of the guild member.</param>
        /// <param name="serverId">The identifier of the server from which the guild member left.</param>
        void GuildMemberLeftGame(uint guildId, Guid guildMemberId, byte serverId);

        /// <summary>
        /// Gets the guild member list.
        /// </summary>
        /// <param name="guildId">The guild identifier.</param>
        /// <returns>The guild member list.</returns>
        IEnumerable<GuildListEntry> GetGuildList(uint guildId);

        /// <summary>
        /// Kicks a guild member from a guild.
        /// </summary>
        /// <param name="guildId">The guild identifier.</param>
        /// <param name="playerName">Name of the player which is getting kicked.</param>
        void KickMember(uint guildId, string playerName);

        /// <summary>
        /// Gets the guild position of a specific character.
        /// </summary>
        /// <param name="characterId">The character identifier.</param>
        /// <returns>The guild position.</returns>
        GuildPosition? GetGuildPosition(Guid characterId);
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
