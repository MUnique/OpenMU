// <copyright file="RegenerationsRefactorPlugInBase.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.Persistence.Initialization.Updates;

using MUnique.OpenMU.AttributeSystem;
using MUnique.OpenMU.DataModel.Configuration;
using MUnique.OpenMU.GameLogic;
using MUnique.OpenMU.GameLogic.Attributes;

/// <summary>
/// This update fixes and reworks some regeneration attributes (health, mana, ability).
/// </summary>
public abstract class RegenerationsRefactorPlugInBase : UpdatePlugInBase
{
    /// <summary>
    /// The plug in name.
    /// </summary>
    internal const string PlugInName = "Regenerations Refactor";

    /// <summary>
    /// The plug in description.
    /// </summary>
    internal const string PlugInDescription = "This update fixes and reworks some regeneration attributes (health, mana, ability).";

    /// <inheritdoc />
    public override string Name => PlugInName;

    /// <inheritdoc />
    public override string Description => PlugInDescription;

    /// <inheritdoc />
    public override bool IsMandatory => true;

    /// <inheritdoc />
    public override DateTime CreatedAt => new(2026, 8, 27, 16, 0, 0, DateTimeKind.Utc);

    /// <inheritdoc />
    protected override async ValueTask ApplyAsync(IContext context, GameConfiguration gameConfiguration)
    {
        // Create new Stats
        var isResting = context.CreateNew<AttributeDefinition>(Stats.IsResting.Id, Stats.IsResting.Designation, Stats.IsResting.Description);
        gameConfiguration.Attributes.Add(isResting);

        var areTwoWeaponsEquipped = Stats.AreTwoWeaponsEquipped.GetPersistent(gameConfiguration);
        var attackSpeedByWeapon = Stats.AttackSpeedByWeapon.GetPersistent(gameConfiguration);
        var innovationDefDecrement = Stats.InnovationDefDecrement.GetPersistent(gameConfiguration);
        var defenseDecrement = Stats.DefenseDecrement.GetPersistent(gameConfiguration);

        var healthRecoveryMultiplier = Stats.HealthRecoveryMultiplier.GetPersistent(gameConfiguration);
        var manaRecoveryMultiplier = Stats.ManaRecoveryMultiplier.GetPersistent(gameConfiguration);
        var abilityRecoveryMultiplier = Stats.AbilityRecoveryMultiplier.GetPersistent(gameConfiguration);
        var abilityRecoveryAbsolute = Stats.AbilityRecoveryAbsolute.GetPersistent(gameConfiguration);
        var isInSafezone = Stats.IsInSafezone.GetPersistent(gameConfiguration);
        var nearbyPartyMemberCount = Stats.NearbyPartyMemberCount.GetPersistent(gameConfiguration);

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

            charClass.AttributeCombinations.Add(isRestingToHealthRecoveryMultiplier);
            charClass.AttributeCombinations.Add(isRestingToManaRecoveryMultiplier);
            charClass.AttributeCombinations.Add(isInSafezoneToAbilityRecoveryAbsolute);

            // Remove recovery multiplier combos
            if (attrCombos.FirstOrDefault(ac => ac.InputAttribute == isInSafezone && ac.TargetAttribute == healthRecoveryMultiplier) is { } isInSafezoneToHealthRecoveryMultiplier)
            {
                attrCombos.Remove(isInSafezoneToHealthRecoveryMultiplier);
            }

            // Change common base attribute values (ConstValueAttribute)
            if (charClass.BaseAttributeValues.FirstOrDefault(bav => bav.Definition == manaRecoveryMultiplier) is { } baseManaRecoveryMultiplier)
            {
                charClass.BaseAttributeValues.Remove(baseManaRecoveryMultiplier);
            }

            charClass.BaseAttributeValues.Add(context.CreateNew<ConstValueAttribute>(0.037f, manaRecoveryMultiplier, AggregateType.AddRaw));

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
        });
    }
}
