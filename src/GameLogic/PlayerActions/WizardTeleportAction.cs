// <copyright file="WizardTeleportAction.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.GameLogic.PlayerActions;

using MUnique.OpenMU.GameLogic.Views.World;
using MUnique.OpenMU.Pathfinding;

/// <summary>
/// Action to warp to another place through a gate.
/// </summary>
public class WizardTeleportAction
{
    private const ushort TeleportSkillId = 6;
    private const ushort TeleportTargetSkillId = 15;

    private static readonly byte[] PreventingMagicEffects =
    {
        0x39, // Stone
        0x3D, // Stun
        0x48, // Sleep
        0x92, // Freeze2
        0x93, // Earth Binds
    };

    /// <summary>
    /// Tries to teleport to the specified target with the teleport skill.
    /// </summary>
    /// <param name="player">The player.</param>
    /// <param name="target">The target.</param>
    public async ValueTask TryTeleportWithSkillAsync(Player player, Point target)
    {
        // todo: additional checks:
        //  - During castle siege, can't teleport over the non-destroyed gates (simple y-axis check)
        if (!player.IsAtSafezone()
            && player.IsActive()
            && player.SkillList?.GetSkill(TeleportSkillId) is { Skill: { } skill }
            && player.CurrentMap!.Terrain.WalkMap[target.X, target.Y]
            && !player.CurrentMap.Terrain.SafezoneMap[target.X, target.Y]
            && player.IsInRange(target, skill.Range)
            && CanPlayerBeTeleported(player)
            && await player.TryConsumeForSkillAsync(skill).ConfigureAwait(false))
        {
            _ = Task.Run(() => player.TeleportAsync(player.CurrentMap!, target, skill));
        }
        else
        {
            await player.InvokeViewPlugInAsync<ITeleportPlugIn>(p => p.ShowTeleportedAsync()).ConfigureAwait(false);
        }
    }

    /// <summary>
    /// Tries to teleport the target with the 'Teleport Ally' skill.
    /// </summary>
    /// <param name="player">The player.</param>
    /// <param name="targetId">The target identifier.</param>
    /// <param name="target">The target.</param>
    public async ValueTask TryTeleportTargetWithSkillAsync(Player player, ushort targetId, Point target)
    {
        if (!player.IsAtSafezone()
            && player.IsActive()
            && player.SkillList?.GetSkill(TeleportTargetSkillId) is { Skill: { } skill }
            && player.Party is not null
            && player.CurrentMap!.Terrain.WalkMap[target.X, target.Y]
            && !player.CurrentMap.Terrain.SafezoneMap[target.X, target.Y]
            && await player.GetObservingPlayerWithIdAsync(targetId).ConfigureAwait(false) is { } targetPlayer
            && targetPlayer.Party == player.Party
            && targetPlayer.IsActive()
            && CanPlayerBeTeleported(targetPlayer)
            && targetPlayer.IsInRange(target, skill.Range + 1)
            && await player.TryConsumeForSkillAsync(skill).ConfigureAwait(false))
        {
            _ = Task.Run(() => targetPlayer.TeleportAsync(player.CurrentMap!, target, skill));
        }
    }

    private static bool CanPlayerBeTeleported(Player player)
    {
        var currentEffects = player.MagicEffectList.ActiveEffects;
        if (currentEffects.Count == 0)
        {
            return true;
        }

        for (int i = 0; i < PreventingMagicEffects.Length; i++)
        {
            if (currentEffects.ContainsKey(PreventingMagicEffects[i]))
            {
                return false;
            }
        }

        return true;
    }
}