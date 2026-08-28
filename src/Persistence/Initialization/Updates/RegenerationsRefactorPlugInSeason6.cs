// <copyright file="RegenerationsRefactorPlugInSeason6.cs" company="MUnique">
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
public class RegenerationsRefactorPlugInSeason6 : RegenerationsRefactorPlugInBase
{
    /// <summary>
    /// The plug in description.
    /// </summary>
    internal new const string PlugInDescription = "This update fixes and reworks some regeneration attributes (health, shield, mana, ability). It also adds default running (and fast swimming) speed for tier 2 chars (MG, DL, RF).";

    /// <inheritdoc />
    public override UpdateVersion Version => UpdateVersion.RegenerationsRefactorSeason6;

    /// <inheritdoc />
    public override string DataInitializationKey => DataInitialization.Id;

    /// <inheritdoc />
    public override string Description => PlugInDescription;

    /// <inheritdoc />
    protected override async ValueTask ApplyAsync(IContext context, GameConfiguration gameConfiguration)
    {
        await base.ApplyAsync(context, gameConfiguration).ConfigureAwait(false);

        // Create new Stats
        var isShieldRecoveryActive = context.CreateNew<AttributeDefinition>(Stats.IsShieldRecoveryActive.Id, Stats.IsShieldRecoveryActive.Designation, Stats.IsShieldRecoveryActive.Description);
        gameConfiguration.Attributes.Add(isShieldRecoveryActive);
        var shieldRecoveryHiatus = context.CreateNew<AttributeDefinition>(Stats.ShieldRecoveryHiatus.Id, Stats.ShieldRecoveryHiatus.Designation, Stats.ShieldRecoveryHiatus.Description);
        gameConfiguration.Attributes.Add(shieldRecoveryHiatus);
        var shieldRecoveryRampFactor = context.CreateNew<AttributeDefinition>(Stats.ShieldRecoveryRampFactor.Id, Stats.ShieldRecoveryRampFactor.Designation, Stats.ShieldRecoveryRampFactor.Description);
        shieldRecoveryRampFactor.MaximumValue = 3;
        gameConfiguration.Attributes.Add(shieldRecoveryRampFactor);

        var isInSafezone = Stats.IsInSafezone.GetPersistent(gameConfiguration);
        var shieldRecoveryMultiplier = Stats.ShieldRecoveryMultiplier.GetPersistent(gameConfiguration);
        var shieldRecoveryEverywhere = Stats.ShieldRecoveryEverywhere.GetPersistent(gameConfiguration);

        var movementSpeed = Stats.MovementSpeed.GetPersistent(gameConfiguration);
        var movementSpeedUnderwater = Stats.MovementSpeedUnderwater.GetPersistent(gameConfiguration);

        gameConfiguration.CharacterClasses.ForEach(charClass =>
        {
            // Remove obsolete shiled recovery combo
            var attrCombos = charClass.AttributeCombinations;
            if (attrCombos.FirstOrDefault(ac => ac.InputAttribute == isInSafezone && ac.TargetAttribute == shieldRecoveryMultiplier) is { } isInSafeZoneToShieldRecoveryMultiplier)
            {
                attrCombos.Remove(isInSafeZoneToShieldRecoveryMultiplier);
            }

            // Add new attribute combinations
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

            charClass.AttributeCombinations.Add(isInSafezoneToIsShieldRecoveryActive);
            charClass.AttributeCombinations.Add(shieldRecoveryEverywhereToIsShieldRecoveryActive);
            charClass.AttributeCombinations.Add(shieldRecoveryHiatusToShieldRecoveryRampFactor);
            charClass.AttributeCombinations.Add(shieldRecoveryRampFactorToShieldRecoveryMultiplier);

            // Change common base attribute values (ConstValueAttribute)
            charClass.BaseAttributeValues.Add(context.CreateNew<ConstValueAttribute>(4f / 3f, shieldRecoveryRampFactor, AggregateType.AddRaw));

            if (charClass.BaseAttributeValues.FirstOrDefault(bav => bav.Definition == shieldRecoveryMultiplier) is { } baseShieldRecoveryMultiplier)
            {
                charClass.BaseAttributeValues.Remove(baseShieldRecoveryMultiplier);
            }

            charClass.BaseAttributeValues.Add(context.CreateNew<ConstValueAttribute>(100, shieldRecoveryMultiplier, AggregateType.AddRaw));
            charClass.BaseAttributeValues.Add(context.CreateNew<ConstValueAttribute>(1f / 75000, shieldRecoveryMultiplier, AggregateType.Multiplicate));

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
