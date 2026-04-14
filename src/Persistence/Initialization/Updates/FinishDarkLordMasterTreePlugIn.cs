// <copyright file="FinishDarkLordMasterTreePlugIn.cs" company="MUnique">
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
/// This update completes the dark lord master tree skills and effects.
/// </summary>
[PlugIn]
[Display(Name = PlugInName, Description = PlugInDescription)]
[Guid("1A2B3C4D-5E6F-7890-ABCD-EF1234567890")]
public class FinishDarkLordMasterTreePlugIn : UpdatePlugInBase
{
    /// <summary>
    /// The plug in name.
    /// </summary>
    internal const string PlugInName = "Finish Dark Lord Master Tree PlugIn";

    /// <summary>
    /// The plug in description.
    /// </summary>
    internal const string PlugInDescription = "This update completes the dark lord master tree skills and effects.";

    /// <inheritdoc />
    public override UpdateVersion Version => UpdateVersion.FinishDarkLordMasterTree;

    /// <inheritdoc />
    public override string DataInitializationKey => VersionSeasonSix.DataInitialization.Id;

    /// <inheritdoc />
    public override string Name => PlugInName;

    /// <inheritdoc />
    public override string Description => PlugInDescription;

    /// <inheritdoc />
    public override bool IsMandatory => true;

    /// <inheritdoc />
    public override DateTime CreatedAt => new(2026, 4, 14, 16, 0, 0, DateTimeKind.Utc);

    /// <inheritdoc />
    protected override async ValueTask ApplyAsync(IContext context, GameConfiguration gameConfiguration)
    {
        // Create Critical Damage Increase Mastery Skill Effect
        var critDmgIncMasteryEffect = this.CreateCritDmgIncMasteryEffect(context, gameConfiguration);

        // Update Stunned effect
        var stunnedEffect = gameConfiguration.MagicEffects.First(e => e.Number == (short)MagicEffectNumber.Stunned);
        stunnedEffect.Duration = context.CreateNew<PowerUpDefinitionValue>();
        stunnedEffect.Duration.ConstantValue.Value = 2;

        var stunChancePowerUpDefinition = context.CreateNew<PowerUpDefinition>();
        stunnedEffect.PowerUpDefinitions.Add(stunChancePowerUpDefinition);
        stunChancePowerUpDefinition.TargetAttribute = Stats.StunChance.GetPersistent(gameConfiguration);
        stunChancePowerUpDefinition.Boost = context.CreateNew<PowerUpDefinitionValue>();
        stunChancePowerUpDefinition.Boost.ConstantValue.Value = 0;

        // Map skills to effects
        this.MapSkillToEffect(gameConfiguration, SkillNumber.FireBurstMastery, stunnedEffect);
        this.MapSkillToEffect(gameConfiguration, SkillNumber.EarthshakeMastery, stunnedEffect);
        this.MapSkillToEffect(gameConfiguration, SkillNumber.CritDmgIncPowUp3, critDmgIncMasteryEffect);

        // Update AreaSkillSettings
        this.AddAreaSkillSettings(gameConfiguration, context, SkillNumber.Earthshake, false, 0, 0, 0, useTargetAreaFilter: true, targetAreaDiameter: 10, minimumHitsPerAttack: 9, maximumHitsPerAttack: 15);
        this.AddAreaSkillSettings(gameConfiguration, context, SkillNumber.EarthshakeStreng, false, 0, 0, 0, useTargetAreaFilter: true, targetAreaDiameter: 10, minimumHitsPerAttack: 9, maximumHitsPerAttack: 15);
        this.AddAreaSkillSettings(gameConfiguration, context, SkillNumber.EarthshakeMastery, false, 0, 0, 0, useTargetAreaFilter: true, targetAreaDiameter: 10, minimumHitsPerAttack: 9, maximumHitsPerAttack: 15);

        if (gameConfiguration.Skills.FirstOrDefault(s => s.Number == (short)SkillNumber.ChaoticDiseier) is { } chaoticDiseier)
        {
            chaoticDiseier.AreaSkillSettings!.MinimumNumberOfHitsPerAttack = 7;
        }

        // Update master skills
        if (gameConfiguration.Skills.FirstOrDefault(s => s.Number == (short)SkillNumber.CriticalDmgIncPowUp)?.MasterDefinition is { } fireTomeMastery)
        {
            fireTomeMastery.TargetAttribute = Stats.CriticalDamageBonus.GetPersistent(gameConfiguration);
            fireTomeMastery.Aggregation = AggregateType.AddRaw;
        }

        if (gameConfiguration.Skills.FirstOrDefault(s => s.Number == (short)SkillNumber.FireBurstMastery)?.MasterDefinition is { } fireBurstMastery)
        {
            fireBurstMastery.ReplacedSkill = gameConfiguration.Skills.First(s => s.Number == (short)SkillNumber.FireBurstStreng);
            fireBurstMastery.TargetAttribute = Stats.StunChance.GetPersistent(gameConfiguration);
            fireBurstMastery.Aggregation = AggregateType.AddRaw;
            fireBurstMastery.ValueFormula = $"{fireBurstMastery.ValueFormula} / 100";
        }

        if (gameConfiguration.Skills.FirstOrDefault(s => s.Number == (short)SkillNumber.CritDmgIncPowUp2)?.MasterDefinition is { } critDmgIncPowUp2)
        {
            critDmgIncPowUp2.ReplacedSkill = gameConfiguration.Skills.First(s => s.Number == (short)SkillNumber.CriticalDmgIncPowUp);
            critDmgIncPowUp2.ExtendsDuration = true;
        }

        if (gameConfiguration.Skills.FirstOrDefault(s => s.Number == (short)SkillNumber.EarthshakeMastery)?.MasterDefinition is { } earthshakeMastery)
        {
            earthshakeMastery.ReplacedSkill = gameConfiguration.Skills.First(s => s.Number == (short)SkillNumber.EarthshakeStreng);
            earthshakeMastery.TargetAttribute = Stats.StunChance.GetPersistent(gameConfiguration);
            earthshakeMastery.Aggregation = AggregateType.AddRaw;
            earthshakeMastery.ValueFormula = $"{earthshakeMastery.ValueFormula} / 100";
        }

        if (gameConfiguration.Skills.FirstOrDefault(s => s.Number == (short)SkillNumber.CritDmgIncPowUp3)?.MasterDefinition is { } critDmgIncPowUp3)
        {
            critDmgIncPowUp3.ReplacedSkill = gameConfiguration.Skills.First(s => s.Number == (short)SkillNumber.CritDmgIncPowUp2);
            critDmgIncPowUp3.TargetAttribute = Stats.CriticalDamageChance.GetPersistent(gameConfiguration);
            critDmgIncPowUp3.Aggregation = AggregateType.AddRaw;
            critDmgIncPowUp3.ValueFormula = $"{critDmgIncPowUp3.ValueFormula} / 100";
        }
    }

