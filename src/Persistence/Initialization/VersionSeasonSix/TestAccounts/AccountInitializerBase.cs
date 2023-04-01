// <copyright file="AccountInitializerBase.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.Persistence.Initialization.VersionSeasonSix.TestAccounts;

using System;
using MUnique.OpenMU.AttributeSystem;
using MUnique.OpenMU.DataModel.Configuration;
using MUnique.OpenMU.DataModel.Configuration.Items;
using MUnique.OpenMU.DataModel.Entities;
using MUnique.OpenMU.GameLogic;
using MUnique.OpenMU.GameLogic.Attributes;
using MUnique.OpenMU.GameServer.MessageHandler.Quests;
using MUnique.OpenMU.Persistence.Initialization.CharacterClasses;
using MUnique.OpenMU.Persistence.Initialization.Items;
using MUnique.OpenMU.Persistence.Initialization.Skills;

/// <summary>
/// Abstract base class for a test account initializer.
/// </summary>
internal abstract class AccountInitializerBase : InitializerBase
{
    private static readonly HashSet<SkillNumber> EventSkills = new()
    {
        SkillNumber.Stun,
        SkillNumber.CancelStun,
        SkillNumber.SwellMana,
        SkillNumber.Invisibility,
        SkillNumber.CancelInvisibility,
        SkillNumber.SpellofProtection,
        SkillNumber.SpellofRestriction,
        SkillNumber.SpellofPursuit,
        SkillNumber.ShieldBurn,
    };

    /// <summary>
    /// Initializes a new instance of the <see cref="AccountInitializerBase"/> class.
    /// </summary>
    /// <param name="context">The context.</param>
    /// <param name="gameConfiguration">The game configuration.</param>
    /// <param name="accountName">Name of the account.</param>
    /// <param name="level">The level.</param>
    protected AccountInitializerBase(IContext context, GameConfiguration gameConfiguration, string accountName, int level)
        : base(context, gameConfiguration)
    {
        this.Level = level;
        this.AccountName = accountName;
        this.ItemHelper = new ItemHelper(this.Context, this.GameConfiguration);
    }

    /// <summary>
    /// Gets the level.
    /// </summary>
    /// <value>
    /// The level.
    /// </value>
    protected int Level { get; }

    /// <summary>
    /// Gets a value indicating whether to add all available skills automatically.
    /// </summary>
    protected bool AddAllSkills { get; set; }

    /// <summary>
    /// Gets the name of the account.
    /// </summary>
    /// <value>
    /// The name of the account.
    /// </value>
    protected string AccountName { get; }

    protected ItemHelper ItemHelper { get; }

    /// <inheritdoc />
    public sealed override void Initialize()
    {
        this.CreateAccount();
    }

    /// <summary>
    /// Creates the account.
    /// </summary>
    /// <returns>The created account.</returns>
    protected virtual Account CreateAccount()
    {
        var account = this.Context.CreateNew<Account>();
        account.LoginName = this.AccountName;
        account.PasswordHash = BCrypt.Net.BCrypt.HashPassword(this.AccountName);
        account.Vault = this.Context.CreateNew<ItemStorage>();
        this.AddVaultItems(account);

        if (this.CreateKnight() is { } knight)
        {
            account.Characters.Add(knight);
        }

        if (this.CreateElf() is { } elf)
        {
            account.Characters.Add(elf);
        }

        if (this.CreateWizard() is { } wizard)
        {
            account.Characters.Add(wizard);
        }

        if (this.CreateDarkLord() is { } darkLord)
        {
            account.Characters.Add(darkLord);
        }

        if (this.CreateMagicGladiator() is { } magicGladiator)
        {
            account.Characters.Add(magicGladiator);
        }

        return account;
    }

    /// <summary>
    /// Creates the dark lord.
    /// </summary>
    /// <returns>The dark lord, or null.</returns>
    protected virtual Character? CreateDarkLord() => null;

    /// <summary>
    /// Creates the magic gladiator.
    /// </summary>
    /// <returns>The magic gladiator, or null.</returns>
    protected virtual Character? CreateMagicGladiator() => null;

    /// <summary>
    /// Creates the knight.
    /// </summary>
    /// <returns>The dark knight, or null.</returns>
    protected virtual Character? CreateKnight() => null;

