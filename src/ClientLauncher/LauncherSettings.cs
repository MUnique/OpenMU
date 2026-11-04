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
    /// Gets the default resolutions, which are based on the open source MuMain.
    /// </summary>
    public static ClientResolution[] DefaultResolutions =>
    [
        new(0, "640x480"),
        new(1, "800x600"),
        new(2, "1024x768"),
        new(3, "1280x1024"),
        new(4, "1600x1200"),
        new(5, "1864x1400"),
        new(6, "1600x900"),
        new(7, "1600x1280"),
        new(8, "1680x1050"),
        new(9, "1920x1080"),
        new(10, "2560x1440"),
    ];

    /// <summary>
    /// Gets or sets the main executable path.
    /// </summary>
    public string? MainExePath { get; set; }

    /// <summary>
    /// Gets or sets the configured hosts.
    /// </summary>
    public List<ServerHostSettings> Hosts { get; set; } = [];

    /// <summary>
    /// Gets or sets the resolutions.
    /// </summary>
    public List<ClientResolution> AvailableResolutions { get; set; } = [];
}