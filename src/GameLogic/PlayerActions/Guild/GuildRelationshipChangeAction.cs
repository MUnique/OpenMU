// <copyright file="GuildRelationshipChangeAction.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.GameLogic.PlayerActions.Guild;

using MUnique.OpenMU.GameLogic.Views;
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
            await player.InvokeViewPlugInAsync<IGuildRelationshipChangeResultPlugIn>(p => p.ShowResultAsync(relationshipType, requestType, GuildRelationshipChangeResultType.Failed, targetPlayerId)).ConfigureAwait(false);
            return;
        }

        if (guildStatus.Position != GuildPosition.GuildMaster)
        {
            await player.InvokeViewPlugInAsync<IGuildRelationshipChangeResultPlugIn>(p => p.ShowResultAsync(relationshipType, requestType, GuildRelationshipChangeResultType.NoAuthorization, targetPlayerId)).ConfigureAwait(false);
            return;
        }

        // For hostility leave (clearing hostility), no target needed
        if (relationshipType == GuildRelationshipType.Hostility && requestType == GuildRelationshipRequestType.Leave)
        {
            // When clearing hostility, targetGuildId is not used (the current hostility target is removed)
            await serverContext.GuildServer.SetHostilityAsync(guildStatus.GuildId, 0, false).ConfigureAwait(false);
            await player.InvokeViewPlugInAsync<IGuildRelationshipChangeResultPlugIn>(p => p.ShowResultAsync(relationshipType, requestType, GuildRelationshipChangeResultType.Success, targetPlayerId)).ConfigureAwait(false);
            return;
        }

        // Find the target player
        var targetPlayer = await player.GetObservingPlayerWithIdAsync(targetPlayerId).ConfigureAwait(false);
        if (targetPlayer?.GuildStatus is not { } targetGuildStatus
            || await serverContext.GuildServer.GetGuildAsync(targetGuildStatus.GuildId).ConfigureAwait(false) is not { Name: not null } targetGuild)
        {
            await player.InvokeViewPlugInAsync<IGuildRelationshipChangeResultPlugIn>(p => p.ShowResultAsync(relationshipType, requestType, GuildRelationshipChangeResultType.Failed, targetPlayerId)).ConfigureAwait(false);
            return;
        }

        if (targetGuildStatus.Position != GuildPosition.GuildMaster)
        {
            await player.InvokeViewPlugInAsync<IGuildRelationshipChangeResultPlugIn>(p => p.ShowResultAsync(relationshipType, requestType, GuildRelationshipChangeResultType.Failed, targetPlayerId)).ConfigureAwait(false);
            return;
        }

        var sourceGuild = await serverContext.GuildServer.GetGuildAsync(guildStatus.GuildId).ConfigureAwait(false);
        if (sourceGuild is null)
        {
            await player.InvokeViewPlugInAsync<IGuildRelationshipChangeResultPlugIn>(p => p.ShowResultAsync(relationshipType, requestType, GuildRelationshipChangeResultType.GuildNotFound, targetPlayerId)).ConfigureAwait(false);
            return;
        }

        if (relationshipType == GuildRelationshipType.Hostility && requestType == GuildRelationshipRequestType.Join)
        {
            if (targetGuild.Hostility is not null)
            {
                await player.InvokeViewPlugInAsync<IGuildRelationshipChangeResultPlugIn>(p => p.ShowResultAsync(relationshipType, requestType, GuildRelationshipChangeResultType.AlreadyInHostility, targetPlayerId)).ConfigureAwait(false);
                return;
            }

            // Hostility is set unilaterally (no consent from target needed)
            var success = await serverContext.GuildServer.SetHostilityAsync(guildStatus.GuildId, targetGuildStatus.GuildId, true).ConfigureAwait(false);
            await player.InvokeViewPlugInAsync<IGuildRelationshipChangeResultPlugIn>(p => p.ShowResultAsync(relationshipType, requestType, success ? GuildRelationshipChangeResultType.Success : GuildRelationshipChangeResultType.Failed, targetPlayerId)).ConfigureAwait(false);
            return;
        }

        if (relationshipType == GuildRelationshipType.Alliance && requestType == GuildRelationshipRequestType.Join)
        {
            if (targetGuild.AllianceGuild is not null)
            {
                await player.InvokeViewPlugInAsync<IGuildRelationshipChangeResultPlugIn>(p => p.ShowResultAsync(relationshipType, requestType, GuildRelationshipChangeResultType.AlreadyInAlliance, targetPlayerId)).ConfigureAwait(false);
                return;
            }

            if (sourceGuild.AllianceGuild is not null && sourceGuild.AllianceGuild != sourceGuild)
            {
                // Request is not done by the master of the alliance, but by the master of a sub-guild
                await player.InvokeViewPlugInAsync<IGuildRelationshipChangeResultPlugIn>(p => p.ShowResultAsync(relationshipType, requestType, GuildRelationshipChangeResultType.NoAuthorization, targetPlayerId)).ConfigureAwait(false);
                return;
            }

            // Store the pending request on the target player and ask for consent
            targetPlayer.PendingAllianceRequest = player;
            await targetPlayer.InvokeViewPlugInAsync<IShowGuildRelationshipRequestPlugIn>(p => p.ShowRequestAsync(
                player,
                relationshipType,
                requestType)).ConfigureAwait(false);
            return;
        }

        await player.InvokeViewPlugInAsync<IGuildRelationshipChangeResultPlugIn>(p => p.ShowResultAsync(relationshipType, requestType, GuildRelationshipChangeResultType.Failed, targetPlayerId)).ConfigureAwait(false);
    }

    /// <summary>
    /// Handles an incoming relationship leave request from a guild master.
    /// Validates the request and processes the leave action.
    /// </summary>
    /// <param name="player">The player requesting the relationship change.</param>
    /// <param name="targetGuildName">The name of the guild which should be removed. If <see langword="null"/>, then the own guild should be removed.</param>
    public async ValueTask RequestLeaveAllianceAsync(Player player, string? targetGuildName = null)
    {
        if (player.GuildStatus is not { } guildStatus
            || player.GameContext is not IGameServerContext serverContext)
        {
            await player.InvokeViewPlugInAsync<IGuildRelationshipChangeResultPlugIn>(p => p.ShowResultAsync(GuildRelationshipType.Alliance, GuildRelationshipRequestType.Leave, GuildRelationshipChangeResultType.Failed, null)).ConfigureAwait(false);
            return;
        }

        if (guildStatus.Position != GuildPosition.GuildMaster)
        {
            await player.InvokeViewPlugInAsync<IGuildRelationshipChangeResultPlugIn>(p => p.ShowResultAsync(GuildRelationshipType.Alliance, GuildRelationshipRequestType.Leave, GuildRelationshipChangeResultType.NoAuthorization, null)).ConfigureAwait(false);
            return;
        }

        var sourceGuild = await serverContext.GuildServer.GetGuildAsync(guildStatus.GuildId).ConfigureAwait(false);
        if (sourceGuild is null)
        {
            await player.InvokeViewPlugInAsync<IGuildRelationshipChangeResultPlugIn>(p => p.ShowResultAsync(GuildRelationshipType.Alliance, GuildRelationshipRequestType.Leave, GuildRelationshipChangeResultType.GuildNotFound, null)).ConfigureAwait(false);
            return;
        }

        var targetGuildId = guildStatus.GuildId;
        var isOtherGuildThanOwn = sourceGuild.Name != targetGuildName;
        if (!isOtherGuildThanOwn)
        {
            // Not allowed in other sources?
            await player.InvokeViewPlugInAsync<IGuildRelationshipChangeResultPlugIn>(p => p.ShowResultAsync(GuildRelationshipType.Alliance, GuildRelationshipRequestType.Leave, GuildRelationshipChangeResultType.GuildNotFound, null)).ConfigureAwait(false);
            return;
        }

        if (!string.IsNullOrEmpty(targetGuildName) && isOtherGuildThanOwn)
        {
            if (!await serverContext.GuildServer.IsAllianceMasterAsync(guildStatus.GuildId).ConfigureAwait(false))
            {
                await player.InvokeViewPlugInAsync<IGuildRelationshipChangeResultPlugIn>(p => p.ShowResultAsync(GuildRelationshipType.Alliance, GuildRelationshipRequestType.Leave, GuildRelationshipChangeResultType.NoAuthorization, null)).ConfigureAwait(false);
                return;
            }

            targetGuildId = await serverContext.GuildServer.GetGuildIdByNameAsync(targetGuildName).ConfigureAwait(false);
        }

        var success = await serverContext.GuildServer.RemoveAllianceGuildAsync(guildStatus.GuildId, targetGuildId).ConfigureAwait(false);
        await player.InvokeViewPlugInAsync<IGuildRelationshipChangeResultPlugIn>(p => p.ShowRemoveResultAsync(success)).ConfigureAwait(false);
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

        var guildMasterId = player.GetId(requester);
        if (!accepted)
        {
            await requester.InvokeViewPlugInAsync<IGuildRelationshipChangeResultPlugIn>(p => p.ShowResultAsync(relationshipType, requestType, GuildRelationshipChangeResultType.RequestCancelled, guildMasterId)).ConfigureAwait(false);
            return;
        }

        if (relationshipType == GuildRelationshipType.Alliance && requestType == GuildRelationshipRequestType.Join)
        {
            var result = await serverContext.GuildServer.CreateAllianceAsync(requesterGuildStatus.GuildId, responderGuildStatus.GuildId).ConfigureAwait(false);
            var mappedResult = result switch
            {
                AllianceCreationResult.Success => GuildRelationshipChangeResultType.Success,
                AllianceCreationResult.MasterGuildNotFound or AllianceCreationResult.TargetGuildNotFound => GuildRelationshipChangeResultType.GuildNotFound,
                AllianceCreationResult.TargetGuildAlreadyInAlliance => GuildRelationshipChangeResultType.AlreadyInAlliance,
                AllianceCreationResult.MaximumAllianceSizeReached => GuildRelationshipChangeResultType.MaximumNumberOfGuildsInAllianceReached,
                _ => GuildRelationshipChangeResultType.Failed,
            };

            await requester.InvokeViewPlugInAsync<IGuildRelationshipChangeResultPlugIn>(p => p.ShowResultAsync(relationshipType, requestType, mappedResult, guildMasterId)).ConfigureAwait(false);
            await player.InvokeViewPlugInAsync<IGuildRelationshipChangeResultPlugIn>(p => p.ShowResultAsync(relationshipType, requestType, mappedResult, guildMasterId)).ConfigureAwait(false);
        }
    }
}