    /// <summary>
    /// Creates the elf.
    /// </summary>
    /// <returns>The elf, or null.</returns>
    protected virtual Character? CreateElf() => null;

    /// <summary>
    /// Creates the wizard.
    /// </summary>
    /// <returns>The wizard, or null.</returns>
    protected virtual Character? CreateWizard() => null;

    /// <summary>
    /// Creates the knight.
    /// </summary>
    /// <param name="characterClassNumber">The character class number.</param>
    /// <returns>The created knight.</returns>
    protected Character CreateKnight(CharacterClassNumber characterClassNumber)
    {
        return this.CreateCharacter(this.AccountName + "Dk", characterClassNumber, this.Level, 0);
    }

    /// <summary>
    /// Creates the wizard.
    /// </summary>
    /// <param name="characterClassNumber">The character class number.</param>
    /// <returns>The created wizard.</returns>
    protected Character CreateWizard(CharacterClassNumber characterClassNumber)
    {
        return this.CreateCharacter(this.AccountName + "Dw", characterClassNumber, this.Level, 1);
    }

    /// <summary>
    /// Creates the elf.
    /// </summary>
    /// <param name="characterClassNumber">The character class number.</param>
    /// <returns>The created elf.</returns>
    protected Character CreateElf(CharacterClassNumber characterClassNumber)
    {
        return this.CreateCharacter(this.AccountName + "Elf", characterClassNumber, this.Level, 2);
    }

    /// <summary>
    /// Creates the dark lord.
    /// </summary>
    /// <param name="characterClassNumber">The character class number.</param>
    /// <returns>The created dark lord.</returns>
    protected Character CreateDarkLord(CharacterClassNumber characterClassNumber)
    {
        return this.CreateCharacter(this.AccountName + "Dl", characterClassNumber, this.Level, 3);
    }

    /// <summary>
    /// Creates the magic gladiator.
    /// </summary>
    /// <param name="characterClassNumber">The character class number.</param>
    /// <returns>The created magic gladiator.</returns>
    protected Character CreateMagicGladiator(CharacterClassNumber characterClassNumber)
    {
        return this.CreateCharacter(this.AccountName + "Mg", characterClassNumber, this.Level, 4);
    }

