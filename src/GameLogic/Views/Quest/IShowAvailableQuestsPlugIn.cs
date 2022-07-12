// <copyright file="IShowAvailableQuestsPlugIn.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.GameLogic.Views.Quest;

/// <summary>
/// Interface of a view whose implementation informs about the available quests.
/// </summary>
/// <remarks>
/// Sends C1F60A.
/// </remarks>
public interface IShowAvailableQuestsPlugIn : IViewPlugIn
{
    /// <summary>
    /// Shows the available quests.
    /// </summary>
    ValueTask ShowAvailableQuestsAsync();
}