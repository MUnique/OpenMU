// <copyright file="AdminPanelSettings.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.Web.AdminPanel;

/// <summary>
/// Settings for the <see cref="AdminPanel"/>.
/// </summary>
public class AdminPanelSettings
{
    /// <summary>
    /// Initializes a new instance of the <see cref="AdminPanelSettings"/> class.
    /// </summary>
    /// <param name="port">The port to which the admin panel should be bound.</param>
    public AdminPanelSettings(ushort port)
    {
        this.Port = port;
    }

    /// <summary>
    /// Gets the port to which the admin panel should be bound.
    /// </summary>
    public ushort Port { get; }
}