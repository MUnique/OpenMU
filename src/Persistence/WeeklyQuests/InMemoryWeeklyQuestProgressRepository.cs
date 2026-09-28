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
    private int _loadCount;

    /// <summary>
    /// Gets the number of calls of <see cref="LoadAsync"/>, for tests.
    /// </summary>
    public int LoadCount => this._loadCount;

    /// <inheritdoc />
    public ValueTask<IList<WeeklyQuestProgress>> LoadAsync(Guid characterId, IReadOnlyCollection<DateTime> periodStarts, CancellationToken cancellationToken = default)
    {
        Interlocked.Increment(ref this._loadCount);
        IList<WeeklyQuestProgress> result = this._entries.Values
            .Where(e => e.CharacterId == characterId && periodStarts.Contains(e.PeriodStart))
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

    /// <inheritdoc />
    public ValueTask<IList<WeeklyQuestProgress>> LoadRewardedByAccountAsync(Guid accountId, IReadOnlyCollection<DateTime> periodStarts, CancellationToken cancellationToken = default)
    {
        IList<WeeklyQuestProgress> result = this._entries.Values
            .Where(e => e.AccountId == accountId && periodStarts.Contains(e.PeriodStart) && e.RewardedAt is not null)
            .Select(Clone)
            .ToList();
        return ValueTask.FromResult(result);
    }

    /// <inheritdoc />
    public ValueTask<int> DeleteExpiredAsync(DateTime olderThan, DateTime keepPeriodStart, CancellationToken cancellationToken = default)
    {
        var deleted = 0;
        foreach (var key in this._entries.Keys.Where(k => k.PeriodStart < olderThan && k.PeriodStart != keepPeriodStart).ToList())
        {
            if (this._entries.TryRemove(key, out _))
            {
                deleted++;
            }
        }

        return ValueTask.FromResult(deleted);
    }

    private static WeeklyQuestProgress Clone(WeeklyQuestProgress source) => new()
    {
        CharacterId = source.CharacterId,
        AccountId = source.AccountId,
        PeriodStart = source.PeriodStart,
        QuestId = source.QuestId,
        Count = source.Count,
        AdditionalCounts = (int[])source.AdditionalCounts.Clone(),
        CompletedAt = source.CompletedAt,
        RewardedAt = source.RewardedAt,
    };
}
