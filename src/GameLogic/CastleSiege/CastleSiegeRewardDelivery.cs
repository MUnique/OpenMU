// <copyright file="CastleSiegeRewardDelivery.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.GameLogic.CastleSiege;

using MUnique.OpenMU.DataModel.Configuration.Items;
using MUnique.OpenMU.DataModel.Entities;
using MUnique.OpenMU.GameLogic.Views.Inventory;
using MUnique.OpenMU.Persistence;

/// <summary>
/// Creates participant reward items and queues rewards which cannot be delivered immediately.
/// </summary>
internal static class CastleSiegeRewardDelivery
{
    /// <summary>
    /// Tries to add a configured reward item to a player's inventory.
    /// </summary>
    /// <param name="player">The rewarded player.</param>
    /// <param name="rewardDefinition">The reward item definition.</param>
    /// <returns><c>true</c> when the item was added; otherwise, <c>false</c>.</returns>
    internal static async ValueTask<bool> TryAddToInventoryAsync(Player player, ItemDefinition rewardDefinition)
    {
        if (player.Inventory is null)
        {
            return false;
        }

        var item = player.PersistenceContext.CreateNew<Item>();
        item.Definition = rewardDefinition;
        item.Durability = item.IsStackable() ? 1 : rewardDefinition.Durability;
        if (await player.Inventory.AddItemAsync(item).ConfigureAwait(false))
        {
            await player.InvokeViewPlugInAsync<IItemAppearPlugIn>(
                    plugIn => plugIn.ItemAppearAsync(item))
                .ConfigureAwait(false);
            return true;
        }

        await player.PersistenceContext.DeleteAsync(item).ConfigureAwait(false);
        return false;
    }

    /// <summary>
    /// Queues a reward for delivery when the character next enters the game.
    /// </summary>
    /// <param name="gameContext">The game context.</param>
    /// <param name="characterIds">The persistent character identifiers.</param>
    /// <param name="rewardDefinition">The reward item definition.</param>
    /// <returns>A task that represents the asynchronous queue operation.</returns>
    internal static async ValueTask QueueAsync(
        IGameContext gameContext,
        IReadOnlyCollection<Guid> characterIds,
        ItemDefinition rewardDefinition)
    {
        if (characterIds.Count == 0)
        {
            return;
        }

        using var persistenceContext = gameContext.PersistenceContextProvider.CreateNewTypedContext(
            typeof(CastleSiegePendingReward),
            false,
            gameContext.Configuration);
        foreach (var characterId in characterIds)
        {
            var pendingReward = persistenceContext.CreateNew<CastleSiegePendingReward>();
            pendingReward.CharacterId = characterId;
            pendingReward.ItemDefinitionId = rewardDefinition.GetId();
        }

        await persistenceContext.SaveChangesAsync().ConfigureAwait(false);
    }
}
