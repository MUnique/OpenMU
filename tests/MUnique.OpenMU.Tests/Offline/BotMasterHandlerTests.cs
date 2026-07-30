// <copyright file="BotMasterHandlerTests.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.Tests.Offline;

using Moq;
using MUnique.OpenMU.DataModel.Configuration;
using MUnique.OpenMU.DataModel.Entities;
using MUnique.OpenMU.GameLogic;
using MUnique.OpenMU.GameLogic.Attributes;
using MUnique.OpenMU.GameLogic.Bots;
using MUnique.OpenMU.GameLogic.Resets;

/// <summary>
/// Tests for <see cref="BotMasterHandler"/>: when a bot evolves into its master class (including the
/// iron rule of reset servers) and which master skill it invests its points into. The point investment
/// itself goes through the regular <see cref="MUnique.OpenMU.GameLogic.PlayerActions.Character.AddMasterPointAction"/>,
/// whose rules are covered by <see cref="MasterSystemTest"/>.
/// </summary>
[TestFixture]
public class BotMasterHandlerTests
{
    private IGameContext _gameContext = null!;

    /// <summary>
    /// Sets up a fresh game context with the usual maximum level before each test.
    /// </summary>
    [SetUp]
    public void SetUp()
    {
        this._gameContext = GameContextTestHelper.CreateGameContext();
        this._gameContext.Configuration.MaximumLevel = 400;
    }

    /// <summary>
    /// Without the reset feature the evolution is due exactly at the game's maximum level.
    /// </summary>
    /// <param name="level">The character level.</param>
    /// <param name="expectsDue">Whether the evolution is expected to be due.</param>
    [TestCase(399, false)]
    [TestCase(400, true)]
    public async ValueTask EvolutionIsDueAtMaximumLevelAsync(int level, bool expectsDue)
    {
        var player = await this.CreatePlayerWithMasterTargetAsync().ConfigureAwait(false);
        player.Attributes![Stats.Level] = level;

        Assert.That(BotMasterHandler.IsMasterEvolutionDue(player), Is.EqualTo(expectsDue));
    }

    /// <summary>
    /// A class which is already a master (or has no master target) never evolves again.
    /// </summary>
    [Test]
    public async ValueTask NoEvolutionWithoutMasterTargetAsync()
    {
        var player = await PlayerTestHelper.CreateOfflineLevelingPlayerAsync(this._gameContext).ConfigureAwait(false);
        player.Attributes![Stats.Level] = 400;

        Assert.That(BotMasterHandler.IsMasterEvolutionDue(player), Is.False);
    }

    /// <summary>
    /// The iron rule of reset servers: the evolution is only due once the reset limit is exhausted,
    /// and with no limit configured (resetting forever is the endgame) it is never due at all.
    /// Uses the plain test context, where the added feature plugin is the effective one (see the
    /// remarks at <see cref="BotResetHandlerTests.EffectiveLevelCountsResetsAsLevelSpansAsync"/>).
    /// </summary>
    /// <param name="resetLimit">The configured reset limit; 0 means no limit.</param>
    /// <param name="resets">The bot's performed resets.</param>
    /// <param name="expectsDue">Whether the evolution is expected to be due.</param>
    [TestCase(3, 2, false)]
    [TestCase(3, 3, true)]
    [TestCase(0, 50, false)]
    public async ValueTask EvolutionOnResetServersOnlyAfterLastResetAsync(int resetLimit, int resets, bool expectsDue)
    {
        var player = await PlayerTestHelper.CreatePlayerAsync().ConfigureAwait(false);
        player.GameContext.Configuration.MaximumLevel = 400;
        GiveMasterTarget(player);
        player.GameContext.FeaturePlugIns.AddPlugIn(
            new ResetFeaturePlugIn { Configuration = new ResetConfiguration { RequiredLevel = 400, ResetLimit = resetLimit } },
            true);
        player.Attributes![Stats.Level] = 400;
        player.Attributes[Stats.Resets] = resets;

        Assert.That(BotMasterHandler.IsMasterEvolutionDue(player), Is.EqualTo(expectsDue));
    }

