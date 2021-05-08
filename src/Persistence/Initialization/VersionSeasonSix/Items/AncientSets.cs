// <copyright file="AncientSets.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.Persistence.Initialization.VersionSeasonSix.Items
{
    using System.Collections.Generic;
    using System.Linq;
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
        private readonly IDictionary<AttributeDefinition, IncreasableItemOption> bonusOptions = new Dictionary<AttributeDefinition, IncreasableItemOption>();
        private readonly ItemOptionType ancientBonusOptionType;
        private readonly ItemOptionType ancientOptionType;

        /// <summary>
        /// Initializes a new instance of the <see cref="AncientSets"/> class.
        /// </summary>
        /// <param name="context">The context.</param>
        /// <param name="gameConfiguration">The game configuration.</param>
        public AncientSets(IContext context, GameConfiguration gameConfiguration)
            : base(context, gameConfiguration)
        {
            this.ancientBonusOptionType = this.GameConfiguration.ItemOptionTypes.First(iot => iot == ItemOptionTypes.AncientBonus);
            this.ancientOptionType = this.GameConfiguration.ItemOptionTypes.First(iot => iot == ItemOptionTypes.AncientOption);
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
            this.AddItems(
                warrior,
                (5, ItemGroups.Boots, Stats.TotalVitality),
                (5, ItemGroups.Gloves, Stats.TotalVitality),
                (5, ItemGroups.Helm, Stats.TotalVitality),
                (5, ItemGroups.Pants, Stats.TotalVitality),
                (5, ItemGroups.Armor, Stats.TotalVitality),
                (1, ItemGroups.Axes, Stats.TotalStrength), // Morning Star
                (8, ItemGroups.Misc1, Stats.TotalAgility)); // Ring of Ice

            var anonymous = this.AddAncientSet(
                "Anonymous", // Leather
                2,
                (Stats.MaximumHealth, 50.0f),
                (Stats.TotalAgility, 50.0f),
                (Stats.ShieldBlockDamageDecrement, 0.25f),
                (Stats.BaseDamageBonus, 30.0f));
            this.AddItems(
                anonymous,
                (5, ItemGroups.Boots, Stats.TotalVitality),
                (5, ItemGroups.Helm, Stats.TotalVitality),
                (5, ItemGroups.Pants, Stats.TotalVitality),
                (0, ItemGroups.Shields, Stats.TotalVitality)); // Small Shield

            var hyperion = this.AddAncientSet(
                "Hyperion", // Bronze
                1,
                (Stats.TotalEnergy, 15.0f),
                (Stats.TotalAgility, 15.0f),
                (Stats.SkillDamageBonus, 20.0f),
                (Stats.MaximumMana, 30.0f));
            this.AddItems(
                hyperion,
                (0, ItemGroups.Boots, Stats.TotalVitality),
                (0, ItemGroups.Pants, Stats.TotalVitality),
                (0, ItemGroups.Armor, Stats.TotalVitality));

            var mist = this.AddAncientSet(
                "Mist", // Bronze
                2,
                (Stats.TotalVitality, 20.0f),
                (Stats.SkillDamageBonus, 30.0f),
                (Stats.DoubleDamageChance, 0.1f),
                (Stats.TotalAgility, 20.0f));
            this.AddItems(
                mist,
                (0, ItemGroups.Gloves, Stats.TotalVitality),
                (0, ItemGroups.Pants, Stats.TotalVitality),
                (0, ItemGroups.Helm, Stats.TotalVitality));

            var eplete = this.AddAncientSet(
                "Eplete", // Scale
                1,
                (Stats.SkillDamageBonus, 15.0f),
                (Stats.AttackRatePvm, 50.0f),
                (Stats.WizardryAttackDamageIncrease, 0.05f),
                (Stats.MaximumHealth, 50.0f),
                (Stats.MaximumAbility, 30.0f),
                (Stats.CriticalDamageChance, 0.10f),
                (Stats.ExcellentDamageChance, 0.10f));
            this.AddItems(
                eplete,
                (6, ItemGroups.Pants, Stats.TotalVitality),
                (6, ItemGroups.Armor, Stats.TotalVitality),
                (6, ItemGroups.Helm, Stats.TotalVitality),
                (9, ItemGroups.Shields, Stats.TotalVitality), // Plate Shield
                (12, ItemGroups.Misc1, Stats.TotalEnergy)); // Pendant of Lightning

            var berserker = this.AddAncientSet(
                "Berserker", // Scale
                2,
                (Stats.MaximumPhysBaseDmg, 10.0f),
                (Stats.MaximumPhysBaseDmg, 20.0f),
                (Stats.MaximumPhysBaseDmg, 30.0f),
                (Stats.MaximumPhysBaseDmg, 40.0f),
                (Stats.SkillDamageBonus, 50.0f),
                (Stats.TotalStrength, 40.0f));
            this.AddItems(
                berserker,
                (6, ItemGroups.Pants, Stats.TotalVitality),
                (6, ItemGroups.Armor, Stats.TotalVitality),
                (6, ItemGroups.Helm, Stats.TotalVitality),
                (6, ItemGroups.Gloves, Stats.TotalVitality),
                (6, ItemGroups.Boots, Stats.TotalVitality));

            var garuda = this.AddAncientSet(
                "Garuda", // Brass
                1,
                (Stats.MaximumAbility, 30.0f),
                (Stats.DoubleDamageChance, 0.05f),
                (Stats.TotalEnergy, 15.0f),
                (Stats.MaximumHealth, 50.0f),
                (Stats.SkillDamageBonus, 25.0f),
                (Stats.WizardryAttackDamageIncrease, 0.15f));
            this.AddItems(
                garuda,
                (8, ItemGroups.Pants, Stats.TotalVitality),
                (8, ItemGroups.Armor, Stats.TotalVitality),
                (8, ItemGroups.Gloves, Stats.TotalVitality),
                (8, ItemGroups.Boots, Stats.TotalVitality),
                (13, ItemGroups.Misc1, Stats.TotalStrength)); // Pendant of Fire

            var cloud = this.AddAncientSet(
                "Cloud", // Brass
                2,
                (Stats.CriticalDamageChance, 0.20f),
                (Stats.CriticalDamageBonus, 50.0f));
            this.AddItems(
                cloud,
                (8, ItemGroups.Pants, Stats.TotalVitality),
                (8, ItemGroups.Helm, Stats.TotalVitality));

            var kantata = this.AddAncientSet(
                "Kantata", // Plate
                1,
                (Stats.TotalEnergy, 15.0f),
                (Stats.TotalVitality, 30.0f),
                (Stats.WizardryAttackDamageIncrease, 0.10f),
                (Stats.TotalStrength, 15.0f),
                (Stats.SkillDamageBonus, 25.0f),
                (Stats.ExcellentDamageChance, 10.0f),
                (Stats.ExcellentDamageBonus, 20.0f));
            this.AddItems(
                kantata,
                (9, ItemGroups.Boots, Stats.TotalVitality),
                (9, ItemGroups.Gloves, Stats.TotalVitality),
                (9, ItemGroups.Armor, Stats.TotalVitality),
                (23, ItemGroups.Misc1, Stats.TotalAgility), // Ring of Wind
                (9, ItemGroups.Misc1, Stats.TotalVitality)); // Ring of Poison

            var rave = this.AddAncientSet(
                "Rave", // Plate
                2,
                (Stats.SkillDamageBonus, 20.0f),
                (Stats.DoubleDamageChance, 0.10f),
                (Stats.TwoHandedWeaponDamageIncrease, 0.30f),
                (Stats.DefenseIgnoreChance, 0.05f));
            this.AddItems(
                rave,
                (9, ItemGroups.Helm, Stats.TotalVitality),
                (9, ItemGroups.Pants, Stats.TotalVitality),
                (9, ItemGroups.Armor, Stats.TotalVitality));

            var hyon = this.AddAncientSet(
                "Hyon", // Dragon
                1,
                (Stats.DefenseBase, 25),
                (Stats.DoubleDamageChance, 0.10f),
                (Stats.SkillDamageBonus, 20),
                (Stats.CriticalDamageChance, 0.15f),
                (Stats.ExcellentDamageChance, 0.15f),
                (Stats.CriticalDamageBonus, 20f),
                (Stats.ExcellentDamageBonus, 20f));
            this.AddItems(
                hyon,
                (14, ItemGroups.Swords, Stats.TotalStrength), // Lightning Sword
                (1, ItemGroups.Helm, Stats.TotalVitality),
                (1, ItemGroups.Boots, Stats.TotalVitality),
                (1, ItemGroups.Gloves, Stats.TotalVitality));

            var vicious = this.AddAncientSet(
                "Vicious", // Dragon
                2,
                (Stats.SkillDamageBonus, 15.0f),
                (Stats.BaseDamageBonus, 15.0f),
                (Stats.DoubleDamageChance, 0.10f),
                (Stats.MinimumPhysBaseDmg, 20f),
                (Stats.MaximumPhysBaseDmg, 30f),
                (Stats.DefenseIgnoreChance, 0.05f));
            this.AddItems(
                vicious,
                (22, ItemGroups.Misc1, Stats.TotalStrength), // Ring of Earth
                (1, ItemGroups.Helm, Stats.TotalVitality),
                (1, ItemGroups.Pants, Stats.TotalVitality),
                (1, ItemGroups.Armor, Stats.TotalVitality));

            var apollo = this.AddAncientSet(
                "Apollo", // Pad
                1,
                (Stats.TotalEnergy, 10.0f),
                (Stats.WizardryAttackDamageIncrease, 0.05f),
                (Stats.SkillDamageBonus, 10.0f),
                (Stats.MaximumMana, 30.0f),
                (Stats.MaximumHealth, 30.0f),
                (Stats.MaximumAbility, 20.0f),
                (Stats.CriticalDamageChance, 0.10f),
                (Stats.ExcellentDamageChance, 0.10f),
                (Stats.TotalEnergy, 30.0f));
            this.AddItems(
                apollo,
                (0, ItemGroups.Staff, Stats.TotalEnergy), // Skull Staff
                (2, ItemGroups.Helm, Stats.TotalVitality),
                (2, ItemGroups.Armor, Stats.TotalVitality),
                (2, ItemGroups.Pants, Stats.TotalVitality),
                (2, ItemGroups.Gloves, Stats.TotalVitality),
                (25, ItemGroups.Misc1, Stats.TotalStrength), // Pendant of Ice
                (24, ItemGroups.Misc1, Stats.BaseEnergy)); // Ring of Magic

            var barnake = this.AddAncientSet(
                "Barnake", // Pad
                2,
                (Stats.WizardryAttackDamageIncrease, 0.10f),
                (Stats.TotalEnergy, 20.0f),
                (Stats.SkillDamageBonus, 30.0f),
                (Stats.MaximumMana, 100.0f));
            this.AddItems(
                barnake,
                (2, ItemGroups.Helm, Stats.TotalVitality),
                (2, ItemGroups.Pants, Stats.TotalVitality),
                (2, ItemGroups.Boots, Stats.TotalVitality));

            var evis = this.AddAncientSet(
                "Evis", // Bone
                1,
                (Stats.SkillDamageBonus, 15.0f),
                (Stats.TotalVitality, 20.0f),
                (Stats.WizardryAttackDamageIncrease, 0.10f),
                (Stats.DoubleDamageChance, 0.05f),
                (Stats.AttackRatePvm, 50.0f),
                (Stats.AbilityRecoveryAbsolute, 5.0f));
            this.AddItems(
                evis,
                (4, ItemGroups.Armor, Stats.TotalVitality),
                (4, ItemGroups.Pants, Stats.TotalVitality),
                (4, ItemGroups.Boots, Stats.TotalVitality),
                (26, ItemGroups.Misc1, Stats.TotalAgility)); // Pendant of Wind

            var sylion = this.AddAncientSet(
                "Sylion", // Bone
                2,
                (Stats.DoubleDamageChance, 0.05f),
                (Stats.CriticalDamageChance, 0.05f),
                (Stats.DefenseBase, 20.0f),
                (Stats.TotalStrength, 50.0f),
                (Stats.TotalAgility, 50.0f),
                (Stats.TotalVitality, 50.0f),
                (Stats.TotalEnergy, 50.0f));
            this.AddItems(
                sylion,
                (4, ItemGroups.Armor, Stats.TotalVitality),
                (4, ItemGroups.Gloves, Stats.TotalVitality),
                (4, ItemGroups.Boots, Stats.TotalVitality),
                (4, ItemGroups.Helm, Stats.TotalVitality));

            var heras = this.AddAncientSet(
                "Heras", // Sphinx
                1,
                (Stats.TotalStrength, 15.0f),
                (Stats.WizardryAttackDamageIncrease, 0.10f),
                (Stats.DefenseIncreaseWithEquippedShield, 0.05f),
                (Stats.TotalEnergy, 15.0f),
                (Stats.AttackRatePvm, 50.0f),
                (Stats.CriticalDamageChance, 0.10f),
                (Stats.ExcellentDamageChance, 0.10f),
                (Stats.MaximumHealth, 50.0f),
                (Stats.MaximumMana, 50.0f));
            this.AddItems(
                heras,
                (6, ItemGroups.Shields, Stats.TotalVitality), // Skull Shield
                (7, ItemGroups.Pants, Stats.TotalVitality),
                (7, ItemGroups.Armor, Stats.TotalVitality),
                (7, ItemGroups.Helm, Stats.TotalVitality),
                (7, ItemGroups.Gloves, Stats.TotalVitality),
                (7, ItemGroups.Boots, Stats.TotalVitality));

            var minet = this.AddAncientSet(
                "Minet", // Sphinx
                2,
                (Stats.TotalEnergy, 30.0f),
                (Stats.DefenseBase, 30.0f),
                (Stats.MaximumMana, 100.0f),
                (Stats.SkillDamageBonus, 15.0f));
            this.AddItems(
                minet,
                (7, ItemGroups.Pants, Stats.TotalVitality),
                (7, ItemGroups.Armor, Stats.TotalVitality),
                (7, ItemGroups.Boots, Stats.TotalVitality));

            var anubis = this.AddAncientSet(
                "Anubis", // Legendary
                1,
                (Stats.DoubleDamageChance, 0.10f),
                (Stats.MaximumMana, 50.0f),
                (Stats.WizardryAttackDamageIncrease, 0.10f),
                (Stats.CriticalDamageChance, 0.15f),
                (Stats.ExcellentDamageChance, 0.15f),
                (Stats.CriticalDamageBonus, 20.0f),
                (Stats.ExcellentDamageChance, 20.0f));
            this.AddItems(
                anubis,
                (3, ItemGroups.Armor, Stats.TotalVitality),
                (3, ItemGroups.Helm, Stats.TotalVitality),
                (3, ItemGroups.Gloves, Stats.TotalVitality),
                (21, ItemGroups.Misc1, Stats.TotalEnergy)); // Ring of Fire

            var enis = this.AddAncientSet(
                "Enis", // Legendary
                2,
                (Stats.SkillDamageBonus, 10.0f),
                (Stats.DoubleDamageChance, 0.10f),
                (Stats.TotalEnergy, 30.0f),
                (Stats.WizardryAttackDamageIncrease, 0.10f),
                (Stats.DefenseIgnoreChance, 0.05f));
            this.AddItems(
                enis,
                (3, ItemGroups.Armor, Stats.TotalVitality),
                (3, ItemGroups.Helm, Stats.TotalVitality),
                (3, ItemGroups.Boots, Stats.TotalVitality),
                (3, ItemGroups.Pants, Stats.TotalVitality));

            var ceto = this.AddAncientSet(
                "Ceto", // Vine
                1,
                (Stats.TotalAgility, 10.0f),
                (Stats.MaximumHealth, 50.0f),
                (Stats.DefenseBase, 20.0f),
                (Stats.DefenseIncreaseWithEquippedShield, 0.05f),
                (Stats.TotalEnergy, 10.0f),
                (Stats.MaximumHealth, 50.0f),
                (Stats.TotalStrength, 20.0f));
            this.AddItems(
                ceto,
                (10, ItemGroups.Boots, Stats.TotalVitality),
                (10, ItemGroups.Gloves, Stats.TotalVitality),
                (10, ItemGroups.Helm, Stats.TotalVitality),
                (10, ItemGroups.Pants, Stats.TotalVitality),
                (2, ItemGroups.Swords, Stats.TotalStrength), // Rapier
                (22, ItemGroups.Misc1, Stats.TotalStrength)); // Ring of Earth

            var drake = this.AddAncientSet(
                "Drake", // Vine
                2,
                (Stats.TotalAgility, 20.0f),
                (Stats.BaseDamageBonus, 25.0f),
                (Stats.DoubleDamageChance, 0.20f),
                (Stats.DefenseBase, 40.0f),
                (Stats.CriticalDamageChance, 0.10f));
            this.AddItems(
                drake,
                (10, ItemGroups.Boots, Stats.TotalVitality),
                (10, ItemGroups.Armor, Stats.TotalVitality),
                (10, ItemGroups.Helm, Stats.TotalVitality),
                (10, ItemGroups.Pants, Stats.TotalVitality));

            var gaia = this.AddAncientSet(
                "Gaia", // Silk
                1,
                (Stats.SkillDamageBonus, 10.0f),
                (Stats.MaximumMana, 25.0f),
                (Stats.TotalStrength, 10.0f),
                (Stats.DoubleDamageChance, 0.05f),
                (Stats.TotalAgility, 30.0f),
                (Stats.ExcellentDamageChance, 0.10f),
                (Stats.ExcellentDamageBonus, 10.0f));
            this.AddItems(
                gaia,
                (11, ItemGroups.Armor, Stats.TotalVitality),
                (11, ItemGroups.Gloves, Stats.TotalVitality),
                (11, ItemGroups.Helm, Stats.TotalVitality),
                (11, ItemGroups.Pants, Stats.TotalVitality),
                (9, ItemGroups.Bows, Stats.TotalStrength)); // Golden Crossbow

            var fase = this.AddAncientSet(
                "Fase", // Silk
                2,
                (Stats.MaximumHealth, 100.0f),
                (Stats.MaximumMana, 100.0f),
                (Stats.DefenseBase, 100.0f));
            this.AddItems(
                fase,
                (11, ItemGroups.Gloves, Stats.TotalVitality),
                (11, ItemGroups.Pants, Stats.TotalVitality),
                (11, ItemGroups.Boots, Stats.TotalVitality));

            var odin = this.AddAncientSet(
                "Odin", // Wind
                1,
                (Stats.TotalEnergy, 15.0f),
                (Stats.MaximumHealth, 50.0f),
                (Stats.AttackRatePvm, 50.0f),
                (Stats.TotalAgility, 30.0f),
                (Stats.MaximumMana, 50.0f),
                (Stats.DefenseIgnoreChance, 0.05f),
                (Stats.MaximumAbility, 50.0f));
            this.AddItems(
                odin,
                (12, ItemGroups.Armor, Stats.TotalVitality),
                (12, ItemGroups.Gloves, Stats.TotalVitality),
                (12, ItemGroups.Helm, Stats.TotalVitality),
                (12, ItemGroups.Pants, Stats.TotalVitality),
                (12, ItemGroups.Boots, Stats.TotalVitality));

            var elvian = this.AddAncientSet(
                "Elvian", // Wind
                2,
                (Stats.TotalAgility, 30.0f),
                (Stats.DefenseIgnoreChance, 0.05f));
            this.AddItems(
                elvian,
                (12, ItemGroups.Pants, Stats.TotalVitality),
                (12, ItemGroups.Boots, Stats.TotalVitality));

            var argo = this.AddAncientSet(
                "Argo", // Spirit
                1,
                (Stats.MaximumPhysBaseDmg, 20.0f),
                (Stats.SkillDamageBonus, 25.0f),
                (Stats.MaximumAbility, 50.0f),
                (Stats.DoubleDamageChance, 0.05f));
            this.AddItems(
                argo,
                (13, ItemGroups.Armor, Stats.TotalVitality),
                (13, ItemGroups.Gloves, Stats.TotalVitality),
                (13, ItemGroups.Pants, Stats.TotalVitality));

            var karis = this.AddAncientSet(
                "Karis", // Spirit
                2,
                (Stats.SkillDamageBonus, 15.0f),
                (Stats.DoubleDamageChance, 0.10f),
                (Stats.CriticalDamageChance, 0.10f),
                (Stats.TotalAgility, 40f));
            this.AddItems(
                karis,
                (13, ItemGroups.Helm, Stats.TotalVitality),
                (13, ItemGroups.Boots, Stats.TotalVitality),
                (13, ItemGroups.Pants, Stats.TotalVitality));

            var gywen = this.AddAncientSet(
                "Gywen", // Guardian
                1,
                (Stats.TotalAgility, 30.0f),
                (Stats.MinimumPhysBaseDmg, 20.0f),
                (Stats.DefenseBase, 20.0f),
                (Stats.MaximumPhysBaseDmg, 20.0f),
                (Stats.CriticalDamageChance, 0.15f),
                (Stats.ExcellentDamageChance, 0.15f),
                (Stats.CriticalDamageBonus, 20.0f),
                (Stats.ExcellentDamageBonus, 20.0f));
            this.AddItems(
                gywen,
                (14, ItemGroups.Boots, Stats.TotalVitality),
                (14, ItemGroups.Gloves, Stats.TotalVitality),
                (14, ItemGroups.Armor, Stats.TotalVitality),
                (5, ItemGroups.Bows, Stats.TotalAgility), // Silver Bow
                (28, ItemGroups.Misc1, null)); // Pendant of Ability

            var aruan = this.AddAncientSet(
                "Aruan", // Guardian
                2,
                (Stats.BaseDamageBonus, 10.0f),
                (Stats.DoubleDamageChance, 0.10f),
                (Stats.SkillDamageBonus, 20.0f),
                (Stats.CriticalDamageChance, 0.15f),
                (Stats.ExcellentDamageChance, 0.15f),
                (Stats.DefenseIgnoreChance, 0.05f));
            this.AddItems(
                aruan,
                (14, ItemGroups.Boots, Stats.TotalVitality),
                (14, ItemGroups.Pants, Stats.TotalVitality),
                (14, ItemGroups.Armor, Stats.TotalVitality),
                (14, ItemGroups.Helm, Stats.TotalVitality));

            var gaion = this.AddAncientSet(
                "Gaion", // Storm Crow
                1,
                (Stats.DefenseIgnoreChance, 0.05f),
                (Stats.DoubleDamageChance, 0.15f),
                (Stats.SkillDamageBonus, 15.0f),
                (Stats.ExcellentDamageChance, 0.15f),
                (Stats.ExcellentDamageBonus, 30.0f),
                (Stats.WizardryAttackDamageIncrease, 0.10f),
                (Stats.TotalStrength, 30.0f));
            this.AddItems(
                gaion,
                (15, ItemGroups.Boots, Stats.TotalVitality),
                (15, ItemGroups.Pants, Stats.TotalVitality),
                (15, ItemGroups.Armor, Stats.TotalVitality),
                (27, ItemGroups.Misc1, Stats.TotalVitality)); // Pendant of Water

            var muren = this.AddAncientSet(
                "Muren", // Storm Crow
                2,
                (Stats.SkillDamageBonus, 10.0f),
                (Stats.WizardryAttackDamageIncrease, 0.10f),
                (Stats.DoubleDamageChance, 0.10f),
                (Stats.CriticalDamageChance, 0.15f),
                (Stats.ExcellentDamageChance, 0.15f),
                (Stats.DefenseBase, 25f),
                (Stats.TwoHandedWeaponDamageIncrease, 0.20f));
            this.AddItems(
                muren,
                (15, ItemGroups.Gloves, Stats.TotalVitality),
                (15, ItemGroups.Pants, Stats.TotalVitality),
                (15, ItemGroups.Armor, Stats.TotalVitality),
                (21, ItemGroups.Misc1, Stats.TotalVitality)); // Ring of Fire

            var agnis = this.AddAncientSet(
                "Agnis", // Adamantine
                1,
                (Stats.DoubleDamageChance, 0.10f),
                (Stats.DefenseBase, 40.0f),
                (Stats.SkillDamageBonus, 20.0f),
                (Stats.CriticalDamageChance, 0.15f),
                (Stats.ExcellentDamageChance, 0.15f),
                (Stats.CriticalDamageBonus, 20.0f),
                (Stats.ExcellentDamageBonus, 20.0f));
            this.AddItems(
                agnis,
                (26, ItemGroups.Armor, Stats.TotalVitality),
                (26, ItemGroups.Pants, Stats.TotalVitality),
                (26, ItemGroups.Helm, Stats.TotalVitality),
                (9, ItemGroups.Misc1, Stats.TotalVitality)); // Ring of Poison

            var broy = this.AddAncientSet(
                "Broy", // Adamantine
                2,
                (Stats.BaseDamageBonus, 20.0f),
                (Stats.SkillDamageBonus, 20.0f),
                (Stats.TotalEnergy, 30.0f),
                (Stats.CriticalDamageChance, 0.15f),
                (Stats.ExcellentDamageChance, 0.15f),
                (Stats.DefenseIgnoreChance, 0.05f),
                (Stats.TotalLeadership, 30.0f));
            this.AddItems(
                broy,
                (26, ItemGroups.Pants, Stats.TotalVitality),
                (26, ItemGroups.Gloves, Stats.TotalVitality),
                (26, ItemGroups.Boots, Stats.TotalVitality),
                (25, ItemGroups.Misc1, Stats.TotalStrength)); // Pendant of Ice

            var chrono = this.AddAncientSet(
                "Chrono", // Red Wing
                1,
                (Stats.DoubleDamageChance, 0.20f),
                (Stats.DefenseBase, 60.0f),
                (Stats.SkillDamageBonus, 30.0f),
                (Stats.CriticalDamageChance, 0.15f),
                (Stats.ExcellentDamageChance, 0.15f),
                (Stats.CriticalDamageBonus, 20.0f),
                (Stats.ExcellentDamageBonus, 20.0f));
            this.AddItems(
                chrono,
                (40, ItemGroups.Helm, Stats.TotalVitality),
                (40, ItemGroups.Pants, Stats.TotalVitality),
                (40, ItemGroups.Gloves, Stats.TotalVitality),
                (24, ItemGroups.Misc1, Stats.TotalEnergy)); // Ring of Magic

            var semeden = this.AddAncientSet(
                "Semeden", // Red Wing
                2,
                (Stats.WizardryAttackDamageIncrease, 0.15f),
                (Stats.SkillDamageBonus, 25.0f),
                (Stats.TotalEnergy, 30.0f),
                (Stats.CriticalDamageChance, 0.15f),
                (Stats.ExcellentDamageChance, 0.15f),
                (Stats.DefenseIgnoreChance, 0.05f));
            this.AddItems(
                semeden,
                (40, ItemGroups.Boots, Stats.TotalVitality),
                (40, ItemGroups.Gloves, Stats.TotalVitality),
                (40, ItemGroups.Armor, Stats.TotalVitality),
                (40, ItemGroups.Helm, Stats.TotalVitality));
        }

        private void AddItems(ItemSetGroup set, params (short Number, ItemGroups Group, AttributeDefinition? BonusOption)[] items)
        {
            foreach (var itemTuple in items)
            {
                var item = this.GameConfiguration.Items.First(i => i.Number == itemTuple.Number && i.Group == (byte)itemTuple.Group);
                var itemOfSet = this.Context.CreateNew<ItemOfItemSet>();
                itemOfSet.ItemDefinition = item;
                itemOfSet.BonusOption = this.CreateAncientBonusOption(itemTuple.BonusOption);
                set.Items.Add(itemOfSet);
                item.PossibleItemSetGroups.Add(set);
            }
        }

        private ItemSetGroup AddAncientSet(string name, byte discriminator, params (AttributeDefinition Attribute, float Value)[] ancientOptions)
        {
            var set = this.Context.CreateNew<ItemSetGroup>();
            set.Name = name;
            set.CountDistinct = true;
            set.MinimumItemCount = 2;
            set.AncientSetDiscriminator = discriminator;
            int number = 1;
            foreach (var optionTuple in ancientOptions)
            {
                var option = this.Context.CreateNew<IncreasableItemOption>();
                option.Number = number++;
                option.OptionType = this.ancientOptionType;
                option.PowerUpDefinition = this.Context.CreateNew<PowerUpDefinition>();
                option.PowerUpDefinition.TargetAttribute = optionTuple.Attribute.GetPersistent(this.GameConfiguration);
                option.PowerUpDefinition.Boost = this.Context.CreateNew<PowerUpDefinitionValue>();
                option.PowerUpDefinition.Boost.ConstantValue.Value = optionTuple.Value;
                set.Options.Add(option);
            }

            this.GameConfiguration.ItemSetGroups.Add(set);
            return set;
        }

        private IncreasableItemOption? CreateAncientBonusOption(AttributeDefinition? attribute)
        {
            if (attribute is null)
            {
                return null;
            }

            if (this.bonusOptions.TryGetValue(attribute, out var option))
            {
                return option;
            }

            var optionDefinition = this.Context.CreateNew<ItemOptionDefinition>();
            optionDefinition.Name = $"Ancient Bonus of {attribute.Designation}";
            optionDefinition.AddsRandomly = false;
            optionDefinition.MaximumOptionsPerItem = 1;
            this.GameConfiguration.ItemOptions.Add(optionDefinition);

            option = this.Context.CreateNew<IncreasableItemOption>();
            option.OptionType = this.ancientBonusOptionType;
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
            this.bonusOptions.Add(attribute, option);
            return option;
        }
    }
}
