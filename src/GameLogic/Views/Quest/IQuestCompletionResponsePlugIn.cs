// <copyright file="IQuestCompletionResponsePlugIn.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.GameLogic.Views.Quest;

using MUnique.OpenMU.DataModel.Configuration.Quests;

/// <summary>
/// Interface of a view whose implementation informs about the requested quest completion.
/// </summary>
/// <remarks>
/// Could send C1A2 and C1F60D.
/// </remarks>
public interface IQuestCompletionResponsePlugIn : IViewPlugIn
{
    /// <summary>
    /// Shows if the specified quest was completed.
    /// </summary>
    /// <param name="quest">The quest.</param>
    /// <remarks>
    /// The success/state of the quest can be determined by inspecting the player object.
    /// </remarks>
    ValueTask QuestCompletedAsync(QuestDefinition quest);
}