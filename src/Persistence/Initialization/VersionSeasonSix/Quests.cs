// <copyright file="Quests.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.Persistence.Initialization.VersionSeasonSix;

using MUnique.OpenMU.DataModel.Configuration;
using MUnique.OpenMU.DataModel.Configuration.Quests;
using MUnique.OpenMU.GameLogic.Attributes;
using MUnique.OpenMU.GameServer.MessageHandler.Quests;
using MUnique.OpenMU.Persistence.Initialization.CharacterClasses;
using MUnique.OpenMU.Persistence.Initialization.Skills;
using MUnique.OpenMU.Persistence.Initialization.VersionSeasonSix.Items;

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
        this.FindScrollOfEmperor(CharacterClassNumber.Summoner);
        this.TreasuresOfMu(CharacterClassNumber.DarkKnight, Quest.BrokenSwordNumber);
        this.TreasuresOfMu(CharacterClassNumber.FairyElf, Quest.TearOfElfNumber);
        this.TreasuresOfMu(CharacterClassNumber.DarkWizard, Quest.SoulShardOfWizardNumber);
        this.TreasuresOfMu(CharacterClassNumber.Summoner, Quest.EyeOfAbyssalNumber);

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

    private QuestDefinition CreateQuest(string name, short @group, short startingNumber, short number,
        short refuseNumber, int minimumLevel, int maximumLevel, short npcNumber,
        CharacterClassNumber? qualifiedCharacter = null)
    {
        var npc = this.GameConfiguration.Monsters.First(m => m.Number == npcNumber);
        var questDefinition = this.Context.CreateNew<QuestDefinition>();
        questDefinition.SetGuid(group, number, (byte?)qualifiedCharacter ?? 0);
        npc.Quests.Add(questDefinition);
        questDefinition.QuestGiver = npc;
        questDefinition.Name = name;
        questDefinition.Group = group;
        questDefinition.Number = number;
        questDefinition.StartingNumber = startingNumber;
        questDefinition.RefuseNumber = refuseNumber;
        questDefinition.MinimumCharacterLevel = minimumLevel;
        questDefinition.MaximumCharacterLevel = maximumLevel;
        questDefinition.Repeatable = true;
        if (qualifiedCharacter.HasValue)
        {
            questDefinition.QualifiedCharacter =
                this.GameConfiguration.CharacterClasses.FirstOrDefault(c =>
                    c.Number == (byte)qualifiedCharacter.Value);
            questDefinition.Name = name + $" ({questDefinition.QualifiedCharacter?.Name})";
        }

        return questDefinition;
    }

    // See also http://muonlinefanz.com/guide/quests/master/#ibb
    private void InfiltrationOfBarracksOfBallgass()
    {
        var apostleDevin = this.GameConfiguration.Monsters.First(m => m.Number == 406);
        var infiltrationOfBarracks = this.Context.CreateNew<QuestDefinition>();
        infiltrationOfBarracks.SetGuid(QuestConstants.LegacyQuestGroup, 5);
        apostleDevin.Quests.Add(infiltrationOfBarracks);
        infiltrationOfBarracks.QuestGiver = apostleDevin;
        infiltrationOfBarracks.Name = "Infiltrate The Barracks of Balgass";
        infiltrationOfBarracks.Group = QuestConstants.LegacyQuestGroup;
        infiltrationOfBarracks.Number = 5;
        infiltrationOfBarracks.RequiredStartMoney = 7000000;
        infiltrationOfBarracks.MinimumCharacterLevel = 400;

        var balram = this.Context.CreateNew<QuestMonsterKillRequirement>();
        balram.SetGuid(infiltrationOfBarracks.Number, 409);
        balram.MinimumNumber = 20;
        balram.Monster = this.GameConfiguration.Monsters.First(m => m.Number == 409);
        infiltrationOfBarracks.RequiredMonsterKills.Add(balram);

        var deathSpirit = this.Context.CreateNew<QuestMonsterKillRequirement>();
        deathSpirit.SetGuid(infiltrationOfBarracks.Number, 410);
        deathSpirit.MinimumNumber = 20;
        deathSpirit.Monster = this.GameConfiguration.Monsters.First(m => m.Number == 410);
        infiltrationOfBarracks.RequiredMonsterKills.Add(deathSpirit);

        var soram = this.Context.CreateNew<QuestMonsterKillRequirement>();
        soram.SetGuid(infiltrationOfBarracks.Number, 411);
        soram.MinimumNumber = 20;
        soram.Monster = this.GameConfiguration.Monsters.First(m => m.Number == 411);
        infiltrationOfBarracks.RequiredMonsterKills.Add(soram);

        // Rewards:
        var pointReward = this.Context.CreateNew<QuestReward>();
        pointReward.SetGuid(infiltrationOfBarracks.Number, (short)QuestRewardType.LevelUpPoints);
        pointReward.Value = 10;
        pointReward.RewardType = QuestRewardType.LevelUpPoints;
        infiltrationOfBarracks.Rewards.Add(pointReward);
    }

    // See also http://muonlinefanz.com/guide/quests/master/#itrod
    private void IntoTheDarknessZone()
    {
        var apostleDevin = this.GameConfiguration.Monsters.First(m => m.Number == 406);
        var intoTheDarkness = this.Context.CreateNew<QuestDefinition>();
        intoTheDarkness.SetGuid(QuestConstants.LegacyQuestGroup, 6);
        apostleDevin.Quests.Add(intoTheDarkness);
        intoTheDarkness.QuestGiver = apostleDevin;
        intoTheDarkness.Name = "Into the 'Darkness' Zone";
        intoTheDarkness.Group = QuestConstants.LegacyQuestGroup;
        intoTheDarkness.Number = 6;
        intoTheDarkness.RequiredStartMoney = 10000000;
        intoTheDarkness.MinimumCharacterLevel = 400;

        var darkElf = this.Context.CreateNew<QuestMonsterKillRequirement>();
        darkElf.SetGuid(intoTheDarkness.Number, 412);
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
        evidenceOfStrength.SetGuid(QuestConstants.LegacyQuestGroup, 4);
        apostleDevin.Quests.Add(evidenceOfStrength);
        evidenceOfStrength.QuestGiver = apostleDevin;
        evidenceOfStrength.Name = "Evidence of Strength";
        evidenceOfStrength.Group = QuestConstants.LegacyQuestGroup;
        evidenceOfStrength.Number = 4;
        evidenceOfStrength.RequiredStartMoney = 5000000;
        evidenceOfStrength.MinimumCharacterLevel = 380;

        this.AddItemRequirement(evidenceOfStrength, 14, Quest.FlameOfDeathBeamKnightNumber, 0, 63, 3.0 / 100.0);
        this.AddItemRequirement(evidenceOfStrength, 14, Quest.HornOfHellMaineNumber, 0, 309, 4.0 / 100.0);
        this.AddItemRequirement(evidenceOfStrength, 14, Quest.FeatherOfDarkPhoenixNumber, 0, 77, 5.0 / 100.0);

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
        secretDarkStone.SetGuid(QuestConstants.LegacyQuestGroup, 3, (byte)CharacterClassNumber.BladeKnight);
        marlon.Quests.Add(secretDarkStone);
        secretDarkStone.QuestGiver = marlon;
        secretDarkStone.Name = "Secret of the 'Dark Stone'";
        secretDarkStone.Group = QuestConstants.LegacyQuestGroup;
        secretDarkStone.Number = 3;
        secretDarkStone.RequiredStartMoney = 2000000;
        secretDarkStone.MinimumCharacterLevel = 220;
        secretDarkStone.QualifiedCharacter = this.GetCharacterClass(CharacterClassNumber.BladeKnight);
        this.AddItemRequirement(secretDarkStone, 14, Quest.BrokenSwordNumber, 1, 78, 108);

        // Rewards:
        // Combo Skill
        var comboSkill = this.Context.CreateNew<QuestReward>();
        comboSkill.Value = 1;
        comboSkill.AttributeReward = Stats.IsSkillComboAvailable.GetPersistent(this.GameConfiguration);
        comboSkill.RewardType = QuestRewardType.Attribute;

        secretDarkStone.Rewards.Add(comboSkill);
    }

    // See also http://muonlinefanz.com/guide/quests/hero-status/
    private void GainHeroStatus(CharacterClassNumber characterClass)
    {
        var marlon = this.GameConfiguration.Monsters.First(m => m.Number == 229);
        var heroStatus = this.Context.CreateNew<QuestDefinition>();
        heroStatus.SetGuid(QuestConstants.LegacyQuestGroup, 2, (byte)characterClass);
        marlon.Quests.Add(heroStatus);
        heroStatus.QuestGiver = marlon;
        heroStatus.Group = QuestConstants.LegacyQuestGroup;
        heroStatus.Number = 2;
        heroStatus.RequiredStartMoney = 3000000;
        heroStatus.MinimumCharacterLevel = 220;
        heroStatus.QualifiedCharacter = this.GetCharacterClass(characterClass);
        heroStatus.Name = $"Gain Hero Status ({heroStatus.QualifiedCharacter?.Name})";
        this.AddItemRequirement(heroStatus, 14, Quest.ScrollOfEmperorNumber, 1, 72, 108);

        // Rewards:
        // One character level point more, after level 220
        var pointReward = this.Context.CreateNew<QuestReward>();
        pointReward.Value = 1;
        pointReward.AttributeReward = Stats.PointsPerLevelUp.GetPersistent(this.GameConfiguration);
        pointReward.RewardType = QuestRewardType.Attribute;

        var attributeReward = this.Context.CreateNew<QuestReward>();
        attributeReward.Value = 1;
        attributeReward.AttributeReward = Stats.GainHeroStatusQuestCompleted.GetPersistent(this.GameConfiguration);
        attributeReward.RewardType = QuestRewardType.Attribute;

        if (characterClass == CharacterClassNumber.MuseElf)
        {
            var skillReward = this.Context.CreateNew<QuestReward>();
            skillReward.Value = 1;
            skillReward.SkillReward = this.GameConfiguration.Skills.First(s => s.Number == (short)SkillNumber.InfinityArrow);
            skillReward.RewardType = QuestRewardType.Skill;
            heroStatus.Rewards.Add(skillReward);
        }

        heroStatus.Rewards.Add(pointReward);
        heroStatus.Rewards.Add(attributeReward);
    }

    // See also http://muonlinefanz.com/guide/quests/treasure/
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
        findScrollOfEmperor.SetGuid(QuestConstants.LegacyQuestGroup, 0, (byte)characterClass);
        sebinaThePriestess.Quests.Add(findScrollOfEmperor);
        findScrollOfEmperor.QuestGiver = sebinaThePriestess;
        findScrollOfEmperor.Group = QuestConstants.LegacyQuestGroup;
        findScrollOfEmperor.Number = 0;
        findScrollOfEmperor.RequiredStartMoney = 1000000;
        findScrollOfEmperor.MinimumCharacterLevel = 150;
        findScrollOfEmperor.QualifiedCharacter = this.GetCharacterClass(characterClass);
        findScrollOfEmperor.Name = $"Find the 'Scroll of Emperor' ({findScrollOfEmperor.QualifiedCharacter?.Name})";

        this.AddItemRequirement(findScrollOfEmperor, 14, Quest.ScrollOfEmperorNumber, 0, 45, 60);

        // Rewards:
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

    private void AddItemRequirement(QuestDefinition quest, byte itemGroup, short itemNumber, byte itemLevel, short monsterNumber, double dropRate)
    {
        var itemRequirement = this.AddItemRequirement(quest, itemGroup, itemNumber, itemLevel);
        var dropItemGroup = itemRequirement.DropItemGroup!;
        dropItemGroup.Monster = this.GameConfiguration.Monsters.First(m => m.Number == monsterNumber);
        dropItemGroup.Chance = dropRate;
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

    private void CreateNewQuests()
    {
        this.CreateQuest("Spider Hunt!", 18, 0, 1, 2, 1, 14, 257)
            .WithMonsterKillRequirement(10, 3, this.Context, this.GameConfiguration)
            .WithExperienceReward(2000, this.Context);

        this.CreateQuest("Dispose of the Town Plunderers!", 18, 3, 4, 8, 15, 25, 257, CharacterClassNumber.DarkWizard)
            .WithMonsterKillRequirement(15, 7, this.Context, this.GameConfiguration)
            .WithExperienceReward(6000, this.Context);
        this.CreateQuest("Dispose of the Town Plunderers!", 18, 3, 4, 8, 15, 25, 257, CharacterClassNumber.DarkKnight)
            .WithMonsterKillRequirement(15, 7, this.Context, this.GameConfiguration)
            .WithExperienceReward(6000, this.Context);

        this.CreateQuest("Dispose of the Town Plunderers!", 18, 3, 5, 8, 15, 25, 257, CharacterClassNumber.MagicGladiator)
            .WithMonsterKillRequirement(15, 6, this.Context, this.GameConfiguration)
            .WithExperienceReward(6000, this.Context);
        this.CreateQuest("Dispose of the Town Plunderers!", 18, 3, 5, 8, 15, 25, 257, CharacterClassNumber.DarkLord)
            .WithMonsterKillRequirement(15, 6, this.Context, this.GameConfiguration)
            .WithExperienceReward(6000, this.Context);
        this.CreateQuest("Dispose of the Town Plunderers!", 18, 3, 5, 8, 15, 25, 257, CharacterClassNumber.RageFighter)
            .WithMonsterKillRequirement(15, 6, this.Context, this.GameConfiguration)
            .WithExperienceReward(6000, this.Context);

        this.CreateQuest("Dispose of the Town Plunderers!", 18, 3, 6, 8, 15, 25, 257, CharacterClassNumber.FairyElf)
            .WithMonsterKillRequirement(15, 31, this.Context, this.GameConfiguration)
            .WithExperienceReward(6000, this.Context);

        this.CreateQuest("Dispose of the Town Plunderers!", 18, 3, 7, 8, 15, 25, 257, CharacterClassNumber.Summoner)
            .WithMonsterKillRequirement(15, 419, this.Context, this.GameConfiguration)
            .WithExperienceReward(6000, this.Context);

        this.CreateQuest("Boss of predator treatment!", 18, 9, 10, 14, 26, 35, 257, CharacterClassNumber.MagicGladiator)
            .WithMonsterKillRequirement(20, 14, this.Context, this.GameConfiguration)
            .WithExperienceReward(10000, this.Context);
        this.CreateQuest("Boss of predator treatment!", 18, 9, 10, 14, 26, 35, 257, CharacterClassNumber.DarkLord)
            .WithMonsterKillRequirement(20, 14, this.Context, this.GameConfiguration)
            .WithExperienceReward(10000, this.Context);
        this.CreateQuest("Boss of predator treatment!", 18, 9, 10, 14, 26, 35, 257, CharacterClassNumber.RageFighter)
            .WithMonsterKillRequirement(20, 14, this.Context, this.GameConfiguration)
            .WithExperienceReward(10000, this.Context);

        this.CreateQuest("Boss of predator treatment!", 18, 9, 11, 14, 26, 35, 257, CharacterClassNumber.DarkWizard)
            .WithMonsterKillRequirement(20, 7, this.Context, this.GameConfiguration)
            .WithExperienceReward(10000, this.Context);
        this.CreateQuest("Boss of predator treatment!", 18, 9, 11, 14, 26, 35, 257, CharacterClassNumber.DarkKnight)
            .WithMonsterKillRequirement(20, 7, this.Context, this.GameConfiguration)
            .WithExperienceReward(10000, this.Context);

        this.CreateQuest("Boss of predator treatment!", 18, 9, 12, 14, 26, 35, 257, CharacterClassNumber.FairyElf)
            .WithMonsterKillRequirement(20, 32, this.Context, this.GameConfiguration)
            .WithExperienceReward(10000, this.Context);

        this.CreateQuest("Boss of predator treatment!", 18, 9, 13, 14, 26, 35, 257, CharacterClassNumber.Summoner)
            .WithMonsterKillRequirement(20, 419, this.Context, this.GameConfiguration)
            .WithExperienceReward(10000, this.Context);

        this.CreateQuest("Monster Supply Route in Devias!", 18, 15, 16, 18, 36, 45, 257, CharacterClassNumber.MagicGladiator)
            .WithMonsterKillRequirement(25, 22, this.Context, this.GameConfiguration)
            .WithExperienceReward(14000, this.Context);
        this.CreateQuest("Monster Supply Route in Devias!", 18, 15, 16, 18, 36, 45, 257, CharacterClassNumber.DarkLord)
            .WithMonsterKillRequirement(25, 22, this.Context, this.GameConfiguration)
            .WithExperienceReward(14000, this.Context);
        this.CreateQuest("Monster Supply Route in Devias!", 18, 15, 16, 18, 36, 45, 257, CharacterClassNumber.RageFighter)
            .WithMonsterKillRequirement(25, 22, this.Context, this.GameConfiguration)
            .WithExperienceReward(14000, this.Context);

        this.CreateQuest("Monster Supply Route in Devias!", 18, 15, 17, 18, 36, 45, 257, CharacterClassNumber.DarkWizard)
            .WithMonsterKillRequirement(25, 24, this.Context, this.GameConfiguration)
            .WithExperienceReward(14000, this.Context);
        this.CreateQuest("Monster Supply Route in Devias!", 18, 15, 17, 18, 36, 45, 257, CharacterClassNumber.DarkKnight)
            .WithMonsterKillRequirement(25, 24, this.Context, this.GameConfiguration)
            .WithExperienceReward(14000, this.Context);
        this.CreateQuest("Monster Supply Route in Devias!", 18, 15, 17, 18, 36, 45, 257, CharacterClassNumber.FairyElf)
            .WithMonsterKillRequirement(25, 24, this.Context, this.GameConfiguration)
            .WithExperienceReward(14000, this.Context);
        this.CreateQuest("Monster Supply Route in Devias!", 18, 15, 17, 18, 36, 45, 257, CharacterClassNumber.Summoner)
            .WithMonsterKillRequirement(25, 24, this.Context, this.GameConfiguration)
            .WithExperienceReward(14000, this.Context);

        this.CreateQuest("Attack on the Supply Corps!", 18, 19, 20, 22, 46, 55, 257, CharacterClassNumber.MagicGladiator)
            .WithMonsterKillRequirement(30, 21, this.Context, this.GameConfiguration)
            .WithExperienceReward(21000, this.Context);
        this.CreateQuest("Attack on the Supply Corps!", 18, 19, 20, 22, 46, 55, 257, CharacterClassNumber.DarkLord)
            .WithMonsterKillRequirement(30, 21, this.Context, this.GameConfiguration)
            .WithExperienceReward(21000, this.Context);
        this.CreateQuest("Attack on the Supply Corps!", 18, 19, 20, 22, 46, 55, 257, CharacterClassNumber.RageFighter)
            .WithMonsterKillRequirement(30, 21, this.Context, this.GameConfiguration)
            .WithExperienceReward(21000, this.Context);

        this.CreateQuest("Attack on the Supply Corps!", 18, 19, 21, 22, 46, 55, 257, CharacterClassNumber.DarkWizard)
            .WithMonsterKillRequirement(30, 22, this.Context, this.GameConfiguration)
            .WithExperienceReward(21000, this.Context);
        this.CreateQuest("Attack on the Supply Corps!", 18, 19, 21, 22, 46, 55, 257, CharacterClassNumber.DarkKnight)
            .WithMonsterKillRequirement(30, 22, this.Context, this.GameConfiguration)
            .WithExperienceReward(21000, this.Context);
        this.CreateQuest("Attack on the Supply Corps!", 18, 19, 21, 22, 46, 55, 257, CharacterClassNumber.FairyElf)
            .WithMonsterKillRequirement(30, 22, this.Context, this.GameConfiguration)
            .WithExperienceReward(21000, this.Context);
        this.CreateQuest("Attack on the Supply Corps!", 18, 19, 21, 22, 46, 55, 257, CharacterClassNumber.Summoner)
            .WithMonsterKillRequirement(30, 22, this.Context, this.GameConfiguration)
            .WithExperienceReward(21000, this.Context);

        this.CreateQuest("Inland Supply Route!", 18, 23, 24, 26, 56, 65, 257, CharacterClassNumber.MagicGladiator)
            .WithMonsterKillRequirement(30, 19, this.Context, this.GameConfiguration)
            .WithExperienceReward(28000, this.Context);
        this.CreateQuest("Inland Supply Route!", 18, 23, 24, 26, 56, 65, 257, CharacterClassNumber.DarkLord)
            .WithMonsterKillRequirement(30, 19, this.Context, this.GameConfiguration)
            .WithExperienceReward(28000, this.Context);
        this.CreateQuest("Inland Supply Route!", 18, 23, 24, 26, 56, 65, 257, CharacterClassNumber.RageFighter)
            .WithMonsterKillRequirement(30, 19, this.Context, this.GameConfiguration)
            .WithExperienceReward(28000, this.Context);

        this.CreateQuest("Inland Supply Route!", 18, 23, 25, 26, 56, 65, 257, CharacterClassNumber.DarkWizard)
            .WithMonsterKillRequirement(30, 23, this.Context, this.GameConfiguration)
            .WithExperienceReward(28000, this.Context);
        this.CreateQuest("Inland Supply Route!", 18, 23, 25, 26, 56, 65, 257, CharacterClassNumber.DarkKnight)
            .WithMonsterKillRequirement(30, 23, this.Context, this.GameConfiguration)
            .WithExperienceReward(28000, this.Context);
        this.CreateQuest("Inland Supply Route!", 18, 23, 25, 26, 56, 65, 257, CharacterClassNumber.FairyElf)
            .WithMonsterKillRequirement(30, 23, this.Context, this.GameConfiguration)
            .WithExperienceReward(28000, this.Context);
        this.CreateQuest("Inland Supply Route!", 18, 23, 25, 26, 56, 65, 257, CharacterClassNumber.Summoner)
            .WithMonsterKillRequirement(30, 23, this.Context, this.GameConfiguration)
            .WithExperienceReward(28000, this.Context);

        this.CreateQuest("Dungeon Sweep!", 18, 27, 28, 30, 66, 79, 257, CharacterClassNumber.MagicGladiator)
            .WithMonsterKillRequirement(40, 17, this.Context, this.GameConfiguration)
            .WithExperienceReward(36000, this.Context);
        this.CreateQuest("Dungeon Sweep!", 18, 27, 28, 30, 66, 79, 257, CharacterClassNumber.DarkLord)
            .WithMonsterKillRequirement(40, 17, this.Context, this.GameConfiguration)
            .WithExperienceReward(36000, this.Context);
        this.CreateQuest("Dungeon Sweep!", 18, 27, 28, 30, 66, 79, 257, CharacterClassNumber.RageFighter)
            .WithMonsterKillRequirement(40, 17, this.Context, this.GameConfiguration)
            .WithExperienceReward(36000, this.Context);

        this.CreateQuest("Dungeon Sweep!", 18, 27, 29, 30, 66, 79, 257, CharacterClassNumber.DarkWizard)
            .WithMonsterKillRequirement(40, 12, this.Context, this.GameConfiguration)
            .WithExperienceReward(36000, this.Context);
        this.CreateQuest("Dungeon Sweep!", 18, 27, 29, 30, 66, 79, 257, CharacterClassNumber.DarkKnight)
            .WithMonsterKillRequirement(40, 12, this.Context, this.GameConfiguration)
            .WithExperienceReward(36000, this.Context);
        this.CreateQuest("Dungeon Sweep!", 18, 27, 29, 30, 66, 79, 257, CharacterClassNumber.FairyElf)
            .WithMonsterKillRequirement(40, 12, this.Context, this.GameConfiguration)
            .WithExperienceReward(36000, this.Context);
        this.CreateQuest("Dungeon Sweep!", 18, 27, 29, 30, 66, 79, 257, CharacterClassNumber.Summoner)
            .WithMonsterKillRequirement(40, 12, this.Context, this.GameConfiguration)
            .WithExperienceReward(36000, this.Context);

        this.CreateQuest("and asked the elders of the Elbe!", 18, 31, 32, 34, 80, 90, 257, CharacterClassNumber.Summoner)
            .WithMonsterKillRequirement(50, 423, this.Context, this.GameConfiguration)
            .WithExperienceReward(38000, this.Context);

        this.CreateQuest("and asked the elders of the Elbe!", 18, 31, 33, 34, 80, 90, 257, CharacterClassNumber.DarkWizard)
            .WithMonsterKillRequirement(50, 422, this.Context, this.GameConfiguration)
            .WithExperienceReward(38000, this.Context);
        this.CreateQuest("and asked the elders of the Elbe!", 18, 31, 33, 34, 80, 90, 257, CharacterClassNumber.DarkKnight)
            .WithMonsterKillRequirement(50, 422, this.Context, this.GameConfiguration)
            .WithExperienceReward(38000, this.Context);
        this.CreateQuest("and asked the elders of the Elbe!", 18, 31, 33, 34, 80, 90, 257, CharacterClassNumber.FairyElf)
            .WithMonsterKillRequirement(50, 422, this.Context, this.GameConfiguration)
            .WithExperienceReward(38000, this.Context);
        this.CreateQuest("and asked the elders of the Elbe!", 18, 31, 33, 34, 80, 90, 257, CharacterClassNumber.MagicGladiator)
            .WithMonsterKillRequirement(50, 422, this.Context, this.GameConfiguration)
            .WithExperienceReward(38000, this.Context);
        this.CreateQuest("and asked the elders of the Elbe!", 18, 31, 33, 34, 80, 90, 257, CharacterClassNumber.DarkLord)
            .WithMonsterKillRequirement(50, 422, this.Context, this.GameConfiguration)
            .WithExperienceReward(38000, this.Context);
        this.CreateQuest("and asked the elders of the Elbe!", 18, 31, 33, 34, 80, 90, 257, CharacterClassNumber.RageFighter)
            .WithMonsterKillRequirement(50, 422, this.Context, this.GameConfiguration)
            .WithExperienceReward(38000, this.Context);

        this.CreateQuest("Situation at the Dungeon (1)", 18, 35, 36, 38, 91, 100, 257, CharacterClassNumber.DarkWizard)
            .WithMonsterKillRequirement(50, 5, this.Context, this.GameConfiguration)
            .WithExperienceReward(43000, this.Context);
        this.CreateQuest("Situation at the Dungeon (1)", 18, 35, 36, 38, 91, 100, 257, CharacterClassNumber.DarkKnight)
            .WithMonsterKillRequirement(50, 5, this.Context, this.GameConfiguration)
            .WithExperienceReward(43000, this.Context);
        this.CreateQuest("Situation at the Dungeon (1)", 18, 35, 36, 38, 91, 100, 257, CharacterClassNumber.FairyElf)
            .WithMonsterKillRequirement(50, 5, this.Context, this.GameConfiguration)
            .WithExperienceReward(43000, this.Context);
        this.CreateQuest("Situation at the Dungeon (1)", 18, 35, 36, 38, 91, 100, 257, CharacterClassNumber.Summoner)
            .WithMonsterKillRequirement(50, 5, this.Context, this.GameConfiguration)
            .WithExperienceReward(43000, this.Context);

        this.CreateQuest("Situation at the Dungeon (1)", 18, 35, 37, 38, 91, 100, 257, CharacterClassNumber.MagicGladiator)
            .WithMonsterKillRequirement(50, 9, this.Context, this.GameConfiguration)
            .WithExperienceReward(43000, this.Context);
        this.CreateQuest("Situation at the Dungeon (1)", 18, 35, 37, 38, 91, 100, 257, CharacterClassNumber.DarkLord)
            .WithMonsterKillRequirement(50, 9, this.Context, this.GameConfiguration)
            .WithExperienceReward(43000, this.Context);
        this.CreateQuest("Situation at the Dungeon (1)", 18, 35, 37, 38, 91, 100, 257, CharacterClassNumber.RageFighter)
            .WithMonsterKillRequirement(50, 9, this.Context, this.GameConfiguration)
            .WithExperienceReward(43000, this.Context);

        this.CreateQuest("Situation at the Dungeon (2)", 18, 39, 40, 42, 101, 110, 257, CharacterClassNumber.MagicGladiator)
            .WithMonsterKillRequirement(50, 13, this.Context, this.GameConfiguration)
            .WithExperienceReward(50000, this.Context);
        this.CreateQuest("Situation at the Dungeon (2)", 18, 39, 40, 42, 101, 110, 257, CharacterClassNumber.DarkLord)
            .WithMonsterKillRequirement(50, 13, this.Context, this.GameConfiguration)
            .WithExperienceReward(50000, this.Context);
        this.CreateQuest("Situation at the Dungeon (2)", 18, 39, 40, 42, 101, 110, 257, CharacterClassNumber.RageFighter)
            .WithMonsterKillRequirement(50, 13, this.Context, this.GameConfiguration)
            .WithExperienceReward(50000, this.Context);

        this.CreateQuest("Situation at the Dungeon (2)", 18, 39, 41, 42, 101, 110, 257, CharacterClassNumber.DarkWizard)
            .WithMonsterKillRequirement(50, 15, this.Context, this.GameConfiguration)
            .WithExperienceReward(50000, this.Context);
        this.CreateQuest("Situation at the Dungeon (2)", 18, 39, 41, 42, 101, 110, 257, CharacterClassNumber.DarkKnight)
            .WithMonsterKillRequirement(50, 15, this.Context, this.GameConfiguration)
            .WithExperienceReward(50000, this.Context);
        this.CreateQuest("Situation at the Dungeon (2)", 18, 39, 41, 42, 101, 110, 257, CharacterClassNumber.FairyElf)
            .WithMonsterKillRequirement(50, 15, this.Context, this.GameConfiguration)
            .WithExperienceReward(50000, this.Context);
        this.CreateQuest("Situation at the Dungeon (2)", 18, 39, 41, 42, 101, 110, 257, CharacterClassNumber.Summoner)
            .WithMonsterKillRequirement(50, 15, this.Context, this.GameConfiguration)
            .WithExperienceReward(50000, this.Context);

        this.CreateQuest("Situation at the Dungeon (3)", 18, 43, 44, 46, 111, 120, 257, CharacterClassNumber.MagicGladiator)
            .WithMonsterKillRequirement(50, 9, this.Context, this.GameConfiguration)
            .WithExperienceReward(54000, this.Context);
        this.CreateQuest("Situation at the Dungeon (3)", 18, 43, 44, 46, 111, 120, 257, CharacterClassNumber.DarkLord)
            .WithMonsterKillRequirement(50, 9, this.Context, this.GameConfiguration)
            .WithExperienceReward(54000, this.Context);
        this.CreateQuest("Situation at the Dungeon (3)", 18, 43, 44, 46, 111, 120, 257, CharacterClassNumber.RageFighter)
            .WithMonsterKillRequirement(50, 9, this.Context, this.GameConfiguration)
            .WithExperienceReward(54000, this.Context);

        this.CreateQuest("Situation at the Dungeon (3)", 18, 43, 45, 46, 111, 120, 257, CharacterClassNumber.DarkWizard)
            .WithMonsterKillRequirement(50, 5, this.Context, this.GameConfiguration)
            .WithExperienceReward(54000, this.Context);
        this.CreateQuest("Situation at the Dungeon (3)", 18, 43, 45, 46, 111, 120, 257, CharacterClassNumber.DarkKnight)
            .WithMonsterKillRequirement(50, 5, this.Context, this.GameConfiguration)
            .WithExperienceReward(54000, this.Context);
        this.CreateQuest("Situation at the Dungeon (3)", 18, 43, 45, 46, 111, 120, 257, CharacterClassNumber.FairyElf)
            .WithMonsterKillRequirement(50, 5, this.Context, this.GameConfiguration)
            .WithExperienceReward(54000, this.Context);
        this.CreateQuest("Situation at the Dungeon (3)", 18, 43, 45, 46, 111, 120, 257, CharacterClassNumber.Summoner)
            .WithMonsterKillRequirement(50, 5, this.Context, this.GameConfiguration)
            .WithExperienceReward(54000, this.Context);

        this.CreateQuest("Situation at the Dungeon (4)", 18, 47, 48, 50, 121, 130, 257, CharacterClassNumber.MagicGladiator)
            .WithMonsterKillRequirement(50, 8, this.Context, this.GameConfiguration)
            .WithExperienceReward(60000, this.Context);
        this.CreateQuest("Situation at the Dungeon (4)", 18, 47, 48, 50, 121, 130, 257, CharacterClassNumber.DarkLord)
            .WithMonsterKillRequirement(50, 8, this.Context, this.GameConfiguration)
            .WithExperienceReward(60000, this.Context);
        this.CreateQuest("Situation at the Dungeon (4)", 18, 47, 48, 50, 121, 130, 257, CharacterClassNumber.RageFighter)
            .WithMonsterKillRequirement(50, 8, this.Context, this.GameConfiguration)
            .WithExperienceReward(60000, this.Context);

        this.CreateQuest("Situation at the Dungeon (4)", 18, 47, 49, 50, 121, 130, 257, CharacterClassNumber.DarkWizard)
            .WithMonsterKillRequirement(50, 13, this.Context, this.GameConfiguration)
            .WithExperienceReward(60000, this.Context);
        this.CreateQuest("Situation at the Dungeon (4)", 18, 47, 49, 50, 121, 130, 257, CharacterClassNumber.DarkKnight)
            .WithMonsterKillRequirement(50, 13, this.Context, this.GameConfiguration)
            .WithExperienceReward(60000, this.Context);
        this.CreateQuest("Situation at the Dungeon (4)", 18, 47, 49, 50, 121, 130, 257, CharacterClassNumber.FairyElf)
            .WithMonsterKillRequirement(50, 13, this.Context, this.GameConfiguration)
            .WithExperienceReward(60000, this.Context);
        this.CreateQuest("Situation at the Dungeon (4)", 18, 47, 49, 50, 121, 130, 257, CharacterClassNumber.Summoner)
            .WithMonsterKillRequirement(50, 13, this.Context, this.GameConfiguration)
            .WithExperienceReward(60000, this.Context);

        this.CreateQuest("Situation at the Dungeon (5)", 18, 51, 52, 54, 131, 140, 257, CharacterClassNumber.MagicGladiator)
            .WithMonsterKillRequirement(50, 10, this.Context, this.GameConfiguration)
            .WithExperienceReward(66000, this.Context);
        this.CreateQuest("Situation at the Dungeon (5)", 18, 51, 52, 54, 131, 140, 257, CharacterClassNumber.DarkLord)
            .WithMonsterKillRequirement(50, 10, this.Context, this.GameConfiguration)
            .WithExperienceReward(66000, this.Context);
        this.CreateQuest("Situation at the Dungeon (5)", 18, 51, 52, 54, 131, 140, 257, CharacterClassNumber.RageFighter)
            .WithMonsterKillRequirement(50, 10, this.Context, this.GameConfiguration)
            .WithExperienceReward(66000, this.Context);

        this.CreateQuest("Situation at the Dungeon (5)", 18, 51, 53, 54, 131, 140, 257, CharacterClassNumber.DarkWizard)
            .WithMonsterKillRequirement(50, 9, this.Context, this.GameConfiguration)
            .WithExperienceReward(66000, this.Context);
        this.CreateQuest("Situation at the Dungeon (5)", 18, 51, 53, 54, 131, 140, 257, CharacterClassNumber.DarkKnight)
            .WithMonsterKillRequirement(50, 9, this.Context, this.GameConfiguration)
            .WithExperienceReward(66000, this.Context);
        this.CreateQuest("Situation at the Dungeon (5)", 18, 51, 53, 54, 131, 140, 257, CharacterClassNumber.FairyElf)
            .WithMonsterKillRequirement(50, 9, this.Context, this.GameConfiguration)
            .WithExperienceReward(66000, this.Context);
        this.CreateQuest("Situation at the Dungeon (5)", 18, 51, 53, 54, 131, 140, 257, CharacterClassNumber.Summoner)
            .WithMonsterKillRequirement(50, 9, this.Context, this.GameConfiguration)
            .WithExperienceReward(66000, this.Context);

        this.CreateQuest("Situation at the Dungeon (6)", 18, 55, 56, 58, 141, 160, 257, CharacterClassNumber.MagicGladiator)
            .WithMonsterKillRequirement(50, 18, this.Context, this.GameConfiguration)
            .WithExperienceReward(72000, this.Context);
        this.CreateQuest("Situation at the Dungeon (6)", 18, 55, 56, 58, 141, 160, 257, CharacterClassNumber.DarkLord)
            .WithMonsterKillRequirement(50, 18, this.Context, this.GameConfiguration)
            .WithExperienceReward(72000, this.Context);
        this.CreateQuest("Situation at the Dungeon (6)", 18, 55, 56, 58, 141, 160, 257, CharacterClassNumber.RageFighter)
            .WithMonsterKillRequirement(50, 18, this.Context, this.GameConfiguration)
            .WithExperienceReward(72000, this.Context);

        this.CreateQuest("Situation at the Dungeon (6)", 18, 55, 57, 58, 141, 160, 257, CharacterClassNumber.SoulMaster)
            .WithMonsterKillRequirement(15, 8, this.Context, this.GameConfiguration)
            .WithExperienceReward(72000, this.Context);
        this.CreateQuest("Situation at the Dungeon (6)", 18, 55, 57, 58, 141, 160, 257, CharacterClassNumber.DarkWizard)
            .WithMonsterKillRequirement(15, 8, this.Context, this.GameConfiguration)
            .WithExperienceReward(72000, this.Context);
        this.CreateQuest("Situation at the Dungeon (6)", 18, 55, 57, 58, 141, 160, 257, CharacterClassNumber.BladeKnight)
            .WithMonsterKillRequirement(15, 8, this.Context, this.GameConfiguration)
            .WithExperienceReward(72000, this.Context);
        this.CreateQuest("Situation at the Dungeon (6)", 18, 55, 57, 58, 141, 160, 257, CharacterClassNumber.DarkKnight)
            .WithMonsterKillRequirement(15, 8, this.Context, this.GameConfiguration)
            .WithExperienceReward(72000, this.Context);
        this.CreateQuest("Situation at the Dungeon (6)", 18, 55, 57, 58, 141, 160, 257, CharacterClassNumber.MuseElf)
            .WithMonsterKillRequirement(15, 8, this.Context, this.GameConfiguration)
            .WithExperienceReward(72000, this.Context);
        this.CreateQuest("Situation at the Dungeon (6)", 18, 55, 57, 58, 141, 160, 257, CharacterClassNumber.FairyElf)
            .WithMonsterKillRequirement(15, 8, this.Context, this.GameConfiguration)
            .WithExperienceReward(72000, this.Context);
        this.CreateQuest("Situation at the Dungeon (6)", 18, 55, 57, 58, 141, 160, 257, CharacterClassNumber.BloodySummoner)
            .WithMonsterKillRequirement(15, 8, this.Context, this.GameConfiguration)
            .WithExperienceReward(72000, this.Context);
        this.CreateQuest("Situation at the Dungeon (6)", 18, 55, 57, 58, 141, 160, 257, CharacterClassNumber.Summoner)
            .WithMonsterKillRequirement(15, 8, this.Context, this.GameConfiguration)
            .WithExperienceReward(72000, this.Context);

        this.CreateQuest("Continuing Requests for Help (1)", 18, 59, 60, 62, 161, 165, 257, CharacterClassNumber.MagicGladiator)
            .WithMonsterKillRequirement(50, 36, this.Context, this.GameConfiguration)
            .WithExperienceReward(80000, this.Context);
        this.CreateQuest("Continuing Requests for Help (1)", 18, 59, 60, 62, 161, 165, 257, CharacterClassNumber.DarkLord)
            .WithMonsterKillRequirement(50, 36, this.Context, this.GameConfiguration)
            .WithExperienceReward(80000, this.Context);
        this.CreateQuest("Continuing Requests for Help (1)", 18, 59, 60, 62, 161, 165, 257, CharacterClassNumber.RageFighter)
            .WithMonsterKillRequirement(50, 36, this.Context, this.GameConfiguration)
            .WithExperienceReward(80000, this.Context);

        this.CreateQuest("Continuing Requests for Help (1)", 18, 59, 61, 62, 161, 165, 257, CharacterClassNumber.SoulMaster)
            .WithMonsterKillRequirement(3, 25, this.Context, this.GameConfiguration)
            .WithExperienceReward(80000, this.Context);
        this.CreateQuest("Continuing Requests for Help (1)", 18, 59, 61, 62, 161, 165, 257, CharacterClassNumber.DarkWizard)
            .WithMonsterKillRequirement(3, 25, this.Context, this.GameConfiguration)
            .WithExperienceReward(80000, this.Context);
        this.CreateQuest("Continuing Requests for Help (1)", 18, 59, 61, 62, 161, 165, 257, CharacterClassNumber.BladeKnight)
            .WithMonsterKillRequirement(3, 25, this.Context, this.GameConfiguration)
            .WithExperienceReward(80000, this.Context);
        this.CreateQuest("Continuing Requests for Help (1)", 18, 59, 61, 62, 161, 165, 257, CharacterClassNumber.DarkKnight)
            .WithMonsterKillRequirement(3, 25, this.Context, this.GameConfiguration)
            .WithExperienceReward(80000, this.Context);
        this.CreateQuest("Continuing Requests for Help (1)", 18, 59, 61, 62, 161, 165, 257, CharacterClassNumber.MuseElf)
            .WithMonsterKillRequirement(3, 25, this.Context, this.GameConfiguration)
            .WithExperienceReward(80000, this.Context);
        this.CreateQuest("Continuing Requests for Help (1)", 18, 59, 61, 62, 161, 165, 257, CharacterClassNumber.FairyElf)
            .WithMonsterKillRequirement(3, 25, this.Context, this.GameConfiguration)
            .WithExperienceReward(80000, this.Context);
        this.CreateQuest("Continuing Requests for Help (1)", 18, 59, 61, 62, 161, 165, 257, CharacterClassNumber.BloodySummoner)
            .WithMonsterKillRequirement(3, 25, this.Context, this.GameConfiguration)
            .WithExperienceReward(80000, this.Context);
        this.CreateQuest("Continuing Requests for Help (1)", 18, 59, 61, 62, 161, 165, 257, CharacterClassNumber.Summoner)
            .WithMonsterKillRequirement(3, 25, this.Context, this.GameConfiguration)
            .WithExperienceReward(80000, this.Context);

        this.CreateQuest("Continuing Requests for Help (2)", 18, 63, 64, 66, 166, 170, 257, CharacterClassNumber.MagicGladiator)
            .WithMonsterKillRequirement(50, 39, this.Context, this.GameConfiguration)
            .WithExperienceReward(82000, this.Context);
        this.CreateQuest("Continuing Requests for Help (2)", 18, 63, 64, 66, 166, 170, 257, CharacterClassNumber.DarkLord)
            .WithMonsterKillRequirement(50, 39, this.Context, this.GameConfiguration)
            .WithExperienceReward(82000, this.Context);
        this.CreateQuest("Continuing Requests for Help (2)", 18, 63, 64, 66, 166, 170, 257, CharacterClassNumber.RageFighter)
            .WithMonsterKillRequirement(50, 39, this.Context, this.GameConfiguration)
            .WithExperienceReward(82000, this.Context);

        this.CreateQuest("Continuing Requests for Help (2)", 18, 63, 65, 66, 166, 170, 257, CharacterClassNumber.SoulMaster)
            .WithMonsterKillRequirement(50, 424, this.Context, this.GameConfiguration)
            .WithExperienceReward(82000, this.Context);
        this.CreateQuest("Continuing Requests for Help (2)", 18, 63, 65, 66, 166, 170, 257, CharacterClassNumber.DarkWizard)
            .WithMonsterKillRequirement(50, 424, this.Context, this.GameConfiguration)
            .WithExperienceReward(82000, this.Context);
        this.CreateQuest("Continuing Requests for Help (2)", 18, 63, 65, 66, 166, 170, 257, CharacterClassNumber.BladeKnight)
            .WithMonsterKillRequirement(50, 424, this.Context, this.GameConfiguration)
            .WithExperienceReward(82000, this.Context);
        this.CreateQuest("Continuing Requests for Help (2)", 18, 63, 65, 66, 166, 170, 257, CharacterClassNumber.DarkKnight)
            .WithMonsterKillRequirement(50, 424, this.Context, this.GameConfiguration)
            .WithExperienceReward(82000, this.Context);
        this.CreateQuest("Continuing Requests for Help (2)", 18, 63, 65, 66, 166, 170, 257, CharacterClassNumber.MuseElf)
            .WithMonsterKillRequirement(50, 424, this.Context, this.GameConfiguration)
            .WithExperienceReward(82000, this.Context);
        this.CreateQuest("Continuing Requests for Help (2)", 18, 63, 65, 66, 166, 170, 257, CharacterClassNumber.FairyElf)
            .WithMonsterKillRequirement(50, 424, this.Context, this.GameConfiguration)
            .WithExperienceReward(82000, this.Context);
        this.CreateQuest("Continuing Requests for Help (2)", 18, 63, 65, 66, 166, 170, 257, CharacterClassNumber.BloodySummoner)
            .WithMonsterKillRequirement(50, 424, this.Context, this.GameConfiguration)
            .WithExperienceReward(82000, this.Context);
        this.CreateQuest("Continuing Requests for Help (2)", 18, 63, 65, 66, 166, 170, 257, CharacterClassNumber.Summoner)
            .WithMonsterKillRequirement(50, 424, this.Context, this.GameConfiguration)
            .WithExperienceReward(82000, this.Context);

        this.CreateQuest("Continuing Requests for Help (3)", 18, 67, 68, 70, 171, 179, 257, CharacterClassNumber.MagicGladiator)
            .WithMonsterKillRequirement(50, 34, this.Context, this.GameConfiguration)
            .WithExperienceReward(84000, this.Context);
        this.CreateQuest("Continuing Requests for Help (3)", 18, 67, 68, 70, 171, 179, 257, CharacterClassNumber.DarkLord)
            .WithMonsterKillRequirement(50, 34, this.Context, this.GameConfiguration)
            .WithExperienceReward(84000, this.Context);
        this.CreateQuest("Continuing Requests for Help (3)", 18, 67, 68, 70, 171, 179, 257, CharacterClassNumber.RageFighter)
            .WithMonsterKillRequirement(50, 34, this.Context, this.GameConfiguration)
            .WithExperienceReward(84000, this.Context);

        this.CreateQuest("Continuing Requests for Help (3)", 18, 67, 69, 70, 171, 179, 257, CharacterClassNumber.SoulMaster)
            .WithMonsterKillRequirement(50, 425, this.Context, this.GameConfiguration)
            .WithExperienceReward(84000, this.Context);
        this.CreateQuest("Continuing Requests for Help (3)", 18, 67, 69, 70, 171, 179, 257, CharacterClassNumber.DarkWizard)
            .WithMonsterKillRequirement(50, 425, this.Context, this.GameConfiguration)
            .WithExperienceReward(84000, this.Context);
        this.CreateQuest("Continuing Requests for Help (3)", 18, 67, 69, 70, 171, 179, 257, CharacterClassNumber.BladeKnight)
            .WithMonsterKillRequirement(50, 425, this.Context, this.GameConfiguration)
            .WithExperienceReward(84000, this.Context);
        this.CreateQuest("Continuing Requests for Help (3)", 18, 67, 69, 70, 171, 179, 257, CharacterClassNumber.DarkKnight)
            .WithMonsterKillRequirement(50, 425, this.Context, this.GameConfiguration)
            .WithExperienceReward(84000, this.Context);
        this.CreateQuest("Continuing Requests for Help (3)", 18, 67, 69, 70, 171, 179, 257, CharacterClassNumber.MuseElf)
            .WithMonsterKillRequirement(50, 425, this.Context, this.GameConfiguration)
            .WithExperienceReward(84000, this.Context);
        this.CreateQuest("Continuing Requests for Help (3)", 18, 67, 69, 70, 171, 179, 257, CharacterClassNumber.FairyElf)
            .WithMonsterKillRequirement(50, 425, this.Context, this.GameConfiguration)
            .WithExperienceReward(84000, this.Context);
        this.CreateQuest("Continuing Requests for Help (3)", 18, 67, 69, 70, 171, 179, 257, CharacterClassNumber.BloodySummoner)
            .WithMonsterKillRequirement(50, 425, this.Context, this.GameConfiguration)
            .WithExperienceReward(84000, this.Context);
        this.CreateQuest("Continuing Requests for Help (3)", 18, 67, 69, 70, 171, 179, 257, CharacterClassNumber.Summoner)
            .WithMonsterKillRequirement(50, 425, this.Context, this.GameConfiguration)
            .WithExperienceReward(84000, this.Context);

        this.CreateQuest("Continuing Requests for Help (4)", 18, 71, 72, 74, 180, 189, 257, CharacterClassNumber.MagicGladiator)
            .WithMonsterKillRequirement(50, 41, this.Context, this.GameConfiguration)
            .WithExperienceReward(95000, this.Context);
        this.CreateQuest("Continuing Requests for Help (4)", 18, 71, 72, 74, 180, 189, 257, CharacterClassNumber.DarkLord)
            .WithMonsterKillRequirement(50, 41, this.Context, this.GameConfiguration)
            .WithExperienceReward(95000, this.Context);
        this.CreateQuest("Continuing Requests for Help (4)", 18, 71, 72, 74, 180, 189, 257, CharacterClassNumber.RageFighter)
            .WithMonsterKillRequirement(50, 41, this.Context, this.GameConfiguration)
            .WithExperienceReward(95000, this.Context);

        this.CreateQuest("Continuing Requests for Help (4)", 18, 71, 73, 74, 180, 189, 257, CharacterClassNumber.SoulMaster)
            .WithMonsterKillRequirement(50, 36, this.Context, this.GameConfiguration)
            .WithExperienceReward(95000, this.Context);
        this.CreateQuest("Continuing Requests for Help (4)", 18, 71, 73, 74, 180, 189, 257, CharacterClassNumber.DarkWizard)
            .WithMonsterKillRequirement(50, 36, this.Context, this.GameConfiguration)
            .WithExperienceReward(95000, this.Context);
        this.CreateQuest("Continuing Requests for Help (4)", 18, 71, 73, 74, 180, 189, 257, CharacterClassNumber.BladeKnight)
            .WithMonsterKillRequirement(50, 36, this.Context, this.GameConfiguration)
            .WithExperienceReward(95000, this.Context);
        this.CreateQuest("Continuing Requests for Help (4)", 18, 71, 73, 74, 180, 189, 257, CharacterClassNumber.DarkKnight)
            .WithMonsterKillRequirement(50, 36, this.Context, this.GameConfiguration)
            .WithExperienceReward(95000, this.Context);
        this.CreateQuest("Continuing Requests for Help (4)", 18, 71, 73, 74, 180, 189, 257, CharacterClassNumber.MuseElf)
            .WithMonsterKillRequirement(50, 36, this.Context, this.GameConfiguration)
            .WithExperienceReward(95000, this.Context);
        this.CreateQuest("Continuing Requests for Help (4)", 18, 71, 73, 74, 180, 189, 257, CharacterClassNumber.FairyElf)
            .WithMonsterKillRequirement(50, 36, this.Context, this.GameConfiguration)
            .WithExperienceReward(95000, this.Context);
        this.CreateQuest("Continuing Requests for Help (4)", 18, 71, 73, 74, 180, 189, 257, CharacterClassNumber.BloodySummoner)
            .WithMonsterKillRequirement(50, 36, this.Context, this.GameConfiguration)
            .WithExperienceReward(95000, this.Context);
        this.CreateQuest("Continuing Requests for Help (4)", 18, 71, 73, 74, 180, 189, 257, CharacterClassNumber.Summoner)
            .WithMonsterKillRequirement(50, 36, this.Context, this.GameConfiguration)
            .WithExperienceReward(95000, this.Context);

        this.CreateQuest("Continuing Attacks on Barriers (1)", 18, 75, 76, 78, 190, 199, 257, CharacterClassNumber.MagicGladiator)
            .WithMonsterKillRequirement(50, 37, this.Context, this.GameConfiguration)
            .WithExperienceReward(100000, this.Context);
        this.CreateQuest("Continuing Attacks on Barriers (1)", 18, 75, 76, 78, 190, 199, 257, CharacterClassNumber.DarkLord)
            .WithMonsterKillRequirement(50, 37, this.Context, this.GameConfiguration)
            .WithExperienceReward(100000, this.Context);
        this.CreateQuest("Continuing Attacks on Barriers (1)", 18, 75, 76, 78, 190, 199, 257, CharacterClassNumber.RageFighter)
            .WithMonsterKillRequirement(50, 37, this.Context, this.GameConfiguration)
            .WithExperienceReward(100000, this.Context);

        this.CreateQuest("Continuing Attacks on Barriers (1)", 18, 75, 77, 78, 190, 199, 257, CharacterClassNumber.SoulMaster)
            .WithMonsterKillRequirement(50, 34, this.Context, this.GameConfiguration)
            .WithExperienceReward(100000, this.Context);
        this.CreateQuest("Continuing Attacks on Barriers (1)", 18, 75, 77, 78, 190, 199, 257, CharacterClassNumber.DarkWizard)
            .WithMonsterKillRequirement(50, 34, this.Context, this.GameConfiguration)
            .WithExperienceReward(100000, this.Context);
        this.CreateQuest("Continuing Attacks on Barriers (1)", 18, 75, 77, 78, 190, 199, 257, CharacterClassNumber.BladeKnight)
            .WithMonsterKillRequirement(50, 34, this.Context, this.GameConfiguration)
            .WithExperienceReward(100000, this.Context);
        this.CreateQuest("Continuing Attacks on Barriers (1)", 18, 75, 77, 78, 190, 199, 257, CharacterClassNumber.DarkKnight)
            .WithMonsterKillRequirement(50, 34, this.Context, this.GameConfiguration)
            .WithExperienceReward(100000, this.Context);
        this.CreateQuest("Continuing Attacks on Barriers (1)", 18, 75, 77, 78, 190, 199, 257, CharacterClassNumber.MuseElf)
            .WithMonsterKillRequirement(50, 34, this.Context, this.GameConfiguration)
            .WithExperienceReward(100000, this.Context);
        this.CreateQuest("Continuing Attacks on Barriers (1)", 18, 75, 77, 78, 190, 199, 257, CharacterClassNumber.FairyElf)
            .WithMonsterKillRequirement(50, 34, this.Context, this.GameConfiguration)
            .WithExperienceReward(100000, this.Context);
        this.CreateQuest("Continuing Attacks on Barriers (1)", 18, 75, 77, 78, 190, 199, 257, CharacterClassNumber.BloodySummoner)
            .WithMonsterKillRequirement(50, 34, this.Context, this.GameConfiguration)
            .WithExperienceReward(100000, this.Context);
        this.CreateQuest("Continuing Attacks on Barriers (1)", 18, 75, 77, 78, 190, 199, 257, CharacterClassNumber.Summoner)
            .WithMonsterKillRequirement(50, 34, this.Context, this.GameConfiguration)
            .WithExperienceReward(100000, this.Context);

        this.CreateQuest("Continuing Attacks on Barriers (2)", 18, 79, 80, 82, 200, 209, 257, CharacterClassNumber.MagicGladiator)
            .WithMonsterKillRequirement(50, 40, this.Context, this.GameConfiguration)
            .WithExperienceReward(115000, this.Context);
        this.CreateQuest("Continuing Attacks on Barriers (2)", 18, 79, 80, 82, 200, 209, 257, CharacterClassNumber.DarkLord)
            .WithMonsterKillRequirement(50, 40, this.Context, this.GameConfiguration)
            .WithExperienceReward(115000, this.Context);
        this.CreateQuest("Continuing Attacks on Barriers (2)", 18, 79, 80, 82, 200, 209, 257, CharacterClassNumber.RageFighter)
            .WithMonsterKillRequirement(50, 40, this.Context, this.GameConfiguration)
            .WithExperienceReward(115000, this.Context);

        this.CreateQuest("Continuing Attacks on Barriers (2)", 18, 79, 81, 82, 200, 209, 257, CharacterClassNumber.SoulMaster)
            .WithMonsterKillRequirement(50, 41, this.Context, this.GameConfiguration)
            .WithExperienceReward(115000, this.Context);
        this.CreateQuest("Continuing Attacks on Barriers (2)", 18, 79, 81, 82, 200, 209, 257, CharacterClassNumber.DarkWizard)
            .WithMonsterKillRequirement(50, 41, this.Context, this.GameConfiguration)
            .WithExperienceReward(115000, this.Context);
        this.CreateQuest("Continuing Attacks on Barriers (2)", 18, 79, 81, 82, 200, 209, 257, CharacterClassNumber.BladeKnight)
            .WithMonsterKillRequirement(50, 41, this.Context, this.GameConfiguration)
            .WithExperienceReward(115000, this.Context);
        this.CreateQuest("Continuing Attacks on Barriers (2)", 18, 79, 81, 82, 200, 209, 257, CharacterClassNumber.DarkKnight)
            .WithMonsterKillRequirement(50, 41, this.Context, this.GameConfiguration)
            .WithExperienceReward(115000, this.Context);
        this.CreateQuest("Continuing Attacks on Barriers (2)", 18, 79, 81, 82, 200, 209, 257, CharacterClassNumber.MuseElf)
            .WithMonsterKillRequirement(50, 41, this.Context, this.GameConfiguration)
            .WithExperienceReward(115000, this.Context);
        this.CreateQuest("Continuing Attacks on Barriers (2)", 18, 79, 81, 82, 200, 209, 257, CharacterClassNumber.FairyElf)
            .WithMonsterKillRequirement(50, 41, this.Context, this.GameConfiguration)
            .WithExperienceReward(115000, this.Context);
        this.CreateQuest("Continuing Attacks on Barriers (2)", 18, 79, 81, 82, 200, 209, 257, CharacterClassNumber.BloodySummoner)
            .WithMonsterKillRequirement(50, 41, this.Context, this.GameConfiguration)
            .WithExperienceReward(115000, this.Context);
        this.CreateQuest("Continuing Attacks on Barriers (2)", 18, 79, 81, 82, 200, 209, 257, CharacterClassNumber.Summoner)
            .WithMonsterKillRequirement(50, 41, this.Context, this.GameConfiguration)
            .WithExperienceReward(115000, this.Context);

        this.CreateQuest("Continuing Attacks on Barriers (3)", 18, 83, 84, 86, 210, 219, 257, CharacterClassNumber.MagicGladiator)
            .WithMonsterKillRequirement(60, 35, this.Context, this.GameConfiguration)
            .WithExperienceReward(130000, this.Context);
        this.CreateQuest("Continuing Attacks on Barriers (3)", 18, 83, 84, 86, 210, 219, 257, CharacterClassNumber.DarkLord)
            .WithMonsterKillRequirement(60, 35, this.Context, this.GameConfiguration)
            .WithExperienceReward(130000, this.Context);
        this.CreateQuest("Continuing Attacks on Barriers (3)", 18, 83, 84, 86, 210, 219, 257, CharacterClassNumber.RageFighter)
            .WithMonsterKillRequirement(60, 35, this.Context, this.GameConfiguration)
            .WithExperienceReward(130000, this.Context);

        this.CreateQuest("Continuing Attacks on Barriers (3)", 18, 83, 85, 86, 210, 219, 257, CharacterClassNumber.SoulMaster)
            .WithMonsterKillRequirement(60, 37, this.Context, this.GameConfiguration)
            .WithExperienceReward(130000, this.Context);
        this.CreateQuest("Continuing Attacks on Barriers (3)", 18, 83, 85, 86, 210, 219, 257, CharacterClassNumber.DarkWizard)
            .WithMonsterKillRequirement(60, 37, this.Context, this.GameConfiguration)
            .WithExperienceReward(130000, this.Context);
        this.CreateQuest("Continuing Attacks on Barriers (3)", 18, 83, 85, 86, 210, 219, 257, CharacterClassNumber.BladeKnight)
            .WithMonsterKillRequirement(60, 37, this.Context, this.GameConfiguration)
            .WithExperienceReward(130000, this.Context);
        this.CreateQuest("Continuing Attacks on Barriers (3)", 18, 83, 85, 86, 210, 219, 257, CharacterClassNumber.DarkKnight)
            .WithMonsterKillRequirement(60, 37, this.Context, this.GameConfiguration)
            .WithExperienceReward(130000, this.Context);
        this.CreateQuest("Continuing Attacks on Barriers (3)", 18, 83, 85, 86, 210, 219, 257, CharacterClassNumber.MuseElf)
            .WithMonsterKillRequirement(60, 37, this.Context, this.GameConfiguration)
            .WithExperienceReward(130000, this.Context);
        this.CreateQuest("Continuing Attacks on Barriers (3)", 18, 83, 85, 86, 210, 219, 257, CharacterClassNumber.FairyElf)
            .WithMonsterKillRequirement(60, 37, this.Context, this.GameConfiguration)
            .WithExperienceReward(130000, this.Context);
        this.CreateQuest("Continuing Attacks on Barriers (3)", 18, 83, 85, 86, 210, 219, 257, CharacterClassNumber.BloodySummoner)
            .WithMonsterKillRequirement(60, 37, this.Context, this.GameConfiguration)
            .WithExperienceReward(130000, this.Context);
        this.CreateQuest("Continuing Attacks on Barriers (3)", 18, 83, 85, 86, 210, 219, 257, CharacterClassNumber.Summoner)
            .WithMonsterKillRequirement(60, 37, this.Context, this.GameConfiguration)
            .WithExperienceReward(130000, this.Context);

        this.CreateQuest("Clearing a Path to Icarus", 18, 87, 88, 90, 220, 229, 257, CharacterClassNumber.MagicGladiator)
            .WithMonsterKillRequirement(15, 38, this.Context, this.GameConfiguration)
            .WithExperienceReward(140000, this.Context);
        this.CreateQuest("Clearing a Path to Icarus", 18, 87, 88, 90, 220, 229, 257, CharacterClassNumber.DarkLord)
            .WithMonsterKillRequirement(15, 38, this.Context, this.GameConfiguration)
            .WithExperienceReward(140000, this.Context);
        this.CreateQuest("Clearing a Path to Icarus", 18, 87, 88, 90, 220, 229, 257, CharacterClassNumber.RageFighter)
            .WithMonsterKillRequirement(15, 38, this.Context, this.GameConfiguration)
            .WithExperienceReward(140000, this.Context);

        this.CreateQuest("Clearing a Path to Icarus", 18, 87, 89, 90, 220, 229, 257, CharacterClassNumber.SoulMaster)
            .WithMonsterKillRequirement(60, 40, this.Context, this.GameConfiguration)
            .WithExperienceReward(140000, this.Context);
        this.CreateQuest("Clearing a Path to Icarus", 18, 87, 89, 90, 220, 229, 257, CharacterClassNumber.DarkWizard)
            .WithMonsterKillRequirement(60, 40, this.Context, this.GameConfiguration)
            .WithExperienceReward(140000, this.Context);
        this.CreateQuest("Clearing a Path to Icarus", 18, 87, 89, 90, 220, 229, 257, CharacterClassNumber.BladeKnight)
            .WithMonsterKillRequirement(60, 40, this.Context, this.GameConfiguration)
            .WithExperienceReward(140000, this.Context);
        this.CreateQuest("Clearing a Path to Icarus", 18, 87, 89, 90, 220, 229, 257, CharacterClassNumber.DarkKnight)
            .WithMonsterKillRequirement(60, 40, this.Context, this.GameConfiguration)
            .WithExperienceReward(140000, this.Context);
        this.CreateQuest("Clearing a Path to Icarus", 18, 87, 89, 90, 220, 229, 257, CharacterClassNumber.MuseElf)
            .WithMonsterKillRequirement(60, 40, this.Context, this.GameConfiguration)
            .WithExperienceReward(140000, this.Context);
        this.CreateQuest("Clearing a Path to Icarus", 18, 87, 89, 90, 220, 229, 257, CharacterClassNumber.FairyElf)
            .WithMonsterKillRequirement(60, 40, this.Context, this.GameConfiguration)
            .WithExperienceReward(140000, this.Context);
        this.CreateQuest("Clearing a Path to Icarus", 18, 87, 89, 90, 220, 229, 257, CharacterClassNumber.BloodySummoner)
            .WithMonsterKillRequirement(60, 40, this.Context, this.GameConfiguration)
            .WithExperienceReward(140000, this.Context);
        this.CreateQuest("Clearing a Path to Icarus", 18, 87, 89, 90, 220, 229, 257, CharacterClassNumber.Summoner)
            .WithMonsterKillRequirement(60, 40, this.Context, this.GameConfiguration)
            .WithExperienceReward(140000, this.Context);

        this.CreateQuest("Clearing a Path to Icarus", 18, 91, 92, 94, 220, 229, 257, CharacterClassNumber.MagicGladiator)
            .WithMonsterKillRequirement(15, 38, this.Context, this.GameConfiguration)
            .WithItemReward(12, 15, this.Context, this.GameConfiguration);
        this.CreateQuest("Clearing a Path to Icarus", 18, 91, 92, 94, 220, 229, 257, CharacterClassNumber.DarkLord)
            .WithMonsterKillRequirement(15, 38, this.Context, this.GameConfiguration)
            .WithItemReward(12, 15, this.Context, this.GameConfiguration);
        this.CreateQuest("Clearing a Path to Icarus", 18, 91, 92, 94, 220, 229, 257, CharacterClassNumber.RageFighter)
            .WithMonsterKillRequirement(15, 38, this.Context, this.GameConfiguration)
            .WithItemReward(12, 15, this.Context, this.GameConfiguration);

        this.CreateQuest("Clearing a Path to Icarus", 18, 91, 93, 94, 220, 229, 257, CharacterClassNumber.SoulMaster)
            .WithMonsterKillRequirement(60, 40, this.Context, this.GameConfiguration)
            .WithItemReward(12, 15, this.Context, this.GameConfiguration);
        this.CreateQuest("Clearing a Path to Icarus", 18, 91, 93, 94, 220, 229, 257, CharacterClassNumber.DarkWizard)
            .WithMonsterKillRequirement(60, 40, this.Context, this.GameConfiguration)
            .WithItemReward(12, 15, this.Context, this.GameConfiguration);
        this.CreateQuest("Clearing a Path to Icarus", 18, 91, 93, 94, 220, 229, 257, CharacterClassNumber.BladeKnight)
            .WithMonsterKillRequirement(60, 40, this.Context, this.GameConfiguration)
            .WithItemReward(12, 15, this.Context, this.GameConfiguration);
        this.CreateQuest("Clearing a Path to Icarus", 18, 91, 93, 94, 220, 229, 257, CharacterClassNumber.DarkKnight)
            .WithMonsterKillRequirement(60, 40, this.Context, this.GameConfiguration)
            .WithItemReward(12, 15, this.Context, this.GameConfiguration);
        this.CreateQuest("Clearing a Path to Icarus", 18, 91, 93, 94, 220, 229, 257, CharacterClassNumber.MuseElf)
            .WithMonsterKillRequirement(60, 40, this.Context, this.GameConfiguration)
            .WithItemReward(12, 15, this.Context, this.GameConfiguration);
        this.CreateQuest("Clearing a Path to Icarus", 18, 91, 93, 94, 220, 229, 257, CharacterClassNumber.FairyElf)
            .WithMonsterKillRequirement(60, 40, this.Context, this.GameConfiguration)
            .WithItemReward(12, 15, this.Context, this.GameConfiguration);
        this.CreateQuest("Clearing a Path to Icarus", 18, 91, 93, 94, 220, 229, 257, CharacterClassNumber.BloodySummoner)
            .WithMonsterKillRequirement(60, 40, this.Context, this.GameConfiguration)
            .WithItemReward(12, 15, this.Context, this.GameConfiguration);
        this.CreateQuest("Clearing a Path to Icarus", 18, 91, 93, 94, 220, 229, 257, CharacterClassNumber.Summoner)
            .WithMonsterKillRequirement(60, 40, this.Context, this.GameConfiguration)
            .WithItemReward(12, 15, this.Context, this.GameConfiguration);

        this.CreateQuest("Advancement of the Reconnaissance Party (1)", 18, 95, 96, 97, 230, 234, 257, CharacterClassNumber.SoulMaster)
            .WithMonsterKillRequirement(60, 62, this.Context, this.GameConfiguration)
            .WithExperienceReward(150000, this.Context);
        this.CreateQuest("Advancement of the Reconnaissance Party (1)", 18, 95, 96, 97, 230, 234, 257, CharacterClassNumber.DarkWizard)
            .WithMonsterKillRequirement(60, 62, this.Context, this.GameConfiguration)
            .WithExperienceReward(150000, this.Context);
        this.CreateQuest("Advancement of the Reconnaissance Party (1)", 18, 95, 96, 97, 230, 234, 257, CharacterClassNumber.BladeKnight)
            .WithMonsterKillRequirement(60, 62, this.Context, this.GameConfiguration)
            .WithExperienceReward(150000, this.Context);
        this.CreateQuest("Advancement of the Reconnaissance Party (1)", 18, 95, 96, 97, 230, 234, 257, CharacterClassNumber.DarkKnight)
            .WithMonsterKillRequirement(60, 62, this.Context, this.GameConfiguration)
            .WithExperienceReward(150000, this.Context);
        this.CreateQuest("Advancement of the Reconnaissance Party (1)", 18, 95, 96, 97, 230, 234, 257, CharacterClassNumber.MuseElf)
            .WithMonsterKillRequirement(60, 62, this.Context, this.GameConfiguration)
            .WithExperienceReward(150000, this.Context);
        this.CreateQuest("Advancement of the Reconnaissance Party (1)", 18, 95, 96, 97, 230, 234, 257, CharacterClassNumber.FairyElf)
            .WithMonsterKillRequirement(60, 62, this.Context, this.GameConfiguration)
            .WithExperienceReward(150000, this.Context);
        this.CreateQuest("Advancement of the Reconnaissance Party (1)", 18, 95, 96, 97, 230, 234, 257, CharacterClassNumber.BloodySummoner)
            .WithMonsterKillRequirement(60, 62, this.Context, this.GameConfiguration)
            .WithExperienceReward(150000, this.Context);
        this.CreateQuest("Advancement of the Reconnaissance Party (1)", 18, 95, 96, 97, 230, 234, 257, CharacterClassNumber.Summoner)
            .WithMonsterKillRequirement(60, 62, this.Context, this.GameConfiguration)
            .WithExperienceReward(150000, this.Context);

        this.CreateQuest("Advancement of the Reconnaissance Party (1)", 18, 98, 99, 100, 230, 234, 257, CharacterClassNumber.MagicGladiator)
            .WithMonsterKillRequirement(60, 62, this.Context, this.GameConfiguration)
            .WithExperienceReward(150000, this.Context);
        this.CreateQuest("Advancement of the Reconnaissance Party (1)", 18, 98, 99, 100, 230, 234, 257, CharacterClassNumber.DarkLord)
            .WithMonsterKillRequirement(60, 62, this.Context, this.GameConfiguration)
            .WithExperienceReward(150000, this.Context);
        this.CreateQuest("Advancement of the Reconnaissance Party (1)", 18, 98, 99, 100, 230, 234, 257, CharacterClassNumber.RageFighter)
            .WithMonsterKillRequirement(60, 62, this.Context, this.GameConfiguration)
            .WithExperienceReward(150000, this.Context);

        this.CreateQuest("Advancement of the Reconnaissance Party (2)", 18, 101, 102, 103, 235, 239, 257, CharacterClassNumber.MagicGladiator)
            .WithMonsterKillRequirement(60, 60, this.Context, this.GameConfiguration)
            .WithItemReward(14, 14, this.Context, this.GameConfiguration);
        this.CreateQuest("Advancement of the Reconnaissance Party (2)", 18, 101, 102, 103, 235, 239, 257, CharacterClassNumber.DarkLord)
            .WithMonsterKillRequirement(60, 60, this.Context, this.GameConfiguration)
            .WithItemReward(14, 14, this.Context, this.GameConfiguration);
        this.CreateQuest("Advancement of the Reconnaissance Party (2)", 18, 101, 102, 103, 235, 239, 257, CharacterClassNumber.RageFighter)
            .WithMonsterKillRequirement(60, 60, this.Context, this.GameConfiguration)
            .WithItemReward(14, 14, this.Context, this.GameConfiguration);

        this.CreateQuest("Advancement of the Reconnaissance Party (2)", 18, 104, 105, 106, 235, 239, 257, CharacterClassNumber.SoulMaster)
            .WithMonsterKillRequirement(60, 60, this.Context, this.GameConfiguration)
            .WithItemReward(14, 14, this.Context, this.GameConfiguration);
        this.CreateQuest("Advancement of the Reconnaissance Party (2)", 18, 104, 105, 106, 235, 239, 257, CharacterClassNumber.DarkWizard)
            .WithMonsterKillRequirement(60, 60, this.Context, this.GameConfiguration)
            .WithItemReward(14, 14, this.Context, this.GameConfiguration);
        this.CreateQuest("Advancement of the Reconnaissance Party (2)", 18, 104, 105, 106, 235, 239, 257, CharacterClassNumber.BladeKnight)
            .WithMonsterKillRequirement(60, 60, this.Context, this.GameConfiguration)
            .WithItemReward(14, 14, this.Context, this.GameConfiguration);
        this.CreateQuest("Advancement of the Reconnaissance Party (2)", 18, 104, 105, 106, 235, 239, 257, CharacterClassNumber.DarkKnight)
            .WithMonsterKillRequirement(60, 60, this.Context, this.GameConfiguration)
            .WithItemReward(14, 14, this.Context, this.GameConfiguration);
        this.CreateQuest("Advancement of the Reconnaissance Party (2)", 18, 104, 105, 106, 235, 239, 257, CharacterClassNumber.MuseElf)
            .WithMonsterKillRequirement(60, 60, this.Context, this.GameConfiguration)
            .WithItemReward(14, 14, this.Context, this.GameConfiguration);
        this.CreateQuest("Advancement of the Reconnaissance Party (2)", 18, 104, 105, 106, 235, 239, 257, CharacterClassNumber.FairyElf)
            .WithMonsterKillRequirement(60, 60, this.Context, this.GameConfiguration)
            .WithItemReward(14, 14, this.Context, this.GameConfiguration);
        this.CreateQuest("Advancement of the Reconnaissance Party (2)", 18, 104, 105, 106, 235, 239, 257, CharacterClassNumber.BloodySummoner)
            .WithMonsterKillRequirement(60, 60, this.Context, this.GameConfiguration)
            .WithItemReward(14, 14, this.Context, this.GameConfiguration);
        this.CreateQuest("Advancement of the Reconnaissance Party (2)", 18, 104, 105, 106, 235, 239, 257, CharacterClassNumber.Summoner)
            .WithMonsterKillRequirement(60, 60, this.Context, this.GameConfiguration)
            .WithItemReward(14, 14, this.Context, this.GameConfiguration);

        this.CreateQuest("Advancement of the Reconnaissance Party (3)", 18, 107, 108, 109, 240, 244, 257)
            .WithMonsterKillRequirement(60, 57, this.Context, this.GameConfiguration)
            .WithItemReward(14, 14, this.Context, this.GameConfiguration);

        this.CreateQuest("Advancement of the Reconnaissance Party (3)", 18, 110, 111, 112, 240, 244, 257)
            .WithMonsterKillRequirement(60, 57, this.Context, this.GameConfiguration)
            .WithExperienceReward(170000, this.Context);

        this.CreateQuest("Road to Floating Castle (1)", 18, 113, 114, 115, 245, 249, 257)
            .WithMonsterKillRequirement(80, 69, this.Context, this.GameConfiguration)
            .WithExperienceReward(200000, this.Context);

        this.CreateQuest("Road to Floating Castle (1)", 18, 116, 117, 118, 245, 249, 257)
            .WithMonsterKillRequirement(80, 69, this.Context, this.GameConfiguration)
            .WithItemReward(14, 13, this.Context, this.GameConfiguration);

        this.CreateQuest("Road to Floating Castle (2)", 18, 119, 120, 121, 250, 254, 257, CharacterClassNumber.MagicGladiator)
            .WithMonsterKillRequirement(80, 71, this.Context, this.GameConfiguration)
            .WithExperienceReward(190000, this.Context);
        this.CreateQuest("Road to Floating Castle (2)", 18, 119, 120, 121, 250, 254, 257, CharacterClassNumber.DarkLord)
            .WithMonsterKillRequirement(80, 71, this.Context, this.GameConfiguration)
            .WithExperienceReward(190000, this.Context);
        this.CreateQuest("Road to Floating Castle (2)", 18, 119, 120, 121, 250, 254, 257, CharacterClassNumber.RageFighter)
            .WithMonsterKillRequirement(80, 71, this.Context, this.GameConfiguration)
            .WithExperienceReward(190000, this.Context);

        this.CreateQuest("Road to Floating Castle (2)", 18, 122, 123, 124, 250, 254, 257, CharacterClassNumber.SoulMaster)
            .WithMonsterKillRequirement(80, 71, this.Context, this.GameConfiguration)
            .WithExperienceReward(190000, this.Context);
        this.CreateQuest("Road to Floating Castle (2)", 18, 122, 123, 124, 250, 254, 257, CharacterClassNumber.DarkWizard)
            .WithMonsterKillRequirement(80, 71, this.Context, this.GameConfiguration)
            .WithExperienceReward(190000, this.Context);
        this.CreateQuest("Road to Floating Castle (2)", 18, 122, 123, 124, 250, 254, 257, CharacterClassNumber.BladeKnight)
            .WithMonsterKillRequirement(80, 71, this.Context, this.GameConfiguration)
            .WithExperienceReward(190000, this.Context);
        this.CreateQuest("Road to Floating Castle (2)", 18, 122, 123, 124, 250, 254, 257, CharacterClassNumber.DarkKnight)
            .WithMonsterKillRequirement(80, 71, this.Context, this.GameConfiguration)
            .WithExperienceReward(190000, this.Context);
        this.CreateQuest("Road to Floating Castle (2)", 18, 122, 123, 124, 250, 254, 257, CharacterClassNumber.MuseElf)
            .WithMonsterKillRequirement(80, 71, this.Context, this.GameConfiguration)
            .WithExperienceReward(190000, this.Context);
        this.CreateQuest("Road to Floating Castle (2)", 18, 122, 123, 124, 250, 254, 257, CharacterClassNumber.FairyElf)
            .WithMonsterKillRequirement(80, 71, this.Context, this.GameConfiguration)
            .WithExperienceReward(190000, this.Context);
        this.CreateQuest("Road to Floating Castle (2)", 18, 122, 123, 124, 250, 254, 257, CharacterClassNumber.BloodySummoner)
            .WithMonsterKillRequirement(80, 71, this.Context, this.GameConfiguration)
            .WithExperienceReward(190000, this.Context);
        this.CreateQuest("Road to Floating Castle (2)", 18, 122, 123, 124, 250, 254, 257, CharacterClassNumber.Summoner)
            .WithMonsterKillRequirement(80, 71, this.Context, this.GameConfiguration)
            .WithExperienceReward(190000, this.Context);

        this.CreateQuest("Road to Floating Castle (3)", 18, 125, 126, 127, 255, 259, 257, CharacterClassNumber.SoulMaster)
            .WithMonsterKillRequirement(80, 70, this.Context, this.GameConfiguration)
            .WithItemReward(14, 13, this.Context, this.GameConfiguration)
            .WithItemReward(14, 14, this.Context, this.GameConfiguration)
            .WithItemReward(12, 15, this.Context, this.GameConfiguration);
        this.CreateQuest("Road to Floating Castle (3)", 18, 125, 126, 127, 255, 259, 257, CharacterClassNumber.DarkWizard)
            .WithMonsterKillRequirement(80, 70, this.Context, this.GameConfiguration)
            .WithItemReward(14, 13, this.Context, this.GameConfiguration)
            .WithItemReward(14, 14, this.Context, this.GameConfiguration)
            .WithItemReward(12, 15, this.Context, this.GameConfiguration);
        this.CreateQuest("Road to Floating Castle (3)", 18, 125, 126, 127, 255, 259, 257, CharacterClassNumber.BladeKnight)
            .WithMonsterKillRequirement(80, 70, this.Context, this.GameConfiguration)
            .WithItemReward(14, 13, this.Context, this.GameConfiguration)
            .WithItemReward(14, 14, this.Context, this.GameConfiguration)
            .WithItemReward(12, 15, this.Context, this.GameConfiguration);
        this.CreateQuest("Road to Floating Castle (3)", 18, 125, 126, 127, 255, 259, 257, CharacterClassNumber.DarkKnight)
            .WithMonsterKillRequirement(80, 70, this.Context, this.GameConfiguration)
            .WithItemReward(14, 13, this.Context, this.GameConfiguration)
            .WithItemReward(14, 14, this.Context, this.GameConfiguration)
            .WithItemReward(12, 15, this.Context, this.GameConfiguration);
        this.CreateQuest("Road to Floating Castle (3)", 18, 125, 126, 127, 255, 259, 257, CharacterClassNumber.MuseElf)
            .WithMonsterKillRequirement(80, 70, this.Context, this.GameConfiguration)
            .WithItemReward(14, 13, this.Context, this.GameConfiguration)
            .WithItemReward(14, 14, this.Context, this.GameConfiguration)
            .WithItemReward(12, 15, this.Context, this.GameConfiguration);
        this.CreateQuest("Road to Floating Castle (3)", 18, 125, 126, 127, 255, 259, 257, CharacterClassNumber.FairyElf)
            .WithMonsterKillRequirement(80, 70, this.Context, this.GameConfiguration)
            .WithItemReward(14, 13, this.Context, this.GameConfiguration)
            .WithItemReward(14, 14, this.Context, this.GameConfiguration)
            .WithItemReward(12, 15, this.Context, this.GameConfiguration);
        this.CreateQuest("Road to Floating Castle (3)", 18, 125, 126, 127, 255, 259, 257, CharacterClassNumber.BloodySummoner)
            .WithMonsterKillRequirement(80, 70, this.Context, this.GameConfiguration)
            .WithItemReward(14, 13, this.Context, this.GameConfiguration)
            .WithItemReward(14, 14, this.Context, this.GameConfiguration)
            .WithItemReward(12, 15, this.Context, this.GameConfiguration);
        this.CreateQuest("Road to Floating Castle (3)", 18, 125, 126, 127, 255, 259, 257, CharacterClassNumber.Summoner)
            .WithMonsterKillRequirement(80, 70, this.Context, this.GameConfiguration)
            .WithItemReward(14, 13, this.Context, this.GameConfiguration)
            .WithItemReward(14, 14, this.Context, this.GameConfiguration)
            .WithItemReward(12, 15, this.Context, this.GameConfiguration);

        this.CreateQuest("Road to Floating Castle (3)", 18, 128, 129, 130, 255, 259, 257, CharacterClassNumber.MagicGladiator)
            .WithMonsterKillRequirement(80, 70, this.Context, this.GameConfiguration)
            .WithItemReward(14, 13, this.Context, this.GameConfiguration)
            .WithItemReward(14, 14, this.Context, this.GameConfiguration)
            .WithItemReward(12, 15, this.Context, this.GameConfiguration);
        this.CreateQuest("Road to Floating Castle (3)", 18, 128, 129, 130, 255, 259, 257, CharacterClassNumber.DarkLord)
            .WithMonsterKillRequirement(80, 70, this.Context, this.GameConfiguration)
            .WithItemReward(14, 13, this.Context, this.GameConfiguration)
            .WithItemReward(14, 14, this.Context, this.GameConfiguration)
            .WithItemReward(12, 15, this.Context, this.GameConfiguration);
        this.CreateQuest("Road to Floating Castle (3)", 18, 128, 129, 130, 255, 259, 257, CharacterClassNumber.RageFighter)
            .WithMonsterKillRequirement(80, 70, this.Context, this.GameConfiguration)
            .WithItemReward(14, 13, this.Context, this.GameConfiguration)
            .WithItemReward(14, 14, this.Context, this.GameConfiguration)
            .WithItemReward(12, 15, this.Context, this.GameConfiguration);

        this.CreateQuest("The Beginning of Requests", 15, 1, 3, 2, 80, 90, 257, CharacterClassNumber.FairyElf)
            .WithMonsterKillRequirement(50, 422, this.Context, this.GameConfiguration)
            .WithMonsterKillRequirement(50, 19, this.Context, this.GameConfiguration)
            .WithExperienceReward(60000, this.Context)
            .WithMoneyReward(1500000, this.Context)
            .WithItemReward(12, 15, this.Context, this.GameConfiguration);
        this.CreateQuest("The Beginning of Requests", 15, 1, 3, 2, 80, 90, 257, CharacterClassNumber.Summoner)
            .WithMonsterKillRequirement(50, 422, this.Context, this.GameConfiguration)
            .WithMonsterKillRequirement(50, 19, this.Context, this.GameConfiguration)
            .WithExperienceReward(60000, this.Context)
            .WithMoneyReward(1500000, this.Context)
            .WithItemReward(12, 15, this.Context, this.GameConfiguration);

        this.CreateQuest("The Beginning of Requests", 15, 1, 4, 2, 80, 90, 257, CharacterClassNumber.DarkWizard)
            .WithMonsterKillRequirement(50, 423, this.Context, this.GameConfiguration)
            .WithMonsterKillRequirement(50, 20, this.Context, this.GameConfiguration)
            .WithExperienceReward(60000, this.Context)
            .WithMoneyReward(1500000, this.Context)
            .WithItemReward(12, 15, this.Context, this.GameConfiguration);
        this.CreateQuest("The Beginning of Requests", 15, 1, 4, 2, 80, 90, 257, CharacterClassNumber.DarkKnight)
            .WithMonsterKillRequirement(50, 423, this.Context, this.GameConfiguration)
            .WithMonsterKillRequirement(50, 20, this.Context, this.GameConfiguration)
            .WithExperienceReward(60000, this.Context)
            .WithMoneyReward(1500000, this.Context)
            .WithItemReward(12, 15, this.Context, this.GameConfiguration);

        this.CreateQuest("Conquer the Dungeon! (1)", 15, 5, 6, 10, 91, 100, 257, CharacterClassNumber.DarkKnight)
            .WithMonsterKillRequirement(50, 15, this.Context, this.GameConfiguration)
            .WithMonsterKillRequirement(30, 5, this.Context, this.GameConfiguration)
            .WithItemReward(10, 1, this.Context, this.GameConfiguration, 8);

        this.CreateQuest("Conquer the Dungeon! (1)", 15, 5, 7, 10, 91, 100, 257, CharacterClassNumber.DarkWizard)
            .WithMonsterKillRequirement(50, 17, this.Context, this.GameConfiguration)
            .WithMonsterKillRequirement(30, 11, this.Context, this.GameConfiguration)
            .WithItemReward(10, 3, this.Context, this.GameConfiguration, 8);

        this.CreateQuest("Conquer the Dungeon! (1)", 15, 5, 8, 10, 91, 100, 257, CharacterClassNumber.FairyElf)
            .WithMonsterKillRequirement(50, 15, this.Context, this.GameConfiguration)
            .WithMonsterKillRequirement(30, 5, this.Context, this.GameConfiguration)
            .WithItemReward(10, 14, this.Context, this.GameConfiguration, 8);

        this.CreateQuest("Conquer the Dungeon! (1)", 15, 5, 9, 10, 91, 100, 257, CharacterClassNumber.Summoner)
            .WithMonsterKillRequirement(50, 17, this.Context, this.GameConfiguration)
            .WithMonsterKillRequirement(30, 11, this.Context, this.GameConfiguration)
            .WithItemReward(10, 40, this.Context, this.GameConfiguration, 8);

        this.CreateQuest("Conquer the Dungeon! (2)", 15, 11, 12, 16, 101, 110, 257, CharacterClassNumber.DarkKnight)
            .WithMonsterKillRequirement(50, 5, this.Context, this.GameConfiguration)
            .WithMonsterKillRequirement(30, 13, this.Context, this.GameConfiguration)
            .WithItemReward(11, 1, this.Context, this.GameConfiguration, 8);

        this.CreateQuest("Conquer the Dungeon! (2)", 15, 11, 13, 16, 101, 110, 257, CharacterClassNumber.DarkWizard)
            .WithMonsterKillRequirement(50, 15, this.Context, this.GameConfiguration)
            .WithMonsterKillRequirement(30, 5, this.Context, this.GameConfiguration)
            .WithItemReward(11, 3, this.Context, this.GameConfiguration, 8);

        this.CreateQuest("Conquer the Dungeon! (2)", 15, 11, 14, 16, 101, 110, 257, CharacterClassNumber.FairyElf)
            .WithMonsterKillRequirement(50, 5, this.Context, this.GameConfiguration)
            .WithMonsterKillRequirement(30, 13, this.Context, this.GameConfiguration)
            .WithItemReward(11, 14, this.Context, this.GameConfiguration, 8);

        this.CreateQuest("Conquer the Dungeon! (2)", 15, 11, 15, 16, 101, 110, 257, CharacterClassNumber.Summoner)
            .WithMonsterKillRequirement(50, 15, this.Context, this.GameConfiguration)
            .WithMonsterKillRequirement(30, 5, this.Context, this.GameConfiguration)
            .WithItemReward(11, 40, this.Context, this.GameConfiguration, 8);

        this.CreateQuest("Conquer the Dungeon! (3)", 15, 17, 18, 22, 111, 120, 257, CharacterClassNumber.DarkKnight)
            .WithMonsterKillRequirement(50, 16, this.Context, this.GameConfiguration)
            .WithMonsterKillRequirement(30, 9, this.Context, this.GameConfiguration)
            .WithItemReward(7, 1, this.Context, this.GameConfiguration, 8);

        this.CreateQuest("Conquer the Dungeon! (3)", 15, 17, 19, 22, 111, 120, 257, CharacterClassNumber.DarkWizard)
            .WithMonsterKillRequirement(50, 5, this.Context, this.GameConfiguration)
            .WithMonsterKillRequirement(30, 13, this.Context, this.GameConfiguration)
            .WithItemReward(7, 3, this.Context, this.GameConfiguration, 8);

        this.CreateQuest("Conquer the Dungeon! (3)", 15, 17, 20, 22, 111, 120, 257, CharacterClassNumber.FairyElf)
            .WithMonsterKillRequirement(50, 16, this.Context, this.GameConfiguration)
            .WithMonsterKillRequirement(30, 9, this.Context, this.GameConfiguration)
            .WithItemReward(7, 14, this.Context, this.GameConfiguration, 8);

        this.CreateQuest("Conquer the Dungeon! (3)", 15, 17, 21, 22, 111, 120, 257, CharacterClassNumber.Summoner)
            .WithMonsterKillRequirement(50, 5, this.Context, this.GameConfiguration)
            .WithMonsterKillRequirement(30, 13, this.Context, this.GameConfiguration)
            .WithItemReward(7, 40, this.Context, this.GameConfiguration, 8);

        this.CreateQuest("Conquer the Dungeon! (4)", 15, 23, 24, 28, 121, 170, 257, CharacterClassNumber.BladeKnight)
            .WithMonsterKillRequirement(50, 8, this.Context, this.GameConfiguration)
            .WithMonsterKillRequirement(30, 10, this.Context, this.GameConfiguration)
            .WithItemReward(9, 1, this.Context, this.GameConfiguration, 8);
        this.CreateQuest("Conquer the Dungeon! (4)", 15, 23, 24, 28, 121, 170, 257, CharacterClassNumber.DarkKnight)
            .WithMonsterKillRequirement(50, 8, this.Context, this.GameConfiguration)
            .WithMonsterKillRequirement(30, 10, this.Context, this.GameConfiguration)
            .WithItemReward(9, 1, this.Context, this.GameConfiguration, 8);

        this.CreateQuest("Conquer the Dungeon! (4)", 15, 23, 25, 28, 121, 170, 257, CharacterClassNumber.SoulMaster)
            .WithMonsterKillRequirement(50, 13, this.Context, this.GameConfiguration)
            .WithMonsterKillRequirement(30, 16, this.Context, this.GameConfiguration)
            .WithItemReward(9, 3, this.Context, this.GameConfiguration, 8);
        this.CreateQuest("Conquer the Dungeon! (4)", 15, 23, 25, 28, 121, 170, 257, CharacterClassNumber.DarkWizard)
            .WithMonsterKillRequirement(50, 13, this.Context, this.GameConfiguration)
            .WithMonsterKillRequirement(30, 16, this.Context, this.GameConfiguration)
            .WithItemReward(9, 3, this.Context, this.GameConfiguration, 8);

        this.CreateQuest("Conquer the Dungeon! (4)", 15, 23, 26, 28, 121, 170, 257, CharacterClassNumber.MuseElf)
            .WithMonsterKillRequirement(50, 8, this.Context, this.GameConfiguration)
            .WithMonsterKillRequirement(30, 10, this.Context, this.GameConfiguration)
            .WithItemReward(9, 14, this.Context, this.GameConfiguration, 8);
        this.CreateQuest("Conquer the Dungeon! (4)", 15, 23, 26, 28, 121, 170, 257, CharacterClassNumber.FairyElf)
            .WithMonsterKillRequirement(50, 8, this.Context, this.GameConfiguration)
            .WithMonsterKillRequirement(30, 10, this.Context, this.GameConfiguration)
            .WithItemReward(9, 14, this.Context, this.GameConfiguration, 8);

        this.CreateQuest("Conquer the Dungeon! (4)", 15, 23, 27, 28, 121, 170, 257, CharacterClassNumber.BloodySummoner)
            .WithMonsterKillRequirement(50, 13, this.Context, this.GameConfiguration)
            .WithMonsterKillRequirement(30, 16, this.Context, this.GameConfiguration)
            .WithItemReward(9, 40, this.Context, this.GameConfiguration, 8);
        this.CreateQuest("Conquer the Dungeon! (4)", 15, 23, 27, 28, 121, 170, 257, CharacterClassNumber.Summoner)
            .WithMonsterKillRequirement(50, 13, this.Context, this.GameConfiguration)
            .WithMonsterKillRequirement(30, 16, this.Context, this.GameConfiguration)
            .WithItemReward(9, 40, this.Context, this.GameConfiguration, 8);

        this.CreateQuest("Conquer the Dungeon! (5)", 15, 29, 30, 34, 131, 190, 257, CharacterClassNumber.BladeKnight)
            .WithMonsterKillRequirement(50, 10, this.Context, this.GameConfiguration)
            .WithMonsterKillRequirement(10, 18, this.Context, this.GameConfiguration)
            .WithItemReward(8, 1, this.Context, this.GameConfiguration, 8);
        this.CreateQuest("Conquer the Dungeon! (5)", 15, 29, 30, 34, 131, 190, 257, CharacterClassNumber.DarkKnight)
            .WithMonsterKillRequirement(50, 10, this.Context, this.GameConfiguration)
            .WithMonsterKillRequirement(10, 18, this.Context, this.GameConfiguration)
            .WithItemReward(8, 1, this.Context, this.GameConfiguration, 8);

        this.CreateQuest("Conquer the Dungeon! (5)", 15, 29, 31, 34, 131, 190, 257, CharacterClassNumber.SoulMaster)
            .WithMonsterKillRequirement(50, 16, this.Context, this.GameConfiguration)
            .WithMonsterKillRequirement(30, 9, this.Context, this.GameConfiguration)
            .WithItemReward(8, 3, this.Context, this.GameConfiguration, 8);
        this.CreateQuest("Conquer the Dungeon! (5)", 15, 29, 31, 34, 131, 190, 257, CharacterClassNumber.DarkWizard)
            .WithMonsterKillRequirement(50, 16, this.Context, this.GameConfiguration)
            .WithMonsterKillRequirement(30, 9, this.Context, this.GameConfiguration)
            .WithItemReward(8, 3, this.Context, this.GameConfiguration, 8);

        this.CreateQuest("Conquer the Dungeon! (5)", 15, 29, 32, 34, 131, 190, 257, CharacterClassNumber.MuseElf)
            .WithMonsterKillRequirement(50, 10, this.Context, this.GameConfiguration)
            .WithMonsterKillRequirement(10, 18, this.Context, this.GameConfiguration)
            .WithItemReward(8, 14, this.Context, this.GameConfiguration, 8);
        this.CreateQuest("Conquer the Dungeon! (5)", 15, 29, 32, 34, 131, 190, 257, CharacterClassNumber.FairyElf)
            .WithMonsterKillRequirement(50, 10, this.Context, this.GameConfiguration)
            .WithMonsterKillRequirement(10, 18, this.Context, this.GameConfiguration)
            .WithItemReward(8, 14, this.Context, this.GameConfiguration, 8);

        this.CreateQuest("Conquer the Dungeon! (5)", 15, 29, 33, 34, 131, 190, 257, CharacterClassNumber.BloodySummoner)
            .WithMonsterKillRequirement(50, 16, this.Context, this.GameConfiguration)
            .WithMonsterKillRequirement(30, 9, this.Context, this.GameConfiguration)
            .WithItemReward(8, 40, this.Context, this.GameConfiguration, 8);
        this.CreateQuest("Conquer the Dungeon! (5)", 15, 29, 33, 34, 131, 190, 257, CharacterClassNumber.Summoner)
            .WithMonsterKillRequirement(50, 16, this.Context, this.GameConfiguration)
            .WithMonsterKillRequirement(30, 9, this.Context, this.GameConfiguration)
            .WithItemReward(8, 40, this.Context, this.GameConfiguration, 8);

        this.CreateQuest("Conquer the Dungeon! (6)", 15, 35, 36, 40, 141, 160, 257, CharacterClassNumber.BladeKnight)
            .WithMonsterKillRequirement(50, 10, this.Context, this.GameConfiguration)
            .WithMonsterKillRequirement(30, 18, this.Context, this.GameConfiguration)
            .WithItemReward(0, 13, this.Context, this.GameConfiguration, 8, 0, hasLuck: false, hasSkill: true);
        this.CreateQuest("Conquer the Dungeon! (6)", 15, 35, 36, 40, 141, 160, 257, CharacterClassNumber.DarkKnight)
            .WithMonsterKillRequirement(50, 10, this.Context, this.GameConfiguration)
            .WithMonsterKillRequirement(30, 18, this.Context, this.GameConfiguration)
            .WithItemReward(0, 13, this.Context, this.GameConfiguration, 8, 0, hasLuck: false, hasSkill: true);

        this.CreateQuest("Conquer the Dungeon! (6)", 15, 35, 37, 40, 141, 160, 257, CharacterClassNumber.SoulMaster)
            .WithMonsterKillRequirement(50, 8, this.Context, this.GameConfiguration)
            .WithMonsterKillRequirement(5, 10, this.Context, this.GameConfiguration)
            .WithMonsterKillRequirement(1, 18, this.Context, this.GameConfiguration)
            .WithItemReward(5, 5, this.Context, this.GameConfiguration, 7);
        this.CreateQuest("Conquer the Dungeon! (6)", 15, 35, 37, 40, 141, 160, 257, CharacterClassNumber.DarkWizard)
            .WithMonsterKillRequirement(50, 8, this.Context, this.GameConfiguration)
            .WithMonsterKillRequirement(5, 10, this.Context, this.GameConfiguration)
            .WithMonsterKillRequirement(1, 18, this.Context, this.GameConfiguration)
            .WithItemReward(5, 5, this.Context, this.GameConfiguration, 7);

        this.CreateQuest("Conquer the Dungeon! (6)", 15, 35, 38, 40, 141, 160, 257, CharacterClassNumber.MuseElf)
            .WithMonsterKillRequirement(50, 10, this.Context, this.GameConfiguration)
            .WithMonsterKillRequirement(30, 18, this.Context, this.GameConfiguration)
            .WithItemReward(4, 13, this.Context, this.GameConfiguration, 8, 0, hasLuck: false, hasSkill: true);
        this.CreateQuest("Conquer the Dungeon! (6)", 15, 35, 38, 40, 141, 160, 257, CharacterClassNumber.FairyElf)
            .WithMonsterKillRequirement(50, 10, this.Context, this.GameConfiguration)
            .WithMonsterKillRequirement(30, 18, this.Context, this.GameConfiguration)
            .WithItemReward(4, 13, this.Context, this.GameConfiguration, 8, 0, hasLuck: false, hasSkill: true);

        this.CreateQuest("Conquer the Dungeon! (6)", 15, 35, 39, 40, 141, 160, 257, CharacterClassNumber.BloodySummoner)
            .WithMonsterKillRequirement(50, 8, this.Context, this.GameConfiguration)
            .WithMonsterKillRequirement(5, 10, this.Context, this.GameConfiguration)
            .WithMonsterKillRequirement(1, 18, this.Context, this.GameConfiguration)
            .WithItemReward(5, 16, this.Context, this.GameConfiguration, 7);
        this.CreateQuest("Conquer the Dungeon! (6)", 15, 35, 39, 40, 141, 160, 257, CharacterClassNumber.Summoner)
            .WithMonsterKillRequirement(50, 8, this.Context, this.GameConfiguration)
            .WithMonsterKillRequirement(5, 10, this.Context, this.GameConfiguration)
            .WithMonsterKillRequirement(1, 18, this.Context, this.GameConfiguration)
            .WithItemReward(5, 16, this.Context, this.GameConfiguration, 7);

        this.CreateQuest("The mission of reconnaissance troops (1)", 15, 41, 42, 46, 161, 165, 257, CharacterClassNumber.BladeKnight)
            .WithMonsterKillRequirement(50, 36, this.Context, this.GameConfiguration)
            .WithMonsterKillRequirement(50, 39, this.Context, this.GameConfiguration)
            .WithItemReward(10, 34, this.Context, this.GameConfiguration, 7);
        this.CreateQuest("The mission of reconnaissance troops (1)", 15, 41, 42, 46, 161, 165, 257, CharacterClassNumber.DarkKnight)
            .WithMonsterKillRequirement(50, 36, this.Context, this.GameConfiguration)
            .WithMonsterKillRequirement(50, 39, this.Context, this.GameConfiguration)
            .WithItemReward(10, 34, this.Context, this.GameConfiguration, 7);

        this.CreateQuest("The mission of reconnaissance troops (1)", 15, 41, 43, 46, 161, 165, 257, CharacterClassNumber.SoulMaster)
            .WithMonsterKillRequirement(5, 25, this.Context, this.GameConfiguration)
            .WithItemReward(10, 35, this.Context, this.GameConfiguration, 7);
        this.CreateQuest("The mission of reconnaissance troops (1)", 15, 41, 43, 46, 161, 165, 257, CharacterClassNumber.DarkWizard)
            .WithMonsterKillRequirement(5, 25, this.Context, this.GameConfiguration)
            .WithItemReward(10, 35, this.Context, this.GameConfiguration, 7);

        this.CreateQuest("The mission of reconnaissance troops (1)", 15, 41, 44, 46, 161, 165, 257, CharacterClassNumber.MuseElf)
            .WithMonsterKillRequirement(50, 36, this.Context, this.GameConfiguration)
            .WithMonsterKillRequirement(50, 39, this.Context, this.GameConfiguration)
            .WithItemReward(10, 36, this.Context, this.GameConfiguration, 7);
        this.CreateQuest("The mission of reconnaissance troops (1)", 15, 41, 44, 46, 161, 165, 257, CharacterClassNumber.FairyElf)
            .WithMonsterKillRequirement(50, 36, this.Context, this.GameConfiguration)
            .WithMonsterKillRequirement(50, 39, this.Context, this.GameConfiguration)
            .WithItemReward(10, 36, this.Context, this.GameConfiguration, 7);

        this.CreateQuest("The mission of reconnaissance troops (1)", 15, 41, 45, 46, 161, 165, 257, CharacterClassNumber.BloodySummoner)
            .WithMonsterKillRequirement(5, 25, this.Context, this.GameConfiguration)
            .WithItemReward(10, 41, this.Context, this.GameConfiguration, 7);
        this.CreateQuest("The mission of reconnaissance troops (1)", 15, 41, 45, 46, 161, 165, 257, CharacterClassNumber.Summoner)
            .WithMonsterKillRequirement(5, 25, this.Context, this.GameConfiguration)
            .WithItemReward(10, 41, this.Context, this.GameConfiguration, 7);

        this.CreateQuest("The mission of reconnaissance troops (2)", 15, 47, 48, 52, 166, 170, 257, CharacterClassNumber.BladeKnight)
            .WithMonsterKillRequirement(50, 39, this.Context, this.GameConfiguration)
            .WithMonsterKillRequirement(30, 34, this.Context, this.GameConfiguration)
            .WithItemReward(11, 34, this.Context, this.GameConfiguration, 7);
        this.CreateQuest("The mission of reconnaissance troops (2)", 15, 47, 48, 52, 166, 170, 257, CharacterClassNumber.DarkKnight)
            .WithMonsterKillRequirement(50, 39, this.Context, this.GameConfiguration)
            .WithMonsterKillRequirement(30, 34, this.Context, this.GameConfiguration)
            .WithItemReward(11, 34, this.Context, this.GameConfiguration, 7);

        this.CreateQuest("The mission of reconnaissance troops (2)", 15, 47, 49, 52, 166, 170, 257, CharacterClassNumber.SoulMaster)
            .WithMonsterKillRequirement(100, 424, this.Context, this.GameConfiguration)
            .WithItemReward(11, 35, this.Context, this.GameConfiguration, 7);
        this.CreateQuest("The mission of reconnaissance troops (2)", 15, 47, 49, 52, 166, 170, 257, CharacterClassNumber.DarkWizard)
            .WithMonsterKillRequirement(100, 424, this.Context, this.GameConfiguration)
            .WithItemReward(11, 35, this.Context, this.GameConfiguration, 7);

        this.CreateQuest("The mission of reconnaissance troops (2)", 15, 47, 50, 52, 166, 170, 257, CharacterClassNumber.MuseElf)
            .WithMonsterKillRequirement(50, 39, this.Context, this.GameConfiguration)
            .WithMonsterKillRequirement(30, 34, this.Context, this.GameConfiguration)
            .WithItemReward(11, 36, this.Context, this.GameConfiguration, 7);
        this.CreateQuest("The mission of reconnaissance troops (2)", 15, 47, 50, 52, 166, 170, 257, CharacterClassNumber.FairyElf)
            .WithMonsterKillRequirement(50, 39, this.Context, this.GameConfiguration)
            .WithMonsterKillRequirement(30, 34, this.Context, this.GameConfiguration)
            .WithItemReward(11, 36, this.Context, this.GameConfiguration, 7);

        this.CreateQuest("The mission of reconnaissance troops (2)", 15, 47, 51, 52, 166, 170, 257, CharacterClassNumber.BloodySummoner)
            .WithMonsterKillRequirement(100, 424, this.Context, this.GameConfiguration)
            .WithItemReward(11, 41, this.Context, this.GameConfiguration, 7);
        this.CreateQuest("The mission of reconnaissance troops (2)", 15, 47, 51, 52, 166, 170, 257, CharacterClassNumber.Summoner)
            .WithMonsterKillRequirement(100, 424, this.Context, this.GameConfiguration)
            .WithItemReward(11, 41, this.Context, this.GameConfiguration, 7);

        this.CreateQuest("The mission of reconnaissance troops (3)", 15, 53, 54, 58, 171, 179, 257, CharacterClassNumber.BladeKnight)
            .WithMonsterKillRequirement(50, 34, this.Context, this.GameConfiguration)
            .WithMonsterKillRequirement(50, 41, this.Context, this.GameConfiguration)
            .WithItemReward(9, 34, this.Context, this.GameConfiguration, 7)
            .WithItemReward(7, 34, this.Context, this.GameConfiguration, 7);
        this.CreateQuest("The mission of reconnaissance troops (3)", 15, 53, 54, 58, 171, 179, 257, CharacterClassNumber.DarkKnight)
            .WithMonsterKillRequirement(50, 34, this.Context, this.GameConfiguration)
            .WithMonsterKillRequirement(50, 41, this.Context, this.GameConfiguration)
            .WithItemReward(9, 34, this.Context, this.GameConfiguration, 7)
            .WithItemReward(7, 34, this.Context, this.GameConfiguration, 7);

        this.CreateQuest("The mission of reconnaissance troops (3)", 15, 53, 55, 58, 171, 179, 257, CharacterClassNumber.SoulMaster)
            .WithMonsterKillRequirement(100, 425, this.Context, this.GameConfiguration)
            .WithItemReward(9, 35, this.Context, this.GameConfiguration, 7)
            .WithItemReward(7, 35, this.Context, this.GameConfiguration, 7);
        this.CreateQuest("The mission of reconnaissance troops (3)", 15, 53, 55, 58, 171, 179, 257, CharacterClassNumber.DarkWizard)
            .WithMonsterKillRequirement(100, 425, this.Context, this.GameConfiguration)
            .WithItemReward(9, 35, this.Context, this.GameConfiguration, 7)
            .WithItemReward(7, 35, this.Context, this.GameConfiguration, 7);

        this.CreateQuest("The mission of reconnaissance troops (3)", 15, 53, 56, 58, 171, 179, 257, CharacterClassNumber.MuseElf)
            .WithMonsterKillRequirement(50, 34, this.Context, this.GameConfiguration)
            .WithMonsterKillRequirement(50, 41, this.Context, this.GameConfiguration)
            .WithItemReward(9, 36, this.Context, this.GameConfiguration, 7)
            .WithItemReward(7, 36, this.Context, this.GameConfiguration, 7);
        this.CreateQuest("The mission of reconnaissance troops (3)", 15, 53, 56, 58, 171, 179, 257, CharacterClassNumber.FairyElf)
            .WithMonsterKillRequirement(50, 34, this.Context, this.GameConfiguration)
            .WithMonsterKillRequirement(50, 41, this.Context, this.GameConfiguration)
            .WithItemReward(9, 36, this.Context, this.GameConfiguration, 7)
            .WithItemReward(7, 36, this.Context, this.GameConfiguration, 7);

        this.CreateQuest("The mission of reconnaissance troops (3)", 15, 53, 57, 58, 171, 179, 257, CharacterClassNumber.BloodySummoner)
            .WithMonsterKillRequirement(100, 425, this.Context, this.GameConfiguration)
            .WithItemReward(9, 41, this.Context, this.GameConfiguration, 7)
            .WithItemReward(7, 41, this.Context, this.GameConfiguration, 7);
        this.CreateQuest("The mission of reconnaissance troops (3)", 15, 53, 57, 58, 171, 179, 257, CharacterClassNumber.Summoner)
            .WithMonsterKillRequirement(100, 425, this.Context, this.GameConfiguration)
            .WithItemReward(9, 41, this.Context, this.GameConfiguration, 7)
            .WithItemReward(7, 41, this.Context, this.GameConfiguration, 7);

        this.CreateQuest("The mission of reconnaissance troops (4)", 15, 59, 60, 64, 180, 185, 257, CharacterClassNumber.BladeKnight)
            .WithMonsterKillRequirement(100, 41, this.Context, this.GameConfiguration)
            .WithItemReward(8, 34, this.Context, this.GameConfiguration, 7)
            .WithItemReward(2, 6, this.Context, this.GameConfiguration, 4, 4);
        this.CreateQuest("The mission of reconnaissance troops (4)", 15, 59, 60, 64, 180, 185, 257, CharacterClassNumber.DarkKnight)
            .WithMonsterKillRequirement(100, 41, this.Context, this.GameConfiguration)
            .WithItemReward(8, 34, this.Context, this.GameConfiguration, 7)
            .WithItemReward(2, 6, this.Context, this.GameConfiguration, 4, 4);

        this.CreateQuest("The mission of reconnaissance troops (4)", 15, 59, 61, 64, 180, 185, 257, CharacterClassNumber.SoulMaster)
            .WithMonsterKillRequirement(100, 36, this.Context, this.GameConfiguration)
            .WithItemReward(8, 35, this.Context, this.GameConfiguration, 7)
            .WithItemReward(2, 6, this.Context, this.GameConfiguration, 4, 4);
        this.CreateQuest("The mission of reconnaissance troops (4)", 15, 59, 61, 64, 180, 185, 257, CharacterClassNumber.DarkWizard)
            .WithMonsterKillRequirement(100, 36, this.Context, this.GameConfiguration)
            .WithItemReward(8, 35, this.Context, this.GameConfiguration, 7)
            .WithItemReward(2, 6, this.Context, this.GameConfiguration, 4, 4);

        this.CreateQuest("The mission of reconnaissance troops (4)", 15, 59, 62, 64, 180, 185, 257, CharacterClassNumber.MuseElf)
            .WithMonsterKillRequirement(100, 41, this.Context, this.GameConfiguration)
            .WithItemReward(8, 36, this.Context, this.GameConfiguration, 7)
            .WithItemReward(2, 6, this.Context, this.GameConfiguration, 4, 4);
        this.CreateQuest("The mission of reconnaissance troops (4)", 15, 59, 62, 64, 180, 185, 257, CharacterClassNumber.FairyElf)
            .WithMonsterKillRequirement(100, 41, this.Context, this.GameConfiguration)
            .WithItemReward(8, 36, this.Context, this.GameConfiguration, 7)
            .WithItemReward(2, 6, this.Context, this.GameConfiguration, 4, 4);

        this.CreateQuest("The mission of reconnaissance troops (4)", 15, 59, 63, 64, 180, 185, 257, CharacterClassNumber.BloodySummoner)
            .WithMonsterKillRequirement(100, 36, this.Context, this.GameConfiguration)
            .WithItemReward(8, 41, this.Context, this.GameConfiguration, 7)
            .WithItemReward(2, 6, this.Context, this.GameConfiguration, 4, 4);
        this.CreateQuest("The mission of reconnaissance troops (4)", 15, 59, 63, 64, 180, 185, 257, CharacterClassNumber.Summoner)
            .WithMonsterKillRequirement(100, 36, this.Context, this.GameConfiguration)
            .WithItemReward(8, 41, this.Context, this.GameConfiguration, 7)
            .WithItemReward(2, 6, this.Context, this.GameConfiguration, 4, 4);

        this.CreateQuest("Put the Lost Tower to Sleep! (1)", 15, 65, 66, 70, 186, 189, 257, CharacterClassNumber.BladeKnight)
            .WithMonsterKillRequirement(50, 41, this.Context, this.GameConfiguration)
            .WithMonsterKillRequirement(30, 37, this.Context, this.GameConfiguration)
            .WithItemReward(10, 34, this.Context, this.GameConfiguration, 8);
        this.CreateQuest("Put the Lost Tower to Sleep! (1)", 15, 65, 66, 70, 186, 189, 257, CharacterClassNumber.DarkKnight)
            .WithMonsterKillRequirement(50, 41, this.Context, this.GameConfiguration)
            .WithMonsterKillRequirement(30, 37, this.Context, this.GameConfiguration)
            .WithItemReward(10, 34, this.Context, this.GameConfiguration, 8);

        this.CreateQuest("Put the Lost Tower to Sleep! (1)", 15, 65, 67, 70, 186, 189, 257, CharacterClassNumber.SoulMaster)
            .WithMonsterKillRequirement(50, 36, this.Context, this.GameConfiguration)
            .WithMonsterKillRequirement(30, 39, this.Context, this.GameConfiguration)
            .WithItemReward(10, 35, this.Context, this.GameConfiguration, 8);
        this.CreateQuest("Put the Lost Tower to Sleep! (1)", 15, 65, 67, 70, 186, 189, 257, CharacterClassNumber.DarkWizard)
            .WithMonsterKillRequirement(50, 36, this.Context, this.GameConfiguration)
            .WithMonsterKillRequirement(30, 39, this.Context, this.GameConfiguration)
            .WithItemReward(10, 35, this.Context, this.GameConfiguration, 8);

        this.CreateQuest("Put the Lost Tower to Sleep! (1)", 15, 65, 68, 70, 186, 189, 257, CharacterClassNumber.MuseElf)
            .WithMonsterKillRequirement(50, 41, this.Context, this.GameConfiguration)
            .WithMonsterKillRequirement(30, 37, this.Context, this.GameConfiguration)
            .WithItemReward(10, 36, this.Context, this.GameConfiguration, 8);
        this.CreateQuest("Put the Lost Tower to Sleep! (1)", 15, 65, 68, 70, 186, 189, 257, CharacterClassNumber.FairyElf)
            .WithMonsterKillRequirement(50, 41, this.Context, this.GameConfiguration)
            .WithMonsterKillRequirement(30, 37, this.Context, this.GameConfiguration)
            .WithItemReward(10, 36, this.Context, this.GameConfiguration, 8);

        this.CreateQuest("Put the Lost Tower to Sleep! (1)", 15, 65, 69, 70, 186, 189, 257, CharacterClassNumber.BloodySummoner)
            .WithMonsterKillRequirement(50, 36, this.Context, this.GameConfiguration)
            .WithMonsterKillRequirement(30, 39, this.Context, this.GameConfiguration)
            .WithItemReward(10, 41, this.Context, this.GameConfiguration, 8);
        this.CreateQuest("Put the Lost Tower to Sleep! (1)", 15, 65, 69, 70, 186, 189, 257, CharacterClassNumber.Summoner)
            .WithMonsterKillRequirement(50, 36, this.Context, this.GameConfiguration)
            .WithMonsterKillRequirement(30, 39, this.Context, this.GameConfiguration)
            .WithItemReward(10, 41, this.Context, this.GameConfiguration, 8);

        this.CreateQuest("Put the Lost Tower to Sleep! (2)", 15, 71, 72, 76, 190, 193, 257, CharacterClassNumber.BladeKnight)
            .WithMonsterKillRequirement(100, 37, this.Context, this.GameConfiguration)
            .WithItemReward(11, 34, this.Context, this.GameConfiguration, 8);
        this.CreateQuest("Put the Lost Tower to Sleep! (2)", 15, 71, 72, 76, 190, 193, 257, CharacterClassNumber.DarkKnight)
            .WithMonsterKillRequirement(100, 37, this.Context, this.GameConfiguration)
            .WithItemReward(11, 34, this.Context, this.GameConfiguration, 8);

        this.CreateQuest("Put the Lost Tower to Sleep! (2)", 15, 71, 73, 76, 190, 193, 257, CharacterClassNumber.SoulMaster)
            .WithMonsterKillRequirement(50, 39, this.Context, this.GameConfiguration)
            .WithMonsterKillRequirement(30, 34, this.Context, this.GameConfiguration)
            .WithItemReward(11, 35, this.Context, this.GameConfiguration, 8);
        this.CreateQuest("Put the Lost Tower to Sleep! (2)", 15, 71, 73, 76, 190, 193, 257, CharacterClassNumber.DarkWizard)
            .WithMonsterKillRequirement(50, 39, this.Context, this.GameConfiguration)
            .WithMonsterKillRequirement(30, 34, this.Context, this.GameConfiguration)
            .WithItemReward(11, 35, this.Context, this.GameConfiguration, 8);

        this.CreateQuest("Put the Lost Tower to Sleep! (2)", 15, 71, 74, 76, 190, 193, 257, CharacterClassNumber.MuseElf)
            .WithMonsterKillRequirement(100, 37, this.Context, this.GameConfiguration)
            .WithItemReward(11, 36, this.Context, this.GameConfiguration, 8);
        this.CreateQuest("Put the Lost Tower to Sleep! (2)", 15, 71, 74, 76, 190, 193, 257, CharacterClassNumber.FairyElf)
            .WithMonsterKillRequirement(100, 37, this.Context, this.GameConfiguration)
            .WithItemReward(11, 36, this.Context, this.GameConfiguration, 8);

        this.CreateQuest("Put the Lost Tower to Sleep! (2)", 15, 71, 75, 76, 190, 193, 257, CharacterClassNumber.BloodySummoner)
            .WithMonsterKillRequirement(50, 39, this.Context, this.GameConfiguration)
            .WithMonsterKillRequirement(30, 34, this.Context, this.GameConfiguration)
            .WithItemReward(11, 41, this.Context, this.GameConfiguration, 8);
        this.CreateQuest("Put the Lost Tower to Sleep! (2)", 15, 71, 75, 76, 190, 193, 257, CharacterClassNumber.Summoner)
            .WithMonsterKillRequirement(50, 39, this.Context, this.GameConfiguration)
            .WithMonsterKillRequirement(30, 34, this.Context, this.GameConfiguration)
            .WithItemReward(11, 41, this.Context, this.GameConfiguration, 8);

        this.CreateQuest("Put the Lost Tower to Sleep! (3)", 15, 77, 78, 82, 194, 197, 257, CharacterClassNumber.BladeKnight)
            .WithMonsterKillRequirement(100, 40, this.Context, this.GameConfiguration)
            .WithItemReward(7, 34, this.Context, this.GameConfiguration, 8);
        this.CreateQuest("Put the Lost Tower to Sleep! (3)", 15, 77, 78, 82, 194, 197, 257, CharacterClassNumber.DarkKnight)
            .WithMonsterKillRequirement(100, 40, this.Context, this.GameConfiguration)
            .WithItemReward(7, 34, this.Context, this.GameConfiguration, 8);

        this.CreateQuest("Put the Lost Tower to Sleep! (3)", 15, 77, 79, 82, 194, 197, 257, CharacterClassNumber.SoulMaster)
            .WithMonsterKillRequirement(50, 34, this.Context, this.GameConfiguration)
            .WithMonsterKillRequirement(50, 41, this.Context, this.GameConfiguration)
            .WithItemReward(7, 35, this.Context, this.GameConfiguration, 8);
        this.CreateQuest("Put the Lost Tower to Sleep! (3)", 15, 77, 79, 82, 194, 197, 257, CharacterClassNumber.DarkWizard)
            .WithMonsterKillRequirement(50, 34, this.Context, this.GameConfiguration)
            .WithMonsterKillRequirement(50, 41, this.Context, this.GameConfiguration)
            .WithItemReward(7, 35, this.Context, this.GameConfiguration, 8);

        this.CreateQuest("Put the Lost Tower to Sleep! (3)", 15, 77, 80, 82, 194, 197, 257, CharacterClassNumber.MuseElf)
            .WithMonsterKillRequirement(100, 40, this.Context, this.GameConfiguration)
            .WithItemReward(7, 36, this.Context, this.GameConfiguration, 8);
        this.CreateQuest("Put the Lost Tower to Sleep! (3)", 15, 77, 80, 82, 194, 197, 257, CharacterClassNumber.FairyElf)
            .WithMonsterKillRequirement(100, 40, this.Context, this.GameConfiguration)
            .WithItemReward(7, 36, this.Context, this.GameConfiguration, 8);

        this.CreateQuest("Put the Lost Tower to Sleep! (3)", 15, 77, 81, 82, 194, 197, 257, CharacterClassNumber.BloodySummoner)
            .WithMonsterKillRequirement(50, 34, this.Context, this.GameConfiguration)
            .WithMonsterKillRequirement(50, 41, this.Context, this.GameConfiguration)
            .WithItemReward(7, 41, this.Context, this.GameConfiguration, 8);
        this.CreateQuest("Put the Lost Tower to Sleep! (3)", 15, 77, 81, 82, 194, 197, 257, CharacterClassNumber.Summoner)
            .WithMonsterKillRequirement(50, 34, this.Context, this.GameConfiguration)
            .WithMonsterKillRequirement(50, 41, this.Context, this.GameConfiguration)
            .WithItemReward(7, 41, this.Context, this.GameConfiguration, 8);

        this.CreateQuest("Put the Lost Tower to Sleep! (4)", 15, 83, 84, 88, 198, 201, 257, CharacterClassNumber.BladeKnight)
            .WithMonsterKillRequirement(50, 40, this.Context, this.GameConfiguration)
            .WithMonsterKillRequirement(30, 35, this.Context, this.GameConfiguration)
            .WithItemReward(9, 34, this.Context, this.GameConfiguration, 8);
        this.CreateQuest("Put the Lost Tower to Sleep! (4)", 15, 83, 84, 88, 198, 201, 257, CharacterClassNumber.DarkKnight)
            .WithMonsterKillRequirement(50, 40, this.Context, this.GameConfiguration)
            .WithMonsterKillRequirement(30, 35, this.Context, this.GameConfiguration)
            .WithItemReward(9, 34, this.Context, this.GameConfiguration, 8);

        this.CreateQuest("Put the Lost Tower to Sleep! (4)", 15, 83, 85, 88, 198, 201, 257, CharacterClassNumber.SoulMaster)
            .WithMonsterKillRequirement(100, 41, this.Context, this.GameConfiguration)
            .WithItemReward(9, 35, this.Context, this.GameConfiguration, 8);
        this.CreateQuest("Put the Lost Tower to Sleep! (4)", 15, 83, 85, 88, 198, 201, 257, CharacterClassNumber.DarkWizard)
            .WithMonsterKillRequirement(100, 41, this.Context, this.GameConfiguration)
            .WithItemReward(9, 35, this.Context, this.GameConfiguration, 8);

        this.CreateQuest("Put the Lost Tower to Sleep! (4)", 15, 83, 86, 88, 198, 201, 257, CharacterClassNumber.MuseElf)
            .WithMonsterKillRequirement(50, 40, this.Context, this.GameConfiguration)
            .WithMonsterKillRequirement(30, 35, this.Context, this.GameConfiguration)
            .WithItemReward(9, 36, this.Context, this.GameConfiguration, 8);
        this.CreateQuest("Put the Lost Tower to Sleep! (4)", 15, 83, 86, 88, 198, 201, 257, CharacterClassNumber.FairyElf)
            .WithMonsterKillRequirement(50, 40, this.Context, this.GameConfiguration)
            .WithMonsterKillRequirement(30, 35, this.Context, this.GameConfiguration)
            .WithItemReward(9, 36, this.Context, this.GameConfiguration, 8);

        this.CreateQuest("Put the Lost Tower to Sleep! (4)", 15, 83, 87, 88, 198, 201, 257, CharacterClassNumber.BloodySummoner)
            .WithMonsterKillRequirement(100, 41, this.Context, this.GameConfiguration)
            .WithItemReward(9, 41, this.Context, this.GameConfiguration, 8);
        this.CreateQuest("Put the Lost Tower to Sleep! (4)", 15, 83, 87, 88, 198, 201, 257, CharacterClassNumber.Summoner)
            .WithMonsterKillRequirement(100, 41, this.Context, this.GameConfiguration)
            .WithItemReward(9, 41, this.Context, this.GameConfiguration, 8);

        this.CreateQuest("Put the Lost Tower to Sleep! (5)", 15, 89, 90, 94, 202, 205, 257, CharacterClassNumber.BladeKnight)
            .WithMonsterKillRequirement(50, 35, this.Context, this.GameConfiguration)
            .WithMonsterKillRequirement(10, 38, this.Context, this.GameConfiguration)
            .WithItemReward(8, 34, this.Context, this.GameConfiguration, 8);
        this.CreateQuest("Put the Lost Tower to Sleep! (5)", 15, 89, 90, 94, 202, 205, 257, CharacterClassNumber.DarkKnight)
            .WithMonsterKillRequirement(50, 35, this.Context, this.GameConfiguration)
            .WithMonsterKillRequirement(10, 38, this.Context, this.GameConfiguration)
            .WithItemReward(8, 34, this.Context, this.GameConfiguration, 8);

        this.CreateQuest("Put the Lost Tower to Sleep! (5)", 15, 89, 91, 94, 202, 205, 257, CharacterClassNumber.SoulMaster)
            .WithMonsterKillRequirement(100, 37, this.Context, this.GameConfiguration)
            .WithItemReward(8, 35, this.Context, this.GameConfiguration, 8);
        this.CreateQuest("Put the Lost Tower to Sleep! (5)", 15, 89, 91, 94, 202, 205, 257, CharacterClassNumber.DarkWizard)
            .WithMonsterKillRequirement(100, 37, this.Context, this.GameConfiguration)
            .WithItemReward(8, 35, this.Context, this.GameConfiguration, 8);

        this.CreateQuest("Put the Lost Tower to Sleep! (5)", 15, 89, 92, 94, 202, 205, 257, CharacterClassNumber.MuseElf)
            .WithMonsterKillRequirement(50, 35, this.Context, this.GameConfiguration)
            .WithMonsterKillRequirement(10, 38, this.Context, this.GameConfiguration)
            .WithItemReward(8, 36, this.Context, this.GameConfiguration, 8);
        this.CreateQuest("Put the Lost Tower to Sleep! (5)", 15, 89, 92, 94, 202, 205, 257, CharacterClassNumber.FairyElf)
            .WithMonsterKillRequirement(50, 35, this.Context, this.GameConfiguration)
            .WithMonsterKillRequirement(10, 38, this.Context, this.GameConfiguration)
            .WithItemReward(8, 36, this.Context, this.GameConfiguration, 8);

        this.CreateQuest("Put the Lost Tower to Sleep! (5)", 15, 89, 93, 94, 202, 205, 257, CharacterClassNumber.BloodySummoner)
            .WithMonsterKillRequirement(100, 37, this.Context, this.GameConfiguration)
            .WithItemReward(8, 41, this.Context, this.GameConfiguration, 8);
        this.CreateQuest("Put the Lost Tower to Sleep! (5)", 15, 89, 93, 94, 202, 205, 257, CharacterClassNumber.Summoner)
            .WithMonsterKillRequirement(100, 37, this.Context, this.GameConfiguration)
            .WithItemReward(8, 41, this.Context, this.GameConfiguration, 8);

        this.CreateQuest("Put the Lost Tower to Sleep! (6)", 15, 95, 96, 100, 206, 209, 257, CharacterClassNumber.BladeKnight)
            .WithMonsterKillRequirement(30, 35, this.Context, this.GameConfiguration)
            .WithMonsterKillRequirement(20, 38, this.Context, this.GameConfiguration)
            .WithItemReward(2, 5, this.Context, this.GameConfiguration, 7, 0, hasLuck: false, hasSkill: true);
        this.CreateQuest("Put the Lost Tower to Sleep! (6)", 15, 95, 96, 100, 206, 209, 257, CharacterClassNumber.DarkKnight)
            .WithMonsterKillRequirement(30, 35, this.Context, this.GameConfiguration)
            .WithMonsterKillRequirement(20, 38, this.Context, this.GameConfiguration)
            .WithItemReward(2, 5, this.Context, this.GameConfiguration, 7, 0, hasLuck: false, hasSkill: true);

        this.CreateQuest("Put the Lost Tower to Sleep! (6)", 15, 95, 97, 100, 206, 209, 257, CharacterClassNumber.SoulMaster)
            .WithMonsterKillRequirement(100, 40, this.Context, this.GameConfiguration)
            .WithItemReward(5, 5, this.Context, this.GameConfiguration, 8);
        this.CreateQuest("Put the Lost Tower to Sleep! (6)", 15, 95, 97, 100, 206, 209, 257, CharacterClassNumber.DarkWizard)
            .WithMonsterKillRequirement(100, 40, this.Context, this.GameConfiguration)
            .WithItemReward(5, 5, this.Context, this.GameConfiguration, 8);

        this.CreateQuest("Put the Lost Tower to Sleep! (6)", 15, 95, 98, 100, 206, 209, 257, CharacterClassNumber.MuseElf)
            .WithMonsterKillRequirement(30, 35, this.Context, this.GameConfiguration)
            .WithMonsterKillRequirement(20, 38, this.Context, this.GameConfiguration)
            .WithItemReward(4, 14, this.Context, this.GameConfiguration, 7, 0, hasLuck: false, hasSkill: true);
        this.CreateQuest("Put the Lost Tower to Sleep! (6)", 15, 95, 98, 100, 206, 209, 257, CharacterClassNumber.FairyElf)
            .WithMonsterKillRequirement(30, 35, this.Context, this.GameConfiguration)
            .WithMonsterKillRequirement(20, 38, this.Context, this.GameConfiguration)
            .WithItemReward(4, 14, this.Context, this.GameConfiguration, 7, 0, hasLuck: false, hasSkill: true);

        this.CreateQuest("Put the Lost Tower to Sleep! (6)", 15, 95, 99, 100, 206, 209, 257, CharacterClassNumber.BloodySummoner)
            .WithMonsterKillRequirement(100, 40, this.Context, this.GameConfiguration)
            .WithItemReward(5, 16, this.Context, this.GameConfiguration, 8);
        this.CreateQuest("Put the Lost Tower to Sleep! (6)", 15, 95, 99, 100, 206, 209, 257, CharacterClassNumber.Summoner)
            .WithMonsterKillRequirement(100, 40, this.Context, this.GameConfiguration)
            .WithItemReward(5, 16, this.Context, this.GameConfiguration, 8);

        this.CreateQuest("Random Quest (1)", 19, 3, 5, 4, 260, 289, 257)
            .WithMonsterKillRequirement(30, 57, this.Context, this.GameConfiguration)
            .WithExperienceReward(50000, this.Context)
            .WithItemReward(5, 17, this.Context, this.GameConfiguration, 4, 12);

        this.CreateQuest("Random Quest (1)", 19, 3, 6, 4, 260, 289, 257)
            .WithMonsterKillRequirement(30, 57, this.Context, this.GameConfiguration)
            .WithItemReward(2, 6, this.Context, this.GameConfiguration, 4, 12, hasLuck: false, hasSkill: true)
            .WithMoneyReward(100000, this.Context);

        this.CreateQuest("Random Quest (1)", 19, 3, 7, 4, 260, 289, 257)
            .WithMonsterKillRequirement(30, 57, this.Context, this.GameConfiguration)
            .WithItemReward(14, 14, this.Context, this.GameConfiguration);

        this.CreateQuest("Random Quest (1)", 19, 3, 8, 4, 260, 289, 257)
            .WithMonsterKillRequirement(30, 57, this.Context, this.GameConfiguration)
            .WithItemReward(13, 1, this.Context, this.GameConfiguration)
            .WithItemReward(14, 19, this.Context, this.GameConfiguration, 4);

        this.CreateQuest("Random Quest (1)", 19, 3, 9, 4, 260, 289, 257)
            .WithMonsterKillRequirement(30, 57, this.Context, this.GameConfiguration)
            .WithItemReward(12, 15, this.Context, this.GameConfiguration);

        this.CreateQuest("Random Quest (1)", 19, 3, 10, 4, 260, 289, 257)
            .WithMonsterKillRequirement(30, 57, this.Context, this.GameConfiguration)
            .WithItemReward(13, 0, this.Context, this.GameConfiguration);

        this.CreateQuest("Random Quest (1)", 19, 3, 11, 4, 260, 289, 257)
            .WithMonsterKillRequirement(30, 57, this.Context, this.GameConfiguration)
            .WithItemReward(4, 16, this.Context, this.GameConfiguration, 4, 12, hasLuck: false, hasSkill: true);

        this.CreateQuest("Random Quest (1)", 19, 3, 12, 4, 260, 289, 257)
            .WithMonsterKillRequirement(30, 57, this.Context, this.GameConfiguration)
            .WithMoneyReward(200000, this.Context)
            .WithItemReward(13, 18, this.Context, this.GameConfiguration, 6);

        this.CreateQuest("Random Quest (1)", 19, 3, 13, 4, 260, 289, 257)
            .WithMonsterKillRequirement(20, 62, this.Context, this.GameConfiguration)
            .WithMonsterKillRequirement(20, 58, this.Context, this.GameConfiguration)
            .WithItemReward(2, 10, this.Context, this.GameConfiguration, 4, 12, hasLuck: false, hasSkill: true);

        this.CreateQuest("Random Quest (1)", 19, 3, 14, 4, 260, 289, 257)
            .WithMonsterKillRequirement(20, 62, this.Context, this.GameConfiguration)
            .WithMonsterKillRequirement(20, 58, this.Context, this.GameConfiguration)
            .WithItemReward(5, 9, this.Context, this.GameConfiguration, 4, 12);

        this.CreateQuest("Random Quest (1)", 19, 3, 15, 4, 260, 289, 257)
            .WithMonsterKillRequirement(20, 62, this.Context, this.GameConfiguration)
            .WithMonsterKillRequirement(20, 58, this.Context, this.GameConfiguration)
            .WithItemReward(13, 3, this.Context, this.GameConfiguration, 0, 0, hasLuck: false, hasSkill: true);

        this.CreateQuest("Random Quest (1)", 19, 3, 16, 4, 260, 289, 257)
            .WithMonsterKillRequirement(20, 62, this.Context, this.GameConfiguration)
            .WithMonsterKillRequirement(20, 58, this.Context, this.GameConfiguration)
            .WithItemReward(2, 6, this.Context, this.GameConfiguration, 4, 12, hasLuck: false, hasSkill: true)
            .WithExperienceReward(120000, this.Context);

        this.CreateQuest("Random Quest (1)", 19, 3, 17, 4, 260, 289, 257)
            .WithMonsterKillRequirement(20, 62, this.Context, this.GameConfiguration)
            .WithMonsterKillRequirement(20, 58, this.Context, this.GameConfiguration)
            .WithItemReward(14, 14, this.Context, this.GameConfiguration);

        this.CreateQuest("Random Quest (1)", 19, 3, 18, 4, 260, 289, 257)
            .WithMonsterKillRequirement(20, 62, this.Context, this.GameConfiguration)
            .WithMonsterKillRequirement(20, 58, this.Context, this.GameConfiguration)
            .WithItemReward(5, 17, this.Context, this.GameConfiguration, 4, 12);

        this.CreateQuest("Random Quest (1)", 19, 3, 19, 4, 260, 289, 257)
            .WithMonsterKillRequirement(20, 62, this.Context, this.GameConfiguration)
            .WithMonsterKillRequirement(20, 58, this.Context, this.GameConfiguration)
            .WithItemReward(14, 41, this.Context, this.GameConfiguration);

        this.CreateQuest("Random Quest (1)", 19, 3, 20, 4, 260, 289, 257)
            .WithMonsterKillRequirement(20, 62, this.Context, this.GameConfiguration)
            .WithMonsterKillRequirement(20, 58, this.Context, this.GameConfiguration)
            .WithItemReward(3, 10, this.Context, this.GameConfiguration, 4, 12, hasLuck: false, hasSkill: true);

        this.CreateQuest("Random Quest (1)", 19, 3, 21, 4, 260, 289, 257)
            .WithMonsterKillRequirement(20, 62, this.Context, this.GameConfiguration)
            .WithMonsterKillRequirement(10, 61, this.Context, this.GameConfiguration)
            .WithItemReward(14, 42, this.Context, this.GameConfiguration);

        this.CreateQuest("Random Quest (1)", 19, 3, 22, 4, 260, 289, 257)
            .WithMonsterKillRequirement(20, 62, this.Context, this.GameConfiguration)
            .WithMonsterKillRequirement(10, 61, this.Context, this.GameConfiguration)
            .WithItemReward(13, 18, this.Context, this.GameConfiguration, 5);

        this.CreateQuest("Random Quest (1)", 19, 3, 23, 4, 260, 289, 257)
            .WithMonsterKillRequirement(20, 62, this.Context, this.GameConfiguration)
            .WithMonsterKillRequirement(10, 61, this.Context, this.GameConfiguration)
            .WithItemReward(2, 10, this.Context, this.GameConfiguration, 4, 12, hasLuck: false, hasSkill: true);

        this.CreateQuest("Random Quest (1)", 19, 3, 24, 4, 260, 289, 257)
            .WithMonsterKillRequirement(20, 62, this.Context, this.GameConfiguration)
            .WithMonsterKillRequirement(10, 61, this.Context, this.GameConfiguration)
            .WithItemReward(13, 1, this.Context, this.GameConfiguration)
            .WithItemReward(12, 15, this.Context, this.GameConfiguration);

        this.CreateQuest("Random Quest (1)", 19, 3, 25, 4, 260, 289, 257)
            .WithMonsterKillRequirement(20, 62, this.Context, this.GameConfiguration)
            .WithMonsterKillRequirement(10, 61, this.Context, this.GameConfiguration)
            .WithItemReward(14, 14, this.Context, this.GameConfiguration);

        this.CreateQuest("Random Quest (1)", 19, 3, 26, 4, 260, 289, 257)
            .WithMonsterKillRequirement(20, 62, this.Context, this.GameConfiguration)
            .WithMonsterKillRequirement(10, 61, this.Context, this.GameConfiguration)
            .WithItemReward(5, 9, this.Context, this.GameConfiguration, 4, 12);

        this.CreateQuest("Random Quest (1)", 19, 3, 27, 4, 260, 289, 257)
            .WithMonsterKillRequirement(20, 62, this.Context, this.GameConfiguration)
            .WithMonsterKillRequirement(10, 61, this.Context, this.GameConfiguration)
            .WithExperienceReward(50000, this.Context)
            .WithItemReward(13, 18, this.Context, this.GameConfiguration, 6);

        this.CreateQuest("Random Quest (1)", 19, 3, 28, 4, 260, 289, 257)
            .WithMonsterKillRequirement(20, 62, this.Context, this.GameConfiguration)
            .WithMonsterKillRequirement(10, 61, this.Context, this.GameConfiguration)
            .WithItemReward(14, 42, this.Context, this.GameConfiguration);

        this.CreateQuest("Random Quest (1)", 19, 3, 29, 4, 260, 289, 257)
            .WithMonsterKillRequirement(20, 62, this.Context, this.GameConfiguration)
            .WithItemRequirement(5, 6, this.Context, this.GameConfiguration, itemOption: 12)
            .WithItemReward(14, 41, this.Context, this.GameConfiguration);

        this.CreateQuest("Random Quest (1)", 19, 3, 30, 4, 260, 289, 257)
            .WithMonsterKillRequirement(20, 62, this.Context, this.GameConfiguration)
            .WithItemRequirement(5, 6, this.Context, this.GameConfiguration, itemOption: 12)
            .WithItemReward(14, 14, this.Context, this.GameConfiguration);

        this.CreateQuest("Random Quest (1)", 19, 3, 31, 4, 260, 289, 257)
            .WithMonsterKillRequirement(20, 62, this.Context, this.GameConfiguration)
            .WithItemRequirement(5, 6, this.Context, this.GameConfiguration, itemOption: 12)
            .WithItemReward(3, 10, this.Context, this.GameConfiguration, 4, 12, hasLuck: false, hasSkill: true);

        this.CreateQuest("Random Quest (1)", 19, 3, 32, 4, 260, 289, 257)
            .WithMonsterKillRequirement(20, 62, this.Context, this.GameConfiguration)
            .WithItemRequirement(5, 6, this.Context, this.GameConfiguration, itemOption: 12)
            .WithItemReward(14, 19, this.Context, this.GameConfiguration, 5);

        this.CreateQuest("Random Quest (1)", 19, 3, 33, 4, 260, 289, 257)
            .WithMonsterKillRequirement(20, 62, this.Context, this.GameConfiguration)
            .WithItemRequirement(5, 6, this.Context, this.GameConfiguration, itemOption: 12)
            .WithItemReward(13, 0, this.Context, this.GameConfiguration);

        this.CreateQuest("Random Quest (1)", 19, 3, 34, 4, 260, 289, 257)
            .WithMonsterKillRequirement(20, 62, this.Context, this.GameConfiguration)
            .WithItemRequirement(5, 6, this.Context, this.GameConfiguration, itemOption: 12)
            .WithItemReward(14, 19, this.Context, this.GameConfiguration, 4);

        this.CreateQuest("Random Quest (1)", 19, 3, 35, 4, 260, 289, 257)
            .WithMonsterKillRequirement(20, 62, this.Context, this.GameConfiguration)
            .WithItemRequirement(5, 6, this.Context, this.GameConfiguration, itemOption: 12)
            .WithItemReward(4, 16, this.Context, this.GameConfiguration, 4, 12, hasLuck: false, hasSkill: true)
            .WithItemReward(13, 3, this.Context, this.GameConfiguration, 0, 0, hasLuck: false, hasSkill: true);

        this.CreateQuest("Random Quest (1)", 19, 3, 36, 4, 260, 289, 257)
            .WithMonsterKillRequirement(20, 62, this.Context, this.GameConfiguration)
            .WithItemRequirement(5, 6, this.Context, this.GameConfiguration, itemOption: 12)
            .WithItemReward(14, 13, this.Context, this.GameConfiguration);

        this.CreateQuest("Random Quest (1)", 19, 3, 37, 4, 260, 289, 257)
            .WithMonsterKillRequirement(20, 62, this.Context, this.GameConfiguration)
            .WithItemRequirement(13, 2, this.Context, this.GameConfiguration, itemCount: 10)
            .WithItemReward(5, 17, this.Context, this.GameConfiguration, 4, 12);

        this.CreateQuest("Random Quest (1)", 19, 3, 38, 4, 260, 289, 257)
            .WithMonsterKillRequirement(20, 62, this.Context, this.GameConfiguration)
            .WithItemRequirement(13, 2, this.Context, this.GameConfiguration, itemCount: 10)
            .WithItemReward(2, 6, this.Context, this.GameConfiguration, 4, 12, hasLuck: false, hasSkill: true);

        this.CreateQuest("Random Quest (1)", 19, 3, 39, 4, 260, 289, 257)
            .WithMonsterKillRequirement(20, 62, this.Context, this.GameConfiguration)
            .WithItemRequirement(13, 2, this.Context, this.GameConfiguration, itemCount: 10)
            .WithExperienceReward(50000, this.Context)
            .WithItemReward(4, 16, this.Context, this.GameConfiguration, 4, 12, hasLuck: false, hasSkill: true);

        this.CreateQuest("Random Quest (1)", 19, 3, 40, 4, 260, 289, 257)
            .WithMonsterKillRequirement(20, 62, this.Context, this.GameConfiguration)
            .WithItemRequirement(13, 2, this.Context, this.GameConfiguration, itemCount: 10)
            .WithItemReward(14, 19, this.Context, this.GameConfiguration, 5)
            .WithItemReward(14, 14, this.Context, this.GameConfiguration);

        this.CreateQuest("Random Quest (1)", 19, 3, 41, 4, 260, 289, 257)
            .WithMonsterKillRequirement(20, 62, this.Context, this.GameConfiguration)
            .WithItemRequirement(13, 2, this.Context, this.GameConfiguration, itemCount: 10);

        this.CreateQuest("Random Quest (1)", 19, 3, 42, 4, 260, 289, 257)
            .WithMonsterKillRequirement(20, 62, this.Context, this.GameConfiguration)
            .WithItemRequirement(13, 2, this.Context, this.GameConfiguration, itemCount: 10)
            .WithItemReward(14, 42, this.Context, this.GameConfiguration);

        this.CreateQuest("Random Quest (1)", 19, 3, 43, 4, 260, 289, 257)
            .WithMonsterKillRequirement(20, 62, this.Context, this.GameConfiguration)
            .WithItemRequirement(13, 2, this.Context, this.GameConfiguration, itemCount: 10)
            .WithItemReward(14, 41, this.Context, this.GameConfiguration);

        this.CreateQuest("Random Quest (1)", 19, 3, 44, 4, 260, 289, 257)
            .WithMonsterKillRequirement(20, 62, this.Context, this.GameConfiguration)
            .WithItemRequirement(13, 2, this.Context, this.GameConfiguration, itemCount: 10)
            .WithItemReward(5, 9, this.Context, this.GameConfiguration, 4, 12)
            .WithItemReward(2, 6, this.Context, this.GameConfiguration, 4, 12, hasLuck: false, hasSkill: true);

        this.CreateQuest("Random Quest (2)", 19, 48, 50, 49, 290, 319, 257)
            .WithMonsterKillRequirement(30, 70, this.Context, this.GameConfiguration)
            .WithItemReward(5, 17, this.Context, this.GameConfiguration, 6, 12);

        this.CreateQuest("Random Quest (2)", 19, 48, 51, 49, 290, 319, 257)
            .WithMonsterKillRequirement(30, 70, this.Context, this.GameConfiguration)
            .WithItemReward(14, 42, this.Context, this.GameConfiguration);

        this.CreateQuest("Random Quest (2)", 19, 48, 52, 49, 290, 319, 257)
            .WithMonsterKillRequirement(30, 70, this.Context, this.GameConfiguration)
            .WithItemReward(2, 10, this.Context, this.GameConfiguration, 6, 12, hasLuck: false, hasSkill: true);

        this.CreateQuest("Random Quest (2)", 19, 48, 53, 49, 290, 319, 257)
            .WithMonsterKillRequirement(30, 70, this.Context, this.GameConfiguration)
            .WithItemReward(14, 19, this.Context, this.GameConfiguration, 5)
            .WithExperienceReward(140000, this.Context);

        this.CreateQuest("Random Quest (2)", 19, 48, 54, 49, 290, 319, 257)
            .WithMonsterKillRequirement(30, 70, this.Context, this.GameConfiguration)
            .WithMoneyReward(200000, this.Context)
            .WithItemReward(14, 19, this.Context, this.GameConfiguration, 6);

        this.CreateQuest("Random Quest (2)", 19, 48, 55, 49, 290, 319, 257)
            .WithMonsterKillRequirement(30, 70, this.Context, this.GameConfiguration)
            .WithItemReward(14, 13, this.Context, this.GameConfiguration)
            .WithItemReward(13, 0, this.Context, this.GameConfiguration);

        this.CreateQuest("Random Quest (2)", 19, 48, 56, 49, 290, 319, 257)
            .WithMonsterKillRequirement(30, 70, this.Context, this.GameConfiguration)
            .WithItemReward(13, 1, this.Context, this.GameConfiguration)
            .WithExperienceReward(70000, this.Context);

        this.CreateQuest("Random Quest (2)", 19, 48, 57, 49, 290, 319, 257)
            .WithMonsterKillRequirement(30, 70, this.Context, this.GameConfiguration)
            .WithItemReward(14, 13, this.Context, this.GameConfiguration);

        this.CreateQuest("Random Quest (2)", 19, 48, 58, 49, 290, 319, 257)
            .WithMonsterKillRequirement(20, 69, this.Context, this.GameConfiguration)
            .WithMonsterKillRequirement(20, 73, this.Context, this.GameConfiguration)
            .WithItemReward(4, 16, this.Context, this.GameConfiguration, 6, 12, hasLuck: false, hasSkill: true);

        this.CreateQuest("Random Quest (2)", 19, 48, 59, 49, 290, 319, 257)
            .WithMonsterKillRequirement(20, 69, this.Context, this.GameConfiguration)
            .WithMonsterKillRequirement(20, 73, this.Context, this.GameConfiguration)
            .WithItemReward(12, 15, this.Context, this.GameConfiguration)
            .WithItemReward(13, 3, this.Context, this.GameConfiguration, 0, 0, hasLuck: false, hasSkill: true);

        this.CreateQuest("Random Quest (2)", 19, 48, 60, 49, 290, 319, 257)
            .WithMonsterKillRequirement(20, 69, this.Context, this.GameConfiguration)
            .WithMonsterKillRequirement(20, 73, this.Context, this.GameConfiguration)
            .WithItemReward(13, 18, this.Context, this.GameConfiguration, 6);

        this.CreateQuest("Random Quest (2)", 19, 48, 61, 49, 290, 319, 257)
            .WithMonsterKillRequirement(20, 69, this.Context, this.GameConfiguration)
            .WithMonsterKillRequirement(20, 73, this.Context, this.GameConfiguration)
            .WithItemReward(14, 14, this.Context, this.GameConfiguration);

        this.CreateQuest("Random Quest (2)", 19, 48, 62, 49, 290, 319, 257)
            .WithMonsterKillRequirement(20, 69, this.Context, this.GameConfiguration)
            .WithMonsterKillRequirement(20, 73, this.Context, this.GameConfiguration)
            .WithItemReward(3, 10, this.Context, this.GameConfiguration, 6, 12, hasLuck: false, hasSkill: true);

        this.CreateQuest("Random Quest (2)", 19, 48, 63, 49, 290, 319, 257)
            .WithMonsterKillRequirement(20, 69, this.Context, this.GameConfiguration)
            .WithMonsterKillRequirement(20, 73, this.Context, this.GameConfiguration)
            .WithItemReward(14, 41, this.Context, this.GameConfiguration)
            .WithItemReward(13, 18, this.Context, this.GameConfiguration, 7);

        this.CreateQuest("Random Quest (2)", 19, 48, 64, 49, 290, 319, 257)
            .WithMonsterKillRequirement(20, 69, this.Context, this.GameConfiguration)
            .WithMonsterKillRequirement(20, 73, this.Context, this.GameConfiguration)
            .WithItemReward(14, 16, this.Context, this.GameConfiguration)
            .WithExperienceReward(140000, this.Context);

        this.CreateQuest("Random Quest (2)", 19, 48, 65, 49, 290, 319, 257)
            .WithMonsterKillRequirement(20, 69, this.Context, this.GameConfiguration)
            .WithMonsterKillRequirement(20, 73, this.Context, this.GameConfiguration)
            .WithItemReward(13, 18, this.Context, this.GameConfiguration, 6)
            .WithItemReward(13, 3, this.Context, this.GameConfiguration, 0, 0, hasLuck: false, hasSkill: true);

        this.CreateQuest("Random Quest (2)", 19, 48, 66, 49, 290, 319, 257)
            .WithMonsterKillRequirement(20, 69, this.Context, this.GameConfiguration)
            .WithMonsterKillRequirement(10, 74, this.Context, this.GameConfiguration)
            .WithItemReward(5, 9, this.Context, this.GameConfiguration, 6, 12);

        this.CreateQuest("Random Quest (2)", 19, 48, 67, 49, 290, 319, 257)
            .WithMonsterKillRequirement(20, 69, this.Context, this.GameConfiguration)
            .WithMonsterKillRequirement(10, 74, this.Context, this.GameConfiguration)
            .WithItemReward(4, 16, this.Context, this.GameConfiguration, 6, 12, hasLuck: false, hasSkill: true);

        this.CreateQuest("Random Quest (2)", 19, 48, 68, 49, 290, 319, 257)
            .WithMonsterKillRequirement(20, 69, this.Context, this.GameConfiguration)
            .WithMonsterKillRequirement(10, 74, this.Context, this.GameConfiguration)
            .WithExperienceReward(70000, this.Context)
            .WithItemReward(13, 18, this.Context, this.GameConfiguration, 7);

        this.CreateQuest("Random Quest (2)", 19, 48, 69, 49, 290, 319, 257)
            .WithMonsterKillRequirement(20, 69, this.Context, this.GameConfiguration)
            .WithMonsterKillRequirement(10, 74, this.Context, this.GameConfiguration)
            .WithMoneyReward(200000, this.Context)
            .WithItemReward(14, 41, this.Context, this.GameConfiguration);

        this.CreateQuest("Random Quest (2)", 19, 48, 70, 49, 290, 319, 257)
            .WithMonsterKillRequirement(20, 69, this.Context, this.GameConfiguration)
            .WithMonsterKillRequirement(10, 74, this.Context, this.GameConfiguration)
            .WithItemReward(3, 10, this.Context, this.GameConfiguration, 6, 12, hasLuck: false, hasSkill: true);

        this.CreateQuest("Random Quest (2)", 19, 48, 71, 49, 290, 319, 257)
            .WithMonsterKillRequirement(20, 69, this.Context, this.GameConfiguration)
            .WithMonsterKillRequirement(10, 74, this.Context, this.GameConfiguration)
            .WithItemReward(5, 17, this.Context, this.GameConfiguration, 6, 12);

        this.CreateQuest("Random Quest (2)", 19, 48, 72, 49, 290, 319, 257)
            .WithMonsterKillRequirement(20, 69, this.Context, this.GameConfiguration)
            .WithMonsterKillRequirement(10, 74, this.Context, this.GameConfiguration)
            .WithItemReward(14, 19, this.Context, this.GameConfiguration, 6)
            .WithItemReward(13, 0, this.Context, this.GameConfiguration);

        this.CreateQuest("Random Quest (2)", 19, 48, 73, 49, 290, 319, 257)
            .WithMonsterKillRequirement(20, 69, this.Context, this.GameConfiguration)
            .WithMonsterKillRequirement(10, 74, this.Context, this.GameConfiguration)
            .WithItemReward(14, 13, this.Context, this.GameConfiguration);

        this.CreateQuest("Random Quest (2)", 19, 48, 74, 49, 290, 319, 257)
            .WithMonsterKillRequirement(20, 69, this.Context, this.GameConfiguration)
            .WithItemRequirement(0, 16, this.Context, this.GameConfiguration, itemOption: 12)
            .WithItemReward(5, 9, this.Context, this.GameConfiguration, 6, 12);

        this.CreateQuest("Random Quest (2)", 19, 48, 75, 49, 290, 319, 257)
            .WithMonsterKillRequirement(20, 69, this.Context, this.GameConfiguration)
            .WithItemRequirement(0, 16, this.Context, this.GameConfiguration, itemOption: 12)
            .WithItemReward(2, 10, this.Context, this.GameConfiguration, 6, 12, hasLuck: false, hasSkill: true);

        this.CreateQuest("Random Quest (2)", 19, 48, 76, 49, 290, 319, 257)
            .WithMonsterKillRequirement(20, 69, this.Context, this.GameConfiguration)
            .WithItemRequirement(0, 16, this.Context, this.GameConfiguration, itemOption: 12)
            .WithItemReward(13, 1, this.Context, this.GameConfiguration)
            .WithItemReward(12, 15, this.Context, this.GameConfiguration);

        this.CreateQuest("Random Quest (2)", 19, 48, 77, 49, 290, 319, 257)
            .WithMonsterKillRequirement(20, 69, this.Context, this.GameConfiguration)
            .WithItemRequirement(0, 16, this.Context, this.GameConfiguration, itemOption: 12)
            .WithItemReward(14, 14, this.Context, this.GameConfiguration);

        this.CreateQuest("Random Quest (2)", 19, 48, 78, 49, 290, 319, 257)
            .WithMonsterKillRequirement(20, 69, this.Context, this.GameConfiguration)
            .WithItemRequirement(0, 16, this.Context, this.GameConfiguration, itemOption: 12)
            .WithItemReward(14, 42, this.Context, this.GameConfiguration);

        this.CreateQuest("Random Quest (2)", 19, 48, 79, 49, 290, 319, 257)
            .WithMonsterKillRequirement(20, 69, this.Context, this.GameConfiguration)
            .WithItemRequirement(0, 16, this.Context, this.GameConfiguration, itemOption: 12)
            .WithItemReward(14, 13, this.Context, this.GameConfiguration)
            .WithItemReward(14, 19, this.Context, this.GameConfiguration, 5);

        this.CreateQuest("Random Quest (2)", 19, 48, 80, 49, 290, 319, 257)
            .WithMonsterKillRequirement(20, 69, this.Context, this.GameConfiguration)
            .WithItemRequirement(0, 16, this.Context, this.GameConfiguration, itemOption: 12)
            .WithItemReward(14, 16, this.Context, this.GameConfiguration)
            .WithExperienceReward(140000, this.Context);

        this.CreateQuest("Random Quest (2)", 19, 48, 81, 49, 290, 319, 257)
            .WithMonsterKillRequirement(20, 69, this.Context, this.GameConfiguration)
            .WithItemRequirement(0, 16, this.Context, this.GameConfiguration, itemOption: 12)
            .WithItemReward(14, 19, this.Context, this.GameConfiguration, 6)
            .WithItemReward(14, 14, this.Context, this.GameConfiguration);

        this.CreateQuest("Random Quest (2)", 19, 48, 82, 49, 290, 319, 257)
            .WithMonsterKillRequirement(20, 69, this.Context, this.GameConfiguration)
            .WithItemRequirement(2, 5, this.Context, this.GameConfiguration, itemOption: 12)
            .WithItemReward(4, 16, this.Context, this.GameConfiguration, 6, 12, hasLuck: false, hasSkill: true);

        this.CreateQuest("Random Quest (2)", 19, 48, 83, 49, 290, 319, 257)
            .WithMonsterKillRequirement(20, 69, this.Context, this.GameConfiguration)
            .WithItemRequirement(2, 5, this.Context, this.GameConfiguration, itemOption: 12)
            .WithItemReward(14, 13, this.Context, this.GameConfiguration);

        this.CreateQuest("Random Quest (2)", 19, 48, 84, 49, 290, 319, 257)
            .WithMonsterKillRequirement(20, 69, this.Context, this.GameConfiguration)
            .WithItemRequirement(2, 5, this.Context, this.GameConfiguration, itemOption: 12)
            .WithMoneyReward(200000, this.Context)
            .WithItemReward(14, 42, this.Context, this.GameConfiguration);

        this.CreateQuest("Random Quest (2)", 19, 48, 85, 49, 290, 319, 257)
            .WithMonsterKillRequirement(20, 69, this.Context, this.GameConfiguration)
            .WithItemRequirement(2, 5, this.Context, this.GameConfiguration, itemOption: 12)
            .WithItemReward(2, 10, this.Context, this.GameConfiguration, 6, 12, hasLuck: false, hasSkill: true);

        this.CreateQuest("Random Quest (2)", 19, 48, 86, 49, 290, 319, 257)
            .WithMonsterKillRequirement(20, 69, this.Context, this.GameConfiguration)
            .WithItemRequirement(2, 5, this.Context, this.GameConfiguration, itemOption: 12)
            .WithItemReward(13, 18, this.Context, this.GameConfiguration, 7)
            .WithItemReward(14, 13, this.Context, this.GameConfiguration);

        this.CreateQuest("Random Quest (2)", 19, 48, 87, 49, 290, 319, 257)
            .WithMonsterKillRequirement(20, 69, this.Context, this.GameConfiguration)
            .WithItemRequirement(2, 5, this.Context, this.GameConfiguration, itemOption: 12)
            .WithItemReward(3, 10, this.Context, this.GameConfiguration, 6, 12, hasLuck: false, hasSkill: true);

        this.CreateQuest("Random Quest (2)", 19, 48, 88, 49, 290, 319, 257)
            .WithMonsterKillRequirement(20, 69, this.Context, this.GameConfiguration)
            .WithItemRequirement(2, 5, this.Context, this.GameConfiguration, itemOption: 12)
            .WithItemReward(5, 17, this.Context, this.GameConfiguration, 6, 12);

        this.CreateQuest("Random Quest (2)", 19, 48, 89, 49, 290, 319, 257)
            .WithMonsterKillRequirement(20, 69, this.Context, this.GameConfiguration)
            .WithItemRequirement(2, 5, this.Context, this.GameConfiguration, itemOption: 12)
            .WithItemReward(5, 9, this.Context, this.GameConfiguration, 6, 12);

        this.CreateQuest("Random Quest (3)", 19, 93, 95, 94, 320, 349, 257)
            .WithMonsterKillRequirement(20, 351, this.Context, this.GameConfiguration)
            .WithMonsterKillRequirement(20, 353, this.Context, this.GameConfiguration)
            .WithItemReward(5, 18, this.Context, this.GameConfiguration, 0, 12);

        this.CreateQuest("Random Quest (3)", 19, 93, 96, 94, 320, 349, 257)
            .WithMonsterKillRequirement(20, 351, this.Context, this.GameConfiguration)
            .WithMonsterKillRequirement(20, 353, this.Context, this.GameConfiguration)
            .WithItemReward(14, 13, this.Context, this.GameConfiguration);

        this.CreateQuest("Random Quest (3)", 19, 93, 97, 94, 320, 349, 257)
            .WithMonsterKillRequirement(20, 351, this.Context, this.GameConfiguration)
            .WithMonsterKillRequirement(20, 353, this.Context, this.GameConfiguration)
            .WithItemReward(13, 3, this.Context, this.GameConfiguration, 0, 0, hasLuck: false, hasSkill: true)
            .WithExperienceReward(90000, this.Context);

        this.CreateQuest("Random Quest (3)", 19, 93, 98, 94, 320, 349, 257)
            .WithMonsterKillRequirement(20, 351, this.Context, this.GameConfiguration)
            .WithMonsterKillRequirement(20, 353, this.Context, this.GameConfiguration)
            .WithExperienceReward(160000, this.Context)
            .WithItemReward(14, 14, this.Context, this.GameConfiguration);

        this.CreateQuest("Random Quest (3)", 19, 93, 99, 94, 320, 349, 257)
            .WithMonsterKillRequirement(20, 351, this.Context, this.GameConfiguration)
            .WithMonsterKillRequirement(20, 353, this.Context, this.GameConfiguration)
            .WithItemReward(13, 18, this.Context, this.GameConfiguration, 7)
            .WithItemReward(13, 0, this.Context, this.GameConfiguration);

        this.CreateQuest("Random Quest (3)", 19, 93, 100, 94, 320, 349, 257)
            .WithMonsterKillRequirement(20, 351, this.Context, this.GameConfiguration)
            .WithMonsterKillRequirement(20, 353, this.Context, this.GameConfiguration)
            .WithItemReward(14, 41, this.Context, this.GameConfiguration);

        this.CreateQuest("Random Quest (3)", 19, 93, 101, 94, 320, 349, 257)
            .WithMonsterKillRequirement(20, 351, this.Context, this.GameConfiguration)
            .WithMonsterKillRequirement(20, 353, this.Context, this.GameConfiguration)
            .WithItemReward(14, 13, this.Context, this.GameConfiguration)
            .WithItemReward(14, 19, this.Context, this.GameConfiguration, 5);

        this.CreateQuest("Random Quest (3)", 19, 93, 102, 94, 320, 349, 257)
            .WithMonsterKillRequirement(20, 351, this.Context, this.GameConfiguration)
            .WithMonsterKillRequirement(20, 353, this.Context, this.GameConfiguration)
            .WithItemReward(2, 6, this.Context, this.GameConfiguration, 5, 12, hasLuck: false, hasSkill: true);

        this.CreateQuest("Random Quest (3)", 19, 93, 103, 94, 320, 349, 257)
            .WithMonsterKillRequirement(20, 351, this.Context, this.GameConfiguration)
            .WithMonsterKillRequirement(10, 354, this.Context, this.GameConfiguration)
            .WithItemReward(4, 22, this.Context, this.GameConfiguration, 0, 12, hasLuck: false, hasSkill: true);

        this.CreateQuest("Random Quest (3)", 19, 93, 104, 94, 320, 349, 257)
            .WithMonsterKillRequirement(20, 351, this.Context, this.GameConfiguration)
            .WithMonsterKillRequirement(10, 354, this.Context, this.GameConfiguration)
            .WithItemReward(14, 19, this.Context, this.GameConfiguration, 6)
            .WithItemReward(13, 1, this.Context, this.GameConfiguration);

        this.CreateQuest("Random Quest (3)", 19, 93, 105, 94, 320, 349, 257)
            .WithMonsterKillRequirement(20, 351, this.Context, this.GameConfiguration)
            .WithMonsterKillRequirement(10, 354, this.Context, this.GameConfiguration)
            .WithItemReward(2, 6, this.Context, this.GameConfiguration, 6, 12, hasLuck: false, hasSkill: true);

        this.CreateQuest("Random Quest (3)", 19, 93, 106, 94, 320, 349, 257)
            .WithMonsterKillRequirement(20, 351, this.Context, this.GameConfiguration)
            .WithMonsterKillRequirement(10, 354, this.Context, this.GameConfiguration)
            .WithItemReward(5, 11, this.Context, this.GameConfiguration, 0, 12);

        this.CreateQuest("Random Quest (3)", 19, 93, 107, 94, 320, 349, 257)
            .WithMonsterKillRequirement(20, 351, this.Context, this.GameConfiguration)
            .WithMonsterKillRequirement(10, 354, this.Context, this.GameConfiguration)
            .WithItemReward(12, 15, this.Context, this.GameConfiguration);

        this.CreateQuest("Random Quest (3)", 19, 93, 108, 94, 320, 349, 257)
            .WithMonsterKillRequirement(20, 351, this.Context, this.GameConfiguration)
            .WithMonsterKillRequirement(10, 354, this.Context, this.GameConfiguration)
            .WithItemReward(14, 16, this.Context, this.GameConfiguration);

        this.CreateQuest("Random Quest (3)", 19, 93, 109, 94, 320, 349, 257)
            .WithMonsterKillRequirement(20, 351, this.Context, this.GameConfiguration)
            .WithMonsterKillRequirement(10, 354, this.Context, this.GameConfiguration)
            .WithItemReward(13, 18, this.Context, this.GameConfiguration, 6)
            .WithItemReward(14, 42, this.Context, this.GameConfiguration);

        this.CreateQuest("Random Quest (3)", 19, 93, 110, 94, 320, 349, 257)
            .WithMonsterKillRequirement(20, 351, this.Context, this.GameConfiguration)
            .WithMonsterKillRequirement(10, 354, this.Context, this.GameConfiguration)
            .WithItemReward(14, 1, this.Context, this.GameConfiguration);

        this.CreateQuest("Random Quest (3)", 19, 93, 111, 94, 320, 349, 257)
            .WithMonsterKillRequirement(20, 351, this.Context, this.GameConfiguration)
            .WithMonsterKillRequirement(5, 355, this.Context, this.GameConfiguration)
            .WithItemReward(0, 20, this.Context, this.GameConfiguration, 0, 12, hasLuck: false, hasSkill: true);

        this.CreateQuest("Random Quest (3)", 19, 93, 112, 94, 320, 349, 257)
            .WithMonsterKillRequirement(20, 351, this.Context, this.GameConfiguration)
            .WithMonsterKillRequirement(5, 355, this.Context, this.GameConfiguration)
            .WithItemReward(14, 41, this.Context, this.GameConfiguration);

        this.CreateQuest("Random Quest (3)", 19, 93, 113, 94, 320, 349, 257)
            .WithMonsterKillRequirement(20, 351, this.Context, this.GameConfiguration)
            .WithMonsterKillRequirement(5, 355, this.Context, this.GameConfiguration)
            .WithExperienceReward(160000, this.Context)
            .WithItemReward(13, 3, this.Context, this.GameConfiguration, 0, 0, hasLuck: false, hasSkill: true);

        this.CreateQuest("Random Quest (3)", 19, 93, 114, 94, 320, 349, 257)
            .WithMonsterKillRequirement(20, 351, this.Context, this.GameConfiguration)
            .WithMonsterKillRequirement(5, 355, this.Context, this.GameConfiguration)
            .WithItemReward(5, 11, this.Context, this.GameConfiguration, 0, 12);

        this.CreateQuest("Random Quest (3)", 19, 93, 115, 94, 320, 349, 257)
            .WithMonsterKillRequirement(20, 351, this.Context, this.GameConfiguration)
            .WithMonsterKillRequirement(5, 355, this.Context, this.GameConfiguration)
            .WithItemReward(13, 0, this.Context, this.GameConfiguration)
            .WithExperienceReward(90000, this.Context);

        this.CreateQuest("Random Quest (3)", 19, 93, 116, 94, 320, 349, 257)
            .WithMonsterKillRequirement(20, 351, this.Context, this.GameConfiguration)
            .WithMonsterKillRequirement(5, 355, this.Context, this.GameConfiguration)
            .WithItemReward(12, 15, this.Context, this.GameConfiguration)
            .WithItemReward(13, 1, this.Context, this.GameConfiguration);

        this.CreateQuest("Random Quest (3)", 19, 93, 117, 94, 320, 349, 257)
            .WithMonsterKillRequirement(20, 351, this.Context, this.GameConfiguration)
            .WithMonsterKillRequirement(5, 355, this.Context, this.GameConfiguration)
            .WithItemReward(14, 19, this.Context, this.GameConfiguration, 5)
            .WithItemReward(13, 18, this.Context, this.GameConfiguration, 7);

        this.CreateQuest("Random Quest (3)", 19, 93, 118, 94, 320, 349, 257)
            .WithMonsterKillRequirement(20, 351, this.Context, this.GameConfiguration)
            .WithMonsterKillRequirement(5, 355, this.Context, this.GameConfiguration)
            .WithItemReward(2, 6, this.Context, this.GameConfiguration, 6, 12, hasLuck: false, hasSkill: true);

        this.CreateQuest("Random Quest (3)", 19, 93, 119, 94, 320, 349, 257)
            .WithMonsterKillRequirement(30, 352, this.Context, this.GameConfiguration)
            .WithItemReward(2, 6, this.Context, this.GameConfiguration, 5, 12, hasLuck: false, hasSkill: true);

        this.CreateQuest("Random Quest (3)", 19, 93, 120, 94, 320, 349, 257)
            .WithMonsterKillRequirement(30, 352, this.Context, this.GameConfiguration)
            .WithItemReward(0, 20, this.Context, this.GameConfiguration, 0, 12, hasLuck: false, hasSkill: true);

        this.CreateQuest("Random Quest (3)", 19, 93, 121, 94, 320, 349, 257)
            .WithMonsterKillRequirement(30, 352, this.Context, this.GameConfiguration)
            .WithItemReward(4, 22, this.Context, this.GameConfiguration, 0, 12, hasLuck: false, hasSkill: true);

        this.CreateQuest("Random Quest (3)", 19, 93, 122, 94, 320, 349, 257)
            .WithMonsterKillRequirement(30, 352, this.Context, this.GameConfiguration)
            .WithItemReward(14, 14, this.Context, this.GameConfiguration)
            .WithItemReward(14, 16, this.Context, this.GameConfiguration);

        this.CreateQuest("Random Quest (3)", 19, 93, 123, 94, 320, 349, 257)
            .WithMonsterKillRequirement(30, 352, this.Context, this.GameConfiguration)
            .WithItemReward(5, 18, this.Context, this.GameConfiguration, 0, 12)
            .WithItemReward(13, 18, this.Context, this.GameConfiguration, 6);

        this.CreateQuest("Random Quest (3)", 19, 93, 124, 94, 320, 349, 257)
            .WithMonsterKillRequirement(30, 352, this.Context, this.GameConfiguration)
            .WithItemReward(14, 19, this.Context, this.GameConfiguration, 6)
            .WithItemReward(14, 42, this.Context, this.GameConfiguration);

        this.CreateQuest("Random Quest (3)", 19, 93, 125, 94, 320, 349, 257)
            .WithMonsterKillRequirement(30, 352, this.Context, this.GameConfiguration)
            .WithItemReward(14, 13, this.Context, this.GameConfiguration);

        this.CreateQuest("Random Quest (3)", 19, 93, 126, 94, 320, 349, 257)
            .WithMonsterKillRequirement(30, 352, this.Context, this.GameConfiguration)
            .WithItemReward(14, 13, this.Context, this.GameConfiguration);

        this.CreateQuest("Random Quest (3)", 19, 93, 127, 94, 320, 349, 257)
            .WithMonsterKillRequirement(20, 351, this.Context, this.GameConfiguration)
            .WithItemRequirement(5, 17, this.Context, this.GameConfiguration, itemOption: 12)
            .WithItemReward(5, 18, this.Context, this.GameConfiguration, 0, 12);

        this.CreateQuest("Random Quest (3)", 19, 93, 128, 94, 320, 349, 257)
            .WithMonsterKillRequirement(20, 351, this.Context, this.GameConfiguration)
            .WithItemRequirement(5, 17, this.Context, this.GameConfiguration, itemOption: 12)
            .WithExperienceReward(160000, this.Context)
            .WithItemReward(14, 42, this.Context, this.GameConfiguration);

        this.CreateQuest("Random Quest (3)", 19, 93, 129, 94, 320, 349, 257)
            .WithMonsterKillRequirement(20, 351, this.Context, this.GameConfiguration)
            .WithItemRequirement(5, 17, this.Context, this.GameConfiguration, itemOption: 12)
            .WithExperienceReward(90000, this.Context)
            .WithItemReward(14, 13, this.Context, this.GameConfiguration);

        this.CreateQuest("Random Quest (3)", 19, 93, 130, 94, 320, 349, 257)
            .WithMonsterKillRequirement(20, 351, this.Context, this.GameConfiguration)
            .WithItemRequirement(5, 17, this.Context, this.GameConfiguration, itemOption: 12)
            .WithItemReward(12, 15, this.Context, this.GameConfiguration)
            .WithItemReward(2, 6, this.Context, this.GameConfiguration, 5, 12, hasLuck: false, hasSkill: true);

        this.CreateQuest("Random Quest (3)", 19, 93, 131, 94, 320, 349, 257)
            .WithMonsterKillRequirement(20, 351, this.Context, this.GameConfiguration)
            .WithItemRequirement(5, 17, this.Context, this.GameConfiguration, itemOption: 12)
            .WithItemReward(0, 20, this.Context, this.GameConfiguration, 0, 12, hasLuck: false, hasSkill: true);

        this.CreateQuest("Random Quest (3)", 19, 93, 132, 94, 320, 349, 257)
            .WithMonsterKillRequirement(20, 351, this.Context, this.GameConfiguration)
            .WithItemRequirement(5, 17, this.Context, this.GameConfiguration, itemOption: 12)
            .WithItemReward(13, 18, this.Context, this.GameConfiguration, 7)
            .WithItemReward(14, 16, this.Context, this.GameConfiguration);

        this.CreateQuest("Random Quest (3)", 19, 93, 133, 94, 320, 349, 257)
            .WithMonsterKillRequirement(20, 351, this.Context, this.GameConfiguration)
            .WithItemRequirement(5, 17, this.Context, this.GameConfiguration, itemOption: 12)
            .WithItemReward(5, 11, this.Context, this.GameConfiguration, 0, 12);

        this.CreateQuest("Random Quest (3)", 19, 93, 134, 94, 320, 349, 257)
            .WithMonsterKillRequirement(20, 351, this.Context, this.GameConfiguration)
            .WithItemRequirement(5, 17, this.Context, this.GameConfiguration, itemOption: 12)
            .WithItemReward(14, 19, this.Context, this.GameConfiguration, 6)
            .WithItemReward(14, 41, this.Context, this.GameConfiguration);
    }
}