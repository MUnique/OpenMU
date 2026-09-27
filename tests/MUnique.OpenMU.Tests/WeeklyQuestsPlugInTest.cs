// <copyright file="WeeklyQuestsPlugInTest.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.Tests;

using Microsoft.Extensions.Logging.Abstractions;
using MUnique.OpenMU.DataModel.Configuration;
using MUnique.OpenMU.DataModel.Configuration.Items;
using MUnique.OpenMU.GameLogic;
using MUnique.OpenMU.GameLogic.Attributes;
using MUnique.OpenMU.GameLogic.NPC;
using MUnique.OpenMU.GameLogic.PlugIns;
using MUnique.OpenMU.GameLogic.PlugIns.WeeklyQuests;
using MUnique.OpenMU.Persistence;
using MUnique.OpenMU.Persistence.WeeklyQuests;
using MUnique.OpenMU.Interfaces;
using MUnique.OpenMU.PlugIns;
using Moq;

/// <summary>
/// Tests for the <see cref="WeeklyQuestsPlugIn"/> and <see cref="WeeklyPeriod"/>.
/// </summary>
[TestFixture]
public class WeeklyQuestsPlugInTest
{
    private const string QuestId = "resets-2";

    /// <summary>
    /// Tests that the period starts at the configured day and time of the current week.
    /// </summary>
    [Test]
    public void PeriodStartsAtResetDayOfCurrentWeek()
    {
        // 2026-09-25 is a friday.
        var now = new DateTime(2026, 9, 25, 15, 0, 0, DateTimeKind.Utc);
        var start = WeeklyPeriod.GetPeriodStartUtc(now, DayOfWeek.Monday, new TimeOnly(3, 0), TimeZoneInfo.Utc);
        Assert.That(start, Is.EqualTo(new DateTime(2026, 9, 21, 3, 0, 0, DateTimeKind.Utc)));
    }

    /// <summary>
    /// Tests that a point in time on the reset day, but before the reset time, belongs to the previous period.
    /// </summary>
    [Test]
    public void PeriodBeforeResetTimeBelongsToPreviousWeek()
    {
        var now = new DateTime(2026, 9, 21, 2, 59, 0, DateTimeKind.Utc);
        var start = WeeklyPeriod.GetPeriodStartUtc(now, DayOfWeek.Monday, new TimeOnly(3, 0), TimeZoneInfo.Utc);
        Assert.That(start, Is.EqualTo(new DateTime(2026, 9, 14, 3, 0, 0, DateTimeKind.Utc)));
    }

    /// <summary>
    /// Tests that the reset time is interpreted in the time zone of the server.
    /// </summary>
    [Test]
    public void PeriodUsesServerTimeZone()
    {
        var timeZone = TimeZoneInfo.CreateCustomTimeZone("UTC-3", TimeSpan.FromHours(-3), "UTC-3", "UTC-3");
        var now = new DateTime(2026, 9, 21, 2, 0, 0, DateTimeKind.Utc); // sunday 23:00 local
        var start = WeeklyPeriod.GetPeriodStartUtc(now, DayOfWeek.Monday, TimeOnly.MinValue, timeZone);
        // Monday 00:00 local is 03:00 UTC. Sunday 23:00 local still belongs to the week which started on the 14th.
        Assert.That(start, Is.EqualTo(new DateTime(2026, 9, 14, 3, 0, 0, DateTimeKind.Utc)));
        Assert.That(WeeklyPeriod.GetNextPeriodStartUtc(start, timeZone), Is.EqualTo(new DateTime(2026, 9, 21, 3, 0, 0, DateTimeKind.Utc)));
    }

    /// <summary>
    /// Tests that the progress is counted and the reward is given exactly once when the objective is reached.
    /// </summary>
    [Test]
    public async Task CompletesQuestAndRewardsOnceAsync()
    {
        var player = await CreatePlayerAsync().ConfigureAwait(false);
        var plugIn = CreatePlugIn(new InMemoryWeeklyQuestProgressRepository());

        await plugIn.CharacterResetAsync(player, 1).ConfigureAwait(false);
        Assert.That(player.Money, Is.EqualTo(0));

        await plugIn.CharacterResetAsync(player, 2).ConfigureAwait(false);
        Assert.That(player.Money, Is.EqualTo(1000));

        await plugIn.CharacterResetAsync(player, 3).ConfigureAwait(false);
        Assert.That(player.Money, Is.EqualTo(1000));

        var overview = await plugIn.GetOverviewAsync(player).ConfigureAwait(false);
        var entry = overview!.Entries.Single();
        Assert.That(entry.Count, Is.EqualTo(2));
        Assert.That(entry.IsCompleted, Is.True);
        Assert.That(entry.IsRewarded, Is.True);
    }

