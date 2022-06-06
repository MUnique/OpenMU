// <copyright file="PersistentLoginServer.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.LoginServer.Host;

using global::Dapr.Client;
using Microsoft.Extensions.Logging;
using MUnique.OpenMU.Interfaces;

/// <summary>
/// An implementation of a <see cref="ILoginServer"/> which persists the login state in a dapr state store.
/// </summary>
public sealed class PersistentLoginServer : ILoginServer
{
    private const string StoreName = "login-state";

    private const int OfflineServerId = -1;

    private readonly ILogger<PersistentLoginServer> _logger;

    private readonly DaprClient _daprClient;

    /// <summary>
    /// Initializes a new instance of the <see cref="PersistentLoginServer"/> class.
    /// </summary>
    /// <param name="logger">The logger.</param>
    /// <param name="daprClient">The dapr client.</param>
    public PersistentLoginServer(ILogger<PersistentLoginServer> logger, DaprClient daprClient)
    {
        this._logger = logger;
        this._daprClient = daprClient;
    }

    /// <summary>
    /// Removes the server.
    /// </summary>
    /// <param name="serverId">The server identifier.</param>
    public async Task RemoveServerAsync(byte serverId)
    {
        var indexName = $"serverindex-{serverId}";
        var (serverIndex, eTag) = await this._daprClient.GetStateAndETagAsync<HashSet<string>>(StoreName, indexName, ConsistencyMode.Strong);
        if (serverIndex is null || serverIndex.Count == 0)
        {
            return;
        }

        foreach (var accountName in serverIndex)
        {
            await this.SetAccountOfflineAsync(accountName);
        }

        serverIndex.Clear();

        if (!await this._daprClient.TrySaveStateAsync(StoreName, indexName, serverIndex, eTag))
        {
            // try again, if it failed
            await this.RemoveServerAsync(serverId);
        }
    }

    /// <inheritdoc />
    public async Task<bool> TryLogin(string accountName, byte serverId)
    {
        try
        {
            var (currentServerId, eTag) = await this._daprClient.GetStateAndETagAsync<int?>(StoreName, accountName, ConsistencyMode.Strong);
            if (currentServerId is >= 0)
            {
                return false;
            }

            if (string.IsNullOrEmpty(eTag))
            {
                // Never logged in, so first insert a fresh state and try again.
                // We never want to have the same account logged in twice, because that may lead to game mechanic exploits.
                await this._daprClient.SaveStateAsync<int?>(StoreName, accountName, OfflineServerId, new StateOptions { Concurrency = ConcurrencyMode.FirstWrite, Consistency = ConsistencyMode.Strong });
                return await this.TryLogin(accountName, serverId);
            }

            var success = await this._daprClient.TrySaveStateAsync(StoreName, accountName, serverId, eTag);
            await this.AddToIndexAsync(accountName, serverId);
            return success;
        }
        catch (Exception ex)
        {
            this._logger.LogError(ex, "Couldn't get/set logged-in state for account {0}", accountName);
            return false;
        }
    }

    /// <inheritdoc />
    public void LogOff(string accountName, byte serverId)
    {
        Task.Run(async () =>
        {
            try
            {
                await this.SetAccountOfflineAsync(accountName);
                await this.RemoveFromIndexAsync(accountName, serverId);
            }
            catch (Exception ex)
            {
                this._logger.LogError(ex, "Unexpected error when removing account {0} from server {1}", accountName, serverId);
            }
        });
    }

    private async Task AddToIndexAsync(string accountName, byte serverId)
    {
        var indexName = $"serverindex-{serverId}";
        var (serverIndex, eTag) = await this._daprClient.GetStateAndETagAsync<HashSet<string>>(StoreName, indexName, ConsistencyMode.Strong);
        if (serverIndex is null)
        {
            serverIndex = new HashSet<string>();
            serverIndex.Add(accountName);
            await this._daprClient.SaveStateAsync(StoreName, indexName, serverIndex);
            return;
        }

        if (serverIndex.Add(accountName))
        {
            if (!await this._daprClient.TrySaveStateAsync(StoreName, indexName, serverIndex, eTag))
            {
                // try again, if it failed
                await this.AddToIndexAsync(accountName, serverId);
            }
        }
    }

    private async Task RemoveFromIndexAsync(string accountName, byte serverId)
    {
        var indexName = $"serverindex-{serverId}";
        var (serverIndex, eTag) = await this._daprClient.GetStateAndETagAsync<HashSet<string>>(StoreName, indexName, ConsistencyMode.Strong);
        if (serverIndex is null)
        {
            return;
        }

        if (serverIndex.Remove(accountName))
        {
            if (!await this._daprClient.TrySaveStateAsync(StoreName, indexName, serverIndex, eTag))
            {
                // try again, if it failed
                await this.RemoveFromIndexAsync(accountName, serverId);
            }
        }
    }

    private async Task SetAccountOfflineAsync(string accountName)
    {
        try
        {
            await this._daprClient.SaveStateAsync<int?>(StoreName, accountName, OfflineServerId);
        }
        catch (Exception ex)
        {
            this._logger.LogError(ex, "Couldn't get/set logged-out state for account {0}", accountName);
        }
    }
}