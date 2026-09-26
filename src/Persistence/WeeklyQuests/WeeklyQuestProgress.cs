// <copyright file="WeeklyQuestProgress.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.Persistence.WeeklyQuests;

/// <summary>
/// The progress of a character in one weekly quest of one weekly period.
/// </summary>
/// <remarks>
/// The progress is keyed by the start of the period, so a new week automatically starts
/// with fresh rows - there is no job which has to reset anything.
/// </remarks>
public class WeeklyQuestProgress
{
    /// <summary>
    /// Gets or sets the identifier of the character.
    /// </summary>
    public Guid CharacterId { get; set; }

    /// <summary>
    /// Gets or sets the identifier of the account of the character.
    /// </summary>
    /// <remarks>
    /// It's <c>null</c> for entries which were stored before it was introduced.
    /// </remarks>
    public Guid? AccountId { get; set; }

    /// <summary>
    /// Gets or sets the start of the weekly period (UTC) to which this progress belongs.
    /// </summary>
    public DateTime PeriodStart { get; set; }

    /// <summary>
    /// Gets or sets the identifier of the quest, as configured in the plugin configuration.
    /// </summary>
    public string QuestId { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets the achieved count towards the (first) objective of the quest.
    /// </summary>
    public int Count { get; set; }

    /// <summary>
    /// Gets or sets the achieved counts towards the further objectives of a quest with several objectives.
    /// The element 0 belongs to the second objective, because the first one is <see cref="Count"/>.
    /// </summary>
    public int[] AdditionalCounts { get; set; } = [];

    /// <summary>
    /// Gets or sets the timestamp (UTC) when the objective has been reached.
    /// </summary>
    public DateTime? CompletedAt { get; set; }

    /// <summary>
    /// Gets or sets the timestamp (UTC) when the rewards have been handed out.
    /// </summary>
    /// <remarks>
    /// It stays <c>null</c> as long as a completed quest couldn't be rewarded yet, e.g. because the inventory was full.
    /// </remarks>
    public DateTime? RewardedAt { get; set; }

    /// <summary>
    /// Gets a value indicating whether any progress has been made in the quest.
    /// </summary>
    public bool HasAnyProgress => this.Count > 0 || this.CompletedAt is not null || this.AdditionalCounts.Any(c => c > 0);

    /// <summary>
    /// Gets the achieved count towards the objective with the specified index.
    /// </summary>
    /// <param name="objectiveIndex">The index of the objective.</param>
    /// <returns>The achieved count.</returns>
    public int GetCount(int objectiveIndex)
    {
        if (objectiveIndex == 0)
        {
            return this.Count;
        }

        return objectiveIndex - 1 < this.AdditionalCounts.Length ? this.AdditionalCounts[objectiveIndex - 1] : 0;
    }

    /// <summary>
    /// Sets the achieved count towards the objective with the specified index.
    /// </summary>
    /// <param name="objectiveIndex">The index of the objective.</param>
    /// <param name="count">The achieved count.</param>
    public void SetCount(int objectiveIndex, int count)
    {
        if (objectiveIndex == 0)
        {
            this.Count = count;
            return;
        }

        // A new array, so that the change is detected when the entry is saved.
        var counts = this.AdditionalCounts;
        if (objectiveIndex - 1 >= counts.Length)
        {
            Array.Resize(ref counts, objectiveIndex);
        }
        else
        {
            counts = (int[])counts.Clone();
        }

        counts[objectiveIndex - 1] = count;
        this.AdditionalCounts = counts;
    }
}
