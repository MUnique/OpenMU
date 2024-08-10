// <copyright file="GameConfigurationInitializerBase.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.Persistence.Initialization;

using System.Reflection;
using MUnique.OpenMU.AttributeSystem;
using MUnique.OpenMU.DataModel.Attributes;
using MUnique.OpenMU.DataModel.Configuration;
using MUnique.OpenMU.DataModel.Configuration.Items;
using MUnique.OpenMU.GameLogic.Attributes;
using MUnique.OpenMU.Persistence.Initialization.Version075.Items;
using MUnique.OpenMU.Persistence.Initialization.CharacterClasses;
using MUnique.OpenMU.Persistence.Initialization.VersionSeasonSix.Maps;

/// <summary>
/// Common base for an initialization of a <see cref="GameConfiguration"/>.
/// </summary>
public abstract class GameConfigurationInitializerBase : InitializerBase
{
    /// <summary>
    /// Initializes a new instance of the <see cref="GameConfigurationInitializerBase"/> class.
    /// </summary>
    /// <param name="context">The context.</param>
    /// <param name="gameConfiguration">The game configuration.</param>
    protected GameConfigurationInitializerBase(IContext context, GameConfiguration gameConfiguration)
        : base(context, gameConfiguration)
    {
    }

    /// <summary>
    /// Gets the option types which are available.
    /// </summary>
    protected abstract IEnumerable<ItemOptionType> OptionTypes { get; }

    /// <inheritdoc />
    public override void Initialize()
    {
        this.GameConfiguration.ExperienceRate = 1.0f;
        this.GameConfiguration.MinimumMonsterLevelForMasterExperience = 95;
        this.GameConfiguration.MaximumLevel = 400;
        this.GameConfiguration.MaximumMasterLevel = 200;
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
        this.GameConfiguration.ItemDropDuration = TimeSpan.FromSeconds(60);
        this.GameConfiguration.DamagePerOneItemDurability = 2000;
        this.GameConfiguration.DamagePerOnePetDurability = 100000;
        this.GameConfiguration.HitsPerOneItemDurability = 10000;

        this.GameConfiguration.ExperienceFormula = "if(level == 0, 0, if(level < 256, 10 * (level + 8) * (level - 1) * (level - 1), (10 * (level + 8) * (level - 1) * (level - 1)) + (1000 * (level - 247) * (level - 256) * (level - 256))))";
        this.GameConfiguration.MasterExperienceFormula = "(505 * level * level * level) + (35278500 * level) + (228045 * level * level)";

        this.AddItemDropGroups();

        this.CreateStatAttributes();

        new SlotTypesInitializer(this.Context, this.GameConfiguration).Initialize();
        this.CreateItemOptionTypes();
        this.GameConfiguration.ItemOptions.Add(this.CreateLuckOptionDefinition());
        this.GameConfiguration.ItemOptions.Add(this.CreateOptionDefinition(Stats.DefenseBase, ItemOptionDefinitionNumbers.DefenseOption));
        this.GameConfiguration.ItemOptions.Add(this.CreateOptionDefinition(Stats.MaximumPhysBaseDmg, ItemOptionDefinitionNumbers.PhysicalAttack));
        this.GameConfiguration.ItemOptions.Add(this.CreateOptionDefinition(Stats.MaximumWizBaseDmg, ItemOptionDefinitionNumbers.WizardryAttack));
    }

