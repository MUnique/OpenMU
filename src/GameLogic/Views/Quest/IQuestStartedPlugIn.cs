// <copyright file="IQuestStartedPlugIn.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.GameLogic.Views.Quest;

using MUnique.OpenMU.DataModel.Configuration.Quests;

/// <summary>
/// Interface of a view whose implementation informs about a started quest.
/// </summary>
/// <remarks>
/// Sends C1F60B.
/// </remarks>
public interface IQuestStartedPlugIn : IViewPlugIn
{
    /// <summary>
    /// Informs the client that the specified quest has been the started.
    /// </summary>
    /// <param name="quest">The started quest.</param>
    ValueTask QuestStartedAsync(QuestDefinition quest);
}