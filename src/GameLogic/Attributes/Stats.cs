// <copyright file="Stats.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.GameLogic.Attributes;

using MUnique.OpenMU.AttributeSystem;

/// <summary>
/// A collection of standard attributes.
/// </summary>
public class Stats
{
    /// <summary>
    /// Gets the base strength attribute definition.
    /// </summary>
    public static AttributeDefinition BaseStrength { get; } = new (new Guid("123282FE-FEAD-448E-AD2C-BAECE939B4B1"), "Base Strength", "The base strength of the character.");

    /// <summary>
    /// Gets the total strength attribute definition.
    /// </summary>
    public static AttributeDefinition TotalStrength { get; } = new (new Guid("F59709A3-44B0-4147-AAEB-E90CEC251641"), "Total Strength", "The total strength of the character, which contains the base strength and additional strength powerups");

    /// <summary>
    /// Gets the total strength requirement value attribute definition.
    /// </summary>
    public static AttributeDefinition TotalStrengthRequirementValue { get; } = new (new Guid("7BC30A84-5FDE-490B-81A4-FFECB41CA901"), "Total Strength Requirement Value", "The total strength requirement value of an item, which is used to calculate the required total strength of a character to equip an item. The required total strength depends on the options, drop level and level of the item.");

    /// <summary>
    /// Gets the base agility attribute definition.
    /// </summary>
    public static AttributeDefinition BaseAgility { get; } = new (new Guid("1AE9C014-E3CD-4703-BD05-1B65F5F94CEB"), "Base Agility", "The base agility of the character.");

    /// <summary>
    /// Gets the total agility attribute definition.
    /// </summary>
    public static AttributeDefinition TotalAgility { get; } = new (new Guid("364F1207-00F8-485B-9F2C-74E04CB78C73"), "Total Agility", "The total agility of the character.");

    /// <summary>
    /// Gets the total agility requirement value attribute definition.
    /// </summary>
    public static AttributeDefinition TotalAgilityRequirementValue { get; } = new (new Guid("DFE7A14F-BF1C-414F-8B77-B8FE6FD76A7D"), "Total Agility Requirement Value", "The total agility requirement value of an item, which is used to calculate the required total agility of a character to equip an item. The required total agility depends on the options, drop level and level of the item.");

    /// <summary>
    /// Gets the base vitality attribute definition.
    /// </summary>
    public static AttributeDefinition BaseVitality { get; } = new (new Guid("6CA5C3A6-B109-45A5-87A7-FDCB107B4982"), "Base Vitality", string.Empty);

    /// <summary>
    /// Gets the total vitality attribute definition.
    /// </summary>
    public static AttributeDefinition TotalVitality { get; } = new (new Guid("6A0076E4-69DC-42E7-A92B-C8711392EF82"), "Total Vitality", string.Empty);

    /// <summary>
    /// Gets the total vitality requirement value attribute definition.
    /// </summary>
    public static AttributeDefinition TotalVitalityRequirementValue { get; } = new (new Guid("B7B77FF8-1833-4739-98EE-0C2DD7344F56"), "Total Vitality Requirement Value", "The total vitality requirement value of an item, which is used to calculate the required total vitality of a character to equip an item. The required total vitality depends on the options, drop level and level of the item.");

    /// <summary>
    /// Gets the base energy attribute definition.
    /// </summary>
    public static AttributeDefinition BaseEnergy { get; } = new (new Guid("01B0EF28-F7A0-46B5-97BA-2B624A54CD75"), "Base Energy", string.Empty);

    /// <summary>
    /// Gets the total energy attribute definition.
    /// </summary>
    public static AttributeDefinition TotalEnergy { get; } = new (new Guid("12956B45-007C-453A-AE1F-36475B8CEBBF"), "Total Energy", string.Empty);

    /// <summary>
    /// Gets the total energy requirement value attribute definition.
    /// </summary>
    public static AttributeDefinition TotalEnergyRequirementValue { get; } = new (new Guid("5EF1FCD1-0C08-4087-BFCE-C655BB121CDD"), "Total Energy Requirement Value", "The total energy requirement value of an item, which is used to calculate the required total energy of a character to equip an item. The required total energy depends on the options, drop level and level of the item.");

    /// <summary>
    /// Gets the base leadership attribute definition.
    /// </summary>
    public static AttributeDefinition BaseLeadership { get; } = new (new Guid("6AF2C9DF-3AE4-4721-8462-9A8EC7F56FE4"), "Base Leadership", string.Empty);

    /// <summary>
    /// Gets the total leadership attribute definition.
    /// </summary>
    public static AttributeDefinition TotalLeadership { get; } = new (new Guid("35E04272-63F3-4EBB-8FB5-EF2128DDB9F6"), "Total Leadership", string.Empty);

    /// <summary>
    /// Gets the total leadership requirement value attribute definition.
    /// </summary>
    public static AttributeDefinition TotalLeadershipRequirementValue { get; } = new (new Guid("E38A897E-ED6F-4C06-AE11-7CAA7EAEC5A9"), "Total Leadership Requirement Value", "The total leadership requirement value of an item, which is used to calculate the required total leadership of a character to equip an item. The required total leadership depends on the options, drop level and level of the item.");

    /// <summary>
    /// Gets the level attribute definition.
    /// </summary>
    public static AttributeDefinition Level { get; } = new (new Guid("560931AD-0901-4342-B7F4-FD2E2FCC0563"), "Level", "The level of the character.");

    /// <summary>
    /// Gets the <see cref="Character.LevelUpPoints"/> per level up.
    /// </summary>
    public static AttributeDefinition PointsPerLevelUp { get; } = new (new Guid("48074BC6-DDC9-4264-8F1E-004D46D5B6EC"), "Points per Level up", "Defines the level up points per achieved level.");

    /// <summary>
    /// Gets the <see cref="Character.MasterLevelUpPoints"/> per level up.
    /// </summary>
    public static AttributeDefinition MasterPointsPerLevelUp { get; } = new(new Guid("E0C7D483-E7B2-4898-89C3-4C72E64A4418"), "Master points per master Level up", "Defines the master level up points per achieved master level.");

    /// <summary>
    /// Gets the experience rate attribute definition.
    /// </summary>
    public static AttributeDefinition ExperienceRate { get; } = new (new Guid("1AD454D4-BEF9-416E-BC49-82A5B0277FC7"), "Experience Rate", "Defines the experience rate multiplier of a character. By default it's 1.0 and may be modified by seals or other stuff.");

    /// <summary>
    /// Gets the master level definition.
    /// </summary>
    public static AttributeDefinition MasterLevel { get; } = new (new Guid("70CD8C10-391A-4C51-9AA4-A854600E3A9F"), "Master Level", "The level of the character.");

    /// <summary>
    /// Gets the master experience rate definition.
    /// </summary>
    public static AttributeDefinition MasterExperienceRate { get; } = new (new Guid("E367A231-C8A4-4F92-B553-C665F98DB1FC"), "Master Experience Rate", string.Empty);

    /// <summary>
    /// Gets the reset quantity attribute definition.
    /// </summary>
    public static AttributeDefinition Resets { get; } = new (new Guid("89A891A7-F9F9-4AB5-AF36-12056E53A5F7"), "Resets", "Reset quantity of current character");

    /// <summary>
    /// Gets the zen amount rate attribute definition.
    /// </summary>
    public static AttributeDefinition MoneyAmountRate { get; } = new (new Guid("D84D1A5C-3A56-4CB9-8DD4-158AFD4D1EDB"), "Money Drop Amount Rate", "Defines a multiplier for the amount of a money drop.");

