// <copyright file="FixDefenseCalcsPlugInBase.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.Persistence.Initialization.Updates;

using MUnique.OpenMU.AttributeSystem;
using MUnique.OpenMU.DataModel.Configuration;
using MUnique.OpenMU.GameLogic;
using MUnique.OpenMU.GameLogic.Attributes;
using MUnique.OpenMU.Persistence.Initialization.Items;
using MUnique.OpenMU.Persistence.Initialization.Skills;

/// <summary>
/// This update fixes character stats, magic effects, and options related to defense.
/// </summary>
public abstract class FixDefenseCalcsPlugInBase : UpdatePlugInBase
{
    /// <summary>
    /// The plug in name.
    /// </summary>
    internal const string PlugInName = "Fix Defense Calculations";

    /// <summary>
    /// The plug in description.
    /// </summary>
    internal const string PlugInDescription = "This update fixes character stats, magic effects, and options related to defense.";

    /// <inheritdoc />
    public override string Name => PlugInName;

    /// <inheritdoc />
    public override string Description => PlugInDescription;

    /// <inheritdoc />
    public override bool IsMandatory => true;

    /// <inheritdoc />
    public override DateTime CreatedAt => new(2025, 05, 23, 16, 0, 0, DateTimeKind.Utc);

    /// <inheritdoc />
    protected override async ValueTask ApplyAsync(IContext context, GameConfiguration gameConfiguration)
    {
        // Update attributes
        var defenseFinal = context.CreateNew<AttributeDefinition>(Stats.DefenseFinal.Id, Stats.DefenseFinal.Designation, Stats.DefenseFinal.Description);
        gameConfiguration.Attributes.Add(defenseFinal);
        var bonusDefenseRateWithShield = context.CreateNew<AttributeDefinition>(Stats.BonusDefenseRateWithShield.Id, Stats.BonusDefenseRateWithShield.Designation, Stats.BonusDefenseRateWithShield.Description);
        gameConfiguration.Attributes.Add(bonusDefenseRateWithShield);
        var defenseShield = context.CreateNew<AttributeDefinition>(Stats.DefenseShield.Id, Stats.DefenseShield.Designation, Stats.DefenseShield.Description);
        gameConfiguration.Attributes.Add(defenseShield);
        var shieldItemDefenseIncrease = context.CreateNew<AttributeDefinition>(Stats.ShieldItemDefenseIncrease.Id, Stats.ShieldItemDefenseIncrease.Designation, Stats.ShieldItemDefenseIncrease.Description);
        gameConfiguration.Attributes.Add(shieldItemDefenseIncrease);

        var shieldBlockDamageDecrementId = new Guid("DAC6690B-5922-4446-BCE5-5E701BE62EC1");
        if (gameConfiguration.Attributes.FirstOrDefault(a => a.Id == shieldBlockDamageDecrementId) is { } shieldBlockDamageDecrement)
        {
            gameConfiguration.Attributes.Remove(shieldBlockDamageDecrement);
        }

        // Update attribute combinations
        var defenseBase = Stats.DefenseBase.GetPersistent(gameConfiguration);
        var defensePvm = Stats.DefensePvm.GetPersistent(gameConfiguration);
        var defensePvp = Stats.DefensePvp.GetPersistent(gameConfiguration);
        var bonusDefenseWithShield = Stats.BonusDefenseWithShield.GetPersistent(gameConfiguration);
        var isShieldEquipped = Stats.IsShieldEquipped.GetPersistent(gameConfiguration);
        var defenseRatePvm = Stats.DefenseRatePvm.GetPersistent(gameConfiguration);
        var isHorseEquipped = Stats.IsHorseEquipped.GetPersistent(gameConfiguration);
        var bonusDefenseWithHorse = Stats.BonusDefenseWithHorse.GetPersistent(gameConfiguration);

        gameConfiguration.CharacterClasses.ForEach(charClass =>
        {
            var attrCombos = charClass.AttributeCombinations.ToList();
            foreach (var attrCombo in attrCombos)
            {
                if ((attrCombo.TargetAttribute == Stats.DefensePvm && attrCombo.InputAttribute == Stats.DefenseBase)
                    || (attrCombo.TargetAttribute == Stats.DefensePvp && attrCombo.InputAttribute == Stats.DefenseBase))
                {
                    charClass.AttributeCombinations.Remove(attrCombo);
                }
            }

            if (attrCombos.FirstOrDefault(attrCombo =>
                attrCombo.TargetAttribute == Stats.DefenseBase
                && attrCombo.InputAttribute == Stats.BonusDefenseWithShield
                && attrCombo.OperandAttribute == Stats.IsShieldEquipped) is { } bonusDefenseWithShieldToDefenseBase)
            {
                charClass.AttributeCombinations.Remove(bonusDefenseWithShieldToDefenseBase);
            }

            var shieldDefenseToDefenseBase = context.CreateNew<AttributeRelationship>(
                defenseBase,
                1,
                defenseShield,
                InputOperator.Multiply,
                default(AttributeDefinition?),
                AggregateType.AddRaw);

            var defenseBaseToDefenseFinal = context.CreateNew<AttributeRelationship>(
                defenseFinal,
                0.5f,
                defenseBase,
                InputOperator.Multiply,
                default(AttributeDefinition?),
                AggregateType.AddRaw);

            var defenseFinalToDefensePvm = context.CreateNew<AttributeRelationship>(
                defensePvm,
                1,
                defenseFinal,
                InputOperator.Multiply,
                default(AttributeDefinition?),
                AggregateType.AddRaw);

            var defenseFinalToDefensePvp = context.CreateNew<AttributeRelationship>(
                defensePvp,
                1,
                defenseFinal,
                InputOperator.Multiply,
                default(AttributeDefinition?),
                AggregateType.AddRaw);

            if (attrCombos.FirstOrDefault(attrCombo => attrCombo.InputAttribute == Stats.DefenseIncreaseWithEquippedShield
                && attrCombo.OperandAttribute == Stats.IsShieldEquipped) is { } defenseIncWithEquippedShieldToTempDefense)
            {
                var tempDefenseToDefenseFinal = context.CreateNew<AttributeRelationship>(
                    defenseFinal,
                    1,
                    defenseIncWithEquippedShieldToTempDefense.TargetAttribute,
                    InputOperator.Add,
                    default(AttributeDefinition?),
                    AggregateType.Multiplicate);

                charClass.AttributeCombinations.Add(tempDefenseToDefenseFinal);
            }

            var shieldItemDefenseIncreaseToDefenseFinal = context.CreateNew<AttributeRelationship>(
                defenseFinal,
                defenseShield,
                shieldItemDefenseIncrease,
                AggregateType.AddRaw);

            var bonusDefenseWithShieldToDefenseFinal = context.CreateNew<AttributeRelationship>(
                defenseFinal,
                isShieldEquipped,
                bonusDefenseWithShield,
                AggregateType.AddFinal);

            var bonusDefenseRateWithShieldToDefenseRatePvm = context.CreateNew<AttributeRelationship>(
                defenseRatePvm,
                isShieldEquipped,
                bonusDefenseRateWithShield,
                AggregateType.AddFinal);

            charClass.AttributeCombinations.Add(shieldDefenseToDefenseBase);
            charClass.AttributeCombinations.Add(defenseBaseToDefenseFinal);
            charClass.AttributeCombinations.Add(defenseFinalToDefensePvm);
            charClass.AttributeCombinations.Add(defenseFinalToDefensePvp);
            charClass.AttributeCombinations.Add(shieldItemDefenseIncreaseToDefenseFinal);
            charClass.AttributeCombinations.Add(bonusDefenseWithShieldToDefenseFinal);
            charClass.AttributeCombinations.Add(bonusDefenseRateWithShieldToDefenseRatePvm);

            if (charClass.Number == 16 || charClass.Number == 17) // Lord classes
            {
                var bonusDefenseWithHorseToDefenseFinal = context.CreateNew<AttributeRelationship>(
                    defenseFinal,
                    isHorseEquipped,
                    bonusDefenseWithHorse,
                    AggregateType.AddFinal);

                charClass.AttributeCombinations.Add(bonusDefenseWithHorseToDefenseFinal);
            }
        });

        // Update magic effects
        var defenseEffect = gameConfiguration.MagicEffects.FirstOrDefault(m => m.Number == (short)MagicEffectNumber.ShieldSkill);
        if (defenseEffect?.Duration is not null)
        {
            defenseEffect.Duration.ConstantValue.Value = 4;
        }

        var greaterDefenseEffect = gameConfiguration.MagicEffects.FirstOrDefault(m => m.Number == (short)MagicEffectNumber.GreaterDefense);
        if (greaterDefenseEffect?.PowerUpDefinitions.FirstOrDefault(p => p.TargetAttribute == defenseBase) is { } greaterDefenseEffectPowerUp)
        {
            greaterDefenseEffectPowerUp.TargetAttribute = defenseFinal;
            greaterDefenseEffectPowerUp.Boost!.ConstantValue.AggregateType = AggregateType.AddFinal;
        }

        // Update shields
        var shields = gameConfiguration.Items.Where(i => i.Group == (byte)ItemGroups.Shields);
        foreach (var shield in shields)
        {
            if (shield.BasePowerUpAttributes.FirstOrDefault(bpua => bpua.TargetAttribute == Stats.DefenseBase) is { } defensePowerUp)
            {
                defensePowerUp.TargetAttribute = defenseShield;
            }
        }
    }
}