// <copyright file="QuestPeriodStarts.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.GameLogic.PlugIns.WeeklyQuests;

/// <summary>
/// The starts (UTC) of the current periods of the quests.
/// </summary>
/// <param name="Weekly">The start of the current week.</param>
/// <param name="Daily">The start of the current day.</param>
public readonly record struct QuestPeriodStarts(DateTime Weekly, DateTime Daily)
{
    /// <summary>
    /// Gets all distinct period starts, including the one of the quests which are done once.
    /// </summary>
    public IReadOnlyCollection<DateTime> All => new HashSet<DateTime> { this.Weekly, this.Daily, WeeklyPeriod.OncePeriodStartUtc };

    /// <summary>
    /// Gets the start of the current period of the specified kind.
    /// </summary>
    /// <param name="period">The kind of the period.</param>
    /// <returns>The start of the current period.</returns>
    public DateTime Get(QuestPeriod period) => period switch
    {
        QuestPeriod.Daily => this.Daily,
        QuestPeriod.Once => WeeklyPeriod.OncePeriodStartUtc,
        _ => this.Weekly,
    };
}
