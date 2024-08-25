// <copyright file="AncientSets.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

using MUnique.OpenMU.Network;

namespace MUnique.OpenMU.Persistence.Initialization.VersionSeasonSix.Items;

using MUnique.OpenMU.AttributeSystem;
using MUnique.OpenMU.DataModel.Attributes;
using MUnique.OpenMU.DataModel.Configuration;
using MUnique.OpenMU.DataModel.Configuration.Items;
using MUnique.OpenMU.GameLogic.Attributes;
using MUnique.OpenMU.Persistence.Initialization.Items;

/// <summary>
/// Initialization code for ancient sets.
/// </summary>
public class AncientSets : InitializerBase
{
    private readonly IDictionary<AttributeDefinition, IncreasableItemOption> _bonusOptions = new Dictionary<AttributeDefinition, IncreasableItemOption>();
    private readonly ItemOptionType _ancientBonusOptionType;
    private readonly ItemOptionType _ancientOptionType;

    /// <summary>
    /// Initializes a new instance of the <see cref="AncientSets"/> class.
    /// </summary>
    /// <param name="context">The context.</param>
    /// <param name="gameConfiguration">The game configuration.</param>
    public AncientSets(IContext context, GameConfiguration gameConfiguration)
        : base(context, gameConfiguration)
    {
        this._ancientBonusOptionType = this.GameConfiguration.ItemOptionTypes.First(iot => iot == ItemOptionTypes.AncientBonus);
        this._ancientOptionType = this.GameConfiguration.ItemOptionTypes.First(iot => iot == ItemOptionTypes.AncientOption);
    }

