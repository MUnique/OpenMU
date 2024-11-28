// <copyright file="ForceSkillAction.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.GameLogic.PlayerActions.Skills;

/// <summary>
/// The Force skill action.
/// </summary>
public class ForceSkillAction : TargetedSkillActionDefault
{
    private const ushort ForceWaveSkillId = 66;

    /// <inheritdoc/>
    public override ushort Key => 60;

    /// <inheritdoc/>
    public override async ValueTask PerformSkillAsync(Player player, IAttackable target, ushort skillId)
    {
        // Special handling of force wave skill. The client might send skill id 60,
        // even though it's performing force wave.
        await base.PerformSkillAsync(player, target, ForceWaveSkillId).ConfigureAwait(false);
    }
}