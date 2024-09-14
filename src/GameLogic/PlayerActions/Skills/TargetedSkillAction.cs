// <copyright file="TargetedSkillAction.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.GameLogic.PlayerActions.Skills;

using MUnique.OpenMU.GameLogic.Attributes;
using MUnique.OpenMU.GameLogic.NPC;
using MUnique.OpenMU.GameLogic.PlugIns;
using MUnique.OpenMU.GameLogic.Views.World;

/// <summary>
/// Action to perform a skill which is explicitly aimed to a target.
/// </summary>
public class TargetedSkillAction
{
    private static readonly Dictionary<short, short> SummonSkillToMonsterMapping = new()
    {
        { 30, 26 }, // Goblin
        { 31, 32 }, // Stone Golem
        { 32, 21 }, // Assassin
        { 33, 20 }, // Elite Yeti
        { 34, 10 }, // Dark Knight
        { 35, 150 }, // Bali
        { 36, 151 }, // Soldier
    };

    /// <summary>
    /// Performs the skill.
    /// </summary>
    /// <param name="player">The player.</param>
    /// <param name="target">The target.</param>
    /// <param name="skillId">The skill identifier.</param>
    public async ValueTask PerformSkillAsync(Player player, IAttackable target, ushort skillId)
    {
        using var loggerScope = player.Logger.BeginScope(this.GetType());

        if (target is null)
        {
            return;
        }

        if (player.Attributes is not { } attributes)
        {
            return;
        }

        if (attributes[Stats.IsStunned] > 0)
        {
            player.Logger.LogWarning($"Probably Hacker - player {player} is attacking in stunned state");
            return;
        }

        if (attributes[Stats.IsAsleep] > 0)
        {
            player.Logger.LogWarning($"Probably Hacker - player {player} is attacking in asleep state");
            return;
        }

        var skillEntry = player.SkillList?.GetSkill(skillId);
        var skill = skillEntry?.Skill;
        if (skill is null || skill.SkillType == SkillType.PassiveBoost)
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

        if (skill.SkillType == SkillType.SummonMonster && player.Summon is { })
        {
            await player.RemoveSummonAsync().ConfigureAwait(false);
            return;
        }

        // enough mana, ag etc?
        if (!await player.TryConsumeForSkillAsync(skill).ConfigureAwait(false))
        {
            return;
        }

        if (skill.MovesToTarget)
        {
            await player.MoveAsync(target.Position).ConfigureAwait(false);
        }

        if (skill.MovesTarget)
        {
            await target.MoveRandomlyAsync().ConfigureAwait(false);
        }

        var effectApplied = false;
        if (skill.SkillType == SkillType.SummonMonster)
        {
            if (SummonSkillToMonsterMapping.TryGetValue(skill.Number, out var monsterNumber)
                && player.GameContext.Configuration.Monsters.FirstOrDefault(m => m.Number == monsterNumber) is { } monsterDefinition)
            {
                await player.CreateSummonedMonsterAsync(monsterDefinition).ConfigureAwait(false);
            }
        }
        else
        {
            effectApplied = await this.ApplySkillAsync(player, target, skillEntry!).ConfigureAwait(false);
        }

        await player.ForEachWorldObserverAsync<IShowSkillAnimationPlugIn>(p => p.ShowSkillAnimationAsync(player, target, skill, effectApplied), true).ConfigureAwait(false);
    }

