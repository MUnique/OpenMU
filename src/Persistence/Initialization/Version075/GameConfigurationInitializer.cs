﻿// <copyright file="GameConfigurationInitializer.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.Persistence.Initialization.Version075;

using System.Reflection;
using MUnique.OpenMU.AttributeSystem;
using MUnique.OpenMU.DataModel.Attributes;
using MUnique.OpenMU.DataModel.Configuration;
using MUnique.OpenMU.DataModel.Configuration.Items;
using MUnique.OpenMU.GameLogic.Attributes;
using MUnique.OpenMU.Persistence.Initialization.CharacterClasses;
using MUnique.OpenMU.Persistence.Initialization.Version075.Items;
using MUnique.OpenMU.Persistence.Initialization.VersionSeasonSix.Maps;

/// <summary>
/// Initializes the <see cref="GameConfiguration"/>.
/// </summary>
public class GameConfigurationInitializer : InitializerBase
{
    /// <summary>
    /// Initializes a new instance of the <see cref="GameConfigurationInitializer"/> class.
    /// </summary>
    /// <param name="context">The context.</param>
    /// <param name="gameConfiguration">The game configuration.</param>
    public GameConfigurationInitializer(IContext context, GameConfiguration gameConfiguration)
        : base(context, gameConfiguration)
    {
    }

    /// <inheritdoc />
    public override void Initialize()
    {
        this.GameConfiguration.ExperienceRate = 1.0f;
        this.GameConfiguration.MaximumLevel = 400;
        this.GameConfiguration.InfoRange = 12;
        this.GameConfiguration.AreaSkillHitsPlayer = false;
        this.GameConfiguration.MaximumInventoryMoney = int.MaxValue;
        this.GameConfiguration.MaximumVaultMoney = int.MaxValue;
        this.GameConfiguration.RecoveryInterval = 3000;
        this.GameConfiguration.MaximumLetters = 50;
        this.GameConfiguration.LetterSendPrice = 1000;
        this.GameConfiguration.MaximumCharactersPerAccount = 5;
        this.GameConfiguration.CharacterNameRegex = "^[a-zA-Z0-9]{3,10}$";
        this.GameConfiguration.MaximumPasswordLength = 20;
        this.GameConfiguration.MaximumPartySize = 5;
        this.GameConfiguration.ShouldDropMoney = true;
        this.GameConfiguration.DamagePerOneItemDurability = 2000;
        this.GameConfiguration.DamagePerOnePetDurability = 100000;
        this.GameConfiguration.HitsPerOneItemDurability = 10000;
        this.GameConfiguration.ExperienceTable =
            Enumerable.Range(0, this.GameConfiguration.MaximumLevel + 2)
                .Select(level => CalculateNeededExperience(level))
                .ToArray();
        this.GameConfiguration.MasterExperienceTable =
            Enumerable.Range(0, 201).Select(level => this.CalcNeededMasterExp(level)).ToArray();

        var moneyDropItemGroup = this.Context.CreateNew<DropItemGroup>();
        moneyDropItemGroup.Chance = 0.5;
        moneyDropItemGroup.ItemType = SpecialItemType.Money;
        moneyDropItemGroup.Description = "The common money drop item group (50 % drop chance)";
        this.GameConfiguration.DropItemGroups.Add(moneyDropItemGroup);
        BaseMapInitializer.RegisterDefaultDropItemGroup(moneyDropItemGroup);

        var randomItemDropItemGroup = this.Context.CreateNew<DropItemGroup>();
        randomItemDropItemGroup.Chance = 0.3;
        randomItemDropItemGroup.ItemType = SpecialItemType.RandomItem;
        randomItemDropItemGroup.Description = "The common drop item group for random items (30 % drop chance)";
        this.GameConfiguration.DropItemGroups.Add(randomItemDropItemGroup);
        BaseMapInitializer.RegisterDefaultDropItemGroup(randomItemDropItemGroup);

        this.CreateStatAttributes();

        this.CreateItemSlotTypes();
        this.CreateItemOptionTypes();
        this.GameConfiguration.ItemOptions.Add(this.CreateLuckOptionDefinition());
        this.GameConfiguration.ItemOptions.Add(this.CreateOptionDefinition(Stats.DefenseBase));
        this.GameConfiguration.ItemOptions.Add(this.CreateOptionDefinition(Stats.MaximumPhysBaseDmg));
        this.GameConfiguration.ItemOptions.Add(this.CreateOptionDefinition(Stats.MaximumWizBaseDmg));

        new CharacterClassInitialization(this.Context, this.GameConfiguration).Initialize();

        new SkillsInitializer(this.Context, this.GameConfiguration).Initialize();
        new Orbs(this.Context, this.GameConfiguration).Initialize();
        new Scrolls(this.Context, this.GameConfiguration).Initialize();
        new Jewels(this.Context, this.GameConfiguration).Initialize();
        new Armors(this.Context, this.GameConfiguration).Initialize();
        new Wings(this.Context, this.GameConfiguration).Initialize();
        new Pets(this.Context, this.GameConfiguration).Initialize();
        new Weapons(this.Context, this.GameConfiguration).Initialize();
        new Potions(this.Context, this.GameConfiguration).Initialize();
        new Jewelery(this.Context, this.GameConfiguration).Initialize();

        new NpcInitialization(this.Context, this.GameConfiguration).Initialize();

        new GameMapsInitializer(this.Context, this.GameConfiguration).Initialize();
        this.AssignCharacterClassHomeMaps();
        new ChaosMixes(this.Context, this.GameConfiguration).Initialize(); // todo
        new Gates(this.Context, this.GameConfiguration).Initialize();
        //// TODO: ItemSetGroups for set bonus
    }

