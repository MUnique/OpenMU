// <copyright file="WeeklyQuestProgressRepository.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.Persistence.EntityFramework.WeeklyQuests;

using System.Threading;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using MUnique.OpenMU.Persistence.WeeklyQuests;
using Nito.AsyncEx;

/// <summary>
/// Implementation of the <see cref="IWeeklyQuestProgressRepository"/> which stores the progress
/// in the <c>weekly</c> schema of the configured PostgreSQL database.
/// </summary>
public sealed class WeeklyQuestProgressRepository : IWeeklyQuestProgressRepository, IDisposable
{
    /// <summary>
    /// The time to wait before the storage is probed again after a failed attempt.
    /// </summary>
    private static readonly TimeSpan RetryDelay = TimeSpan.FromSeconds(30);

    private readonly ILogger<WeeklyQuestProgressRepository> _logger;
    private readonly SetupService? _setupService;
    private readonly AsyncLock _storageLock = new();
    private bool _isStorageReady;
    private DateTime _nextProbeAt = DateTime.MinValue;

    /// <summary>
    /// Initializes a new instance of the <see cref="WeeklyQuestProgressRepository"/> class.
    /// </summary>
    /// <param name="logger">The logger.</param>
    /// <param name="setupService">The setup service, which notifies about a re-created database.</param>
    public WeeklyQuestProgressRepository(ILogger<WeeklyQuestProgressRepository> logger, SetupService? setupService = null)
    {
        this._logger = logger;
        this._setupService = setupService;
        if (this._setupService is not null)
        {
            this._setupService.DatabaseInitialized += this.OnDatabaseInitialized;
        }
    }

    /// <inheritdoc />
    public void Dispose()
    {
        if (this._setupService is not null)
        {
            this._setupService.DatabaseInitialized -= this.OnDatabaseInitialized;
        }
    }

    /// <inheritdoc />
    public async ValueTask<IList<WeeklyQuestProgress>> LoadAsync(Guid characterId, IReadOnlyCollection<DateTime> periodStarts, CancellationToken cancellationToken = default)
    {
        await this.EnsureAvailableStorageAsync(cancellationToken).ConfigureAwait(false);

        var starts = periodStarts.ToArray();
        await using var context = new WeeklyQuestContext();
        return await context.Progress
            .AsNoTracking()
            .Where(p => p.CharacterId == characterId && starts.Contains(p.PeriodStart))
            .ToListAsync(cancellationToken)
            .ConfigureAwait(false);
    }

    /// <inheritdoc />
    public async ValueTask SaveAsync(IEnumerable<WeeklyQuestProgress> progress, CancellationToken cancellationToken = default)
    {
        var entries = progress.ToList();
        if (entries.Count == 0)
        {
            return;
        }

        await this.EnsureAvailableStorageAsync(cancellationToken).ConfigureAwait(false);

        await using var context = new WeeklyQuestContext();
        foreach (var group in entries.GroupBy(e => (e.CharacterId, e.PeriodStart)))
        {
            var questIds = group.Select(e => e.QuestId).ToList();
            var existing = await context.Progress
                .Where(p => p.CharacterId == group.Key.CharacterId && p.PeriodStart == group.Key.PeriodStart && questIds.Contains(p.QuestId))
                .ToDictionaryAsync(p => p.QuestId, cancellationToken)
                .ConfigureAwait(false);

            foreach (var entry in group)
            {
                if (existing.TryGetValue(entry.QuestId, out var stored))
                {
                    stored.AccountId ??= entry.AccountId;
                    stored.Count = entry.Count;
                    stored.AdditionalCounts = (int[])entry.AdditionalCounts.Clone();
                    stored.CompletedAt = entry.CompletedAt;
                    stored.RewardedAt = entry.RewardedAt;
                }
                else
                {
                    context.Progress.Add(new WeeklyQuestProgress
                    {
                        CharacterId = entry.CharacterId,
                        AccountId = entry.AccountId,
                        PeriodStart = entry.PeriodStart,
                        QuestId = entry.QuestId,
                        Count = entry.Count,
                        AdditionalCounts = (int[])entry.AdditionalCounts.Clone(),
                        CompletedAt = entry.CompletedAt,
                        RewardedAt = entry.RewardedAt,
                    });
                }
            }
        }

        await context.SaveChangesAsync(cancellationToken).ConfigureAwait(false);
    }

    /// <inheritdoc />
    public async ValueTask<IList<WeeklyQuestProgress>> LoadRewardedByAccountAsync(Guid accountId, IReadOnlyCollection<DateTime> periodStarts, CancellationToken cancellationToken = default)
    {
        await this.EnsureAvailableStorageAsync(cancellationToken).ConfigureAwait(false);

        var starts = periodStarts.ToArray();
        await using var context = new WeeklyQuestContext();
        return await context.Progress
            .AsNoTracking()
            .Where(p => p.AccountId == accountId && starts.Contains(p.PeriodStart) && p.RewardedAt != null)
            .ToListAsync(cancellationToken)
            .ConfigureAwait(false);
    }

    /// <inheritdoc />
    public async ValueTask<int> DeleteExpiredAsync(DateTime olderThan, DateTime keepPeriodStart, CancellationToken cancellationToken = default)
    {
        await this.EnsureAvailableStorageAsync(cancellationToken).ConfigureAwait(false);

        await using var context = new WeeklyQuestContext();
        return await context.Progress
            .Where(p => p.PeriodStart < olderThan && p.PeriodStart != keepPeriodStart)
            .ExecuteDeleteAsync(cancellationToken)
            .ConfigureAwait(false);
    }

    private async ValueTask EnsureAvailableStorageAsync(CancellationToken cancellationToken)
    {
        if (this._isStorageReady)
        {
            return;
        }

        if (DateTime.UtcNow < this._nextProbeAt)
        {
            throw new InvalidOperationException("The weekly quest storage is not available. Please check the database connection.");
        }

        using var l = await this._storageLock.LockAsync(cancellationToken).ConfigureAwait(false);
        if (this._isStorageReady)
        {
            return;
        }

        try
        {
            await using var context = new WeeklyQuestContext();
            await context.Database.MigrateAsync(cancellationToken).ConfigureAwait(false);
            this._isStorageReady = true;
        }
        catch (Exception ex)
        {
            this._nextProbeAt = DateTime.UtcNow + RetryDelay;
            this._logger.LogWarning(ex, "The weekly quest storage is not available (yet).");
            throw new InvalidOperationException("The weekly quest storage is not available. Please check the database connection.", ex);
        }
    }

    private ValueTask OnDatabaseInitialized()
    {
        this._isStorageReady = false;
        return ValueTask.CompletedTask;
    }
}
