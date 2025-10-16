// <copyright file="FixJeweleryPetsDamageCalcsPlugInSeason6.cs" company="MUnique">
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
using MUnique.OpenMU.PlugIns;

/// <summary>
/// This update fixes jewelery items and pet options related to damage, and jewelery resistance values.
/// </summary>
[PlugIn(PlugInName, PlugInDescription)]
[Guid("DD5B0424-89DB-4DE4-A1BB-B294F2C1FCE6")]
public class FixJeweleryPetsDamageCalcsPlugInSeason6 : FixJeweleryPetsDamageCalcsPlugInBase
{
    /// <summary>
    /// The plug in description.
    /// </summary>
    internal new const string PlugInName = "Fix Jewelery and Pets Damage Calculations";

    /// <summary>
    /// The plug in description.
    /// </summary>
    internal new const string PlugInDescription = "This update fixes jewelery items and pet options related to damage, and jewelery resistance values.";

    /// <inheritdoc />
    public override string Name => PlugInName;

    /// <inheritdoc />
    public override string Description => PlugInDescription;

    /// <inheritdoc />
    public override string DataInitializationKey => VersionSeasonSix.DataInitialization.Id;

    /// <inheritdoc />
    public override UpdateVersion Version => UpdateVersion.FixJeweleryPetsDamageCalcsSeason6;