    /// <summary>
    /// Calculates the needed experience for the specified character level.
    /// </summary>
    /// <param name="level">The character level.</param>
    /// <returns>The calculated needed experience.</returns>
    internal static long CalculateNeededExperience(long level)
    {
        if (level == 0)
        {
            return 0;
        }

        if (level < 256)
        {
            return 10 * (level + 8) * (level - 1) * (level - 1);
        }

        return (10 * (level + 8) * (level - 1) * (level - 1)) +
               (1000 * (level - 247) * (level - 256) * (level - 256));
    }

    private ItemOptionDefinition CreateLuckOptionDefinition()
    {
        var definition = this.Context.CreateNew<ItemOptionDefinition>();

        definition.Name = "Luck";
        definition.AddChance = 0.25f;
        definition.AddsRandomly = true;
        definition.MaximumOptionsPerItem = 1;

        var itemOption = this.Context.CreateNew<IncreasableItemOption>();
        itemOption.OptionType =
            this.GameConfiguration.ItemOptionTypes.FirstOrDefault(o => o == ItemOptionTypes.Luck);
        itemOption.PowerUpDefinition = this.Context.CreateNew<PowerUpDefinition>();
        itemOption.PowerUpDefinition.TargetAttribute =
            this.GameConfiguration.Attributes.FirstOrDefault(a => a == Stats.CriticalDamageChance);
        itemOption.PowerUpDefinition.Boost = this.Context.CreateNew<PowerUpDefinitionValue>();
        itemOption.PowerUpDefinition.Boost.ConstantValue!.Value = 0.05f;
        definition.PossibleOptions.Add(itemOption);

        return definition;
    }

    private ItemOptionDefinition CreateOptionDefinition(AttributeDefinition attributeDefinition)
    {
        var definition = this.Context.CreateNew<ItemOptionDefinition>();

        definition.Name = attributeDefinition.Designation + " Option";
        definition.AddChance = 0.25f;
        definition.AddsRandomly = true;
        definition.MaximumOptionsPerItem = 1;

        var itemOption = this.Context.CreateNew<IncreasableItemOption>();
        itemOption.OptionType =
            this.GameConfiguration.ItemOptionTypes.FirstOrDefault(o => o == ItemOptionTypes.Option);
        itemOption.PowerUpDefinition = this.Context.CreateNew<PowerUpDefinition>();
        itemOption.PowerUpDefinition.TargetAttribute =
            this.GameConfiguration.Attributes.First(a => a == attributeDefinition);
        itemOption.PowerUpDefinition.Boost = this.Context.CreateNew<PowerUpDefinitionValue>();
        itemOption.PowerUpDefinition.Boost.ConstantValue!.Value = 4;
        for (int level = 2; level <= Constants.MaximumOptionLevel; level++)
        {
            var levelDependentOption = this.Context.CreateNew<ItemOptionOfLevel>();
            levelDependentOption.Level = level;
            var powerUpDefinition = this.Context.CreateNew<PowerUpDefinition>();
            powerUpDefinition.TargetAttribute = itemOption.PowerUpDefinition.TargetAttribute;
            powerUpDefinition.Boost = this.Context.CreateNew<PowerUpDefinitionValue>();
            powerUpDefinition.Boost.ConstantValue!.Value = level * 4;
            levelDependentOption.PowerUpDefinition = powerUpDefinition;
            itemOption.LevelDependentOptions.Add(levelDependentOption);
        }

        definition.PossibleOptions.Add(itemOption);

        return definition;
    }

