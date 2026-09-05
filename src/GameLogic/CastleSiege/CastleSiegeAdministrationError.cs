// <copyright file="CastleSiegeAdministrationError.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.GameLogic.CastleSiege;

/// <summary>
/// Identifies a failed Castle Siege administration operation without coupling game logic to UI text.
/// </summary>
public enum CastleSiegeAdministrationError
{
    /// <summary>
    /// The operation succeeded.
    /// </summary>
    None,

    /// <summary>
    /// The requested state is invalid.
    /// </summary>
    InvalidState,

    /// <summary>
    /// Castle Siege has not initialized.
    /// </summary>
    NotInitialized,

    /// <summary>
    /// A guild name is required.
    /// </summary>
    GuildNameRequired,

    /// <summary>
    /// The operation requires a game-server context.
    /// </summary>
    GameServerContextRequired,

    /// <summary>
    /// The guild was not found.
    /// </summary>
    GuildNotFound,

    /// <summary>
    /// The owner cannot be changed during the battle.
    /// </summary>
    OwnerChangeDuringBattle,

    /// <summary>
    /// The cycle cannot be reset during an active siege phase.
    /// </summary>
    ResetDuringActiveSiege,

    /// <summary>
    /// One or more tax values are invalid.
    /// </summary>
    TaxOutOfRange,

    /// <summary>
    /// Taxes cannot be changed during the battle.
    /// </summary>
    TaxChangeDuringBattle,

    /// <summary>
    /// Tribute cannot be cleared during the battle.
    /// </summary>
    TributeClearDuringBattle,

    /// <summary>
    /// Registrations cannot be changed in the current state.
    /// </summary>
    RegistrationChangeOutsideRegistration,

    /// <summary>
    /// The registration no longer exists.
    /// </summary>
    RegistrationMissing,

    /// <summary>
    /// The selected game server is unavailable.
    /// </summary>
    GameServerUnavailable,

    /// <summary>
    /// Castle Siege administration requires an all-in-one deployment.
    /// </summary>
    AllInOneDeploymentRequired,

    /// <summary>
    /// The Castle Siege plug-in is inactive.
    /// </summary>
    PlugInInactive,
}
