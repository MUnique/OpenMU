// -----------------------------------------------------------------------
// <copyright file="ItemUpgradeConfiguration.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>
// -----------------------------------------------------------------------

namespace MUnique.OpenMU.GameLogic.PlayerActions.ItemConsumeActions;

using System.ComponentModel.DataAnnotations;
using MUnique.OpenMU.DataModel.Configuration.Items;

/// <summary>
/// The configuration of an <see cref="ItemUpgradeConsumeHandlerPlugIn"/>.
/// </summary>
public class ItemUpgradeConfiguration
{
    /// <summary>
    /// Initializes a new instance of the <see cref="ItemUpgradeConfiguration"/> class.
    /// </summary>
    public ItemUpgradeConfiguration()
    {
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="ItemUpgradeConfiguration"/> class.
    /// </summary>
    /// <param name="optionType">Type of the option.</param>
    /// <param name="addsOption">if set to <c>true</c> [adds option].</param>
    /// <param name="increasesOption">if set to <c>true</c> [increases option].</param>
    /// <param name="successChance">The success chance.</param>
    /// <param name="failResult">The fail result.</param>
    public ItemUpgradeConfiguration(ItemOptionType optionType, bool addsOption, bool increasesOption, double successChance, ItemUpgradeConsumeHandlerPlugIn.ItemFailResult failResult)
    {
        this.OptionType = optionType;
        this.AddsOption = addsOption;
        this.IncreasesOption = increasesOption;
        this.SuccessChance = successChance;
        this.FailResult = failResult;
    }

    private ItemUpgradeConfiguration(ItemUpgradeConfiguration other)
    {
        this.OptionType = other.OptionType;
        this.AddsOption = other.AddsOption;
        this.IncreasesOption = other.IncreasesOption;
        this.SuccessChance = other.SuccessChance;
        this.FailResult = other.FailResult;
        this.BoostOptionType = other.BoostOptionType;
        this.SuccessChanceBoost = other.SuccessChanceBoost;
    }

    /// <summary>
    /// Gets or sets the type of the option.
    /// </summary>
    [Display(ResourceType = typeof(PlugInResources), Name = nameof(PlugInResources.ItemUpgradeConfiguration_OptionType_Name), Description = nameof(PlugInResources.ItemUpgradeConfiguration_OptionType_Description))]
    public ItemOptionType? OptionType { get; set; }

    /// <summary>
    /// Gets or sets a value indicating whether the handler adds option, if the item does not already have it.
    /// </summary>
    [Display(ResourceType = typeof(PlugInResources), Name = nameof(PlugInResources.ItemUpgradeConfiguration_AddsOption_Name), Description = nameof(PlugInResources.ItemUpgradeConfiguration_AddsOption_Description))]
    public bool AddsOption { get; set; }

    /// <summary>
    /// Gets or sets a value indicating whether the handler increases the option.
    /// </summary>
    [Display(ResourceType = typeof(PlugInResources), Name = nameof(PlugInResources.ItemUpgradeConfiguration_IncreasesOption_Name), Description = nameof(PlugInResources.ItemUpgradeConfiguration_IncreasesOption_Description))]
    public bool IncreasesOption { get; set; }

    /// <summary>
    /// Gets or sets the success chance between 0 and 1.
    /// </summary>
    [Display(ResourceType = typeof(PlugInResources), Name = nameof(PlugInResources.ItemUpgradeConfiguration_SuccessChance_Name), Description = nameof(PlugInResources.ItemUpgradeConfiguration_SuccessChance_Description))]
    public double SuccessChance { get; set; }

    /// <summary>
    /// Gets or sets what should happen with the item if the upgrade fails because of no success.
    /// </summary>
    [Display(ResourceType = typeof(PlugInResources), Name = nameof(PlugInResources.ItemUpgradeConfiguration_FailResult_Name), Description = nameof(PlugInResources.ItemUpgradeConfiguration_FailResult_Description))]
    public ItemUpgradeConsumeHandlerPlugIn.ItemFailResult FailResult { get; set; }

    /// <summary>
    /// Gets or sets the option type which can boost the success rate.
    /// </summary>
    /// <remarks>
    /// e.g. luck option which adds 25 per cent.
    /// </remarks>
    [Display(ResourceType = typeof(PlugInResources), Name = nameof(PlugInResources.ItemUpgradeConfiguration_BoostOptionType_Name), Description = nameof(PlugInResources.ItemUpgradeConfiguration_BoostOptionType_Description))]
    public ItemOptionType? BoostOptionType { get; set; }

    /// <summary>
    /// Gets or sets the success chance boost if the target item has the option of type specified in <see cref="BoostOptionType"/>.
    /// </summary>
    [Display(ResourceType = typeof(PlugInResources), Name = nameof(PlugInResources.ItemUpgradeConfiguration_SuccessChanceBoost_Name), Description = nameof(PlugInResources.ItemUpgradeConfiguration_SuccessChanceBoost_Description))]
    public double SuccessChanceBoost { get; set; }

    /// <summary>
    /// Creates a copy of this configuration.
    /// </summary>
    /// <returns>The copy.</returns>
    public ItemUpgradeConfiguration Clone() => new(this);
}
