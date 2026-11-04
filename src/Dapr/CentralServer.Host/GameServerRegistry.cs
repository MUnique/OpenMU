// <copyright file="GameServerRegistry.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.CentralServer.Host;

using System.Threading;
using Microsoft.Extensions.Logging;
using MUnique.OpenMU.PlugIns;
using MUnique.OpenMU.ServerClients;
using Nito.AsyncEx;

/// <summary>
/// The registry of the game servers, which is kept up to date by their heartbeats.
/// </summary>
/// <remarks>
/// The connect server builds its server list from it, and the login server cleans up
/// the login states of a game server which went offline or got restarted.
/// </remarks>
public sealed class GameServerRegistry : IDisposable
{
    /// <summary>
    /// The time without heartbeat after which a game server is considered to be offline.
    /// </summary>
    private readonly TimeSpan _timeout = TimeSpan.FromSeconds(20);

    private readonly CancellationTokenSource _disposeCts = new();
    private readonly ILogger<GameServerRegistry> _logger;
    private readonly Dictionary<ushort, DateTime> _entries = new();
    private readonly AsyncLock _lock = new();

    /// <summary>
    /// Initializes a new instance of the <see cref="GameServerRegistry"/> class.
    /// </summary>
    /// <param name="logger">The logger.</param>
    public GameServerRegistry(ILogger<GameServerRegistry> logger)
    {
        this._logger = logger;

        async Task RunCleanupLoopAsync()
        {
            try
            {
                await this.CleanupLoopAsync(this._disposeCts.Token).ConfigureAwait(false);
            }
            catch (OperationCanceledException)
            {
                // expected when disposing
            }
            catch (Exception ex)
            {
                this._logger.LogError(ex, "Error in cleanup loop");
            }
        }

        _ = RunCleanupLoopAsync();
    }

    /// <summary>
    /// Occurs when a game server was added to the registry, with its first received heartbeat.
    /// </summary>
    public event AsyncEventHandler<GameServerHeartbeatArguments>? GameServerAdded;

    /// <summary>
    /// Occurs when a heartbeat of an already registered game server was received.
    /// </summary>
    public event AsyncEventHandler<GameServerHeartbeatArguments>? GameServerUpdated;

    /// <summary>
    /// Occurs when a game server was removed from the registry, because its heartbeat timed out.
    /// </summary>
    public event AsyncEventHandler<ushort>? GameServerRemoved;

    /// <inheritdoc />
    public void Dispose()
    {
        this._disposeCts.Cancel();
        this._disposeCts.Dispose();
    }

    /// <summary>
    /// Updates the registration of the game server with its heartbeat.
    /// </summary>
    /// <param name="heartbeat">The heartbeat of the game server.</param>
    public async Task UpdateRegistrationAsync(GameServerHeartbeatArguments heartbeat)
    {
        bool isNew;
        using (await this._lock.LockAsync().ConfigureAwait(false))
        {
            isNew = !this._entries.ContainsKey(heartbeat.ServerInfo.Id);
            this._entries[heartbeat.ServerInfo.Id] = DateTime.UtcNow;
        }

        if (isNew)
        {
            await this.GameServerAdded.SafeInvokeAsync(heartbeat).ConfigureAwait(false);
        }
        else
        {
            await this.GameServerUpdated.SafeInvokeAsync(heartbeat).ConfigureAwait(false);
        }
    }

    private async Task CleanupLoopAsync(CancellationToken cancellationToken)
    {
        var removed = new List<ushort>();
        while (!cancellationToken.IsCancellationRequested)
        {
            await Task.Delay(2000, cancellationToken).ConfigureAwait(false);
            using (await this._lock.LockAsync(cancellationToken).ConfigureAwait(false))
            {
                foreach (var (serverId, lastUpdate) in this._entries)
                {
                    var diff = DateTime.UtcNow - lastUpdate;
                    if (diff > this._timeout)
                    {
                        this._logger.LogInformation("Difference of {0} higher than timeout for server {1}", diff, serverId);
                        removed.Add(serverId);
                    }
                }

                foreach (var serverId in removed)
                {
                    this._entries.Remove(serverId);
                }
            }

            foreach (var serverId in removed)
            {
                await this.GameServerRemoved.SafeInvokeAsync(serverId).ConfigureAwait(false);
            }

            removed.Clear();
        }
    }
}
