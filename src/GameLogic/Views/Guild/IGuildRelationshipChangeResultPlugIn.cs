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

public enum GuildRelationshipChangeResultType
{
    Failed,

    Success,

    GuildNotFound,

    // GUILD_ANS_UNIONFAIL_BY_CASTLE: Alliance function will be restricted due to the Castle Siege.
    FailedDuringCastleSiege,

    // GUILD_ANS_NOTEXIST_PERMISSION
    NoAuthorization,

    // GUILD_ANS_EXIST_RELATIONSHIP_UNION
    AlreadyInAlliance,

    // GUILD_ANS_EXIST_RELATIONSHIP_RIVAL
    AlreadyInHostility,

    // GUILD_ANS_EXIST_UNION
    GuildAllianceExists,

    // GUILD_ANS_EXIST_RIVAL
    HostileGuildExists,

    // GUILD_ANS_NOTEXIST_UNION
    GuildAllianceDoesNotExist,

    // GUILD_ANS_NOTEXIST_RIVAL
    HostileGuildDoesNotExist,

    // GUILD_ANS_NOT_UNION_MASTER
    NotMasterOfGuildAlliance,

    // GUILD_ANS_NOT_GUILD_RIVAL
    NotGuildRival,

    // GUILD_ANS_CANNOT_BE_UNION_MASTER_GUILD
    IncompleteRequirementsToCreateAlliance,

    // GUILD_ANS_EXCEED_MAX_UNION_MEMBER
    MaximumNumberOfGuildsInAllianceReached,

    // GUILD_ANS_CANCEL_REQUEST
    RequestCancelled,

    // GUILD_ANS_UNION_MASTER_NOT_GENS
    AllianceMasterNotInGens,

    // GUILD_ANS_GUILD_MASTER_NOT_GENS
    GuildMasterNotInGens,

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
    ValueTask ShowResultAsync(GuildRelationshipType relationshipType, GuildRelationshipRequestType requestType, GuildRelationshipChangeResultType resultType, ushort guildMasterId);
}
