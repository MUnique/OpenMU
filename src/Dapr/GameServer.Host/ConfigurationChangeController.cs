// <copyright file="ConfigurationChangeController.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.GameServer.Host;

using global::Dapr;
using Microsoft.AspNetCore.Mvc;
using MUnique.OpenMU.Dapr.Common;
using MUnique.OpenMU.Interfaces;
using MUnique.OpenMU.Persistence.EntityFramework;

/// <summary>
/// The API controller which handles the calls of the <see cref="ConfigurationChangePublisher"/>
/// from other services, such as the AdminPanel.
/// It forwards the changes to a <see cref="IConfigurationChangeListener"/>, so
/// that the caches are updated and the game logic can react to that.
/// </summary>
[ApiController]
[Route("")]
public class ConfigurationChangeController : ControllerBase
{
    private readonly IConfigurationChangeListener _changeListener;

    /// <summary>
    /// Initializes a new instance of the <see cref="ConfigurationChangeController" /> class.
    /// </summary>
    /// <param name="changeListener">The change listener.</param>
    public ConfigurationChangeController(IConfigurationChangeListener changeListener)
    {
        this._changeListener = changeListener;
    }

    /// <summary>
    /// Called when a configuration got added on the admin panel.
    /// </summary>
    /// <param name="arguments">The message arguments.</param>
    [HttpPost(nameof(IConfigurationChangePublisher.ConfigurationAddedAsync))]
    [Topic("pubsub", nameof(IConfigurationChangePublisher.ConfigurationAddedAsync))]
    public ValueTask ConfigurationAddedAsync([FromBody] ConfigurationChangeArguments arguments)
    {
        return this._changeListener.ConfigurationAddedAsync(arguments.Type, arguments.Id, arguments.Configuration!, null, null);
    }

    /// <summary>
    /// Called when a configuration got added on the admin panel.
    /// </summary>
    /// <param name="arguments">The message arguments.</param>
    [HttpPost(nameof(IConfigurationChangePublisher.ConfigurationChangedAsync))]
    [Topic("pubsub", nameof(IConfigurationChangePublisher.ConfigurationChangedAsync))]
    public ValueTask ConfigurationChangedAsync([FromBody] ConfigurationChangeArguments arguments)
    {
        return this._changeListener.ConfigurationChangedAsync(arguments.Type, arguments.Id, arguments.Configuration!, null);
    }

    /// <summary>
    /// Called when a configuration got removed on the admin panel.
    /// </summary>
    /// <param name="arguments">The message arguments.</param>
    [HttpPost(nameof(IConfigurationChangePublisher.ConfigurationRemovedAsync))]
    [Topic("pubsub", nameof(IConfigurationChangePublisher.ConfigurationRemovedAsync))]
    public ValueTask ConfigurationRemovedAsync([FromBody] ConfigurationChangeArguments arguments)
    {
        return this._changeListener.ConfigurationRemovedAsync(arguments.Type, arguments.Id, null, null);
    }
}