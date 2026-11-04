// <copyright file="IAreaSkillTargetFilter.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.GameLogic.PlugIns;

using System.Runtime.InteropServices;
using MUnique.OpenMU.Pathfinding;
using MUnique.OpenMU.PlugIns;

/// <summary>
/// Plugins which will be executed when an area skill is about to hit its targets.
/// It allows to filter out targets which are out of range.
/// The key is the <see cref="Skill.Number"/>.
/// </summary>
[Guid("CD813E47-A926-48E1-A63F-9A80121CDBE9")]
[PlugInPoint("Area skill target filter", "Plugins which will be executed when an area skill is about to hit its targets. It allows to filter out targets which are out of range.")]
public interface IAreaSkillTargetFilter : IStrategyPlugIn<short>
{
    /// <summary>
    /// Determines whether the target is within the hit bounds.
    /// </summary>
    /// <param name="attacker">The attacker.</param>
    /// <param name="target">The target.</param>
    /// <param name="targetAreaCenter">The target area center.</param>
    /// <param name="rotation">The rotation.</param>
    /// <returns><c>true</c> if the target is within hit bounds; otherwise, <c>false</c>.</returns>
    bool IsTargetWithinBounds(ILocateable attacker, ILocateable target, Point targetAreaCenter, byte rotation);
}