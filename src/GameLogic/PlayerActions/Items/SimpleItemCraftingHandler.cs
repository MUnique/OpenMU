// <copyright file="SimpleItemCraftingHandler.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.GameLogic.PlayerActions.Items
{
    using System.Collections.Generic;
    using System.Linq;
    using MUnique.OpenMU.DataModel.Configuration.ItemCrafting;
    using MUnique.OpenMU.DataModel.Configuration.Items;
    using MUnique.OpenMU.DataModel.Entities;
    using MUnique.OpenMU.GameLogic.PlugIns;
    using MUnique.OpenMU.GameLogic.Views.Inventory;
    using MUnique.OpenMU.GameLogic.Views.NPC;

    /// <summary>
    /// The simple item crafting handler which can be configured to handle the most crafting requirements.
    /// </summary>
    public class SimpleItemCraftingHandler : IItemCraftingHandler
    {
        private static readonly log4net.ILog Log = log4net.LogManager.GetLogger(typeof(SimpleItemCraftingHandler));

        private readonly ItemPriceCalculator priceCalculator = new ItemPriceCalculator();

        private readonly SimpleCraftingSettings settings;

        /// <summary>
        /// Initializes a new instance of the <see cref="SimpleItemCraftingHandler"/> class.
        /// </summary>
        /// <param name="settings">The settings.</param>
        public SimpleItemCraftingHandler(SimpleCraftingSettings settings)
        {
            this.settings = settings;
        }

        /// <inheritdoc/>
        public (CraftingResult, Item) DoMix(Player player)
        {
            var successRate = this.settings.SuccessPercent;

            // Find all Required Items first
            var items = new List<CraftingRequiredItemLink>(this.settings.RequiredItems.Count);
            var storage = player.TemporaryStorage.Items.ToList();
            foreach (var requiredItem in this.settings.RequiredItems)
            {
                var foundItems = storage
                    .Where(item => requiredItem.ItemDefinition == null || item.Definition == requiredItem.ItemDefinition)
                    .Where(item => requiredItem.RequiredItemOptions.All(r => item.ItemOptions.Any(o => o.ItemOption.OptionType == r)))
                    .Where(item => item.Level >= requiredItem.MinimumItemLevel
                                   && item.Level <= requiredItem.MaximumItemLevel).ToList();

                if (foundItems.Count < requiredItem.MinimumAmount)
                {
                    Log.WarnFormat("LackingMixItems: Suspicious action for player with name: {0}, could be hack attempt.", player.Name);
                    return (CraftingResult.LackingMixItems, null);
                }

                if (foundItems.Count > requiredItem.MaximumAmount && requiredItem.MaximumAmount > 0)
                {
                    Log.WarnFormat("TooManyItems: Suspicious action for player with name: {0}, could be hack attempt.", player.Name);
                    return (CraftingResult.TooManyItems, null);
                }

                successRate += (byte)(requiredItem.AddPercentage * (foundItems.Count - requiredItem.MinimumAmount));
                if (requiredItem.NpcPriceDivisor > 0)
                {
                    successRate += (byte)(foundItems.Sum(this.priceCalculator.CalculateBuyingPrice) / requiredItem.NpcPriceDivisor);
                }

                foreach (var item in foundItems)
                {
                    if (this.settings.SuccessPercentageAdditionForLuck != default
                        && item.ItemOptions.Any(o => o.ItemOption.OptionType == ItemOptionTypes.Luck))
                    {
                        successRate = (byte)(successRate + this.settings.SuccessPercentageAdditionForLuck);
                    }

                    if (this.settings.SuccessPercentageAdditionForExcellentItem != default
                        && item.ItemOptions.Any(o => o.ItemOption.OptionType == ItemOptionTypes.Excellent))
                    {
                        successRate = (byte)(successRate + this.settings.SuccessPercentageAdditionForExcellentItem);
                    }

                    if (this.settings.SuccessPercentageAdditionForAncientItem != default
                        && item.ItemOptions.Any(o => o.ItemOption.OptionType == ItemOptionTypes.AncientBonus))
                    {
                        successRate = (byte)(successRate + this.settings.SuccessPercentageAdditionForAncientItem);
                    }

                    if (this.settings.SuccessPercentageAdditionForSocketItem != default && item.SocketCount > 0)
                    {
                        successRate = (byte)(successRate + this.settings.SuccessPercentageAdditionForSocketItem);
                    }
                }

                foundItems.ForEach(i => storage.Remove(i));
                items.Add(new CraftingRequiredItemLink(foundItems, requiredItem));
            }

            // The list of unprocessed items must be empty now; otherwise, something is wrong.
            if (storage.Any())
            {
                return (CraftingResult.IncorrectMixItems, null);
            }

            var price = this.settings.Money + (this.settings.MoneyPerFinalSuccessPercentage * successRate);
            if (!player.TryRemoveMoney(price))
            {
                return (CraftingResult.NotEnoughMoney, null);
            }

            player.ViewPlugIns.GetPlugIn<IUpdateMoneyPlugIn>()?.UpdateMoney();

            var success = Rand.NextRandomBool(successRate);
            if (success)
            {
                var item = this.DoTheMix(items, player);
                return (CraftingResult.Success, item);
            }

            items.ForEach(i => this.RequiredItemChange(player, i, false));
            return (CraftingResult.Failed, null);
        }

        private Item DoTheMix(IList<CraftingRequiredItemLink> items, Player player)
        {
            var referencedItems = new List<CraftingRequiredItemLink>();

            foreach (var requiredItemLink in items)
            {
                if (requiredItemLink.ItemRequirement.Reference != 0)
                {
                    referencedItems.Add(requiredItemLink);
                }

                this.RequiredItemChange(player, requiredItemLink, true);
            }

            var resultItems = this.settings.ResultItemSelect == ResultItemSelection.All
                ? this.settings.ResultItems
                : this.settings.ResultItems.SelectRandom().GetAsEnumerable();

            Item affectedItem = null;
            foreach (var craftingResultItem in resultItems)
            {
                if (craftingResultItem.Reference > 0
                    && referencedItems.FirstOrDefault(i => i.ItemRequirement.Reference == craftingResultItem.Reference) is { } referencedItem)
                {
                    foreach (var item in referencedItem.StoredItem)
                    {
                        item.Level += craftingResultItem.AddLevel;
                        affectedItem = item;
                    }

                    continue;
                }

                if (craftingResultItem.ItemDefinition == null)
                {
                    // Should never happen
                    Log.Warn($"CraftingResultItem has no {nameof(ItemCraftingResultItem.Reference)} and no {nameof(ItemCraftingResultItem.ItemDefinition)}. It's ignored.");
                    continue;
                }

                affectedItem = this.CreateResultItems(player, referencedItems, craftingResultItem);
            }

            return affectedItem;
        }

        private Item CreateResultItems(Player player, List<CraftingRequiredItemLink> referencedItems, ItemCraftingResultItem craftingResultItem)
        {
            Item affectedItem = null;
            int resultItemCount = referencedItems.FirstOrDefault()?.StoredItem.Count() ?? 1;
            for (int i = 0; i < resultItemCount; i++)
            {
                // Create new Item
                var resultItem = player.PersistenceContext.CreateNew<Item>();
                resultItem.Definition = craftingResultItem.ItemDefinition;
                resultItem.Level = (byte) Rand.NextInt(
                    craftingResultItem.RandomMinimumLevel,
                    craftingResultItem.RandomMaximumLevel + 1);
                resultItem.Durability =
                    craftingResultItem.Durability ?? resultItem.GetMaximumDurabilityOfOnePiece();

                this.AddRandomLuckOption(resultItem, player);
                this.AddRandomExcellentOptions(resultItem, player);
                if (!resultItem.HasSkill
                    && this.settings.ResultItemSkillChance > 0
                    && Rand.NextRandomBool(this.settings.ResultItemSkillChance)
                    && resultItem.Definition.Skill is { })
                {
                    resultItem.HasSkill = true;
                }

                player.TemporaryStorage.AddItem(resultItem);
                affectedItem = resultItem;
            }

            return affectedItem;
        }

        private void AddRandomLuckOption(Item resultItem, Player player)
        {
            if (this.settings.ResultItemLuckOptionChance > 0
                && Rand.NextRandomBool(this.settings.ResultItemLuckOptionChance)
                && resultItem.Definition.PossibleItemOptions
                        .FirstOrDefault(o => o.PossibleOptions.Any(po => po.OptionType == ItemOptionTypes.Luck))
                    is { } luck)
            {
                var luckOption = player.PersistenceContext.CreateNew<ItemOptionLink>();
                luckOption.ItemOption = luck.PossibleOptions.First();
                resultItem.ItemOptions.Add(luckOption);
                if (resultItem.Definition.Skill != null)
                {
                    // Excellent items always have skill.
                    resultItem.HasSkill = true;
                }
            }
        }

        private void AddRandomExcellentOptions(Item resultItem, Player player)
        {
            if (this.settings.ResultItemExcellentOptionChance > 0
                && resultItem.Definition.PossibleItemOptions.FirstOrDefault(o =>
                        o.PossibleOptions.Any(p => p.OptionType == ItemOptionTypes.Excellent))
                    is { } optionDefinition)
            {
                for (int j = 0;
                    j < optionDefinition.MaximumOptionsPerItem && Rand.NextRandomBool(this.settings.ResultItemExcellentOptionChance);
                    j++)
                {
                    var link = player.PersistenceContext.CreateNew<ItemOptionLink>();
                    link.ItemOption = optionDefinition.PossibleOptions
                        .Except(resultItem.ItemOptions.Select(io => io.ItemOption)).SelectRandom();
                    resultItem.ItemOptions.Add(link);
                }
            }
        }

        private void RequiredItemChange(Player player, CraftingRequiredItemLink itemLink, bool success)
        {
            var mixResult = success ? itemLink.ItemRequirement.SuccessResult : itemLink.ItemRequirement.FailResult;

            switch (mixResult)
            {
                case MixResult.Disappear:
                    var point = player.GameContext.PlugInManager.GetPlugInPoint<IItemDestroyedPlugIn>();
                    foreach (var item in itemLink.StoredItem)
                    {
                        Log.DebugFormat("Item {0} is getting destroyed.", item);
                        player.TemporaryStorage.RemoveItem(item);
                        player.PersistenceContext.Delete(item);
                        point?.ItemDestroyed(item);
                    }

                    break;
                case MixResult.DowngradedRandom:
                    itemLink.StoredItem.ForEach(item =>
                    {
                        var previousLevel = item.Level;
                        item.Level = (byte) Rand.NextInt(0, item.Level);
                        Log.DebugFormat("Item {0} was downgraded from {1} to {2}.", item, previousLevel, item.Level);
                    });

                    break;
                case MixResult.DowngradedTo0:
                    itemLink.StoredItem.ForEach(item =>
                    {
                        Log.DebugFormat("Item {0} is getting downgraded to level 0.", item);
                        item.Level = 0;
                    });

                    break;
                default:
                    if (Log.IsDebugEnabled)
                    {
                        itemLink.StoredItem.ForEach(item => Log.DebugFormat("Item {0} stays as-is.", item));
                    }

                    // The item stays as is.
                    break;
            }
        }
    }
}
