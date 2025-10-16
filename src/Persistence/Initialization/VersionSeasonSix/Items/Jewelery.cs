// <copyright file="Jewelery.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.Persistence.Initialization.VersionSeasonSix.Items;

using MUnique.OpenMU.AttributeSystem;
using MUnique.OpenMU.DataModel.Attributes;
using MUnique.OpenMU.DataModel.Configuration;
using MUnique.OpenMU.DataModel.Configuration.Items;
using MUnique.OpenMU.GameLogic.Attributes;
using MUnique.OpenMU.Persistence.Initialization.Items;

/// <summary>
/// Initializer for jewelery (rings and pendants).
/// </summary>
internal class Jewelery : Version095d.Items.Jewelery
{
    /// <summary>
    /// Initializes a new instance of the <see cref="Jewelery"/> class.
    /// </summary>
    /// <param name="context">The context.</param>
    /// <param name="gameConfiguration">The game configuration.</param>
    public Jewelery(IContext context, GameConfiguration gameConfiguration)
        : base(context, gameConfiguration)
    {
    }

    /// <inheritdoc/>
    protected override void CreateItems()
    {
        base.CreateItems();

        this.CreateRing(21, "Ring of Fire", 30, 50, Stats.HealthRecoveryMultiplier, Stats.FireResistance);
        this.CreateRing(22, "Ring of Earth", 38, 50, Stats.HealthRecoveryMultiplier, Stats.EarthResistance);
        this.CreateRing(23, "Ring of Wind", 44, 50, Stats.HealthRecoveryMultiplier, Stats.WindResistance);
        this.CreateRing(24, "Ring of Magic", 47, 50, Stats.MaximumMana, null);

        this.CreatePendant(25, "Pendant of Ice", 34, 50, DamageType.Wizardry, Stats.HealthRecoveryMultiplier, Stats.IceResistance);
        this.CreatePendant(26, "Pendant of Wind", 42, 50, DamageType.Physical, Stats.HealthRecoveryMultiplier, Stats.WindResistance);
        this.CreatePendant(27, "Pendant of Water", 46, 50, DamageType.Wizardry, Stats.HealthRecoveryMultiplier, Stats.WaterResistance);
        this.CreatePendant(28, "Pendant of Ability", 50, 50, DamageType.Physical, Stats.MaximumAbility, null);

        // Requirement for Kanturu Event:
        var moonStonePendant = this.CreateJewelery(38, 10, false, "Moonstone Pendant", 21, 120, null, null, null);
        {
            var powerUp = this.Context.CreateNew<ItemBasePowerUpDefinition>();
            powerUp.TargetAttribute = Stats.MoonstonePendantEquipped.GetPersistent(this.GameConfiguration);
            powerUp.BaseValue = 1;
            moonStonePendant.BasePowerUpAttributes.Add(powerUp);
        }

        this.CreateWizardsRing();

        /* Cash shop rings
        this.CreateJewelery(107, 10, false, "Lethal Wizard's Ring", 0, 100, null, null, null);
        this.CreateJewelery(109, 10, false, "Sapphire Ring", 0, 0, null, null, null);
        this.CreateJewelery(110, 10, false, "Ruby Ring", 0, 0, null, null, null);
        this.CreateJewelery(111, 10, false, "Topaz Ring", 0, 0, null, null, null);
        this.CreateJewelery(112, 10, false, "Amethyst Ring", 0, 0, null, null, null);
        this.CreateJewelery(113, 9, false, "Ruby Necklace", 0, 0, null, null, null);
        this.CreateJewelery(114, 9, false, "Emerald Necklace", 0, 0, null, null, null);
        this.CreateJewelery(115, 9, false, "Sapphire Necklace", 0, 0, null, null, null);
        */

#pragma warning disable SA1117 // Parameters should be on same line or separete lines
        var eliteSkeletonRing = this.CreateTransformationRing(39, "Elite Skeleton Transformation Ring", 10, 255, 10, CharacterTransformationSkin.EliteSkeleton,
            (Stats.DefenseBase, 1.1f, AggregateType.Multiplicate));
        eliteSkeletonRing.PossibleItemOptions.Add(
            this.CreateItemOptionDefinition("Elite Skeleton Transformation Ring", ItemOptionDefinitionNumbers.EliteSkeletonTransformationRing,
                (Stats.MaximumHealth, 0, AggregateType.AddRaw, (Stats.Level, 1))));

        this.CreateTransformationRing(40, "Jack O'lantern Transformation Ring", 10, 100, 10, CharacterTransformationSkin.JackOlantern);
        this.CreateTransformationRing(41, "Christmas Transformation Ring", 1, 100, 0, CharacterTransformationSkin.Christmas,
            (Stats.BaseDamageBonus, 20, AggregateType.AddRaw));
        this.CreateTransformationRing(42, "Game Master Transformation Ring", 0, 255, 0, CharacterTransformationSkin.GameMaster,
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
        this.CreateTransformationRing(68, "Snowman Transformation Ring", 10, 100, 10, CharacterTransformationSkin.Snowman);
        this.CreateTransformationRing(76, "Panda Transformation Ring", 28, 255, 0, CharacterTransformationSkin.Panda,
            (Stats.BaseDamageBonus, 30, AggregateType.AddRaw),
            (Stats.CurseBaseDmg, 30, AggregateType.AddRaw),
            (Stats.MoneyAmountRate, 1.5f, AggregateType.Multiplicate),
            (Stats.FinalDamageBonus, 30, AggregateType.AddRaw));

        var skeletonRing = this.CreateTransformationRing(122, "Skeleton Transformation Ring", 1, 255, 0, CharacterTransformationSkin.Skeleton,
            (Stats.BaseDamageBonus, 40, AggregateType.AddRaw),
            (Stats.CurseBaseDmg, 40, AggregateType.AddRaw));
        skeletonRing.PossibleItemOptions.Add(
            this.CreateItemOptionDefinition("Skeleton Transformation Ring", ItemOptionDefinitionNumbers.SkeletonTransformationRing,
                (Stats.BonusExperienceRate, 0, AggregateType.AddRaw, (Stats.IsPetSkeletonEquipped, 0.3f))));

        /* Next season xfm rings
        var robotKnightRing = this.CreateTransformationRing(163, "Robot Knight Transformation Ring", 1, 255, ??, (CharacterTransformationSkin)625,
            (Stats.BaseDamageBonus, 30, AggregateType.AddRaw),
            (Stats.CurseBaseDmg, 30, AggregateType.AddRaw),
            (Stats.DefenseFinal, 10, AggregateType.AddRaw));
        var miniRobotRing = this.CreateTransformationRing(164, "Mini Robot Transformation Ring", 1, 255, ??, (CharacterTransformationSkin)626,
            (Stats.BaseDamageBonus, 30, AggregateType.AddRaw),
            (Stats.CurseBaseDmg, 30, AggregateType.AddRaw),
            (Stats.AttackSpeedAny, 7, AggregateType.AddRaw));
        var greatHeavenlyMageRing = this.CreateTransformationRing(165, "Great Heavenly Mage Transformation Ring", 1, 255, ??, (CharacterTransformationSkin)642,
            (Stats.BaseDamageBonus, 40, AggregateType.AddRaw),
            (Stats.CurseBaseDmg, 40, AggregateType.AddRaw));*/
#pragma warning restore SA1011
    }

    private ItemOptionDefinition CreateItemOptionDefinition(string name, short number, params (AttributeDefinition TargetOption, float Value, AggregateType AggregateType, (AttributeDefinition? SourceAttribute, float Multiplier))[] options)
    {
        var optionDefinition = this.Context.CreateNew<ItemOptionDefinition>();
        optionDefinition.SetGuid(number);
        this.GameConfiguration.ItemOptions.Add(optionDefinition);
        optionDefinition.Name = name;
        foreach (var (targetOption, value, aggregateType, (sourceAttribute, multiplier)) in options)
        {
            optionDefinition.PossibleOptions.Add(this.CreateItemOption(targetOption, value, aggregateType, number, (sourceAttribute, multiplier)));
        }

        // Always add all options "randomly" when it drops ;)
        optionDefinition.AddChance = 1.0f;
        optionDefinition.AddsRandomly = true;
        optionDefinition.MaximumOptionsPerItem = options.Length;

        return optionDefinition;
    }

    private IncreasableItemOption CreateItemOption(AttributeDefinition targetOption, float value, AggregateType aggregateType, short number, (AttributeDefinition? SourceAttribute, float Multiplier) relatedAttribute)
    {
        var itemOption = this.Context.CreateNew<IncreasableItemOption>();
        itemOption.SetGuid(number, targetOption.Id.ExtractFirstTwoBytes());
        itemOption.PowerUpDefinition = this.Context.CreateNew<PowerUpDefinition>();
        itemOption.PowerUpDefinition.TargetAttribute = targetOption.GetPersistent(this.GameConfiguration);
        itemOption.PowerUpDefinition.Boost = this.Context.CreateNew<PowerUpDefinitionValue>();
        itemOption.PowerUpDefinition.Boost.ConstantValue.Value = value;
        itemOption.PowerUpDefinition.Boost.ConstantValue.AggregateType = aggregateType;

        if (relatedAttribute.SourceAttribute is not null)
        {
            var attributeRelationship = this.Context.CreateNew<AttributeRelationship>();
            attributeRelationship.SetGuid(number, targetOption.Id.ExtractFirstTwoBytes(), 0);
            attributeRelationship.InputAttribute = relatedAttribute.SourceAttribute.GetPersistent(this.GameConfiguration);
            attributeRelationship.InputOperator = InputOperator.Multiply;
            attributeRelationship.InputOperand = relatedAttribute.Multiplier;
            itemOption.PowerUpDefinition.Boost.RelatedValues.Add(attributeRelationship);
        }

        return itemOption;
    }

    /// <summary>
    /// Creates the wizard's ring which is dropped by the white wizard (level 0).
    /// Level 1 and 2 are Ring of Warriors which can be dropped at level 40 and 80, <see cref="BoxOfLuck.CreateWizardsRings"/>.
    /// These can be equipped but have no options, and are bound to the character.
    /// </summary>
    /// <remarks>
    /// Options:
    /// Increase Damage +10%
    /// Increase Wizardry Damage +10%
    /// Increase Attacking (Wizardry) Speed +10.
    /// </remarks>
    private void CreateWizardsRing()
    {
        var ring = this.CreateJewelery(20, 10, false, "Wizard's Ring", 0, 250, null, null, null);
        ring.MaximumItemLevel = 2;
        ring.IsBoundToCharacter = true;
        ring.Durability = 30;

        ring.BasePowerUpAttributes.Add(this.CreateItemBasePowerUpDefinition(Stats.MinimumPhysBaseDmg, 1.1f, AggregateType.Multiplicate));
        ring.BasePowerUpAttributes.Add(this.CreateItemBasePowerUpDefinition(Stats.MaximumPhysBaseDmg, 1.1f, AggregateType.Multiplicate));
        ring.BasePowerUpAttributes.Add(this.CreateItemBasePowerUpDefinition(Stats.AttackSpeedAny, 10f, AggregateType.AddRaw));
        ring.BasePowerUpAttributes.Add(this.CreateItemBasePowerUpDefinition(Stats.MinimumWizBaseDmg, 1.1f, AggregateType.Multiplicate));
        ring.BasePowerUpAttributes.Add(this.CreateItemBasePowerUpDefinition(Stats.MaximumWizBaseDmg, 1.1f, AggregateType.Multiplicate));
    }

    /// <summary>
    /// Creates a ring.
    /// </summary>
    /// <param name="number">The number.</param>
    /// <param name="name">The name.</param>
    /// <param name="level">The level.</param>
    /// <param name="durability">The durability.</param>
    /// <param name="optionTargetAttribute">The option target attribute.</param>
    /// <param name="resistanceAttribute">The resistance attribute.</param>
    /// <remarks>
    /// Rings always have defensive excellent options.
    /// </remarks>
    private void CreateRing(byte number, string name, byte level, byte durability, AttributeDefinition? optionTargetAttribute, AttributeDefinition? resistanceAttribute)
    {
        this.CreateJewelery(number, 10, true, name, level, durability, this.GameConfiguration.ExcellentDefenseOptions(), optionTargetAttribute, resistanceAttribute);
    }

    /// <summary>
    /// Creates a pendant.
    /// </summary>
    /// <param name="number">The number.</param>
    /// <param name="name">The name.</param>
    /// <param name="level">The level.</param>
    /// <param name="durability">The durability.</param>
    /// <param name="excellentOptionDamageType">Type of the excellent option damage.</param>
    /// <param name="optionTargetAttribute">The option target attribute.</param>
    /// <param name="resistanceAttribute">The resistance attribute.</param>
    /// <remarks>
    /// Pendants always have offensive excellent options. If it's wizardry or physical depends on the specific item. I didn't find a pattern yet.
    /// </remarks>
    private void CreatePendant(byte number, string name, byte level, byte durability, DamageType excellentOptionDamageType, AttributeDefinition? optionTargetAttribute, AttributeDefinition? resistanceAttribute)
    {
        var excellentOption = excellentOptionDamageType == DamageType.Physical
            ? this.GameConfiguration.ExcellentPhysicalAttackOptions()
            : this.GameConfiguration.ExcellentWizardryAttackOptions();
        this.CreateJewelery(number, 9, true, name, level, durability, excellentOption, optionTargetAttribute, resistanceAttribute);
    }

    private ItemDefinition CreateJewelery(byte number, int slot, bool dropsFromMonsters, string name, byte level, byte durability, ItemOptionDefinition? excellentOptionDefinition, AttributeDefinition? optionTargetAttribute, AttributeDefinition? resistanceAttribute)
    {
        var item = this.CreateJewelery(number, slot, dropsFromMonsters, name, level, durability, excellentOptionDefinition, resistanceAttribute, optionTargetAttribute == Stats.HealthRecoveryMultiplier);

        if (optionTargetAttribute != Stats.HealthRecoveryMultiplier && optionTargetAttribute is not null)
        {
            // Then it's either maximum mana or ability increase by 1% for each option level
            var option = this.CreateOption("Jewelery option " + optionTargetAttribute.Designation, optionTargetAttribute, 0.01f, item.GetItemId(), AggregateType.Multiplicate);

            item.PossibleItemOptions.Add(option);
        }

        return item;
    }

    /// <summary>
    /// Creates a transformation ring.
    /// </summary>
    /// <param name="number">The number.</param>
    /// <param name="name">The name.</param>
    /// <param name="dropLevel">The level.</param>
    /// <param name="durability">The durability.</param>
    /// <param name="requiredlevel">The required level to wear.</param>
    /// <param name="transformationSkin">The transformation skin.</param>
    /// <param name="basePowerUps">The base power ups.</param>
    /// <returns>The definition of the created ring.</returns>
    private ItemDefinition CreateTransformationRing(byte number, string name, byte dropLevel, byte durability, byte requiredlevel, CharacterTransformationSkin transformationSkin, params (AttributeDefinition, float, AggregateType)[] basePowerUps)
    {
        var ring = this.CreateTransformationRing(number, name, dropLevel, durability, requiredlevel, [transformationSkin]);

        foreach (var (attribute, value, aggregateType) in basePowerUps)
        {
            ring.BasePowerUpAttributes.Add(this.CreateItemBasePowerUpDefinition(attribute, value, aggregateType));
        }

        return ring;
    }
}