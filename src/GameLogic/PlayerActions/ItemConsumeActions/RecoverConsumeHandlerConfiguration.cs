// <copyright file="RecoverConsumeHandlerConfiguration.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.GameLogic.PlayerActions.ItemConsumeActions;

using System.ComponentModel.DataAnnotations;
using MUnique.OpenMU.DataModel.Composition;

/// <summary>
/// The configuration of a <see cref="RecoverConsumeHandlerPlugIn"/>.
/// </summary>
public class RecoverConsumeHandlerConfiguration
{
    /// <summary>
    /// Gets or sets the total recover percentage.
    /// </summary>
    [Display(ResourceType = typeof(PlugInResources), Name = nameof(PlugInResources.RecoverConsumeHandlerConfiguration_TotalRecoverPercentage_Name))]
    public double TotalRecoverPercentage { get; set; }

    /// <summary>
    /// Gets or sets the recover percentage increase by potion level.
    /// </summary>
    [Display(ResourceType = typeof(PlugInResources), Name = nameof(PlugInResources.RecoverConsumeHandlerConfiguration_RecoverPercentageIncreaseByPotionLevel_Name))]
    public double RecoverPercentageIncreaseByPotionLevel { get; set; }

    /// <summary>
    /// Gets or sets the recover delay reduction by potion level.
    /// A value between 0 and 1 (exclusive).
    /// 1 would mean that the recover works instantly since level 1.
    /// </summary>
    [Display(ResourceType = typeof(PlugInResources), Name = nameof(PlugInResources.RecoverConsumeHandlerConfiguration_RecoverDelayReductionByPotionLevel_Name), Description = nameof(PlugInResources.RecoverConsumeHandlerConfiguration_RecoverDelayReductionByPotionLevel_Description))]
    public double RecoverDelayReductionByPotionLevel { get; set; }

    /// <summary>
    /// Gets or sets the value which is additionally recovered.
    /// From this value, the character level is subtracted.
    /// </summary>
    [Display(ResourceType = typeof(PlugInResources), Name = nameof(PlugInResources.RecoverConsumeHandlerConfiguration_AdditionalRecoverMinusCharacterLevel_Name), Description = nameof(PlugInResources.RecoverConsumeHandlerConfiguration_AdditionalRecoverMinusCharacterLevel_Description))]
    public int AdditionalRecoverMinusCharacterLevel { get; set; }

    /// <summary>
    /// Gets or sets the cooldown time for the next consumption.
    /// </summary>
    [Display(ResourceType = typeof(PlugInResources), Name = nameof(PlugInResources.RecoverConsumeHandlerConfiguration_CooldownTime_Name), Description = nameof(PlugInResources.RecoverConsumeHandlerConfiguration_CooldownTime_Description))]
    public TimeSpan CooldownTime { get; set; }

    /// <summary>
    /// Gets or sets the recover steps. If none are defined, the recover happens
    /// instantly.
    /// </summary>
    [Display(ResourceType = typeof(PlugInResources), Name = nameof(PlugInResources.RecoverConsumeHandlerConfiguration_RecoverSteps_Name), Description = nameof(PlugInResources.RecoverConsumeHandlerConfiguration_RecoverSteps_Description))]
    [ScaffoldColumn(true)]
    [MemberOfAggregate]
    public ICollection<RecoverStep> RecoverSteps { get; set; } = new List<RecoverStep>();
}