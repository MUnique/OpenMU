// <copyright file="KanturuState.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.GameLogic.MiniGames.Kanturu;

/// <summary>
/// Kanturu main state codes matching the client's <c>KANTURU_STATE_TYPE</c> enum.
/// </summary>
public enum KanturuState : byte
{
    /// <summary>No active state.</summary>
    None = 0,

    /// <summary>Waiting for players to enter.</summary>
    Standby = 1,

    /// <summary>Maya battle phase (Phases 1-3 + boss waves).</summary>
    MayaBattle = 2,

    /// <summary>Nightmare battle phase.</summary>
    NightmareBattle = 3,

    /// <summary>Tower of Refinement open (post-victory).</summary>
    Tower = 4,

    /// <summary>Event ended.</summary>
    End = 5,
}
