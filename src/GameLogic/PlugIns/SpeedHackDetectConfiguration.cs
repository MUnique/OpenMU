// <copyright file="SpeedHackDetectConfiguration.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.GameLogic.PlugIns;

using System.ComponentModel;
using MUnique.OpenMU.DataModel.Composition;

/// <summary>
/// Configuration for the speedhack detection anti-cheat system.
/// </summary>
public class SpeedHackDetectConfiguration
{
    /// <summary>
    /// Gets or sets a value indicating whether to auto-ban players that are cheating with speedhacks.
    /// </summary>
    /// <remarks>
    /// The checks are heuristics based on the time when the server processes the packets, so they
    /// can be affected by network jitter. Because a ban is a heavy action, it's disabled by default.
    /// </remarks>
    [DefaultValue(false)]
    [System.ComponentModel.DataAnnotations.Display(Name = nameof(MUnique.OpenMU.GameLogic.Properties.PlugInResources.SpeedHackDetectConfiguration_AutoBan_Caption), ResourceType = typeof(MUnique.OpenMU.GameLogic.Properties.PlugInResources))]
    public bool AutoBan { get; set; }

    /// <summary>
    /// Gets or sets a value indicating whether to disconnect players that are cheating with speedhacks.
    /// </summary>
    /// <remarks>
    /// The checks are heuristics based on the time when the server processes the packets, so they
    /// can be affected by network jitter. Disconnecting is therefore disabled by default; enable it
    /// after the detection has been observed on the server.
    /// </remarks>
    [DefaultValue(false)]
    [System.ComponentModel.DataAnnotations.Display(Name = nameof(MUnique.OpenMU.GameLogic.Properties.PlugInResources.SpeedHackDetectConfiguration_DisconnectOnViolation_Caption), ResourceType = typeof(MUnique.OpenMU.GameLogic.Properties.PlugInResources))]
    public bool DisconnectOnViolation { get; set; }

    /// <summary>
    /// Gets or sets a value indicating whether violations are only detected and logged,
    /// without rejecting walks, warning, banning or disconnecting.
    /// Useful to trial the detection on a live server before enforcing it.
    /// </summary>
    [DefaultValue(false)]
    [System.ComponentModel.DataAnnotations.Display(Name = nameof(MUnique.OpenMU.GameLogic.Properties.PlugInResources.SpeedHackDetectConfiguration_ObserveOnly_Caption), ResourceType = typeof(MUnique.OpenMU.GameLogic.Properties.PlugInResources))]
    public bool ObserveOnly { get; set; }

    /// <summary>
    /// Gets or sets the threshold of warnings a player receives before being banned/disconnected.
    /// </summary>
    [DefaultValue(3)]
    [System.ComponentModel.DataAnnotations.Display(Name = nameof(MUnique.OpenMU.GameLogic.Properties.PlugInResources.SpeedHackDetectConfiguration_MaxWarnings_Caption), ResourceType = typeof(MUnique.OpenMU.GameLogic.Properties.PlugInResources))]
    public int MaxWarnings { get; set; } = 3;

    /// <summary>
    /// Gets or sets the warning alert debounce period in seconds.
    /// Consecutive warnings within this period are ignored to avoid spamming/jitter.
    /// </summary>
    [DefaultValue(5)]
    [System.ComponentModel.DataAnnotations.Display(Name = nameof(MUnique.OpenMU.GameLogic.Properties.PlugInResources.SpeedHackDetectConfiguration_AlertDebounceSeconds_Caption), ResourceType = typeof(MUnique.OpenMU.GameLogic.Properties.PlugInResources))]
    public int AlertDebounceSeconds { get; set; } = 5;

    /// <summary>
    /// Gets or sets the warning history expiration period in hours.
    /// Warnings older than this time are cleared from the player's history.
    /// </summary>
    [DefaultValue(1)]
    [System.ComponentModel.DataAnnotations.Display(Name = nameof(MUnique.OpenMU.GameLogic.Properties.PlugInResources.SpeedHackDetectConfiguration_WarningHistoryHours_Caption), ResourceType = typeof(MUnique.OpenMU.GameLogic.Properties.PlugInResources))]
    public int WarningHistoryHours { get; set; } = 1;

