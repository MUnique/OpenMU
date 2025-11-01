// <copyright file="AllianceLeaveAction.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.GameLogic.PlayerActions.Guild;

using MUnique.OpenMU.GameLogic.Views.Guild;
using MUnique.OpenMU.Interfaces;

/// <summary>
/// Action to leave a guild alliance.
/// </summary>
public class AllianceLeaveAction
{
    /// <summary>
    /// Leaves the current alliance.
    /// </summary>
    /// <param name="player">The player (must be guild master).</param>
    public async ValueTask LeaveAllianceAsync(Player player)
    {
        if (player.GuildStatus is not { } guildStatus
            || player.GameContext is not IGameServerContext serverContext
            || await serverContext.GuildServer.GetGuildAsync(player.GuildStatus.GuildId).ConfigureAwait(false) is not { Name: not null } guild)
        {
            await player.InvokeViewPlugInAsync<IShowAllianceResponsePlugIn>(p => p.ShowResponseAsync(AllianceResponse.NotInGuild, string.Empty)).ConfigureAwait(false);
            return;
        }

        if (guildStatus.Position != GuildPosition.GuildMaster)
        {
            await player.InvokeViewPlugInAsync<IShowAllianceResponsePlugIn>(p => p.ShowResponseAsync(AllianceResponse.NotTheGuildMaster, string.Empty)).ConfigureAwait(false);
            return;
        }

        var allianceMasterGuildId = await serverContext.GuildServer.GetAllianceMasterGuildIdAsync(guildStatus.GuildId).ConfigureAwait(false);

        var success = await serverContext.GuildServer.RemoveFromAllianceAsync(guildStatus.GuildId).ConfigureAwait(false);

        if (success)
        {
            // Notify all members of this guild
            await serverContext.ForEachGuildPlayerAsync(guildStatus.GuildId, async p =>
            {
                await p.InvokeViewPlugInAsync<IShowAllianceResponsePlugIn>(plugin => plugin.ShowResponseAsync(AllianceResponse.Success, string.Empty)).ConfigureAwait(false);
                await p.InvokeViewPlugInAsync<IShowAllianceListUpdatePlugIn>(plugin => plugin.UpdateAllianceListAsync()).ConfigureAwait(false);
            }).ConfigureAwait(false);

            // Notify remaining alliance members if there was an alliance
            if (allianceMasterGuildId != 0)
            {
                await this.NotifyAllianceGuildsAsync(serverContext, allianceMasterGuildId).ConfigureAwait(false);
            }
        }
        else
        {
            await player.InvokeViewPlugInAsync<IShowAllianceResponsePlugIn>(p => p.ShowResponseAsync(AllianceResponse.Failed, string.Empty)).ConfigureAwait(false);
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
