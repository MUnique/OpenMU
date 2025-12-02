// <copyright file="ForceWaveStrengSkillAction.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.GameLogic.PlayerActions.Skills;

using System.Runtime.InteropServices;
using MUnique.OpenMU.PlugIns;

/// <summary>
/// The Force Wave Strengthener skill action.
/// </summary>
[PlugIn(nameof(ForceWaveStrengSkillAction), "Handles the force wave strengthener skill of the dark lord.")]
[Guid("9072ce9c-1482-4838-ba0a-4c312062e090")]
public class ForceWaveStrengSkillAction : ForceSkillAction
{
    /// <inheritdoc/>
    public override short Key => 509;

    /// <inheritdoc/>
    public override async ValueTask PerformSkillAsync(Player player, IAttackable target, ushort skillId)
    {
        // Originally, force wave strengthener could be used on top of force or force wave skills.
        // In OpenMU, you can only use it with force wave (which means a scepter with skill must be equipped).
        // This simplifies code at the expense of not allowing usage of FWS with any weapon (or fists).
        // Since a Lord Emperor will likely be wearing a skilled scepter as a weapon, this is acceptable.
        // It also makes the skill name more accurate :-).
        if (player.SkillList?.ContainsSkill(ForceWaveSkillId) is true)
        {
            await base.PerformSkillAsync(player, target, skillId).ConfigureAwait(false);
        }
    }
}