    /// <summary>
    /// Tests that a completed quest is persisted, so that it's not rewarded again after a restart.
    /// </summary>
    [Test]
    public async Task CompletedQuestIsPersistedAsync()
    {
        var repository = new InMemoryWeeklyQuestProgressRepository();
        var player = await CreatePlayerAsync().ConfigureAwait(false);
        var plugIn = CreatePlugIn(repository);
        await plugIn.CharacterResetAsync(player, 1).ConfigureAwait(false);
        await plugIn.CharacterResetAsync(player, 2).ConfigureAwait(false);

        Assert.That(player.Money, Is.EqualTo(1000));

        // Leaving the game drops the in-memory state, so the next plugin has to load it from the repository.
        await plugIn.PlayerStateChangedAsync(player, PlayerState.EnteredWorld, PlayerState.CharacterSelection).ConfigureAwait(false);
        var restartedPlugIn = CreatePlugIn(repository);
        await restartedPlugIn.CharacterResetAsync(player, 3).ConfigureAwait(false);

        Assert.That(player.Money, Is.EqualTo(1000));
        var overview = await restartedPlugIn.GetOverviewAsync(player).ConfigureAwait(false);
        Assert.That(overview!.Entries.Single().Count, Is.EqualTo(2));
        Assert.That(overview.Entries.Single().IsRewarded, Is.True);
    }

    /// <summary>
    /// Tests that the reward stays pending when it can't be given, and that it's given later.
    /// </summary>
    [Test]
    public async Task RewardStaysPendingUntilItFitsAsync()
    {
        var player = await CreatePlayerAsync().ConfigureAwait(false);
        player.GameContext.Configuration.MaximumInventoryMoney = 500;
        var plugIn = CreatePlugIn(new InMemoryWeeklyQuestProgressRepository());

        await plugIn.CharacterResetAsync(player, 1).ConfigureAwait(false);
        await plugIn.CharacterResetAsync(player, 2).ConfigureAwait(false);
        Assert.That(player.Money, Is.EqualTo(0));

        var overview = await plugIn.GetOverviewAsync(player).ConfigureAwait(false);
        Assert.That(overview!.Entries.Single().IsRewarded, Is.False);

        player.GameContext.Configuration.MaximumInventoryMoney = int.MaxValue;
        overview = await plugIn.GetOverviewAsync(player).ConfigureAwait(false);
        Assert.That(overview!.Entries.Single().IsRewarded, Is.True);
        Assert.That(player.Money, Is.EqualTo(1000));
    }

    /// <summary>
    /// Tests that no progress is made when the character doesn't meet the minimum level.
    /// </summary>
    [Test]
    public async Task NoProgressBelowMinimumLevelAsync()
    {
        var player = await CreatePlayerAsync().ConfigureAwait(false);
        var plugIn = CreatePlugIn(new InMemoryWeeklyQuestProgressRepository());
        plugIn.Configuration!.Quests.Single().MinimumLevel = 100;

        await plugIn.CharacterResetAsync(player, 1).ConfigureAwait(false);

        var overview = await plugIn.GetOverviewAsync(player).ConfigureAwait(false);
        Assert.That(overview!.Entries.Single().Count, Is.EqualTo(0));

        player.Attributes![Stats.Level] = 100;
        await plugIn.CharacterResetAsync(player, 1).ConfigureAwait(false);
        overview = await plugIn.GetOverviewAsync(player).ConfigureAwait(false);
        Assert.That(overview!.Entries.Single().Count, Is.EqualTo(1));
    }

    /// <summary>
    /// Tests that inactive quests are ignored.
    /// </summary>
    [Test]
    public async Task InactiveQuestIsIgnoredAsync()
    {
        var player = await CreatePlayerAsync().ConfigureAwait(false);
        var plugIn = CreatePlugIn(new InMemoryWeeklyQuestProgressRepository());
        plugIn.Configuration!.Quests.Single().IsActive = false;

        await plugIn.CharacterResetAsync(player, 1).ConfigureAwait(false);

        var overview = await plugIn.GetOverviewAsync(player).ConfigureAwait(false);
        Assert.That(overview!.Entries, Is.Empty);
    }

    /// <summary>
    /// Tests that a quest without qualified classes is available for every class.
    /// </summary>
    [Test]
    public void QuestWithoutClassesQualifiesEveryClass()
    {
        var quest = new WeeklyQuestDefinition();
        Assert.That(quest.IsQualified(new CharacterClass { Number = 4 }), Is.True);
        Assert.That(quest.IsQualified(null), Is.True);
    }

    /// <summary>
    /// Tests that a qualified class includes its evolutions, but not its previous classes.
    /// </summary>
    [Test]
    public void QualifiedClassIncludesItsEvolutions()
    {
        var (wizard, soulMaster, grandMaster) = CreateWizardLine();
        var knight = new CharacterClass { Number = 4 };

        var wizardQuest = new WeeklyQuestDefinition { QualifiedCharacters = { wizard } };
        Assert.That(wizardQuest.IsQualified(wizard), Is.True);
        Assert.That(wizardQuest.IsQualified(soulMaster), Is.True);
        Assert.That(wizardQuest.IsQualified(grandMaster), Is.True);
        Assert.That(wizardQuest.IsQualified(knight), Is.False);
        Assert.That(wizardQuest.IsQualified(null), Is.False);

        var soulMasterQuest = new WeeklyQuestDefinition { QualifiedCharacters = { soulMaster } };
        Assert.That(soulMasterQuest.IsQualified(wizard), Is.False);
        Assert.That(soulMasterQuest.IsQualified(soulMaster), Is.True);
        Assert.That(soulMasterQuest.IsQualified(grandMaster), Is.True);
    }

