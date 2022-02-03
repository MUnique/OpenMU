using Dapr.Client;
using MUnique.OpenMU.Interfaces;

namespace MUnique.OpenMU.Dapr.Common;

public class ConfigurationChangePublisher : IConfigurationChangePublisher
{
    private readonly DaprClient _daprClient;
        
    public ConfigurationChangePublisher(DaprClient daprClient)
    {
        _daprClient = daprClient;
    }

    public void ConfigurationChanged(Type type, Guid id, object configuration)
    {
        this._daprClient.PublishEventAsync("pubsub", nameof(ConfigurationChanged), configuration);
    }

    public void ConfigurationAdded(Type type, Guid id, object configuration)
    {
        this._daprClient.PublishEventAsync("pubsub", nameof(ConfigurationAdded), configuration);
    }

    public void ConfigurationRemoved(Type type, Guid id)
    {
        this._daprClient.PublishEventAsync("pubsub", nameof(ConfigurationRemoved), id);
    }
}