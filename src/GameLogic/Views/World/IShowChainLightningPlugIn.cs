// <copyright file="IShowChainLightningPlugIn.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.GameLogic.Views.World;

/// <summary>
/// Interface of a view whose implementation informs about chain lightning skill animations of players.
/// </summary>
public interface IShowChainLightningPlugIn : IViewPlugIn
{
    /// <summary>
    /// Shows the chain lightning skill animation.
    /// </summary>
    /// <param name="attacker">The attacker.</param>
    /// <param name="skill">The skill.</param>
    /// <param name="targets">The targets of the lightning chain.</param>
    ValueTask ShowLightningChainAnimationAsync(IAttacker attacker, Skill skill, IReadOnlyCollection<IAttackable> targets);
}