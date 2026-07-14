// <copyright file="SpeedHackDetectConfiguration.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.GameLogic.PlugIns;

using System.ComponentModel;

/// <summary>
/// Configuration for the speedhack detection anti-cheat system.
/// </summary>
public class SpeedHackDetectConfiguration
{
    /// <summary>
    /// Gets or sets a value indicating whether to auto-ban players that are cheating with speedhacks.
    /// </summary>
    [DefaultValue(true)]
    public bool AutoBan { get; set; } = true;

    /// <summary>
    /// Gets or sets a value indicating whether to disconnect players that are cheating with speedhacks.
    /// </summary>
    [DefaultValue(true)]
    public bool DisconnectOnViolation { get; set; } = true;

    /// <summary>
    /// Gets or sets the threshold of warnings a player receives before being banned/disconnected.
    /// </summary>
    [DefaultValue(3)]
    public int MaxWarnings { get; set; } = 3;

    /// <summary>
    /// Gets or sets the warning alert debounce period in seconds.
    /// Consecutive warnings within this period are ignored to avoid spamming/jitter.
    /// </summary>
    [DefaultValue(5)]
    public int AlertDebounceSeconds { get; set; } = 5;

    /// <summary>
    /// Gets or sets the warning history expiration period in hours.
    /// Warnings older than this time are cleared from the player's history.
    /// </summary>
    [DefaultValue(1)]
    public int WarningHistoryHours { get; set; } = 1;

    /// <summary>
    /// Gets or sets the walk speed check tolerance threshold in milliseconds.
    /// Represents the maximum deficits allowed compared to expectations before a violation is flagged.
    /// </summary>
    [DefaultValue(900)]
    public int WalkSpeedToleranceMs { get; set; } = 900;

    /// <summary>
    /// Gets or sets the maximum distance offset between client walk start position and server position
    /// before resynchronizing (rubberbanding) the client.
    /// </summary>
    [DefaultValue(5)]
    public int MaxAllowedWalkStartOffset { get; set; } = 5;

    /// <summary>
    /// Gets or sets the maximum tokens for the token bucket attack check.
    /// </summary>
    [DefaultValue(5.0)]
    public double MaxAttackTokens { get; set; } = 5.0;

    /// <summary>
    /// Gets or sets the base delay in milliseconds used in attack speed calculation.
    /// </summary>
    [DefaultValue(450.0)]
    public double AttackSpeedBaseDelayMs { get; set; } = 450.0;

    /// <summary>
    /// Gets or sets the scaling factor applied to attack speed attribute in delay calculation.
    /// </summary>
    [DefaultValue(1.2)]
    public double AttackSpeedScalingFactor { get; set; } = 1.2;

    /// <summary>
    /// Gets or sets the minimum delay floor in milliseconds between attacks.
    /// </summary>
    [DefaultValue(60.0)]
    public double AttackSpeedMinIntervalMs { get; set; } = 60.0;
}
