// <copyright file="Wings.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.Persistence.Initialization.Items
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using MUnique.OpenMU.AttributeSystem;
    using MUnique.OpenMU.DataModel.Attributes;
    using MUnique.OpenMU.DataModel.Configuration;
    using MUnique.OpenMU.DataModel.Configuration.Items;
    using MUnique.OpenMU.GameLogic.Attributes;

    /// <summary>
    /// Initializer for wing items.
    /// </summary>
    /// <seealso cref="MUnique.OpenMU.Persistence.Initialization.InitializerBase" />
    public class Wings : InitializerBase
    {
        private const int MaximumItemLevel = 15;
        private const int MaximumOptionLevel = 4;

        private static readonly int[] DefenseIncreaseByLevel = { 0, 3, 6, 9, 12, 15, 18, 21, 24, 27, 31, 36, 42, 49, 57, 66 };
        private static readonly int[] DefenseIncreaseByLevelThirdWings = { 0, 4, 8, 12, 16, 20, 24, 28, 32, 36, 41, 47, 54, 62, 71, 81 };

        private readonly IncreasableItemOption healthRecoverOption;
        private readonly IncreasableItemOption physicalDamageOption;
        private readonly IncreasableItemOption wizardryDamageOption;
        private readonly IncreasableItemOption curseDamageOption;
        private readonly IncreasableItemOption defenseOption;

        private List<LevelBonus> defenseBonusPerLevel;
        private List<LevelBonus> defenseBonusPerLevelThridWings;
        private List<LevelBonus> damageIncreasePerLevelFirstWings;
        private List<LevelBonus> damageIncreasePerLevelSecondWings;
        private List<LevelBonus> damageIncreasePerLevelThirdWings;
        private List<LevelBonus> damageAbsorbPerLevel;

        /// <summary>
        /// Initializes a new instance of the <see cref="Wings"/> class.
        /// </summary>
        /// <param name="context">The context.</param>
        /// <param name="gameConfiguration">The game configuration.</param>
        public Wings(IContext context, GameConfiguration gameConfiguration)
            : base(context, gameConfiguration)
        {
            this.CreateAbsorbBonusPerLevel();
            this.CreateBonusDefensePerLevel();
            this.CreateDamageIncreaseBonusPerLevel();
            this.healthRecoverOption = this.CreateOption(0, Stats.HealthRecovery, AggregateType.AddRaw, 0.01f);
            this.physicalDamageOption = this.CreateOption(0, Stats.MaximumPhysBaseDmg, AggregateType.AddRaw, 4f);
            this.wizardryDamageOption = this.CreateOption(0, Stats.MaximumWizBaseDmg, AggregateType.AddRaw, 4f);
            this.curseDamageOption = this.CreateOption(0, Stats.MaximumCurseBaseDmg, AggregateType.AddRaw, 4f);
            this.defenseOption = this.CreateOption(0, Stats.DefenseBase, AggregateType.AddRaw, 4f);
        }

        [Flags]
        private enum Options
        {
            HealthRecover = 1,
            PhysDamage = 2,
            WizDamage = 4,
            CurseDamage = 8,
            Defense = 16
        }

        /// <summary>
        /// Initializes all wings.
        /// </summary>
        /// <remarks>
        /// Regex: (?m)^\s*(\d+)\s+(-*\d+)\s+(\d+)\s+(\d+)\s+(\d+)\s+(\d+)\s+(\d+)\s+(\d+)\s+\"(.+?)\"\s+(\d+)\s+(\d+)\s+(\d+)\s+(\d+)\s+(\d+)\s+(\d+)\s+(\d+)\s+(\d+)\s+(\d+)\s+(\d+)\s+(\d+)\s+(\d+)\s+(\d+)\s+(\d+)\s+(\d+).*$
        /// Replace by: this.CreateWing($1, $4, $5, "$9", $10, $11, $12, $13, $19, $20, $21, $22, $23, $24, $25);
        /// Sources for stats not provided by the Item.txt:
        /// http://muonlinefanz.com/tools/items/
        /// https://wiki.infinitymu.net/index.php?title=2nd_Level_Wing
        /// https://wiki.infinitymu.net/index.php?title=3rd_Level_Wing
        /// http://www.guiamuonline.com/items-de-mu-online/wings
        /// </remarks>
        public override void Initialize()
        {
            // First class wings:
            this.CreateWing(0, 3, 2, "Wings of Elf", 100, 10, 200, 180, 0, 0, 1, 0, 0, 0, 0, Options.HealthRecover, 12, 12, this.damageIncreasePerLevelFirstWings, this.defenseBonusPerLevel, null);
            this.CreateWing(1, 5, 3, "Wings of Heaven", 100, 10, 200, 180, 1, 0, 0, 1, 0, 0, 0, Options.WizDamage, 12, 12, this.damageIncreasePerLevelFirstWings, this.defenseBonusPerLevel, null);
            this.CreateWing(2, 5, 2, "Wings of Satan", 100, 20, 200, 180, 0, 1, 0, 1, 0, 0, 0, Options.PhysDamage, 12, 12, this.damageIncreasePerLevelFirstWings, this.defenseBonusPerLevel, null);
            this.CreateWing(41, 4, 2, "Wing of Curse", 100, 10, 200, 180, 0, 0, 0, 0, 0, 1, 0, Options.CurseDamage, 12, 12, this.damageIncreasePerLevelFirstWings, this.defenseBonusPerLevel, null);

            // Second class wings:
            var secondWingOptions = this.CreateSecondClassWingOptions();
            this.CreateWing(3, 5, 3, "Wings of Spirits", 150, 30, 200, 215, 0, 0, 2, 0, 0, 0, 0, Options.HealthRecover | Options.PhysDamage, 32, 25, this.damageIncreasePerLevelSecondWings, this.defenseBonusPerLevel, secondWingOptions);
            this.CreateWing(4, 5, 3, "Wings of Soul", 150, 30, 200, 215, 2, 0, 0, 0, 0, 0, 0, Options.HealthRecover | Options.WizDamage, 32, 25, this.damageIncreasePerLevelSecondWings, this.defenseBonusPerLevel, secondWingOptions);
            this.CreateWing(5, 3, 3, "Wings of Dragon", 150, 45, 200, 215, 0, 2, 0, 0, 0, 0, 0, Options.HealthRecover | Options.PhysDamage, 32, 25, this.damageIncreasePerLevelSecondWings, this.defenseBonusPerLevel, secondWingOptions);
            this.CreateWing(6, 4, 2, "Wings of Darkness", 150, 40, 200, 215, 0, 0, 0, 1, 0, 0, 0, Options.WizDamage | Options.PhysDamage, 32, 25, this.damageIncreasePerLevelSecondWings, this.defenseBonusPerLevel, secondWingOptions);
            this.CreateWing(42, 4, 3, "Wings of Despair", 150, 30, 200, 215, 0, 0, 0, 0, 0, 2, 0, Options.HealthRecover | Options.CurseDamage, 32, 25, this.damageIncreasePerLevelSecondWings, this.defenseBonusPerLevel, secondWingOptions);

            // The capes are a bit of a hybrid. Their damage gets increased like first wings, but they start slightly lower than 2nd wings.
            this.CreateWing(49, 2, 3, "Cape of Fighter", 180, 15, 200, 180, 0, 0, 0, 0, 0, 0, 1, Options.HealthRecover | Options.PhysDamage, 20, 10, this.damageIncreasePerLevelFirstWings, this.defenseBonusPerLevel, secondWingOptions);
            this.CreateWing(30, 2, 3, "Cape of Lord", 180, 15, 200, 180, 0, 0, 0, 0, 1, 0, 0, Options.PhysDamage, 20, 10, this.damageIncreasePerLevelFirstWings, this.defenseBonusPerLevel, this.CreateCapeOptions()).Group = 13;

            // Third class wings:
            var thirdWingOptions = this.CreateThirdClassWingOptions();
            this.CreateWing(36, 4, 3, "Wing of Storm", 150, 60, 220, 300, 0, 3, 0, 0, 0, 0, 0, Options.HealthRecover | Options.PhysDamage | Options.Defense, 39, 39, this.damageIncreasePerLevelThirdWings, this.defenseBonusPerLevelThridWings, thirdWingOptions);
            this.CreateWing(37, 4, 3, "Wing of Eternal", 150, 45, 220, 300, 3, 0, 0, 0, 0, 0, 0, Options.HealthRecover | Options.PhysDamage | Options.Defense, 39, 39, this.damageIncreasePerLevelThirdWings, this.defenseBonusPerLevelThridWings, thirdWingOptions);
            this.CreateWing(38, 4, 3, "Wing of Illusion", 150, 45, 220, 300, 0, 0, 3, 0, 0, 0, 0, Options.HealthRecover | Options.PhysDamage | Options.Defense, 39, 39, this.damageIncreasePerLevelThirdWings, this.defenseBonusPerLevelThridWings, thirdWingOptions);
            this.CreateWing(39, 4, 3, "Wing of Ruin", 150, 55, 220, 300, 0, 0, 0, 3, 0, 0, 0, Options.HealthRecover | Options.PhysDamage | Options.WizDamage, 39, 39, this.damageIncreasePerLevelThirdWings, this.defenseBonusPerLevelThridWings, thirdWingOptions);
            this.CreateWing(40, 2, 3, "Cape of Emperor", 150, 45, 220, 300, 0, 0, 0, 0, 3, 0, 0, Options.HealthRecover | Options.PhysDamage | Options.Defense, 39, 39, this.damageIncreasePerLevelThirdWings, this.defenseBonusPerLevelThridWings, thirdWingOptions);
            this.CreateWing(43, 4, 3, "Wing of Dimension", 150, 45, 220, 300, 0, 0, 0, 0, 0, 3, 0, Options.HealthRecover | Options.CurseDamage | Options.Defense, 42, 24, this.damageIncreasePerLevelThirdWings, this.defenseBonusPerLevelThridWings, thirdWingOptions);
            this.CreateWing(50, 2, 3, "Cape of Overrule", 150, 45, 220, 300, 0, 0, 0, 0, 0, 0, 3, Options.HealthRecover | Options.PhysDamage | Options.Defense, 42, 24, this.damageIncreasePerLevelThirdWings, this.defenseBonusPerLevelThridWings, thirdWingOptions);
        }

        private ItemDefinition CreateWing(byte number, byte width, byte height, string name, byte dropLevel, int defense, byte durability, int levelRequirement, int darkWizardClassLevel, int darkKnightClassLevel, int elfClassLevel, int magicGladiatorClassLevel, int darkLordClassLevel, int summonerClassLevel, int ragefighterClassLevel, Options possibleOptions, int damageIncreaseInitial, int damageAbsorbInitial, List<LevelBonus> damageIncreasePerLevel, List<LevelBonus> defenseIncreasePerLevel, ItemOptionDefinition wingOptionDefinition)
        {
            var wing = this.CreateWing(number, width, height, name, dropLevel, defense, durability, levelRequirement, darkWizardClassLevel, darkKnightClassLevel, elfClassLevel, magicGladiatorClassLevel, darkLordClassLevel, summonerClassLevel, ragefighterClassLevel);
            if (wingOptionDefinition != null)
            {
                wing.PossibleItemOptions.Add(wingOptionDefinition);
            }

            var defensePowerUp = wing.BasePowerUpAttributes.First(p => p.TargetAttribute == Stats.DefenseBase);
            defenseIncreasePerLevel.ForEach(defensePowerUp.BonusPerLevel.Add);

            if (damageAbsorbInitial > 0)
            {
                var powerUp = this.Context.CreateNew<ItemBasePowerUpDefinition>();
                powerUp.TargetAttribute = Stats.DamageReceiveDecrement;
                powerUp.BaseValue = damageAbsorbInitial;
                this.damageAbsorbPerLevel.ForEach(powerUp.BonusPerLevel.Add);
                wing.BasePowerUpAttributes.Add(powerUp);
            }

            if (damageIncreaseInitial > 0)
            {
                var powerUp = this.Context.CreateNew<ItemBasePowerUpDefinition>();
                powerUp.TargetAttribute = Stats.AttackDamageIncrease;
                powerUp.BaseValue = damageIncreaseInitial;
                damageIncreasePerLevel?.ForEach(powerUp.BonusPerLevel.Add);
                wing.BasePowerUpAttributes.Add(powerUp);
            }

            var optionDefinition = this.Context.CreateNew<ItemOptionDefinition>();
            this.GameConfiguration.ItemOptions.Add(optionDefinition);
            optionDefinition.Name = $"{name} Options";
            optionDefinition.AddChance = 0.5f;
            optionDefinition.AddsRandomly = true;
            optionDefinition.MaximumOptionsPerItem = 1;

            if (possibleOptions.HasFlag(Options.PhysDamage))
            {
                optionDefinition.PossibleOptions.Add(this.physicalDamageOption);
            }

            if (possibleOptions.HasFlag(Options.WizDamage))
            {
                optionDefinition.PossibleOptions.Add(this.wizardryDamageOption);
            }

            if (possibleOptions.HasFlag(Options.CurseDamage))
            {
                optionDefinition.PossibleOptions.Add(this.curseDamageOption);
            }

            if (possibleOptions.HasFlag(Options.HealthRecover))
            {
                optionDefinition.PossibleOptions.Add(this.healthRecoverOption);
            }

            if (possibleOptions.HasFlag(Options.Defense))
            {
                optionDefinition.PossibleOptions.Add(this.defenseOption);
            }

            return wing;
        }

        private IncreasableItemOption CreateOption(int number, AttributeDefinition attributeDefinition, AggregateType aggregateType, float valueIncrementPerLevel)
        {
            var itemOption = this.Context.CreateNew<IncreasableItemOption>();
            itemOption.OptionType = this.GameConfiguration.ItemOptionTypes.First(t => t == ItemOptionTypes.Option);
            itemOption.Number = number;

            var targetAttribute = this.GameConfiguration.Attributes.First(a => a == attributeDefinition);
            itemOption.PowerUpDefinition = this.CreatePowerUpDefinition(targetAttribute, 0, aggregateType);

            for (int level = 1; level <= MaximumOptionLevel; level++)
            {
                var optionOfLevel = this.Context.CreateNew<ItemOptionOfLevel>();
                optionOfLevel.Level = level;
                optionOfLevel.PowerUpDefinition = this.CreatePowerUpDefinition(targetAttribute, level * valueIncrementPerLevel, aggregateType);
                itemOption.LevelDependentOptions.Add(optionOfLevel);
            }

            return itemOption;
        }

        private ItemDefinition CreateWing(byte number, byte width, byte height, string name, byte dropLevel, int defense, byte durability, int levelRequirement, int darkWizardClassLevel, int darkKnightClassLevel, int elfClassLevel, int magicGladiatorClassLevel, int darkLordClassLevel, int summonerClassLevel, int ragefighterClassLevel)
        {
            var wing = this.Context.CreateNew<ItemDefinition>();
            this.GameConfiguration.Items.Add(wing);
            wing.Group = 12;
            wing.Number = number;
            wing.Width = width;
            wing.Height = height;
            wing.Name = name;
            wing.DropLevel = dropLevel;
            wing.DropsFromMonsters = false;
            wing.Durability = durability;
            wing.ItemSlot = this.GameConfiguration.ItemSlotTypes.First(st => st.ItemSlots.Contains(7));
            if (levelRequirement > 0)
            {
                var requirement = this.Context.CreateNew<AttributeRequirement>();
                requirement.Attribute = Stats.Level;
                requirement.MinimumValue = levelRequirement;
                //// TODO: each level increases the requirement by 5 Levels
                wing.Requirements.Add(requirement);
            }

            if (defense > 0)
            {
                var powerUp = this.Context.CreateNew<ItemBasePowerUpDefinition>();
                powerUp.TargetAttribute = Stats.DefenseBase;
                powerUp.BaseValue = defense;
                wing.BasePowerUpAttributes.Add(powerUp);
            }

            var classes = this.GameConfiguration.DetermineCharacterClasses(darkWizardClassLevel, darkKnightClassLevel, elfClassLevel, magicGladiatorClassLevel, darkLordClassLevel, summonerClassLevel, ragefighterClassLevel);
            foreach (var characterClass in classes)
            {
                wing.QualifiedCharacters.Add(characterClass);
            }

            return wing;
        }

        private void CreateDamageIncreaseBonusPerLevel()
        {
            this.damageIncreasePerLevelFirstWings = new List<LevelBonus>();
            this.damageIncreasePerLevelSecondWings = new List<LevelBonus>();
            this.damageIncreasePerLevelThirdWings = new List<LevelBonus>();

            for (int level = 1; level <= MaximumItemLevel; level++)
            {
                var absorb = this.Context.CreateNew<LevelBonus>();
                absorb.Level = level;
                absorb.AdditionalValue = 2 * level;
                this.damageIncreasePerLevelFirstWings.Add(absorb);
                this.damageIncreasePerLevelThirdWings.Add(absorb);

                var absorbSecondWing = this.Context.CreateNew<LevelBonus>();
                absorbSecondWing.Level = level;
                absorbSecondWing.AdditionalValue = level;
                this.damageIncreasePerLevelSecondWings.Add(absorbSecondWing);
            }
        }

        private void CreateAbsorbBonusPerLevel()
        {
            this.damageAbsorbPerLevel = new List<LevelBonus>();

            for (int level = 1; level <= MaximumItemLevel; level++)
            {
                var absorb = this.Context.CreateNew<LevelBonus>();
                absorb.Level = level;
                absorb.AdditionalValue = 2 * level;
                this.damageAbsorbPerLevel.Add(absorb);
            }
        }

        private void CreateBonusDefensePerLevel()
        {
            this.defenseBonusPerLevel = new List<LevelBonus>();
            this.defenseBonusPerLevelThridWings = new List<LevelBonus>();
            for (int level = 1; level <= MaximumItemLevel; level++)
            {
                var levelBonus = this.Context.CreateNew<LevelBonus>();
                levelBonus.Level = level;
                levelBonus.AdditionalValue = DefenseIncreaseByLevel[level];
                this.defenseBonusPerLevel.Add(levelBonus);

                var levelBonusThird = this.Context.CreateNew<LevelBonus>();
                levelBonusThird.Level = level;
                levelBonusThird.AdditionalValue = DefenseIncreaseByLevelThirdWings[level];
                this.defenseBonusPerLevelThridWings.Add(levelBonusThird);
            }
        }

        private ItemOptionDefinition CreateCapeOptions()
        {
            var definition = this.CreateSecondClassWingOptions();
            definition.Name = "Cape of Lord Options";
            definition.PossibleOptions.Add(this.CreateWingOption(8, Stats.TotalLeadership, 10f, AggregateType.AddRaw, 5f)); // Increase Command +10~75 Increases your Command by 10 plus 5 for each level. Only Cape of Lord can have it (PvM, PvP)
            this.GameConfiguration.ItemOptions.Add(definition);
            return definition;
        }

        private ItemOptionDefinition CreateSecondClassWingOptions()
        {
            var definition = this.Context.CreateNew<ItemOptionDefinition>();
            this.GameConfiguration.ItemOptions.Add(definition);
            definition.Name = "2nd Wing Options";
            definition.AddChance = 0.1f;
            definition.AddsRandomly = true;
            definition.MaximumOptionsPerItem = 1;

            // "Excellent" 2nd wing options:
            // TODO: The option numbers are probably not correct yet
            definition.PossibleOptions.Add(this.CreateWingOption(1, Stats.MaximumHealth, 50f, AggregateType.AddRaw, 5f)); // Increase max HP +50~115 Increases your maximum amount of life by 50 plus 5 for each level (PvM, PvP)
            definition.PossibleOptions.Add(this.CreateWingOption(2, Stats.MaximumMana, 50f, AggregateType.AddRaw, 5f)); // Increase max mana +50~115 Increases your maximum amount of mana by 50 plus 5 for each level (PvM, PvP)
            definition.PossibleOptions.Add(this.CreateWingOption(4, Stats.DefenseIgnoreChance, 0.03f, AggregateType.AddRaw)); // Ignore opponent's defensive power by 3% Gives you 3% chance to lower your opponent's defence to 0 for a strike. This strike is shown with yellow colour (PvP)

            return definition;
        }

        private ItemOptionDefinition CreateThirdClassWingOptions()
        {
            var definition = this.Context.CreateNew<ItemOptionDefinition>();
            this.GameConfiguration.ItemOptions.Add(definition);
            definition.Name = "3rd Wing Options";
            definition.AddChance = 0.1f;
            definition.AddsRandomly = true;
            definition.MaximumOptionsPerItem = 1;

            // TODO: The option numbers are probably not correct yet
            definition.PossibleOptions.Add(this.CreateWingOption(1, Stats.DefenseIgnoreChance, 0.05f, AggregateType.AddRaw)); // Ignore opponent's defensive power by 5%
            definition.PossibleOptions.Add(this.CreateWingOption(2, Stats.DamageReflection, 0.05f, AggregateType.AddRaw)); // Ignore opponent's defensive power by 5%
            definition.PossibleOptions.Add(this.CreateWingOption(4, Stats.FullyRecoverHealthAfterHitChance, 0.05f, AggregateType.AddRaw)); // Fully restore health when hit by 5 %
            definition.PossibleOptions.Add(this.CreateWingOption(8, Stats.FullyRecoverManaAfterHitChance, 0.05f, AggregateType.AddRaw)); // Fully recover mana when hit by 5 %

            return definition;
        }

        private IncreasableItemOption CreateWingOption(int number, AttributeDefinition attributeDefinition, float value, AggregateType aggregateType, float? valueIncrementPerLevel = null)
        {
            var itemOption = this.Context.CreateNew<IncreasableItemOption>();
            itemOption.OptionType = this.GameConfiguration.ItemOptionTypes.First(t => t == ItemOptionTypes.Wing);
            itemOption.Number = number;
            var targetAttribute = this.GameConfiguration.Attributes.First(a => a == attributeDefinition);
            itemOption.PowerUpDefinition = this.CreatePowerUpDefinition(targetAttribute, value, aggregateType);

            if (valueIncrementPerLevel.HasValue)
            {
                itemOption.LevelType = LevelType.ItemLevel;
                for (int level = 1; level <= MaximumItemLevel; level++)
                {
                    var optionOfLevel = this.Context.CreateNew<ItemOptionOfLevel>();
                    optionOfLevel.Level = level;
                    optionOfLevel.PowerUpDefinition = this.CreatePowerUpDefinition(targetAttribute, value + (level * valueIncrementPerLevel.Value), aggregateType);
                    itemOption.LevelDependentOptions.Add(optionOfLevel);
                }
            }

            return itemOption;
        }

        private PowerUpDefinition CreatePowerUpDefinition(AttributeDefinition attributeDefinition, float value, AggregateType aggregateType)
        {
            var powerUpDefinition = this.Context.CreateNew<PowerUpDefinition>();
            powerUpDefinition.TargetAttribute = this.GameConfiguration.Attributes.First(a => a == attributeDefinition);
            powerUpDefinition.Boost = this.Context.CreateNew<PowerUpDefinitionValue>();
            powerUpDefinition.Boost.ConstantValue.Value = value;
            powerUpDefinition.Boost.ConstantValue.AggregateType = aggregateType;
            return powerUpDefinition;
        }
    }
}