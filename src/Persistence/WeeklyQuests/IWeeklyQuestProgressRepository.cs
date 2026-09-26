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
}
