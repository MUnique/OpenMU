// <copyright file="KanturuStates.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.GameLogic.MiniGames;

/// <summary>
/// Kanturu main state codes matching the client's <c>KANTURU_STATE_TYPE</c> enum.
/// </summary>
public enum KanturuState : byte
{
    /// <summary>No active state.</summary>
    None = 0,

    /// <summary>Waiting for players to enter.</summary>
    Standby = 1,

    /// <summary>Maya battle phase (Phases 1–3 + boss waves).</summary>
    MayaBattle = 2,

    /// <summary>Nightmare battle phase.</summary>
    NightmareBattle = 3,

    /// <summary>Tower of Refinement open (post-victory).</summary>
    Tower = 4,

    /// <summary>Event ended.</summary>
    End = 5,
}

/// <summary>
/// Detail state codes for the Nightmare battle phase,
/// matching <c>KANTURU_NIGHTMARE_DIRECTION_TYPE</c>.
/// </summary>
public enum KanturuNightmareDetailState : byte
{
    /// <summary>No direction set.</summary>
    None = 0,

    /// <summary>Idle — Nightmare present but not yet in battle.</summary>
    Idle = 1,

    /// <summary>Nightmare intro animation.</summary>
    NightmareIntro = 2,

    /// <summary>Active battle — shows the HUD on the client.</summary>
    Battle = 3,

    /// <summary>Battle ended.</summary>
    End = 4,
}

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

    /// <summary>Tower closing soon — client warns players.</summary>
    Notify = 2,

    /// <summary>Tower closed.</summary>
    Close = 3,
}

/// <summary>
/// Detail state codes for the Maya battle phase,
/// matching <c>KANTURU_MAYA_DIRECTION_TYPE</c>.
/// </summary>
public enum KanturuMayaDetailState : byte
{
    /// <summary>No direction.</summary>
    None = 0,

    /// <summary>Notify cinematic — camera pan + Maya rise animation (KANTURU_MAYA_DIRECTION_NOTIFY).</summary>
    Notify = 2,

    /// <summary>Phase 1 monster wave active — shows HUD.</summary>
    Monster1 = 3,

    /// <summary>Phase 1 boss (Maya Left Hand) — shows HUD.</summary>
    Maya1 = 4,

    /// <summary>Phase 2 monster wave active — shows HUD.</summary>
    Monster2 = 8,

    /// <summary>Phase 2 boss (Maya Right Hand) — shows HUD.</summary>
    Maya2 = 9,

    /// <summary>Phase 3 monster wave active — shows HUD.</summary>
    Monster3 = 13,

    /// <summary>Phase 3 bosses (both hands) — shows HUD.</summary>
    Maya3 = 14,

    /// <summary>
    /// Maya phase 3 end cycle — triggers the full Maya explosion + player fall cinematic
    /// (<c>KANTURU_MAYA_DIRECTION_ENDCYCLE_MAYA3 = 16</c> on the client).
    /// Sending this via 0xD1/0x03 activates <c>Move2ndDirection()</c>:
    /// camera pans to Maya room → <c>m_bMayaDie = true</c> (explosion) → <c>m_bDownHero = true</c> (fall).
    /// </summary>
    EndCycleMaya3 = 16,
}
