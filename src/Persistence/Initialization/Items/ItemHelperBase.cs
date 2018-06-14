// <copyright file="ItemHelperBase.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.Persistence.Initialization.Items
{
    using System.Collections.Generic;
    using System.Linq;
    using MUnique.OpenMU.AttributeSystem;
    using MUnique.OpenMU.DataModel.Attributes;
    using MUnique.OpenMU.DataModel.Configuration;
    using MUnique.OpenMU.DataModel.Configuration.Items;
    using MUnique.OpenMU.GameLogic.Attributes;

    /// <summary>
    /// The base class for the item creation helpers.
    /// </summary>
    internal class ItemHelperBase
    {
        /// <summary>
        /// The maximum item level for weapons and armors.
        /// </summary>
        protected const int MaximumItemLevel = 15;

        /// <summary>
        /// The durability increase per level.
        /// </summary>
        protected static readonly int[] DurabilityIncreasePerLevel = { 0, 1, 2, 3, 4, 6, 8, 10, 12, 14, 17, 21, 26, 32, 39, 47 };

        /// <summary>
        /// The weapon damage increase by level.
        /// </summary>
        private static readonly int[] DamageIncreaseByLevel = { 0, 3, 6, 9, 12, 15, 18, 21, 24, 27, 31, 36, 42, 49, 57, 66 };

        private readonly List<LevelBonus> damageBonusPerLevel;

        private ItemOptionDefinition excellentDefenseOptions;

        private ItemOptionDefinition excellentPhysicalWeaponOptions;

        private ItemOptionDefinition excellentWizardyWeaponOptions;

        /// <summary>
        /// Initializes a new instance of the <see cref="ItemHelperBase" /> class.
        /// </summary>
        /// <param name="context">The persistence context.</param>
        /// <param name="gameConfiguration">The game configration.</param>
        public ItemHelperBase(IContext context, GameConfiguration gameConfiguration)
        {
            this.Context = context;
            this.GameConfiguration = gameConfiguration;

            this.damageBonusPerLevel = new List<LevelBonus>();
            for (int level = 1; level <= MaximumItemLevel; level++)
            {
                var levelBonus = this.Context.CreateNew<LevelBonus>();
                levelBonus.Level = level;
                levelBonus.AdditionalValue = DamageIncreaseByLevel[level];
                this.damageBonusPerLevel.Add(levelBonus);
            }
        }

        /// <summary>
        /// Gets the repository manager.
        /// </summary>
        /// <value>
        /// The repository manager.
        /// </value>
        protected IContext Context { get; }

        /// <summary>
        /// Gets the item repository.
        /// </summary>
        /// <value>
        /// The item repository.
        /// </value>
        protected GameConfiguration GameConfiguration { get; }

        /// <summary>
        /// Gets the luck item option definition.
        /// </summary>
        protected ItemOptionDefinition Luck
        {
            get
            {
                return this.GameConfiguration.ItemOptions.FirstOrDefault(iod => iod.PossibleOptions.Any(o => o.OptionType == ItemOptionTypes.Luck));
            }
        }

        /// <summary>
        /// Gets the defense option definition.
        /// </summary>
        protected ItemOptionDefinition DefenseOption
        {
            get
            {
                return this.GameConfiguration.ItemOptions.FirstOrDefault(iod => iod.PossibleOptions.Any(o => o.OptionType == ItemOptionTypes.Option && o.PowerUpDefinition.TargetAttribute == Stats.DefenseBase));
            }
        }

        /// <summary>
        /// Gets the physical damage option definition.
        /// </summary>
        protected ItemOptionDefinition PhysicalDamageOption
        {
            get
            {
                return this.GameConfiguration.ItemOptions.FirstOrDefault(iod => iod.PossibleOptions.Any(o => o.OptionType == ItemOptionTypes.Option && o.PowerUpDefinition.TargetAttribute == Stats.MaximumPhysBaseDmg));
            }
        }

        /// <summary>
        /// Gets the wizardry damage option definition.
        /// </summary>
        protected ItemOptionDefinition WizardryDamageOption
        {
            get
            {
                return this.GameConfiguration.ItemOptions.FirstOrDefault(iod => iod.PossibleOptions.Any(o => o.OptionType == ItemOptionTypes.Option && o.PowerUpDefinition.TargetAttribute == Stats.MaximumWizBaseDmg));
            }
        }

        /// <summary>
        /// Gets the excellent defense options.
        /// </summary>
        protected ItemOptionDefinition ExcellentDefenseOptions
        {
            get
            {
                if (this.excellentDefenseOptions == null)
                {
                    var definition = this.Context.CreateNew<ItemOptionDefinition>();
                    this.GameConfiguration.ItemOptions.Add(definition);
                    definition.Name = "Excellent Defense Options";
                    definition.AddChance = 0.001f;
                    definition.AddsRandomly = true;
                    definition.MaximumOptionsPerItem = 2;

                    definition.PossibleOptions.Add(this.CreateExcellentDefenseOption(1, Stats.MoneyAmountRate, 1.4f, AggregateType.Multiplicate));
                    definition.PossibleOptions.Add(this.CreateExcellentDefenseOption(2, Stats.DefenseRatePvm, 1.1f, AggregateType.Multiplicate));
                    definition.PossibleOptions.Add(this.CreateExcellentDefenseOption(4, Stats.DamageReflection, 0.4f, AggregateType.AddRaw));
                    definition.PossibleOptions.Add(this.CreateExcellentDefenseOption(8, Stats.DamageReceiveDecrement, 0.96f, AggregateType.Multiplicate));
                    definition.PossibleOptions.Add(this.CreateExcellentDefenseOption(16, Stats.MaximumMana, 1.04f, AggregateType.Multiplicate));
                    definition.PossibleOptions.Add(this.CreateExcellentDefenseOption(32, Stats.MaximumHealth, 1.04f, AggregateType.Multiplicate));
                    this.excellentDefenseOptions = definition;
                }

                return this.excellentDefenseOptions;
            }
        }

        /// <summary>
        /// Gets the excellent physical weapon options.
        /// </summary>
        private ItemOptionDefinition ExcellentPhysicalWeaponOptions
        {
            get
            {
                if (this.excellentPhysicalWeaponOptions == null)
                {
                    var definition = this.Context.CreateNew<ItemOptionDefinition>();
                    this.GameConfiguration.ItemOptions.Add(definition);
                    definition.Name = "Excellent Physical Weapon Options";
                    definition.AddChance = 0.001f;
                    definition.AddsRandomly = true;
                    definition.MaximumOptionsPerItem = 2;

                    definition.PossibleOptions.Add(this.CreateExcellentAttackOption(1, Stats.ManaAfterMonsterKill, 1f / 8f, AggregateType.AddRaw));
                    definition.PossibleOptions.Add(this.CreateExcellentAttackOption(2, Stats.HealthAfterMonsterKill, 1f / 8f, AggregateType.AddRaw));
                    definition.PossibleOptions.Add(this.CreateExcellentAttackOption(4, Stats.AttackSpeed, 7, AggregateType.AddRaw));
                    definition.PossibleOptions.Add(this.CreateExcellentAttackOption(8, Stats.MaximumPhysBaseDmg, 1.02f, AggregateType.Multiplicate));
                    definition.PossibleOptions.Add(this.CreateExcellentAttackOption(16, Stats.MaximumPhysBaseDmg, 20, AggregateType.AddRaw)); // TODO: Dependent on character level!
                    definition.PossibleOptions.Add(this.CreateExcellentAttackOption(32, Stats.ExcellentDamageChance, 0.1f, AggregateType.AddRaw));
                    this.excellentPhysicalWeaponOptions = definition;
                }

                return this.excellentPhysicalWeaponOptions;
            }
        }

        /// <summary>
        /// Gets the excellent wizardry weapon options.
        /// </summary>
        private ItemOptionDefinition ExcellentWizardryWeaponOptions
        {
            get
            {
                if (this.excellentWizardyWeaponOptions == null)
                {
                    var definition = this.Context.CreateNew<ItemOptionDefinition>();
                    this.GameConfiguration.ItemOptions.Add(definition);
                    definition.Name = "Excellent Wizardry Weapon Options";
                    definition.AddChance = 0.001f;
                    definition.AddsRandomly = true;
                    definition.MaximumOptionsPerItem = 2;

                    definition.PossibleOptions.Add(this.CreateExcellentAttackOption(1, Stats.ManaAfterMonsterKill, 1f / 8f, AggregateType.AddRaw));
                    definition.PossibleOptions.Add(this.CreateExcellentAttackOption(2, Stats.HealthAfterMonsterKill, 1f / 8f, AggregateType.AddRaw));
                    definition.PossibleOptions.Add(this.CreateExcellentAttackOption(4, Stats.AttackSpeed, 7, AggregateType.AddRaw));
                    definition.PossibleOptions.Add(this.CreateExcellentAttackOption(8, Stats.MaximumWizBaseDmg, 1.02f, AggregateType.Multiplicate));
                    definition.PossibleOptions.Add(this.CreateExcellentAttackOption(16, Stats.MaximumWizBaseDmg, 20, AggregateType.AddRaw)); // TODO: Dependent on character level!
                    definition.PossibleOptions.Add(this.CreateExcellentAttackOption(32, Stats.ExcellentDamageChance, 0.1f, AggregateType.AddRaw));
                    this.excellentWizardyWeaponOptions = definition;
                }

                return this.excellentWizardyWeaponOptions;
            }
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
        /// <param name="staffRise">The staff rise.</param>
        /// <param name="levelRequirement">The level requirement.</param>
        /// <param name="strengthRequirement">The strength requirement.</param>
        /// <param name="agilityRequirement">The agility requirement.</param>
        /// <param name="energyRequirement">The energy requirement.</param>
        /// <param name="vitalityRequirement">The vitality requirement.</param>
        /// <param name="leadershipRequirement">The leadership requirement.</param>
        /// <param name="wizardClass">The wizard class.</param>
        /// <param name="knightClass">The knight class.</param>
        /// <param name="elfClass">The elf class.</param>
        /// <param name="magicGladiatorClass">The magic gladiator class.</param>
        /// <param name="darkLordClass">The dark lord class.</param>
        /// <param name="summonerClass">The summoner class.</param>
        /// <param name="ragefighterClass">The ragefighter class.</param>
        protected void CreateItem(
            byte group, byte number, byte slot, int skillNumber, byte width, byte height,
            bool dropsFromMonsters, string name, byte dropLevel, int minimumDamage, int maximumDamage, int attackSpeed,
            byte durability, int staffRise, int levelRequirement, int strengthRequirement, int agilityRequirement,
            int energyRequirement, int vitalityRequirement, int leadershipRequirement,
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
            item.DropsFromMonsters = dropsFromMonsters;
            item.ItemSlot = this.GameConfiguration.ItemSlotTypes.FirstOrDefault(t => t.ItemSlots.Contains(slot));
            if (skillNumber > 0)
            {
                var itemSkill = this.GameConfiguration.Skills.FirstOrDefault(s => s.SkillID == skillNumber);
                item.Skill = itemSkill;
            }

            item.Durability = durability;
            var qualifiedCharacterClasses = this.GameConfiguration.DetermineCharacterClasses(wizardClass, knightClass, elfClass, magicGladiatorClass, darkLordClass, summonerClass, ragefighterClass);
            qualifiedCharacterClasses.ToList().ForEach(item.QualifiedCharacters.Add);

            var minDamagePowerUp = this.Context.CreateNew<ItemBasePowerUpDefinition>();
            minDamagePowerUp.TargetAttribute = this.GameConfiguration.Attributes.First(a => a == Stats.MinimumPhysBaseDmgByWeapon);
            minDamagePowerUp.BaseValue = minimumDamage;
            var maxDamagePowerUp = this.Context.CreateNew<ItemBasePowerUpDefinition>();
            maxDamagePowerUp.TargetAttribute = this.GameConfiguration.Attributes.First(a => a == Stats.MaximumPhysBaseDmgByWeapon);
            maxDamagePowerUp.BaseValue = maximumDamage;
            this.damageBonusPerLevel.ForEach(minDamagePowerUp.BonusPerLevel.Add);
            this.damageBonusPerLevel.ForEach(maxDamagePowerUp.BonusPerLevel.Add);
            item.BasePowerUpAttributes.Add(minDamagePowerUp);
            item.BasePowerUpAttributes.Add(maxDamagePowerUp);
            var speedPowerUp = this.Context.CreateNew<ItemBasePowerUpDefinition>();
            speedPowerUp.TargetAttribute = this.GameConfiguration.Attributes.First(a => a == Stats.AttackSpeed);
            speedPowerUp.BaseValue = attackSpeed;
            item.BasePowerUpAttributes.Add(speedPowerUp);

            if (levelRequirement > 0)
            {
                item.Requirements.Add(
                    this.CreateAttributeRequirement(this.GameConfiguration.Attributes.First(a => a == Stats.Level), levelRequirement));
            }

            if (strengthRequirement > 0)
            {
                item.Requirements.Add(
                    this.CreateAttributeRequirement(this.GameConfiguration.Attributes.First(a => a == Stats.TotalStrength), strengthRequirement));
            }

            if (agilityRequirement > 0)
            {
                item.Requirements.Add(
                    this.CreateAttributeRequirement(this.GameConfiguration.Attributes.First(a => a == Stats.TotalAgility), agilityRequirement));
            }

            if (energyRequirement > 0)
            {
                item.Requirements.Add(
                    this.CreateAttributeRequirement(this.GameConfiguration.Attributes.First(a => a == Stats.TotalEnergy), energyRequirement));
            }

            if (vitalityRequirement > 0)
            {
                item.Requirements.Add(
                    this.CreateAttributeRequirement(this.GameConfiguration.Attributes.First(a => a == Stats.TotalVitality), vitalityRequirement));
            }

            if (leadershipRequirement > 0)
            {
                item.Requirements.Add(
                    this.CreateAttributeRequirement(this.GameConfiguration.Attributes.First(a => a == Stats.TotalLeadership), leadershipRequirement));
            }

            item.PossibleItemOptions.Add(this.Luck);

            if (staffRise == 0)
            {
                item.PossibleItemOptions.Add(this.PhysicalDamageOption);
                item.PossibleItemOptions.Add(this.ExcellentPhysicalWeaponOptions);
            }
            else
            {
                item.PossibleItemOptions.Add(this.WizardryDamageOption);
                item.PossibleItemOptions.Add(this.ExcellentWizardryWeaponOptions);
                var staffRisePowerUpMinDmg = this.Context.CreateNew<ItemBasePowerUpDefinition>();
                staffRisePowerUpMinDmg.TargetAttribute = this.GameConfiguration.Attributes.First(a => a == Stats.MinimumWizBaseDmg);
                staffRisePowerUpMinDmg.BaseValue = 1f + (staffRise / 100f);
                //// TODO: staffRisePowerUpMinDmg.BaseValueElement.AggregateType = AggregateType.Multiplicate;
                item.BasePowerUpAttributes.Add(staffRisePowerUpMinDmg);
                var staffRisePowerUpMaxDmg = this.Context.CreateNew<ItemBasePowerUpDefinition>();
                staffRisePowerUpMaxDmg.TargetAttribute = this.GameConfiguration.Attributes.First(a => a == Stats.MaximumPhysBaseDmg);
                staffRisePowerUpMaxDmg.BaseValue = 1f + (staffRise / 100f);
                //// TODO: staffRisePowerUpMaxDmg.BaseValueElement.AggregateType = AggregateType.Multiplicate;
                item.BasePowerUpAttributes.Add(staffRisePowerUpMaxDmg);
            }
        }

        /// <summary>
        /// Creates the attribute requirement.
        /// </summary>
        /// <param name="attributeDefinition">The attribute definition.</param>
        /// <param name="minimumValue">The minimum value.</param>
        /// <returns>The attribute requirement.</returns>
        protected AttributeRequirement CreateAttributeRequirement(AttributeDefinition attributeDefinition, int minimumValue)
        {
            var requirement = this.Context.CreateNew<AttributeRequirement>();
            requirement.Attribute = attributeDefinition;
            requirement.MinimumValue = minimumValue;
            return requirement;
        }

        private IncreasableItemOption CreateExcellentDefenseOption(int number, AttributeDefinition attributeDefinition, float value, AggregateType aggregateType)
        {
            var itemOption = this.Context.CreateNew<IncreasableItemOption>();
            itemOption.OptionType = this.GameConfiguration.ItemOptionTypes.First(t => t == ItemOptionTypes.Excellent);
            itemOption.Number = number;
            itemOption.PowerUpDefinition = this.Context.CreateNew<PowerUpDefinition>();
            itemOption.PowerUpDefinition.TargetAttribute = this.GameConfiguration.Attributes.First(a => a == attributeDefinition);
            itemOption.PowerUpDefinition.Boost = this.Context.CreateNew<PowerUpDefinitionValue>();
            itemOption.PowerUpDefinition.Boost.ConstantValue.Value = value;
            itemOption.PowerUpDefinition.Boost.ConstantValue.AggregateType = aggregateType;
            return itemOption;
        }

        private IncreasableItemOption CreateExcellentAttackOption(int number, AttributeDefinition attributeDefinition, float value, AggregateType aggregateType)
        {
            var itemOption = this.Context.CreateNew<IncreasableItemOption>();
            itemOption.OptionType = this.GameConfiguration.ItemOptionTypes.First(t => t == ItemOptionTypes.Excellent);
            itemOption.Number = number;
            itemOption.PowerUpDefinition = this.Context.CreateNew<PowerUpDefinition>();
            itemOption.PowerUpDefinition.TargetAttribute = this.GameConfiguration.Attributes.First(a => a == attributeDefinition);
            itemOption.PowerUpDefinition.Boost = this.Context.CreateNew<PowerUpDefinitionValue>();
            itemOption.PowerUpDefinition.Boost.ConstantValue.Value = value;
            itemOption.PowerUpDefinition.Boost.ConstantValue.AggregateType = aggregateType;
            return itemOption;
        }
    }
}