    /// <summary>
    /// Tests that a circular chain of classes doesn't hang the check.
    /// </summary>
    [Test]
    public void CircularClassChainTerminates()
    {
        var a = new CharacterClass { Number = 1 };
        var b = new CharacterClass { Number = 2, NextGenerationClass = a };
        a.NextGenerationClass = b;

        var quest = new WeeklyQuestDefinition { QualifiedCharacters = { a } };
        Assert.That(quest.IsQualified(new CharacterClass { Number = 3 }), Is.False);
    }

    /// <summary>
    /// Tests that a character of another class neither makes progress nor sees the quest.
    /// </summary>
    [Test]
    public async Task OtherClassDoesNotSeeNorProgressQuestAsync()
    {
        var player = await CreatePlayerAsync().ConfigureAwait(false);
        player.SelectedCharacter!.CharacterClass!.Number = 4;
        var plugIn = CreatePlugIn(new InMemoryWeeklyQuestProgressRepository());
        plugIn.Configuration!.Quests.Single().QualifiedCharacters.Add(CreateWizardLine().Wizard);

        await plugIn.CharacterResetAsync(player, 1).ConfigureAwait(false);
        await plugIn.CharacterResetAsync(player, 2).ConfigureAwait(false);

        Assert.That(player.Money, Is.EqualTo(0));
        var overview = await plugIn.GetOverviewAsync(player).ConfigureAwait(false);
        Assert.That(overview!.Entries, Is.Empty);
    }

    /// <summary>
    /// Tests that an evolved class makes progress in a quest of its class line.
    /// </summary>
    [Test]
    public async Task EvolvedClassProgressesQuestOfItsLineAsync()
    {
        var player = await CreatePlayerAsync().ConfigureAwait(false);
        var (wizard, soulMaster, _) = CreateWizardLine();
        player.SelectedCharacter!.CharacterClass!.Number = soulMaster.Number;
        var plugIn = CreatePlugIn(new InMemoryWeeklyQuestProgressRepository());
        plugIn.Configuration!.Quests.Single().QualifiedCharacters.Add(wizard);

        await plugIn.CharacterResetAsync(player, 1).ConfigureAwait(false);
        await plugIn.CharacterResetAsync(player, 2).ConfigureAwait(false);

        Assert.That(player.Money, Is.EqualTo(1000));
        var overview = await plugIn.GetOverviewAsync(player).ConfigureAwait(false);
        Assert.That(overview!.Entries.Single().IsRewarded, Is.True);
    }

    /// <summary>
    /// Tests that the qualified classes are stored as references in the plugin configuration
    /// and are resolved to the classes of the game configuration when it's loaded.
    /// </summary>
    [Test]
    public void QualifiedClassesSurviveJsonRoundTrip()
    {
        var wizard = new Persistence.BasicModel.CharacterClass { Id = Guid.NewGuid(), Number = 0 };
        var dataSource = new Mock<IDataSource<GameConfiguration>>();
        dataSource.Setup(d => d.Get(wizard.Id)).Returns(wizard);
        var referenceHandler = new ByDataSourceReferenceHandler(dataSource.Object);

        var configuration = CreatePlugIn(new InMemoryWeeklyQuestProgressRepository()).Configuration!;
        configuration.Quests.Single().QualifiedCharacters.Add(wizard);
        var plugInConfiguration = new PlugInConfiguration();
        plugInConfiguration.SetConfiguration(configuration, referenceHandler);
        var loaded = plugInConfiguration.GetConfiguration<WeeklyQuestsConfiguration>(referenceHandler);

        Assert.That(loaded!.Quests.Single().QualifiedCharacters.Single(), Is.SameAs(wizard));
    }

    /// <summary>
    /// Tests that the monster kills count for the party members who see the killer, but not for the others.
    /// </summary>
    [Test]
    public async Task KillsAreSharedWithNearbyPartyMembersAsync()
    {
        var killer = await CreatePlayerAsync().ConfigureAwait(false);
        var nearby = await CreatePlayerAsync(killer.GameContext).ConfigureAwait(false);
        var farAway = await CreatePlayerAsync(killer.GameContext).ConfigureAwait(false);
        var party = new Party(new PartyManager(5, new NullLogger<Party>()), 5, new NullLogger<Party>());
        await party.AddAsync(killer).ConfigureAwait(false);
        await party.AddAsync(nearby).ConfigureAwait(false);
        await party.AddAsync(farAway).ConfigureAwait(false);
        // The test players enter the same map at the same position, so we set up who sees whom explicitly.
        killer.Observers.Add(nearby);
        killer.Observers.Remove(farAway);
        nearby.IsAlive = true;
        farAway.IsAlive = true;

        var plugIn = CreatePlugIn(new InMemoryWeeklyQuestProgressRepository());
        var receivers = await plugIn.GetKillReceiversAsync(killer).ConfigureAwait(false);
        Assert.That(receivers, Is.EquivalentTo(new[] { killer, nearby }));

        plugIn.Configuration!.ShareKillsWithParty = false;
        receivers = await plugIn.GetKillReceiversAsync(killer).ConfigureAwait(false);
        Assert.That(receivers, Is.EquivalentTo(new[] { killer }));
    }

