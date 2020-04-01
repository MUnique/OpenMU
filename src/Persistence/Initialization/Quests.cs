// <copyright file="Quests.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.Persistence.Initialization
{
    using System.Linq;
    using MUnique.OpenMU.DataModel.Configuration;
    using MUnique.OpenMU.DataModel.Configuration.Quests;
    using MUnique.OpenMU.GameLogic.Attributes;
    using MUnique.OpenMU.GameServer.MessageHandler.Quests;
    using MUnique.OpenMU.Persistence.Initialization.CharacterClasses;

    /// <summary>
    /// Data Initialization for quests.
    /// </summary>
    internal class Quests : InitializerBase
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="Quests"/> class.
        /// </summary>
        /// <param name="context">The context.</param>
        /// <param name="gameConfiguration">The game configuration.</param>
        public Quests(IContext context, GameConfiguration gameConfiguration)
            : base(context, gameConfiguration)
        {
        }

        /// <inheritdoc />
        public override void Initialize()
        {
            // Level 150 Quests:
            this.FindScrollOfEmperor(CharacterClassNumber.DarkKnight);
            this.FindScrollOfEmperor(CharacterClassNumber.FairyElf);
            this.FindScrollOfEmperor(CharacterClassNumber.DarkWizard);
            this.FindScrollOfEmperor(CharacterClassNumber.DarkKnight);
            this.TreasuresOfMu(CharacterClassNumber.DarkKnight, Items.Quest.BrokenSwordNumber);
            this.TreasuresOfMu(CharacterClassNumber.FairyElf, Items.Quest.TearOfElfNumber);
            this.TreasuresOfMu(CharacterClassNumber.DarkWizard, Items.Quest.SoulShardOfWizardNumber);
            this.TreasuresOfMu(CharacterClassNumber.Summoner, Items.Quest.EyeOfAbyssalNumber);

            // Level 220 Quests:
            this.GainHeroStatus(CharacterClassNumber.BladeKnight);
            this.GainHeroStatus(CharacterClassNumber.SoulMaster);
            this.GainHeroStatus(CharacterClassNumber.MuseElf);
            this.GainHeroStatus(CharacterClassNumber.BloodySummoner);
            this.SecretOfTheDarkStone();

            // Level 380 Quest:
            this.EvidenceOfStrength();

            // Level 400 Quests:
            this.InfiltrationOfBarracksOfBallgass();
            this.IntoTheDarknessZone();
        }

        // See also http://muonlinefanz.com/guide/quests/master/#ibb
        private void InfiltrationOfBarracksOfBallgass()
        {
            var apostleDevin = this.GameConfiguration.Monsters.First(m => m.Number == 406);
            var infiltrationOfBarracks = this.Context.CreateNew<QuestDefinition>();
            apostleDevin.Quests.Add(infiltrationOfBarracks);
            infiltrationOfBarracks.QuestGiver = apostleDevin;
            infiltrationOfBarracks.Name = "Infiltrate The Barracks of Balgass";
            infiltrationOfBarracks.Group = QuestConstants.LegacyQuestGroup;
            infiltrationOfBarracks.Number = 5;
            infiltrationOfBarracks.RequiredStartMoney = 7000000;
            infiltrationOfBarracks.MinimumCharacterLevel = 400;

            var balram = this.Context.CreateNew<QuestMonsterKillRequirement>();
            balram.MinimumNumber = 20;
            balram.Monster = this.GameConfiguration.Monsters.First(m => m.Number == 409);
            infiltrationOfBarracks.RequiredMonsterKills.Add(balram);

            var deathSpirit = this.Context.CreateNew<QuestMonsterKillRequirement>();
            deathSpirit.MinimumNumber = 20;
            deathSpirit.Monster = this.GameConfiguration.Monsters.First(m => m.Number == 410);
            infiltrationOfBarracks.RequiredMonsterKills.Add(deathSpirit);

            var soram = this.Context.CreateNew<QuestMonsterKillRequirement>();
            soram.MinimumNumber = 20;
            soram.Monster = this.GameConfiguration.Monsters.First(m => m.Number == 411);
            infiltrationOfBarracks.RequiredMonsterKills.Add(soram);

            // Rewards:
            var pointReward = this.Context.CreateNew<QuestReward>();
            pointReward.Value = 10;
            pointReward.RewardType = QuestRewardType.LevelUpPoints;
            infiltrationOfBarracks.Rewards.Add(pointReward);
        }

        // See also http://muonlinefanz.com/guide/quests/master/#itrod
        private void IntoTheDarknessZone()
        {
            var apostleDevin = this.GameConfiguration.Monsters.First(m => m.Number == 406);
            var intoTheDarkness = this.Context.CreateNew<QuestDefinition>();
            apostleDevin.Quests.Add(intoTheDarkness);
            intoTheDarkness.QuestGiver = apostleDevin;
            intoTheDarkness.Name = "Into the 'Darkness' Zone";
            intoTheDarkness.Group = QuestConstants.LegacyQuestGroup;
            intoTheDarkness.Number = 6;
            intoTheDarkness.RequiredStartMoney = 10000000;
            intoTheDarkness.MinimumCharacterLevel = 400;

            var darkElf = this.Context.CreateNew<QuestMonsterKillRequirement>();
            darkElf.MinimumNumber = 1;
            darkElf.Monster = this.GameConfiguration.Monsters.First(m => m.Number == 412);
            intoTheDarkness.RequiredMonsterKills.Add(darkElf);

            // Rewards:
            var pointReward = this.Context.CreateNew<QuestReward>();
            pointReward.Value = 30;
            pointReward.RewardType = QuestRewardType.LevelUpPoints;
            intoTheDarkness.Rewards.Add(pointReward);

            var characterClassEvolution = this.Context.CreateNew<QuestReward>();
            characterClassEvolution.RewardType = QuestRewardType.CharacterEvolutionSecondToThird;
            intoTheDarkness.Rewards.Add(characterClassEvolution);
        }

        /// <summary>
        /// Evidences the of strength.
        /// </summary>
        /// <remarks>
        /// Actually, this quest is just available for the corresponding evolved character classes.
        /// However, we can safely implicitly rely that only these classes will be able to
        /// start the quest - because the previous quests must be finished.
        /// See also http://muonlinefanz.com/guide/quests/master/#eos.
        /// </remarks>
        private void EvidenceOfStrength()
        {
            var apostleDevin = this.GameConfiguration.Monsters.First(m => m.Number == 406);
            var evidenceOfStrength = this.Context.CreateNew<QuestDefinition>();
            apostleDevin.Quests.Add(evidenceOfStrength);
            evidenceOfStrength.QuestGiver = apostleDevin;
            evidenceOfStrength.Name = "Evidence of Strength";
            evidenceOfStrength.Group = QuestConstants.LegacyQuestGroup;
            evidenceOfStrength.Number = 4;
            evidenceOfStrength.RequiredStartMoney = 5000000;
            evidenceOfStrength.MinimumCharacterLevel = 380;

            this.AddItemRequirement(evidenceOfStrength, 14, Items.Quest.FlameOfDeathBeamKnightNumber, 0, 63, 3.0 / 100.0);
            this.AddItemRequirement(evidenceOfStrength, 14, Items.Quest.HornOfHellMaineNumber, 0, 309, 4.0 / 100.0);
            this.AddItemRequirement(evidenceOfStrength, 14, Items.Quest.FeatherOfDarkPhoenixNumber, 0, 77, 5.0 / 100.0);

            // Rewards:
            var pointReward = this.Context.CreateNew<QuestReward>();
            pointReward.Value = 20;
            pointReward.RewardType = QuestRewardType.LevelUpPoints;
            evidenceOfStrength.Rewards.Add(pointReward);
        }

        // See also http://muonlinefanz.com/guide/quests/darkstone/
        private void SecretOfTheDarkStone()
        {
            var marlon = this.GameConfiguration.Monsters.First(m => m.Number == 229);
            var secretDarkStone = this.Context.CreateNew<QuestDefinition>();
            marlon.Quests.Add(secretDarkStone);
            secretDarkStone.QuestGiver = marlon;
            secretDarkStone.Name = "Secret of the 'Dark Stone'";
            secretDarkStone.Group = QuestConstants.LegacyQuestGroup;
            secretDarkStone.Number = 3;
            secretDarkStone.RequiredStartMoney = 2000000;
            secretDarkStone.MinimumCharacterLevel = 220;
            secretDarkStone.QualifiedCharacter = this.GetCharacterClass(CharacterClassNumber.BladeKnight);
            this.AddItemRequirement(secretDarkStone, 14, Items.Quest.BrokenSwordNumber, 1, 78, 108);

            // Rewards:
            // Combo Skill
            var comboSkill = this.Context.CreateNew<QuestReward>();
            comboSkill.Value = 1;
            comboSkill.AttributeReward = Stats.IsSkillComboAvailable;
            comboSkill.RewardType = QuestRewardType.Attribute;

            secretDarkStone.Rewards.Add(comboSkill);
        }

        // See also http://muonlinefanz.com/guide/quests/hero-status/
        private void GainHeroStatus(CharacterClassNumber characterClass)
        {
            var marlon = this.GameConfiguration.Monsters.First(m => m.Number == 229);
            var heroStatus = this.Context.CreateNew<QuestDefinition>();
            marlon.Quests.Add(heroStatus);
            heroStatus.QuestGiver = marlon;
            heroStatus.Name = "Gain Hero Status";
            heroStatus.Group = QuestConstants.LegacyQuestGroup;
            heroStatus.Number = 2;
            heroStatus.RequiredStartMoney = 3000000;
            heroStatus.MinimumCharacterLevel = 220;
            heroStatus.QualifiedCharacter = this.GetCharacterClass(characterClass);
            this.AddItemRequirement(heroStatus, 14, Items.Quest.ScrollOfEmperorNumber, 1, 72, 108);

            // Rewards:
            // One character level point more, after level 220
            var pointReward = this.Context.CreateNew<QuestReward>();
            pointReward.Value = 1;
            pointReward.AttributeReward = Stats.PointsPerLevelUp;
            pointReward.RewardType = QuestRewardType.Attribute;

            heroStatus.Rewards.Add(pointReward);
        }

        // See also http://muonlinefanz.com/guide/quests/treasure/
        private void TreasuresOfMu(CharacterClassNumber characterClass, byte itemNumber)
        {
            var sebinaThePriestess = this.GameConfiguration.Monsters.First(m => m.Number == 235);
            var treasuresOfMu = this.Context.CreateNew<QuestDefinition>();
            sebinaThePriestess.Quests.Add(treasuresOfMu);
            treasuresOfMu.QuestGiver = sebinaThePriestess;
            treasuresOfMu.Name = "Treasures of MU";
            treasuresOfMu.Group = QuestConstants.LegacyQuestGroup;
            treasuresOfMu.Number = 1;
            treasuresOfMu.RequiredStartMoney = 2000000;
            treasuresOfMu.MinimumCharacterLevel = 150;
            treasuresOfMu.QualifiedCharacter = this.GetCharacterClass(characterClass);
            this.AddItemRequirement(treasuresOfMu, 14, itemNumber, 0, 62, 76);

            // Rewards:
            var pointReward = this.Context.CreateNew<QuestReward>();
            pointReward.Value = 10;
            pointReward.RewardType = QuestRewardType.LevelUpPoints;
            treasuresOfMu.Rewards.Add(pointReward);
            var characterClassEvolution = this.Context.CreateNew<QuestReward>();
            characterClassEvolution.RewardType = QuestRewardType.CharacterEvolutionFirstToSecond;
            treasuresOfMu.Rewards.Add(characterClassEvolution);
        }

        // See also http://muonlinefanz.com/guide/quests/scroll/
        private void FindScrollOfEmperor(CharacterClassNumber characterClass)
        {
            var sebinaThePriestess = this.GameConfiguration.Monsters.First(m => m.Number == 235);
            var findScrollOfEmperor = this.Context.CreateNew<QuestDefinition>();
            sebinaThePriestess.Quests.Add(findScrollOfEmperor);
            findScrollOfEmperor.QuestGiver = sebinaThePriestess;
            findScrollOfEmperor.Name = "Find the 'Scroll of Emperor'";
            findScrollOfEmperor.Group = QuestConstants.LegacyQuestGroup;
            findScrollOfEmperor.Number = 0;
            findScrollOfEmperor.RequiredStartMoney = 1000000;
            findScrollOfEmperor.MinimumCharacterLevel = 150;
            findScrollOfEmperor.QualifiedCharacter = this.GetCharacterClass(characterClass);

            this.AddItemRequirement(findScrollOfEmperor, 14, Items.Quest.ScrollOfEmperorNumber, 0, 45, 60);

            // Rewards:
            var pointReward = this.Context.CreateNew<QuestReward>();
            pointReward.Value = 10;
            pointReward.RewardType = QuestRewardType.LevelUpPoints;
            findScrollOfEmperor.Rewards.Add(pointReward);
        }

        private void AddItemRequirement(QuestDefinition quest, byte itemGroup, short itemNumber, byte itemLevel, byte minimumMonsterLevel, byte maximumMonsterLevel)
        {
            var itemRequirement = this.AddItemRequirement(quest, itemGroup, itemNumber, itemLevel);
            var dropItemGroup = itemRequirement.DropItemGroup;
            dropItemGroup.MinimumMonsterLevel = minimumMonsterLevel;
            dropItemGroup.MaximumMonsterLevel = maximumMonsterLevel;
        }

        private void AddItemRequirement(QuestDefinition quest, byte itemGroup, short itemNumber, byte itemLevel, short monsterNumber, double dropRate)
        {
            var itemRequirement = this.AddItemRequirement(quest, itemGroup, itemNumber, itemLevel);
            var dropItemGroup = itemRequirement.DropItemGroup;
            dropItemGroup.Monster = this.GameConfiguration.Monsters.First(m => m.Number == monsterNumber);
            dropItemGroup.Chance = dropRate;
        }

        private QuestItemRequirement AddItemRequirement(QuestDefinition quest, byte itemGroup, short itemNumber, byte itemLevel)
        {
            var itemRequirement = this.Context.CreateNew<QuestItemRequirement>();
            itemRequirement.MinimumNumber = 1;
            itemRequirement.Item = this.GameConfiguration.Items.First(item => item.Group == itemGroup && item.Number == itemNumber);

            var dropItemGroup = this.Context.CreateNew<DropItemGroup>();
            dropItemGroup.Description = $"Quest Item '{itemRequirement.Item.Name}'";
            dropItemGroup.PossibleItems.Add(itemRequirement.Item);
            dropItemGroup.Chance = 10.0 / 10000.0;
            dropItemGroup.ItemLevel = itemLevel;

            itemRequirement.DropItemGroup = dropItemGroup;
            this.GameConfiguration.DropItemGroups.Add(dropItemGroup);

            quest.RequiredItems.Add(itemRequirement);
            return itemRequirement;
        }
    }
}
