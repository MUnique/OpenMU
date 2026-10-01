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
            await ShowToBothAsync(requester, target, nameof(PlayerMessage.GensNoPartyInBattleZone)).ConfigureAwait(false);
            return;
        }

        if (!configuration.AllowPartyWithOtherGens
            && await HasMemberOfOtherGensAsync(requester, target).ConfigureAwait(false))
        {
            eventArgs.Cancel = true;
            await ShowToBothAsync(requester, target, nameof(PlayerMessage.GensNoPartyWithOtherGens)).ConfigureAwait(false);
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

    /// <summary>
    /// Determines whether the party of the requester (or the requester itself, when it has no party yet) has a member
    /// of another gens than the target. Members without gens are allowed with any gens, so all members are checked,
    /// not only the party master.
    /// </summary>
    private static async ValueTask<bool> HasMemberOfOtherGensAsync(Player requester, Player target)
    {
        var targetGens = (await target.GetGensMemberAsync().ConfigureAwait(false))?.Gens ?? GensType.None;
        if (targetGens == GensType.None)
        {
            return false;
        }

        var members = requester.Party?.PartyList.OfType<Player>().ToList() ?? [requester];
        foreach (var member in members)
        {
            var memberGens = (await member.GetGensMemberAsync().ConfigureAwait(false))?.Gens ?? GensType.None;
            if (memberGens != GensType.None && memberGens != targetGens)
            {
                return true;
            }
        }

        return false;
    }

    /// <summary>
    /// Shows the reason of a denied party to both players, because the plugin is called for the request and for the answer.
    /// </summary>
    private static async ValueTask ShowToBothAsync(Player requester, Player target, string messageKey)
    {
        await requester.ShowLocalizedBlueMessageAsync(messageKey).ConfigureAwait(false);
        await target.ShowLocalizedBlueMessageAsync(messageKey).ConfigureAwait(false);
    }

    private static bool IsInBattleZone(Player player, GensConfiguration configuration)
    {
        return player.CurrentMap?.Definition is { } map && configuration.IsBattleZone(map.Number);
    }
}
