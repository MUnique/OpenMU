// <copyright file="IQuestProgressPlugIn.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.GameLogic.Views.Quest;

using MUnique.OpenMU.DataModel.Configuration.Quests;

/// <summary>
/// Interface of a view whose implementation informs about the quest progress.
/// </summary>
/// <remarks>
/// Could send C1A4 and C1F60C or C1F61B.
/// </remarks>
public interface IQuestProgressPlugIn : IViewPlugIn
{
    /// <summary>
    /// Shows the progress of the quest.
    /// </summary>
    /// <param name="quest">The quest.</param>
    /// <param name="wasProgressionRequested">If set to <c>true</c>, the client previously requested to increase the progress of the quest. Otherwise, <c>false</c>, e.g. after a specific request or entering the game.</param>
    ValueTask ShowQuestProgressAsync(QuestDefinition quest, bool wasProgressionRequested);
}