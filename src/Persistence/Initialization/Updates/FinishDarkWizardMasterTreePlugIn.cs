// <copyright file="FinishDarkWizardMasterTreePlugIn.cs" company="MUnique">
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
/// This update completes the dark wizard master tree expansion of wizardry effects.
/// </summary>
[PlugIn]
[Display(Name = PlugInName, Description = PlugInDescription)]
[Guid("C1D2E3F4-5A6B-7C8D-9E0F-1A2B3C4D5E6F")]
public class FinishDarkWizardMasterTreePlugIn : UpdatePlugInBase
{
    /// <summary>
    /// The plug in name.
    /// </summary>
    internal const string PlugInName = "Finish Dark Wizard Master Tree PlugIn";

    /// <summary>
    /// The plug in description.
    /// </summary>
    internal const string PlugInDescription = "This update completes the dark wizard master tree expansion of wizardry effects.";

    /// <inheritdoc />
    public override UpdateVersion Version => UpdateVersion.FinishDarkWizardMasterTree;

    /// <inheritdoc />
    public override string DataInitializationKey => VersionSeasonSix.DataInitialization.Id;

    /// <inheritdoc />
    public override string Name => PlugInName;

    /// <inheritdoc />
    public override string Description => PlugInDescription;

    /// <inheritdoc />
    public override bool IsMandatory => true;

    /// <inheritdoc />
    public override DateTime CreatedAt => new(2026, 7, 7, 16, 0, 0, DateTimeKind.Utc);

    /// <inheritdoc />
    protected override async ValueTask ApplyAsync(IContext context, GameConfiguration gameConfiguration)
    {
        // Update Wizardry Enhance effect
        var wizardryEnhanceEffect = gameConfiguration.MagicEffects.First(e => e.Number == (short)MagicEffectNumber.WizEnhance);
        wizardryEnhanceEffect.Duration = context.CreateNew<PowerUpDefinitionValue>();
        wizardryEnhanceEffect.Duration.ConstantValue.Value = 1800f; // 30 minutes
        wizardryEnhanceEffect.PowerUpDefinitions.Clear();

        var minWizDmgPowerUp = context.CreateNew<PowerUpDefinition>();
        wizardryEnhanceEffect.PowerUpDefinitions.Add(minWizDmgPowerUp);
        minWizDmgPowerUp.TargetAttribute = Stats.MinimumWizBaseDmg.GetPersistent(gameConfiguration);
        minWizDmgPowerUp.Boost = context.CreateNew<PowerUpDefinitionValue>();
        minWizDmgPowerUp.Boost.MaximumValue = 100f; // 100 dmg

        var minWizDmgPerEnergy = context.CreateNew<AttributeRelationship>();
        minWizDmgPerEnergy.InputAttribute = Stats.TotalEnergy.GetPersistent(gameConfiguration);
        minWizDmgPerEnergy.InputOperator = InputOperator.Multiply;
        minWizDmgPerEnergy.InputOperand = 0.2f / 9; // 45 energy adds 1 min wiz damage
        minWizDmgPowerUp.Boost.RelatedValues.Add(minWizDmgPerEnergy);

        // Create Wiz Enhance Strengthener & Mastery Skill Effects
        var wizEnhanceStrengthenerEffect = this.CreateWizEnhanceStrengthenerEffect(context, gameConfiguration);
        var wizEnhanceMasteryEffect = this.CreateWizEnhanceMasteryEffect(context, gameConfiguration);

        // Update master skills
        if (gameConfiguration.Skills.FirstOrDefault(s => s.Number == (short)SkillNumber.ExpansionofWizStreng) is { } expansionOfWizStreng)
        {
            expansionOfWizStreng.MagicEffectDef = wizEnhanceStrengthenerEffect;
        }

        if (gameConfiguration.Skills.FirstOrDefault(s => s.Number == (short)SkillNumber.ExpansionofWizMas) is { } expansionOfWizMastery)
        {
            expansionOfWizMastery.MagicEffectDef = wizEnhanceMasteryEffect;

            if (expansionOfWizMastery.MasterDefinition is { } masterDefinition)
            {
                masterDefinition.ReplacedSkill = gameConfiguration.Skills.First(s => s.Number == (short)SkillNumber.ExpansionofWizStreng);
                masterDefinition.Aggregation = AggregateType.AddRaw;
            }
        }
    }

