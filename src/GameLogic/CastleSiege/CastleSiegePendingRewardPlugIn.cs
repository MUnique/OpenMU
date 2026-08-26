// <copyright file="CastleSiegePendingRewardPlugIn.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.GameLogic.CastleSiege;

using System.Runtime.InteropServices;
using MUnique.OpenMU.GameLogic.PlugIns;
using MUnique.OpenMU.Persistence;
using MUnique.OpenMU.PlugIns;

/// <summary>
/// Delivers pending Castle Siege participant rewards when a character enters the game.
/// </summary>
[PlugIn]
[Display(Name = nameof(PlugInResources.CastleSiegePendingRewardPlugIn_Name), Description = nameof(PlugInResources.CastleSiegePendingRewardPlugIn_Description), ResourceType = typeof(PlugInResources))]
[Guid("EA4DB403-84F2-4E31-B4D4-441B3EADFCB6")]
public class CastleSiegePendingRewardPlugIn : IPlayerStateChangedPlugIn
{
    /// <inheritdoc />
    public async ValueTask PlayerStateChangedAsync(Player player, State previousState, State currentState)
    {
        if (previousState != PlayerState.CharacterSelection
            || currentState != PlayerState.EnteredWorld
            || player.SelectedCharacter is not { } character)
        {
            return;
        }

        var pendingRewards = await player.PersistenceContext
            .GetPendingCastleSiegeRewardsAsync(character.Id)
            .ConfigureAwait(false);
        var hasChanges = false;
        foreach (var pendingReward in pendingRewards)
        {
            var rewardDefinition = player.GameContext.Configuration.Items
                .FirstOrDefault(item => item.GetId() == pendingReward.ItemDefinitionId);
            if (rewardDefinition is null)
            {
                player.Logger.LogWarning(
                    "Discarding Castle Siege reward {RewardId} because item definition {ItemDefinitionId} is unavailable.",
                    pendingReward.Id,
                    pendingReward.ItemDefinitionId);
                await player.PersistenceContext.DeleteAsync(pendingReward).ConfigureAwait(false);
                hasChanges = true;
                continue;
            }

            if (!await CastleSiegeRewardDelivery
                    .TryAddToInventoryAsync(player, rewardDefinition)
                    .ConfigureAwait(false))
            {
                continue;
            }

            await player.PersistenceContext.DeleteAsync(pendingReward).ConfigureAwait(false);
            hasChanges = true;
        }

        if (hasChanges)
        {
            // The item creation and pending-row deletion are committed together by the player's context.
            await player.SaveProgressAsync().ConfigureAwait(false);
        }
    }
}
