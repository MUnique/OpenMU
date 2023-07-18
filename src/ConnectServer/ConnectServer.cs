// <copyright file="ConnectServer.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.ConnectServer;

using System.Collections.Concurrent;
using System.ComponentModel;
using System.Net;
using System.Runtime.CompilerServices;
using System.Threading;
using Microsoft.Extensions.Logging;
using MUnique.OpenMU.Interfaces;
using MUnique.OpenMU.Network.PlugIns;

/// <summary>
/// The connect server.
/// </summary>
public class ConnectServer : IConnectServer, OpenMU.Interfaces.IConnectServer
{
    private readonly ILoggerFactory _loggerFactory;
    private readonly ILogger _logger;
    private readonly ServerList _serverList;
    private ServerState _serverState;

    /// <summary>
    /// Initializes a new instance of the <see cref="ConnectServer" /> class.
    /// </summary>
    /// <param name="connectServerSettings">The settings.</param>
    /// <param name="loggerFactory">The logger factory.</param>
    public ConnectServer(IConnectServerSettings connectServerSettings, ILoggerFactory loggerFactory)
    {
        this._loggerFactory = loggerFactory;
        this.Settings = connectServerSettings;
        this.ClientVersion = new ClientVersion(this.Settings.Client.Season, this.Settings.Client.Episode, ClientLanguage.Invariant);
        this.ConfigurationId = this.Settings.ConfigurationId;

        this._logger = this._loggerFactory.CreateLogger<ConnectServer>();

        this.ConnectInfos = new ConcurrentDictionary<ushort, byte[]>();
        this._serverList = new ServerList(this.ClientVersion);

        this.ClientListener = new ClientListener(this, loggerFactory);
        this.ClientListener.ConnectedClientsChanged += (_, _) =>
        {
            this.RaisePropertyChanged(nameof(this.CurrentConnections));
        };
        this.CreatePlugins();
    }

    /// <inheritdoc />
    public event PropertyChangedEventHandler? PropertyChanged;

    /// <inheritdoc/>
    public ServerState ServerState
    {
        get => this._serverState;
        private set
        {
            if (value != this._serverState)
            {
                this._serverState = value;
                this.RaisePropertyChanged();
            }
        }
    }

    /// <inheritdoc />
    public ServerType Type => ServerType.ConnectServer;

    /// <inheritdoc/>
    public string Description => this.Settings.Description;

    /// <inheritdoc/>
    public int Id => SpecialServerIds.ConnectServer + this.Settings.ServerId;

    /// <inheritdoc />
    public Guid ConfigurationId { get; }

    /// <inheritdoc/>
    public ConcurrentDictionary<ushort, byte[]> ConnectInfos { get; }

    /// <inheritdoc/>
    ServerList IConnectServer.ServerList => this._serverList;

    /// <inheritdoc cref="IConnectServer"/>
    public IConnectServerSettings Settings { get; }

    /// <inheritdoc />
    public ClientVersion ClientVersion { get; }

    /// <summary>
    /// Gets the maximum allowed connections.
    /// </summary>
    public int MaximumConnections => this.Settings.MaxConnections;

    /// <summary>
    /// Gets the current connection count.
    /// </summary>
    public int CurrentConnections => this.ClientListener.Clients.Count;

    /// <summary>
    /// Gets the current game server connection count.
    /// </summary>
    public int CurrentGameServerConnections => this._serverList.TotalConnectionCount;

    /// <summary>
    /// Gets the registered game servers.
    /// </summary>
    public IEnumerable<IGameServerEntry> RegisteredGameServers => this._serverList.Items;

    /// <summary>
    /// Gets the client listener.
    /// </summary>
    internal ClientListener ClientListener { get; }

    /// <inheritdoc />
    public async Task StartAsync(CancellationToken cancellationToken)
    {
        await this.StartAsync().ConfigureAwait(false);
    }

