// <copyright file="CastleSiegeNpcOperationResult.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.GameLogic.Views.CastleSiege;

/// <summary>
/// The result of a Castle Siege NPC management request.
/// </summary>
public enum CastleSiegeNpcOperationResult : byte
{
    /// <summary>
    /// The request failed validation.
    /// </summary>
    Failed = 0,

    /// <summary>
    /// The request succeeded.
    /// </summary>
    Success = 1,

    /// <summary>
    /// The player is not authorized to manage the defense structure.
    /// </summary>
    NotAuthorized = 2,

    /// <summary>
    /// The player does not have enough Zen.
    /// </summary>
    InsufficientMoney = 3,

    /// <summary>
    /// An operation-specific requirement was not met.
    /// For purchase requests, the structure already exists; for upgrades, required items are missing.
    /// </summary>
    RequirementNotMet = 4,

    /// <summary>
    /// The requested upgrade type is invalid for the structure.
    /// </summary>
    InvalidUpgradeType = 5,

    /// <summary>
    /// The requested upgrade value is invalid.
    /// </summary>
    InvalidUpgradeValue = 6,

    /// <summary>
    /// The requested defense structure does not exist.
    /// </summary>
    NpcNotFound = 7,
}
