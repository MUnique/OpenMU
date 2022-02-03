using System.Collections.Concurrent;
using System.Diagnostics;
using System.Net;
using Dapr;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using MUnique.OpenMU.ServerClients;
using OpenTelemetry.Trace;

namespace MUnique.OpenMU.ConnectServer.Host;

[ApiController]
[Route("")]
public class ConnectServerController : ControllerBase
{
    private readonly GameServerRegistry _registry;
    private readonly ILogger<ConnectServerController> _logger;

    public ConnectServerController(GameServerRegistry registry, ILogger<ConnectServerController> logger)
    {
        this._registry = registry;
        _logger = logger;
    }

    [HttpPost("GameServerHeartbeat")]
    [Topic("pubsub", "GameServerHeartbeat")]
    public async Task GameServerHeartbeat([FromBody] GameServerHeartbeatArguments data)
    {
        try
        {
            await this._registry.UpdateRegistrationAsync(data.ServerInfo, IPEndPoint.Parse(data.PublicEndPoint));
        }
        catch (Exception ex)
        {
            this._logger.LogError(ex, "Error updating the GameServerRegistry");
        }
    }
}