    /// <summary>
    /// Gets the current health attribute definition.
    /// </summary>
    public static AttributeDefinition CurrentHealth { get; } = new (new Guid("20686FFD-7A96-4BE2-9889-2A4DD9FF5A25"), "Current Health", string.Empty);

    /// <summary>
    /// Gets the maximum health attribute definition.
    /// </summary>
    public static AttributeDefinition MaximumHealth { get; } = new (new Guid("A6C39A5C-295F-415E-A314-5E9F9A748D27"), "Maximum Health", string.Empty);

    /// <summary>
    /// Gets the current mana attribute definition.
    /// </summary>
    public static AttributeDefinition CurrentMana { get; } = new (new Guid("B3299EE6-3815-4E48-B620-95DB78F8A142"), "Current Mana", string.Empty);

    /// <summary>
    /// Gets the maximum mana attribute definition.
    /// </summary>
    public static AttributeDefinition MaximumMana { get; } = new (new Guid("17CB8826-0677-4C93-A0C9-C0E3D2DA7D73"), "Maximum Mana", string.Empty);

    /// <summary>
    /// Gets the current shield attribute definition.
    /// </summary>
    public static AttributeDefinition CurrentShield { get; } = new (new Guid("0E255161-8A3D-4367-BFF0-EFCD238C16FD"), "Current Shield", string.Empty);

    /// <summary>
    /// Gets the maximum shield attribute definition.
    /// </summary>
    public static AttributeDefinition MaximumShield { get; } = new (new Guid("BC745471-6EC6-48BC-8C7A-C8AACA3A92D9"), "Maximum Shield", string.Empty);

    /// <summary>
    /// Gets the maximum shield intermediate attribute definition, which should contain the Level ^ 2.
    /// </summary>
    public static AttributeDefinition MaximumShieldTemp { get; } = new (new Guid("91321D76-F60E-41E1-948F-5B05D788C2BB"), "Maximum shield level pow intermediate", "Intermediate value for maximum shield");

    /// <summary>
    /// Gets the current ability attribute definition.
    /// </summary>
    public static AttributeDefinition CurrentAbility { get; } = new (new Guid("39EB6747-0689-4BBF-B832-8936E00C5DF6"), "Current Ability", string.Empty);

    /// <summary>
    /// Gets the maximum ability attribute definition.
    /// </summary>
    public static AttributeDefinition MaximumAbility { get; } = new (new Guid("466BBBBA-C1D8-45DC-8832-2EAA1130ACFD"), "Maximum Ability", string.Empty);

    /// <summary>
    /// Gets the attack rate PVM attribute definition.
    /// </summary>
    public static AttributeDefinition AttackRatePvm { get; } = new (new Guid("1129442A-E1C7-4240-8866-B781C2838C25"), "Attack Rate (PvM)", string.Empty);

    /// <summary>
    /// Gets the attack rate PVP attribute definition.
    /// </summary>
    public static AttributeDefinition AttackRatePvp { get; } = new (new Guid("C39C1C4B-0F58-49FC-9C71-8A58D570C5D2"), "Attack Rate (PvP)", string.Empty);

    /// <summary>
    /// Gets the minimum physical base DMG by weapon attribute definition.
    /// </summary>
    public static AttributeDefinition MinimumPhysBaseDmgByWeapon { get; } = new (new Guid("1AC59D93-6A52-4E88-8201-5F125A63B2A1"), "Minimum Physical Base Damage By Weapon", string.Empty);

    /// <summary>
    /// Gets the maximum physical base DMG by weapon attribute definition.
    /// </summary>
    public static AttributeDefinition MaximumPhysBaseDmgByWeapon { get; } = new (new Guid("EC0D70BE-839C-4BAD-9FC5-1AD7438C75F8"), "Maximum Physical Base Damage By Weapon", string.Empty);

    /// <summary>
    /// Gets the minimum physical base DMG attribute definition.
    /// </summary>
    public static AttributeDefinition MinimumPhysBaseDmg { get; } = new (new Guid("3E8D6A02-E973-4AE4-9DF3-CDDC3D3183B3"), "Minimum Physical Base Damage", string.Empty);

    /// <summary>
    /// Gets the maximum physical base DMG attribute definition.
    /// </summary>
    public static AttributeDefinition MaximumPhysBaseDmg { get; } = new (new Guid("8A918EA2-893A-48B2-A684-3E71526CA71F"), "Maximum Physical Base Damage", string.Empty);

    /// <summary>
    /// Gets the min and max physical base DMG attribute definition.
    /// </summary>
    public static AttributeDefinition PhysicalBaseDmg { get; } = new(new Guid("DD1E13E4-BFFD-45B5-9B91-9080710324B2"), "Physical Base Damage (min and max)", string.Empty);

    /// <summary>
    /// Gets the minimum wiz base DMG attribute definition.
    /// </summary>
    public static AttributeDefinition MinimumWizBaseDmg { get; } = new (new Guid("65583A02-AB94-4A17-9B79-86ECC82DC835"), "Minimum Wizardry Base Damage", string.Empty);

    /// <summary>
    /// Gets the maximum wiz base DMG attribute definition.
    /// </summary>
    public static AttributeDefinition MaximumWizBaseDmg { get; } = new (new Guid("44B8236A-BF5B-4082-BA8B-5DEDA1458D33"), "Maximum Wizardry Base Damage", string.Empty);

    /// <summary>
    /// Gets the min and max wiz base DMG attribute definition.
    /// </summary>
    public static AttributeDefinition WizardryBaseDmg { get; } = new(new Guid("7F4F3646-33A6-40AC-8DA6-29A0A0F46016"), "Wizardry Base Damage (min and max)", string.Empty);

    /// <summary>
    /// Gets the staff rise percentage attribute definition.
    /// </summary>
    public static AttributeDefinition StaffRise { get; } = new(new Guid("DB2F48FD-42AB-4204-B863-AFEE138A9D43"), "Staff Rise Percentage", string.Empty);

    /// <summary>
    /// Gets the scepter rise percentage attribute definition.
    /// </summary>
    public static AttributeDefinition ScepterRise { get; } = new(new Guid("FB374862-D360-4FF0-AB88-2C170E6A9F85"), "Scepter Rise Percentage", string.Empty);

    /// <summary>
    /// Gets the minimum curse base DMG attribute definition.
    /// </summary>
    public static AttributeDefinition MinimumCurseBaseDmg { get; } = new (new Guid("B8AE2D6B-05CE-43A9-B2BB-3C32F288A043"), "Minimum Curse Base Damage", string.Empty);

    /// <summary>
    /// Gets the maximum curse base DMG attribute definition.
    /// </summary>
    public static AttributeDefinition MaximumCurseBaseDmg { get; } = new (new Guid("5E7B5B56-BB4D-4645-9593-836FE86E80EA"), "Maximum Curse Base Damage", string.Empty);

    /// <summary>
    /// Gets the attribute definition for the base damage of the fenrir pet.
    /// </summary>
    public static AttributeDefinition FenrirBaseDmg { get; } = new (new Guid("96F47E70-5C85-4A92-B224-944A9359240E"), "Fenrir Base Damage", string.Empty);

    /// <summary>
    /// Gets the base damage bonus attribute definition.
    /// </summary>
    public static AttributeDefinition BaseDamageBonus { get; } = new (new Guid("BB6F0151-EAB2-4A9D-BFE3-51E145F36C52"), "Base Damage Bonus", "A bonus value which gets added to all min/max damage values during the damage calculation");

    /// <summary>
    /// Gets the base min damage bonus attribute definition.
    /// </summary>
    public static AttributeDefinition BaseMinDamageBonus { get; } = new (new Guid("ACE8CC0A-3288-491C-A49F-4B754A18BA1F"), "Base Min Damage Bonus", "A bonus value which gets added to all min damage values during the damage calculation");

