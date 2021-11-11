﻿// <copyright file="GuildListRequestAction.cs" company="MUnique">
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
    public void RequestGuildList(Player player)
    {
        if (player.GuildStatus is null)
        {
            return;
        }

        if ((player.GameContext as IGameServerContext)?.GuildServer is { } guildServer)
        {
            var players = guildServer.GetGuildList(player.GuildStatus.GuildId);
            player.ViewPlugIns.GetPlugIn<IShowGuildListPlugIn>()?.ShowGuildList(players);
        }
    }
}