// <copyright file="IGuildServer.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.Interfaces;

using System.Collections.Immutable;

/// <summary>
/// Describes the relationship between two guilds.
/// </summary>
public enum GuildRelationship
{
    /// <summary>
    /// No special relationship.
    /// </summary>
    None = 0,

    /// <summary>
    /// Both guilds are in the same alliance.
    /// </summary>
    Union = 1,

    /// <summary>
    /// The guilds are rivals / hostile to each other.
    /// </summary>
    Rival = 2,
}

/// <summary>
/// Defines the result of an alliance creation attempt.
/// </summary>
public enum AllianceCreationResult
{
    /// <summary>
    /// The alliance creation failed for an unspecified reason.
    /// </summary>
    Failed,

    /// <summary>
    /// The alliance was created successfully.
    /// </summary>
    Success,

    /// <summary>
    /// The master guild could not be found.
    /// </summary>
    MasterGuildNotFound,

    /// <summary>
    /// The target guild could not be found.
    /// </summary>
    TargetGuildNotFound,

    /// <summary>
    /// The target guild is already a member of an alliance.
    /// </summary>
    TargetGuildAlreadyInAlliance,

    /// <summary>
    /// The maximum number of guilds allowed in an alliance has been reached.
    /// </summary>
    MaximumAllianceSizeReached,

    /// <summary>
    /// The guild could not be found in the target context.
    /// </summary>
    GuildNotFoundInTargetContext,

    /// <summary>
    /// An unexpected error occurred during alliance creation.
    /// </summary>
    Error,
}

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
    /// Creates an alliance between the master guild and the target guild.
    /// The master guild becomes (or remains) the alliance master.
    /// </summary>
    /// <param name="masterGuildId">The identifier of the master guild that initiates the alliance.</param>
    /// <param name="targetGuildId">The identifier of the target guild to add to the alliance.</param>
    /// <returns><c>true</c> if the alliance was created successfully; <c>false</c> otherwise.</returns>
    ValueTask<AllianceCreationResult> CreateAllianceAsync(uint masterGuildId, uint targetGuildId);

    /// <summary>
    /// Removes a guild from an alliance. Only the alliance master can remove members.
    /// </summary>
    /// <param name="masterGuildId">The identifier of the alliance master guild.</param>
    /// <param name="targetGuildId">The identifier of the guild to remove from the alliance.</param>
    /// <returns><c>true</c> if the guild was removed successfully; <c>false</c> otherwise.</returns>
    ValueTask<bool> RemoveAllianceGuildAsync(uint masterGuildId, uint targetGuildId);

    /// <summary>
    /// Disbands the entire alliance, clearing alliance membership for all guilds.
    /// </summary>
    /// <param name="masterGuildId">The identifier of the alliance master guild.</param>
    /// <returns><c>true</c> if the alliance was disbanded successfully; <c>false</c> otherwise.</returns>
    ValueTask<bool> DisbandAllianceAsync(uint masterGuildId);

    /// <summary>
    /// Gets the list of guilds in the alliance of the specified guild.
    /// </summary>
    /// <param name="guildId">The identifier of any guild in the alliance.</param>
    /// <returns>The list of alliance guilds.</returns>
    ValueTask<IImmutableList<AllianceGuildEntry>> GetAllianceGuildsAsync(uint guildId);

    /// <summary>
    /// Determines whether the specified guild is the alliance master.
    /// </summary>
    /// <param name="guildId">The guild identifier.</param>
    /// <returns><c>true</c> if the guild is the alliance master; <c>false</c> otherwise.</returns>
    ValueTask<bool> IsAllianceMasterAsync(uint guildId);

    /// <summary>
    /// Gets the alliance master guild identifier for the given guild.
    /// </summary>
    /// <param name="guildId">The guild identifier of any alliance member.</param>
    /// <returns>The uint identifier of the alliance master guild, or 0 if not in an alliance.</returns>
    ValueTask<uint> GetAllianceMasterIdAsync(uint guildId);

    /// <summary>
    /// Sets or clears the hostility between guilds.
    /// </summary>
    /// <param name="guildId">The guild identifier of the requesting guild.</param>
    /// <param name="targetGuildId">The identifier of the target guild. Only used when <paramref name="create"/> is <c>true</c>.</param>
    /// <param name="create"><c>true</c> to set hostility; <c>false</c> to clear any existing hostility.</param>
    /// <returns><c>true</c> if the hostility state was changed successfully; <c>false</c> otherwise.</returns>
    ValueTask<bool> SetHostilityAsync(uint guildId, uint targetGuildId, bool create);

    /// <summary>
    /// Gets the relationship between two guilds.
    /// </summary>
    /// <param name="guild1">The first guild identifier.</param>
    /// <param name="guild2">The second guild identifier.</param>
    /// <returns>The relationship between the two guilds.</returns>
    ValueTask<GuildRelationship> GetGuildRelationshipAsync(uint guild1, uint guild2);
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