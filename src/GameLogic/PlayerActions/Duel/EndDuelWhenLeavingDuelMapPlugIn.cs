// <copyright file="EndDuelWhenLeavingDuelMapPlugIn.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.GameLogic.PlayerActions.Duel;

using System.Runtime.InteropServices;
using MUnique.OpenMU.GameLogic.PlugIns;
using MUnique.OpenMU.PlugIns;

/// <summary>
/// Ends the duel when a player leaves the duel map.
/// </summary>
[PlugIn(nameof(EndDuelWhenLeavingDuelMapPlugIn), "Updates the state of the weather of each hosted map in a random way.")]
[Guid("3DF85180-4C51-437A-8072-8F42EEFED983")]
public class EndDuelWhenLeavingDuelMapPlugIn : IObjectRemovedFromMapPlugIn
{
    private readonly DuelActions _duelActions = new();

    public async ValueTask ObjectRemovedFromMapAsync(GameMap map, ILocateable removedObject)
    {
        if (removedObject is not Player player)
        {
            return;
        }

        if (player.DuelRoom is not { } duelRoom)
        {
            return;
        }

        // When respawning during the duel, don't end the duel
        var removedFromDuelMap = duelRoom.Area.FirstPlayerGate?.Map == map.Definition;
        if (removedFromDuelMap
            && duelRoom.IsDuelist(player)
            && duelRoom.State is DuelState.DuelStarted
            && player.IsAlive)
        {
            await duelRoom.CancelDuelAsync().ConfigureAwait(false);
            return;
        }

        if (duelRoom.IsDuelist(player))
        {
            return;
        }

        var config = player.GameContext.Configuration.DuelConfiguration;
        var isInDuelMap = config?.DuelAreas.Any(area => area.SpectatorsGate?.Map == player.CurrentMap?.Definition) ?? false;
        if (!isInDuelMap)
        {
            return;
        }

        await duelRoom.RemoveSpectatorAsync(player).ConfigureAwait(false);
    }
}