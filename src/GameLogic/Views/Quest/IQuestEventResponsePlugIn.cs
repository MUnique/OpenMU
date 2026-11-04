// <copyright file="IQuestEventResponsePlugIn.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.GameLogic.Views.Quest;

/// <summary>
/// Interface of a view whose implementation informs about the currently event quests.
/// </summary>
/// <remarks>
/// Sends C1F603.
/// </remarks>
public interface IQuestEventResponsePlugIn : IViewPlugIn
{
    /// <summary>
    /// Shows the active event quests.
    /// </summary>
    ValueTask ShowActiveEventQuestsAsync();
}