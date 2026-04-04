// <copyright file="GuildListRequestAction.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.GameLogic.PlayerActions.Guild;

using MUnique.OpenMU.GameLogic.Views.Guild;

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
            var players = await guildServer.GetGuildListAsync(player.GuildStatus.GuildId).ConfigureAwait(false);
            await player.InvokeViewPlugInAsync<IShowGuildListPlugIn>(p => p.ShowGuildListAsync(players, guild)).ConfigureAwait(false);
        }
    }
}