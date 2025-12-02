// <copyright file="IChaosCastleStateViewPlugin.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.GameLogic.MiniGames;

using MUnique.OpenMU.GameLogic.Views;

/// <summary>
/// Interface of a view whose implementation informs about the status of a chaos castle event.
/// </summary>
public interface IChaosCastleStateViewPlugin : IViewPlugIn
{
    /// <summary>
    /// Update the state of the blood castle event.
    /// </summary>
    /// <param name="status">The status of the blood castle event.</param>
    /// <param name="remainingTime">The remaining time of the blood castle event.</param>
    /// <param name="maxMonster">Maximum number of monsters to kill.</param>
    /// <param name="curMonster">Current number of monsters killed.</param>
    ValueTask UpdateStateAsync(ChaosCastleStatus status, TimeSpan remainingTime, int maxMonster, int curMonster);
}