    /// <summary>
    /// Gets or sets a value indicating whether the walk speed check may reject walks and
    /// contribute to warnings and punishments.
    /// </summary>
    /// <remarks>
    /// Disabled by default: the check remains a heuristic and can reject a walk, which the client is
    /// otherwise authoritative for. It measures the walk steps which the server actually executed over a
    /// window of <see cref="WalkSpeedWindowSeconds"/> seconds, so the timing of single walk packets does not
    /// affect it. The check still observes and logs suspicious walk pace when this is disabled.
    /// </remarks>
    [DefaultValue(false)]
    [System.ComponentModel.DataAnnotations.Display(Name = nameof(MUnique.OpenMU.GameLogic.Properties.PlugInResources.SpeedHackDetectConfiguration_EnforceWalkSpeed_Caption), ResourceType = typeof(MUnique.OpenMU.GameLogic.Properties.PlugInResources))]
    public bool EnforceWalkSpeed { get; set; }

    /// <summary>
    /// Gets or sets the length of the sliding window in seconds, in which the distance of the walk steps
    /// executed by the server is compared to the time which passed. A long window makes the check robust
    /// against short-term bursts and timer granularity, but needs more time to detect a speed hack.
    /// </summary>
    /// <remarks>
    /// Existing databases predate this setting and deserialize it as 0; values
    /// below or equal to 0 fall back to the default at the check.
    /// </remarks>
    [DefaultValue(WalkSpeedValidator.DefaultWindowSeconds)]
    [System.ComponentModel.DataAnnotations.Display(Name = nameof(MUnique.OpenMU.GameLogic.Properties.PlugInResources.SpeedHackDetectConfiguration_WalkSpeedWindowSeconds_Caption), ResourceType = typeof(MUnique.OpenMU.GameLogic.Properties.PlugInResources))]
    public int WalkSpeedWindowSeconds { get; set; } = WalkSpeedValidator.DefaultWindowSeconds;

    /// <summary>
    /// Gets or sets the walk speed check tolerance in milliseconds.
    /// It's added to the length of the window, and defines how much walking time the server-executed walk
    /// steps may cost beyond the time which passed within the window before a violation is flagged.
    /// The value is scaled by the step delay of the player, relative to the normal step delay of 300 ms.
    /// </summary>
    [DefaultValue(2000)]
    [System.ComponentModel.DataAnnotations.Display(Name = nameof(MUnique.OpenMU.GameLogic.Properties.PlugInResources.SpeedHackDetectConfiguration_WalkSpeedToleranceMs_Caption), ResourceType = typeof(MUnique.OpenMU.GameLogic.Properties.PlugInResources))]
    public int WalkSpeedToleranceMs { get; set; } = 2000;

    /// <summary>
    /// Gets or sets the maximum distance between a client-reported walk start position and the server
    /// position when the client isn't already walking the same path. PlayerMovement rejects and
    /// rubberbands larger offsets, so this setting is the server-side position bound; the check also
    /// records a violation. While a walk is running, the time since the last server step bounds the
    /// accepted offset instead.
    /// </summary>
    [DefaultValue(5)]
    [System.ComponentModel.DataAnnotations.Display(Name = nameof(MUnique.OpenMU.GameLogic.Properties.PlugInResources.SpeedHackDetectConfiguration_MaxAllowedWalkStartOffset_Caption), ResourceType = typeof(MUnique.OpenMU.GameLogic.Properties.PlugInResources))]
    public int MaxAllowedWalkStartOffset { get; set; } = 5;

    /// <summary>
    /// Gets or sets the extra tiles added to an explicit-hit target area, so a target which moved
    /// between the cast and its hit packet is still accepted.
    /// </summary>
    [DefaultValue(2)]
    [System.ComponentModel.DataAnnotations.Display(Name = nameof(MUnique.OpenMU.GameLogic.Properties.PlugInResources.SpeedHackDetectConfiguration_SkillHitAreaRangeSlack_Caption), ResourceType = typeof(MUnique.OpenMU.GameLogic.Properties.PlugInResources))]
    public int SkillHitAreaRangeSlack { get; set; } = 2;

    /// <summary>
    /// Gets or sets the time window in milliseconds in which the same hit counter is treated as a replay.
    /// Zero disables the replay check.
    /// </summary>
    [DefaultValue(1000)]
    [System.ComponentModel.DataAnnotations.Display(Name = nameof(MUnique.OpenMU.GameLogic.Properties.PlugInResources.SpeedHackDetectConfiguration_SkillHitCounterReplayWindowMs_Caption), ResourceType = typeof(MUnique.OpenMU.GameLogic.Properties.PlugInResources))]
    public int SkillHitCounterReplayWindowMs { get; set; } = 1000;