    /// <inheritdoc />
    public override void Initialize()
    {
        var warrior = this.AddAncientSet(
            "Warrior", // Leather
            1,
            (Stats.TotalStrength, 10.0f),
            (Stats.SkillDamageBonus, 10.0f),
            (Stats.MaximumAbility, 20.0f),
            (Stats.AbilityRecoveryAbsolute, 5.0f),
            (Stats.DefenseBase, 20.0f),
            (Stats.TotalAgility, 10.0f),
            (Stats.CriticalDamageChance, 0.05f),
            (Stats.ExcellentDamageChance, 0.05f),
            (Stats.TotalStrength, 25.0f));
        warrior.SetGuid(5, 1);
        this.AddItems(
            warrior,
            (5, ItemGroups.Boots, Stats.TotalVitality, 1),
            (5, ItemGroups.Gloves, Stats.TotalVitality, 1),
            (5, ItemGroups.Helm, Stats.TotalVitality, 1),
            (5, ItemGroups.Pants, Stats.TotalVitality, 1),
            (5, ItemGroups.Armor, Stats.TotalVitality, 1),
            (1, ItemGroups.Scepters, Stats.TotalStrength, 1), // Morning Star
            (8, ItemGroups.Misc1, Stats.TotalAgility, 1)); // Ring of Ice

        var anonymous = this.AddAncientSet(
            "Anonymous", // Leather
            2,
            (Stats.MaximumHealth, 50.0f),
            (Stats.TotalAgility, 50.0f),
            (Stats.ShieldBlockDamageDecrement, 0.25f),
            (Stats.BaseDamageBonus, 30.0f));
        anonymous.SetGuid(5, 2);
        this.AddItems(
            anonymous,
            (5, ItemGroups.Boots, Stats.TotalVitality, 2),
            (5, ItemGroups.Helm, Stats.TotalVitality, 2),
            (5, ItemGroups.Pants, Stats.TotalVitality, 2),
            (0, ItemGroups.Shields, Stats.TotalVitality, 1)); // Small Shield

        var hyperion = this.AddAncientSet(
            "Hyperion", // Bronze
            3,
            (Stats.TotalEnergy, 15.0f),
            (Stats.TotalAgility, 15.0f),
            (Stats.SkillDamageBonus, 20.0f),
            (Stats.MaximumMana, 30.0f));
        hyperion.SetGuid(0, 1);
        this.AddItems(
            hyperion,
            (0, ItemGroups.Boots, Stats.TotalVitality, 1),
            (0, ItemGroups.Pants, Stats.TotalVitality, 1),
            (0, ItemGroups.Armor, Stats.TotalVitality, 1));

        var mist = this.AddAncientSet(
            "Mist", // Bronze
            4,
            (Stats.TotalVitality, 20.0f),
            (Stats.SkillDamageBonus, 30.0f),
            (Stats.DoubleDamageChance, 0.1f),
            (Stats.TotalAgility, 20.0f));
        mist.SetGuid(0, 2);
        this.AddItems(
            mist,
            (0, ItemGroups.Gloves, Stats.TotalVitality, 1),
            (0, ItemGroups.Pants, Stats.TotalVitality, 2),
            (0, ItemGroups.Helm, Stats.TotalVitality, 1));

        var eplete = this.AddAncientSet(
            "Eplete", // Scale
            5,
            (Stats.SkillDamageBonus, 15.0f),
            (Stats.AttackRatePvm, 50.0f),
            (Stats.WizardryAttackDamageIncrease, 0.05f),
            (Stats.MaximumHealth, 50.0f),
            (Stats.MaximumAbility, 30.0f),
            (Stats.CriticalDamageChance, 0.10f),
            (Stats.ExcellentDamageChance, 0.10f));
        eplete.SetGuid(6, 1);
        this.AddItems(
            eplete,
            (6, ItemGroups.Pants, Stats.TotalVitality, 1),
            (6, ItemGroups.Armor, Stats.TotalVitality, 1),
            (6, ItemGroups.Helm, Stats.TotalVitality, 1),
            (9, ItemGroups.Shields, Stats.TotalVitality, 1), // Plate Shield
            (12, ItemGroups.Misc1, Stats.TotalEnergy, 1)); // Pendant of Lightning

        var berserker = this.AddAncientSet(
            "Berserker", // Scale
            6,
            (Stats.MaximumPhysBaseDmg, 10.0f),
            (Stats.MaximumPhysBaseDmg, 20.0f),
            (Stats.MaximumPhysBaseDmg, 30.0f),
            (Stats.MaximumPhysBaseDmg, 40.0f),
            (Stats.SkillDamageBonus, 50.0f),
            (Stats.TotalStrength, 40.0f));
        berserker.SetGuid(6, 2);
        this.AddItems(
            berserker,
            (6, ItemGroups.Pants, Stats.TotalVitality, 2),
            (6, ItemGroups.Armor, Stats.TotalVitality, 2),
            (6, ItemGroups.Helm, Stats.TotalVitality, 2),
            (6, ItemGroups.Gloves, Stats.TotalVitality, 1),
            (6, ItemGroups.Boots, Stats.TotalVitality, 1));

        var garuda = this.AddAncientSet(
            "Garuda", // Brass
            7,
            (Stats.MaximumAbility, 30.0f),
            (Stats.DoubleDamageChance, 0.05f),
            (Stats.TotalEnergy, 15.0f),
            (Stats.MaximumHealth, 50.0f),
            (Stats.SkillDamageBonus, 25.0f),
            (Stats.WizardryAttackDamageIncrease, 0.15f));
        garuda.SetGuid(8, 1);
        this.AddItems(
            garuda,
            (8, ItemGroups.Pants, Stats.TotalVitality, 1),
            (8, ItemGroups.Armor, Stats.TotalVitality, 1),
            (8, ItemGroups.Gloves, Stats.TotalVitality, 1),
            (8, ItemGroups.Boots, Stats.TotalVitality, 1),
            (13, ItemGroups.Misc1, Stats.TotalStrength, 1)); // Pendant of Fire

        var cloud = this.AddAncientSet(
            "Cloud", // Brass
            8,
            (Stats.CriticalDamageChance, 0.20f),
            (Stats.CriticalDamageBonus, 50.0f));
        cloud.SetGuid(8, 2);
        this.AddItems(
            cloud,
            (8, ItemGroups.Pants, Stats.TotalVitality, 2),
            (8, ItemGroups.Helm, Stats.TotalVitality, 1));

        var kantata = this.AddAncientSet(
            "Kantata", // Plate
            9,
            (Stats.TotalEnergy, 15.0f),
            (Stats.TotalVitality, 30.0f),
            (Stats.WizardryAttackDamageIncrease, 0.10f),
            (Stats.TotalStrength, 15.0f),
            (Stats.SkillDamageBonus, 25.0f),
            (Stats.ExcellentDamageChance, 10.0f),
            (Stats.ExcellentDamageBonus, 20.0f));
        kantata.SetGuid(9, 1);
        this.AddItems(
            kantata,
            (9, ItemGroups.Boots, Stats.TotalVitality, 1),
            (9, ItemGroups.Gloves, Stats.TotalVitality, 1),
            (9, ItemGroups.Armor, Stats.TotalVitality, 1),
            (23, ItemGroups.Misc1, Stats.TotalAgility, 1), // Ring of Wind
            (9, ItemGroups.Misc1, Stats.TotalVitality, 1)); // Ring of Poison

        var rave = this.AddAncientSet(
            "Rave", // Plate
            10,
            (Stats.SkillDamageBonus, 20.0f),
            (Stats.DoubleDamageChance, 0.10f),
            (Stats.TwoHandedWeaponDamageIncrease, 0.30f),
            (Stats.DefenseIgnoreChance, 0.05f));
        rave.SetGuid(9, 2);
        this.AddItems(
            rave,
            (9, ItemGroups.Helm, Stats.TotalVitality, 1),
            (9, ItemGroups.Pants, Stats.TotalVitality, 1),
            (9, ItemGroups.Armor, Stats.TotalVitality, 2));

        var hyon = this.AddAncientSet(
            "Hyon", // Dragon
            11,
            (Stats.DefenseBase, 25),
            (Stats.DoubleDamageChance, 0.10f),
            (Stats.SkillDamageBonus, 20),
            (Stats.CriticalDamageChance, 0.15f),
            (Stats.ExcellentDamageChance, 0.15f),
            (Stats.CriticalDamageBonus, 20f),
            (Stats.ExcellentDamageBonus, 20f));
        hyon.SetGuid(1, 1);
        this.AddItems(
            hyon,
            (14, ItemGroups.Swords, Stats.TotalStrength, 1), // Lightning Sword
            (1, ItemGroups.Helm, Stats.TotalVitality, 1),
            (1, ItemGroups.Boots, Stats.TotalVitality, 1),
            (1, ItemGroups.Gloves, Stats.TotalVitality, 1));

        var vicious = this.AddAncientSet(
            "Vicious", // Dragon
            12,
            (Stats.SkillDamageBonus, 15.0f),
            (Stats.BaseDamageBonus, 15.0f),
            (Stats.DoubleDamageChance, 0.10f),
            (Stats.MinimumPhysBaseDmg, 20f),
            (Stats.MaximumPhysBaseDmg, 30f),
            (Stats.DefenseIgnoreChance, 0.05f));
        vicious.SetGuid(1, 2);
        this.AddItems(
            vicious,
            (22, ItemGroups.Misc1, Stats.TotalStrength, 2), // Ring of Earth
            (1, ItemGroups.Helm, Stats.TotalVitality, 2),
            (1, ItemGroups.Pants, Stats.TotalVitality, 2),
            (1, ItemGroups.Armor, Stats.TotalVitality, 2));

        var apollo = this.AddAncientSet(
            "Apollo", // Pad
            13,
            (Stats.TotalEnergy, 10.0f),
            (Stats.WizardryAttackDamageIncrease, 0.05f),
            (Stats.SkillDamageBonus, 10.0f),
            (Stats.MaximumMana, 30.0f),
            (Stats.MaximumHealth, 30.0f),
            (Stats.MaximumAbility, 20.0f),
            (Stats.CriticalDamageChance, 0.10f),
            (Stats.ExcellentDamageChance, 0.10f),
            (Stats.TotalEnergy, 30.0f));
        apollo.SetGuid(2, 1);
        this.AddItems(
            apollo,
            (0, ItemGroups.Staff, Stats.TotalEnergy, 1), // Skull Staff
            (2, ItemGroups.Helm, Stats.TotalVitality, 1),
            (2, ItemGroups.Armor, Stats.TotalVitality, 1),
            (2, ItemGroups.Pants, Stats.TotalVitality, 1),
            (2, ItemGroups.Gloves, Stats.TotalVitality, 1),
            (25, ItemGroups.Misc1, Stats.TotalStrength, 1), // Pendant of Ice
            (24, ItemGroups.Misc1, Stats.BaseEnergy, 1)); // Ring of Magic

        var barnake = this.AddAncientSet(
            "Barnake", // Pad
            14,
            (Stats.WizardryAttackDamageIncrease, 0.10f),
            (Stats.TotalEnergy, 20.0f),
            (Stats.SkillDamageBonus, 30.0f),
            (Stats.MaximumMana, 100.0f));
        barnake.SetGuid(2, 2);
        this.AddItems(
            barnake,
            (2, ItemGroups.Helm, Stats.TotalVitality, 2),
            (2, ItemGroups.Pants, Stats.TotalVitality, 2),
            (2, ItemGroups.Boots, Stats.TotalVitality, 1));

        var evis = this.AddAncientSet(
            "Evis", // Bone
            15,
            (Stats.SkillDamageBonus, 15.0f),
            (Stats.TotalVitality, 20.0f),
            (Stats.WizardryAttackDamageIncrease, 0.10f),
            (Stats.DoubleDamageChance, 0.05f),
            (Stats.AttackRatePvm, 50.0f),
            (Stats.AbilityRecoveryAbsolute, 5.0f));
        evis.SetGuid(4, 1);
        this.AddItems(
            evis,
            (4, ItemGroups.Armor, Stats.TotalVitality, 1),
            (4, ItemGroups.Pants, Stats.TotalVitality, 1),
            (4, ItemGroups.Boots, Stats.TotalVitality, 1),
            (26, ItemGroups.Misc1, Stats.TotalAgility, 1)); // Pendant of Wind

        var sylion = this.AddAncientSet(
            "Sylion", // Bone
            16,
            (Stats.DoubleDamageChance, 0.05f),
            (Stats.CriticalDamageChance, 0.05f),
            (Stats.DefenseBase, 20.0f),
            (Stats.TotalStrength, 50.0f),
            (Stats.TotalAgility, 50.0f),
            (Stats.TotalVitality, 50.0f),
            (Stats.TotalEnergy, 50.0f));
        sylion.SetGuid(4, 2);
        this.AddItems(
            sylion,
            (4, ItemGroups.Armor, Stats.TotalVitality, 2),
            (4, ItemGroups.Gloves, Stats.TotalVitality, 1),
            (4, ItemGroups.Boots, Stats.TotalVitality, 2),
            (4, ItemGroups.Helm, Stats.TotalVitality, 1));

        var heras = this.AddAncientSet(
            "Heras", // Sphinx
            17,
            (Stats.TotalStrength, 15.0f),
            (Stats.WizardryAttackDamageIncrease, 0.10f),
            (Stats.DefenseIncreaseWithEquippedShield, 0.05f),
            (Stats.TotalEnergy, 15.0f),
            (Stats.AttackRatePvm, 50.0f),
            (Stats.CriticalDamageChance, 0.10f),
            (Stats.ExcellentDamageChance, 0.10f),
            (Stats.MaximumHealth, 50.0f),
            (Stats.MaximumMana, 50.0f));
        heras.SetGuid(7, 1);
        this.AddItems(
            heras,
            (6, ItemGroups.Shields, Stats.TotalVitality, 1), // Skull Shield
            (7, ItemGroups.Pants, Stats.TotalVitality, 1),
            (7, ItemGroups.Armor, Stats.TotalVitality, 1),
            (7, ItemGroups.Helm, Stats.TotalVitality, 1),
            (7, ItemGroups.Gloves, Stats.TotalVitality, 1),
            (7, ItemGroups.Boots, Stats.TotalVitality, 1));

        var minet = this.AddAncientSet(
            "Minet", // Sphinx
            18,
            (Stats.TotalEnergy, 30.0f),
            (Stats.DefenseBase, 30.0f),
            (Stats.MaximumMana, 100.0f),
            (Stats.SkillDamageBonus, 15.0f));
        minet.SetGuid(7, 2);
        this.AddItems(
            minet,
            (7, ItemGroups.Pants, Stats.TotalVitality, 2),
            (7, ItemGroups.Armor, Stats.TotalVitality, 2),
            (7, ItemGroups.Boots, Stats.TotalVitality, 2));

        var anubis = this.AddAncientSet(
            "Anubis", // Legendary
            19,
            (Stats.DoubleDamageChance, 0.10f),
            (Stats.MaximumMana, 50.0f),
            (Stats.WizardryAttackDamageIncrease, 0.10f),
            (Stats.CriticalDamageChance, 0.15f),
            (Stats.ExcellentDamageChance, 0.15f),
            (Stats.CriticalDamageBonus, 20.0f),
            (Stats.ExcellentDamageChance, 20.0f));
        anubis.SetGuid(3, 1);
        this.AddItems(
            anubis,
            (3, ItemGroups.Armor, Stats.TotalVitality, 1),
            (3, ItemGroups.Helm, Stats.TotalVitality, 1),
            (3, ItemGroups.Gloves, Stats.TotalVitality, 1),
            (21, ItemGroups.Misc1, Stats.TotalEnergy, 1)); // Ring of Fire

        var enis = this.AddAncientSet(
            "Enis", // Legendary
            20,
            (Stats.SkillDamageBonus, 10.0f),
            (Stats.DoubleDamageChance, 0.10f),
            (Stats.TotalEnergy, 30.0f),
            (Stats.WizardryAttackDamageIncrease, 0.10f),
            (Stats.DefenseIgnoreChance, 0.05f));
        enis.SetGuid(3, 2);
        this.AddItems(
            enis,
            (3, ItemGroups.Armor, Stats.TotalVitality, 2),
            (3, ItemGroups.Helm, Stats.TotalVitality, 2),
            (3, ItemGroups.Boots, Stats.TotalVitality, 2),
            (3, ItemGroups.Pants, Stats.TotalVitality, 2));

        var ceto = this.AddAncientSet(
            "Ceto", // Vine
            21,
            (Stats.TotalAgility, 10.0f),
            (Stats.MaximumHealth, 50.0f),
            (Stats.DefenseBase, 20.0f),
            (Stats.DefenseIncreaseWithEquippedShield, 0.05f),
            (Stats.TotalEnergy, 10.0f),
            (Stats.MaximumHealth, 50.0f),
            (Stats.TotalStrength, 20.0f));
        ceto.SetGuid(10, 1);
        this.AddItems(
            ceto,
            (10, ItemGroups.Boots, Stats.TotalVitality, 1),
            (10, ItemGroups.Gloves, Stats.TotalVitality, 1),
            (10, ItemGroups.Helm, Stats.TotalVitality, 1),
            (10, ItemGroups.Pants, Stats.TotalVitality, 1),
            (2, ItemGroups.Swords, Stats.TotalStrength, 1), // Rapier
            (22, ItemGroups.Misc1, Stats.TotalStrength, 1)); // Ring of Earth

        var drake = this.AddAncientSet(
            "Drake", // Vine
            22,
            (Stats.TotalAgility, 20.0f),
            (Stats.BaseDamageBonus, 25.0f),
            (Stats.DoubleDamageChance, 0.20f),
            (Stats.DefenseBase, 40.0f),
            (Stats.CriticalDamageChance, 0.10f));
        drake.SetGuid(10, 2);
        this.AddItems(
            drake,
            (10, ItemGroups.Boots, Stats.TotalVitality, 2),
            (10, ItemGroups.Armor, Stats.TotalVitality, 1),
            (10, ItemGroups.Helm, Stats.TotalVitality, 2),
            (10, ItemGroups.Pants, Stats.TotalVitality, 2));

        var gaia = this.AddAncientSet(
            "Gaia", // Silk
            23,
            (Stats.SkillDamageBonus, 10.0f),
            (Stats.MaximumMana, 25.0f),
            (Stats.TotalStrength, 10.0f),
            (Stats.DoubleDamageChance, 0.05f),
            (Stats.TotalAgility, 30.0f),
            (Stats.ExcellentDamageChance, 0.10f),
            (Stats.ExcellentDamageBonus, 10.0f));
        gaia.SetGuid(11, 1);
        this.AddItems(
            gaia,
            (11, ItemGroups.Armor, Stats.TotalVitality, 1),
            (11, ItemGroups.Gloves, Stats.TotalVitality, 1),
            (11, ItemGroups.Helm, Stats.TotalVitality, 1),
            (11, ItemGroups.Pants, Stats.TotalVitality, 1),
            (9, ItemGroups.Bows, Stats.TotalStrength, 1)); // Golden Crossbow

        var fase = this.AddAncientSet(
            "Fase", // Silk
            24,
            (Stats.MaximumHealth, 100.0f),
            (Stats.MaximumMana, 100.0f),
            (Stats.DefenseBase, 100.0f));
        fase.SetGuid(11, 2);
        this.AddItems(
            fase,
            (11, ItemGroups.Gloves, Stats.TotalVitality, 2),
            (11, ItemGroups.Pants, Stats.TotalVitality, 2),
            (11, ItemGroups.Boots, Stats.TotalVitality, 1));

        var odin = this.AddAncientSet(
            "Odin", // Wind
            25,
            (Stats.TotalEnergy, 15.0f),
            (Stats.MaximumHealth, 50.0f),
            (Stats.AttackRatePvm, 50.0f),
            (Stats.TotalAgility, 30.0f),
            (Stats.MaximumMana, 50.0f),
            (Stats.DefenseIgnoreChance, 0.05f),
            (Stats.MaximumAbility, 50.0f));
        odin.SetGuid(12, 1);
        this.AddItems(
            odin,
            (12, ItemGroups.Armor, Stats.TotalVitality, 1),
            (12, ItemGroups.Gloves, Stats.TotalVitality, 1),
            (12, ItemGroups.Helm, Stats.TotalVitality, 1),
            (12, ItemGroups.Pants, Stats.TotalVitality, 1),
            (12, ItemGroups.Boots, Stats.TotalVitality, 1));

        var elvian = this.AddAncientSet(
            "Elvian", // Wind
            26,
            (Stats.TotalAgility, 30.0f),
            (Stats.DefenseIgnoreChance, 0.05f));
        elvian.SetGuid(12, 2);
        this.AddItems(
            elvian,
            (12, ItemGroups.Pants, Stats.TotalVitality, 2),
            (12, ItemGroups.Boots, Stats.TotalVitality, 2));

        var argo = this.AddAncientSet(
            "Argo", // Spirit
            27,
            (Stats.MaximumPhysBaseDmg, 20.0f),
            (Stats.SkillDamageBonus, 25.0f),
            (Stats.MaximumAbility, 50.0f),
            (Stats.DoubleDamageChance, 0.05f));
        argo.SetGuid(13, 1);
        this.AddItems(
            argo,
            (13, ItemGroups.Armor, Stats.TotalVitality, 1),
            (13, ItemGroups.Gloves, Stats.TotalVitality, 1),
            (13, ItemGroups.Pants, Stats.TotalVitality, 1));

        var karis = this.AddAncientSet(
            "Karis", // Spirit
            28,
            (Stats.SkillDamageBonus, 15.0f),
            (Stats.DoubleDamageChance, 0.10f),
            (Stats.CriticalDamageChance, 0.10f),
            (Stats.TotalAgility, 40f));
        karis.SetGuid(13, 2);
        this.AddItems(
            karis,
            (13, ItemGroups.Helm, Stats.TotalVitality, 2),
            (13, ItemGroups.Boots, Stats.TotalVitality, 2),
            (13, ItemGroups.Pants, Stats.TotalVitality, 2));

        var gywen = this.AddAncientSet(
            "Gywen", // Guardian
            29,
            (Stats.TotalAgility, 30.0f),
            (Stats.MinimumPhysBaseDmg, 20.0f),
            (Stats.DefenseBase, 20.0f),
            (Stats.MaximumPhysBaseDmg, 20.0f),
            (Stats.CriticalDamageChance, 0.15f),
            (Stats.ExcellentDamageChance, 0.15f),
            (Stats.CriticalDamageBonus, 20.0f),
            (Stats.ExcellentDamageBonus, 20.0f));
        gywen.SetGuid(14, 1);
        this.AddItems(
            gywen,
            (14, ItemGroups.Boots, Stats.TotalVitality, 1),
            (14, ItemGroups.Gloves, Stats.TotalVitality, 1),
            (14, ItemGroups.Armor, Stats.TotalVitality, 1),
            (5, ItemGroups.Bows, Stats.TotalAgility, 1), // Silver Bow
            (28, ItemGroups.Misc1, null, 1)); // Pendant of Ability

        var aruan = this.AddAncientSet(
            "Aruan", // Guardian
            30,
            (Stats.BaseDamageBonus, 10.0f),
            (Stats.DoubleDamageChance, 0.10f),
            (Stats.SkillDamageBonus, 20.0f),
            (Stats.CriticalDamageChance, 0.15f),
            (Stats.ExcellentDamageChance, 0.15f),
            (Stats.DefenseIgnoreChance, 0.05f));
        aruan.SetGuid(14, 2);
        this.AddItems(
            aruan,
            (14, ItemGroups.Boots, Stats.TotalVitality, 2),
            (14, ItemGroups.Pants, Stats.TotalVitality, 2),
            (14, ItemGroups.Armor, Stats.TotalVitality, 2),
            (14, ItemGroups.Helm, Stats.TotalVitality, 2));

        var gaion = this.AddAncientSet(
            "Gaion", // Storm Crow
            31,
            (Stats.DefenseIgnoreChance, 0.05f),
            (Stats.DoubleDamageChance, 0.15f),
            (Stats.SkillDamageBonus, 15.0f),
            (Stats.ExcellentDamageChance, 0.15f),
            (Stats.ExcellentDamageBonus, 30.0f),
            (Stats.WizardryAttackDamageIncrease, 0.10f),
            (Stats.TotalStrength, 30.0f));
        gaion.SetGuid(15, 1);
        this.AddItems(
            gaion,
            (15, ItemGroups.Boots, Stats.TotalVitality, 1),
            (15, ItemGroups.Pants, Stats.TotalVitality, 1),
            (15, ItemGroups.Armor, Stats.TotalVitality, 1),
            (27, ItemGroups.Misc1, Stats.TotalVitality, 1)); // Pendant of Water

        var muren = this.AddAncientSet(
            "Muren", // Storm Crow
            32,
            (Stats.SkillDamageBonus, 10.0f),
            (Stats.WizardryAttackDamageIncrease, 0.10f),
            (Stats.DoubleDamageChance, 0.10f),
            (Stats.CriticalDamageChance, 0.15f),
            (Stats.ExcellentDamageChance, 0.15f),
            (Stats.DefenseBase, 25f),
            (Stats.TwoHandedWeaponDamageIncrease, 0.20f));
        muren.SetGuid(15, 2);
        this.AddItems(
            muren,
            (15, ItemGroups.Gloves, Stats.TotalVitality, 2),
            (15, ItemGroups.Pants, Stats.TotalVitality, 2),
            (15, ItemGroups.Armor, Stats.TotalVitality, 2),
            (21, ItemGroups.Misc1, Stats.TotalVitality, 2)); // Ring of Fire

        var agnis = this.AddAncientSet(
            "Agnis", // Adamantine
            33,
            (Stats.DoubleDamageChance, 0.10f),
            (Stats.DefenseBase, 40.0f),
            (Stats.SkillDamageBonus, 20.0f),
            (Stats.CriticalDamageChance, 0.15f),
            (Stats.ExcellentDamageChance, 0.15f),
            (Stats.CriticalDamageBonus, 20.0f),
            (Stats.ExcellentDamageBonus, 20.0f));
        agnis.SetGuid(26, 1);
        this.AddItems(
            agnis,
            (26, ItemGroups.Armor, Stats.TotalVitality, 1),
            (26, ItemGroups.Pants, Stats.TotalVitality, 1),
            (26, ItemGroups.Helm, Stats.TotalVitality, 1),
            (9, ItemGroups.Misc1, Stats.TotalVitality, 2)); // Ring of Poison

        var broy = this.AddAncientSet(
            "Broy", // Adamantine
            34,
            (Stats.BaseDamageBonus, 20.0f),
            (Stats.SkillDamageBonus, 20.0f),
            (Stats.TotalEnergy, 30.0f),
            (Stats.CriticalDamageChance, 0.15f),
            (Stats.ExcellentDamageChance, 0.15f),
            (Stats.DefenseIgnoreChance, 0.05f),
            (Stats.TotalLeadership, 30.0f));
        broy.SetGuid(26, 2);
        this.AddItems(
            broy,
            (26, ItemGroups.Pants, Stats.TotalVitality, 2),
            (26, ItemGroups.Gloves, Stats.TotalVitality, 2),
            (26, ItemGroups.Boots, Stats.TotalVitality, 2),
            (25, ItemGroups.Misc1, Stats.TotalStrength, 2)); // Pendant of Ice

        var chrono = this.AddAncientSet(
            "Chrono", // Red Wing
            35,
            (Stats.DoubleDamageChance, 0.20f),
            (Stats.DefenseBase, 60.0f),
            (Stats.SkillDamageBonus, 30.0f),
            (Stats.CriticalDamageChance, 0.15f),
            (Stats.ExcellentDamageChance, 0.15f),
            (Stats.CriticalDamageBonus, 20.0f),
            (Stats.ExcellentDamageBonus, 20.0f));
        chrono.SetGuid(40, 1);
        this.AddItems(
            chrono,
            (40, ItemGroups.Helm, Stats.TotalVitality, 1),
            (40, ItemGroups.Pants, Stats.TotalVitality, 1),
            (40, ItemGroups.Gloves, Stats.TotalVitality, 1),
            (24, ItemGroups.Misc1, Stats.TotalEnergy, 2)); // Ring of Magic

        var semeden = this.AddAncientSet(
            "Semeden", // Red Wing
            36,
            (Stats.WizardryAttackDamageIncrease, 0.15f),
            (Stats.SkillDamageBonus, 25.0f),
            (Stats.TotalEnergy, 30.0f),
            (Stats.CriticalDamageChance, 0.15f),
            (Stats.ExcellentDamageChance, 0.15f),
            (Stats.DefenseIgnoreChance, 0.05f));
        semeden.SetGuid(40, 2);
        this.AddItems(
            semeden,
            (40, ItemGroups.Boots, Stats.TotalVitality, 2),
            (40, ItemGroups.Gloves, Stats.TotalVitality, 2),
            (40, ItemGroups.Armor, Stats.TotalVitality, 2),
            (40, ItemGroups.Helm, Stats.TotalVitality, 2));
    }

