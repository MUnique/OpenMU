// <copyright file="FixDefenseCalcsPlugInSeason6.cs" company="MUnique">
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
using MUnique.OpenMU.PlugIns;

/// <summary>
/// This update fixes character stats, magic effects, and options related to defense.
/// </summary>
[PlugIn(PlugInName, PlugInDescription)]
[Guid("447FA95B-091B-4950-B1F3-F4EB6D20DE19")]
public class FixDefenseCalcsPlugInSeason6 : FixDefenseCalcsPlugInBase
{
    /// <inheritdoc />
    public override string DataInitializationKey => VersionSeasonSix.DataInitialization.Id;

    /// <inheritdoc />
    public override UpdateVersion Version => UpdateVersion.FixDefenseCalcsSeason6;

    /// <inheritdoc />
    protected override async ValueTask ApplyAsync(IContext context, GameConfiguration gameConfiguration)
    {
        // Update Anonymous Leather ancient set obsolete option prior to its removal
        var shieldBlockDamageDecrementId = new Guid("DAC6690B-5922-4446-BCE5-5E701BE62EC1");
        var anonymousLeatherOptsId = new Guid("00000083-0029-0002-0100-000000000000");
        var anonymousLeatherOpts = gameConfiguration.ItemOptions.FirstOrDefault(i => i.GetId() == anonymousLeatherOptsId);
        if (gameConfiguration.Attributes.FirstOrDefault(a => a.Id == shieldBlockDamageDecrementId) is { } shieldBlockDamageDecrement
            && anonymousLeatherOpts?.PossibleOptions.FirstOrDefault(opt => opt.PowerUpDefinition?.TargetAttribute == shieldBlockDamageDecrement) is { } shieldBlockDamageDecrementOpt)
        {
            shieldBlockDamageDecrementOpt.PowerUpDefinition!.TargetAttribute = Stats.DefenseIncreaseWithEquippedShield.GetPersistent(gameConfiguration);
        }

        await base.ApplyAsync(context, gameConfiguration).ConfigureAwait(false);

        var defenseBase = Stats.DefenseBase.GetPersistent(gameConfiguration);
        var defenseFinal = Stats.DefenseFinal.GetPersistent(gameConfiguration);
        var defensePvm = Stats.DefensePvm.GetPersistent(gameConfiguration);
        var defensePvp = Stats.DefensePvp.GetPersistent(gameConfiguration);
        var shieldItemDefenseIncrease = Stats.ShieldItemDefenseIncrease.GetPersistent(gameConfiguration);
        var bonusDefenseRateWithShield = Stats.BonusDefenseRateWithShield.GetPersistent(gameConfiguration);
        var excellentDamageChance = Stats.ExcellentDamageChance.GetPersistent(gameConfiguration);
        var excellentDamageBonus = Stats.ExcellentDamageBonus.GetPersistent(gameConfiguration);

        // Update attribute combinations
        gameConfiguration.CharacterClasses.ForEach(charClass =>
        {
            if (charClass.AttributeCombinations.FirstOrDefault(attrCombo => attrCombo.TargetAttribute == Stats.MaximumShield && attrCombo.InputAttribute == Stats.DefenseBase) is { } defenseBaseToMaximumShield)
            {
                defenseBaseToMaximumShield.InputOperand = 1;
                defenseBaseToMaximumShield.InputAttribute = defenseFinal;
            }
        });

        // Update magic effects
        var defenseReductionEffect = gameConfiguration.MagicEffects.FirstOrDefault(m => m.Number == (short)MagicEffectNumber.DefenseReduction);
        if (defenseReductionEffect is not null)
        {
            defenseReductionEffect.PowerUpDefinitions.Clear();

            var reducePvmDefenseEffect = context.CreateNew<PowerUpDefinition>();
            defenseReductionEffect.PowerUpDefinitions.Add(reducePvmDefenseEffect);
            reducePvmDefenseEffect.TargetAttribute = defensePvm;
            reducePvmDefenseEffect.Boost = context.CreateNew<PowerUpDefinitionValue>();
            reducePvmDefenseEffect.Boost.ConstantValue.Value = 0.9f;
            reducePvmDefenseEffect.Boost.ConstantValue.AggregateType = AggregateType.Multiplicate;

            var reducePvpDefenseEffect = context.CreateNew<PowerUpDefinition>();
            defenseReductionEffect.PowerUpDefinitions.Add(reducePvpDefenseEffect);
            reducePvpDefenseEffect.TargetAttribute = defensePvp;
            reducePvpDefenseEffect.Boost = context.CreateNew<PowerUpDefinitionValue>();
            reducePvpDefenseEffect.Boost.ConstantValue.Value = 0.9f;
            reducePvpDefenseEffect.Boost.ConstantValue.AggregateType = AggregateType.Multiplicate;
        }

        var jackOlanternCry = gameConfiguration.Items.FirstOrDefault(m => m.Group == 14 && m.Number == 48);
        if (jackOlanternCry?.ConsumeEffect is { } effect)
        {
            foreach (var powerUp in effect.PowerUpDefinitions)
            {
                powerUp.TargetAttribute = defenseFinal;
                powerUp.Boost!.ConstantValue.Value = 100 / 2;
                powerUp.Boost.ConstantValue.AggregateType = AggregateType.AddFinal;
            }
        }

        // Update options
        var ancientSetsOpts = gameConfiguration.ItemOptions.Where(io => io.Name.EndsWith("(Ancient Set)"));
        foreach (var ancientSetOpts in ancientSetsOpts)
        {
            if (ancientSetOpts.PossibleOptions.FirstOrDefault(o => o.PowerUpDefinition?.TargetAttribute == defenseBase) is { } defenseBaseAncOpt)
            {
                defenseBaseAncOpt.PowerUpDefinition!.Boost!.ConstantValue.AggregateType = AggregateType.AddFinal;
            }
        }

        var anubisLegendaryOptsId = new Guid("00000083-0029-0013-0100-000000000000");
        var anubisLegendaryOpts = gameConfiguration.ItemOptions.FirstOrDefault(i => i.GetId() == anubisLegendaryOptsId);
        if (anubisLegendaryOpts?.PossibleOptions.FirstOrDefault(opt =>
            opt.PowerUpDefinition?.TargetAttribute == excellentDamageChance
            && opt.PowerUpDefinition.Boost?.ConstantValue.Value == 20.0f) is { } excellentDamageChanceOpt)
        {
            excellentDamageChanceOpt.PowerUpDefinition!.TargetAttribute = excellentDamageBonus;
        }

        var pantsGuardianOptions = gameConfiguration.ItemOptions.FirstOrDefault(o => o.Name == "Guardian Option (Pants)");
        if (pantsGuardianOptions is not null
            && pantsGuardianOptions.PossibleOptions.FirstOrDefault(o => o.PowerUpDefinition?.TargetAttribute == defenseBase) is { } defenseOpt)
        {
            defenseOpt.PowerUpDefinition!.TargetAttribute = defensePvp;
            defenseOpt.PowerUpDefinition.Boost!.ConstantValue.Value = 200 / 2;
        }

        var harmonyDefOptions = gameConfiguration.ItemOptions.FirstOrDefault(o => o.Name == "Harmony Defense Options");
        if (harmonyDefOptions is not null
            && harmonyDefOptions.PossibleOptions.FirstOrDefault(o => o.Number == 1) is { } defenseBaseOpt)
        {
            foreach (var level in defenseBaseOpt.LevelDependentOptions)
            {
                level.PowerUpDefinition!.Boost!.ConstantValue.AggregateType = AggregateType.AddFinal;
            }
        }

        var socketOptionsWaterId = new Guid("00000083-0033-0000-0000-000000000000");
        var waterSocketOptions = gameConfiguration.ItemOptions.FirstOrDefault(o => o.GetId() == socketOptionsWaterId);
        if (waterSocketOptions is not null)
        {
            if (waterSocketOptions.PossibleOptions.FirstOrDefault(o => o.Number == 1) is { } defenseBaseSockOpt)
            {
                foreach (var level in defenseBaseSockOpt.LevelDependentOptions)
                {
                    level.PowerUpDefinition!.TargetAttribute = defenseFinal;
                }
            }

            if (waterSocketOptions.PossibleOptions.FirstOrDefault(o => o.Number == 2) is { } shieldItemdefenseSockOpt)
            {
                foreach (var level in shieldItemdefenseSockOpt.LevelDependentOptions)
                {
                    level.PowerUpDefinition!.TargetAttribute = shieldItemDefenseIncrease;
                }
            }
        }

        var socketBonusOptionsArmorsId = new Guid("00000083-0031-0003-0000-000000000000");
        var socketArmorsBonusOptions = gameConfiguration.ItemOptions.FirstOrDefault(o => o.GetId() == socketBonusOptionsArmorsId);
        if (socketArmorsBonusOptions is not null
            && socketArmorsBonusOptions.PossibleOptions.FirstOrDefault(o => o.Number == 4) is { } defenseBaseSockBonusOpt)
        {
            defenseBaseSockBonusOpt.PowerUpDefinition!.TargetAttribute = defenseFinal;
        }

        // Update master skills
        if (gameConfiguration.Skills.FirstOrDefault(s => s.Number == (short)SkillNumber.DefenseIncrease)?.MasterDefinition is { } defenseIncrease)
        {
            defenseIncrease.Aggregation = AggregateType.AddFinal;
        }

        if (gameConfiguration.Skills.FirstOrDefault(s => s.Number == (short)SkillNumber.IncreasesDefense)?.MasterDefinition is { } increasesDefense)
        {
            increasesDefense.Aggregation = AggregateType.AddFinal;
        }

        if (gameConfiguration.Skills.FirstOrDefault(s => s.Number == (short)SkillNumber.DefenseSuccessRateInc)?.MasterDefinition is { } defenseSuccessRateInc)
        {
            string formula120 = "(1 + (((((((level - 30) ^ 3) + 25000) / 499) / 50) * 100) / 12))";
            defenseSuccessRateInc.ValueFormula = $"1 + {formula120} / 100";
            defenseSuccessRateInc.Aggregation = AggregateType.Multiplicate;
        }

        if (gameConfiguration.Skills.FirstOrDefault(s => s.Number == (short)SkillNumber.ShieldMasteryGrandMaster)?.MasterDefinition is { } shieldMasteryGrandMaster)
        {
            shieldMasteryGrandMaster.TargetAttribute = bonusDefenseRateWithShield;
        }

        if (gameConfiguration.Skills.FirstOrDefault(s => s.Number == (short)SkillNumber.ShieldMasteryHighElf)?.MasterDefinition is { } shieldMasteryHighElf)
        {
            shieldMasteryHighElf.TargetAttribute = bonusDefenseRateWithShield;
        }

        if (gameConfiguration.Skills.FirstOrDefault(s => s.Number == (short)SkillNumber.ShieldMastery)?.MasterDefinition is { } shieldMastery)
        {
            shieldMastery.TargetAttribute = bonusDefenseRateWithShield;
        }
    }
}