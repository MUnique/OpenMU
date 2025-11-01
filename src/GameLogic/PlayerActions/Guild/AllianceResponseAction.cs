// <copyright file="AllianceResponseAction.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.GameLogic.PlayerActions.Guild;

using MUnique.OpenMU.GameLogic.Views.Guild;
using MUnique.OpenMU.Interfaces;

/// <summary>
/// Action to respond to a guild alliance request.
/// </summary>
public class AllianceResponseAction
{
    /// <summary>
    /// Responds to an alliance request.
    /// </summary>
    /// <param name="player">The player (must be guild master).</param>
    /// <param name="accepted">if set to <c>true</c>, the alliance request is accepted.</param>
    public async ValueTask RespondToAllianceAsync(Player player, bool accepted)
    {
        if (player.PendingAllianceRequest is not { } request)
        {
            return;
        }

        if (player.GuildStatus is not { } guildStatus
            || player.GameContext is not IGameServerContext serverContext
            || await serverContext.GuildServer.GetGuildAsync(player.GuildStatus.GuildId).ConfigureAwait(false) is not { Name: not null } targetGuild)
        {
            player.PendingAllianceRequest = null;
            return;
        }

        if (guildStatus.Position != GuildPosition.GuildMaster)
        {
            player.PendingAllianceRequest = null;
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
            player.PendingAllianceRequest = null;
            if (requester is not null)
            {
                await requester.InvokeViewPlugInAsync<IShowAllianceResponsePlugIn>(p => p.ShowResponseAsync(AllianceResponse.Failed, targetGuild.Name!)).ConfigureAwait(false);
            }

            return;
        }

        // Check if target guild has hostility
        if (targetGuild.Hostility is not null)
        {
            player.PendingAllianceRequest = null;
            await player.InvokeViewPlugInAsync<IShowAllianceResponsePlugIn>(p => p.ShowResponseAsync(AllianceResponse.HasHostility, request.RequesterGuildName)).ConfigureAwait(false);
            if (requester is not null)
            {
                await requester.InvokeViewPlugInAsync<IShowAllianceResponsePlugIn>(p => p.ShowResponseAsync(AllianceResponse.Failed, targetGuild.Name!)).ConfigureAwait(false);
            }

            return;
        }

        // Create the alliance
        var success = await serverContext.GuildServer.CreateAllianceAsync(requestingGuildId, guildStatus.GuildId).ConfigureAwait(false);

        player.PendingAllianceRequest = null;

        if (success)
        {
            // Notify all members of both guilds
            await this.NotifyAllianceGuildsAsync(serverContext, requestingGuildId).ConfigureAwait(false);

            await player.InvokeViewPlugInAsync<IShowAllianceResponsePlugIn>(p => p.ShowResponseAsync(AllianceResponse.Success, request.RequesterGuildName)).ConfigureAwait(false);
            if (requester is not null)
            {
                await requester.InvokeViewPlugInAsync<IShowAllianceResponsePlugIn>(p => p.ShowResponseAsync(AllianceResponse.Success, targetGuild.Name!)).ConfigureAwait(false);
            }
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

    private async ValueTask NotifyAllianceGuildsAsync(IGameServerContext serverContext, uint allianceMasterGuildId)
    {
        var allianceGuildIds = await serverContext.GuildServer.GetAllianceMemberGuildIdsAsync(allianceMasterGuildId).ConfigureAwait(false);

        foreach (var guildId in allianceGuildIds)
        {
            await serverContext.ForEachGuildPlayerAsync(guildId, async p =>
            {
                await p.InvokeViewPlugInAsync<IShowAllianceListUpdatePlugIn>(plugin => plugin.UpdateAllianceListAsync()).ConfigureAwait(false);
            }).ConfigureAwait(false);
        }
    }
}
