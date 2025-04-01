﻿// <copyright file="ForceWaveStrengSkillAction.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.GameLogic.PlayerActions.Skills;

using System.Runtime.InteropServices;
using MUnique.OpenMU.GameLogic.Attributes;
using MUnique.OpenMU.PlugIns;

/// <summary>
/// The Force Wave Strengthener skill action.
/// </summary>
[PlugIn(nameof(ForceWaveStrengSkillAction), "Handles the force wave strengthener skill of the dark lord.")]
[Guid("9072ce9c-1482-4838-ba0a-4c312062e090")]
public class ForceWaveStrengSkillAction : TargetedSkillDefaultPlugin
{
    private const ushort ForceWaveStrengAltSkillId = 5090;

    /// <inheritdoc/>
    public override short Key => 509;

    /// <inheritdoc/>
    public override async ValueTask PerformSkillAsync(Player player, IAttackable target, ushort skillId)
    {
        // If there is an equipped scepter with skill, we call the alt master skill which has force wave (not force) as its related skill
        if (player.Attributes?[Stats.IsScepterEquipped] > 0 is true
            && player.Inventory!.EquippedItems.First(item => item.ItemSlot == InventoryConstants.LeftHandSlot).HasSkill)
        {
            skillId = ForceWaveStrengAltSkillId;
        }

        await base.PerformSkillAsync(player, target, skillId).ConfigureAwait(false);
    }
}