// <copyright file="FinishElfMasterTreePlugIn.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.Persistence.Initialization.Updates;

using System.Runtime.InteropServices;
using MUnique.OpenMU.AttributeSystem;
using MUnique.OpenMU.DataModel.Attributes;
using MUnique.OpenMU.DataModel.Configuration;
using MUnique.OpenMU.DataModel.Configuration.Items;
using MUnique.OpenMU.GameLogic.Attributes;
using MUnique.OpenMU.Persistence.Initialization.Items;
using MUnique.OpenMU.Persistence.Initialization.Skills;
using MUnique.OpenMU.Persistence.Initialization.VersionSeasonSix;
using MUnique.OpenMU.PlugIns;

/// <summary>
/// This update completes the elf master tree and fixes some of its skill values. It also adds the missing extra projectile (4th) on some higher level (cross)bows.
/// </summary>
[PlugIn]
[Display(Name = PlugInName, Description = PlugInDescription)]
[Guid("D1E2F3A4-B5C6-7D8E-9F0A-1B2C3D4E5F6A")]
public class FinishElfMasterTreePlugIn : UpdatePlugInBase
{
    /// <summary>
    /// The plug in name.
    /// </summary>
    internal const string PlugInName = "Finish Elf Master Tree PlugIn";

    /// <summary>
    /// The plug in description.
    /// </summary>
    internal const string PlugInDescription = "This update completes the elf master tree and fixes some of its skill values. It also adds the missing extra projectile (4th) on some higher level (cross)bows.";

    /// <inheritdoc />
    public override UpdateVersion Version => UpdateVersion.FinishElfMasterTree;

    /// <inheritdoc />
    public override string DataInitializationKey => DataInitialization.Id;

    /// <inheritdoc />
    public override string Name => PlugInName;

    /// <inheritdoc />
    public override string Description => PlugInDescription;

    /// <inheritdoc />
    public override bool IsMandatory => true;

    /// <inheritdoc />
    public override DateTime CreatedAt => new(2026, 7, 17, 16, 0, 0, DateTimeKind.Utc);

