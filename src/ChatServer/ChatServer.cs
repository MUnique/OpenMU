// <copyright file="ChatServer.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.ChatServer;

using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Security.Cryptography;
using System.Threading;
using System.Timers;
using Microsoft.Extensions.Logging;
using MUnique.OpenMU.Interfaces;
using MUnique.OpenMU.Network;
using MUnique.OpenMU.PlugIns;
using Timer = System.Timers.Timer;

/// <summary>
/// Chat Server Listener, accepts incoming connections.
/// </summary>
public sealed class ChatServer : IChatServer, IDisposable
{
    private readonly ChatRoomManager _manager;
    private readonly ILogger<ChatServer> _logger;
    private readonly ILoggerFactory _loggerFactory;
    private readonly PlugInManager _plugInManager;
    private readonly IIpAddressResolver _addressResolver;

    private readonly RandomNumberGenerator _randomNumberGenerator;

    private readonly IList<IChatClient> _connectedClients = new List<IChatClient>();

    private readonly IList<ChatServerListener> _listeners = new List<ChatServerListener>();

    private Timer? _clientCleanupTimer;
    private Timer? _roomCleanupTimer;

    private ChatServerSettings? _settings;

    private string? _publicIp;

    private bool _isDisposed;

    private ServerState _serverState;

    /// <summary>
    /// Initializes a new instance of the <see cref="ChatServer" /> class.
    /// </summary>
    /// <param name="addressResolver">The address resolver which returns the address on which the listener will be bound to.</param>
    /// <param name="loggerFactory">The logger factory.</param>
    /// <param name="plugInManager">The plug in manager.</param>
    public ChatServer(IIpAddressResolver addressResolver, ILoggerFactory loggerFactory, PlugInManager plugInManager)
    {
        this._loggerFactory = loggerFactory;
        this._plugInManager = plugInManager;
        this._logger = loggerFactory.CreateLogger<ChatServer>();
        this._addressResolver = addressResolver;
        this._manager = new ChatRoomManager(loggerFactory);
        this._randomNumberGenerator = RandomNumberGenerator.Create();
    }

    /// <inheritdoc/>
    public event PropertyChangedEventHandler? PropertyChanged;

    /// <inheritdoc/>
    public string Description => this._settings?.Description ?? string.Empty;

    /// <inheritdoc/>
    public int Id => this.Settings?.ServerId ?? SpecialServerIds.ChatServer;

    /// <inheritdoc />
    public Guid ConfigurationId => this._settings?.Id ?? Guid.Empty;

    /// <inheritdoc />
    public ServerType Type => ServerType.ChatServer;

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

    /// <inheritdoc/>
    public int MaximumConnections => this.Settings.MaximumConnections;

    /// <inheritdoc/>
    public int CurrentConnections => this._connectedClients.Count;

    /// <summary>
    /// Gets the ip address of the server.
    /// </summary>
    /// <returns>The ip address of the server.</returns>
    public string IpAddress => this._publicIp ??= this._addressResolver.ResolveIPv4().ToString();

    private ChatServerSettings Settings => this._settings ?? throw new InvalidOperationException("The server was not initialized before");

    /// <inheritdoc/>
    public ChatServerAuthenticationInfo RegisterClient(ushort roomId, string clientName)
    {
        var room = this._manager.GetChatRoom(roomId);
        if (room is null)
        {
            var errorMessage = $"RegisterClient: Could not find chat room with id {roomId} for '{clientName}'.";
            this._logger.LogError(errorMessage);
            throw new ArgumentException(errorMessage, nameof(roomId));
        }

        var index = room.GetNextClientIndex();
        var authenticationInfo = new ChatServerAuthenticationInfo(index, roomId, clientName, this.IpAddress, this.GetRandomAuthenticationToken(index));
        room.RegisterClient(authenticationInfo);
        return authenticationInfo;
    }

    /// <inheritdoc/>
    public ushort CreateChatRoom()
    {
        return this._manager.CreateChatRoom();
    }

    /// <inheritdoc />
    public Task StartAsync(CancellationToken cancellationToken)
    {
        this.Start();
        return Task.CompletedTask;
    }

    /// <inheritdoc />
    public Task StopAsync(CancellationToken cancellationToken)
    {
        this.Shutdown();
        return Task.CompletedTask;
    }

    /// <summary>
    /// Initializes the server with the specified settings.
    /// </summary>
    /// <param name="settings">The settings.</param>
    /// <exception cref="System.InvalidOperationException">Can only initialize when server is stopped.</exception>
    public void Initialize(ChatServerSettings settings)
    {
        if (this.ServerState != ServerState.Stopped)
        {
            throw new InvalidOperationException("Can only initialize when server is stopped.");
        }

        this._settings = settings;
    }

    private void CreateCleanupTimers()
    {
        this._clientCleanupTimer = new Timer(this.Settings.ClientCleanUpInterval.TotalMilliseconds);
        this._clientCleanupTimer.Elapsed += this.ClientCleanupInactiveClients;
        this._clientCleanupTimer.Start();
        this._roomCleanupTimer = new Timer(this.Settings.RoomCleanUpInterval.TotalMilliseconds);
        this._roomCleanupTimer.Elapsed += this.ClientCleanupUnusedRooms;
        this._roomCleanupTimer.Start();
    }

