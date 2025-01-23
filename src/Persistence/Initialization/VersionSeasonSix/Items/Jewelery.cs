﻿// <copyright file="Jewelery.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

using MUnique.OpenMU.GameLogic.PlayerActions.Guild;

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

        var eliteSkeletonRing = this.CreateTransformationRing(39, "Elite Transfer Skeleton Ring", 10, 255, CharacterTransformationSkin.EliteSkillSoldier);
        eliteSkeletonRing.PossibleItemOptions.Add(
            this.CreateItemOptionDefinition(
                "Elite Transfer Skeleton Ring",
                ItemOptionDefinitionNumbers.EliteTransferSkeletonRing,
                (Stats.DefenseFinal, 1.1f, AggregateType.Multiplicate),
                (Stats.MaximumHealth, 400, AggregateType.AddRaw))); // todo: added health is equal to Stats.TotalLevel
        this.CreateTransformationRing(40, "Jack Olantern Transformation Ring", 10, 100, CharacterTransformationSkin.JackOlantern);
        var christmasRing = this.CreateTransformationRing(41, "Christmas Transformation Ring", 1, 100, CharacterTransformationSkin.Christmas);
        christmasRing.PossibleItemOptions.Add(
            this.CreateItemOptionDefinition(
                "Christmas Transformation Ring",
                ItemOptionDefinitionNumbers.ChristmasTransformationRing,
                (Stats.MinimumPhysBaseDmg, 20, AggregateType.AddFinal),
                (Stats.MaximumPhysBaseDmg, 20, AggregateType.AddFinal),
                (Stats.MinimumWizBaseDmg, 20, AggregateType.AddFinal),
                (Stats.MaximumWizBaseDmg, 20, AggregateType.AddFinal)));
        this.CreateTransformationRing(42, "Game Master Transformation Ring", 0, 255, CharacterTransformationSkin.GameMaster);
        this.CreateTransformationRing(68, "Snowman Transformation Ring", 10, 100, CharacterTransformationSkin.Snowman);
        var pandaRing = this.CreateTransformationRing(76, "Panda Transformation Ring", 28, 255, CharacterTransformationSkin.Panda);
        pandaRing.PossibleItemOptions.Add(
            this.CreateItemOptionDefinition(
                "Panda Transformation Ring",
                ItemOptionDefinitionNumbers.PandaRing,
                (Stats.BaseDamageBonus, 30, AggregateType.AddRaw),
                (Stats.MoneyAmountRate, 1.5f, AggregateType.Multiplicate),
                (Stats.PandaRingDamageBonus, 30, AggregateType.AddRaw)));
        var skeletonRing = this.CreateTransformationRing(122, "Skeleton Transformation Ring", 1, 255, CharacterTransformationSkin.Skeleton);
        skeletonRing.PossibleItemOptions.Add(
            this.CreateItemOptionDefinition(
                "Skeleton Transformation Ring",
                ItemOptionDefinitionNumbers.SkeletonTransformationRing,
                (Stats.BaseDamageBonus, 40, AggregateType.AddRaw),
                (Stats.ExperienceRate, 1.5f, AggregateType.Multiplicate))); // todo: exp rate only with equipped skeleton pet
        var robotKnightRing = this.CreateTransformationRing(163, "Robot Knight Transformation Ring", 1, 255, (CharacterTransformationSkin)625);
        robotKnightRing.PossibleItemOptions.Add(
            this.CreateItemOptionDefinition(
                "Robot Knight Transformation Ring",
                ItemOptionDefinitionNumbers.RobotKnightTransformationRing,
                (Stats.BaseDamageBonus, 30, AggregateType.AddRaw),
                (Stats.DefenseFinal, 10, AggregateType.AddRaw)));
        var miniRobotRing = this.CreateTransformationRing(164, "Mini Robot Transformation Ring", 1, 255, (CharacterTransformationSkin)626);
        miniRobotRing.PossibleItemOptions.Add(
            this.CreateItemOptionDefinition(
                "Mini Robot Transformation Ring",
                ItemOptionDefinitionNumbers.MiniRobotTransformationRing,
                (Stats.BaseDamageBonus, 30, AggregateType.AddRaw),
                (Stats.AttackSpeed, 7, AggregateType.AddRaw),
                (Stats.MagicSpeed, 7, AggregateType.AddRaw)));
        var greatHeavenlyMageRing = this.CreateTransformationRing(165, "Great Heavenly Mage Transformation Ring", 1, 255, (CharacterTransformationSkin)642);
        greatHeavenlyMageRing.PossibleItemOptions.Add(
            this.CreateItemOptionDefinition(
                "Great Heavenly Mage Transformation Ring",
                ItemOptionDefinitionNumbers.GreatHeavenlyMageTransformationRing,
                (Stats.BaseDamageBonus, 40, AggregateType.AddRaw)));
    }

    private ItemOptionDefinition CreateItemOptionDefinition(string name, short number, params (AttributeDefinition targetOption, float value, AggregateType aggregateType)[] options)
    {
        var optionDefinition = this.Context.CreateNew<ItemOptionDefinition>();
        optionDefinition.SetGuid(number);
        this.GameConfiguration.ItemOptions.Add(optionDefinition);
        optionDefinition.Name = name;
        foreach (var (targetOption, value, aggregateType) in options)
        {
            optionDefinition.PossibleOptions.Add(this.CreateItemOption(targetOption, value, aggregateType, number));
        }

        return optionDefinition;
    }

    private IncreasableItemOption CreateItemOption(AttributeDefinition targetOption, float value, AggregateType aggregateType, short number)
    {
        var itemOption = this.Context.CreateNew<IncreasableItemOption>();
        itemOption.SetGuid(number, targetOption.Id.ExtractFirstTwoBytes());
        itemOption.PowerUpDefinition = this.Context.CreateNew<PowerUpDefinition>();
        itemOption.PowerUpDefinition.TargetAttribute = targetOption.GetPersistent(this.GameConfiguration);
        itemOption.PowerUpDefinition.Boost = this.Context.CreateNew<PowerUpDefinitionValue>();
        itemOption.PowerUpDefinition.Boost.ConstantValue.Value = value;
        itemOption.PowerUpDefinition.Boost.ConstantValue.AggregateType = aggregateType;
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
        var optionDefinition = this.Context.CreateNew<ItemOptionDefinition>();
        optionDefinition.SetGuid(ItemOptionDefinitionNumbers.WizardRing);
        this.GameConfiguration.ItemOptions.Add(optionDefinition);
        ring.PossibleItemOptions.Add(optionDefinition);
        optionDefinition.Name = "Wizard's Ring Options";

        var increaseMinPhysBaseDamage = this.Context.CreateNew<IncreasableItemOption>();
        increaseMinPhysBaseDamage.SetGuid(ItemOptionDefinitionNumbers.WizardRing, 1);
        increaseMinPhysBaseDamage.PowerUpDefinition = this.Context.CreateNew<PowerUpDefinition>();
        increaseMinPhysBaseDamage.PowerUpDefinition.TargetAttribute = Stats.MinimumPhysBaseDmg.GetPersistent(this.GameConfiguration);
        increaseMinPhysBaseDamage.PowerUpDefinition.Boost = this.Context.CreateNew<PowerUpDefinitionValue>();
        increaseMinPhysBaseDamage.PowerUpDefinition.Boost.ConstantValue.Value = 1.1f;
        increaseMinPhysBaseDamage.PowerUpDefinition.Boost.ConstantValue.AggregateType = AggregateType.Multiplicate;
        optionDefinition.PossibleOptions.Add(increaseMinPhysBaseDamage);

        var increaseMaxPhysBaseDamage = this.Context.CreateNew<IncreasableItemOption>();
        increaseMaxPhysBaseDamage.SetGuid(ItemOptionDefinitionNumbers.WizardRing, 2);
        increaseMaxPhysBaseDamage.PowerUpDefinition = this.Context.CreateNew<PowerUpDefinition>();
        increaseMaxPhysBaseDamage.PowerUpDefinition.TargetAttribute = Stats.MaximumPhysBaseDmg.GetPersistent(this.GameConfiguration);
        increaseMaxPhysBaseDamage.PowerUpDefinition.Boost = this.Context.CreateNew<PowerUpDefinitionValue>();
        increaseMaxPhysBaseDamage.PowerUpDefinition.Boost.ConstantValue.Value = 1.1f;
        increaseMaxPhysBaseDamage.PowerUpDefinition.Boost.ConstantValue.AggregateType = AggregateType.Multiplicate;
        optionDefinition.PossibleOptions.Add(increaseMaxPhysBaseDamage);

        var increaseAttackSpeed = this.Context.CreateNew<IncreasableItemOption>();
        increaseAttackSpeed.SetGuid(ItemOptionDefinitionNumbers.WizardRing, 3);
        increaseAttackSpeed.PowerUpDefinition = this.Context.CreateNew<PowerUpDefinition>();
        increaseAttackSpeed.PowerUpDefinition.TargetAttribute = Stats.AttackSpeed.GetPersistent(this.GameConfiguration);
        increaseAttackSpeed.PowerUpDefinition.Boost = this.Context.CreateNew<PowerUpDefinitionValue>();
        increaseAttackSpeed.PowerUpDefinition.Boost.ConstantValue.Value = 10f;
        optionDefinition.PossibleOptions.Add(increaseAttackSpeed);

        var increaseMagicSpeed = this.Context.CreateNew<IncreasableItemOption>();
        increaseMagicSpeed.SetGuid(ItemOptionDefinitionNumbers.WizardRing, 4);
        increaseMagicSpeed.PowerUpDefinition = this.Context.CreateNew<PowerUpDefinition>();
        increaseMagicSpeed.PowerUpDefinition.TargetAttribute = Stats.MagicSpeed.GetPersistent(this.GameConfiguration);
        increaseMagicSpeed.PowerUpDefinition.Boost = this.Context.CreateNew<PowerUpDefinitionValue>();
        increaseMagicSpeed.PowerUpDefinition.Boost.ConstantValue.Value = 10f;
        optionDefinition.PossibleOptions.Add(increaseMagicSpeed);

        var increaseMinWizBaseDamage = this.Context.CreateNew<IncreasableItemOption>();
        increaseMinWizBaseDamage.SetGuid(ItemOptionDefinitionNumbers.WizardRing, 5);
        increaseMinWizBaseDamage.PowerUpDefinition = this.Context.CreateNew<PowerUpDefinition>();
        increaseMinWizBaseDamage.PowerUpDefinition.TargetAttribute = Stats.MinimumWizBaseDmg.GetPersistent(this.GameConfiguration);
        increaseMinWizBaseDamage.PowerUpDefinition.Boost = this.Context.CreateNew<PowerUpDefinitionValue>();
        increaseMinWizBaseDamage.PowerUpDefinition.Boost.ConstantValue.Value = 1.1f;
        increaseMinWizBaseDamage.PowerUpDefinition.Boost.ConstantValue.AggregateType = AggregateType.Multiplicate;
        optionDefinition.PossibleOptions.Add(increaseMinWizBaseDamage);

        var increaseMaxWizBaseDamage = this.Context.CreateNew<IncreasableItemOption>();
        increaseMaxWizBaseDamage.SetGuid(ItemOptionDefinitionNumbers.WizardRing, 6);
        increaseMaxWizBaseDamage.PowerUpDefinition = this.Context.CreateNew<PowerUpDefinition>();
        increaseMaxWizBaseDamage.PowerUpDefinition.TargetAttribute = Stats.MaximumWizBaseDmg.GetPersistent(this.GameConfiguration);
        increaseMaxWizBaseDamage.PowerUpDefinition.Boost = this.Context.CreateNew<PowerUpDefinitionValue>();
        increaseMaxWizBaseDamage.PowerUpDefinition.Boost.ConstantValue.Value = 1.1f;
        increaseMaxWizBaseDamage.PowerUpDefinition.Boost.ConstantValue.AggregateType = AggregateType.Multiplicate;
        optionDefinition.PossibleOptions.Add(increaseMaxWizBaseDamage);

        // Always add all options "randomly" when it drops ;)
        optionDefinition.AddChance = 1.0f;
        optionDefinition.AddsRandomly = true;
        optionDefinition.MaximumOptionsPerItem = 6;
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
}