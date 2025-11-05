// <copyright file="FireScreamSkillPlugIn.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.GameLogic.PlayerActions.Skills;

using System.Runtime.InteropServices;
using MUnique.OpenMU.GameLogic.PlugIns;
using MUnique.OpenMU.Pathfinding;
using MUnique.OpenMU.PlugIns;

/// <summary>
/// Handles the fire scream skill of the dark lord class. After the main attack, it triggers an explosion (Explosion79) at the target's position.
/// </summary>
[PlugIn(nameof(FireScreamSkillPlugIn), "Handles the fire scream skill of the dark lord class. After the main attack, it triggers an explosion at the target's position.")]
[Guid("A9F2E8D4-5C6B-4E7A-9F3D-1B8C4A6E2D9F")]
public class FireScreamSkillPlugIn : IAreaSkillPlugIn
{
    /// <inheritdoc />
    public short Key => 78; // SkillNumber.FireScream

    /// <inheritdoc />
    public async ValueTask AfterTargetGotAttackedAsync(IAttacker attacker, IAttackable target, SkillEntry skillEntry, Point targetAreaCenter, HitInfo? hitInfo)
    {
        if (attacker is not Player player || player.GameContext.Configuration is not { } config)
        {
            return;
        }

        // Get the Explosion79 skill
        var explosionSkill = config.Skills.FirstOrDefault(s => s.Number == 79);
        if (explosionSkill is null)
        {
            return;
        }

        // Create a skill entry for the explosion
        var explosionSkillEntry = new SkillEntry
        {
            Level = skillEntry.Level,
            Skill = explosionSkill,
        };

        // Delay the explosion slightly for visual effect
        await Task.Delay(100).ConfigureAwait(false);

        // Get all attackables in range of the explosion (distance: 2 from the explosion skill definition)
        var explosionTargets = target.CurrentMap?.GetAttackablesInRange(target.Position, explosionSkill.Range)
            .Where(t => t.IsAlive && !t.IsAtSafezone())
            .ToList() ?? [];

        // Attack each target with the explosion
        foreach (var explosionTarget in explosionTargets)
        {
            if (explosionTarget != player && !player.IsAtSafezone())
            {
                await explosionTarget.AttackByAsync(player, explosionSkillEntry, false).ConfigureAwait(false);
                await explosionTarget.TryApplyElementalEffectsAsync(player, explosionSkillEntry).ConfigureAwait(false);
            }
        }
    }
}
