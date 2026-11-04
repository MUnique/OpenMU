// <copyright file="CastleSiegeRegistrationStateResult.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.GameLogic.Views.CastleSiege;

/// <summary>
/// The result of a Castle Siege registration-state request.
/// </summary>
public enum CastleSiegeRegistrationStateResult : byte
{
    /// <summary>
    /// The guild is not registered.
    /// </summary>
    NotRegistered,

    /// <summary>
    /// The guild is registered.
    /// </summary>
    Registered,

    /// <summary>
    /// The registration state is unavailable.
    /// </summary>
    Unavailable,
}
