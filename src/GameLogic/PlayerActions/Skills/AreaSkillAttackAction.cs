// <copyright file="AreaSkillAttackAction.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.GameLogic.PlayerActions.Skills;

using System.Collections.Concurrent;
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
    private const int UndefinedTarget = 0xFFFF;

    private static readonly ConcurrentDictionary<AreaSkillSettings, FrustumBasedTargetFilter> FrustumFilters = new();

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
        var skill = skillEntry?.Skill;
        if (skill is null || skill.SkillType == SkillType.PassiveBoost)
        {
            return;
        }

        if (!await player.TryConsumeForSkillAsync(skill).ConfigureAwait(false))
        {
            return;
        }

        if (skill.SkillType is SkillType.AreaSkillAutomaticHits or SkillType.AreaSkillExplicitTarget
            || (skill.SkillType is SkillType.AreaSkillExplicitHits && hitImplicitlyForExplicitSkill))
        {
            // todo: delayed automatic hits, like evil spirit, flame, triple shot... when hitImplicitlyForExplicitSkill = true.

            await this.PerformAutomaticHitsAsync(player, extraTargetId, targetAreaCenter, skillEntry!, skill, rotation).ConfigureAwait(false);
        }

        await player.ForEachWorldObserverAsync<IShowAreaSkillAnimationPlugIn>(p => p.ShowAreaSkillAnimationAsync(player, skill, targetAreaCenter, rotation), true).ConfigureAwait(false);
    }

    private async ValueTask PerformAutomaticHitsAsync(Player player, ushort extraTargetId, Point targetAreaCenter, SkillEntry skillEntry, Skill skill, byte rotation)
    {
        if (player.Attributes is not { } attributes)
        {
            return;
        }

        if (attributes[Stats.IsStunned] > 0)
        {
            player.Logger.LogWarning("Probably Hacker - player {player} is attacking in stunned state", player);
            return;
        }

        if (player.IsAtSafezone())
        {
            player.Logger.LogWarning("Probably Hacker - player {player} is attacking from safezone", player);
            return;
        }

        if (player.Attributes[Stats.AmmunitionConsumptionRate] > player.Attributes[Stats.AmmunitionAmount])
        {
            return;
        }

        bool isCombo = false;
        if (player.ComboState is { } comboState)
        {
            isCombo = await comboState.RegisterSkillAsync(skill).ConfigureAwait(false);
        }

        IAttackable? extraTarget = null;
        var targets = GetTargets(player, targetAreaCenter, skill, rotation, extraTargetId);
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
            extraTarget = await this.AttackTargetsAsync(player, extraTargetId, targetAreaCenter, skillEntry, areaSkillSettings, targets, isCombo).ConfigureAwait(false);
        }

        if (isCombo)
        {
            await player.ForEachWorldObserverAsync<IShowSkillAnimationPlugIn>(p => p.ShowComboAnimationAsync(player, extraTarget), true).ConfigureAwait(false);
        }
    }

    private async Task<IAttackable?> AttackTargetsAsync(Player player, ushort extraTargetId, Point targetAreaCenter, SkillEntry skillEntry, AreaSkillSettings areaSkillSettings, IEnumerable<IAttackable> targets, bool isCombo)
    {
        IAttackable? extraTarget = null;
        var attackCount = 0;
        var maxAttacks = areaSkillSettings.MaximumNumberOfHitsPerAttack == 0 ? int.MaxValue : areaSkillSettings.MaximumNumberOfHitsPerAttack;
        var currentDelay = TimeSpan.Zero;

        for (int attackRound = 0; attackRound < areaSkillSettings.MaximumNumberOfHitsPerTarget; attackRound++)
        {
            if (attackCount > maxAttacks)
            {
                break;
            }

            foreach (var target in targets)
            {
                if (target.Id == extraTargetId)
                {
                    extraTarget = target;
                }

                var hitChance = attackRound < areaSkillSettings.MinimumNumberOfHitsPerTarget
                    ? 1.0
                    : Math.Min(areaSkillSettings.HitChancePerDistanceMultiplier, Math.Pow(areaSkillSettings.HitChancePerDistanceMultiplier, player.GetDistanceTo(target)));
                if (hitChance < 1.0 && !Rand.NextRandomBool(hitChance))
                {
                    continue;
                }

                var distanceDelay = areaSkillSettings.DelayPerOneDistance * player.GetDistanceTo(target);
                var attackDelay = currentDelay + distanceDelay;
                attackCount++;

                if (attackDelay == TimeSpan.Zero)
                {
                    await this.ApplySkillAsync(player, skillEntry, target, targetAreaCenter, isCombo).ConfigureAwait(false);
                }
                else
                {
                    // The most pragmatic approach is just spawning a Task for each hit.
                    // We have to see, how this works out in terms of performance.
                    _ = Task.Run(async () =>
                    {
                        await Task.Delay(attackDelay).ConfigureAwait(false);
                        if (!target.IsAtSafezone() && target.IsActive())
                        {
                            await this.ApplySkillAsync(player, skillEntry, target, targetAreaCenter, isCombo).ConfigureAwait(false);
                        }
                    });
                }
            }

            currentDelay += areaSkillSettings.DelayBetweenHits;
        }

        return extraTarget;
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
               && settings.MaximumNumberOfHitsPerAttack == 0
               && Math.Abs(settings.HitChancePerDistanceMultiplier - 1.0) <= 0.00001f;
    }

    private static IEnumerable<IAttackable> GetTargets(Player player, Point targetAreaCenter, Skill skill, byte rotation, ushort extraTargetId)
    {
        var isExtraTargetDefined = extraTargetId != UndefinedTarget;
        var extraTarget = isExtraTargetDefined ? player.GetObject(extraTargetId) as IAttackable : null;

        if (skill.SkillType == SkillType.AreaSkillExplicitTarget)
        {
            if (extraTarget?.CheckSkillTargetRestrictions(player, skill) is true
                && player.IsInRange(extraTarget.Position, skill.Range + 2)
                && !extraTarget.IsAtSafezone())
            {
                yield return extraTarget;
            }

            yield break;
        }

        foreach (var target in GetTargetsInRange(player, targetAreaCenter, skill, rotation))
        {
            yield return target;
        }
    }

    private static IEnumerable<IAttackable> GetTargetsInRange(Player player, Point targetAreaCenter, Skill skill, byte rotation)
    {
        var targetsInRange = player.CurrentMap?
                    .GetAttackablesInRange(targetAreaCenter, skill.Range)
                    .Where(a => a != player)
                    .Where(a => !a.IsAtSafezone())
            ?? [];

        if (skill.AreaSkillSettings is { UseFrustumFilter: true } areaSkillSettings)
        {
            var filter = FrustumFilters.GetOrAdd(areaSkillSettings, static s => new FrustumBasedTargetFilter(s.FrustumStartWidth, s.FrustumEndWidth, s.FrustumDistance));
            targetsInRange = targetsInRange.Where(a => filter.IsTargetWithinBounds(player, a, targetAreaCenter, rotation));
        }

        if (skill.AreaSkillSettings is { UseTargetAreaFilter: true })
        {
            targetsInRange = targetsInRange.Where(a => a.GetDistanceTo(targetAreaCenter) < skill.AreaSkillSettings.TargetAreaDiameter * 0.5f);
        }

        if (!player.GameContext.Configuration.AreaSkillHitsPlayer)
        {
            targetsInRange = targetsInRange.Where(a => a is not Player);
        }

        targetsInRange = targetsInRange.Where(target => target.CheckSkillTargetRestrictions(player, skill));

        return targetsInRange;
    }

    private async ValueTask ApplySkillAsync(Player player, SkillEntry skillEntry, IAttackable target, Point targetAreaCenter, bool isCombo)
    {
        skillEntry.ThrowNotInitializedProperty(skillEntry.Skill is null, nameof(skillEntry.Skill));

        var hitInfo = await target.AttackByAsync(player, skillEntry, isCombo).ConfigureAwait(false);
        await target.TryApplyElementalEffectsAsync(player, skillEntry).ConfigureAwait(false);
        var baseSkill = skillEntry.GetBaseSkill();

        if (player.GameContext.PlugInManager.GetStrategy<short, IAreaSkillPlugIn>(baseSkill.Number) is { } strategy)
        {
            await strategy.AfterTargetGotAttackedAsync(player, target, skillEntry, targetAreaCenter, hitInfo).ConfigureAwait(false);
        }
    }
}