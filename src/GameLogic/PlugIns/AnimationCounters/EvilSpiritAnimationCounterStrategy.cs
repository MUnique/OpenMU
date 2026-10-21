// <copyright file="EvilSpiritAnimationCounterStrategy.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.GameLogic.PlugIns.AnimationCounters;

using System.Runtime.InteropServices;
using MUnique.OpenMU.PlugIns;

/// <summary>
/// Describes how the client counts the animations of Evil Spirit.
/// </summary>
[Guid("CDACDEBF-C2FF-466C-A926-4A76DA2A2DDB")]
[PlugIn]
[Display(Name = nameof(PlugInResources.EvilSpiritAnimationCounterStrategy_Name), Description = nameof(PlugInResources.EvilSpiritAnimationCounterStrategy_Description), ResourceType = typeof(PlugInResources))]
public class EvilSpiritAnimationCounterStrategy : SkillAnimationCounterStrategyBase
{
    /// <summary>
    /// The skill number.
    /// </summary>
    internal const short SkillNumber = 9;

    /// <inheritdoc />
    public override short Key => SkillNumber;

    /// <inheritdoc />
    /// <remarks>The client counts every animation of this skill, even with a counter of 0.</remarks>
    public override bool IsAnimationCounted(byte animationCounter) => true;
}
