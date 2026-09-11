// <copyright file="GuildListRequestAction.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.GameLogic.PlayerActions.Guild;

using MUnique.OpenMU.GameLogic.Views.Guild;
using MUnique.OpenMU.Interfaces;

/// <summary>
/// Action to request the guild list.
/// </summary>
public class GuildListRequestAction
{
    /// <summary>
    /// Requests the guild list of the guild the player is currently part of.
    /// </summary>
    /// <param name="player">The player.</param>
    public async ValueTask RequestGuildListAsync(Player player)
    {
        if (player.GuildStatus is null)
        {
            return;
        }

        // TODO: We may want to retrieve guild and guild members in one call, to avoid multiple calls. But for now, we can live with that.
        if ((player.GameContext as IGameServerContext)?.GuildServer is { } guildServer
            && await guildServer.GetGuildAsync(player.GuildStatus.GuildId).ConfigureAwait(false) is { } guild)
        {
            // The client displays the members in the received order, so we sort by rank
            // (master, assistant, battle master, normal members) and then by name.
            var players = (await guildServer.GetGuildListAsync(player.GuildStatus.GuildId).ConfigureAwait(false))
                .OrderBy(member => GetDisplayRank(member.PlayerPosition))
                .ThenBy(member => member.PlayerName, StringComparer.OrdinalIgnoreCase)
                .ToList();
            await player.InvokeViewPlugInAsync<IShowGuildListPlugIn>(p => p.ShowGuildListAsync(players, guild)).ConfigureAwait(false);
        }
    }

    private static int GetDisplayRank(GuildPosition position)
    {
        return position switch
        {
            GuildPosition.GuildMaster => 0,
            GuildPosition.AssistantMaster => 1,
            GuildPosition.BattleMaster => 2,
            GuildPosition.NormalMember => 3,
            _ => 4,
        };
    }
}