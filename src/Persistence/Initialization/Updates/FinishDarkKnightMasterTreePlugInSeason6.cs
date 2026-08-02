// <copyright file="FinishDarkKnightMasterTreePlugInSeason6.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.Persistence.Initialization.Updates;

using System.Runtime.InteropServices;
using MUnique.OpenMU.AttributeSystem;
using MUnique.OpenMU.DataModel.Attributes;
using MUnique.OpenMU.DataModel.Configuration;
using MUnique.OpenMU.GameLogic.Attributes;
using MUnique.OpenMU.Persistence.Initialization.Skills;
using MUnique.OpenMU.PlugIns;

/// <summary>
/// This update completes the dark knight master tree skills and effects. It also fixes the double wield damage calculations.
/// </summary>
[PlugIn]
[Display(Name = PlugInName, Description = PlugInDescription)]
[Guid("F7B2C9E4-1A3D-56F8-9B0C-4E2D7A1F8B3C")]
public class FinishDarkKnightMasterTreePlugInSeason6 : FinishDarkKnightMasterTreePlugInBase
{
    /// <summary>
    /// The plug in name.
    /// </summary>
    internal new const string PlugInName = "Finish Dark Knight Master Tree PlugIn";

    /// <summary>
    /// The plug in description.
    /// </summary>
    internal new const string PlugInDescription = "This update completes the dark knight master tree skills and effects. It also fixes the double wield damage calculations.";

    /// <inheritdoc />
    public override string Name => PlugInName;

    /// <inheritdoc />
    public override string Description => PlugInDescription;

    /// <inheritdoc />
    public override UpdateVersion Version => UpdateVersion.FinishDarkKnightMasterTreeSeason6;

    /// <inheritdoc />
    public override string DataInitializationKey => VersionSeasonSix.DataInitialization.Id;

