// <copyright file="PeriodicSaveProgressPlugInConfiguration.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.GameLogic.PlugIns;

/// <summary>
/// Configuration for the <see cref="PeriodicSaveProgressPlugIn"/>.
/// </summary>
public class PeriodicSaveProgressPlugInConfiguration
{
    /// <summary>
    /// Gets or sets the save interval.
    /// </summary>
    [Display(ResourceType = typeof(PlugInResources), Name = nameof(PlugInResources.PeriodicSaveProgressPlugInConfiguration_Interval_Name))]
    public TimeSpan Interval { get; set; } = TimeSpan.FromMinutes(1);
}