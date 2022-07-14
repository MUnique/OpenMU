// <copyright file="ConnectionServerController.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.ConnectServer.Host;

using System.Net;
using global::Dapr;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using MUnique.OpenMU.ServerClients;

/// <summary>
/// API Controller which receives messages from other services.
/// </summary>
[ApiController]
[Route("")]
public class ConnectServerController : ControllerBase
{
    private readonly GameServerRegistry _registry;
    private readonly ILogger<ConnectServerController> _logger;

    /// <summary>
    /// Initializes a new instance of the <see cref="ConnectServerController"/> class.
    /// </summary>
    /// <param name="registry">The registry.</param>
    /// <param name="logger">The logger.</param>
    public ConnectServerController(GameServerRegistry registry, ILogger<ConnectServerController> logger)
    {
        this._registry = registry;
        this._logger = logger;
    }

    /// <summary>
    /// Handles the game server heartbeat.
    /// </summary>
    /// <param name="data">The data.</param>
    [HttpPost("GameServerHeartbeat")]
    [Topic("pubsub", "GameServerHeartbeat")]
    public async Task GameServerHeartbeatAsync([FromBody] GameServerHeartbeatArguments data)
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