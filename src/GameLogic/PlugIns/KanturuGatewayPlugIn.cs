// <copyright file="KanturuGatewayPlugIn.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.GameLogic.PlugIns;

using System.Runtime.InteropServices;
using MUnique.OpenMU.DataModel.Configuration;
using MUnique.OpenMU.GameLogic.MiniGames;
using MUnique.OpenMU.GameLogic.MiniGames.Kanturu;
using MUnique.OpenMU.GameLogic.NPC;
using MUnique.OpenMU.GameLogic.PlayerActions.MiniGames;
using MUnique.OpenMU.GameLogic.PlugIns.PeriodicTasks;
using MUnique.OpenMU.PlugIns;

/// <summary>
/// Handles the Gateway Machine NPC (NPC #367) in Kanturu Relics.
/// When a player talks to this NPC, the server sends a <c>0xD1/0x00</c> StateInfo
/// packet so that the client opens the <c>INTERFACE_KANTURU2ND_ENTERNPC</c> dialog.
/// The actual entry is triggered later when the client sends <c>0xD1/0x01</c>
/// (KanturuEnterRequest) — handled by the <c>KanturuEnterRequestHandlerPlugIn</c>.
/// </summary>
[Guid("B7E4D2A3-1F85-4DAB-9074-19B4708389D5")]
[PlugIn]
[Display(Name = nameof(KanturuGatewayPlugIn), Description = "Handles the Kanturu Gateway Machine NPC for event entry.")]
public class KanturuGatewayPlugIn : IPlayerTalkToNpcPlugIn
{
    /// <summary>
    /// The NPC number of the Gateway Machine.
    /// </summary>
    public const short GatewayMachineNumber = 367;

    // Detail state for the dialog when entry is open:
    // KANTURU_MAYA_DIRECTION_STANBY1 = 1 — shows user count and enables Enter button.
    private const byte DetailStandbyOpen = 1;

    // Detail state for the dialog while the next start is awaited:
    // STANBY_START = 1 — client shows "Opens in X minutes".
    private const byte DetailStandbyStart = 1;

    /// <summary>
    /// Sends the 0xD1/0x00 StateInfo packet to the player so the client opens the
    /// gateway dialog. Also called from the <c>KanturuInfoRequestHandlerPlugIn</c>
    /// when the client refreshes the dialog.
    /// </summary>
    /// <param name="player">The player which requested the state info.</param>
    public static async ValueTask SendKanturuStateInfoAsync(Player player)
    {
        var miniGameStartPlugIn = player.GameContext.PlugInManager
            .GetStrategy<MiniGameType, IPeriodicMiniGameStartPlugIn>(MiniGameType.Kanturu);

        var miniGameDefinition = player.GetSuitableMiniGameDefinition(MiniGameType.Kanturu, 1);

        if (miniGameStartPlugIn is null || miniGameDefinition is null)
        {
            // Event not configured — show nothing; the client won't open the dialog.
            return;
        }

        // Always fetch the running context first so we can reflect the live event state
        // regardless of whether the initial entry window is open or has already closed.
        var ctx = await miniGameStartPlugIn
            .GetMiniGameContextAsync(player.GameContext, miniGameDefinition)
            .ConfigureAwait(false);

        var timeUntilOpening = await miniGameStartPlugIn
            .GetDurationUntilNextStartAsync(player.GameContext, miniGameDefinition)
            .ConfigureAwait(false);

        var info = GetRunningEventInfo(ctx, player.GameContext)
            ?? GetOpenTowerInfo(player, miniGameDefinition)
            ?? GetLobbyOrCountdownInfo(timeUntilOpening);

        await player.InvokeViewPlugInAsync<IKanturuEventViewPlugIn>(p =>
            p.ShowStateInfoAsync(info.State, info.DetailState, info.CanEnter, info.UserCount, info.RemainTime))
            .ConfigureAwait(false);
    }

