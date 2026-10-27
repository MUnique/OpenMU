// <copyright file="AttackSpeedProfile.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.GameLogic.PlugIns;

using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

/// <summary>
/// An optional attack speed override for a single skill.
/// </summary>
public class AttackSpeedProfile
{
    /// <summary>
    /// Gets or sets the number of the skill to which this profile applies.
    /// </summary>
    [DefaultValue((short)-1)]
    [Display(Name = nameof(Properties.PlugInResources.AttackSpeedProfile_SkillId_Caption), ResourceType = typeof(Properties.PlugInResources))]
    public short SkillId { get; set; } = -1;

    /// <summary>
    /// Gets or sets the speed attribute which scales the delay.
    /// </summary>
    [DefaultValue(AttackSpeedSource.AttackSpeed)]
    [Display(Name = nameof(Properties.PlugInResources.AttackSpeedProfile_SpeedSource_Caption), Description = nameof(Properties.PlugInResources.AttackSpeedProfile_SpeedSource_Description), ResourceType = typeof(Properties.PlugInResources))]
    public AttackSpeedSource SpeedSource { get; set; } = AttackSpeedSource.AttackSpeed;

    /// <summary>
    /// Gets or sets the base delay in milliseconds before the speed scaling is applied.
    /// </summary>
    [DefaultValue(450.0)]
    [Display(Name = nameof(Properties.PlugInResources.AttackSpeedProfile_BaseDelayMs_Caption), ResourceType = typeof(Properties.PlugInResources))]
    public double BaseDelayMs { get; set; } = 450.0;

    /// <summary>
    /// Gets or sets the scaling factor applied to the selected speed attribute.
    /// </summary>
    [DefaultValue(1.2)]
    [Display(Name = nameof(Properties.PlugInResources.AttackSpeedProfile_ScalingFactor_Caption), ResourceType = typeof(Properties.PlugInResources))]
    public double ScalingFactor { get; set; } = 1.2;

    /// <summary>
    /// Gets or sets the minimum delay in milliseconds between two attacks with this skill.
    /// </summary>
    [DefaultValue(60.0)]
    [Display(Name = nameof(Properties.PlugInResources.AttackSpeedProfile_MinimumIntervalMs_Caption), ResourceType = typeof(Properties.PlugInResources))]
    public double MinimumIntervalMs { get; set; } = 60.0;

    /// <inheritdoc />
    public override string ToString() => this.SkillId >= 0 ? $"Skill {this.SkillId} ({this.SpeedSource})" : base.ToString()!;
}
