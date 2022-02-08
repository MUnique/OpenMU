using Dapr;
using Dapr.Client;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;

namespace MUnique.OpenMU.Dapr.Common;

[ApiController]
[Route("")]
public class ServerStateController
{
    private readonly ManagableServerRegistry _registry;
    private readonly ILogger<ServerStateController> _logger;

    public ServerStateController(ManagableServerRegistry registry, ILogger<ServerStateController> logger)
    {
        _registry = registry;
        _logger = logger;
    }

    [HttpPost(ManagableServerStatePublisher.TopicName)]
    [Topic("pubsub", ManagableServerStatePublisher.TopicName)]
    public void ServerStateUpdate([FromBody] ServerStateData data)
    {
        try
        {
            this._registry.HandleUpdate(data);
        }
        catch (Exception ex)
        {
            this._logger.LogError(ex, "Error updating the ManagableServerRegistry");
        }
    }
}