    /// <summary>
    /// Creates the character.
    /// </summary>
    /// <param name="name">The name.</param>
    /// <param name="characterClass">The character class.</param>
    /// <param name="level">The level.</param>
    /// <param name="slot">The slot.</param>
    /// <returns>The created character.</returns>
    protected virtual Character CreateCharacter(string name, CharacterClassNumber characterClass, int level, byte slot)
    {
        var character = this.Context.CreateNew<Character>();
        character.CharacterClass = this.GameConfiguration.CharacterClasses.First(c => c.Number == (byte)characterClass);
        character.Name = name;
        character.CharacterSlot = slot;
        character.CreateDate = DateTime.UtcNow;
        character.KeyConfiguration = new byte[30];
        foreach (
            var attribute in
            character.CharacterClass.StatAttributes.Select(
                a => this.Context.CreateNew<StatAttribute>(a.Attribute, a.BaseValue)))
        {
            character.Attributes.Add(attribute);
        }

        character.CurrentMap = character.CharacterClass.HomeMap;
        var spawnGate = character.CurrentMap!.ExitGates.Where(m => m.IsSpawnGate).SelectRandom();
        if (spawnGate is not null)
        {
            character.PositionX = (byte)Rand.NextInt(spawnGate.X1, spawnGate.X2);
            character.PositionY = (byte)Rand.NextInt(spawnGate.Y1, spawnGate.Y2);
        }
        else
        {
            throw new InvalidOperationException($"{character.CurrentMap.Name} has no spawn gate.");
        }

        character.Attributes.First(a => a.Definition == Stats.Level).Value = level;
        character.Experience = GameConfigurationInitializerBase.CalculateNeededExperience(level);
        character.LevelUpPoints = (int)((character.Attributes.First(a => a.Definition == Stats.Level).Value - 1)
                                        * character.CharacterClass.StatAttributes.First(a => a.Attribute == Stats.PointsPerLevelUp).BaseValue);
        character.Inventory = this.Context.CreateNew<ItemStorage>();
        character.Inventory.Money = 10000000;

        if (level > 220)
        {
            // Some skills require a completed level 220 quest, so we add it here.
            var marlonNpc = this.GameConfiguration.Monsters.First(m => m.Designation == "Marlon");
            if (marlonNpc.Quests
                    .Where(q => q.QualifiedCharacter == character.CharacterClass
                                || q.QualifiedCharacter!.NextGenerationClass == character.CharacterClass)
                    .OrderByDescending(q => q.Number).FirstOrDefault() is { } marlonQuest)
            {
                var questInfo220 = this.Context.CreateNew<CharacterQuestState>();
                questInfo220.Group = QuestConstants.LegacyQuestGroup;
                questInfo220.LastFinishedQuest = marlonQuest;
                character.QuestStates.Add(questInfo220);
            }

            var comboQuest = marlonNpc.Quests
                .Where(q => q.QualifiedCharacter == character.CharacterClass
                            || q.QualifiedCharacter!.NextGenerationClass == character.CharacterClass)
                .FirstOrDefault(q => q.Rewards.Any(r => r.AttributeReward == Stats.IsSkillComboAvailable));
            if (comboQuest is { })
            {
                var attribute = this.Context.CreateNew<StatAttribute>(Stats.IsSkillComboAvailable.GetPersistent(this.GameConfiguration), 1);
                character.Attributes.Add(attribute);
            }
        }

        if (this.AddAllSkills)
        {
            var weaponSkills = this.GameConfiguration.Items.Where(i => i.Skill is not null && i.ItemSlot is not null).Select(i => i.Skill!).Distinct().ToHashSet();
            var availableSkills = this.GameConfiguration.Skills.Where(s => s.QualifiedCharacters.Contains(character.CharacterClass)
                                                                           && s.Number != (short)SkillNumber.NovaStart
                                                                           && s.Number < 300 // no master skills
                                                                           && !EventSkills.Contains((SkillNumber)s.Number)
                                                                           && !weaponSkills.Contains(s)).ToList();
            foreach (var availableSkill in availableSkills)
            {
                var skillEntry = this.Context.CreateNew<SkillEntry>();
                skillEntry.Skill = availableSkill;
                character.LearnedSkills.Add(skillEntry);
            }
        }

        return character;
    }

    /// <summary>
    /// Creates an armor item.
    /// </summary>
    /// <param name="itemSlot">The item slot.</param>
    /// <param name="setNumber">The set number.</param>
    /// <param name="group">The group.</param>
    /// <param name="targetExcellentOption">The target excellent option.</param>
    /// <param name="level">The level.</param>
    /// <param name="optionLevel">The option level.</param>
    /// <param name="luck">If set to <c>true</c>, the item should have luck.</param>
    /// <returns>The created item.</returns>
    protected Item CreateArmorItem(byte itemSlot, byte setNumber, byte group, AttributeDefinition? targetExcellentOption = null, byte level = 0, byte optionLevel = 0, bool luck = false)
    {
        var item = this.Context.CreateNew<Item>();
        item.Definition = this.GameConfiguration.Items.First(def => def.Group == group && def.Number == setNumber);
        item.Level = level;
        item.Durability = item.Definition.Durability;
        item.ItemSlot = itemSlot;
        if (targetExcellentOption != null)
        {
            var optionLink = this.Context.CreateNew<ItemOptionLink>();
            optionLink.ItemOption = item.Definition.PossibleItemOptions.SelectMany(o => o.PossibleOptions)
                .Where(o => o.OptionType == ItemOptionTypes.Excellent)
                .First(o => o.PowerUpDefinition!.TargetAttribute == targetExcellentOption);
            item.ItemOptions.Add(optionLink);
        }

        if (optionLevel > 0)
        {
            var optionLink = this.Context.CreateNew<ItemOptionLink>();
            optionLink.ItemOption = item.Definition.PossibleItemOptions.SelectMany(o => o.PossibleOptions)
                .First(o => o.OptionType == ItemOptionTypes.Option);
            optionLink.Level = optionLevel;
            item.ItemOptions.Add(optionLink);
        }

        if (luck)
        {
            var optionLink = this.Context.CreateNew<ItemOptionLink>();
            optionLink.ItemOption = item.Definition.PossibleItemOptions.SelectMany(o => o.PossibleOptions)
                .First(o => o.OptionType == ItemOptionTypes.Luck);
            item.ItemOptions.Add(optionLink);
        }

        return item;
    }

