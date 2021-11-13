// <copyright file="ISupportPlugInConfigurationChangedNotification.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.AdminPanel.Services;

using MUnique.OpenMU.PlugIns;

/// <summary>
/// Interface for an instance which notifies about changed plugin configurations.
/// </summary>
public interface ISupportPlugInConfigurationChangedNotification
{
    /// <summary>
    /// Occurs when a plug in got activated.
    /// </summary>
    event EventHandler<Guid>? PlugInActivated;

    /// <summary>
    /// Occurs when a plug in got deactivated.
    /// </summary>
    event EventHandler<Guid>? PlugInDeactivated;

    /// <summary>
    /// Occurs when a plug in got configured.
    /// </summary>
    event EventHandler<(Guid, PlugInConfiguration)>? PlugInConfigurationChanged;
}