    private void CreateItemOptionTypes()
    {
        var optionTypes = new[] { ItemOptionTypes.Option, ItemOptionTypes.Luck };

        foreach (var optionType in optionTypes)
        {
            var persistentOptionType = this.Context.CreateNew<ItemOptionType>();
            persistentOptionType.Description = optionType.Description;
            persistentOptionType.Id = optionType.Id;
            persistentOptionType.Name = optionType.Name;
            persistentOptionType.IsVisible = optionType.IsVisible;
            this.GameConfiguration.ItemOptionTypes.Add(persistentOptionType);
        }
    }

    /// <summary>
    /// Creates the stat attributes.
    /// </summary>
    private void CreateStatAttributes()
    {
        var attributes = typeof(Stats)
            .GetProperties(BindingFlags.Public | BindingFlags.Static | BindingFlags.DeclaredOnly)
            .Where(p => p.PropertyType == typeof(AttributeDefinition))
            .Select(p => p.GetValue(typeof(Stats)))
            .OfType<AttributeDefinition>()
            .ToList();

        foreach (var attribute in attributes)
        {
            var persistentAttribute = this.Context.CreateNew<AttributeDefinition>(attribute.Id, attribute.Designation, attribute.Description);
            this.GameConfiguration.Attributes.Add(persistentAttribute);
        }
    }

    private void CreateItemSlotTypes()
    {
        var leftHand = this.Context.CreateNew<ItemSlotType>();
        leftHand.Description = "Left Hand";
        leftHand.ItemSlots.Add(0);
        this.GameConfiguration.ItemSlotTypes.Add(leftHand);

        var rightHand = this.Context.CreateNew<ItemSlotType>();
        rightHand.Description = "Right Hand";
        rightHand.ItemSlots.Add(1);
        this.GameConfiguration.ItemSlotTypes.Add(rightHand);

        var leftOrRightHand = this.Context.CreateNew<ItemSlotType>();
        leftOrRightHand.Description = "Left or Right Hand";
        leftOrRightHand.ItemSlots.Add(0);
        leftOrRightHand.ItemSlots.Add(1);
        this.GameConfiguration.ItemSlotTypes.Add(leftOrRightHand);

        var helm = this.Context.CreateNew<ItemSlotType>();
        helm.Description = "Helm";
        helm.ItemSlots.Add(2);
        this.GameConfiguration.ItemSlotTypes.Add(helm);

        var armor = this.Context.CreateNew<ItemSlotType>();
        armor.Description = "Armor";
        armor.ItemSlots.Add(3);
        this.GameConfiguration.ItemSlotTypes.Add(armor);

        var pants = this.Context.CreateNew<ItemSlotType>();
        pants.Description = "Pants";
        pants.ItemSlots.Add(4);
        this.GameConfiguration.ItemSlotTypes.Add(pants);

        var gloves = this.Context.CreateNew<ItemSlotType>();
        gloves.Description = "Gloves";
        gloves.ItemSlots.Add(5);
        this.GameConfiguration.ItemSlotTypes.Add(gloves);

        var boots = this.Context.CreateNew<ItemSlotType>();
        boots.Description = "Boots";
        boots.ItemSlots.Add(6);
        this.GameConfiguration.ItemSlotTypes.Add(boots);

        var wings = this.Context.CreateNew<ItemSlotType>();
        wings.Description = "Wings";
        wings.ItemSlots.Add(7);
        this.GameConfiguration.ItemSlotTypes.Add(wings);

        var pet = this.Context.CreateNew<ItemSlotType>();
        pet.Description = "Pet";
        pet.ItemSlots.Add(8);
        this.GameConfiguration.ItemSlotTypes.Add(pet);

        var pendant = this.Context.CreateNew<ItemSlotType>();
        pendant.Description = "Pendant";
        pendant.ItemSlots.Add(9);
        this.GameConfiguration.ItemSlotTypes.Add(pendant);

        var ring = this.Context.CreateNew<ItemSlotType>();
        ring.Description = "Ring";
        ring.ItemSlots.Add(10);
        ring.ItemSlots.Add(11);
        this.GameConfiguration.ItemSlotTypes.Add(ring);
    }

    private long CalcNeededMasterExp(long lvl)
    {
        // f(x) = 505 * x^3 + 35278500 * x + 228045 * x^2
        return (505 * lvl * lvl * lvl) + (35278500 * lvl) + (228045 * lvl * lvl);
    }

    private void AssignCharacterClassHomeMaps()
    {
        foreach (var characterClass in this.GameConfiguration.CharacterClasses)
        {
            var mapNumber = (CharacterClassNumber)characterClass.Number == CharacterClassNumber.FairyElf ? Noria.Number : Lorencia.Number;

            characterClass.HomeMap = this.GameConfiguration.Maps.First(map => map.Number == mapNumber);
        }
    }
}