    /// <summary>
    /// Tests that the kills of guild mates don't count, and that the same victim only counts again after the cooldown.
    /// </summary>
    [Test]
    public async Task PlayerKillsIgnoreGuildMatesAndRespectCooldownAsync()
    {
        var killer = await CreatePlayerAsync().ConfigureAwait(false);
        var victim = await CreatePlayerAsync(killer.GameContext).ConfigureAwait(false);
        var plugIn = CreatePlugIn(
            new InMemoryWeeklyQuestProgressRepository(),
            new WeeklyQuestDefinition { Id = "pvp", Name = "PvP", ObjectiveType = WeeklyQuestObjectiveType.KillPlayer, RequiredCount = 10, VictimCooldownMinutes = 30 });

        killer.GuildStatus = new GuildMemberStatus(1, GuildPosition.NormalMember);
        victim.GuildStatus = new GuildMemberStatus(1, GuildPosition.NormalMember);
        await plugIn.AttackableGotKilledAsync(victim, killer).ConfigureAwait(false);
        Assert.That(await GetCountAsync(plugIn, killer, "pvp").ConfigureAwait(false), Is.EqualTo(0));

        victim.GuildStatus = new GuildMemberStatus(2, GuildPosition.NormalMember);
        await plugIn.AttackableGotKilledAsync(victim, killer).ConfigureAwait(false);
        await plugIn.AttackableGotKilledAsync(victim, killer).ConfigureAwait(false);
        Assert.That(await GetCountAsync(plugIn, killer, "pvp").ConfigureAwait(false), Is.EqualTo(1));
    }

    /// <summary>
    /// Tests that picked up items count, unless they were dropped by a player, and only for the configured item and level.
    /// </summary>
    [Test]
    public async Task CollectItemCountsOnlyDropsOfTheItemAsync()
    {
        var player = await CreatePlayerAsync().ConfigureAwait(false);
        var jewel = new ItemDefinition { Group = 14, Number = 13 };
        var plugIn = CreatePlugIn(
            new InMemoryWeeklyQuestProgressRepository(),
            new WeeklyQuestDefinition { Id = "bless", Name = "Bless", ObjectiveType = WeeklyQuestObjectiveType.CollectItem, RequiredCount = 10, Item = jewel, MinimumItemLevel = 1 });

        await plugIn.ItemPickedUpAsync(player, new TemporaryItem { Definition = jewel, Level = 1 }, false).ConfigureAwait(false);
        await plugIn.ItemPickedUpAsync(player, new TemporaryItem { Definition = jewel, Level = 1 }, true).ConfigureAwait(false);
        await plugIn.ItemPickedUpAsync(player, new TemporaryItem { Definition = jewel, Level = 0 }, false).ConfigureAwait(false);
        await plugIn.ItemPickedUpAsync(player, new TemporaryItem { Definition = new ItemDefinition { Group = 14, Number = 14 }, Level = 1 }, false).ConfigureAwait(false);

        Assert.That(await GetCountAsync(plugIn, player, "bless").ConfigureAwait(false), Is.EqualTo(1));
    }

    /// <summary>
    /// Tests that the bonus is shown with the progress of the quests and rewarded when all of them are completed.
    /// </summary>
    [Test]
    public async Task AllCompletedBonusIsRewardedAsync()
    {
        var player = await CreatePlayerAsync().ConfigureAwait(false);
        var plugIn = CreatePlugIn(
            new InMemoryWeeklyQuestProgressRepository(),
            new WeeklyQuestDefinition { Id = "resets", Name = "Resets", ObjectiveType = WeeklyQuestObjectiveType.GainResets, RequiredCount = 1 },
            new WeeklyQuestDefinition { Id = "masters", Name = "Masters", ObjectiveType = WeeklyQuestObjectiveType.GainMasterLevels, RequiredCount = 1 });
        plugIn.Configuration!.AllCompletedBonus = new WeeklyQuestDefinition
        {
            Name = "Bonus",
            Rewards = { new WeeklyQuestReward { RewardType = WeeklyQuestRewardType.Money, Amount = 5000 } },
        };

        await plugIn.CharacterResetAsync(player, 1).ConfigureAwait(false);
        var overview = await plugIn.GetOverviewAsync(player).ConfigureAwait(false);
        var bonus = overview!.Entries.Single(e => e.Quest.Id == WeeklyQuestSelector.AllCompletedBonusId);
        Assert.That(bonus.Count, Is.EqualTo(1));
        Assert.That(bonus.Quest.RequiredCount, Is.EqualTo(2));
        Assert.That(player.Money, Is.EqualTo(0));

        await plugIn.CharacterMasterLeveledUpAsync(player).ConfigureAwait(false);
        overview = await plugIn.GetOverviewAsync(player).ConfigureAwait(false);
        bonus = overview!.Entries.Single(e => e.Quest.Id == WeeklyQuestSelector.AllCompletedBonusId);
        Assert.That(bonus.IsRewarded, Is.True);
        Assert.That(player.Money, Is.EqualTo(5000));
    }