    /// <summary>
    /// Adds the test jewels and potions.
    /// </summary>
    /// <param name="inventory">The inventory.</param>
    protected void AddTestJewelsAndPotions(ItemStorage inventory)
    {
        inventory.Items.Add(this.CreateJewelOfBless(12));
        inventory.Items.Add(this.CreateJewelOfBless(13));
        inventory.Items.Add(this.CreateJewelOfBless(14));
        inventory.Items.Add(this.CreateJewelOfBless(15));
        inventory.Items.Add(this.CreateJewelOfBless(16));
        inventory.Items.Add(this.CreateJewelOfBless(17));
        inventory.Items.Add(this.CreateJewelOfBless(18));
        inventory.Items.Add(this.CreateJewelOfBless(19));
        inventory.Items.Add(this.CreateJewelOfSoul(20));
        inventory.Items.Add(this.CreateJewelOfSoul(21));
        inventory.Items.Add(this.CreateJewelOfSoul(22));
        inventory.Items.Add(this.CreateJewelOfSoul(23));
        inventory.Items.Add(this.CreateJewelOfSoul(24));
        inventory.Items.Add(this.CreateJewelOfSoul(25));
        inventory.Items.Add(this.CreateJewelOfSoul(26));
        inventory.Items.Add(this.CreateJewelOfSoul(27));
        inventory.Items.Add(this.CreateJewelOfLife(28));
        inventory.Items.Add(this.CreateJewelOfLife(29));
        inventory.Items.Add(this.CreateJewelOfLife(30));
        inventory.Items.Add(this.CreateJewelOfLife(31));
        inventory.Items.Add(this.CreateJewelOfLife(32));
        inventory.Items.Add(this.CreateJewelOfLife(33));
        inventory.Items.Add(this.CreateJewelOfLife(34));
        inventory.Items.Add(this.CreateJewelOfLife(35));
        inventory.Items.Add(this.CreateHealthPotion(36, 0));
        inventory.Items.Add(this.CreateHealthPotion(37, 1));
        inventory.Items.Add(this.CreateHealthPotion(38, 2));
        inventory.Items.Add(this.CreateHealthPotion(39, 3));
        inventory.Items.Add(this.CreateManaPotion(40, 0));
        inventory.Items.Add(this.CreateManaPotion(41, 1));
        inventory.Items.Add(this.CreateManaPotion(42, 2));
        inventory.Items.Add(this.CreateAlcohol(43));
        inventory.Items.Add(this.CreateShieldPotion(44, 0));
        inventory.Items.Add(this.CreateShieldPotion(45, 1));
        inventory.Items.Add(this.CreateShieldPotion(46, 2));
    }

