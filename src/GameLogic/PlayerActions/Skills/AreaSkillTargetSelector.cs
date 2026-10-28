// <copyright file="AreaSkillTargetSelector.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.GameLogic.PlayerActions.Skills;

using System.Collections.Concurrent;
using MUnique.OpenMU.DataModel.Configuration;
using MUnique.OpenMU.GameLogic.NPC;
using MUnique.OpenMU.GameLogic.Views;
using MUnique.OpenMU.Pathfinding;

/// <summary>
/// Selects the targets of area skills at cast time.
/// </summary>
public static class AreaSkillTargetSelector
{
    /// <summary>
    /// The target identifier which marks "no explicit target".
    /// </summary>
    internal const int UndefinedTarget = 0xFFFF;

    /// <summary>
    /// Scale from degrees to rotation byte. It matches the lookup table of the
    /// <see cref="FrustumBasedTargetFilter"/>, which divides by <c>byte.MaxValue</c>.
    /// </summary>
    internal const float RotationByteScale = 255.0f;

    private static readonly ConcurrentDictionary<AreaSkillSettings, FrustumBasedTargetFilter> FrustumFilters = new();

    /// <summary>
    /// Determines the targets of an area skill attack.
    /// </summary>
    /// <param name="player">The attacking player.</param>
    /// <param name="targetAreaCenter">The coordinates of the center of the target area.</param>
    /// <param name="skill">The skill.</param>
    /// <param name="rotation">The rotation in which the player is looking.</param>
    /// <param name="extraTargetId">The extra target identifier.</param>
    /// <returns>The targets.</returns>
    public static IEnumerable<IAttackable> GetTargets(Player player, Point targetAreaCenter, Skill skill, byte rotation, ushort extraTargetId)
    {
        var isExtraTargetDefined = extraTargetId != UndefinedTarget;
        var extraTarget = isExtraTargetDefined ? player.GetObject(extraTargetId) as IAttackable : null;

        player.Logger.LogDebug(
            "GetTargets: skill={Skill}, extraTargetId={ExtraTargetId}, extraTarget={ExtraTarget}", skill.Name, extraTargetId, extraTarget?.ToString() ?? "null");

        if (skill.SkillType == SkillType.AreaSkillExplicitTarget)
        {
            if (extraTarget?.CheckSkillTargetRestrictions(player, skill) is true
                && player.IsInRange(extraTarget.Position, skill.Range)
                && !extraTarget.IsAtSafezone())
            {
                yield return extraTarget;
            }

            yield break;
        }

        // Include the explicit extra target if it's valid and in range.
        // Both positions are server-side and evaluated instantaneously, so no
        // slack is added here, unlike for the client-aimed point.
        if (extraTarget is not null
            && extraTarget.CheckSkillTargetRestrictions(player, skill)
            && player.IsInRange(extraTarget.Position, skill.Range)
            && !extraTarget.IsAtSafezone())
        {
            yield return extraTarget;
        }

        foreach (var target in GetTargetsInRange(player, targetAreaCenter, skill, rotation))
        {
            // Skip the extra target if we already yielded it
            if (target.Id == extraTargetId)
            {
                continue;
            }

            yield return target;
        }
    }

    /// <summary>
    /// Corrects the attack rotation with the server-side direction to a locked target.
    /// The client rotation is coarse and can be desynced.
    /// </summary>
    /// <param name="player">The attacking player.</param>
    /// <param name="skill">The skill.</param>
    /// <param name="extraTargetId">The extra target identifier.</param>
    /// <param name="rotation">The client-provided rotation.</param>
    /// <returns>The rotation to use for target selection.</returns>
    public static byte CorrectRotationForLockedTarget(Player player, Skill skill, ushort extraTargetId, byte rotation)
    {
        if (skill.AreaSkillSettings is { UseFrustumFilter: true }
            && extraTargetId != UndefinedTarget
            && player.GetObject(extraTargetId) as IAttackable is { } lockedTarget
            && player.IsInRange(lockedTarget.Position, skill.Range))
        {
            var angleToTarget = (float)player.Position.GetAngleDegreeTo(lockedTarget.Position);
            return ConvertAngleToRotation(angleToTarget);
        }

        return rotation;
    }

    /// <summary>
    /// Converts a direction angle into the rotation byte of the skill packets.
    /// The value is rounded, not truncated: truncation drops up to one full
    /// step (about 1.4 degrees), which is enough to push a diagonally adjacent
    /// target out of a side projectile lane whose overlap ends right at the
    /// frustum center.
    /// </summary>
    /// <param name="angle">The direction angle in degrees.</param>
    /// <returns>The rotation byte.</returns>
    internal static byte ConvertAngleToRotation(float angle)
    {
        return (byte)Math.Round(angle / 360.0f * RotationByteScale, MidpointRounding.AwayFromZero);
    }

