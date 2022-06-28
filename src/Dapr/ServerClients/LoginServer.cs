// <copyright file="LoginServer.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.ServerClients;

using Microsoft.Extensions.Logging;
using Dapr.Client;
using MUnique.OpenMU.Interfaces;

/// <summary>
/// Implementation of an <see cref="ILoginServer"/> which accesses the login server remotely over Dapr.
/// </summary>
public class LoginServer : ILoginServer
{
    private readonly DaprClient _daprClient;
    private readonly ILogger<LoginServer> _logger;
    private readonly string _targetAppId;

    /// <summary>
    /// Initializes a new instance of the <see cref="LoginServer"/> class.
    /// </summary>
    /// <param name="daprClient">The dapr client.</param>
    /// <param name="logger">The logger.</param>
    public LoginServer(DaprClient daprClient, ILogger<LoginServer> logger)
    {
        this._daprClient = daprClient;
        this._logger = logger;
        this._targetAppId = "loginServer";
    }

    /// <inheritdoc />
    public async Task<bool> TryLogin(string accountName, byte serverId)
    {
        try
        {
            return await this._daprClient.InvokeMethodAsync<LoginArguments, bool>(this._targetAppId, nameof(this.TryLogin), new LoginArguments(accountName, serverId));
        }
        catch (Exception ex)
        {
            this._logger.LogError(ex, "Unexpected error when trying to call TryLogin on the login server.");
            return false;
        }
    }

    /// <inheritdoc />
    public void LogOff(string accountName, byte serverId)
    {
        try
        {
            this._daprClient.InvokeMethodAsync(this._targetAppId, nameof(this.LogOff), new LoginArguments(accountName, serverId));
        }
        catch (Exception ex)
        {
            this._logger.LogError(ex, "Unexpected error when trying to call LogOff on the login server.");
        }
    }
}