    /// <summary>
    /// Gets the base max damage bonus attribute definition.
    /// </summary>
    public static AttributeDefinition BaseMaxDamageBonus { get; } = new (new Guid("7C9E419B-63B0-4237-B799-B80418693A61"), "Base Max Damage Bonus", "A bonus value which gets added to all max damage values during the damage calculation");

    /// <summary>
    /// Gets the skill multiplier attribute definition.
    /// </summary>
    public static AttributeDefinition SkillMultiplier { get; } = new (new Guid("D9FB3323-6DF5-48F7-8253-FDBB5EF82114"), "Skill Damage Multiplier", string.Empty);

    /// <summary>
    /// Gets the skill damage bonus attribute definition.
    /// </summary>
    public static AttributeDefinition SkillDamageBonus { get; } = new (new Guid("B8B214B1-396B-4CA8-9A77-240AA70A989B"), "Skill Damage Bonus", "A bonus value which gets added to the damage calculation when the damage is calculated with a skill.");

    /// <summary>
    /// Gets the critical damage bonus attribute definition.
    /// </summary>
    public static AttributeDefinition CriticalDamageBonus { get; } = new (new Guid("33F53519-16F3-44C2-9D36-432C36329C78"), "Critical Damage Bonus", "A bonus value which gets added to the damage calculation when the damage is calculated and critical damage applies.");

    /// <summary>
    /// Gets the excellent damage bonus attribute definition.
    /// </summary>
    public static AttributeDefinition ExcellentDamageBonus { get; } = new (new Guid("9CB8705A-398D-4158-BC60-D6ADBED36A28"), "Excellent Damage Bonus", "A bonus value which gets added to the damage calculation when the damage is calculated and excellent damage applies.");

    /// <summary>
    /// Gets the attack speed attribute definition.
    /// </summary>
    public static AttributeDefinition AttackSpeed { get; } = new (new Guid("BACC1115-1E8B-4E62-B952-8F8DDB58A949"), "Attack Speed", string.Empty);

    /// <summary>
    /// Gets the attack damage increase attribute definition.
    /// </summary>
    public static AttributeDefinition AttackDamageIncrease { get; } = new (new Guid("0765CCD2-C70A-4338-BF49-0D652364C223"), "Attack Damage Increase Multiplier", string.Empty);

    /// <summary>
    /// Gets the wizardry attack damage increase attribute definition.
    /// </summary>
    public static AttributeDefinition WizardryAttackDamageIncrease { get; } = new (new Guid("8F1CD5A5-3792-42FC-89B8-E6D50F997F4B"), "Wizardry Attack Damage Increase Multiplier", "The wizardry damage increase which is multiplied with the min/max wiz base damage and added to it.");

    /// <summary>
    /// Gets the pet attack damage increase attribute definition.
    /// </summary>
    public static AttributeDefinition PetAttackDamageIncrease { get; } = new(new Guid("662467B2-CBCF-4347-9B39-A2BBEE04E6D7"), "Pet Attack Damage Increase Multiplier", "The pet damage increase which is multiplied with the min/max wiz base damage and added to it.");

    /// <summary>
    /// Gets the curse attack damage increase attribute definition.
    /// </summary>
    public static AttributeDefinition CurseAttackDamageIncrease { get; } = new (new Guid("2B8904D5-9901-40C0-BFDE-66675672D9DC"), "Curse Attack Damage Increase Multiplier", "The cursed damage increase which is multiplied with the min/max curse base damage and added to it.");

    /// <summary>
    /// Gets the two handed weapon damage increase attribute definition.
    /// </summary>
    public static AttributeDefinition TwoHandedWeaponDamageIncrease { get; } = new (new Guid("BA3D57E9-68A5-47AC-A6E9-43793F4DDE2A"), "Two-Handed Weapon Damage Increase", "The damage increase which is multiplied with the min/max base damage and added to it when using a two-handed weapon.");

    /// <summary>
    /// Gets the is two handed weapon equipped.
    /// </summary>
    public static AttributeDefinition IsTwoHandedWeaponEquipped { get; } = new (new Guid("7426781F-CD87-4F2B-8B03-9447B670C632"), "Is Two-Handed Weapon Equipped", string.Empty);

    /// <summary>
    /// Gets the is a bow equipped.
    /// </summary>
    public static AttributeDefinition IsBowEquipped { get; } = new(new Guid("8B1135B7-C323-4822-9B98-1A56C802BB6A"), "Is Bow Equipped", string.Empty);

    /// <summary>
    /// Gets the bow bonus base damage.
    /// </summary>
    public static AttributeDefinition BowBonusBaseDamage { get; } = new(new Guid("AF19A56D-2F31-4A80-8DF7-B99ADF7D275B"), "Bow Bonus Base Damage", string.Empty);

    /// <summary>
    /// Gets the is a cross bow equipped.
    /// </summary>
    public static AttributeDefinition IsCrossBowEquipped { get; } = new(new Guid("EDB70177-D824-45FC-8B76-7AE26859B7E5"), "Is Cross Bow Equipped", string.Empty);

    /// <summary>
    /// Gets the bow bonus base damage.
    /// </summary>
    public static AttributeDefinition CrossBowBonusBaseDamage { get; } = new(new Guid("DBAD454A-EB1C-4FEE-9B40-97EA70278BB5"), "Cross Bow Bonus Base Damage", string.Empty);

    /// <summary>
    /// Gets the is a one handed sword equipped.
    /// </summary>
    public static AttributeDefinition IsOneHandedSwordEquipped { get; } = new(new Guid("21B71774-EB4F-41F6-A0AA-0E600F41226C"), "Is One Handed Sword Equipped", string.Empty);

    /// <summary>
    /// Gets the one handed sword bonus base damage.
    /// </summary>
    public static AttributeDefinition OneHandedSwordBonusBaseDamage { get; } = new(new Guid("60FA78A3-4E8C-4304-9077-5D3312EB1BE8"), "One Handed Sword Bonus Base Damage", string.Empty);

    /// <summary>
    /// Gets the is a two handed sword equipped.
    /// </summary>
    public static AttributeDefinition IsTwoHandedSwordEquipped { get; } = new(new Guid("15530767-556E-45F6-8C2D-9AC71FC4070F"), "Is Two Handed Sword Equipped", string.Empty);

    /// <summary>
    /// Gets the two handed sword bonus base damage.
    /// </summary>
    public static AttributeDefinition TwoHandedSwordBonusBaseDamage { get; } = new(new Guid("C77F9C38-336B-43AE-8B82-A8D73612E9D2"), "Two Handed Sword Bonus Base Damage", string.Empty);

    /// <summary>
    /// Gets the is a mace equipped.
    /// </summary>
    public static AttributeDefinition IsMaceEquipped { get; } = new(new Guid("47BD42D1-2E27-407B-92B5-925F418C30DE"), "Is Mace Equipped", string.Empty);

    /// <summary>
    /// Gets the mace bonus base damage.
    /// </summary>
    public static AttributeDefinition MaceBonusBaseDamage { get; } = new(new Guid("E7F2A75F-E8F5-4877-A0D8-59D59041A973"), "Mace Bonus Base Damage", string.Empty);

    /// <summary>
    /// Gets the is a spear equipped.
    /// </summary>
    public static AttributeDefinition IsSpearEquipped { get; } = new(new Guid("5CBBD700-B897-4425-AADE-FDE249318505"), "Is Spear Equipped", string.Empty);

    /// <summary>
    /// Gets the spear bonus base damage.
    /// </summary>
    public static AttributeDefinition SpearBonusBaseDamage { get; } = new(new Guid("207DA941-CDDC-4231-A82C-B76A2441B77A"), "Spear Bonus Base Damage", string.Empty);