    /// <inheritdoc />
    public async ValueTask PlayerTalksToNpcAsync(Player player, NonPlayerCharacter npc, NpcTalkEventArgs eventArgs)
    {
        if (npc.Definition.Number != GatewayMachineNumber)
        {
            return;
        }

        // Mark as handled before any await so TalkNpcAction sees it synchronously.
        eventArgs.HasBeenHandled = true;

        // The client keeps the dialog open until the player enters or closes it, so the
        // player stays assigned to this NPC. The 0xD1 handlers check that assignment.
        eventArgs.LeavesDialogOpen = true;

        await SendKanturuStateInfoAsync(player).ConfigureAwait(false);
    }

    /// <summary>
    /// Gets the remaining tower window for the state info dialog, so the client can
    /// show when the tower closes.
    /// </summary>
    /// <param name="gameContext">The game context.</param>
    /// <returns>The remaining open window, or <see cref="TimeSpan.Zero"/> when closed.</returns>
    private static TimeSpan GetTowerRemainingTime(IGameContext gameContext)
    {
        if (KanturuTowerWindow.GetOpenUntilUtc(gameContext) is { } until)
        {
            var remaining = until - DateTime.UtcNow;
            if (remaining > TimeSpan.Zero)
            {
                return remaining;
            }
        }

        return TimeSpan.Zero;
    }

    private static KanturuDialogInfo? GetRunningEventInfo(MiniGameContext? ctx, IGameContext gameContext)
    {
        if (ctx is not KanturuContext kanturuCtx || kanturuCtx.State is MiniGameState.Ended or MiniGameState.Disposed)
        {
            return null;
        }

        // A Kanturu event is actively running — reflect its real-time phase.
        var state = kanturuCtx.CurrentKanturuState;

        // Entry allowed before the event starts and while the Tower of Refinement
        // is open, so that players who died or left can rejoin the tower. Fights
        // can never be joined mid-event; the Nightmare encounter is sealed.
        var canEnter = kanturuCtx.IsJoinable && state is KanturuState.MayaBattle or KanturuState.Tower;

        // During a refill standby the map HUD is hidden, but the dialog shows the
        // standby state, which is what enables its Enter button.
        var detailState = kanturuCtx.CurrentKanturuDetailState;
        if (canEnter && state == KanturuState.MayaBattle && detailState == KanturuContext.HudHiddenDetailState)
        {
            detailState = DetailStandbyOpen;
        }

        var remainTime = state == KanturuState.Tower ? GetTowerRemainingTime(gameContext) : TimeSpan.Zero;
        return new KanturuDialogInfo(state, detailState, canEnter, kanturuCtx.PlayerCount, remainTime);
    }

    private static KanturuDialogInfo? GetOpenTowerInfo(Player player, MiniGameDefinition miniGameDefinition)
    {
        if (KanturuTowerWindow.GetOpenUntilUtc(player.GameContext) is not { } towerUntil || towerUntil <= DateTime.UtcNow)
        {
            return null;
        }

        // No event game runs, but the tower window is still open (e.g. after a
        // server restart): entering recreates the tower without the event phases.
        PrewarmTowerGame(player, miniGameDefinition);
        return new KanturuDialogInfo(
            KanturuState.Tower,
            (byte)KanturuTowerDetailState.Revitalization,
            true,
            0,
            GetTowerRemainingTime(player.GameContext));
    }

    private static KanturuDialogInfo GetLobbyOrCountdownInfo(TimeSpan? timeUntilOpening)
    {
        if (timeUntilOpening == TimeSpan.Zero)
        {
            // Entry is open but no game exists yet: entering creates it on demand.
            return new KanturuDialogInfo(KanturuState.MayaBattle, DetailStandbyOpen, true, 0, TimeSpan.Zero);
        }

        // No active event — show countdown to the next scheduled start.
        return new KanturuDialogInfo(KanturuState.Standby, DetailStandbyStart, false, 0, timeUntilOpening ?? TimeSpan.Zero);
    }

    private static void PrewarmTowerGame(Player player, MiniGameDefinition miniGameDefinition)
    {
        // Start the creation while the player reads the dialog, so entering itself
        // feels like any other map. Failures are logged inside and surface as a
        // clean rejection on entry.
        _ = Task.Run(() => KanturuTowerEntry.EnsureTowerGameAsync(player, miniGameDefinition).AsTask());
    }

    private readonly record struct KanturuDialogInfo(KanturuState State, byte DetailState, bool CanEnter, int UserCount, TimeSpan RemainTime);
}
