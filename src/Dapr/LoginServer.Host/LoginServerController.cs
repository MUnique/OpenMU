// <copyright file="LoginServerController.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.LoginServer.Host;

using global::Dapr;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using MUnique.OpenMU.Interfaces;
using MUnique.OpenMU.ServerClients;

/// <summary>
/// The API controller for the login server.
/// </summary>
[ApiController]
[Route("")]
public class LoginServerController : ControllerBase
{
    private readonly ILoginServer _loginServer;

    private readonly ILogger<LoginServerController> _logger;

    private readonly GameServerRegistry _registry;

    /// <summary>
    /// Initializes a new instance of the <see cref="LoginServerController"/> class.
    /// </summary>
    /// <param name="loginServer">The login server.</param>
    /// <param name="logger">The logger.</param>
    /// <param name="registry">The registry.</param>
    public LoginServerController(PersistentLoginServer loginServer, ILogger<LoginServerController> logger, GameServerRegistry registry)
    {
        this._loginServer = loginServer;
        this._logger = logger;
        this._registry = registry;
    }

    /// <summary>
    /// Tries to login the account on the specified server.
    /// </summary>
    /// <param name="data">The login data.</param>
    /// <returns>The success.</returns>
    [HttpPost(nameof(TryLogin))]
    public async Task<bool> TryLogin([FromBody] LoginArguments data)
    {
        try
        {
            return await this._loginServer.TryLogin(data.AccountName, data.ServerId);
        }
        catch (Exception ex)
        {
            this._logger.LogError(ex, "Unexpected error when calling TryLogin on the login server. Data: {0}", data);
            return false;
        }
    }

    /// <summary>
    /// Logs the account off from the specified server.
    /// </summary>
    /// <param name="data">The login data.</param>
    [HttpPost(nameof(LogOff))]
    public void LogOff([FromBody] LoginArguments data)
    {
        try
        {
            this._loginServer.LogOff(data.AccountName, data.ServerId);
        }
        catch (Exception ex)
        {
            this._logger.LogError(ex, "Unexpected error when calling LogOff on the login server. Data: {0}", data);
        }
    }

    /// <summary>
    /// Handles the game server heartbeat by updating the registry.
    /// </summary>
    /// <param name="data">The game server heartbeat arguments.</param>
    [HttpPost("GameServerHeartbeat")]
    [Topic("pubsub", "GameServerHeartbeat")]
    public Task GameServerHeartbeatAsync([FromBody] GameServerHeartbeatArguments data)
    {
        return this._registry.UpdateRegistrationAsync(data.ServerInfo.Id, data.UpTime);
    }
}