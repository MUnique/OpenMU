// <copyright file="IBloodCastleStateViewPlugin.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.GameLogic.MiniGames;

using MUnique.OpenMU.GameLogic.Views;

/// <summary>
/// Interface of a view whose implementation informs about the state of a blood castle event.
/// </summary>
public interface IBloodCastleStateViewPlugin : IViewPlugIn
{
    /// <summary>
    /// Update the state of the blood castle event.
    /// </summary>
    /// <param name="state">The state of the blood castle event.</param>
    /// <param name="remainSecond">The remaining time of the blood castle event.</param>
    /// <param name="maxMonster">Maximum number of monsters to kill.</param>
    /// <param name="curMonster">Current number of monsters killed.</param>
    /// <param name="itemOwner">The item ownwe.</param>
    /// <param name="itemLevel">The item level.</param>
    void UpdateState(byte state, int remainSecond, int maxMonster, int curMonster, int itemOwner, byte itemLevel);
}