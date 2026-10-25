// <copyright file="CrywolfContractResult.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.GameLogic.Crywolf;

/// <summary>
/// The result of a request to contract an altar of the crywolf event.
/// </summary>
public enum CrywolfContractResult
{
    /// <summary>
    /// The elf is trying to contract the altar. The contract gets valid when the elf stays at the altar.
    /// </summary>
    Success,

    /// <summary>
    /// The altar can't be contracted now, e.g. because it's contracted already or the event isn't in the right state.
    /// </summary>
    NotAvailable,

    /// <summary>
    /// The character can't contract an altar, because of its class or level.
    /// </summary>
    NotQualified,

    /// <summary>
    /// The altar can't be contracted yet, because its last contract ended a short time ago.
    /// </summary>
    Cooldown,

    /// <summary>
    /// The character doesn't stand at the altar.
    /// </summary>
    WrongPosition,

    /// <summary>
    /// The character rides a mount, which isn't allowed for a contract.
    /// </summary>
    Mounted,

    /// <summary>
    /// The character is already contracting the altar, e.g. because the client sent the request twice.
    /// </summary>
    AlreadyContracting,
}