    /// <inheritdoc/>
    public async ValueTask StartAsync()
    {
        if (this.ServerState != ServerState.Stopped)
        {
            return;
        }

        this._logger.LogInformation("Begin starting");
        var oldState = this.ServerState;
        this.ServerState = OpenMU.Interfaces.ServerState.Starting;
        try
        {
            this.ClientListener.StartListener();
            this.ServerState = OpenMU.Interfaces.ServerState.Started;
        }
        catch (Exception ex)
        {
            this._logger.LogError(ex, ex.Message);
            this.ServerState = oldState;
        }

        this._logger.LogInformation("Finished starting");
    }

    /// <inheritdoc />
    public async Task StopAsync(CancellationToken cancellationToken)
    {
        await this.ShutdownAsync().ConfigureAwait(false);
    }

    /// <inheritdoc/>
    public async ValueTask ShutdownAsync()
    {
        this._logger.LogInformation("Begin stopping");
        this.ServerState = OpenMU.Interfaces.ServerState.Stopping;
        this.ClientListener.StopListener();
        this.ServerState = OpenMU.Interfaces.ServerState.Stopped;
        this._logger.LogInformation("Finished stopping");
    }

    /// <inheritdoc/>
    public void RegisterGameServer(ServerInfo gameServer, IPEndPoint publicEndPoint)
    {
        this._logger.LogInformation("GameServer {0} is registering with endpoint {1}", gameServer, publicEndPoint);
        try
        {
            if (this.ConnectInfos.ContainsKey(gameServer.Id))
            {
                this._logger.LogInformation("GameServer {0} was already registered and needs to be removed before...", gameServer);
                this.UnregisterGameServer(gameServer.Id);
            }

            var serverListItem = new ServerListItem(this._serverList)
            {
                ServerId = gameServer.Id,
                EndPoint = publicEndPoint,
                MaximumConnections = gameServer.MaximumConnections,
                CurrentConnections = gameServer.CurrentConnections,
            };

            if (this.ConnectInfos.TryAdd(serverListItem.ServerId, serverListItem.ConnectInfo))
            {
                this._serverList.Add(serverListItem);
            }
        }
        catch (Exception ex)
        {
            this._logger.LogError(ex, "Error during registration process");
            throw;
        }

        this._logger.LogInformation("GameServer {0} has registered with endpoint {1}", gameServer, publicEndPoint);
    }

    /// <inheritdoc/>
    public void UnregisterGameServer(ushort gameServerId)
    {
        this._logger.LogInformation("GameServer {0} is unregistering", gameServerId);
        var serverListItem = this._serverList.GetItem(gameServerId);
        if (serverListItem != null)
        {
            this.ConnectInfos.Remove(serverListItem.ServerId, out _);
            this._serverList.Remove(serverListItem);
        }

        this._logger.LogInformation("GameServer {0} has unregistered", gameServerId);
    }

    /// <inheritdoc />
    public void CurrentConnectionsChanged(ushort serverId, int currentConnections)
    {
        var serverListItem = this._serverList.GetItem(serverId);
        if (serverListItem is null)
        {
            return;
        }

        serverListItem.CurrentConnections = currentConnections;
    }

    private void CreatePlugins()
    {
        this._logger.LogDebug("Begin creating plugins");
        this.ClientListener.ClientSocketAcceptPlugins.Add(new CheckMaximumConnectionsPlugin(this, this._loggerFactory.CreateLogger<CheckMaximumConnectionsPlugin>()));
        var clientCountPlugin = new ClientConnectionCountPlugin(this.Settings, this._loggerFactory.CreateLogger<ClientConnectionCountPlugin>());
        this.ClientListener.ClientSocketAcceptPlugins.Add(clientCountPlugin);
        this.ClientListener.ClientSocketDisconnectPlugins.Add(clientCountPlugin);
        this._logger.LogDebug("Finished creating plugins");
    }

    /// <summary>
    /// Called when a property changed.
    /// </summary>
    /// <param name="propertyName">Name of the property.</param>
    private void RaisePropertyChanged([CallerMemberName] string? propertyName = null)
    {
        this.PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
    }
}