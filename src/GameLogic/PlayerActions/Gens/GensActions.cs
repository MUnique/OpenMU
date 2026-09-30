// <copyright file="GensActions.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.GameLogic.PlayerActions.Gens;

using MUnique.OpenMU.GameLogic.Gens;
using MUnique.OpenMU.GameLogic.Views.Gens;
using MUnique.OpenMU.Interfaces;

/// <summary>
/// The actions of a player to join and leave a gens at the gens npcs.
/// </summary>
public class GensActions
{
    /// <summary>
    /// Joins the player to the requested gens, at the npc of the gens.
    /// </summary>
    /// <param name="player">The player.</param>
    /// <param name="gens">The requested gens.</param>
    public async ValueTask JoinAsync(Player player, GensType gens)
    {
        if (GensFeaturePlugIn.GetConfiguration(player.GameContext) is not { } configuration)
        {
            return;
        }

        if (gens == GensType.None || GetGensOfOpenedNpc(player, configuration) != gens)
        {
            player.Logger.LogWarning("Player {player} requested to join the gens {gens}, but doesn't talk to its npc.", player, gens);
            return;
        }

        var result = CheckJoinRequirements(player, configuration);
        if (result == GensJoinResult.Success)
        {
            await player.RunPersistenceExclusiveAsync(async () =>
            {
                var member = player.GensMember;
                if (member is null)
                {
                    member = player.PersistenceContext.CreateNew<GensMember>();
                    member.CharacterId = player.SelectedCharacter!.Id;
                    player.GensMember = member;
                }

                member.Gens = gens;
                member.Contribution = configuration.StartingContribution;
                member.Rank = configuration.StartingRank;
                member.RankingPosition = 0;
                member.JoinedAt = DateTime.UtcNow;
                await player.SaveProgressAsync().ConfigureAwait(false);
            }).ConfigureAwait(false);

            player.Logger.LogInformation("Player {player} joined the gens {gens}.", player, gens);
        }

        await player.InvokeViewPlugInAsync<IGensViewPlugIn>(p => p.ShowJoinResultAsync(result, gens)).ConfigureAwait(false);
        if (result == GensJoinResult.Success)
        {
            await ShowChangedGensAsync(player).ConfigureAwait(false);
        }
    }

    /// <summary>
    /// Removes the player from its gens, at the npc of the gens.
    /// </summary>
    /// <param name="player">The player.</param>
    public async ValueTask LeaveAsync(Player player)
    {
        if (GensFeaturePlugIn.GetConfiguration(player.GameContext) is not { } configuration)
        {
            return;
        }

        var npcGens = GetGensOfOpenedNpc(player, configuration);
        if (npcGens == GensType.None)
        {
            player.Logger.LogWarning("Player {player} requested to leave the gens, but doesn't talk to a gens npc.", player);
            return;
        }

        var result = CheckLeaveRequirements(player, npcGens);
        if (result == GensLeaveResult.Success)
        {
            var gens = player.GensMember!.Gens;
            await player.RunPersistenceExclusiveAsync(async () =>
            {
                var member = player.GensMember!;
                member.Gens = GensType.None;
                member.Contribution = 0;
                member.Rank = 0;
                member.RankingPosition = 0;
                member.LeftAt = DateTime.UtcNow;
                await player.SaveProgressAsync().ConfigureAwait(false);
            }).ConfigureAwait(false);

            player.Logger.LogInformation("Player {player} left the gens {gens}.", player, gens);
        }

        await player.InvokeViewPlugInAsync<IGensViewPlugIn>(p => p.ShowLeaveResultAsync(result)).ConfigureAwait(false);
        if (result == GensLeaveResult.Success)
        {
            await ShowChangedGensAsync(player).ConfigureAwait(false);
        }
    }

    /// <summary>
    /// Shows the gens info of the player, e.g. when it opens the gens info window.
    /// </summary>
    /// <param name="player">The player.</param>
    public async ValueTask ShowInfoAsync(Player player)
    {
        if (GensFeaturePlugIn.GetConfiguration(player.GameContext) is null)
        {
            return;
        }

        await player.InvokeViewPlugInAsync<IGensViewPlugIn>(p => p.ShowGensInfoAsync()).ConfigureAwait(false);
    }

    private static GensType GetGensOfOpenedNpc(Player player, GensConfiguration configuration)
    {
        return player.OpenedNpc?.Definition is { } npcDefinition
            ? configuration.GetGensOfNpc(npcDefinition.Number)
            : GensType.None;
    }

    private static GensJoinResult CheckJoinRequirements(Player player, GensConfiguration configuration)
    {
        if (player.GensMember is { } member)
        {
            if (member.Gens != GensType.None)
            {
                return GensJoinResult.AlreadyJoined;
            }

            if (member.LeftAt + configuration.RejoinWaitTime > DateTime.UtcNow)
            {
                return GensJoinResult.LeftRecently;
            }
        }

        if (player.Level < configuration.MinimumLevel)
        {
            return GensJoinResult.LevelTooLow;
        }

        if (player.Party is not null)
        {
            return GensJoinResult.InParty;
        }

        if (player.GuildStatus is { } guildStatus)
        {
            return guildStatus.Position == GuildPosition.GuildMaster
                ? GensJoinResult.GuildMaster
                : GensJoinResult.GuildMember;
        }

        return GensJoinResult.Success;
    }

    private static GensLeaveResult CheckLeaveRequirements(Player player, GensType npcGens)
    {
        if (player.GensMember is not { Gens: not GensType.None } member)
        {
            return GensLeaveResult.NotJoined;
        }

        if (member.Gens != npcGens)
        {
            return GensLeaveResult.DifferentGensNpc;
        }

        if (player.GuildStatus?.Position == GuildPosition.GuildMaster)
        {
            return GensLeaveResult.GuildMaster;
        }

        return GensLeaveResult.Success;
    }

    private static async ValueTask ShowChangedGensAsync(Player player)
    {
        await player.InvokeViewPlugInAsync<IGensViewPlugIn>(p => p.ShowGensInfoAsync()).ConfigureAwait(false);
        await player.ForEachWorldObserverAsync<IAssignPlayersToGensPlugIn>(p => p.AssignPlayersToGensAsync([player]), false).ConfigureAwait(false);
    }
}
