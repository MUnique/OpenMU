// <copyright file="ForceSkillAction.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.GameLogic.PlayerActions.Skills;

using System.Runtime.InteropServices;
using MUnique.OpenMU.PlugIns;

/// <summary>
/// The Force skill action.
/// </summary>
[PlugIn(nameof(ForceSkillAction), "Handles the force skill of the dark lord.")]
[Guid("552e4e3d-8215-44f4-bee3-b006da049eb2")]
public class ForceSkillAction : TargetedSkillDefaultPlugin
{
    private const ushort ForceWaveSkillId = 66;

    /// <inheritdoc/>
    public override short Key => 60;

    /// <inheritdoc/>
    public override async ValueTask PerformSkillAsync(Player player, IAttackable target, ushort skillId)
    {
        // Special handling of force wave skill. The client might send skill id 60,
        // even though it's performing force wave.
        await base.PerformSkillAsync(player, target, ForceWaveSkillId).ConfigureAwait(false);
    }
}