// <copyright file="RegenerationsRefactorPlugIn.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.Persistence.Initialization.Updates;

using System.Runtime.InteropServices;
using MUnique.OpenMU.AttributeSystem;
using MUnique.OpenMU.DataModel.Configuration;
using MUnique.OpenMU.GameLogic;
using MUnique.OpenMU.GameLogic.Attributes;
using MUnique.OpenMU.Persistence.Initialization.Skills;
using MUnique.OpenMU.Persistence.Initialization.VersionSeasonSix;
using MUnique.OpenMU.PlugIns;

/// <summary>
/// This update fixes and reworks some regeneration attributes (health, shield, mana, ability). It also adds default running (and fast swimming) speed for tier 2 chars (MG, DL, RF).
/// </summary>
[PlugIn]
[Display(Name = PlugInName, Description = PlugInDescription)]
[Guid("E6A3B9F1-7C4D-48E2-A5B8-1F9D3C6E2A7B")]
public class RegenerationsRefactorPlugIn : UpdatePlugInBase
{
    /// <summary>
    /// The plug in name.
    /// </summary>
    internal const string PlugInName = "Regenerations Refactor";

    /// <summary>
    /// The plug in description.
    /// </summary>
    internal const string PlugInDescription = "This update fixes and reworks some regeneration attributes (health, shield, mana, ability). It also adds default running (and fast swimming) speed for tier 2 chars (MG, DL, RF).";

    /// <inheritdoc />
    public override UpdateVersion Version => UpdateVersion.RegenerationsRefactor;

    /// <inheritdoc />
    public override string DataInitializationKey => DataInitialization.Id;

    /// <inheritdoc />
    public override string Name => PlugInName;

    /// <inheritdoc />
    public override string Description => PlugInDescription;

    /// <inheritdoc />
    public override bool IsMandatory => true;

    /// <inheritdoc />
    public override DateTime CreatedAt => new(2026, 8, 26, 16, 0, 0, DateTimeKind.Utc);