    /// <summary>
    /// Tests that a quest which is limited to one character per account is hidden, when another character of the account received its rewards.
    /// </summary>
    [Test]
    public async Task OncePerAccountQuestIsHiddenForOtherCharactersAsync()
    {
        var repository = new InMemoryWeeklyQuestProgressRepository();
        var player = await CreatePlayerAsync().ConfigureAwait(false);
        var accountId = Guid.NewGuid();
        player.Account = new Persistence.BasicModel.Account { Id = accountId, LoginName = "test" };
        var plugIn = CreatePlugIn(repository);
        var quest = plugIn.Configuration!.Quests.Single();
        quest.OncePerAccount = true;

        var periodStart = WeeklyPeriod.GetPeriodStartUtc(DateTime.UtcNow, DayOfWeek.Monday, TimeOnly.MinValue, TimeZoneInfo.Utc);
        await repository.SaveAsync(new[]
        {
            new WeeklyQuestProgress
            {
                CharacterId = Guid.NewGuid(),
                AccountId = accountId,
                PeriodStart = periodStart,
                QuestId = quest.Id,
                Count = 2,
                CompletedAt = DateTime.UtcNow,
                RewardedAt = DateTime.UtcNow,
            },
        }).ConfigureAwait(false);

        await plugIn.CharacterResetAsync(player, 1).ConfigureAwait(false);
        var overview = await plugIn.GetOverviewAsync(player).ConfigureAwait(false);
        Assert.That(overview!.Entries, Is.Empty);

        // Another account isn't affected.
        var otherPlayer = await CreatePlayerAsync(player.GameContext).ConfigureAwait(false);
        otherPlayer.Account = new Persistence.BasicModel.Account { Id = Guid.NewGuid(), LoginName = "other" };
        overview = await plugIn.GetOverviewAsync(otherPlayer).ConfigureAwait(false);
        Assert.That(overview!.Entries, Has.Count.EqualTo(1));
    }

    /// <summary>
    /// Tests that the daily period starts at the configured time of the current day.
    /// </summary>
    [Test]
    public void DailyPeriodStartsAtResetTimeOfCurrentDay()
    {
        var now = new DateTime(2026, 9, 25, 15, 0, 0, DateTimeKind.Utc);
        Assert.That(WeeklyPeriod.GetDailyPeriodStartUtc(now, new TimeOnly(3, 0), TimeZoneInfo.Utc), Is.EqualTo(new DateTime(2026, 9, 25, 3, 0, 0, DateTimeKind.Utc)));

        now = new DateTime(2026, 9, 25, 2, 0, 0, DateTimeKind.Utc);
        var start = WeeklyPeriod.GetDailyPeriodStartUtc(now, new TimeOnly(3, 0), TimeZoneInfo.Utc);
        Assert.That(start, Is.EqualTo(new DateTime(2026, 9, 24, 3, 0, 0, DateTimeKind.Utc)));
        Assert.That(WeeklyPeriod.GetNextDailyPeriodStartUtc(start, TimeZoneInfo.Utc), Is.EqualTo(new DateTime(2026, 9, 25, 3, 0, 0, DateTimeKind.Utc)));
    }

    /// <summary>
    /// Tests that the objectives of a sequential quest only make progress in their order, and that the quest is rewarded after the last one.
    /// </summary>
    [Test]
    public async Task SequentialStepsOnlyCountTheCurrentStepAsync()
    {
        var player = await CreatePlayerAsync().ConfigureAwait(false);
        var plugIn = CreatePlugIn(new InMemoryWeeklyQuestProgressRepository(), CreateStoryQuest(
            new WeeklyQuestObjective { ObjectiveType = WeeklyQuestObjectiveType.GainMasterLevels },
            new WeeklyQuestObjective { ObjectiveType = WeeklyQuestObjectiveType.GainResets, RequiredCount = 2 }));

        await plugIn.CharacterResetAsync(player, 1).ConfigureAwait(false);
        var entry = await GetEntryAsync(plugIn, player, "story").ConfigureAwait(false);
        Assert.That(entry.Objectives.Select(o => o.Count), Is.EqualTo(new[] { 0, 0 }));

        await plugIn.CharacterMasterLeveledUpAsync(player).ConfigureAwait(false);
        await plugIn.CharacterResetAsync(player, 2).ConfigureAwait(false);
        entry = await GetEntryAsync(plugIn, player, "story").ConfigureAwait(false);
        Assert.That(entry.Objectives.Select(o => o.Count), Is.EqualTo(new[] { 1, 1 }));
        Assert.That(entry.CurrentStep, Is.EqualTo(1));
        Assert.That(player.Money, Is.EqualTo(0));

        await plugIn.CharacterResetAsync(player, 3).ConfigureAwait(false);
        entry = await GetEntryAsync(plugIn, player, "story").ConfigureAwait(false);
        Assert.That(entry.IsRewarded, Is.True);
        Assert.That(player.Money, Is.EqualTo(1000));
    }