    /// <summary>
    /// Adds the elf items.
    /// </summary>
    /// <param name="inventory">The inventory.</param>
    protected void AddElfItems(ItemStorage inventory)
    {
        inventory.Items.Add(this.CreateJewelOfBless(12));
        inventory.Items.Add(this.CreateJewelOfBless(13));
        inventory.Items.Add(this.CreateJewelOfBless(14));
        inventory.Items.Add(this.CreateJewelOfBless(15));
        inventory.Items.Add(this.CreateJewelOfBless(16));
        inventory.Items.Add(this.CreateJewelOfBless(17));
        inventory.Items.Add(this.CreateJewelOfBless(18));
        inventory.Items.Add(this.CreateJewelOfBless(19));
        inventory.Items.Add(this.CreateJewelOfSoul(20));
        inventory.Items.Add(this.CreateJewelOfSoul(21));
        inventory.Items.Add(this.CreateJewelOfSoul(22));
        inventory.Items.Add(this.CreateJewelOfSoul(23));
        inventory.Items.Add(this.CreateJewelOfSoul(24));
        inventory.Items.Add(this.CreateJewelOfSoul(25));
        inventory.Items.Add(this.CreateJewelOfSoul(26));
        inventory.Items.Add(this.CreateJewelOfSoul(27));
        inventory.Items.Add(this.CreateJewelOfLife(28));
        inventory.Items.Add(this.CreateJewelOfLife(29));
        inventory.Items.Add(this.CreateJewelOfLife(30));
        inventory.Items.Add(this.CreateJewelOfLife(31));
        inventory.Items.Add(this.CreateJewelOfCreation(32));
        inventory.Items.Add(this.CreateJewelOfCreation(33));
        inventory.Items.Add(this.CreateJewelOfCreation(34));
        inventory.Items.Add(this.CreateJewelOfCreation(35));
        inventory.Items.Add(this.CreateJewelOfChaos(36));
        inventory.Items.Add(this.CreateJewelOfChaos(37));
        inventory.Items.Add(this.CreateJewelOfChaos(38));
        inventory.Items.Add(this.CreateJewelOfChaos(39));
        inventory.Items.Add(this.CreateJewelOfChaos(40));
        inventory.Items.Add(this.CreateJewelOfChaos(41));
        inventory.Items.Add(this.CreateJewelOfChaos(42));
        inventory.Items.Add(this.CreateJewelOfChaos(43));
        inventory.Items.Add(this.CreateJewelOfChaos(44));
    }

    /// <summary>
    /// Adds the dark knight items.
    /// </summary>
    /// <param name="inventory">The inventory.</param>
    protected void AddDarkKnightItems(ItemStorage inventory)
    {
        inventory.Items.Add(this.CreateOrb(47, 12));
        inventory.Items.Add(this.CreateOrb(48, 14));
        inventory.Items.Add(this.CreateOrb(49, 19));
        inventory.Items.Add(this.CreateOrb(50, 44));
        inventory.Items.Add(this.CreateOrb(56, 7));
        inventory.Items.Add(this.CreateFullOptionJewellery(52, 20)); // Wizards Ring
        inventory.Items.Add(this.CreateFullOptionJewellery(53, 8)); // Ring of Ice
        inventory.Items.Add(this.CreateFullOptionJewellery(54, 9)); // Ring of Poison
        inventory.Items.Add(this.CreateFullOptionJewellery(55, 12)); // Pendant of Lightning
    }

    /// <summary>
    /// Adds the dark lord items.
    /// </summary>
    /// <param name="inventory">The inventory.</param>
    protected void AddDarkLordItems(ItemStorage inventory)
    {
        inventory.Items.Add(this.CreateOrb(47, 21));
        inventory.Items.Add(this.CreateOrb(48, 22));
        inventory.Items.Add(this.CreateOrb(49, 23));
        inventory.Items.Add(this.CreateOrb(50, 24));
        inventory.Items.Add(this.CreateOrb(60, 35));
        inventory.Items.Add(this.CreateOrb(61, 48));
        inventory.Items.Add(this.CreateFullOptionJewellery(62, 8)); // Ring of Ice
        inventory.Items.Add(this.CreateFullOptionJewellery(63, 9)); // Ring of Poison
        inventory.Items.Add(this.CreateFullOptionJewellery(64, 12)); // Pendant of Lightning
        inventory.Items.Add(this.CreatePet(53, 5)); // Raven
    }

    /// <summary>
    /// Adds the pets.
    /// </summary>
    /// <param name="inventory">The inventory.</param>
    protected void AddPets(ItemStorage inventory)
    {
        inventory.Items.Add(this.CreatePet(62, 0)); // Guardian Angel
        inventory.Items.Add(this.CreatePet(60, 1)); // Imp
        inventory.Items.Add(this.CreatePet(70, 2)); // Uniria
        inventory.Items.Add(this.CreatePet(59, 3)); // Dinorant
    }

    /// <summary>
    /// Creates jewellery with full options.
    /// </summary>
    /// <param name="itemSlot">The item slot.</param>
    /// <param name="number">The number.</param>
    /// <returns>The created jewellery.</returns>
    protected Item CreateFullOptionJewellery(byte itemSlot, int number)
    {
        var item = this.Context.CreateNew<Item>();
        item.Definition = this.GameConfiguration.Items.First(def => def.Group == 13 && def.Number == number);
        item.Durability = item.Definition.Durability;
        item.ItemSlot = itemSlot;
        foreach (var possibleOption in item.Definition.PossibleItemOptions.SelectMany(o => o.PossibleOptions))
        {
            var optionLink = this.Context.CreateNew<ItemOptionLink>();
            optionLink.ItemOption = possibleOption;
            item.ItemOptions.Add(optionLink);
        }

        return item;
    }

