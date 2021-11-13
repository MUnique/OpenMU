// <copyright file="PlugInConfigurationChangedEventArgs.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.PlugIns;

/// <summary>
/// <see cref="EventArgs"/> for the <see cref="PlugInManager.PlugInConfigurationChanged"/> event.
/// </summary>
public class PlugInConfigurationChangedEventArgs : PlugInEventArgs
{
    /// <summary>
    /// Initializes a new instance of the <see cref="PlugInConfigurationChangedEventArgs"/> class.
    /// </summary>
    /// <param name="plugInType">Type of the plug in.</param>
    /// <param name="configuration">The changed configuration.</param>
    public PlugInConfigurationChangedEventArgs(Type plugInType, PlugInConfiguration configuration)
        : base(plugInType)
    {
        this.Configuration = configuration;
    }

    /// <summary>
    /// Gets the changed configuration.
    /// </summary>
    public PlugInConfiguration Configuration { get; }
}