// <copyright file="DrainLifeSkillPlugIn.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.GameLogic.PlayerActions.Skills;

using System.Runtime.InteropServices;
using MUnique.OpenMU.GameLogic.Attributes;
using MUnique.OpenMU.GameLogic.PlugIns;
using MUnique.OpenMU.GameLogic.Views.Character;
using MUnique.OpenMU.Pathfinding;
using MUnique.OpenMU.PlugIns;

/// <summary>
/// Handles the drain life skill of the summoner class. Additionally to the attacked target, it regains life for damage dealt.
/// </summary>
[PlugIn(nameof(ChainLightningSkillPlugIn), "Handles the drain life skill of the summoner class. Additionally to the attacked target, it regains life for damage dealt.")]
[Guid("9A5A5671-3A8C-4C01-984F-1A8F8E0E7BDA")]
public class DrainLifeSkillPlugIn : IAreaSkillPlugIn
{
    /// <inheritdoc/>
    public short Key => 214;

    /// <inheritdoc/>
    public async ValueTask AfterTargetGotAttackedAsync(IAttacker attacker, IAttackable target, SkillEntry skillEntry, Point targetAreaCenter, HitInfo? hitInfo)
    {
        if (attacker is not Player attackerPlayer
            || hitInfo is not { HealthDamage: > 0 }
            || attackerPlayer.Attributes is not { } playerAttributes)
        {
            return;
        }

        playerAttributes[Stats.CurrentHealth] = (uint)Math.Min(playerAttributes[Stats.MaximumHealth], playerAttributes[Stats.CurrentHealth] + hitInfo.Value.HealthDamage);
    }
}