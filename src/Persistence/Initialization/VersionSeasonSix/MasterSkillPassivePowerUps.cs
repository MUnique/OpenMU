// <copyright file="MasterSkillPassivePowerUps.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.Persistence.Initialization.VersionSeasonSix;

using MUnique.OpenMU.AttributeSystem;
using MUnique.OpenMU.DataModel.Attributes;
using MUnique.OpenMU.GameLogic.Attributes;
using MUnique.OpenMU.Persistence.Initialization.Skills;

/// <summary>
/// Creates the <see cref="MasterSkillDefinition.PassivePowerUps"/> of the season 6 master skills.
/// </summary>
/// <remarks>
/// It's used by the data initialization and by the configuration update for existing databases,
/// so both end up with the same data.
/// </remarks>
internal static class MasterSkillPassivePowerUps
{
    /// <summary>
    /// The factor by which the durability reduction skills reduce the <see cref="Stats.DurabilityReductionFactor"/>
    /// per skill level. Each level reduces it by 0.2 percentage points, so at level 20 it's 6% instead of the default 10%.
    /// </summary>
    private const float DurabilityReductionFactorPerLevel = -1f / 500;

    /// <summary>
    /// The castable master skills, whose value also applies passively to their <see cref="MasterSkillDefinition.TargetAttribute"/>
    /// as long as they're learned.
    /// </summary>
    private static readonly SkillNumber[] CastableSkillsWithPassiveValue =
    [
        SkillNumber.TwistingSlashMastery,
        SkillNumber.RagefulBlowMastery,
        SkillNumber.TripleShotMastery,
        SkillNumber.SleepStrengthener,
        SkillNumber.DrainLifeStrengthener,
    ];

    /// <summary>
    /// The skills which additionally reduce the <see cref="Stats.DurabilityReductionFactor"/>.
    /// </summary>
    private static readonly SkillNumber[] DurabilityReductionSkills =
    [
        SkillNumber.DurabilityReduction1,
        SkillNumber.DurabilityReduction1FistMaster,
    ];

    /// <summary>
    /// Adds the passive power-ups to all master skills which don't have any yet.
    /// </summary>
    /// <param name="context">The context.</param>
    /// <param name="gameConfiguration">The game configuration.</param>
    public static void AddMissing(IContext context, GameConfiguration gameConfiguration)
    {
        foreach (var skill in gameConfiguration.Skills)
        {
            if (skill.MasterDefinition is not { PassivePowerUps.Count: 0 } masterDefinition)
            {
                continue;
            }

            if (masterDefinition.TargetAttribute is { } targetAttribute
                && (skill.SkillType == SkillType.PassiveBoost || CastableSkillsWithPassiveValue.Contains((SkillNumber)skill.Number)))
            {
                masterDefinition.PassivePowerUps.Add(CreatePowerUp(context, gameConfiguration, targetAttribute, masterDefinition.Aggregation, Stats.MasterSkillValue, 1));
            }

            if (DurabilityReductionSkills.Contains((SkillNumber)skill.Number))
            {
                masterDefinition.PassivePowerUps.Add(CreatePowerUp(context, gameConfiguration, Stats.DurabilityReductionFactor, AggregateType.AddRaw, Stats.SkillLevel, DurabilityReductionFactorPerLevel));
            }
        }
    }

    private static PowerUpDefinition CreatePowerUp(IContext context, GameConfiguration gameConfiguration, AttributeDefinition targetAttribute, AggregateType aggregateType, AttributeDefinition inputAttribute, float inputOperand)
    {
        var powerUp = context.CreateNew<PowerUpDefinition>();
        powerUp.TargetAttribute = targetAttribute.GetPersistent(gameConfiguration);
        powerUp.Boost = context.CreateNew<PowerUpDefinitionValue>();

        // The constant value is part of the aggregation, so it must not change the related value.
        powerUp.Boost.ConstantValue.Value = aggregateType == AggregateType.Multiplicate ? 1 : 0;
        powerUp.Boost.ConstantValue.AggregateType = aggregateType;

        var relatedValue = context.CreateNew<AttributeRelationship>();
        relatedValue.InputAttribute = inputAttribute.GetPersistent(gameConfiguration);
        relatedValue.InputOperand = inputOperand;
        relatedValue.InputOperator = InputOperator.Multiply;
        powerUp.Boost.RelatedValues.Add(relatedValue);
        return powerUp;
    }
}
