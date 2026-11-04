// <copyright file="GensJoinResult.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.GameLogic.Views.Gens;

/// <summary>
/// The result of a request to join a gens.
/// </summary>
public enum GensJoinResult
{
    /// <summary>
    /// The player joined the gens.
    /// </summary>
    Success,

    /// <summary>
    /// The player is already member of a gens.
    /// </summary>
    AlreadyJoined,

    /// <summary>
    /// The player left a gens recently and has to wait before joining again.
    /// </summary>
    LeftRecently,

    /// <summary>
    /// The level of the character is too low.
    /// </summary>
    LevelTooLow,

    /// <summary>
    /// The player is member of a guild.
    /// </summary>
    GuildMember,

    /// <summary>
    /// The player is the master of a guild.
    /// </summary>
    GuildMaster,

    /// <summary>
    /// The player is in a party.
    /// </summary>
    InParty,
}
