// <copyright file="TargetedSkillPluginBase.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>
namespace MUnique.OpenMU.GameLogic.PlayerActions.Skills;

using MUnique.OpenMU.GameLogic.PlugIns;

/// <summary>
/// Action to perform a skill which is explicitly aimed to a target.
/// </summary>
public abstract class TargetedSkillPluginBase : ITargetedSkillPlugin
{
    /// <inheritdoc />
    public virtual short Key => 0;

    /// <summary>
    /// Performs the skill.
    /// </summary>
    /// <param name="player">The player.</param>
    /// <param name="target">The target.</param>
    /// <param name="skillId">The skill identifier.</param>
    public abstract ValueTask PerformSkillAsync(Player player, IAttackable target, ushort skillId);
}