    private void AddItems(ItemSetGroup set, params (short Number, ItemGroups Group, AttributeDefinition? BonusOption, int Discriminator)[] items)
    {
        foreach (var itemTuple in items)
        {
            var item = this.GameConfiguration.Items.First(i => i.Number == itemTuple.Number && i.Group == (byte)itemTuple.Group);
            var itemOfSet = this.Context.CreateNew<ItemOfItemSet>();
            itemOfSet.AncientSetDiscriminator = itemTuple.Discriminator;
            itemOfSet.ItemDefinition = item;
            itemOfSet.BonusOption = this.CreateAncientBonusOption(itemTuple.BonusOption);
            itemOfSet.ItemSetGroup = set;
            itemOfSet.SetGuid(item.Group, item.Number, (byte)itemTuple.Discriminator);
            set.Items.Add(itemOfSet);
            item.PossibleItemSetGroups.Add(set);
        }
    }

    private ItemSetGroup AddAncientSet(string name, short setNumber, params (AttributeDefinition Attribute, float Value)[] ancientOptions)
    {
        var set = this.Context.CreateNew<ItemSetGroup>();
        set.SetGuid(setNumber);
        set.Name = name;
        set.CountDistinct = true;
        set.MinimumItemCount = 2;
        int number = 1;
        var options = this.Context.CreateNew<ItemOptionDefinition>();
        options.SetGuid(ItemOptionDefinitionNumbers.AncientOption, setNumber, (byte)number);
        options.Name = $"{name} (Ancient Set)";
        set.Options = options;
        foreach (var optionTuple in ancientOptions)
        {
            var option = this.Context.CreateNew<IncreasableItemOption>();
            option.SetGuid(ItemOptionDefinitionNumbers.AncientOption, setNumber, (byte)number);
            option.Number = number++;
            option.OptionType = this._ancientOptionType;
            option.PowerUpDefinition = this.Context.CreateNew<PowerUpDefinition>();
            option.PowerUpDefinition.TargetAttribute = optionTuple.Attribute.GetPersistent(this.GameConfiguration);
            option.PowerUpDefinition.Boost = this.Context.CreateNew<PowerUpDefinitionValue>();
            option.PowerUpDefinition.Boost.ConstantValue.Value = optionTuple.Value;
            options.PossibleOptions.Add(option);
        }

        this.GameConfiguration.ItemSetGroups.Add(set);
        this.GameConfiguration.ItemOptions.Add(options);
        return set;
    }

