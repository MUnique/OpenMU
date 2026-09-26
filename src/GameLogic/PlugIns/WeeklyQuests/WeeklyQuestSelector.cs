// <copyright file="WeeklyQuestSelector.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.GameLogic.PlugIns.WeeklyQuests;

using System.Text;
using MUnique.OpenMU.Persistence.WeeklyQuests;

/// <summary>
/// Decides which quests are part of the current periods and which of them are available for a character.
/// </summary>
/// <remarks>
/// It doesn't depend on a <see cref="Player"/>, so that it can be used for characters which are not in the game, too.
/// </remarks>
public static class WeeklyQuestSelector
{
    /// <summary>
    /// The identifier of the progress of the <see cref="WeeklyQuestsConfiguration.AllCompletedBonus"/>.
    /// </summary>
    public const string AllCompletedBonusId = "__all__";

    /// <summary>
    /// Gets the quests of the current periods: all active quests, or the drawn ones of the daily and weekly quests when they rotate.
    /// </summary>
    /// <param name="configuration">The configuration.</param>
    /// <param name="periods">The starts of the current periods.</param>
    /// <returns>The quests of the periods, in the configured order.</returns>
    public static IReadOnlyList<WeeklyQuestDefinition> GetQuestsOfPeriod(WeeklyQuestsConfiguration configuration, QuestPeriodStarts periods)
    {
        var active = configuration.Quests
            .Where(q => q.IsActive && !string.IsNullOrWhiteSpace(q.Id) && q.Id != AllCompletedBonusId)
            .ToList();
        var drawnWeekly = Draw(active, QuestPeriod.Weekly, configuration.QuestsPerWeek, periods.Weekly);
        var drawnDaily = Draw(active, QuestPeriod.Daily, configuration.QuestsPerDay, periods.Daily);
        if (drawnWeekly is null && drawnDaily is null)
        {
            return active;
        }

        return active.Where(q => q.AlwaysIncluded || q.Period switch
        {
            QuestPeriod.Weekly => drawnWeekly?.Contains(q) ?? true,
            QuestPeriod.Daily => drawnDaily?.Contains(q) ?? true,
            _ => true,
        }).ToList();
    }

    /// <summary>
    /// Determines whether the quest is available for the character, so it's shown and can make progress.
    /// </summary>
    /// <param name="quest">The quest.</param>
    /// <param name="character">The character.</param>
    /// <param name="hasProgress">If set to <c>true</c>, the character already made progress in the quest.</param>
    /// <param name="isBlockedByAccount">If set to <c>true</c>, another character of the account already received the rewards of the quest.</param>
    /// <param name="isLocked">If set to <c>true</c>, the <see cref="WeeklyQuestDefinition.PrerequisiteQuestId"/> is not completed yet.</param>
    /// <returns><c>true</c>, if the quest is available.</returns>
    /// <remarks>
    /// The minimum level and resets are not checked here: such quests are shown, but don't make progress yet.
    /// </remarks>
    public static bool IsAvailable(WeeklyQuestDefinition quest, WeeklyQuestCharacterInfo character, bool hasProgress, bool isBlockedByAccount, bool isLocked = false)
    {
        if (isBlockedByAccount)
        {
            return false;
        }

        if (hasProgress)
        {
            // A quest which was started isn't hidden, even if the character outgrew it meanwhile.
            return true;
        }

        if (isLocked || !quest.IsQualified(character.Class))
        {
            return false;
        }

        return (quest.MaximumLevel <= 0 || character.Level <= quest.MaximumLevel)
               && (quest.MaximumResets <= 0 || character.Resets <= quest.MaximumResets);
    }

    /// <summary>
    /// Determines whether the prerequisite of the quest is not completed yet.
    /// </summary>
    /// <param name="quest">The quest.</param>
    /// <param name="progress">The progress of the character in the current periods, per quest id.</param>
    /// <returns><c>true</c>, if the quest is locked.</returns>
    public static bool IsLocked(WeeklyQuestDefinition quest, IReadOnlyDictionary<string, WeeklyQuestProgress> progress)
    {
        return !string.IsNullOrWhiteSpace(quest.PrerequisiteQuestId)
               && !(progress.TryGetValue(quest.PrerequisiteQuestId, out var p) && p.CompletedAt is not null);
    }

