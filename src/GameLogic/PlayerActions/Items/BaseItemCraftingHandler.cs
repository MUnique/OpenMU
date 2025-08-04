// <copyright file="BaseItemCraftingHandler.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.GameLogic.PlayerActions.Items;

using MUnique.OpenMU.DataModel.Configuration.ItemCrafting;
using MUnique.OpenMU.DataModel.Configuration.Items;
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
            if (await this.DoTheMixAsync(items, player, socketSlot, successRate).ConfigureAwait(false) is { } item)
            {
                player.Logger.LogInformation("Crafted item: {item}", item);
                player.BackupInventory = null;

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

    /// <inheritdoc/>
    public abstract CraftingResult? TryGetRequiredItems(Player player, out IList<CraftingRequiredItemLink> items, out byte successRateByItems);

    /// <summary>
    /// Gets the price based on the success rate and the required items.
    /// </summary>
    /// <param name="successRate">The success rate.</param>
    /// <param name="requiredItems">The required items.</param>
    /// <returns>The calculated price of the crafting.</returns>
    protected abstract int GetPrice(byte successRate, IList<CraftingRequiredItemLink> requiredItems);

    /// <summary>
    /// Creates the result items or modifies referenced required items as a result.
    /// </summary>
    /// <param name="requiredItems">The required items.</param>
    /// <param name="player">The player.</param>
    /// <param name="socketSlot">The slot of the socket.</param>
    /// <param name="successRate">The success rate of the combination.</param>
    /// <returns>The created or modified items.</returns>
    protected abstract ValueTask<List<Item>> CreateOrModifyResultItemsAsync(IList<CraftingRequiredItemLink> requiredItems, Player player, byte socketSlot, byte successRate);

    /// <summary>
    /// Performs the crafting with the specified items.
    /// </summary>
    /// <param name="requiredItems">The required items.</param>
    /// <param name="player">The player.</param>
    /// <param name="socketSlot">The slot of the socket.</param>
    /// <param name="successRate">The success rate of the combination.</param>
    /// <returns>
    /// The created or modified item. If there are multiple, only the last one is returned.
    /// </returns>
    private async ValueTask<Item?> DoTheMixAsync(IList<CraftingRequiredItemLink> requiredItems, Player player, byte socketSlot, byte successRate)
    {
        foreach (var requiredItemLink in requiredItems)
        {
            await this.RequiredItemChangeAsync(player, requiredItemLink, true).ConfigureAwait(false);
        }

        var resultItems = await this.CreateOrModifyResultItemsAsync(requiredItems, player, socketSlot, successRate).ConfigureAwait(false);
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
            case MixResult.ChaosWeaponAndFirstWingsDowngradedRandom:
                itemLink.Items.ForEach(item =>
                {
                    var previousLevel = item.Level;
                    var hadSkill = item.HasSkill;
                    var optionLowered = false;
                    var previousMaxDurability = item.GetMaximumDurabilityOfOnePiece();

                    item.Level = (byte)Rand.NextInt(0, previousLevel);
                    if (item.HasSkill && !item.IsExcellent() && Rand.NextRandomBool())
                    {
                        item.HasSkill = false;
                    }

                    if (item.ItemOptions.FirstOrDefault(o => o.ItemOption?.OptionType == ItemOptionTypes.Option) is { } optionLink && Rand.NextRandomBool())
                    {
                        optionLowered = true;
                        if (optionLink.Level > 1)
                        {
                            optionLink.Level--;
                        }
                        else
                        {
                            item.ItemOptions.Remove(optionLink);
                        }
                    }

                    item.Durability = item.GetMaximumDurabilityOfOnePiece() * item.Durability / previousMaxDurability;
                    player.Logger.LogDebug(
                        "Item {0} was downgraded from level {1} to {2}. Skill removed: {3}. Item option lowered by 1 level: {4}.",
                        item,
                        previousLevel,
                        item.Level,
                        hadSkill && !item.HasSkill,
                        optionLowered);
                });

                break;
            case MixResult.ThirdWingsDowngradedRandom:
                itemLink.Items.ForEach(item =>
                {
                    var previousLevel = item.Level;
                    item.Level -= (byte)(Rand.NextRandomBool() ? 2 : 3);
                    if (item.ItemOptions.FirstOrDefault(o => o.ItemOption?.OptionType == ItemOptionTypes.Option) is { } optionLink)
                    {
                        item.ItemOptions.Remove(optionLink);
                    }

                    item.Durability = item.GetMaximumDurabilityOfOnePiece();
                    player.Logger.LogDebug("Item {0} was downgraded from level {1} to {2}. Item option was removed.", item, previousLevel, item.Level);
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