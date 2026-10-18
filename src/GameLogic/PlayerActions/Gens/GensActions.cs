// <copyright file="GensActions.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.GameLogic.PlayerActions.Gens;

using MUnique.OpenMU.DataModel.Configuration.Items;
using MUnique.OpenMU.GameLogic.Gens;
using MUnique.OpenMU.GameLogic.Views.Gens;
using MUnique.OpenMU.GameLogic.Views.Inventory;
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

        if (player.SelectedCharacter is not { } character)
        {
            return;
        }

        // The requirements are checked in the same exclusive operation in which the membership is changed,
        // so that two requests can't both pass the check.
        var result = await player.RunPersistenceExclusiveAsync(async () =>
        {
            var member = await player.GetGensMemberAsync().ConfigureAwait(false);
            var requirementsResult = CheckJoinRequirements(player, member, configuration);
            if (requirementsResult != GensJoinResult.Success)
            {
                return requirementsResult;
            }

            if (member is null)
            {
                member = player.PersistenceContext.CreateNew<GensMember>();
                member.CharacterId = character.Id;
                player.GensMember = member;
            }

            member.Gens = gens;
            member.Contribution = configuration.StartingContribution;
            member.Rank = configuration.StartingRank;
            member.RankingPosition = 0;
            member.JoinedAt = DateTime.UtcNow;
            await player.SaveProgressAsync().ConfigureAwait(false);
            return GensJoinResult.Success;
        }).ConfigureAwait(false);

        if (result == GensJoinResult.Success)
        {
            player.Logger.LogInformation("Player {player} joined the gens {gens}.", player, gens);
        }

        await player.InvokeViewPlugInAsync<IGensViewPlugIn>(p => p.ShowJoinResultAsync(result, gens)).ConfigureAwait(false);
        if (result == GensJoinResult.Success)
        {
            await player.ShowChangedGensAsync().ConfigureAwait(false);
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

        if (player.SelectedCharacter is null)
        {
            return;
        }

        var leftGens = GensType.None;
        var result = await player.RunPersistenceExclusiveAsync(async () =>
        {
            var member = await player.GetGensMemberAsync().ConfigureAwait(false);
            var requirementsResult = CheckLeaveRequirements(player, member, npcGens);
            if (requirementsResult != GensLeaveResult.Success)
            {
                return requirementsResult;
            }

            leftGens = member!.Gens;
            member.Gens = GensType.None;
            member.Contribution = 0;
            member.Rank = 0;
            member.RankingPosition = 0;
            member.LeftAt = DateTime.UtcNow;
            await player.SaveProgressAsync().ConfigureAwait(false);
            return GensLeaveResult.Success;
        }).ConfigureAwait(false);

        if (result == GensLeaveResult.Success)
        {
            player.Logger.LogInformation("Player {player} left the gens {gens}.", player, leftGens);
        }

        await player.InvokeViewPlugInAsync<IGensViewPlugIn>(p => p.ShowLeaveResultAsync(result)).ConfigureAwait(false);
        if (result == GensLeaveResult.Success)
        {
            await player.ShowChangedGensAsync().ConfigureAwait(false);
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

    /// <summary>
    /// Handles the request of the monthly gens ranking reward, at the npc of the gens.
    /// In the reward period of a month, a member gets the reward of its current rank once.
    /// The game client waits for the result, so it's always sent for a gens npc.
    /// </summary>
    /// <param name="player">The player.</param>
    /// <param name="gens">The gens of the npc, as requested by the game client.</param>
    public async ValueTask RequestRewardAsync(Player player, GensType gens)
    {
        if (GensFeaturePlugIn.GetConfiguration(player.GameContext) is not { } configuration)
        {
            return;
        }

        var npcGens = GetGensOfOpenedNpc(player, configuration);
        if (npcGens == GensType.None || npcGens != gens)
        {
            player.Logger.LogWarning("Player {player} requested the reward of the gens {gens}, but doesn't talk to its npc.", player, gens);
            return;
        }

        if (player.SelectedCharacter is null)
        {
            return;
        }

        // The checks and the claim are done in one exclusive operation, so that two requests can't both get the reward.
        IReadOnlyList<Item>? rewardItems = null;
        var result = await player.RunPersistenceExclusiveAsync(async () =>
        {
            var member = await player.GetGensMemberAsync().ConfigureAwait(false);
            var memberGens = member?.Gens ?? GensType.None;
            if (memberGens == GensType.None)
            {
                return GensRewardResult.NotJoined;
            }

            if (memberGens != npcGens)
            {
                return GensRewardResult.DifferentGensNpc;
            }

            var now = DateTime.UtcNow;
            if (now.Day < configuration.RewardStartDay || now.Day > configuration.RewardEndDay)
            {
                return GensRewardResult.OutsideRewardPeriod;
            }

            if (member!.RewardClaimedAt is { } claimedAt && claimedAt.Year == now.Year && claimedAt.Month == now.Month)
            {
                return GensRewardResult.AlreadyClaimed;
            }

            if (configuration.Rewards.FirstOrDefault(reward => reward.Rank == member.Rank) is not { Count: > 0 } reward
                || player.GameContext.Configuration.Items.FirstOrDefault(item => item.Group == reward.ItemGroup && item.Number == reward.ItemNumber) is not { } itemDefinition)
            {
                return GensRewardResult.NotEligible;
            }

            if (await TryAddRewardItemsAsync(player, itemDefinition, reward.Count).ConfigureAwait(false) is not { } items)
            {
                return GensRewardResult.InventoryFull;
            }

            // The claim is set before the items are shown, so that the reward can't be claimed again when showing them fails.
            member.RewardClaimedAt = now;
            if (!await player.SaveProgressAsync().ConfigureAwait(false))
            {
                player.Logger.LogWarning("The gens reward of player {player} couldn't be saved yet. It's saved with the next save of the player.", player);
            }

            rewardItems = items;
            return GensRewardResult.Success;
        }).ConfigureAwait(false);

        if (result == GensRewardResult.Success)
        {
            player.Logger.LogInformation("Player {player} claimed the gens reward.", player);
            foreach (var item in rewardItems!)
            {
                await player.InvokeViewPlugInAsync<IItemAppearPlugIn>(p => p.ItemAppearAsync(item)).ConfigureAwait(false);
            }
        }

        await player.InvokeViewPlugInAsync<IGensViewPlugIn>(p => p.ShowRewardResultAsync(result)).ConfigureAwait(false);
    }

    /// <summary>
    /// Adds the reward items to the inventory of the player. When not all fit into it, the added ones are removed again.
    /// </summary>
    /// <returns>The added items; <c>null</c>, if not all of them fit into the inventory.</returns>
    private static async ValueTask<IReadOnlyList<Item>?> TryAddRewardItemsAsync(Player player, ItemDefinition itemDefinition, int count)
    {
        if (player.Inventory is not { } inventory
            || inventory.FreeSlots.Count() < count * itemDefinition.Width * itemDefinition.Height)
        {
            return null;
        }

        var addedItems = new List<Item>(count);
        for (var i = 0; i < count; i++)
        {
            var item = player.PersistenceContext.CreateNew<Item>();
            item.Definition = itemDefinition;
            item.Durability = itemDefinition.Durability;
            if (!await inventory.AddItemAsync(item).ConfigureAwait(false))
            {
                await player.PersistenceContext.DeleteAsync(item).ConfigureAwait(false);
                foreach (var addedItem in addedItems)
                {
                    await inventory.RemoveItemAsync(addedItem).ConfigureAwait(false);
                    await player.PersistenceContext.DeleteAsync(addedItem).ConfigureAwait(false);
                }

                return null;
            }

            addedItems.Add(item);
        }

        return addedItems;
    }

    private static GensType GetGensOfOpenedNpc(Player player, GensConfiguration configuration)
    {
        return player.OpenedNpc?.Definition is { } npcDefinition
            ? configuration.GetGensOfNpc(npcDefinition.Number)
            : GensType.None;
    }

    private static GensJoinResult CheckJoinRequirements(Player player, GensMember? member, GensConfiguration configuration)
    {
        if (member is not null)
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

    private static GensLeaveResult CheckLeaveRequirements(Player player, GensMember? member, GensType npcGens)
    {
        if (member is not { Gens: not GensType.None })
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
}