    /// <summary>
    /// A due evolution assigns the master class - the same assignment the master quest performs.
    /// </summary>
    [Test]
    public async ValueTask EvolutionAssignsMasterClassAsync()
    {
        var player = await this.CreatePlayerWithMasterTargetAsync().ConfigureAwait(false);
        var masterClass = player.SelectedCharacter!.CharacterClass!.NextGenerationClass!;
        player.Attributes![Stats.Level] = 400;

        var evolved = await BotMasterHandler.TryEvolveAsync(player).ConfigureAwait(false);

        Assert.That(evolved, Is.True);
        Assert.That(player.SelectedCharacter!.CharacterClass, Is.SameAs(masterClass));
    }

    /// <summary>
    /// The point spending loop learns the picked skill through the regular action and invests all
    /// available points.
    /// </summary>
    [Test]
    public async ValueTask SpendsPointsThroughRegularActionAsync()
    {
        // The plain test context is required here - its configuration accepts the mocked skills.
        var contextDonor = await PlayerTestHelper.CreatePlayerAsync().ConfigureAwait(false);
        var player = await PlayerTestHelper.CreateOfflineLevelingPlayerAsync(contextDonor.GameContext).ConfigureAwait(false);
        player.SelectedCharacter!.CharacterClass!.IsMasterClass = true;
        var skill = this.CreateMasterSkill(1, rank: 1, player.SelectedCharacter.CharacterClass);
        player.GameContext.Configuration.Skills.Add(skill);
        player.SelectedCharacter.MasterLevelUpPoints = 3;

        await BotMasterHandler.TrySpendMasterPointsAsync(player).ConfigureAwait(false);

        Assert.That(player.SelectedCharacter.MasterLevelUpPoints, Is.Zero);
        var learned = player.SelectedCharacter.LearnedSkills.FirstOrDefault(l => l.Skill == skill);
        Assert.That(learned, Is.Not.Null);
        Assert.That(learned!.Level, Is.EqualTo(3));
    }

    /// <summary>
    /// A started skill is pushed to the rank-unlock level of 10 before anything new is learned.
    /// </summary>
    [Test]
    public async ValueTask FinishesStartedSkillBeforeLearningNewAsync()
    {
        var player = await PlayerTestHelper.CreatePlayerAsync().ConfigureAwait(false);
        var characterClass = player.SelectedCharacter!.CharacterClass!;
        var startedSkill = this.CreateMasterSkill(1, rank: 1, characterClass);
        var otherSkill = this.CreateMasterSkill(2, rank: 1, characterClass);
        player.GameContext.Configuration.Skills.Add(startedSkill);
        player.GameContext.Configuration.Skills.Add(otherSkill);
        player.SelectedCharacter.LearnedSkills.Add(new SkillEntry { Skill = startedSkill, Level = 5 });
        player.SelectedCharacter.MasterLevelUpPoints = 1;

        Assert.That(BotMasterHandler.PickNextMasterSkill(player), Is.SameAs(startedSkill));
    }

    /// <summary>
    /// A skill of the next rank only becomes eligible once a skill of the previous rank of the same
    /// root reached level 10; a next-rank skill of another root stays out of reach and the points go
    /// into pumping the finished skill instead.
    /// </summary>
    /// <param name="sameRoot">Whether the rank-2 skill shares the root of the learned rank-1 skill.</param>
    [TestCase(true)]
    [TestCase(false)]
    public async ValueTask RespectsRankGatePerRootAsync(bool sameRoot)
    {
        var player = await PlayerTestHelper.CreatePlayerAsync().ConfigureAwait(false);
        var characterClass = player.SelectedCharacter!.CharacterClass!;
        var rank1 = this.CreateMasterSkill(1, rank: 1, characterClass, rootId: 1);
        var rank2 = this.CreateMasterSkill(2, rank: 2, characterClass, rootId: sameRoot ? (byte)1 : (byte)2);
        player.GameContext.Configuration.Skills.Add(rank1);
        player.GameContext.Configuration.Skills.Add(rank2);
        player.SelectedCharacter.LearnedSkills.Add(new SkillEntry { Skill = rank1, Level = 10 });
        player.SelectedCharacter.MasterLevelUpPoints = 1;

        var pick = BotMasterHandler.PickNextMasterSkill(player);

        Assert.That(pick, Is.SameAs(sameRoot ? rank2 : rank1));
    }

