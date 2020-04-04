// <copyright file="AncientSets.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.Persistence.Initialization.Items
{
    using System.Linq;
    using MUnique.OpenMU.AttributeSystem;
    using MUnique.OpenMU.DataModel.Attributes;
    using MUnique.OpenMU.DataModel.Configuration;
    using MUnique.OpenMU.DataModel.Configuration.Items;
    using MUnique.OpenMU.GameLogic.Attributes;

    /// <summary>
    /// Initialization code for ancient sets.
    /// </summary>
    public class AncientSets : InitializerBase
    {
        private ItemOptionType ancientBonusOptionType;
        private ItemOptionType ancientOptionType;

        /// <summary>
        /// Initializes a new instance of the <see cref="AncientSets"/> class.
        /// </summary>
        /// <param name="context">The context.</param>
        /// <param name="gameConfiguration">The game configuration.</param>
        public AncientSets(IContext context, GameConfiguration gameConfiguration)
            : base(context, gameConfiguration)
        {
        }

        /// <inheritdoc />
        public override void Initialize()
        {
            this.ancientBonusOptionType = this.GameConfiguration.ItemOptionTypes.First(iot => iot == ItemOptionTypes.AncientBonus);
            this.ancientOptionType = this.GameConfiguration.ItemOptionTypes.First(iot => iot == ItemOptionTypes.AncientOption);

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
                (Stats.WizardryAttackDamageIncrease, 15.0f));
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
                (Stats.WizardryAttackDamageIncrease, 10.0f),
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
        }

        private void AddItems(ItemSetGroup set, params (short Number, ItemGroups Group, AttributeDefinition BonusOption)[] items)
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

        private IncreasableItemOption CreateAncientBonusOption(AttributeDefinition attribute)
        {
            var option = this.Context.CreateNew<IncreasableItemOption>();
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
            return option;
        }
    }
}
