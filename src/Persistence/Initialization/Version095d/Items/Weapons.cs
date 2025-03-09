// <copyright file="Weapons.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.Persistence.Initialization.Version095d.Items;

using MUnique.OpenMU.AttributeSystem;
using MUnique.OpenMU.DataModel.Configuration;
using MUnique.OpenMU.DataModel.Configuration.Items;
using MUnique.OpenMU.GameLogic.Attributes;
using MUnique.OpenMU.Persistence.Initialization.CharacterClasses;
using MUnique.OpenMU.Persistence.Initialization.Items;

/// <summary>
/// Helper class to create weapon item definitions.
/// </summary>
internal class Weapons : InitializerBase
{
    /// <summary>
    /// The durability increase per level.
    /// </summary>
    protected static readonly int[] DurabilityIncreasePerLevel = { 0, 1, 2, 3, 4, 6, 8, 10, 12, 14, 17, 21 };

    /// <summary>
    /// The weapon damage increase by level.
    /// </summary>
    private static readonly float[] DamageIncreaseByLevel = { 0, 3, 6, 9, 12, 15, 18, 21, 24, 27, 31, 36 };

    private static readonly float[] StaffRiseIncreaseByLevelEven = { 0, 3, 7, 10, 14, 17, 21, 24, 28, 31, 35, 40, 45, 50, 56, 63 }; // Staff with even magic power
    private static readonly float[] StaffRiseIncreaseByLevelOdd = { 0, 4, 7, 11, 14, 18, 21, 25, 28, 32, 36, 40, 45, 51, 57, 63 }; // Staff with odd magic power

    private ItemLevelBonusTable? _weaponDamageIncreaseTable;

    private ItemLevelBonusTable? _staffRiseTableEven;
    private ItemLevelBonusTable? _staffRiseTableOdd;

    /// <summary>
    /// Initializes a new instance of the <see cref="Weapons" /> class.
    /// </summary>
    /// <param name="context">The persistence context.</param>
    /// <param name="gameConfiguration">The game configration.</param>
    public Weapons(IContext context, GameConfiguration gameConfiguration)
        : base(context, gameConfiguration)
    {
    }

    /// <summary>
    /// Gets the luck item option definition.
    /// </summary>
    protected ItemOptionDefinition Luck
    {
        get
        {
            return this.GameConfiguration.ItemOptions.First(iod => iod.PossibleOptions.Any(o => o.OptionType == ItemOptionTypes.Luck));
        }
    }

    /// <summary>
    /// Gets the physical damage option definition.
    /// </summary>
    protected ItemOptionDefinition PhysicalDamageOption
    {
        get
        {
            return this.GameConfiguration.ItemOptions.First(iod => iod.PossibleOptions.Any(o => o.OptionType == ItemOptionTypes.Option && o.PowerUpDefinition?.TargetAttribute == Stats.PhysicalBaseDmg));
        }
    }

    /// <summary>
    /// Gets the wizardry damage option definition.
    /// </summary>
    protected ItemOptionDefinition WizardryDamageOption
    {
        get
        {
            return this.GameConfiguration.ItemOptions.First(iod => iod.PossibleOptions.Any(o => o.OptionType == ItemOptionTypes.Option && o.PowerUpDefinition?.TargetAttribute == Stats.WizardryBaseDmg));
        }
    }

