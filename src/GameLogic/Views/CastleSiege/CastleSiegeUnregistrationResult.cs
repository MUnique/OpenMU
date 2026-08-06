// <copyright file="CastleSiegeUnregistrationResult.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.GameLogic.Views.CastleSiege;

/// <summary>
/// The result of a Castle Siege guild-unregistration request.
/// </summary>
public enum CastleSiegeUnregistrationResult : byte
{
    /// <summary>
    /// The request failed.
    /// </summary>
    Failed,

    /// <summary>
    /// The guild was unregistered.
    /// </summary>
    Success,

    /// <summary>
    /// The guild was not registered.
    /// </summary>
    NotRegistered,

    /// <summary>
    /// Guilds cannot unregister during the current state.
    /// </summary>
    WrongState,
}
