// <copyright file="SimpleItemCraftingHandler.cs" company="MUnique">
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
    private readonly ItemPriceCalculator _priceCalculator = new();

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

                if (this._settings.SuccessPercentageAdditionFor380Item != default
                    && item.Is380())
                {
                    rate = (byte)(rate + this._settings.SuccessPercentageAdditionFor380Item);
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

        if (this._settings.MinimumSuccessPercent > 0)
        {
            rate = Math.Max(this._settings.MinimumSuccessPercent, rate);
        }

        successRate = (byte)Math.Min(100, rate);

        return default;
    }

    /// <inheritdoc />
    protected override async ValueTask<List<Item>> CreateOrModifyResultItemsAsync(IList<CraftingRequiredItemLink> requiredItems, Player player, byte socketSlot, byte successRate)
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

            resultList.AddRange(await this.CreateResultItemsAsync(player, requiredItems, craftingResultItem, successRate).ConfigureAwait(false));
        }

        return resultList;
    }

    private async ValueTask<List<Item>> CreateResultItemsAsync(Player player, IList<CraftingRequiredItemLink> referencedItems, ItemCraftingResultItem craftingResultItem, byte successRate)
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
            resultItem.Level = resultItem.Definition.Group == ItemConstants.Fruits.Group && resultItem.Definition.Number == ItemConstants.Fruits.Number
                ? (byte)new List<int>() { 0, 1, 2, 3, 4 }.SelectWeightedRandom([30, 25, 20, 20, 5])
                : (byte)Rand.NextInt(craftingResultItem.RandomMinimumLevel, craftingResultItem.RandomMaximumLevel + 1);
            resultItem.Durability = craftingResultItem.Durability ?? resultItem.GetMaximumDurabilityOfOnePiece();

            this.AddRandomLuckOption(resultItem, player, successRate);
            this.AddRandomExcellentOptions(resultItem, player);
            this.AddRandomSkill(resultItem, successRate);
            if (this._settings.ResultItemRateDependentOptions || this._settings.ResultItemIs2ndWing || this._settings.ResultItemIs3rdWing)
            {
                this.AddRandomItemOption(resultItem, player, successRate);
            }

            await player.TemporaryStorage!.AddItemAsync(resultItem).ConfigureAwait(false);
            resultList.Add(resultItem);
        }

        return resultList;
    }

    private void AddRandomLuckOption(Item resultItem, Player player, byte successRate)
    {
        if (((this._settings.ResultItemLuckOptionChance > 0 && Rand.NextRandomBool(this._settings.ResultItemLuckOptionChance))
                || (this._settings.ResultItemRateDependentOptions && Rand.NextRandomBool((successRate / 5) + 4)))
            && resultItem.Definition!.PossibleItemOptions.FirstOrDefault(o =>
                    o.PossibleOptions.Any(po => po.OptionType == ItemOptionTypes.Luck))
                is { } luck)
        {
            var luckOption = player.PersistenceContext.CreateNew<ItemOptionLink>();
            luckOption.ItemOption = luck.PossibleOptions.First();
            resultItem.ItemOptions.Add(luckOption);
        }
    }

    private void AddRandomItemOption(Item resultItem, Player player, byte successRate)
    {
        if (resultItem.Definition!.PossibleItemOptions.Where(o => o.PossibleOptions.Any(p => p.OptionType == ItemOptionTypes.Option))
            is { } options && options.Any())
        {
            if (this._settings.ResultItemRateDependentOptions)
            {
                int i = Rand.NextInt(0, 3);
                if (Rand.NextRandomBool((successRate / 5) + (4 * (i + 1))))
                {
                    var link = player.PersistenceContext.CreateNew<ItemOptionLink>();
                    link.ItemOption = options.First().PossibleOptions.First();
                    link.Level = 3 - i;
                    resultItem.ItemOptions.Add(link);
                }
            }
            else if (this._settings.ResultItemIs2ndWing)
            {
                (int chance, int level) = Rand.NextInt(0, 3) switch
                {
                    0 => (20, 1),
                    1 => (10, 2),
                    _ => (4, 3),
                }; // From 300 created wings about 20+10+4=34 (~11%) will have item option

                if (Rand.NextRandomBool(chance))
                {
                    var link = player.PersistenceContext.CreateNew<ItemOptionLink>();
                    link.Level = level;
                    if (options.Count() > 1)
                    {
                        link.ItemOption = options.ElementAt(Rand.NextInt(0, 2)).PossibleOptions.First();
                    }
                    else
                    {
                        link.ItemOption = options.ElementAt(0).PossibleOptions.First(); // Cape of Lord
                    }

                    resultItem.ItemOptions.Add(link);
                }
            }
            else
            {
                // 3rd level wings
                (int chance1, int level) = Rand.NextInt(0, 4) switch
                {
                    0 => (0, 0),
                    1 => (12, 1),
                    2 => (6, 2),
                    _ => (3, 3),
                }; // From 400 created wings about 0+12+6+3=21 (~5%) will have item option

                if (Rand.NextRandomBool(chance1))
                {
                    var link = player.PersistenceContext.CreateNew<ItemOptionLink>();
                    link.Level = level;
                    (int chance2, int type) = Rand.NextInt(0, 2) switch
                    {
                        0 => (40, 1),
                        _ => (30, 2),
                    };

                    if (Rand.NextRandomBool(chance2))
                    {
                        link.ItemOption = options.ElementAt(type).PossibleOptions.First();  // Additional dmg (phys, wiz, curse) or defense
                    }
                    else
                    {
                        link.ItemOption = options.ElementAt(0).PossibleOptions.First(); // HP recovery %
                    }

                    resultItem.ItemOptions.Add(link);
                }
            }
        }
    }

    private void AddRandomExcellentOptions(Item resultItem, Player player)
    {
        if (resultItem.Definition!.PossibleItemOptions.FirstOrDefault(o =>
                    o.PossibleOptions.Any(p => p.OptionType == ItemOptionTypes.Excellent || p.OptionType == ItemOptionTypes.Wing))
                is { } optionDefinition)
        {
            if (this._settings.ResultItemExcellentOptionChance > 0)
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

                    if (resultItem.Definition.Group == 13 && resultItem.Definition.Number == 3 && Rand.NextRandomBool(13)) // 0.2*0.66 = 13.2%
                    {
                        // For Dinorant there is a second rollout for an additional bonus option to the first.
                        var bonusLink = player.PersistenceContext.CreateNew<ItemOptionLink>();
                        bonusLink.ItemOption = optionDefinition.PossibleOptions
                            .Except(resultItem.ItemOptions.Select(io => io.ItemOption)).SelectRandom();
                        resultItem.ItemOptions.Add(bonusLink);
                    }
                }
            }
            else if (this._settings.ResultItemIs3rdWing)
            {
                (int chance, int type) = Rand.NextInt(0, 4) switch
                {
                    0 => (4, 0),   // Ignore def
                    1 => (2, 1),   // 5% full reflect
                    2 => (7, 2),   // 5% HP restore
                    _ => (7, 3),   // 5% mana restore
                };  // From 400 created wings about 4+2+7+7=20 (5%) will have exc option

                if (Rand.NextRandomBool(chance))
                {
                    var link = player.PersistenceContext.CreateNew<ItemOptionLink>();
                    link.ItemOption = optionDefinition.PossibleOptions.ElementAt(type);
                    resultItem.ItemOptions.Add(link);
                }
            }
            else
            {
                // Nothing to do here...
            }
        }
    }

    private void AddRandomSkill(Item resultItem, byte successRate)
    {
        if (!resultItem.HasSkill
            && ((this._settings.ResultItemSkillChance > 0 && Rand.NextRandomBool(this._settings.ResultItemSkillChance))
                || (this._settings.ResultItemRateDependentOptions && Rand.NextRandomBool((successRate / 5) + 6)))
            && resultItem.Definition!.Skill is { })
        {
            resultItem.HasSkill = true;
        }
    }
}