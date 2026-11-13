// <copyright file="WaitingItemsDeliveryPlugInConfiguration.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.GameLogic.WaitingItems;

/// <summary>
/// Configuration for the <see cref="WaitingItemsDeliveryPlugIn"/>.
/// </summary>
public class WaitingItemsDeliveryPlugInConfiguration
{
    /// <summary>
    /// Gets or sets the interval in which the waiting items are put into the inventories, as far as they fit there.
    /// </summary>
    [Display(ResourceType = typeof(PlugInResources), Name = nameof(PlugInResources.WaitingItemsDeliveryPlugInConfiguration_CheckInterval_Name), Description = nameof(PlugInResources.WaitingItemsDeliveryPlugInConfiguration_CheckInterval_Description))]
    public TimeSpan CheckInterval { get; set; } = TimeSpan.FromSeconds(30);

    /// <summary>
    /// Gets or sets the number of waiting items which are named when the player is told about them,
    /// when entering the game or opening the vault.
    /// </summary>
    [Display(ResourceType = typeof(PlugInResources), Name = nameof(PlugInResources.WaitingItemsDeliveryPlugInConfiguration_ItemsNamed_Name), Description = nameof(PlugInResources.WaitingItemsDeliveryPlugInConfiguration_ItemsNamed_Description))]
    public int ItemsNamed { get; set; } = 3;
}
