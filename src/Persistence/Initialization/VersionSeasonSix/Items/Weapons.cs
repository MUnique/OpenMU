// <copyright file="Weapons.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.Persistence.Initialization.VersionSeasonSix.Items;

using MUnique.OpenMU.AttributeSystem;
using MUnique.OpenMU.DataModel.Configuration;
using MUnique.OpenMU.DataModel.Configuration.Items;
using MUnique.OpenMU.GameLogic.Attributes;
using MUnique.OpenMU.Persistence.Initialization.CharacterClasses;
using MUnique.OpenMU.Persistence.Initialization.Items;
using MUnique.OpenMU.Persistence.Initialization.Skills;

/// <summary>
/// Helper class to create weapon item definitions.
/// </summary>
internal class Weapons : InitializerBase
{
    /// <summary>
    /// The maximum item level for weapons and armors.
    /// </summary>
    protected const int MaximumItemLevel = 15;

    /// <summary>
    /// The durability increase per level.
    /// </summary>
    protected static readonly float[] DurabilityIncreasePerLevel = { 0, 1, 2, 3, 4, 6, 8, 10, 12, 14, 17, 21, 26, 32, 39, 47 };

    /// <summary>
    /// The weapon damage increase by level.
    /// </summary>
    private static readonly float[] DamageIncreaseByLevel = { 0, 3, 6, 9, 12, 15, 18, 21, 24, 27, 31, 36, 42, 49, 57, 66 };

    private static readonly float[] StaffRiseIncreaseByLevelEven = { 0, 3, 7, 10, 14, 17, 21, 24, 28, 31, 35, 40, 45, 50, 56, 63 }; // Staff/stick with even magic power
    private static readonly float[] StaffRiseIncreaseByLevelOdd = { 0, 4, 7, 11, 14, 18, 21, 25, 28, 32, 36, 40, 45, 51, 57, 63 }; // Staff/stick with odd magic power

    private static readonly float[] ScepterRiseIncreaseByLevelEven = { 0, 1, 3, 4, 6, 7, 9, 10, 12, 13, 15, 18, 21, 24, 28, 33 }; // Scepter with even magic power
    private static readonly float[] ScepterRiseIncreaseByLevelOdd = { 0, 2, 3, 5, 6, 8, 9, 11, 12, 14, 16, 18, 21, 25, 29, 33 }; // Scepter with odd magic power

    private ItemLevelBonusTable? _weaponDamageIncreaseTable;

    private ItemLevelBonusTable? _staffRiseTableEven;
    private ItemLevelBonusTable? _staffRiseTableOdd;

    private ItemLevelBonusTable? _scepterRiseTableEven;
    private ItemLevelBonusTable? _scepterRiseTableOdd;

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
    /// Gets the curse damage option definition.
    /// </summary>
    protected ItemOptionDefinition CurseDamageOption
    {
        get
        {
            return this.GameConfiguration.ItemOptions.First(iod => iod.PossibleOptions.Any(o => o.OptionType == ItemOptionTypes.Option && o.PowerUpDefinition?.TargetAttribute == Stats.CurseBaseDmg));
        }
    }

    /// <summary>
    /// Gets the physical and wizardry damage option definition.
    /// </summary>
    protected ItemOptionDefinition PhysicalAndWizardryDamageOption
    {
        get
        {
            return this.GameConfiguration.ItemOptions.First(iod => iod.PossibleOptions.Any(o => o.OptionType == ItemOptionTypes.Option && o.PowerUpDefinition?.TargetAttribute == Stats.BaseDamageBonus));
        }
    }

