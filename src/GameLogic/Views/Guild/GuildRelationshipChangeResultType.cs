// <copyright file="GuildRelationshipChangeResultType.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.GameLogic.Views.Guild;

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
    FailedDuringCastleSiege,

    /// <summary>
    /// No authorization for the request.
    /// </summary>
    NoAuthorization,

    /// <summary>
    /// The guilds are already in an alliance.
    /// </summary>
    AlreadyInAlliance,

    /// <summary>
    /// The guilds are already in a hostility relationship.
    /// </summary>
    AlreadyInHostility,

    /// <summary>
    /// The guild alliance already exists.
    /// </summary>
    GuildAllianceExists,

    /// <summary>
    /// The hostile guild already exists.
    /// </summary>
    HostileGuildExists,

    /// <summary>
    /// The guild alliance does not exist.
    /// </summary>
    GuildAllianceDoesNotExist,

    /// <summary>
    /// The hostile guild does not exist.
    /// </summary>
    HostileGuildDoesNotExist,

    /// <summary>
    /// The requesting guild master is not the master of the alliance.
    /// </summary>
    NotMasterOfGuildAlliance,

    /// <summary>
    /// The requesting guild is not a rival.
    /// </summary>
    NotGuildRival,

    /// <summary>
    /// Incomplete requirements to create an alliance.
    /// </summary>
    IncompleteRequirementsToCreateAlliance,

    /// <summary>
    /// The maximum number of guilds in an alliance was reached.
    /// </summary>
    MaximumNumberOfGuildsInAllianceReached,

    /// <summary>
    /// The request was cancelled.
    /// </summary>
    RequestCancelled,

    /// <summary>
    /// The alliance master is not in Gens.
    /// </summary>
    AllianceMasterNotInGens,

    /// <summary>
    /// The guild master is not in Gens.
    /// </summary>
    GuildMasterNotInGens,

    /// <summary>
    /// The guilds are in different Gens factions.
    /// </summary>
    DifferentGens = 0xA3,
}