    /// <summary>
    /// Creates the wings with random options.
    /// </summary>
    /// <param name="itemSlot">The item slot.</param>
    /// <param name="number">The number.</param>
    /// <param name="level">The level.</param>
    /// <param name="group">The group.</param>
    /// <returns>The created wings.</returns>
    protected Item CreateWings(byte itemSlot, byte number, byte level, byte group = 12)
    {
        var item = this.Context.CreateNew<Item>();
        item.Definition = this.GameConfiguration.Items.First(def => def.Group == group && def.Number == number);
        item.Durability = item.Definition.Durability;
        item.ItemSlot = itemSlot;
        item.Level = level;
        var option = item.Definition.PossibleItemOptions.First(o => o.PossibleOptions.Any(p => p.OptionType == ItemOptionTypes.Wing));
        var optionLink = this.Context.CreateNew<ItemOptionLink>();
        optionLink.ItemOption = option.PossibleOptions.SelectRandom();
        item.ItemOptions.Add(optionLink);
        return item;
    }

    /// <summary>
    /// Creates the weapon.
    /// </summary>
    /// <param name="itemSlot">The item slot.</param>
    /// <param name="group">The group.</param>
    /// <param name="number">The number.</param>
    /// <param name="level">The level.</param>
    /// <param name="optionLevel">The option level.</param>
    /// <param name="luck">If set to <c>true</c>, the item should have luck.</param>
    /// <param name="skill">If set to <c>true</c>, the item should have skill.</param>
    /// <param name="targetExcellentOption">The target excellent option.</param>
    /// <returns>The created weapon.</returns>
    protected Item CreateWeapon(byte itemSlot, byte group, byte number, byte level, byte optionLevel, bool luck, bool skill, AttributeDefinition? targetExcellentOption = null)
    {
        var weapon = this.Context.CreateNew<Item>();
        weapon.Definition = this.GameConfiguration.Items.First(def => def.Group == group && def.Number == number);
        weapon.Durability = weapon.Definition.Durability;
        weapon.ItemSlot = itemSlot;
        weapon.Level = level;
        weapon.HasSkill = skill;
        if (targetExcellentOption != null)
        {
            var optionLink = this.Context.CreateNew<ItemOptionLink>();
            optionLink.ItemOption = weapon.Definition.PossibleItemOptions.SelectMany(o => o.PossibleOptions)
                .Where(o => o.OptionType == ItemOptionTypes.Excellent)
                .First(o => o.PowerUpDefinition!.TargetAttribute == targetExcellentOption);
            weapon.ItemOptions.Add(optionLink);
        }

        if (optionLevel > 0)
        {
            var optionLink = this.Context.CreateNew<ItemOptionLink>();
            optionLink.ItemOption = weapon.Definition.PossibleItemOptions.SelectMany(o => o.PossibleOptions)
                .First(o => o.OptionType == ItemOptionTypes.Option);
            optionLink.Level = optionLevel;
            weapon.ItemOptions.Add(optionLink);
        }

        if (luck)
        {
            var optionLink = this.Context.CreateNew<ItemOptionLink>();
            optionLink.ItemOption = weapon.Definition.PossibleItemOptions.SelectMany(o => o.PossibleOptions)
                .First(o => o.OptionType == ItemOptionTypes.Luck);
            weapon.ItemOptions.Add(optionLink);
        }

        return weapon;
    }

    /// <summary>
    /// Creates the orb.
    /// </summary>
    /// <param name="itemSlot">The item slot.</param>
    /// <param name="itemNumber">The item number.</param>
    /// <returns>The created orb.</returns>
    protected Item CreateOrb(byte itemSlot, byte itemNumber)
    {
        return this.ItemHelper.CreateOrb(itemSlot, itemNumber);
    }

