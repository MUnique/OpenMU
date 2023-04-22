﻿// <copyright file="SimpleItemCraftingHandler.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.GameLogic.PlayerActions.Items;

using MUnique.OpenMU.DataModel.Configuration.ItemCrafting;
using MUnique.OpenMU.DataModel.Configuration.Items;
using MUnique.OpenMU.GameLogic.Views.NPC;

/// <summary>
/// The simple item crafting handler which can be configured to handle the most crafting requirements.
/// </summary>
public class SimpleItemCraftingHandler : BaseItemCraftingHandler
{
    private readonly ItemPriceCalculator _priceCalculator = new ();

    private readonly SimpleCraftingSettings _settings;

    /// <summary>
    /// Initializes a new instance of the <see cref="SimpleItemCraftingHandler"/> class.
    /// </summary>
    /// <param name="settings">The settings.</param>
    public SimpleItemCraftingHandler(SimpleCraftingSettings settings)
    {
        this._settings = settings ?? throw new ArgumentNullException(nameof(settings));
    }

    /// <inheritdoc />
    protected override int GetPrice(byte successRate, IList<CraftingRequiredItemLink> requiredItems)
    {
        return this._settings.Money + (this._settings.MoneyPerFinalSuccessPercentage * successRate);
    }

    /// <summary>
    /// Determines whether the actual item matches with the required item definition.
    /// </summary>
    /// <param name="item">The item.</param>
    /// <param name="requiredItem">The required item.</param>
    /// <returns><c>true</c>, if the actual item matches with the required item definition.</returns>
    protected virtual bool RequiredItemMatches(Item item, ItemCraftingRequiredItem requiredItem)
    {
        item.ThrowNotInitializedProperty(item.Definition is null, nameof(item.Definition));

        return (!requiredItem.PossibleItems.Any() || requiredItem.PossibleItems.Contains(item.Definition))
               && item.Level >= requiredItem.MinimumItemLevel
               && item.Level <= requiredItem.MaximumItemLevel
               && requiredItem.RequiredItemOptions.All(r => item.ItemOptions.Any(o => o.ItemOption!.OptionType == r));
    }

    /// <inheritdoc />
    protected override CraftingResult? TryGetRequiredItems(Player player, out IList<CraftingRequiredItemLink> items, out byte successRate)
    {
        successRate = 0;
        int rate = this._settings.SuccessPercent;
        items = new List<CraftingRequiredItemLink>(this._settings.RequiredItems.Count);
        var storage = player.TemporaryStorage?.Items.ToList() ?? new List<Item>();
        foreach (var requiredItem in this._settings.RequiredItems.OrderByDescending(i => i.MinimumAmount))
        {
            var foundItems = storage.Where(item => this.RequiredItemMatches(item, requiredItem)).ToList();
            var itemCount = foundItems.Sum(i => i.IsStackable() ? i.Durability : 1);
            if (itemCount < requiredItem.MinimumAmount)
            {
                player.Logger.LogWarning("LackingMixItems: Suspicious action for player with name: {0}, could be hack attempt. Missing item(s): {1}", player.Name, requiredItem);
                return CraftingResult.LackingMixItems;
            }

            if (itemCount > requiredItem.MaximumAmount && requiredItem.MaximumAmount > 0)
            {
                player.Logger.LogWarning("TooManyItems: Suspicious action for player with name: {0}, could be hack attempt. ItemCount: {1}, Required: {2}", player.Name, itemCount, requiredItem);
                return CraftingResult.TooManyItems;
            }

            rate += (byte)(requiredItem.AddPercentage * (itemCount - requiredItem.MinimumAmount));
            if (requiredItem.NpcPriceDivisor > 0)
            {
                rate += (byte)(foundItems.Sum(this._priceCalculator.CalculateBuyingPrice) /
                                      requiredItem.NpcPriceDivisor);
            }

            foreach (var item in foundItems)
            {
                if (this._settings.SuccessPercentageAdditionForLuck != default
                    && item.ItemOptions.Any(o => o.ItemOption?.OptionType == ItemOptionTypes.Luck))
                {
                    rate = (byte)(rate + this._settings.SuccessPercentageAdditionForLuck);
                }

                if (this._settings.SuccessPercentageAdditionForExcellentItem != default
                    && item.IsExcellent())
                {
                    rate = (byte)(rate + this._settings.SuccessPercentageAdditionForExcellentItem);
                }

                if (this._settings.SuccessPercentageAdditionForAncientItem != default
                    && item.IsAncient())
                {
                    rate = (byte)(rate + this._settings.SuccessPercentageAdditionForAncientItem);
                }

                if (this._settings.SuccessPercentageAdditionForSocketItem != default && item.SocketCount > 0)
                {
                    rate = (byte)(rate + this._settings.SuccessPercentageAdditionForSocketItem);
                }
            }

            foundItems.ForEach(i => storage.Remove(i));
            items.Add(new CraftingRequiredItemLink(foundItems, requiredItem));
        }

        // The list of unprocessed items must be empty now; otherwise, something is wrong.
        if (storage.Any())
        {
            return CraftingResult.IncorrectMixItems;
        }

        if (this._settings.MaximumSuccessPercent > 0)
        {
            rate = Math.Min(this._settings.MaximumSuccessPercent, rate);
        }

        successRate = (byte)Math.Min(100, rate);

        return default;
    }

