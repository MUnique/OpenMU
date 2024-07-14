// -----------------------------------------------------------------------
// <copyright file="UpgradeItemLevelConfiguration.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>
// -----------------------------------------------------------------------

namespace MUnique.OpenMU.GameLogic.PlayerActions.ItemConsumeActions;

using System.ComponentModel.DataAnnotations;

/// <summary>
/// Configuration for a <see cref="UpgradeItemLevelJewelConsumeHandlerPlugIn{TConfig}"/>.
/// </summary>
public class UpgradeItemLevelConfiguration
{
    /// <summary>
    /// Gets or sets the success rate percentage.
    /// </summary>
    public byte SuccessRatePercentage { get; set; }

    /// <summary>
    /// Gets or sets the success rate bonus with luck percentage.
    /// </summary>
    [Display(Name = "Success Rate Luck Bonus %", Description = "The additional success rate, when the item has luck option.")]
    public byte SuccessRateBonusWithLuckPercentage { get; set; }

    /// <summary>
    /// Gets or sets the minimum item level which the item has to have before applying the jewel.
    /// </summary>
    [Display(Name = "Item Minimum Level", Description = "The minimum item level which the item has to have before applying the jewel.")]
    public byte MinimumLevel { get; set; }

    /// <summary>
    /// Gets or sets the maximum item level which the item has to have before applying the jewel.
    /// </summary>
    [Display(Name = "Item Maximum Level", Description = "The maximum item level which the item has to have before applying the jewel.")]
    public byte MaximumLevel { get; set; }

    /// <summary>
    /// Gets or sets the item level after which the item will drop to level 0 when it fails.
    /// </summary>
    [Display(Name = "Fail To 0 min. Level", Description = "The item level after which the item will drop to level 0 when it fails.")]
    public byte ResetToLevel0WhenFailMinLevel { get; set; }
}