// <copyright file="HostilityResponseAction.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.GameLogic.PlayerActions.Guild;

using MUnique.OpenMU.GameLogic.Views.Guild;
using MUnique.OpenMU.Interfaces;

/// <summary>
/// Action to respond to a guild hostility request.
/// </summary>
public class HostilityResponseAction
{
    /// <summary>
    /// Responds to a hostility request.
    /// </summary>
    /// <param name="player">The player (must be guild master).</param>
    /// <param name="accepted">if set to <c>true</c>, the hostility request is accepted.</param>
    public async ValueTask RespondToHostilityAsync(Player player, bool accepted)
    {
        if (player.PendingHostilityRequest is not { } request)
        {
            return;
        }

        if (player.GuildStatus is not { } guildStatus
            || player.GameContext is not IGameServerContext serverContext
            || await serverContext.GuildServer.GetGuildAsync(player.GuildStatus.GuildId).ConfigureAwait(false) is not { Name: not null } targetGuild)
        {
            player.PendingHostilityRequest = null;
            return;
        }

        if (guildStatus.Position != GuildPosition.GuildMaster)
        {
            player.PendingHostilityRequest = null;
            return;
        }

        var requestingGuildId = await serverContext.GuildServer.GetGuildIdByNameAsync(request.RequesterGuildName).ConfigureAwait(false);
        Player? requester = null;

        await serverContext.ForEachGuildPlayerAsync(requestingGuildId, p =>
        {
            if (p.Name == request.RequesterPlayerName)
            {
                requester = p;
            }

            return Task.CompletedTask;
        }).ConfigureAwait(false);

        if (!accepted)
        {
            player.PendingHostilityRequest = null;
            if (requester is not null)
            {
                await requester.InvokeViewPlugInAsync<IShowAllianceResponsePlugIn>(p => p.ShowResponseAsync(AllianceResponse.Failed, targetGuild.Name!)).ConfigureAwait(false);
            }

            return;
        }

        // Check if target guild already has hostility
        if (targetGuild.Hostility is not null)
        {
            player.PendingHostilityRequest = null;
            await player.InvokeViewPlugInAsync<IShowAllianceResponsePlugIn>(p => p.ShowResponseAsync(AllianceResponse.HasHostility, request.RequesterGuildName)).ConfigureAwait(false);
            if (requester is not null)
            {
                await requester.InvokeViewPlugInAsync<IShowAllianceResponsePlugIn>(p => p.ShowResponseAsync(AllianceResponse.Failed, targetGuild.Name!)).ConfigureAwait(false);
            }

            return;
        }

        // Create the hostility
        var success = await serverContext.GuildServer.CreateHostilityAsync(requestingGuildId, guildStatus.GuildId).ConfigureAwait(false);

        player.PendingHostilityRequest = null;

        if (success)
        {
            // Notify both guild masters
            await player.InvokeViewPlugInAsync<IShowAllianceResponsePlugIn>(p => p.ShowResponseAsync(AllianceResponse.Success, request.RequesterGuildName)).ConfigureAwait(false);
            if (requester is not null)
            {
                await requester.InvokeViewPlugInAsync<IShowAllianceResponsePlugIn>(p => p.ShowResponseAsync(AllianceResponse.Success, targetGuild.Name!)).ConfigureAwait(false);
            }

            // Notify all members of both guilds about the hostility
            await this.NotifyGuildMembersAsync(serverContext, requestingGuildId).ConfigureAwait(false);
            await this.NotifyGuildMembersAsync(serverContext, guildStatus.GuildId).ConfigureAwait(false);
        }
        else
        {
            await player.InvokeViewPlugInAsync<IShowAllianceResponsePlugIn>(p => p.ShowResponseAsync(AllianceResponse.Failed, request.RequesterGuildName)).ConfigureAwait(false);
            if (requester is not null)
            {
                await requester.InvokeViewPlugInAsync<IShowAllianceResponsePlugIn>(p => p.ShowResponseAsync(AllianceResponse.Failed, targetGuild.Name!)).ConfigureAwait(false);
            }
        }
    }

    private async ValueTask NotifyGuildMembersAsync(IGameServerContext serverContext, uint guildId)
    {
        await serverContext.ForEachGuildPlayerAsync(guildId, async p =>
        {
            // Refresh the guild list to show the new hostility
            if (p.GuildStatus is not null)
            {
                var guildList = await serverContext.GuildServer.GetGuildListAsync(p.GuildStatus.GuildId).ConfigureAwait(false);
                await p.InvokeViewPlugInAsync<IShowGuildListPlugIn>(plugin => plugin.ShowGuildListAsync(guildList)).ConfigureAwait(false);
            }
        }).ConfigureAwait(false);
    }
}
