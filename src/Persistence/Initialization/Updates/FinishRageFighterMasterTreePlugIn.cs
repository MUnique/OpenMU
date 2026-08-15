// <copyright file="FinishRageFighterMasterTreePlugIn.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.Persistence.Initialization.Updates;

using System.Runtime.InteropServices;
using MUnique.OpenMU.AttributeSystem;
using MUnique.OpenMU.DataModel.Attributes;
using MUnique.OpenMU.DataModel.Configuration;
using MUnique.OpenMU.GameLogic;
using MUnique.OpenMU.GameLogic.Attributes;
using MUnique.OpenMU.Persistence.Initialization.Skills;
using MUnique.OpenMU.Persistence.Initialization.VersionSeasonSix;
using MUnique.OpenMU.PlugIns;

/// <summary>
/// This update completes the rage fighter master tree and fixes some of its skill values.
/// </summary>
[PlugIn]
[Display(Name = PlugInName, Description = PlugInDescription)]
[Guid("2DAE95BC-AE08-45E8-942A-9F61AE1C277B")]
public class FinishRageFighterMasterTreePlugIn : UpdatePlugInBase
{
    /// <summary>
    /// The plug in name.
    /// </summary>
    internal const string PlugInName = "Finish Rage Fighter Master Tree PlugIn";

    /// <summary>
    /// The plug in description.
    /// </summary>
    internal const string PlugInDescription = "This update completes the rage fighter master tree and fixes some of its skill values.";

    /// <inheritdoc />
    public override UpdateVersion Version => UpdateVersion.FinishRageFighterMasterTree;

    /// <inheritdoc />
    public override string DataInitializationKey => DataInitialization.Id;

    /// <inheritdoc />
    public override string Name => PlugInName;

    /// <inheritdoc />
    public override string Description => PlugInDescription;

    /// <inheritdoc />
    public override bool IsMandatory => true;

    /// <inheritdoc />
    public override DateTime CreatedAt => new(2026, 8, 7, 16, 0, 0, DateTimeKind.Utc);

