// <copyright file="LostConnectionDetectionConfiguration.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.GameLogic.PlugIns;

/// <summary>
/// Configuration for the <see cref="LostConnectionDetectionPlugIn"/>.
/// </summary>
public class LostConnectionDetectionConfiguration
{
    /// <summary>
    /// Gets or sets the time after which a connection is considered lost, when its client didn't report that it's alive.
    /// </summary>
    /// <remarks>
    /// The game client reports it every 20 seconds, so the default tolerates five missing reports,
    /// e.g. while a mobile network is switched. A value of zero disables the detection.
    /// </remarks>
    [Display(ResourceType = typeof(PlugInResources), Name = nameof(PlugInResources.LostConnectionDetectionConfiguration_Timeout_Name), Description = nameof(PlugInResources.LostConnectionDetectionConfiguration_Timeout_Description))]
    public TimeSpan Timeout { get; set; } = TimeSpan.FromMinutes(2);
}