    /// <inheritdoc />
    protected override async ValueTask<List<Item>> CreateOrModifyResultItemsAsync(IList<CraftingRequiredItemLink> requiredItems, Player player, byte socketSlot)
    {
        var resultItems = this._settings.ResultItemSelect == ResultItemSelection.All
            ? this._settings.ResultItems
            : this._settings.ResultItems.SelectRandom().GetAsEnumerable();

        var resultList = new List<Item>();
        foreach (var craftingResultItem in resultItems)
        {
            if (craftingResultItem.Reference > 0
                && requiredItems.FirstOrDefault(i => i.ItemRequirement.Reference == craftingResultItem.Reference) is { } referencedItem)
            {
                foreach (var item in referencedItem.Items)
                {
                    item.Level += craftingResultItem.AddLevel;
                    resultList.Add(item);
                }

                continue;
            }

            if (craftingResultItem.ItemDefinition is null)
            {
                // Should never happen
                player.Logger.LogWarning($"CraftingResultItem has no {nameof(ItemCraftingResultItem.Reference)} and no {nameof(ItemCraftingResultItem.ItemDefinition)}. It's ignored.");
                continue;
            }

            resultList.AddRange(await this.CreateResultItemsAsync(player, requiredItems, craftingResultItem).ConfigureAwait(false));
        }

        return resultList;
    }

    private async ValueTask<List<Item>> CreateResultItemsAsync(Player player, IList<CraftingRequiredItemLink> referencedItems, ItemCraftingResultItem craftingResultItem)
    {
        int resultItemCount = this._settings.MultipleAllowed
            ? referencedItems.FirstOrDefault(r => r.ItemRequirement.Reference > 0)?.Items.Count() ?? 1
            : 1;

        var resultList = new List<Item>(resultItemCount);
        for (int i = 0; i < resultItemCount; i++)
        {
            // Create new Item
            var resultItem = player.PersistenceContext.CreateNew<Item>();
            resultItem.Definition = craftingResultItem.ItemDefinition ?? throw Error.NotInitializedProperty(craftingResultItem, nameof(craftingResultItem.ItemDefinition));
            resultItem.Level = (byte)Rand.NextInt(
                craftingResultItem.RandomMinimumLevel,
                craftingResultItem.RandomMaximumLevel + 1);
            resultItem.Durability =
                craftingResultItem.Durability ?? resultItem.GetMaximumDurabilityOfOnePiece();

            this.AddRandomLuckOption(resultItem, player);
            this.AddRandomExcellentOptions(resultItem, player);
            if (!resultItem.HasSkill
                && this._settings.ResultItemSkillChance > 0
                && Rand.NextRandomBool(this._settings.ResultItemSkillChance)
                && resultItem.Definition!.Skill is { })
            {
                resultItem.HasSkill = true;
            }

            await player.TemporaryStorage!.AddItemAsync(resultItem).ConfigureAwait(false);
            resultList.Add(resultItem);
        }

        return resultList;
    }

    private void AddRandomLuckOption(Item resultItem, Player player)
    {
        if (this._settings.ResultItemLuckOptionChance > 0
            && Rand.NextRandomBool(this._settings.ResultItemLuckOptionChance)
            && resultItem.Definition!.PossibleItemOptions
                    .FirstOrDefault(o => o.PossibleOptions.Any(po => po.OptionType == ItemOptionTypes.Luck))
                is { } luck)
        {
            var luckOption = player.PersistenceContext.CreateNew<ItemOptionLink>();
            luckOption.ItemOption = luck.PossibleOptions.First();
            resultItem.ItemOptions.Add(luckOption);
        }
    }

    private void AddRandomExcellentOptions(Item resultItem, Player player)
    {
        if (this._settings.ResultItemExcellentOptionChance > 0
            && resultItem.Definition!.PossibleItemOptions.FirstOrDefault(o =>
                    o.PossibleOptions.Any(p => p.OptionType == ItemOptionTypes.Excellent || p.OptionType == ItemOptionTypes.Wing))
                is { } optionDefinition)
        {
            for (int j = 0;
                 j < optionDefinition.MaximumOptionsPerItem && Rand.NextRandomBool(this._settings.ResultItemExcellentOptionChance);
                 j++)
            {
                var link = player.PersistenceContext.CreateNew<ItemOptionLink>();
                link.ItemOption = optionDefinition.PossibleOptions
                    .Except(resultItem.ItemOptions.Select(io => io.ItemOption)).SelectRandom();
                resultItem.ItemOptions.Add(link);
                if (resultItem.Definition.Skill != null)
                {
                    // Excellent items always have skill.
                    resultItem.HasSkill = true;
                }
            }
        }
    }
}