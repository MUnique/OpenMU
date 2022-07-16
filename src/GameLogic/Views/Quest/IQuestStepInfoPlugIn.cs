// <copyright file="IQuestStepInfoPlugIn.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.GameLogic.Views.Quest;

using MUnique.OpenMU.DataModel.Configuration.Quests;

/// <summary>
/// Interface of a view whose implementation informs about the step of the quest.
/// </summary>
/// <remarks>
/// Sends C1F60B.
/// </remarks>
public interface IQuestStepInfoPlugIn : IViewPlugIn
{
    /// <summary>
    /// Shows the quest step information of the specified step number.
    /// </summary>
    /// <param name="questGroup">The quest group.</param>
    /// <param name="stepNumber">The step number, usually one of <see cref="QuestDefinition.StartingNumber" /> or <see cref="QuestDefinition.RefuseNumber" />.</param>
    ValueTask ShowQuestStepInfoAsync(short questGroup, short stepNumber);
}