    /// <inheritdoc />
    protected override async ValueTask ApplyAsync(IContext context, GameConfiguration gameConfiguration)
    {
        // Create new Stats
        var increaseBlockBonus = context.CreateNew<AttributeDefinition>(Stats.IncreaseBlockBonus.Id, Stats.IncreaseBlockBonus.Designation, Stats.IncreaseBlockBonus.Description);
        gameConfiguration.Attributes.Add(increaseBlockBonus);
        var gloveWeaponMasteryDoubleDamageChance = context.CreateNew<AttributeDefinition>(Stats.GloveWeaponMasteryDoubleDamageChance.Id, Stats.GloveWeaponMasteryDoubleDamageChance.Designation, Stats.GloveWeaponMasteryDoubleDamageChance.Description);
        gameConfiguration.Attributes.Add(gloveWeaponMasteryDoubleDamageChance);

        var defenseRatePvm = Stats.DefenseRatePvm.GetPersistent(gameConfiguration);
        var ammunitionConsumptionRate = Stats.AmmunitionConsumptionRate.GetPersistent(gameConfiguration);
        var skillExtraManaCost = Stats.SkillExtraManaCost.GetPersistent(gameConfiguration);
        var doubleDamageChance = Stats.DoubleDamageChance.GetPersistent(gameConfiguration);
        var isGloveWeaponEquipped = Stats.IsGloveWeaponEquipped.GetPersistent(gameConfiguration);

        gameConfiguration.CharacterClasses.ForEach(charClass =>
        {
            // Add new attribute combination
            var defenseRatePvmToIncreaseBlockBonus = context.CreateNew<AttributeRelationship>(
                increaseBlockBonus,
                1,
                defenseRatePvm,
                InputOperator.Multiply,
                default(AttributeDefinition?),
                AggregateType.Multiplicate);

            charClass.AttributeCombinations.Add(defenseRatePvmToIncreaseBlockBonus);
            charClass.BaseAttributeValues.Add(context.CreateNew<ConstValueAttribute>(0, increaseBlockBonus));

            if (charClass.Number == 8 || charClass.Number == 10 || charClass.Number == 11) // Elf classes
            {
                var ammunitionConsumptionRateToSkillExtraManaCost = context.CreateNew<AttributeRelationship>(
                    skillExtraManaCost,
                    0,
                    ammunitionConsumptionRate,
                    InputOperator.ExponentiateByAttribute,
                    default(AttributeDefinition?),
                    AggregateType.Multiplicate);

                charClass.AttributeCombinations.Add(ammunitionConsumptionRateToSkillExtraManaCost);
            }

            if (charClass.Number == 24 || charClass.Number == 25) // Rage Fighter classes
            {
                var gloveWeaponMasteryDoubleDamageChanceToDoubleDamageChance = context.CreateNew<AttributeRelationship>(
                    doubleDamageChance,
                    isGloveWeaponEquipped,
                    gloveWeaponMasteryDoubleDamageChance,
                    AggregateType.AddRaw);

                charClass.AttributeCombinations.Add(gloveWeaponMasteryDoubleDamageChanceToDoubleDamageChance);
            }
        });

        // Update Ignore Defense effect
        var ignoreDefenseEffect = gameConfiguration.MagicEffects.First(e => e.Number == (short)MagicEffectNumber.IgnoreDefense);
        ignoreDefenseEffect.Duration?.MaximumValue = 180f;

        if (ignoreDefenseEffect.Duration?.RelatedValues.FirstOrDefault() is AttributeRelationship ignoreDurationPerEnergy)
        {
            ignoreDurationPerEnergy.InputOperand = 1f / 5f;
        }

        if (ignoreDefenseEffect.PowerUpDefinitions.FirstOrDefault() is PowerUpDefinition ignoreChance)
        {
            ignoreChance.Boost?.ConstantValue.Value = -0.0104f;
            ignoreChance.Boost?.MaximumValue = 0.1f;
        }

        // Update Increase Block effect
        var increaseBlockEffect = gameConfiguration.MagicEffects.First(e => e.Number == (short)MagicEffectNumber.IncreaseBlock);
        increaseBlockEffect.SubType = 74;
        increaseBlockEffect.Duration?.MaximumValue = 180f;
        increaseBlockEffect.PowerUpDefinitions.Clear();

        if (increaseBlockEffect.Duration?.RelatedValues.FirstOrDefault() is AttributeRelationship blockDurationPerEnergy)
        {
            blockDurationPerEnergy.InputOperand = 1f / 5f;
        }

        var powerUpDefinition = context.CreateNew<PowerUpDefinition>();
        increaseBlockEffect.PowerUpDefinitions.Add(powerUpDefinition);
        powerUpDefinition.TargetAttribute = Stats.IncreaseBlockBonus.GetPersistent(gameConfiguration);
        powerUpDefinition.Boost = context.CreateNew<PowerUpDefinitionValue>();
        powerUpDefinition.Boost.ConstantValue.Value = 2f;   // The parchment requires 80 energy => base value = 10
        powerUpDefinition.Boost.ConstantValue.AggregateType = AggregateType.AddFinal;
        powerUpDefinition.Boost.MaximumValue = 100f;

        var boostPerEnergy = context.CreateNew<AttributeRelationship>();
        boostPerEnergy.InputAttribute = Stats.TotalEnergy.GetPersistent(gameConfiguration);
        boostPerEnergy.InputOperator = InputOperator.Multiply;
        boostPerEnergy.InputOperand = 1f / 10f; // one defense rate per 10 energy
        powerUpDefinition.Boost.RelatedValues.Add(boostPerEnergy);

        // Create Increase Block Power Up, Increase Block Mastery effects
        var increaseBlockPowerUpEffect = this.CreateIncreaseBlockPowerUpEffect(context, gameConfiguration);
        var increaseBlockMasteryEffect = this.CreateIncreaseBlockMasteryEffect(context, gameConfiguration);

        // Update Increase Health effect
        var increaseHealthEffect = gameConfiguration.MagicEffects.First(e => e.Number == (short)MagicEffectNumber.IncreaseHealth);
        increaseHealthEffect.SubType = 73;
        increaseHealthEffect.Duration?.MaximumValue = 180f;

        if (increaseHealthEffect.Duration?.RelatedValues.FirstOrDefault() is AttributeRelationship healthDurationPerEnergy)
        {
            healthDurationPerEnergy.InputOperand = 1f / 5f;
        }

        if (increaseHealthEffect.PowerUpDefinitions.FirstOrDefault() is PowerUpDefinition increaseVitality)
        {
            increaseVitality.Boost?.ConstantValue.Value = 16.8f;
        }

        // Create Increase Health Strengthener effect
        var increaseHealthStrengthenerEffect = this.CreateIncreaseHealthStrengthenerEffect(context, gameConfiguration);

        // Update skills
        if (gameConfiguration.Skills.FirstOrDefault(s => s.Number == (short)SkillNumber.Explosion79) is { } explosion79)
        {
            explosion79.QualifiedCharacters.Clear();
        }

        if (gameConfiguration.Skills.FirstOrDefault(s => s.Number == (short)SkillNumber.DoppelgangerSelfExplosion) is { } doppelgangerSelfExplosion)
        {
            doppelgangerSelfExplosion.QualifiedCharacters.Clear();
        }

        // Add skill attributes for elf shooting skills
        var elfShootingSkills = new[]
        {
            SkillNumber.TripleShot,
            SkillNumber.IceArrow,
            SkillNumber.Penetration,
            SkillNumber.Starfall,
            SkillNumber.MultiShot,
        };

        foreach (var skillNumber in elfShootingSkills)
        {
            if (gameConfiguration.Skills.FirstOrDefault(s => s.Number == (short)skillNumber) is { } skill)
            {
                skill.AttributeRelationships.Add(context.CreateNew<AttributeRelationship>(
                    skillExtraManaCost,
                    1,
                    skillExtraManaCost,
                    InputOperator.Multiply,
                    default(AttributeDefinition?),
                    AggregateType.AddRaw));
            }
        }

        // Update master skills
        if (gameConfiguration.Skills.FirstOrDefault(s => s.Number == (short)SkillNumber.KillingBlowMastery)?.MasterDefinition is { } killingBlowMastery)
        {
            killingBlowMastery.ValueFormula = SkillsInitializer.Formula120Value;
        }

        if (gameConfiguration.Skills.FirstOrDefault(s => s.Number == (short)SkillNumber.BeastUppercutMastery)?.MasterDefinition is { } beastUppercutMastery)
        {
            beastUppercutMastery.ValueFormula = $"-1 * {SkillsInitializer.Formula120Value}";
        }

        if (gameConfiguration.Skills.FirstOrDefault(s => s.Number == (short)SkillNumber.DefSuccessRateIncPowUp) is { } defSuccessRateIncPowUp)
        {
            defSuccessRateIncPowUp.MagicEffectDef = increaseBlockPowerUpEffect;

            if (defSuccessRateIncPowUp.MasterDefinition is { } masterDefinition)
            {
                masterDefinition.ValueFormula = $"{SkillsInitializer.Formula502} / 100";
                masterDefinition.TargetAttribute = increaseBlockBonus;
                masterDefinition.Aggregation = AggregateType.AddRaw;
            }
        }

        if (gameConfiguration.Skills.FirstOrDefault(s => s.Number == (short)SkillNumber.EquippedWeaponMastery)?.MasterDefinition is { } equippedWeaponMastery)
        {
            equippedWeaponMastery.ValueFormula = SkillsInitializer.Formula120Value;
            equippedWeaponMastery.TargetAttribute = gloveWeaponMasteryDoubleDamageChance;
            equippedWeaponMastery.Aggregation = AggregateType.AddRaw;
        }

        if (gameConfiguration.Skills.FirstOrDefault(s => s.Number == (short)SkillNumber.DefSuccessRateIncMastery) is { } defSuccessRateIncMastery)
        {
            defSuccessRateIncMastery.MagicEffectDef = increaseBlockMasteryEffect;

            if (defSuccessRateIncMastery.MasterDefinition is { } masterDefinition)
            {
                masterDefinition.ReplacedSkill = gameConfiguration.Skills.First(s => s.Number == (short)SkillNumber.DefSuccessRateIncPowUp);
                masterDefinition.TargetAttribute = Stats.DefenseFinal.GetPersistent(gameConfiguration);
                masterDefinition.Aggregation = AggregateType.AddFinal;
            }
        }

        if (gameConfiguration.Skills.FirstOrDefault(s => s.Number == (short)SkillNumber.StaminaIncreaseStrengthener) is { } staminaIncreaseStrengthener)
        {
            staminaIncreaseStrengthener.MagicEffectDef = increaseHealthStrengthenerEffect;

            if (staminaIncreaseStrengthener.MasterDefinition is { } masterDefinition)
            {
                masterDefinition.TargetAttribute = Stats.TotalVitality.GetPersistent(gameConfiguration);
                masterDefinition.Aggregation = AggregateType.AddFinal;
            }
        }

        if (gameConfiguration.Skills.FirstOrDefault(s => s.Number == (short)SkillNumber.DecreaseMana)?.MasterDefinition is { } decreaseMana)
        {
            decreaseMana.ValueFormula = SkillsInitializer.Formula722Value;
            decreaseMana.TargetAttribute = Stats.ManaUsageReduction.GetPersistent(gameConfiguration);
            decreaseMana.Aggregation = AggregateType.AddRaw;
        }
    }

