// <copyright file="BaseItemCraftingHandler.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.GameLogic.PlayerActions.Items;

using MUnique.OpenMU.DataModel.Configuration.ItemCrafting;
using MUnique.OpenMU.GameLogic.PlugIns;
using MUnique.OpenMU.GameLogic.Views.Inventory;
using MUnique.OpenMU.GameLogic.Views.NPC;

/// <summary>
/// An abstract item crafting handler which can be overwritten to handle specific crafting requirements.
/// </summary>
public abstract class BaseItemCraftingHandler : IItemCraftingHandler
{
    /// <inheritdoc/>
    public async ValueTask<(CraftingResult Result, Item? Item)> DoMixAsync(Player player, byte socketSlot)
    {
        using var loggerScope = player.Logger.BeginScope(this.GetType());
        if (player.TemporaryStorage is null)
        {
            return (CraftingResult.Failed, null);
        }

        if (this.TryGetRequiredItems(player, out var items, out var successRate) is { } error)
        {
            return (error, null);
        }

        player.Logger.LogInformation("Crafting success chance: {successRate} %", successRate);

        var price = this.GetPrice(successRate, items);
        if (!player.TryRemoveMoney(price))
        {
            return (CraftingResult.NotEnoughMoney, null);
        }

        await player.InvokeViewPlugInAsync<IUpdateMoneyPlugIn>(p => p.UpdateMoneyAsync()).ConfigureAwait(false);

        var success = Rand.NextRandomBool(successRate);
        if (success)
        {
            player.Logger.LogInformation("Crafting succeeded with success chance: {successRate} %", successRate);
            if (await this.DoTheMixAsync(items, player, socketSlot).ConfigureAwait(false) is { } item)
            {
                player.Logger.LogInformation("Crafted item: {item}", item);
                return (CraftingResult.Success, item);
            }

            player.Logger.LogInformation("Crafting handler failed to mix the items.");
            return (CraftingResult.Failed, null);
        }

        player.Logger.LogInformation("Crafting failed with success chance: {successRate} %", successRate);
        foreach (var i in items)
        {
            await this.RequiredItemChangeAsync(player, i, false).ConfigureAwait(false);
        }

        return (CraftingResult.Failed, null);
    }

    /// <summary>
    /// Gets the price based on the success rate and the required items.
    /// </summary>
    /// <param name="successRate">The success rate.</param>
    /// <param name="requiredItems">The required items.</param>
    /// <returns>The calculated price of the crafting.</returns>
    protected abstract int GetPrice(byte successRate, IList<CraftingRequiredItemLink> requiredItems);

    /// <summary>
    /// Tries to get the required items for this crafting.
    /// If they can't be get or something is wrong, a <see cref="CraftingResult"/> with the
    /// corresponding error is returned. Otherwise, it's <c>null</c>.
    /// </summary>
    /// <param name="player">The player.</param>
    /// <param name="items">The items.</param>
    /// <param name="successRateByItems">The success rate by items.</param>
    /// <returns><c>null</c>, if the required items could be get; Otherwise, the corresponding error is returned.</returns>
    protected abstract CraftingResult? TryGetRequiredItems(Player player, out IList<CraftingRequiredItemLink> items, out byte successRateByItems);

    /// <summary>
    /// Creates the result items or modifies referenced required items as a result.
    /// </summary>
    /// <param name="requiredItems">The required items.</param>
    /// <param name="player">The player.</param>
    /// <param name="socketSlot">The slot of the socket.</param>
    /// <returns>The created or modified items.</returns>
    protected abstract ValueTask<List<Item>> CreateOrModifyResultItemsAsync(IList<CraftingRequiredItemLink> requiredItems, Player player, byte socketSlot);

    /// <summary>
    /// Performs the crafting with the specified items.
    /// </summary>
    /// <param name="requiredItems">The required items.</param>
    /// <param name="player">The player.</param>
    /// <param name="socketSlot">The slot of the socket.</param>
    /// <returns>
    /// The created or modified item. If there are multiple, only the last one is returned.
    /// </returns>
    private async ValueTask<Item?> DoTheMixAsync(IList<CraftingRequiredItemLink> requiredItems, Player player, byte socketSlot)
    {
        foreach (var requiredItemLink in requiredItems)
        {
            await this.RequiredItemChangeAsync(player, requiredItemLink, true).ConfigureAwait(false);
        }

        var resultItems = await this.CreateOrModifyResultItemsAsync(requiredItems, player, socketSlot).ConfigureAwait(false);
        return resultItems.LastOrDefault();
    }

    /// <summary>
    /// Changes the <see cref="CraftingRequiredItemLink.Items"/> depending on the success.
    /// </summary>
    /// <param name="player">The player.</param>
    /// <param name="itemLink">The item link.</param>
    /// <param name="success"><c>true</c>, if the crafting was successful; Otherwise, <c>false</c>.</param>
    private async ValueTask RequiredItemChangeAsync(Player player, CraftingRequiredItemLink itemLink, bool success)
    {
        var mixResult = success ? itemLink.ItemRequirement.SuccessResult : itemLink.ItemRequirement.FailResult;

        switch (mixResult)
        {
            case MixResult.Disappear:
                var point = player.GameContext.PlugInManager.GetPlugInPoint<IItemDestroyedPlugIn>();
                foreach (var item in itemLink.Items)
                {
                    player.Logger.LogDebug("Item {0} is getting destroyed.", item);
                    await player.TemporaryStorage!.RemoveItemAsync(item).ConfigureAwait(false);
                    await player.PersistenceContext.DeleteAsync(item).ConfigureAwait(false);
                    point?.ItemDestroyed(item);
                }

                break;
            case MixResult.DowngradedRandom:
                itemLink.Items.ForEach(item =>
                {
                    var previousLevel = item.Level;
                    item.Level = (byte)Rand.NextInt(0, item.Level);
                    player.Logger.LogDebug("Item {0} was downgraded from {1} to {2}.", item, previousLevel, item.Level);
                });

                break;
            case MixResult.DowngradedTo0:
                itemLink.Items.ForEach(item =>
                {
                    player.Logger.LogDebug("Item {0} is getting downgraded to level 0.", item);
                    item.Level = 0;
                });

                break;
            default:
                if (player.Logger.IsEnabled(LogLevel.Debug))
                {
                    itemLink.Items.ForEach(item => player.Logger.LogDebug("Item {0} stays as-is.", item));
                }

                // The item stays as is.
                break;
        }
    }
}