    /// <summary>
    /// Initializes the weapons.
    /// </summary>
    /// <remarks>
    ///   Regex: (?m)^\s*(\d+)\s+(-*\d+)\s+(\d+)\s+(\d+)\s+(\d+)\s+(\d+)\s+(\d+)\s+(\d+)\s+\"(.+?)\"\s+(\d+)\s+(\d+)\s+(\d+)\s+(\d+)\s+(\d+)\s+(\d+)\s+(\d+)\s+(\d+)\s+(\d+)\s+(\d+)\s+(\d+)\s+(\d+)\s+(\d+)\s+(\d+)\s+(\d+)\s+(\d+)\s+(\d+)\s+(\d+)\s+(\d+)\s+(\d+)\s+(\d+).*$
    /// Replace: this.CreateItem(0, $1, $2, $3, $4, $5, $8 == 1, "$9", $10, $11, $12, $13, $14, $16, $17, $18, $19, $20, $21, $24, $25, $26, $27, $28, $29, $30);.
    /// </remarks>
    public override void Initialize()
    {
        this._weaponDamageIncreaseTable = this.CreateItemBonusTable(DamageIncreaseByLevel, "Damage Increase (Weapons)", "The damage increase by weapon level. It increases by 3 per level, and 1 more after level 10.");
        this._staffRiseTableEven = this.CreateItemBonusTable(StaffRiseIncreaseByLevelEven, "Staff Rise (even)", "The staff rise bonus per item level for even magic power staves.");
        this._staffRiseTableOdd = this.CreateItemBonusTable(StaffRiseIncreaseByLevelOdd, "Staff Rise (odd)", "The staff rise bonus per item level for odd magic power staves.");
        this._scepterRiseTableEven = this.CreateItemBonusTable(ScepterRiseIncreaseByLevelEven, "Scepter Rise (even)", "The scepter rise bonus per item level for even magic power scepters.");
        this._scepterRiseTableOdd = this.CreateItemBonusTable(ScepterRiseIncreaseByLevelOdd, "Scepter Rise (odd)", "The scepter rise bonus per item level for odd magic power scepters.");

        this.CreateWeapon(0, 0, 0, 0, 1, 2, true, "Kris", 6, 6, 11, 50, 20, 0, 0, 40, 40, 0, 0, 1, 1, 1, 1, 1, 1, 1);
        this.CreateWeapon(0, 1, 0, 0, 1, 3, true, "Short Sword", 3, 3, 7, 20, 22, 0, 0, 60, 0, 0, 0, 1, 1, 1, 1, 1, 1, 1);
        this.CreateWeapon(0, 2, 0, 0, 1, 3, true, "Rapier", 9, 9, 15, 40, 23, 0, 0, 50, 40, 0, 0, 0, 1, 1, 1, 1, 1, 0);
        this.CreateWeapon(0, 3, 0, 20, 1, 3, true, "Katache", 16, 16, 26, 35, 27, 0, 0, 80, 40, 0, 0, 0, 1, 0, 1, 1, 0, 0);
        this.CreateWeapon(0, 4, 0, 21, 1, 3, true, "Sword of Assassin", 12, 12, 18, 30, 24, 0, 0, 60, 40, 0, 0, 0, 1, 0, 1, 1, 0, 0);
        this.CreateWeapon(0, 5, 0, 22, 1, 3, true, "Blade", 36, 36, 47, 30, 39, 0, 0, 80, 50, 0, 0, 1, 1, 1, 1, 1, 0, 0);
        this.CreateWeapon(0, 6, 0, 20, 1, 3, true, "Gladius", 20, 20, 30, 20, 30, 0, 0, 110, 0, 0, 0, 0, 1, 1, 1, 1, 0, 0);
        this.CreateWeapon(0, 7, 0, 21, 1, 3, true, "Falchion", 24, 24, 34, 25, 34, 0, 0, 120, 0, 0, 0, 0, 1, 0, 1, 1, 0, 0);
        this.CreateWeapon(0, 8, 0, 21, 1, 3, true, "Serpent Sword", 30, 30, 40, 20, 36, 0, 0, 130, 0, 0, 0, 0, 1, 0, 1, 1, 0, 0);
        this.CreateWeapon(0, 9, 0, 20, 2, 3, true, "Sword of Salamander", 32, 32, 46, 30, 40, 0, 0, 103, 0, 0, 0, 0, 1, 0, 1, 0, 0, 0);
        this.CreateWeapon(0, 10, 0, 22, 2, 4, true, "Light Saber", 40, 47, 61, 25, 50, 0, 0, 80, 60, 0, 0, 0, 1, 1, 1, 0, 0, 0);
        this.CreateWeapon(0, 11, 0, 20, 2, 3, true, "Legendary Sword", 44, 56, 72, 20, 54, 0, 0, 120, 0, 0, 0, 0, 1, 0, 1, 0, 0, 0);
        this.CreateWeapon(0, 12, 0, 19, 2, 3, true, "Heliacal Sword", 56, 73, 98, 25, 66, 0, 0, 140, 0, 0, 0, 0, 1, 0, 1, 0, 0, 0);
        this.CreateWeapon(0, 13, 0, 22, 1, 3, true, "Double Blade", 48, 48, 56, 30, 43, 0, 0, 70, 70, 0, 0, 0, 1, 1, 1, 0, 0, 0);
        this.CreateWeapon(0, 14, 0, 22, 1, 3, true, "Lighting Sword", 59, 59, 67, 30, 50, 0, 0, 90, 50, 0, 0, 0, 1, 1, 1, 0, 0, 0);
        this.CreateWeapon(0, 15, 0, 23, 2, 3, true, "Giant Sword", 52, 60, 85, 20, 60, 0, 0, 140, 0, 0, 0, 0, 1, 0, 1, 0, 0, 0);
        this.CreateWeapon(0, 16, 0, 22, 1, 4, true, "Sword of Destruction", 82, 82, 90, 35, 84, 0, 0, 160, 60, 0, 0, 0, 1, 0, 1, 0, 0, 0);
        this.CreateWeapon(0, 17, 0, 23, 2, 4, true, "Dark Breaker", 104, 128, 153, 40, 89, 0, 0, 180, 50, 0, 0, 0, 2, 0, 0, 0, 0, 0);
        this.CreateWeapon(0, 18, 0, 23, 2, 3, true, "Thunder Blade", 105, 140, 168, 40, 86, 0, 0, 180, 50, 0, 0, 0, 0, 0, 1, 0, 0, 0);
        this.CreateWeapon(0, 19, 0, 22, 1, 4, false, "Divine Sword of Archangel", 86, 220, 230, 45, 168, 0, 0, 140, 50, 0, 0, 0, 1, 0, 1, 1, 0, 0);
        this.CreateWeapon(0, 20, 0, 22, 1, 4, true, "Knight Blade", 140, 107, 115, 35, 90, 0, 0, 116, 38, 0, 0, 0, 2, 0, 0, 0, 0, 0);
        this.CreateWeapon(0, 21, 0, 56, 2, 4, true, "Dark Reign Blade", 140, 115, 142, 40, 100, 115, 0, 116, 53, 9, 0, 0, 0, 0, 1, 0, 0, 0);
        this.CreateWeapon(0, 22, 0, 22, 1, 4, true, "Bone Blade", 147, 122, 135, 40, 95, 0, 380, 100, 35, 0, 0, 0, 2, 0, 0, 0, 0, 0);
        this.CreateWeapon(0, 23, 0, 56, 2, 4, true, "Explosion Blade", 147, 127, 155, 45, 110, 134, 380, 98, 48, 7, 0, 0, 0, 0, 1, 0, 0, 0);
        this.CreateWeapon(0, 24, 0, 22, 2, 2, true, "Daybreak", 115, 182, 218, 40, 90, 0, 0, 192, 30, 0, 0, 0, 2, 0, 0, 0, 0, 0);
        this.CreateWeapon(0, 25, 0, 56, 2, 4, true, "Sword Dancer", 115, 109, 136, 40, 90, 108, 0, 136, 57, 9, 0, 0, 0, 0, 1, 0, 0, 0);
        this.CreateWeapon(0, 26, 0, 22, 1, 4, true, "Flamberge", 137, 115, 126, 40, 90, 0, 380, 193, 53, 0, 0, 0, 2, 0, 0, 0, 0, 0);
        this.CreateWeapon(0, 27, 0, 22, 1, 4, true, "Sword Breaker", 133, 91, 99, 35, 90, 0, 380, 53, 176, 0, 0, 0, 2, 0, 0, 0, 0, 0);
        this.CreateWeapon(0, 28, 0, 56, 1, 4, true, "Imperial Sword", 139, 98, 122, 45, 90, 109, 380, 91, 73, 17, 0, 0, 0, 0, 1, 0, 0, 0);
        this.CreateWeapon(0, 31, 0, 56, 2, 4, true, "Rune Blade", 100, 104, 130, 35, 93, 104, 0, 135, 62, 9, 0, 0, 0, 0, 1, 0, 0, 0);
        this.CreateWeapon(0, 32, 0, 260, 1, 2, true, "Sacred Glove", 52, 52, 58, 25, 65, 0, 0, 85, 35, 0, 0, 0, 0, 0, 0, 0, 0, 1);
        this.CreateWeapon(0, 33, 0, 261, 1, 2, true, "Storm Hard Glove", 82, 82, 88, 30, 77, 0, 0, 100, 50, 0, 0, 0, 0, 0, 0, 0, 0, 1);
        this.CreateWeapon(0, 34, 0, 260, 1, 2, true, "Piercing Blade Glove", 105, 95, 101, 35, 86, 0, 0, 120, 60, 0, 0, 0, 0, 0, 0, 0, 0, 1);
        this.CreateWeapon(0, 35, 0, 270, 1, 2, false, "Phoenix Soul Star", 147, 122, 128, 40, 98, 0, 380, 101, 51, 0, 0, 0, 0, 0, 0, 0, 0, 1);

        this.CreateWeapon(1, 0, 0, 0, 1, 3, true, "Small Axe", 1, 1, 6, 20, 18, 0, 0, 50, 0, 0, 0, 1, 1, 1, 1, 1, 1, 1);
        this.CreateWeapon(1, 1, 0, 0, 1, 3, true, "Hand Axe", 4, 4, 9, 30, 20, 0, 0, 70, 0, 0, 0, 1, 1, 1, 1, 1, 1, 1);
        this.CreateWeapon(1, 2, 0, 19, 1, 3, true, "Double Axe", 14, 14, 24, 20, 26, 0, 0, 90, 0, 0, 0, 0, 1, 0, 1, 1, 0, 0);
        this.CreateWeapon(1, 3, 0, 19, 1, 3, true, "Tomahawk", 18, 18, 28, 30, 28, 0, 0, 100, 0, 0, 0, 0, 1, 0, 1, 1, 0, 1);
        this.CreateWeapon(1, 4, 0, 0, 1, 3, true, "Elven Axe", 26, 26, 38, 40, 32, 0, 0, 50, 70, 0, 0, 1, 0, 1, 1, 0, 1, 0);
        this.CreateWeapon(1, 5, 0, 19, 2, 3, true, "Battle Axe", 30, 36, 44, 20, 36, 0, 0, 120, 0, 0, 0, 0, 1, 1, 1, 0, 0, 0);
        this.CreateWeapon(1, 6, 0, 19, 2, 3, true, "Nikkea Axe", 34, 38, 50, 30, 44, 0, 0, 130, 0, 0, 0, 0, 1, 1, 1, 0, 0, 0);
        this.CreateWeapon(1, 7, 0, 19, 2, 3, true, "Larkan Axe", 46, 54, 67, 25, 55, 0, 0, 140, 0, 0, 0, 0, 1, 0, 1, 0, 0, 0);
        this.CreateWeapon(1, 8, 0, 19, 2, 3, true, "Crescent Axe", 54, 69, 89, 30, 65, 0, 0, 100, 40, 0, 0, 1, 1, 0, 1, 0, 0, 0);

        this.CreateWeapon(2, 0, 0, 0, 1, 3, true, "Mace", 7, 7, 13, 15, 21, 0, 0, 100, 0, 0, 0, 0, 1, 0, 1, 1, 0, 1);
        this.CreateWeapon(2, 1, 0, 19, 1, 3, true, "Morning Star", 13, 13, 22, 15, 25, 0, 0, 100, 0, 0, 0, 0, 1, 0, 1, 1, 0, 1);
        this.CreateWeapon(2, 2, 0, 0, 1, 3, true, "Flail", 22, 22, 32, 15, 32, 0, 0, 80, 50, 0, 0, 0, 1, 0, 1, 1, 0, 1);
        this.CreateWeapon(2, 3, 0, 19, 2, 3, true, "Great Hammer", 38, 45, 56, 15, 50, 0, 0, 150, 0, 0, 0, 0, 1, 0, 1, 0, 0, 1);
        this.CreateWeapon(2, 4, 0, 19, 2, 3, true, "Crystal Morning Star", 66, 78, 107, 30, 72, 0, 0, 130, 0, 0, 0, 1, 1, 1, 1, 0, 0, 1);
        this.CreateWeapon(2, 5, 0, 23, 2, 4, true, "Crystal Sword", 72, 89, 120, 40, 76, 0, 0, 130, 70, 0, 0, 1, 1, 1, 1, 0, 0, 0);
        this.CreateWeapon(2, 6, 0, 23, 2, 4, false, "Chaos Dragon Axe", 75, 102, 130, 35, 80, 0, 0, 140, 50, 0, 0, 0, 1, 0, 1, 0, 0, 0);
        this.CreateWeapon(2, 7, 0, 0, 1, 3, true, "Elemental Mace", 90, 62, 80, 50, 50, 0, 0, 15, 42, 0, 0, 0, 0, 2, 0, 0, 0, 0);
        this.CreateWeapon(2, 8, 0, 66, 1, 3, true, "Battle Scepter", 54, 41, 52, 45, 40, 3, 0, 80, 17, 0, 0, 0, 0, 0, 0, 1, 0, 0);
        this.CreateWeapon(2, 9, 0, 66, 1, 3, true, "Master Scepter", 72, 57, 68, 45, 45, 20, 0, 87, 18, 0, 0, 0, 0, 0, 0, 1, 0, 0);
        this.CreateWeapon(2, 10, 0, 66, 1, 4, true, "Great Scepter", 82, 74, 85, 45, 65, 35, 0, 100, 21, 0, 0, 0, 0, 0, 0, 1, 0, 0);
        this.CreateWeapon(2, 11, 0, 66, 1, 4, true, "Lord Scepter", 98, 91, 102, 40, 72, 52, 0, 105, 23, 0, 0, 0, 0, 0, 0, 1, 0, 0);
        this.CreateWeapon(2, 12, 0, 66, 1, 4, true, "Great Lord Scepter", 140, 108, 120, 40, 84, 67, 0, 90, 20, 0, 0, 0, 0, 0, 0, 1, 0, 0);
        this.CreateWeapon(2, 13, 0, 66, 1, 4, false, "Divine Scepter of Archangel", 150, 200, 223, 45, 90, 138, 0, 75, 16, 0, 0, 0, 0, 0, 0, 1, 0, 0);
        this.CreateWeapon(2, 14, 0, 66, 1, 4, true, "Soleil Scepter", 146, 130, 153, 40, 95, 84, 380, 80, 15, 0, 0, 0, 0, 0, 0, 1, 0, 0);
        this.CreateWeapon(2, 15, 0, 66, 1, 4, true, "Shining Scepter", 110, 99, 111, 40, 78, 60, 0, 108, 22, 0, 0, 0, 0, 0, 0, 1, 0, 0);
        this.CreateWeapon(2, 16, 0, 0, 1, 3, true, "Frost Mace", 121, 106, 146, 50, 80, 0, 0, 27, 19, 0, 0, 0, 0, 2, 0, 0, 0, 0);
        this.CreateWeapon(2, 17, 0, 66, 1, 4, true, "Absolute Scepter", 135, 114, 132, 40, 90, 72, 0, 119, 24, 0, 0, 0, 0, 0, 0, 1, 0, 0);
        this.CreateWeapon(2, 18, 0, 66, 1, 4, false, "Stryker Scepter", 147, 112, 124, 40, 86, 70, 0, 87, 20, 0, 0, 0, 0, 0, 0, 1, 0, 0);
        // this.CreateWeapon(2, 22, 0, 0, 1, 3, true, "Mace of The king", 54, 132, 153, 45, 40, 3, 0, 80, 17, 0, 0, 0, 1, 1, 1, 1, 0, 0);

        this.CreateWeapon(3, 0, 0, 22, 2, 4, true, "Light Spear", 42, 50, 63, 25, 56, 0, 0, 60, 70, 0, 0, 0, 1, 1, 1, 0, 0, 0);
        this.CreateWeapon(3, 1, 0, 0, 2, 4, true, "Spear", 23, 30, 41, 30, 42, 0, 0, 70, 50, 0, 0, 0, 1, 1, 1, 0, 0, 0);
        this.CreateWeapon(3, 2, 0, 0, 2, 4, true, "Dragon Lance", 15, 21, 33, 30, 34, 0, 0, 70, 50, 0, 0, 0, 1, 1, 1, 0, 0, 0);
        this.CreateWeapon(3, 3, 0, 0, 2, 4, true, "Giant Trident", 29, 35, 43, 25, 44, 0, 0, 90, 30, 0, 0, 0, 1, 1, 1, 0, 0, 0);
        this.CreateWeapon(3, 4, 0, 20, 2, 4, true, "Serpent Spear", 46, 58, 80, 20, 58, 0, 0, 90, 30, 0, 0, 0, 1, 1, 1, 0, 0, 0);
        this.CreateWeapon(3, 5, 0, 0, 2, 4, true, "Double Poleaxe", 13, 19, 31, 30, 38, 0, 0, 70, 50, 0, 0, 0, 1, 1, 1, 0, 0, 0);
        this.CreateWeapon(3, 6, 0, 0, 2, 4, true, "Halberd", 19, 25, 35, 30, 40, 0, 0, 70, 50, 0, 0, 0, 1, 1, 1, 0, 0, 0);
        this.CreateWeapon(3, 7, 0, 22, 2, 4, true, "Berdysh", 37, 42, 54, 30, 54, 0, 0, 80, 50, 0, 0, 0, 1, 1, 1, 0, 0, 0);
        this.CreateWeapon(3, 8, 0, 22, 2, 4, true, "Great Scythe", 54, 71, 92, 25, 68, 0, 0, 90, 50, 0, 0, 0, 1, 1, 1, 0, 0, 0);
        this.CreateWeapon(3, 9, 0, 22, 2, 4, true, "Bill of Balrog", 63, 76, 102, 25, 74, 0, 0, 80, 50, 0, 0, 0, 1, 1, 1, 0, 0, 0);
        this.CreateWeapon(3, 10, 0, 22, 2, 4, true, "Dragon Spear", 92, 112, 140, 35, 85, 0, 0, 170, 60, 0, 0, 0, 2, 0, 0, 0, 0, 0);
        this.CreateWeapon(3, 11, 0, 22, 2, 4, false, "Beuroba", 147, 190, 226, 40, 90, 0, 0, 152, 25, 0, 0, 0, 2, 0, 1, 0, 0, 0);

        this.CreateWeapon(4, 0, 1, 24, 2, 3, true, "Short Bow", 2, 3, 5, 30, 20, 0, 0, 21, 24, 0, 0, 0, 0, 1, 0, 0, 0, 0);
        this.CreateWeapon(4, 1, 1, 24, 2, 3, true, "Bow", 8, 9, 13, 30, 24, 0, 0, 27, 41, 0, 0, 0, 0, 1, 0, 0, 0, 0);
        this.CreateWeapon(4, 2, 1, 24, 2, 3, true, "Elven Bow", 16, 17, 24, 30, 28, 0, 0, 34, 63, 0, 0, 0, 0, 1, 0, 0, 0, 0);
        this.CreateWeapon(4, 3, 1, 24, 2, 3, true, "Battle Bow", 26, 28, 37, 30, 36, 0, 0, 43, 90, 0, 0, 0, 0, 1, 0, 0, 0, 0);
        this.CreateWeapon(4, 4, 1, 24, 2, 4, true, "Tiger Bow", 40, 42, 52, 30, 43, 0, 0, 56, 140, 0, 0, 0, 0, 1, 0, 0, 0, 0);
        this.CreateWeapon(4, 5, 1, 24, 2, 4, true, "Silver Bow", 56, 59, 71, 40, 48, 0, 0, 70, 188, 0, 0, 0, 0, 1, 0, 0, 0, 0);
        this.CreateWeapon(4, 6, 1, 24, 2, 4, false, "Chaos Nature Bow", 75, 88, 106, 35, 68, 0, 0, 110, 357, 0, 0, 0, 0, 1, 0, 0, 0, 0);
        this.CreateWeapon(4, 7, 1, 0, 1, 1, false, "Bolt", 0, 0, 0, 0, 255, 0, 0, 0, 0, 0, 0, 0, 0, 1, 0, 0, 0, 0);
        this.CreateWeapon(4, 8, 0, 24, 2, 2, true, "Crossbow", 4, 5, 8, 40, 22, 0, 0, 20, 90, 0, 0, 0, 0, 1, 0, 0, 0, 0);
        this.CreateWeapon(4, 9, 0, 24, 2, 2, true, "Golden Crossbow", 12, 13, 19, 40, 26, 0, 0, 30, 90, 0, 0, 0, 0, 1, 0, 0, 0, 0);
        this.CreateWeapon(4, 10, 0, 24, 2, 2, true, "Arquebus", 20, 22, 30, 40, 31, 0, 0, 30, 90, 0, 0, 0, 0, 1, 0, 0, 0, 0);
        this.CreateWeapon(4, 11, 0, 24, 2, 3, true, "Light Crossbow", 32, 35, 44, 40, 40, 0, 0, 30, 90, 0, 0, 0, 0, 1, 0, 0, 0, 0);
        this.CreateWeapon(4, 12, 0, 24, 2, 3, true, "Serpent Crossbow", 48, 50, 61, 40, 45, 0, 0, 30, 100, 0, 0, 0, 0, 1, 0, 0, 0, 0);
        this.CreateWeapon(4, 13, 0, 24, 2, 3, true, "Bluewing Crossbow", 68, 68, 82, 40, 56, 0, 0, 40, 110, 0, 0, 0, 0, 1, 0, 0, 0, 0);
        this.CreateWeapon(4, 14, 0, 24, 2, 3, true, "Aquagold Crossbow", 72, 78, 92, 30, 60, 0, 0, 50, 130, 0, 0, 0, 0, 1, 0, 0, 0, 0);
        this.CreateWeapon(4, 15, 0, 0, 1, 1, false, "Arrows", 0, 0, 0, 0, 255, 0, 0, 0, 0, 0, 0, 0, 0, 1, 0, 0, 0, 0);
        this.CreateWeapon(4, 16, 0, 24, 2, 4, true, "Saint Crossbow", 84, 102, 127, 35, 72, 0, 0, 50, 160, 0, 0, 0, 0, 1, 0, 0, 0, 0);
        this.CreateWeapon(4, 17, 1, 24, 2, 4, true, "Celestial Bow", 92, 127, 155, 35, 76, 0, 0, 54, 198, 0, 0, 0, 0, 2, 0, 0, 0, 0);
        this.CreateWeapon(4, 18, 0, 24, 2, 3, false, "Divine Crossbow of Archangel", 100, 224, 246, 45, 200, 0, 0, 40, 110, 0, 0, 0, 0, 1, 0, 0, 0, 0);
        this.CreateWeapon(4, 19, 0, 24, 2, 3, true, "Great Reign Crossbow", 100, 150, 172, 40, 80, 0, 0, 61, 285, 0, 0, 0, 0, 2, 0, 0, 0, 0);
        this.CreateWeapon(4, 20, 1, 24, 2, 4, true, "Arrow Viper Bow", 135, 166, 190, 45, 86, 0, 0, 52, 245, 0, 0, 0, 0, 2, 0, 0, 0, 0);
        this.CreateWeapon(4, 21, 1, 24, 2, 4, true, "Sylph Wind Bow", 147, 177, 200, 45, 93, 0, 380, 46, 210, 0, 0, 0, 0, 2, 0, 0, 0, 0);
        this.CreateWeapon(4, 22, 1, 24, 2, 4, true, "Albatross Bow", 110, 155, 177, 45, 70, 0, 0, 60, 265, 0, 0, 0, 0, 2, 0, 0, 0, 0);
        this.CreateWeapon(4, 23, 1, 24, 2, 4, true, "Stinger Bow", 134, 162, 184, 45, 80, 0, 0, 32, 209, 0, 0, 0, 0, 2, 0, 0, 0, 0);
        this.CreateWeapon(4, 24, 1, 24, 2, 4, false, "Air Lyn Bow", 147, 170, 194, 45, 88, 0, 0, 49, 226, 0, 0, 0, 0, 2, 0, 0, 0, 0);

        this.CreateWeapon(5, 0, 0, 0, 1, 3, true, "Skull Staff", 6, 3, 4, 20, 20, 6, 0, 40, 0, 0, 0, 1, 0, 0, 1, 0, 1, 0);
        this.CreateWeapon(5, 1, 0, 0, 2, 3, true, "Angelic Staff", 18, 10, 12, 25, 38, 20, 0, 50, 0, 0, 0, 1, 0, 0, 1, 0, 0, 0);
        this.CreateWeapon(5, 2, 0, 0, 2, 3, true, "Serpent Staff", 30, 17, 18, 25, 50, 34, 0, 50, 0, 0, 0, 1, 0, 0, 1, 0, 0, 0);
        this.CreateWeapon(5, 3, 0, 0, 2, 4, true, "Thunder Staff", 42, 23, 25, 25, 60, 46, 0, 40, 10, 0, 0, 1, 0, 0, 1, 0, 0, 0);
        this.CreateWeapon(5, 4, 0, 0, 2, 4, true, "Gorgon Staff", 52, 29, 32, 25, 65, 58, 0, 50, 0, 0, 0, 1, 0, 0, 1, 0, 0, 0);
        this.CreateWeapon(5, 5, 0, 0, 1, 4, true, "Legendary Staff", 59, 29, 31, 25, 66, 59, 0, 50, 0, 0, 0, 1, 0, 0, 1, 0, 0, 0);
        this.CreateWeapon(5, 6, 0, 0, 1, 4, true, "Staff of Resurrection", 70, 35, 39, 25, 70, 70, 0, 60, 10, 0, 0, 1, 0, 0, 1, 0, 0, 0);
        this.CreateWeapon(5, 7, 0, 0, 2, 4, false, "Chaos Lightning Staff", 75, 47, 48, 30, 70, 94, 0, 60, 10, 0, 0, 1, 0, 0, 1, 0, 0, 0);
        this.CreateWeapon(5, 8, 0, 0, 2, 4, true, "Staff of Destruction", 90, 50, 54, 30, 85, 101, 0, 60, 10, 0, 0, 1, 0, 0, 1, 0, 0, 0);
        this.CreateWeapon(5, 9, 0, 0, 1, 4, true, "Dragon Soul Staff", 100, 46, 48, 30, 91, 92, 0, 52, 16, 0, 0, 2, 0, 0, 0, 0, 0, 0);
        this.CreateWeapon(5, 10, 0, 0, 1, 4, false, "Divine Staff of Archangel", 104, 153, 165, 30, 182, 156, 0, 36, 4, 0, 0, 1, 0, 0, 1, 0, 0, 0);
        this.CreateWeapon(5, 11, 0, 0, 1, 4, true, "Staff of Kundun", 140, 55, 61, 30, 95, 110, 0, 45, 16, 0, 0, 2, 0, 0, 1, 0, 0, 0);
        this.CreateWeapon(5, 12, 0, 0, 1, 4, true, "Grand Viper Staff", 147, 66, 74, 30, 100, 130, 380, 39, 13, 0, 0, 2, 0, 0, 0, 0, 0, 0);
        this.CreateWeapon(5, 13, 0, 0, 1, 4, true, "Platina Staff", 110, 51, 53, 30, 78, 120, 0, 50, 16, 0, 0, 2, 0, 0, 0, 0, 0, 0);
        this.CreateWeapon(5, 14, 0, 0, 1, 4, true, "Mistery Stick", 28, 17, 18, 25, 50, 34, 0, 34, 14, 0, 0, 0, 0, 0, 0, 0, 1, 0);
        this.CreateWeapon(5, 15, 0, 0, 1, 4, true, "Violent Wind Stick", 42, 23, 25, 25, 60, 46, 0, 33, 17, 0, 0, 0, 0, 0, 0, 0, 1, 0);
        this.CreateWeapon(5, 16, 0, 0, 1, 4, true, "Red Wing Stick", 59, 29, 31, 25, 65, 59, 0, 36, 14, 0, 0, 0, 0, 0, 0, 0, 1, 0);
        this.CreateWeapon(5, 17, 0, 0, 1, 4, true, "Ancient Stick", 78, 38, 40, 25, 81, 76, 0, 50, 19, 0, 0, 0, 0, 0, 0, 0, 1, 0);
        this.CreateWeapon(5, 18, 0, 0, 1, 4, true, "Demonic Stick", 100, 46, 48, 30, 91, 92, 0, 54, 15, 0, 0, 0, 0, 0, 0, 0, 2, 0);
        this.CreateWeapon(5, 19, 0, 0, 1, 4, true, "Storm Blitz Stick", 110, 51, 53, 30, 95, 110, 380, 64, 15, 0, 0, 0, 0, 0, 0, 0, 2, 0);
        this.CreateWeapon(5, 20, 0, 0, 1, 4, true, "Eternal Wing Stick", 147, 66, 74, 30, 100, 106, 380, 57, 13, 0, 0, 0, 0, 0, 0, 0, 2, 0);
        this.CreateWeapon(5, 21, 1, 223, 1, 2, true, "Book of Sahamutt", 52, 0, 0, 25, 60, 46, 0, 0, 20, 135, 0, 0, 0, 0, 0, 0, 1, 0);
        this.CreateWeapon(5, 22, 1, 224, 1, 2, true, "Book of Neil", 59, 0, 0, 25, 65, 59, 0, 0, 25, 168, 0, 0, 0, 0, 0, 0, 1, 0);
        this.CreateWeapon(5, 23, 1, 225, 1, 2, true, "Book of Lagle", 65, 0, 0, 25, 50, 72, 0, 0, 30, 201, 0, 0, 0, 0, 0, 0, 1, 0);
        this.CreateWeapon(5, 30, 0, 0, 1, 4, true, "Deadly Staff", 138, 57, 59, 30, 91, 126, 380, 47, 18, 0, 0, 0, 0, 0, 1, 0, 0, 0);
        this.CreateWeapon(5, 31, 0, 0, 1, 4, true, "Imperial Staff", 137, 57, 61, 30, 182, 124, 380, 48, 14, 0, 0, 2, 0, 0, 0, 0, 0, 0);
        this.CreateWeapon(5, 33, 0, 0, 1, 4, false, "Chromatic Staff", 147, 55, 57, 30, 78, 124, 0, 50, 12, 0, 0, 2, 0, 0, 1, 0, 0, 0);
        this.CreateWeapon(5, 34, 0, 0, 1, 4, false, "Raven Stick", 147, 70, 78, 30, 98, 130, 0, 50, 14, 0, 0, 0, 0, 0, 0, 0, 2, 0);
        this.CreateWeapon(5, 36, 0, 0, 1, 4, false, "Divine Stick of Archangel", 104, 153, 165, 30, 182, 146, 0, 55, 13, 0, 0, 0, 0, 0, 0, 0, 1, 0);

        this.AddGuardianOptions();
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
    /// <param name="dropsFromMonsters">if set to <c>true</c> [drops from monsters].</param>
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
    /// <param name="darkLordClass">The dark lord class.</param>
    /// <param name="summonerClass">The summoner class.</param>
    /// <param name="ragefighterClass">The ragefighter class.</param>
    protected void CreateWeapon(byte @group, byte number, byte slot, int skillNumber, byte width, byte height,
        bool dropsFromMonsters, string name, byte dropLevel, int minimumDamage, int maximumDamage, int attackSpeed,
        byte durability, int magicPower, int levelRequirement, int strengthRequirement, int agilityRequirement,
        int energyRequirement, int vitalityRequirement,
        int wizardClass, int knightClass, int elfClass, int magicGladiatorClass, int darkLordClass, int summonerClass, int ragefighterClass)
    {
        var item = this.Context.CreateNew<ItemDefinition>();
        this.GameConfiguration.Items.Add(item);
        item.Name = name;
        item.Group = group;
        item.Number = number;

        item.Height = height;
        item.Width = width;
        item.DropLevel = dropLevel;
        item.MaximumItemLevel = MaximumItemLevel;
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
        var qualifiedCharacterClasses = this.GameConfiguration.DetermineCharacterClasses(wizardClass, knightClass, elfClass, magicGladiatorClass, darkLordClass, summonerClass, ragefighterClass);
        qualifiedCharacterClasses.ToList().ForEach(item.QualifiedCharacters.Add);

        if (minimumDamage > 0)
        {
            var minDamagePowerUp = this.CreateItemBasePowerUpDefinition(Stats.MinimumPhysBaseDmgByWeapon, minimumDamage, AggregateType.AddRaw);
            minDamagePowerUp.BonusPerLevelTable = this._weaponDamageIncreaseTable;
            item.BasePowerUpAttributes.Add(minDamagePowerUp);

            var maxDamagePowerUp = this.CreateItemBasePowerUpDefinition(Stats.MaximumPhysBaseDmgByWeapon, maximumDamage, AggregateType.AddRaw);
            maxDamagePowerUp.BonusPerLevelTable = this._weaponDamageIncreaseTable;
            item.BasePowerUpAttributes.Add(maxDamagePowerUp);
        }

        var speedPowerUp = this.CreateItemBasePowerUpDefinition(Stats.AttackSpeedByWeapon, attackSpeed, AggregateType.AddRaw);
        item.BasePowerUpAttributes.Add(speedPowerUp);

        this.CreateItemRequirementIfNeeded(item, Stats.Level, levelRequirement);
        this.CreateItemRequirementIfNeeded(item, Stats.TotalStrengthRequirementValue, strengthRequirement);
        this.CreateItemRequirementIfNeeded(item, Stats.TotalAgilityRequirementValue, agilityRequirement);
        this.CreateItemRequirementIfNeeded(item, Stats.TotalEnergyRequirementValue, energyRequirement);
        this.CreateItemRequirementIfNeeded(item, Stats.TotalVitalityRequirementValue, vitalityRequirement);

        item.PossibleItemOptions.Add(this.Luck);

        if (magicPower == 0 || darkLordClass > 0 || group == (int)ItemGroups.Swords)
        {
            item.PossibleItemOptions.Add(this.GameConfiguration.ItemOptions.Single(o => o.Name == ExcellentOptions.PhysicalAttackOptionsName));
            item.PossibleItemOptions.Add(this.GameConfiguration.ItemOptions.Single(o => o.Name == HarmonyOptions.PhysicalAttackOptionsName));

            if (skillNumber == (int)SkillNumber.PowerSlash)
            {
                // MG "magic swords" have a double item option, and wizardry rise, functioning as both sword and staff
                item.PossibleItemOptions.Add(this.PhysicalAndWizardryDamageOption);

                var swordRisePowerUp = this.CreateItemBasePowerUpDefinition(Stats.StaffRise, magicPower / 2.0f, AggregateType.AddRaw);
                swordRisePowerUp.BonusPerLevelTable = magicPower % 2 == 0 ? this._staffRiseTableEven : this._staffRiseTableOdd;
                item.BasePowerUpAttributes.Add(swordRisePowerUp);
            }
            else
            {
                item.PossibleItemOptions.Add(this.PhysicalDamageOption);

                if (skillNumber == (int)SkillNumber.ForceWave)
                {
                    var scepterRisePowerUp = this.CreateItemBasePowerUpDefinition(Stats.ScepterRise, magicPower / 2.0f, AggregateType.AddRaw);
                    scepterRisePowerUp.BonusPerLevelTable = magicPower % 2 == 0 ? this._scepterRiseTableEven : this._scepterRiseTableOdd;
                    item.BasePowerUpAttributes.Add(scepterRisePowerUp);
                }
            }
        }
        else
        {
            item.PossibleItemOptions.Add(this.GameConfiguration.ItemOptions.Single(o => o.Name == ExcellentOptions.WizardryAttackOptionsName));
            item.PossibleItemOptions.Add(this.GameConfiguration.ItemOptions.Single(o => o.Name == HarmonyOptions.WizardryAttackOptionsName));

            if (summonerClass > 0 && slot == 1)
            {
                item.PossibleItemOptions.Add(this.CurseDamageOption);

                var bookRisePowerUp = this.CreateItemBasePowerUpDefinition(Stats.BookRise, magicPower / 2.0f, AggregateType.AddRaw);
                bookRisePowerUp.BonusPerLevelTable = magicPower % 2 == 0 ? this._staffRiseTableEven : this._staffRiseTableOdd;
                item.BasePowerUpAttributes.Add(bookRisePowerUp);
            }
            else
            {
                item.PossibleItemOptions.Add(this.WizardryDamageOption);

                var staffRisePowerUp = this.CreateItemBasePowerUpDefinition(Stats.StaffRise, magicPower / 2.0f, AggregateType.AddRaw);
                staffRisePowerUp.BonusPerLevelTable = magicPower % 2 == 0 ? this._staffRiseTableEven : this._staffRiseTableOdd;
                item.BasePowerUpAttributes.Add(staffRisePowerUp);
            }
        }

        if (height > 1) // exclude bolts and arrows
        {
            item.BasePowerUpAttributes.Add(this.CreateItemBasePowerUpDefinition(Stats.EquippedWeaponCount, 1, AggregateType.AddRaw));
        }

        if (group == (int)ItemGroups.Bows && height > 1)
        {
            item.BasePowerUpAttributes.Add(this.CreateItemBasePowerUpDefinition(Stats.AmmunitionConsumptionRate, 1, AggregateType.AddRaw));
        }

        item.IsAmmunition = group == (int)ItemGroups.Bows && height == 1;

        if (group != (int)ItemGroups.Bows && group != (int)ItemGroups.Staff && width == 2)
        {
            item.BasePowerUpAttributes.Add(this.CreateItemBasePowerUpDefinition(Stats.IsTwoHandedWeaponEquipped, 1, AggregateType.AddRaw));
        }

        if (group == (int)ItemGroups.Swords || (group == (int)ItemGroups.Scepters && number == 5)) // Crystal Sword
        {
            if (ragefighterClass == 0 || number < 2)
            {
                if (width == 1)
                {
                    item.BasePowerUpAttributes.Add(this.CreateItemBasePowerUpDefinition(Stats.IsOneHandedSwordEquipped, 1, AggregateType.AddRaw));
                }
                else
                {
                    item.BasePowerUpAttributes.Add(this.CreateItemBasePowerUpDefinition(Stats.IsTwoHandedSwordEquipped, 1, AggregateType.AddRaw));
                }
            }
            else
            {
                item.BasePowerUpAttributes.Add(this.CreateItemBasePowerUpDefinition(Stats.IsGloveWeaponEquipped, 1, AggregateType.AddRaw));
            }
        }

        if (group == (int)ItemGroups.Spears)
        {
            item.BasePowerUpAttributes.Add(this.CreateItemBasePowerUpDefinition(Stats.IsSpearEquipped, 1, AggregateType.AddRaw));
        }

        if (group == (int)ItemGroups.Scepters)
        {
            if (skillNumber == (int)SkillNumber.ForceWave)
            {
                item.BasePowerUpAttributes.Add(this.CreateItemBasePowerUpDefinition(Stats.IsScepterEquipped, 1, AggregateType.AddRaw));
            }
            else if (knightClass > 0 && (skillNumber == (int)SkillNumber.FallingSlash || number < 5))
            {
                item.BasePowerUpAttributes.Add(this.CreateItemBasePowerUpDefinition(Stats.IsMaceEquipped, 1, AggregateType.AddRaw));
            }
            else
            {
                // not a relevant mace or scepter ...
            }
        }

        if (group == (int)ItemGroups.Staff)
        {
            if (wizardClass == 0 && summonerClass > 0 && slot == 0)
            {
                item.BasePowerUpAttributes.Add(this.CreateItemBasePowerUpDefinition(Stats.IsStickEquipped, 1, AggregateType.AddRaw));
            }
            else if (wizardClass > 0 || magicGladiatorClass > 0)
            {
                item.BasePowerUpAttributes.Add(this.CreateItemBasePowerUpDefinition(width == 1 ? Stats.IsOneHandedStaffEquipped : Stats.IsTwoHandedStaffEquipped, 1, AggregateType.AddRaw));
            }
            else
            {
                // It's a book. Nothing to do here.
            }
        }

        if (group == (int)ItemGroups.Bows && !item.IsAmmunition)
        {
            item.BasePowerUpAttributes.Add(this.CreateItemBasePowerUpDefinition(slot == 0 ? Stats.IsCrossBowEquipped : Stats.IsBowEquipped, 1, AggregateType.AddRaw));
        }
    }

    private void AddGuardianOptions()
    {
        var weaponOption = this.GameConfiguration.ItemOptions.First(io => io.PossibleOptions.Any(po => po.OptionType == ItemOptionTypes.GuardianOption && po.Number == (int)ItemGroups.Weapon));

        var boneBlade = this.GameConfiguration.Items.First(i => i.Number == 22 && i.Group == (int)ItemGroups.Swords);
        var explosionBlade = this.GameConfiguration.Items.First(i => i.Number == 23 && i.Group == (int)ItemGroups.Swords);
        var phoenixSoulStar = this.GameConfiguration.Items.First(i => i.Number == 35 && i.Group == (int)ItemGroups.Swords);
        var soleilScepter = this.GameConfiguration.Items.First(i => i.Number == 14 && i.Group == (int)ItemGroups.Scepters);
        var sylphWindBow = this.GameConfiguration.Items.First(i => i.Number == 21 && i.Group == (int)ItemGroups.Bows);
        var viperStaff = this.GameConfiguration.Items.First(i => i.Number == 12 && i.Group == (int)ItemGroups.Staff);
        var stormBlitzStick = this.GameConfiguration.Items.First(i => i.Number == 19 && i.Group == (int)ItemGroups.Staff);

        boneBlade.PossibleItemOptions.Add(weaponOption);
        explosionBlade.PossibleItemOptions.Add(weaponOption);
        phoenixSoulStar.PossibleItemOptions.Add(weaponOption);
        soleilScepter.PossibleItemOptions.Add(weaponOption);
        sylphWindBow.PossibleItemOptions.Add(weaponOption);
        viperStaff.PossibleItemOptions.Add(weaponOption);
        stormBlitzStick.PossibleItemOptions.Add(weaponOption);
    }
}