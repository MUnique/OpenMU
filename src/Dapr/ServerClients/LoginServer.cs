// <copyright file="LoginServer.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.ServerClients;

using Microsoft.Extensions.Logging;
using Dapr.Client;
using MUnique.OpenMU.Interfaces;

public class LoginServer : ILoginServer
{
    private readonly DaprClient _daprClient;
    private readonly ILogger<LoginServer> _logger;
    private readonly string _targetAppId;

    public LoginServer(DaprClient daprClient, ILogger<LoginServer> logger)
    {
        this._daprClient = daprClient;
        this._logger = logger;
        this._targetAppId = "loginServer";
    }

    public async Task<bool> TryLogin(string accountName, byte serverId)
    {
        try
        {
            return await this._daprClient.InvokeMethodAsync<LoginArguments, bool>(this._targetAppId, nameof(TryLogin), new LoginArguments(accountName, serverId));
        }
        catch (Exception ex)
        {
            this._logger.LogError(ex, "Unexpected error when trying to call TryLogin on the login server.");
            return false;
        }
    }

    public void LogOff(string accountName, byte serverId)
    {
        try
        {
            this._daprClient.InvokeMethodAsync(this._targetAppId, nameof(LogOff), new LoginArguments(accountName, serverId));
        }
        catch (Exception ex)
        {
            this._logger.LogError(ex, "Unexpected error when trying to call LogOff on the login server.");
        }
    }
}