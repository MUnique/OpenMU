// <copyright file="FruitConsumeHandler.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.GameLogic.PlayerActions.ItemConsumeActions
{
    using System;
    using System.Linq;
    using MUnique.OpenMU.AttributeSystem;
    using MUnique.OpenMU.DataModel.Entities;
    using MUnique.OpenMU.GameLogic.Attributes;
    using MUnique.OpenMU.GameLogic.Views.Character;

    /// <summary>
    /// Implementation of a consume handler for fruits.
    /// </summary>
    public class FruitConsumeHandler : BaseConsumeHandler
    {
        /// <inheritdoc />
        public override bool ConsumeItem(Player player, Item item, Item targetItem, FruitUsage fruitUsage)
        {
            if (!this.CheckPreconditions(player, item, targetItem))
            {
                return false;
            }

            var isAdding = fruitUsage != FruitUsage.RemovePoints;
            var statAttribute = this.GetStatAttribute(item);
            if (player.Level < 10 || item.Level > 4)
            {
                player.ViewPlugIns.GetPlugIn<IFruitConsumptionResponsePlugIn>()?
                    .ShowResponse(isAdding ? FruitConsumptionResult.PlusPrevented : FruitConsumptionResult.MinusPrevented, 0, statAttribute);
                return false;
            }

            var statAttributeDefinition = player.SelectedCharacter.CharacterClass.StatAttributes.FirstOrDefault(a =>
                a.IncreasableByPlayer && a.Attribute == statAttribute);
            if (statAttributeDefinition is null)
            {
                player.ViewPlugIns.GetPlugIn<IFruitConsumptionResponsePlugIn>()?
                    .ShowResponse(isAdding ? FruitConsumptionResult.PlusPrevented : FruitConsumptionResult.MinusPrevented, 0, statAttribute);
                return false;
            }

            if (player.Inventory.EquippedItems.Any())
            {
                player.ViewPlugIns.GetPlugIn<IFruitConsumptionResponsePlugIn>()?.ShowResponse(FruitConsumptionResult.PreventedByEquippedItems, 0, statAttribute);
                return false;
            }

            var maximumRemainingPoints = player.SelectedCharacter.GetMaximumFruitPoints()
                                         - (isAdding
                                             ? player.SelectedCharacter.UsedFruitPoints
                                             : player.SelectedCharacter.UsedNegFruitPoints);

            if (maximumRemainingPoints <= 0)
            {
                player.ViewPlugIns.GetPlugIn<IFruitConsumptionResponsePlugIn>()?
                    .ShowResponse(isAdding ? FruitConsumptionResult.PlusPreventedByMaximum : FruitConsumptionResult.MinusPreventedByMaximum, 0, statAttribute);
                return false;
            }

            if (!isAdding && player.Attributes[statAttribute] <= statAttributeDefinition.BaseValue)
            {
                player.ViewPlugIns.GetPlugIn<IFruitConsumptionResponsePlugIn>()?
                    .ShowResponse(FruitConsumptionResult.MinusPreventedByDefault, 0, statAttribute);
                return false;
            }

            var successPercentage = this.GetSuccessPercentage(player, isAdding);
            if (Rand.NextRandomBool(successPercentage))
            {
                var randomPoints = (byte) Math.Min(maximumRemainingPoints, this.GetRandomPoints(isAdding));
                if (isAdding)
                {
                    player.Attributes[statAttribute] += randomPoints;
                    player.SelectedCharacter.UsedFruitPoints += randomPoints;
                    player.ViewPlugIns.GetPlugIn<IFruitConsumptionResponsePlugIn>()?
                        .ShowResponse(FruitConsumptionResult.PlusSuccess, randomPoints, statAttribute);
                }
                else
                {
                    player.Attributes[statAttribute] -= randomPoints;
                    player.SelectedCharacter.UsedNegFruitPoints += randomPoints;
                    player.ViewPlugIns.GetPlugIn<IFruitConsumptionResponsePlugIn>()?
                        .ShowResponse(FruitConsumptionResult.MinusSuccess, randomPoints, statAttribute);
                }
            }
            else
            {
                player.ViewPlugIns.GetPlugIn<IFruitConsumptionResponsePlugIn>()?
                    .ShowResponse(isAdding ? FruitConsumptionResult.PlusFailed : FruitConsumptionResult.MinusFailed, 0, statAttribute);
            }

            this.ConsumeSourceItem(player, item);
            return true;
        }

        private int GetRandomPoints(bool isAdding)
        {
            var random = Rand.NextInt(0, 101);
            if (isAdding)
            {
                if (random < 70)
                {
                    return 1;
                }

                if (random < 95)
                {
                    return 2;
                }

                return 3;
            }

            if (random < 50)
            {
                return 1;
            }

            if (random < 75)
            {
                return 3;
            }

            if (random < 91)
            {
                return 5;
            }

            if (random < 98)
            {
                return 7;
            }

            return 9;
        }

        private int GetSuccessPercentage(Player player, bool isAdding)
        {
            var currentUseCount = isAdding
                ? player.SelectedCharacter.UsedFruitPoints
                : player.SelectedCharacter.UsedNegFruitPoints;

            var maximumUseCount = player.SelectedCharacter.GetMaximumFruitPoints();
            if (currentUseCount <= 10)
            {
                return 100;
            }

            if ((currentUseCount - 10) < (maximumUseCount * 0.1))
            {
                return 90;
            }

            if ((currentUseCount - 10) < (maximumUseCount * 0.3))
            {
                return 80;
            }

            if ((currentUseCount - 10) < (maximumUseCount * 0.5))
            {
                return 70;
            }

            if ((currentUseCount - 10) < (maximumUseCount * 0.8))
            {
                return 60;
            }

            return 50;
        }

        private AttributeDefinition GetStatAttribute(Item item)
        {
            return item.Level switch
            {
                0 => Stats.BaseEnergy,
                1 => Stats.BaseVitality,
                2 => Stats.BaseAgility,
                3 => Stats.BaseStrength,
                4 => Stats.BaseLeadership,
                _ => throw new ArgumentException("Invalid item level {item.Level}"),
            };
        }
    }
}