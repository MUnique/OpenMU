// <copyright file="FixRageFighterMultipleHitSkillsPlugIn.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.Persistence.Initialization.Updates;

using System.Runtime.InteropServices;
using MUnique.OpenMU.AttributeSystem;
using MUnique.OpenMU.DataModel;
using MUnique.OpenMU.DataModel.Attributes;
using MUnique.OpenMU.DataModel.Configuration;
using MUnique.OpenMU.GameLogic;
using MUnique.OpenMU.GameLogic.Attributes;
using MUnique.OpenMU.Persistence.Initialization.Skills;
using MUnique.OpenMU.PlugIns;

/// <summary>
/// This update adds the missing multiple hits to the Killing Blow, Beast Uppercut, Chain Drive, Dragon Roar and Phoenix Shot Rage Fighter skills, as well as their magic effects.
/// </summary>
[PlugIn(PlugInName, PlugInDescription)]
[Guid("EDDD17F9-BEA5-40F0-A653-8567566C40E7")]
public class FixRageFighterMultipleHitSkillsPlugIn : UpdatePlugInBase
{
    /// <summary>
    /// The plug in name.
    /// </summary>
    internal const string PlugInName = "Fix Rage Fighter Multiple Hit Skills";

    /// <summary>
    /// The plug in description.
    /// </summary>
    internal const string PlugInDescription = "This update adds the missing multiple hits to the Killing Blow, Beast Uppercut, Chain Drive, Dragon Roar and Phoenix Shot Rage Fighter skills, as well as their magic effects.";

    /// <inheritdoc />
    public override string Name => PlugInName;

    /// <inheritdoc />
    public override string Description => PlugInDescription;

    /// <inheritdoc />
    public override string DataInitializationKey => VersionSeasonSix.DataInitialization.Id;

    /// <inheritdoc />
    public override UpdateVersion Version => UpdateVersion.FixRageFighterMultipleHitSkills;

    /// <inheritdoc />
    public override bool IsMandatory => true;

    /// <inheritdoc />
    public override DateTime CreatedAt => new(2026, 1, 12, 16, 0, 0, DateTimeKind.Utc);