    /// <summary>
    /// Gets or sets the maximum age in seconds of an area skill attack for its explicit hit packets.
    /// Zero disables the age check.
    /// </summary>
    [DefaultValue(10)]
    [System.ComponentModel.DataAnnotations.Display(Name = nameof(MUnique.OpenMU.GameLogic.Properties.PlugInResources.SpeedHackDetectConfiguration_SkillHitMaxAreaToHitDelaySeconds_Caption), ResourceType = typeof(MUnique.OpenMU.GameLogic.Properties.PlugInResources))]
    public int SkillHitMaxAreaToHitDelaySeconds { get; set; } = 10;

    /// <summary>
    /// Gets or sets the violation score at which a walk violation escalates from
    /// a soft response to a hard rejection. Each violation adds to the
    /// score proportionally to its overshoot, and the score halves every
    /// <see cref="ViolationScoreHalfLifeMinutes"/> minutes. Isolated blips therefore
    /// never escalate on their own, while sustained cheating does.
    /// </summary>
    /// <remarks>
    /// Existing databases predate this setting and deserialize it as 0; values
    /// below or equal to 0 fall back to the default at the check.
    /// </remarks>
    [DefaultValue(ViolationPolicy.DefaultRejectThreshold)]
    [System.ComponentModel.DataAnnotations.Display(Name = nameof(MUnique.OpenMU.GameLogic.Properties.PlugInResources.SpeedHackDetectConfiguration_ViolationScoreRejectThreshold_Caption), ResourceType = typeof(MUnique.OpenMU.GameLogic.Properties.PlugInResources))]
    public double ViolationScoreRejectThreshold { get; set; } = ViolationPolicy.DefaultRejectThreshold;

    /// <summary>
    /// Gets or sets the half-life in minutes for the decay of the violation score.
    /// </summary>
    /// <remarks>
    /// Existing databases predate this setting and deserialize it as 0; values
    /// below or equal to 0 fall back to the default at the check.
    /// </remarks>
    [DefaultValue(ViolationPolicy.DefaultHalfLifeMinutes)]
    [System.ComponentModel.DataAnnotations.Display(Name = nameof(MUnique.OpenMU.GameLogic.Properties.PlugInResources.SpeedHackDetectConfiguration_ViolationScoreHalfLifeMinutes_Caption), ResourceType = typeof(MUnique.OpenMU.GameLogic.Properties.PlugInResources))]
    public double ViolationScoreHalfLifeMinutes { get; set; } = ViolationPolicy.DefaultHalfLifeMinutes;

    /// <summary>
    /// Gets or sets the maximum tokens for the token bucket attack check.
    /// </summary>
    [DefaultValue(5.0)]
    [System.ComponentModel.DataAnnotations.Display(Name = nameof(MUnique.OpenMU.GameLogic.Properties.PlugInResources.SpeedHackDetectConfiguration_MaxAttackTokens_Caption), ResourceType = typeof(MUnique.OpenMU.GameLogic.Properties.PlugInResources))]
    public double MaxAttackTokens { get; set; } = 5.0;

    /// <summary>
    /// Gets or sets the base delay in milliseconds used in attack speed calculation.
    /// </summary>
    [DefaultValue(450.0)]
    [System.ComponentModel.DataAnnotations.Display(Name = nameof(MUnique.OpenMU.GameLogic.Properties.PlugInResources.SpeedHackDetectConfiguration_AttackSpeedBaseDelayMs_Caption), ResourceType = typeof(MUnique.OpenMU.GameLogic.Properties.PlugInResources))]
    public double AttackSpeedBaseDelayMs { get; set; } = 450.0;

    /// <summary>
    /// Gets or sets the scaling factor applied to attack speed attribute in delay calculation.
    /// </summary>
    [DefaultValue(1.2)]
    [System.ComponentModel.DataAnnotations.Display(Name = nameof(MUnique.OpenMU.GameLogic.Properties.PlugInResources.SpeedHackDetectConfiguration_AttackSpeedScalingFactor_Caption), ResourceType = typeof(MUnique.OpenMU.GameLogic.Properties.PlugInResources))]
    public double AttackSpeedScalingFactor { get; set; } = 1.2;

