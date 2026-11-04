// <copyright file="SkillAnimationCounterStrategyBase.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.GameLogic.PlugIns.AnimationCounters;

/// <summary>
/// The base class of a <see cref="ISkillAnimationCounterStrategy"/>, which implements the usual behavior of the client:
/// Only animations with a counter greater than 0 are counted, and hits contain the counter of their animation.
/// </summary>
public abstract class SkillAnimationCounterStrategyBase : ISkillAnimationCounterStrategy
{
    /// <inheritdoc />
    public abstract short Key { get; }

    /// <inheritdoc />
    public virtual bool IsAnimationCounted(byte animationCounter) => animationCounter > 0;

    /// <inheritdoc />
    public virtual byte GetAnimationCounterOfHit(byte animationCounterOfHit, byte lastAnimationCounter) => animationCounterOfHit;
}
