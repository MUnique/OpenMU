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
    /// Loads the progress of a character for the specified period.
    /// </summary>
    /// <param name="characterId">The identifier of the character.</param>
    /// <param name="periodStart">The start of the period.</param>
    /// <param name="cancellationToken">The cancellation token.</param>
    /// <returns>The stored progress of the character in this period.</returns>
    ValueTask<IList<WeeklyQuestProgress>> LoadAsync(Guid characterId, DateTime periodStart, CancellationToken cancellationToken = default);

    /// <summary>
    /// Inserts or updates the specified progress entries.
    /// </summary>
    /// <param name="progress">The progress entries.</param>
    /// <param name="cancellationToken">The cancellation token.</param>
    ValueTask SaveAsync(IEnumerable<WeeklyQuestProgress> progress, CancellationToken cancellationToken = default);

    /// <summary>
    /// Loads the rewarded progress entries of all characters of an account for the specified period.
    /// </summary>
    /// <param name="accountId">The identifier of the account.</param>
    /// <param name="periodStart">The start of the period.</param>
    /// <param name="cancellationToken">The cancellation token.</param>
    /// <returns>The rewarded progress entries of the characters of the account in this period.</returns>
    ValueTask<IList<WeeklyQuestProgress>> LoadRewardedByAccountAsync(Guid accountId, DateTime periodStart, CancellationToken cancellationToken = default);
}