    /// <summary>
    /// Initializes the weapons.
    /// </summary>
    public override void Initialize()
    {
        this._weaponDamageIncreaseTable = this.CreateItemBonusTable(DamageIncreaseByLevel, "Damage Increase (Weapons)", "The damage increase by weapon level. It increases by 3 per level, and 1 more after level 10.");
        this._staffRiseTableEven = this.CreateItemBonusTable(StaffRiseIncreaseByLevelEven, "Staff Rise (even)", "The staff rise bonus per item level for even magic power staves.");
        this._staffRiseTableOdd = this.CreateItemBonusTable(StaffRiseIncreaseByLevelOdd, "Staff Rise (odd)", "The staff rise bonus per item level for odd magic power staves.");

        this.CreateWeapon(0, 0, 0, 0, 1, 2, true, "Kris", 6, 6, 11, 50, 20, 0, 0, 40, 40, 0, 0, 1, 1, 1);
        this.CreateWeapon(0, 1, 0, 0, 1, 3, true, "Short Sword", 3, 3, 7, 20, 22, 0, 0, 60, 0, 0, 0, 1, 1, 1);
        this.CreateWeapon(0, 2, 0, 0, 1, 3, true, "Rapier", 9, 9, 15, 40, 23, 0, 0, 50, 40, 0, 0, 0, 1, 1);
        this.CreateWeapon(0, 3, 0, 20, 1, 3, true, "Katache", 16, 16, 26, 35, 27, 0, 0, 80, 40, 0, 0, 0, 1, 0);
        this.CreateWeapon(0, 4, 0, 21, 1, 3, true, "Sword of Assassin", 12, 12, 18, 30, 24, 0, 0, 60, 40, 0, 0, 0, 1, 0);
        this.CreateWeapon(0, 5, 0, 22, 1, 3, true, "Blade", 36, 36, 47, 30, 39, 0, 0, 80, 50, 0, 0, 1, 1, 1);
        this.CreateWeapon(0, 6, 0, 20, 1, 3, true, "Gladius", 20, 20, 30, 20, 30, 0, 0, 110, 0, 0, 0, 0, 1, 1);
        this.CreateWeapon(0, 7, 0, 21, 1, 3, true, "Falchion", 24, 24, 34, 25, 34, 0, 0, 120, 0, 0, 0, 0, 1, 0);
        this.CreateWeapon(0, 8, 0, 21, 1, 3, true, "Serpent Sword", 30, 30, 40, 20, 36, 0, 0, 130, 0, 0, 0, 0, 1, 0);
        this.CreateWeapon(0, 9, 0, 20, 2, 3, true, "Sword of Salamander", 32, 32, 46, 30, 40, 0, 0, 103, 0, 0, 0, 0, 1, 0);
        this.CreateWeapon(0, 10, 0, 22, 2, 4, true, "Light Saber", 40, 47, 61, 25, 50, 0, 0, 80, 60, 0, 0, 0, 1, 1);
        this.CreateWeapon(0, 11, 0, 20, 2, 3, true, "Legendary Sword", 44, 56, 72, 20, 54, 0, 0, 120, 0, 0, 0, 0, 1, 0);
        this.CreateWeapon(0, 12, 0, 19, 2, 3, true, "Heliacal Sword", 56, 73, 98, 25, 66, 0, 0, 140, 0, 0, 0, 0, 1, 0);
        this.CreateWeapon(0, 13, 0, 22, 1, 3, true, "Double Blade", 48, 48, 56, 30, 43, 0, 0, 70, 70, 0, 0, 0, 1, 1);
        this.CreateWeapon(0, 14, 0, 22, 1, 3, true, "Lighting Sword", 59, 59, 67, 30, 50, 0, 0, 90, 50, 0, 0, 0, 1, 1);
        this.CreateWeapon(0, 15, 0, 23, 2, 3, true, "Giant Sword", 52, 60, 85, 20, 60, 0, 0, 140, 0, 0, 0, 0, 1, 0);
        this.CreateWeapon(0, 16, 0, 22, 1, 4, true, "Sword of Destruction", 82, 82, 90, 35, 84, 0, 0, 160, 60, 0, 0, 0, 1, 0, 1);
        this.CreateWeapon(0, 17, 0, 23, 2, 4, true, "Dark Breaker", 104, 128, 153, 40, 89, 0, 0, 180, 50, 0, 0, 0, 2, 0);
        this.CreateWeapon(0, 18, 0, 23, 2, 3, true, "Thunder Blade", 105, 140, 168, 40, 86, 0, 0, 180, 50, 0, 0, 0, 0, 0, 1);

        this.CreateWeapon(1, 0, 0, 0, 1, 3, true, "Small Axe", 1, 1, 6, 20, 18, 0, 0, 50, 0, 0, 0, 1, 1, 1);
        this.CreateWeapon(1, 1, 0, 0, 1, 3, true, "Hand Axe", 4, 4, 9, 30, 20, 0, 0, 70, 0, 0, 0, 1, 1, 1);
        this.CreateWeapon(1, 2, 0, 19, 1, 3, true, "Double Axe", 14, 14, 24, 20, 26, 0, 0, 90, 0, 0, 0, 0, 1, 0);
        this.CreateWeapon(1, 3, 0, 19, 1, 3, true, "Tomahawk", 18, 18, 28, 30, 28, 0, 0, 100, 0, 0, 0, 0, 1, 0);
        this.CreateWeapon(1, 4, 0, 0, 1, 3, true, "Elven Axe", 26, 26, 38, 40, 32, 0, 0, 50, 70, 0, 0, 1, 0, 1);
        this.CreateWeapon(1, 5, 0, 19, 2, 3, true, "Battle Axe", 30, 36, 44, 20, 36, 0, 0, 120, 0, 0, 0, 0, 1, 1);
        this.CreateWeapon(1, 6, 0, 19, 2, 3, true, "Nikkea Axe", 34, 38, 50, 30, 44, 0, 0, 130, 0, 0, 0, 0, 1, 1);
        this.CreateWeapon(1, 7, 0, 19, 2, 3, true, "Larkan Axe", 46, 54, 67, 25, 55, 0, 0, 140, 0, 0, 0, 0, 1, 0);
        this.CreateWeapon(1, 8, 0, 19, 2, 3, true, "Crescent Axe", 54, 69, 89, 30, 65, 0, 0, 100, 40, 0, 0, 1, 1, 0);

        this.CreateWeapon(2, 0, 0, 0, 1, 3, true, "Mace", 7, 7, 13, 15, 21, 0, 0, 100, 0, 0, 0, 0, 1, 0);
        this.CreateWeapon(2, 1, 0, 19, 1, 3, true, "Morning Star", 13, 13, 22, 15, 25, 0, 0, 100, 0, 0, 0, 0, 1, 0);
        this.CreateWeapon(2, 2, 0, 0, 1, 3, true, "Flail", 22, 22, 32, 15, 32, 0, 0, 80, 50, 0, 0, 0, 1, 0);
        this.CreateWeapon(2, 3, 0, 19, 2, 3, true, "Great Hammer", 38, 45, 56, 15, 50, 0, 0, 150, 0, 0, 0, 0, 1, 0);
        this.CreateWeapon(2, 4, 0, 19, 2, 3, true, "Crystal Morning Star", 66, 78, 107, 30, 72, 0, 0, 130, 0, 0, 0, 1, 1, 1);
        this.CreateWeapon(2, 5, 0, 23, 2, 4, true, "Crystal Sword", 72, 89, 120, 40, 76, 0, 0, 130, 70, 0, 0, 1, 1, 1);
        this.CreateWeapon(2, 6, 0, 23, 2, 4, false, "Chaos Dragon Axe", 75, 102, 130, 35, 80, 0, 0, 140, 50, 0, 0, 0, 1, 0);

        this.CreateWeapon(3, 0, 0, 22, 2, 4, true, "Light Spear", 42, 50, 63, 25, 56, 0, 0, 60, 70, 0, 0, 0, 1, 1);
        this.CreateWeapon(3, 1, 0, 0, 2, 4, true, "Spear", 23, 30, 41, 30, 42, 0, 0, 70, 50, 0, 0, 0, 1, 1);
        this.CreateWeapon(3, 2, 0, 0, 2, 4, true, "Dragon Lance", 15, 21, 33, 30, 34, 0, 0, 70, 50, 0, 0, 0, 1, 1);
        this.CreateWeapon(3, 3, 0, 0, 2, 4, true, "Giant Trident", 29, 35, 43, 25, 44, 0, 0, 90, 30, 0, 0, 0, 1, 1);
        this.CreateWeapon(3, 4, 0, 20, 2, 4, true, "Serpent Spear", 46, 58, 80, 20, 58, 0, 0, 90, 30, 0, 0, 0, 1, 1);
        this.CreateWeapon(3, 5, 0, 0, 2, 4, true, "Double Poleaxe", 13, 19, 31, 30, 38, 0, 0, 70, 50, 0, 0, 0, 1, 1);
        this.CreateWeapon(3, 6, 0, 0, 2, 4, true, "Halberd", 19, 25, 35, 30, 40, 0, 0, 70, 50, 0, 0, 0, 1, 1);
        this.CreateWeapon(3, 7, 0, 22, 2, 4, true, "Berdysh", 37, 42, 54, 30, 54, 0, 0, 80, 50, 0, 0, 0, 1, 1);
        this.CreateWeapon(3, 8, 0, 22, 2, 4, true, "Great Scythe", 54, 71, 92, 25, 68, 0, 0, 90, 50, 0, 0, 0, 1, 1);
        this.CreateWeapon(3, 9, 0, 22, 2, 4, true, "Bill of Balrog", 63, 76, 102, 25, 74, 0, 0, 80, 50, 0, 0, 0, 1, 1);

        this.CreateWeapon(4, 0, 1, 24, 2, 3, true, "Short Bow", 2, 3, 5, 30, 20, 0, 0, 20, 80, 0, 0, 0, 0, 1);
        this.CreateWeapon(4, 1, 1, 24, 2, 3, true, "Bow", 8, 9, 13, 30, 24, 0, 0, 30, 90, 0, 0, 0, 0, 1);
        this.CreateWeapon(4, 2, 1, 24, 2, 3, true, "Elven Bow", 16, 17, 24, 30, 28, 0, 0, 30, 90, 0, 0, 0, 0, 1);
        this.CreateWeapon(4, 3, 1, 24, 2, 3, true, "Battle Bow", 26, 28, 37, 30, 36, 0, 0, 30, 90, 0, 0, 0, 0, 1);
        this.CreateWeapon(4, 4, 1, 24, 2, 4, true, "Tiger Bow", 40, 42, 52, 30, 43, 0, 0, 30, 100, 0, 0, 0, 0, 1);
        this.CreateWeapon(4, 5, 1, 24, 2, 4, true, "Silver Bow", 56, 59, 71, 40, 48, 0, 0, 30, 100, 0, 0, 0, 0, 1);
        this.CreateWeapon(4, 6, 1, 24, 2, 4, false, "Chaos Nature Bow", 75, 88, 106, 35, 68, 0, 0, 40, 150, 0, 0, 0, 0, 1);
        this.CreateAmmunition(4, 7, 1, 1, 2, false, "Bolt", 0, 255, 0, 0, 1);
        this.CreateWeapon(4, 8, 0, 24, 2, 2, true, "Crossbow", 4, 5, 8, 40, 22, 0, 0, 20, 90, 0, 0, 0, 0, 1);
        this.CreateWeapon(4, 9, 0, 24, 2, 2, true, "Golden Crossbow", 12, 13, 19, 40, 26, 0, 0, 30, 90, 0, 0, 0, 0, 1);
        this.CreateWeapon(4, 10, 0, 24, 2, 2, true, "Arquebus", 20, 22, 30, 40, 31, 0, 0, 30, 90, 0, 0, 0, 0, 1);
        this.CreateWeapon(4, 11, 0, 24, 2, 3, true, "Light Crossbow", 32, 35, 44, 40, 40, 0, 0, 30, 90, 0, 0, 0, 0, 1);
        this.CreateWeapon(4, 12, 0, 24, 2, 3, true, "Serpent Crossbow", 48, 50, 61, 40, 45, 0, 0, 30, 100, 0, 0, 0, 0, 1);
        this.CreateWeapon(4, 13, 0, 24, 2, 3, true, "Bluewing Crossbow", 68, 68, 82, 40, 56, 0, 0, 40, 110, 0, 0, 0, 0, 1);
        this.CreateWeapon(4, 14, 0, 24, 2, 3, true, "Aquagold Crossbow", 72, 78, 92, 30, 60, 0, 0, 50, 130, 0, 0, 0, 0, 1);
        this.CreateAmmunition(4, 15, 0, 1, 2, false, "Arrows", 0, 255, 0, 0, 1);
        this.CreateWeapon(4, 16, 0, 24, 2, 4, true, "Saint Crossbow", 84, 102, 127, 35, 72, 0, 0, 50, 160, 0, 0, 0, 0, 1);

        this.CreateWeapon(5, 0, 0, 0, 1, 3, true, "Skull Staff", 6, 3, 4, 20, 20, 6, 0, 40, 0, 0, 0, 1, 0, 0);
        this.CreateWeapon(5, 1, 0, 0, 2, 3, true, "Angelic Staff", 18, 10, 12, 25, 38, 20, 0, 50, 0, 0, 0, 1, 0, 0);
        this.CreateWeapon(5, 2, 0, 0, 2, 3, true, "Serpent Staff", 30, 17, 18, 25, 50, 34, 0, 50, 0, 0, 0, 1, 0, 0);
        this.CreateWeapon(5, 3, 0, 0, 2, 4, true, "Thunder Staff", 42, 23, 25, 25, 60, 46, 0, 40, 10, 0, 0, 1, 0, 0);
        this.CreateWeapon(5, 4, 0, 0, 2, 4, true, "Gorgon Staff", 52, 29, 32, 25, 65, 58, 0, 50, 0, 0, 0, 1, 0, 0);
        this.CreateWeapon(5, 5, 0, 0, 1, 4, true, "Legendary Staff", 59, 29, 31, 25, 66, 59, 0, 50, 0, 0, 0, 1, 0, 0);
        this.CreateWeapon(5, 6, 0, 0, 1, 4, true, "Staff of Resurrection", 70, 35, 39, 25, 70, 70, 0, 60, 10, 0, 0, 1, 0, 0);
        this.CreateWeapon(5, 7, 0, 0, 2, 4, false, "Chaos Lightning Staff", 75, 47, 48, 30, 70, 94, 0, 60, 10, 0, 0, 1, 0, 0);
        this.CreateWeapon(5, 8, 0, 0, 2, 4, true, "Staff of Destruction", 90, 50, 54, 30, 85, 101, 0, 60, 10, 0, 0, 1, 0, 0, 1);
    }

