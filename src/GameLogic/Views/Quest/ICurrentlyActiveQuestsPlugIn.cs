// <copyright file="ICurrentlyActiveQuestsPlugIn.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.GameLogic.Views.Quest;

/// <summary>
/// Interface of a view whose implementation informs about the currently active quests.
/// </summary>
/// <remarks>
/// Sends C1F61A.
/// </remarks>
public interface ICurrentlyActiveQuestsPlugIn : IViewPlugIn
{
    /// <summary>
    /// Shows the active quests.
    /// </summary>
    ValueTask ShowActiveQuestsAsync();
}