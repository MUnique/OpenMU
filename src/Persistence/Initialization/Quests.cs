// <copyright file="Quests.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.Persistence.Initialization
{
    using System.Linq;
    using MUnique.OpenMU.DataModel.Configuration;
    using MUnique.OpenMU.DataModel.Configuration.Quests;
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
            this.FindScrollOfEmperor(CharacterClassNumber.DarkKnight);
            this.FindScrollOfEmperor(CharacterClassNumber.FairyElf);
            this.FindScrollOfEmperor(CharacterClassNumber.DarkWizard);
            this.FindScrollOfEmperor(CharacterClassNumber.DarkKnight);
            this.TreasuresOfMu(CharacterClassNumber.DarkKnight, Items.Quest.BrokenSwordNumber);
            this.TreasuresOfMu(CharacterClassNumber.FairyElf, Items.Quest.TearOfElfNumber);
            this.TreasuresOfMu(CharacterClassNumber.DarkWizard, Items.Quest.SoulShardOfWizardNumber);
            this.TreasuresOfMu(CharacterClassNumber.Summoner, Items.Quest.EyeOfAbyssalNumber);
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
            this.AddItemRequirement(treasuresOfMu, 14, itemNumber, 62, 76);

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

            this.AddItemRequirement(findScrollOfEmperor, 14, Items.Quest.ScrollOfEmperorNumber, 45, 60);

            // Rewards:
            var pointReward = this.Context.CreateNew<QuestReward>();
            pointReward.Value = 10;
            pointReward.RewardType = QuestRewardType.LevelUpPoints;
            findScrollOfEmperor.Rewards.Add(pointReward);
        }

        private void AddItemRequirement(QuestDefinition quest, byte itemGroup, short itemNumber, byte minimumMonsterLevel, byte maximumMonsterLevel)
        {
            var itemRequirement = this.Context.CreateNew<QuestItemRequirement>();
            itemRequirement.MinimumNumber = 1;
            itemRequirement.Item = this.GameConfiguration.Items.First(item => item.Group == itemGroup && item.Number == itemNumber);

            var dropItemGroup = this.Context.CreateNew<DropItemGroup>();
            dropItemGroup.Description = $"Quest Item '{itemRequirement.Item.Name}'";
            dropItemGroup.PossibleItems.Add(itemRequirement.Item);
            dropItemGroup.Chance = 10.0 / 10000.0;
            dropItemGroup.MinimumMonsterLevel = minimumMonsterLevel;
            dropItemGroup.MaximumMonsterLevel = maximumMonsterLevel;

            itemRequirement.DropItemGroup = dropItemGroup;
            this.GameConfiguration.DropItemGroups.Add(dropItemGroup);

            quest.RequiredItems.Add(itemRequirement);
        }
    }
}