    /// <inheritdoc />
    protected override async ValueTask ApplyAsync(IContext context, GameConfiguration gameConfiguration)
    {
        // Create new Stats
        var extraProjectiles = context.CreateNew<AttributeDefinition>(Stats.ExtraProjectiles.Id, Stats.ExtraProjectiles.Designation, Stats.ExtraProjectiles.Description);
        gameConfiguration.Attributes.Add(extraProjectiles);
        var greaterDefenseBonus = context.CreateNew<AttributeDefinition>(Stats.GreaterDefenseBonus.Id, Stats.GreaterDefenseBonus.Designation, Stats.GreaterDefenseBonus.Description);
        gameConfiguration.Attributes.Add(greaterDefenseBonus);

        var greaterDamageBonus = Stats.GreaterDamageBonus.GetPersistent(gameConfiguration);

        // Update Greater Damage effect
        var greaterDamageEffect = gameConfiguration.MagicEffects.First(e => e.Number == (short)MagicEffectNumber.GreaterDamage);

        var greaterDamagePowerUp2 = context.CreateNew<PowerUpDefinition>();
        greaterDamageEffect.PowerUpDefinitions.Add(greaterDamagePowerUp2);
        greaterDamagePowerUp2.TargetAttribute = greaterDamageBonus;
        greaterDamagePowerUp2.Boost = context.CreateNew<PowerUpDefinitionValue>();
        greaterDamagePowerUp2.Boost.ConstantValue.Value = 1f;
        greaterDamagePowerUp2.Boost.ConstantValue.AggregateType = AggregateType.Multiplicate;

        // Update Greater Defense effect
        var greaterDefenseEffect = gameConfiguration.MagicEffects.First(e => e.Number == (short)MagicEffectNumber.GreaterDefense);

        if (greaterDefenseEffect.PowerUpDefinitions.FirstOrDefault() is PowerUpDefinition defensePowerUp)
        {
            defensePowerUp.TargetAttribute = greaterDefenseBonus;
            defensePowerUp.Boost?.ConstantValue.AggregateType = AggregateType.AddRaw;
        }

        var greaterDefensePowerUp2 = context.CreateNew<PowerUpDefinition>();
        greaterDefenseEffect.PowerUpDefinitions.Add(greaterDefensePowerUp2);
        greaterDefensePowerUp2.TargetAttribute = greaterDefenseBonus;
        greaterDefensePowerUp2.Boost = context.CreateNew<PowerUpDefinitionValue>();
        greaterDefensePowerUp2.Boost.ConstantValue.Value = 1f;
        greaterDefensePowerUp2.Boost.ConstantValue.AggregateType = AggregateType.Multiplicate;

        // Update Infinity Arrow effect
        var infinityArrowEffect = gameConfiguration.MagicEffects.First(e => e.Number == (short)MagicEffectNumber.InfiniteArrow);

        if (infinityArrowEffect.PowerUpDefinitions.FirstOrDefault(pud => pud.TargetAttribute == Stats.AttackDamageIncrease) is PowerUpDefinition infinityArrowPowerUp)
        {
            infinityArrowPowerUp.Boost?.ConstantValue.Value = 1;
            infinityArrowPowerUp.Boost?.ConstantValue.AggregateType = AggregateType.Multiplicate;
        }

        // Update 4-arrow (cross)bows
        var affectedBows = new short[] { 18, 19, 22, 23, 24 };
        var bows = gameConfiguration.Items.Where(i => i.Group == (byte)ItemGroups.Bows && affectedBows.Contains(i.Number));
        foreach (var bow in bows)
        {
            bow.BasePowerUpAttributes.Add(this.CreateItemBasePowerUpDefinition(context, gameConfiguration, extraProjectiles, 1, AggregateType.AddRaw));
        }

        // Update master skills
        if (gameConfiguration.Skills.FirstOrDefault(s => s.Number == (short)SkillNumber.DefenseIncreaseStr)?.MasterDefinition is { } defenseIncreaseStr)
        {
            defenseIncreaseStr.TargetAttribute = greaterDefenseBonus;
            defenseIncreaseStr.Aggregation = AggregateType.Multiplicate;
        }

        if (gameConfiguration.Skills.FirstOrDefault(s => s.Number == (short)SkillNumber.TripleShotStrengthener)?.MasterDefinition is { } tripleShotStrengthener)
        {
            tripleShotStrengthener.TargetAttribute = extraProjectiles;
            tripleShotStrengthener.Aggregation = AggregateType.AddRaw;
        }

        if (gameConfiguration.Skills.FirstOrDefault(s => s.Number == (short)SkillNumber.AttackIncreaseStr)?.MasterDefinition is { } attackIncreaseStr)
        {
            attackIncreaseStr.TargetAttribute = greaterDamageBonus;
            attackIncreaseStr.Aggregation = AggregateType.Multiplicate;
        }

        if (gameConfiguration.Skills.FirstOrDefault(s => s.Number == (short)SkillNumber.AttackIncreaseMastery)?.MasterDefinition is { } attackIncreaseMastery)
        {
            attackIncreaseMastery.ReplacedSkill = gameConfiguration.Skills.First(s => s.Number == (short)SkillNumber.AttackIncreaseStr);
            attackIncreaseMastery.TargetAttribute = greaterDamageBonus;
            attackIncreaseMastery.Aggregation = AggregateType.Multiplicate;
            attackIncreaseMastery.ExtendsDuration = true;
        }

        if (gameConfiguration.Skills.FirstOrDefault(s => s.Number == (short)SkillNumber.DefenseIncreaseMastery)?.MasterDefinition is { } defenseIncreaseMastery)
        {
            defenseIncreaseMastery.ReplacedSkill = gameConfiguration.Skills.First(s => s.Number == (short)SkillNumber.DefenseIncreaseStr);
            defenseIncreaseMastery.TargetAttribute = greaterDefenseBonus;
            defenseIncreaseMastery.Aggregation = AggregateType.Multiplicate;
            defenseIncreaseMastery.ExtendsDuration = true;
        }

        if (gameConfiguration.Skills.FirstOrDefault(s => s.Number == (short)SkillNumber.InfinityArrowStr)?.MasterDefinition is { } infinityArrowStr)
        {
            infinityArrowStr.ValueFormula = $"{SkillsInitializer.Formula120} / 100";
        }
    }

    private ItemBasePowerUpDefinition CreateItemBasePowerUpDefinition(IContext context, GameConfiguration gameConfiguration, AttributeDefinition attributeDefinition, float value, AggregateType aggregateType)
    {
        var powerUpDefinition = context.CreateNew<ItemBasePowerUpDefinition>();
        powerUpDefinition.TargetAttribute = attributeDefinition.GetPersistent(gameConfiguration);
        powerUpDefinition.BaseValue = value;
        powerUpDefinition.AggregateType = aggregateType;
        return powerUpDefinition;
    }
}
