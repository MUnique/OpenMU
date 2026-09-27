// <copyright file="WeeklyQuestObjectiveProgress.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.GameLogic.PlugIns.WeeklyQuests;

/// <summary>
/// The progress of an objective of a <see cref="WeeklyQuestOverviewEntry"/>.
/// </summary>
/// <param name="Objective">The objective.</param>
/// <param name="Count">The achieved count.</param>
/// <param name="Required">The required count.</param>
/// <param name="IsDone">A value indicating whether the objective is done.</param>
public record WeeklyQuestObjectiveProgress(WeeklyQuestObjective Objective, int Count, int Required, bool IsDone);
