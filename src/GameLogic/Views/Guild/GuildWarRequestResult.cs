// <copyright file="GuildWarRequestResult.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.GameLogic.Views.Guild;

/// <summary>
/// The result of a guild war request.
/// </summary>
public enum GuildWarRequestResult
{
    /// <summary>
    /// The guild was not found.
    /// </summary>
    GuildNotFound,

    /// <summary>
    /// The request has been sent to the guild master of the specified guild.
    /// </summary>
    RequestSentToGuildMaster,

    /// <summary>
    /// The guild master is offline and can't accept the request.
    /// </summary>
    GuildMasterOffline,

    /// <summary>
    /// The player is not in a guild.
    /// </summary>
    NotInGuild,

    /// <summary>
    /// The request failed.
    /// </summary>
    Failed,

    /// <summary>
    /// The player is not the guild master.
    /// </summary>
    NotTheGuildMaster,

    /// <summary>
    /// The guild is already in war and can't accept another one.
    /// </summary>
    AlreadyInWar,
}