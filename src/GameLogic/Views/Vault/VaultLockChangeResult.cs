// <copyright file="VaultLockChangeResult.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.GameLogic.Views.Vault;

/// <summary>
/// Result of a request to change the lock state of the vault.
/// </summary>
public enum VaultLockChangeResult
{
    /// <summary>
    /// The vault is protected and locked. The user-requested unlock failed by a wrong pin.
    /// </summary>
    UnlockFailedByWrongPin,

    /// <summary>
    /// The vault is protected and locked and the player-requested pin setting failed because of the lock.
    /// </summary>
    SetPinFailedBecauseLock,

    /// <summary>
    /// The vault is protected, but was unlocked by the player.
    /// </summary>
    Unlocked,

    /// <summary>
    /// The vault is protected and the player-requested pin removal failed by using the wrong password.
    /// </summary>
    RemovePinFailedByWrongPassword,
}