    private MagicEffectDefinition CreateCritDmgIncMasteryEffect(IContext context, GameConfiguration gameConfiguration)
    {
        var magicEffect = context.CreateNew<MagicEffectDefinition>();
        gameConfiguration.MagicEffects.Add(magicEffect);
        magicEffect.Number = (byte)MagicEffectNumber.CriticalDamageIncreaseMastery;
        magicEffect.Name = "Critical Damage Increase Mastery Skill Effect";

        var critDmgIncEffect = gameConfiguration.MagicEffects.First(e => e.Number == (short)MagicEffectNumber.CriticalDamageIncrease);
        critDmgIncEffect.Duration?.MaximumValue = 180;
        critDmgIncEffect.SubType = 17;
        magicEffect.InformObservers = critDmgIncEffect.InformObservers;
        magicEffect.SubType = critDmgIncEffect.SubType;
        magicEffect.SendDuration = critDmgIncEffect.SendDuration;
        magicEffect.StopByDeath = critDmgIncEffect.StopByDeath;
        magicEffect.Duration = context.CreateNew<PowerUpDefinitionValue>();
        magicEffect.Duration.ConstantValue.Value = critDmgIncEffect.Duration!.ConstantValue.Value;
        magicEffect.Duration.MaximumValue = critDmgIncEffect.Duration.MaximumValue;

        foreach (var durationRelatedValue in critDmgIncEffect.Duration.RelatedValues)
        {
            var durationRelatedValueCopy = context.CreateNew<AttributeRelationship>();
            durationRelatedValueCopy.InputAttribute = durationRelatedValue.InputAttribute!.GetPersistent(gameConfiguration);
            durationRelatedValueCopy.InputOperator = durationRelatedValue.InputOperator;
            durationRelatedValueCopy.InputOperand = durationRelatedValue.InputOperand;
            magicEffect.Duration.RelatedValues.Add(durationRelatedValueCopy);
        }

        foreach (var powerUp in critDmgIncEffect.PowerUpDefinitions)
        {
            var powerUpCopy = context.CreateNew<PowerUpDefinition>();
            magicEffect.PowerUpDefinitions.Add(powerUpCopy);
            powerUpCopy.TargetAttribute = powerUp.TargetAttribute!.GetPersistent(gameConfiguration);
            powerUpCopy.Boost = context.CreateNew<PowerUpDefinitionValue>();
            powerUpCopy.Boost.ConstantValue.Value = powerUp.Boost!.ConstantValue.Value;

            foreach (var boostRelatedValue in powerUp.Boost.RelatedValues)
            {
                var boostRelatedValueCopy = context.CreateNew<AttributeRelationship>();
                boostRelatedValueCopy.InputAttribute = boostRelatedValue.InputAttribute!.GetPersistent(gameConfiguration);
                boostRelatedValueCopy.InputOperator = boostRelatedValue.InputOperator;
                boostRelatedValueCopy.InputOperand = boostRelatedValue.InputOperand;
                powerUpCopy.Boost.RelatedValues.Add(boostRelatedValueCopy);
            }
        }

        var critChancePowerUpDefinition = context.CreateNew<PowerUpDefinition>();
        magicEffect.PowerUpDefinitions.Add(critChancePowerUpDefinition);
        critChancePowerUpDefinition.TargetAttribute = Stats.CriticalDamageChance.GetPersistent(gameConfiguration);
        critChancePowerUpDefinition.Boost = context.CreateNew<PowerUpDefinitionValue>();
        critChancePowerUpDefinition.Boost.ConstantValue.Value = 0;

        return magicEffect;
    }

