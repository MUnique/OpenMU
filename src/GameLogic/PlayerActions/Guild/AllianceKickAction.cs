// <copyright file="AllianceKickAction.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.GameLogic.PlayerActions.Guild;

using MUnique.OpenMU.GameLogic.Views.Guild;
using MUnique.OpenMU.Interfaces;

/// <summary>
/// Action to kick a guild from an alliance.
/// </summary>
public class AllianceKickAction
{
    /// <summary>
    /// Kicks a guild from the alliance.
    /// </summary>
    /// <param name="player">The player (must be alliance master).</param>
    /// <param name="targetGuildName">Name of the guild to kick from the alliance.</param>
    public async ValueTask KickGuildFromAllianceAsync(Player player, string targetGuildName)
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

        // Check if this guild is an alliance master
        var allianceMasterGuildId = await serverContext.GuildServer.GetAllianceMasterGuildIdAsync(guildStatus.GuildId).ConfigureAwait(false);
        if (allianceMasterGuildId == 0 || allianceMasterGuildId != guildStatus.GuildId)
        {
            await player.InvokeViewPlugInAsync<IShowAllianceResponsePlugIn>(p => p.ShowResponseAsync(AllianceResponse.NotTheGuildMaster, targetGuildName)).ConfigureAwait(false);
            return;
        }

        var targetGuildId = await serverContext.GuildServer.GetGuildIdByNameAsync(targetGuildName).ConfigureAwait(false);
        if (targetGuildId == 0)
        {
            await player.InvokeViewPlugInAsync<IShowAllianceResponsePlugIn>(p => p.ShowResponseAsync(AllianceResponse.GuildNotFound, targetGuildName)).ConfigureAwait(false);
            return;
        }

        // Remove the guild from the alliance
        var success = await serverContext.GuildServer.RemoveFromAllianceAsync(targetGuildId).ConfigureAwait(false);

        if (success)
        {
            // Notify all members of the alliance
            await this.NotifyAllianceGuildsAsync(serverContext, guildStatus.GuildId).ConfigureAwait(false);

            // Notify removed guild
            await serverContext.ForEachGuildPlayerAsync(targetGuildId, async p =>
            {
                await p.InvokeViewPlugInAsync<IShowAllianceResponsePlugIn>(plugin => plugin.ShowResponseAsync(AllianceResponse.Removed, guild.Name!)).ConfigureAwait(false);
                await p.InvokeViewPlugInAsync<IShowAllianceListUpdatePlugIn>(plugin => plugin.UpdateAllianceListAsync()).ConfigureAwait(false);
            }).ConfigureAwait(false);

            await player.InvokeViewPlugInAsync<IShowAllianceResponsePlugIn>(p => p.ShowResponseAsync(AllianceResponse.Success, targetGuildName)).ConfigureAwait(false);
        }
        else
        {
            await player.InvokeViewPlugInAsync<IShowAllianceResponsePlugIn>(p => p.ShowResponseAsync(AllianceResponse.Failed, targetGuildName)).ConfigureAwait(false);
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
