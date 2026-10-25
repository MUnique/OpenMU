// <copyright file="PersistentLoginServer.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.CentralServer.Host;

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

    // Applies only to reservations. Ordinary offline/login state must remain durable.
    private static readonly IReadOnlyDictionary<string, string> ReservationMetadata = new Dictionary<string, string> { ["ttlInSeconds"] = "300" };
    private static readonly IReadOnlyDictionary<string, string> DurableMetadata = new Dictionary<string, string> { ["ttlInSeconds"] = "-1" };

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

    /// <inheritdoc />
    public async ValueTask<bool> TryReserveAccountAsync(string accountName, int reservationId)
    {
        ArgumentOutOfRangeException.ThrowIfLessThanOrEqual(reservationId, byte.MaxValue);
        var (current, etag) = await this._daprClient.GetStateAndETagAsync<int?>(StoreName, accountName, ConsistencyMode.Strong).ConfigureAwait(false);
        if (current == reservationId)
        {
            return true;
        }

        if (current is >= 0)
        {
            return false;
        }

        // Accounts edited through the admin layer must have an initialized state entry.
        // Failing closed avoids an unconditional first insert racing a real login.
        if (string.IsNullOrEmpty(etag))
        {
            return false;
        }

        return await this._daprClient.TrySaveStateAsync(StoreName, accountName, reservationId, etag, metadata: ReservationMetadata).ConfigureAwait(false);
    }

    /// <inheritdoc />
    public async ValueTask ReleaseAccountReservationAsync(string accountName, int reservationId)
    {
        ArgumentOutOfRangeException.ThrowIfLessThanOrEqual(reservationId, byte.MaxValue);
        var (current, etag) = await this._daprClient.GetStateAndETagAsync<int?>(StoreName, accountName, ConsistencyMode.Strong).ConfigureAwait(false);
        if (current == reservationId && !await this._daprClient.TrySaveStateAsync(StoreName, accountName, OfflineServerId, etag, metadata: DurableMetadata).ConfigureAwait(false))
        {
            throw new InvalidOperationException("The offline account reservation could not be released.");
        }
    }

    /// <summary>
    /// Removes the server.
    /// </summary>
    /// <param name="serverId">The server identifier.</param>
    public async Task RemoveServerAsync(byte serverId)
    {
        var indexName = GetIndexName(serverId);
        var (serverIndex, eTag) = await this._daprClient.GetStateAndETagAsync<HashSet<string>>(StoreName, indexName, ConsistencyMode.Strong).ConfigureAwait(false);
        if (serverIndex is null || serverIndex.Count == 0)
        {
            return;
        }

        foreach (var accountName in serverIndex)
        {
            await this.SetAccountOfflineAsync(accountName, serverId).ConfigureAwait(false);
        }

        serverIndex.Clear();

        if (!await this._daprClient.TrySaveStateAsync(StoreName, indexName, serverIndex, eTag).ConfigureAwait(false))
        {
            // try again, if it failed
            await this.RemoveServerAsync(serverId).ConfigureAwait(false);
        }
    }

    /// <inheritdoc />
    public async Task<bool> TryLoginAsync(string accountName, byte serverId)
    {
        try
        {
            var (currentServerId, eTag) = await this._daprClient.GetStateAndETagAsync<int?>(StoreName, accountName, ConsistencyMode.Strong).ConfigureAwait(false);
            if (currentServerId is >= 0)
            {
                return false;
            }

            if (string.IsNullOrEmpty(eTag))
            {
                // Never logged in, so first insert a fresh state and try again.
                // We never want to have the same account logged in twice, because that may lead to game mechanic exploits.
                await this._daprClient.SaveStateAsync<int?>(StoreName, accountName, OfflineServerId, new StateOptions { Concurrency = ConcurrencyMode.FirstWrite, Consistency = ConsistencyMode.Strong }, metadata: DurableMetadata).ConfigureAwait(false);
                return await this.TryLoginAsync(accountName, serverId).ConfigureAwait(false);
            }

            if (!await this._daprClient.TrySaveStateAsync(StoreName, accountName, serverId, eTag, metadata: DurableMetadata).ConfigureAwait(false))
            {
                // Another server changed the state of the account in the meantime.
                return false;
            }

            await this.AddToIndexAsync(accountName, serverId).ConfigureAwait(false);
            return true;
        }
        catch (Exception ex)
        {
            this._logger.LogError(ex, "Couldn't get/set logged-in state for account {0}", accountName);
            return false;
        }
    }

    /// <inheritdoc />
    public async ValueTask LogOffAsync(string accountName, byte serverId)
    {
        try
        {
            await this.SetAccountOfflineAsync(accountName, serverId).ConfigureAwait(false);
            await this.RemoveFromIndexAsync(accountName, serverId).ConfigureAwait(false);
        }
        catch (Exception ex)
        {
            this._logger.LogError(ex, "Unexpected error when removing account {0} from server {1}", accountName, serverId);
        }
    }

    /// <inheritdoc />
    public async ValueTask<Dictionary<string, byte>> GetSnapshotAsync()
    {
        var result = new Dictionary<string, byte>();
        var indexNames = Enumerable.Range(byte.MinValue, byte.MaxValue + 1).Select(GetIndexName).ToList();
        var serverIndexes = await this._daprClient.GetBulkStateAsync<HashSet<string>>(StoreName, indexNames, parallelism: null).ConfigureAwait(false);
        foreach (var serverIndex in serverIndexes)
        {
            if (serverIndex.Value is null)
            {
                continue;
            }

            var serverId = (byte)indexNames.IndexOf(serverIndex.Key);
            foreach (var accountName in serverIndex.Value)
            {
                result[accountName] = serverId;
            }
        }

        return result;
    }

    private static string GetIndexName(int serverId) => $"serverindex-{serverId}";

    private async Task AddToIndexAsync(string accountName, byte serverId)
    {
        var indexName = GetIndexName(serverId);
        var (serverIndex, eTag) = await this._daprClient.GetStateAndETagAsync<HashSet<string>>(StoreName, indexName, ConsistencyMode.Strong).ConfigureAwait(false);
        if (serverIndex is null)
        {
            serverIndex = new HashSet<string>();
            serverIndex.Add(accountName);
            await this._daprClient.SaveStateAsync(StoreName, indexName, serverIndex).ConfigureAwait(false);
            return;
        }

        if (serverIndex.Add(accountName))
        {
            if (!await this._daprClient.TrySaveStateAsync(StoreName, indexName, serverIndex, eTag).ConfigureAwait(false))
            {
                // try again, if it failed
                await this.AddToIndexAsync(accountName, serverId).ConfigureAwait(false);
            }
        }
    }

    private async Task RemoveFromIndexAsync(string accountName, byte serverId)
    {
        var indexName = GetIndexName(serverId);
        var (serverIndex, eTag) = await this._daprClient.GetStateAndETagAsync<HashSet<string>>(StoreName, indexName, ConsistencyMode.Strong).ConfigureAwait(false);
        if (serverIndex is null)
        {
            return;
        }

        if (serverIndex.Remove(accountName))
        {
            if (!await this._daprClient.TrySaveStateAsync(StoreName, indexName, serverIndex, eTag).ConfigureAwait(false))
            {
                // try again, if it failed
                await this.RemoveFromIndexAsync(accountName, serverId).ConfigureAwait(false);
            }
        }
    }

    private async Task SetAccountOfflineAsync(string accountName, byte serverId)
    {
        try
        {
            var (currentServerId, eTag) = await this._daprClient.GetStateAndETagAsync<int?>(StoreName, accountName, ConsistencyMode.Strong).ConfigureAwait(false);
            if (currentServerId != serverId)
            {
                // It's already offline, or logged in at another server, e.g. when this is a late log off.
                return;
            }

            await this._daprClient.TrySaveStateAsync<int?>(StoreName, accountName, OfflineServerId, eTag, metadata: DurableMetadata).ConfigureAwait(false);
        }
        catch (Exception ex)
        {
            this._logger.LogError(ex, "Couldn't get/set logged-out state for account {0}", accountName);
        }
    }
}