    /// <summary>
    /// Tests that one event makes progress in only one step of a sequential quest, even if the next step has the same type.
    /// </summary>
    [Test]
    public async Task OneEventAdvancesOnlyOneSequentialStepAsync()
    {
        var player = await CreatePlayerAsync().ConfigureAwait(false);
        var plugIn = CreatePlugIn(new InMemoryWeeklyQuestProgressRepository(), CreateStoryQuest(
            new WeeklyQuestObjective { ObjectiveType = WeeklyQuestObjectiveType.GainResets },
            new WeeklyQuestObjective { ObjectiveType = WeeklyQuestObjectiveType.GainResets }));

        await plugIn.CharacterResetAsync(player, 1).ConfigureAwait(false);
        var entry = await GetEntryAsync(plugIn, player, "story").ConfigureAwait(false);
        Assert.That(entry.Count, Is.EqualTo(1));
        Assert.That(entry.IsCompleted, Is.False);

        await plugIn.CharacterResetAsync(player, 2).ConfigureAwait(false);
        entry = await GetEntryAsync(plugIn, player, "story").ConfigureAwait(false);
        Assert.That(entry.IsCompleted, Is.True);
    }

    /// <summary>
    /// Tests that the objectives of a quest which isn't sequential make progress in any order.
    /// </summary>
    [Test]
    public async Task ParallelObjectivesProgressInAnyOrderAsync()
    {
        var player = await CreatePlayerAsync().ConfigureAwait(false);
        var quest = CreateStoryQuest(
            new WeeklyQuestObjective { ObjectiveType = WeeklyQuestObjectiveType.GainResets },
            new WeeklyQuestObjective { ObjectiveType = WeeklyQuestObjectiveType.GainMasterLevels });
        quest.SequentialObjectives = false;
        var plugIn = CreatePlugIn(new InMemoryWeeklyQuestProgressRepository(), quest);

        await plugIn.CharacterMasterLeveledUpAsync(player).ConfigureAwait(false);
        var entry = await GetEntryAsync(plugIn, player, "story").ConfigureAwait(false);
        Assert.That(entry.Objectives.Select(o => o.IsDone), Is.EqualTo(new[] { false, true }));

        await plugIn.CharacterResetAsync(player, 1).ConfigureAwait(false);
        Assert.That(player.Money, Is.EqualTo(1000));
    }

    /// <summary>
    /// Tests that talking to the configured NPC counts, and that such an NPC doesn't show "not implemented".
    /// </summary>
    [Test]
    public async Task TalkToNpcCountsTheConfiguredNpcAsync()
    {
        var player = await CreatePlayerAsync().ConfigureAwait(false);
        var guardian = new MonsterDefinition { Number = 240 };
        var plugIn = CreatePlugIn(new InMemoryWeeklyQuestProgressRepository(), CreateStoryQuest(
            new WeeklyQuestObjective { ObjectiveType = WeeklyQuestObjectiveType.TalkToNpc, Monster = guardian }));

        var otherNpc = new NonPlayerCharacter(null!, new MonsterDefinition { Number = 241 }, null!);
        await plugIn.NpcTalkStartedAsync(player, otherNpc).ConfigureAwait(false);
        Assert.That(player.Money, Is.EqualTo(0));

        var otherArgs = new NpcTalkEventArgs();
        await plugIn.PlayerTalksToNpcAsync(player, otherNpc, otherArgs).ConfigureAwait(false);
        Assert.That(otherArgs.HasBeenHandled, Is.False);

        var guardianNpc = new NonPlayerCharacter(null!, guardian, null!);
        var guardianArgs = new NpcTalkEventArgs();
        await plugIn.PlayerTalksToNpcAsync(player, guardianNpc, guardianArgs).ConfigureAwait(false);
        Assert.That(guardianArgs.HasBeenHandled, Is.True);

        await plugIn.NpcTalkStartedAsync(player, guardianNpc).ConfigureAwait(false);
        Assert.That(player.Money, Is.EqualTo(1000));
    }

    /// <summary>
    /// Tests that entering a map counts, but being added to the same map again (e.g. after a respawn) doesn't.
    /// </summary>
    [Test]
    public async Task EnterMapCountsOncePerMapChangeAsync()
    {
        var player = await CreatePlayerAsync().ConfigureAwait(false);
        var map = (await player.GameContext.GetMapAsync(0).ConfigureAwait(false))!;
        var quest = CreateStoryQuest(new WeeklyQuestObjective
        {
            ObjectiveType = WeeklyQuestObjectiveType.EnterMap,
            Map = map.Definition,
            RequiredCount = 2,
        });
        var plugIn = CreatePlugIn(new InMemoryWeeklyQuestProgressRepository(), quest);

        await plugIn.ObjectAddedToMapAsync(map, player).ConfigureAwait(false);
        await plugIn.ObjectAddedToMapAsync(map, player).ConfigureAwait(false);

        var entry = await GetEntryAsync(plugIn, player, "story").ConfigureAwait(false);
        Assert.That(entry.Count, Is.EqualTo(1));
    }

