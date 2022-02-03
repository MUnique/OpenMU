using System.Net;
using System.Security.Cryptography.X509Certificates;
using Dapr;

namespace MUnique.OpenMU.LoginServer.Host;

using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using MUnique.OpenMU.Interfaces;
using MUnique.OpenMU.ServerClients;

[ApiController]
[Route("")]
public class LoginServerController : ControllerBase
{
    private readonly ILoginServer _loginServer;

    private readonly ILogger<LoginServerController> _logger;

    private readonly GameServerRegistry _registry;

    public LoginServerController(PersistentLoginServer loginServer, ILogger<LoginServerController> logger, GameServerRegistry registry)
    {
        this._loginServer = loginServer;
        this._logger = logger;
        _registry = registry;
    }

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

    [HttpPost("GameServerHeartbeat")]
    [Topic("pubsub", "GameServerHeartbeat")]
    public Task GameServerHeartbeatAsync([FromBody] GameServerHeartbeatArguments data)
    {
        return this._registry.UpdateRegistrationAsync(data.ServerInfo.Id, data.UpTime);
    }
}