// <copyright file="ConnectServerListUpdater.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.CentralServer.Host;

using System.Net;
using System.Threading;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using MUnique.OpenMU.Interfaces;
using MUnique.OpenMU.ServerClients;

/// <summary>
/// A <see cref="IHostedService"/> which keeps the server list of the <see cref="IConnectServer"/>
/// up to date with the game servers of the <see cref="GameServerRegistry"/>.
/// </summary>
public sealed class ConnectServerListUpdater : IHostedService
{
    private readonly GameServerRegistry _registry;
    private readonly IConnectServer _connectServer;
    private readonly ILogger<ConnectServerListUpdater> _logger;

    /// <summary>
    /// Initializes a new instance of the <see cref="ConnectServerListUpdater"/> class.
    /// </summary>
    /// <param name="registry">The registry.</param>
    /// <param name="connectServer">The connect server.</param>
    /// <param name="logger">The logger.</param>
    public ConnectServerListUpdater(GameServerRegistry registry, IConnectServer connectServer, ILogger<ConnectServerListUpdater> logger)
    {
        this._registry = registry;
        this._connectServer = connectServer;
        this._logger = logger;
    }

    /// <inheritdoc />
    public Task StartAsync(CancellationToken cancellationToken)
    {
        this._registry.GameServerAdded += this.OnGameServerAddedAsync;
        this._registry.GameServerUpdated += this.OnGameServerUpdatedAsync;
        this._registry.GameServerRemoved += this.OnGameServerRemovedAsync;
        return Task.CompletedTask;
    }

    /// <inheritdoc />
    public Task StopAsync(CancellationToken cancellationToken)
    {
        this._registry.GameServerAdded -= this.OnGameServerAddedAsync;
        this._registry.GameServerUpdated -= this.OnGameServerUpdatedAsync;
        this._registry.GameServerRemoved -= this.OnGameServerRemovedAsync;
        return Task.CompletedTask;
    }

    private ValueTask OnGameServerAddedAsync(GameServerHeartbeatArguments heartbeat)
    {
        try
        {
            this._connectServer.RegisterGameServer(heartbeat.ServerInfo, IPEndPoint.Parse(heartbeat.PublicEndPoint));
        }
        catch (Exception ex)
        {
            this._logger.LogError(ex, "Error when adding game server {0} to the server list", heartbeat.ServerInfo.Id);
        }

        return ValueTask.CompletedTask;
    }

    private ValueTask OnGameServerUpdatedAsync(GameServerHeartbeatArguments heartbeat)
    {
        try
        {
            this._connectServer.CurrentConnectionsChanged(heartbeat.ServerInfo.Id, heartbeat.ServerInfo.CurrentConnections);
        }
        catch (Exception ex)
        {
            this._logger.LogError(ex, "Error when updating game server {0} in the server list", heartbeat.ServerInfo.Id);
        }

        return ValueTask.CompletedTask;
    }

    private ValueTask OnGameServerRemovedAsync(ushort serverId)
    {
        try
        {
            this._connectServer.UnregisterGameServer(serverId);
        }
        catch (Exception ex)
        {
            this._logger.LogError(ex, "Error when removing game server {0} from the server list", serverId);
        }

        return ValueTask.CompletedTask;
    }
}
