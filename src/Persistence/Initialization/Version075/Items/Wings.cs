// <copyright file="Wings.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.Persistence.Initialization.Version075.Items
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using MUnique.OpenMU.AttributeSystem;
    using MUnique.OpenMU.DataModel.Configuration;
    using MUnique.OpenMU.DataModel.Configuration.Items;
    using MUnique.OpenMU.GameLogic.Attributes;
    using MUnique.OpenMU.GameServer.RemoteView;
    using MUnique.OpenMU.Persistence.Initialization.CharacterClasses;

    /// <summary>
    /// Initializer for wing items.
    /// </summary>
    /// <seealso cref="MUnique.OpenMU.Persistence.Initialization.InitializerBase" />
    public class Wings : InitializerBase
    {
        private static readonly int[] DefenseIncreaseByLevel = { 0, 3, 6, 9, 12, 15, 18, 21, 24, 27, 31, 36 };

        private List<LevelBonus> defenseBonusPerLevel = null!;
        private List<LevelBonus> damageIncreasePerLevelFirstWings = null!;
        private List<LevelBonus> damageAbsorbPerLevel = null!;

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
        }

        private enum OptionType
        {
            HealthRecover,
            PhysDamage,
            WizDamage,
            CurseDamage,
            Defense,
        }

        /// <summary>
        /// Initializes all wings.
        /// </summary>
        public override void Initialize()
        {
            this.CreateWing(0, 3, 2, "Wings of Elf", 100, 10, 200, 180, 0, 0, 1, this.BuildOptions((0, OptionType.HealthRecover)), 12, 12, this.damageIncreasePerLevelFirstWings, this.defenseBonusPerLevel, null);
            this.CreateWing(1, 5, 3, "Wings of Heaven", 100, 10, 200, 180, 1, 0, 0, this.BuildOptions((0, OptionType.WizDamage)), 12, 12, this.damageIncreasePerLevelFirstWings, this.defenseBonusPerLevel, null);
            this.CreateWing(2, 5, 2, "Wings of Satan", 100, 20, 200, 180, 0, 1, 0, this.BuildOptions((0, OptionType.PhysDamage)), 12, 12, this.damageIncreasePerLevelFirstWings, this.defenseBonusPerLevel, null);

            this.CreateFeather();
        }

        /// <summary>
        /// Builds the options based on the given parameters.
        /// </summary>
        /// <remarks>
        /// Some wings can possibly have different item options of <see cref="ItemOptionTypes.Option"/>, depending on the outcome of consumption of the 'Jewel of Life'.
        /// Since webzen did a "great" job defining different numbers (representations in their transmitted item data) for the same options,
        /// we have to build <see cref="IncreasableItemOption"/>s for each item separately.
        /// We don't want to handle this stuff per-item in our <see cref="ItemSerializer"/> since we want a generic solution.
        /// </remarks>
        /// <param name="optionsWithNumbers">The tuples of option type with their number.</param>
        /// <returns>The built <see cref="IncreasableItemOption"/>s.</returns>
        private IEnumerable<IncreasableItemOption> BuildOptions(params (int, OptionType)[] optionsWithNumbers)
        {
            foreach (var tuple in optionsWithNumbers)
            {
                switch (tuple.Item2)
                {
                    case OptionType.CurseDamage:
                        yield return this.CreateOption(tuple.Item1, Stats.MaximumCurseBaseDmg, 0, AggregateType.AddRaw, 4f);
                        break;
                    case OptionType.Defense:
                        yield return this.CreateOption(tuple.Item1, Stats.DefenseBase, 0, AggregateType.AddRaw, 4f);
                        break;
                    case OptionType.HealthRecover:
                        yield return this.CreateOption(tuple.Item1, Stats.HealthRecoveryMultiplier, 0, AggregateType.AddRaw, 0.01f);
                        break;
                    case OptionType.PhysDamage:
                        yield return this.CreateOption(tuple.Item1, Stats.MaximumPhysBaseDmg, 0, AggregateType.AddRaw, 4f);
                        break;
                    case OptionType.WizDamage:
                        yield return this.CreateOption(tuple.Item1, Stats.MaximumWizBaseDmg, 0, AggregateType.AddRaw, 4f);
                        break;
                    default:
                        throw new ArgumentException("unknown OptionType");
                }
            }
        }

        private void CreateFeather()
        {
            var feather = this.Context.CreateNew<ItemDefinition>();
            feather.Name = "Loch's Feather";
            feather.MaximumItemLevel = 1;
            feather.Number = 14;
            feather.Group = 13;
            feather.DropLevel = 78;
            feather.Width = 1;
            feather.Height = 2;
            feather.Durability = 1;
            this.GameConfiguration.Items.Add(feather);
        }

        private ItemDefinition CreateWing(byte number, byte width, byte height, string name, byte dropLevel, int defense, byte durability, int levelRequirement, int darkWizardClassLevel, int darkKnightClassLevel, int elfClassLevel, IEnumerable<IncreasableItemOption> possibleOptions, int damageIncreaseInitial, int damageAbsorbInitial, List<LevelBonus> damageIncreasePerLevel, List<LevelBonus> defenseIncreasePerLevel, ItemOptionDefinition? wingOptionDefinition)
        {
            var wing = this.CreateWing(number, width, height, name, dropLevel, defense, durability, levelRequirement, darkWizardClassLevel, darkKnightClassLevel, elfClassLevel);
            if (wingOptionDefinition != null)
            {
                wing.PossibleItemOptions.Add(wingOptionDefinition);
            }

            var defensePowerUp = wing.BasePowerUpAttributes.First(p => p.TargetAttribute == Stats.DefenseBase);
            defenseIncreasePerLevel.ForEach(defensePowerUp.BonusPerLevel.Add);

            if (damageAbsorbInitial > 0)
            {
                var powerUp = this.Context.CreateNew<ItemBasePowerUpDefinition>();
                powerUp.TargetAttribute = Stats.DamageReceiveDecrement.GetPersistent(this.GameConfiguration);
                powerUp.BaseValue = 0f - (damageAbsorbInitial / 100f);
                this.damageAbsorbPerLevel.ForEach(powerUp.BonusPerLevel.Add);
                wing.BasePowerUpAttributes.Add(powerUp);
            }

            if (damageIncreaseInitial > 0)
            {
                var powerUp = this.Context.CreateNew<ItemBasePowerUpDefinition>();
                powerUp.TargetAttribute = Stats.AttackDamageIncrease.GetPersistent(this.GameConfiguration);
                powerUp.BaseValue = damageIncreaseInitial / 100f;
                damageIncreasePerLevel.ForEach(powerUp.BonusPerLevel.Add);
                wing.BasePowerUpAttributes.Add(powerUp);
            }

            var optionDefinition = this.Context.CreateNew<ItemOptionDefinition>();
            this.GameConfiguration.ItemOptions.Add(optionDefinition);
            optionDefinition.Name = $"{name} Options";
            optionDefinition.AddChance = 0.25f;
            optionDefinition.AddsRandomly = true;
            optionDefinition.MaximumOptionsPerItem = 1;
            wing.PossibleItemOptions.Add(optionDefinition);
            foreach (var option in possibleOptions)
            {
                optionDefinition.PossibleOptions.Add(option);
            }

            wing.PossibleItemOptions.Add(this.GameConfiguration.ItemOptions.First(iod => iod.PossibleOptions.Any(o => o?.OptionType == ItemOptionTypes.Luck)));
            return wing;
        }

        private ItemDefinition CreateWing(byte number, byte width, byte height, string name, byte dropLevel, int defense, byte durability, int levelRequirement, int darkWizardClassLevel, int darkKnightClassLevel, int elfClassLevel)
        {
            var wing = this.Context.CreateNew<ItemDefinition>();
            this.GameConfiguration.Items.Add(wing);
            wing.Group = 12;
            wing.Number = number;
            wing.Width = width;
            wing.Height = height;
            wing.Name = name;
            wing.DropLevel = dropLevel;
            wing.MaximumItemLevel = Constants.MaximumItemLevel;
            wing.DropsFromMonsters = false;
            wing.Durability = durability;
            wing.ItemSlot = this.GameConfiguration.ItemSlotTypes.First(st => st.ItemSlots.Contains(7));

            //// TODO: each level increases the requirement by 5 Levels
            this.CreateItemRequirementIfNeeded(wing, Stats.Level, levelRequirement);

            if (defense > 0)
            {
                var powerUp = this.Context.CreateNew<ItemBasePowerUpDefinition>();
                powerUp.TargetAttribute = Stats.DefenseBase.GetPersistent(this.GameConfiguration);
                powerUp.BaseValue = defense;
                wing.BasePowerUpAttributes.Add(powerUp);
            }

            var classes = this.GameConfiguration.DetermineCharacterClasses(darkWizardClassLevel == 1, darkKnightClassLevel == 1, elfClassLevel == 1);
            foreach (var characterClass in classes)
            {
                wing.QualifiedCharacters.Add(characterClass);
            }

            // add CanFly Attribute to all wings
            var canFlyPowerUp = this.Context.CreateNew<ItemBasePowerUpDefinition>();
            canFlyPowerUp.TargetAttribute = Stats.CanFly.GetPersistent(this.GameConfiguration);
            canFlyPowerUp.BaseValue = 1;
            wing.BasePowerUpAttributes.Add(canFlyPowerUp);

            return wing;
        }

        private IncreasableItemOption CreateOption(int number, AttributeDefinition attributeDefinition, float value, AggregateType aggregateType, float valueIncrementPerLevel)
        {
            var itemOption = this.Context.CreateNew<IncreasableItemOption>();
            itemOption.OptionType = this.GameConfiguration.ItemOptionTypes.First(t => t == ItemOptionTypes.Option);
            itemOption.Number = number;

            itemOption.PowerUpDefinition = this.CreatePowerUpDefinition(attributeDefinition, value, aggregateType);

            for (int level = 1; level <= Constants.MaximumOptionLevel; level++)
            {
                var optionOfLevel = this.Context.CreateNew<ItemOptionOfLevel>();
                optionOfLevel.Level = level;
                optionOfLevel.PowerUpDefinition = this.CreatePowerUpDefinition(itemOption.PowerUpDefinition.TargetAttribute!, level * valueIncrementPerLevel, aggregateType);
                itemOption.LevelDependentOptions.Add(optionOfLevel);
            }

            return itemOption;
        }

        private void CreateDamageIncreaseBonusPerLevel()
        {
            this.damageIncreasePerLevelFirstWings = new List<LevelBonus>();

            for (int level = 1; level <= Constants.MaximumItemLevel; level++)
            {
                var absorb = this.Context.CreateNew<LevelBonus>();
                absorb.Level = level;
                absorb.AdditionalValue = 0f - (0.02f * level);
                this.damageIncreasePerLevelFirstWings.Add(absorb);
            }
        }

        private void CreateAbsorbBonusPerLevel()
        {
            this.damageAbsorbPerLevel = new List<LevelBonus>();

            for (int level = 1; level <= Constants.MaximumItemLevel; level++)
            {
                var absorb = this.Context.CreateNew<LevelBonus>();
                absorb.Level = level;
                absorb.AdditionalValue = 0f - (0.02f * level);
                this.damageAbsorbPerLevel.Add(absorb);
            }
        }

        private void CreateBonusDefensePerLevel()
        {
            this.defenseBonusPerLevel = new List<LevelBonus>();
            for (int level = 1; level <= Constants.MaximumItemLevel; level++)
            {
                var levelBonus = this.Context.CreateNew<LevelBonus>();
                levelBonus.Level = level;
                levelBonus.AdditionalValue = DefenseIncreaseByLevel[level];
                this.defenseBonusPerLevel.Add(levelBonus);
            }
        }
    }
}