// <copyright file="SoulBarrierStrengSkillAction.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.GameLogic.PlayerActions.Skills;

using System.Runtime.InteropServices;
using MUnique.OpenMU.AttributeSystem;
using MUnique.OpenMU.GameLogic.Attributes;
using MUnique.OpenMU.GameLogic.Views.World;
using MUnique.OpenMU.PlugIns;

/// <summary>
/// The Soul Barrier Strengthener skill action.
/// </summary>
[PlugIn(nameof(SoulBarrierStrengSkillAction), "Handles the soul barrier strengthener skill of the grand master.")]
[Guid("05fdea2a-ac92-4b2c-8305-001e97ec26a8")]
public class SoulBarrierStrengSkillAction : TargetedSkillDefaultPlugin
{
    private const ushort SoulBarrierStrengSkilId = 403;
    private const ushort SoulBarrierProficieSkilId = 404;

    /// <inheritdoc/>
    public override short Key => 403;

    /// <inheritdoc/>
    public override async ValueTask PerformSkillAsync(Player player, IAttackable target, ushort skillId)
    {
        var skillEntry = player.SkillList!.GetSkill(skillId);
        var skill = skillEntry?.Skill;
        if (skill is null)
        {
            return;
        }

        var miniGame = player.CurrentMiniGame;
        var inMiniGame = miniGame is { };
        var isBuff = skill.SkillType is SkillType.Buff or SkillType.Regeneration;
        if (player.IsAtSafezone() && !(inMiniGame && isBuff))
        {
            return;
        }

        if (inMiniGame && !miniGame!.IsSkillAllowed(skill, player, target))
        {
            return;
        }

        if (!target.IsActive())
        {
            return;
        }

        if (!target.CheckSkillTargetRestrictions(player, skill))
        {
            return;
        }

        if (!player.IsInRange(target.Position, skill.Range + 2))
        {
            // target position might be out of sync so we send the current coordinates to the client again.
            if (!(target is ISupportWalk { IsWalking: true }))
            {
                await player.InvokeViewPlugInAsync<IObjectMovedPlugIn>(p => p.ObjectMovedAsync(target, MoveType.Instant)).ConfigureAwait(false);
            }

            return;
        }

        if (!await player.TryConsumeForSkillAsync(skill).ConfigureAwait(false))
        {
            return;
        }

        if (skillEntry!.PowerUps is null)
        {
            player.CreateMagicEffectPowerUp(skillEntry);

            var strengSkillLevel = skillId == SoulBarrierProficieSkilId ? player.SkillList!.GetSkill(SoulBarrierStrengSkilId) : skillEntry;
            skillEntry.PowerUps =
            [
                .. skillEntry.PowerUps!,
                (Stats.SoulBarrierManaTollPerHit, new AttributeRelationshipElement(
                    [player.Attributes!.GetOrCreateAttribute(Stats.MaximumMana)],
                    new ConstantElement(strengSkillLevel!.Level * 0.001f), // extra 0.1% per streng skill level
                    InputOperator.Multiply))
            ];
        }

        await AttackableExtensions.ApplyMagicEffectAsync(target, player, skillEntry).ConfigureAwait(false);
        await player.ForEachWorldObserverAsync<IShowSkillAnimationPlugIn>(p => p.ShowSkillAnimationAsync(player, target, skill, true), true).ConfigureAwait(false);
    }
}