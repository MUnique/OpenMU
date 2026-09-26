// <copyright file="WeeklyQuestSelectorTest.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.Tests;

using MUnique.OpenMU.DataModel.Configuration;
using MUnique.OpenMU.GameLogic.PlugIns.WeeklyQuests;

/// <summary>
/// Tests for the <see cref="WeeklyQuestSelector"/>.
/// </summary>
[TestFixture]
public class WeeklyQuestSelectorTest
{
    private static readonly DateTime Week1 = new(2026, 9, 21, 3, 0, 0, DateTimeKind.Utc);

    private static readonly WeeklyQuestCharacterInfo Character = new(new CharacterClass { Number = 0 }, 100, 5);

    /// <summary>
    /// Tests that without rotation, all active quests are part of the period.
    /// </summary>
    [Test]
    public void WithoutRotationAllActiveQuestsAreSelected()
    {
        var configuration = CreateConfiguration(10);
        configuration.Quests.First().IsActive = false;

        var quests = WeeklyQuestSelector.GetQuestsOfPeriod(configuration, Week1);

        Assert.That(quests, Has.Count.EqualTo(9));
    }

    /// <summary>
    /// Tests that the rotation draws the configured number of quests, plus the ones which are always included,
    /// and that the drawing is the same for the same week.
    /// </summary>
    [Test]
    public void RotationDrawsQuestsDeterministically()
    {
        var configuration = CreateConfiguration(10);
        configuration.QuestsPerWeek = 3;
        configuration.Quests.First().AlwaysIncluded = true;

        var quests = WeeklyQuestSelector.GetQuestsOfPeriod(configuration, Week1);
        var again = WeeklyQuestSelector.GetQuestsOfPeriod(configuration, Week1);

        Assert.That(quests, Has.Count.EqualTo(4));
        Assert.That(quests, Does.Contain(configuration.Quests.First()));
        Assert.That(again, Is.EqualTo(quests));
    }

    /// <summary>
    /// Tests that the rotation draws other quests in other weeks.
    /// </summary>
    [Test]
    public void RotationChangesBetweenWeeks()
    {
        var configuration = CreateConfiguration(20);
        configuration.QuestsPerWeek = 3;

        var selections = Enumerable.Range(0, 8)
            .Select(week => string.Join(",", WeeklyQuestSelector.GetQuestsOfPeriod(configuration, Week1.AddDays(7 * week)).Select(q => q.Id)))
            .ToHashSet();

        Assert.That(selections, Has.Count.GreaterThan(1));
    }

    /// <summary>
    /// Tests that the maximum level and resets hide a quest, unless the character already made progress in it.
    /// </summary>
    [Test]
    public void MaximumsHideQuestWithoutProgress()
    {
        var quest = new WeeklyQuestDefinition { Id = "q", MaximumLevel = 50 };
        Assert.That(WeeklyQuestSelector.IsAvailable(quest, Character, false, false), Is.False);
        Assert.That(WeeklyQuestSelector.IsAvailable(quest, Character, true, false), Is.True);

        quest = new WeeklyQuestDefinition { Id = "q", MaximumResets = 4 };
        Assert.That(WeeklyQuestSelector.IsAvailable(quest, Character, false, false), Is.False);

        quest = new WeeklyQuestDefinition { Id = "q", MaximumLevel = 100, MaximumResets = 5 };
        Assert.That(WeeklyQuestSelector.IsAvailable(quest, Character, false, false), Is.True);
    }

    /// <summary>
    /// Tests that a quest which was rewarded to another character of the account is never available.
    /// </summary>
    [Test]
    public void QuestBlockedByAccountIsNotAvailable()
    {
        var quest = new WeeklyQuestDefinition { Id = "q", OncePerAccount = true };
        Assert.That(WeeklyQuestSelector.IsAvailable(quest, Character, true, true), Is.False);
    }

    /// <summary>
    /// Tests that the bonus quest requires the number of quests and uses the configured rewards.
    /// </summary>
    [Test]
    public void AllCompletedBonusQuestIsCreatedOnlyWithRewards()
    {
        var configuration = CreateConfiguration(1);
        Assert.That(WeeklyQuestSelector.CreateAllCompletedBonusQuest(configuration, 3), Is.Null);

        configuration.AllCompletedBonus = new WeeklyQuestDefinition { Name = "Bonus" };
        Assert.That(WeeklyQuestSelector.CreateAllCompletedBonusQuest(configuration, 3), Is.Null);

        configuration.AllCompletedBonus.Rewards.Add(new WeeklyQuestReward { RewardType = WeeklyQuestRewardType.Money, Amount = 1 });
        var bonus = WeeklyQuestSelector.CreateAllCompletedBonusQuest(configuration, 3);
        Assert.That(bonus!.Id, Is.EqualTo(WeeklyQuestSelector.AllCompletedBonusId));
        Assert.That(bonus.RequiredCount, Is.EqualTo(3));
        Assert.That(bonus.Rewards, Has.Count.EqualTo(1));
    }

    private static WeeklyQuestsConfiguration CreateConfiguration(int questCount)
    {
        return new WeeklyQuestsConfiguration
        {
            Quests = Enumerable.Range(0, questCount)
                .Select(i => new WeeklyQuestDefinition { Id = $"q{i}", Name = $"Quest {i}" })
                .ToList(),
        };
    }
}