    /// <summary>
    /// Creates the jewel.
    /// </summary>
    /// <param name="itemSlot">The item slot.</param>
    /// <param name="itemNumber">The item number.</param>
    /// <returns>The created jewel.</returns>
    protected Item CreateJewel(byte itemSlot, byte itemNumber)
    {
        var jewel = this.Context.CreateNew<Item>();
        jewel.Definition = this.GameConfiguration.Items.FirstOrDefault(def => def.Group == 14 && def.Number == itemNumber);
        jewel.Durability = 1;
        jewel.ItemSlot = itemSlot;
        return jewel;
    }

    /// <summary>
    /// Creates the fenrir.
    /// </summary>
    /// <param name="itemSlot">The item slot.</param>
    /// <param name="color">The color.</param>
    /// <returns>The created fenrir.</returns>
    protected Item CreateFenrir(byte itemSlot, ItemOptionType? color = null)
    {
        var fenrir = this.CreatePet(itemSlot, 37);

        if (color is null)
        {
            return fenrir;
        }

        var options = fenrir.Definition!.PossibleItemOptions.First().PossibleOptions.Where(p => p.OptionType == color);
        foreach (var option in options)
        {
            var optionLink = this.Context.CreateNew<ItemOptionLink>();
            optionLink.ItemOption = option;
            fenrir.ItemOptions.Add(optionLink);
        }

        return fenrir;
    }

    /// <summary>
    /// Creates the pet.
    /// </summary>
    /// <param name="itemSlot">The item slot.</param>
    /// <param name="itemNumber">The item number.</param>
    /// <returns>The created pet.</returns>
    protected Item CreatePet(byte itemSlot, byte itemNumber)
    {
        var pet = this.Context.CreateNew<Item>();
        pet.Definition = this.GameConfiguration.Items.First(def => def.Group == 13 && def.Number == itemNumber);
        pet.Durability = 255;
        pet.ItemSlot = itemSlot;
        if (pet.Definition?.Skill != null)
        {
            pet.HasSkill = true;
        }

        return pet;
    }

    /// <summary>
    /// Creates the arrows.
    /// </summary>
    /// <param name="itemSlot">The item slot.</param>
    /// <returns>The created arrows.</returns>
    protected Item CreateArrows(byte itemSlot)
    {
        var arrows = this.Context.CreateNew<Item>();
        arrows.Definition = this.GameConfiguration.Items.FirstOrDefault(def => def.Group == 4 && def.Number == 15); // short bow
        arrows.Durability = 255;
        arrows.ItemSlot = itemSlot;
        return arrows;
    }

    private Item CreateAlcohol(byte itemSlot)
    {
        var potion = this.Context.CreateNew<Item>();
        potion.Definition = this.GameConfiguration.Items.FirstOrDefault(def => def.Group == 14 && def.Number == 9);
        potion.Durability = 1;
        potion.ItemSlot = itemSlot;
        return potion;
    }

    private Item CreateManaPotion(byte itemSlot, byte size)
    {
        return this.CreatePotion(itemSlot, (byte)(size + 4));
    }

    private Item CreateHealthPotion(byte itemSlot, byte size)
    {
        return this.CreatePotion(itemSlot, size);
    }

    private Item CreateShieldPotion(byte itemSlot, byte size)
    {
        return this.CreatePotion(itemSlot, (byte)(size + 35));
    }

    private Item CreatePotion(byte itemSlot, byte itemNumber)
    {
        return this.ItemHelper.CreatePotion(itemSlot, itemNumber, 3, 0);
    }

    private Item CreateJewelOfBless(byte itemSlot)
    {
        return this.CreateJewel(itemSlot, 13);
    }

    private Item CreateJewelOfSoul(byte itemSlot)
    {
        return this.CreateJewel(itemSlot, 14);
    }

    private Item CreateJewelOfLife(byte itemSlot)
    {
        return this.CreateJewel(itemSlot, 16);
    }

    private Item CreateJewelOfCreation(byte itemSlot)
    {
        return this.CreateJewel(itemSlot, 22);
    }

    private Item CreateJewelOfChaos(byte itemSlot)
    {
        var jewel = this.Context.CreateNew<Item>();
        jewel.Definition = this.GameConfiguration.Items.FirstOrDefault(def => def.Group == 12 && def.Number == 15);
        jewel.Durability = 1;
        jewel.ItemSlot = itemSlot;
        return jewel;
    }