    /// <summary>
    /// Gets the is a one handed staff equipped.
    /// </summary>
    public static AttributeDefinition IsOneHandedStaffEquipped { get; } = new(new Guid("C452EFD8-9FFE-4015-BE93-8BC8D5D81572"), "Is One Handed Staff Equipped", string.Empty);

    /// <summary>
    /// Gets the one handed staff bonus base damage.
    /// </summary>
    public static AttributeDefinition OneHandedStaffBonusBaseDamage { get; } = new(new Guid("12F2F553-DA04-432F-83E0-28F30C1B93A2"), "One Handed Staff Bonus Base Damage", string.Empty);

    /// <summary>
    /// Gets the is a two handed staff equipped.
    /// </summary>
    public static AttributeDefinition IsTwoHandedStaffEquipped { get; } = new(new Guid("FE669B70-D36D-4F56-A1DA-4A4B98ED503D"), "Is Two Handed Staff Equipped", string.Empty);

    /// <summary>
    /// Gets the two handed staff bonus base damage.
    /// </summary>
    public static AttributeDefinition TwoHandedStaffBonusBaseDamage { get; } = new(new Guid("8A46D105-4B58-4E42-B602-52715F6BEE61"), "Two Handed Staff Bonus Base Damage", string.Empty);

    /// <summary>
    /// Gets the is a stick equipped.
    /// </summary>
    public static AttributeDefinition IsStickEquipped { get; } = new(new Guid("85C46193-8EC9-4CCE-B18B-F79E938FE9C3"), "Is Stick Equipped", string.Empty);

    /// <summary>
    /// Gets the stick bonus base damage.
    /// </summary>
    public static AttributeDefinition StickBonusBaseDamage { get; } = new(new Guid("0EF0D78B-9518-445B-BE5A-469962AF7C9D"), "Stick Bonus Base Damage", string.Empty);

    /// <summary>
    /// Gets the is a scepter equipped.
    /// </summary>
    public static AttributeDefinition IsScepterEquipped { get; } = new(new Guid("8650DE3B-BE11-458E-B352-4046CE264402"), "Is Scepter Equipped", string.Empty);

    /// <summary>
    /// Gets the scepter bonus base damage.
    /// </summary>
    public static AttributeDefinition ScepterBonusBaseDamage { get; } = new(new Guid("4E4373A5-F5DB-4D06-B5C1-EE798B58017E"), "Scepter Bonus Base Damage", string.Empty);

    /// <summary>
    /// Gets the scepter pet bonus base damage.
    /// </summary>
    public static AttributeDefinition ScepterPetBonusBaseDamage { get; } = new(new Guid("5706451D-6A73-4462-B559-31C7F847994E"), "Scepter Pet Bonus Base Damage", string.Empty);

    /// <summary>
    /// Gets the is a horse equipped.
    /// </summary>
    public static AttributeDefinition IsHorseEquipped { get; } = new(new Guid("4CDC3CCC-9266-4EEE-9C91-0E6AAFA8B636"), "Is Horse equipped", string.Empty);

    /// <summary>
    /// Gets the is a glove weapon equipped.
    /// </summary>
    public static AttributeDefinition IsGloveWeaponEquipped { get; } = new(new Guid("BC8FFFCA-A593-4B64-BE37-BF7EDC97CA1E"), "Is Glove Weapon Equipped", string.Empty);

    /// <summary>
    /// Gets the glove weapon bonus base damage.
    /// </summary>
    public static AttributeDefinition GloveWeaponBonusBaseDamage { get; } = new(new Guid("F9CB1174-0184-4F21-B71D-AFC7359BC594"), "Glove Weapon Bonus Base Damage", string.Empty);

    /// <summary>
    /// The summoned monster health increase, percentage.
    /// </summary>
    public static AttributeDefinition SummonedMonsterHealthIncrease { get; } = new(new Guid("7B0625C8-DA1A-4A5D-BCA5-26AACDA0BDC6"), "Summoned Monster Health Increase %", string.Empty);

    /// <summary>
    /// The summoned monster defense increase, absolute.
    /// </summary>
    public static AttributeDefinition SummonedMonsterDefenseIncrease { get; } = new(new Guid("0D55CFCA-751F-4E66-B327-635576A9A0B3"), "Summoned Monster Defense Increase", string.Empty);

    /// <summary>
    /// Gets the combo bonus attribute definition.
    /// </summary>
    public static AttributeDefinition ComboBonus { get; } = new (new Guid("53A479FE-8A73-4A45-AACA-5B1AA4362CF9"), "Combo Bonus", string.Empty);

    /// <summary>
    /// Gets the attribute if skill combos are available.
    /// </summary>
    public static AttributeDefinition IsSkillComboAvailable { get; } = new (new Guid("0B648F95-E9C1-4AFD-90A6-3DD954BF6995"), "Is Skill Combo Available", string.Empty);

    /// <summary>
    /// Gets the attribute if the 'Gain Hero' quest was completed.
    /// </summary>
    public static AttributeDefinition GainHeroStatusQuestCompleted { get; } = new (new Guid("4A847231-171B-4FE2-A203-009CB4A26227"), "Is 'Gain Hero Status' Quest completed?", string.Empty);

    /// <summary>
    /// Gets the final damage increase PVP attribute definition.
    /// </summary>
    public static AttributeDefinition FinalDamageIncreasePvp { get; } = new (new Guid("20BE9BFA-A2DC-4868-8ABF-B6DE4B51D4D2"), "Final Damage Increase (PvP)", string.Empty);

    /// <summary>
    /// Gets the defense base attribute definition.
    /// </summary>
    public static AttributeDefinition DefenseBase { get; } = new (new Guid("EB098C46-60D4-4CA6-BBD4-5B6270A1407B"), "Base Defense", string.Empty);

    /// <summary>
    /// Gets the defense PVM attribute definition.
    /// </summary>
    public static AttributeDefinition DefensePvm { get; } = new (new Guid("B4201610-2824-4EC1-A145-76B15DB9DEC6"), "Defense (PvM)", string.Empty);

    /// <summary>
    /// Gets the defense PVP attribute definition.
    /// </summary>
    public static AttributeDefinition DefensePvp { get; } = new (new Guid("28D14EB7-1049-45BE-A7B7-D5E28E63943B"), "Defense (PvP)", string.Empty);

    /// <summary>
    /// Gets the defense rate PVM attribute definition.
    /// </summary>
    public static AttributeDefinition DefenseRatePvm { get; } = new (new Guid("C520DD2D-1B06-4392-95EE-3C41F33E68DA"), "Defense Rate (PvM)", string.Empty);

    /// <summary>
    /// Gets the defense rate PVP attribute definition.
    /// </summary>
    public static AttributeDefinition DefenseRatePvp { get; } = new (new Guid("B995C627-C17B-4D24-9FA5-3830AACC6912"), "Defense Rate (PvP)", string.Empty);

    /// <summary>
    /// Gets the damage receive decrement attribute definition.
    /// </summary>
    public static AttributeDefinition DamageReceiveDecrement { get; } = new (new Guid("9D9761EF-EF47-4E5C-8106-EBC555786F20"), "Damage Receive Multiplier", string.Empty);

    /// <summary>
    /// Gets the shield block damage decrement attribute definition.
    /// TODO: Usage in a shield skill handler.
    /// </summary>
    public static AttributeDefinition ShieldBlockDamageDecrement { get; } = new (new Guid("DAC6690B-5922-4446-BCE5-5E701BE62EC1"), "Shield Block Damage Multiplier", string.Empty);

    /// <summary>
    /// Gets the defense increase with equipped shield attribute definition.
    /// </summary>
    public static AttributeDefinition DefenseIncreaseWithEquippedShield { get; } = new (new Guid("41BCEC8D-A7A8-4930-AB2E-A07D8BF1B86C"), "Defense Increase Multiplier With Equipped Shield", string.Empty);

