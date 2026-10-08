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
    [DefaultValue(true)]
    [System.ComponentModel.DataAnnotations.Display(Name = nameof(MUnique.OpenMU.GameLogic.Properties.PlugInResources.SpeedHackDetectConfiguration_DisconnectOnViolation_Caption), ResourceType = typeof(MUnique.OpenMU.GameLogic.Properties.PlugInResources))]
    public bool DisconnectOnViolation { get; set; } = true;

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
    /// Gets or sets the walk speed check tolerance threshold in milliseconds.
    /// Represents the maximum deficits allowed compared to expectations before a violation is flagged.
    /// It's also the maximum walking time which is credited while no walk packets arrive, so it defines
    /// how many tiles may arrive in a burst (e.g. after a network stall) without being flagged.
    /// The value is scaled by the step delay of the player, relative to the normal step delay of 300 ms.
    /// </summary>
    [DefaultValue(2000)]
    [System.ComponentModel.DataAnnotations.Display(Name = nameof(MUnique.OpenMU.GameLogic.Properties.PlugInResources.SpeedHackDetectConfiguration_WalkSpeedToleranceMs_Caption), ResourceType = typeof(MUnique.OpenMU.GameLogic.Properties.PlugInResources))]
    public int WalkSpeedToleranceMs { get; set; } = 2000;

    /// <summary>
    /// Gets or sets the maximum distance offset between client walk start position and server position
    /// before resynchronizing (rubberbanding) the client.
    /// </summary>
    [DefaultValue(5)]
    [System.ComponentModel.DataAnnotations.Display(Name = nameof(MUnique.OpenMU.GameLogic.Properties.PlugInResources.SpeedHackDetectConfiguration_MaxAllowedWalkStartOffset_Caption), ResourceType = typeof(MUnique.OpenMU.GameLogic.Properties.PlugInResources))]
    public int MaxAllowedWalkStartOffset { get; set; } = 5;

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
}
