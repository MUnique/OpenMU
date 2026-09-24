// <copyright file="IIllusionTempleSkillEndedViewPlugin.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.GameLogic.MiniGames;

using MUnique.OpenMU.GameLogic.Views;

/// <summary>
/// Interface of a view whose implementation informs about a special illusion temple skill (210 to 213)
/// wearing off on an object.
/// </summary>
public interface IIllusionTempleSkillEndedViewPlugin : IViewPlugIn
{
    /// <summary>
    /// Announces that a skill's effect has ended on an object.
    /// </summary>
    /// <param name="skillNumber">The number of the skill (210 to 213).</param>
    /// <param name="objectId">The id of the affected object.</param>
    ValueTask ShowSkillEndedAsync(ushort skillNumber, ushort objectId);
}