    private void MapSkillToEffect(GameConfiguration gameConfiguration, SkillNumber skillNumber, MagicEffectDefinition magicEffect)
    {
        if (gameConfiguration.Skills.FirstOrDefault(s => s.Number == (short)skillNumber) is { } skill)
        {
            skill.MagicEffectDef = magicEffect;
        }
    }

    private void AddAreaSkillSettings(
        GameConfiguration gameConfiguration,
        IContext context,
        SkillNumber skillNumber,
        bool useFrustumFilter,
        float frustumStartWidth,
        float frustumEndWidth,
        float frustumDistance,
        bool useDeferredHits = false,
        TimeSpan delayPerOneDistance = default,
        TimeSpan delayBetweenHits = default,
        int minimumHitsPerTarget = 1,
        int maximumHitsPerTarget = 1,
        int minimumHitsPerAttack = default,
        int maximumHitsPerAttack = default,
        float hitChancePerDistanceMultiplier = 1.0f,
        bool useTargetAreaFilter = false,
        float targetAreaDiameter = default,
        int projectileCount = 1,
        int effectRange = default)
    {
        if (gameConfiguration.Skills.FirstOrDefault(s => s.Number == (short)skillNumber) is not { } skill)
        {
            return;
        }

        skill.SkillType = SkillType.AreaSkillAutomaticHits;
        var areaSkillSettings = context.CreateNew<AreaSkillSettings>();
        skill.AreaSkillSettings = areaSkillSettings;

        areaSkillSettings.UseFrustumFilter = useFrustumFilter;
        areaSkillSettings.FrustumStartWidth = frustumStartWidth;
        areaSkillSettings.FrustumEndWidth = frustumEndWidth;
        areaSkillSettings.FrustumDistance = frustumDistance;
        areaSkillSettings.UseTargetAreaFilter = useTargetAreaFilter;
        areaSkillSettings.TargetAreaDiameter = targetAreaDiameter;
        areaSkillSettings.UseDeferredHits = useDeferredHits;
        areaSkillSettings.DelayPerOneDistance = delayPerOneDistance;
        areaSkillSettings.DelayBetweenHits = delayBetweenHits;
        areaSkillSettings.MinimumNumberOfHitsPerTarget = minimumHitsPerTarget;
        areaSkillSettings.MaximumNumberOfHitsPerTarget = maximumHitsPerTarget;
        areaSkillSettings.MinimumNumberOfHitsPerAttack = minimumHitsPerAttack;
        areaSkillSettings.MaximumNumberOfHitsPerAttack = maximumHitsPerAttack;
        areaSkillSettings.HitChancePerDistanceMultiplier = hitChancePerDistanceMultiplier;
        areaSkillSettings.ProjectileCount = projectileCount;
        areaSkillSettings.EffectRange = effectRange;
    }
}
