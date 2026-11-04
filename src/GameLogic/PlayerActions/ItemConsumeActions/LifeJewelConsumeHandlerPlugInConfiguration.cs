// <copyright file="LifeJewelConsumeHandlerPlugInConfiguration.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.GameLogic.PlayerActions.ItemConsumeActions;

using System.ComponentModel.DataAnnotations;

/// <summary>
/// The configuration for the <see cref="LifeJewelConsumeHandlerPlugIn"/>.
/// </summary>
/// <remarks>
/// These values used to be hard-coded in the plugin. They are configurable now, so that a
/// server owner can adjust the behaviour of the <see cref="DataModel.Configuration.Items.ItemOptionTypes.Option"/>
/// upgrade without rebuilding the server.
/// </remarks>
public class LifeJewelConsumeHandlerPlugInConfiguration
{
    /// <summary>
    /// Gets or sets the success chance between 0 and 1.
    /// </summary>
    [Display(ResourceType = typeof(PlugInResources), Name = nameof(PlugInResources.LifeJewelConsumeHandlerPlugInConfiguration_SuccessChance_Name), Description = nameof(PlugInResources.LifeJewelConsumeHandlerPlugInConfiguration_SuccessChance_Description))]
    public double SuccessChance { get; set; } = 0.5;

    /// <summary>
    /// Gets or sets what happens to the item option when the upgrade fails because of the random success chance.
    /// </summary>
    [Display(ResourceType = typeof(PlugInResources), Name = nameof(PlugInResources.LifeJewelConsumeHandlerPlugInConfiguration_FailResult_Name), Description = nameof(PlugInResources.LifeJewelConsumeHandlerPlugInConfiguration_FailResult_Description))]
    public ItemUpgradeConsumeHandlerPlugIn.ItemFailResult FailResult { get; set; } = ItemUpgradeConsumeHandlerPlugIn.ItemFailResult.RemoveOption;
}
