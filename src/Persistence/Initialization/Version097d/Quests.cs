// <copyright file="Quests.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.Persistence.Initialization.Version097d;

using MUnique.OpenMU.DataModel.Configuration;
using MUnique.OpenMU.DataModel.Configuration.Quests;
using MUnique.OpenMU.GameServer.MessageHandler.Quests;
using MUnique.OpenMU.Persistence.Initialization.CharacterClasses;

/// <summary>
/// Minimal legacy quest initialization for 0.97d (class evolution).
/// </summary>
internal class Quests : InitializerBase
{
    private const byte ScrollOfEmperorNumber = 23;
    private const byte BrokenSwordNumber = 24;
    private const byte TearOfElfNumber = 25;
    private const byte SoulShardOfWizardNumber = 26;

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
        // Level 150 legacy quests (DW/DK/FE only in 0.97).
        this.FindScrollOfEmperor(CharacterClassNumber.DarkKnight);
        this.FindScrollOfEmperor(CharacterClassNumber.FairyElf);
        this.FindScrollOfEmperor(CharacterClassNumber.DarkWizard);

        this.TreasuresOfMu(CharacterClassNumber.DarkKnight, BrokenSwordNumber);
        this.TreasuresOfMu(CharacterClassNumber.FairyElf, TearOfElfNumber);
        this.TreasuresOfMu(CharacterClassNumber.DarkWizard, SoulShardOfWizardNumber);
    }

    private CharacterClass? GetCharacterClass(CharacterClassNumber characterClass)
    {
        return this.GameConfiguration.CharacterClasses.FirstOrDefault(c => c.Number == (byte)characterClass);
    }

    private void TreasuresOfMu(CharacterClassNumber characterClass, byte itemNumber)
    {
        var sebinaThePriestess = this.GameConfiguration.Monsters.First(m => m.Number == 235);
        var treasuresOfMu = this.Context.CreateNew<QuestDefinition>();
        treasuresOfMu.SetGuid(QuestConstants.LegacyQuestGroup, 1, (byte)characterClass);
        sebinaThePriestess.Quests.Add(treasuresOfMu);
        treasuresOfMu.QuestGiver = sebinaThePriestess;
        treasuresOfMu.Group = QuestConstants.LegacyQuestGroup;
        treasuresOfMu.Number = 1;
        treasuresOfMu.RequiredStartMoney = 2000000;
        treasuresOfMu.MinimumCharacterLevel = 150;
        treasuresOfMu.QualifiedCharacter = this.GetCharacterClass(characterClass);
        treasuresOfMu.Name = $"Treasures of MU ({treasuresOfMu.QualifiedCharacter?.Name})";

        this.AddItemRequirement(treasuresOfMu, 14, itemNumber, 0, 62, 76);

        var pointReward = this.Context.CreateNew<QuestReward>();
        pointReward.Value = 10;
        pointReward.RewardType = QuestRewardType.LevelUpPoints;
        treasuresOfMu.Rewards.Add(pointReward);

        var characterClassEvolution = this.Context.CreateNew<QuestReward>();
        characterClassEvolution.RewardType = QuestRewardType.CharacterEvolutionFirstToSecond;
        treasuresOfMu.Rewards.Add(characterClassEvolution);
    }

    private void FindScrollOfEmperor(CharacterClassNumber characterClass)
    {
        var sebinaThePriestess = this.GameConfiguration.Monsters.First(m => m.Number == 235);
        var findScrollOfEmperor = this.Context.CreateNew<QuestDefinition>();
        findScrollOfEmperor.SetGuid(QuestConstants.LegacyQuestGroup, 0, (byte)characterClass);
        sebinaThePriestess.Quests.Add(findScrollOfEmperor);
        findScrollOfEmperor.QuestGiver = sebinaThePriestess;
        findScrollOfEmperor.Group = QuestConstants.LegacyQuestGroup;
        findScrollOfEmperor.Number = 0;
        findScrollOfEmperor.RequiredStartMoney = 1000000;
        findScrollOfEmperor.MinimumCharacterLevel = 150;
        findScrollOfEmperor.QualifiedCharacter = this.GetCharacterClass(characterClass);
        findScrollOfEmperor.Name = $"Find the 'Scroll of Emperor' ({findScrollOfEmperor.QualifiedCharacter?.Name})";

        this.AddItemRequirement(findScrollOfEmperor, 14, ScrollOfEmperorNumber, 0, 45, 60);

        var pointReward = this.Context.CreateNew<QuestReward>();
        pointReward.Value = 10;
        pointReward.RewardType = QuestRewardType.LevelUpPoints;
        findScrollOfEmperor.Rewards.Add(pointReward);
    }

    private void AddItemRequirement(QuestDefinition quest, byte itemGroup, short itemNumber, byte itemLevel, byte minimumMonsterLevel, byte maximumMonsterLevel)
    {
        var itemRequirement = this.AddItemRequirement(quest, itemGroup, itemNumber, itemLevel);
        var dropItemGroup = itemRequirement.DropItemGroup!;
        dropItemGroup.MinimumMonsterLevel = minimumMonsterLevel;
        dropItemGroup.MaximumMonsterLevel = maximumMonsterLevel;
    }

    private QuestItemRequirement AddItemRequirement(QuestDefinition quest, byte itemGroup, short itemNumber, byte itemLevel)
    {
        var itemRequirement = this.Context.CreateNew<QuestItemRequirement>();
        itemRequirement.MinimumNumber = 1;
        itemRequirement.Item = this.GameConfiguration.Items.First(item => item.Group == itemGroup && item.Number == itemNumber);

        var dropItemGroup = this.Context.CreateNew<DropItemGroup>();
        dropItemGroup.SetGuid(quest.Number, itemNumber, quest.QualifiedCharacter?.Number ?? 0);
        dropItemGroup.Description = $"Quest Item '{itemRequirement.Item.Name}' ({quest.QualifiedCharacter?.Name})";
        dropItemGroup.PossibleItems.Add(itemRequirement.Item);
        dropItemGroup.Chance = 10.0 / 10000.0;
        dropItemGroup.ItemLevel = itemLevel;

        itemRequirement.DropItemGroup = dropItemGroup;
        this.GameConfiguration.DropItemGroups.Add(dropItemGroup);

        quest.RequiredItems.Add(itemRequirement);
        return itemRequirement;
    }
}
