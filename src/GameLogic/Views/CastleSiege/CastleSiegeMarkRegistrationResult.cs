// <copyright file="CastleSiegeMarkRegistrationResult.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.GameLogic.Views.CastleSiege;

/// <summary>
/// The result of a Sign of Lord registration request.
/// </summary>
public enum CastleSiegeMarkRegistrationResult : byte
{
    /// <summary>
    /// The registration failed for an unspecified reason.
    /// </summary>
    Failed = 0,

    /// <summary>
    /// The Sign of Lord was registered successfully.
    /// </summary>
    Success = 1,

    /// <summary>
    /// The guild does not participate in the Castle Siege.
    /// </summary>
    GuildNotRegistered = 2,

    /// <summary>
    /// The selected inventory item is not a Sign of Lord.
    /// </summary>
    IncorrectItem = 3,
}
