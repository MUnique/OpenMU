﻿// <copyright file="DefaultTcpGameServerListener.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.GameServer;

using System.IO.Pipelines;
using System.Net;
using Microsoft.Extensions.Logging;
using MUnique.OpenMU.DataModel.Configuration;
using MUnique.OpenMU.GameLogic;
using MUnique.OpenMU.GameServer.RemoteView;
using MUnique.OpenMU.Interfaces;
using MUnique.OpenMU.Network;
using MUnique.OpenMU.Network.PlugIns;

/// <summary>
/// A game server listener that listens on a TCP port.
/// To be visible in the server list, this listener also registers the game server at the connect server.
/// </summary>
/// <seealso cref="MUnique.OpenMU.GameServer.IGameServerListener" />
public class DefaultTcpGameServerListener : IGameServerListener
{
    private readonly ILogger<DefaultTcpGameServerListener> _logger;

    private readonly GameServerEndpoint _endPoint;
    private readonly IGameServerInfo _gameServerInfo;

    private readonly IGameServerContext _gameContext;

    private readonly IGameServerStateObserver _stateObserver;
    private readonly IIpAddressResolver _addressResolver;
    private readonly ILoggerFactory _loggerFactory;
    private Listener? _listener;

    /// <summary>
    /// Initializes a new instance of the <see cref="DefaultTcpGameServerListener" /> class.
    /// </summary>
    /// <param name="endPoint">The endpoint to which this listener is listening.</param>
    /// <param name="gameServerInfo">The game server information.</param>
    /// <param name="gameContext">The game context.</param>
    /// <param name="stateObserver">The connect server.</param>
    /// <param name="addressResolver">The address resolver which returns the address on which the listener will be bound to.</param>
    /// <param name="loggerFactory">The logger factory.</param>
    public DefaultTcpGameServerListener(GameServerEndpoint endPoint, IGameServerInfo gameServerInfo, IGameServerContext gameContext, IGameServerStateObserver stateObserver, IIpAddressResolver addressResolver, ILoggerFactory loggerFactory)
    {
        this._endPoint = endPoint;
        this._gameServerInfo = gameServerInfo;
        this._gameContext = gameContext;
        this._stateObserver = stateObserver;
        this._addressResolver = addressResolver;
        this._loggerFactory = loggerFactory;
        this._logger = this._loggerFactory.CreateLogger<DefaultTcpGameServerListener>();
    }

    /// <inheritdoc/>
    public event EventHandler<PlayerConnectedEventArgs>? PlayerConnected;

    private INetworkEncryptionFactoryPlugIn? EncryptionFactoryPlugIn { get; set; }

    private ClientVersion ClientVersion => new (this._endPoint.Client!.Season, this._endPoint.Client.Episode, this._endPoint.Client.Language);

    /// <inheritdoc/>
    public void Start()
    {
        if (this._listener is { IsBound: true })
        {
            this._logger.LogDebug("listener is already running.");
            return;
        }

        this.EncryptionFactoryPlugIn = this._gameContext.PlugInManager.GetStrategy<ClientVersion, INetworkEncryptionFactoryPlugIn>(this.ClientVersion)
                                       ?? this._gameContext.PlugInManager.GetStrategy<ClientVersion, INetworkEncryptionFactoryPlugIn>(default);
        if (this.EncryptionFactoryPlugIn is null)
        {
            this.Log(l => l.LogWarning("No network encryption plugin for version {clientVersion} available. It falls back to default encryption.", this.ClientVersion));
        }

        var port = this._endPoint.NetworkPort;
        this._logger.LogInformation("Starting Server Listener, port {port}", port);
        this._listener = new Listener(port, this.CreateDecryptor, this.CreateEncryptor, this._loggerFactory);
        this._listener.ClientAccepted += this.OnClientAccepted;
        this._listener.ClientAccepting += this.OnClientAccepting;
        if (this._endPoint.AlternativePublishedPort > 0)
        {
            port = this._endPoint.AlternativePublishedPort;
            this._logger.LogWarning("GameServer endpoint of port {0} has registered an alternative public port of {1}.", this._endPoint.NetworkPort, port);
        }

        this._stateObserver.RegisterGameServer(this._gameServerInfo, new IPEndPoint(this._addressResolver.ResolveIPv4(), port));
        this._listener.Start();
        this._logger.LogInformation("Server listener started.");
    }

    /// <inheritdoc/>
    public void Stop()
    {
        var port = this._endPoint.NetworkPort;
        this._stateObserver.UnregisterGameServer(this._gameServerInfo);
        this._logger.LogInformation($"Stopping listener on port {port}.");
        if (this._listener is null || !this._listener.IsBound)
        {
            this._logger.LogDebug("listener not running, nothing to shut down.");
            return;
        }

        this._listener.Stop();

        this._logger.LogInformation($"Stopped listener on port {port}.");
        this.EncryptionFactoryPlugIn = null;
    }

    private IPipelinedEncryptor? CreateEncryptor(PipeWriter arg)
    {
        return this.EncryptionFactoryPlugIn is { } plugin
            ? plugin.CreateEncryptor(arg, DataDirection.ServerToClient)
            : new PipelinedEncryptor(arg);
    }

    private IPipelinedDecryptor? CreateDecryptor(PipeReader arg)
    {
        return this.EncryptionFactoryPlugIn is { } plugin
            ? plugin.CreateDecryptor(arg, DataDirection.ClientToServer)
            : new PipelinedDecryptor(arg);
    }

    private void OnClientAccepting(object? sender, ClientAcceptingEventArgs e)
    {
        if (this._gameContext.PlayerCount >= this._gameContext.ServerConfiguration.MaximumPlayers)
        {
            this.Log(l => l.LogDebug($"The server is full... disconnecting the game client {e.AcceptingSocket.RemoteEndPoint}"));

            e.Cancel = true;
        }
    }

    private void OnClientAccepted(object? sender, ClientAcceptedEventArgs e)
    {
        var connection = e.AcceptedConnection;
        var remoteEndPoint = connection.EndPoint as IPEndPoint;
        this.Log(l => l.LogDebug($"Game Client connected, Address {remoteEndPoint}"));

        var remotePlayer = new RemotePlayer(this._gameContext, connection, this.ClientVersion);
        connection.Disconnected += (_, _) => remotePlayer.Disconnect();
        this.OnPlayerConnected(remotePlayer);

        // we don't want to await the call.
        connection.BeginReceive();
    }

    private void OnPlayerConnected(Player player)
    {
        var eventHandler = this.PlayerConnected;
        if (eventHandler != null)
        {
            eventHandler(this, new PlayerConnectedEventArgs(player));
        }
        else
        {
            this.Log(l => l.LogError($"Event {nameof(this.PlayerConnected)} was not handled."));
        }
    }

    private void Log(Action<ILogger<DefaultTcpGameServerListener>> logAction)
    {
        using var contextScope = this._logger.BeginScope(("GameServer", this._gameContext.Id), ("EndPoint", this._endPoint));
        logAction(this._logger);
    }
}