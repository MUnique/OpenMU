// <copyright file="AllianceRequestAction.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.GameLogic.PlayerActions.Guild;

using MUnique.OpenMU.GameLogic.Views.Guild;
using MUnique.OpenMU.Interfaces;

/// <summary>
/// Action to request a guild alliance.
/// </summary>
public class AllianceRequestAction
{
    /// <summary>
    /// Requests an alliance with the target guild master.
    /// </summary>
    /// <param name="player">The player (must be guild master).</param>
    /// <param name="targetGuildName">Name of the target guild.</param>
    public async ValueTask RequestAllianceAsync(Player player, string targetGuildName)
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

        // Check if requesting guild has hostility
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
        targetGuildMaster.PendingAllianceRequest = new PendingAllianceRequest
        {
            RequesterGuildName = guild.Name!,
            RequesterPlayerName = player.Name!,
        };

        await targetGuildMaster.InvokeViewPlugInAsync<IShowAllianceRequestPlugIn>(p => p.ShowRequestAsync(guild.Name!)).ConfigureAwait(false);
        await player.InvokeViewPlugInAsync<IShowAllianceResponsePlugIn>(p => p.ShowResponseAsync(AllianceResponse.RequestSent, targetGuildName)).ConfigureAwait(false);
    }
}

/// <summary>
/// Represents the response result for alliance operations.
/// </summary>
public enum AllianceResponse
{
    /// <summary>
    /// The player is not in a guild.
    /// </summary>
    NotInGuild,

    /// <summary>
    /// The player is not the guild master.
    /// </summary>
    NotTheGuildMaster,

    /// <summary>
    /// The target guild was not found.
    /// </summary>
    GuildNotFound,

    /// <summary>
    /// The guild master is offline.
    /// </summary>
    GuildMasterOffline,

    /// <summary>
    /// The request was sent successfully.
    /// </summary>
    RequestSent,

    /// <summary>
    /// The alliance was created successfully.
    /// </summary>
    Success,

    /// <summary>
    /// The operation failed.
    /// </summary>
    Failed,

    /// <summary>
    /// The guild has a hostility relationship.
    /// </summary>
    HasHostility,

    /// <summary>
    /// The alliance is full.
    /// </summary>
    AllianceFull,

    /// <summary>
    /// The guild is already in an alliance.
    /// </summary>
    AlreadyInAlliance,

    /// <summary>
    /// The guild was removed from the alliance.
    /// </summary>
    Removed,
}
