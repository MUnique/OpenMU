// <copyright file="LoginServerController.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.CentralServer.Host;

using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using MUnique.OpenMU.Interfaces;
using MUnique.OpenMU.ServerClients;

/// <summary>
/// The API controller for the login server.
/// </summary>
[ApiController]
[Route(CentralServer.LoginServerRoute)]
public class LoginServerController : ControllerBase
{
    private readonly ILoginServer _loginServer;

    private readonly ILogger<LoginServerController> _logger;

    /// <summary>
    /// Initializes a new instance of the <see cref="LoginServerController"/> class.
    /// </summary>
    /// <param name="loginServer">The login server.</param>
    /// <param name="logger">The logger.</param>
    public LoginServerController(PersistentLoginServer loginServer, ILogger<LoginServerController> logger)
    {
        this._loginServer = loginServer;
        this._logger = logger;
    }

    /// <summary>
    /// Tries to login the account on the specified server.
    /// </summary>
    /// <param name="data">The login data.</param>
    /// <returns>The success.</returns>
    [HttpPost(nameof(TryLoginAsync))]
    public async Task<bool> TryLoginAsync([FromBody] LoginArguments data)
    {
        try
        {
            return await this._loginServer.TryLoginAsync(data.AccountName, data.ServerId).ConfigureAwait(false);
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
    [HttpPost(nameof(LogOffAsync))]
    public async Task LogOffAsync([FromBody] LoginArguments data)
    {
        try
        {
            await this._loginServer.LogOffAsync(data.AccountName, data.ServerId).ConfigureAwait(false);
        }
        catch (Exception ex)
        {
            this._logger.LogError(ex, "Unexpected error when calling LogOff on the login server. Data: {0}", data);
        }
    }
}