    private async ValueTask<bool> ApplySkillAsync(Player player, IAttackable targetedTarget, SkillEntry skillEntry)
    {
        skillEntry.ThrowNotInitializedProperty(skillEntry.Skill is null, nameof(skillEntry.Skill));
        var skill = skillEntry.Skill;
        var success = false;
        var targets = this.DetermineTargets(player, targetedTarget, skill);
        bool isCombo = false;
        if (skill.SkillType is SkillType.DirectHit or SkillType.CastleSiegeSkill
            && player.ComboState is { } comboState
            && !targetedTarget.IsAtSafezone()
            && !player.IsAtSafezone()
            && targetedTarget.IsActive()
            && player.IsActive())
        {
            isCombo = await comboState.RegisterSkillAsync(skill).ConfigureAwait(false);
        }

        foreach (var target in targets)
        {
            if (skill.SkillType == SkillType.DirectHit || skill.SkillType == SkillType.CastleSiegeSkill)
            {
                if (!target.IsAtSafezone() && !player.IsAtSafezone() && target != player)
                {
                    await target.AttackByAsync(player, skillEntry, isCombo).ConfigureAwait(false);
                    player.LastAttackedTarget.SetTarget(target);
                    success = await target.TryApplyElementalEffectsAsync(player, skillEntry).ConfigureAwait(false) || success;
                }
            }
            else if (skill.MagicEffectDef != null)
            {
                // Buffs are allowed in the Safezone of Blood Castle.
                var canDoBuff = !player.IsAtSafezone() || player.CurrentMiniGame is { };
                if (!canDoBuff)
                {
                    player.Logger.LogWarning($"Can't apply magic effect when being in the safezone. skill: {skill.Name} ({skill.Number}), skillType: {skill.SkillType}.");
                    break;
                }

                if (skill.SkillType == SkillType.Buff)
                {
                    await target.ApplyMagicEffectAsync(player, skillEntry).ConfigureAwait(false);
                    success = true;
                }
                else if (skill.SkillType == SkillType.Regeneration)
                {
                    await target.ApplyRegenerationAsync(player, skillEntry).ConfigureAwait(false);
                    success = true;
                }
                else
                {
                    player.Logger.LogWarning($"Skill.MagicEffectDef isn't null, but it's not a buff or regeneration skill. skill: {skill.Name} ({skill.Number}), skillType: {skill.SkillType}.");
                }
            }
            else
            {
                player.Logger.LogWarning($"Skill.MagicEffectDef is null, skill: {skill.Name} ({skill.Number}), skillType: {skill.SkillType}.");
            }
        }

        if (isCombo)
        {
            await player.ForEachWorldObserverAsync<IShowSkillAnimationPlugIn>(p => p.ShowComboAnimationAsync(player, targetedTarget), true).ConfigureAwait(false);
        }

        return success;
    }

    private IEnumerable<IAttackable> DetermineTargets(Player player, IAttackable targetedTarget, Skill skill)
    {
        if (skill.Target == SkillTarget.ImplicitPlayer)
        {
            return player.GetAsEnumerable();
        }

        if (skill.Target == SkillTarget.ImplicitParty)
        {
            if (player.Party != null)
            {
                return player.Party.PartyList.OfType<IAttackable>().Where(p => player.Observers.Contains((IWorldObserver)p));
            }

            return player.GetAsEnumerable();
        }

        if (skill.Target == SkillTarget.Explicit)
        {
            return targetedTarget.GetAsEnumerable();
        }

        if (skill.Target == SkillTarget.ExplicitWithImplicitInRange)
        {
            if (player.GameContext.PlugInManager.GetStrategy<short, IAreaSkillTargetFilter>(skill.Number) is { } filterPlugin)
            {
                var rotationToTarget = (byte)(player.Position.GetAngleDegreeTo(targetedTarget.Position) / 360.0 * 255.0);
                var attackablesInRange =
                    player.CurrentMap?
                        .GetAttackablesInRange(player.Position, skill.Range)
                        .Where(a => a != player)
                        .Where(a => player.GameContext.Configuration.AreaSkillHitsPlayer || a is NonPlayerCharacter)
                        .Where(a => !a.IsAtSafezone())
                        .Where(a => filterPlugin.IsTargetWithinBounds(player, a, player.Position, rotationToTarget))
                        .ToList();
                if (attackablesInRange is not null)
                {
                    if (!attackablesInRange.Contains(targetedTarget))
                    {
                        attackablesInRange.Add(targetedTarget);
                    }

                    return attackablesInRange;
                }
            }
            else if (skill.ImplicitTargetRange > 0)
            {
                var targetsOfTarget = targetedTarget.CurrentMap?.GetAttackablesInRange(targetedTarget.Position, skill.ImplicitTargetRange) ?? Enumerable.Empty<IAttackable>();
                if (!player.GameContext.Configuration.AreaSkillHitsPlayer && targetedTarget is Monster)
                {
                    return targetsOfTarget.OfType<Monster>();
                }

                return targetsOfTarget;
            }
            else
            {
                // do nothing.
            }

            return targetedTarget.GetAsEnumerable();
        }

        var targets = player.CurrentMap?.GetAttackablesInRange(player.Position, skill.ImplicitTargetRange) ?? Enumerable.Empty<IAttackable>();

        if (skill.Target == SkillTarget.ImplicitAllInRange)
        {
            return targets;
        }

        if (skill.Target == SkillTarget.ImplicitPlayersInRange)
        {
            return targets.OfType<Player>();
        }

        if (skill.Target == SkillTarget.ImplicitNpcsInRange)
        {
            return targets.OfType<Monster>();
        }

        return Enumerable.Empty<IAttackable>();
    }
}