// <copyright file="KanturuGatewayPlugIn.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.GameLogic.PlugIns;

using System.Runtime.InteropServices;
using MUnique.OpenMU.DataModel.Configuration;
using MUnique.OpenMU.GameLogic.MiniGames;
using MUnique.OpenMU.GameLogic.NPC;
using MUnique.OpenMU.GameLogic.PlayerActions.MiniGames;
using MUnique.OpenMU.GameLogic.PlugIns.PeriodicTasks;
using MUnique.OpenMU.PlugIns;

/// <summary>
/// Handles the Gateway Machine NPC (NPC #367) in Kanturu Relics.
/// When a player talks to this NPC, the server sends a <c>0xD1/0x00</c> StateInfo
/// packet so that the client opens the <c>INTERFACE_KANTURU2ND_ENTERNPC</c> dialog.
/// The actual entry is triggered later when the client sends <c>0xD1/0x01</c>
/// (KanturuEnterRequest) — handled by <see cref="KanturuEnterRequestHandlerPlugIn"/>.
/// </summary>
[Guid("B7E4D2A3-1F85-4DAB-9074-19B4708389D5")]
[PlugIn]
[Display(Name = nameof(KanturuGatewayPlugIn), Description = "Handles the Kanturu Gateway Machine NPC for event entry.")]
public class KanturuGatewayPlugIn : IPlayerTalkToNpcPlugIn
{
    private const short GatewayMachineNumber = 367;

    // Detail state for the dialog when entry is open:
    // KANTURU_MAYA_DIRECTION_STANBY1 = 1 — shows user count and enables Enter button.
    private const byte DetailStandbyOpen = 1;

    /// <inheritdoc />
    public async ValueTask PlayerTalksToNpcAsync(Player player, NonPlayerCharacter npc, NpcTalkEventArgs eventArgs)
    {
        if (npc.Definition.Number != GatewayMachineNumber)
        {
            return;
        }

        // Mark as handled before any await so TalkNpcAction sees it synchronously.
        eventArgs.HasBeenHandled = true;

        await SendKanturuStateInfoAsync(player).ConfigureAwait(false);
    }

    /// <summary>
    /// Sends the 0xD1/0x00 StateInfo packet to the player so the client opens the
    /// gateway dialog.  Also called from <see cref="KanturuInfoRequestHandlerPlugIn"/>
    /// when the client refreshes the dialog.
    /// </summary>
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

        byte state;
        byte detailState;
        byte enter;
        byte userCount;
        int remainTimeSec;

        if (ctx is KanturuContext kanturuCtx)
        {
            // A Kanturu event is actively running — reflect its real-time phase.
            // The client dialog will show "MayaBattle" or "NightmareBattle" as appropriate,
            // and optionally the Maya sub-phase (e.g., Monster1 or Maya2) when available.
            state = kanturuCtx.CurrentKanturuState;
            detailState = kanturuCtx.CurrentKanturuDetailState;

            // Entry allowed only during Maya-battle phases (including inter-phase standby).
            // NightmareBattle and Tower phase are both sealed — players who are already
            // inside the event access the Refinery Tower by walking through the opened
            // Elphis barrier; the Gateway does not re-admit anyone for those phases.
            enter = state == KanturuStateCode.MayaBattle ? (byte)1 : (byte)0;

            userCount = (byte)Math.Min(255, ctx.PlayerCount);
            remainTimeSec = 0;
        }
        else if (timeUntilOpening == TimeSpan.Zero)
        {
            // Entry window is open but the game context has not been created yet
            // (race: the scheduler opened the window but OnGameStartAsync hasn't run).
            state = KanturuStateCode.MayaBattle;
            detailState = DetailStandbyOpen;
            enter = 1;
            userCount = 0;
            remainTimeSec = 0;
        }
        else
        {
            // No active event — show countdown to the next scheduled start.
            state = KanturuStateCode.Standby;
            detailState = 1;   // STANBY_START — client shows "Opens in X minutes"
            enter = 0;
            userCount = 0;
            remainTimeSec = timeUntilOpening.HasValue
                ? (int)Math.Ceiling(timeUntilOpening.Value.TotalSeconds)
                : 0;
        }

        await player.InvokeViewPlugInAsync<IKanturuEventViewPlugIn>(p =>
            p.SendStateInfoAsync(state, detailState, enter, userCount, remainTimeSec))
            .ConfigureAwait(false);
    }
}
