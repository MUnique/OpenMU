// <copyright file="KanturuNightmareDetailState.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.GameLogic.MiniGames.Kanturu;

/// <summary>
/// Detail state codes for the Nightmare battle phase,
/// matching <c>KANTURU_NIGHTMARE_DIRECTION_TYPE</c>.
/// </summary>
public enum KanturuNightmareDetailState : byte
{
    /// <summary>No direction set.</summary>
    None = 0,

    /// <summary>Idle - Nightmare present but not yet in battle.</summary>
    Idle = 1,

    /// <summary>Nightmare intro animation.</summary>
    NightmareIntro = 2,

    /// <summary>Active battle - shows the HUD on the client.</summary>
    Battle = 3,

    /// <summary>Battle ended.</summary>
    End = 4,
}
