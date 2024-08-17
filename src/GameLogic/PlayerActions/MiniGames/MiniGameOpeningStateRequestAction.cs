// <copyright file="MiniGameOpeningStateRequestAction.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.GameLogic.PlayerActions.MiniGames;

using MUnique.OpenMU.GameLogic.MiniGames;
using MUnique.OpenMU.GameLogic.PlugIns.ChatCommands;
using MUnique.OpenMU.GameLogic.PlugIns.PeriodicTasks;

/// <summary>
/// Action which handles the mini game opening state request.
/// </summary>
public class MiniGameOpeningStateRequestAction
{
    /// <summary>
    /// Handles the opening state request.
    /// </summary>
    /// <param name="player">The player.</param>
    /// <param name="miniGameType">Type of the mini game.</param>
    /// <param name="eventLevel">The event level.</param>
    public async ValueTask HandleRequestAsync(Player player, MiniGameType miniGameType, byte eventLevel)
    {
        if (player.GameContext.PlugInManager.GetStrategy<MiniGameType, IPeriodicMiniGameStartPlugIn>(miniGameType) is { } miniGameStartPlugIn
              && player.GetSuitableMiniGameDefinition(miniGameType, eventLevel) is { } miniGameDefinition)
        {
            var timeUntilOpening = await miniGameStartPlugIn.GetDurationUntilNextStartAsync(player.GameContext, miniGameDefinition).ConfigureAwait(false);
            var playerCount = 0;
            if (timeUntilOpening == TimeSpan.Zero
                && await miniGameStartPlugIn.GetMiniGameContextAsync(player.GameContext, miniGameDefinition).ConfigureAwait(false) is { } miniGameContext)
            {
                playerCount = miniGameContext.PlayerCount;
            }

            await player.InvokeViewPlugInAsync<IShowMiniGameOpeningStatePlugIn>(p => p.ShowOpeningStateAsync(miniGameType, timeUntilOpening, playerCount)).ConfigureAwait(false);
            return;
        }

        switch (miniGameType)
        {
            case MiniGameType.BloodCastle:
            case MiniGameType.DevilSquare:
                await player.ShowMessageAsync("Event map is created on entrance. No fixed time table.").ConfigureAwait(false);
                break;
            case MiniGameType.Doppelganger:
            case MiniGameType.IllusionTemple:
                await player.ShowMessageAsync("This event is not implemented yet.").ConfigureAwait(false);
                break;
            default:
                throw new ArgumentOutOfRangeException($"Unhandled event type {miniGameType}.");
        }
    }
}