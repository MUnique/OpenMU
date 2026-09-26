// <copyright file="InMemoryWeeklyQuestProgressRepository.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.Persistence.WeeklyQuests;

using System.Collections.Concurrent;
using System.Threading;

/// <summary>
/// A <see cref="IWeeklyQuestProgressRepository"/> which keeps the progress only in memory.
/// It's used when no database backed repository is registered, e.g. for the demo mode and tests.
/// </summary>
public class InMemoryWeeklyQuestProgressRepository : IWeeklyQuestProgressRepository
{
    private readonly ConcurrentDictionary<(Guid CharacterId, DateTime PeriodStart, string QuestId), WeeklyQuestProgress> _entries = new();

    /// <inheritdoc />
    public ValueTask<IList<WeeklyQuestProgress>> LoadAsync(Guid characterId, DateTime periodStart, CancellationToken cancellationToken = default)
    {
        IList<WeeklyQuestProgress> result = this._entries.Values
            .Where(e => e.CharacterId == characterId && e.PeriodStart == periodStart)
            .Select(Clone)
            .ToList();
        return ValueTask.FromResult(result);
    }

    /// <inheritdoc />
    public ValueTask SaveAsync(IEnumerable<WeeklyQuestProgress> progress, CancellationToken cancellationToken = default)
    {
        foreach (var entry in progress)
        {
            this._entries[(entry.CharacterId, entry.PeriodStart, entry.QuestId)] = Clone(entry);
        }

        return ValueTask.CompletedTask;
    }

    private static WeeklyQuestProgress Clone(WeeklyQuestProgress source) => new()
    {
        CharacterId = source.CharacterId,
        PeriodStart = source.PeriodStart,
        QuestId = source.QuestId,
        Count = source.Count,
        CompletedAt = source.CompletedAt,
        RewardedAt = source.RewardedAt,
    };
}