    /// <summary>
    /// Gets the bonus defense (absolute) with an equipped shield attribute definition.
    /// </summary>
    public static AttributeDefinition BonusDefenseWithShield { get; } = new(new Guid("05F08D89-9BC6-4164-9B30-26EFAF4C0E0F"), "Defense Increase Bonus (absolute) With equipped Shield", string.Empty);

    /// <summary>
    /// Gets the bonus defense (absolute) with an equipped scepter attribute definition.
    /// </summary>
    public static AttributeDefinition BonusDefenseWithScepter { get; } = new(new Guid("7977FE44-FC22-4A6B-A4E0-BA522FE807DF"), "Defense Increase Bonus (absolute) with equipped Scepter", string.Empty);

    /// <summary>
    /// Gets the bonus defense (absolute) command/leadership divisor with an equipped shield attribute definition.
    /// </summary>
    public static AttributeDefinition BonusDefenseWithScepterCmdDiv { get; } = new(new Guid("9A3C99C4-4F94-4CD0-8BFC-C8B870CD5FE4"), "Defense Increase Bonus (absolute) with equipped Scepter", string.Empty);

    /// <summary>
    /// Gets the bonus defense (absolute) with an equipped dark horse attribute definition.
    /// </summary>
    public static AttributeDefinition BonusDefenseWithHorse { get; } = new(new Guid("8D22E36C-EB36-47DB-8CE0-9ABD599C533C"), "Defense Increase Bonus (absolute) With equipped Horse", string.Empty);

    /// <summary>
    /// Gets the 'is shield equipped' attribute definition.
    /// </summary>
    public static AttributeDefinition IsShieldEquipped { get; } = new (new Guid("394DFAA0-B18D-44DA-A99D-094BC5E7C9C5"), "Is Shield Equipped", string.Empty);

    /// <summary>
    /// Gets the attribute definition, which defines if a player has ice effect applied.
    /// </summary>
    public static AttributeDefinition IsIced { get; } = new (new Guid("4A0266F4-B15F-48D7-A9BB-7DCB657124D0"), "Is iced", "The player can only move slowly");

    /// <summary>
    /// Gets the attribute definition, which defines if a player has frozen effect applied.
    /// </summary>
    public static AttributeDefinition IsFrozen { get; } = new (new Guid("18B8933A-28C3-4B79-9350-FFE5F648B754"), "Is frozen", "The player can't move because he is frozen (ice arrow).");

    /// <summary>
    /// Gets the attribute definition, which defines if a player has poison effect applied.
    /// </summary>
    public static AttributeDefinition IsPoisoned { get; } = new (new Guid("F8970D12-069B-4A18-8E18-A9296B85B4ED"), "Is poisoned", "The player is poisoned and loses health");

    /// <summary>
    /// Gets the attribute definition, which defines if a player has stun effect applied.
    /// </summary>
    public static AttributeDefinition IsStunned { get; } = new (new Guid("22C86BAF-7F27-478D-8075-E4465C2859DD"), "Is stunned", "The player is stunned and can't move.");

    /// <summary>
    /// Gets the attribute definition, which defines if a player has asleep effect applied.
    /// </summary>
    public static AttributeDefinition IsAsleep { get; } = new(new Guid("0518F532-7A8F-4491-8A23-98B620608CB3"), "Is asleep", "The player is asleep and can't move until hit.");

    /// <summary>
    /// Gets the ice resistance attribute definition. Value range from 0 to 1.
    /// </summary>
    public static AttributeDefinition IceResistance { get; } = new (new Guid("47235C36-41BB-44B4-8823-6FC415709F59"), "Ice Resistance", string.Empty);

    /// <summary>
    /// Gets the fire resistance attribute definition. Value range from 0 to 1.
    /// </summary>
    public static AttributeDefinition FireResistance { get; } = new (new Guid("9AE4D80D-5706-48B9-AD11-EAC4FE088A81"), "Fire Resistance", string.Empty);

    /// <summary>
    /// Gets the water resistance attribute definition. Value range from 0 to 1.
    /// </summary>
    public static AttributeDefinition WaterResistance { get; } = new (new Guid("3AF88672-D8DB-44E1-937A-7E6484134C39"), "Water Resistance", string.Empty);

    /// <summary>
    /// Gets the earth resistance attribute definition. Value range from 0 to 1.
    /// </summary>
    public static AttributeDefinition EarthResistance { get; } = new (new Guid("4470890F-00CE-44A6-BADB-203684B6014D"), "Earth Resistance", string.Empty);

    /// <summary>
    /// Gets the wind resistance attribute definition. Value range from 0 to 1.
    /// </summary>
    public static AttributeDefinition WindResistance { get; } = new (new Guid("03A29C46-7B7E-424D-8325-8390692570C3"), "Wind Resistance", string.Empty);

    /// <summary>
    /// Gets the poison resistance attribute definition. Value range from 0 to 1.
    /// </summary>
    public static AttributeDefinition PoisonResistance { get; } = new (new Guid("3D50D0B7-63A2-4DA9-8855-12173EAE6B39"), "Poison Resistance", string.Empty);

    /// <summary>
    /// Gets the lightning resistance attribute definition. Value range from 0 to 1.
    /// </summary>
    public static AttributeDefinition LightningResistance { get; } = new (new Guid("3E339393-2D17-452E-81D9-3987947A407F"), "Lightning Resistance", string.Empty);

    /// <summary>
    /// Gets the damage reflection attribute definition.
    /// </summary>
    public static AttributeDefinition DamageReflection { get; } = new (new Guid("1535A2FB-6094-48B8-8578-086BA166A0F7"), "Damage Reflection", string.Empty);

    /// <summary>
    /// Gets the mana recovery attribute definition.
    /// </summary>
    public static AttributeDefinition ManaRecoveryMultiplier { get; } = new (new Guid("E4EC7913-5004-48FC-ACB1-E1764237A251"), "Mana Recovery Multiplier", string.Empty);

    /// <summary>
    /// Gets the health recovery attribute definition.
    /// </summary>
    public static AttributeDefinition HealthRecoveryMultiplier { get; } = new (new Guid("0A427A13-3708-4125-BA83-A2DF7C0753B8"), "Health Recovery Multiplier", string.Empty);

    /// <summary>
    /// Gets the ability recovery attribute definition.
    /// </summary>
    public static AttributeDefinition AbilityRecoveryMultiplier { get; } = new (new Guid("A3E274F5-FA74-4E6A-97EA-D0930AAF0374"), "Ability Recovery Multiplier", string.Empty);

    /// <summary>
    /// Gets the shield recovery attribute definition.
    /// </summary>
    public static AttributeDefinition ShieldRecoveryMultiplier { get; } = new (new Guid("6B99AA99-C1A3-413B-8C70-602567EB5163"), "Shield Recovery Multiplier", string.Empty);

    /// <summary>
    /// Gets the poison damage multiplier attribute definition.
    /// </summary>
    public static AttributeDefinition PoisonDamageMultiplier { get; } = new (new Guid("8581CD4D-C6AE-4C35-9147-9642DE7CC013"), "Poison Damage Multiplier", string.Empty);

    /// <summary>
    /// Gets the mana recovery absolute attribute definition.
    /// </summary>
    public static AttributeDefinition ManaRecoveryAbsolute { get; } = new (new Guid("33DE588D-1FAB-493A-8FB1-837BF9C5131F"), "Mana Recovery Absolute Increase", string.Empty);

