// <copyright file="FixSkillMultipliersPlugIn.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.Persistence.Initialization.Updates;

using System.Runtime.InteropServices;
using MUnique.OpenMU.AttributeSystem;
using MUnique.OpenMU.DataModel.Attributes;
using MUnique.OpenMU.DataModel.Configuration;
using MUnique.OpenMU.GameLogic;
using MUnique.OpenMU.GameLogic.Attributes;
using MUnique.OpenMU.Persistence.Initialization.CharacterClasses;
using MUnique.OpenMU.Persistence.Initialization.Skills;
using MUnique.OpenMU.PlugIns;

/// <summary>
/// This update fixes RF skill multipliers and adds several skill-specific multipliers.
/// </summary>
[PlugIn(PlugInName, PlugInDescription)]
[Guid("753F01BA-5FCA-42FA-9587-7055631C27B7")]
public class FixSkillMultipliersPlugIn : UpdatePlugInBase
{
    /// <summary>
    /// The plug in name.
    /// </summary>
    internal const string PlugInName = "Fix Skill Multipliers";

    /// <summary>
    /// The plug in description.
    /// </summary>
    internal const string PlugInDescription = "This update fixes RF skill multipliers and adds several skill-specific multipliers.";

    /// <inheritdoc />
    public override string Name => PlugInName;

    /// <inheritdoc />
    public override string Description => PlugInDescription;

    /// <inheritdoc />
    public override string DataInitializationKey => VersionSeasonSix.DataInitialization.Id;

    /// <inheritdoc />
    public override UpdateVersion Version => UpdateVersion.FixSkillMultipliers;

    /// <inheritdoc />
    public override bool IsMandatory => true;

    /// <inheritdoc />
    public override DateTime CreatedAt => new(2025, 11, 28, 16, 0, 0, DateTimeKind.Utc);

