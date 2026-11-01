// <copyright file="AttackSpeedModel.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.GameLogic.PlugIns;

using MUnique.OpenMU.DataModel.Configuration;
using MUnique.OpenMU.GameLogic.Attributes;

/// <summary>
/// Resolves the minimum delay between two attacks from the common attack/magic speed
/// settings and the optional per-skill profiles.
/// </summary>
internal static class AttackSpeedModel
{
    /// <summary>
    /// Resolves the attack speed limit for the specified skill.
    /// </summary>
    /// <param name="config">The configuration.</param>
    /// <param name="player">The attacking player.</param>
    /// <param name="skill">The used skill, or <c>null</c> for a regular hit.</param>
    /// <returns>The resolved attack speed limit.</returns>
    internal static AttackSpeedLimit Resolve(SpeedHackDetectConfiguration config, Player player, Skill? skill)
    {
        if (FindProfile(config, skill) is { } profile)
        {
            var speed = GetSpeed(player, profile.SpeedSource);
            return new AttackSpeedLimit(
                GetInterval(profile.BaseDelayMs, profile.ScalingFactor, profile.MinimumIntervalMs, speed),
                profile.SpeedSource,
                speed);
        }

        var speedSource = GetDefaultSpeedSource(skill);
        var baseDelayMs = speedSource is AttackSpeedSource.MagicSpeed
            ? config.MagicSpeedBaseDelayMs
            : config.AttackSpeedBaseDelayMs;
        var scalingFactor = speedSource is AttackSpeedSource.MagicSpeed
            ? config.MagicSpeedScalingFactor
            : config.AttackSpeedScalingFactor;
        var minimumIntervalMs = speedSource is AttackSpeedSource.MagicSpeed
            ? config.MagicSpeedMinIntervalMs
            : config.AttackSpeedMinIntervalMs;
        var commonSpeed = GetSpeed(player, speedSource);

        return new AttackSpeedLimit(
            GetInterval(baseDelayMs, scalingFactor, minimumIntervalMs, commonSpeed),
            speedSource,
            commonSpeed);
    }

    private static AttackSpeedProfile? FindProfile(SpeedHackDetectConfiguration config, Skill? skill)
    {
        if (skill is null || config.AttackSpeedProfiles is not { Count: > 0 } profiles)
        {
            return null;
        }

        foreach (var profile in profiles)
        {
            if (profile.SkillId == skill.Number)
            {
                return profile;
            }
        }

        var baseSkill = skill.GetBaseSkill();
        if (baseSkill.Number == skill.Number)
        {
            return null;
        }

        foreach (var profile in profiles)
        {
            if (profile.SkillId == baseSkill.Number)
            {
                return profile;
            }
        }

        return null;
    }

    private static AttackSpeedSource GetDefaultSpeedSource(Skill? skill) =>
        skill?.DamageType is DamageType.Wizardry or DamageType.Curse
            ? AttackSpeedSource.MagicSpeed
            : AttackSpeedSource.AttackSpeed;

    private static double GetSpeed(Player player, AttackSpeedSource speedSource)
    {
        if (player.Attributes is not { } attributes)
        {
            return 0;
        }

        return speedSource is AttackSpeedSource.MagicSpeed
            ? attributes[Stats.MagicSpeed]
            : attributes[Stats.AttackSpeed];
    }

    private static double GetInterval(double baseDelayMs, double scalingFactor, double minimumIntervalMs, double speed)
    {
        var interval = Math.Max(minimumIntervalMs, baseDelayMs - (speed * scalingFactor));
        return Math.Max(1.0, interval);
    }

    /// <summary>
    /// The resolved attack speed limit.
    /// </summary>
    /// <param name="IntervalMs">The minimum interval between two attacks in milliseconds.</param>
    /// <param name="SpeedSource">The speed attribute which was used.</param>
    /// <param name="Speed">The value of the used speed attribute.</param>
    internal readonly record struct AttackSpeedLimit(double IntervalMs, AttackSpeedSource SpeedSource, double Speed);
}
