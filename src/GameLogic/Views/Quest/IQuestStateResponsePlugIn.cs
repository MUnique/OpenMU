// <copyright file="IQuestStateResponsePlugIn.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.GameLogic.Views.Quest;

/// <summary>
/// Interface of a view whose implementation informs about the requested quest state.
/// </summary>
/// <remarks>
/// Could send C1A0 and C1F61B.
/// </remarks>
public interface IQuestStateResponsePlugIn : IViewPlugIn
{
    /// <summary>
    /// Shows the state of the quest.
    /// </summary>
    /// <param name="questState">The quest state.</param>
    ValueTask ShowQuestStateAsync(CharacterQuestState? questState);
}