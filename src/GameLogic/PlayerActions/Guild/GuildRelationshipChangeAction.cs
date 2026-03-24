// <copyright file="GuildRelationshipChangeAction.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.GameLogic.PlayerActions.Guild;

using MUnique.OpenMU.GameLogic.Views.Guild;
using MUnique.OpenMU.Interfaces;

/// <summary>
/// Action which handles guild relationship changes (alliance creation/removal and hostility).
/// </summary>
public class GuildRelationshipChangeAction
{
    /// <summary>
    /// Handles an incoming relationship change request from a guild master.
    /// Validates and forwards the request to the target guild master.
    /// </summary>
    /// <param name="player">The player requesting the relationship change.</param>
    /// <param name="targetPlayerId">The player id of the target guild master.</param>
    /// <param name="relationshipType">The type of relationship change (Alliance or Hostility).</param>
    /// <param name="requestType">The type of request (Join or Leave).</param>
    public async ValueTask RequestAsync(Player player, ushort targetPlayerId, GuildRelationshipType relationshipType, GuildRelationshipRequestType requestType)
    {
        if (player.GuildStatus is not { } guildStatus
            || player.GameContext is not IGameServerContext serverContext)
        {
            await player.InvokeViewPlugInAsync<IGuildRelationshipChangeResultPlugIn>(
                p => p.ShowResultAsync(relationshipType, requestType, false)).ConfigureAwait(false);
            return;
        }

        if (guildStatus.Position != GuildPosition.GuildMaster)
        {
            await player.InvokeViewPlugInAsync<IGuildRelationshipChangeResultPlugIn>(
                p => p.ShowResultAsync(relationshipType, requestType, false)).ConfigureAwait(false);
            return;
        }

        // For hostility leave (clearing hostility), no target needed
        if (relationshipType == GuildRelationshipType.Hostility && requestType == GuildRelationshipRequestType.Leave)
        {
            await serverContext.GuildServer.SetHostilityAsync(guildStatus.GuildId, 0, false).ConfigureAwait(false);
            await player.InvokeViewPlugInAsync<IGuildRelationshipChangeResultPlugIn>(
                p => p.ShowResultAsync(relationshipType, requestType, true)).ConfigureAwait(false);
            return;
        }

        // Find the target player
        var targetPlayer = await player.GetObservingPlayerWithIdAsync(targetPlayerId).ConfigureAwait(false);
        if (targetPlayer?.GuildStatus is not { } targetGuildStatus
            || await serverContext.GuildServer.GetGuildAsync(targetGuildStatus.GuildId).ConfigureAwait(false) is not { Name: not null } targetGuild)
        {
            await player.InvokeViewPlugInAsync<IGuildRelationshipChangeResultPlugIn>(
                p => p.ShowResultAsync(relationshipType, requestType, false)).ConfigureAwait(false);
            return;
        }

        if (targetGuildStatus.Position != GuildPosition.GuildMaster)
        {
            await player.InvokeViewPlugInAsync<IGuildRelationshipChangeResultPlugIn>(
                p => p.ShowResultAsync(relationshipType, requestType, false)).ConfigureAwait(false);
            return;
        }

        if (relationshipType == GuildRelationshipType.Hostility && requestType == GuildRelationshipRequestType.Join)
        {
            // Hostility is set unilaterally (no consent from target needed)
            var success = await serverContext.GuildServer.SetHostilityAsync(
                guildStatus.GuildId, targetGuildStatus.GuildId, true).ConfigureAwait(false);
            await player.InvokeViewPlugInAsync<IGuildRelationshipChangeResultPlugIn>(
                p => p.ShowResultAsync(relationshipType, requestType, success)).ConfigureAwait(false);
            return;
        }

        if (relationshipType == GuildRelationshipType.Alliance && requestType == GuildRelationshipRequestType.Join)
        {
            // Store the pending request on the target player and ask for consent
            targetPlayer.PendingAllianceRequest = player;
            var requestingGuild = await serverContext.GuildServer.GetGuildAsync(guildStatus.GuildId).ConfigureAwait(false);
            var requestingGuildName = requestingGuild?.Name ?? string.Empty;
            await targetPlayer.InvokeViewPlugInAsync<IShowGuildRelationshipRequestPlugIn>(
                p => p.ShowRequestAsync(
                    requestingGuildName,
                    relationshipType,
                    requestType)).ConfigureAwait(false);
            return;
        }

        await player.InvokeViewPlugInAsync<IGuildRelationshipChangeResultPlugIn>(
            p => p.ShowResultAsync(relationshipType, requestType, false)).ConfigureAwait(false);
    }

    /// <summary>
    /// Processes the response from the target guild master to an alliance request.
    /// </summary>
    /// <param name="player">The target guild master who is responding.</param>
    /// <param name="relationshipType">The type of relationship change (Alliance or Hostility).</param>
    /// <param name="requestType">The type of request (Join or Leave).</param>
    /// <param name="accepted">Whether the relationship change was accepted.</param>
    public async ValueTask ProcessResponseAsync(Player player, GuildRelationshipType relationshipType, GuildRelationshipRequestType requestType, bool accepted)
    {
        var requester = player.PendingAllianceRequest;
        player.PendingAllianceRequest = null;

        if (requester is null
            || requester.GuildStatus is not { } requesterGuildStatus
            || player.GuildStatus is not { } responderGuildStatus
            || player.GameContext is not IGameServerContext serverContext)
        {
            return;
        }

        if (!accepted)
        {
            await requester.InvokeViewPlugInAsync<IGuildRelationshipChangeResultPlugIn>(
                p => p.ShowResultAsync(relationshipType, requestType, false)).ConfigureAwait(false);
            return;
        }

        if (relationshipType == GuildRelationshipType.Alliance && requestType == GuildRelationshipRequestType.Join)
        {
            var success = await serverContext.GuildServer.CreateAllianceAsync(
                requesterGuildStatus.GuildId, responderGuildStatus.GuildId).ConfigureAwait(false);

            await requester.InvokeViewPlugInAsync<IGuildRelationshipChangeResultPlugIn>(
                p => p.ShowResultAsync(relationshipType, requestType, success)).ConfigureAwait(false);
            await player.InvokeViewPlugInAsync<IGuildRelationshipChangeResultPlugIn>(
                p => p.ShowResultAsync(relationshipType, requestType, success)).ConfigureAwait(false);
        }
    }
}
