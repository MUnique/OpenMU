// <copyright file="IKanturuEventViewPlugIn.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.GameLogic.MiniGames.Kanturu;

using MUnique.OpenMU.GameLogic.Views;

/// <summary>
/// View plugin interface for Kanturu-specific server-to-client packets (0xD1 group).
/// Each method maps to one packet subcode.
/// </summary>
public interface IKanturuEventViewPlugIn : IViewPlugIn
{
    /// <summary>
    /// Shows packet 0xD1/0x00 — Kanturu state info (response to NPC talk / info request).
    /// This opens the <c>INTERFACE_KANTURU2ND_ENTERNPC</c> dialog on the client, showing
    /// the current event state, how many players are inside, and whether entry is possible.
    /// </summary>
    /// <param name="state">Main event state.</param>
    /// <param name="detailState">Detail state for the current main state.</param>
    /// <param name="canEnter">True if entrance is open (Enter button enabled).</param>
    /// <param name="userCount">Number of players currently inside the event map.</param>
    /// <param name="remainTime">
    /// Remaining time. Semantics depend on state:
    /// Standby → time until the event opens (client shows minutes).
    /// Tower   → time the tower has been open (client shows hours).
    /// Otherwise zero.
    /// </param>
    ValueTask ShowStateInfoAsync(KanturuState state, byte detailState, bool canEnter, int userCount, TimeSpan remainTime);

    /// <summary>
    /// Shows packet 0xD1/0x01 — Kanturu enter result.
    /// Sent after the player's entry attempt is processed.
    /// On the client side this stops the NPC animation and closes the dialog.
    /// On success the player is teleported before this is sent.
    /// </summary>
    /// <param name="success">True if the player was successfully entered; false otherwise.</param>
    ValueTask ShowEnterResultAsync(bool success);

    /// <summary>
    /// Shows packet 0xD1/0x03 — Kanturu state change.
    /// On the client side this:
    ///   - Shows/hides the in-map HUD (<c>INTERFACE_KANTURU_INFO</c>).
    ///   - Switches background music (Maya, Nightmare, Tower themes).
    ///   - When <paramref name="state"/> == <see cref="KanturuState.Tower"/> and
    ///     <paramref name="detailState"/> is <see cref="KanturuTowerDetailState.Revitalization"/> or <see cref="KanturuTowerDetailState.Notify"/>,
    ///     reloads the success terrain file (<c>EncTerrain&lt;n&gt;01.att</c>), which visually removes the Elphis barrier.
    /// </summary>
    /// <param name="state">Main state.</param>
    /// <param name="detailState">Detail state for the current main state.</param>
    ValueTask ShowStateChangeAsync(KanturuState state, byte detailState);

    /// <summary>
    /// Shows packet 0xD1/0x04 — Kanturu battle result.
    /// <list type="bullet">
    ///   <item>False = failure — shows <c>Failure_kantru.tga</c> overlay.</item>
    ///   <item>True = success — shows <c>Success_kantru.tga</c> overlay
    ///     (only when client state is already <see cref="KanturuState.NightmareBattle"/>).</item>
    /// </list>
    /// </summary>
    /// <param name="victory">True if the event was won; false if defeated.</param>
    ValueTask ShowBattleResultAsync(bool victory);

    /// <summary>
    /// Shows packet 0xD1/0x05 — countdown timer for the Kanturu HUD.
    /// The HUD divides by 1000 to get seconds and counts down visually.
    /// </summary>
    /// <param name="timeLimit">The time limit to display.</param>
    ValueTask ShowTimeLimitAsync(TimeSpan timeLimit);

    /// <summary>
    /// Shows packet 0xD1/0x07 — remaining monster count and current user count.
    /// Updates the numbers displayed in the Kanturu HUD.
    /// </summary>
    /// <param name="monsterCount">Number of monsters still alive in the current wave.</param>
    /// <param name="userCount">Number of players currently inside the event map.</param>
    ValueTask ShowMonsterUserCountAsync(int monsterCount, int userCount);

    /// <summary>
    /// Shows packet 0xD1/0x06 — Maya wide-area attack visual trigger.
    /// The client calls <c>MayaSceneMayaAction(attackType)</c> on receipt, which plays
    /// one of two client-side visual sequences on the Maya body object:
    /// <list type="bullet">
    ///   <item>Storm (true) = stone-storm effect (<c>MODEL_STORM3</c> + falling debris around Hero)</item>
    ///   <item>Rain (false) = stone-rain effect (<c>MODEL_MAYASTONE</c> projectiles falling on Hero)</item>
    /// </list>
    /// This is a purely cosmetic broadcast — damage is handled by the server-side
    /// <see cref="MonsterDefinition.AttackSkill"/> on Maya Hands (#362/#363).
    /// </summary>
    /// <param name="isStorm">True for storm visual; false for stone-rain visual.</param>
    ValueTask ShowMayaWideAreaAttackAsync(bool isStorm);
}