    /// <inheritdoc />
    protected override async ValueTask ApplyAsync(IContext context, GameConfiguration gameConfiguration)
    {
        // Create new Stats
        var isShieldRecoveryActive = context.CreateNew<AttributeDefinition>(Stats.IsShieldRecoveryActive.Id, Stats.IsShieldRecoveryActive.Designation, Stats.IsShieldRecoveryActive.Description);
        gameConfiguration.Attributes.Add(isShieldRecoveryActive);
        var shieldRecoveryHiatus = context.CreateNew<AttributeDefinition>(Stats.ShieldRecoveryHiatus.Id, Stats.ShieldRecoveryHiatus.Designation, Stats.ShieldRecoveryHiatus.Description);
        gameConfiguration.Attributes.Add(shieldRecoveryHiatus);
        var shieldRecoveryRampFactor = context.CreateNew<AttributeDefinition>(Stats.ShieldRecoveryRampFactor.Id, Stats.ShieldRecoveryRampFactor.Designation, Stats.ShieldRecoveryRampFactor.Description);
        shieldRecoveryRampFactor.MaximumValue = 3;
        gameConfiguration.Attributes.Add(shieldRecoveryRampFactor);
        var isResting = context.CreateNew<AttributeDefinition>(Stats.IsResting.Id, Stats.IsResting.Designation, Stats.IsResting.Description);
        gameConfiguration.Attributes.Add(isResting);

        var areTwoWeaponsEquipped = Stats.AreTwoWeaponsEquipped.GetPersistent(gameConfiguration);
        var equippedWeaponCount = Stats.EquippedWeaponCount.GetPersistent(gameConfiguration);
        var attackSpeedByWeapon = Stats.AttackSpeedByWeapon.GetPersistent(gameConfiguration);
        var innovationDefDecrement = Stats.InnovationDefDecrement.GetPersistent(gameConfiguration);
        var defenseDecrement = Stats.DefenseDecrement.GetPersistent(gameConfiguration);

        var healthRecoveryMultiplier = Stats.HealthRecoveryMultiplier.GetPersistent(gameConfiguration);
        var manaRecoveryMultiplier = Stats.ManaRecoveryMultiplier.GetPersistent(gameConfiguration);
        var shieldRecoveryMultiplier = Stats.ShieldRecoveryMultiplier.GetPersistent(gameConfiguration);
        var abilityRecoveryMultiplier = Stats.AbilityRecoveryMultiplier.GetPersistent(gameConfiguration);
        var abilityRecoveryAbsolute = Stats.AbilityRecoveryAbsolute.GetPersistent(gameConfiguration);
        var isInSafezone = Stats.IsInSafezone.GetPersistent(gameConfiguration);
        var shieldRecoveryEverywhere = Stats.ShieldRecoveryEverywhere.GetPersistent(gameConfiguration);
        var nearbyPartyMemberCount = Stats.NearbyPartyMemberCount.GetPersistent(gameConfiguration);

        var movementSpeed = Stats.MovementSpeed.GetPersistent(gameConfiguration);
        var movementSpeedUnderwater = Stats.MovementSpeedUnderwater.GetPersistent(gameConfiguration);

        gameConfiguration.CharacterClasses.ForEach(charClass =>
        {
            var attrCombos = charClass.AttributeCombinations;

            // Remove temp attack speed combos
            var tempAttackSpeeds = gameConfiguration.Attributes.Where(a => a.Designation == "Temp Half weapon attack speed");
            if (tempAttackSpeeds.Any())
            {
                if (attrCombos.FirstOrDefault(ac => ac.TargetAttribute == areTwoWeaponsEquipped) is { } equippedWeaponCountToAreTwoWeaponsEquipped)
                {
                    equippedWeaponCountToAreTwoWeaponsEquipped.InputOperator = InputOperator.Maximum;
                }

                AttributeDefinition? classTempAttackSpeed = null;
                if (attrCombos.FirstOrDefault(ac => tempAttackSpeeds.Contains(ac.TargetAttribute)) is { } attrCombo1)
                {
                    attrCombos.Remove(attrCombo1);
                    classTempAttackSpeed = attrCombo1.TargetAttribute;
                }

                if (attrCombos.FirstOrDefault(ac => tempAttackSpeeds.Contains(ac.InputAttribute)) is { } attrCombo2)
                {
                    attrCombos.Remove(attrCombo2);
                }

                if (classTempAttackSpeed is not null)
                {
                    gameConfiguration.Attributes.Remove(classTempAttackSpeed);
                }
            }

            var areTwoWeaponsEquippedToAttackSpeedByWeapon = context.CreateNew<AttributeRelationship>(
                attackSpeedByWeapon,
                0.5f,
                areTwoWeaponsEquipped,
                InputOperator.ExponentiateByAttribute,
                default(AttributeDefinition?),
                AggregateType.Multiplicate);

            charClass.AttributeCombinations.Add(areTwoWeaponsEquippedToAttackSpeedByWeapon);

            // Remove temp innovation defense decrement combos
            var tempInnovationDefDecrements = gameConfiguration.Attributes.Where(a => a.Designation == "Temp Innovation defense decrement");
            if (tempInnovationDefDecrements.Any())
            {
                AttributeDefinition? classTempInnovationDefDecrement = null;
                if (attrCombos.FirstOrDefault(ac => tempInnovationDefDecrements.Contains(ac.InputAttribute)) is { } attrCombo)
                {
                    attrCombos.Remove(attrCombo);
                    classTempInnovationDefDecrement = attrCombo.InputAttribute;
                }

                if (attrCombos.FirstOrDefault(ac => ac.InputAttribute == innovationDefDecrement && ac.InputOperand == -1) is { } innovationDefDecrementToTempInnovDefDec)
                {
                    innovationDefDecrementToTempInnovDefDec.TargetAttribute = defenseDecrement;
                }

                if (classTempInnovationDefDecrement is not null)
                {
                    gameConfiguration.Attributes.Remove(classTempInnovationDefDecrement);
                }
            }

            // Add new attribute combinations
            var isRestingToHealthRecoveryMultiplier = context.CreateNew<AttributeRelationship>(
                healthRecoveryMultiplier,
                0.03f,
                isResting,
                InputOperator.Multiply,
                default(AttributeDefinition?),
                AggregateType.AddRaw);

            var isRestingToManaRecoveryMultiplier = context.CreateNew<AttributeRelationship>(
                manaRecoveryMultiplier,
                0.03f,
                isResting,
                InputOperator.Multiply,
                default(AttributeDefinition?),
                AggregateType.AddRaw);

            var isInSafezoneToAbilityRecoveryAbsolute = context.CreateNew<AttributeRelationship>(
                abilityRecoveryAbsolute,
                3f,
                isInSafezone,
                InputOperator.Multiply,
                default(AttributeDefinition?),
                AggregateType.AddRaw);

            var isInSafezoneToIsShieldRecoveryActive = context.CreateNew<AttributeRelationship>(
                isShieldRecoveryActive,
                1f,
                isInSafezone,
                InputOperator.Multiply,
                default(AttributeDefinition?),
                AggregateType.AddRaw);

            var shieldRecoveryEverywhereToIsShieldRecoveryActive = context.CreateNew<AttributeRelationship>(
                isShieldRecoveryActive,
                1f,
                shieldRecoveryEverywhere,
                InputOperator.Multiply,
                default(AttributeDefinition?),
                AggregateType.AddRaw);

            var shieldRecoveryHiatusToShieldRecoveryRampFactor = context.CreateNew<AttributeRelationship>(
                shieldRecoveryRampFactor,
                1f / 15f,
                shieldRecoveryHiatus,
                InputOperator.Multiply,
                default(AttributeDefinition?),
                AggregateType.AddRaw);

            var shieldRecoveryRampFactorToShieldRecoveryMultiplier = context.CreateNew<AttributeRelationship>(
                shieldRecoveryMultiplier,
                1f,
                shieldRecoveryRampFactor,
                InputOperator.Multiply,
                default(AttributeDefinition?),
                AggregateType.Multiplicate);

            charClass.AttributeCombinations.Add(isRestingToHealthRecoveryMultiplier);
            charClass.AttributeCombinations.Add(isRestingToManaRecoveryMultiplier);
            charClass.AttributeCombinations.Add(isInSafezoneToAbilityRecoveryAbsolute);
            charClass.AttributeCombinations.Add(isInSafezoneToIsShieldRecoveryActive);
            charClass.AttributeCombinations.Add(shieldRecoveryEverywhereToIsShieldRecoveryActive);
            charClass.AttributeCombinations.Add(shieldRecoveryHiatusToShieldRecoveryRampFactor);
            charClass.AttributeCombinations.Add(shieldRecoveryRampFactorToShieldRecoveryMultiplier);

            // Remove recovery multiplier combos
            if (attrCombos.FirstOrDefault(ac => ac.InputAttribute == isInSafezone && ac.TargetAttribute == healthRecoveryMultiplier) is { } isInSafezoneToHealthRecoveryMultiplier)
            {
                attrCombos.Remove(isInSafezoneToHealthRecoveryMultiplier);
            }

            if (attrCombos.FirstOrDefault(ac => ac.InputAttribute == isInSafezone && ac.TargetAttribute == shieldRecoveryMultiplier) is { } isInSafezoneToShieldRecoveryMultiplier)
            {
                attrCombos.Remove(isInSafezoneToShieldRecoveryMultiplier);
            }

            // Change common base attribute values (ConstValueAttribute)
            if (charClass.BaseAttributeValues.FirstOrDefault(bav => bav.Definition == manaRecoveryMultiplier) is { } baseManaRecoveryMultiplier)
            {
                charClass.BaseAttributeValues.Remove(baseManaRecoveryMultiplier);
            }

            charClass.BaseAttributeValues.Add(context.CreateNew<ConstValueAttribute>(0.037f, manaRecoveryMultiplier, AggregateType.AddRaw));
            charClass.BaseAttributeValues.Add(context.CreateNew<ConstValueAttribute>(4f / 3f, shieldRecoveryRampFactor, AggregateType.AddRaw));

            if (charClass.BaseAttributeValues.FirstOrDefault(bav => bav.Definition == shieldRecoveryMultiplier) is { } baseShieldRecoveryMultiplier)
            {
                charClass.BaseAttributeValues.Remove(baseShieldRecoveryMultiplier);
            }

            charClass.BaseAttributeValues.Add(context.CreateNew<ConstValueAttribute>(100, shieldRecoveryMultiplier, AggregateType.AddRaw));
            charClass.BaseAttributeValues.Add(context.CreateNew<ConstValueAttribute>(1f / 75000, shieldRecoveryMultiplier, AggregateType.Multiplicate));

            // Create new StatAttributDefinitions
            charClass.StatAttributes.Add(context.CreateNew<StatAttributeDefinition>(isResting, 0, false));
            charClass.StatAttributes.Add(context.CreateNew<StatAttributeDefinition>(nearbyPartyMemberCount, 0, false));

            // Change base ability recovery multiplier
            if (charClass.Number != 4 && charClass.Number != 6 && charClass.Number != 7) // DK classes
            {
                if (charClass.BaseAttributeValues.FirstOrDefault(bav => bav.Definition == abilityRecoveryMultiplier) is { } baseAbilityRecoveryMultiplier)
                {
                    charClass.BaseAttributeValues.Remove(baseAbilityRecoveryMultiplier);
                }

                charClass.BaseAttributeValues.Add(context.CreateNew<ConstValueAttribute>(0.03f, abilityRecoveryMultiplier, AggregateType.AddRaw));
            }

            // Add default movement speeds for tier 2 chars
            if (charClass.Number == 16 || charClass.Number == 17 // DL classes
                || charClass.Number == 12 || charClass.Number == 13 // MG classes
                || charClass.Number == 24 || charClass.Number == 25) // RF classes
            {
                charClass.BaseAttributeValues.Add(context.CreateNew<ConstValueAttribute>(MovementSpeedConstants.RunningGearMovementSpeed, movementSpeed, AggregateType.Maximum));
                charClass.BaseAttributeValues.Add(context.CreateNew<ConstValueAttribute>(MovementSpeedConstants.RunningGearMovementSpeed, movementSpeedUnderwater, AggregateType.Maximum));
            }
        });

        // Update master skills
        if (gameConfiguration.Skills.FirstOrDefault(s => s.Number == (short)SkillNumber.SdRecoverySpeedInc)?.MasterDefinition is { } sdRecoverySpeedInc)
        {
            sdRecoverySpeedInc.ValueFormula = $"1 + {SkillsInitializer.FormulaRecoveryIncrease120}";
            sdRecoverySpeedInc.TargetAttribute = shieldRecoveryMultiplier;
            sdRecoverySpeedInc.Aggregation = AggregateType.Multiplicate;
        }

        if (gameConfiguration.Skills.FirstOrDefault(s => s.Number == (short)SkillNumber.IncreaseSdRecoveryRate)?.MasterDefinition is { } increaseSdRecoveryRate)
        {
            increaseSdRecoveryRate.ValueFormula = $"1 + {SkillsInitializer.FormulaRecoveryIncrease120}";
            increaseSdRecoveryRate.TargetAttribute = shieldRecoveryMultiplier;
            increaseSdRecoveryRate.Aggregation = AggregateType.Multiplicate;
        }
    }
}
