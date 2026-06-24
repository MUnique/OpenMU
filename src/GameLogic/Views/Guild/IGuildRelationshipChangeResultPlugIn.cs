// <copyright file="IGuildRelationshipChangeResultPlugIn.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.GameLogic.Views.Guild;

/// <summary>
/// The type of guild relationship.
/// </summary>
public enum GuildRelationshipType
{
    /// <summary>
    /// An undefined relationship type.
    /// </summary>
    Undefined,

    /// <summary>
    /// An alliance relationship.
    /// </summary>
    Alliance,

    /// <summary>
    /// A hostility relationship.
    /// </summary>
    Hostility,
}

/// <summary>
/// The type of guild relationship request.
/// </summary>
public enum GuildRelationshipRequestType
{
    /// <summary>
    /// An undefined request type.
    /// </summary>
    Undefined,

    /// <summary>
    /// A join request.
    /// </summary>
    Join,

    /// <summary>
    /// A leave request.
    /// </summary>
    Leave,

    /// <summary>
    /// A disband request? Only possible for <see cref="GuildRelationshipType.Alliance"/>.
    /// </summary>
    Disband,
}

/// <summary>
/// The result of a guild relationship change request.
/// </summary>
public enum GuildRelationshipChangeResultType
{
    /// <summary>
    /// The request failed.
    /// </summary>
    Failed,

    /// <summary>
    /// The request succeeded.
    /// </summary>
    Success,

    /// <summary>
    /// The guild was not found.
    /// </summary>
    GuildNotFound,

    /// <summary>
    /// Failed because the alliance function is restricted due to the Castle Siege.
    /// </summary>
    // GUILD_ANS_UNIONFAIL_BY_CASTLE
    FailedDuringCastleSiege,

    /// <summary>
    /// No authorization for the request.
    /// </summary>
    // GUILD_ANS_NOTEXIST_PERMISSION
    NoAuthorization,

    /// <summary>
    /// The guilds are already in an alliance.
    /// </summary>
    // GUILD_ANS_EXIST_RELATIONSHIP_UNION
    AlreadyInAlliance,

    /// <summary>
    /// The guilds are already in a hostility relationship.
    /// </summary>
    // GUILD_ANS_EXIST_RELATIONSHIP_RIVAL
    AlreadyInHostility,

    /// <summary>
    /// The guild alliance already exists.
    /// </summary>
    // GUILD_ANS_EXIST_UNION
    GuildAllianceExists,

    /// <summary>
    /// The hostile guild already exists.
    /// </summary>
    // GUILD_ANS_EXIST_RIVAL
    HostileGuildExists,

    /// <summary>
    /// The guild alliance does not exist.
    /// </summary>
    // GUILD_ANS_NOTEXIST_UNION
    GuildAllianceDoesNotExist,

    /// <summary>
    /// The hostile guild does not exist.
    /// </summary>
    // GUILD_ANS_NOTEXIST_RIVAL
    HostileGuildDoesNotExist,

    /// <summary>
    /// The requesting guild master is not the master of the alliance.
    /// </summary>
    // GUILD_ANS_NOT_UNION_MASTER
    NotMasterOfGuildAlliance,

    /// <summary>
    /// The requesting guild is not a rival.
    /// </summary>
    // GUILD_ANS_NOT_GUILD_RIVAL
    NotGuildRival,

    /// <summary>
    /// Incomplete requirements to create an alliance.
    /// </summary>
    // GUILD_ANS_CANNOT_BE_UNION_MASTER_GUILD
    IncompleteRequirementsToCreateAlliance,

    /// <summary>
    /// The maximum number of guilds in an alliance was reached.
    /// </summary>
    // GUILD_ANS_EXCEED_MAX_UNION_MEMBER
    MaximumNumberOfGuildsInAllianceReached,

    /// <summary>
    /// The request was cancelled.
    /// </summary>
    // GUILD_ANS_CANCEL_REQUEST
    RequestCancelled,

    /// <summary>
    /// The alliance master is not in Gens.
    /// </summary>
    // GUILD_ANS_UNION_MASTER_NOT_GENS
    AllianceMasterNotInGens,

    /// <summary>
    /// The guild master is not in Gens.
    /// </summary>
    // GUILD_ANS_GUILD_MASTER_NOT_GENS
    GuildMasterNotInGens,

    /// <summary>
    /// The guilds are in different Gens factions.
    /// </summary>
    // GUILD_ANS_UNION_MASTER_DISAGREE_GENS
    DifferentGens = 0xA3,
}

/// <summary>
/// Interface of a view whose implementation informs about the result of a guild relationship change request.
/// </summary>
public interface IGuildRelationshipChangeResultPlugIn : IViewPlugIn
{
    /// <summary>
    /// Shows the result of the guild relationship change request.
    /// </summary>
    /// <param name="relationshipType">The type of relationship that was being changed.</param>
    /// <param name="requestType">The type of request (join/leave).</param>
    /// <param name="resultType">The result of the relationship change request.</param>
    /// <param name="guildMasterId">The id of the guild master which was asked for the relationship change.</param>
    ValueTask ShowResultAsync(GuildRelationshipType relationshipType, GuildRelationshipRequestType requestType, GuildRelationshipChangeResultType resultType, ushort? guildMasterId);

    /// <summary>
    /// Shows the result of a requested remove request, when a guild is removed from an alliance by the alliance master.
    /// </summary>
    /// <param name="result">The success flag.</param>
    /// <param name="relationshipType">Type of the relationship, usually <see cref="GuildRelationshipType.Alliance"/>.</param>
    /// <param name="requestType">Type of the request, usually <see cref="GuildRelationshipRequestType.Leave"/>.</param>
    ValueTask ShowRemoveResultAsync(bool result, GuildRelationshipType relationshipType = GuildRelationshipType.Alliance, GuildRelationshipRequestType requestType = GuildRelationshipRequestType.Leave);
}
