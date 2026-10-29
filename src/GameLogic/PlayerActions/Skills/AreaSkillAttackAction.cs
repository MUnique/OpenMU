// <copyright file="AreaSkillAttackAction.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.GameLogic.PlayerActions.Skills;

using System.Diagnostics.CodeAnalysis;
using MUnique.OpenMU.DataModel.Configuration;
using MUnique.OpenMU.GameLogic.Attributes;
using MUnique.OpenMU.GameLogic.PlugIns;
using MUnique.OpenMU.GameLogic.Views;
using MUnique.OpenMU.GameLogic.Views.World;
using MUnique.OpenMU.Pathfinding;

/// <summary>
/// Action to attack with a skill which inflicts damage to an area of the current map of the player.
/// </summary>
public class AreaSkillAttackAction
{
    private const short ElectricSpikeSkillId = 65;

    /// <summary>
    /// Performs the skill by the player at the specified area. Additionally, to the target area, a target object can be specified.
    /// </summary>
    /// <param name="player">The player.</param>
    /// <param name="extraTargetId">The extra target identifier.</param>
    /// <param name="skillId">The skill identifier.</param>
    /// <param name="targetAreaCenter">The coordinates of the center of the target area.</param>
    /// <param name="rotation">The rotation in which the player is looking. It's not really relevant for the hitted objects yet, but for some directed skills in the future it might be.</param>
    /// <param name="hitImplicitlyForExplicitSkill">If set to <c>true</c>, hit implicitly for <see cref="SkillType.AreaSkillExplicitHits"/>.</param>
    public async ValueTask AttackAsync(Player player, ushort extraTargetId, ushort skillId, Point targetAreaCenter, byte rotation, bool hitImplicitlyForExplicitSkill = false)
    {
        var skillEntry = player.SkillList?.GetSkill(skillId);
        if (skillEntry?.Skill is not { } skill || skill.SkillType == SkillType.PassiveBoost)
        {
            return;
        }

        if (!await AttackActionValidator.CanAttackAsync(player, skill).ConfigureAwait(false))
        {
            return;
        }

        if (!await player.TryConsumeForSkillAsync(skillEntry).ConfigureAwait(false))
        {
            return;
        }

        if (skill.SkillType is SkillType.AreaSkillExplicitHits)
        {
            player.SkillHitValidator.RegisterArea((ushort)skill.Number, skill, player.Position, targetAreaCenter);
        }

        if (skill.SkillType is SkillType.AreaSkillAutomaticHits or SkillType.AreaSkillExplicitTarget or SkillType.Buff
            || (skill.SkillType is SkillType.AreaSkillExplicitHits && hitImplicitlyForExplicitSkill))
        {
            // todo: delayed automatic hits, like evil spirit, flame, triple shot... when hitImplicitlyForExplicitSkill = true.
            await this.PerformAutomaticHitsAsync(player, extraTargetId, targetAreaCenter, skillEntry!, skill, rotation).ConfigureAwait(false);
        }

        await player.ForEachWorldObserverAsync<IShowAreaSkillAnimationPlugIn>(p => p.ShowAreaSkillAnimationAsync(player, skill, targetAreaCenter, rotation), true).ConfigureAwait(false);
    }

    private static bool AreaSkillSettingsAreDefault([NotNullWhen(true)] AreaSkillSettings? settings)
    {
        if (settings is null)
        {
            return true;
        }

        return !settings.UseDeferredHits
               && settings.DelayPerOneDistance <= TimeSpan.Zero
               && settings.MinimumNumberOfHitsPerTarget == 1
               && settings.MaximumNumberOfHitsPerTarget == 1
               && settings.MinimumNumberOfHitsPerAttack == 0
               && settings.MaximumNumberOfHitsPerAttack == 0
               && Math.Abs(settings.HitChancePerDistanceMultiplier - 1.0) <= 0.00001f;
    }

    /// <summary>
    /// Determines whether the specified target can be scheduled for a hit.
    /// </summary>
    /// <param name="target">The potential target.</param>
    /// <param name="attacker">The attacker.</param>
    /// <param name="filter">The projectile filter, if multiple projectiles are used.</param>
    /// <param name="rotation">The rotation of the attack.</param>
    /// <param name="projectileIndex">The index of the current projectile.</param>
    /// <param name="extraProjectiles">The number of extra projectiles.</param>
    /// <returns><c>true</c> if the target can be scheduled.</returns>
    private static bool IsTargetAvailable(IAttackable target, ILocateable attacker, FrustumBasedTargetFilter? filter, byte rotation, int projectileIndex, int extraProjectiles)
    {
        // Skip targets that have died in previous rounds.
        if (!target.IsAlive)
        {
            return false;
        }

        // For multiple projectiles, only the projectile covering the target can hit it.
        return filter is null || filter.IsTargetWithinBounds(attacker, target, rotation, projectileIndex, extraProjectiles);
    }