    /// <summary>
    /// Gets the health recovery absolute attribute definition.
    /// </summary>
    public static AttributeDefinition HealthRecoveryAbsolute { get; } = new (new Guid("CBC4AB00-FD01-44D4-823A-D04B5E208AA0"), "Health Recovery Absolute Increase", string.Empty);

    /// <summary>
    /// Gets the ability recovery absolute attribute definition.
    /// </summary>
    public static AttributeDefinition AbilityRecoveryAbsolute { get; } = new (new Guid("19A76D11-B7AA-4C1E-885C-2B7E29071E3F"), "Ability Recovery Absolute Increase", string.Empty);

    /// <summary>
    /// Gets the shield recovery absolute attribute definition.
    /// </summary>
    public static AttributeDefinition ShieldRecoveryAbsolute { get; } = new (new Guid("323F479D-3205-4E24-B9BC-0A3EFC851EDF"), "Shield Recovery Absolute", string.Empty);

    /// <summary>
    /// Gets the shield recovery everywhere attribute definition.
    /// By default, shield recovery is limited to the safezone only. With this attribute (value >= 1), recovery works everywhere on a map.
    /// </summary>
    public static AttributeDefinition ShieldRecoveryEverywhere { get; } = new (new Guid("3D0A78FF-CCD4-442E-8B4E-64E5082ABD78"), "Is Shield Recovery Active Everwhere", "By default, shield recovery is limited to the safezone only. With this attribute (value >= 1), recovery works everywhere on a map.");

    /// <summary>
    /// Gets the ability usage reduction attribute definition. Value ranges from 0 (no reduction) to 1 (full reduction).
    /// </summary>
    public static AttributeDefinition AbilityUsageReduction { get; } = new (new Guid("5A712BEF-EA3C-404A-90DB-0FC37CDC65D4"), "Ability Usage Reduction", "Factor (0~1) which describes how much of required ability of a skill is not consumed.");

    /// <summary>
    /// Gets the mana usage reduction attribute definition. Value ranges from 0 (no reduction) to 1 (full reduction).
    /// </summary>
    public static AttributeDefinition ManaUsageReduction { get; } = new (new Guid("95548E17-F00E-4B22-A1AE-4CBB0C84C7D1"), "Mana Usage Reduction", "Factor (0~1) which describes how much of required mana of a skill is not consumed.");

    /// <summary>
    /// Gets the critical damage chance attribute definition.
    /// </summary>
    public static AttributeDefinition CriticalDamageChance { get; } = new (new Guid("ADE8092E-870F-4968-B707-8BFD6CBF8FFC"), "Critical Damage Chance", string.Empty);

    /// <summary>
    /// Gets the excellent damage chance attribute definition.
    /// </summary>
    public static AttributeDefinition ExcellentDamageChance { get; } = new (new Guid("577AE8D4-D9E5-4A1F-9A69-45D1A2519EEE"), "Excellent Damage Chance", string.Empty);

    /// <summary>
    /// Gets the defense ignore chance attribute definition.
    /// </summary>
    public static AttributeDefinition DefenseIgnoreChance { get; } = new (new Guid("56BDC72E-9D37-4EDA-8AC8-9E12473966FC"), "Defense Ignore Chance", string.Empty);

    /// <summary>
    /// Gets the shield bypass chance attribute definition.
    /// </summary>
    public static AttributeDefinition ShieldBypassChance { get; } = new (new Guid("7F6F1804-271E-40DF-A970-0CE19064A54B"), "Shield Bypass Chance", string.Empty);

    /// <summary>
    /// Gets the double damage chance attribute definition.
    /// </summary>
    public static AttributeDefinition DoubleDamageChance { get; } = new (new Guid("2B8A03E6-1CC2-48A0-8633-3F36E17050F4"), "Double Damage Chance", string.Empty);

    /// <summary>
    /// Gets the mana after monster kill attribute definition.
    /// </summary>
    public static AttributeDefinition ManaAfterMonsterKillMultiplier { get; } = new (new Guid("3DE9DEE5-C717-456B-8E94-6C224553674F"), "Mana recover after Monster kill, multiplier of max mana", string.Empty);

    /// <summary>
    /// Gets the health after monster kill attribute definition.
    /// </summary>
    public static AttributeDefinition HealthAfterMonsterKillMultiplier { get; } = new (new Guid("0498AA9E-A4BB-4DE5-B112-58D921101899"), "Health recover after Monster kill, multiplier of max health", string.Empty);

    /// <summary>
    /// Gets the shield after monster kill attribute definition.
    /// </summary>
    public static AttributeDefinition ShieldAfterMonsterKillMultiplier { get; } = new (new Guid("783F0178-C23C-4F20-BE10-B73D3D31D4F6"), "Shield recover after Monster kill, multiplier of max shield", string.Empty);

    /// <summary>
    /// Gets the ability after monster kill attribute definition.
    /// </summary>
    public static AttributeDefinition AbilityAfterMonsterKillMultiplier { get; } = new (new Guid("47433CD1-C7D5-4BF5-AC52-E0F33BF90504"), "Ability recover after Monster kill, multiplier of max ability", string.Empty);

    /// <summary>
    /// Gets the mana after monster kill attribute definition.
    /// </summary>
    public static AttributeDefinition ManaAfterMonsterKillAbsolute { get; } = new (new Guid("C8491372-FF87-41DF-A89E-0B912F0EA8F5"), "Mana recover after Monster kill, absolute", string.Empty);

    /// <summary>
    /// Gets the health after monster kill attribute definition.
    /// </summary>
    public static AttributeDefinition HealthAfterMonsterKillAbsolute { get; } = new (new Guid("7F56242C-B060-40EC-B250-85CF933A22ED"), "Health recover after Monster kill, absolute", string.Empty);

    /// <summary>
    /// Gets the shield after monster kill attribute definition.
    /// </summary>
    public static AttributeDefinition ShieldAfterMonsterKillAbsolute { get; } = new (new Guid("D2FC4D9C-3C1B-4555-AE5F-8EE90728DBF4"), "Shield recover after Monster kill, absolute", string.Empty);

    /// <summary>
    /// Gets the ability after monster kill attribute definition.
    /// </summary>
    public static AttributeDefinition AbilityAfterMonsterKillAbsolute { get; } = new (new Guid("477E1363-3087-4B8E-AAAE-153B2C2746CB"), "Ability recover after Monster kill, absolute", string.Empty);

    /// <summary>
    /// Gets the shield decrease rate increase attribute definition.
    /// </summary>
    public static AttributeDefinition ShieldDecreaseRateIncrease { get; } = new (new Guid("A0F376C9-C9C6-4E0D-83D6-24EC005E282E"), "SD decrease rate increase", "The damage absorbed by the opponents' shield in PVP directly translates into HP damange according to set value.");

    /// <summary>
    /// Gets the shield rate increase attribute definition.
    /// </summary>
    public static AttributeDefinition ShieldRateIncrease { get; } = new (new Guid("C2FFBA46-4DC3-4861-84F5-313DED038997"), "SD Rate Increase", "Increases the shield’s damage absorb rate in PVP according to set value.");

    /// <summary>
    /// Gets the item duration increase attribute definition.
    /// </summary>
    public static AttributeDefinition ItemDurationIncrease { get; } = new (new Guid("03EBE702-90FF-473C-8CBB-E83669FE3C68"), "Item Duration Increase", string.Empty);

    /// <summary>
    /// Gets the pet duration increase attribute definition.
    /// </summary>
    public static AttributeDefinition PetDurationIncrease { get; } = new (new Guid("B4455150-D3A9-4A5F-914B-F41F9387FE9A"), "Pet Duraction Increase", string.Empty);

    /// <summary>
    /// Gets the horse level attribute definition.
    /// </summary>
    public static AttributeDefinition HorseLevel { get; } = new(new Guid("9AF1F91F-6093-4CDE-BEDA-A118F10F2E29"), "Dark Horse Level", string.Empty);

