// <copyright file="LoginTimeoutPlugInConfiguration.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.GameLogic.PlugIns;

/// <summary>
/// Configuration for the <see cref="LoginTimeoutPlugIn"/>.
/// </summary>
public class LoginTimeoutPlugInConfiguration
{
    /// <summary>
    /// Gets or sets the time in which a client has to log in after it connected.
    /// </summary>
    [Display(ResourceType = typeof(PlugInResources), Name = nameof(PlugInResources.LoginTimeoutPlugInConfiguration_Timeout_Caption))]
    public TimeSpan Timeout { get; set; } = TimeSpan.FromMinutes(5);
}
