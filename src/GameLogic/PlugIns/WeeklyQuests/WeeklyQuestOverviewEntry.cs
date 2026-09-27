// <copyright file="WeeklyQuestOverviewEntry.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.GameLogic.PlugIns.WeeklyQuests;

/// <summary>
/// An entry of a <see cref="WeeklyQuestOverview"/>.
/// </summary>
/// <param name="Quest">The quest.</param>
/// <param name="Count">The achieved count of the objective; with several objectives, the number of the completed ones.</param>
/// <param name="Required">The required count of the objective; with several objectives, their number.</param>
/// <param name="IsCompleted">A value indicating whether the objectives have been reached.</param>
/// <param name="IsRewarded">A value indicating whether the rewards have been handed out.</param>
/// <param name="CurrentStep">The index of the first objective which isn't done yet; the number of objectives when all are done.</param>
/// <param name="Objectives">The progress of each objective.</param>
public record WeeklyQuestOverviewEntry(
    WeeklyQuestDefinition Quest,
    int Count,
    int Required,
    bool IsCompleted,
    bool IsRewarded,
    int CurrentStep,
    IReadOnlyList<WeeklyQuestObjectiveProgress> Objectives);
