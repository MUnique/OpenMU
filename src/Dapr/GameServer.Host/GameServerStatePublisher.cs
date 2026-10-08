// <copyright file="GameServerStatePublisher.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.GameServer.Host;

using System.Diagnostics;
using System.Net;
using System.Threading;
using global::Dapr.Client;
using Microsoft.Extensions.Logging;
using MUnique.OpenMU.DataModel.Configuration;
using MUnique.OpenMU.Interfaces;
using MUnique.OpenMU.ServerClients;
using Nito.AsyncEx.Synchronous;

/// <summary>
/// Publishes the state of the game server by sending a heartbeat to a Dapr pub/sub component.
/// </summary>
/// <remarks>
/// The game server has one listener for each <see cref="GameServerEndpoint"/>, i.e. for each
/// supported client version. Each listener registers its public end point through the observer
/// of <see cref="ForEndpoint"/>, and the heartbeat contains all of them, so that the central server
/// can list the game server at the connect server of each client version.
/// </remarks>
public sealed class GameServerStatePublisher : IDisposable
{
    private const string PubSubName = "pubsub";
    private readonly DaprClient _daprClient;
    private readonly ILogger<GameServerStatePublisher> _logger;
    private readonly Dictionary<GameServerEndpoint, GameServerEndPointInfo> _endPoints = new();
    private readonly object _syncRoot = new();
    private readonly Stopwatch _upTime = Stopwatch.StartNew();

    private int _currentConnections;
    private ServerInfo? _serverInfo;

    private CancellationTokenSource? _heartbeatCancellationTokenSource;

    /// <summary>
    /// Initializes a new instance of the <see cref="GameServerStatePublisher"/> class.
    /// </summary>
    /// <param name="daprClient">The dapr client.</param>
    /// <param name="logger">The logger.</param>
    public GameServerStatePublisher(DaprClient daprClient, ILogger<GameServerStatePublisher> logger)
    {
        this._daprClient = daprClient;
        this._logger = logger;
    }

    /// <summary>
    /// Gets the observer for the listener of the specified endpoint.
    /// </summary>
    /// <param name="endpoint">The endpoint.</param>
    /// <returns>The observer for the listener of the specified endpoint.</returns>
    public IGameServerStateObserver ForEndpoint(GameServerEndpoint endpoint) => new EndpointObserver(this, endpoint);

    /// <inheritdoc />
    public void Dispose()
    {
        lock (this._syncRoot)
        {
            this._heartbeatCancellationTokenSource?.Cancel();
            this._heartbeatCancellationTokenSource?.Dispose();
            this._heartbeatCancellationTokenSource = null;
        }
    }

    private void RegisterEndPoint(GameServerEndpoint endpoint, ServerInfo serverInfo, IPEndPoint publicEndPoint)
    {
        var client = endpoint.Client ?? throw new InvalidOperationException($"The client of the endpoint with port {endpoint.NetworkPort} is not set.");
        lock (this._syncRoot)
        {
            this._serverInfo = serverInfo;
            this._endPoints[endpoint] = new GameServerEndPointInfo(publicEndPoint.ToString(), client.Season, client.Episode, (byte)client.Language);
            if (this._heartbeatCancellationTokenSource is null)
            {
                this.StartHeartbeat();
            }
        }
    }

    private void UnregisterEndPoint(GameServerEndpoint endpoint)
    {
        lock (this._syncRoot)
        {
            this._endPoints.Remove(endpoint);
            if (this._endPoints.Count == 0 && this._heartbeatCancellationTokenSource is { } cts)
            {
                this._logger.LogInformation("Stopping heartbeat thread");
                cts.Cancel();
                cts.Dispose();
                this._heartbeatCancellationTokenSource = null;
            }
        }
    }

    private void StartHeartbeat()
    {
        var cancellationTokenSource = new CancellationTokenSource();
        this._heartbeatCancellationTokenSource = cancellationTokenSource;
        var cancellationToken = cancellationTokenSource.Token;
        try
        {
            this._logger.LogInformation("Starting heartbeat thread ...");
            var heartbeatThread = new Thread(
                () =>
                {
                    try
                    {
                        this.HeartbeatLoop(cancellationToken);
                    }
                    catch (Exception ex)
                    {
                        this._logger.LogError(ex, "Error in heartbeat loop.");
                    }
                })
            {
                Name = "Heartbeat",
                IsBackground = true,
            };
            heartbeatThread.Start();

            this._logger.LogInformation("...started heartbeat thread.");
        }
        catch (Exception ex)
        {
            this._logger.LogError(ex, "Unexpected error when publishing the game server registration.");
        }
    }

    private void HeartbeatLoop(CancellationToken cancellationToken)
    {
        while (!cancellationToken.IsCancellationRequested)
        {
            GameServerHeartbeatArguments? arguments = null;
            lock (this._syncRoot)
            {
                if (this._serverInfo is { } serverInfo && this._endPoints.Count > 0)
                {
                    serverInfo.CurrentConnections = this._currentConnections;
                    arguments = new GameServerHeartbeatArguments(serverInfo, this._endPoints.Values.ToList(), this._upTime.Elapsed);
                }
            }

            if (arguments is not null)
            {
                try
                {
                    this._daprClient.PublishEventAsync(PubSubName, "GameServerHeartbeat", arguments, cancellationToken).WaitAndUnwrapException(cancellationToken);
                }
                catch (Exception ex)
                {
                    this._logger.LogDebug(ex, "Error when publishing game server heartbeat");
                }
            }

            cancellationToken.WaitHandle.WaitOne(TimeSpan.FromSeconds(5));
        }
    }

    /// <summary>
    /// The observer for the listener of one endpoint.
    /// </summary>
    private sealed class EndpointObserver : IGameServerStateObserver
    {
        private readonly GameServerStatePublisher _publisher;
        private readonly GameServerEndpoint _endpoint;

        public EndpointObserver(GameServerStatePublisher publisher, GameServerEndpoint endpoint)
        {
            this._publisher = publisher;
            this._endpoint = endpoint;
        }

        public void RegisterGameServer(ServerInfo gameServer, IPEndPoint publicEndPoint)
            => this._publisher.RegisterEndPoint(this._endpoint, gameServer, publicEndPoint);

        public void UnregisterGameServer(ushort serverId)
            => this._publisher.UnregisterEndPoint(this._endpoint);

        public void CurrentConnectionsChanged(ushort serverId, int currentConnections)
            => this._publisher._currentConnections = currentConnections;
    }
}
