// <copyright file="Wings.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.Persistence.Initialization.Items
{
    using System.Collections.Generic;
    using System.Linq;
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
        private static readonly int[] DefenseIncreaseByLevel = { 0, 3, 6, 9, 12, 15, 18, 21, 24, 27, 31, 36, 42, 49, 57, 66 };
        private static readonly int[] DefenseIncreaseByLevelThirdWings = { 0, 4, 8, 12, 16, 20, 24, 28, 32, 36, 41, 47, 54, 62, 71, 81 };

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
            this.CreateWing(0, 3, 2, "Wings of Elf", 100, 10, 200, 180, 0, 0, 1, 0, 0, 0, 0);
            this.CreateWing(1, 5, 3, "Wings of Heaven", 100, 10, 200, 180, 1, 0, 0, 1, 0, 0, 0);
            this.CreateWing(2, 5, 2, "Wings of Satan", 100, 20, 200, 180, 0, 1, 0, 1, 0, 0, 0);
            this.CreateWing(3, 5, 3, "Wings of Spirits", 150, 30, 200, 215, 0, 0, 2, 0, 0, 0, 0);
            this.CreateWing(4, 5, 3, "Wings of Soul", 150, 30, 200, 215, 2, 0, 0, 0, 0, 0, 0);
            this.CreateWing(5, 3, 3, "Wings of Dragon", 150, 45, 200, 215, 0, 2, 0, 0, 0, 0, 0);
            this.CreateWing(6, 4, 2, "Wings of Darkness", 150, 40, 200, 215, 0, 0, 0, 1, 0, 0, 0);
            this.CreateWing(36, 4, 3, "Wing of Storm", 150, 60, 220, 300, 0, 3, 0, 0, 0, 0, 0);
            this.CreateWing(37, 4, 3, "Wing of Eternal", 150, 45, 220, 300, 3, 0, 0, 0, 0, 0, 0);
            this.CreateWing(38, 4, 3, "Wing of Illusion", 150, 45, 220, 300, 0, 0, 3, 0, 0, 0, 0);
            this.CreateWing(39, 4, 3, "Wing of Ruin", 150, 55, 220, 300, 0, 0, 0, 3, 0, 0, 0);
            this.CreateWing(40, 2, 3, "Cape of Emperor", 150, 45, 220, 300, 0, 0, 0, 0, 3, 0, 0);
            this.CreateWing(41, 4, 2, "Wing of Curse", 100, 10, 200, 180, 0, 0, 0, 0, 0, 1, 0);
            this.CreateWing(42, 4, 3, "Wind of Despair", 150, 30, 200, 215, 0, 0, 0, 0, 0, 2, 0);
            this.CreateWing(43, 4, 3, "Wing of Dimension", 150, 45, 220, 300, 0, 0, 0, 0, 0, 3, 0);
            this.CreateWing(49, 2, 3, "Cape of Fighter", 180, 15, 200, 180, 0, 0, 0, 0, 0, 0, 1);
            this.CreateWing(50, 2, 3, "Cape of Overrule", 150, 45, 220, 300, 0, 0, 0, 0, 0, 0, 3);
            /* Small wings which nobody wants, but webzen added them to sell them for cash:
            this.CreateWing(130, 2, 2, "Small Cape of Lord", 1, 15, 200, 1, 0, 0, 0, 0, 1, 0, 0);
            this.CreateWing(131, 3, 2, "Small Wing of Curse", 1, 10, 200, 1, 0, 0, 0, 0, 0, 1, 0);
            this.CreateWing(132, 3, 2, "Small Wings of Elf", 1, 10, 200, 1, 0, 0, 1, 0, 0, 0, 0);
            this.CreateWing(133, 3, 2, "Small Wings of Heaven", 1, 10, 200, 1, 1, 0, 0, 1, 0, 0, 0);
            this.CreateWing(134, 3, 2, "Small Wings of Satan", 1, 20, 200, 1, 0, 1, 0, 1, 0, 0, 0);
            this.CreateWing(135, 2, 2, "Little Warrior's Cloak", 1, 15, 200, 1, 0, 0, 0, 0, 0, 0, 1);*/
        }

        private void CreateWing(byte number, byte width, byte height, string name, byte dropLevel, int defense, byte durability, int levelRequirement, int darkWizardClassLevel, int darkKnightClassLevel, int elfClassLevel, int magicGladiatorClassLevel, int darkLordClassLevel, int summonerClassLevel, int ragefighterClassLevel)
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
                if (levelRequirement < 300)
                {
                    this.defenseBonusPerLevel.ForEach(powerUp.BonusPerLevel.Add);
                }
                else
                {
                    this.defenseBonusPerLevelThridWings.ForEach(powerUp.BonusPerLevel.Add);
                }

                wing.BasePowerUpAttributes.Add(powerUp);
            }

            int damageIncreaseInitial = 0, damageAbsorbInitial = 0;
            List<LevelBonus> damageIncreasePerLevel = null;

            if (levelRequirement == 180)
            {
                // first level wings
                damageIncreasePerLevel = this.damageIncreasePerLevelFirstWings;
                damageIncreaseInitial = 12;
                damageAbsorbInitial = 12;
                if (darkLordClassLevel > 0 || ragefighterClassLevel > 0)
                {
                    damageIncreaseInitial = 20;
                    damageAbsorbInitial = 10;
                }
            }
            else if (levelRequirement == 215)
            {
                // second level wings
                damageIncreasePerLevel = this.damageIncreasePerLevelSecondWings;
                damageIncreaseInitial = 32;
                damageAbsorbInitial = 25;
            }

            // actually, these wings can only be equipped by characters with at least level 400 because of the class restrictions
            else if (levelRequirement >= 300)
            {
                // third level wings
                damageIncreasePerLevel = this.damageIncreasePerLevelThirdWings;
                damageIncreaseInitial = 39;
                damageAbsorbInitial = 39;
                if (darkLordClassLevel > 0 || ragefighterClassLevel > 0)
                {
                    damageIncreaseInitial = 42;
                    damageAbsorbInitial = 24;
                }
            }

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

            var classes = this.GameConfiguration.DetermineCharacterClasses(darkWizardClassLevel, darkKnightClassLevel, elfClassLevel, magicGladiatorClassLevel, darkLordClassLevel, summonerClassLevel, ragefighterClassLevel);
            foreach (var characterClass in classes)
            {
                wing.QualifiedCharacters.Add(characterClass);
            }

            // TODO: Wing (exc) options, see https://wiki.infinitymu.net/index.php?title=Item_Options
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
    }
}