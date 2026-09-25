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
    /// Gets or sets the start of the weekly period (UTC) to which this progress belongs.
    /// </summary>
    public DateTime PeriodStart { get; set; }

    /// <summary>
    /// Gets or sets the identifier of the quest, as configured in the plugin configuration.
    /// </summary>
    public string QuestId { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets the achieved count towards the objective of the quest.
    /// </summary>
    public int Count { get; set; }

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
}
