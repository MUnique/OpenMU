// <copyright file="IIllusionTempleSkillPointUpdateViewPlugin.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.GameLogic.MiniGames;

using MUnique.OpenMU.GameLogic.Views;

/// <summary>
/// Interface of a view whose implementation informs a player about his current skill point balance
/// during a running illusion temple event.
/// </summary>
public interface IIllusionTempleSkillPointUpdateViewPlugin : IViewPlugIn
{
    /// <summary>
    /// Updates the skill points of the receiving player.
    /// </summary>
    /// <param name="skillPoints">The current skill point balance.</param>
    ValueTask UpdateSkillPointsAsync(byte skillPoints);
}