    protected ItemOptionDefinition CreateOptionDefinition(AttributeDefinition attributeDefinition, short number)
    {
        var definition = this.Context.CreateNew<ItemOptionDefinition>();
        definition.SetGuid(number);
        definition.Name = attributeDefinition.Designation + " Option";
        definition.AddChance = 0.25f;
        definition.AddsRandomly = true;
        definition.MaximumOptionsPerItem = 1;

        var itemOption = this.Context.CreateNew<IncreasableItemOption>();
        itemOption.SetGuid(number);
        itemOption.OptionType =
            this.GameConfiguration.ItemOptionTypes.FirstOrDefault(o => o == ItemOptionTypes.Option);
        itemOption.PowerUpDefinition = this.Context.CreateNew<PowerUpDefinition>();
        itemOption.PowerUpDefinition.TargetAttribute =
            this.GameConfiguration.Attributes.First(a => a == attributeDefinition);
        itemOption.PowerUpDefinition.Boost = this.Context.CreateNew<PowerUpDefinitionValue>();
        itemOption.PowerUpDefinition.Boost.ConstantValue!.Value = 4;
        for (short level = 2; level <= this.MaximumOptionLevel; level++)
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

    private void AddItemDropGroups()
    {
        var moneyDropItemGroup = this.Context.CreateNew<DropItemGroup>();
        moneyDropItemGroup.SetGuid(1);
        moneyDropItemGroup.Chance = 0.5;
        moneyDropItemGroup.ItemType = SpecialItemType.Money;
        moneyDropItemGroup.Description = "The common money drop item group (50 % drop chance)";
        this.GameConfiguration.DropItemGroups.Add(moneyDropItemGroup);
        BaseMapInitializer.RegisterDefaultDropItemGroup(moneyDropItemGroup);

        var randomItemDropItemGroup = this.Context.CreateNew<DropItemGroup>();
        randomItemDropItemGroup.SetGuid(2);
        randomItemDropItemGroup.Chance = 0.3;
        randomItemDropItemGroup.ItemType = SpecialItemType.RandomItem;
        randomItemDropItemGroup.Description = "The common drop item group for random items (30 % drop chance)";
        this.GameConfiguration.DropItemGroups.Add(randomItemDropItemGroup);
        BaseMapInitializer.RegisterDefaultDropItemGroup(randomItemDropItemGroup);

        if (this.OptionTypes.Contains(ItemOptionTypes.Excellent))
        {
            var excellentItemDropItemGroup = this.Context.CreateNew<DropItemGroup>();
            excellentItemDropItemGroup.SetGuid(3);
            excellentItemDropItemGroup.Chance = 0.0001;
            excellentItemDropItemGroup.ItemType = SpecialItemType.Excellent;
            excellentItemDropItemGroup.ItemLevel = 0;
            excellentItemDropItemGroup.Description =
                "The common drop item group for random excellent items (0.01 % drop chance)";
            this.GameConfiguration.DropItemGroups.Add(excellentItemDropItemGroup);
            BaseMapInitializer.RegisterDefaultDropItemGroup(excellentItemDropItemGroup);
        }
    }

    private ItemOptionDefinition CreateLuckOptionDefinition()
    {
        var definition = this.Context.CreateNew<ItemOptionDefinition>();
        definition.SetGuid(ItemOptionDefinitionNumbers.Luck);
        definition.Name = "Luck";
        definition.AddChance = 0.25f;
        definition.AddsRandomly = true;
        definition.MaximumOptionsPerItem = 1;

        var itemOption = this.Context.CreateNew<IncreasableItemOption>();
        itemOption.SetGuid(ItemOptionDefinitionNumbers.Luck);
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

    private void CreateItemOptionTypes()
    {
        foreach (var optionType in this.OptionTypes)
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

    private long CalcNeededMasterExp(long lvl)
    {
        // f(x) = 505 * x^3 + 35278500 * x + 228045 * x^2
        return (505 * lvl * lvl * lvl) + (35278500 * lvl) + (228045 * lvl * lvl);
    }

    /// <summary>
    /// Assigns the character class home maps.
    /// Needs to be called after the character classes and maps have been initialized.
    /// </summary>
    protected void AssignCharacterClassHomeMaps()
    {
        foreach (var characterClass in this.GameConfiguration.CharacterClasses)
        {
            byte mapNumber;
            switch ((CharacterClassNumber)characterClass.Number)
            {
                case CharacterClassNumber.FairyElf:
                case CharacterClassNumber.HighElf:
                case CharacterClassNumber.MuseElf:
                    mapNumber = Noria.Number;
                    break;
                case CharacterClassNumber.BloodySummoner:
                case CharacterClassNumber.Summoner:
                    mapNumber = Elvenland.Number;
                    break;
                default:
                    mapNumber = Lorencia.Number;
                    break;
            }

            characterClass.HomeMap = this.GameConfiguration.Maps.First(map => map.Number == mapNumber);
        }
    }
}