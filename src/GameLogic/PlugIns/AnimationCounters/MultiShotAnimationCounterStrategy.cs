// <copyright file="MultiShotAnimationCounterStrategy.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.GameLogic.PlugIns.AnimationCounters;

using System.Runtime.InteropServices;
using MUnique.OpenMU.PlugIns;

/// <summary>
/// Describes how the client counts the animations of Multi-Shot.
/// </summary>
[Guid("38EFC180-911C-4AE5-B423-EB7712527ADB")]
[PlugIn]
[Display(Name = nameof(PlugInResources.MultiShotAnimationCounterStrategy_Name), Description = nameof(PlugInResources.MultiShotAnimationCounterStrategy_Description), ResourceType = typeof(PlugInResources))]
public class MultiShotAnimationCounterStrategy : SkillAnimationCounterStrategyBase
{
    /// <summary>
    /// The skill number.
    /// </summary>
    internal const short SkillNumber = 235;

    /// <inheritdoc />
    public override short Key => SkillNumber;

    /// <inheritdoc />
    /// <remarks>The client sends the counter of the animations, but not in the hits.</remarks>
    public override byte GetAnimationCounterOfHit(byte animationCounterOfHit, byte lastAnimationCounter)
        => animationCounterOfHit == 0 ? lastAnimationCounter : animationCounterOfHit;
}
