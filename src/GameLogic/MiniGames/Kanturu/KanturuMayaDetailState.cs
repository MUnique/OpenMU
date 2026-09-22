// <copyright file="KanturuMayaDetailState.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.GameLogic.MiniGames.Kanturu;

/// <summary>
/// Detail state codes for the Maya battle phase,
/// matching <c>KANTURU_MAYA_DIRECTION_TYPE</c>.
/// </summary>
public enum KanturuMayaDetailState : byte
{
    /// <summary>No direction.</summary>
    None = 0,

    /// <summary>Notify cinematic - camera pan + Maya rise animation (KANTURU_MAYA_DIRECTION_NOTIFY).</summary>
    Notify = 2,

    /// <summary>Phase 1 monster wave active - shows HUD.</summary>
    Monster1 = 3,

    /// <summary>Phase 1 boss (Maya Left Hand) - shows HUD.</summary>
    Maya1 = 4,

    /// <summary>Phase 2 monster wave active - shows HUD.</summary>
    Monster2 = 8,

    /// <summary>Phase 2 boss (Maya Right Hand) - shows HUD.</summary>
    Maya2 = 9,

    /// <summary>Phase 3 monster wave active - shows HUD.</summary>
    Monster3 = 13,

    /// <summary>Phase 3 bosses (both hands) - shows HUD.</summary>
    Maya3 = 14,

    /// <summary>
    /// Maya phase 3 end cycle - triggers the full Maya explosion + player fall cinematic
    /// (<c>KANTURU_MAYA_DIRECTION_ENDCYCLE_MAYA3 = 16</c> on the client).
    /// Sending this via 0xD1/0x03 activates <c>Move2ndDirection()</c>:
    /// camera pans to Maya room -&gt; <c>m_bMayaDie = true</c> (explosion) -&gt; <c>m_bDownHero = true</c> (fall).
    /// </summary>
    EndCycleMaya3 = 16,
}