    /// <summary>
    /// Gets the raven level attribute definition.
    /// </summary>
    public static AttributeDefinition RavenLevel { get; } = new(new Guid("2C9AA85C-AB8B-4E0F-8F0B-BC6E49EE134E"), "Dark Raven Level", string.Empty);

    /// <summary>
    /// Gets the raven base damage definition.
    /// </summary>
    public static AttributeDefinition RavenBaseDamage { get; } = new(new Guid("3F77AD23-3833-4F2F-A264-23F7A4688FDA"), "Min and Max damage of the dark raven.", string.Empty);

    /// <summary>
    /// Gets the raven minimum damage definition.
    /// </summary>
    public static AttributeDefinition RavenMinimumDamage { get; } = new(new Guid("D03B0A96-8233-42A5-9EA1-D92D5F91CE13"), "Minimum damage of the dark raven.", string.Empty);

    /// <summary>
    /// Gets the raven maximum damage definition.
    /// </summary>
    public static AttributeDefinition RavenMaximumDamage { get; } = new(new Guid("9F427253-7D5E-4766-BDD7-A8D7AE9DC715"), "Maximum damage of the dark raven.", string.Empty);

    /// <summary>
    /// Gets the raven attack rate definition.
    /// </summary>
    public static AttributeDefinition RavenAttackRate { get; } = new(new Guid("740634FF-9DA3-4642-807F-89A30F3DC5D6"), "Attack rate of the dark raven.", string.Empty);

    /// <summary>
    /// Gets the raven attack speed definition.
    /// </summary>
    public static AttributeDefinition RavenAttackSpeed { get; } = new(new Guid("3359F7E9-936C-48DD-BB0A-E44E2347D3CA"), "Attack speed of the dark raven.", string.Empty);

    /// <value>
    /// Gets the raven critical damage chance bonus.
    /// </value>
    public static AttributeDefinition RavenCriticalDamageChanceBonus { get; } = new(new Guid("D9C8D708-F94A-430E-B812-99A2F3F0768E"), "Raven critical damage chance bonus", string.Empty);

    /// <value>
    /// Gets the raven excellent damage chance bonus.
    /// </value>
    public static AttributeDefinition RavenExcDamageChanceBonus { get; } = new(new Guid("31EF09B0-0F79-439B-900C-C9350BAD99DF"), "Raven exc damage chance bonus", string.Empty);

    /// <summary>
    /// Gets the maximum guild size attribute definition.
    /// </summary>
    public static AttributeDefinition MaximumGuildSize { get; } = new (new Guid("898EF69B-3965-4DBF-9783-E9709698236B"), "Maximum Guild Size", string.Empty);

    /// <summary>
    /// Gets the fully recover mana after hit chance definition.
    /// </summary>
    public static AttributeDefinition FullyRecoverManaAfterHitChance { get; } = new (new Guid("EB06D3A2-DA82-4B81-81B9-A16D39974531"), "Chance to fully recover mana when getting hit", "3rd Wing Option");

    /// <summary>
    /// Gets the fully recover health after hit chance definition.
    /// </summary>
    public static AttributeDefinition FullyRecoverHealthAfterHitChance { get; } = new (new Guid("3CA72C07-9C2C-4FC5-8BCB-9BD737F83664"), "Chance to fully recover health when getting hit", "3rd Wing Option");

    /// <summary>
    /// Gets the health loss after hit definition.
    /// </summary>
    public static AttributeDefinition HealthLossAfterHit { get; } = new (new Guid("D84A719B-D18E-433E-BF55-9F08A214AB00"), "Health loss after hitting a target", "Caused by wearing wings");

    /// <summary>
    /// Gets the mana loss after hit definition.
    /// </summary>
    public static AttributeDefinition ManaLossAfterHit { get; } = new(new Guid("321B9B7D-EF8C-45FC-9CA7-F511FC7D802B"), "Mana loss after hitting a target", "Caused by infinity arrow effect");

    /// <summary>
    /// Gets the CanFly attribute for warping to icarus.
    /// </summary>
    public static AttributeDefinition CanFly { get; } = new (new Guid("EC34C673-84DE-4811-8962-CD2164A2248C"), "Requirement of the Icarus map.", "You can enter Icarus only with wings, dinorant, fenrir.");

    /// <summary>
    /// Gets the MoonstonePendantEquipped attribute for warping to Karutan.
    /// </summary>
    public static AttributeDefinition MoonstonePendantEquipped { get; } = new (new Guid("4BC010D0-9E75-4ECB-8963-08A3697278C3"), "Requirement of the Kanturu Event Map during the event.", "You can enter the Kanturu Event only with an equipped Moonstone Pendant.");

    /// <summary>
    /// Gets the Ammo consumption rate attribute which defines how much ammo is consumed by some skills.
    /// </summary>
    public static AttributeDefinition AmmunitionConsumptionRate { get; } = new (new Guid("4CD0B1AE-3FE0-499B-B421-4E4078182090"), "The amount of ammo which is required to perform certain skills.", "You can only execute certain skills if you have enough ammo, or if this rate is 0.");

    /// <summary>
    /// Gets the Ammo attribute which defines how much ammo is available.
    /// </summary>
    public static AttributeDefinition AmmunitionAmount { get; } = new (new Guid("064543E6-2559-4033-B363-AE76214E7DEE"), "The amount of ammo which is equipped.", "You can only execute certain skills if you have enough ammo, or if the ammo consumption rate is 0.");

    /// <summary>
    /// Gets the <see cref="IsInSafezone"/> attribute which defines if the character is located in a safezone of a game map.
    /// </summary>
    public static AttributeDefinition IsInSafezone { get; } = new (new Guid("82044DF9-F528-4AD6-9AAA-6FEAA4C786E7"), "Flag, if the character is located in a safezone of a game map", "Characters at the safezone recover additional health and shield.");

    /// <summary>
    /// Gets the attribute definition, which defines if a player has MU Helper activated.
    /// </summary>
    public static AttributeDefinition IsMuHelperActive { get; } = new(new Guid("1FBD3CC0-DFDC-4A19-9B73-5B2DC0E12983"), "Is MU Helper active", "");

    /// <summary>
    /// Gets the <see cref="TransformationSkin"/> attribute which defines if and how the character is skinned as a monster.
    /// This value is > 0, when the character got skinned as a monster, by wearing a transformation ring. The value specifies the type of monster (skin).
    /// </summary>
    public static AttributeDefinition TransformationSkin { get; } = new (new Guid("E5B886B0-B1A6-4EA2-8EF9-08D27AADB7C3"), "Character to Monster transformation", "This value is > 0, when the character got transformed into a monster, by wearing a transformation ring. The value specifies the type of monster (skin).");

    /// <summary>
    /// Gets the <see cref="IsInvisible"/> attribute which defines if the player is invisible.
    /// This value is > 0, when the character is either a game master which used the hide-command, or when a player is spectating a duel.
    /// </summary>
    public static AttributeDefinition IsInvisible { get; } = new(new Guid("8B0721BC-7AC6-4677-B488-ED4319AE9A56"), "Is invisible", "This value is > 0, when the character is either a game master which used the hide-command, or when a player is spectating a duel.");

    /// <summary>
    /// Gets the attribute for a strength requirement reduction. Items with this option require less strength, according to the option's value.
    /// </summary>
    /// <remarks>
    /// This has no effect on the player itself, just on the requirement of an item.
    /// </remarks>
    public static AttributeDefinition RequiredStrengthReduction { get; } = new (new Guid("60A6B398-24BC-4C2A-8A0B-695D6FA11349"), "Strength Requirement reduction", "Items with this option require less strength, according to the option's value.");