    private MagicEffectDefinition CreateWizEnhanceStrengthenerEffect(IContext context, GameConfiguration gameConfiguration)
    {
        var magicEffect = context.CreateNew<MagicEffectDefinition>();
        gameConfiguration.MagicEffects.Add(magicEffect);
        magicEffect.Number = (byte)MagicEffectNumber.WizEnhanceStrengthener;
        magicEffect.Name = "Wizardry Enhance Strengthener Skill Effect";

        var wizardryEnhanceEffect = gameConfiguration.MagicEffects.First(e => e.Number == (short)MagicEffectNumber.WizEnhance);
        magicEffect.InformObservers = wizardryEnhanceEffect.InformObservers;
        magicEffect.SubType = wizardryEnhanceEffect.SubType;
        magicEffect.SendDuration = wizardryEnhanceEffect.SendDuration;
        magicEffect.StopByDeath = wizardryEnhanceEffect.StopByDeath;
        magicEffect.Duration = context.CreateNew<PowerUpDefinitionValue>();
        magicEffect.Duration.ConstantValue.Value = wizardryEnhanceEffect.Duration!.ConstantValue.Value;
        magicEffect.Duration.MaximumValue = wizardryEnhanceEffect.Duration.MaximumValue;

        foreach (var powerUp in wizardryEnhanceEffect.PowerUpDefinitions)
        {
            var powerUpCopy = context.CreateNew<PowerUpDefinition>();
            magicEffect.PowerUpDefinitions.Add(powerUpCopy);
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

        var maxDmgPowerUp = context.CreateNew<PowerUpDefinition>();
        magicEffect.PowerUpDefinitions.Add(maxDmgPowerUp);
        maxDmgPowerUp.TargetAttribute = Stats.MaximumWizBaseDmg.GetPersistent(gameConfiguration);
        maxDmgPowerUp.Boost = context.CreateNew<PowerUpDefinitionValue>();
        maxDmgPowerUp.Boost.ConstantValue.Value = 1f;
        maxDmgPowerUp.Boost.ConstantValue.AggregateType = AggregateType.Multiplicate;

        return magicEffect;
    }

    private MagicEffectDefinition CreateWizEnhanceMasteryEffect(IContext context, GameConfiguration gameConfiguration)
    {
        var magicEffect = context.CreateNew<MagicEffectDefinition>();
        gameConfiguration.MagicEffects.Add(magicEffect);
        magicEffect.Number = (byte)MagicEffectNumber.WizEnhanceMastery;
        magicEffect.Name = "Wizardry Enhance Mastery Skill Effect";

        var wizardryEnhanceEffect = gameConfiguration.MagicEffects.First(e => e.Number == (short)MagicEffectNumber.WizEnhanceStrengthener);
        magicEffect.InformObservers = wizardryEnhanceEffect.InformObservers;
        magicEffect.SubType = wizardryEnhanceEffect.SubType;
        magicEffect.SendDuration = wizardryEnhanceEffect.SendDuration;
        magicEffect.StopByDeath = wizardryEnhanceEffect.StopByDeath;
        magicEffect.Duration = context.CreateNew<PowerUpDefinitionValue>();
        magicEffect.Duration.ConstantValue.Value = wizardryEnhanceEffect.Duration!.ConstantValue.Value;
        magicEffect.Duration.MaximumValue = wizardryEnhanceEffect.Duration.MaximumValue;

        foreach (var powerUp in wizardryEnhanceEffect.PowerUpDefinitions)
        {
            var powerUpCopy = context.CreateNew<PowerUpDefinition>();
            magicEffect.PowerUpDefinitions.Add(powerUpCopy);
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

        var critChancePowerUp = context.CreateNew<PowerUpDefinition>();
        magicEffect.PowerUpDefinitions.Add(critChancePowerUp);
        critChancePowerUp.TargetAttribute = Stats.CriticalDamageChance.GetPersistent(gameConfiguration);
        critChancePowerUp.Boost = context.CreateNew<PowerUpDefinitionValue>();

        return magicEffect;
    }
}
