// <copyright file="ClientListener.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.ConnectServer;

using System.Net;
using Microsoft.Extensions.Logging;
using MUnique.OpenMU.ConnectServer.PacketHandler;
using MUnique.OpenMU.Interfaces;
using MUnique.OpenMU.Network;

/// <summary>
/// The listener which is waiting for new connecting clients.
/// </summary>
internal class ClientListener
{
    private readonly ILoggerFactory _loggerFactory;
    private readonly ILogger<ClientListener> _logger;
    private readonly object _clientListLock = new ();
    private readonly IConnectServerSettings _connectServerSettings;
    private readonly IPacketHandler<Client> _packetHandler;
    private Listener? _listener;

    /// <summary>
    /// Initializes a new instance of the <see cref="ClientListener" /> class.
    /// </summary>
    /// <param name="connectServer">The connect server.</param>
    /// <param name="loggerFactory">The logger factory.</param>
    public ClientListener(IConnectServer connectServer, ILoggerFactory loggerFactory)
    {
        this._loggerFactory = loggerFactory;
        this._connectServerSettings = connectServer.Settings;
        this._logger = this._loggerFactory.CreateLogger<ClientListener>();
        this._packetHandler = new ClientPacketHandler(connectServer, loggerFactory);
        this.Clients = new List<Client>();
        this.ClientSocketAcceptPlugins = new List<IAfterSocketAcceptPlugin>();
        this.ClientSocketDisconnectPlugins = new List<IAfterDisconnectPlugin>();
    }

    /// <summary>
    /// Occurs when the number of connected clients changed.
    /// </summary>
    public event EventHandler? ConnectedClientsChanged;

    /// <summary>
    /// Gets the connected clients.
    /// </summary>
    public ICollection<Client> Clients { get; }

    /// <summary>
    /// Gets the client socket accept plugins.
    /// </summary>
    public IList<IAfterSocketAcceptPlugin> ClientSocketAcceptPlugins { get; }

    /// <summary>
    /// Gets the client socket disconnect plugins.
    /// </summary>
    public ICollection<IAfterDisconnectPlugin> ClientSocketDisconnectPlugins { get; }

    /// <summary>
    /// Starts the listener.
    /// </summary>
    public void StartListener()
    {
        this._listener = new Listener(this._connectServerSettings.ClientListenerPort, null, null, this._loggerFactory);
        this._listener.ClientAccepting += this.OnClientAccepting;
        this._listener.ClientAccepted += this.OnClientAccepted;
        this._listener.Start(this._connectServerSettings.ListenerBacklog);

        this._logger.LogInformation("Client Listener started, Port {0}", this._connectServerSettings.ClientListenerPort);
    }

    /// <summary>
    /// Stops the listener.
    /// </summary>
    public void StopListener()
    {
        this._listener?.Stop();
        this._logger.LogInformation("Client Listener stopped");
    }

    private void OnClientAccepting(object? sender, ClientAcceptingEventArgs e)
    {
        for (var i = 0; i < this.ClientSocketAcceptPlugins.Count; ++i)
        {
            var plugin = this.ClientSocketAcceptPlugins[i];
            if (!plugin.OnAfterSocketAccept(e.AcceptingSocket))
            {
                e.Cancel = true;
                return;
            }
        }
    }

    private void OnClientAccepted(object? sender, ClientAcceptedEventArgs e)
    {
        var connection = e.AcceptedConnection;
        var client = new Client(connection, this._connectServerSettings.Timeout, this._packetHandler, this._connectServerSettings.MaximumReceiveSize, this._loggerFactory.CreateLogger<Client>());
        var ipEndpoint = connection.EndPoint as IPEndPoint;
        client.Address = ipEndpoint?.Address ?? IPAddress.None;
        client.Port = ipEndpoint?.Port ?? 0;
        client.Timeout = this._connectServerSettings.Timeout;

        lock (this._clientListLock)
        {
            this.Clients.Add(client);
        }

        client.Connection.Disconnected += (sender, e) => this.OnClientDisconnect(client);
        this._logger.LogDebug("Client connected: {0}, current client count: {1}", connection.EndPoint, this.Clients.Count);
        client.SendHello();
        client.Connection.BeginReceive();
        this.ConnectedClientsChanged?.Invoke(this, EventArgs.Empty);
    }

    private void OnClientDisconnect(Client client)
    {
        foreach (var plugin in this.ClientSocketDisconnectPlugins)
        {
            plugin.OnAfterDisconnect(client);
        }

        this._logger.LogDebug("Connection to Client {0}:{1} disconnected.", client.Address, client.Port);
        lock (this._clientListLock)
        {
            this.Clients.Remove(client);
        }

        this.ConnectedClientsChanged?.Invoke(this, EventArgs.Empty);
    }
}