// <copyright file="BaseItemRegistrationStrategy.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.GameLogic.PlayerActions.Items.ItemRegistration;

using System;
using System.Linq;
using System.Threading.Tasks;
using MUnique.OpenMU.AttributeSystem;

/// <summary>
/// A base strategy for item registration that implements common registration logic.
/// </summary>
public abstract class BaseItemRegistrationStrategy : IItemRegistrationStrategy
{
    /// <inheritdoc />
    public short Key => this.NpcNumber;

    /// <inheritdoc />
    public abstract short NpcNumber { get; }

    /// <summary>
    /// Gets the target stat attribute which counts the current registered items.
    /// </summary>
    public abstract AttributeDefinition? TargetStat { get; }

    /// <summary>
    /// Gets the target stat attribute which counts the total historical registered items.
    /// </summary>
    public abstract AttributeDefinition? TargetTotalStat { get; }

    /// <inheritdoc />
    public abstract ValueTask OpenDialogAsync(Player player);

    /// <inheritdoc />
    public virtual async ValueTask RegisterAsync(Player player, PlugIns.ItemRegistration.NpcItemRegistrationRule rule)
    {
        var item = player.Inventory?.Items.FirstOrDefault(i => i.Definition?.Group == rule.AcceptedItemGroup && i.Definition?.Number == rule.AcceptedItemNumber);
        if (item is null)
        {
            await player.ShowBlueMessageAsync("You don't have the required item in your inventory.").ConfigureAwait(false);
            await this.OnMissingItemAsync(player).ConfigureAwait(false);
            return;
        }

        int requiredItemsCount = rule.RequiredItemsCount;
        if (requiredItemsCount <= 0 || this.TargetStat is null)
        {
            requiredItemsCount = 1;
        }

        int peekRegistered = (int)(player.SelectedCharacter?.Attributes.FirstOrDefault(a => a.Definition == this.TargetStat)?.Value
            ?? player.Attributes?[this.TargetStat] ?? 0) + 1;
        bool willCompleteRegistration = this.TargetStat is null || peekRegistered >= requiredItemsCount;

        if (willCompleteRegistration && rule.RewardZen > 0 && (long)player.Money + rule.RewardZen > player.GameContext.Configuration.MaximumInventoryMoney)
        {
            await player.ShowBlueMessageAsync("You have reached the maximum inventory money limit.").ConfigureAwait(false);
            await this.OnRegistrationCompletedAsync(player).ConfigureAwait(false);
            return;
        }

        await player.DestroyInventoryItemAsync(item).ConfigureAwait(false);

        int currentRegistered = 1;
        int totalRegistered = 1;

        StatAttribute? registeredStat = null;
        if (this.TargetStat != null)
        {
            registeredStat = this.EnsureStatExists(player, this.TargetStat);
            currentRegistered = (int)(registeredStat?.Value ?? player.Attributes?[this.TargetStat] ?? 0) + 1;

            if (registeredStat != null)
            {
                registeredStat.Value = currentRegistered;
            }
        }

        StatAttribute? totalStat = null;
        if (this.TargetTotalStat != null)
        {
            totalStat = this.EnsureStatExists(player, this.TargetTotalStat);
            totalRegistered = (int)(totalStat?.Value ?? player.Attributes?[this.TargetTotalStat] ?? 0) + 1;

            if (totalStat != null)
            {
                totalStat.Value = totalRegistered;
            }
        }

        if (player.Attributes != null)
        {
            if (this.TargetStat != null)
            {
                player.Attributes[this.TargetStat] = currentRegistered;
            }

            if (this.TargetTotalStat != null)
            {
                player.Attributes[this.TargetTotalStat] = totalRegistered;
            }
        }

        if (currentRegistered >= requiredItemsCount)
        {
            int remaining = currentRegistered - requiredItemsCount;
            if (registeredStat != null)
            {
                registeredStat.Value = remaining;
            }

            if (player.Attributes != null && this.TargetStat != null)
            {
                player.Attributes[this.TargetStat] = remaining;
            }

            int zenReward = rule.RewardZen;
            if (zenReward > 0)
            {
                player.TryAddMoney(zenReward);
                await player.InvokeViewPlugInAsync<Views.Inventory.IUpdateMoneyPlugIn>(
                    p => p.UpdateMoneyAsync()).ConfigureAwait(false);
            }

            if (rule.RewardDropItemGroup is { } dropGroup && Rand.NextRandomBool(dropGroup.Chance))
            {
                var droppedItem = player.GameContext.DropGenerator.GenerateItemDrop(dropGroup);
                if (droppedItem != null)
                {
                    var dropCoordinates = player.CurrentMap!.Terrain.GetRandomCoordinate(player.Position, 2);
                    var dropped = new DroppedItem(droppedItem, dropCoordinates, player.CurrentMap, player);
                    await player.CurrentMap.AddAsync(dropped).ConfigureAwait(false);
                }
            }

            await player.ShowBlueMessageAsync($"Registered {requiredItemsCount}/{requiredItemsCount} items! Reward claimed.").ConfigureAwait(false);
            await player.ShowBlueMessageAsync($"Total items registered all-time: {totalRegistered}.").ConfigureAwait(false);
        }
        else
        {
            await player.ShowBlueMessageAsync($"Registered {currentRegistered}/{requiredItemsCount} items.").ConfigureAwait(false);
        }

        await this.OnRegistrationCompletedAsync(player).ConfigureAwait(false);
    }

    /// <summary>
    /// Called when the player is missing the required item.
    /// Can be overridden to send specific response packets to the client.
    /// </summary>
    /// <param name="player">The player.</param>
    protected virtual ValueTask OnMissingItemAsync(Player player)
    {
        return ValueTask.CompletedTask;
    }

    /// <summary>
    /// Called when a registration action is successfully completed (even if it didn't reach the target count yet).
    /// Can be overridden to send specific response packets to the client.
    /// </summary>
    /// <param name="player">The player.</param>
    protected virtual ValueTask OnRegistrationCompletedAsync(Player player)
    {
        return ValueTask.CompletedTask;
    }

    private StatAttribute? EnsureStatExists(Player player, AttributeDefinition statDefinition)
    {
        if (player.SelectedCharacter == null)
        {
            return null;
        }

        var trackedDefinition = player.GameContext.Configuration.Attributes?.FirstOrDefault(a => a.Id == statDefinition.Id) ?? statDefinition;
        var stat = player.SelectedCharacter.Attributes.FirstOrDefault(a => a.Definition == trackedDefinition || (a.Definition != null && a.Definition.Id == statDefinition.Id));

        if (stat == null)
        {
            stat = player.PersistenceContext.CreateNew<StatAttribute>(trackedDefinition, 0);
            player.SelectedCharacter.Attributes.Add(stat);
        }

        return stat;
    }
}
