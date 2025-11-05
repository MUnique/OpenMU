// <copyright file="HostilityRequestAction.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.GameLogic.PlayerActions.Guild;

using MUnique.OpenMU.GameLogic.GuildWar;
using MUnique.OpenMU.GameLogic.Views.Guild;
using MUnique.OpenMU.Interfaces;

/// <summary>
/// Action to request a guild hostility.
/// </summary>
public class HostilityRequestAction
{
    /// <summary>
    /// Requests a hostility with the target guild master.
    /// </summary>
    /// <param name="player">The player (must be guild master).</param>
    /// <param name="targetGuildName">Name of the target guild.</param>
    public async ValueTask RequestHostilityAsync(Player player, string targetGuildName)
    {
        if (player.GuildStatus is not { } guildStatus
            || player.GameContext is not IGameServerContext serverContext
            || await serverContext.GuildServer.GetGuildAsync(player.GuildStatus.GuildId).ConfigureAwait(false) is not { Name: not null } guild)
        {
            await player.InvokeViewPlugInAsync<IShowAllianceResponsePlugIn>(p => p.ShowResponseAsync(AllianceResponse.NotInGuild, targetGuildName)).ConfigureAwait(false);
            return;
        }

        if (guildStatus.Position != GuildPosition.GuildMaster)
        {
            await player.InvokeViewPlugInAsync<IShowAllianceResponsePlugIn>(p => p.ShowResponseAsync(AllianceResponse.NotTheGuildMaster, targetGuildName)).ConfigureAwait(false);
            return;
        }

        if (!await serverContext.GuildServer.GuildExistsAsync(targetGuildName).ConfigureAwait(false))
        {
            await player.InvokeViewPlugInAsync<IShowAllianceResponsePlugIn>(p => p.ShowResponseAsync(AllianceResponse.GuildNotFound, targetGuildName)).ConfigureAwait(false);
            return;
        }

        // Check if requesting guild already has hostility
        if (guild.Hostility is not null)
        {
            await player.InvokeViewPlugInAsync<IShowAllianceResponsePlugIn>(p => p.ShowResponseAsync(AllianceResponse.HasHostility, targetGuildName)).ConfigureAwait(false);
            return;
        }

        var targetGuildId = await serverContext.GuildServer.GetGuildIdByNameAsync(targetGuildName).ConfigureAwait(false);

        Player? targetGuildMaster = null;
        await serverContext.ForEachGuildPlayerAsync(targetGuildId, p =>
        {
            targetGuildMaster = p.GuildStatus?.Position == GuildPosition.GuildMaster ? p : targetGuildMaster;
            return Task.CompletedTask;
        }).ConfigureAwait(false);

        if (targetGuildMaster is null)
        {
            await player.InvokeViewPlugInAsync<IShowAllianceResponsePlugIn>(p => p.ShowResponseAsync(AllianceResponse.GuildMasterOffline, targetGuildName)).ConfigureAwait(false);
            return;
        }

        // Store the pending request in the target guild master's context
        targetGuildMaster.PendingHostilityRequest = new PendingHostilityRequest
        {
            RequesterGuildName = guild.Name!,
            RequesterPlayerName = player.Name!,
        };

        await targetGuildMaster.InvokeViewPlugInAsync<IShowGuildWarRequestPlugIn>(p => p.ShowRequestAsync(guild.Name!, GuildWarType.Normal)).ConfigureAwait(false);
        await player.InvokeViewPlugInAsync<IShowAllianceResponsePlugIn>(p => p.ShowResponseAsync(AllianceResponse.RequestSent, targetGuildName)).ConfigureAwait(false);
    }
}