    /// <summary>
    /// Gets or sets the minimum delay floor in milliseconds between attacks.
    /// </summary>
    [DefaultValue(60.0)]
    [System.ComponentModel.DataAnnotations.Display(Name = nameof(MUnique.OpenMU.GameLogic.Properties.PlugInResources.SpeedHackDetectConfiguration_AttackSpeedMinIntervalMs_Caption), ResourceType = typeof(MUnique.OpenMU.GameLogic.Properties.PlugInResources))]
    public double AttackSpeedMinIntervalMs { get; set; } = 60.0;

    /// <summary>
    /// Gets or sets the base delay in milliseconds used in magic speed calculation.
    /// </summary>
    [DefaultValue(450.0)]
    [System.ComponentModel.DataAnnotations.Display(Name = nameof(MUnique.OpenMU.GameLogic.Properties.PlugInResources.SpeedHackDetectConfiguration_MagicSpeedBaseDelayMs_Caption), ResourceType = typeof(MUnique.OpenMU.GameLogic.Properties.PlugInResources))]
    public double MagicSpeedBaseDelayMs { get; set; } = 450.0;

    /// <summary>
    /// Gets or sets the scaling factor applied to the magic speed attribute in delay calculation.
    /// </summary>
    [DefaultValue(1.2)]
    [System.ComponentModel.DataAnnotations.Display(Name = nameof(MUnique.OpenMU.GameLogic.Properties.PlugInResources.SpeedHackDetectConfiguration_MagicSpeedScalingFactor_Caption), ResourceType = typeof(MUnique.OpenMU.GameLogic.Properties.PlugInResources))]
    public double MagicSpeedScalingFactor { get; set; } = 1.2;

    /// <summary>
    /// Gets or sets the minimum delay floor in milliseconds between magic attacks.
    /// </summary>
    [DefaultValue(60.0)]
    [System.ComponentModel.DataAnnotations.Display(Name = nameof(MUnique.OpenMU.GameLogic.Properties.PlugInResources.SpeedHackDetectConfiguration_MagicSpeedMinIntervalMs_Caption), ResourceType = typeof(MUnique.OpenMU.GameLogic.Properties.PlugInResources))]
    public double MagicSpeedMinIntervalMs { get; set; } = 60.0;

    /// <summary>
    /// Gets or sets the allowed difference in milliseconds between the time claimed by the
    /// client ping tick count and the time which actually passed on the server.
    /// </summary>
    [DefaultValue(3000)]
    [System.ComponentModel.DataAnnotations.Display(Name = nameof(MUnique.OpenMU.GameLogic.Properties.PlugInResources.SpeedHackDetectConfiguration_PingClockToleranceMs_Caption), ResourceType = typeof(MUnique.OpenMU.GameLogic.Properties.PlugInResources))]
    public int PingClockToleranceMs { get; set; } = 3000;

    /// <summary>
    /// Gets or sets the allowed difference between the attack speed reported by the client
    /// ping packet and the attack speed calculated by the server.
    /// </summary>
    [DefaultValue(20)]
    [System.ComponentModel.DataAnnotations.Display(Name = nameof(MUnique.OpenMU.GameLogic.Properties.PlugInResources.SpeedHackDetectConfiguration_PingAttackSpeedTolerance_Caption), ResourceType = typeof(MUnique.OpenMU.GameLogic.Properties.PlugInResources))]
    public int PingAttackSpeedTolerance { get; set; } = 20;

    /// <summary>
    /// Gets or sets optional attack speed overrides for individual skills.
    /// </summary>
    [System.ComponentModel.DataAnnotations.Display(Name = nameof(MUnique.OpenMU.GameLogic.Properties.PlugInResources.SpeedHackDetectConfiguration_AttackSpeedProfiles_Caption), Description = nameof(MUnique.OpenMU.GameLogic.Properties.PlugInResources.SpeedHackDetectConfiguration_AttackSpeedProfiles_Description), ResourceType = typeof(MUnique.OpenMU.GameLogic.Properties.PlugInResources))]
    [MemberOfAggregate]
    [System.ComponentModel.DataAnnotations.ScaffoldColumn(true)]
    public ICollection<AttackSpeedProfile> AttackSpeedProfiles { get; set; } = new List<AttackSpeedProfile>();
}
