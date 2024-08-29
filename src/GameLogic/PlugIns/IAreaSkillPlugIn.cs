// <copyright file="IAreaSkillPlugIn.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.GameLogic.PlugIns;

using System.Runtime.InteropServices;
using MUnique.OpenMU.Pathfinding;
using MUnique.OpenMU.PlugIns;

/// <summary>
/// A plugin interface which is called when a target got attacked by an area skill.
/// The key is the skill number.
/// </summary>
[Guid("BAE1E31E-08EA-4B77-BE0E-89DECD9EAA29")]
[PlugInPoint("Area skill plugins", "Is called when a target got attacked by an area skill.")]
public interface IAreaSkillPlugIn : IStrategyPlugIn<short>
{
    /// <summary>
    /// Is called after a target got automatically attacked by an area skill, regardless if the attack effectively hit or not.
    /// </summary>
    /// <param name="attacker">The attacker.</param>
    /// <param name="target">The target.</param>
    /// <param name="skillEntry">The skill entry.</param>
    /// <param name="targetAreaCenter">The target area center.</param>
    /// <param name="hitInfo">Hit info produced by the skill.</param>
    ValueTask AfterTargetGotAttackedAsync(IAttacker attacker, IAttackable target, SkillEntry skillEntry, Point targetAreaCenter, HitInfo? hitInfo);
}