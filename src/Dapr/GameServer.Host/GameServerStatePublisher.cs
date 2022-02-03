using System.Diagnostics;

namespace MUnique.OpenMU.GameServer.Host;

using System.Net;
using System.Threading;
using global::Dapr.Client;
using Microsoft.Extensions.Logging;
using MUnique.OpenMU.Interfaces;
using MUnique.OpenMU.ServerClients;
using Nito.AsyncEx.Synchronous;

public sealed class GameServerStatePublisher : IGameServerStateObserver, IDisposable
{
    private const string PubSubName = "pubsub";
    private readonly DaprClient _daprClient;
    private readonly ILogger<GameServerStatePublisher> _logger;

    private int _currentConnections;
    private ServerInfo? _serverInfo;
    private IPEndPoint? _publicEndPoint;

    private CancellationTokenSource? _heartbeatCancellationTokenSource;
    private Thread? _heartbeatThread;
    

    public GameServerStatePublisher(DaprClient daprClient, ILogger<GameServerStatePublisher> logger)
    {
        _daprClient = daprClient;
        _logger = logger;
    }

    public void Dispose()
    {
        this._heartbeatCancellationTokenSource?.Cancel();
        this._heartbeatCancellationTokenSource?.Dispose();
        this._heartbeatThread = null;
    }

    public void RegisterGameServer(ServerInfo serverInfo, IPEndPoint publicEndPoint)
    {
        this._heartbeatCancellationTokenSource?.Cancel(false);

        this._serverInfo = serverInfo;
        this._publicEndPoint = publicEndPoint;
        this._heartbeatCancellationTokenSource = new ();
        try
        {
            this._logger.LogInformation("Starting heartbeat thread ...");
            this._heartbeatThread = new (
                () =>
                {
                    try
                    {
                        this.HeartbeatLoop(this._heartbeatCancellationTokenSource.Token);
                    }
                    catch (Exception ex)
                    {
                        this._logger.LogError(ex, "Error in heartbeat loop.");
                    }
                })
            {
                Name = "Heartbeat"
            };
            this._heartbeatThread.Start();

            this._logger.LogInformation("...started heartbeat thread.");
        }
        catch (Exception ex)
        {
            this._logger.LogError(ex, "Unexpected error when publishing the game server registration.");
        }
    }

    public void UnregisterGameServer(ushort serverId)
    {
        this._logger.LogInformation("Stopping heartbeat thread");
        this._heartbeatCancellationTokenSource?.Cancel();
        this._heartbeatThread = null;
    }

    public void CurrentConnectionsChanged(ushort serverId, int currentConnections)
    {
        this._currentConnections = currentConnections;
    }

    private void HeartbeatLoop(CancellationToken cancellationToken)
    {
        if (this._serverInfo is not { } serverInfo
            || this._publicEndPoint is not { } publicEndPoint)
        {
            return;
        }

        var stopWatch = new Stopwatch();
        stopWatch.Start();
        var publicEndPointString = publicEndPoint.ToString();
        var arguments = new GameServerHeartbeatArguments(serverInfo, publicEndPointString, stopWatch.Elapsed);
        
        while (!cancellationToken.IsCancellationRequested)
        {
            serverInfo.CurrentConnections = this._currentConnections;
            arguments.UpTime = stopWatch.Elapsed;

            try
            {
                this._daprClient.PublishEventAsync(PubSubName, "GameServerHeartbeat", arguments, cancellationToken).WaitAndUnwrapException(cancellationToken);
            }
            catch (Exception ex)
            {
                this._logger.LogDebug(ex, "Error when publishing game server heartbeat");   
            }

            Thread.Sleep(5000);
        }
    }
}