    /// <inheritdoc />
    protected override async ValueTask ApplyAsync(IContext context, GameConfiguration gameConfiguration)
    {
        await base.ApplyAsync(context, gameConfiguration).ConfigureAwait(false);

        var masteryStunChance = Stats.MasteryStunChance.GetPersistent(gameConfiguration);
        var ragefulBlowMasteryDurabilityDecChance = Stats.RagefulBlowMasteryDurabilityDecChance.GetPersistent(gameConfiguration);
        var spearMasteryDoubleDamageChance = Stats.SpearMasteryDoubleDamageChance.GetPersistent(gameConfiguration);
        var swellLifeHealthIncrease = Stats.SwellLifeHealthIncrease.GetPersistent(gameConfiguration);
        var swellLifeManaIncrease = Stats.SwellLifeManaIncrease.GetPersistent(gameConfiguration);
        var physicalBaseDmg = Stats.PhysicalBaseDmg.GetPersistent(gameConfiguration);

        // Update Life Swell effect
        var lifeSwellEffect = gameConfiguration.MagicEffects.First(e => e.Number == (short)MagicEffectNumber.GreaterFortitude);
        lifeSwellEffect.SubType = 4;
        lifeSwellEffect.Duration!.MaximumValue = 180;

        if (lifeSwellEffect.Duration.RelatedValues.FirstOrDefault() is { } durationPerEnergy)
        {
            durationPerEnergy.InputOperand = 1f / 180f;
        }

        if (lifeSwellEffect.PowerUpDefinitions.FirstOrDefault() is { } maxHealth)
        {
            maxHealth.TargetAttribute = swellLifeHealthIncrease;
            maxHealth.Boost!.ConstantValue.Value = 0.12f;
            maxHealth.Boost.ConstantValue.AggregateType = AggregateType.AddRaw;
            maxHealth.Boost.MaximumValue = 2f;

            foreach (var boostRelatedValue in maxHealth.Boost.RelatedValues)
            {
                if (boostRelatedValue.InputAttribute == Stats.TotalEnergy)
                {
                    boostRelatedValue.InputOperator = InputOperator.Multiply;
                    boostRelatedValue.InputOperand = 1f / 2000;
                    continue;
                }

                if (boostRelatedValue.InputAttribute == Stats.TotalVitality)
                {
                    boostRelatedValue.InputOperator = InputOperator.Multiply;
                    boostRelatedValue.InputOperand = 1f / 10000;
                }
            }

            var boostPerPartyMember = context.CreateNew<AttributeRelationship>();
            boostPerPartyMember.InputAttribute = Stats.NearbyPartyMemberCount.GetPersistent(gameConfiguration);
            boostPerPartyMember.InputOperator = InputOperator.Multiply;
            boostPerPartyMember.InputOperand = 1f / 100;
            maxHealth.Boost.RelatedValues.Add(boostPerPartyMember);
        }

        // Create Life Swell Proficiency Skill Effect
        var lifeSwellProficiencyEffect = this.CreateLifeSwellProficiencyEffect(context, gameConfiguration);
        if (gameConfiguration.Skills.FirstOrDefault(s => s.Number == (short)SkillNumber.SwellLifeProficiency) is { } skill)
        {
            skill.MagicEffectDef = lifeSwellProficiencyEffect;
        }

        // Restore iced effect (revert bug)
        if (gameConfiguration.MagicEffects.FirstOrDefault(e => e.Number == (short)MagicEffectNumber.Iced && e.Chance is { }) is { } originalIced)
        {
            originalIced.Chance = null;
        }

        // Remove existing cold effect
        var existingColdEffect = gameConfiguration.MagicEffects.FirstOrDefault(e => e.Number == (short)MagicEffectNumber.Cold);
        if (existingColdEffect is not null)
        {
            gameConfiguration.MagicEffects.Remove(existingColdEffect);
        }

        // Create chain drive cold effect
        var chainDriveCold = this.CreateEffect(context, gameConfiguration, ElementalType.Ice, MagicEffectNumber.Cold, Stats.IsIced, 10, 0.4f);
        if (gameConfiguration.Skills.FirstOrDefault(s => s.Number == (short)SkillNumber.ChainDrive) is { } chainDrive)
        {
            chainDrive.MagicEffectDef = chainDriveCold;
        }

        if (gameConfiguration.Skills.FirstOrDefault(s => s.Number == (short)SkillNumber.ChainDriveStrengthener) is { } chainDriveStrengthener)
        {
            chainDriveStrengthener.MagicEffectDef = chainDriveCold;
        }

        // Create strike of destruction cold effect
        var strikeOfDestructCold = this.CreateEffect(context, gameConfiguration, ElementalType.Ice, MagicEffectNumber.Cold, Stats.IsIced, 10);
        if (gameConfiguration.Skills.FirstOrDefault(s => s.Number == (short)SkillNumber.StrikeofDestruction) is { } strikeOfDestruction)
        {
            strikeOfDestruction.SkipElementalModifier = true;
            strikeOfDestruction.MagicEffectDef = strikeOfDestructCold;
        }

        if (gameConfiguration.Skills.FirstOrDefault(s => s.Number == (short)SkillNumber.StrikeofDestrStr) is { } strikeOfDestrStr)
        {
            strikeOfDestrStr.SkipElementalModifier = true;
            strikeOfDestrStr.MagicEffectDef = strikeOfDestructCold;
        }

        // Update harmony option
        if (gameConfiguration.ItemOptions.FirstOrDefault(o => o.Name == "Harmony Physical Attack Options") is { } harmonyPhysAttackOptions
            && harmonyPhysAttackOptions.PossibleOptions.FirstOrDefault(o => o.Number == 5) is { } physBaseDmgOpt)
        {
            foreach (var level in physBaseDmgOpt.LevelDependentOptions)
            {
                level.PowerUpDefinition!.Boost!.ConstantValue.AggregateType = AggregateType.AddFinal;
            }
        }

        // Update gold fenrir option
        var goldFenrirOptionId = new Guid("00000083-0081-0000-0000-000000000000");
        if (gameConfiguration.ItemOptions.FirstOrDefault(o => o.GetId() == goldFenrirOptionId) is { } goldFenrirOption)
        {
            foreach (var option in goldFenrirOption.PossibleOptions)
            {
                if (option.PowerUpDefinition?.TargetAttribute == physicalBaseDmg)
                {
                    option.PowerUpDefinition.Boost!.ConstantValue.AggregateType = AggregateType.AddFinal;
                }
            }
        }

        // Update master skills
        if (gameConfiguration.Skills.FirstOrDefault(s => s.Number == (short)SkillNumber.TwistingSlashMastery)?.MasterDefinition is { } twistingSlashMastery)
        {
            twistingSlashMastery.ReplacedSkill = gameConfiguration.Skills.First(s => s.Number == (short)SkillNumber.TwistingSlashStreng);
            twistingSlashMastery.TargetAttribute = Stats.MasteryMoveTargetChance.GetPersistent(gameConfiguration);
            twistingSlashMastery.Aggregation = AggregateType.AddRaw;
            twistingSlashMastery.ValueFormula = $"{twistingSlashMastery.ValueFormula} / 100";
        }

        if (gameConfiguration.Skills.FirstOrDefault(s => s.Number == (short)SkillNumber.RagefulBlowMastery) is { } ragefulBlowMastery)
        {
            ragefulBlowMastery.AttributeRelationships.Add(context.CreateNew<AttributeRelationship>(
                ragefulBlowMasteryDurabilityDecChance,
                1,
                ragefulBlowMasteryDurabilityDecChance,
                InputOperator.Multiply,
                default(AttributeDefinition?),
                AggregateType.AddRaw));

            if (ragefulBlowMastery.MasterDefinition is { } masterDefinition)
            {
                masterDefinition.ReplacedSkill = gameConfiguration.Skills.First(s => s.Number == (short)SkillNumber.RagefulBlowStreng);
                masterDefinition.TargetAttribute = ragefulBlowMasteryDurabilityDecChance;
                masterDefinition.Aggregation = AggregateType.AddRaw;
                masterDefinition.ValueFormula = $"{masterDefinition.ValueFormula} / 100";
            }
        }

        if (gameConfiguration.Skills.FirstOrDefault(s => s.Number == (short)SkillNumber.MaceMastery)?.MasterDefinition is { } maceMastery)
        {
            maceMastery.TargetAttribute = masteryStunChance;
            maceMastery.Aggregation = AggregateType.AddRaw;
            maceMastery.ValueFormula = $"{maceMastery.ValueFormula} / 100";
        }

        if (gameConfiguration.Skills.FirstOrDefault(s => s.Number == (short)SkillNumber.SpearMastery)?.MasterDefinition is { } spearMastery)
        {
            spearMastery.TargetAttribute = spearMasteryDoubleDamageChance;
            spearMastery.Aggregation = AggregateType.AddRaw;
            spearMastery.ValueFormula = $"{spearMastery.ValueFormula} / 100";
        }

        if (gameConfiguration.Skills.FirstOrDefault(s => s.Number == (short)SkillNumber.SwellLifeStrengt)?.MasterDefinition is { } swellLifeStrengt)
        {
            swellLifeStrengt.TargetAttribute = swellLifeHealthIncrease;
            swellLifeStrengt.Aggregation = AggregateType.AddRaw;
            swellLifeStrengt.ValueFormula = $"{swellLifeStrengt.ValueFormula} / 100";
        }

        if (gameConfiguration.Skills.FirstOrDefault(s => s.Number == (short)SkillNumber.SwellLifeProficiency)?.MasterDefinition is { } swellLifeProficiency)
        {
            swellLifeProficiency.ReplacedSkill = gameConfiguration.Skills.First(s => s.Number == (short)SkillNumber.SwellLifeStrengt);
            swellLifeProficiency.TargetAttribute = swellLifeManaIncrease;
            swellLifeProficiency.Aggregation = AggregateType.AddRaw;
            swellLifeProficiency.ValueFormula = $"{swellLifeProficiency.ValueFormula} / 100";
        }
    }

