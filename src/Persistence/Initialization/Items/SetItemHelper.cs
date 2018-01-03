// <copyright file="SetItemHelper.cs" company="MUnique">
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
    /// Helper class to create item definitions of armor sets.
    /// </summary>
    internal class SetItemHelper : ItemHelperBase
    {
        //// private static readonly int[] rise = new int[] { 0, 3, 7, 10, 14, 17, 21, 24, 28, 31, 35, 40, 45, 50, 56, 63 };
        //// private static readonly int[] duradd = new int[] { 0, 1, 2, 3, 4, 6, 8, 10, 12, 14, 17, 21, 26, 32, 39, 47 };
        //// private static readonly int[] dmgadd = new int[] { 0, 3, 6, 9, 12, 15, 18, 21, 24, 27, 31, 36, 42, 49, 57, 66 };
        private static readonly int[] DefenseIncreaseByLevel = { 0, 3, 6, 9, 12, 15, 18, 21, 24, 27, 31, 36, 42, 49, 57, 66 };

        private List<LevelBonus> defenseBonusPerLevel;

        /// <summary>
        /// Initializes a new instance of the <see cref="SetItemHelper" /> class.
        /// </summary>
        /// <param name="repositoryManager">The repository manager.</param>
        /// <param name="gameConfiguration">The game configration.</param>
        public SetItemHelper(IRepositoryManager repositoryManager, GameConfiguration gameConfiguration)
            : base(repositoryManager, gameConfiguration)
        {
            this.CreateBonusDefensePerLevel();
            var slotTypes = gameConfiguration.ItemSlotTypes;
            this.HelmSlot = slotTypes.FirstOrDefault(t => t.ItemSlots.Contains(2));
            this.ArmorSlot = slotTypes.FirstOrDefault(t => t.ItemSlots.Contains(3));
            this.PantsSlot = slotTypes.FirstOrDefault(t => t.ItemSlots.Contains(4));
            this.GlovesSlot = slotTypes.FirstOrDefault(t => t.ItemSlots.Contains(5));
            this.BootsSlot = slotTypes.FirstOrDefault(t => t.ItemSlots.Contains(6));
        }

        private ItemSlotType ArmorSlot { get; }

        private ItemSlotType HelmSlot { get; }

        private ItemSlotType PantsSlot { get; }

        private ItemSlotType GlovesSlot { get; }

        private ItemSlotType BootsSlot { get; }

        /// <summary>
        /// Creates the sets.
        /// </summary>
        public void CreateSets()
        {
            this.CreateLeatherSetItems();
        }

        private void CreateArmor(string itemName, byte number, byte group, byte height, byte dropLevel, int baseDefense, byte baseDurability, int strengthRequirement, int agilityRequirement, IEnumerable<CharacterClass> characterRequirement)
        {
            var itemDefinition = this.RepositoryManager.CreateNew<ItemDefinition>();
            this.GameConfiguration.Items.Add(itemDefinition);

            itemDefinition.Name = itemName;
            itemDefinition.Group = group;
            itemDefinition.Number = number;

            itemDefinition.Height = height;
            itemDefinition.Width = 2;
            itemDefinition.DropLevel = dropLevel;
            itemDefinition.DropsFromMonsters = true;
            itemDefinition.ItemSlot = this.ItemSlotOfGroup(group);
            itemDefinition.Durability = baseDurability;
            characterRequirement.ToList().ForEach(itemDefinition.QualifiedCharacters.Add);

            var defensePowerUp = this.RepositoryManager.CreateNew<ItemBasePowerUpDefinition>();
            defensePowerUp.TargetAttribute = this.GameConfiguration.Attributes.First(a => a == Stats.DefenseBase);
            defensePowerUp.BaseValue = baseDefense;
            this.defenseBonusPerLevel.ForEach(defensePowerUp.BonusPerLevel.Add);
            itemDefinition.BasePowerUpAttributes.Add(defensePowerUp);
            itemDefinition.Requirements.Add(this.CreateAttributeRequirement(this.GameConfiguration.Attributes.First(a => a == Stats.TotalStrength), strengthRequirement));
            itemDefinition.Requirements.Add(this.CreateAttributeRequirement(this.GameConfiguration.Attributes.First(a => a == Stats.TotalAgility), agilityRequirement));
            itemDefinition.PossibleItemOptions.Add(this.Luck);
            itemDefinition.PossibleItemOptions.Add(this.DefenseOption);
            itemDefinition.PossibleItemOptions.Add(this.ExcellentDefenseOptions);
            //// TODO: HarmonyOptions for level 380 items
        }

        private ItemSlotType ItemSlotOfGroup(byte group)
        {
            switch (group)
            {
                case 7:
                    return this.HelmSlot;
                case 8:
                    return this.ArmorSlot;
                case 9:
                    return this.PantsSlot;
                case 10:
                    return this.GlovesSlot;
                case 11:
                    return this.BootsSlot;
            }

            return null;
        }

        private void CreateBonusDefensePerLevel()
        {
            this.defenseBonusPerLevel = new List<LevelBonus>();
            for (int level = 1; level <= MaximumItemLevel; level++)
            {
                var levelBonus = this.RepositoryManager.CreateNew<LevelBonus>();
                levelBonus.Level = level;
                levelBonus.AdditionalValue = DefenseIncreaseByLevel[level];
                this.defenseBonusPerLevel.Add(levelBonus);
            }
        }

        /// <summary>
        /// Creates the definitions of the leather armor set.
        /// </summary>
        private void CreateLeatherSetItems()
        {
            var classes = this.GameConfiguration.CharacterClasses;
            var allowedClassNumbers = new[] { CharacterClassNumber.DarkKnight, CharacterClassNumber.BladeKnight, CharacterClassNumber.BattleMaster, CharacterClassNumber.MagicGladiator, CharacterClassNumber.DuelMaster };
            var allowedClassNumbersHelm = new[] { CharacterClassNumber.DarkKnight, CharacterClassNumber.BladeKnight, CharacterClassNumber.BattleMaster };
            var allowedClasses = classes.Where(c => allowedClassNumbers.Contains((CharacterClassNumber)c.Number)).ToList();
            string setName = "Leather";
            byte setNumber = 5;
            this.CreateArmor(setName + " Helm", setNumber, 7, 2, 6, 5, 30, 34, 0, classes.Where(c => allowedClassNumbersHelm.Contains((CharacterClassNumber)c.Number)));
            this.CreateArmor(setName + " Armor", setNumber, 8, 3, 10, 10, 30, 44, 0, allowedClasses);
            this.CreateArmor(setName + " Pants", setNumber, 9, 2, 8, 7, 30, 39, 0, allowedClasses);
            this.CreateArmor(setName + " Gloves", setNumber, 10, 2, 4, 2, 30, 29, 0, allowedClasses);
            this.CreateArmor(setName + " Boots", setNumber, 11, 2, 5, 2, 30, 32, 0, allowedClasses);
        }
    }
}