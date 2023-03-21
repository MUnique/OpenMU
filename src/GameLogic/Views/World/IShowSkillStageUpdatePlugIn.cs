// <copyright file="IShowSkillStageUpdatePlugIn.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.GameLogic.Views.World;

/// <summary>
/// Interface of a view whose implementation informs about skill stage (usually Nova) of objects.
/// </summary>
public interface IShowSkillStageUpdatePlugIn : IViewPlugIn
{
    /// <summary>
    /// Updates the skill stage of an attacker and a skill, on the client side.
    /// </summary>
    /// <param name="attacker">The attacker.</param>
    /// <param name="skillNumber">The skill number (usually 40 for Nova).</param>
    /// <param name="stageNumber">The stage number of the skill (usually 1 to 12 for Nova).</param>
    ValueTask UpdateSkillStageAsync(IAttacker attacker, short skillNumber, byte stageNumber);
}