    /// <summary>
    /// Tests that the progress of a daily quest is stored in the daily period.
    /// </summary>
    [Test]
    public async Task DailyQuestIsStoredInDailyPeriodAsync()
    {
        var repository = new InMemoryWeeklyQuestProgressRepository();
        var player = await CreatePlayerAsync().ConfigureAwait(false);
        var plugIn = CreatePlugIn(repository);
        plugIn.Configuration!.Quests.Single().Period = QuestPeriod.Daily;

        await plugIn.CharacterResetAsync(player, 1).ConfigureAwait(false);
        await plugIn.PlayerStateChangedAsync(player, PlayerState.EnteredWorld, PlayerState.CharacterSelection).ConfigureAwait(false);

        var dailyStart = WeeklyPeriod.GetDailyPeriodStartUtc(DateTime.UtcNow, TimeOnly.MinValue, TimeZoneInfo.Utc);
        var stored = await repository.LoadAsync(player.SelectedCharacter!.GetId(), new[] { dailyStart }).ConfigureAwait(false);
        Assert.That(stored.Single().Count, Is.EqualTo(1));
    }

    /// <summary>
    /// Tests that a completed story chapter stays completed, and unlocks the next chapter.
    /// </summary>
    [Test]
    public async Task CompletedOnceQuestUnlocksNextChapterAsync()
    {
        var repository = new InMemoryWeeklyQuestProgressRepository();
        var player = await CreatePlayerAsync().ConfigureAwait(false);
        var chapter1 = CreateStoryQuest(new WeeklyQuestObjective { ObjectiveType = WeeklyQuestObjectiveType.GainResets });
        var chapter2 = CreateStoryQuest(new WeeklyQuestObjective { ObjectiveType = WeeklyQuestObjectiveType.GainResets });
        chapter2.Id = "story-2";
        chapter2.PrerequisiteQuestId = chapter1.Id;
        var plugIn = CreatePlugIn(repository, chapter1, chapter2);
        await repository.SaveAsync(new[]
        {
            new WeeklyQuestProgress
            {
                CharacterId = player.SelectedCharacter!.GetId(),
                PeriodStart = WeeklyPeriod.OncePeriodStartUtc,
                QuestId = chapter1.Id,
                Count = 1,
                CompletedAt = DateTime.UtcNow.AddDays(-30),
                RewardedAt = DateTime.UtcNow.AddDays(-30),
            },
        }).ConfigureAwait(false);

        var overview = await plugIn.GetOverviewAsync(player).ConfigureAwait(false);

        Assert.That(overview!.Entries.Select(e => e.Quest.Id), Is.EqualTo(new[] { "story", "story-2" }));
        Assert.That(overview.Entries[0].IsRewarded, Is.True);

        // The completed chapter doesn't make progress anymore, the unlocked one does.
        await plugIn.CharacterResetAsync(player, 1).ConfigureAwait(false);
        Assert.That(player.Money, Is.EqualTo(1000));
        Assert.That((await GetEntryAsync(plugIn, player, "story-2").ConfigureAwait(false)).IsRewarded, Is.True);
    }

    /// <summary>
    /// Tests that stored progress of another period of a quest (e.g. after its period was changed in the configuration) is ignored.
    /// </summary>
    [Test]
    public async Task ProgressOfAnotherPeriodIsIgnoredAsync()
    {
        var repository = new InMemoryWeeklyQuestProgressRepository();
        var player = await CreatePlayerAsync().ConfigureAwait(false);
        var plugIn = CreatePlugIn(repository);
        var quest = plugIn.Configuration!.Quests.Single();
        quest.Period = QuestPeriod.Once;
        var weekStart = WeeklyPeriod.GetPeriodStartUtc(DateTime.UtcNow, DayOfWeek.Monday, TimeOnly.MinValue, TimeZoneInfo.Utc);
        await repository.SaveAsync(new[]
        {
            new WeeklyQuestProgress { CharacterId = player.SelectedCharacter!.GetId(), PeriodStart = weekStart, QuestId = quest.Id, Count = 1 },
        }).ConfigureAwait(false);

        Assert.That(await GetCountAsync(plugIn, player, quest.Id).ConfigureAwait(false), Is.EqualTo(0));
    }

    /// <summary>
    /// Tests that the progress is loaded once, and not again for each event.
    /// </summary>
    [Test]
    public async Task ProgressIsLoadedOnceAsync()
    {
        var repository = new InMemoryWeeklyQuestProgressRepository();
        var player = await CreatePlayerAsync().ConfigureAwait(false);
        var plugIn = CreatePlugIn(repository);
        plugIn.Configuration!.Quests.Single().RequiredCount = 100;

        for (var i = 0; i < 10; i++)
        {
            await plugIn.CharacterResetAsync(player, i).ConfigureAwait(false);
        }

        Assert.That(repository.LoadCount, Is.EqualTo(1));
        Assert.That(await GetCountAsync(plugIn, player, QuestId).ConfigureAwait(false), Is.EqualTo(10));
    }

