using System.Text.Json;
using Dapr.Client;
using Google.Protobuf;
using Microsoft.Extensions.Logging;
using MUnique.OpenMU.Interfaces;

namespace MUnique.OpenMU.LoginServer.Host;

public sealed class PersistentLoginServer : ILoginServer
{
    private const string StoreName = "login-state";

    private const int OfflineServerId = -1;

    private readonly ILogger<PersistentLoginServer> _logger;

    private readonly DaprClient _daprClient;

    public PersistentLoginServer(ILogger<PersistentLoginServer> logger, DaprClient daprClient)
    {
        _logger = logger;
        _daprClient = daprClient;
    }

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
            await SetAccountOffline(accountName);
        }

        serverIndex.Clear();

        if (!await this._daprClient.TrySaveStateAsync(StoreName, indexName, serverIndex, eTag))
        {
            // try again, if it failed
            await this.RemoveServerAsync(serverId);
        }
    }
    
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

    public void LogOff(string accountName, byte serverId)
    {
        Task.Run(async () =>
        {
            try
            {
                await SetAccountOffline(accountName);
                await RemoveFromIndexAsync(accountName, serverId);
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

    private async Task SetAccountOffline(string accountName)
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

    public static ByteString ToJsonByteString<T>(T data, JsonSerializerOptions options)
    {
        var bytes = JsonSerializer.SerializeToUtf8Bytes(data, options);
        return ByteString.CopyFrom(bytes);
    }
}