    private void RemoveCleanupTimers()
    {
        this._clientCleanupTimer?.Stop();
        this._clientCleanupTimer?.Dispose();
        this._clientCleanupTimer = null;

        this._roomCleanupTimer?.Stop();
        this._roomCleanupTimer?.Dispose();
        this._roomCleanupTimer = null;
    }

    private void CreateListeners()
    {
        foreach (var endpoint in this.Settings.Endpoints)
        {
            var listener = new ChatServerListener(endpoint, this._plugInManager, this._loggerFactory);
            listener.ClientAccepted += this.ChatClientAccepted;
            listener.ClientAccepting += this.ChatClientAccepting;
            this._listeners.Add(listener);
        }
    }

    /// <summary>
    /// Starts the listener of this chat server instance.
    /// </summary>
    public void Start()
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
            this.CreateListeners();
            foreach (var listener in this._listeners)
            {
                listener.Start();
            }

            this.CreateCleanupTimers();

            this.ServerState = OpenMU.Interfaces.ServerState.Started;
        }
        catch (Exception ex)
        {
            this._logger.LogError(ex, "Error while starting");
            this.ServerState = oldState;
        }

        this._logger.LogInformation("Finished starting");
    }

    /// <inheritdoc/>
    public void Shutdown()
    {
        if (this.ServerState != ServerState.Started)
        {
            return;
        }

        this._logger.LogInformation("Begin shutdown");
        this.ServerState = OpenMU.Interfaces.ServerState.Stopping;
        this.RemoveCleanupTimers(); 
        foreach (var listener in this._listeners)
        {
            listener.Stop();
        }

        this._listeners.Clear();

        this._logger.LogDebug("Disconnecting all clients");
        var clients = this._connectedClients.ToList();
        foreach (var client in clients)
        {
            client.LogOff();
        }

        this.ServerState = OpenMU.Interfaces.ServerState.Stopped;
        this._logger.LogInformation("Finished shutdown");
    }

    /// <summary>
    /// Performs application-defined tasks associated with freeing, releasing, or resetting unmanaged resources.
    /// </summary>
    public void Dispose()
    {
        if (!this._isDisposed)
        {
            this._isDisposed = true;
            this._randomNumberGenerator.Dispose();
            this._clientCleanupTimer?.Dispose();
            this._roomCleanupTimer?.Dispose();
        }
    }

    /// <summary>
    /// Gets a random authentication token.
    /// </summary>
    /// <param name="clientIndex">Index of the client.</param>
    /// <returns>The random authentication token as string.</returns>
    /// <remarks>
    ///  This is the original way of generating the token - not especially secure, but to keep it simple, I leave it that way.
    /// </remarks>
    private string GetRandomAuthenticationToken(byte clientIndex)
    {
        var authenticationToken = new byte[] { clientIndex, 0, 0, 0 };
        this._randomNumberGenerator.GetBytes(authenticationToken, 2, 2);
        var tokenAsString = authenticationToken.MakeDwordBigEndian(0).ToString();
        return tokenAsString;
    }

    private void ChatClientAccepting(object? sender, CancelEventArgs e)
    {
        if (this.Settings.MaximumConnections == int.MaxValue)
        {
            return;
        }

        e.Cancel = this.CurrentConnections >= this.Settings.MaximumConnections;
    }

    private void ChatClientAccepted(object? sender, ClientAcceptedEventArgs e)
    {
        var chatClient = new ChatClient(e.AcceptedConnection, this._manager, this._loggerFactory.CreateLogger<ChatClient>());
        this._connectedClients.Add(chatClient);
        this.RaisePropertyChanged(nameof(this.CurrentConnections));
        chatClient.Disconnected += this.ChatClient_Disconnected;
    }

    private void ChatClient_Disconnected(object? sender, EventArgs e)
    {
        if (sender is IChatClient client)
        {
            this._connectedClients.Remove(client);
        }

        this.RaisePropertyChanged(nameof(this.CurrentConnections));
    }

    private void ClientCleanupInactiveClients(object? sender, ElapsedEventArgs e)
    {
        try
        {
            var bottomDateTimeMargin = DateTime.Now.Subtract(this.Settings.ClientTimeout);
            for (int i = this._connectedClients.Count - 1; i >= 0; i--)
            {
                var client = this._connectedClients[i];
                if (client.LastActivity < bottomDateTimeMargin)
                {
                    this._logger.LogDebug($"Disconnecting client {client}, because of activity timeout. LastActivity: {client.LastActivity}");
                    client.LogOff();
                }
            }
        }
        catch (Exception ex)
        {
            this._logger.LogError(ex, "Error during checking for inactive clients");
        }
    }

    private void ClientCleanupUnusedRooms(object? sender, ElapsedEventArgs e)
    {
        try
        {
            var rooms = this._manager.OpenedRooms.Where(room => room.AuthenticationRequiredUntil < DateTime.Now && room.ConnectedClients.Count < 2).ToList();
            foreach (var room in rooms)
            {
                this._logger.LogInformation($"Cleaning up room {room.RoomId}");
                room.Close();
            }
        }
        catch (Exception ex)
        {
            this._logger.LogError(ex, "Error during cleanup of unused rooms");
        }
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