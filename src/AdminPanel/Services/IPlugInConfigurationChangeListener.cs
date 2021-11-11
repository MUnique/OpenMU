// <copyright file="IPlugInConfigurationChangeListener.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.AdminPanel.Services;

using MUnique.OpenMU.PlugIns;

/// <summary>
/// Interface for an instance which listenes to changes of the plugin configuration.
/// </summary>
public interface IPlugInConfigurationChangeListener
{
    /// <summary>
    /// Notifies, that the plugin with the specified id got activated.
    /// </summary>
    /// <param name="plugInId">The plug in identifier.</param>
    void PlugInActivated(Guid plugInId);

    /// <summary>
    /// Notifies, that the plugin with the specified id got deactivated.
    /// </summary>
    /// <param name="plugInId">The plug in identifier.</param>
    void PlugInDeactivated(Guid plugInId);

    /// <summary>
    /// Notifies, that the plugin with the specified id got configured.
    /// </summary>
    /// <param name="plugInId">The plug in identifier.</param>
    /// <param name="updatedConfiguration">The updated configuration.</param>
    void PlugInConfigured(Guid plugInId, PlugInConfiguration updatedConfiguration);
}