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

    /// <summary>
    /// Tries to teleport to the specified target with the teleport skill.
    /// </summary>
    /// <param name="player">The player.</param>
    /// <param name="target">The target.</param>
    public async ValueTask TryTeleportWithSkillAsync(Player player, Point target)
    {
        // todo: additional checks:
        //  - MagicEffects which should also prevent skill usage: Stone (0x39), Stun (0x3D), Sleep (0x48), Freeze2(0x92), Earth Binds(0x93)
        //  - During castle siege, can't teleport over the non-destroyed gates (simple y-axis check)
        if (player.SkillList?.GetSkill(TeleportSkillId) is { Skill: { } skill }
            && player.CurrentMap!.Terrain.WalkMap[target.X, target.Y]
            && !player.CurrentMap.Terrain.SafezoneMap[target.X, target.Y]
            && player.IsInRange(target, skill.Range)
            && await player.TryConsumeForSkillAsync(skill))
        {
            _ = Task.Run(() => player.TeleportAsync(target, skill));
        }
        else
        {
            await player.InvokeViewPlugInAsync<ITeleportPlugIn>(p => p.ShowTeleportedAsync()).ConfigureAwait(false);
        }
    }
}