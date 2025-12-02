// <copyright file="NovaSkillStopPlugin.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.GameLogic.PlayerActions.Skills;

using System.Runtime.InteropServices;
using MUnique.OpenMU.PlugIns;

/// <summary>
/// The stop nova skill action.
/// </summary>
[PlugIn(nameof(NovaSkillStopPlugin), "Handles the stopping of nova skill of the wizard class.")]
[Guid("3cb98892-b3ce-42de-8956-5ed5625c6285")]
public class NovaSkillStopPlugin : TargetedSkillPluginBase
{
    /// <inheritdoc/>
    public override short Key => 40;

    /// <inheritdoc />
    public override async ValueTask PerformSkillAsync(Player player, IAttackable target, ushort skillId)
    {
        player.SkillCancelTokenSource?.CancelWithExtraTarget(target.Id);
    }
}