// <copyright file="NovaSkillStopAction.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.GameLogic.PlayerActions.Skills;

/// <summary>
/// The stop nova skill action.
/// </summary>
public class NovaSkillStopAction : TargetedSkillActionBase
{
    /// <inheritdoc/>
    public override ushort Key => 40;

    /// <inheritdoc />
    public override async ValueTask PerformSkillAsync(Player player, IAttackable target, ushort skillId)
    {
        player.TargetedSkillCancelTokenSource?.CancelWithExtraTarget(target.Id);
    }
}