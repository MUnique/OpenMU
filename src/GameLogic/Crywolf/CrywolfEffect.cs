// <copyright file="CrywolfEffect.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.GameLogic.Crywolf;

/// <summary>
/// The effects of the crywolf event, which the client shows at objects, matching the values expected by the client.
/// </summary>
public enum CrywolfEffect : byte
{
    /// <summary>
    /// The altar can be contracted.
    /// </summary>
    AltarEnabled = 21,

    /// <summary>
    /// The altar can't be contracted anymore.
    /// </summary>
    AltarDisabled = 22,

    /// <summary>
    /// The altar is contracted.
    /// </summary>
    AltarContracted = 23,

    /// <summary>
    /// An elf is trying to contract the altar.
    /// </summary>
    AltarAttempt = 24,

    /// <summary>
    /// The NPC is hidden, because the fortress isn't in peace.
    /// </summary>
    NpcHidden = 27,
}
