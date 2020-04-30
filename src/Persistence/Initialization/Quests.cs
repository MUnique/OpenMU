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

            this.CreateNewQuests();
        }

        private QuestDefinition CreateQuest(string name, short group, short number, int minimumLevel, int maximumLevel, short npcNumber, CharacterClassNumber? qualifiedCharacter = null)
        {
            var npc = this.GameConfiguration.Monsters.First(m => m.Number == npcNumber);
            var questDefinition = this.Context.CreateNew<QuestDefinition>();
            npc.Quests.Add(questDefinition);
            questDefinition.QuestGiver = npc;
            questDefinition.Name = name;
            questDefinition.Group = group;
            questDefinition.Number = number;
            questDefinition.MinimumCharacterLevel = minimumLevel;
            questDefinition.MaximumCharacterLevel = maximumLevel;
            questDefinition.Repeatable = true;
            if (qualifiedCharacter.HasValue)
            {
                questDefinition.QualifiedCharacter =
                    this.GameConfiguration.CharacterClasses.FirstOrDefault(c =>
                        c.Number == (byte)qualifiedCharacter.Value);
            }

            return questDefinition;
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

        private void CreateNewQuests()
        {
            this.CreateQuest("Spider Hunt!", 18, 1, 1, 14, 257)
    .WithMonsterKillRequirement(10, 3, this.Context, this.GameConfiguration)
    .WithExperienceReward(2000, this.Context)
    ;

            this.CreateQuest("Dispose of the Town Plunderers!", 18, 4, 15, 25, 257, CharacterClassNumber.DarkWizard)
                .WithMonsterKillRequirement(15, 7, this.Context, this.GameConfiguration)
                .WithExperienceReward(6000, this.Context);
            this.CreateQuest("Dispose of the Town Plunderers!", 18, 4, 15, 25, 257, CharacterClassNumber.DarkKnight)
                .WithMonsterKillRequirement(15, 7, this.Context, this.GameConfiguration)
                .WithExperienceReward(6000, this.Context);

            this.CreateQuest("Dispose of the Town Plunderers!", 18, 5, 15, 25, 257, CharacterClassNumber.MagicGladiator)
                .WithMonsterKillRequirement(15, 6, this.Context, this.GameConfiguration)
                .WithExperienceReward(6000, this.Context);
            this.CreateQuest("Dispose of the Town Plunderers!", 18, 5, 15, 25, 257, CharacterClassNumber.DarkLord)
                .WithMonsterKillRequirement(15, 6, this.Context, this.GameConfiguration)
                .WithExperienceReward(6000, this.Context);
            this.CreateQuest("Dispose of the Town Plunderers!", 18, 5, 15, 25, 257, CharacterClassNumber.RageFighter)
                .WithMonsterKillRequirement(15, 6, this.Context, this.GameConfiguration)
                .WithExperienceReward(6000, this.Context);

            this.CreateQuest("Dispose of the Town Plunderers!", 18, 6, 15, 25, 257, CharacterClassNumber.FairyElf)
                .WithMonsterKillRequirement(15, 31, this.Context, this.GameConfiguration)
                .WithExperienceReward(6000, this.Context);

            this.CreateQuest("Dispose of the Town Plunderers!", 18, 7, 15, 25, 257, CharacterClassNumber.Summoner)
                .WithMonsterKillRequirement(15, 419, this.Context, this.GameConfiguration)
                .WithExperienceReward(6000, this.Context);

            this.CreateQuest("Boss of predator treatment!", 18, 10, 26, 35, 257, CharacterClassNumber.MagicGladiator)
                .WithMonsterKillRequirement(20, 14, this.Context, this.GameConfiguration)
                .WithExperienceReward(10000, this.Context);
            this.CreateQuest("Boss of predator treatment!", 18, 10, 26, 35, 257, CharacterClassNumber.DarkLord)
                .WithMonsterKillRequirement(20, 14, this.Context, this.GameConfiguration)
                .WithExperienceReward(10000, this.Context);
            this.CreateQuest("Boss of predator treatment!", 18, 10, 26, 35, 257, CharacterClassNumber.RageFighter)
                .WithMonsterKillRequirement(20, 14, this.Context, this.GameConfiguration)
                .WithExperienceReward(10000, this.Context);

            this.CreateQuest("Boss of predator treatment!", 18, 11, 26, 35, 257, CharacterClassNumber.DarkWizard)
                .WithMonsterKillRequirement(20, 7, this.Context, this.GameConfiguration)
                .WithExperienceReward(10000, this.Context);
            this.CreateQuest("Boss of predator treatment!", 18, 11, 26, 35, 257, CharacterClassNumber.DarkKnight)
                .WithMonsterKillRequirement(20, 7, this.Context, this.GameConfiguration)
                .WithExperienceReward(10000, this.Context);

            this.CreateQuest("Boss of predator treatment!", 18, 12, 26, 35, 257, CharacterClassNumber.FairyElf)
                .WithMonsterKillRequirement(20, 32, this.Context, this.GameConfiguration)
                .WithExperienceReward(10000, this.Context);

            this.CreateQuest("Boss of predator treatment!", 18, 13, 26, 35, 257, CharacterClassNumber.Summoner)
                .WithMonsterKillRequirement(20, 419, this.Context, this.GameConfiguration)
                .WithExperienceReward(10000, this.Context);

            this.CreateQuest("Monster Supply Route in Devias!", 18, 16, 36, 45, 257, CharacterClassNumber.MagicGladiator)
                .WithMonsterKillRequirement(25, 22, this.Context, this.GameConfiguration)
                .WithExperienceReward(14000, this.Context);
            this.CreateQuest("Monster Supply Route in Devias!", 18, 16, 36, 45, 257, CharacterClassNumber.DarkLord)
                .WithMonsterKillRequirement(25, 22, this.Context, this.GameConfiguration)
                .WithExperienceReward(14000, this.Context);
            this.CreateQuest("Monster Supply Route in Devias!", 18, 16, 36, 45, 257, CharacterClassNumber.RageFighter)
                .WithMonsterKillRequirement(25, 22, this.Context, this.GameConfiguration)
                .WithExperienceReward(14000, this.Context);

            this.CreateQuest("Monster Supply Route in Devias!", 18, 17, 36, 45, 257, CharacterClassNumber.DarkWizard)
                .WithMonsterKillRequirement(25, 24, this.Context, this.GameConfiguration)
                .WithExperienceReward(14000, this.Context);
            this.CreateQuest("Monster Supply Route in Devias!", 18, 17, 36, 45, 257, CharacterClassNumber.DarkKnight)
                .WithMonsterKillRequirement(25, 24, this.Context, this.GameConfiguration)
                .WithExperienceReward(14000, this.Context);
            this.CreateQuest("Monster Supply Route in Devias!", 18, 17, 36, 45, 257, CharacterClassNumber.FairyElf)
                .WithMonsterKillRequirement(25, 24, this.Context, this.GameConfiguration)
                .WithExperienceReward(14000, this.Context);
            this.CreateQuest("Monster Supply Route in Devias!", 18, 17, 36, 45, 257, CharacterClassNumber.Summoner)
                .WithMonsterKillRequirement(25, 24, this.Context, this.GameConfiguration)
                .WithExperienceReward(14000, this.Context);

            this.CreateQuest("Attack on the Supply Corps!", 18, 20, 46, 55, 257, CharacterClassNumber.MagicGladiator)
                .WithMonsterKillRequirement(30, 21, this.Context, this.GameConfiguration)
                .WithExperienceReward(21000, this.Context);
            this.CreateQuest("Attack on the Supply Corps!", 18, 20, 46, 55, 257, CharacterClassNumber.DarkLord)
                .WithMonsterKillRequirement(30, 21, this.Context, this.GameConfiguration)
                .WithExperienceReward(21000, this.Context);
            this.CreateQuest("Attack on the Supply Corps!", 18, 20, 46, 55, 257, CharacterClassNumber.RageFighter)
                .WithMonsterKillRequirement(30, 21, this.Context, this.GameConfiguration)
                .WithExperienceReward(21000, this.Context);

            this.CreateQuest("Attack on the Supply Corps!", 18, 21, 46, 55, 257, CharacterClassNumber.DarkWizard)
                .WithMonsterKillRequirement(30, 22, this.Context, this.GameConfiguration)
                .WithExperienceReward(21000, this.Context);
            this.CreateQuest("Attack on the Supply Corps!", 18, 21, 46, 55, 257, CharacterClassNumber.DarkKnight)
                .WithMonsterKillRequirement(30, 22, this.Context, this.GameConfiguration)
                .WithExperienceReward(21000, this.Context);
            this.CreateQuest("Attack on the Supply Corps!", 18, 21, 46, 55, 257, CharacterClassNumber.FairyElf)
                .WithMonsterKillRequirement(30, 22, this.Context, this.GameConfiguration)
                .WithExperienceReward(21000, this.Context);
            this.CreateQuest("Attack on the Supply Corps!", 18, 21, 46, 55, 257, CharacterClassNumber.Summoner)
                .WithMonsterKillRequirement(30, 22, this.Context, this.GameConfiguration)
                .WithExperienceReward(21000, this.Context);

            this.CreateQuest("Inland Supply Route!", 18, 24, 56, 65, 257, CharacterClassNumber.MagicGladiator)
                .WithMonsterKillRequirement(30, 19, this.Context, this.GameConfiguration)
                .WithExperienceReward(28000, this.Context);
            this.CreateQuest("Inland Supply Route!", 18, 24, 56, 65, 257, CharacterClassNumber.DarkLord)
                .WithMonsterKillRequirement(30, 19, this.Context, this.GameConfiguration)
                .WithExperienceReward(28000, this.Context);
            this.CreateQuest("Inland Supply Route!", 18, 24, 56, 65, 257, CharacterClassNumber.RageFighter)
                .WithMonsterKillRequirement(30, 19, this.Context, this.GameConfiguration)
                .WithExperienceReward(28000, this.Context);

            this.CreateQuest("Inland Supply Route!", 18, 25, 56, 65, 257, CharacterClassNumber.DarkWizard)
                .WithMonsterKillRequirement(30, 23, this.Context, this.GameConfiguration)
                .WithExperienceReward(28000, this.Context);
            this.CreateQuest("Inland Supply Route!", 18, 25, 56, 65, 257, CharacterClassNumber.DarkKnight)
                .WithMonsterKillRequirement(30, 23, this.Context, this.GameConfiguration)
                .WithExperienceReward(28000, this.Context);
            this.CreateQuest("Inland Supply Route!", 18, 25, 56, 65, 257, CharacterClassNumber.FairyElf)
                .WithMonsterKillRequirement(30, 23, this.Context, this.GameConfiguration)
                .WithExperienceReward(28000, this.Context);
            this.CreateQuest("Inland Supply Route!", 18, 25, 56, 65, 257, CharacterClassNumber.Summoner)
                .WithMonsterKillRequirement(30, 23, this.Context, this.GameConfiguration)
                .WithExperienceReward(28000, this.Context);

            this.CreateQuest("Dungeon Sweep!", 18, 28, 66, 79, 257, CharacterClassNumber.MagicGladiator)
                .WithMonsterKillRequirement(40, 17, this.Context, this.GameConfiguration)
                .WithExperienceReward(36000, this.Context);
            this.CreateQuest("Dungeon Sweep!", 18, 28, 66, 79, 257, CharacterClassNumber.DarkLord)
                .WithMonsterKillRequirement(40, 17, this.Context, this.GameConfiguration)
                .WithExperienceReward(36000, this.Context);
            this.CreateQuest("Dungeon Sweep!", 18, 28, 66, 79, 257, CharacterClassNumber.RageFighter)
                .WithMonsterKillRequirement(40, 17, this.Context, this.GameConfiguration)
                .WithExperienceReward(36000, this.Context);

            this.CreateQuest("Dungeon Sweep!", 18, 29, 66, 79, 257, CharacterClassNumber.DarkWizard)
                .WithMonsterKillRequirement(40, 12, this.Context, this.GameConfiguration)
                .WithExperienceReward(36000, this.Context);
            this.CreateQuest("Dungeon Sweep!", 18, 29, 66, 79, 257, CharacterClassNumber.DarkKnight)
                .WithMonsterKillRequirement(40, 12, this.Context, this.GameConfiguration)
                .WithExperienceReward(36000, this.Context);
            this.CreateQuest("Dungeon Sweep!", 18, 29, 66, 79, 257, CharacterClassNumber.FairyElf)
                .WithMonsterKillRequirement(40, 12, this.Context, this.GameConfiguration)
                .WithExperienceReward(36000, this.Context);
            this.CreateQuest("Dungeon Sweep!", 18, 29, 66, 79, 257, CharacterClassNumber.Summoner)
                .WithMonsterKillRequirement(40, 12, this.Context, this.GameConfiguration)
                .WithExperienceReward(36000, this.Context);

            this.CreateQuest("and asked the elders of the Elbe!", 18, 32, 80, 90, 257, CharacterClassNumber.Summoner)
                .WithMonsterKillRequirement(50, 423, this.Context, this.GameConfiguration)
                .WithExperienceReward(38000, this.Context);

            this.CreateQuest("and asked the elders of the Elbe!", 18, 33, 80, 90, 257, CharacterClassNumber.DarkWizard)
                .WithMonsterKillRequirement(50, 422, this.Context, this.GameConfiguration)
                .WithExperienceReward(38000, this.Context);
            this.CreateQuest("and asked the elders of the Elbe!", 18, 33, 80, 90, 257, CharacterClassNumber.DarkKnight)
                .WithMonsterKillRequirement(50, 422, this.Context, this.GameConfiguration)
                .WithExperienceReward(38000, this.Context);
            this.CreateQuest("and asked the elders of the Elbe!", 18, 33, 80, 90, 257, CharacterClassNumber.FairyElf)
                .WithMonsterKillRequirement(50, 422, this.Context, this.GameConfiguration)
                .WithExperienceReward(38000, this.Context);
            this.CreateQuest("and asked the elders of the Elbe!", 18, 33, 80, 90, 257, CharacterClassNumber.MagicGladiator)
                .WithMonsterKillRequirement(50, 422, this.Context, this.GameConfiguration)
                .WithExperienceReward(38000, this.Context);
            this.CreateQuest("and asked the elders of the Elbe!", 18, 33, 80, 90, 257, CharacterClassNumber.DarkLord)
                .WithMonsterKillRequirement(50, 422, this.Context, this.GameConfiguration)
                .WithExperienceReward(38000, this.Context);
            this.CreateQuest("and asked the elders of the Elbe!", 18, 33, 80, 90, 257, CharacterClassNumber.RageFighter)
                .WithMonsterKillRequirement(50, 422, this.Context, this.GameConfiguration)
                .WithExperienceReward(38000, this.Context);

            this.CreateQuest("Situation at the Dungeon (1)", 18, 36, 91, 100, 257, CharacterClassNumber.DarkWizard)
                .WithMonsterKillRequirement(50, 5, this.Context, this.GameConfiguration)
                .WithExperienceReward(43000, this.Context);
            this.CreateQuest("Situation at the Dungeon (1)", 18, 36, 91, 100, 257, CharacterClassNumber.DarkKnight)
                .WithMonsterKillRequirement(50, 5, this.Context, this.GameConfiguration)
                .WithExperienceReward(43000, this.Context);
            this.CreateQuest("Situation at the Dungeon (1)", 18, 36, 91, 100, 257, CharacterClassNumber.FairyElf)
                .WithMonsterKillRequirement(50, 5, this.Context, this.GameConfiguration)
                .WithExperienceReward(43000, this.Context);
            this.CreateQuest("Situation at the Dungeon (1)", 18, 36, 91, 100, 257, CharacterClassNumber.Summoner)
                .WithMonsterKillRequirement(50, 5, this.Context, this.GameConfiguration)
                .WithExperienceReward(43000, this.Context);

            this.CreateQuest("Situation at the Dungeon (1)", 18, 37, 91, 100, 257, CharacterClassNumber.MagicGladiator)
                .WithMonsterKillRequirement(50, 9, this.Context, this.GameConfiguration)
                .WithExperienceReward(43000, this.Context);
            this.CreateQuest("Situation at the Dungeon (1)", 18, 37, 91, 100, 257, CharacterClassNumber.DarkLord)
                .WithMonsterKillRequirement(50, 9, this.Context, this.GameConfiguration)
                .WithExperienceReward(43000, this.Context);
            this.CreateQuest("Situation at the Dungeon (1)", 18, 37, 91, 100, 257, CharacterClassNumber.RageFighter)
                .WithMonsterKillRequirement(50, 9, this.Context, this.GameConfiguration)
                .WithExperienceReward(43000, this.Context);

            this.CreateQuest("Situation at the Dungeon (2)", 18, 40, 101, 110, 257, CharacterClassNumber.MagicGladiator)
                .WithMonsterKillRequirement(50, 13, this.Context, this.GameConfiguration)
                .WithExperienceReward(50000, this.Context);
            this.CreateQuest("Situation at the Dungeon (2)", 18, 40, 101, 110, 257, CharacterClassNumber.DarkLord)
                .WithMonsterKillRequirement(50, 13, this.Context, this.GameConfiguration)
                .WithExperienceReward(50000, this.Context);
            this.CreateQuest("Situation at the Dungeon (2)", 18, 40, 101, 110, 257, CharacterClassNumber.RageFighter)
                .WithMonsterKillRequirement(50, 13, this.Context, this.GameConfiguration)
                .WithExperienceReward(50000, this.Context);

            this.CreateQuest("Situation at the Dungeon (2)", 18, 41, 101, 110, 257, CharacterClassNumber.DarkWizard)
                .WithMonsterKillRequirement(50, 15, this.Context, this.GameConfiguration)
                .WithExperienceReward(50000, this.Context);
            this.CreateQuest("Situation at the Dungeon (2)", 18, 41, 101, 110, 257, CharacterClassNumber.DarkKnight)
                .WithMonsterKillRequirement(50, 15, this.Context, this.GameConfiguration)
                .WithExperienceReward(50000, this.Context);
            this.CreateQuest("Situation at the Dungeon (2)", 18, 41, 101, 110, 257, CharacterClassNumber.FairyElf)
                .WithMonsterKillRequirement(50, 15, this.Context, this.GameConfiguration)
                .WithExperienceReward(50000, this.Context);
            this.CreateQuest("Situation at the Dungeon (2)", 18, 41, 101, 110, 257, CharacterClassNumber.Summoner)
                .WithMonsterKillRequirement(50, 15, this.Context, this.GameConfiguration)
                .WithExperienceReward(50000, this.Context);

            this.CreateQuest("Situation at the Dungeon (3)", 18, 44, 111, 120, 257, CharacterClassNumber.MagicGladiator)
                .WithMonsterKillRequirement(50, 9, this.Context, this.GameConfiguration)
                .WithExperienceReward(54000, this.Context);
            this.CreateQuest("Situation at the Dungeon (3)", 18, 44, 111, 120, 257, CharacterClassNumber.DarkLord)
                .WithMonsterKillRequirement(50, 9, this.Context, this.GameConfiguration)
                .WithExperienceReward(54000, this.Context);
            this.CreateQuest("Situation at the Dungeon (3)", 18, 44, 111, 120, 257, CharacterClassNumber.RageFighter)
                .WithMonsterKillRequirement(50, 9, this.Context, this.GameConfiguration)
                .WithExperienceReward(54000, this.Context);

            this.CreateQuest("Situation at the Dungeon (3)", 18, 45, 111, 120, 257, CharacterClassNumber.DarkWizard)
                .WithMonsterKillRequirement(50, 5, this.Context, this.GameConfiguration)
                .WithExperienceReward(54000, this.Context);
            this.CreateQuest("Situation at the Dungeon (3)", 18, 45, 111, 120, 257, CharacterClassNumber.DarkKnight)
                .WithMonsterKillRequirement(50, 5, this.Context, this.GameConfiguration)
                .WithExperienceReward(54000, this.Context);
            this.CreateQuest("Situation at the Dungeon (3)", 18, 45, 111, 120, 257, CharacterClassNumber.FairyElf)
                .WithMonsterKillRequirement(50, 5, this.Context, this.GameConfiguration)
                .WithExperienceReward(54000, this.Context);
            this.CreateQuest("Situation at the Dungeon (3)", 18, 45, 111, 120, 257, CharacterClassNumber.Summoner)
                .WithMonsterKillRequirement(50, 5, this.Context, this.GameConfiguration)
                .WithExperienceReward(54000, this.Context);

            this.CreateQuest("Situation at the Dungeon (4)", 18, 48, 121, 130, 257, CharacterClassNumber.MagicGladiator)
                .WithMonsterKillRequirement(50, 8, this.Context, this.GameConfiguration)
                .WithExperienceReward(60000, this.Context);
            this.CreateQuest("Situation at the Dungeon (4)", 18, 48, 121, 130, 257, CharacterClassNumber.DarkLord)
                .WithMonsterKillRequirement(50, 8, this.Context, this.GameConfiguration)
                .WithExperienceReward(60000, this.Context);
            this.CreateQuest("Situation at the Dungeon (4)", 18, 48, 121, 130, 257, CharacterClassNumber.RageFighter)
                .WithMonsterKillRequirement(50, 8, this.Context, this.GameConfiguration)
                .WithExperienceReward(60000, this.Context);

            this.CreateQuest("Situation at the Dungeon (4)", 18, 49, 121, 130, 257, CharacterClassNumber.DarkWizard)
                .WithMonsterKillRequirement(50, 13, this.Context, this.GameConfiguration)
                .WithExperienceReward(60000, this.Context);
            this.CreateQuest("Situation at the Dungeon (4)", 18, 49, 121, 130, 257, CharacterClassNumber.DarkKnight)
                .WithMonsterKillRequirement(50, 13, this.Context, this.GameConfiguration)
                .WithExperienceReward(60000, this.Context);
            this.CreateQuest("Situation at the Dungeon (4)", 18, 49, 121, 130, 257, CharacterClassNumber.FairyElf)
                .WithMonsterKillRequirement(50, 13, this.Context, this.GameConfiguration)
                .WithExperienceReward(60000, this.Context);
            this.CreateQuest("Situation at the Dungeon (4)", 18, 49, 121, 130, 257, CharacterClassNumber.Summoner)
                .WithMonsterKillRequirement(50, 13, this.Context, this.GameConfiguration)
                .WithExperienceReward(60000, this.Context);

            this.CreateQuest("Situation at the Dungeon (5)", 18, 52, 131, 140, 257, CharacterClassNumber.MagicGladiator)
                .WithMonsterKillRequirement(50, 10, this.Context, this.GameConfiguration)
                .WithExperienceReward(66000, this.Context);
            this.CreateQuest("Situation at the Dungeon (5)", 18, 52, 131, 140, 257, CharacterClassNumber.DarkLord)
                .WithMonsterKillRequirement(50, 10, this.Context, this.GameConfiguration)
                .WithExperienceReward(66000, this.Context);
            this.CreateQuest("Situation at the Dungeon (5)", 18, 52, 131, 140, 257, CharacterClassNumber.RageFighter)
                .WithMonsterKillRequirement(50, 10, this.Context, this.GameConfiguration)
                .WithExperienceReward(66000, this.Context);

            this.CreateQuest("Situation at the Dungeon (5)", 18, 53, 131, 140, 257, CharacterClassNumber.DarkWizard)
                .WithMonsterKillRequirement(50, 9, this.Context, this.GameConfiguration)
                .WithExperienceReward(66000, this.Context);
            this.CreateQuest("Situation at the Dungeon (5)", 18, 53, 131, 140, 257, CharacterClassNumber.DarkKnight)
                .WithMonsterKillRequirement(50, 9, this.Context, this.GameConfiguration)
                .WithExperienceReward(66000, this.Context);
            this.CreateQuest("Situation at the Dungeon (5)", 18, 53, 131, 140, 257, CharacterClassNumber.FairyElf)
                .WithMonsterKillRequirement(50, 9, this.Context, this.GameConfiguration)
                .WithExperienceReward(66000, this.Context);
            this.CreateQuest("Situation at the Dungeon (5)", 18, 53, 131, 140, 257, CharacterClassNumber.Summoner)
                .WithMonsterKillRequirement(50, 9, this.Context, this.GameConfiguration)
                .WithExperienceReward(66000, this.Context);

            this.CreateQuest("Situation at the Dungeon (6)", 18, 56, 141, 160, 257, CharacterClassNumber.MagicGladiator)
                .WithMonsterKillRequirement(50, 18, this.Context, this.GameConfiguration)
                .WithExperienceReward(72000, this.Context);
            this.CreateQuest("Situation at the Dungeon (6)", 18, 56, 141, 160, 257, CharacterClassNumber.DarkLord)
                .WithMonsterKillRequirement(50, 18, this.Context, this.GameConfiguration)
                .WithExperienceReward(72000, this.Context);
            this.CreateQuest("Situation at the Dungeon (6)", 18, 56, 141, 160, 257, CharacterClassNumber.RageFighter)
                .WithMonsterKillRequirement(50, 18, this.Context, this.GameConfiguration)
                .WithExperienceReward(72000, this.Context);

            this.CreateQuest("Situation at the Dungeon (6)", 18, 57, 141, 160, 257, CharacterClassNumber.SoulMaster)
                .WithMonsterKillRequirement(15, 8, this.Context, this.GameConfiguration)
                .WithExperienceReward(72000, this.Context);
            this.CreateQuest("Situation at the Dungeon (6)", 18, 57, 141, 160, 257, CharacterClassNumber.DarkWizard)
                .WithMonsterKillRequirement(15, 8, this.Context, this.GameConfiguration)
                .WithExperienceReward(72000, this.Context);
            this.CreateQuest("Situation at the Dungeon (6)", 18, 57, 141, 160, 257, CharacterClassNumber.BladeKnight)
                .WithMonsterKillRequirement(15, 8, this.Context, this.GameConfiguration)
                .WithExperienceReward(72000, this.Context);
            this.CreateQuest("Situation at the Dungeon (6)", 18, 57, 141, 160, 257, CharacterClassNumber.DarkKnight)
                .WithMonsterKillRequirement(15, 8, this.Context, this.GameConfiguration)
                .WithExperienceReward(72000, this.Context);
            this.CreateQuest("Situation at the Dungeon (6)", 18, 57, 141, 160, 257, CharacterClassNumber.MuseElf)
                .WithMonsterKillRequirement(15, 8, this.Context, this.GameConfiguration)
                .WithExperienceReward(72000, this.Context);
            this.CreateQuest("Situation at the Dungeon (6)", 18, 57, 141, 160, 257, CharacterClassNumber.FairyElf)
                .WithMonsterKillRequirement(15, 8, this.Context, this.GameConfiguration)
                .WithExperienceReward(72000, this.Context);
            this.CreateQuest("Situation at the Dungeon (6)", 18, 57, 141, 160, 257, CharacterClassNumber.BloodySummoner)
                .WithMonsterKillRequirement(15, 8, this.Context, this.GameConfiguration)
                .WithExperienceReward(72000, this.Context);
            this.CreateQuest("Situation at the Dungeon (6)", 18, 57, 141, 160, 257, CharacterClassNumber.Summoner)
                .WithMonsterKillRequirement(15, 8, this.Context, this.GameConfiguration)
                .WithExperienceReward(72000, this.Context);

            this.CreateQuest("Continuing Requests for Help (1)", 18, 60, 161, 165, 257, CharacterClassNumber.MagicGladiator)
                .WithMonsterKillRequirement(50, 36, this.Context, this.GameConfiguration)
                .WithExperienceReward(80000, this.Context);
            this.CreateQuest("Continuing Requests for Help (1)", 18, 60, 161, 165, 257, CharacterClassNumber.DarkLord)
                .WithMonsterKillRequirement(50, 36, this.Context, this.GameConfiguration)
                .WithExperienceReward(80000, this.Context);
            this.CreateQuest("Continuing Requests for Help (1)", 18, 60, 161, 165, 257, CharacterClassNumber.RageFighter)
                .WithMonsterKillRequirement(50, 36, this.Context, this.GameConfiguration)
                .WithExperienceReward(80000, this.Context);

            this.CreateQuest("Continuing Requests for Help (1)", 18, 61, 161, 165, 257, CharacterClassNumber.SoulMaster)
                .WithMonsterKillRequirement(3, 25, this.Context, this.GameConfiguration)
                .WithExperienceReward(80000, this.Context);
            this.CreateQuest("Continuing Requests for Help (1)", 18, 61, 161, 165, 257, CharacterClassNumber.DarkWizard)
                .WithMonsterKillRequirement(3, 25, this.Context, this.GameConfiguration)
                .WithExperienceReward(80000, this.Context);
            this.CreateQuest("Continuing Requests for Help (1)", 18, 61, 161, 165, 257, CharacterClassNumber.BladeKnight)
                .WithMonsterKillRequirement(3, 25, this.Context, this.GameConfiguration)
                .WithExperienceReward(80000, this.Context);
            this.CreateQuest("Continuing Requests for Help (1)", 18, 61, 161, 165, 257, CharacterClassNumber.DarkKnight)
                .WithMonsterKillRequirement(3, 25, this.Context, this.GameConfiguration)
                .WithExperienceReward(80000, this.Context);
            this.CreateQuest("Continuing Requests for Help (1)", 18, 61, 161, 165, 257, CharacterClassNumber.MuseElf)
                .WithMonsterKillRequirement(3, 25, this.Context, this.GameConfiguration)
                .WithExperienceReward(80000, this.Context);
            this.CreateQuest("Continuing Requests for Help (1)", 18, 61, 161, 165, 257, CharacterClassNumber.FairyElf)
                .WithMonsterKillRequirement(3, 25, this.Context, this.GameConfiguration)
                .WithExperienceReward(80000, this.Context);
            this.CreateQuest("Continuing Requests for Help (1)", 18, 61, 161, 165, 257, CharacterClassNumber.BloodySummoner)
                .WithMonsterKillRequirement(3, 25, this.Context, this.GameConfiguration)
                .WithExperienceReward(80000, this.Context);
            this.CreateQuest("Continuing Requests for Help (1)", 18, 61, 161, 165, 257, CharacterClassNumber.Summoner)
                .WithMonsterKillRequirement(3, 25, this.Context, this.GameConfiguration)
                .WithExperienceReward(80000, this.Context);

            this.CreateQuest("Continuing Requests for Help (2)", 18, 64, 166, 170, 257, CharacterClassNumber.MagicGladiator)
                .WithMonsterKillRequirement(50, 39, this.Context, this.GameConfiguration)
                .WithExperienceReward(82000, this.Context);
            this.CreateQuest("Continuing Requests for Help (2)", 18, 64, 166, 170, 257, CharacterClassNumber.DarkLord)
                .WithMonsterKillRequirement(50, 39, this.Context, this.GameConfiguration)
                .WithExperienceReward(82000, this.Context);
            this.CreateQuest("Continuing Requests for Help (2)", 18, 64, 166, 170, 257, CharacterClassNumber.RageFighter)
                .WithMonsterKillRequirement(50, 39, this.Context, this.GameConfiguration)
                .WithExperienceReward(82000, this.Context);

            this.CreateQuest("Continuing Requests for Help (2)", 18, 65, 166, 170, 257, CharacterClassNumber.SoulMaster)
                .WithMonsterKillRequirement(50, 424, this.Context, this.GameConfiguration)
                .WithExperienceReward(82000, this.Context);
            this.CreateQuest("Continuing Requests for Help (2)", 18, 65, 166, 170, 257, CharacterClassNumber.DarkWizard)
                .WithMonsterKillRequirement(50, 424, this.Context, this.GameConfiguration)
                .WithExperienceReward(82000, this.Context);
            this.CreateQuest("Continuing Requests for Help (2)", 18, 65, 166, 170, 257, CharacterClassNumber.BladeKnight)
                .WithMonsterKillRequirement(50, 424, this.Context, this.GameConfiguration)
                .WithExperienceReward(82000, this.Context);
            this.CreateQuest("Continuing Requests for Help (2)", 18, 65, 166, 170, 257, CharacterClassNumber.DarkKnight)
                .WithMonsterKillRequirement(50, 424, this.Context, this.GameConfiguration)
                .WithExperienceReward(82000, this.Context);
            this.CreateQuest("Continuing Requests for Help (2)", 18, 65, 166, 170, 257, CharacterClassNumber.MuseElf)
                .WithMonsterKillRequirement(50, 424, this.Context, this.GameConfiguration)
                .WithExperienceReward(82000, this.Context);
            this.CreateQuest("Continuing Requests for Help (2)", 18, 65, 166, 170, 257, CharacterClassNumber.FairyElf)
                .WithMonsterKillRequirement(50, 424, this.Context, this.GameConfiguration)
                .WithExperienceReward(82000, this.Context);
            this.CreateQuest("Continuing Requests for Help (2)", 18, 65, 166, 170, 257, CharacterClassNumber.BloodySummoner)
                .WithMonsterKillRequirement(50, 424, this.Context, this.GameConfiguration)
                .WithExperienceReward(82000, this.Context);
            this.CreateQuest("Continuing Requests for Help (2)", 18, 65, 166, 170, 257, CharacterClassNumber.Summoner)
                .WithMonsterKillRequirement(50, 424, this.Context, this.GameConfiguration)
                .WithExperienceReward(82000, this.Context);

            this.CreateQuest("Continuing Requests for Help (3)", 18, 68, 171, 179, 257, CharacterClassNumber.MagicGladiator)
                .WithMonsterKillRequirement(50, 34, this.Context, this.GameConfiguration)
                .WithExperienceReward(84000, this.Context);
            this.CreateQuest("Continuing Requests for Help (3)", 18, 68, 171, 179, 257, CharacterClassNumber.DarkLord)
                .WithMonsterKillRequirement(50, 34, this.Context, this.GameConfiguration)
                .WithExperienceReward(84000, this.Context);
            this.CreateQuest("Continuing Requests for Help (3)", 18, 68, 171, 179, 257, CharacterClassNumber.RageFighter)
                .WithMonsterKillRequirement(50, 34, this.Context, this.GameConfiguration)
                .WithExperienceReward(84000, this.Context);

            this.CreateQuest("Continuing Requests for Help (3)", 18, 69, 171, 179, 257, CharacterClassNumber.SoulMaster)
                .WithMonsterKillRequirement(50, 425, this.Context, this.GameConfiguration)
                .WithExperienceReward(84000, this.Context);
            this.CreateQuest("Continuing Requests for Help (3)", 18, 69, 171, 179, 257, CharacterClassNumber.DarkWizard)
                .WithMonsterKillRequirement(50, 425, this.Context, this.GameConfiguration)
                .WithExperienceReward(84000, this.Context);
            this.CreateQuest("Continuing Requests for Help (3)", 18, 69, 171, 179, 257, CharacterClassNumber.BladeKnight)
                .WithMonsterKillRequirement(50, 425, this.Context, this.GameConfiguration)
                .WithExperienceReward(84000, this.Context);
            this.CreateQuest("Continuing Requests for Help (3)", 18, 69, 171, 179, 257, CharacterClassNumber.DarkKnight)
                .WithMonsterKillRequirement(50, 425, this.Context, this.GameConfiguration)
                .WithExperienceReward(84000, this.Context);
            this.CreateQuest("Continuing Requests for Help (3)", 18, 69, 171, 179, 257, CharacterClassNumber.MuseElf)
                .WithMonsterKillRequirement(50, 425, this.Context, this.GameConfiguration)
                .WithExperienceReward(84000, this.Context);
            this.CreateQuest("Continuing Requests for Help (3)", 18, 69, 171, 179, 257, CharacterClassNumber.FairyElf)
                .WithMonsterKillRequirement(50, 425, this.Context, this.GameConfiguration)
                .WithExperienceReward(84000, this.Context);
            this.CreateQuest("Continuing Requests for Help (3)", 18, 69, 171, 179, 257, CharacterClassNumber.BloodySummoner)
                .WithMonsterKillRequirement(50, 425, this.Context, this.GameConfiguration)
                .WithExperienceReward(84000, this.Context);
            this.CreateQuest("Continuing Requests for Help (3)", 18, 69, 171, 179, 257, CharacterClassNumber.Summoner)
                .WithMonsterKillRequirement(50, 425, this.Context, this.GameConfiguration)
                .WithExperienceReward(84000, this.Context);

            this.CreateQuest("Continuing Requests for Help (4)", 18, 72, 180, 189, 257, CharacterClassNumber.MagicGladiator)
                .WithMonsterKillRequirement(50, 41, this.Context, this.GameConfiguration)
                .WithExperienceReward(95000, this.Context);
            this.CreateQuest("Continuing Requests for Help (4)", 18, 72, 180, 189, 257, CharacterClassNumber.DarkLord)
                .WithMonsterKillRequirement(50, 41, this.Context, this.GameConfiguration)
                .WithExperienceReward(95000, this.Context);
            this.CreateQuest("Continuing Requests for Help (4)", 18, 72, 180, 189, 257, CharacterClassNumber.RageFighter)
                .WithMonsterKillRequirement(50, 41, this.Context, this.GameConfiguration)
                .WithExperienceReward(95000, this.Context);

            this.CreateQuest("Continuing Requests for Help (4)", 18, 73, 180, 189, 257, CharacterClassNumber.SoulMaster)
                .WithMonsterKillRequirement(50, 36, this.Context, this.GameConfiguration)
                .WithExperienceReward(95000, this.Context);
            this.CreateQuest("Continuing Requests for Help (4)", 18, 73, 180, 189, 257, CharacterClassNumber.DarkWizard)
                .WithMonsterKillRequirement(50, 36, this.Context, this.GameConfiguration)
                .WithExperienceReward(95000, this.Context);
            this.CreateQuest("Continuing Requests for Help (4)", 18, 73, 180, 189, 257, CharacterClassNumber.BladeKnight)
                .WithMonsterKillRequirement(50, 36, this.Context, this.GameConfiguration)
                .WithExperienceReward(95000, this.Context);
            this.CreateQuest("Continuing Requests for Help (4)", 18, 73, 180, 189, 257, CharacterClassNumber.DarkKnight)
                .WithMonsterKillRequirement(50, 36, this.Context, this.GameConfiguration)
                .WithExperienceReward(95000, this.Context);
            this.CreateQuest("Continuing Requests for Help (4)", 18, 73, 180, 189, 257, CharacterClassNumber.MuseElf)
                .WithMonsterKillRequirement(50, 36, this.Context, this.GameConfiguration)
                .WithExperienceReward(95000, this.Context);
            this.CreateQuest("Continuing Requests for Help (4)", 18, 73, 180, 189, 257, CharacterClassNumber.FairyElf)
                .WithMonsterKillRequirement(50, 36, this.Context, this.GameConfiguration)
                .WithExperienceReward(95000, this.Context);
            this.CreateQuest("Continuing Requests for Help (4)", 18, 73, 180, 189, 257, CharacterClassNumber.BloodySummoner)
                .WithMonsterKillRequirement(50, 36, this.Context, this.GameConfiguration)
                .WithExperienceReward(95000, this.Context);
            this.CreateQuest("Continuing Requests for Help (4)", 18, 73, 180, 189, 257, CharacterClassNumber.Summoner)
                .WithMonsterKillRequirement(50, 36, this.Context, this.GameConfiguration)
                .WithExperienceReward(95000, this.Context);

            this.CreateQuest("Continuing Attacks on Barriers (1)", 18, 76, 190, 199, 257, CharacterClassNumber.MagicGladiator)
                .WithMonsterKillRequirement(50, 37, this.Context, this.GameConfiguration)
                .WithExperienceReward(100000, this.Context);
            this.CreateQuest("Continuing Attacks on Barriers (1)", 18, 76, 190, 199, 257, CharacterClassNumber.DarkLord)
                .WithMonsterKillRequirement(50, 37, this.Context, this.GameConfiguration)
                .WithExperienceReward(100000, this.Context);
            this.CreateQuest("Continuing Attacks on Barriers (1)", 18, 76, 190, 199, 257, CharacterClassNumber.RageFighter)
                .WithMonsterKillRequirement(50, 37, this.Context, this.GameConfiguration)
                .WithExperienceReward(100000, this.Context);

            this.CreateQuest("Continuing Attacks on Barriers (1)", 18, 77, 190, 199, 257, CharacterClassNumber.SoulMaster)
                .WithMonsterKillRequirement(50, 34, this.Context, this.GameConfiguration)
                .WithExperienceReward(100000, this.Context);
            this.CreateQuest("Continuing Attacks on Barriers (1)", 18, 77, 190, 199, 257, CharacterClassNumber.DarkWizard)
                .WithMonsterKillRequirement(50, 34, this.Context, this.GameConfiguration)
                .WithExperienceReward(100000, this.Context);
            this.CreateQuest("Continuing Attacks on Barriers (1)", 18, 77, 190, 199, 257, CharacterClassNumber.BladeKnight)
                .WithMonsterKillRequirement(50, 34, this.Context, this.GameConfiguration)
                .WithExperienceReward(100000, this.Context);
            this.CreateQuest("Continuing Attacks on Barriers (1)", 18, 77, 190, 199, 257, CharacterClassNumber.DarkKnight)
                .WithMonsterKillRequirement(50, 34, this.Context, this.GameConfiguration)
                .WithExperienceReward(100000, this.Context);
            this.CreateQuest("Continuing Attacks on Barriers (1)", 18, 77, 190, 199, 257, CharacterClassNumber.MuseElf)
                .WithMonsterKillRequirement(50, 34, this.Context, this.GameConfiguration)
                .WithExperienceReward(100000, this.Context);
            this.CreateQuest("Continuing Attacks on Barriers (1)", 18, 77, 190, 199, 257, CharacterClassNumber.FairyElf)
                .WithMonsterKillRequirement(50, 34, this.Context, this.GameConfiguration)
                .WithExperienceReward(100000, this.Context);
            this.CreateQuest("Continuing Attacks on Barriers (1)", 18, 77, 190, 199, 257, CharacterClassNumber.BloodySummoner)
                .WithMonsterKillRequirement(50, 34, this.Context, this.GameConfiguration)
                .WithExperienceReward(100000, this.Context);
            this.CreateQuest("Continuing Attacks on Barriers (1)", 18, 77, 190, 199, 257, CharacterClassNumber.Summoner)
                .WithMonsterKillRequirement(50, 34, this.Context, this.GameConfiguration)
                .WithExperienceReward(100000, this.Context);

            this.CreateQuest("Continuing Attacks on Barriers (2)", 18, 80, 200, 209, 257, CharacterClassNumber.MagicGladiator)
                .WithMonsterKillRequirement(50, 40, this.Context, this.GameConfiguration)
                .WithExperienceReward(115000, this.Context);
            this.CreateQuest("Continuing Attacks on Barriers (2)", 18, 80, 200, 209, 257, CharacterClassNumber.DarkLord)
                .WithMonsterKillRequirement(50, 40, this.Context, this.GameConfiguration)
                .WithExperienceReward(115000, this.Context);
            this.CreateQuest("Continuing Attacks on Barriers (2)", 18, 80, 200, 209, 257, CharacterClassNumber.RageFighter)
                .WithMonsterKillRequirement(50, 40, this.Context, this.GameConfiguration)
                .WithExperienceReward(115000, this.Context);

            this.CreateQuest("Continuing Attacks on Barriers (2)", 18, 81, 200, 209, 257, CharacterClassNumber.SoulMaster)
                .WithMonsterKillRequirement(50, 41, this.Context, this.GameConfiguration)
                .WithExperienceReward(115000, this.Context);
            this.CreateQuest("Continuing Attacks on Barriers (2)", 18, 81, 200, 209, 257, CharacterClassNumber.DarkWizard)
                .WithMonsterKillRequirement(50, 41, this.Context, this.GameConfiguration)
                .WithExperienceReward(115000, this.Context);
            this.CreateQuest("Continuing Attacks on Barriers (2)", 18, 81, 200, 209, 257, CharacterClassNumber.BladeKnight)
                .WithMonsterKillRequirement(50, 41, this.Context, this.GameConfiguration)
                .WithExperienceReward(115000, this.Context);
            this.CreateQuest("Continuing Attacks on Barriers (2)", 18, 81, 200, 209, 257, CharacterClassNumber.DarkKnight)
                .WithMonsterKillRequirement(50, 41, this.Context, this.GameConfiguration)
                .WithExperienceReward(115000, this.Context);
            this.CreateQuest("Continuing Attacks on Barriers (2)", 18, 81, 200, 209, 257, CharacterClassNumber.MuseElf)
                .WithMonsterKillRequirement(50, 41, this.Context, this.GameConfiguration)
                .WithExperienceReward(115000, this.Context);
            this.CreateQuest("Continuing Attacks on Barriers (2)", 18, 81, 200, 209, 257, CharacterClassNumber.FairyElf)
                .WithMonsterKillRequirement(50, 41, this.Context, this.GameConfiguration)
                .WithExperienceReward(115000, this.Context);
            this.CreateQuest("Continuing Attacks on Barriers (2)", 18, 81, 200, 209, 257, CharacterClassNumber.BloodySummoner)
                .WithMonsterKillRequirement(50, 41, this.Context, this.GameConfiguration)
                .WithExperienceReward(115000, this.Context);
            this.CreateQuest("Continuing Attacks on Barriers (2)", 18, 81, 200, 209, 257, CharacterClassNumber.Summoner)
                .WithMonsterKillRequirement(50, 41, this.Context, this.GameConfiguration)
                .WithExperienceReward(115000, this.Context);

            this.CreateQuest("Continuing Attacks on Barriers (3)", 18, 84, 210, 219, 257, CharacterClassNumber.MagicGladiator)
                .WithMonsterKillRequirement(60, 35, this.Context, this.GameConfiguration)
                .WithExperienceReward(130000, this.Context);
            this.CreateQuest("Continuing Attacks on Barriers (3)", 18, 84, 210, 219, 257, CharacterClassNumber.DarkLord)
                .WithMonsterKillRequirement(60, 35, this.Context, this.GameConfiguration)
                .WithExperienceReward(130000, this.Context);
            this.CreateQuest("Continuing Attacks on Barriers (3)", 18, 84, 210, 219, 257, CharacterClassNumber.RageFighter)
                .WithMonsterKillRequirement(60, 35, this.Context, this.GameConfiguration)
                .WithExperienceReward(130000, this.Context);

            this.CreateQuest("Continuing Attacks on Barriers (3)", 18, 85, 210, 219, 257, CharacterClassNumber.SoulMaster)
                .WithMonsterKillRequirement(60, 37, this.Context, this.GameConfiguration)
                .WithExperienceReward(130000, this.Context);
            this.CreateQuest("Continuing Attacks on Barriers (3)", 18, 85, 210, 219, 257, CharacterClassNumber.DarkWizard)
                .WithMonsterKillRequirement(60, 37, this.Context, this.GameConfiguration)
                .WithExperienceReward(130000, this.Context);
            this.CreateQuest("Continuing Attacks on Barriers (3)", 18, 85, 210, 219, 257, CharacterClassNumber.BladeKnight)
                .WithMonsterKillRequirement(60, 37, this.Context, this.GameConfiguration)
                .WithExperienceReward(130000, this.Context);
            this.CreateQuest("Continuing Attacks on Barriers (3)", 18, 85, 210, 219, 257, CharacterClassNumber.DarkKnight)
                .WithMonsterKillRequirement(60, 37, this.Context, this.GameConfiguration)
                .WithExperienceReward(130000, this.Context);
            this.CreateQuest("Continuing Attacks on Barriers (3)", 18, 85, 210, 219, 257, CharacterClassNumber.MuseElf)
                .WithMonsterKillRequirement(60, 37, this.Context, this.GameConfiguration)
                .WithExperienceReward(130000, this.Context);
            this.CreateQuest("Continuing Attacks on Barriers (3)", 18, 85, 210, 219, 257, CharacterClassNumber.FairyElf)
                .WithMonsterKillRequirement(60, 37, this.Context, this.GameConfiguration)
                .WithExperienceReward(130000, this.Context);
            this.CreateQuest("Continuing Attacks on Barriers (3)", 18, 85, 210, 219, 257, CharacterClassNumber.BloodySummoner)
                .WithMonsterKillRequirement(60, 37, this.Context, this.GameConfiguration)
                .WithExperienceReward(130000, this.Context);
            this.CreateQuest("Continuing Attacks on Barriers (3)", 18, 85, 210, 219, 257, CharacterClassNumber.Summoner)
                .WithMonsterKillRequirement(60, 37, this.Context, this.GameConfiguration)
                .WithExperienceReward(130000, this.Context);

            this.CreateQuest("Clearing a Path to Icarus", 18, 88, 220, 229, 257, CharacterClassNumber.MagicGladiator)
                .WithMonsterKillRequirement(15, 38, this.Context, this.GameConfiguration)
                .WithExperienceReward(140000, this.Context);
            this.CreateQuest("Clearing a Path to Icarus", 18, 88, 220, 229, 257, CharacterClassNumber.DarkLord)
                .WithMonsterKillRequirement(15, 38, this.Context, this.GameConfiguration)
                .WithExperienceReward(140000, this.Context);
            this.CreateQuest("Clearing a Path to Icarus", 18, 88, 220, 229, 257, CharacterClassNumber.RageFighter)
                .WithMonsterKillRequirement(15, 38, this.Context, this.GameConfiguration)
                .WithExperienceReward(140000, this.Context);

            this.CreateQuest("Clearing a Path to Icarus", 18, 89, 220, 229, 257, CharacterClassNumber.SoulMaster)
                .WithMonsterKillRequirement(60, 40, this.Context, this.GameConfiguration)
                .WithExperienceReward(140000, this.Context);
            this.CreateQuest("Clearing a Path to Icarus", 18, 89, 220, 229, 257, CharacterClassNumber.DarkWizard)
                .WithMonsterKillRequirement(60, 40, this.Context, this.GameConfiguration)
                .WithExperienceReward(140000, this.Context);
            this.CreateQuest("Clearing a Path to Icarus", 18, 89, 220, 229, 257, CharacterClassNumber.BladeKnight)
                .WithMonsterKillRequirement(60, 40, this.Context, this.GameConfiguration)
                .WithExperienceReward(140000, this.Context);
            this.CreateQuest("Clearing a Path to Icarus", 18, 89, 220, 229, 257, CharacterClassNumber.DarkKnight)
                .WithMonsterKillRequirement(60, 40, this.Context, this.GameConfiguration)
                .WithExperienceReward(140000, this.Context);
            this.CreateQuest("Clearing a Path to Icarus", 18, 89, 220, 229, 257, CharacterClassNumber.MuseElf)
                .WithMonsterKillRequirement(60, 40, this.Context, this.GameConfiguration)
                .WithExperienceReward(140000, this.Context);
            this.CreateQuest("Clearing a Path to Icarus", 18, 89, 220, 229, 257, CharacterClassNumber.FairyElf)
                .WithMonsterKillRequirement(60, 40, this.Context, this.GameConfiguration)
                .WithExperienceReward(140000, this.Context);
            this.CreateQuest("Clearing a Path to Icarus", 18, 89, 220, 229, 257, CharacterClassNumber.BloodySummoner)
                .WithMonsterKillRequirement(60, 40, this.Context, this.GameConfiguration)
                .WithExperienceReward(140000, this.Context);
            this.CreateQuest("Clearing a Path to Icarus", 18, 89, 220, 229, 257, CharacterClassNumber.Summoner)
                .WithMonsterKillRequirement(60, 40, this.Context, this.GameConfiguration)
                .WithExperienceReward(140000, this.Context);

            this.CreateQuest("Clearing a Path to Icarus", 18, 92, 220, 229, 257, CharacterClassNumber.MagicGladiator)
                .WithMonsterKillRequirement(15, 38, this.Context, this.GameConfiguration)
                .WithItemReward(12, 15, this.Context, this.GameConfiguration);
            this.CreateQuest("Clearing a Path to Icarus", 18, 92, 220, 229, 257, CharacterClassNumber.DarkLord)
                .WithMonsterKillRequirement(15, 38, this.Context, this.GameConfiguration)
                .WithItemReward(12, 15, this.Context, this.GameConfiguration);
            this.CreateQuest("Clearing a Path to Icarus", 18, 92, 220, 229, 257, CharacterClassNumber.RageFighter)
                .WithMonsterKillRequirement(15, 38, this.Context, this.GameConfiguration)
                .WithItemReward(12, 15, this.Context, this.GameConfiguration);

            this.CreateQuest("Clearing a Path to Icarus", 18, 93, 220, 229, 257, CharacterClassNumber.SoulMaster)
                .WithMonsterKillRequirement(60, 40, this.Context, this.GameConfiguration)
                .WithItemReward(12, 15, this.Context, this.GameConfiguration);
            this.CreateQuest("Clearing a Path to Icarus", 18, 93, 220, 229, 257, CharacterClassNumber.DarkWizard)
                .WithMonsterKillRequirement(60, 40, this.Context, this.GameConfiguration)
                .WithItemReward(12, 15, this.Context, this.GameConfiguration);
            this.CreateQuest("Clearing a Path to Icarus", 18, 93, 220, 229, 257, CharacterClassNumber.BladeKnight)
                .WithMonsterKillRequirement(60, 40, this.Context, this.GameConfiguration)
                .WithItemReward(12, 15, this.Context, this.GameConfiguration);
            this.CreateQuest("Clearing a Path to Icarus", 18, 93, 220, 229, 257, CharacterClassNumber.DarkKnight)
                .WithMonsterKillRequirement(60, 40, this.Context, this.GameConfiguration)
                .WithItemReward(12, 15, this.Context, this.GameConfiguration);
            this.CreateQuest("Clearing a Path to Icarus", 18, 93, 220, 229, 257, CharacterClassNumber.MuseElf)
                .WithMonsterKillRequirement(60, 40, this.Context, this.GameConfiguration)
                .WithItemReward(12, 15, this.Context, this.GameConfiguration);
            this.CreateQuest("Clearing a Path to Icarus", 18, 93, 220, 229, 257, CharacterClassNumber.FairyElf)
                .WithMonsterKillRequirement(60, 40, this.Context, this.GameConfiguration)
                .WithItemReward(12, 15, this.Context, this.GameConfiguration);
            this.CreateQuest("Clearing a Path to Icarus", 18, 93, 220, 229, 257, CharacterClassNumber.BloodySummoner)
                .WithMonsterKillRequirement(60, 40, this.Context, this.GameConfiguration)
                .WithItemReward(12, 15, this.Context, this.GameConfiguration);
            this.CreateQuest("Clearing a Path to Icarus", 18, 93, 220, 229, 257, CharacterClassNumber.Summoner)
                .WithMonsterKillRequirement(60, 40, this.Context, this.GameConfiguration)
                .WithItemReward(12, 15, this.Context, this.GameConfiguration);

            this.CreateQuest("Advancement of the Reconnaissance Party (1)", 18, 96, 230, 234, 257, CharacterClassNumber.SoulMaster)
                .WithMonsterKillRequirement(60, 62, this.Context, this.GameConfiguration)
                .WithExperienceReward(150000, this.Context);
            this.CreateQuest("Advancement of the Reconnaissance Party (1)", 18, 96, 230, 234, 257, CharacterClassNumber.DarkWizard)
                .WithMonsterKillRequirement(60, 62, this.Context, this.GameConfiguration)
                .WithExperienceReward(150000, this.Context);
            this.CreateQuest("Advancement of the Reconnaissance Party (1)", 18, 96, 230, 234, 257, CharacterClassNumber.BladeKnight)
                .WithMonsterKillRequirement(60, 62, this.Context, this.GameConfiguration)
                .WithExperienceReward(150000, this.Context);
            this.CreateQuest("Advancement of the Reconnaissance Party (1)", 18, 96, 230, 234, 257, CharacterClassNumber.DarkKnight)
                .WithMonsterKillRequirement(60, 62, this.Context, this.GameConfiguration)
                .WithExperienceReward(150000, this.Context);
            this.CreateQuest("Advancement of the Reconnaissance Party (1)", 18, 96, 230, 234, 257, CharacterClassNumber.MuseElf)
                .WithMonsterKillRequirement(60, 62, this.Context, this.GameConfiguration)
                .WithExperienceReward(150000, this.Context);
            this.CreateQuest("Advancement of the Reconnaissance Party (1)", 18, 96, 230, 234, 257, CharacterClassNumber.FairyElf)
                .WithMonsterKillRequirement(60, 62, this.Context, this.GameConfiguration)
                .WithExperienceReward(150000, this.Context);
            this.CreateQuest("Advancement of the Reconnaissance Party (1)", 18, 96, 230, 234, 257, CharacterClassNumber.BloodySummoner)
                .WithMonsterKillRequirement(60, 62, this.Context, this.GameConfiguration)
                .WithExperienceReward(150000, this.Context);
            this.CreateQuest("Advancement of the Reconnaissance Party (1)", 18, 96, 230, 234, 257, CharacterClassNumber.Summoner)
                .WithMonsterKillRequirement(60, 62, this.Context, this.GameConfiguration)
                .WithExperienceReward(150000, this.Context);

            this.CreateQuest("Advancement of the Reconnaissance Party (1)", 18, 99, 230, 234, 257, CharacterClassNumber.MagicGladiator)
                .WithMonsterKillRequirement(60, 62, this.Context, this.GameConfiguration)
                .WithExperienceReward(150000, this.Context);
            this.CreateQuest("Advancement of the Reconnaissance Party (1)", 18, 99, 230, 234, 257, CharacterClassNumber.DarkLord)
                .WithMonsterKillRequirement(60, 62, this.Context, this.GameConfiguration)
                .WithExperienceReward(150000, this.Context);
            this.CreateQuest("Advancement of the Reconnaissance Party (1)", 18, 99, 230, 234, 257, CharacterClassNumber.RageFighter)
                .WithMonsterKillRequirement(60, 62, this.Context, this.GameConfiguration)
                .WithExperienceReward(150000, this.Context);

            this.CreateQuest("Advancement of the Reconnaissance Party (2)", 18, 102, 235, 239, 257, CharacterClassNumber.MagicGladiator)
                .WithMonsterKillRequirement(60, 60, this.Context, this.GameConfiguration)
                .WithItemReward(14, 14, this.Context, this.GameConfiguration);
            this.CreateQuest("Advancement of the Reconnaissance Party (2)", 18, 102, 235, 239, 257, CharacterClassNumber.DarkLord)
                .WithMonsterKillRequirement(60, 60, this.Context, this.GameConfiguration)
                .WithItemReward(14, 14, this.Context, this.GameConfiguration);
            this.CreateQuest("Advancement of the Reconnaissance Party (2)", 18, 102, 235, 239, 257, CharacterClassNumber.RageFighter)
                .WithMonsterKillRequirement(60, 60, this.Context, this.GameConfiguration)
                .WithItemReward(14, 14, this.Context, this.GameConfiguration);

            this.CreateQuest("Advancement of the Reconnaissance Party (2)", 18, 105, 235, 239, 257, CharacterClassNumber.SoulMaster)
                .WithMonsterKillRequirement(60, 60, this.Context, this.GameConfiguration)
                .WithItemReward(14, 14, this.Context, this.GameConfiguration);
            this.CreateQuest("Advancement of the Reconnaissance Party (2)", 18, 105, 235, 239, 257, CharacterClassNumber.DarkWizard)
                .WithMonsterKillRequirement(60, 60, this.Context, this.GameConfiguration)
                .WithItemReward(14, 14, this.Context, this.GameConfiguration);
            this.CreateQuest("Advancement of the Reconnaissance Party (2)", 18, 105, 235, 239, 257, CharacterClassNumber.BladeKnight)
                .WithMonsterKillRequirement(60, 60, this.Context, this.GameConfiguration)
                .WithItemReward(14, 14, this.Context, this.GameConfiguration);
            this.CreateQuest("Advancement of the Reconnaissance Party (2)", 18, 105, 235, 239, 257, CharacterClassNumber.DarkKnight)
                .WithMonsterKillRequirement(60, 60, this.Context, this.GameConfiguration)
                .WithItemReward(14, 14, this.Context, this.GameConfiguration);
            this.CreateQuest("Advancement of the Reconnaissance Party (2)", 18, 105, 235, 239, 257, CharacterClassNumber.MuseElf)
                .WithMonsterKillRequirement(60, 60, this.Context, this.GameConfiguration)
                .WithItemReward(14, 14, this.Context, this.GameConfiguration);
            this.CreateQuest("Advancement of the Reconnaissance Party (2)", 18, 105, 235, 239, 257, CharacterClassNumber.FairyElf)
                .WithMonsterKillRequirement(60, 60, this.Context, this.GameConfiguration)
                .WithItemReward(14, 14, this.Context, this.GameConfiguration);
            this.CreateQuest("Advancement of the Reconnaissance Party (2)", 18, 105, 235, 239, 257, CharacterClassNumber.BloodySummoner)
                .WithMonsterKillRequirement(60, 60, this.Context, this.GameConfiguration)
                .WithItemReward(14, 14, this.Context, this.GameConfiguration);
            this.CreateQuest("Advancement of the Reconnaissance Party (2)", 18, 105, 235, 239, 257, CharacterClassNumber.Summoner)
                .WithMonsterKillRequirement(60, 60, this.Context, this.GameConfiguration)
                .WithItemReward(14, 14, this.Context, this.GameConfiguration);

            this.CreateQuest("Advancement of the Reconnaissance Party (3)", 18, 108, 240, 244, 257)
                .WithMonsterKillRequirement(60, 57, this.Context, this.GameConfiguration)
                .WithItemReward(14, 14, this.Context, this.GameConfiguration);

            this.CreateQuest("Advancement of the Reconnaissance Party (3)", 18, 111, 240, 244, 257)
                .WithMonsterKillRequirement(60, 57, this.Context, this.GameConfiguration)
                .WithExperienceReward(170000, this.Context);

            this.CreateQuest("Road to Floating Castle (1)", 18, 114, 245, 249, 257)
                .WithMonsterKillRequirement(80, 69, this.Context, this.GameConfiguration)
                .WithExperienceReward(200000, this.Context);

            this.CreateQuest("Road to Floating Castle (1)", 18, 117, 245, 249, 257)
                .WithMonsterKillRequirement(80, 69, this.Context, this.GameConfiguration)
                .WithItemReward(14, 13, this.Context, this.GameConfiguration);

            this.CreateQuest("Road to Floating Castle (2)", 18, 120, 250, 254, 257, CharacterClassNumber.MagicGladiator)
                .WithMonsterKillRequirement(80, 71, this.Context, this.GameConfiguration)
                .WithExperienceReward(190000, this.Context);
            this.CreateQuest("Road to Floating Castle (2)", 18, 120, 250, 254, 257, CharacterClassNumber.DarkLord)
                .WithMonsterKillRequirement(80, 71, this.Context, this.GameConfiguration)
                .WithExperienceReward(190000, this.Context);
            this.CreateQuest("Road to Floating Castle (2)", 18, 120, 250, 254, 257, CharacterClassNumber.RageFighter)
                .WithMonsterKillRequirement(80, 71, this.Context, this.GameConfiguration)
                .WithExperienceReward(190000, this.Context);

            this.CreateQuest("Road to Floating Castle (2)", 18, 123, 250, 254, 257, CharacterClassNumber.SoulMaster)
                .WithMonsterKillRequirement(80, 71, this.Context, this.GameConfiguration)
                .WithExperienceReward(190000, this.Context);
            this.CreateQuest("Road to Floating Castle (2)", 18, 123, 250, 254, 257, CharacterClassNumber.DarkWizard)
                .WithMonsterKillRequirement(80, 71, this.Context, this.GameConfiguration)
                .WithExperienceReward(190000, this.Context);
            this.CreateQuest("Road to Floating Castle (2)", 18, 123, 250, 254, 257, CharacterClassNumber.BladeKnight)
                .WithMonsterKillRequirement(80, 71, this.Context, this.GameConfiguration)
                .WithExperienceReward(190000, this.Context);
            this.CreateQuest("Road to Floating Castle (2)", 18, 123, 250, 254, 257, CharacterClassNumber.DarkKnight)
                .WithMonsterKillRequirement(80, 71, this.Context, this.GameConfiguration)
                .WithExperienceReward(190000, this.Context);
            this.CreateQuest("Road to Floating Castle (2)", 18, 123, 250, 254, 257, CharacterClassNumber.MuseElf)
                .WithMonsterKillRequirement(80, 71, this.Context, this.GameConfiguration)
                .WithExperienceReward(190000, this.Context);
            this.CreateQuest("Road to Floating Castle (2)", 18, 123, 250, 254, 257, CharacterClassNumber.FairyElf)
                .WithMonsterKillRequirement(80, 71, this.Context, this.GameConfiguration)
                .WithExperienceReward(190000, this.Context);
            this.CreateQuest("Road to Floating Castle (2)", 18, 123, 250, 254, 257, CharacterClassNumber.BloodySummoner)
                .WithMonsterKillRequirement(80, 71, this.Context, this.GameConfiguration)
                .WithExperienceReward(190000, this.Context);
            this.CreateQuest("Road to Floating Castle (2)", 18, 123, 250, 254, 257, CharacterClassNumber.Summoner)
                .WithMonsterKillRequirement(80, 71, this.Context, this.GameConfiguration)
                .WithExperienceReward(190000, this.Context);

            this.CreateQuest("Road to Floating Castle (3)", 18, 126, 255, 259, 257, CharacterClassNumber.SoulMaster)
                .WithMonsterKillRequirement(80, 70, this.Context, this.GameConfiguration)
                .WithItemReward(14, 13, this.Context, this.GameConfiguration)
                .WithItemReward(14, 14, this.Context, this.GameConfiguration)
                .WithItemReward(12, 15, this.Context, this.GameConfiguration);
            this.CreateQuest("Road to Floating Castle (3)", 18, 126, 255, 259, 257, CharacterClassNumber.DarkWizard)
                .WithMonsterKillRequirement(80, 70, this.Context, this.GameConfiguration)
                .WithItemReward(14, 13, this.Context, this.GameConfiguration)
                .WithItemReward(14, 14, this.Context, this.GameConfiguration)
                .WithItemReward(12, 15, this.Context, this.GameConfiguration);
            this.CreateQuest("Road to Floating Castle (3)", 18, 126, 255, 259, 257, CharacterClassNumber.BladeKnight)
                .WithMonsterKillRequirement(80, 70, this.Context, this.GameConfiguration)
                .WithItemReward(14, 13, this.Context, this.GameConfiguration)
                .WithItemReward(14, 14, this.Context, this.GameConfiguration)
                .WithItemReward(12, 15, this.Context, this.GameConfiguration);
            this.CreateQuest("Road to Floating Castle (3)", 18, 126, 255, 259, 257, CharacterClassNumber.DarkKnight)
                .WithMonsterKillRequirement(80, 70, this.Context, this.GameConfiguration)
                .WithItemReward(14, 13, this.Context, this.GameConfiguration)
                .WithItemReward(14, 14, this.Context, this.GameConfiguration)
                .WithItemReward(12, 15, this.Context, this.GameConfiguration);
            this.CreateQuest("Road to Floating Castle (3)", 18, 126, 255, 259, 257, CharacterClassNumber.MuseElf)
                .WithMonsterKillRequirement(80, 70, this.Context, this.GameConfiguration)
                .WithItemReward(14, 13, this.Context, this.GameConfiguration)
                .WithItemReward(14, 14, this.Context, this.GameConfiguration)
                .WithItemReward(12, 15, this.Context, this.GameConfiguration);
            this.CreateQuest("Road to Floating Castle (3)", 18, 126, 255, 259, 257, CharacterClassNumber.FairyElf)
                .WithMonsterKillRequirement(80, 70, this.Context, this.GameConfiguration)
                .WithItemReward(14, 13, this.Context, this.GameConfiguration)
                .WithItemReward(14, 14, this.Context, this.GameConfiguration)
                .WithItemReward(12, 15, this.Context, this.GameConfiguration);
            this.CreateQuest("Road to Floating Castle (3)", 18, 126, 255, 259, 257, CharacterClassNumber.BloodySummoner)
                .WithMonsterKillRequirement(80, 70, this.Context, this.GameConfiguration)
                .WithItemReward(14, 13, this.Context, this.GameConfiguration)
                .WithItemReward(14, 14, this.Context, this.GameConfiguration)
                .WithItemReward(12, 15, this.Context, this.GameConfiguration);
            this.CreateQuest("Road to Floating Castle (3)", 18, 126, 255, 259, 257, CharacterClassNumber.Summoner)
                .WithMonsterKillRequirement(80, 70, this.Context, this.GameConfiguration)
                .WithItemReward(14, 13, this.Context, this.GameConfiguration)
                .WithItemReward(14, 14, this.Context, this.GameConfiguration)
                .WithItemReward(12, 15, this.Context, this.GameConfiguration);

            this.CreateQuest("Road to Floating Castle (3)", 18, 129, 255, 259, 257, CharacterClassNumber.MagicGladiator)
                .WithMonsterKillRequirement(80, 70, this.Context, this.GameConfiguration)
                .WithItemReward(14, 13, this.Context, this.GameConfiguration)
                .WithItemReward(14, 14, this.Context, this.GameConfiguration)
                .WithItemReward(12, 15, this.Context, this.GameConfiguration);
            this.CreateQuest("Road to Floating Castle (3)", 18, 129, 255, 259, 257, CharacterClassNumber.DarkLord)
                .WithMonsterKillRequirement(80, 70, this.Context, this.GameConfiguration)
                .WithItemReward(14, 13, this.Context, this.GameConfiguration)
                .WithItemReward(14, 14, this.Context, this.GameConfiguration)
                .WithItemReward(12, 15, this.Context, this.GameConfiguration);
            this.CreateQuest("Road to Floating Castle (3)", 18, 129, 255, 259, 257, CharacterClassNumber.RageFighter)
                .WithMonsterKillRequirement(80, 70, this.Context, this.GameConfiguration)
                .WithItemReward(14, 13, this.Context, this.GameConfiguration)
                .WithItemReward(14, 14, this.Context, this.GameConfiguration)
                .WithItemReward(12, 15, this.Context, this.GameConfiguration);

            this.CreateQuest("The Beginning of Requests", 15, 3, 80, 90, 257, CharacterClassNumber.FairyElf)
                .WithMonsterKillRequirement(50, 422, this.Context, this.GameConfiguration)
                .WithMonsterKillRequirement(50, 19, this.Context, this.GameConfiguration)
                .WithExperienceReward(60000, this.Context)
                .WithMoneyReward(1500000, this.Context)
                .WithItemReward(12, 15, this.Context, this.GameConfiguration);
            this.CreateQuest("The Beginning of Requests", 15, 3, 80, 90, 257, CharacterClassNumber.Summoner)
                .WithMonsterKillRequirement(50, 422, this.Context, this.GameConfiguration)
                .WithMonsterKillRequirement(50, 19, this.Context, this.GameConfiguration)
                .WithExperienceReward(60000, this.Context)
                .WithMoneyReward(1500000, this.Context)
                .WithItemReward(12, 15, this.Context, this.GameConfiguration);

            this.CreateQuest("The Beginning of Requests", 15, 4, 80, 90, 257, CharacterClassNumber.DarkWizard)
                .WithMonsterKillRequirement(50, 423, this.Context, this.GameConfiguration)
                .WithMonsterKillRequirement(50, 20, this.Context, this.GameConfiguration)
                .WithExperienceReward(60000, this.Context)
                .WithMoneyReward(1500000, this.Context)
                .WithItemReward(12, 15, this.Context, this.GameConfiguration);
            this.CreateQuest("The Beginning of Requests", 15, 4, 80, 90, 257, CharacterClassNumber.DarkKnight)
                .WithMonsterKillRequirement(50, 423, this.Context, this.GameConfiguration)
                .WithMonsterKillRequirement(50, 20, this.Context, this.GameConfiguration)
                .WithExperienceReward(60000, this.Context)
                .WithMoneyReward(1500000, this.Context)
                .WithItemReward(12, 15, this.Context, this.GameConfiguration);

            this.CreateQuest("Conquer the Dungeon! (1)", 15, 6, 91, 100, 257, CharacterClassNumber.DarkKnight)
                .WithMonsterKillRequirement(50, 15, this.Context, this.GameConfiguration)
                .WithMonsterKillRequirement(30, 5, this.Context, this.GameConfiguration)
                .WithItemReward(10, 1, this.Context, this.GameConfiguration, 8);

            this.CreateQuest("Conquer the Dungeon! (1)", 15, 7, 91, 100, 257, CharacterClassNumber.DarkWizard)
                .WithMonsterKillRequirement(50, 17, this.Context, this.GameConfiguration)
                .WithMonsterKillRequirement(30, 11, this.Context, this.GameConfiguration)
                .WithItemReward(10, 3, this.Context, this.GameConfiguration, 8);

            this.CreateQuest("Conquer the Dungeon! (1)", 15, 8, 91, 100, 257, CharacterClassNumber.FairyElf)
                .WithMonsterKillRequirement(50, 15, this.Context, this.GameConfiguration)
                .WithMonsterKillRequirement(30, 5, this.Context, this.GameConfiguration)
                .WithItemReward(10, 14, this.Context, this.GameConfiguration, 8);

            this.CreateQuest("Conquer the Dungeon! (1)", 15, 9, 91, 100, 257, CharacterClassNumber.Summoner)
                .WithMonsterKillRequirement(50, 17, this.Context, this.GameConfiguration)
                .WithMonsterKillRequirement(30, 11, this.Context, this.GameConfiguration)
                .WithItemReward(10, 40, this.Context, this.GameConfiguration, 8);

            this.CreateQuest("Conquer the Dungeon! (2)", 15, 12, 101, 110, 257, CharacterClassNumber.DarkKnight)
                .WithMonsterKillRequirement(50, 5, this.Context, this.GameConfiguration)
                .WithMonsterKillRequirement(30, 13, this.Context, this.GameConfiguration)
                .WithItemReward(11, 1, this.Context, this.GameConfiguration, 8);

            this.CreateQuest("Conquer the Dungeon! (2)", 15, 13, 101, 110, 257, CharacterClassNumber.DarkWizard)
                .WithMonsterKillRequirement(50, 15, this.Context, this.GameConfiguration)
                .WithMonsterKillRequirement(30, 5, this.Context, this.GameConfiguration)
                .WithItemReward(11, 3, this.Context, this.GameConfiguration, 8);

            this.CreateQuest("Conquer the Dungeon! (2)", 15, 14, 101, 110, 257, CharacterClassNumber.FairyElf)
                .WithMonsterKillRequirement(50, 5, this.Context, this.GameConfiguration)
                .WithMonsterKillRequirement(30, 13, this.Context, this.GameConfiguration)
                .WithItemReward(11, 14, this.Context, this.GameConfiguration, 8);

            this.CreateQuest("Conquer the Dungeon! (2)", 15, 15, 101, 110, 257, CharacterClassNumber.Summoner)
                .WithMonsterKillRequirement(50, 15, this.Context, this.GameConfiguration)
                .WithMonsterKillRequirement(30, 5, this.Context, this.GameConfiguration)
                .WithItemReward(11, 40, this.Context, this.GameConfiguration, 8);

            this.CreateQuest("Conquer the Dungeon! (3)", 15, 18, 111, 120, 257, CharacterClassNumber.DarkKnight)
                .WithMonsterKillRequirement(50, 16, this.Context, this.GameConfiguration)
                .WithMonsterKillRequirement(30, 9, this.Context, this.GameConfiguration)
                .WithItemReward(7, 1, this.Context, this.GameConfiguration, 8);

            this.CreateQuest("Conquer the Dungeon! (3)", 15, 19, 111, 120, 257, CharacterClassNumber.DarkWizard)
                .WithMonsterKillRequirement(50, 5, this.Context, this.GameConfiguration)
                .WithMonsterKillRequirement(30, 13, this.Context, this.GameConfiguration)
                .WithItemReward(7, 3, this.Context, this.GameConfiguration, 8);

            this.CreateQuest("Conquer the Dungeon! (3)", 15, 20, 111, 120, 257, CharacterClassNumber.FairyElf)
                .WithMonsterKillRequirement(50, 16, this.Context, this.GameConfiguration)
                .WithMonsterKillRequirement(30, 9, this.Context, this.GameConfiguration)
                .WithItemReward(7, 14, this.Context, this.GameConfiguration, 8);

            this.CreateQuest("Conquer the Dungeon! (3)", 15, 21, 111, 120, 257, CharacterClassNumber.Summoner)
                .WithMonsterKillRequirement(50, 5, this.Context, this.GameConfiguration)
                .WithMonsterKillRequirement(30, 13, this.Context, this.GameConfiguration)
                .WithItemReward(7, 40, this.Context, this.GameConfiguration, 8);

            this.CreateQuest("Conquer the Dungeon! (4)", 15, 24, 121, 170, 257, CharacterClassNumber.BladeKnight)
                .WithMonsterKillRequirement(50, 8, this.Context, this.GameConfiguration)
                .WithMonsterKillRequirement(30, 10, this.Context, this.GameConfiguration)
                .WithItemReward(9, 1, this.Context, this.GameConfiguration, 8);
            this.CreateQuest("Conquer the Dungeon! (4)", 15, 24, 121, 170, 257, CharacterClassNumber.DarkKnight)
                .WithMonsterKillRequirement(50, 8, this.Context, this.GameConfiguration)
                .WithMonsterKillRequirement(30, 10, this.Context, this.GameConfiguration)
                .WithItemReward(9, 1, this.Context, this.GameConfiguration, 8);

            this.CreateQuest("Conquer the Dungeon! (4)", 15, 25, 121, 170, 257, CharacterClassNumber.SoulMaster)
                .WithMonsterKillRequirement(50, 13, this.Context, this.GameConfiguration)
                .WithMonsterKillRequirement(30, 16, this.Context, this.GameConfiguration)
                .WithItemReward(9, 3, this.Context, this.GameConfiguration, 8);
            this.CreateQuest("Conquer the Dungeon! (4)", 15, 25, 121, 170, 257, CharacterClassNumber.DarkWizard)
                .WithMonsterKillRequirement(50, 13, this.Context, this.GameConfiguration)
                .WithMonsterKillRequirement(30, 16, this.Context, this.GameConfiguration)
                .WithItemReward(9, 3, this.Context, this.GameConfiguration, 8);

            this.CreateQuest("Conquer the Dungeon! (4)", 15, 26, 121, 170, 257, CharacterClassNumber.MuseElf)
                .WithMonsterKillRequirement(50, 8, this.Context, this.GameConfiguration)
                .WithMonsterKillRequirement(30, 10, this.Context, this.GameConfiguration)
                .WithItemReward(9, 14, this.Context, this.GameConfiguration, 8);
            this.CreateQuest("Conquer the Dungeon! (4)", 15, 26, 121, 170, 257, CharacterClassNumber.FairyElf)
                .WithMonsterKillRequirement(50, 8, this.Context, this.GameConfiguration)
                .WithMonsterKillRequirement(30, 10, this.Context, this.GameConfiguration)
                .WithItemReward(9, 14, this.Context, this.GameConfiguration, 8);

            this.CreateQuest("Conquer the Dungeon! (4)", 15, 27, 121, 170, 257, CharacterClassNumber.BloodySummoner)
                .WithMonsterKillRequirement(50, 13, this.Context, this.GameConfiguration)
                .WithMonsterKillRequirement(30, 16, this.Context, this.GameConfiguration)
                .WithItemReward(9, 40, this.Context, this.GameConfiguration, 8);
            this.CreateQuest("Conquer the Dungeon! (4)", 15, 27, 121, 170, 257, CharacterClassNumber.Summoner)
                .WithMonsterKillRequirement(50, 13, this.Context, this.GameConfiguration)
                .WithMonsterKillRequirement(30, 16, this.Context, this.GameConfiguration)
                .WithItemReward(9, 40, this.Context, this.GameConfiguration, 8);

            this.CreateQuest("Conquer the Dungeon! (5)", 15, 30, 131, 190, 257, CharacterClassNumber.BladeKnight)
                .WithMonsterKillRequirement(50, 10, this.Context, this.GameConfiguration)
                .WithMonsterKillRequirement(10, 18, this.Context, this.GameConfiguration)
                .WithItemReward(8, 1, this.Context, this.GameConfiguration, 8);
            this.CreateQuest("Conquer the Dungeon! (5)", 15, 30, 131, 190, 257, CharacterClassNumber.DarkKnight)
                .WithMonsterKillRequirement(50, 10, this.Context, this.GameConfiguration)
                .WithMonsterKillRequirement(10, 18, this.Context, this.GameConfiguration)
                .WithItemReward(8, 1, this.Context, this.GameConfiguration, 8);

            this.CreateQuest("Conquer the Dungeon! (5)", 15, 31, 131, 190, 257, CharacterClassNumber.SoulMaster)
                .WithMonsterKillRequirement(50, 16, this.Context, this.GameConfiguration)
                .WithMonsterKillRequirement(30, 9, this.Context, this.GameConfiguration)
                .WithItemReward(8, 3, this.Context, this.GameConfiguration, 8);
            this.CreateQuest("Conquer the Dungeon! (5)", 15, 31, 131, 190, 257, CharacterClassNumber.DarkWizard)
                .WithMonsterKillRequirement(50, 16, this.Context, this.GameConfiguration)
                .WithMonsterKillRequirement(30, 9, this.Context, this.GameConfiguration)
                .WithItemReward(8, 3, this.Context, this.GameConfiguration, 8);

            this.CreateQuest("Conquer the Dungeon! (5)", 15, 32, 131, 190, 257, CharacterClassNumber.MuseElf)
                .WithMonsterKillRequirement(50, 10, this.Context, this.GameConfiguration)
                .WithMonsterKillRequirement(10, 18, this.Context, this.GameConfiguration)
                .WithItemReward(8, 14, this.Context, this.GameConfiguration, 8);
            this.CreateQuest("Conquer the Dungeon! (5)", 15, 32, 131, 190, 257, CharacterClassNumber.FairyElf)
                .WithMonsterKillRequirement(50, 10, this.Context, this.GameConfiguration)
                .WithMonsterKillRequirement(10, 18, this.Context, this.GameConfiguration)
                .WithItemReward(8, 14, this.Context, this.GameConfiguration, 8);

            this.CreateQuest("Conquer the Dungeon! (5)", 15, 33, 131, 190, 257, CharacterClassNumber.BloodySummoner)
                .WithMonsterKillRequirement(50, 16, this.Context, this.GameConfiguration)
                .WithMonsterKillRequirement(30, 9, this.Context, this.GameConfiguration)
                .WithItemReward(8, 40, this.Context, this.GameConfiguration, 8);
            this.CreateQuest("Conquer the Dungeon! (5)", 15, 33, 131, 190, 257, CharacterClassNumber.Summoner)
                .WithMonsterKillRequirement(50, 16, this.Context, this.GameConfiguration)
                .WithMonsterKillRequirement(30, 9, this.Context, this.GameConfiguration)
                .WithItemReward(8, 40, this.Context, this.GameConfiguration, 8);

            this.CreateQuest("Conquer the Dungeon! (6)", 15, 36, 141, 160, 257, CharacterClassNumber.BladeKnight)
                .WithMonsterKillRequirement(50, 10, this.Context, this.GameConfiguration)
                .WithMonsterKillRequirement(30, 18, this.Context, this.GameConfiguration)
                .WithItemReward(0, 13, this.Context, this.GameConfiguration, 8, 0, hasLuck: false, hasSkill: true);
            this.CreateQuest("Conquer the Dungeon! (6)", 15, 36, 141, 160, 257, CharacterClassNumber.DarkKnight)
                .WithMonsterKillRequirement(50, 10, this.Context, this.GameConfiguration)
                .WithMonsterKillRequirement(30, 18, this.Context, this.GameConfiguration)
                .WithItemReward(0, 13, this.Context, this.GameConfiguration, 8, 0, hasLuck: false, hasSkill: true);

            this.CreateQuest("Conquer the Dungeon! (6)", 15, 37, 141, 160, 257, CharacterClassNumber.SoulMaster)
                .WithMonsterKillRequirement(50, 8, this.Context, this.GameConfiguration)
                .WithMonsterKillRequirement(5, 10, this.Context, this.GameConfiguration)
                .WithMonsterKillRequirement(1, 18, this.Context, this.GameConfiguration)
                .WithItemReward(5, 5, this.Context, this.GameConfiguration, 7);
            this.CreateQuest("Conquer the Dungeon! (6)", 15, 37, 141, 160, 257, CharacterClassNumber.DarkWizard)
                .WithMonsterKillRequirement(50, 8, this.Context, this.GameConfiguration)
                .WithMonsterKillRequirement(5, 10, this.Context, this.GameConfiguration)
                .WithMonsterKillRequirement(1, 18, this.Context, this.GameConfiguration)
                .WithItemReward(5, 5, this.Context, this.GameConfiguration, 7);

            this.CreateQuest("Conquer the Dungeon! (6)", 15, 38, 141, 160, 257, CharacterClassNumber.MuseElf)
                .WithMonsterKillRequirement(50, 10, this.Context, this.GameConfiguration)
                .WithMonsterKillRequirement(30, 18, this.Context, this.GameConfiguration)
                .WithItemReward(4, 13, this.Context, this.GameConfiguration, 8, 0, hasLuck: false, hasSkill: true);
            this.CreateQuest("Conquer the Dungeon! (6)", 15, 38, 141, 160, 257, CharacterClassNumber.FairyElf)
                .WithMonsterKillRequirement(50, 10, this.Context, this.GameConfiguration)
                .WithMonsterKillRequirement(30, 18, this.Context, this.GameConfiguration)
                .WithItemReward(4, 13, this.Context, this.GameConfiguration, 8, 0, hasLuck: false, hasSkill: true);

            this.CreateQuest("Conquer the Dungeon! (6)", 15, 39, 141, 160, 257, CharacterClassNumber.BloodySummoner)
                .WithMonsterKillRequirement(50, 8, this.Context, this.GameConfiguration)
                .WithMonsterKillRequirement(5, 10, this.Context, this.GameConfiguration)
                .WithMonsterKillRequirement(1, 18, this.Context, this.GameConfiguration)
                .WithItemReward(5, 16, this.Context, this.GameConfiguration, 7);
            this.CreateQuest("Conquer the Dungeon! (6)", 15, 39, 141, 160, 257, CharacterClassNumber.Summoner)
                .WithMonsterKillRequirement(50, 8, this.Context, this.GameConfiguration)
                .WithMonsterKillRequirement(5, 10, this.Context, this.GameConfiguration)
                .WithMonsterKillRequirement(1, 18, this.Context, this.GameConfiguration)
                .WithItemReward(5, 16, this.Context, this.GameConfiguration, 7);

            this.CreateQuest("The mission of reconnaissance troops (1)", 15, 42, 161, 165, 257, CharacterClassNumber.BladeKnight)
                .WithMonsterKillRequirement(50, 36, this.Context, this.GameConfiguration)
                .WithMonsterKillRequirement(50, 39, this.Context, this.GameConfiguration)
                .WithItemReward(10, 34, this.Context, this.GameConfiguration, 7);
            this.CreateQuest("The mission of reconnaissance troops (1)", 15, 42, 161, 165, 257, CharacterClassNumber.DarkKnight)
                .WithMonsterKillRequirement(50, 36, this.Context, this.GameConfiguration)
                .WithMonsterKillRequirement(50, 39, this.Context, this.GameConfiguration)
                .WithItemReward(10, 34, this.Context, this.GameConfiguration, 7);

            this.CreateQuest("The mission of reconnaissance troops (1)", 15, 43, 161, 165, 257, CharacterClassNumber.SoulMaster)
                .WithMonsterKillRequirement(5, 25, this.Context, this.GameConfiguration)
                .WithItemReward(10, 35, this.Context, this.GameConfiguration, 7);
            this.CreateQuest("The mission of reconnaissance troops (1)", 15, 43, 161, 165, 257, CharacterClassNumber.DarkWizard)
                .WithMonsterKillRequirement(5, 25, this.Context, this.GameConfiguration)
                .WithItemReward(10, 35, this.Context, this.GameConfiguration, 7);

            this.CreateQuest("The mission of reconnaissance troops (1)", 15, 44, 161, 165, 257, CharacterClassNumber.MuseElf)
                .WithMonsterKillRequirement(50, 36, this.Context, this.GameConfiguration)
                .WithMonsterKillRequirement(50, 39, this.Context, this.GameConfiguration)
                .WithItemReward(10, 36, this.Context, this.GameConfiguration, 7);
            this.CreateQuest("The mission of reconnaissance troops (1)", 15, 44, 161, 165, 257, CharacterClassNumber.FairyElf)
                .WithMonsterKillRequirement(50, 36, this.Context, this.GameConfiguration)
                .WithMonsterKillRequirement(50, 39, this.Context, this.GameConfiguration)
                .WithItemReward(10, 36, this.Context, this.GameConfiguration, 7);

            this.CreateQuest("The mission of reconnaissance troops (1)", 15, 45, 161, 165, 257, CharacterClassNumber.BloodySummoner)
                .WithMonsterKillRequirement(5, 25, this.Context, this.GameConfiguration)
                .WithItemReward(10, 41, this.Context, this.GameConfiguration, 7);
            this.CreateQuest("The mission of reconnaissance troops (1)", 15, 45, 161, 165, 257, CharacterClassNumber.Summoner)
                .WithMonsterKillRequirement(5, 25, this.Context, this.GameConfiguration)
                .WithItemReward(10, 41, this.Context, this.GameConfiguration, 7);

            this.CreateQuest("The mission of reconnaissance troops (2)", 15, 48, 166, 170, 257, CharacterClassNumber.BladeKnight)
                .WithMonsterKillRequirement(50, 39, this.Context, this.GameConfiguration)
                .WithMonsterKillRequirement(30, 34, this.Context, this.GameConfiguration)
                .WithItemReward(11, 34, this.Context, this.GameConfiguration, 7);
            this.CreateQuest("The mission of reconnaissance troops (2)", 15, 48, 166, 170, 257, CharacterClassNumber.DarkKnight)
                .WithMonsterKillRequirement(50, 39, this.Context, this.GameConfiguration)
                .WithMonsterKillRequirement(30, 34, this.Context, this.GameConfiguration)
                .WithItemReward(11, 34, this.Context, this.GameConfiguration, 7);

            this.CreateQuest("The mission of reconnaissance troops (2)", 15, 49, 166, 170, 257, CharacterClassNumber.SoulMaster)
                .WithMonsterKillRequirement(100, 424, this.Context, this.GameConfiguration)
                .WithItemReward(11, 35, this.Context, this.GameConfiguration, 7);
            this.CreateQuest("The mission of reconnaissance troops (2)", 15, 49, 166, 170, 257, CharacterClassNumber.DarkWizard)
                .WithMonsterKillRequirement(100, 424, this.Context, this.GameConfiguration)
                .WithItemReward(11, 35, this.Context, this.GameConfiguration, 7);

            this.CreateQuest("The mission of reconnaissance troops (2)", 15, 50, 166, 170, 257, CharacterClassNumber.MuseElf)
                .WithMonsterKillRequirement(50, 39, this.Context, this.GameConfiguration)
                .WithMonsterKillRequirement(30, 34, this.Context, this.GameConfiguration)
                .WithItemReward(11, 36, this.Context, this.GameConfiguration, 7);
            this.CreateQuest("The mission of reconnaissance troops (2)", 15, 50, 166, 170, 257, CharacterClassNumber.FairyElf)
                .WithMonsterKillRequirement(50, 39, this.Context, this.GameConfiguration)
                .WithMonsterKillRequirement(30, 34, this.Context, this.GameConfiguration)
                .WithItemReward(11, 36, this.Context, this.GameConfiguration, 7);

            this.CreateQuest("The mission of reconnaissance troops (2)", 15, 51, 166, 170, 257, CharacterClassNumber.BloodySummoner)
                .WithMonsterKillRequirement(100, 424, this.Context, this.GameConfiguration)
                .WithItemReward(11, 41, this.Context, this.GameConfiguration, 7);
            this.CreateQuest("The mission of reconnaissance troops (2)", 15, 51, 166, 170, 257, CharacterClassNumber.Summoner)
                .WithMonsterKillRequirement(100, 424, this.Context, this.GameConfiguration)
                .WithItemReward(11, 41, this.Context, this.GameConfiguration, 7);

            this.CreateQuest("The mission of reconnaissance troops (3)", 15, 54, 171, 179, 257, CharacterClassNumber.BladeKnight)
                .WithMonsterKillRequirement(50, 34, this.Context, this.GameConfiguration)
                .WithMonsterKillRequirement(50, 41, this.Context, this.GameConfiguration)
                .WithItemReward(9, 34, this.Context, this.GameConfiguration, 7)
                .WithItemReward(7, 34, this.Context, this.GameConfiguration, 7);
            this.CreateQuest("The mission of reconnaissance troops (3)", 15, 54, 171, 179, 257, CharacterClassNumber.DarkKnight)
                .WithMonsterKillRequirement(50, 34, this.Context, this.GameConfiguration)
                .WithMonsterKillRequirement(50, 41, this.Context, this.GameConfiguration)
                .WithItemReward(9, 34, this.Context, this.GameConfiguration, 7)
                .WithItemReward(7, 34, this.Context, this.GameConfiguration, 7);

            this.CreateQuest("The mission of reconnaissance troops (3)", 15, 55, 171, 179, 257, CharacterClassNumber.SoulMaster)
                .WithMonsterKillRequirement(100, 425, this.Context, this.GameConfiguration)
                .WithItemReward(9, 35, this.Context, this.GameConfiguration, 7)
                .WithItemReward(7, 35, this.Context, this.GameConfiguration, 7);
            this.CreateQuest("The mission of reconnaissance troops (3)", 15, 55, 171, 179, 257, CharacterClassNumber.DarkWizard)
                .WithMonsterKillRequirement(100, 425, this.Context, this.GameConfiguration)
                .WithItemReward(9, 35, this.Context, this.GameConfiguration, 7)
                .WithItemReward(7, 35, this.Context, this.GameConfiguration, 7);

            this.CreateQuest("The mission of reconnaissance troops (3)", 15, 56, 171, 179, 257, CharacterClassNumber.MuseElf)
                .WithMonsterKillRequirement(50, 34, this.Context, this.GameConfiguration)
                .WithMonsterKillRequirement(50, 41, this.Context, this.GameConfiguration)
                .WithItemReward(9, 36, this.Context, this.GameConfiguration, 7)
                .WithItemReward(7, 36, this.Context, this.GameConfiguration, 7);
            this.CreateQuest("The mission of reconnaissance troops (3)", 15, 56, 171, 179, 257, CharacterClassNumber.FairyElf)
                .WithMonsterKillRequirement(50, 34, this.Context, this.GameConfiguration)
                .WithMonsterKillRequirement(50, 41, this.Context, this.GameConfiguration)
                .WithItemReward(9, 36, this.Context, this.GameConfiguration, 7)
                .WithItemReward(7, 36, this.Context, this.GameConfiguration, 7);

            this.CreateQuest("The mission of reconnaissance troops (3)", 15, 57, 171, 179, 257, CharacterClassNumber.BloodySummoner)
                .WithMonsterKillRequirement(100, 425, this.Context, this.GameConfiguration)
                .WithItemReward(9, 41, this.Context, this.GameConfiguration, 7)
                .WithItemReward(7, 41, this.Context, this.GameConfiguration, 7);
            this.CreateQuest("The mission of reconnaissance troops (3)", 15, 57, 171, 179, 257, CharacterClassNumber.Summoner)
                .WithMonsterKillRequirement(100, 425, this.Context, this.GameConfiguration)
                .WithItemReward(9, 41, this.Context, this.GameConfiguration, 7)
                .WithItemReward(7, 41, this.Context, this.GameConfiguration, 7);

            this.CreateQuest("The mission of reconnaissance troops (4)", 15, 60, 180, 185, 257, CharacterClassNumber.BladeKnight)
                .WithMonsterKillRequirement(100, 41, this.Context, this.GameConfiguration)
                .WithItemReward(8, 34, this.Context, this.GameConfiguration, 7)
                .WithItemReward(2, 6, this.Context, this.GameConfiguration, 4, 4);
            this.CreateQuest("The mission of reconnaissance troops (4)", 15, 60, 180, 185, 257, CharacterClassNumber.DarkKnight)
                .WithMonsterKillRequirement(100, 41, this.Context, this.GameConfiguration)
                .WithItemReward(8, 34, this.Context, this.GameConfiguration, 7)
                .WithItemReward(2, 6, this.Context, this.GameConfiguration, 4, 4);

            this.CreateQuest("The mission of reconnaissance troops (4)", 15, 61, 180, 185, 257, CharacterClassNumber.SoulMaster)
                .WithMonsterKillRequirement(100, 36, this.Context, this.GameConfiguration)
                .WithItemReward(8, 35, this.Context, this.GameConfiguration, 7)
                .WithItemReward(2, 6, this.Context, this.GameConfiguration, 4, 4);
            this.CreateQuest("The mission of reconnaissance troops (4)", 15, 61, 180, 185, 257, CharacterClassNumber.DarkWizard)
                .WithMonsterKillRequirement(100, 36, this.Context, this.GameConfiguration)
                .WithItemReward(8, 35, this.Context, this.GameConfiguration, 7)
                .WithItemReward(2, 6, this.Context, this.GameConfiguration, 4, 4);

            this.CreateQuest("The mission of reconnaissance troops (4)", 15, 62, 180, 185, 257, CharacterClassNumber.MuseElf)
                .WithMonsterKillRequirement(100, 41, this.Context, this.GameConfiguration)
                .WithItemReward(8, 36, this.Context, this.GameConfiguration, 7)
                .WithItemReward(2, 6, this.Context, this.GameConfiguration, 4, 4);
            this.CreateQuest("The mission of reconnaissance troops (4)", 15, 62, 180, 185, 257, CharacterClassNumber.FairyElf)
                .WithMonsterKillRequirement(100, 41, this.Context, this.GameConfiguration)
                .WithItemReward(8, 36, this.Context, this.GameConfiguration, 7)
                .WithItemReward(2, 6, this.Context, this.GameConfiguration, 4, 4);

            this.CreateQuest("The mission of reconnaissance troops (4)", 15, 63, 180, 185, 257, CharacterClassNumber.BloodySummoner)
                .WithMonsterKillRequirement(100, 36, this.Context, this.GameConfiguration)
                .WithItemReward(8, 41, this.Context, this.GameConfiguration, 7)
                .WithItemReward(2, 6, this.Context, this.GameConfiguration, 4, 4);
            this.CreateQuest("The mission of reconnaissance troops (4)", 15, 63, 180, 185, 257, CharacterClassNumber.Summoner)
                .WithMonsterKillRequirement(100, 36, this.Context, this.GameConfiguration)
                .WithItemReward(8, 41, this.Context, this.GameConfiguration, 7)
                .WithItemReward(2, 6, this.Context, this.GameConfiguration, 4, 4);

            this.CreateQuest("Put the Lost Tower to Sleep! (1)", 15, 66, 186, 189, 257, CharacterClassNumber.BladeKnight)
                .WithMonsterKillRequirement(50, 41, this.Context, this.GameConfiguration)
                .WithMonsterKillRequirement(30, 37, this.Context, this.GameConfiguration)
                .WithItemReward(10, 34, this.Context, this.GameConfiguration, 8);
            this.CreateQuest("Put the Lost Tower to Sleep! (1)", 15, 66, 186, 189, 257, CharacterClassNumber.DarkKnight)
                .WithMonsterKillRequirement(50, 41, this.Context, this.GameConfiguration)
                .WithMonsterKillRequirement(30, 37, this.Context, this.GameConfiguration)
                .WithItemReward(10, 34, this.Context, this.GameConfiguration, 8);

            this.CreateQuest("Put the Lost Tower to Sleep! (1)", 15, 67, 186, 189, 257, CharacterClassNumber.SoulMaster)
                .WithMonsterKillRequirement(50, 36, this.Context, this.GameConfiguration)
                .WithMonsterKillRequirement(30, 39, this.Context, this.GameConfiguration)
                .WithItemReward(10, 35, this.Context, this.GameConfiguration, 8);
            this.CreateQuest("Put the Lost Tower to Sleep! (1)", 15, 67, 186, 189, 257, CharacterClassNumber.DarkWizard)
                .WithMonsterKillRequirement(50, 36, this.Context, this.GameConfiguration)
                .WithMonsterKillRequirement(30, 39, this.Context, this.GameConfiguration)
                .WithItemReward(10, 35, this.Context, this.GameConfiguration, 8);

            this.CreateQuest("Put the Lost Tower to Sleep! (1)", 15, 68, 186, 189, 257, CharacterClassNumber.MuseElf)
                .WithMonsterKillRequirement(50, 41, this.Context, this.GameConfiguration)
                .WithMonsterKillRequirement(30, 37, this.Context, this.GameConfiguration)
                .WithItemReward(10, 36, this.Context, this.GameConfiguration, 8);
            this.CreateQuest("Put the Lost Tower to Sleep! (1)", 15, 68, 186, 189, 257, CharacterClassNumber.FairyElf)
                .WithMonsterKillRequirement(50, 41, this.Context, this.GameConfiguration)
                .WithMonsterKillRequirement(30, 37, this.Context, this.GameConfiguration)
                .WithItemReward(10, 36, this.Context, this.GameConfiguration, 8);

            this.CreateQuest("Put the Lost Tower to Sleep! (1)", 15, 69, 186, 189, 257, CharacterClassNumber.BloodySummoner)
                .WithMonsterKillRequirement(50, 36, this.Context, this.GameConfiguration)
                .WithMonsterKillRequirement(30, 39, this.Context, this.GameConfiguration)
                .WithItemReward(10, 41, this.Context, this.GameConfiguration, 8);
            this.CreateQuest("Put the Lost Tower to Sleep! (1)", 15, 69, 186, 189, 257, CharacterClassNumber.Summoner)
                .WithMonsterKillRequirement(50, 36, this.Context, this.GameConfiguration)
                .WithMonsterKillRequirement(30, 39, this.Context, this.GameConfiguration)
                .WithItemReward(10, 41, this.Context, this.GameConfiguration, 8);

            this.CreateQuest("Put the Lost Tower to Sleep! (2)", 15, 72, 190, 193, 257, CharacterClassNumber.BladeKnight)
                .WithMonsterKillRequirement(100, 37, this.Context, this.GameConfiguration)
                .WithItemReward(11, 34, this.Context, this.GameConfiguration, 8);
            this.CreateQuest("Put the Lost Tower to Sleep! (2)", 15, 72, 190, 193, 257, CharacterClassNumber.DarkKnight)
                .WithMonsterKillRequirement(100, 37, this.Context, this.GameConfiguration)
                .WithItemReward(11, 34, this.Context, this.GameConfiguration, 8);

            this.CreateQuest("Put the Lost Tower to Sleep! (2)", 15, 73, 190, 193, 257, CharacterClassNumber.SoulMaster)
                .WithMonsterKillRequirement(50, 39, this.Context, this.GameConfiguration)
                .WithMonsterKillRequirement(30, 34, this.Context, this.GameConfiguration)
                .WithItemReward(11, 35, this.Context, this.GameConfiguration, 8);
            this.CreateQuest("Put the Lost Tower to Sleep! (2)", 15, 73, 190, 193, 257, CharacterClassNumber.DarkWizard)
                .WithMonsterKillRequirement(50, 39, this.Context, this.GameConfiguration)
                .WithMonsterKillRequirement(30, 34, this.Context, this.GameConfiguration)
                .WithItemReward(11, 35, this.Context, this.GameConfiguration, 8);

            this.CreateQuest("Put the Lost Tower to Sleep! (2)", 15, 74, 190, 193, 257, CharacterClassNumber.MuseElf)
                .WithMonsterKillRequirement(100, 37, this.Context, this.GameConfiguration)
                .WithItemReward(11, 36, this.Context, this.GameConfiguration, 8);
            this.CreateQuest("Put the Lost Tower to Sleep! (2)", 15, 74, 190, 193, 257, CharacterClassNumber.FairyElf)
                .WithMonsterKillRequirement(100, 37, this.Context, this.GameConfiguration)
                .WithItemReward(11, 36, this.Context, this.GameConfiguration, 8);

            this.CreateQuest("Put the Lost Tower to Sleep! (2)", 15, 75, 190, 193, 257, CharacterClassNumber.BloodySummoner)
                .WithMonsterKillRequirement(50, 39, this.Context, this.GameConfiguration)
                .WithMonsterKillRequirement(30, 34, this.Context, this.GameConfiguration)
                .WithItemReward(11, 41, this.Context, this.GameConfiguration, 8);
            this.CreateQuest("Put the Lost Tower to Sleep! (2)", 15, 75, 190, 193, 257, CharacterClassNumber.Summoner)
                .WithMonsterKillRequirement(50, 39, this.Context, this.GameConfiguration)
                .WithMonsterKillRequirement(30, 34, this.Context, this.GameConfiguration)
                .WithItemReward(11, 41, this.Context, this.GameConfiguration, 8);

            this.CreateQuest("Put the Lost Tower to Sleep! (3)", 15, 78, 194, 197, 257, CharacterClassNumber.BladeKnight)
                .WithMonsterKillRequirement(100, 40, this.Context, this.GameConfiguration)
                .WithItemReward(7, 34, this.Context, this.GameConfiguration, 8);
            this.CreateQuest("Put the Lost Tower to Sleep! (3)", 15, 78, 194, 197, 257, CharacterClassNumber.DarkKnight)
                .WithMonsterKillRequirement(100, 40, this.Context, this.GameConfiguration)
                .WithItemReward(7, 34, this.Context, this.GameConfiguration, 8);

            this.CreateQuest("Put the Lost Tower to Sleep! (3)", 15, 79, 194, 197, 257, CharacterClassNumber.SoulMaster)
                .WithMonsterKillRequirement(50, 34, this.Context, this.GameConfiguration)
                .WithMonsterKillRequirement(50, 41, this.Context, this.GameConfiguration)
                .WithItemReward(7, 35, this.Context, this.GameConfiguration, 8);
            this.CreateQuest("Put the Lost Tower to Sleep! (3)", 15, 79, 194, 197, 257, CharacterClassNumber.DarkWizard)
                .WithMonsterKillRequirement(50, 34, this.Context, this.GameConfiguration)
                .WithMonsterKillRequirement(50, 41, this.Context, this.GameConfiguration)
                .WithItemReward(7, 35, this.Context, this.GameConfiguration, 8);

            this.CreateQuest("Put the Lost Tower to Sleep! (3)", 15, 80, 194, 197, 257, CharacterClassNumber.MuseElf)
                .WithMonsterKillRequirement(100, 40, this.Context, this.GameConfiguration)
                .WithItemReward(7, 36, this.Context, this.GameConfiguration, 8);
            this.CreateQuest("Put the Lost Tower to Sleep! (3)", 15, 80, 194, 197, 257, CharacterClassNumber.FairyElf)
                .WithMonsterKillRequirement(100, 40, this.Context, this.GameConfiguration)
                .WithItemReward(7, 36, this.Context, this.GameConfiguration, 8);

            this.CreateQuest("Put the Lost Tower to Sleep! (3)", 15, 81, 194, 197, 257, CharacterClassNumber.BloodySummoner)
                .WithMonsterKillRequirement(50, 34, this.Context, this.GameConfiguration)
                .WithMonsterKillRequirement(50, 41, this.Context, this.GameConfiguration)
                .WithItemReward(7, 41, this.Context, this.GameConfiguration, 8);
            this.CreateQuest("Put the Lost Tower to Sleep! (3)", 15, 81, 194, 197, 257, CharacterClassNumber.Summoner)
                .WithMonsterKillRequirement(50, 34, this.Context, this.GameConfiguration)
                .WithMonsterKillRequirement(50, 41, this.Context, this.GameConfiguration)
                .WithItemReward(7, 41, this.Context, this.GameConfiguration, 8);

            this.CreateQuest("Put the Lost Tower to Sleep! (4)", 15, 84, 198, 201, 257, CharacterClassNumber.BladeKnight)
                .WithMonsterKillRequirement(50, 40, this.Context, this.GameConfiguration)
                .WithMonsterKillRequirement(30, 35, this.Context, this.GameConfiguration)
                .WithItemReward(9, 34, this.Context, this.GameConfiguration, 8);
            this.CreateQuest("Put the Lost Tower to Sleep! (4)", 15, 84, 198, 201, 257, CharacterClassNumber.DarkKnight)
                .WithMonsterKillRequirement(50, 40, this.Context, this.GameConfiguration)
                .WithMonsterKillRequirement(30, 35, this.Context, this.GameConfiguration)
                .WithItemReward(9, 34, this.Context, this.GameConfiguration, 8);

            this.CreateQuest("Put the Lost Tower to Sleep! (4)", 15, 85, 198, 201, 257, CharacterClassNumber.SoulMaster)
                .WithMonsterKillRequirement(100, 41, this.Context, this.GameConfiguration)
                .WithItemReward(9, 35, this.Context, this.GameConfiguration, 8);
            this.CreateQuest("Put the Lost Tower to Sleep! (4)", 15, 85, 198, 201, 257, CharacterClassNumber.DarkWizard)
                .WithMonsterKillRequirement(100, 41, this.Context, this.GameConfiguration)
                .WithItemReward(9, 35, this.Context, this.GameConfiguration, 8);

            this.CreateQuest("Put the Lost Tower to Sleep! (4)", 15, 86, 198, 201, 257, CharacterClassNumber.MuseElf)
                .WithMonsterKillRequirement(50, 40, this.Context, this.GameConfiguration)
                .WithMonsterKillRequirement(30, 35, this.Context, this.GameConfiguration)
                .WithItemReward(9, 36, this.Context, this.GameConfiguration, 8);
            this.CreateQuest("Put the Lost Tower to Sleep! (4)", 15, 86, 198, 201, 257, CharacterClassNumber.FairyElf)
                .WithMonsterKillRequirement(50, 40, this.Context, this.GameConfiguration)
                .WithMonsterKillRequirement(30, 35, this.Context, this.GameConfiguration)
                .WithItemReward(9, 36, this.Context, this.GameConfiguration, 8);

            this.CreateQuest("Put the Lost Tower to Sleep! (4)", 15, 87, 198, 201, 257, CharacterClassNumber.BloodySummoner)
                .WithMonsterKillRequirement(100, 41, this.Context, this.GameConfiguration)
                .WithItemReward(9, 41, this.Context, this.GameConfiguration, 8);
            this.CreateQuest("Put the Lost Tower to Sleep! (4)", 15, 87, 198, 201, 257, CharacterClassNumber.Summoner)
                .WithMonsterKillRequirement(100, 41, this.Context, this.GameConfiguration)
                .WithItemReward(9, 41, this.Context, this.GameConfiguration, 8);

            this.CreateQuest("Put the Lost Tower to Sleep! (5)", 15, 90, 202, 205, 257, CharacterClassNumber.BladeKnight)
                .WithMonsterKillRequirement(50, 35, this.Context, this.GameConfiguration)
                .WithMonsterKillRequirement(10, 38, this.Context, this.GameConfiguration)
                .WithItemReward(8, 34, this.Context, this.GameConfiguration, 8);
            this.CreateQuest("Put the Lost Tower to Sleep! (5)", 15, 90, 202, 205, 257, CharacterClassNumber.DarkKnight)
                .WithMonsterKillRequirement(50, 35, this.Context, this.GameConfiguration)
                .WithMonsterKillRequirement(10, 38, this.Context, this.GameConfiguration)
                .WithItemReward(8, 34, this.Context, this.GameConfiguration, 8);

            this.CreateQuest("Put the Lost Tower to Sleep! (5)", 15, 91, 202, 205, 257, CharacterClassNumber.SoulMaster)
                .WithMonsterKillRequirement(100, 37, this.Context, this.GameConfiguration)
                .WithItemReward(8, 35, this.Context, this.GameConfiguration, 8);
            this.CreateQuest("Put the Lost Tower to Sleep! (5)", 15, 91, 202, 205, 257, CharacterClassNumber.DarkWizard)
                .WithMonsterKillRequirement(100, 37, this.Context, this.GameConfiguration)
                .WithItemReward(8, 35, this.Context, this.GameConfiguration, 8);

            this.CreateQuest("Put the Lost Tower to Sleep! (5)", 15, 92, 202, 205, 257, CharacterClassNumber.MuseElf)
                .WithMonsterKillRequirement(50, 35, this.Context, this.GameConfiguration)
                .WithMonsterKillRequirement(10, 38, this.Context, this.GameConfiguration)
                .WithItemReward(8, 36, this.Context, this.GameConfiguration, 8);
            this.CreateQuest("Put the Lost Tower to Sleep! (5)", 15, 92, 202, 205, 257, CharacterClassNumber.FairyElf)
                .WithMonsterKillRequirement(50, 35, this.Context, this.GameConfiguration)
                .WithMonsterKillRequirement(10, 38, this.Context, this.GameConfiguration)
                .WithItemReward(8, 36, this.Context, this.GameConfiguration, 8);

            this.CreateQuest("Put the Lost Tower to Sleep! (5)", 15, 93, 202, 205, 257, CharacterClassNumber.BloodySummoner)
                .WithMonsterKillRequirement(100, 37, this.Context, this.GameConfiguration)
                .WithItemReward(8, 41, this.Context, this.GameConfiguration, 8);
            this.CreateQuest("Put the Lost Tower to Sleep! (5)", 15, 93, 202, 205, 257, CharacterClassNumber.Summoner)
                .WithMonsterKillRequirement(100, 37, this.Context, this.GameConfiguration)
                .WithItemReward(8, 41, this.Context, this.GameConfiguration, 8);

            this.CreateQuest("Put the Lost Tower to Sleep! (6)", 15, 96, 206, 209, 257, CharacterClassNumber.BladeKnight)
                .WithMonsterKillRequirement(30, 35, this.Context, this.GameConfiguration)
                .WithMonsterKillRequirement(20, 38, this.Context, this.GameConfiguration)
                .WithItemReward(2, 5, this.Context, this.GameConfiguration, 7, 0, hasLuck: false, hasSkill: true);
            this.CreateQuest("Put the Lost Tower to Sleep! (6)", 15, 96, 206, 209, 257, CharacterClassNumber.DarkKnight)
                .WithMonsterKillRequirement(30, 35, this.Context, this.GameConfiguration)
                .WithMonsterKillRequirement(20, 38, this.Context, this.GameConfiguration)
                .WithItemReward(2, 5, this.Context, this.GameConfiguration, 7, 0, hasLuck: false, hasSkill: true);

            this.CreateQuest("Put the Lost Tower to Sleep! (6)", 15, 97, 206, 209, 257, CharacterClassNumber.SoulMaster)
                .WithMonsterKillRequirement(100, 40, this.Context, this.GameConfiguration)
                .WithItemReward(5, 5, this.Context, this.GameConfiguration, 8);
            this.CreateQuest("Put the Lost Tower to Sleep! (6)", 15, 97, 206, 209, 257, CharacterClassNumber.DarkWizard)
                .WithMonsterKillRequirement(100, 40, this.Context, this.GameConfiguration)
                .WithItemReward(5, 5, this.Context, this.GameConfiguration, 8);

            this.CreateQuest("Put the Lost Tower to Sleep! (6)", 15, 98, 206, 209, 257, CharacterClassNumber.MuseElf)
                .WithMonsterKillRequirement(30, 35, this.Context, this.GameConfiguration)
                .WithMonsterKillRequirement(20, 38, this.Context, this.GameConfiguration)
                .WithItemReward(4, 14, this.Context, this.GameConfiguration, 7, 0, hasLuck: false, hasSkill: true);
            this.CreateQuest("Put the Lost Tower to Sleep! (6)", 15, 98, 206, 209, 257, CharacterClassNumber.FairyElf)
                .WithMonsterKillRequirement(30, 35, this.Context, this.GameConfiguration)
                .WithMonsterKillRequirement(20, 38, this.Context, this.GameConfiguration)
                .WithItemReward(4, 14, this.Context, this.GameConfiguration, 7, 0, hasLuck: false, hasSkill: true);

            this.CreateQuest("Put the Lost Tower to Sleep! (6)", 15, 99, 206, 209, 257, CharacterClassNumber.BloodySummoner)
                .WithMonsterKillRequirement(100, 40, this.Context, this.GameConfiguration)
                .WithItemReward(5, 16, this.Context, this.GameConfiguration, 8);
            this.CreateQuest("Put the Lost Tower to Sleep! (6)", 15, 99, 206, 209, 257, CharacterClassNumber.Summoner)
                .WithMonsterKillRequirement(100, 40, this.Context, this.GameConfiguration)
                .WithItemReward(5, 16, this.Context, this.GameConfiguration, 8);

            this.CreateQuest("Random Quest (1)", 19, 5, 260, 289, 257)
                .WithMonsterKillRequirement(30, 57, this.Context, this.GameConfiguration)
                .WithExperienceReward(50000, this.Context)
                .WithItemReward(5, 17, this.Context, this.GameConfiguration, 4, 12);

            this.CreateQuest("Random Quest (1)", 19, 6, 260, 289, 257)
                .WithMonsterKillRequirement(30, 57, this.Context, this.GameConfiguration)
                .WithItemReward(2, 6, this.Context, this.GameConfiguration, 4, 12, hasLuck: false, hasSkill: true)
                .WithMoneyReward(100000, this.Context);

            this.CreateQuest("Random Quest (1)", 19, 7, 260, 289, 257)
                .WithMonsterKillRequirement(30, 57, this.Context, this.GameConfiguration)
                .WithItemReward(14, 14, this.Context, this.GameConfiguration);

            this.CreateQuest("Random Quest (1)", 19, 8, 260, 289, 257)
                .WithMonsterKillRequirement(30, 57, this.Context, this.GameConfiguration)
                .WithItemReward(13, 1, this.Context, this.GameConfiguration)
                .WithItemReward(14, 19, this.Context, this.GameConfiguration, 4);

            this.CreateQuest("Random Quest (1)", 19, 9, 260, 289, 257)
                .WithMonsterKillRequirement(30, 57, this.Context, this.GameConfiguration)
                .WithItemReward(12, 15, this.Context, this.GameConfiguration);

            this.CreateQuest("Random Quest (1)", 19, 10, 260, 289, 257)
                .WithMonsterKillRequirement(30, 57, this.Context, this.GameConfiguration)
                .WithItemReward(13, 0, this.Context, this.GameConfiguration);

            this.CreateQuest("Random Quest (1)", 19, 11, 260, 289, 257)
                .WithMonsterKillRequirement(30, 57, this.Context, this.GameConfiguration)
                .WithItemReward(4, 16, this.Context, this.GameConfiguration, 4, 12, hasLuck: false, hasSkill: true);

            this.CreateQuest("Random Quest (1)", 19, 12, 260, 289, 257)
                .WithMonsterKillRequirement(30, 57, this.Context, this.GameConfiguration)
                .WithMoneyReward(200000, this.Context)
                .WithItemReward(13, 18, this.Context, this.GameConfiguration, 6);

            this.CreateQuest("Random Quest (1)", 19, 13, 260, 289, 257)
                .WithMonsterKillRequirement(20, 62, this.Context, this.GameConfiguration)
                .WithMonsterKillRequirement(20, 58, this.Context, this.GameConfiguration)
                .WithItemReward(2, 10, this.Context, this.GameConfiguration, 4, 12, hasLuck: false, hasSkill: true);

            this.CreateQuest("Random Quest (1)", 19, 14, 260, 289, 257)
                .WithMonsterKillRequirement(20, 62, this.Context, this.GameConfiguration)
                .WithMonsterKillRequirement(20, 58, this.Context, this.GameConfiguration)
                .WithItemReward(5, 9, this.Context, this.GameConfiguration, 4, 12);

            this.CreateQuest("Random Quest (1)", 19, 15, 260, 289, 257)
                .WithMonsterKillRequirement(20, 62, this.Context, this.GameConfiguration)
                .WithMonsterKillRequirement(20, 58, this.Context, this.GameConfiguration)
                .WithItemReward(13, 3, this.Context, this.GameConfiguration, 0, 0, hasLuck: false, hasSkill: true);

            this.CreateQuest("Random Quest (1)", 19, 16, 260, 289, 257)
                .WithMonsterKillRequirement(20, 62, this.Context, this.GameConfiguration)
                .WithMonsterKillRequirement(20, 58, this.Context, this.GameConfiguration)
                .WithItemReward(2, 6, this.Context, this.GameConfiguration, 4, 12, hasLuck: false, hasSkill: true)
                .WithExperienceReward(120000, this.Context);

            this.CreateQuest("Random Quest (1)", 19, 17, 260, 289, 257)
                .WithMonsterKillRequirement(20, 62, this.Context, this.GameConfiguration)
                .WithMonsterKillRequirement(20, 58, this.Context, this.GameConfiguration)
                .WithItemReward(14, 14, this.Context, this.GameConfiguration);

            this.CreateQuest("Random Quest (1)", 19, 18, 260, 289, 257)
                .WithMonsterKillRequirement(20, 62, this.Context, this.GameConfiguration)
                .WithMonsterKillRequirement(20, 58, this.Context, this.GameConfiguration)
                .WithItemReward(5, 17, this.Context, this.GameConfiguration, 4, 12);

            this.CreateQuest("Random Quest (1)", 19, 19, 260, 289, 257)
                .WithMonsterKillRequirement(20, 62, this.Context, this.GameConfiguration)
                .WithMonsterKillRequirement(20, 58, this.Context, this.GameConfiguration)
                .WithItemReward(14, 41, this.Context, this.GameConfiguration);

            this.CreateQuest("Random Quest (1)", 19, 20, 260, 289, 257)
                .WithMonsterKillRequirement(20, 62, this.Context, this.GameConfiguration)
                .WithMonsterKillRequirement(20, 58, this.Context, this.GameConfiguration)
                .WithItemReward(3, 10, this.Context, this.GameConfiguration, 4, 12, hasLuck: false, hasSkill: true);

            this.CreateQuest("Random Quest (1)", 19, 21, 260, 289, 257)
                .WithMonsterKillRequirement(20, 62, this.Context, this.GameConfiguration)
                .WithMonsterKillRequirement(10, 61, this.Context, this.GameConfiguration)
                .WithItemReward(14, 42, this.Context, this.GameConfiguration);

            this.CreateQuest("Random Quest (1)", 19, 22, 260, 289, 257)
                .WithMonsterKillRequirement(20, 62, this.Context, this.GameConfiguration)
                .WithMonsterKillRequirement(10, 61, this.Context, this.GameConfiguration)
                .WithItemReward(13, 18, this.Context, this.GameConfiguration, 5);

            this.CreateQuest("Random Quest (1)", 19, 23, 260, 289, 257)
                .WithMonsterKillRequirement(20, 62, this.Context, this.GameConfiguration)
                .WithMonsterKillRequirement(10, 61, this.Context, this.GameConfiguration)
                .WithItemReward(2, 10, this.Context, this.GameConfiguration, 4, 12, hasLuck: false, hasSkill: true);

            this.CreateQuest("Random Quest (1)", 19, 24, 260, 289, 257)
                .WithMonsterKillRequirement(20, 62, this.Context, this.GameConfiguration)
                .WithMonsterKillRequirement(10, 61, this.Context, this.GameConfiguration)
                .WithItemReward(13, 1, this.Context, this.GameConfiguration)
                .WithItemReward(12, 15, this.Context, this.GameConfiguration);

            this.CreateQuest("Random Quest (1)", 19, 25, 260, 289, 257)
                .WithMonsterKillRequirement(20, 62, this.Context, this.GameConfiguration)
                .WithMonsterKillRequirement(10, 61, this.Context, this.GameConfiguration)
                .WithItemReward(14, 14, this.Context, this.GameConfiguration);

            this.CreateQuest("Random Quest (1)", 19, 26, 260, 289, 257)
                .WithMonsterKillRequirement(20, 62, this.Context, this.GameConfiguration)
                .WithMonsterKillRequirement(10, 61, this.Context, this.GameConfiguration)
                .WithItemReward(5, 9, this.Context, this.GameConfiguration, 4, 12);

            this.CreateQuest("Random Quest (1)", 19, 27, 260, 289, 257)
                .WithMonsterKillRequirement(20, 62, this.Context, this.GameConfiguration)
                .WithMonsterKillRequirement(10, 61, this.Context, this.GameConfiguration)
                .WithExperienceReward(50000, this.Context)
                .WithItemReward(13, 18, this.Context, this.GameConfiguration, 6);

            this.CreateQuest("Random Quest (1)", 19, 28, 260, 289, 257)
                .WithMonsterKillRequirement(20, 62, this.Context, this.GameConfiguration)
                .WithMonsterKillRequirement(10, 61, this.Context, this.GameConfiguration)
                .WithItemReward(14, 42, this.Context, this.GameConfiguration);

            this.CreateQuest("Random Quest (1)", 19, 29, 260, 289, 257)
                .WithMonsterKillRequirement(20, 62, this.Context, this.GameConfiguration)
                .WithItemRequirement(5, 6, this.Context, this.GameConfiguration, itemOption: 12)
                .WithItemReward(14, 41, this.Context, this.GameConfiguration);

            this.CreateQuest("Random Quest (1)", 19, 30, 260, 289, 257)
                .WithMonsterKillRequirement(20, 62, this.Context, this.GameConfiguration)
                .WithItemRequirement(5, 6, this.Context, this.GameConfiguration, itemOption: 12)
                .WithItemReward(14, 14, this.Context, this.GameConfiguration);

            this.CreateQuest("Random Quest (1)", 19, 31, 260, 289, 257)
                .WithMonsterKillRequirement(20, 62, this.Context, this.GameConfiguration)
                .WithItemRequirement(5, 6, this.Context, this.GameConfiguration, itemOption: 12)
                .WithItemReward(3, 10, this.Context, this.GameConfiguration, 4, 12, hasLuck: false, hasSkill: true);

            this.CreateQuest("Random Quest (1)", 19, 32, 260, 289, 257)
                .WithMonsterKillRequirement(20, 62, this.Context, this.GameConfiguration)
                .WithItemRequirement(5, 6, this.Context, this.GameConfiguration, itemOption: 12)
                .WithItemReward(14, 19, this.Context, this.GameConfiguration, 5);

            this.CreateQuest("Random Quest (1)", 19, 33, 260, 289, 257)
                .WithMonsterKillRequirement(20, 62, this.Context, this.GameConfiguration)
                .WithItemRequirement(5, 6, this.Context, this.GameConfiguration, itemOption: 12)
                .WithItemReward(13, 0, this.Context, this.GameConfiguration);

            this.CreateQuest("Random Quest (1)", 19, 34, 260, 289, 257)
                .WithMonsterKillRequirement(20, 62, this.Context, this.GameConfiguration)
                .WithItemRequirement(5, 6, this.Context, this.GameConfiguration, itemOption: 12)
                .WithItemReward(14, 19, this.Context, this.GameConfiguration, 4);

            this.CreateQuest("Random Quest (1)", 19, 35, 260, 289, 257)
                .WithMonsterKillRequirement(20, 62, this.Context, this.GameConfiguration)
                .WithItemRequirement(5, 6, this.Context, this.GameConfiguration, itemOption: 12)
                .WithItemReward(4, 16, this.Context, this.GameConfiguration, 4, 12, hasLuck: false, hasSkill: true)
                .WithItemReward(13, 3, this.Context, this.GameConfiguration, 0, 0, hasLuck: false, hasSkill: true);

            this.CreateQuest("Random Quest (1)", 19, 36, 260, 289, 257)
                .WithMonsterKillRequirement(20, 62, this.Context, this.GameConfiguration)
                .WithItemRequirement(5, 6, this.Context, this.GameConfiguration, itemOption: 12)
                .WithItemReward(14, 13, this.Context, this.GameConfiguration);

            this.CreateQuest("Random Quest (1)", 19, 37, 260, 289, 257)
                .WithMonsterKillRequirement(20, 62, this.Context, this.GameConfiguration)
                .WithItemRequirement(13, 2, this.Context, this.GameConfiguration, itemCount: 10)
                .WithItemReward(5, 17, this.Context, this.GameConfiguration, 4, 12);

            this.CreateQuest("Random Quest (1)", 19, 38, 260, 289, 257)
                .WithMonsterKillRequirement(20, 62, this.Context, this.GameConfiguration)
                .WithItemRequirement(13, 2, this.Context, this.GameConfiguration, itemCount: 10)
                .WithItemReward(2, 6, this.Context, this.GameConfiguration, 4, 12, hasLuck: false, hasSkill: true);

            this.CreateQuest("Random Quest (1)", 19, 39, 260, 289, 257)
                .WithMonsterKillRequirement(20, 62, this.Context, this.GameConfiguration)
                .WithItemRequirement(13, 2, this.Context, this.GameConfiguration, itemCount: 10)
                .WithExperienceReward(50000, this.Context)
                .WithItemReward(4, 16, this.Context, this.GameConfiguration, 4, 12, hasLuck: false, hasSkill: true);

            this.CreateQuest("Random Quest (1)", 19, 40, 260, 289, 257)
                .WithMonsterKillRequirement(20, 62, this.Context, this.GameConfiguration)
                .WithItemRequirement(13, 2, this.Context, this.GameConfiguration, itemCount: 10)
                .WithItemReward(14, 19, this.Context, this.GameConfiguration, 5)
                .WithItemReward(14, 14, this.Context, this.GameConfiguration);

            this.CreateQuest("Random Quest (1)", 19, 41, 260, 289, 257)
                .WithMonsterKillRequirement(20, 62, this.Context, this.GameConfiguration)
                .WithItemRequirement(13, 2, this.Context, this.GameConfiguration, itemCount: 10);

            this.CreateQuest("Random Quest (1)", 19, 42, 260, 289, 257)
                .WithMonsterKillRequirement(20, 62, this.Context, this.GameConfiguration)
                .WithItemRequirement(13, 2, this.Context, this.GameConfiguration, itemCount: 10)
                .WithItemReward(14, 42, this.Context, this.GameConfiguration);

            this.CreateQuest("Random Quest (1)", 19, 43, 260, 289, 257)
                .WithMonsterKillRequirement(20, 62, this.Context, this.GameConfiguration)
                .WithItemRequirement(13, 2, this.Context, this.GameConfiguration, itemCount: 10)
                .WithItemReward(14, 41, this.Context, this.GameConfiguration);

            this.CreateQuest("Random Quest (1)", 19, 44, 260, 289, 257)
                .WithMonsterKillRequirement(20, 62, this.Context, this.GameConfiguration)
                .WithItemRequirement(13, 2, this.Context, this.GameConfiguration, itemCount: 10)
                .WithItemReward(5, 9, this.Context, this.GameConfiguration, 4, 12)
                .WithItemReward(2, 6, this.Context, this.GameConfiguration, 4, 12, hasLuck: false, hasSkill: true);

            this.CreateQuest("Random Quest (2)", 19, 50, 290, 319, 257)
                .WithMonsterKillRequirement(30, 70, this.Context, this.GameConfiguration)
                .WithItemReward(5, 17, this.Context, this.GameConfiguration, 6, 12);

            this.CreateQuest("Random Quest (2)", 19, 51, 290, 319, 257)
                .WithMonsterKillRequirement(30, 70, this.Context, this.GameConfiguration)
                .WithItemReward(14, 42, this.Context, this.GameConfiguration);

            this.CreateQuest("Random Quest (2)", 19, 52, 290, 319, 257)
                .WithMonsterKillRequirement(30, 70, this.Context, this.GameConfiguration)
                .WithItemReward(2, 10, this.Context, this.GameConfiguration, 6, 12, hasLuck: false, hasSkill: true);

            this.CreateQuest("Random Quest (2)", 19, 53, 290, 319, 257)
                .WithMonsterKillRequirement(30, 70, this.Context, this.GameConfiguration)
                .WithItemReward(14, 19, this.Context, this.GameConfiguration, 5)
                .WithExperienceReward(140000, this.Context);

            this.CreateQuest("Random Quest (2)", 19, 54, 290, 319, 257)
                .WithMonsterKillRequirement(30, 70, this.Context, this.GameConfiguration)
                .WithMoneyReward(200000, this.Context)
                .WithItemReward(14, 19, this.Context, this.GameConfiguration, 6);

            this.CreateQuest("Random Quest (2)", 19, 55, 290, 319, 257)
                .WithMonsterKillRequirement(30, 70, this.Context, this.GameConfiguration)
                .WithItemReward(14, 13, this.Context, this.GameConfiguration)
                .WithItemReward(13, 0, this.Context, this.GameConfiguration);

            this.CreateQuest("Random Quest (2)", 19, 56, 290, 319, 257)
                .WithMonsterKillRequirement(30, 70, this.Context, this.GameConfiguration)
                .WithItemReward(13, 1, this.Context, this.GameConfiguration)
                .WithExperienceReward(70000, this.Context);

            this.CreateQuest("Random Quest (2)", 19, 57, 290, 319, 257)
                .WithMonsterKillRequirement(30, 70, this.Context, this.GameConfiguration)
                .WithItemReward(14, 13, this.Context, this.GameConfiguration);

            this.CreateQuest("Random Quest (2)", 19, 58, 290, 319, 257)
                .WithMonsterKillRequirement(20, 69, this.Context, this.GameConfiguration)
                .WithMonsterKillRequirement(20, 73, this.Context, this.GameConfiguration)
                .WithItemReward(4, 16, this.Context, this.GameConfiguration, 6, 12, hasLuck: false, hasSkill: true);

            this.CreateQuest("Random Quest (2)", 19, 59, 290, 319, 257)
                .WithMonsterKillRequirement(20, 69, this.Context, this.GameConfiguration)
                .WithMonsterKillRequirement(20, 73, this.Context, this.GameConfiguration)
                .WithItemReward(12, 15, this.Context, this.GameConfiguration)
                .WithItemReward(13, 3, this.Context, this.GameConfiguration, 0, 0, hasLuck: false, hasSkill: true);

            this.CreateQuest("Random Quest (2)", 19, 60, 290, 319, 257)
                .WithMonsterKillRequirement(20, 69, this.Context, this.GameConfiguration)
                .WithMonsterKillRequirement(20, 73, this.Context, this.GameConfiguration)
                .WithItemReward(13, 18, this.Context, this.GameConfiguration, 6);

            this.CreateQuest("Random Quest (2)", 19, 61, 290, 319, 257)
                .WithMonsterKillRequirement(20, 69, this.Context, this.GameConfiguration)
                .WithMonsterKillRequirement(20, 73, this.Context, this.GameConfiguration)
                .WithItemReward(14, 14, this.Context, this.GameConfiguration);

            this.CreateQuest("Random Quest (2)", 19, 62, 290, 319, 257)
                .WithMonsterKillRequirement(20, 69, this.Context, this.GameConfiguration)
                .WithMonsterKillRequirement(20, 73, this.Context, this.GameConfiguration)
                .WithItemReward(3, 10, this.Context, this.GameConfiguration, 6, 12, hasLuck: false, hasSkill: true);

            this.CreateQuest("Random Quest (2)", 19, 63, 290, 319, 257)
                .WithMonsterKillRequirement(20, 69, this.Context, this.GameConfiguration)
                .WithMonsterKillRequirement(20, 73, this.Context, this.GameConfiguration)
                .WithItemReward(14, 41, this.Context, this.GameConfiguration)
                .WithItemReward(13, 18, this.Context, this.GameConfiguration, 7);

            this.CreateQuest("Random Quest (2)", 19, 64, 290, 319, 257)
                .WithMonsterKillRequirement(20, 69, this.Context, this.GameConfiguration)
                .WithMonsterKillRequirement(20, 73, this.Context, this.GameConfiguration)
                .WithItemReward(14, 16, this.Context, this.GameConfiguration)
                .WithExperienceReward(140000, this.Context);

            this.CreateQuest("Random Quest (2)", 19, 65, 290, 319, 257)
                .WithMonsterKillRequirement(20, 69, this.Context, this.GameConfiguration)
                .WithMonsterKillRequirement(20, 73, this.Context, this.GameConfiguration)
                .WithItemReward(13, 18, this.Context, this.GameConfiguration, 6)
                .WithItemReward(13, 3, this.Context, this.GameConfiguration, 0, 0, hasLuck: false, hasSkill: true);

            this.CreateQuest("Random Quest (2)", 19, 66, 290, 319, 257)
                .WithMonsterKillRequirement(20, 69, this.Context, this.GameConfiguration)
                .WithMonsterKillRequirement(10, 74, this.Context, this.GameConfiguration)
                .WithItemReward(5, 9, this.Context, this.GameConfiguration, 6, 12);

            this.CreateQuest("Random Quest (2)", 19, 67, 290, 319, 257)
                .WithMonsterKillRequirement(20, 69, this.Context, this.GameConfiguration)
                .WithMonsterKillRequirement(10, 74, this.Context, this.GameConfiguration)
                .WithItemReward(4, 16, this.Context, this.GameConfiguration, 6, 12, hasLuck: false, hasSkill: true);

            this.CreateQuest("Random Quest (2)", 19, 68, 290, 319, 257)
                .WithMonsterKillRequirement(20, 69, this.Context, this.GameConfiguration)
                .WithMonsterKillRequirement(10, 74, this.Context, this.GameConfiguration)
                .WithExperienceReward(70000, this.Context)
                .WithItemReward(13, 18, this.Context, this.GameConfiguration, 7);

            this.CreateQuest("Random Quest (2)", 19, 69, 290, 319, 257)
                .WithMonsterKillRequirement(20, 69, this.Context, this.GameConfiguration)
                .WithMonsterKillRequirement(10, 74, this.Context, this.GameConfiguration)
                .WithMoneyReward(200000, this.Context)
                .WithItemReward(14, 41, this.Context, this.GameConfiguration);

            this.CreateQuest("Random Quest (2)", 19, 70, 290, 319, 257)
                .WithMonsterKillRequirement(20, 69, this.Context, this.GameConfiguration)
                .WithMonsterKillRequirement(10, 74, this.Context, this.GameConfiguration)
                .WithItemReward(3, 10, this.Context, this.GameConfiguration, 6, 12, hasLuck: false, hasSkill: true);

            this.CreateQuest("Random Quest (2)", 19, 71, 290, 319, 257)
                .WithMonsterKillRequirement(20, 69, this.Context, this.GameConfiguration)
                .WithMonsterKillRequirement(10, 74, this.Context, this.GameConfiguration)
                .WithItemReward(5, 17, this.Context, this.GameConfiguration, 6, 12);

            this.CreateQuest("Random Quest (2)", 19, 72, 290, 319, 257)
                .WithMonsterKillRequirement(20, 69, this.Context, this.GameConfiguration)
                .WithMonsterKillRequirement(10, 74, this.Context, this.GameConfiguration)
                .WithItemReward(14, 19, this.Context, this.GameConfiguration, 6)
                .WithItemReward(13, 0, this.Context, this.GameConfiguration);

            this.CreateQuest("Random Quest (2)", 19, 73, 290, 319, 257)
                .WithMonsterKillRequirement(20, 69, this.Context, this.GameConfiguration)
                .WithMonsterKillRequirement(10, 74, this.Context, this.GameConfiguration)
                .WithItemReward(14, 13, this.Context, this.GameConfiguration);

            this.CreateQuest("Random Quest (2)", 19, 74, 290, 319, 257)
                .WithMonsterKillRequirement(20, 69, this.Context, this.GameConfiguration)
                .WithItemRequirement(0, 16, this.Context, this.GameConfiguration, itemOption: 12)
                .WithItemReward(5, 9, this.Context, this.GameConfiguration, 6, 12);

            this.CreateQuest("Random Quest (2)", 19, 75, 290, 319, 257)
                .WithMonsterKillRequirement(20, 69, this.Context, this.GameConfiguration)
                .WithItemRequirement(0, 16, this.Context, this.GameConfiguration, itemOption: 12)
                .WithItemReward(2, 10, this.Context, this.GameConfiguration, 6, 12, hasLuck: false, hasSkill: true);

            this.CreateQuest("Random Quest (2)", 19, 76, 290, 319, 257)
                .WithMonsterKillRequirement(20, 69, this.Context, this.GameConfiguration)
                .WithItemRequirement(0, 16, this.Context, this.GameConfiguration, itemOption: 12)
                .WithItemReward(13, 1, this.Context, this.GameConfiguration)
                .WithItemReward(12, 15, this.Context, this.GameConfiguration);

            this.CreateQuest("Random Quest (2)", 19, 77, 290, 319, 257)
                .WithMonsterKillRequirement(20, 69, this.Context, this.GameConfiguration)
                .WithItemRequirement(0, 16, this.Context, this.GameConfiguration, itemOption: 12)
                .WithItemReward(14, 14, this.Context, this.GameConfiguration);

            this.CreateQuest("Random Quest (2)", 19, 78, 290, 319, 257)
                .WithMonsterKillRequirement(20, 69, this.Context, this.GameConfiguration)
                .WithItemRequirement(0, 16, this.Context, this.GameConfiguration, itemOption: 12)
                .WithItemReward(14, 42, this.Context, this.GameConfiguration);

            this.CreateQuest("Random Quest (2)", 19, 79, 290, 319, 257)
                .WithMonsterKillRequirement(20, 69, this.Context, this.GameConfiguration)
                .WithItemRequirement(0, 16, this.Context, this.GameConfiguration, itemOption: 12)
                .WithItemReward(14, 13, this.Context, this.GameConfiguration)
                .WithItemReward(14, 19, this.Context, this.GameConfiguration, 5);

            this.CreateQuest("Random Quest (2)", 19, 80, 290, 319, 257)
                .WithMonsterKillRequirement(20, 69, this.Context, this.GameConfiguration)
                .WithItemRequirement(0, 16, this.Context, this.GameConfiguration, itemOption: 12)
                .WithItemReward(14, 16, this.Context, this.GameConfiguration)
                .WithExperienceReward(140000, this.Context);

            this.CreateQuest("Random Quest (2)", 19, 81, 290, 319, 257)
                .WithMonsterKillRequirement(20, 69, this.Context, this.GameConfiguration)
                .WithItemRequirement(0, 16, this.Context, this.GameConfiguration, itemOption: 12)
                .WithItemReward(14, 19, this.Context, this.GameConfiguration, 6)
                .WithItemReward(14, 14, this.Context, this.GameConfiguration);

            this.CreateQuest("Random Quest (2)", 19, 82, 290, 319, 257)
                .WithMonsterKillRequirement(20, 69, this.Context, this.GameConfiguration)
                .WithItemRequirement(2, 5, this.Context, this.GameConfiguration, itemOption: 12)
                .WithItemReward(4, 16, this.Context, this.GameConfiguration, 6, 12, hasLuck: false, hasSkill: true);

            this.CreateQuest("Random Quest (2)", 19, 83, 290, 319, 257)
                .WithMonsterKillRequirement(20, 69, this.Context, this.GameConfiguration)
                .WithItemRequirement(2, 5, this.Context, this.GameConfiguration, itemOption: 12)
                .WithItemReward(14, 13, this.Context, this.GameConfiguration);

            this.CreateQuest("Random Quest (2)", 19, 84, 290, 319, 257)
                .WithMonsterKillRequirement(20, 69, this.Context, this.GameConfiguration)
                .WithItemRequirement(2, 5, this.Context, this.GameConfiguration, itemOption: 12)
                .WithMoneyReward(200000, this.Context)
                .WithItemReward(14, 42, this.Context, this.GameConfiguration);

            this.CreateQuest("Random Quest (2)", 19, 85, 290, 319, 257)
                .WithMonsterKillRequirement(20, 69, this.Context, this.GameConfiguration)
                .WithItemRequirement(2, 5, this.Context, this.GameConfiguration, itemOption: 12)
                .WithItemReward(2, 10, this.Context, this.GameConfiguration, 6, 12, hasLuck: false, hasSkill: true);

            this.CreateQuest("Random Quest (2)", 19, 86, 290, 319, 257)
                .WithMonsterKillRequirement(20, 69, this.Context, this.GameConfiguration)
                .WithItemRequirement(2, 5, this.Context, this.GameConfiguration, itemOption: 12)
                .WithItemReward(13, 18, this.Context, this.GameConfiguration, 7)
                .WithItemReward(14, 13, this.Context, this.GameConfiguration);

            this.CreateQuest("Random Quest (2)", 19, 87, 290, 319, 257)
                .WithMonsterKillRequirement(20, 69, this.Context, this.GameConfiguration)
                .WithItemRequirement(2, 5, this.Context, this.GameConfiguration, itemOption: 12)
                .WithItemReward(3, 10, this.Context, this.GameConfiguration, 6, 12, hasLuck: false, hasSkill: true);

            this.CreateQuest("Random Quest (2)", 19, 88, 290, 319, 257)
                .WithMonsterKillRequirement(20, 69, this.Context, this.GameConfiguration)
                .WithItemRequirement(2, 5, this.Context, this.GameConfiguration, itemOption: 12)
                .WithItemReward(5, 17, this.Context, this.GameConfiguration, 6, 12);

            this.CreateQuest("Random Quest (2)", 19, 89, 290, 319, 257)
                .WithMonsterKillRequirement(20, 69, this.Context, this.GameConfiguration)
                .WithItemRequirement(2, 5, this.Context, this.GameConfiguration, itemOption: 12)
                .WithItemReward(5, 9, this.Context, this.GameConfiguration, 6, 12);

            this.CreateQuest("Random Quest (3)", 19, 95, 320, 349, 257)
                .WithMonsterKillRequirement(20, 351, this.Context, this.GameConfiguration)
                .WithMonsterKillRequirement(20, 353, this.Context, this.GameConfiguration)
                .WithItemReward(5, 18, this.Context, this.GameConfiguration, 0, 12);

            this.CreateQuest("Random Quest (3)", 19, 96, 320, 349, 257)
                .WithMonsterKillRequirement(20, 351, this.Context, this.GameConfiguration)
                .WithMonsterKillRequirement(20, 353, this.Context, this.GameConfiguration)
                .WithItemReward(14, 13, this.Context, this.GameConfiguration);

            this.CreateQuest("Random Quest (3)", 19, 97, 320, 349, 257)
                .WithMonsterKillRequirement(20, 351, this.Context, this.GameConfiguration)
                .WithMonsterKillRequirement(20, 353, this.Context, this.GameConfiguration)
                .WithItemReward(13, 3, this.Context, this.GameConfiguration, 0, 0, hasLuck: false, hasSkill: true)
                .WithExperienceReward(90000, this.Context);

            this.CreateQuest("Random Quest (3)", 19, 98, 320, 349, 257)
                .WithMonsterKillRequirement(20, 351, this.Context, this.GameConfiguration)
                .WithMonsterKillRequirement(20, 353, this.Context, this.GameConfiguration)
                .WithExperienceReward(160000, this.Context)
                .WithItemReward(14, 14, this.Context, this.GameConfiguration);

            this.CreateQuest("Random Quest (3)", 19, 99, 320, 349, 257)
                .WithMonsterKillRequirement(20, 351, this.Context, this.GameConfiguration)
                .WithMonsterKillRequirement(20, 353, this.Context, this.GameConfiguration)
                .WithItemReward(13, 18, this.Context, this.GameConfiguration, 7)
                .WithItemReward(13, 0, this.Context, this.GameConfiguration);

            this.CreateQuest("Random Quest (3)", 19, 100, 320, 349, 257)
                .WithMonsterKillRequirement(20, 351, this.Context, this.GameConfiguration)
                .WithMonsterKillRequirement(20, 353, this.Context, this.GameConfiguration)
                .WithItemReward(14, 41, this.Context, this.GameConfiguration);

            this.CreateQuest("Random Quest (3)", 19, 101, 320, 349, 257)
                .WithMonsterKillRequirement(20, 351, this.Context, this.GameConfiguration)
                .WithMonsterKillRequirement(20, 353, this.Context, this.GameConfiguration)
                .WithItemReward(14, 13, this.Context, this.GameConfiguration)
                .WithItemReward(14, 19, this.Context, this.GameConfiguration, 5);

            this.CreateQuest("Random Quest (3)", 19, 102, 320, 349, 257)
                .WithMonsterKillRequirement(20, 351, this.Context, this.GameConfiguration)
                .WithMonsterKillRequirement(20, 353, this.Context, this.GameConfiguration)
                .WithItemReward(2, 6, this.Context, this.GameConfiguration, 5, 12, hasLuck: false, hasSkill: true);

            this.CreateQuest("Random Quest (3)", 19, 103, 320, 349, 257)
                .WithMonsterKillRequirement(20, 351, this.Context, this.GameConfiguration)
                .WithMonsterKillRequirement(10, 354, this.Context, this.GameConfiguration)
                .WithItemReward(4, 22, this.Context, this.GameConfiguration, 0, 12, hasLuck: false, hasSkill: true);

            this.CreateQuest("Random Quest (3)", 19, 104, 320, 349, 257)
                .WithMonsterKillRequirement(20, 351, this.Context, this.GameConfiguration)
                .WithMonsterKillRequirement(10, 354, this.Context, this.GameConfiguration)
                .WithItemReward(14, 19, this.Context, this.GameConfiguration, 6)
                .WithItemReward(13, 1, this.Context, this.GameConfiguration);

            this.CreateQuest("Random Quest (3)", 19, 105, 320, 349, 257)
                .WithMonsterKillRequirement(20, 351, this.Context, this.GameConfiguration)
                .WithMonsterKillRequirement(10, 354, this.Context, this.GameConfiguration)
                .WithItemReward(2, 6, this.Context, this.GameConfiguration, 6, 12, hasLuck: false, hasSkill: true);

            this.CreateQuest("Random Quest (3)", 19, 106, 320, 349, 257)
                .WithMonsterKillRequirement(20, 351, this.Context, this.GameConfiguration)
                .WithMonsterKillRequirement(10, 354, this.Context, this.GameConfiguration)
                .WithItemReward(5, 11, this.Context, this.GameConfiguration, 0, 12);

            this.CreateQuest("Random Quest (3)", 19, 107, 320, 349, 257)
                .WithMonsterKillRequirement(20, 351, this.Context, this.GameConfiguration)
                .WithMonsterKillRequirement(10, 354, this.Context, this.GameConfiguration)
                .WithItemReward(12, 15, this.Context, this.GameConfiguration);

            this.CreateQuest("Random Quest (3)", 19, 108, 320, 349, 257)
                .WithMonsterKillRequirement(20, 351, this.Context, this.GameConfiguration)
                .WithMonsterKillRequirement(10, 354, this.Context, this.GameConfiguration)
                .WithItemReward(14, 16, this.Context, this.GameConfiguration);

            this.CreateQuest("Random Quest (3)", 19, 109, 320, 349, 257)
                .WithMonsterKillRequirement(20, 351, this.Context, this.GameConfiguration)
                .WithMonsterKillRequirement(10, 354, this.Context, this.GameConfiguration)
                .WithItemReward(13, 18, this.Context, this.GameConfiguration, 6)
                .WithItemReward(14, 42, this.Context, this.GameConfiguration);

            this.CreateQuest("Random Quest (3)", 19, 110, 320, 349, 257)
                .WithMonsterKillRequirement(20, 351, this.Context, this.GameConfiguration)
                .WithMonsterKillRequirement(10, 354, this.Context, this.GameConfiguration)
                .WithItemReward(14, 1, this.Context, this.GameConfiguration);

            this.CreateQuest("Random Quest (3)", 19, 111, 320, 349, 257)
                .WithMonsterKillRequirement(20, 351, this.Context, this.GameConfiguration)
                .WithMonsterKillRequirement(5, 355, this.Context, this.GameConfiguration)
                .WithItemReward(0, 20, this.Context, this.GameConfiguration, 0, 12, hasLuck: false, hasSkill: true);

            this.CreateQuest("Random Quest (3)", 19, 112, 320, 349, 257)
                .WithMonsterKillRequirement(20, 351, this.Context, this.GameConfiguration)
                .WithMonsterKillRequirement(5, 355, this.Context, this.GameConfiguration)
                .WithItemReward(14, 41, this.Context, this.GameConfiguration);

            this.CreateQuest("Random Quest (3)", 19, 113, 320, 349, 257)
                .WithMonsterKillRequirement(20, 351, this.Context, this.GameConfiguration)
                .WithMonsterKillRequirement(5, 355, this.Context, this.GameConfiguration)
                .WithExperienceReward(160000, this.Context)
                .WithItemReward(13, 3, this.Context, this.GameConfiguration, 0, 0, hasLuck: false, hasSkill: true);

            this.CreateQuest("Random Quest (3)", 19, 114, 320, 349, 257)
                .WithMonsterKillRequirement(20, 351, this.Context, this.GameConfiguration)
                .WithMonsterKillRequirement(5, 355, this.Context, this.GameConfiguration)
                .WithItemReward(5, 11, this.Context, this.GameConfiguration, 0, 12);

            this.CreateQuest("Random Quest (3)", 19, 115, 320, 349, 257)
                .WithMonsterKillRequirement(20, 351, this.Context, this.GameConfiguration)
                .WithMonsterKillRequirement(5, 355, this.Context, this.GameConfiguration)
                .WithItemReward(13, 0, this.Context, this.GameConfiguration)
                .WithExperienceReward(90000, this.Context);

            this.CreateQuest("Random Quest (3)", 19, 116, 320, 349, 257)
                .WithMonsterKillRequirement(20, 351, this.Context, this.GameConfiguration)
                .WithMonsterKillRequirement(5, 355, this.Context, this.GameConfiguration)
                .WithItemReward(12, 15, this.Context, this.GameConfiguration)
                .WithItemReward(13, 1, this.Context, this.GameConfiguration);

            this.CreateQuest("Random Quest (3)", 19, 117, 320, 349, 257)
                .WithMonsterKillRequirement(20, 351, this.Context, this.GameConfiguration)
                .WithMonsterKillRequirement(5, 355, this.Context, this.GameConfiguration)
                .WithItemReward(14, 19, this.Context, this.GameConfiguration, 5)
                .WithItemReward(13, 18, this.Context, this.GameConfiguration, 7);

            this.CreateQuest("Random Quest (3)", 19, 118, 320, 349, 257)
                .WithMonsterKillRequirement(20, 351, this.Context, this.GameConfiguration)
                .WithMonsterKillRequirement(5, 355, this.Context, this.GameConfiguration)
                .WithItemReward(2, 6, this.Context, this.GameConfiguration, 6, 12, hasLuck: false, hasSkill: true);

            this.CreateQuest("Random Quest (3)", 19, 119, 320, 349, 257)
                .WithMonsterKillRequirement(30, 352, this.Context, this.GameConfiguration)
                .WithItemReward(2, 6, this.Context, this.GameConfiguration, 5, 12, hasLuck: false, hasSkill: true);

            this.CreateQuest("Random Quest (3)", 19, 120, 320, 349, 257)
                .WithMonsterKillRequirement(30, 352, this.Context, this.GameConfiguration)
                .WithItemReward(0, 20, this.Context, this.GameConfiguration, 0, 12, hasLuck: false, hasSkill: true);

            this.CreateQuest("Random Quest (3)", 19, 121, 320, 349, 257)
                .WithMonsterKillRequirement(30, 352, this.Context, this.GameConfiguration)
                .WithItemReward(4, 22, this.Context, this.GameConfiguration, 0, 12, hasLuck: false, hasSkill: true);

            this.CreateQuest("Random Quest (3)", 19, 122, 320, 349, 257)
                .WithMonsterKillRequirement(30, 352, this.Context, this.GameConfiguration)
                .WithItemReward(14, 14, this.Context, this.GameConfiguration)
                .WithItemReward(14, 16, this.Context, this.GameConfiguration);

            this.CreateQuest("Random Quest (3)", 19, 123, 320, 349, 257)
                .WithMonsterKillRequirement(30, 352, this.Context, this.GameConfiguration)
                .WithItemReward(5, 18, this.Context, this.GameConfiguration, 0, 12)
                .WithItemReward(13, 18, this.Context, this.GameConfiguration, 6);

            this.CreateQuest("Random Quest (3)", 19, 124, 320, 349, 257)
                .WithMonsterKillRequirement(30, 352, this.Context, this.GameConfiguration)
                .WithItemReward(14, 19, this.Context, this.GameConfiguration, 6)
                .WithItemReward(14, 42, this.Context, this.GameConfiguration);

            this.CreateQuest("Random Quest (3)", 19, 125, 320, 349, 257)
                .WithMonsterKillRequirement(30, 352, this.Context, this.GameConfiguration)
                .WithItemReward(14, 13, this.Context, this.GameConfiguration);

            this.CreateQuest("Random Quest (3)", 19, 126, 320, 349, 257)
                .WithMonsterKillRequirement(30, 352, this.Context, this.GameConfiguration)
                .WithItemReward(14, 13, this.Context, this.GameConfiguration);

            this.CreateQuest("Random Quest (3)", 19, 127, 320, 349, 257)
                .WithMonsterKillRequirement(20, 351, this.Context, this.GameConfiguration)
                .WithItemRequirement(5, 17, this.Context, this.GameConfiguration, itemOption: 12)
                .WithItemReward(5, 18, this.Context, this.GameConfiguration, 0, 12);

            this.CreateQuest("Random Quest (3)", 19, 128, 320, 349, 257)
                .WithMonsterKillRequirement(20, 351, this.Context, this.GameConfiguration)
                .WithItemRequirement(5, 17, this.Context, this.GameConfiguration, itemOption: 12)
                .WithExperienceReward(160000, this.Context)
                .WithItemReward(14, 42, this.Context, this.GameConfiguration);

            this.CreateQuest("Random Quest (3)", 19, 129, 320, 349, 257)
                .WithMonsterKillRequirement(20, 351, this.Context, this.GameConfiguration)
                .WithItemRequirement(5, 17, this.Context, this.GameConfiguration, itemOption: 12)
                .WithExperienceReward(90000, this.Context)
                .WithItemReward(14, 13, this.Context, this.GameConfiguration);

            this.CreateQuest("Random Quest (3)", 19, 130, 320, 349, 257)
                .WithMonsterKillRequirement(20, 351, this.Context, this.GameConfiguration)
                .WithItemRequirement(5, 17, this.Context, this.GameConfiguration, itemOption: 12)
                .WithItemReward(12, 15, this.Context, this.GameConfiguration)
                .WithItemReward(2, 6, this.Context, this.GameConfiguration, 5, 12, hasLuck: false, hasSkill: true);

            this.CreateQuest("Random Quest (3)", 19, 131, 320, 349, 257)
                .WithMonsterKillRequirement(20, 351, this.Context, this.GameConfiguration)
                .WithItemRequirement(5, 17, this.Context, this.GameConfiguration, itemOption: 12)
                .WithItemReward(0, 20, this.Context, this.GameConfiguration, 0, 12, hasLuck: false, hasSkill: true);

            this.CreateQuest("Random Quest (3)", 19, 132, 320, 349, 257)
                .WithMonsterKillRequirement(20, 351, this.Context, this.GameConfiguration)
                .WithItemRequirement(5, 17, this.Context, this.GameConfiguration, itemOption: 12)
                .WithItemReward(13, 18, this.Context, this.GameConfiguration, 7)
                .WithItemReward(14, 16, this.Context, this.GameConfiguration);

            this.CreateQuest("Random Quest (3)", 19, 133, 320, 349, 257)
                .WithMonsterKillRequirement(20, 351, this.Context, this.GameConfiguration)
                .WithItemRequirement(5, 17, this.Context, this.GameConfiguration, itemOption: 12)
                .WithItemReward(5, 11, this.Context, this.GameConfiguration, 0, 12);

            this.CreateQuest("Random Quest (3)", 19, 134, 320, 349, 257)
                .WithMonsterKillRequirement(20, 351, this.Context, this.GameConfiguration)
                .WithItemRequirement(5, 17, this.Context, this.GameConfiguration, itemOption: 12)
                .WithItemReward(14, 19, this.Context, this.GameConfiguration, 6)
                .WithItemReward(14, 41, this.Context, this.GameConfiguration);
        }
    }
}
