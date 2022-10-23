// <copyright file="IShowSkillAnimationPlugIn.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.GameLogic.Views.World;

/// <summary>
/// Interface of a view whose implementation informs about skill animations of objects.
/// </summary>
public interface IShowSkillAnimationPlugIn : IViewPlugIn
{
    /// <summary>
    /// Shows the skill animation.
    /// </summary>
    /// <param name="attacker">The attacking object.</param>
    /// <param name="target">The target.</param>
    /// <param name="skill">The skill.</param>
    /// <param name="effectApplied">Flag, if a magic effect was applied with the skill.</param>
    ValueTask ShowSkillAnimationAsync(IAttacker attacker, IAttackable? target, Skill skill, bool effectApplied);

    /// <summary>
    /// Shows the skill animation.
    /// </summary>
    /// <param name="attacker">The attacking object.</param>
    /// <param name="target">The target.</param>
    /// <param name="skillNumber">The skill number, see also <see cref="Skill.Number"/>.</param>
    /// <param name="effectApplied">Flag, if a magic effect was applied with the skill.</param>
    ValueTask ShowSkillAnimationAsync(IAttacker attacker, IAttackable? target, short skillNumber, bool effectApplied);

    /// <summary>
    /// Shows the combo skill animation.
    /// </summary>
    /// <param name="attacker">The attacking object.</param>
    /// <param name="target">The optional target.</param>
    ValueTask ShowComboAnimationAsync(IAttacker attacker, IAttackable? target);

    /// <summary>
    /// Shows the nova start skill animation.
    /// </summary>
    /// <param name="attacker">The attacking object.</param>
    ValueTask ShowNovaStartAsync(IAttacker attacker);
}