    /// <inheritdoc />
    protected override async ValueTask ApplyAsync(IContext context, GameConfiguration gameConfiguration)
    {
        // Update attributes
        var defenseDecrement = context.CreateNew<AttributeDefinition>(Stats.DefenseDecrement.Id, Stats.DefenseDecrement.Designation, Stats.DefenseDecrement.Description);
        gameConfiguration.Attributes.Add(defenseDecrement);

        var innovationDefDecrement = Stats.InnovationDefDecrement.GetPersistent(gameConfiguration);

        gameConfiguration.CharacterClasses.ForEach(charClass =>
        {
            // Update summoner berserker defense reduction attributes
            if (charClass.Number == 20 || charClass.Number == 22 || charClass.Number == 23) // Summoner classes
            {
                if (charClass.AttributeCombinations.FirstOrDefault(attr => attr.TargetAttribute == Stats.DefensePvm && attr.AggregateType == AggregateType.AddFinal) is { } finalBerserkerHealthDecrementToDefensePvm)
                {
                    finalBerserkerHealthDecrementToDefensePvm.TargetAttribute = Stats.DefenseFinal.GetPersistent(gameConfiguration);
                }

                if (charClass.AttributeCombinations.FirstOrDefault(attr => attr.TargetAttribute == Stats.DefensePvp && attr.AggregateType == AggregateType.AddFinal) is { } finalBerserkerHealthDecrementToDefensePvp)
                {
                    charClass.AttributeCombinations.Remove(finalBerserkerHealthDecrementToDefensePvp);
                }
            }

            // Add defense decrement relationships
            var tempInnovDefDec = context.CreateNew<AttributeDefinition>(Guid.NewGuid(), "Temp Innovation defense decrement", string.Empty);
            gameConfiguration.Attributes.Add(tempInnovDefDec);

            var innovationDefDecrementToTempInnovDefDec = context.CreateNew<AttributeRelationship>(
                tempInnovDefDec,
                -1,
                innovationDefDecrement,
                InputOperator.Multiply,
                default(AttributeDefinition?),
                AggregateType.AddRaw);

            var tempInnovDefDecToDefenseDecrement = context.CreateNew<AttributeRelationship>(
                defenseDecrement,
                1,
                tempInnovDefDec,
                InputOperator.Add,
                default(AttributeDefinition?),
                AggregateType.Multiplicate);

            charClass.AttributeCombinations.Add(innovationDefDecrementToTempInnovDefDec);
            charClass.AttributeCombinations.Add(tempInnovDefDecToDefenseDecrement);
            charClass.BaseAttributeValues.Add(context.CreateNew<ConstValueAttribute>(1, defenseDecrement));
        });

        // Update Increase Health (Vitality) magic effect
        if (gameConfiguration.MagicEffects.FirstOrDefault(m => m.Number == (short)MagicEffectNumber.IncreaseHealth) is { } increaseHealthEffect
            && increaseHealthEffect.PowerUpDefinitions.FirstOrDefault() is { } increaseHealthPowerUp)
        {
            increaseHealthPowerUp.Boost!.MaximumValue = 200f;
        }

        // Update Defense Reduction (Fire Slash) magic effect
        if (gameConfiguration.MagicEffects.FirstOrDefault(m => m.Number == (short)MagicEffectNumber.DefenseReduction) is { } defenseReductionEffect
            && defenseReductionEffect.PowerUpDefinitions.FirstOrDefault() is { } powerUp)
        {
            powerUp.TargetAttribute = defenseDecrement;
            defenseReductionEffect.PowerUpDefinitions.Clear();
            defenseReductionEffect.PowerUpDefinitions.Add(powerUp);
        }

        // Add new magic effects
        var defensereductionBeastUppercut = this.CreateDefenseReductionBeastUppercutMagicEffect(context, gameConfiguration);
        var decreaseBlockEffect = this.CreateDecreaseBlockMagicEffect(context, gameConfiguration);

        // Apply default value of NumberOfHitsPerAttack to all skills
        foreach (var skill in gameConfiguration.Skills)
        {
            skill.NumberOfHitsPerAttack = 1;
        }

        // Update existing skills
        if (gameConfiguration.Skills.FirstOrDefault(s => s.Number == (short)SkillNumber.KillingBlow) is { } killingBlow)
        {
            killingBlow.NumberOfHitsPerAttack = 4;
        }

        var killingBlowStrengthener = gameConfiguration.Skills.FirstOrDefault(s => s.Number == (short)SkillNumber.KillingBlowStrengthener);
        if (killingBlowStrengthener is not null)
        {
            killingBlowStrengthener.NumberOfHitsPerAttack = 4;
        }

        if (gameConfiguration.Skills.FirstOrDefault(s => s.Number == (short)SkillNumber.KillingBlowMastery) is { } killingBlowMastery)
        {
            killingBlowMastery.NumberOfHitsPerAttack = 4;
            killingBlowMastery.MasterDefinition!.ReplacedSkill = killingBlowStrengthener;
            killingBlowMastery.MasterDefinition.ValueFormula = $"{killingBlowMastery.MasterDefinition.ValueFormula} / 100";
            killingBlowMastery.MasterDefinition.TargetAttribute = Stats.WeaknessPhysDmgDecrement.GetPersistent(gameConfiguration);
            killingBlowMastery.MasterDefinition.Aggregation = AggregateType.AddRaw;
        }

        if (gameConfiguration.Skills.FirstOrDefault(s => s.Number == (short)SkillNumber.BeastUppercut) is { } beastUppercut)
        {
            beastUppercut.MagicEffectDef = defensereductionBeastUppercut;
            beastUppercut.NumberOfHitsPerAttack = 2;
        }

        var beastUppercutStrengthener = gameConfiguration.Skills.FirstOrDefault(s => s.Number == (short)SkillNumber.BeastUppercutStrengthener);
        if (beastUppercutStrengthener is not null)
        {
            beastUppercutStrengthener.MagicEffectDef = defensereductionBeastUppercut;
            beastUppercutStrengthener.NumberOfHitsPerAttack = 2;
        }

        if (gameConfiguration.Skills.FirstOrDefault(s => s.Number == (short)SkillNumber.BeastUppercutMastery) is { } beastUppercutMastery)
        {
            beastUppercutMastery.MagicEffectDef = defensereductionBeastUppercut;
            beastUppercutMastery.NumberOfHitsPerAttack = 2;
            beastUppercutMastery.MasterDefinition!.ReplacedSkill = beastUppercutStrengthener;
            beastUppercutMastery.MasterDefinition.ValueFormula = $"{beastUppercutMastery.MasterDefinition.ValueFormula} / -100";
            beastUppercutMastery.MasterDefinition.TargetAttribute = defenseDecrement;
            beastUppercutMastery.MasterDefinition.Aggregation = AggregateType.Multiplicate;
        }

        if (gameConfiguration.Skills.FirstOrDefault(s => s.Number == (short)SkillNumber.ChainDrive) is { } chainDrive)
        {
            chainDrive.MagicEffectDef!.Chance = context.CreateNew<PowerUpDefinitionValue>();
            chainDrive.MagicEffectDef.Chance.ConstantValue.Value = 0.4f;
            chainDrive.NumberOfHitsPerAttack = 4;
        }

        if (gameConfiguration.Skills.FirstOrDefault(s => s.Number == (short)SkillNumber.ChainDriveStrengthener) is { } chainDriveStrengthener)
        {
            chainDriveStrengthener.MagicEffectDef!.Chance = context.CreateNew<PowerUpDefinitionValue>();
            chainDriveStrengthener.MagicEffectDef.Chance.ConstantValue.Value = 0.4f;
            chainDriveStrengthener.NumberOfHitsPerAttack = 4;
        }

        if (gameConfiguration.Skills.FirstOrDefault(s => s.Number == (short)SkillNumber.DragonRoar) is { } dragonRoar)
        {
            dragonRoar.SkillType = SkillType.AreaSkillExplicitTarget;
            dragonRoar.NumberOfHitsPerAttack = 4;
        }

        if (gameConfiguration.Skills.FirstOrDefault(s => s.Number == (short)SkillNumber.DragonRoarStrengthener) is { } dragonRoarStrengthener)
        {
            dragonRoarStrengthener.SkillType = SkillType.AreaSkillExplicitTarget;
            dragonRoarStrengthener.NumberOfHitsPerAttack = 4;
        }

        if (gameConfiguration.Skills.FirstOrDefault(s => s.Number == (short)SkillNumber.PhoenixShot) is { } phoenixShot)
        {
            phoenixShot.MagicEffectDef = decreaseBlockEffect;
            phoenixShot.NumberOfHitsPerAttack = 4;
        }
    }

