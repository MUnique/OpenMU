// <copyright file="TransformationRingConfiguration.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.Persistence.Initialization.Items;

using MUnique.OpenMU.AttributeSystem;
using MUnique.OpenMU.DataModel.Configuration;
using MUnique.OpenMU.DataModel.Configuration.Items;
using MUnique.OpenMU.GameLogic.Attributes;
using MUnique.OpenMU.Interfaces;

/// <summary>
/// Configures the high-level requirements and health bonus of the original transformation ring.
/// </summary>
internal static class TransformationRingConfiguration
{
    private const byte HighLevelRingStartsAt = 3;
    private const int HighLevelRingRequiredCharacterLevel = 50;
    private const int DeathCowRingItemLevel = 5;
    private const float DeathCowRingHealthBonus = 0.2f;

    /// <summary>
    /// Ensures the configured requirements and maximum-health power-up for the original transformation ring.
    /// </summary>
    /// <param name="context">The persistence context.</param>
    /// <param name="gameConfiguration">The game configuration.</param>
    /// <param name="ring">The transformation ring definition.</param>
    public static void ConfigureHighLevelTransformationRing(IContext context, GameConfiguration gameConfiguration, ItemDefinition ring)
    {
        var requirement = ring.Requirements.FirstOrDefault(
            existing => existing.Attribute == Stats.Level && existing.MinimumItemLevel == HighLevelRingStartsAt);
        if (requirement is null)
        {
            requirement = context.CreateNew<AttributeRequirement>();
            requirement.Attribute = Stats.Level.GetPersistent(gameConfiguration);
            requirement.MinimumItemLevel = HighLevelRingStartsAt;
            ring.Requirements.Add(requirement);
        }

        requirement.MinimumValue = HighLevelRingRequiredCharacterLevel;

        var maximumHealth = Stats.MaximumHealth.GetPersistent(gameConfiguration);
        if (ring.BasePowerUpAttributes.Any(powerUp =>
                powerUp.TargetAttribute == maximumHealth
                && powerUp.AggregateType == AggregateType.Multiplicate
                && powerUp.BaseValue == 1
                && powerUp.BonusPerLevelTable?.BonusPerLevel.Any(bonus =>
                    bonus.Level == DeathCowRingItemLevel && bonus.AdditionalValue == DeathCowRingHealthBonus) == true))
        {
            return;
        }

        var bonusTable = context.CreateNew<ItemLevelBonusTable>();
        gameConfiguration.ItemLevelBonusTables.Add(bonusTable);
        bonusTable.Name = LevelBonusNames.ResourceManager.GetLocalizedString("DeathCowTransformationRingHealthBonus");
        bonusTable.Description = LevelBonusDescriptions.ResourceManager.GetLocalizedString("DeathCowTransformationRingHealthBonus");

        var levelBonus = context.CreateNew<LevelBonus>();
        levelBonus.Level = DeathCowRingItemLevel;
        levelBonus.AdditionalValue = DeathCowRingHealthBonus;
        bonusTable.BonusPerLevel.Add(levelBonus);

        var healthPowerUp = context.CreateNew<ItemBasePowerUpDefinition>();
        healthPowerUp.TargetAttribute = maximumHealth;
        healthPowerUp.BaseValue = 1;
        healthPowerUp.AggregateType = AggregateType.Multiplicate;
        healthPowerUp.BonusPerLevelTable = bonusTable;
        ring.BasePowerUpAttributes.Add(healthPowerUp);
    }
}