    /// <summary>
    /// Creates the ammunition.
    /// </summary>
    /// <param name="group">The group.</param>
    /// <param name="number">The number.</param>
    /// <param name="slot">The slot.</param>
    /// <param name="width">The width.</param>
    /// <param name="height">The height.</param>
    /// <param name="dropsFromMonsters">if set to <c>true</c>, the item drops from monsters.</param>
    /// <param name="name">The name.</param>
    /// <param name="dropLevel">The drop level.</param>
    /// <param name="durability">The durability.</param>
    /// <param name="wizardClass">The wizard class.</param>
    /// <param name="knightClass">The knight class.</param>
    /// <param name="elfClass">The elf class.</param>
    protected void CreateAmmunition(byte @group, byte number, byte slot, byte width, byte height, bool dropsFromMonsters, string name, byte dropLevel, byte durability, int wizardClass, int knightClass, int elfClass)
    {
        var item = this.Context.CreateNew<ItemDefinition>();
        this.GameConfiguration.Items.Add(item);
        item.Name = name;
        item.Group = group;
        item.Number = number;
        item.Height = height;
        item.Width = width;
        item.DropLevel = dropLevel;
        item.MaximumItemLevel = 0;
        item.DropsFromMonsters = dropsFromMonsters;
        item.SetGuid(item.Group, item.Number);
        if (slot == 0 && knightClass > 0 && width == 1)
        {
            item.ItemSlot = this.GameConfiguration.ItemSlotTypes.First(t => t.ItemSlots.Contains(0) && t.ItemSlots.Contains(1));
        }
        else
        {
            item.ItemSlot = this.GameConfiguration.ItemSlotTypes.First(t => t.ItemSlots.Contains(slot));
        }

        item.Durability = durability;
        var qualifiedCharacterClasses = this.GameConfiguration.DetermineCharacterClasses(wizardClass == 1, knightClass == 1, elfClass == 1);
        qualifiedCharacterClasses.ToList().ForEach(item.QualifiedCharacters.Add);

        item.IsAmmunition = true;
    }