    private MagicEffectDefinition CreateIncreaseBlockPowerUpEffect(IContext context, GameConfiguration gameConfiguration)
    {
        var magicEffect = context.CreateNew<MagicEffectDefinition>();
        gameConfiguration.MagicEffects.Add(magicEffect);
        magicEffect.Number = (byte)MagicEffectNumber.IncreaseBlockPowerUp;
        magicEffect.Name = "Increase Block Power Up Skill Effect";

        this.CopyMagicEffectValues(context, gameConfiguration, magicEffect, (short)MagicEffectNumber.IncreaseBlock);

        var increaseBlockPowerUp = context.CreateNew<PowerUpDefinition>();
        magicEffect.PowerUpDefinitions.Add(increaseBlockPowerUp);
        increaseBlockPowerUp.TargetAttribute = Stats.IncreaseBlockBonus.GetPersistent(gameConfiguration);
        increaseBlockPowerUp.Boost = context.CreateNew<PowerUpDefinitionValue>();
        increaseBlockPowerUp.Boost.ConstantValue.Value = 0f;
        increaseBlockPowerUp.Boost.ConstantValue.AggregateType = AggregateType.AddRaw;

        return magicEffect;
    }

    private MagicEffectDefinition CreateIncreaseBlockMasteryEffect(IContext context, GameConfiguration gameConfiguration)
    {
        var magicEffect = context.CreateNew<MagicEffectDefinition>();
        gameConfiguration.MagicEffects.Add(magicEffect);
        magicEffect.Number = (byte)MagicEffectNumber.IncreaseBlockMastery;
        magicEffect.Name = "Increase Block Mastery Skill Effect";

        this.CopyMagicEffectValues(context, gameConfiguration, magicEffect, (short)MagicEffectNumber.IncreaseBlockPowerUp);

        var defensePowerUp = context.CreateNew<PowerUpDefinition>();
        magicEffect.PowerUpDefinitions.Add(defensePowerUp);
        defensePowerUp.TargetAttribute = Stats.DefenseFinal.GetPersistent(gameConfiguration);
        defensePowerUp.Boost = context.CreateNew<PowerUpDefinitionValue>();
        defensePowerUp.Boost.ConstantValue.Value = 0f;
        defensePowerUp.Boost.ConstantValue.AggregateType = AggregateType.AddFinal;

        return magicEffect;
    }