    /// <inheritdoc />
    protected override async ValueTask ApplyAsync(IContext context, GameConfiguration gameConfiguration)
    {
        await base.ApplyAsync(context, gameConfiguration).ConfigureAwait(false);

        var jeweleryItemSlots = gameConfiguration.ItemSlotTypes.Where(st => st.ItemSlots.Contains(9) || st.ItemSlots.Contains(10));
        var petItemSlot = gameConfiguration.ItemSlotTypes.First(st => st.ItemSlots.Contains(8));

        var miscItems = gameConfiguration.Items.Where(i => i.Group == (byte)ItemGroups.Misc1);
        var jewelery = miscItems.Where(i => jeweleryItemSlots.Contains(i.ItemSlot));
        var pets = miscItems.Where(i => petItemSlot.Equals(i.ItemSlot));

        // Remove next season rings
        foreach (var jeweleryItem in jewelery.ToList())
        {
            if (jeweleryItem.Number is 163 or 164 or 165)
            {
                gameConfiguration.Items.Remove(jeweleryItem);
            }
        }

        // Update jewelery
#pragma warning disable SA1116, SA1117 // Parameters must be on same line or separate lines
        var eliteSkeletonRing = jewelery.First(i => i.Number == 39);
        this.AddLevelRequirement(context, gameConfiguration, eliteSkeletonRing, 10);
        if (eliteSkeletonRing.PossibleItemOptions.FirstOrDefault() is { } eliteSkeletonRingItemOptDef)
        {
            CreateItemBasePowerUps(eliteSkeletonRing,
                (Stats.DefenseBase, 1.1f, AggregateType.Multiplicate));
            CreateItemOptionDefinition(eliteSkeletonRingItemOptDef, "Elite Skeleton Transformation Ring", ItemOptionDefinitionNumbers.EliteSkeletonTransformationRing,
                (Stats.MaximumHealth, 0, AggregateType.AddRaw, (Stats.Level, 1)));
        }

        var jackOlanternRing = jewelery.First(i => i.Number == 40);
        this.AddLevelRequirement(context, gameConfiguration, jackOlanternRing, 10);

        var christmasRing = jewelery.First(i => i.Number == 41);
        CreateItemBasePowerUps(christmasRing,
            (Stats.BaseDamageBonus, 20, AggregateType.AddRaw));

        var gameMasterRing = jewelery.First(i => i.Number == 42);
        CreateItemBasePowerUps(gameMasterRing,
            (Stats.IceDamageBonus, 255, AggregateType.AddRaw),
            (Stats.PoisonDamageBonus, 255, AggregateType.AddRaw),
            (Stats.LightningDamageBonus, 255, AggregateType.AddRaw),
            (Stats.FireDamageBonus, 255, AggregateType.AddRaw),
            (Stats.EarthDamageBonus, 255, AggregateType.AddRaw),
            (Stats.WindDamageBonus, 255, AggregateType.AddRaw),
            (Stats.WaterDamageBonus, 255, AggregateType.AddRaw),
            (Stats.IceResistance, 255, AggregateType.Maximum),
            (Stats.PoisonResistance, 255, AggregateType.Maximum),
            (Stats.LightningResistance, 255, AggregateType.Maximum),
            (Stats.FireResistance, 255, AggregateType.Maximum),
            (Stats.EarthResistance, 255, AggregateType.Maximum),
            (Stats.WindResistance, 255, AggregateType.Maximum),
            (Stats.WaterResistance, 255, AggregateType.Maximum));

        var snowmanRing = jewelery.First(i => i.Number == 68);
        this.AddLevelRequirement(context, gameConfiguration, snowmanRing, 10);

        var pandaRing = jewelery.First(i => i.Number == 76);
        pandaRing.PossibleItemOptions.Clear();
        CreateItemBasePowerUps(pandaRing,
            (Stats.BaseDamageBonus, 30, AggregateType.AddRaw),
            (Stats.CurseBaseDmg, 30, AggregateType.AddRaw),
            (Stats.MoneyAmountRate, 1.5f, AggregateType.Multiplicate),
            (Stats.FinalDamageBonus, 30, AggregateType.AddRaw));

        var skeletonRing = jewelery.First(i => i.Number == 122);
        if (skeletonRing.PossibleItemOptions.FirstOrDefault() is { } skeletonRingItemOptDef)
        {
            CreateItemBasePowerUps(skeletonRing,
                (Stats.BaseDamageBonus, 40, AggregateType.AddRaw),
                (Stats.CurseBaseDmg, 40, AggregateType.AddRaw));
            CreateItemOptionDefinition(skeletonRingItemOptDef, "Skeleton Transformation Ring", ItemOptionDefinitionNumbers.SkeletonTransformationRing,
                    (Stats.BonusExperienceRate, 0, AggregateType.AddRaw, (Stats.IsPetSkeletonEquipped, 0.3f)));
        }

        var wizardsRing = jewelery.First(i => i.Number == 20);
        wizardsRing.PossibleItemOptions.Clear();
        CreateItemBasePowerUps(wizardsRing,
            (Stats.MinimumPhysBaseDmg, 1.1f, AggregateType.Multiplicate),
            (Stats.MaximumPhysBaseDmg, 1.1f, AggregateType.Multiplicate),
            (Stats.AttackSpeedAny, 10f, AggregateType.AddRaw),
            (Stats.MinimumWizBaseDmg, 1.1f, AggregateType.Multiplicate),
            (Stats.MaximumWizBaseDmg, 1.1f, AggregateType.Multiplicate));

        // Update pets
        var demon = pets.First(i => i.Number == 64);
        demon.BasePowerUpAttributes.Clear();
        CreateItemBasePowerUps(demon,
            (Stats.MinimumPhysBaseDmg, 1.4f, AggregateType.Multiplicate),
            (Stats.MaximumPhysBaseDmg, 1.4f, AggregateType.Multiplicate),
            (Stats.MinimumWizBaseDmg, 1.4f, AggregateType.Multiplicate),
            (Stats.MaximumWizBaseDmg, 1.4f, AggregateType.Multiplicate),
            (Stats.MinimumCurseBaseDmg, 1.4f, AggregateType.Multiplicate),
            (Stats.MaximumCurseBaseDmg, 1.4f, AggregateType.Multiplicate),
            (Stats.AttackSpeedAny, 10f, AggregateType.AddRaw));

        var panda = pets.First(i => i.Number == 80);
        panda.BasePowerUpAttributes.Clear();
        CreateItemBasePowerUps(panda,
            (Stats.BonusExperienceRate, 0.5f, AggregateType.AddRaw),
            (Stats.DefenseFinal, 50f, AggregateType.AddRaw));

        var unicorn = pets.First(i => i.Number == 106);
        unicorn.BasePowerUpAttributes.Clear();
        CreateItemBasePowerUps(unicorn,
            (Stats.MoneyAmountRate, 1.5f, AggregateType.Multiplicate),
            (Stats.DefenseFinal, 50f, AggregateType.AddRaw));

        var skeleton = pets.First(i => i.Number == 123);
        skeleton.BasePowerUpAttributes.Clear();
        CreateItemBasePowerUps(skeleton,
            (Stats.MinimumPhysBaseDmg, 1.2f, AggregateType.Multiplicate),
            (Stats.MaximumPhysBaseDmg, 1.2f, AggregateType.Multiplicate),
            (Stats.MinimumWizBaseDmg, 1.2f, AggregateType.Multiplicate),
            (Stats.MaximumWizBaseDmg, 1.2f, AggregateType.Multiplicate),
            (Stats.MinimumCurseBaseDmg, 1.2f, AggregateType.Multiplicate),
            (Stats.MaximumCurseBaseDmg, 1.2f, AggregateType.Multiplicate),
            (Stats.AttackSpeedAny, 10f, AggregateType.AddRaw),
            (Stats.BonusExperienceRate, 0.3f, AggregateType.AddRaw),
            (Stats.IsPetSkeletonEquipped, 1, AggregateType.AddRaw));
#pragma warning restore SA1116, SA1117

        void CreateItemBasePowerUps(ItemDefinition item, params (AttributeDefinition AttributeDefinition, float Value, AggregateType AggregateType)[] powerUps)
        {
            foreach (var (attributeDefinition, value, aggregateType) in powerUps)
            {
                var powerUpDefinition = context.CreateNew<ItemBasePowerUpDefinition>();
                powerUpDefinition.TargetAttribute = attributeDefinition.GetPersistent(gameConfiguration);
                powerUpDefinition.BaseValue = value;
                powerUpDefinition.AggregateType = aggregateType;
                item.BasePowerUpAttributes.Add(powerUpDefinition);
            }
        }

        void CreateItemOptionDefinition(ItemOptionDefinition itemOptionDef, string name, short number, (AttributeDefinition TargetOption, float Value, AggregateType AggregateType, (AttributeDefinition SourceAttribute, float Multiplier)) option)
        {
            itemOptionDef.Name = name;

            foreach (var possibleOption in itemOptionDef.PossibleOptions.ToList())
            {
                if (possibleOption.PowerUpDefinition?.TargetAttribute != option.TargetOption)
                {
                    itemOptionDef.PossibleOptions.Remove(possibleOption);
                }
            }

            if (itemOptionDef.PossibleOptions.Count == 0)
            {
                itemOptionDef.PossibleOptions.Add(CreateItemOption(option.TargetOption, option.Value, option.AggregateType, number, (option.Item4.SourceAttribute, option.Item4.Multiplier)));
            }
            else
            {
                // Update existing option
                var possibleOption = itemOptionDef.PossibleOptions.First();
                possibleOption.PowerUpDefinition!.Boost!.ConstantValue.Value = option.Value;
                possibleOption.PowerUpDefinition.Boost.ConstantValue.AggregateType = option.AggregateType;

                var attributeRelationship = context.CreateNew<AttributeRelationship>();
                attributeRelationship.SetGuid(number, option.TargetOption.Id.ExtractFirstTwoBytes(), 0);
                attributeRelationship.InputAttribute = option.Item4.SourceAttribute.GetPersistent(gameConfiguration);
                attributeRelationship.InputOperator = InputOperator.Multiply;
                attributeRelationship.InputOperand = option.Item4.Multiplier;
                possibleOption.PowerUpDefinition.Boost.RelatedValues.Add(attributeRelationship);
            }

            // Always add all options "randomly" when it drops ;)
            itemOptionDef.AddChance = 1.0f;
            itemOptionDef.AddsRandomly = true;
            itemOptionDef.MaximumOptionsPerItem = 1;
        }

        IncreasableItemOption CreateItemOption(AttributeDefinition targetOption, float value, AggregateType aggregateType, short number, (AttributeDefinition? SourceAttribute, float Multiplier) relatedAttribute)
        {
            var itemOption = context.CreateNew<IncreasableItemOption>();
            itemOption.SetGuid(number, targetOption.Id.ExtractFirstTwoBytes());
            itemOption.PowerUpDefinition = context.CreateNew<PowerUpDefinition>();
            itemOption.PowerUpDefinition.TargetAttribute = targetOption.GetPersistent(gameConfiguration);
            itemOption.PowerUpDefinition.Boost = context.CreateNew<PowerUpDefinitionValue>();
            itemOption.PowerUpDefinition.Boost.ConstantValue.Value = value;
            itemOption.PowerUpDefinition.Boost.ConstantValue.AggregateType = aggregateType;

            if (relatedAttribute.SourceAttribute is not null)
            {
                var attributeRelationship = context.CreateNew<AttributeRelationship>();
                attributeRelationship.SetGuid(number, targetOption.Id.ExtractFirstTwoBytes(), 0);
                attributeRelationship.InputAttribute = relatedAttribute.SourceAttribute.GetPersistent(gameConfiguration);
                attributeRelationship.InputOperator = InputOperator.Multiply;
                attributeRelationship.InputOperand = relatedAttribute.Multiplier;
                itemOption.PowerUpDefinition.Boost.RelatedValues.Add(attributeRelationship);
            }

            return itemOption;
        }

        // Update skills
        if (gameConfiguration.Skills.FirstOrDefault(s => s.Number == (short)SkillNumber.PoisonResistanceInc)?.MasterDefinition is { } poisonResistanceInc)
        {
            poisonResistanceInc.ValueFormula = poisonResistanceInc.DisplayValueFormula;
        }

        if (gameConfiguration.Skills.FirstOrDefault(s => s.Number == (short)SkillNumber.LightningResistanceInc)?.MasterDefinition is { } lightningResistanceInc)
        {
            lightningResistanceInc.ValueFormula = lightningResistanceInc.DisplayValueFormula;
        }

        if (gameConfiguration.Skills.FirstOrDefault(s => s.Number == (short)SkillNumber.IceResistanceIncrease)?.MasterDefinition is { } iceResistanceIncrease)
        {
            iceResistanceIncrease.ValueFormula = iceResistanceIncrease.DisplayValueFormula;
        }
    }
}