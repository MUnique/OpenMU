// <copyright file="PlugInConfigurationChangeController.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.Discord.Host;

using global::Dapr;
using Microsoft.AspNetCore.Mvc;
using MUnique.OpenMU.Dapr.Common;
using MUnique.OpenMU.Interfaces;
using MUnique.OpenMU.PlugIns;

/// <summary>
/// Applies the plugin configurations which are changed on the admin panel to the plugins of this host,
/// e.g. the ones which receive the game events.
/// </summary>
[ApiController]
[Route("")]
public class PlugInConfigurationChangeController : ControllerBase
{
    private readonly PlugInManager _plugInManager;

    /// <summary>
    /// Initializes a new instance of the <see cref="PlugInConfigurationChangeController"/> class.
    /// </summary>
    /// <param name="plugInManager">The plugin manager.</param>
    public PlugInConfigurationChangeController(PlugInManager plugInManager)
    {
        this._plugInManager = plugInManager;
    }

    /// <summary>
    /// Called when a configuration got changed on the admin panel.
    /// </summary>
    /// <param name="arguments">The message arguments.</param>
    [HttpPost(nameof(IConfigurationChangePublisher.ConfigurationChangedAsync))]
    [Topic("pubsub", nameof(IConfigurationChangePublisher.ConfigurationChangedAsync))]
    public void ConfigurationChanged([FromBody] ConfigurationChangeArguments arguments)
    {
        this._plugInManager.ApplyChangedConfiguration(arguments.Type, arguments.Id, arguments.Configuration);
    }

    /// <summary>
    /// Called when a configuration got removed on the admin panel.
    /// </summary>
    /// <param name="arguments">The message arguments.</param>
    [HttpPost(nameof(IConfigurationChangePublisher.ConfigurationRemovedAsync))]
    [Topic("pubsub", nameof(IConfigurationChangePublisher.ConfigurationRemovedAsync))]
    public void ConfigurationRemoved([FromBody] ConfigurationChangeArguments arguments)
    {
        this._plugInManager.ApplyRemovedConfiguration(arguments.Type, arguments.Id);
    }
}