    /// <summary>
    /// Gets the cached frustum filter for the specified settings.
    /// </summary>
    /// <param name="settings">The area skill settings.</param>
    /// <returns>The frustum filter.</returns>
    internal static FrustumBasedTargetFilter GetLaneFilter(AreaSkillSettings settings)
    {
        return FrustumFilters.GetOrAdd(settings, static s => new FrustumBasedTargetFilter(s.FrustumStartWidth, s.FrustumEndWidth, s.FrustumDistance, s.ProjectileCount > 0 ? s.ProjectileCount : 1));
    }

    /// <summary>
    /// Determines the center and radius of the area query for the specified skill.
    /// </summary>
    /// <param name="casterPosition">The position of the caster when the attack started.</param>
    /// <param name="targetAreaCenter">The client-aimed point.</param>
    /// <param name="skill">The skill.</param>
    /// <returns>The query center and radius.</returns>
    /// <remarks>
    /// Only skills which shape their area around the aimed point (target area
    /// filter, effect range) are queried around it. Frustum skills and all other
    /// skills hit around the caster: the client-aimed point of a melee-range
    /// skill must not extend its reach beyond the skill range, and the client
    /// position can lag the server-side one. Verified in-game.
    /// </remarks>
    internal static (Point Center, int Range) GetQueryCenterAndRange(Point casterPosition, Point targetAreaCenter, Skill skill)
    {
        var range = skill.AreaSkillSettings?.EffectRange > 0 ? skill.AreaSkillSettings.EffectRange : skill.Range;
        if (skill.AreaSkillSettings is { UseFrustumFilter: true } frustumSettings)
        {
            return (casterPosition, Math.Max(range, (int)Math.Ceiling(frustumSettings.FrustumDistance)));
        }

        if (skill.AreaSkillSettings is { UseTargetAreaFilter: true } or { EffectRange: > 0 })
        {
            return (targetAreaCenter, range);
        }

        return (casterPosition, range);
    }

    /// <summary>
    /// Gets the radius in which a target-area skill can hit targets around its aimed center.
    /// </summary>
    /// <param name="skill">The skill.</param>
    /// <returns>The target-area radius, or <c>null</c> if the skill doesn't use a target-area filter.</returns>
    internal static float? GetTargetAreaRadius(Skill skill)
    {
        return skill.AreaSkillSettings is { UseTargetAreaFilter: true } settings
            ? settings.TargetAreaDiameter * 0.5f
            : null;
    }

    /// <summary>
    /// Applies the filters which every implicit area target must pass: not the
    /// attacker, no safezone, active, no summoned monsters, skill restrictions,
    /// and players only when the configuration allows hitting them.
    /// </summary>
    /// <param name="candidates">The candidates.</param>
    /// <param name="player">The attacking player.</param>
    /// <param name="skill">The skill.</param>
    /// <returns>The valid targets.</returns>
    internal static IEnumerable<IAttackable> WhereAttackableByDefault(IEnumerable<IAttackable> candidates, Player player, Skill skill)
    {
        return candidates
            .Where(a => a != player)
            .Where(a => !a.IsAtSafezone())
            .Where(a => a.IsActive())
            .Where(a => a is not Monster { SummonedBy: not null })
            .Where(a => a.CheckSkillTargetRestrictions(player, skill))
            .Where(a => player.GameContext.Configuration.AreaSkillHitsPlayer || a is not Player);
    }

    private static IEnumerable<IAttackable> GetTargetsInRange(Player player, Point targetAreaCenter, Skill skill, byte rotation)
    {
        var (queryCenter, queryRange) = GetQueryCenterAndRange(player.Position, targetAreaCenter, skill);

        IEnumerable<IAttackable> targetsInRange = player.CurrentMap?
                    .GetAttackablesInRange(queryCenter, queryRange)
            ?? [];
        targetsInRange = WhereAttackableByDefault(targetsInRange, player, skill);

        if (skill.AreaSkillSettings is { UseFrustumFilter: true } areaSkillSettings)
        {
            var filter = GetLaneFilter(areaSkillSettings);
            targetsInRange = targetsInRange.Where(a => filter.IsTargetWithinBounds(player, a, rotation));
        }

        if (GetTargetAreaRadius(skill) is { } targetAreaRadius)
        {
            targetsInRange = targetsInRange.Where(a => a.GetDistanceTo(targetAreaCenter) < targetAreaRadius);
        }

        return targetsInRange;
    }
}
