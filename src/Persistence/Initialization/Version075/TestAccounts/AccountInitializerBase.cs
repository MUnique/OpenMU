// <copyright file="AccountInitializerBase.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.Persistence.Initialization.Version075.TestAccounts;

using MUnique.OpenMU.AttributeSystem;
using MUnique.OpenMU.DataModel.Configuration;
using MUnique.OpenMU.DataModel.Configuration.Items;
using MUnique.OpenMU.DataModel.Entities;
using MUnique.OpenMU.GameLogic;
using MUnique.OpenMU.GameLogic.Attributes;
using MUnique.OpenMU.Persistence.Initialization.CharacterClasses;
using MUnique.OpenMU.Persistence.Initialization.Items;

/// <summary>
/// Abstract base class for a test account initializer.
/// </summary>
internal abstract class AccountInitializerBase : InitializerBase
{
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
    }

    /// <summary>
    /// Gets the level.
    /// </summary>
    /// <value>
    /// The level.
    /// </value>
    protected int Level { get; }

    /// <summary>
    /// Gets the name of the account.
    /// </summary>
    /// <value>
    /// The name of the account.
    /// </value>
    protected string AccountName { get; }

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

        return account;
    }

    /// <summary>
    /// Creates the knight.
    /// </summary>
    /// <returns>The dark knight, or null.</returns>
    protected abstract Character? CreateKnight();

    /// <summary>
    /// Creates the elf.
    /// </summary>
    /// <returns>The elf, or null.</returns>
    protected abstract Character? CreateElf();

    /// <summary>
    /// Creates the wizard.
    /// </summary>
    /// <returns>The wizard, or null.</returns>
    protected abstract Character? CreateWizard();

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
        return character;
    }

    /// <summary>
    /// Creates an armor item.
    /// </summary>
    /// <param name="itemSlot">The item slot.</param>
    /// <param name="setNumber">The set number.</param>
    /// <param name="group">The group.</param>
    /// <param name="level">The level.</param>
    /// <param name="optionLevel">The option level.</param>
    /// <param name="luck">If set to <c>true</c>, the item should have luck.</param>
    /// <returns>The created item.</returns>
    protected Item CreateArmorItem(byte itemSlot, byte setNumber, byte group, byte level = 0, byte optionLevel = 0, bool luck = false)
    {
        var item = this.Context.CreateNew<Item>();
        item.Definition = this.GameConfiguration.Items.First(def => def.Group == group && def.Number == setNumber);
        item.Level = level;
        item.Durability = item.Definition.Durability;
        item.ItemSlot = itemSlot;

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
        inventory.Items.Add(this.CreateJewelOfSoul(16));
        inventory.Items.Add(this.CreateJewelOfSoul(17));
        inventory.Items.Add(this.CreateJewelOfSoul(18));
        inventory.Items.Add(this.CreateJewelOfSoul(19));
        inventory.Items.Add(this.CreateHealthPotion(24, 0));
        inventory.Items.Add(this.CreateHealthPotion(25, 1));
        inventory.Items.Add(this.CreateHealthPotion(26, 2));
        inventory.Items.Add(this.CreateHealthPotion(27, 3));
        inventory.Items.Add(this.CreateManaPotion(28, 0));
        inventory.Items.Add(this.CreateManaPotion(29, 1));
        inventory.Items.Add(this.CreateManaPotion(30, 2));
        inventory.Items.Add(this.CreateAlcohol(31));
    }

    protected void AddScrolls(ItemStorage inventory)
    {
        inventory.Items.Add(this.CreateScroll(32, 0)); // Scroll of Poison
        inventory.Items.Add(this.CreateScroll(33, 4)); // Scroll of Flame
        inventory.Items.Add(this.CreateScroll(34, 5)); // Scroll of Teleport
        inventory.Items.Add(this.CreateScroll(35, 7)); // Scroll of Twister
        inventory.Items.Add(this.CreateScroll(36, 8)); // Scroll of Evil Spirit
        inventory.Items.Add(this.CreateScroll(37, 10)); // Scroll of Hellfire
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
        inventory.Items.Add(this.CreateJewelOfChaos(28));
        inventory.Items.Add(this.CreateJewelOfChaos(29));
        inventory.Items.Add(this.CreateJewelOfChaos(30));
        inventory.Items.Add(this.CreateJewelOfChaos(31));
        inventory.Items.Add(this.CreateJewelOfChaos(32));
        inventory.Items.Add(this.CreateJewelOfChaos(33));
        inventory.Items.Add(this.CreateJewelOfChaos(34));
        inventory.Items.Add(this.CreateJewelOfChaos(35));
    }

    /// <summary>
    /// Adds the dark knight items.
    /// </summary>
    /// <param name="inventory">The inventory.</param>
    protected void AddDarkKnightItems(ItemStorage inventory)
    {
        inventory.Items.Add(this.CreateFullOptionJewellery(53, 8)); // Ring of Ice
        inventory.Items.Add(this.CreateFullOptionJewellery(54, 9)); // Ring of Poison
        inventory.Items.Add(this.CreateFullOptionJewellery(55, 12)); // Pendant of Lightning
    }

    /// <summary>
    /// Adds the pets.
    /// </summary>
    /// <param name="inventory">The inventory.</param>
    protected void AddPets(ItemStorage inventory)
    {
        inventory.Items.Add(this.CreatePet(12, 0)); // Guardian Angel
        inventory.Items.Add(this.CreatePet(62, 1)); // Imp
        inventory.Items.Add(this.CreatePet(70, 2)); // Uniria
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
    /// <returns>The created weapon.</returns>
    protected Item CreateWeapon(byte itemSlot, byte group, byte number, byte level, byte optionLevel, bool luck, bool skill)
    {
        var weapon = this.Context.CreateNew<Item>();
        weapon.Definition = this.GameConfiguration.Items.First(def => def.Group == group && def.Number == number);
        weapon.Durability = weapon.Definition.Durability;
        weapon.ItemSlot = itemSlot;
        weapon.Level = level;
        weapon.HasSkill = skill;

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
        var orb = this.Context.CreateNew<Item>();
        orb.Definition = this.GameConfiguration.Items.First(def => def.Group == (byte)ItemGroups.Orbs && def.Number == itemNumber);
        orb.ItemSlot = itemSlot;
        orb.Durability = 1;
        return orb;
    }

    /// <summary>
    /// Creates the scroll.
    /// </summary>
    /// <param name="itemSlot">The item slot.</param>
    /// <param name="itemNumber">The item number.</param>
    /// <returns>The created scroll.</returns>
    protected Item CreateScroll(byte itemSlot, byte itemNumber)
    {
        var scroll = this.Context.CreateNew<Item>();
        scroll.Definition = this.GameConfiguration.Items.First(def => def.Group == (byte)ItemGroups.Scrolls && def.Number == itemNumber);
        scroll.ItemSlot = itemSlot;
        scroll.Durability = 1;
        return scroll;
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
        jewel.Definition = this.GameConfiguration.Items.First(def => def.Group == 14 && def.Number == itemNumber);
        jewel.Durability = 1;
        jewel.ItemSlot = itemSlot;
        return jewel;
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
        arrows.Definition = this.GameConfiguration.Items.First(def => def.Group == 4 && def.Number == 15);
        arrows.Durability = 255;
        arrows.ItemSlot = itemSlot;
        return arrows;
    }

    private Item CreateAlcohol(byte itemSlot)
    {
        var potion = this.Context.CreateNew<Item>();
        potion.Definition = this.GameConfiguration.Items.First(def => def.Group == 14 && def.Number == 9);
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

    private Item CreatePotion(byte itemSlot, byte itemNumber)
    {
        var potion = this.Context.CreateNew<Item>();
        potion.Definition = this.GameConfiguration.Items.First(def => def.Group == 14 && def.Number == itemNumber);
        potion.Durability = 3; // Stack of 3 Potions
        potion.ItemSlot = itemSlot;
        return potion;
    }

    private Item CreateJewelOfBless(byte itemSlot)
    {
        return this.CreateJewel(itemSlot, 13);
    }

    private Item CreateJewelOfSoul(byte itemSlot)
    {
        return this.CreateJewel(itemSlot, 14);
    }

    private Item CreateJewelOfChaos(byte itemSlot)
    {
        var jewel = this.Context.CreateNew<Item>();
        jewel.Definition = this.GameConfiguration.Items.First(def => def.Group == 12 && def.Number == 15);
        jewel.Durability = 1;
        jewel.ItemSlot = itemSlot;
        return jewel;
    }
}