    private MagicEffectDefinition CreateLifeSwellProficiencyEffect(IContext context, GameConfiguration gameConfiguration)
    {
        var magicEffect = context.CreateNew<MagicEffectDefinition>();
        gameConfiguration.MagicEffects.Add(magicEffect);
        magicEffect.Number = (byte)MagicEffectNumber.GreaterFortitudeProficiency;
        magicEffect.Name = "Life Swell Proficiency Skill Effect";

        var lifeSwellEffect = gameConfiguration.MagicEffects.First(e => e.Number == (short)MagicEffectNumber.GreaterFortitude);
        magicEffect.InformObservers = lifeSwellEffect.InformObservers;
        magicEffect.SubType = lifeSwellEffect.SubType;
        magicEffect.SendDuration = lifeSwellEffect.SendDuration;
        magicEffect.StopByDeath = lifeSwellEffect.StopByDeath;
        magicEffect.Duration = context.CreateNew<PowerUpDefinitionValue>();
        magicEffect.Duration.ConstantValue.Value = lifeSwellEffect.Duration!.ConstantValue.Value;
        magicEffect.Duration.MaximumValue = lifeSwellEffect.Duration.MaximumValue;

        foreach (var durationRelatedValue in lifeSwellEffect.Duration.RelatedValues)
        {
            var durationRelatedValueCopy = context.CreateNew<AttributeRelationship>();
            durationRelatedValueCopy.InputAttribute = durationRelatedValue.InputAttribute!.GetPersistent(gameConfiguration);
            durationRelatedValueCopy.InputOperator = durationRelatedValue.InputOperator;
            durationRelatedValueCopy.InputOperand = durationRelatedValue.InputOperand;
            magicEffect.Duration.RelatedValues.Add(durationRelatedValueCopy);
        }

        foreach (var powerUp in lifeSwellEffect.PowerUpDefinitions)
        {
            var powerUpCopy = context.CreateNew<PowerUpDefinition>();
            magicEffect.PowerUpDefinitions.Add(powerUpCopy);
            powerUpCopy.TargetAttribute = powerUp.TargetAttribute!.GetPersistent(gameConfiguration);
            powerUpCopy.Boost = context.CreateNew<PowerUpDefinitionValue>();
            powerUpCopy.Boost.ConstantValue.Value = powerUp.Boost!.ConstantValue.Value;
            powerUpCopy.Boost.MaximumValue = powerUp.Boost.MaximumValue;

            foreach (var boostRelatedValue in powerUp.Boost.RelatedValues)
            {
                var boostRelatedValueCopy = context.CreateNew<AttributeRelationship>();
                boostRelatedValueCopy.InputAttribute = boostRelatedValue.InputAttribute!.GetPersistent(gameConfiguration);
                boostRelatedValueCopy.InputOperator = boostRelatedValue.InputOperator;
                boostRelatedValueCopy.InputOperand = boostRelatedValue.InputOperand;
                powerUpCopy.Boost.RelatedValues.Add(boostRelatedValueCopy);
            }
        }

        // one percent per party member in view
        var boostPerPartyMember = context.CreateNew<AttributeRelationship>();
        boostPerPartyMember.InputAttribute = Stats.NearbyPartyMemberCount.GetPersistent(gameConfiguration);
        boostPerPartyMember.InputOperator = InputOperator.Multiply;
        boostPerPartyMember.InputOperand = 1f / 100;

        var manaPowerUpDefinition = context.CreateNew<PowerUpDefinition>();
        magicEffect.PowerUpDefinitions.Add(manaPowerUpDefinition);
        manaPowerUpDefinition.TargetAttribute = Stats.SwellLifeManaIncrease.GetPersistent(gameConfiguration);
        manaPowerUpDefinition.Boost = context.CreateNew<PowerUpDefinitionValue>();
        manaPowerUpDefinition.Boost.RelatedValues.Add(boostPerPartyMember);

        return magicEffect;
    }