    private MagicEffectDefinition CreateDecreaseBlockMagicEffect(IContext context, GameConfiguration gameConfiguration)
    {
        var magicEffect = context.CreateNew<MagicEffectDefinition>();
        gameConfiguration.MagicEffects.Add(magicEffect);
        magicEffect.Number = (short)MagicEffectNumber.DecreaseBlock;
        magicEffect.Name = "Decrease Block Effect (Phoenix Shot)";
        magicEffect.InformObservers = true;
        magicEffect.SendDuration = false;
        magicEffect.StopByDeath = true;
        magicEffect.Duration = context.CreateNew<PowerUpDefinitionValue>();
        magicEffect.Duration.ConstantValue.Value = 10; // 10 Seconds
        magicEffect.Chance = context.CreateNew<PowerUpDefinitionValue>();
        magicEffect.Chance.ConstantValue.Value = 0.1f; // 10%

        var decDefRatePowerUpDefinition = context.CreateNew<PowerUpDefinition>();
        magicEffect.PowerUpDefinitions.Add(decDefRatePowerUpDefinition);
        decDefRatePowerUpDefinition.TargetAttribute = Stats.DefenseRatePvm.GetPersistent(gameConfiguration);
        decDefRatePowerUpDefinition.Boost = context.CreateNew<PowerUpDefinitionValue>();
        decDefRatePowerUpDefinition.Boost.ConstantValue.Value = -50f;
        decDefRatePowerUpDefinition.Boost.ConstantValue.AggregateType = AggregateType.AddFinal;

        var decDefRatePowerUpDefinitionPvp = context.CreateNew<PowerUpDefinition>();
        magicEffect.PowerUpDefinitionsPvp.Add(decDefRatePowerUpDefinitionPvp);
        decDefRatePowerUpDefinitionPvp.TargetAttribute = Stats.DefenseRatePvm.GetPersistent(gameConfiguration);
        decDefRatePowerUpDefinitionPvp.Boost = context.CreateNew<PowerUpDefinitionValue>();
        decDefRatePowerUpDefinitionPvp.Boost.ConstantValue.Value = -20f;
        decDefRatePowerUpDefinitionPvp.Boost.ConstantValue.AggregateType = AggregateType.AddFinal;

        return magicEffect;
    }

    private MagicEffectDefinition CreateDefenseReductionBeastUppercutMagicEffect(IContext context, GameConfiguration gameConfiguration)
    {
        var magicEffect = context.CreateNew<MagicEffectDefinition>();
        gameConfiguration.MagicEffects.Add(magicEffect);
        magicEffect.Number = (short)MagicEffectNumber.DefenseReduction; // We will map skill to effect by hand in this update, so we use this number instead of DefenseReductionBeastUppercut
        magicEffect.Name = "Defense Reduction Effect (Beast Uppercut)";
        magicEffect.InformObservers = true;
        magicEffect.SendDuration = true;
        magicEffect.StopByDeath = true;
        magicEffect.Duration = context.CreateNew<PowerUpDefinitionValue>();
        magicEffect.Duration.ConstantValue.Value = (float)TimeSpan.FromSeconds(10).TotalSeconds;
        magicEffect.Chance = context.CreateNew<PowerUpDefinitionValue>();
        magicEffect.Chance.ConstantValue.Value = 0.1f; // 10%

        var reduceDefenseEffect = context.CreateNew<PowerUpDefinition>();
        magicEffect.PowerUpDefinitions.Add(reduceDefenseEffect);
        reduceDefenseEffect.TargetAttribute = Stats.DefenseDecrement.GetPersistent(gameConfiguration);
        reduceDefenseEffect.Boost = context.CreateNew<PowerUpDefinitionValue>();
        reduceDefenseEffect.Boost.ConstantValue.Value = 0.9f; // 10% decrease
        reduceDefenseEffect.Boost.ConstantValue.AggregateType = AggregateType.Multiplicate;

        return magicEffect;
    }
}