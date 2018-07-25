// <copyright file="ItemPowerUpFactory.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.GameLogic
{
    using System.Collections.Generic;
    using System.Linq;
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

            if (item.Definition != null)
            {
                foreach (var attribute in item.Definition.BasePowerUpAttributes)
                {
                    yield return new PowerUpWrapper(attribute.BaseValueElement, attribute.TargetAttribute, attributeHolder);
                    if (item.Level > 0)
                    {
                        var levelBonus = (attribute.BonusPerLevel ?? Enumerable.Empty<LevelBonus>()).FirstOrDefault(bonus => bonus.Level == item.Level);
                        if (levelBonus != null)
                        {
                            yield return new PowerUpWrapper(levelBonus.AdditionalValueElement, attribute.TargetAttribute, attributeHolder);
                        }
                    }
                }

                foreach (var powerUp in this.GetPowerUpsOfItemOptions(item, attributeHolder))
                {
                    yield return powerUp;
                }
            }

            ////// TODO: Sockets...
            ////if (item.AppliedSockets != null)
            ////{
            ////    foreach (var socket in item.AppliedSockets)
            ////    {
            ////        yield return socket.Definition.PowerUp;
            ////    }
            ////}
        }

        /// <inheritdoc/>
        public IEnumerable<PowerUpWrapper> GetSetPowerUps(IEnumerable<Item> equippedItems, AttributeSystem attributeHolder)
        {
            var itemGroups = equippedItems
                .Where(i => i.Durability > 0)
                .Where(i => i.ItemSetGroups != null)
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

                var itemsOfGroup = equippedItems.Where(i => group.SetLevel == 0 || i.Level == group.SetLevel).Select(i => i.Definition);

                var itemCount = group.CountDistinct ? itemsOfGroup.Distinct().Count() : itemsOfGroup.Count();
                if (itemCount >= group.MinimumItemCount)
                {
                    result = result.Concat(group.Options.Take(itemCount - 1).Select(o => o.PowerUpDefinition));
                }
            }

            return result.SelectMany(p => PowerUpWrapper.CreateByPowerUpDefintion(p, attributeHolder));
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
                        // TODO: Log, this should never happen.
                        continue;
                    }

                    powerUp = optionOfLevel.PowerUpDefinition;
                }

                if (powerUp?.Boost == null)
                {
                    // Some options are level dependent. If they are at level 0, they might not have any boost yet.
                    continue;
                }

                foreach (var wrapper in PowerUpWrapper.CreateByPowerUpDefintion(powerUp, attributeHolder))
                {
                    yield return wrapper;
                }
            }
        }
    }
}