    private IncreasableItemOption? CreateAncientBonusOption(AttributeDefinition? attribute)
    {
        if (attribute is null)
        {
            return null;
        }

        if (this._bonusOptions.TryGetValue(attribute, out var option))
        {
            return option;
        }

        var optionDefinition = this.Context.CreateNew<ItemOptionDefinition>();
        optionDefinition.SetGuid(ItemOptionDefinitionNumbers.AncientBonus, attribute.Id.ExtractFirstTwoBytes());
        optionDefinition.Name = $"Ancient Bonus of {attribute.Designation}";
        optionDefinition.AddsRandomly = false;
        optionDefinition.MaximumOptionsPerItem = 1;
        this.GameConfiguration.ItemOptions.Add(optionDefinition);

        option = this.Context.CreateNew<IncreasableItemOption>();
        option.SetGuid(ItemOptionDefinitionNumbers.AncientBonus, attribute.Id.ExtractFirstTwoBytes());
        option.OptionType = this._ancientBonusOptionType;
        option.PowerUpDefinition = this.Context.CreateNew<PowerUpDefinition>();
        option.PowerUpDefinition.TargetAttribute = attribute.GetPersistent(this.GameConfiguration);

        var level1 = this.Context.CreateNew<ItemOptionOfLevel>();
        level1.Level = 1;
        level1.PowerUpDefinition = this.Context.CreateNew<PowerUpDefinition>();
        level1.PowerUpDefinition.TargetAttribute = attribute.GetPersistent(this.GameConfiguration);
        level1.PowerUpDefinition.Boost = this.Context.CreateNew<PowerUpDefinitionValue>();
        level1.PowerUpDefinition.Boost.ConstantValue.Value = 5;

        var level2 = this.Context.CreateNew<ItemOptionOfLevel>();
        level2.Level = 2;
        level2.PowerUpDefinition = this.Context.CreateNew<PowerUpDefinition>();
        level2.PowerUpDefinition.TargetAttribute = attribute.GetPersistent(this.GameConfiguration);
        level2.PowerUpDefinition.Boost = this.Context.CreateNew<PowerUpDefinitionValue>();
        level2.PowerUpDefinition.Boost.ConstantValue.Value = 10;

        option.LevelDependentOptions.Add(level1);
        option.LevelDependentOptions.Add(level2);

        optionDefinition.PossibleOptions.Add(option);
        this._bonusOptions.Add(attribute, option);
        return option;
    }
}