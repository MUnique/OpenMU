// <copyright file="IIllusionTempleSkillUsageResultViewPlugin.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.GameLogic.MiniGames;

using MUnique.OpenMU.GameLogic.Views;

/// <summary>
/// Interface of a view whose implementation informs a player about the result of a special illusion
/// temple skill (210 to 213) he requested to use.
/// </summary>
public interface IIllusionTempleSkillUsageResultViewPlugin : IViewPlugIn
{
    /// <summary>
    /// Shows the result of a requested illusion temple skill.
    /// </summary>
    /// <param name="success">Whether the skill was used successfully.</param>
    /// <param name="skillNumber">The number of the skill (210 to 213).</param>
    /// <param name="sourceId">The id of the player who used the skill.</param>
    /// <param name="targetId">The id of the target, or <c>0</c> if the skill didn't target anyone.</param>
    ValueTask ShowSkillUsageResultAsync(bool success, ushort skillNumber, ushort sourceId, ushort targetId);
}
