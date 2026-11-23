// <copyright file="EvolutionQuestOfferTest.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.Tests;

using Moq;
using MUnique.OpenMU.DataModel.Configuration;
using MUnique.OpenMU.DataModel.Configuration.Quests;
using MUnique.OpenMU.GameLogic;
using MUnique.OpenMU.GameLogic.NPC;
using MUnique.OpenMU.GameLogic.PlayerActions.Quests;
using BasicModel = MUnique.OpenMU.Persistence.BasicModel;

/// <summary>
/// Tests that the quests of a class evolution aren't offered to a character which is already in its last class,
/// e.g. a Lord Emperor which got its class without the third class quests (created so, or changed by a game master).
/// </summary>
[TestFixture]
public class EvolutionQuestOfferTest
{
    private const short EvolutionGroup = 0;

    private const short OtherGroup = 1;

    /// <summary>
    /// Tests that a character in its last class gets none of the quests of the evolution's group, but others.
    /// </summary>
    [Test]
    public async ValueTask LastClassGetsNoEvolutionQuestsAsync()
    {
        var (lordEmperor, _) = CreateClasses();
        var player = await CreatePlayerAtNpcAsync(lordEmperor).ConfigureAwait(false);

        var available = player.GetAvailableQuestsOfOpenedNpc().ToList();

        Assert.That(available.Select(q => q.Group), Is.EquivalentTo(new[] { OtherGroup }));
        Assert.That(player.GetQuest(EvolutionGroup, 5), Is.Null);
    }

    /// <summary>
    /// Tests that a character before its last class still gets the quests of the evolution.
    /// </summary>
    [Test]
    public async ValueTask ClassBeforeTheLastGetsTheEvolutionQuestsAsync()
    {
        var (_, darkLord) = CreateClasses();
        var player = await CreatePlayerAtNpcAsync(darkLord).ConfigureAwait(false);

        var available = player.GetAvailableQuestsOfOpenedNpc().ToList();

        Assert.That(available.Where(q => q.Group == EvolutionGroup).Select(q => q.Number), Is.EquivalentTo(new short[] { 4, 5, 6 }));
        Assert.That(player.GetQuest(EvolutionGroup, 5), Is.Not.Null);
    }

    private static (CharacterClass LastClass, CharacterClass ClassBefore) CreateClasses()
    {
        var lordEmperor = new BasicModel.CharacterClass { Number = 17, IsActive = true };
        var darkLord = new BasicModel.CharacterClass { Number = 16, IsActive = true, NextGenerationClass = lordEmperor };
        return (lordEmperor, darkLord);
    }

    private static async ValueTask<Player> CreatePlayerAtNpcAsync(CharacterClass characterClass)
    {
        var player = await PlayerTestHelper.CreatePlayerAsync(GameContextTestHelper.CreateGameContext()).ConfigureAwait(false);
        Mock.Get(player.SelectedCharacter!).Setup(c => c.CharacterClass).Returns(characterClass);

        var npc = new BasicModel.MonsterDefinition();
        npc.Quests.Add(new BasicModel.QuestDefinition { Group = EvolutionGroup, Number = 4 });
        npc.Quests.Add(new BasicModel.QuestDefinition { Group = EvolutionGroup, Number = 5 });
        var evolution = new BasicModel.QuestDefinition { Group = EvolutionGroup, Number = 6 };
        evolution.Rewards.Add(new BasicModel.QuestReward { RewardType = QuestRewardType.CharacterEvolutionSecondToThird });
        npc.Quests.Add(evolution);
        npc.Quests.Add(new BasicModel.QuestDefinition { Group = OtherGroup, Number = 1 });

        player.OpenedNpc = new NonPlayerCharacter(null!, npc, null!);
        return player;
    }
}
