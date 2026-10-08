// <copyright file="ConnectServerListUpdater.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.CentralServer.Host;

using System.Net;
using System.Threading;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using MUnique.OpenMU.ServerClients;
using ConnectServer = MUnique.OpenMU.ConnectServer.ConnectServer;

/// <summary>
/// A <see cref="IHostedService"/> which keeps the server lists of the connect servers
/// up to date with the game servers of the <see cref="GameServerRegistry"/>.
/// A game server is listed at each connect server, for whose client version it has an end point.
/// </summary>
public sealed class ConnectServerListUpdater : IHostedService
{
    private readonly GameServerRegistry _registry;
    private readonly ConnectServerCollection _connectServers;
    private readonly ILogger<ConnectServerListUpdater> _logger;

    /// <summary>
    /// The end points, at which a game server is currently registered at a connect server.
    /// </summary>
    private readonly Dictionary<(ConnectServer ConnectServer, ushort GameServerId), string> _registeredEndPoints = new();

    /// <summary>
    /// Initializes a new instance of the <see cref="ConnectServerListUpdater"/> class.
    /// </summary>
    /// <param name="registry">The registry.</param>
    /// <param name="connectServers">The connect servers.</param>
    /// <param name="logger">The logger.</param>
    public ConnectServerListUpdater(GameServerRegistry registry, ConnectServerCollection connectServers, ILogger<ConnectServerListUpdater> logger)
    {
        this._registry = registry;
        this._connectServers = connectServers;
        this._logger = logger;
    }

    /// <inheritdoc />
    public Task StartAsync(CancellationToken cancellationToken)
    {
        this._registry.GameServerAdded += this.OnGameServerAddedOrUpdatedAsync;
        this._registry.GameServerUpdated += this.OnGameServerAddedOrUpdatedAsync;
        this._registry.GameServerRemoved += this.OnGameServerRemovedAsync;
        return Task.CompletedTask;
    }

    /// <inheritdoc />
    public Task StopAsync(CancellationToken cancellationToken)
    {
        this._registry.GameServerAdded -= this.OnGameServerAddedOrUpdatedAsync;
        this._registry.GameServerUpdated -= this.OnGameServerAddedOrUpdatedAsync;
        this._registry.GameServerRemoved -= this.OnGameServerRemovedAsync;
        return Task.CompletedTask;
    }

    private ValueTask OnGameServerAddedOrUpdatedAsync(GameServerHeartbeatArguments heartbeat)
    {
        var serverInfo = heartbeat.ServerInfo;
        lock (this._registeredEndPoints)
        {
            foreach (var connectServer in this._connectServers)
            {
                var key = (connectServer, serverInfo.Id);
                try
                {
                    var endPoint = heartbeat.EndPoints.FirstOrDefault(ep => ConnectServerCollection.IsFor(connectServer, ep));
                    this._registeredEndPoints.TryGetValue(key, out var registeredEndPoint);
                    if (endPoint is null)
                    {
                        if (registeredEndPoint is not null)
                        {
                            // The game server doesn't listen for this client version anymore.
                            connectServer.UnregisterGameServer(serverInfo.Id);
                            this._registeredEndPoints.Remove(key);
                        }
                    }
                    else if (endPoint.PublicEndPoint != registeredEndPoint)
                    {
                        // It's new, or its end point changed, e.g. because its public IP changed.
                        connectServer.RegisterGameServer(serverInfo, IPEndPoint.Parse(endPoint.PublicEndPoint));
                        this._registeredEndPoints[key] = endPoint.PublicEndPoint;
                    }
                    else
                    {
                        connectServer.CurrentConnectionsChanged(serverInfo.Id, serverInfo.CurrentConnections);
                    }
                }
                catch (Exception ex)
                {
                    this._logger.LogError(ex, "Error when updating game server {0} in the server list of connect server {1}", serverInfo.Id, connectServer.Description);
                }
            }
        }

        return ValueTask.CompletedTask;
    }

    private ValueTask OnGameServerRemovedAsync(ushort serverId)
    {
        lock (this._registeredEndPoints)
        {
            foreach (var connectServer in this._connectServers)
            {
                try
                {
                    if (this._registeredEndPoints.Remove((connectServer, serverId)))
                    {
                        connectServer.UnregisterGameServer(serverId);
                    }
                }
                catch (Exception ex)
                {
                    this._logger.LogError(ex, "Error when removing game server {0} from the server list of connect server {1}", serverId, connectServer.Description);
                }
            }
        }

        return ValueTask.CompletedTask;
    }
}