    /// <summary>
    /// Gets the attribute for a agility requirement reduction. Items with this option require less agility, according to the option's value.
    /// </summary>
    /// <remarks>
    /// This has no effect on the player itself, just on the requirement of an item.
    /// </remarks>
    public static AttributeDefinition RequiredAgilityReduction { get; } = new (new Guid("2FE2CC3B-07E4-4D5F-BD89-C1928D4DE5C3"), "Agility Requirement reduction", "Items with this option require less agility, according to the option's value.");

    /// <summary>
    /// Gets the attribute for a energy requirement reduction. Items with this option require less energy, according to the option's value.
    /// </summary>
    /// <remarks>
    /// This has no effect on the player itself, just on the requirement of an item.
    /// </remarks>
    public static AttributeDefinition RequiredEnergyReduction { get; } = new (new Guid("F0CB6861-DFDA-43CC-AB4F-677636C83A77"), "Energy Requirement reduction", "Items with this option require less energy, according to the option's value.");

    /// <summary>
    /// Gets the attribute for a vitality requirement reduction. Items with this option require less vitality, according to the option's value.
    /// </summary>
    /// <remarks>
    /// This has no effect on the player itself, just on the requirement of an item.
    /// </remarks>
    public static AttributeDefinition RequiredVitalityReduction { get; } = new (new Guid("72D13C56-8CE7-45CA-A78A-3B5995E724C7"), "Vitality Requirement reduction", "Items with this option require less vitality, according to the option's value.");

    /// <summary>
    /// Gets the attribute for a leadership requirement reduction. Items with this option require less leadership, according to the option's value.
    /// </summary>
    /// <remarks>
    /// This has no effect on the player itself, just on the requirement of an item.
    /// </remarks>
    public static AttributeDefinition RequiredLeadershipReduction { get; } = new (new Guid("8E6244B8-8E88-4413-B18E-0D2812830DFE"), "Leadership Requirement reduction", "Items with this option require less leadership, according to the option's value.");

    /// <summary>
    /// Gets the attribute for the nova bonus damage which depends on other stats.
    /// </summary>
    public static AttributeDefinition NovaBonusDamage { get; } = new(new Guid("0ABE8432-7F23-4CE9-BBCC-B8A633DAC08B"), "Nova Bonus Damage", "The nova bonus bonus damage, which depends on other stats.");

    /// <summary>
    /// Gets the attribute for the nova stage damage which depends on the duration of the skill.
    /// </summary>
    public static AttributeDefinition NovaStageDamage { get; } = new(new Guid("9185A46A-4C56-4FF1-A0D2-C0CD58CB17FB"), "Nova Stage Damage", "The currently reached nova stage bonus which depends on the duration of the skill.");

    /// <summary>
    /// Gets the attribute for the VIP flag.
    /// </summary>
    public static AttributeDefinition IsVip { get; } = new(new Guid("195474D6-59A2-4033-9C30-8628ECC0097E"), "Is VIP", "The flag, if an account is a VIP.");

    /// <summary>
    /// Gets the attribute for the number of points this class will receive for reset, overwrites the default <see cref="Resets.ResetConfiguration.PointsPerReset"/> value.
    /// </summary>
    public static AttributeDefinition PointsPerReset { get; } = new(new Guid("a34f4f57-b364-4cdb-9989-64cedd2cd831"), "Points Per Reset", "The number of points the player will receive for reset, overwrites the default 'PointsPerReset' value of the reset configuration.");

    /// <summary>
    /// Gets the attributes which are regenerated in an interval.
    /// </summary>
    public static IEnumerable<Regeneration> IntervalRegenerationAttributes
    {
        get
        {
            yield return ManaRegeneration;
            yield return HealthRegeneration;
            yield return AbilityRegeneration;
            yield return ShieldRegeneration;
        }
    }

    /// <summary>
    /// Gets the attributes which regenerate after a monster has been killed by the player.
    /// </summary>
    public static IEnumerable<Regeneration> AfterMonsterKillRegenerationAttributes
    {
        get
        {
            yield return ManaRegenerationAfterMonsterKill;
            yield return HealthRegenerationAfterMonsterKill;
            yield return AbilityRegenerationAfterMonsterKill;
            yield return ShieldRegenerationAfterMonsterKill;
        }
    }

    private static Regeneration ManaRegeneration { get; } = new (ManaRecoveryMultiplier, MaximumMana, CurrentMana, ManaRecoveryAbsolute);

    private static Regeneration HealthRegeneration { get; } = new (HealthRecoveryMultiplier, MaximumHealth, CurrentHealth, HealthRecoveryAbsolute);

    private static Regeneration AbilityRegeneration { get; } = new (AbilityRecoveryMultiplier, MaximumAbility, CurrentAbility, AbilityRecoveryAbsolute);

    private static Regeneration ShieldRegeneration { get; } = new (ShieldRecoveryMultiplier, MaximumShield, CurrentShield, ShieldRecoveryAbsolute);

    private static Regeneration ManaRegenerationAfterMonsterKill { get; } = new (ManaAfterMonsterKillMultiplier, MaximumMana, CurrentMana, ManaAfterMonsterKillAbsolute);

    private static Regeneration HealthRegenerationAfterMonsterKill { get; } = new (HealthAfterMonsterKillMultiplier, MaximumHealth, CurrentHealth, HealthAfterMonsterKillAbsolute);

    private static Regeneration AbilityRegenerationAfterMonsterKill { get; } = new (AbilityAfterMonsterKillMultiplier, MaximumAbility, CurrentAbility, AbilityAfterMonsterKillAbsolute);

    private static Regeneration ShieldRegenerationAfterMonsterKill { get; } = new (ShieldAfterMonsterKillMultiplier, MaximumShield, CurrentShield, ShieldAfterMonsterKillAbsolute);

    /// <summary>
    /// A regeneration definition.
    /// At regeneration the value of <see cref="RegenerationMultiplier"/> * <see cref="MaximumAttribute"/> is getting added to
    /// <see cref="CurrentAttribute"/>, until the value of <see cref="MaximumAttribute"/> is reached.
    /// </summary>
    public class Regeneration
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="Regeneration" /> class.
        /// At regeneration the value of <paramref name="regenerationMultiplier" /> * <paramref name="maximumAttribute" /> is getting added to
        /// <paramref name="currentAttribute" />, until the value of <paramref name="maximumAttribute" /> is reached.
        /// </summary>
        /// <param name="regenerationMultiplier">The regeneration multiplier.</param>
        /// <param name="maximumAttribute">The maximum attribute.</param>
        /// <param name="currentAttribute">The current attribute.</param>
        /// <param name="absoluteAttribute">The constant regeneration.</param>
        public Regeneration(AttributeDefinition regenerationMultiplier, AttributeDefinition maximumAttribute, AttributeDefinition currentAttribute, AttributeDefinition absoluteAttribute)
        {
            this.RegenerationMultiplier = regenerationMultiplier;
            this.MaximumAttribute = maximumAttribute;
            this.CurrentAttribute = currentAttribute;
            this.AbsoluteAttribute = absoluteAttribute;
        }

        /// <summary>
        /// Gets the regeneration multiplier.
        /// </summary>
        public AttributeDefinition RegenerationMultiplier { get; }

        /// <summary>
        /// Gets the absolute regeneration which is always regenerated, independently from the multiplier.
        /// </summary>
        public AttributeDefinition AbsoluteAttribute { get; }

        /// <summary>
        /// Gets the maximum attribute.
        /// </summary>
        public AttributeDefinition MaximumAttribute { get; }

        /// <summary>
        /// Gets the current attribute.
        /// </summary>
        public AttributeDefinition CurrentAttribute { get; }
    }
}