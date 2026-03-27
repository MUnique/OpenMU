// <copyright file="IKanturuEventViewPlugIn.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.GameLogic.MiniGames;

using MUnique.OpenMU.GameLogic.Views;

/// <summary>
/// View plugin interface for Kanturu-specific server-to-client packets (0xD1 group).
/// Each method maps to one packet subcode. Names use Show... to signal that the game
/// logic is expressing intent, not dictating the transport mechanism.
/// </summary>
public interface IKanturuEventViewPlugIn : IViewPlugIn
{
    /// <summary>
    /// Shows the Kanturu state info dialog to the player (packet 0xD1/0x00).
    /// This opens the <c>INTERFACE_KANTURU2ND_ENTERNPC</c> dialog on the client,
    /// showing the current event state, whether entry is possible, how many players
    /// are inside, and how much time remains.
    /// </summary>
    /// <param name="state">Main event state.</param>
    /// <param name="detailState">
    /// Protocol detail-state byte. Pass the raw byte value from the appropriate
    /// detail-state enum (<see cref="KanturuMayaDetailState"/>,
    /// <see cref="KanturuNightmareDetailState"/>, or <see cref="KanturuTowerDetailState"/>).
    /// </param>
    /// <param name="canEnter"><c>true</c> if the Enter button should be enabled.</param>
    /// <param name="userCount">Number of players currently inside the event map.</param>
    /// <param name="remainingTime">
    /// Remaining time. Standby: time until the event opens.
    /// Tower: time the tower has been open. Otherwise <see cref="TimeSpan.Zero"/>.
    /// </param>
    ValueTask ShowStateInfoAsync(KanturuState state, byte detailState, bool canEnter, int userCount, TimeSpan remainingTime);

    /// <summary>
    /// Shows the result of the player's entry attempt (packet 0xD1/0x01).
    /// On success the player has already been teleported before this is sent;
    /// on failure the client displays the appropriate error popup.
    /// </summary>
    ValueTask ShowEnterResultAsync(KanturuEnterResult result);

    /// <summary>
    /// Updates the client HUD and audio for a Maya battle phase transition (packet 0xD1/0x03).
    /// </summary>
    ValueTask ShowMayaBattleStateAsync(KanturuMayaDetailState detailState);

    /// <summary>
    /// Updates the client HUD and audio for a Nightmare battle phase transition (packet 0xD1/0x03).
    /// </summary>
    ValueTask ShowNightmareStateAsync(KanturuNightmareDetailState detailState);

    /// <summary>
    /// Updates the client HUD and audio for a Tower of Refinement phase transition (packet 0xD1/0x03).
    /// When <paramref name="detailState"/> is <see cref="KanturuTowerDetailState.Revitalization"/>
    /// the client additionally reloads the success terrain file to remove the Elphis barrier visually.
    /// </summary>
    ValueTask ShowTowerStateAsync(KanturuTowerDetailState detailState);

    /// <summary>
    /// Shows the battle outcome overlay to the player (packet 0xD1/0x04).
    /// </summary>
    ValueTask ShowBattleResultAsync(KanturuBattleResult result);

    /// <summary>
    /// Starts the HUD countdown timer (packet 0xD1/0x05).
    /// The client converts the value to seconds for display.
    /// </summary>
    ValueTask ShowTimeLimitAsync(TimeSpan timeLimit);

    /// <summary>
    /// Updates the monster and user count numbers in the Kanturu HUD (packet 0xD1/0x07).
    /// </summary>
    ValueTask ShowMonsterUserCountAsync(int monsterCount, int userCount);

    /// <summary>
    /// Triggers a Maya wide-area attack visual on the client (packet 0xD1/0x06).
    /// This is purely cosmetic — damage is handled server-side by the monster's
    /// <c>AttackSkill</c>.
    /// </summary>
    ValueTask ShowMayaWideAreaAttackAsync(KanturuMayaAttackType attackType);
}

/// <summary>
/// Main state of the Kanturu event. Values are defined by the client's
/// <c>KANTURU_STATE_TYPE</c> enum and mapped to packet bytes in the view plugin layer.
/// </summary>
public enum KanturuState
{
    /// <summary>No active state.</summary>
    None,

