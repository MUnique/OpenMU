// <copyright file="ILauncher.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.ClientLauncher;

/// <summary>
/// The interface of a launcher.
/// </summary>
internal interface ILauncher
{
    /// <summary>
    /// Gets or sets the host ip.
    /// </summary>
    string? HostAddress { get; set; }

    /// <summary>
    /// Gets or sets the host port.
    /// </summary>
    int HostPort { get; set; }

    /// <summary>
    /// Gets or sets the main executable path.
    /// </summary>
    string? MainExePath { get; set; }

    /// <summary>
    /// Launches the MU Online client with the specified settings.
    /// </summary>
    void LaunchClient();
}