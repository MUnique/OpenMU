// <copyright file="KillAttributionTest.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.Tests;

using Moq;
using MUnique.OpenMU.DataModel.Configuration;
using MUnique.OpenMU.DataModel.Entities;
using MUnique.OpenMU.GameLogic;
using MUnique.OpenMU.GameLogic.Attributes;
using MUnique.OpenMU.GameLogic.NPC;
using MUnique.OpenMU.GameLogic.Pet;
using MUnique.OpenMU.GameLogic.PlugIns;
using MUnique.OpenMU.Persistence;
using BasicModel = MUnique.OpenMU.Persistence.BasicModel;

/// <summary>
/// Tests that the kills of a Dark Raven and of summoned monsters are attributed to the player behind them.
/// </summary>
[TestFixture]
public class KillAttributionTest
{
    private IGameContext _gameContext = null!;

    /// <summary>
    /// Sets up a fresh game context before each test.
    /// </summary>
    [SetUp]
    public void SetUp()
    {
        this._gameContext = GameContextTestHelper.CreateGameContext();
    }

    /// <summary>
    /// Tests that the name of a Dark Raven attacker is the name of its owner, so that announcements and
    /// mini games which use <see cref="DeathInformation.KillerName"/> know who made the kill.
    /// </summary>
    [Test]
    public async ValueTask RavenAttackerHasTheNameOfItsOwnerAsync()
    {
        var owner = await PlayerTestHelper.CreatePlayerAsync(this._gameContext).ConfigureAwait(false);
        owner.SelectedCharacter!.Name = "RavenOwner";
        var raven = new AttackerSurrogate(owner, owner.Attributes!);

        Assert.That(raven.GetName(), Is.EqualTo("RavenOwner"));
    }

    /// <summary>
    /// Tests which player is responsible for the attacks of players, pets and monsters.
    /// </summary>
    [Test]
    public async ValueTask ResponsiblePlayerOfAttackersAsync()
    {
        var owner = await PlayerTestHelper.CreatePlayerAsync(this._gameContext).ConfigureAwait(false);
        var raven = new AttackerSurrogate(owner, owner.Attributes!);
        var summon = await this.CreateMonsterAsync(new SummonedMonsterIntelligence(owner)).ConfigureAwait(false);
        var monster = await this.CreateMonsterAsync().ConfigureAwait(false);

        Assert.That(owner.GetResponsiblePlayer(), Is.SameAs(owner));
        Assert.That(raven.GetResponsiblePlayer(), Is.SameAs(owner));
        Assert.That(summon.GetResponsiblePlayer(), Is.SameAs(owner));
        Assert.That(monster.GetResponsiblePlayer(), Is.Null);
        Assert.That(((IAttacker?)null).GetResponsiblePlayer(), Is.Null);
    }

    /// <summary>
    /// Tests that a quest monster killed by the Dark Raven or a summoned monster counts for the quest of the owner.
    /// </summary>
    [Test]
    public async ValueTask PetAndSummonKillsCountForTheQuestOfTheOwnerAsync()
    {
        var owner = await PlayerTestHelper.CreatePlayerAsync(this._gameContext).ConfigureAwait(false);
        var questMonster = await this.CreateMonsterAsync().ConfigureAwait(false);
        var requirementState = this.StartQuestToKill(owner, questMonster.Definition);
        var raven = new AttackerSurrogate(owner, owner.Attributes!);
        var summon = await this.CreateMonsterAsync(new SummonedMonsterIntelligence(owner)).ConfigureAwait(false);
        var plugIn = new QuestMonsterKillCountPlugIn();

        await plugIn.AttackableGotKilledAsync(questMonster, owner).ConfigureAwait(false);
        await plugIn.AttackableGotKilledAsync(questMonster, raven).ConfigureAwait(false);
        await plugIn.AttackableGotKilledAsync(questMonster, summon).ConfigureAwait(false);

        Assert.That(requirementState.KillCount, Is.EqualTo(3));
    }

    /// <summary>
    /// Tests that a quest monster killed by a monster which nobody summoned counts for no quest.
    /// </summary>
    [Test]
    public async ValueTask KillsOfOtherMonstersDontCountForQuestsAsync()
    {
        var player = await PlayerTestHelper.CreatePlayerAsync(this._gameContext).ConfigureAwait(false);
        var questMonster = await this.CreateMonsterAsync().ConfigureAwait(false);
        var requirementState = this.StartQuestToKill(player, questMonster.Definition);
        var otherMonster = await this.CreateMonsterAsync().ConfigureAwait(false);

        await new QuestMonsterKillCountPlugIn().AttackableGotKilledAsync(questMonster, otherMonster).ConfigureAwait(false);

        Assert.That(requirementState.KillCount, Is.Zero);
    }

    private QuestMonsterKillRequirementState StartQuestToKill(Player player, MonsterDefinition monsterDefinition)
    {
        var requirement = new BasicModel.QuestMonsterKillRequirement { Monster = monsterDefinition, MinimumNumber = 10 };
        var quest = new BasicModel.QuestDefinition();
        quest.RequiredMonsterKills.Add(requirement);
        var requirementState = new BasicModel.QuestMonsterKillRequirementState { Requirement = requirement };
        var questState = new BasicModel.CharacterQuestState { ActiveQuest = quest };
        questState.RequirementStates.Add(requirementState);
        Mock.Get(player.SelectedCharacter!).Setup(c => c.QuestStates).Returns(new List<CharacterQuestState> { questState });
        return requirementState;
    }

    private async ValueTask<Monster> CreateMonsterAsync(INpcIntelligence? intelligence = null)
    {
        var monsterDefinition = new BasicModel.MonsterDefinition { ObjectKind = NpcObjectKind.Monster };
        monsterDefinition.Attributes.Add(new BasicModel.MonsterAttribute { AttributeDefinition = Stats.MaximumHealth, Value = 100 });
        var map = await this._gameContext.GetMapAsync(0).ConfigureAwait(false);
        var spawnArea = new MonsterSpawnArea
        {
            MonsterDefinition = monsterDefinition,
            GameMap = map!.Definition,
            X1 = 100,
            Y1 = 100,
            X2 = 100,
            Y2 = 100,
            Quantity = 1,
        };

        var monster = new Monster(
            spawnArea,
            monsterDefinition,
            map,
            NullDropGenerator.Instance,
            intelligence ?? new Mock<INpcIntelligence>().Object,
            this._gameContext.PlugInManager,
            this._gameContext.PathFinderPool);
        monster.Initialize();
        return monster;
    }
}
