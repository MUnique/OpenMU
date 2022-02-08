// <copyright file="ConnectionServerController.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.ConnectServer.Host;

using System.Net;
using global::Dapr;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using MUnique.OpenMU.ServerClients;

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