    /// <inheritdoc />
    protected override async ValueTask ApplyAsync(IContext context, GameConfiguration gameConfiguration)
    {
        // Add SBRD max value
        Stats.SoulBarrierReceiveDecrement.GetPersistent(gameConfiguration).MaximumValue = 0.7f;

        // Add new attributes
        var vitalitySkillMultiplier = context.CreateNew<AttributeDefinition>(Stats.VitalitySkillMultiplier.Id, Stats.VitalitySkillMultiplier.Designation, Stats.VitalitySkillMultiplier.Description);
        gameConfiguration.Attributes.Add(vitalitySkillMultiplier);
        var explosionBonusDmg = context.CreateNew<AttributeDefinition>(Stats.ExplosionBonusDmg.Id, Stats.ExplosionBonusDmg.Designation, Stats.ExplosionBonusDmg.Description);
        gameConfiguration.Attributes.Add(explosionBonusDmg);
        var requiemBonusDmg = context.CreateNew<AttributeDefinition>(Stats.RequiemBonusDmg.Id, Stats.RequiemBonusDmg.Designation, Stats.RequiemBonusDmg.Description);
        gameConfiguration.Attributes.Add(requiemBonusDmg);
        var pollutionBonusDmg = context.CreateNew<AttributeDefinition>(Stats.PollutionBonusDmg.Id, Stats.PollutionBonusDmg.Designation, Stats.PollutionBonusDmg.Description);
        gameConfiguration.Attributes.Add(pollutionBonusDmg);
        var skillBaseMultiplier = context.CreateNew<AttributeDefinition>(Stats.SkillBaseMultiplier.Id, Stats.SkillBaseMultiplier.Designation, Stats.SkillBaseMultiplier.Description);
        gameConfiguration.Attributes.Add(skillBaseMultiplier);
        var skillBaseDamageBonus = context.CreateNew<AttributeDefinition>(Stats.SkillBaseDamageBonus.Id, Stats.SkillBaseDamageBonus.Designation, Stats.SkillBaseDamageBonus.Description);
        gameConfiguration.Attributes.Add(skillBaseDamageBonus);
        var skillFinalMultiplier = context.CreateNew<AttributeDefinition>(Stats.SkillFinalMultiplier.Id, Stats.SkillFinalMultiplier.Designation, Stats.SkillFinalMultiplier.Description);
        gameConfiguration.Attributes.Add(skillFinalMultiplier);
        var skillFinalDamageBonus = context.CreateNew<AttributeDefinition>(Stats.SkillFinalDamageBonus.Id, Stats.SkillFinalDamageBonus.Designation, Stats.SkillFinalDamageBonus.Description);
        gameConfiguration.Attributes.Add(skillFinalDamageBonus);

        var totalEnergy = Stats.TotalEnergy.GetPersistent(gameConfiguration);
        var totalVitality = Stats.TotalVitality.GetPersistent(gameConfiguration);
        var skillMultiplier = Stats.SkillMultiplier.GetPersistent(gameConfiguration);

        // Fix RF classes skill multiplier
        gameConfiguration.CharacterClasses.ForEach(charClass =>
        {
            if (charClass.Number == 24 || charClass.Number == 25) // RF classes
            {
                if (charClass.BaseAttributeValues.FirstOrDefault(a => a.Definition == Stats.SkillMultiplier) is { } skillMult)
                {
                    charClass.BaseAttributeValues.Remove(skillMult);
                    charClass.BaseAttributeValues.Add(context.CreateNew<ConstValueAttribute>(0.5f, skillMultiplier));
                }

                var totalEnergyToSkillMultiplier = context.CreateNew<AttributeRelationship>(
                    skillMultiplier,
                    0.001f,
                    totalEnergy,
                    InputOperator.Multiply,
                    default(AttributeDefinition?),
                    AggregateType.AddRaw);

                var totalVitalityToVitalitySkillMultiplier = context.CreateNew<AttributeRelationship>(
                    vitalitySkillMultiplier,
                    0.001f,
                    totalVitality,
                    InputOperator.Multiply,
                    default(AttributeDefinition?),
                    AggregateType.AddRaw);

                charClass.AttributeCombinations.Add(totalEnergyToSkillMultiplier);
                charClass.AttributeCombinations.Add(totalVitalityToVitalitySkillMultiplier);
                charClass.BaseAttributeValues.Add(context.CreateNew<ConstValueAttribute>(0.5f, vitalitySkillMultiplier));
            }
        });

        // Update Increase Health magic effect
        if (gameConfiguration.MagicEffects.FirstOrDefault(m => m.Number == (short)MagicEffectNumber.IncreaseHealth) is { } increaseHealthEffect)
        {
            if (increaseHealthEffect.PowerUpDefinitions.FirstOrDefault() is { } powerUp)
            {
                powerUp.TargetAttribute = totalVitality;
            }
        }

        // Create Weakness magic effect
        var magicEffect = context.CreateNew<MagicEffectDefinition>();
        gameConfiguration.MagicEffects.Add(magicEffect);
        magicEffect.Number = (short)MagicEffectNumber.Weakness;
        magicEffect.Name = "Weakness Effect";
        magicEffect.InformObservers = true;
        magicEffect.SendDuration = false;
        magicEffect.StopByDeath = true;
        magicEffect.Duration = context.CreateNew<PowerUpDefinitionValue>();
        magicEffect.Duration.ConstantValue.Value = 10; // 10 seconds

        var decDmgPowerUpDefinition = context.CreateNew<PowerUpDefinition>();
        magicEffect.PowerUpDefinitions.Add(decDmgPowerUpDefinition);
        decDmgPowerUpDefinition.TargetAttribute = Stats.WeaknessPhysDmgDecrement.GetPersistent(gameConfiguration);
        decDmgPowerUpDefinition.Boost = context.CreateNew<PowerUpDefinitionValue>();
        decDmgPowerUpDefinition.Boost.ConstantValue.Value = 0.05f;
        decDmgPowerUpDefinition.Boost.ConstantValue.AggregateType = AggregateType.AddRaw;

        // Add Wakness magic effect to Killing Blow skills
        if (gameConfiguration.Skills.FirstOrDefault(s => s.Number == (short)SkillNumber.KillingBlow) is { } killingBlow)
        {
            killingBlow.MagicEffectDef = magicEffect;
        }

        if (gameConfiguration.Skills.FirstOrDefault(s => s.Number == (short)SkillNumber.KillingBlowStrengthener) is { } killingBlowStr)
        {
            killingBlowStr.MagicEffectDef = magicEffect;
        }

        if (gameConfiguration.Skills.FirstOrDefault(s => s.Number == (short)SkillNumber.KillingBlowMastery) is { } killingBlowMastery)
        {
            killingBlowMastery.MagicEffectDef = magicEffect;
        }

        // Add Selupan's skill multiplier
        if (gameConfiguration.Monsters.FirstOrDefault(m => m.Number == 459) is { } selupan)
        {
            selupan.AddAttributes(new Dictionary<AttributeDefinition, float> { { skillMultiplier, 2 } }, context, gameConfiguration);
        }

        // Update skill attribute relationships
        foreach (var skill in gameConfiguration.Skills)
        {
            if (skill.Number == (short)SkillNumber.Nova
                || skill.Number == (short)SkillNumber.Earthshake
                || skill.Number == (short)SkillNumber.ElectricSpike
                || skill.Number == (short)SkillNumber.ChaoticDiseier
                || skill.Number == (short)SkillNumber.Force
                || skill.Number == (short)SkillNumber.FireBlast
                || skill.Number == (short)SkillNumber.FireBurst
                || skill.Number == (short)SkillNumber.ForceWave
                || skill.Number == (short)SkillNumber.FireScream)
            {
                skill.AttributeRelationships.ForEach(rel => { rel.TargetAttribute = skillBaseDamageBonus; });
                continue;
            }

            // Not really a fix, more to standardize
            if (skill.Number == (short)SkillNumber.MultiShot)
            {
                skill.AttributeRelationships.ForEach(rel =>
                {
                    rel.AggregateType = AggregateType.AddRaw;
                });
            }
        }

        // Add new skill attribute relationships
        AddAttributeRelationship(SkillNumber.FallingSlash, Stats.SkillFinalMultiplier, 2.0f, Stats.SkillMultiplier, InputOperator.Maximum);

        AddAttributeRelationship(SkillNumber.IceArrow, Stats.SkillFinalMultiplier, 2.0f, Stats.SkillMultiplier);
        AddAttributeRelationship(SkillNumber.Penetration, Stats.SkillFinalMultiplier, 2.0f, Stats.SkillMultiplier);
        AddAttributeRelationship(SkillNumber.Starfall, Stats.SkillFinalMultiplier, 2.0f, Stats.SkillMultiplier);

        AddAttributeRelationship(SkillNumber.Explosion223, Stats.SkillFinalDamageBonus, 1.0f, Stats.ExplosionBonusDmg);
        AddAttributeRelationship(SkillNumber.Requiem, Stats.SkillFinalDamageBonus, 1.0f, Stats.RequiemBonusDmg);
        AddAttributeRelationship(SkillNumber.Pollution, Stats.SkillFinalDamageBonus, 1.0f, Stats.PollutionBonusDmg);

        AddAttributeRelationship(SkillNumber.PlasmaStorm, Stats.SkillFinalMultiplier, 0.002f, Stats.TotalLevel);
        AddAttributeRelationship(SkillNumber.PlasmaStorm, Stats.SkillFinalMultiplier, -0.6f, Stats.MaximumHealth, InputOperator.Minimum); // 0.002 * 300(min lvl)
        AddAttributeRelationship(SkillNumber.PlasmaStorm, Stats.SkillFinalMultiplier, 2.0f, Stats.MaximumHealth, InputOperator.Minimum);

        AddAttributeRelationship(SkillNumber.ChaoticDiseier, Stats.SkillFinalMultiplier, 0.8f, Stats.SkillMultiplier);

        AddAttributeRelationship(SkillNumber.KillingBlow, Stats.SkillFinalMultiplier, 1.0f, Stats.VitalitySkillMultiplier);
        AddAttributeRelationship(SkillNumber.BeastUppercut, Stats.SkillFinalMultiplier, 1.0f, Stats.VitalitySkillMultiplier);
        AddAttributeRelationship(SkillNumber.ChainDrive, Stats.SkillFinalMultiplier, 1.0f, Stats.VitalitySkillMultiplier);
        AddAttributeRelationship(SkillNumber.Charge, Stats.SkillFinalMultiplier, 1.0f, Stats.VitalitySkillMultiplier);
        AddAttributeRelationship(SkillNumber.PhoenixShot, Stats.SkillFinalMultiplier, 1.0f, Stats.VitalitySkillMultiplier);

        AddAttributeRelationship(SkillNumber.DarkSide, Stats.SkillFinalMultiplier, 0.5f, Stats.SkillMultiplier, InputOperator.Add);
        AddAttributeRelationship(SkillNumber.DarkSide, Stats.SkillFinalMultiplier, 1.0f / 800, Stats.TotalAgility);

        AddAttributeRelationship(SkillNumber.DragonRoar, Stats.SkillFinalMultiplier, 1.0f, Stats.SkillMultiplier);
        AddAttributeRelationship(SkillNumber.DragonSlasher, Stats.SkillFinalMultiplier, 1.0f, Stats.SkillMultiplier);

        void AddAttributeRelationship(SkillNumber skillNumber, AttributeDefinition targetAttribute, float multiplier, AttributeDefinition sourceAttribute, InputOperator inputOperator = InputOperator.Multiply, AggregateType aggregateType = AggregateType.AddRaw)
        {
            var skill = gameConfiguration.Skills.First(s => s.Number == (int)skillNumber);
            var relationship = CharacterClassHelper.CreateAttributeRelationship(context, gameConfiguration, targetAttribute, multiplier, sourceAttribute, inputOperator, aggregateType);
            skill.AttributeRelationships.Add(relationship);
        }

        // Update sum master skills
        if (gameConfiguration.Skills.FirstOrDefault(s => s.Number == (short)SkillNumber.FireTomeStrengthener)?.MasterDefinition is { } fireTomeStr)
        {
            fireTomeStr.TargetAttribute = explosionBonusDmg;
        }

        if (gameConfiguration.Skills.FirstOrDefault(s => s.Number == (short)SkillNumber.WindTomeStrengthener)?.MasterDefinition is { } windTomeStr)
        {
            windTomeStr.TargetAttribute = requiemBonusDmg;
        }

        if (gameConfiguration.Skills.FirstOrDefault(s => s.Number == (short)SkillNumber.LightningTomeStren)?.MasterDefinition is { } lightningTomeStr)
        {
            lightningTomeStr.TargetAttribute = pollutionBonusDmg;
        }
    }
}