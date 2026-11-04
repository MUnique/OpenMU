// <copyright file="KanturuTowerDetailState.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.GameLogic.MiniGames.Kanturu;

/// <summary>
/// Detail state codes for the Tower of Refinement phase,
/// matching <c>KANTURU_TOWER_STATE_TYPE</c>.
/// </summary>
public enum KanturuTowerDetailState : byte
{
    /// <summary>No tower state.</summary>
    None = 0,

    /// <summary>
    /// Tower is open after Nightmare's defeat.
    /// Sending this triggers the client to reload <c>EncTerrain&lt;n&gt;01.att</c>
    /// (the success terrain), which visually removes the Elphis barrier.
    /// </summary>
    Revitalization = 1,

    /// <summary>Tower closing soon - client warns players.</summary>
    Notify = 2,

    /// <summary>Tower closed.</summary>
    Close = 3,
}
