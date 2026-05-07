// <copyright file="FinishDarkKnightMasterTreePlugInBase.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.Persistence.Initialization.Updates;

using MUnique.OpenMU.AttributeSystem;
using MUnique.OpenMU.DataModel.Configuration;
using MUnique.OpenMU.GameLogic;
using MUnique.OpenMU.GameLogic.Attributes;

/// <summary>
/// This update completes the dark knight master tree skills and effects. It also fixes the double wield damage calculations.
/// </summary>
public abstract class FinishDarkKnightMasterTreePlugInBase : UpdatePlugInBase
{
    /// <summary>
    /// The plug in name.
    /// </summary>
    internal const string PlugInName = "Fix Double Wield Damage Calculations";

    /// <summary>
    /// The plug in description.
    /// </summary>
    internal const string PlugInDescription = "This update fixes the double wield damage calculations.";

    /// <inheritdoc />
    public override string Name => PlugInName;

    /// <inheritdoc />
    public override string Description => PlugInDescription;

    /// <inheritdoc />
    public override bool IsMandatory => true;

    /// <inheritdoc />
    public override DateTime CreatedAt => new(2026, 5, 7, 16, 0, 0, DateTimeKind.Utc);

    /// <inheritdoc />
    protected override async ValueTask ApplyAsync(IContext context, GameConfiguration gameConfiguration)
    {
        // Create new Stats
        var maceMasteryStunChance = context.CreateNew<AttributeDefinition>(Stats.MaceMasteryStunChance.Id, Stats.MaceMasteryStunChance.Designation, Stats.MaceMasteryStunChance.Description);
        gameConfiguration.Attributes.Add(maceMasteryStunChance);
        var ragefulBlowMasteryDurabilityDecChance = context.CreateNew<AttributeDefinition>(Stats.RagefulBlowMasteryDurabilityDecChance.Id, Stats.RagefulBlowMasteryDurabilityDecChance.Designation, Stats.RagefulBlowMasteryDurabilityDecChance.Description);
        gameConfiguration.Attributes.Add(ragefulBlowMasteryDurabilityDecChance);
        var spearMasteryDoubleDamageChance = context.CreateNew<AttributeDefinition>(Stats.SpearMasteryDoubleDamageChance.Id, Stats.SpearMasteryDoubleDamageChance.Designation, Stats.SpearMasteryDoubleDamageChance.Description);
        gameConfiguration.Attributes.Add(spearMasteryDoubleDamageChance);
        var swellLifeHealthIncrease = context.CreateNew<AttributeDefinition>(Stats.SwellLifeHealthIncrease.Id, Stats.SwellLifeHealthIncrease.Designation, Stats.SwellLifeHealthIncrease.Description);
        gameConfiguration.Attributes.Add(swellLifeHealthIncrease);
        var swellLifeManaIncrease = context.CreateNew<AttributeDefinition>(Stats.SwellLifeManaIncrease.Id, Stats.SwellLifeManaIncrease.Designation, Stats.SwellLifeManaIncrease.Description);
        gameConfiguration.Attributes.Add(swellLifeManaIncrease);

        // Update attribute combinations
        var maximumHealth = Stats.MaximumHealth.GetPersistent(gameConfiguration);
        var maximumMana = Stats.MaximumMana.GetPersistent(gameConfiguration);
        var obsoleteTempDoubleWield = gameConfiguration.Attributes.FirstOrDefault(a => a.Designation == "Temp Double Wield multiplier"); // we remove this later
        var hasDoubleWield = Stats.HasDoubleWield.GetPersistent(gameConfiguration);
        var minimumPhysBaseDmgByWeapon = Stats.MinimumPhysBaseDmgByWeapon.GetPersistent(gameConfiguration);
        var maximumPhysBaseDmgByWeapon = Stats.MaximumPhysBaseDmgByWeapon.GetPersistent(gameConfiguration);
        var physicalBaseDmg = Stats.PhysicalBaseDmg.GetPersistent(gameConfiguration);
        var physicalBaseDmgIncrease = Stats.PhysicalBaseDmgIncrease.GetPersistent(gameConfiguration);
        var doubleDamageChance = Stats.DoubleDamageChance.GetPersistent(gameConfiguration);
        var isSpearEquipped = Stats.IsSpearEquipped.GetPersistent(gameConfiguration);

        // Update attribute combinations
        gameConfiguration.CharacterClasses.ForEach(charClass =>
        {
            // Update common combination attribute
            if (charClass.AttributeCombinations.FirstOrDefault(attrCombo => attrCombo.TargetAttribute == Stats.PhysicalBaseDmg && attrCombo.InputAttribute == Stats.BaseDamageBonus) is { } baseDmgBonusToPhysicalBaseDmg)
            {
                baseDmgBonusToPhysicalBaseDmg.AggregateType = AggregateType.AddFinal;
            }

            // Add new attribute combinations
            var swellLifeHealthIncreaseToMaxHealth = context.CreateNew<AttributeRelationship>(
                maximumHealth,
                1,
                swellLifeHealthIncrease,
                InputOperator.Multiply,
                default(AttributeDefinition?),
                AggregateType.Multiplicate);

            var swellLifeManaIncreaseToMaxMana = context.CreateNew<AttributeRelationship>(
                maximumMana,
                1,
                swellLifeManaIncrease,
                InputOperator.Multiply,
                default(AttributeDefinition?),
                AggregateType.Multiplicate);

            charClass.AttributeCombinations.Add(swellLifeHealthIncreaseToMaxHealth);
            charClass.AttributeCombinations.Add(swellLifeManaIncreaseToMaxMana);
            charClass.BaseAttributeValues.Add(context.CreateNew<ConstValueAttribute>(1, swellLifeHealthIncrease));
            charClass.BaseAttributeValues.Add(context.CreateNew<ConstValueAttribute>(1, swellLifeManaIncrease));

            // Update/add double wield attribute combinations
            if (charClass.Number == 4 || charClass.Number == 6 || charClass.Number == 7 // DK classes
                || charClass.Number == 12 || charClass.Number == 13 // MG classes
                || charClass.Number == 24 || charClass.Number == 25) // RF classes
            {
                if (charClass.AttributeCombinations.FirstOrDefault(attrCombo => attrCombo.InputOperand == -0.45) is { } hasDoubleWieldToTempDoubleWieldMultiplier)
                {
                    charClass.AttributeCombinations.Remove(hasDoubleWieldToTempDoubleWieldMultiplier);
                }

                if (charClass.AttributeCombinations.FirstOrDefault(attrCombo => attrCombo.TargetAttribute == Stats.PhysicalBaseDmgIncrease && attrCombo.InputAttribute == obsoleteTempDoubleWield) is { } tempDoubleWieldToPhysicalBaseDmgIncrease)
                {
                    tempDoubleWieldToPhysicalBaseDmgIncrease.InputAttribute = hasDoubleWield;
                    tempDoubleWieldToPhysicalBaseDmgIncrease.InputOperand = 0.55f;
                    tempDoubleWieldToPhysicalBaseDmgIncrease.InputOperator = InputOperator.ExponentiateByAttribute;
                }

                var hasDoubleWieldToMinimumPhysBaseDmgByWeapon = context.CreateNew<AttributeRelationship>(
                    minimumPhysBaseDmgByWeapon,
                    0.5f,
                    hasDoubleWield,
                    InputOperator.ExponentiateByAttribute,
                    default(AttributeDefinition?),
                    AggregateType.Multiplicate);

                var hasDoubleWieldToMaximumPhysBaseDmgByWeapon = context.CreateNew<AttributeRelationship>(
                    maximumPhysBaseDmgByWeapon,
                    0.5f,
                    hasDoubleWield,
                    InputOperator.ExponentiateByAttribute,
                    default(AttributeDefinition?),
                    AggregateType.Multiplicate);

                var hasDoubleWieldToPhysicalBaseDmg = context.CreateNew<AttributeRelationship>(
                    physicalBaseDmg,
                    0.5f,
                    hasDoubleWield,
                    InputOperator.ExponentiateByAttribute,
                    default(AttributeDefinition?),
                    AggregateType.Multiplicate);

                var hasDoubleWieldToPhysicalBaseDmgIncrease = context.CreateNew<AttributeRelationship>(
                    physicalBaseDmgIncrease,
                    0.5f,
                    hasDoubleWield,
                    InputOperator.ExponentiateByAttribute,
                    default(AttributeDefinition?),
                    AggregateType.Multiplicate);

                var hasDoubleWieldToPhysicalBaseDmgIncreaseRaw = context.CreateNew<AttributeRelationship>(
                    physicalBaseDmgIncrease,
                    1,
                    hasDoubleWield,
                    InputOperator.Multiply,
                    default(AttributeDefinition?),
                    AggregateType.AddRaw);

                charClass.AttributeCombinations.Add(hasDoubleWieldToMinimumPhysBaseDmgByWeapon);
                charClass.AttributeCombinations.Add(hasDoubleWieldToMaximumPhysBaseDmgByWeapon);
                charClass.AttributeCombinations.Add(hasDoubleWieldToPhysicalBaseDmg);
                charClass.AttributeCombinations.Add(hasDoubleWieldToPhysicalBaseDmgIncrease);
                charClass.AttributeCombinations.Add(hasDoubleWieldToPhysicalBaseDmgIncreaseRaw);

                if (charClass.Number == 4 || charClass.Number == 6 || charClass.Number == 7)
                {
                    var spearMasteryDoubleDamageChanceToDoubleDamageChance = context.CreateNew<AttributeRelationship>(
                        doubleDamageChance,
                        isSpearEquipped,
                        spearMasteryDoubleDamageChance,
                        AggregateType.AddRaw);

                    charClass.AttributeCombinations.Add(spearMasteryDoubleDamageChanceToDoubleDamageChance);
                }
            }
        });

        // Removed obsolete attribute
        if (obsoleteTempDoubleWield is not null)
        {
            gameConfiguration.Attributes.Remove(obsoleteTempDoubleWield);
        }

        // Update wings physical base dmg option
        var wingsSlot = gameConfiguration.ItemSlotTypes.First(st => st.ItemSlots.Contains(7));
        foreach (var wing in gameConfiguration.Items.Where(i => i.ItemSlot == wingsSlot))
        {
            if (wing.PossibleItemOptions.FirstOrDefault() is { } pio
                && pio.PossibleOptions.FirstOrDefault(po => po.PowerUpDefinition?.TargetAttribute == physicalBaseDmg) is { } physicalBaseDmgOption)
            {
                foreach (var levelOption in physicalBaseDmgOption.LevelDependentOptions)
                {
                    levelOption.PowerUpDefinition!.Boost!.ConstantValue.AggregateType = AggregateType.AddFinal;
                }
            }
        }
    }
}
