// <copyright file="WeeklyQuestsPlugInTest.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.Tests;

using MUnique.OpenMU.GameLogic;
using MUnique.OpenMU.GameLogic.Attributes;
using MUnique.OpenMU.GameLogic.PlugIns.WeeklyQuests;
using MUnique.OpenMU.Persistence.WeeklyQuests;

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

    private static async ValueTask<Player> CreatePlayerAsync()
    {
        var player = await PlayerTestHelper.CreatePlayerAsync().ConfigureAwait(false);
        player.GameContext.Configuration.MaximumInventoryMoney = int.MaxValue;
        await player.PlayerState.TryAdvanceToAsync(PlayerState.EnteredWorld).ConfigureAwait(false);
        return player;
    }
}
