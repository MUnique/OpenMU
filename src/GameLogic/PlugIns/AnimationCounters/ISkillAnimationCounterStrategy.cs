// <copyright file="ISkillAnimationCounterStrategy.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.GameLogic.PlugIns.AnimationCounters;

using System.Runtime.InteropServices;
using MUnique.OpenMU.PlugIns;

/// <summary>
/// A strategy which describes how the game client counts the animations of a skill.
/// The animation counters are used by the <see cref="SkillHitValidator"/> to validate the hits of skills with explicit hits.
/// The key is the skill number.
/// </summary>
[Guid("09DE8B97-7398-4AC6-8981-5E3D9AB77C9E")]
[PlugInPoint("Skill animation counter strategies", "Plugins which describe how the game client counts the animations of specific skills.")]
public interface ISkillAnimationCounterStrategy : IStrategyPlugIn<short>
{
    /// <summary>
    /// Determines whether the client counts the animation with the specified counter.
    /// </summary>
    /// <param name="animationCounter">The animation counter which the client sent.</param>
    /// <returns><c>true</c>, if the client counts the animation.</returns>
    bool IsAnimationCounted(byte animationCounter);

    /// <summary>
    /// Gets the counter of the animation to which a hit refers.
    /// </summary>
    /// <param name="animationCounterOfHit">The animation counter which the client sent with the hit.</param>
    /// <param name="lastAnimationCounter">The counter of the last animation of the skill.</param>
    /// <returns>The counter of the animation to which the hit refers.</returns>
    byte GetAnimationCounterOfHit(byte animationCounterOfHit, byte lastAnimationCounter);
}
