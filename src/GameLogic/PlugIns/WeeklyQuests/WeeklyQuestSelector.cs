// <copyright file="WeeklyQuestSelector.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.GameLogic.PlugIns.WeeklyQuests;

using System.Text;
using MUnique.OpenMU.Persistence.WeeklyQuests;

/// <summary>
/// Decides which weekly quests are part of a period and which of them are available for a character.
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
    /// Gets the quests of the period: all active quests, or the drawn ones when the quests rotate.
    /// </summary>
    /// <param name="configuration">The configuration.</param>
    /// <param name="periodStartUtc">The start of the period.</param>
    /// <returns>The quests of the period, in the configured order.</returns>
    public static IReadOnlyList<WeeklyQuestDefinition> GetQuestsOfPeriod(WeeklyQuestsConfiguration configuration, DateTime periodStartUtc)
    {
        var active = configuration.Quests
            .Where(q => q.IsActive && !string.IsNullOrWhiteSpace(q.Id) && q.Id != AllCompletedBonusId)
            .ToList();
        if (configuration.QuestsPerWeek <= 0)
        {
            return active;
        }

        // Ordering by a hash of the id and the period gives each week another selection, which is the same
        // on all servers and after restarts. Adding a quest only changes it, if the new one gets into the drawing.
        var drawn = active
            .Where(q => !q.AlwaysIncluded)
            .OrderBy(q => GetStableHash(q.Id, periodStartUtc))
            .ThenBy(q => q.Id, StringComparer.Ordinal)
            .Take(configuration.QuestsPerWeek)
            .ToHashSet();
        return active.Where(q => q.AlwaysIncluded || drawn.Contains(q)).ToList();
    }

    /// <summary>
    /// Determines whether the quest is available for the character, so it's shown and can make progress.
    /// </summary>
    /// <param name="quest">The quest.</param>
    /// <param name="character">The character.</param>
    /// <param name="hasProgress">If set to <c>true</c>, the character already made progress in the quest.</param>
    /// <param name="isBlockedByAccount">If set to <c>true</c>, another character of the account already received the rewards of the quest.</param>
    /// <returns><c>true</c>, if the quest is available.</returns>
    /// <remarks>
    /// The minimum level and resets are not checked here: such quests are shown, but don't make progress yet.
    /// </remarks>
    public static bool IsAvailable(WeeklyQuestDefinition quest, WeeklyQuestCharacterInfo character, bool hasProgress, bool isBlockedByAccount)
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

        if (!quest.IsQualified(character.Class))
        {
            return false;
        }

        return (quest.MaximumLevel <= 0 || character.Level <= quest.MaximumLevel)
               && (quest.MaximumResets <= 0 || character.Resets <= quest.MaximumResets);
    }

    /// <summary>
    /// Gets the quests of the period which are available for the character.
    /// </summary>
    /// <param name="configuration">The configuration.</param>
    /// <param name="periodStartUtc">The start of the period.</param>
    /// <param name="character">The character.</param>
    /// <param name="progress">The progress of the character in the period, per quest id.</param>
    /// <param name="rewardedByOtherCharacters">The ids of the quests whose rewards another character of the account received in the period.</param>
    /// <returns>The available quests.</returns>
    public static List<WeeklyQuestDefinition> GetAvailableQuests(
        WeeklyQuestsConfiguration configuration,
        DateTime periodStartUtc,
        WeeklyQuestCharacterInfo character,
        IReadOnlyDictionary<string, WeeklyQuestProgress> progress,
        IReadOnlySet<string> rewardedByOtherCharacters)
    {
        return GetQuestsOfPeriod(configuration, periodStartUtc)
            .Where(quest => IsAvailable(
                quest,
                character,
                progress.TryGetValue(quest.Id, out var p) && p.Count > 0,
                quest.OncePerAccount && rewardedByOtherCharacters.Contains(quest.Id)))
            .ToList();
    }

    /// <summary>
    /// Creates the overview entries of the available quests of the character, including the bonus for completing all of them.
    /// </summary>
    /// <param name="configuration">The configuration.</param>
    /// <param name="periodStartUtc">The start of the period.</param>
    /// <param name="character">The character.</param>
    /// <param name="progress">The progress of the character in the period, per quest id.</param>
    /// <param name="rewardedByOtherCharacters">The ids of the quests whose rewards another character of the account received in the period.</param>
    /// <returns>The entries.</returns>
    public static List<WeeklyQuestOverviewEntry> CreateEntries(
        WeeklyQuestsConfiguration configuration,
        DateTime periodStartUtc,
        WeeklyQuestCharacterInfo character,
        IReadOnlyDictionary<string, WeeklyQuestProgress> progress,
        IReadOnlySet<string> rewardedByOtherCharacters)
    {
        var quests = GetAvailableQuests(configuration, periodStartUtc, character, progress, rewardedByOtherCharacters);
        var entries = quests.Select(quest => CreateEntry(quest, progress)).ToList();
        if (quests.Count > 0 && CreateAllCompletedBonusQuest(configuration, quests.Count) is { } bonusQuest)
        {
            entries.Add(CreateEntry(bonusQuest, progress));
        }

        return entries;
    }

    /// <summary>
    /// Creates the overview entry of a quest.
    /// </summary>
    /// <param name="quest">The quest.</param>
    /// <param name="progress">The progress of the character in the period, per quest id.</param>
    /// <returns>The entry.</returns>
    public static WeeklyQuestOverviewEntry CreateEntry(WeeklyQuestDefinition quest, IReadOnlyDictionary<string, WeeklyQuestProgress> progress)
    {
        progress.TryGetValue(quest.Id, out var p);
        return new WeeklyQuestOverviewEntry(quest, p?.Count ?? 0, p?.CompletedAt is not null, p?.RewardedAt is not null);
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
    /// <param name="questCount">The number of the quests of the character.</param>
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
            RequiredCount = Math.Max(1, questCount),
            Rewards = bonus.Rewards,
        };
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
