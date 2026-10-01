// <copyright file="GensRelationshipRulesPlugIn.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.GameLogic.Gens;

using System.ComponentModel;
using System.Runtime.InteropServices;
using MUnique.OpenMU.GameLogic.PlugIns;
using MUnique.OpenMU.GameLogic.Views;
using MUnique.OpenMU.GameLogic.Views.Guild;
using MUnique.OpenMU.PlugIns;

/// <summary>
/// The rules of the gens for parties, guilds and alliances:
/// <list type="bullet">
///   <item>Members of different gens can't form a party, and no party can be formed in the battle zone.</item>
///   <item>A player leaves its party, when it enters the battle zone.</item>
///   <item>A guild can only be created by a gens member, and only joined by members of the gens of the guild master.</item>
///   <item>An alliance can only be formed by the masters of guilds of the same gens.</item>
/// </list>
/// </summary>
[PlugIn]
[Display(Name = nameof(PlugInResources.GensRelationshipRulesPlugIn_Name), Description = nameof(PlugInResources.GensRelationshipRulesPlugIn_Description), ResourceType = typeof(PlugInResources))]
[Guid("F2A6C83D-1B79-4E54-9C0E-7D5B3A8F1E29")]
public class GensRelationshipRulesPlugIn : IPartyRequestingPlugIn, IGuildJoinRequestingPlugIn, IGuildCreatingPlugIn, IGuildRelationshipChangingPlugIn, IObjectAddedToMapPlugIn
{
    /// <inheritdoc />
    public async ValueTask PartyRequestingAsync(Player requester, Player target, CancelEventArgs eventArgs)
    {
        if (GensFeaturePlugIn.GetConfiguration(requester.GameContext) is not { } configuration)
        {
            return;
        }

        if (!configuration.AllowPartyInBattleZone
            && (IsInBattleZone(requester, configuration) || IsInBattleZone(target, configuration)))
        {
            eventArgs.Cancel = true;
            await requester.ShowLocalizedBlueMessageAsync(nameof(PlayerMessage.GensNoPartyInBattleZone)).ConfigureAwait(false);
            return;
        }

        if (!configuration.AllowPartyWithOtherGens)
        {
            var requesterGens = (await requester.GetGensMemberAsync().ConfigureAwait(false))?.Gens ?? GensType.None;
            var targetGens = (await target.GetGensMemberAsync().ConfigureAwait(false))?.Gens ?? GensType.None;
            if (requesterGens != GensType.None && targetGens != GensType.None && requesterGens != targetGens)
            {
                eventArgs.Cancel = true;
                await requester.ShowLocalizedBlueMessageAsync(nameof(PlayerMessage.GensNoPartyWithOtherGens)).ConfigureAwait(false);
            }
        }
    }

    /// <inheritdoc />
    public async ValueTask GuildJoinRequestingAsync(Player requester, Player guildMaster, CancelEventArgs eventArgs)
    {
        if (GensFeaturePlugIn.GetConfiguration(requester.GameContext) is not { GuildRequiresGens: true })
        {
            return;
        }

        var requesterGens = (await requester.GetGensMemberAsync().ConfigureAwait(false))?.Gens ?? GensType.None;
        var guildMasterGens = (await guildMaster.GetGensMemberAsync().ConfigureAwait(false))?.Gens ?? GensType.None;
        GuildRequestAnswerResult? result = guildMasterGens == GensType.None ? GuildRequestAnswerResult.GuildMasterNotInGens
            : requesterGens == GensType.None ? GuildRequestAnswerResult.NotInGensOfGuildMaster
            : requesterGens != guildMasterGens ? GuildRequestAnswerResult.GuildMasterInDifferentGens
            : null;
        if (result is { } denied)
        {
            eventArgs.Cancel = true;
            await requester.InvokeViewPlugInAsync<IGuildJoinResponsePlugIn>(p => p.ShowGuildJoinResponseAsync(denied)).ConfigureAwait(false);
        }
    }

    /// <inheritdoc />
    public async ValueTask GuildCreatingAsync(Player creator, CancelEventArgs eventArgs)
    {
        if (GensFeaturePlugIn.GetConfiguration(creator.GameContext) is { GuildRequiresGens: true }
            && (await creator.GetGensMemberAsync().ConfigureAwait(false))?.Gens is not (GensType.Duprian or GensType.Vanert))
        {
            eventArgs.Cancel = true;
            await creator.ShowLocalizedBlueMessageAsync(nameof(PlayerMessage.GensRequiredToCreateGuild)).ConfigureAwait(false);
        }
    }

    /// <inheritdoc />
    public async ValueTask GuildRelationshipChangingAsync(Player requester, Player targetGuildMaster, GuildRelationshipType relationshipType, GuildRelationshipRequestType requestType, CancelEventArgs eventArgs)
    {
        if (relationshipType != GuildRelationshipType.Alliance
            || requestType != GuildRelationshipRequestType.Join
            || GensFeaturePlugIn.GetConfiguration(requester.GameContext) is not { AllianceRequiresSameGens: true })
        {
            return;
        }

        var requesterGens = (await requester.GetGensMemberAsync().ConfigureAwait(false))?.Gens ?? GensType.None;
        var targetGens = (await targetGuildMaster.GetGensMemberAsync().ConfigureAwait(false))?.Gens ?? GensType.None;
        GuildRelationshipChangeResultType? result = requesterGens == GensType.None ? GuildRelationshipChangeResultType.AllianceMasterNotInGens
            : targetGens == GensType.None ? GuildRelationshipChangeResultType.GuildMasterNotInGens
            : requesterGens != targetGens ? GuildRelationshipChangeResultType.DifferentGens
            : null;
        if (result is { } denied)
        {
            eventArgs.Cancel = true;
            var targetId = targetGuildMaster.GetId(requester);
            await requester.InvokeViewPlugInAsync<IGuildRelationshipChangeResultPlugIn>(p => p.ShowResultAsync(relationshipType, requestType, denied, targetId)).ConfigureAwait(false);
        }
    }

    /// <inheritdoc />
    public async ValueTask ObjectAddedToMapAsync(GameMap map, ILocateable addedObject)
    {
        if (addedObject is not Player { Party: { } party } player
            || GensFeaturePlugIn.GetConfiguration(player.GameContext) is not { AllowPartyInBattleZone: false } configuration
            || !configuration.IsBattleZone(map.Definition.Number))
        {
            return;
        }

        await party.KickMySelfAsync(player).ConfigureAwait(false);
        await player.ShowLocalizedBlueMessageAsync(nameof(PlayerMessage.GensPartyLeftInBattleZone)).ConfigureAwait(false);
    }

    private static bool IsInBattleZone(Player player, GensConfiguration configuration)
    {
        return player.CurrentMap?.Definition is { } map && configuration.IsBattleZone(map.Number);
    }
}
