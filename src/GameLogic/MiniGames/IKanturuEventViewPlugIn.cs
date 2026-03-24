// <copyright file="IKanturuEventViewPlugIn.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.GameLogic.MiniGames;

using MUnique.OpenMU.GameLogic.Views;

/// <summary>
/// View plugin interface for Kanturu-specific server-to-client packets (0xD1 group).
/// Each method maps to one packet subcode.
/// </summary>
public interface IKanturuEventViewPlugIn : IViewPlugIn
{
    /// <summary>
    /// Sends packet 0xD1/0x00 — Kanturu state info (response to NPC talk / info request).
    /// This opens the <c>INTERFACE_KANTURU2ND_ENTERNPC</c> dialog on the client, showing
    /// the current event state, how many players are inside, and whether entry is possible.
    /// </summary>
    /// <param name="state">Main event state (see <see cref="KanturuStateCode"/>).</param>
    /// <param name="detailState">Detail state for the current main state.</param>
    /// <param name="enter">1 = entrance is open (Enter button enabled); 0 = entrance closed.</param>
    /// <param name="userCount">Number of players currently inside the event map.</param>
    /// <param name="remainTimeSec">
    /// Seconds remaining. Semantics depend on state:
    /// Standby → seconds until the event opens (client shows minutes).
    /// Tower   → seconds the tower has been open (client shows hours).
    /// Otherwise 0.
    /// </param>
    ValueTask SendStateInfoAsync(byte state, byte detailState, byte enter, byte userCount, int remainTimeSec);

    /// <summary>
    /// Sends packet 0xD1/0x01 — Kanturu enter result.
    /// Sent after the player's entry attempt is processed.
    /// On the client side this stops the NPC animation and closes the dialog.
    /// Result codes: 0 = failed (generic), use specific POPUP_* values for other errors.
    /// On success the player is teleported before this is sent, so the packet is only
    /// needed for failure cases.
    /// </summary>
    ValueTask SendEnterResultAsync(byte result);

    /// <summary>
    /// Sends packet 0xD1/0x03 — Kanturu state change.
    /// On the client side this:
    ///   - Shows/hides the in-map HUD (<c>INTERFACE_KANTURU_INFO</c>).
    ///   - Switches background music (Maya, Nightmare, Tower themes).
    ///   - When <paramref name="state"/> == <see cref="KanturuStateCode.Tower"/> and
    ///     <paramref name="detailState"/> is 1 or 2, reloads the success terrain file
    ///     (<c>EncTerrain&lt;n&gt;01.att</c>), which visually removes the Elphis barrier.
    /// </summary>
    /// <param name="state">Main state (see <see cref="KanturuStateCode"/>).</param>
    /// <param name="detailState">Detail state for the current main state.</param>
    ValueTask SendStateChangeAsync(byte state, byte detailState);

    /// <summary>
    /// Sends packet 0xD1/0x04 — Kanturu battle result.
    /// <list type="bullet">
    ///   <item>0 = failure — shows <c>Failure_kantru.tga</c> overlay.</item>
    ///   <item>1 = success — shows <c>Success_kantru.tga</c> overlay
    ///     (only when client state is already <see cref="KanturuStateCode.NightmareBattle"/>).</item>
    /// </list>
    /// </summary>
    ValueTask SendBattleResultAsync(byte result);

    /// <summary>
    /// Sends packet 0xD1/0x05 — countdown timer for the Kanturu HUD (in milliseconds).
    /// The HUD divides by 1000 to get seconds and counts down visually.
    /// </summary>
    ValueTask SendTimeLimitAsync(int timeLimitMs);

    /// <summary>
    /// Sends packet 0xD1/0x07 — remaining monster count and current user count.
    /// Updates the numbers displayed in the Kanturu HUD.
    /// </summary>
    ValueTask SendMonsterUserCountAsync(byte monsterCount, byte userCount);

