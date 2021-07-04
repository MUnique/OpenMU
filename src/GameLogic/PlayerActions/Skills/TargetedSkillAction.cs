﻿// <copyright file="TargetedSkillAction.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

using System.Collections.Generic;
using System.Linq;
using Microsoft.Extensions.Logging;
using MUnique.OpenMU.DataModel;
using MUnique.OpenMU.DataModel.Configuration;
using MUnique.OpenMU.DataModel.Entities;
using MUnique.OpenMU.GameLogic.NPC;
using MUnique.OpenMU.GameLogic.Views.World;

namespace MUnique.OpenMU.GameLogic.PlayerActions.Skills
{
    /// <summary>
    /// Action to perform a skill which is explicitly aimed to a target.
    /// </summary>
    public class TargetedSkillAction
    {
        private static readonly Dictionary<short, short> SummonSkillToMonsterMapping = new ()
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
        public void PerformSkill(Player player, IAttackable target, ushort skillId)
        {
            using var loggerScope = player.Logger.BeginScope(this.GetType());
            var skillEntry = player.SkillList?.GetSkill(skillId);
            var skill = skillEntry?.Skill;
            if (skill is null || skill.SkillType == SkillType.PassiveBoost)
            {
                return;
            }

            if (target is null)
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
                if (!(target is ISupportWalk walker && walker.IsWalking))
                {
                    player.ViewPlugIns.GetPlugIn<IObjectMovedPlugIn>()?.ObjectMoved(target, MoveType.Instant);
                }

                return;
            }

            if (skill.SkillType == SkillType.SummonMonster && player.Summon is { } summon)
            {
                // remove summon, if exists
                summon.Item1.Dispose();
                player.SummonDied();
                return;
            }

            // enough mana, ag etc?
            if (!player.TryConsumeForSkill(skill))
            {
                return;
            }

            player.ForEachWorldObserver(obs => obs.ViewPlugIns.GetPlugIn<IShowSkillAnimationPlugIn>()?.ShowSkillAnimation(player, target, skill), true);
            if (skill.MovesToTarget)
            {
                player.Move(target.Position);
            }

            if (skill.MovesTarget)
            {
                target.MoveRandomly();
            }

            if (skill.SkillType == SkillType.SummonMonster)
            {
                if (SummonSkillToMonsterMapping.TryGetValue(skill.Number, out var monsterNumber)
                    && player.GameContext.Configuration.Monsters.FirstOrDefault(m => m.Number == monsterNumber) is { } monsterDefinition)
                {
                    player.CreateSummonedMonster(monsterDefinition);
                }
            }
            else
            {
                this.ApplySkill(player, target, skillEntry!);
            }
        }

        private void ApplySkill(Player player, IAttackable targetedTarget, SkillEntry skillEntry)
        {
            skillEntry.ThrowNotInitializedProperty(skillEntry.Skill is null, nameof(skillEntry.Skill));
            var skill = skillEntry.Skill;

            var targets = this.DetermineTargets(player, targetedTarget, skill);
            foreach (var target in targets)
            {
                if (skill.SkillType == SkillType.DirectHit || skill.SkillType == SkillType.CastleSiegeSkill)
                {
                    target.AttackBy(player, skillEntry);
                    target.ApplyElementalEffects(player, skillEntry);
                }
                else if (skill.MagicEffectDef != null)
                {
                    if (skill.SkillType == SkillType.Buff)
                    {
                        target.ApplyMagicEffect(player, skillEntry);
                    }
                    else if (skill.SkillType == SkillType.Regeneration)
                    {
                        target.ApplyRegeneration(player, skillEntry);
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
        }

        private IEnumerable<IAttackable> DetermineTargets(Player player, IAttackable targetedTarget, Skill skill)
        {
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
                if (skill.ImplicitTargetRange > 0)
                {
                    var targetsOfTarget = targetedTarget.CurrentMap?.GetAttackablesInRange(targetedTarget.Position, skill.ImplicitTargetRange) ?? Enumerable.Empty<IAttackable>();
                    if (!player.GameContext.Configuration.AreaSkillHitsPlayer && targetedTarget is Monster)
                    {
                        return targetsOfTarget.OfType<Monster>();
                    }

                    return targetsOfTarget;
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
}