    /// <summary>Waiting for players to enter before the event starts.</summary>
    Standby,

    /// <summary>Maya battle phase covering Phases 1–3 and their boss waves.</summary>
    MayaBattle,

    /// <summary>Nightmare battle phase after all three Maya phases are cleared.</summary>
    NightmareBattle,

    /// <summary>Tower of Refinement phase; opens after Nightmare is defeated.</summary>
    Tower,

    /// <summary>Event has ended.</summary>
    End,
}

/// <summary>
/// Detail states for the <see cref="KanturuState.MayaBattle"/> phase,
/// matching <c>KANTURU_MAYA_DIRECTION_TYPE</c> on the client.
/// </summary>
public enum KanturuMayaDetailState
{
    /// <summary>No direction; HUD is hidden.</summary>
    None,

    /// <summary>Maya notify/cinematic intro — camera pans to Maya room.</summary>
    Notify,

    /// <summary>Phase 1 monster wave; HUD visible.</summary>
    Monster1,

    /// <summary>Phase 1 boss: Maya Left Hand.</summary>
    MayaLeft,

    /// <summary>Phase 2 monster wave; HUD visible.</summary>
    Monster2,

    /// <summary>Phase 2 boss: Maya Right Hand.</summary>
    MayaRight,

    /// <summary>Phase 3 monster wave; HUD visible.</summary>
    Monster3,

    /// <summary>Phase 3 bosses: both Maya hands simultaneously.</summary>
    BothHands,

    /// <summary>
    /// Maya phase 3 end cycle — triggers the full Maya explosion and player-fall cinematic
    /// (<c>KANTURU_MAYA_DIRECTION_ENDCYCLE_MAYA3 = 16</c> on the client).
    /// </summary>
    EndCycleMaya3,
}

/// <summary>
/// Detail states for the <see cref="KanturuState.NightmareBattle"/> phase,
/// matching <c>KANTURU_NIGHTMARE_DIRECTION_TYPE</c> on the client.
/// </summary>
public enum KanturuNightmareDetailState
{
    /// <summary>No direction set.</summary>
    None,

    /// <summary>Nightmare present but idle — not yet in active battle.</summary>
    Idle,

    /// <summary>Nightmare intro animation playing.</summary>
    Intro,

    /// <summary>Active battle — shows the HUD on the client.</summary>
    Battle,

    /// <summary>Battle ended (Nightmare defeated).</summary>
    End,
}

/// <summary>
/// Detail states for the <see cref="KanturuState.Tower"/> phase,
/// matching <c>KANTURU_TOWER_STATE_TYPE</c> on the client.
/// </summary>
public enum KanturuTowerDetailState
{
    /// <summary>No tower state.</summary>
    None,

    /// <summary>
    /// Tower is open after Nightmare's defeat.
    /// Sending this triggers the client to reload <c>EncTerrain&lt;n&gt;01.att</c>
    /// (the success terrain), which visually removes the Elphis barrier.
    /// </summary>
    Revitalization,

    /// <summary>Tower closing soon — client warns players.</summary>
    Notify,

    /// <summary>Tower is closed.</summary>
    Close,
}

/// <summary>
/// Result of a Kanturu entry request.
/// </summary>
public enum KanturuEnterResult
{
    /// <summary>Entry failed (generic failure — level, missing pendant, event not open, etc.).</summary>
    Failed,

    /// <summary>The player was successfully entered into the event.</summary>
    Success,
}

/// <summary>
/// Outcome of the Kanturu Refinery Tower battle.
/// </summary>
public enum KanturuBattleResult
{
    /// <summary>Event ended in failure; shows the <c>Failure_kantru.tga</c> overlay.</summary>
    Failure,

    /// <summary>Nightmare was defeated; shows the <c>Success_kantru.tga</c> overlay.</summary>
    Victory,
}

/// <summary>
/// Visual type of a Maya wide-area attack broadcast.
/// </summary>
public enum KanturuMayaAttackType
{
    /// <summary>Stone-storm effect (<c>MODEL_STORM3</c> + falling debris).</summary>
    Storm,

    /// <summary>Stone-rain effect (<c>MODEL_MAYASTONE</c> projectiles).</summary>
    Rain,
}