    /// <summary>
    /// Rolls whether an attack round hits a target at the specified distance.
    /// The first <see cref="AreaSkillSettings.MinimumNumberOfHitsPerTarget"/> hits
    /// are guaranteed; further hits get less likely with distance.
    /// </summary>
    /// <param name="settings">The area skill settings.</param>
    /// <param name="distance">The distance between attacker and target.</param>
    /// <param name="attackRound">The current attack round.</param>
    /// <param name="attackCount">The number of scheduled attacks so far.</param>
    /// <param name="minAttacks">The number of attacks after which the hit chance drops.</param>
    /// <returns><c>true</c> if the hit proceeds.</returns>
    private static bool RollHitChance(AreaSkillSettings settings, double distance, int attackRound, int attackCount, int minAttacks)
    {
        double hitChance;
        if (attackRound >= settings.MinimumNumberOfHitsPerTarget)
        {
            hitChance = Math.Pow(settings.HitChancePerDistanceMultiplier, distance);
        }
        else if (attackCount >= minAttacks)
        {
            hitChance = 0.5;
        }
        else
        {
            hitChance = 1.0;
        }

        return hitChance >= 1.0 || Rand.NextRandomBool(hitChance);
    }

    /// <summary>
    /// Applies the party costs of Electric Spike: nearby party members lose
    /// some health and mana when the skill hits.
    /// </summary>
    /// <param name="player">The player who performed the skill.</param>
    /// <param name="attackCount">The number of scheduled attacks.</param>
    private static void ApplyElectricSpikePartyCost(Player player, int attackCount)
    {
        if (attackCount == 0 || player.Attributes![Stats.NearbyPartyMemberCount] <= 0)
        {
            return;
        }

        foreach (var partyMember in player.Party?.PartyList.OfType<Player>().Where(m => m.Observers.Contains(player)) ?? [])
        {
            if (partyMember.Attributes is { } memberAttributes)
            {
                memberAttributes[Stats.CurrentHealth] *= 0.8f;
                memberAttributes[Stats.CurrentMana] *= 0.95f;
            }
        }
    }

    private async ValueTask PerformAutomaticHitsAsync(Player player, ushort extraTargetId, Point targetAreaCenter, SkillEntry skillEntry, Skill skill, byte rotation)
    {
        bool isCombo = false;
        if (player.ComboState is { } comboState)
        {
            isCombo = await comboState.RegisterSkillAsync(skill).ConfigureAwait(false);
        }

        var effectiveRotation = AreaSkillTargetSelector.CorrectRotationForLockedTarget(player, skill, extraTargetId, rotation);

        IAttackable? extraTarget = null;
        var targets = AreaSkillTargetSelector.GetTargets(player, targetAreaCenter, skill, effectiveRotation, extraTargetId);
        if (skill.AreaSkillSettings is not { } areaSkillSettings
            || AreaSkillSettingsAreDefault(areaSkillSettings))
        {
            // Just hit all targets once.
            foreach (var target in targets)
            {
                if (target.Id == extraTargetId)
                {
                    extraTarget = target;
                }

                await this.ApplySkillAsync(player, skillEntry, target, targetAreaCenter, isCombo).ConfigureAwait(false);
            }
        }
        else
        {
            extraTarget = await this.AttackTargetsAsync(player, extraTargetId, targetAreaCenter, skillEntry, areaSkillSettings, targets, effectiveRotation, isCombo).ConfigureAwait(false);
        }

        if (isCombo)
        {
            await player.ForEachWorldObserverAsync<IShowSkillAnimationPlugIn>(p => p.ShowComboAnimationAsync(player, extraTarget), true).ConfigureAwait(false);
        }
    }