    private Item CreateBloodCastleTicket(byte itemSlot, byte level)
    {
        var ticket = this.Context.CreateNew<Item>();
        ticket.Definition = this.GameConfiguration.Items.FirstOrDefault(def => def.Group == 13 && def.Number == 18);
        ticket.Durability = 1;
        ticket.ItemSlot = itemSlot;
        ticket.Level = level;
        return ticket;
    }

    private Item CreateDevilSquareTicket(byte itemSlot, byte level)
    {
        var ticket = this.Context.CreateNew<Item>();
        ticket.Definition = this.GameConfiguration.Items.FirstOrDefault(def => def.Group == 14 && def.Number == 19);
        ticket.Durability = 1;
        ticket.ItemSlot = itemSlot;
        ticket.Level = level;
        return ticket;
    }

    private void AddVaultItems(Account account)
    {
        var vault = account.Vault ??= this.Context.CreateNew<ItemStorage>();
        vault.Items.Add(this.CreateBloodCastleTicket(0, 8));
        vault.Items.Add(this.CreateBloodCastleTicket(2, 8));
        vault.Items.Add(this.CreateBloodCastleTicket(4, 7));
        vault.Items.Add(this.CreateBloodCastleTicket(6, 7));
        vault.Items.Add(this.CreateBloodCastleTicket(16, 6));
        vault.Items.Add(this.CreateBloodCastleTicket(18, 6));
        vault.Items.Add(this.CreateBloodCastleTicket(20, 5));
        vault.Items.Add(this.CreateBloodCastleTicket(22, 5));

        vault.Items.Add(this.CreateDevilSquareTicket(32, 7));
        vault.Items.Add(this.CreateDevilSquareTicket(33, 7));
        vault.Items.Add(this.CreateDevilSquareTicket(34, 6));
        vault.Items.Add(this.CreateDevilSquareTicket(35, 6));
        vault.Items.Add(this.CreateDevilSquareTicket(36, 5));
        vault.Items.Add(this.CreateDevilSquareTicket(37, 5));
        vault.Items.Add(this.CreateDevilSquareTicket(38, 4));
        vault.Items.Add(this.CreateDevilSquareTicket(39, 4));
    }

    protected Item CreateFullAncient(byte itemSlot, ItemGroups group, byte number, byte level, string ancientName)
    {
        var ancient = this.Context.CreateNew<Item>();
        ancient.Definition = this.GameConfiguration.Items.First(def => def.Group == (int)group && def.Number == number);
        ancient.Durability = ancient.Definition.Durability;
        ancient.ItemSlot = itemSlot;
        ancient.Level = level;
        ancient.HasSkill = ancient.Definition.Skill is { };
        var optionLink = this.Context.CreateNew<ItemOptionLink>();
        optionLink.ItemOption = ancient.Definition.PossibleItemOptions.SelectMany(o => o.PossibleOptions)
            .First(o => o.OptionType == ItemOptionTypes.Option);
        optionLink.Level = optionLink.ItemOption.LevelDependentOptions.Max(o => o.Level);
        ancient.ItemOptions.Add(optionLink);

        if (ancient.Definition.PossibleItemOptions.SelectMany(o => o.PossibleOptions)
                .FirstOrDefault(o => o.OptionType == ItemOptionTypes.Luck) is { } luckOption)
        {
            var luck = this.Context.CreateNew<ItemOptionLink>();
            luck.ItemOption = luckOption;
            ancient.ItemOptions.Add(luck);
        }

        var set = ancient.Definition.PossibleItemSetGroups.First(a => a.Name == ancientName);
        var itemOfSet = set.Items.First(i => i.ItemDefinition == ancient.Definition);
        if (itemOfSet.BonusOption is { })
        {
            var ancientBonus = this.Context.CreateNew<ItemOptionLink>();
            ancientBonus.ItemOption = itemOfSet.BonusOption;
            ancientBonus.Level = 2;
            ancient.ItemOptions.Add(ancientBonus);
        }

        ancient.ItemSetGroups.Add(itemOfSet);

        return ancient;
    }
}