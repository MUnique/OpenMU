// <copyright file="GuildRequestAction.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.GameLogic.PlayerActions.Guild;

using MUnique.OpenMU.GameLogic.Views.Guild;
using MUnique.OpenMU.Interfaces;

/// <summary>
/// Action to request guild membership from a guild master player.
/// </summary>
public class GuildRequestAction
{
    /// <summary>
    /// Requests the guild from the guild master player with the specified id.
    /// </summary>
    /// <param name="player">The player.</param>
    /// <param name="guildMasterId">The guild master identifier.</param>
    public async ValueTask RequestGuildAsync(Player player, ushort guildMasterId)
    {
        if (player.Level < 6)
        {
            await player.InvokeViewPlugInAsync<IGuildJoinResponsePlugIn>(p => p.ShowGuildJoinResponseAsync(GuildRequestAnswerResult.MinimumLevel6)).ConfigureAwait(false);
            return;
        }

        if (player.GuildStatus is not null)
        {
            await player.InvokeViewPlugInAsync<IGuildJoinResponsePlugIn>(p => p.ShowGuildJoinResponseAsync(GuildRequestAnswerResult.AlreadyHaveGuild)).ConfigureAwait(false);
            return;
        }

        var guildMaster = player.CurrentMap?.GetObject(guildMasterId) as Player;

        if (guildMaster?.GuildStatus?.Position != GuildPosition.GuildMaster)
        {
            await player.InvokeViewPlugInAsync<IGuildJoinResponsePlugIn>(p => p.ShowGuildJoinResponseAsync(GuildRequestAnswerResult.NotTheGuildMaster)).ConfigureAwait(false);
            return; // targeted player not in a guild or not the guild master
        }

        if (guildMaster.LastGuildRequester != null || player.PlayerState.CurrentState != PlayerState.EnteredWorld)
        {
            await player.InvokeViewPlugInAsync<IGuildJoinResponsePlugIn>(p => p.ShowGuildJoinResponseAsync(GuildRequestAnswerResult.GuildMasterOrRequesterIsBusy)).ConfigureAwait(false);
            return;
        }

        guildMaster.LastGuildRequester = player;
        await guildMaster.InvokeViewPlugInAsync<IShowGuildJoinRequestPlugIn>(p => p.ShowGuildJoinRequestAsync(player)).ConfigureAwait(false);
    }
}