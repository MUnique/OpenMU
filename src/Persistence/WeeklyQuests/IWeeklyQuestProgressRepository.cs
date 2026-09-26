// <copyright file="IWeeklyQuestProgressRepository.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.Persistence.WeeklyQuests;

using System.Threading;

/// <summary>
/// A repository for the <see cref="WeeklyQuestProgress"/> of characters.
/// </summary>
public interface IWeeklyQuestProgressRepository
{
    /// <summary>
    /// Loads the progress of a character for the specified periods, with one query.
    /// </summary>
    /// <param name="characterId">The identifier of the character.</param>
    /// <param name="periodStarts">The starts of the periods, e.g. of the current day, week and of the quests which are done once.</param>
    /// <param name="cancellationToken">The cancellation token.</param>
    /// <returns>The stored progress of the character in these periods.</returns>
    ValueTask<IList<WeeklyQuestProgress>> LoadAsync(Guid characterId, IReadOnlyCollection<DateTime> periodStarts, CancellationToken cancellationToken = default);

    /// <summary>
    /// Inserts or updates the specified progress entries.
    /// </summary>
    /// <param name="progress">The progress entries.</param>
    /// <param name="cancellationToken">The cancellation token.</param>
    ValueTask SaveAsync(IEnumerable<WeeklyQuestProgress> progress, CancellationToken cancellationToken = default);

    /// <summary>
    /// Loads the rewarded progress entries of all characters of an account for the specified periods.
    /// </summary>
    /// <param name="accountId">The identifier of the account.</param>
    /// <param name="periodStarts">The starts of the periods.</param>
    /// <param name="cancellationToken">The cancellation token.</param>
    /// <returns>The rewarded progress entries of the characters of the account in these periods.</returns>
    ValueTask<IList<WeeklyQuestProgress>> LoadRewardedByAccountAsync(Guid accountId, IReadOnlyCollection<DateTime> periodStarts, CancellationToken cancellationToken = default);

    /// <summary>
    /// Deletes the progress of the periods which started before the specified point in time.
    /// </summary>
    /// <param name="olderThan">The progress of periods which started before this point in time (UTC) is deleted.</param>
    /// <param name="keepPeriodStart">The start of a period whose progress is kept anyway, i.e. the one of the quests which are done once.</param>
    /// <param name="cancellationToken">The cancellation token.</param>
    /// <returns>The number of deleted entries.</returns>
    ValueTask<int> DeleteExpiredAsync(DateTime olderThan, DateTime keepPeriodStart, CancellationToken cancellationToken = default);
}
