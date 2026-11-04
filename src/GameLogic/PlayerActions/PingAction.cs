// <copyright file="PingAction.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.GameLogic.PlayerActions;

using MUnique.OpenMU.GameLogic.PlugIns;

/// <summary>
/// Handles the periodic ping of a client.
/// </summary>
public class PingAction
{
    /// <summary>
    /// Reports the client as alive and observes the timing information of the ping.
    /// </summary>
    /// <param name="player">The player.</param>
    /// <param name="tickCount">The client tick count reported by the ping.</param>
    /// <param name="attackSpeed">The attack speed reported by the ping.</param>
    public async ValueTask PingAsync(Player player, uint tickCount, ushort attackSpeed)
    {
        player.ReportAlive();

        if (player.GameContext.PlugInManager.GetPlugInPoint<IPingCheckPlugIn>() is { } pingCheck)
        {
            await pingCheck.PingCheatCheckAsync(player, tickCount, attackSpeed).ConfigureAwait(false);
        }
    }
}