    /// <summary>
    /// Creates the item with the specified parameters.
    /// </summary>
    /// <param name="group">The group number.</param>
    /// <param name="number">The item number inside the group.</param>
    /// <param name="slot">The slot.</param>
    /// <param name="skillNumber">The skill number.</param>
    /// <param name="width">The width.</param>
    /// <param name="height">The height.</param>
    /// <param name="dropsFromMonsters">if set to <c>true</c>, the item drops from monsters.</param>
    /// <param name="name">The name.</param>
    /// <param name="dropLevel">The drop level.</param>
    /// <param name="minimumDamage">The minimum damage.</param>
    /// <param name="maximumDamage">The maximum damage.</param>
    /// <param name="attackSpeed">The attack speed.</param>
    /// <param name="durability">The durability.</param>
    /// <param name="magicPower">The magic power.</param>
    /// <param name="levelRequirement">The level requirement.</param>
    /// <param name="strengthRequirement">The strength requirement.</param>
    /// <param name="agilityRequirement">The agility requirement.</param>
    /// <param name="energyRequirement">The energy requirement.</param>
    /// <param name="vitalityRequirement">The vitality requirement.</param>
    /// <param name="wizardClass">The wizard class.</param>
    /// <param name="knightClass">The knight class.</param>
    /// <param name="elfClass">The elf class.</param>
    /// <param name="magicGladiatorClass">The magic gladiator class.</param>
    /// <param name="isAmmunition">If set to <c>true</c>, the item is ammunition for a weapon.</param>
    protected void CreateWeapon(byte @group, byte number, byte slot, int skillNumber, byte width, byte height,
        bool dropsFromMonsters, string name, byte dropLevel, int minimumDamage, int maximumDamage, int attackSpeed,
        byte durability, int magicPower, int levelRequirement, int strengthRequirement, int agilityRequirement,
        int energyRequirement, int vitalityRequirement,
        int wizardClass, int knightClass, int elfClass, int magicGladiatorClass = 0, bool isAmmunition = false)
    {
        var item = this.Context.CreateNew<ItemDefinition>();
        this.GameConfiguration.Items.Add(item);
        item.Name = name;
        item.Group = group;
        item.Number = number;

        item.Height = height;
        item.Width = width;
        item.DropLevel = dropLevel;
        item.MaximumItemLevel = isAmmunition ? (byte)0 : Constants.MaximumItemLevel;
        item.DropsFromMonsters = dropsFromMonsters;
        item.SetGuid(item.Group, item.Number);
        if (slot == 0 && knightClass > 0 && width == 1)
        {
            item.ItemSlot = this.GameConfiguration.ItemSlotTypes.First(t => t.ItemSlots.Contains(0) && t.ItemSlots.Contains(1));
        }
        else
        {
            item.ItemSlot = this.GameConfiguration.ItemSlotTypes.First(t => t.ItemSlots.Contains(slot));
        }

        if (skillNumber > 0)
        {
            var itemSkill = this.GameConfiguration.Skills.First(s => s.Number == skillNumber);
            item.Skill = itemSkill;
        }

        item.Durability = durability;
        var classes = wizardClass == 1 ? CharacterClasses.DarkWizard | CharacterClasses.MagicGladiator : CharacterClasses.None;
        classes |= knightClass == 1 ? CharacterClasses.DarkKnight | CharacterClasses.MagicGladiator : CharacterClasses.None;
        classes |= elfClass == 1 ? CharacterClasses.FairyElf : CharacterClasses.None;
        classes |= magicGladiatorClass == 1 ? CharacterClasses.MagicGladiator : CharacterClasses.None;
        var qualifiedCharacterClasses = this.GameConfiguration.DetermineCharacterClasses(classes);
        qualifiedCharacterClasses.ToList().ForEach(item.QualifiedCharacters.Add);

        var minDamagePowerUp = this.CreateItemBasePowerUpDefinition(Stats.MinimumPhysBaseDmgByWeapon, minimumDamage, AggregateType.AddRaw);
        minDamagePowerUp.BonusPerLevelTable = this._weaponDamageIncreaseTable;
        item.BasePowerUpAttributes.Add(minDamagePowerUp);

        var maxDamagePowerUp = this.CreateItemBasePowerUpDefinition(Stats.MaximumPhysBaseDmgByWeapon, maximumDamage, AggregateType.AddRaw);
        maxDamagePowerUp.BonusPerLevelTable = this._weaponDamageIncreaseTable;
        item.BasePowerUpAttributes.Add(maxDamagePowerUp);

        var speedPowerUp = this.CreateItemBasePowerUpDefinition(Stats.AttackSpeedByWeapon, attackSpeed, AggregateType.AddRaw);
        item.BasePowerUpAttributes.Add(speedPowerUp);

        this.CreateItemRequirementIfNeeded(item, Stats.Level, levelRequirement);
        this.CreateItemRequirementIfNeeded(item, Stats.TotalStrengthRequirementValue, strengthRequirement);
        this.CreateItemRequirementIfNeeded(item, Stats.TotalAgilityRequirementValue, agilityRequirement);
        this.CreateItemRequirementIfNeeded(item, Stats.TotalEnergyRequirementValue, energyRequirement);
        this.CreateItemRequirementIfNeeded(item, Stats.TotalVitalityRequirementValue, vitalityRequirement);

        item.PossibleItemOptions.Add(this.Luck);

        if (magicPower == 0)
        {
            item.PossibleItemOptions.Add(this.PhysicalDamageOption);
            item.PossibleItemOptions.Add(this.GameConfiguration.ItemOptions.Single(o => o.Name == ExcellentOptions.PhysicalAttackOptionsName));
        }
        else
        {
            item.PossibleItemOptions.Add(this.WizardryDamageOption);
            item.PossibleItemOptions.Add(this.GameConfiguration.ItemOptions.Single(o => o.Name == ExcellentOptions.WizardryAttackOptionsName));

            var staffRisePowerUp = this.CreateItemBasePowerUpDefinition(Stats.StaffRise, magicPower / 2.0f, AggregateType.AddRaw);
            staffRisePowerUp.BonusPerLevelTable = magicPower % 2 == 0 ? this._staffRiseTableEven : this._staffRiseTableOdd;
            item.BasePowerUpAttributes.Add(staffRisePowerUp);
        }

        item.IsAmmunition = isAmmunition;
        if (!item.IsAmmunition)
        {
            var ammunitionConsumption = this.Context.CreateNew<ItemBasePowerUpDefinition>();
            ammunitionConsumption.TargetAttribute = Stats.AmmunitionConsumptionRate.GetPersistent(this.GameConfiguration);
            ammunitionConsumption.BaseValue = 1.0f;
            item.BasePowerUpAttributes.Add(ammunitionConsumption);
        }

        if (group != (int)ItemGroups.Bows && width == 2)
        {
            var isTwoHandedWeapon = this.Context.CreateNew<ItemBasePowerUpDefinition>();
            isTwoHandedWeapon.TargetAttribute = Stats.IsTwoHandedWeaponEquipped.GetPersistent(this.GameConfiguration);
            isTwoHandedWeapon.BaseValue = 1.0f;
            item.BasePowerUpAttributes.Add(isTwoHandedWeapon);
        }
    }
}