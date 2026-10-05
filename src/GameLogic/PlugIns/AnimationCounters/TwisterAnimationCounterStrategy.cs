// <copyright file="TwisterAnimationCounterStrategy.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.GameLogic.PlugIns.AnimationCounters;

using System.Runtime.InteropServices;
using MUnique.OpenMU.PlugIns;

/// <summary>
/// Describes how the client counts the animations of Twister.
/// </summary>
[Guid("DABF3A3F-7ED4-49C0-AA14-11DFAA06811B")]
[PlugIn]
[Display(Name = nameof(PlugInResources.TwisterAnimationCounterStrategy_Name), Description = nameof(PlugInResources.TwisterAnimationCounterStrategy_Description), ResourceType = typeof(PlugInResources))]
public class TwisterAnimationCounterStrategy : SkillAnimationCounterStrategyBase
{
    /// <summary>
    /// The skill number.
    /// </summary>
    internal const short SkillNumber = 8;

    /// <inheritdoc />
    public override short Key => SkillNumber;

    /// <inheritdoc />
    /// <remarks>The client counts every animation of this skill, even with a counter of 0.</remarks>
    public override bool IsAnimationCounted(byte animationCounter) => true;

    /// <inheritdoc />
    /// <remarks>The client sends the counter of the animations, but not in the hits.</remarks>
    public override byte GetAnimationCounterOfHit(byte animationCounterOfHit, byte lastAnimationCounter)
        => animationCounterOfHit == 0 ? lastAnimationCounter : animationCounterOfHit;
}
