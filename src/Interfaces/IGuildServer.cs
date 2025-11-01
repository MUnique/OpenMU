// <copyright file="IGuildServer.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.Interfaces;

using System.Collections.Immutable;

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
    /// Checks if the guild with the specified name exists.
    /// </summary>
    /// <param name="guildName">Name of the guild.</param>
    /// <returns>True, if the guild exists; False, otherwise.</returns>
    ValueTask<bool> GuildExistsAsync(string guildName);

    /// <summary>
    /// Gets the guild by the guild identifier.
    /// </summary>
    /// <param name="guildId">The guild identifier.</param>
    /// <returns>The guild.</returns>
    ValueTask<Guild?> GetGuildAsync(uint guildId);

    /// <summary>
    /// Gets the guild id by the guild name.
    /// </summary>
    /// <param name="guildName">The guild name.</param>
    /// <returns>The guild id. <c>0</c>, if not found.</returns>
    ValueTask<uint> GetGuildIdByNameAsync(string guildName);

    /// <summary>
    /// Creates the guild and sets the guild master online at the guild server. A separate call to <see cref="PlayerEnteredGameAsync"/> is not required.
    /// </summary>
    /// <param name="name">The name.</param>
    /// <param name="masterName">Name of the master.</param>
    /// <param name="masterId">The master identifier.</param>
    /// <param name="logo">The logo.</param>
    /// <param name="serverId">The identifier of the server on which the guild is getting created.</param>
    /// <returns>A flag, indicating if the guild has been created successfully.</returns>
    ValueTask<bool> CreateGuildAsync(string name, string masterName, Guid masterId, byte[] logo, byte serverId);

    /// <summary>
    /// Creates the guild member and sets it online at the guild server. A separate call to <see cref="PlayerEnteredGameAsync"/> is not required.
    /// </summary>
    /// <param name="guildId">The guild identifier.</param>
    /// <param name="characterId">The identifier.</param>
    /// <param name="characterName">The name.</param>
    /// <param name="role">The role of the member.</param>
    /// <param name="serverId">The identifier of the server on which the guild member is getting created.</param>
    ValueTask CreateGuildMemberAsync(uint guildId, Guid characterId, string characterName, GuildPosition role, byte serverId);

    /// <summary>
    /// Updates the guild member position.
    /// </summary>
    /// <param name="guildId">The guild identifier.</param>
    /// <param name="characterId">The id of the character.</param>
    /// <param name="role">The role.</param>
    ValueTask ChangeGuildMemberPositionAsync(uint guildId, Guid characterId, GuildPosition role);

    /// <summary>
    /// Notifies the guild server that a player (potential guild member) entered the game.
    /// </summary>
    /// <param name="characterId">The character identifier.</param>
    /// <param name="characterName">Name of the character.</param>
    /// <param name="serverId">The identifier of the server on which the guild member entered.</param>
    ValueTask PlayerEnteredGameAsync(Guid characterId, string characterName, byte serverId);

    /// <summary>
    /// Notifies the guild server that a guild member left the game.
    /// </summary>
    /// <param name="guildId">The guild identifier.</param>
    /// <param name="guildMemberId">The identifier of the guild member.</param>
    /// <param name="serverId">The identifier of the server from which the guild member left.</param>
    ValueTask GuildMemberLeftGameAsync(uint guildId, Guid guildMemberId, byte serverId);

    /// <summary>
    /// Gets the guild member list.
    /// </summary>
    /// <param name="guildId">The guild identifier.</param>
    /// <returns>The guild member list.</returns>
    ValueTask<IImmutableList<GuildListEntry>> GetGuildListAsync(uint guildId);

    /// <summary>
    /// Kicks a guild member from a guild.
    /// </summary>
    /// <param name="guildId">The guild identifier.</param>
    /// <param name="playerName">Name of the player which is getting kicked.</param>
    ValueTask KickMemberAsync(uint guildId, string playerName);

    /// <summary>
    /// Gets the guild position of a specific character.
    /// </summary>
    /// <param name="characterId">The character identifier.</param>
    /// <returns>The guild position.</returns>
    ValueTask<GuildPosition> GetGuildPositionAsync(Guid characterId);

    /// <summary>
    /// Increases the guild score by one.
    /// </summary>
    /// <param name="guildId">The identifier of the guild.</param>
    ValueTask IncreaseGuildScoreAsync(uint guildId);

    /// <summary>
    /// Creates an alliance between two guilds. The requesting guild becomes or uses its existing alliance.
    /// </summary>
    /// <param name="requestingGuildId">The identifier of the requesting guild.</param>
    /// <param name="targetGuildId">The identifier of the target guild to add to the alliance.</param>
    /// <returns>True if the alliance was created successfully; False otherwise.</returns>
    ValueTask<bool> CreateAllianceAsync(uint requestingGuildId, uint targetGuildId);

    /// <summary>
    /// Removes a guild from its alliance.
    /// </summary>
    /// <param name="guildId">The identifier of the guild to remove from the alliance.</param>
    /// <returns>True if the guild was removed successfully; False otherwise.</returns>
    ValueTask<bool> RemoveFromAllianceAsync(uint guildId);

    /// <summary>
    /// Gets all guilds that are part of the same alliance as the specified guild.
    /// </summary>
    /// <param name="guildId">The identifier of the guild.</param>
    /// <returns>A list of guild IDs that are in the same alliance.</returns>
    ValueTask<IImmutableList<uint>> GetAllianceMemberGuildIdsAsync(uint guildId);

    /// <summary>
    /// Gets the alliance master guild ID for a given guild.
    /// </summary>
    /// <param name="guildId">The identifier of the guild.</param>
    /// <returns>The alliance master guild ID, or 0 if the guild is not in an alliance.</returns>
    ValueTask<uint> GetAllianceMasterGuildIdAsync(uint guildId);

    /// <summary>
    /// Creates a hostility relationship between two guilds.
    /// </summary>
    /// <param name="requestingGuildId">The identifier of the requesting guild.</param>
    /// <param name="targetGuildId">The identifier of the target guild.</param>
    /// <returns>True if the hostility was created successfully; False otherwise.</returns>
    ValueTask<bool> CreateHostilityAsync(uint requestingGuildId, uint targetGuildId);

    /// <summary>
    /// Removes a hostility relationship between two guilds.
    /// </summary>
    /// <param name="guildId">The identifier of the guild.</param>
    /// <returns>True if the hostility was removed successfully; False otherwise.</returns>
    ValueTask<bool> RemoveHostilityAsync(uint guildId);
}

/// <summary>
/// The guild list entry.
/// </summary>
public class GuildListEntry
{
    /// <summary>
    /// Gets or sets the name of the player.
    /// </summary>
    public string? PlayerName { get; set; }

    /// <summary>
    /// Gets or sets the server identifier on which the player is playing.
    /// </summary>
    public byte ServerId { get; set; }

    /// <summary>
    /// Gets or sets the players position in the guild.
    /// </summary>
    public GuildPosition PlayerPosition { get; set; }
}