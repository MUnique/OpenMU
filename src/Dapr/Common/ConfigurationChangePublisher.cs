// <copyright file="ConfigurationChangePublisher.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.Dapr.Common;

using global::Dapr.Client;
using MUnique.OpenMU.Interfaces;

/// <summary>
/// Publisher for configuration changes.
/// Changes are published to the configured pub/sub Dapr component.
/// </summary>
public class ConfigurationChangePublisher : IConfigurationChangePublisher
{
    private readonly DaprClient _daprClient;

    /// <summary>
    /// Initializes a new instance of the <see cref="ConfigurationChangePublisher"/> class.
    /// </summary>
    /// <param name="daprClient">The dapr client.</param>
    public ConfigurationChangePublisher(DaprClient daprClient)
    {
        this._daprClient = daprClient;
    }

    /// <summary>
    /// Publishes a changed, previously existing configuration.
    /// </summary>
    /// <param name="type">The type of the configuration.</param>
    /// <param name="id">The identifier of the changed configuration.</param>
    /// <param name="configuration">The changed configuration.</param>
    public void ConfigurationChanged(Type type, Guid id, object configuration)
    {
        this._daprClient.PublishEventAsync("pubsub", nameof(this.ConfigurationChanged), configuration);
    }

    /// <summary>
    /// Publishes an added configuration.
    /// </summary>
    /// <param name="type">The type of the configuration.</param>
    /// <param name="id">The identifier of the added configuration.</param>
    /// <param name="configuration">The added configuration.</param>
    public void ConfigurationAdded(Type type, Guid id, object configuration)
    {
        this._daprClient.PublishEventAsync("pubsub", nameof(this.ConfigurationAdded), configuration);
    }

    /// <summary>
    /// Publishes a removed, previously existing configuration.
    /// </summary>
    /// <param name="type">The type of the configuration.</param>
    /// <param name="id">The identifier of the removed configuration.</param>
    public void ConfigurationRemoved(Type type, Guid id)
    {
        this._daprClient.PublishEventAsync("pubsub", nameof(this.ConfigurationRemoved), id);
    }
}