    /// <summary>
    /// Gets the quests of the current periods which are available for the character.
    /// </summary>
    /// <param name="configuration">The configuration.</param>
    /// <param name="periods">The starts of the current periods.</param>
    /// <param name="character">The character.</param>
    /// <param name="progress">The progress of the character in the current periods, per quest id.</param>
    /// <param name="rewardedByOtherCharacters">The ids of the quests whose rewards another character of the account received in the current periods.</param>
    /// <returns>The available quests.</returns>
    public static List<WeeklyQuestDefinition> GetAvailableQuests(
        WeeklyQuestsConfiguration configuration,
        QuestPeriodStarts periods,
        WeeklyQuestCharacterInfo character,
        IReadOnlyDictionary<string, WeeklyQuestProgress> progress,
        IReadOnlySet<string> rewardedByOtherCharacters)
    {
        return GetQuestsOfPeriod(configuration, periods)
            .Where(quest => IsAvailable(
                quest,
                character,
                progress.TryGetValue(quest.Id, out var p) && p.HasAnyProgress,
                quest.OncePerAccount && rewardedByOtherCharacters.Contains(quest.Id),
                IsLocked(quest, progress)))
            .ToList();
    }

    /// <summary>
    /// Creates the overview entries of the available quests of the character, including the bonus for completing all weekly quests.
    /// </summary>
    /// <param name="configuration">The configuration.</param>
    /// <param name="periods">The starts of the current periods.</param>
    /// <param name="character">The character.</param>
    /// <param name="progress">The progress of the character in the current periods, per quest id.</param>
    /// <param name="rewardedByOtherCharacters">The ids of the quests whose rewards another character of the account received in the current periods.</param>
    /// <returns>The entries.</returns>
    public static List<WeeklyQuestOverviewEntry> CreateEntries(
        WeeklyQuestsConfiguration configuration,
        QuestPeriodStarts periods,
        WeeklyQuestCharacterInfo character,
        IReadOnlyDictionary<string, WeeklyQuestProgress> progress,
        IReadOnlySet<string> rewardedByOtherCharacters)
    {
        return CreateEntries(configuration, GetAvailableQuests(configuration, periods, character, progress, rewardedByOtherCharacters), progress);
    }

    /// <summary>
    /// Creates the overview entries of the specified available quests, including the bonus for completing all weekly quests.
    /// </summary>
    /// <param name="configuration">The configuration.</param>
    /// <param name="availableQuests">The available quests of the character.</param>
    /// <param name="progress">The progress of the character in the current periods, per quest id.</param>
    /// <returns>The entries.</returns>
    public static List<WeeklyQuestOverviewEntry> CreateEntries(
        WeeklyQuestsConfiguration configuration,
        IReadOnlyList<WeeklyQuestDefinition> availableQuests,
        IReadOnlyDictionary<string, WeeklyQuestProgress> progress)
    {
        var entries = availableQuests.Select(quest => CreateEntry(quest, progress)).ToList();
        var weeklyCount = CountBonusQuests(availableQuests);
        if (weeklyCount > 0 && CreateAllCompletedBonusQuest(configuration, weeklyCount) is { } bonusQuest)
        {
            entries.Add(CreateEntry(bonusQuest, progress));
        }

        return entries;
    }

    /// <summary>
    /// Counts the quests which have to be completed for the <see cref="WeeklyQuestsConfiguration.AllCompletedBonus"/>, i.e. the weekly ones.
    /// </summary>
    /// <param name="availableQuests">The available quests of the character.</param>
    /// <returns>The number of the quests.</returns>
    public static int CountBonusQuests(IEnumerable<WeeklyQuestDefinition> availableQuests)
    {
        return availableQuests.Count(q => q.Period == QuestPeriod.Weekly);
    }

