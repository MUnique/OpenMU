// <copyright file="WanderingMerchantsConfiguration.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.GameLogic.PlugIns.WanderingMerchants;

using MUnique.OpenMU.GameLogic.PlugIns.PeriodicTasks;

/// <summary>
/// Configuration of wandering merchants.
/// </summary>
public class WanderingMerchantsConfiguration : PeriodicTaskConfiguration
{
    /// <summary>
    /// Gets or sets the minimum duration of the spawn.
    /// </summary>
    [Display(ResourceType = typeof(PlugInResources), Name = nameof(PlugInResources.WanderingMerchantsConfiguration_MinimumSpawnDuration_Name))]
    public TimeSpan MinimumSpawnDuration { get; set; } = TimeSpan.FromMinutes(60);

    /// <summary>
    /// Gets or sets the maximum duration of the spawn.
    /// </summary>
    [Display(ResourceType = typeof(PlugInResources), Name = nameof(PlugInResources.WanderingMerchantsConfiguration_MaximumSpawnDuration_Name))]
    public TimeSpan MaximumSpawnDuration { get; set; } = TimeSpan.FromMinutes(180);
}