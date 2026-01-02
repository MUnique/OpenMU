// <copyright file="FixRageFighterMultipleHitSkillsPlugIn.cs" company="MUnique">
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
    public override DateTime CreatedAt => new(2026, 1, 3, 16, 0, 0, DateTimeKind.Utc);

    /// <inheritdoc />
    protected override async ValueTask ApplyAsync(IContext context, GameConfiguration gameConfiguration)
    {
        // Update Increase Health (Vitality) magic effect
        if (gameConfiguration.MagicEffects.FirstOrDefault(m => m.Number == (short)MagicEffectNumber.IncreaseHealth) is { } increaseHealthEffect
            && increaseHealthEffect.PowerUpDefinitions.FirstOrDefault() is { } increaseHealthPowerUp)
        {
            increaseHealthPowerUp.Boost!.MaximumValue = 200f;
        }

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

        if (gameConfiguration.Skills.FirstOrDefault(s => s.Number == (short)SkillNumber.KillingBlowStrengthener) is { } killingBlowStrengthener)
        {
            killingBlowStrengthener.NumberOfHitsPerAttack = 4;
        }

        if (gameConfiguration.Skills.FirstOrDefault(s => s.Number == (short)SkillNumber.KillingBlowMastery) is { } killingBlowMastery)
        {
            killingBlowMastery.NumberOfHitsPerAttack = 4;
        }

        if (gameConfiguration.Skills.FirstOrDefault(s => s.Number == (short)SkillNumber.BeastUppercut) is { } beastUppercut)
        {
            beastUppercut.MagicEffectDef = defensereductionBeastUppercut;
            beastUppercut.NumberOfHitsPerAttack = 2;
        }

        if (gameConfiguration.Skills.FirstOrDefault(s => s.Number == (short)SkillNumber.BeastUppercutStrengthener) is { } beastUppercutStrengthener)
        {
            beastUppercutStrengthener.MagicEffectDef = defensereductionBeastUppercut;
            beastUppercutStrengthener.NumberOfHitsPerAttack = 2;
        }

        if (gameConfiguration.Skills.FirstOrDefault(s => s.Number == (short)SkillNumber.BeastUppercutMastery) is { } beastUppercutMastery)
        {
            beastUppercutMastery.MagicEffectDef = defensereductionBeastUppercut;
            beastUppercutMastery.NumberOfHitsPerAttack = 2;
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
        decDefRatePowerUpDefinition.Boost.ConstantValue.Value = 0.5f; // 50% decrease
        decDefRatePowerUpDefinition.Boost.ConstantValue.AggregateType = AggregateType.Multiplicate;

        var decDefRatePowerUpDefinitionPvp = context.CreateNew<PowerUpDefinition>();
        magicEffect.PowerUpDefinitionsPvp.Add(decDefRatePowerUpDefinitionPvp);
        decDefRatePowerUpDefinitionPvp.TargetAttribute = Stats.DefenseRatePvp.GetPersistent(gameConfiguration);
        decDefRatePowerUpDefinitionPvp.Boost = context.CreateNew<PowerUpDefinitionValue>();
        decDefRatePowerUpDefinitionPvp.Boost.ConstantValue.Value = 0.8f; // 20% decrease
        decDefRatePowerUpDefinitionPvp.Boost.ConstantValue.AggregateType = AggregateType.Multiplicate;

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

        var reducePvmDefenseEffect = context.CreateNew<PowerUpDefinition>();
        magicEffect.PowerUpDefinitions.Add(reducePvmDefenseEffect);
        reducePvmDefenseEffect.TargetAttribute = Stats.DefensePvm.GetPersistent(gameConfiguration);
        reducePvmDefenseEffect.Boost = context.CreateNew<PowerUpDefinitionValue>();
        reducePvmDefenseEffect.Boost.ConstantValue.Value = 0.9f;
        reducePvmDefenseEffect.Boost.ConstantValue.AggregateType = AggregateType.Multiplicate;

        var reducePvpDefenseEffect = context.CreateNew<PowerUpDefinition>();
        magicEffect.PowerUpDefinitionsPvp.Add(reducePvpDefenseEffect);
        reducePvpDefenseEffect.TargetAttribute = Stats.DefensePvp.GetPersistent(gameConfiguration);
        reducePvpDefenseEffect.Boost = context.CreateNew<PowerUpDefinitionValue>();
        reducePvpDefenseEffect.Boost.ConstantValue.Value = 0.9f;
        reducePvpDefenseEffect.Boost.ConstantValue.AggregateType = AggregateType.Multiplicate;

        return magicEffect;
    }
}