// -----------------------------------------------------------------------
// <copyright file="UpgradeItemLevelConfiguration.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>
// -----------------------------------------------------------------------

namespace MUnique.OpenMU.GameLogic.PlayerActions.ItemConsumeActions;

using System.ComponentModel.DataAnnotations;
using MUnique.OpenMU.DataModel.Configuration.Items;

/// <summary>
/// Configuration for a <see cref="UpgradeItemLevelJewelConsumeHandlerPlugIn{TConfig}"/>.
/// </summary>
public class UpgradeItemLevelConfiguration
{
    /// <summary>
    /// Gets or sets the success rate percentage.
    /// </summary>
    [Display(ResourceType = typeof(PlugInResources), Name = nameof(PlugInResources.UpgradeItemLevelConfiguration_SuccessRatePercentage_Name), Description = nameof(PlugInResources.UpgradeItemLevelConfiguration_SuccessRatePercentage_Description))]
    public byte SuccessRatePercentage { get; set; }

    /// <summary>
    /// Gets or sets the success rate bonus with luck percentage.
    /// </summary>
    [Display(ResourceType = typeof(PlugInResources), Name = nameof(PlugInResources.UpgradeItemLevelConfiguration_SuccessRateBonusWithLuckPercentage_Name), Description = nameof(PlugInResources.UpgradeItemLevelConfiguration_SuccessRateBonusWithLuckPercentage_Description))]
    public byte SuccessRateBonusWithLuckPercentage { get; set; }

    /// <summary>
    /// Gets or sets the minimum item level which the item has to have before applying the jewel.
    /// </summary>
    [Display(ResourceType = typeof(PlugInResources), Name = nameof(PlugInResources.UpgradeItemLevelConfiguration_MinimumLevel_Name), Description = nameof(PlugInResources.UpgradeItemLevelConfiguration_MinimumLevel_Description))]
    public byte MinimumLevel { get; set; }

    /// <summary>
    /// Gets or sets the maximum item level which the item has to have before applying the jewel.
    /// </summary>
    [Display(ResourceType = typeof(PlugInResources), Name = nameof(PlugInResources.UpgradeItemLevelConfiguration_MaximumLevel_Name), Description = nameof(PlugInResources.UpgradeItemLevelConfiguration_MaximumLevel_Description))]
    public byte MaximumLevel { get; set; }

    /// <summary>
    /// Gets or sets the amount of levels which the item will be upgraded by.
    /// </summary>
    [Display(ResourceType = typeof(PlugInResources), Name = nameof(PlugInResources.UpgradeItemLevelConfiguration_LevelAmount_Name), Description = nameof(PlugInResources.UpgradeItemLevelConfiguration_LevelAmount_Description))]
    public byte LevelAmount { get; set; } = 1;

    /// <summary>
    /// Gets or sets the items which are allowed to be upgraded. If empty, all items are allowed except those in <see cref="DisallowedItems"/>.
    /// </summary>
    [Display(ResourceType = typeof(PlugInResources), Name = nameof(PlugInResources.UpgradeItemLevelConfiguration_AllowedItems_Name), Description = nameof(PlugInResources.UpgradeItemLevelConfiguration_AllowedItems_Description))]
    public ICollection<ItemDefinition> AllowedItems { get; set; } = new List<ItemDefinition>();

    /// <summary>
    /// Gets or sets the items which are not allowed to be upgraded.
    /// </summary>
    [Display(ResourceType = typeof(PlugInResources), Name = nameof(PlugInResources.UpgradeItemLevelConfiguration_DisallowedItems_Name), Description = nameof(PlugInResources.UpgradeItemLevelConfiguration_DisallowedItems_Description))]
    public ICollection<ItemDefinition> DisallowedItems { get; set; } = new List<ItemDefinition>();

    /// <summary>
    /// Gets or sets the item level after which the item will drop to level 0 when it fails.
    /// </summary>
    [Display(ResourceType = typeof(PlugInResources), Name = nameof(PlugInResources.UpgradeItemLevelConfiguration_ResetToLevel0WhenFailMinLevel_Name), Description = nameof(PlugInResources.UpgradeItemLevelConfiguration_ResetToLevel0WhenFailMinLevel_Description))]
    public byte ResetToLevel0WhenFailMinLevel { get; set; }
}