    private async Task<IAttackable?> AttackTargetsAsync(Player player, ushort extraTargetId, Point targetAreaCenter, SkillEntry skillEntry, AreaSkillSettings areaSkillSettings, IEnumerable<IAttackable> targets, byte rotation, bool isCombo)
    {
        skillEntry.ThrowNotInitializedProperty(skillEntry.Skill is null, nameof(skillEntry.Skill));
        var skill = skillEntry.Skill!;
        IAttackable? extraTarget = null;
        var attackCount = 0;
        var maxAttacks = areaSkillSettings.MaximumNumberOfHitsPerAttack == 0 ? int.MaxValue : areaSkillSettings.MaximumNumberOfHitsPerAttack;
        var minAttacks = areaSkillSettings.MinimumNumberOfHitsPerAttack == 0 ? maxAttacks : areaSkillSettings.MinimumNumberOfHitsPerAttack;
        var currentDelay = TimeSpan.Zero;

        var orderedTargets = targets.ToList();
        FrustumBasedTargetFilter? filter = null;
        var extraProjectiles = DeferredHitScheduler.GetExtraProjectiles(player, skillEntry, areaSkillSettings);
        var projectileCount = 1;
        var attackRounds = areaSkillSettings.MaximumNumberOfHitsPerTarget;
        var effectiveRotation = rotation;

        if (areaSkillSettings is { UseFrustumFilter: true, ProjectileCount: > 1 })
        {
            // Order targets by distance to process nearest targets first
            orderedTargets.Sort((a, b) => player.GetDistanceTo(a).CompareTo(player.GetDistanceTo(b)));
            filter = AreaSkillTargetSelector.GetLaneFilter(areaSkillSettings);
            projectileCount = areaSkillSettings.ProjectileCount + extraProjectiles;
            attackRounds = 1; // One attack round per projectile
            maxAttacks = projectileCount;
            minAttacks = projectileCount;

            extraTarget = orderedTargets.FirstOrDefault(t => t.Id == extraTargetId);
            if (extraTarget is not null)
            {
                // In this case we just calculate the angle on server side, so that lags
                // or desynced positions may not have such a big impact
                var angle = (float)player.Position.GetAngleDegreeTo(extraTarget.Position);
                effectiveRotation = AreaSkillTargetSelector.ConvertAngleToRotation(angle);
            }
        }

        ValueTask ApplyToTargetAsync(IAttackable target) => this.ApplySkillAsync(player, skillEntry, target, targetAreaCenter, isCombo);
        var scheduler = new DeferredHitScheduler(player, skillEntry, skill, areaSkillSettings, targetAreaCenter, effectiveRotation, extraTargetId, extraProjectiles, ApplyToTargetAsync);

        // Process each projectile separately
        for (int projectileIndex = 0; projectileIndex < projectileCount; projectileIndex++)
        {
            if (attackCount >= maxAttacks)
            {
                break;
            }

            var consideredIds = new HashSet<ushort>();
            for (int attackRound = 0; attackRound < attackRounds; attackRound++)
            {
                if (attackCount >= maxAttacks)
                {
                    break;
                }

                foreach (var target in orderedTargets)
                {
                    if (attackCount >= maxAttacks)
                    {
                        break;
                    }

                    if (target.Id == extraTargetId)
                    {
                        extraTarget = target;
                    }

                    if (!IsTargetAvailable(target, player, filter, effectiveRotation, projectileIndex, extraProjectiles))
                    {
                        continue;
                    }

                    var distance = player.GetDistanceTo(target);
                    consideredIds.Add(target.Id);
                    if (!RollHitChance(areaSkillSettings, distance, attackRound, attackCount, minAttacks))
                    {
                        continue;
                    }

                    var distanceDelay = areaSkillSettings.DelayPerOneDistance * distance;
                    var attackDelay = currentDelay + distanceDelay;
                    attackCount++;

                    await scheduler.DeliverHitAsync(target, attackDelay, projectileIndex).ConfigureAwait(false);

                    if (filter != null)
                    {
                        break;  // This projectile has hit, so we move on to the next projectile
                    }
                }

                if (areaSkillSettings.UseDeferredHits && filter is null)
                {
                    scheduler.ScheduleWavefrontSweeps(new HashSet<ushort>(consideredIds), attackRound, maxAttacks - attackCount, currentDelay);
                }

                currentDelay += areaSkillSettings.DelayBetweenHits;
            }
        }

        if (skillEntry.Skill?.Number == ElectricSpikeSkillId)
        {
            ApplyElectricSpikePartyCost(player, attackCount);
        }

        return extraTarget;
    }

    private async ValueTask ApplySkillAsync(Player player, SkillEntry skillEntry, IAttackable target, Point targetAreaCenter, bool isCombo)
    {
        skillEntry.ThrowNotInitializedProperty(skillEntry.Skill is null, nameof(skillEntry.Skill));
        var skill = skillEntry.Skill;

        if (skill.SkillType == SkillType.Buff)
        {
            await target.ApplyMagicEffectAsync(player, skillEntry).ConfigureAwait(false);
            return;
        }

        var hitInfo = await target.AttackByAsync(player, skillEntry, isCombo, 1, skill.NumberOfHitsPerAttack > 1 ? false : null).ConfigureAwait(false);
        await target.TryApplyElementalEffectsAsync(player, skillEntry, hitInfo).ConfigureAwait(false);

        for (int hit = 2; hit <= skill.NumberOfHitsPerAttack; hit++)
        {
            await target.AttackByAsync(player, skillEntry, isCombo, 1, hit == skill.NumberOfHitsPerAttack).ConfigureAwait(false);
        }

        if (player.GameContext.PlugInManager.GetStrategy<short, IAreaSkillPlugIn>(skillEntry.Skill.Number) is { } strategy)
        {
            await strategy.AfterTargetGotAttackedAsync(player, target, skillEntry, targetAreaCenter, hitInfo).ConfigureAwait(false);
            return;
        }

        var baseSkill = skillEntry.GetBaseSkill();
        if (player.GameContext.PlugInManager.GetStrategy<short, IAreaSkillPlugIn>(baseSkill.Number) is { } baseSkillStrategy)
        {
            await baseSkillStrategy.AfterTargetGotAttackedAsync(player, target, skillEntry, targetAreaCenter, hitInfo).ConfigureAwait(false);
        }
    }
}