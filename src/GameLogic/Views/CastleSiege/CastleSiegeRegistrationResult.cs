// <copyright file="CastleSiegeRegistrationResult.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.GameLogic.Views.CastleSiege;

/// <summary>
/// The result of a Castle Siege guild registration request.
/// </summary>
public enum CastleSiegeRegistrationResult : byte
{
    /// <summary>
    /// The request failed for an unspecified reason.
    /// </summary>
    Failed = 0,

    /// <summary>
    /// The guild was registered successfully.
    /// </summary>
    Success = 1,

    /// <summary>
    /// The guild is already registered.
    /// </summary>
    AlreadyRegistered = 2,

    /// <summary>
    /// The guild is the current castle defender.
    /// </summary>
    IsDefender = 3,

    /// <summary>
    /// The guild or the requesting guild member is invalid.
    /// </summary>
    InvalidGuild = 4,

    /// <summary>
    /// The player's combined level is insufficient.
    /// </summary>
    LevelInsufficient = 5,

    /// <summary>
    /// The player is not a guild member.
    /// </summary>
    NoGuild = 6,

    /// <summary>
    /// Guild registration is currently closed.
    /// </summary>
    NotRegistrationPeriod = 7,

    /// <summary>
    /// The guild has too few members.
    /// </summary>
    NotEnoughMembers = 8,
}
