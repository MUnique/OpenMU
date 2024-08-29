// <copyright file="PlasmaStormSkillPlugIn.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.GameLogic.PlayerActions.Skills;

using System.Runtime.InteropServices;
using MUnique.OpenMU.GameLogic.PlugIns;
using MUnique.OpenMU.GameLogic.Views.Inventory;
using MUnique.OpenMU.Pathfinding;
using MUnique.OpenMU.PlugIns;

/// <summary>
/// Handles the plasma storm skill of the fenrir pet. It randomly halves the durability of a target's equipped item.
/// </summary>
[PlugIn("Plasma Storm Skill", "Handles the plasma storm skill of the fenrir pet. It randomly halves the durability of a target's equipped item.")]
[Guid("5EF7C564-B32B-4630-9380-0233BECFA663")]
public class PlasmaStormSkillPlugIn : IAreaSkillPlugIn
{
    /// <inheritdoc />
    public short Key => 76;

    /// <inheritdoc />
    public async ValueTask AfterTargetGotAttackedAsync(IAttacker attacker, IAttackable target, SkillEntry skillEntry, Point targetAreaCenter, HitInfo? hitInfo)
    {
        if (target is Player targetPlayer
            && Rand.NextRandomBool(25)
            && targetPlayer.Inventory?.EquippedItems.SelectRandom() is { } randomItem)
        {
            randomItem.Durability /= 2;
            await targetPlayer.InvokeViewPlugInAsync<IItemDurabilityChangedPlugIn>(p => p.ItemDurabilityChangedAsync(randomItem, false)).ConfigureAwait(false);
        }
    }
}