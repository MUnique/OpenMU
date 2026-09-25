// <copyright file="WeeklyQuestOverview.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.GameLogic.PlugIns.WeeklyQuests;

/// <summary>
/// The overview of the weekly quests of a player.
/// </summary>
/// <param name="Entries">The entries of the active quests.</param>
/// <param name="NextResetUtc">The point in time (UTC) when the next period starts.</param>
public record WeeklyQuestOverview(IReadOnlyList<WeeklyQuestOverviewEntry> Entries, DateTime NextResetUtc);

/// <summary>
/// An entry of a <see cref="WeeklyQuestOverview"/>.
/// </summary>
/// <param name="Quest">The quest.</param>
/// <param name="Count">The achieved count.</param>
/// <param name="IsCompleted">A value indicating whether the objective has been reached.</param>
/// <param name="IsRewarded">A value indicating whether the rewards have been handed out.</param>
public record WeeklyQuestOverviewEntry(WeeklyQuestDefinition Quest, int Count, bool IsCompleted, bool IsRewarded);
