// <copyright file="Armors.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.Persistence.Initialization.Version075.Items
{
    using System.Collections.Generic;
    using System.Linq;
    using MUnique.OpenMU.AttributeSystem;
    using MUnique.OpenMU.DataModel.Attributes;
    using MUnique.OpenMU.DataModel.Configuration;
    using MUnique.OpenMU.DataModel.Configuration.Items;
    using MUnique.OpenMU.GameLogic.Attributes;
    using MUnique.OpenMU.Persistence.Initialization.CharacterClasses;
    using MUnique.OpenMU.Persistence.Initialization.Items;

    /// <summary>
    /// Initializer for armor data.
    /// </summary>
    public class Armors : InitializerBase
    {
        private static readonly int[] DefenseIncreaseByLevel = { 0, 3, 6, 9, 12, 15, 18, 21, 24, 27, 31, 36 };

        private List<LevelBonus>? defenseBonusPerLevel;
        private List<LevelBonus>? shieldDefenseBonusPerLevel;

        /// <summary>
        /// Initializes a new instance of the <see cref="Armors"/> class.
        /// </summary>
        /// <param name="context">The persistence context.</param>
        /// <param name="gameConfiguration">The game configuration.</param>
        public Armors(IContext context, GameConfiguration gameConfiguration)
            : base(context, gameConfiguration)
        {
        }

        /// <summary>
        /// Initializes armor data.
        /// </summary>
        public override void Initialize()
        {
            this.CreateBonusDefensePerLevel();

            // Shields:
            this.CreateShield(0, 1, 0, 2, 2, "Small Shield", 3, 1, 3, 22, 70, 0, 1, 1, 1);
            this.CreateShield(1, 1, 0, 2, 2, "Horn Shield", 9, 3, 9, 28, 100, 0, 0, 1, 0);
            this.CreateShield(2, 1, 0, 2, 2, "Kite Shield", 12, 4, 12, 32, 110, 0, 0, 1, 0);
            this.CreateShield(3, 1, 0, 2, 2, "Elven Shield", 21, 8, 21, 36, 30, 100, 0, 0, 1);
            this.CreateShield(4, 1, 18, 2, 2, "Buckler", 6, 2, 6, 24, 80, 0, 1, 1, 1);
            this.CreateShield(5, 1, 18, 2, 2, "Dragon Slayer Shield", 35, 10, 36, 44, 100, 40, 0, 1, 0);
            this.CreateShield(6, 1, 18, 2, 2, "Skull Shield", 15, 5, 15, 34, 110, 0, 1, 1, 1);
            this.CreateShield(7, 1, 18, 2, 2, "Spiked Shield", 30, 9, 30, 40, 130, 0, 0, 1, 0);
            this.CreateShield(8, 1, 18, 2, 2, "Tower Shield", 40, 11, 40, 46, 130, 0, 0, 1, 1);
            this.CreateShield(9, 1, 18, 2, 2, "Plate Shield", 25, 8, 25, 38, 120, 0, 0, 1, 0);
            this.CreateShield(10, 1, 18, 2, 2, "Big Round Shield", 18, 6, 18, 35, 120, 0, 0, 1, 0);
            this.CreateShield(11, 1, 18, 2, 2, "Serpent Shield", 45, 12, 45, 48, 130, 0, 0, 1, 1);
            this.CreateShield(12, 1, 18, 2, 2, "Bronze Shield", 54, 13, 54, 52, 140, 0, 0, 1, 0);
            this.CreateShield(13, 1, 18, 2, 2, "Dragon Shield", 60, 14, 60, 60, 120, 40, 0, 1, 0);
            this.CreateShield(14, 1, 0, 2, 3, "Legendary Shield", 48, 7, 48, 50, 90, 25, 1, 0, 1);

            // Helmets:
            this.CreateArmor(0, 2, 2, 2, "Bronze Helm", 16, 9, 34, 80, 20, 0, 1, 0);
            this.CreateArmor(1, 2, 2, 2, "Dragon Helm", 57, 24, 68, 120, 30, 0, 1, 0);
            this.CreateArmor(2, 2, 2, 2, "Pad Helm", 5, 4, 28, 20, 0, 1, 0, 0);
            this.CreateArmor(3, 2, 2, 2, "Legendary Helm", 50, 18, 42, 30, 0, 1, 0, 0);
            this.CreateArmor(4, 2, 2, 2, "Bone Helm", 18, 9, 30, 30, 0, 1, 0, 0);
            this.CreateArmor(5, 2, 2, 2, "Leather Helm", 6, 5, 30, 80, 0, 0, 1, 0);
            this.CreateArmor(6, 2, 2, 2, "Scale Helm", 26, 12, 40, 110, 0, 0, 1, 0);
            this.CreateArmor(7, 2, 2, 2, "Sphinx Mask", 32, 13, 36, 30, 0, 1, 0, 0);
            this.CreateArmor(8, 2, 2, 2, "Brass Helm", 36, 17, 44, 100, 30, 0, 1, 0);
            this.CreateArmor(9, 2, 2, 2, "Plate Helm", 46, 20, 50, 130, 0, 0, 1, 0);
            this.CreateArmor(10, 2, 2, 2, "Vine Helm", 6, 4, 22, 30, 60, 0, 0, 1);
            this.CreateArmor(11, 2, 2, 2, "Silk Helm", 16, 8, 26, 30, 70, 0, 0, 1);
            this.CreateArmor(12, 2, 2, 2, "Wind Helm", 28, 12, 32, 30, 80, 0, 0, 1);
            this.CreateArmor(13, 2, 2, 2, "Spirit Helm", 40, 16, 38, 40, 80, 0, 0, 1);
            this.CreateArmor(14, 2, 2, 2, "Guardian Helm", 53, 23, 45, 40, 80, 0, 0, 1);

            // Armors:
            this.CreateArmor(0, 3, 2, 2, "Bronze Armor", 18, 14, 34, 80, 20, 0, 1, 0);
            this.CreateArmor(1, 3, 2, 3, "Dragon Armor", 59, 37, 68, 120, 30, 0, 1, 0);
            this.CreateArmor(2, 3, 2, 2, "Pad Armor", 10, 7, 28, 30, 0, 1, 0, 0);
            this.CreateArmor(3, 3, 2, 2, "Legendary Armor", 56, 22, 42, 40, 0, 1, 0, 0);
            this.CreateArmor(4, 3, 2, 2, "Bone Armor", 22, 13, 30, 40, 0, 1, 0, 0);
            this.CreateArmor(5, 3, 2, 3, "Leather Armor", 10, 10, 30, 80, 0, 0, 1, 0);
            this.CreateArmor(6, 3, 2, 2, "Scale Armor", 28, 18, 40, 110, 0, 0, 1, 0);
            this.CreateArmor(7, 3, 2, 3, "Sphinx Armor", 38, 17, 36, 40, 0, 1, 0, 0);
            this.CreateArmor(8, 3, 2, 2, "Brass Armor", 38, 22, 44, 100, 30, 0, 1, 0);
            this.CreateArmor(9, 3, 2, 2, "Plate Armor", 48, 30, 50, 130, 0, 0, 1, 0);
            this.CreateArmor(10, 3, 2, 2, "Vine Armor", 10, 8, 22, 30, 60, 0, 0, 1);
            this.CreateArmor(11, 3, 2, 2, "Silk Armor", 20, 12, 26, 30, 70, 0, 0, 1);
            this.CreateArmor(12, 3, 2, 2, "Wind Armor", 32, 16, 32, 30, 80, 0, 0, 1);
            this.CreateArmor(13, 3, 2, 2, "Spirit Armor", 44, 21, 38, 40, 80, 0, 0, 1);
            this.CreateArmor(14, 3, 2, 2, "Guardian Armor", 57, 29, 45, 40, 80, 0, 0, 1);

            // Pants:
            this.CreateArmor(0, 4, 2, 2, "Bronze Pants", 15, 10, 34, 80, 20, 0, 1, 0);
            this.CreateArmor(1, 4, 2, 2, "Dragon Pants", 55, 26, 68, 120, 30, 0, 1, 0);
            this.CreateArmor(2, 4, 2, 2, "Pad Pants", 8, 5, 28, 30, 0, 1, 0, 0);
            this.CreateArmor(3, 4, 2, 2, "Legendary Pants", 53, 20, 42, 40, 0, 1, 0, 0);
            this.CreateArmor(4, 4, 2, 2, "Bone Pants", 20, 10, 30, 40, 0, 1, 0, 0);
            this.CreateArmor(5, 4, 2, 2, "Leather Pants", 8, 7, 30, 80, 0, 0, 1, 0);
            this.CreateArmor(6, 4, 2, 2, "Scale Pants", 25, 14, 40, 110, 0, 0, 1, 0);
            this.CreateArmor(7, 4, 2, 2, "Sphinx Pants", 34, 15, 36, 40, 0, 1, 0, 0);
            this.CreateArmor(8, 4, 2, 2, "Brass Pants", 35, 18, 44, 100, 30, 0, 1, 0);
            this.CreateArmor(9, 4, 2, 2, "Plate Pants", 45, 22, 50, 130, 0, 0, 1, 0);
            this.CreateArmor(10, 4, 2, 2, "Vine Pants", 8, 6, 22, 30, 60, 0, 0, 1);
            this.CreateArmor(11, 4, 2, 2, "Silk Pants", 18, 10, 26, 30, 70, 0, 0, 1);
            this.CreateArmor(12, 4, 2, 2, "Wind Pants", 30, 14, 32, 30, 80, 0, 0, 1);
            this.CreateArmor(13, 4, 2, 2, "Spirit Pants", 42, 18, 38, 40, 80, 0, 0, 1);
            this.CreateArmor(14, 4, 2, 2, "Guardian Pants", 54, 25, 45, 40, 80, 0, 0, 1);

            // Gloves:
            this.CreateArmor(0, 5, 2, 2, "Bronze Gloves", 13, 4, 34, 80, 20, 0, 1, 0);
            this.CreateArmor(1, 5, 2, 2, "Dragon Gloves", 52, 14, 68, 120, 30, 0, 1, 0);
            this.CreateArmor(2, 5, 2, 2, "Pad Gloves", 3, 2, 28, 20, 0, 1, 0, 0);
            this.CreateArmor(3, 5, 2, 2, "Legendary Gloves", 44, 11, 42, 20, 0, 1, 0, 0);
            this.CreateArmor(4, 5, 2, 2, "Bone Gloves", 14, 5, 30, 20, 0, 1, 0, 0);
            this.CreateArmor(5, 5, 2, 2, "Leather Gloves", 4, 2, 30, 80, 0, 0, 1, 0);
            this.CreateArmor(6, 5, 2, 2, "Scale Gloves", 22, 7, 40, 110, 0, 0, 1, 0);
            this.CreateArmor(7, 5, 2, 2, "Sphinx Gloves", 28, 8, 36, 20, 0, 1, 0, 0);
            this.CreateArmor(8, 5, 2, 2, "Brass Gloves", 32, 9, 44, 100, 30, 0, 1, 0);
            this.CreateArmor(9, 5, 2, 2, "Plate Gloves", 42, 12, 50, 130, 0, 0, 1, 0);
            this.CreateArmor(10, 5, 2, 2, "Vine Gloves", 4, 2, 22, 30, 60, 0, 0, 1);
            this.CreateArmor(11, 5, 2, 2, "Silk Gloves", 14, 4, 26, 30, 70, 0, 0, 1);
            this.CreateArmor(12, 5, 2, 2, "Wind Gloves", 26, 6, 32, 30, 80, 0, 0, 1);
            this.CreateArmor(13, 5, 2, 2, "Spirit Gloves", 38, 9, 38, 40, 80, 0, 0, 1);
            this.CreateArmor(14, 5, 2, 2, "Guardian Gloves", 50, 15, 45, 40, 80, 0, 0, 1);

            // Boots:
            this.CreateArmor(0, 6, 2, 2, "Bronze Boots", 12, 4, 34, 80, 20, 0, 1, 0);
            this.CreateArmor(1, 6, 2, 2, "Dragon Boots", 54, 15, 68, 120, 30, 0, 1, 0);
            this.CreateArmor(2, 6, 2, 2, "Pad Boots", 4, 3, 28, 20, 0, 1, 0, 0);
            this.CreateArmor(3, 6, 2, 2, "Legendary Boots", 46, 12, 42, 30, 0, 1, 0, 0);
            this.CreateArmor(4, 6, 2, 2, "Bone Boots", 16, 6, 30, 30, 0, 1, 0, 0);
            this.CreateArmor(5, 6, 2, 2, "Leather Boots", 5, 2, 30, 80, 0, 0, 1, 0);
            this.CreateArmor(6, 6, 2, 2, "Scale Boots", 22, 8, 40, 110, 0, 0, 1, 0);
            this.CreateArmor(7, 6, 2, 2, "Sphinx Boots", 30, 9, 36, 30, 0, 1, 0, 0);
            this.CreateArmor(8, 6, 2, 2, "Brass Boots", 32, 10, 44, 100, 30, 0, 1, 0);
            this.CreateArmor(9, 6, 2, 2, "Plate Boots", 42, 12, 50, 130, 0, 0, 1, 0);
            this.CreateArmor(10, 6, 2, 2, "Vine Boots", 5, 2, 22, 30, 60, 0, 0, 1);
            this.CreateArmor(11, 6, 2, 2, "Silk Boots", 15, 4, 26, 30, 70, 0, 0, 1);
            this.CreateArmor(12, 6, 2, 2, "Wind Boots", 27, 7, 32, 30, 80, 0, 0, 1);
            this.CreateArmor(13, 6, 2, 2, "Spirit Boots", 40, 10, 38, 40, 80, 0, 0, 1);
            this.CreateArmor(14, 6, 2, 2, "Guardian Boots", 52, 16, 45, 40, 80, 0, 0, 1);

            this.BuildSets();
        }

        private IncreasableItemOption BuildDefenseBonusOption(float bonus)
        {
            var defenseBonus = this.Context.CreateNew<IncreasableItemOption>();
            defenseBonus.PowerUpDefinition = this.Context.CreateNew<PowerUpDefinition>();
            defenseBonus.PowerUpDefinition.Boost = this.Context.CreateNew<PowerUpDefinitionValue>();
            defenseBonus.PowerUpDefinition.Boost.ConstantValue.AggregateType = AggregateType.Multiplicate;
            defenseBonus.PowerUpDefinition.Boost.ConstantValue.Value = bonus;
            defenseBonus.PowerUpDefinition.TargetAttribute = Stats.DefenseBase.GetPersistent(this.GameConfiguration);
            return defenseBonus;
        }

        private void CreateSetGroup(int setLevel, IncreasableItemOption option, ICollection<ItemDefinition> group)
        {
            var setForDefense = this.Context.CreateNew<ItemSetGroup>();
            setForDefense.Name = $"{group.First().Name.Split(' ')[0]} Defense Bonus (Level {setLevel})";
            setForDefense.MinimumItemCount = group.Count;
            setForDefense.Options.Add(option);
            setForDefense.SetLevel = (byte)setLevel;

            foreach (var item in group)
            {
                var itemOfSet = this.Context.CreateNew<ItemOfItemSet>();
                itemOfSet.ItemDefinition = item;
                setForDefense.Items.Add(itemOfSet);
            }
        }

        private void BuildSets()
        {
            var sets = this.GameConfiguration.Items.Where(item => item.Group is >= 7 and <= 11).GroupBy(item => item.Number);

            var defenseRateBonus = this.Context.CreateNew<IncreasableItemOption>();
            defenseRateBonus.PowerUpDefinition = this.Context.CreateNew<PowerUpDefinition>();
            defenseRateBonus.PowerUpDefinition.Boost = this.Context.CreateNew<PowerUpDefinitionValue>();
            defenseRateBonus.PowerUpDefinition.Boost.ConstantValue.AggregateType = AggregateType.Multiplicate;
            defenseRateBonus.PowerUpDefinition.Boost.ConstantValue.Value = 1.1f;
            defenseRateBonus.PowerUpDefinition.TargetAttribute = Stats.DefenseRatePvm.GetPersistent(this.GameConfiguration);

            var defenseBonus = new Dictionary<int, IncreasableItemOption>
            {
                { 10, this.BuildDefenseBonusOption(1.05f) },
                { 11, this.BuildDefenseBonusOption(1.10f) },
            };

            foreach (var group in sets)
            {
                var setForDefenseRate = this.Context.CreateNew<ItemSetGroup>();
                setForDefenseRate.Name = group.First().Name.Split(' ')[0] + " Defense Rate Bonus";
                setForDefenseRate.MinimumItemCount = group.Count();
                setForDefenseRate.Options.Add(defenseRateBonus);
                foreach (var item in group)
                {
                    var itemOfSet = this.Context.CreateNew<ItemOfItemSet>();
                    itemOfSet.ItemDefinition = item;
                    setForDefenseRate.Items.Add(itemOfSet);
                }

                for (int setLevel = 10; setLevel <= Constants.MaximumItemLevel; setLevel++)
                {
                    this.CreateSetGroup(setLevel, defenseBonus[setLevel], group.ToList());
                }
            }
        }

        private void CreateShield(byte number, byte slot, byte skill, byte width, byte height, string name, byte dropLevel, int defense, int defenseRate, byte durability, int strengthRequirement, int agilityRequirement, int darkWizardClassLevel, int darkKnightClassLevel, int elfClassLevel)
        {
            var shield = this.CreateArmor(number, slot, width, height, name, dropLevel, 0, durability, strengthRequirement, agilityRequirement, darkWizardClassLevel, darkKnightClassLevel, elfClassLevel);
            if (skill != 0)
            {
                shield.Skill = this.GameConfiguration.Skills.First(s => s.Number == skill);
            }

            if (defense > 0)
            {
                var powerUp = this.Context.CreateNew<ItemBasePowerUpDefinition>();
                powerUp.TargetAttribute = Stats.DefenseBase.GetPersistent(this.GameConfiguration);
                powerUp.BaseValue = defense;
                this.shieldDefenseBonusPerLevel?.ForEach(powerUp.BonusPerLevel.Add);
                shield.BasePowerUpAttributes.Add(powerUp);
            }

            if (defenseRate > 0)
            {
                var powerUp = this.Context.CreateNew<ItemBasePowerUpDefinition>();
                powerUp.TargetAttribute = Stats.DefenseRatePvm.GetPersistent(this.GameConfiguration);
                powerUp.BaseValue = defenseRate;
                this.defenseBonusPerLevel?.ForEach(powerUp.BonusPerLevel.Add);
                shield.BasePowerUpAttributes.Add(powerUp);
            }

            var isShieldEquipped = this.Context.CreateNew<ItemBasePowerUpDefinition>();
            isShieldEquipped.TargetAttribute = Stats.IsShieldEquipped.GetPersistent(this.GameConfiguration);
            isShieldEquipped.BaseValue = 1;
            shield.BasePowerUpAttributes.Add(isShieldEquipped);
        }

        private ItemDefinition CreateArmor(byte number, byte slot, byte width, byte height, string name, byte dropLevel, int defense, byte durability, int strengthRequirement, int agilityRequirement, int darkWizardClassLevel, int darkKnightClassLevel, int elfClassLevel)
        {
            var armor = this.Context.CreateNew<ItemDefinition>();
            this.GameConfiguration.Items.Add(armor);
            armor.Group = (byte)(slot + 5);
            armor.Number = number;
            armor.Width = width;
            armor.Height = height;
            armor.Name = name;
            armor.DropLevel = dropLevel;
            armor.MaximumItemLevel = Constants.MaximumItemLevel;
            armor.DropsFromMonsters = true;
            armor.Durability = durability;
            armor.ItemSlot = this.GameConfiguration.ItemSlotTypes.First(st => st.ItemSlots.Contains(slot));
            this.CreateItemRequirementIfNeeded(armor, Stats.TotalStrengthRequirementValue, strengthRequirement);
            this.CreateItemRequirementIfNeeded(armor, Stats.TotalAgilityRequirementValue, agilityRequirement);

            if (defense > 0)
            {
                var powerUp = this.Context.CreateNew<ItemBasePowerUpDefinition>();
                powerUp.TargetAttribute = Stats.DefenseBase.GetPersistent(this.GameConfiguration);
                powerUp.BaseValue = defense;
                this.defenseBonusPerLevel?.ForEach(powerUp.BonusPerLevel.Add);
                armor.BasePowerUpAttributes.Add(powerUp);
            }

            var classes = this.GameConfiguration.DetermineCharacterClasses(darkWizardClassLevel == 1, darkKnightClassLevel == 1, elfClassLevel == 1);
            foreach (var characterClass in classes)
            {
                armor.QualifiedCharacters.Add(characterClass);
            }

            armor.PossibleItemOptions.Add(this.GameConfiguration.GetLuck());
            armor.PossibleItemOptions.Add(this.GameConfiguration.GetDefenseOption());

            return armor;
        }

        private void CreateBonusDefensePerLevel()
        {
            this.defenseBonusPerLevel = new List<LevelBonus>();
            this.shieldDefenseBonusPerLevel = new List<LevelBonus>();
            for (int level = 1; level <= Constants.MaximumItemLevel; level++)
            {
                var levelBonus = this.Context.CreateNew<LevelBonus>();
                levelBonus.Level = level;
                levelBonus.AdditionalValue = DefenseIncreaseByLevel[level];
                this.defenseBonusPerLevel.Add(levelBonus);

                var shieldLevelBonus = this.Context.CreateNew<LevelBonus>();
                levelBonus.Level = level;
                levelBonus.AdditionalValue = level;
                this.shieldDefenseBonusPerLevel.Add(shieldLevelBonus);
            }
        }
    }
}