    private MagicEffectDefinition CreateEffect(IContext context, GameConfiguration gameConfiguration, ElementalType type, MagicEffectNumber effectNumber, AttributeDefinition targetAttribute, float durationInSeconds, float chance = 0)
    {
        var effect = context.CreateNew<MagicEffectDefinition>();
        gameConfiguration.MagicEffects.Add(effect);
        effect.Name = Enum.GetName(effectNumber) ?? string.Empty;
        effect.InformObservers = true;
        effect.Number = (short)effectNumber;
        effect.StopByDeath = true;
        effect.SubType = (byte)(0xFF - type);
        effect.Duration = context.CreateNew<PowerUpDefinitionValue>();
        effect.Duration.ConstantValue.Value = durationInSeconds;
        var powerUpDefinition = context.CreateNew<PowerUpDefinition>();
        effect.PowerUpDefinitions.Add(powerUpDefinition);
        powerUpDefinition.Boost = context.CreateNew<PowerUpDefinitionValue>();
        powerUpDefinition.Boost.ConstantValue.Value = 1;
        powerUpDefinition.TargetAttribute = targetAttribute.GetPersistent(gameConfiguration);
        if (targetAttribute == Stats.IsIced)
        {
            var movementSpeedFactorPowerUp = context.CreateNew<PowerUpDefinition>();
            effect.PowerUpDefinitions.Add(movementSpeedFactorPowerUp);
            movementSpeedFactorPowerUp.Boost = context.CreateNew<PowerUpDefinitionValue>();
            movementSpeedFactorPowerUp.Boost.ConstantValue.AggregateType = AggregateType.Multiplicate;
            movementSpeedFactorPowerUp.TargetAttribute = Stats.MovementSpeedFactor.GetPersistent(gameConfiguration);

            if (effectNumber == MagicEffectNumber.Cold)
            {
                movementSpeedFactorPowerUp.Boost.ConstantValue.Value = MovementSpeedConstants.ColdMovementSpeedFactor;
            }
            else
            {
                movementSpeedFactorPowerUp.Boost.ConstantValue.Value = MovementSpeedConstants.IcedMovementSpeedFactor;
            }
        }

        if (chance > 0)
        {
            effect.Chance = context.CreateNew<PowerUpDefinitionValue>();
            effect.Chance.ConstantValue.Value = chance;
        }

        return effect;
    }
}