    /// <summary>
    /// Tests that the purge deletes the progress of old periods, but keeps the one of the quests which are done once.
    /// </summary>
    [Test]
    public async Task PurgeKeepsQuestsWhichAreDoneOnceAsync()
    {
        var repository = new InMemoryWeeklyQuestProgressRepository();
        var characterId = Guid.NewGuid();
        var old = DateTime.UtcNow.AddDays(-100);
        var current = DateTime.UtcNow.AddDays(-1);
        await repository.SaveAsync(new[]
        {
            new WeeklyQuestProgress { CharacterId = characterId, PeriodStart = old, QuestId = "old" },
            new WeeklyQuestProgress { CharacterId = characterId, PeriodStart = current, QuestId = "current" },
            new WeeklyQuestProgress { CharacterId = characterId, PeriodStart = WeeklyPeriod.OncePeriodStartUtc, QuestId = "story" },
        }).ConfigureAwait(false);

        var deleted = await repository.DeleteExpiredAsync(DateTime.UtcNow.AddDays(-56), WeeklyPeriod.OncePeriodStartUtc).ConfigureAwait(false);

        Assert.That(deleted, Is.EqualTo(1));
        var remaining = await repository.LoadAsync(characterId, new[] { old, current, WeeklyPeriod.OncePeriodStartUtc }).ConfigureAwait(false);
        Assert.That(remaining.Select(p => p.QuestId), Is.EquivalentTo(new[] { "current", "story" }));
    }

    private static WeeklyQuestDefinition CreateStoryQuest(params WeeklyQuestObjective[] objectives)
    {
        return new WeeklyQuestDefinition
        {
            Id = "story",
            Name = "Capítulo I",
            Category = QuestCategory.Main,
            Period = QuestPeriod.Once,
            SequentialObjectives = true,
            Objectives = objectives.ToList(),
            Rewards = new List<WeeklyQuestReward>
            {
                new() { RewardType = WeeklyQuestRewardType.Money, Amount = 1000 },
            },
        };
    }

    private static async ValueTask<WeeklyQuestOverviewEntry> GetEntryAsync(WeeklyQuestsPlugIn plugIn, Player player, string questId)
    {
        var overview = await plugIn.GetOverviewAsync(player).ConfigureAwait(false);
        return overview!.Entries.Single(e => e.Quest.Id == questId);
    }

    private static async ValueTask<int> GetCountAsync(WeeklyQuestsPlugIn plugIn, Player player, string questId)
    {
        var overview = await plugIn.GetOverviewAsync(player).ConfigureAwait(false);
        return overview!.Entries.Single(e => e.Quest.Id == questId).Count;
    }

    private static WeeklyQuestsPlugIn CreatePlugIn(IWeeklyQuestProgressRepository repository, params WeeklyQuestDefinition[] quests)
    {
        return new WeeklyQuestsPlugIn(repository)
        {
            Configuration = new WeeklyQuestsConfiguration { Quests = quests.ToList() },
        };
    }

    private static (CharacterClass Wizard, CharacterClass SoulMaster, CharacterClass GrandMaster) CreateWizardLine()
    {
        var grandMaster = new CharacterClass { Number = 3 };
        var soulMaster = new CharacterClass { Number = 1, NextGenerationClass = grandMaster };
        var wizard = new CharacterClass { Number = 0, NextGenerationClass = soulMaster };
        return (wizard, soulMaster, grandMaster);
    }

    private static WeeklyQuestsPlugIn CreatePlugIn(IWeeklyQuestProgressRepository repository)
    {
        return new WeeklyQuestsPlugIn(repository)
        {
            Configuration = new WeeklyQuestsConfiguration
            {
                Quests = new List<WeeklyQuestDefinition>
                {
                    new()
                    {
                        Id = QuestId,
                        Name = "Resets",
                        ObjectiveType = WeeklyQuestObjectiveType.GainResets,
                        RequiredCount = 2,
                        Rewards = new List<WeeklyQuestReward>
                        {
                            new() { RewardType = WeeklyQuestRewardType.Money, Amount = 1000 },
                        },
                    },
                },
            },
        };
    }

    private static async ValueTask<Player> CreatePlayerAsync(IGameContext? gameContext = null)
    {
        var player = gameContext is null
            ? await PlayerTestHelper.CreatePlayerAsync().ConfigureAwait(false)
            : await PlayerTestHelper.CreatePlayerAsync(gameContext).ConfigureAwait(false);
        player.GameContext.Configuration.MaximumInventoryMoney = int.MaxValue;
        await player.PlayerState.TryAdvanceToAsync(PlayerState.EnteredWorld).ConfigureAwait(false);
        return player;
    }
}