    /// <summary>
    /// Among equally reachable new skills, a "useful" one (here: a passive boosting a stat) is
    /// preferred even when a useless one comes first by number.
    /// </summary>
    [Test]
    public async ValueTask PrefersUsefulSkillAsync()
    {
        var player = await PlayerTestHelper.CreatePlayerAsync().ConfigureAwait(false);
        var characterClass = player.SelectedCharacter!.CharacterClass!;
        var uselessSkill = this.CreateMasterSkill(1, rank: 1, characterClass);
        var passiveSkill = this.CreateMasterSkill(2, rank: 1, characterClass);
        passiveSkill.MasterDefinition!.TargetAttribute = Stats.MaximumHealth;
        player.GameContext.Configuration.Skills.Add(uselessSkill);
        player.GameContext.Configuration.Skills.Add(passiveSkill);
        player.SelectedCharacter.MasterLevelUpPoints = 1;

        Assert.That(BotMasterHandler.PickNextMasterSkill(player), Is.SameAs(passiveSkill));
    }

    /// <summary>
    /// A bonus which only applies against other players does nothing for a bot: it spends its life
    /// hunting monsters. It is picked last, after a passive which helps it there.
    /// </summary>
    [Test]
    public async ValueTask PrefersPvmBonusOverPvpBonusAsync()
    {
        var player = await PlayerTestHelper.CreatePlayerAsync().ConfigureAwait(false);
        var characterClass = player.SelectedCharacter!.CharacterClass!;
        var pvpSkill = this.CreateMasterSkill(1, rank: 1, characterClass);
        pvpSkill.MasterDefinition!.TargetAttribute = Stats.DefenseRatePvp;
        var pvmSkill = this.CreateMasterSkill(2, rank: 1, characterClass);
        pvmSkill.MasterDefinition!.TargetAttribute = Stats.MaximumHealth;
        player.GameContext.Configuration.Skills.Add(pvpSkill);
        player.GameContext.Configuration.Skills.Add(pvmSkill);
        player.SelectedCharacter.MasterLevelUpPoints = 1;

        Assert.That(BotMasterHandler.PickNextMasterSkill(player), Is.SameAs(pvmSkill));
    }

    /// <summary>
    /// With everything learned at its maximum nothing is picked - the loop stops.
    /// </summary>
    [Test]
    public async ValueTask PicksNothingWhenTreeIsFullAsync()
    {
        var player = await PlayerTestHelper.CreatePlayerAsync().ConfigureAwait(false);
        var characterClass = player.SelectedCharacter!.CharacterClass!;
        var skill = this.CreateMasterSkill(1, rank: 1, characterClass);
        player.GameContext.Configuration.Skills.Add(skill);
        player.SelectedCharacter.LearnedSkills.Add(new SkillEntry { Skill = skill, Level = 20 });
        player.SelectedCharacter.MasterLevelUpPoints = 5;

        Assert.That(BotMasterHandler.PickNextMasterSkill(player), Is.Null);
    }

    /// <summary>
    /// Creates an offline test player whose class has a master class as next generation.
    /// </summary>
    private async ValueTask<GameLogic.Offline.OfflinePlayer> CreatePlayerWithMasterTargetAsync()
    {
        var player = await PlayerTestHelper.CreateOfflineLevelingPlayerAsync(this._gameContext).ConfigureAwait(false);
        GiveMasterTarget(player);
        return player;
    }

    /// <summary>
    /// Gives the player's character class a master class as next generation.
    /// </summary>
    private static void GiveMasterTarget(Player player)
    {
        var masterClass = new CharacterClass
        {
            Name = "Test Master",
            IsMasterClass = true,
        };
        Mock.Get(player.SelectedCharacter!.CharacterClass!)
            .Setup(c => c.NextGenerationClass)
            .Returns(masterClass);
    }

    private Skill CreateMasterSkill(short number, byte rank, CharacterClass qualifiedClass, byte rootId = 1)
    {
        var masterDefinition = new Mock<MasterSkillDefinition>();
        masterDefinition.SetupAllProperties();
        masterDefinition.Object.Rank = rank;
        masterDefinition.Object.MaximumLevel = 20;
        masterDefinition.Object.MinimumLevel = 1;
        masterDefinition.Object.Root = new MasterSkillRoot { Id = new Guid(0, 0, 0, 0, 0, 0, 0, 0, 0, 0, rootId) };
        masterDefinition.Setup(m => m.RequiredMasterSkills).Returns(new List<Skill>());

        var skill = new Mock<Skill>();
        skill.SetupAllProperties();
        skill.Object.Number = number;
        skill.Setup(s => s.QualifiedCharacters).Returns(new List<CharacterClass> { qualifiedClass });
        skill.Object.MasterDefinition = masterDefinition.Object;
        return skill.Object;
    }
}