    /// <summary>
    /// Creates the overview entry of a quest.
    /// </summary>
    /// <param name="quest">The quest.</param>
    /// <param name="progress">The progress of the character in the current periods, per quest id.</param>
    /// <returns>The entry.</returns>
    public static WeeklyQuestOverviewEntry CreateEntry(WeeklyQuestDefinition quest, IReadOnlyDictionary<string, WeeklyQuestProgress> progress)
    {
        progress.TryGetValue(quest.Id, out var p);
        var isCompleted = p?.CompletedAt is not null;
        var objectives = quest.GetObjectives();
        var objectiveProgress = new WeeklyQuestObjectiveProgress[objectives.Count];
        var currentStep = -1;
        for (var i = 0; i < objectives.Count; i++)
        {
            var required = Math.Max(1, objectives[i].RequiredCount);
            var count = Math.Min(p?.GetCount(i) ?? 0, required);
            var isDone = isCompleted || count >= required;
            if (!isDone && currentStep < 0)
            {
                currentStep = i;
            }

            objectiveProgress[i] = new WeeklyQuestObjectiveProgress(objectives[i], count, required, isDone);
        }

        if (currentStep < 0)
        {
            currentStep = objectives.Count;
        }

        var (entryCount, entryRequired) = objectives.Count == 1
            ? (objectiveProgress[0].Count, objectiveProgress[0].Required)
            : (objectiveProgress.Count(o => o.IsDone), objectives.Count);
        return new WeeklyQuestOverviewEntry(quest, entryCount, entryRequired, isCompleted, p?.RewardedAt is not null, currentStep, objectiveProgress);
    }

    /// <summary>
    /// Determines whether the character meets the minimum requirements to make progress in the quest.
    /// </summary>
    /// <param name="quest">The quest.</param>
    /// <param name="character">The character.</param>
    /// <returns><c>true</c>, if the character can make progress.</returns>
    public static bool MeetsMinimumRequirements(WeeklyQuestDefinition quest, WeeklyQuestCharacterInfo character)
    {
        return character.Level >= quest.MinimumLevel && character.Resets >= quest.MinimumResets;
    }

    /// <summary>
    /// Determines whether the configuration has a bonus for completing all quests.
    /// </summary>
    /// <param name="configuration">The configuration.</param>
    /// <returns><c>true</c>, if there is a bonus.</returns>
    public static bool HasAllCompletedBonus(WeeklyQuestsConfiguration configuration)
    {
        return configuration.AllCompletedBonus is { Rewards.Count: > 0 };
    }

    /// <summary>
    /// Creates the quest which represents the <see cref="WeeklyQuestsConfiguration.AllCompletedBonus"/>,
    /// with the number of the quests which have to be completed as <see cref="WeeklyQuestDefinition.RequiredCount"/>.
    /// </summary>
    /// <param name="configuration">The configuration.</param>
    /// <param name="questCount">The number of the weekly quests of the character.</param>
    /// <returns>The quest; <c>null</c>, if there is no bonus.</returns>
    public static WeeklyQuestDefinition? CreateAllCompletedBonusQuest(WeeklyQuestsConfiguration configuration, int questCount)
    {
        if (!HasAllCompletedBonus(configuration) || configuration.AllCompletedBonus is not { } bonus)
        {
            return null;
        }

        return new WeeklyQuestDefinition
        {
            Id = AllCompletedBonusId,
            Name = string.IsNullOrWhiteSpace(bonus.Name) ? "Bonus semanal" : bonus.Name,
            Description = bonus.Description,
            Category = QuestCategory.Weekly,
            Period = QuestPeriod.Weekly,
            RequiredCount = Math.Max(1, questCount),
            Rewards = bonus.Rewards,
        };
    }

    private static HashSet<WeeklyQuestDefinition>? Draw(List<WeeklyQuestDefinition> active, QuestPeriod period, int count, DateTime periodStartUtc)
    {
        if (count <= 0)
        {
            return null;
        }

        // Ordering by a hash of the id and the period gives each period another selection, which is the same
        // on all servers and after restarts. Adding a quest only changes it, if the new one gets into the drawing.
        return active
            .Where(q => q.Period == period && !q.AlwaysIncluded)
            .OrderBy(q => GetStableHash(q.Id, periodStartUtc))
            .ThenBy(q => q.Id, StringComparer.Ordinal)
            .Take(count)
            .ToHashSet();
    }

    /// <summary>
    /// Gets a hash which is the same on all processes, unlike <see cref="string.GetHashCode()"/>.
    /// </summary>
    private static uint GetStableHash(string questId, DateTime periodStartUtc)
    {
        // FNV-1a
        var hash = 2166136261;
        foreach (var b in Encoding.UTF8.GetBytes($"{questId}|{periodStartUtc.Ticks}"))
        {
            hash = (hash ^ b) * 16777619;
        }

        return hash;
    }
}
