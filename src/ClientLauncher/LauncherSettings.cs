// <copyright file="LauncherSettings.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.ClientLauncher;

/// <summary>
/// Settings of the launcher.
/// </summary>
public class LauncherSettings
{
    /// <summary>
    /// Gets or sets the main executable path.
    /// </summary>
    public string? MainExePath { get; set; }

    /// <summary>
    /// Gets or sets the configured hosts.
    /// </summary>
    public List<ServerHostSettings> Hosts { get; set; } = [];
}