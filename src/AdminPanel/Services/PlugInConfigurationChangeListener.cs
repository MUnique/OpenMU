// <copyright file="PlugInConfigurationChangeListener.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.AdminPanel.Services;

using MUnique.OpenMU.PlugIns;

/// <summary>
/// Implementation of <see cref="IPlugInConfigurationChangeListener"/> which notifies the <see cref="PlugInManager"/>.
/// </summary>
public class PlugInConfigurationChangeListener : IPlugInConfigurationChangeListener
{
    private readonly PlugInManager _plugInManager;

    /// <summary>
    /// Initializes a new instance of the <see cref="PlugInConfigurationChangeListener"/> class.
    /// </summary>
    /// <param name="plugInManager">The plug in manager.</param>
    public PlugInConfigurationChangeListener(PlugInManager plugInManager)
    {
        this._plugInManager = plugInManager;
    }

    /// <inheritdoc />
    public void PlugInActivated(Guid plugInId)
    {
        this._plugInManager.ActivatePlugIn(plugInId);
    }

    /// <inheritdoc />
    public void PlugInDeactivated(Guid plugInId)
    {
        this._plugInManager.DeactivatePlugIn(plugInId);
    }

    /// <inheritdoc />
    public void PlugInConfigured(Guid plugInId, PlugInConfiguration updatedConfiguration)
    {
        this._plugInManager.ConfigurePlugIn(plugInId, updatedConfiguration);
    }
}