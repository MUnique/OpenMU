// <copyright file="WebsiteRankingCache.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.Web.AdminPanel.Operations.Website;

using System.Collections.Concurrent;
using System.Threading;
using Nito.AsyncEx;

/// <summary>
/// Caches the character rankings of the website API for a short time, like the ranking of the Discord bot.
/// </summary>
/// <remarks>
/// A website may show the ranking on every page, and loading it reads all characters and then the guild of each ranked
/// character. A ranking changes slowly, so it's cached for <see cref="Duration"/>, and it may be that old.
/// Each ranking is cached with all <see cref="WebsiteOperations.MaximumRankingCount"/> entries, and a request for fewer
/// entries gets its beginning; all classes of a line share one ranking. So the cache holds at most one ranking of all classes
/// and one per class line, whatever the website requests.
/// Only one ranking is loaded at a time, so an expired ranking which many visitors request at once is loaded only once.
/// The rankings are kept in the memory of the process, so several instances of the admin panel cache them separately.
/// </remarks>
public sealed class WebsiteRankingCache
{
    /// <summary>
    /// The key of the ranking of all classes; the keys of the class lines are class numbers, which are bytes.
    /// </summary>
    private const int AllClassesKey = -1;

    private readonly ConcurrentDictionary<int, Entry> _rankings = new();

    private readonly AsyncLock _loadLock = new();

    /// <summary>
    /// Gets the time for which a ranking is cached; it's the same as for the ranking of the Discord bot.
    /// </summary>
    internal static TimeSpan Duration { get; } = TimeSpan.FromMinutes(2);

    /// <summary>
    /// Gets a ranking from the cache, or loads and caches it, if it isn't cached or older than <see cref="Duration"/>.
    /// </summary>
    /// <param name="classLine">The key of the class line, e.g. the lowest number of its classes; <c>null</c> for all classes.</param>
    /// <param name="loadAsync">The function which loads the ranking with all <see cref="WebsiteOperations.MaximumRankingCount"/> entries.</param>
    /// <param name="utcNow">The current time, in UTC.</param>
    /// <param name="cancellationToken">The cancellation token.</param>
    /// <returns>The ranking.</returns>
    public async ValueTask<IReadOnlyList<WebsiteRankingEntry>> GetOrLoadAsync(
        byte? classLine,
        Func<CancellationToken, ValueTask<IReadOnlyList<WebsiteRankingEntry>>> loadAsync,
        DateTime utcNow,
        CancellationToken cancellationToken)
    {
        var key = classLine ?? AllClassesKey;
        if (this.TryGetCurrent(key, utcNow, out var ranking))
        {
            return ranking;
        }

        using (await this._loadLock.LockAsync(cancellationToken).ConfigureAwait(false))
        {
            // Another request may have loaded the ranking while this one waited for the lock.
            if (this.TryGetCurrent(key, utcNow, out ranking))
            {
                return ranking;
            }

            ranking = await loadAsync(cancellationToken).ConfigureAwait(false);
            this._rankings[key] = new Entry(utcNow, ranking);
            return ranking;
        }
    }

    private bool TryGetCurrent(int key, DateTime utcNow, out IReadOnlyList<WebsiteRankingEntry> ranking)
    {
        if (this._rankings.TryGetValue(key, out var entry) && utcNow - entry.LoadedAtUtc < Duration)
        {
            ranking = entry.Ranking;
            return true;
        }

        ranking = [];
        return false;
    }

    /// <summary>
    /// A cached ranking.
    /// </summary>
    /// <param name="LoadedAtUtc">The time at which it was loaded.</param>
    /// <param name="Ranking">The ranking.</param>
    private sealed record Entry(DateTime LoadedAtUtc, IReadOnlyList<WebsiteRankingEntry> Ranking);
}
