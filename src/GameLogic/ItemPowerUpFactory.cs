// <copyright file="ItemPowerUpFactory.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.GameLogic
{
    using System.Collections.Generic;
    using System.Linq;
    using log4net;
    using MUnique.OpenMU.AttributeSystem;
    using MUnique.OpenMU.DataModel.Attributes;
    using MUnique.OpenMU.DataModel.Configuration.Items;
    using MUnique.OpenMU.DataModel.Entities;
    using MUnique.OpenMU.GameLogic.Attributes;

    /// <summary>
    /// The implementation of the item power up factory.
    /// </summary>
    public class ItemPowerUpFactory : IItemPowerUpFactory
    {
        private static readonly ILog Log = LogManager.GetLogger(typeof(ItemPowerUpFactory));

        /// <inheritdoc/>
        public IEnumerable<PowerUpWrapper> GetPowerUps(Item item, AttributeSystem attributeHolder)
        {
            if (item.Durability <= 0)
            {
                yield break;
            }

            if (item.ItemSlot < InventoryConstants.FirstEquippableItemSlotIndex || item.ItemSlot > InventoryConstants.LastEquippableItemSlotIndex)
            {
                yield break;
            }

            if (item.Definition == null)
            {
                Log.Warn($"Item of slot {item.ItemSlot} got no Definition.");
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
        public IEnumerable<PowerUpWrapper> GetSetPowerUps(IEnumerable<Item> equippedItems, AttributeSystem attributeHolder)
        {
            var activeItems = equippedItems
                .Where(i => i.Durability > 0)
                .Where(i => i.ItemSetGroups != null)
                .ToList();
            var itemGroups = activeItems
                .SelectMany(i => i.ItemSetGroups)
                .Distinct();

            var result = Enumerable.Empty<PowerUpDefinition>();
            foreach (var group in itemGroups)
            {
                if (group.AlwaysApplies)
                {
                    result = result.Concat(group.Options.Select(o => o.PowerUpDefinition));

                    continue;
                }

                if (group.SetLevel > 0 && group.MinimumItemCount == group.Items.Count && activeItems.All(i => i.Level > group.SetLevel))
                {
                    // When all items are of higher level and the set bonus is applied when all items are there, another item set group will take care.
                    // This should prevent that for example set bonus defense is applied multiple times.
                    continue;
                }

                var itemsOfGroup = activeItems.Where(i => group.SetLevel == 0 || i.Level >= group.SetLevel).Select(i => i.Definition).ToList();

                // The set item bonus options are always given, regardless if the set is complete or not.
                result = result.Concat(group.Items.Where(i => i.BonusOption != null && itemsOfGroup.Contains(i.ItemDefinition)).Select(i => i.BonusOption.PowerUpDefinition));

                var itemCount = group.CountDistinct ? itemsOfGroup.Distinct().Count() : itemsOfGroup.Count;
                if (itemCount >= group.MinimumItemCount)
                {
                    result = result.Concat(group.Options.OrderBy(o => o.Number).Take(itemCount - 1).Select(o => o.PowerUpDefinition));
                }
            }

            return result.SelectMany(p => PowerUpWrapper.CreateByPowerUpDefinition(p, attributeHolder));
        }

        private IEnumerable<PowerUpWrapper> GetBasePowerUpWrappers(Item item, AttributeSystem attributeHolder, ItemBasePowerUpDefinition attribute)
        {
            yield return new PowerUpWrapper(attribute.BaseValueElement, attribute.TargetAttribute, attributeHolder);
            if (item.Level == 0)
            {
                yield break;
            }

            var levelBonus = (attribute.BonusPerLevel ?? Enumerable.Empty<LevelBonus>()).FirstOrDefault(bonus => bonus.Level == item.Level);
            if (levelBonus != null)
            {
                yield return new PowerUpWrapper(levelBonus.AdditionalValueElement, attribute.TargetAttribute, attributeHolder);
            }
        }

        private IEnumerable<PowerUpWrapper> GetPowerUpsOfItemOptions(Item item, AttributeSystem attributeHolder)
        {
            var options = item.ItemOptions;
            if (options == null)
            {
                yield break;
            }

            foreach (var optionLink in options)
            {
                var option = optionLink.ItemOption;
                var powerUp = option.PowerUpDefinition;
                var level = option.LevelType == LevelType.ItemLevel ? item.Level : optionLink.Level;
                if (level > 1)
                {
                    var optionOfLevel = option.LevelDependentOptions.FirstOrDefault(l => l.Level == level);
                    if (optionOfLevel == null)
                    {
                        Log.Warn($"Item has {nameof(IncreasableItemOption)} with level > 0, but no definition in {nameof(IncreasableItemOption.LevelDependentOptions)}");
                        continue;
                    }

                    powerUp = optionOfLevel.PowerUpDefinition;
                }

                if (powerUp?.Boost == null)
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