    /// <summary>
    /// Sends packet 0xD1/0x06 — Maya wide-area attack visual trigger.
    /// The client calls <c>MayaSceneMayaAction(attackType)</c> on receipt, which plays
    /// one of two client-side visual sequences on the Maya body object:
    /// <list type="bullet">
    ///   <item>0 = stone-storm effect (<c>MODEL_STORM3</c> + falling debris around Hero)</item>
    ///   <item>1 = stone-rain effect (<c>MODEL_MAYASTONE</c> projectiles falling on Hero)</item>
    /// </list>
    /// This is a purely cosmetic broadcast — damage is handled by the server-side
    /// <see cref="MonsterDefinition.AttackSkill"/> on Maya Hands (#362/#363).
    /// </summary>
    /// <param name="attackType">0 = storm visual; 1 = stone-rain visual.</param>
    ValueTask SendMayaWideAreaAttackAsync(byte attackType);
}

/// <summary>
/// Kanturu main state codes matching the client's <c>KANTURU_STATE_TYPE</c> enum.
/// </summary>
public static class KanturuStateCode
{
    /// <summary>No active state.</summary>
    public const byte None = 0;

    /// <summary>Waiting for players to enter.</summary>
    public const byte Standby = 1;

    /// <summary>Maya battle phase (Phases 1–3 + boss waves).</summary>
    public const byte MayaBattle = 2;

    /// <summary>Nightmare battle phase.</summary>
    public const byte NightmareBattle = 3;

    /// <summary>Tower of Refinement open (post-victory).</summary>
    public const byte Tower = 4;

    /// <summary>Event ended.</summary>
    public const byte End = 5;
}

/// <summary>
/// Detail state codes for the Nightmare battle phase,
/// matching <c>KANTURU_NIGHTMARE_DIRECTION_TYPE</c>.
/// </summary>
public static class KanturuNightmareDetail
{
    /// <summary>No direction set.</summary>
    public const byte None = 0;

    /// <summary>Idle — Nightmare present but not yet in battle.</summary>
    public const byte Idle = 1;

    /// <summary>Nightmare intro animation.</summary>
    public const byte NightmareIntro = 2;

    /// <summary>Active battle — shows the HUD on the client.</summary>
    public const byte Battle = 3;

    /// <summary>Battle ended.</summary>
    public const byte End = 4;
}

/// <summary>
/// Detail state codes for the Tower of Refinement phase,
/// matching <c>KANTURU_TOWER_STATE_TYPE</c>.
/// </summary>
public static class KanturuTowerDetail
{
    /// <summary>No tower state.</summary>
    public const byte None = 0;

    /// <summary>
    /// Tower is open after Nightmare's defeat.
    /// Sending this triggers the client to reload <c>EncTerrain&lt;n&gt;01.att</c>
    /// (the success terrain), which visually removes the Elphis barrier.
    /// </summary>
    public const byte Revitalization = 1;

    /// <summary>Tower closing soon — client warns players.</summary>
    public const byte Notify = 2;

    /// <summary>Tower closed.</summary>
    public const byte Close = 3;
}

/// <summary>
/// Detail state codes for the Maya battle phase,
/// matching <c>KANTURU_MAYA_DIRECTION_TYPE</c>.
/// </summary>
public static class KanturuMayaDetail
{
    /// <summary>No direction.</summary>
    public const byte None = 0;

    /// <summary>Phase 1 monster wave active — shows HUD.</summary>
    public const byte Monster1 = 3;

    /// <summary>Phase 1 boss (Maya Left Hand) — shows HUD.</summary>
    public const byte Maya1 = 4;

    /// <summary>Phase 2 monster wave active — shows HUD.</summary>
    public const byte Monster2 = 8;

    /// <summary>Phase 2 boss (Maya Right Hand) — shows HUD.</summary>
    public const byte Maya2 = 9;

    /// <summary>Phase 3 monster wave active — shows HUD.</summary>
    public const byte Monster3 = 13;

    /// <summary>Phase 3 bosses (both hands) — shows HUD.</summary>
    public const byte Maya3 = 14;

    /// <summary>
    /// Maya phase 3 end cycle — triggers the full Maya explosion + player fall cinematic
    /// (<c>KANTURU_MAYA_DIRECTION_ENDCYCLE_MAYA3 = 16</c> on the client).
    /// Sending this via 0xD1/0x03 activates <c>Move2ndDirection()</c>:
    /// camera pans to Maya room → <c>m_bMayaDie = true</c> (explosion) → <c>m_bDownHero = true</c> (fall).
    /// </summary>
    public const byte EndCycleMaya3 = 16;
}