    private MagicEffectDefinition CreateIncreaseHealthStrengthenerEffect(IContext context, GameConfiguration gameConfiguration)
    {
        var magicEffect = context.CreateNew<MagicEffectDefinition>();
        gameConfiguration.MagicEffects.Add(magicEffect);
        magicEffect.Number = (byte)MagicEffectNumber.IncreaseHealthStrengthener;
        magicEffect.Name = "Increase Health Strengthener Skill Effect";

        this.CopyMagicEffectValues(context, gameConfiguration, magicEffect, (short)MagicEffectNumber.IncreaseHealth);

        return magicEffect;
    }

    private MagicEffectDefinition CopyMagicEffectValues(IContext context, GameConfiguration gameConfiguration, MagicEffectDefinition targetMagicEffect, short sourceMagicEffectNumber)
    {
        var sourceMagicEffect = gameConfiguration.MagicEffects.First(e => e.Number == sourceMagicEffectNumber);
        targetMagicEffect.InformObservers = sourceMagicEffect.InformObservers;
        targetMagicEffect.SubType = sourceMagicEffect.SubType;
        targetMagicEffect.SendDuration = sourceMagicEffect.SendDuration;
        targetMagicEffect.StopByDeath = sourceMagicEffect.StopByDeath;
        targetMagicEffect.Duration = context.CreateNew<PowerUpDefinitionValue>();
        targetMagicEffect.Duration.ConstantValue.Value = sourceMagicEffect.Duration!.ConstantValue.Value;
        targetMagicEffect.Duration.ConstantValue.AggregateType = sourceMagicEffect.Duration.ConstantValue.AggregateType;
        targetMagicEffect.Duration.MaximumValue = sourceMagicEffect.Duration.MaximumValue;

        foreach (var durationRelatedValue in sourceMagicEffect.Duration.RelatedValues)
        {
            var durationRelatedValueCopy = context.CreateNew<AttributeRelationship>();
            durationRelatedValueCopy.InputAttribute = durationRelatedValue.InputAttribute!.GetPersistent(gameConfiguration);
            durationRelatedValueCopy.InputOperator = durationRelatedValue.InputOperator;
            durationRelatedValueCopy.InputOperand = durationRelatedValue.InputOperand;
            durationRelatedValueCopy.AggregateType = durationRelatedValue.AggregateType;
            targetMagicEffect.Duration.RelatedValues.Add(durationRelatedValueCopy);
        }

        foreach (var powerUp in sourceMagicEffect.PowerUpDefinitions)
        {
            var powerUpCopy = context.CreateNew<PowerUpDefinition>();
            targetMagicEffect.PowerUpDefinitions.Add(powerUpCopy);
            powerUpCopy.TargetAttribute = powerUp.TargetAttribute!.GetPersistent(gameConfiguration);
            powerUpCopy.Boost = context.CreateNew<PowerUpDefinitionValue>();
            powerUpCopy.Boost.ConstantValue.Value = powerUp.Boost!.ConstantValue.Value;
            powerUpCopy.Boost.ConstantValue.AggregateType = powerUp.Boost.ConstantValue.AggregateType;
            powerUpCopy.Boost.MaximumValue = powerUp.Boost.MaximumValue;

            foreach (var boostRelatedValue in powerUp.Boost.RelatedValues)
            {
                var boostRelatedValueCopy = context.CreateNew<AttributeRelationship>();
                boostRelatedValueCopy.InputAttribute = boostRelatedValue.InputAttribute!.GetPersistent(gameConfiguration);
                boostRelatedValueCopy.InputOperator = boostRelatedValue.InputOperator;
                boostRelatedValueCopy.InputOperand = boostRelatedValue.InputOperand;
                boostRelatedValueCopy.AggregateType = boostRelatedValue.AggregateType;
                powerUpCopy.Boost.RelatedValues.Add(boostRelatedValueCopy);
            }
        }

        return targetMagicEffect;
    }
}
