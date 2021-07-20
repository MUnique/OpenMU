// <copyright file="ItemPowerUpFactory.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.GameLogic
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using Microsoft.Extensions.Logging;
    using MUnique.OpenMU.AttributeSystem;
    using MUnique.OpenMU.DataModel;
    using MUnique.OpenMU.DataModel.Attributes;
    using MUnique.OpenMU.DataModel.Configuration;
    using MUnique.OpenMU.DataModel.Configuration.Items;
    using MUnique.OpenMU.DataModel.Entities;
    using MUnique.OpenMU.GameLogic.Attributes;

    /// <summary>
    /// The implementation of the item power up factory.
    /// </summary>
    public class ItemPowerUpFactory : IItemPowerUpFactory
    {
        private readonly ILogger<ItemPowerUpFactory> logger;

        /// <summary>
        /// Initializes a new instance of the <see cref="ItemPowerUpFactory"/> class.
        /// </summary>
        /// <param name="logger">The logger.</param>
        public ItemPowerUpFactory(ILogger<ItemPowerUpFactory> logger)
        {
            this.logger = logger;
        }

        /// <inheritdoc/>
        public IEnumerable<PowerUpWrapper> GetPowerUps(Item item, AttributeSystem attributeHolder)
        {
            if (item.Definition is null)
            {
                throw new ArgumentException($"Item of slot {item.ItemSlot} got no Definition.", nameof(item));
            }

            if (item.Durability <= 0)
            {
                yield break;
            }

            if (item.ItemSlot < InventoryConstants.FirstEquippableItemSlotIndex || item.ItemSlot > InventoryConstants.LastEquippableItemSlotIndex)
            {
                yield break;
            }

            foreach (var attribute in item.Definition.BasePowerUpAttributes)
            {
                foreach (var powerUp in this.GetBasePowerUpWrappers(item, attributeHolder, attribute))
                {
                    yield return powerUp;
                }
            }

            foreach (var powerUp in this.GetPowerUpsOfItemOptions(item, attributeHolder))
            {
                yield return powerUp;
            }
        }

        /// <inheritdoc/>
        public IEnumerable<PowerUpWrapper> GetSetPowerUps(
            IEnumerable<Item> equippedItems,
            AttributeSystem attributeHolder,
            GameConfiguration gameConfiguration)
        {
            var activeItems = equippedItems
                .Where(i => i.Durability > 0)
                .ToList();
            var itemGroups = activeItems
                .SelectMany(i => i.ItemSetGroups ?? Enumerable.Empty<ItemSetGroup>())
                .Distinct();

            var result = Enumerable.Empty<PowerUpDefinition>();
            foreach (var group in itemGroups)
            {
                if (group.AlwaysApplies)
                {
                    result = result.Concat(group.Options.Select(o => o.PowerUpDefinition ?? throw Error.NotInitializedProperty(o, nameof(o.PowerUpDefinition))));

                    continue;
                }

                var itemsOfGroup = activeItems.Where(i => (i.ItemSetGroups?.Contains(group) ?? false)
                    && (group.SetLevel == 0 || i.Level >= group.SetLevel));
                var setMustBeComplete = group.MinimumItemCount == group.Items.Count;
                if (group.SetLevel > 0 && setMustBeComplete && itemsOfGroup.All(i => i.Level > group.SetLevel))
                {
                    // When all items are of higher level and the set bonus is applied when all items are there, another item set group will take care.
                    // This should prevent that for example set bonus defense is applied multiple times.
                    continue;
                }

                var itemCount = group.CountDistinct ? itemsOfGroup.Select(item => item.Definition).Distinct().Count() : itemsOfGroup.Count();
                var setIsComplete = itemCount == group.Items.Count;
                if (setIsComplete)
                {
                    // Take all options when the set is complete
                    result = result.Concat(group.Options.Select(o => o.PowerUpDefinition ?? throw Error.NotInitializedProperty(o, nameof(o.PowerUpDefinition))));
                    continue;
                }

                if (itemCount >= group.MinimumItemCount)
                {
                    // Take the first n-1 options
                    result = result.Concat(group.Options.OrderBy(o => o.Number)
                        .Take(itemCount - 1)
                        .Select(o => o.PowerUpDefinition ?? throw Error.NotInitializedProperty(o, nameof(o.PowerUpDefinition))));
                }
            }

            result = result.Concat(this.GetOptionCombinationBonus(activeItems, gameConfiguration));

            return result.SelectMany(p => PowerUpWrapper.CreateByPowerUpDefinition(p, attributeHolder));
        }

        private IEnumerable<PowerUpDefinition> GetOptionCombinationBonus(IEnumerable<Item> activeItems, GameConfiguration gameConfiguration)
        {
            if (gameConfiguration?.ItemOptionCombinationBonuses is null
                 || gameConfiguration.ItemOptionCombinationBonuses.Count == 0)
            {
                yield break;
            }

            var activeItemOptions = activeItems.SelectMany(i => i.ItemOptions.Select(o => o.ItemOption ?? throw Error.NotInitializedProperty(o, nameof(o.ItemOption)))).ToList();
            foreach (var combinationBonus in gameConfiguration.ItemOptionCombinationBonuses.Where(c => c.Bonus is { }))
            {
                var remainingOptions = activeItemOptions.ToList<ItemOption>();
                while (this.AreRequiredOptionsFound(combinationBonus, remainingOptions))
                {
                    yield return combinationBonus.Bonus ?? throw Error.NotInitializedProperty(combinationBonus, nameof(combinationBonus.Bonus));
                    if (!combinationBonus.AppliesMultipleTimes)
                    {
                        break;
                    }
                }
            }
        }

        private bool AreRequiredOptionsFound(ItemOptionCombinationBonus bonus, IList<ItemOption> itemOptions)
        {
            var allMatches = new List<ItemOption>();
            foreach (var requirement in bonus.Requirements)
            {
                var matches = itemOptions
                    .Where(o => o.OptionType == requirement.OptionType && o.SubOptionType == requirement.SubOptionType)
                    .Take(requirement.MinimumCount)
                    .ToList();
                if (matches.Count < requirement.MinimumCount)
                {
                    return false;
                }

                allMatches.AddRange(matches);
            }

            allMatches.ForEach(o => itemOptions.Remove(o));
            return true;
        }

        private IEnumerable<PowerUpWrapper> GetBasePowerUpWrappers(Item item, AttributeSystem attributeHolder, ItemBasePowerUpDefinition attribute)
        {
            attribute.ThrowNotInitializedProperty(attribute.BaseValueElement is null, nameof(attribute.BaseValueElement));
            attribute.ThrowNotInitializedProperty(attribute.TargetAttribute is null, nameof(attribute.TargetAttribute));

            yield return new PowerUpWrapper(attribute.BaseValueElement, attribute.TargetAttribute, attributeHolder);

            var levelBonus = (attribute.BonusPerLevel ?? Enumerable.Empty<LevelBonus>()).FirstOrDefault(bonus => bonus.Level == item.Level);
            if (levelBonus != null)
            {
                levelBonus.ThrowNotInitializedProperty(levelBonus.AdditionalValueElement is null, nameof(levelBonus.AdditionalValueElement));
                yield return new PowerUpWrapper(levelBonus.AdditionalValueElement, attribute.TargetAttribute, attributeHolder);
            }
        }

        private IEnumerable<PowerUpWrapper> GetPowerUpsOfItemOptions(Item item, AttributeSystem attributeHolder)
        {
            var options = item.ItemOptions;
            if (options is null)
            {
                yield break;
            }

            foreach (var optionLink in options)
            {
                var option = optionLink.ItemOption ?? throw Error.NotInitializedProperty(optionLink, nameof(optionLink.ItemOption));
                var powerUp = option.PowerUpDefinition ?? throw Error.NotInitializedProperty(option, nameof(option.PowerUpDefinition));
                var level = option.LevelType == LevelType.ItemLevel ? item.Level : optionLink.Level;
                if (level > 0)
                {
                    var optionOfLevel = option.LevelDependentOptions?.FirstOrDefault(l => l.Level == level);
                    if (optionOfLevel is null && level > 1)
                    {
                        this.logger.LogWarning($"Item has {nameof(IncreasableItemOption)} with level > 1, but no definition in {nameof(IncreasableItemOption.LevelDependentOptions)}");
                        continue;
                    }

                    powerUp = optionOfLevel?.PowerUpDefinition ?? powerUp;
                }

                if (powerUp?.Boost is null)
                {
                    // Some options are level dependent. If they are at level 0, they might not have any boost yet.
                    continue;
                }

                foreach (var wrapper in PowerUpWrapper.CreateByPowerUpDefinition(powerUp, attributeHolder))
                {
                    yield return wrapper;
                }
            }
        }
    }
}
