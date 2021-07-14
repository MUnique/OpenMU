// <copyright file="ChatServer.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.ChatServer
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel;
    using System.Linq;
    using System.Runtime.CompilerServices;
    using System.Security.Cryptography;
    using System.Threading;
    using System.Threading.Tasks;
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
        private readonly ChatRoomManager manager;
        private readonly ILogger<ChatServer> logger;
        private readonly ILoggerFactory loggerFactory;
        private readonly ChatServerSettings settings;

        private readonly RandomNumberGenerator randomNumberGenerator;

        private readonly IList<IChatClient> connectedClients = new List<IChatClient>();

        private readonly IList<ChatServerListener> listeners = new List<ChatServerListener>();

        private readonly Timer clientCleanupTimer;

        private readonly Timer roomCleanupTimer;

        private readonly IIpAddressResolver addressResolver;

        private string? publicIp;

        private ServerState serverState;

        /// <summary>
        /// Initializes a new instance of the <see cref="ChatServer" /> class.
        /// </summary>
        /// <param name="settings">The settings.</param>
        /// <param name="addressResolver">The address resolver which returns the address on which the listener will be bound to.</param>
        /// <param name="loggerFactory">The logger factory.</param>
        /// <param name="plugInManager">The plug in manager.</param>
        public ChatServer(ChatServerSettings settings, IIpAddressResolver addressResolver, ILoggerFactory loggerFactory, PlugInManager plugInManager)
        {
            this.loggerFactory = loggerFactory;
            this.logger = loggerFactory.CreateLogger<ChatServer>();
            this.settings = settings;
            this.addressResolver = addressResolver;
            this.manager = new ChatRoomManager(loggerFactory);
            this.randomNumberGenerator = RandomNumberGenerator.Create();
            this.clientCleanupTimer = new Timer(this.settings.ClientCleanUpInterval.TotalMilliseconds);
            this.clientCleanupTimer.Elapsed += this.ClientCleanupInactiveClients;
            this.clientCleanupTimer.Start();
            this.roomCleanupTimer = new Timer(this.settings.RoomCleanUpInterval.TotalMilliseconds);
            this.roomCleanupTimer.Elapsed += this.ClientCleanupUnusedRooms;
            this.roomCleanupTimer.Start();
            foreach (var endpoint in this.settings.Endpoints)
            {
                var listener = new ChatServerListener(endpoint, plugInManager, loggerFactory);
                listener.ClientAccepted += this.ChatClientAccepted;
                listener.ClientAccepting += this.ChatClientAccepting;
                this.listeners.Add(listener);
            }
        }

        /// <inheritdoc/>
        public event PropertyChangedEventHandler? PropertyChanged;

        /// <inheritdoc/>
        public string Description => this.settings.Description;

        /// <inheritdoc/>
        public int Id => this.settings.ServerId;

        /// <inheritdoc />
        public Guid ConfigurationId => this.settings.Id;

        /// <inheritdoc />
        public ServerType Type => ServerType.ChatServer;

        /// <inheritdoc/>
        public ServerState ServerState
        {
            get => this.serverState;
            private set
            {
                if (value != this.serverState)
                {
                    this.serverState = value;
                    this.RaisePropertyChanged();
                }
            }
        }

        /// <inheritdoc/>
        public int MaximumConnections => this.settings.MaximumConnections;

        /// <inheritdoc/>
        public int CurrentConnections => this.connectedClients.Count;

        /// <summary>
        /// Gets the ip address of the server.
        /// </summary>
        /// <returns>The ip address of the server.</returns>
        public string IpAddress => this.publicIp ??= this.addressResolver.ResolveIPv4().ToString();

        /// <inheritdoc/>
        public ChatServerAuthenticationInfo RegisterClient(ushort roomId, string clientName)
        {
            var room = this.manager.GetChatRoom(roomId);
            if (room is null)
            {
                var errorMessage = $"RegisterClient: Could not find chat room with id {roomId} for '{clientName}'.";
                this.logger.LogError(errorMessage);
                throw new ArgumentException(errorMessage, nameof(roomId));
            }

            var index = room.GetNextClientIndex();
            var authenticationInfo = new ChatServerAuthenticationInfo(index, roomId, clientName, this.GetRandomAuthenticationToken(index));
            room.RegisterClient(authenticationInfo);
            return authenticationInfo;
        }

        /// <inheritdoc/>
        public ushort CreateChatRoom()
        {
            return this.manager.CreateChatRoom();
        }

        /// <inheritdoc />
        public Task StartAsync(CancellationToken cancellationToken)
        {
            // this.Start();
            return Task.CompletedTask;
        }

        /// <inheritdoc />
        public Task StopAsync(CancellationToken cancellationToken)
        {
            this.Shutdown();
            return Task.CompletedTask;
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

            this.logger.LogInformation("Begin starting");
            var oldState = this.ServerState;
            this.ServerState = OpenMU.Interfaces.ServerState.Starting;
            try
            {
                foreach (var listener in this.listeners)
                {
                    listener.Start();
                }

                this.clientCleanupTimer.Start();
                this.roomCleanupTimer.Start();
                this.ServerState = OpenMU.Interfaces.ServerState.Started;
            }
            catch (Exception ex)
            {
                this.logger.LogError("Error while starting", ex);
                this.ServerState = oldState;
            }

            this.logger.LogInformation("Finished starting");
        }

        /// <inheritdoc/>
        public void Shutdown()
        {
            if (this.ServerState != ServerState.Started)
            {
                return;
            }

            this.logger.LogInformation("Begin shutdown");
            this.ServerState = OpenMU.Interfaces.ServerState.Stopping;
            this.clientCleanupTimer.Stop();
            this.roomCleanupTimer.Stop();
            foreach (var listener in this.listeners)
            {
                listener.Stop();
            }

            this.logger.LogDebug("Disconnecting all clients");
            var clients = this.connectedClients.ToList();
            foreach (var client in clients)
            {
                client.LogOff();
            }

            this.ServerState = OpenMU.Interfaces.ServerState.Stopped;
            this.logger.LogInformation("Finished shutdown");
        }

        /// <summary>
        /// Performs application-defined tasks associated with freeing, releasing, or resetting unmanaged resources.
        /// </summary>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2213:DisposableFieldsShouldBeDisposed", MessageId = "randomNumberGenerator", Justification = "Null-conditional confuses the code analysis.")]
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2213:DisposableFieldsShouldBeDisposed", MessageId = "clientCleanupTimer", Justification = "Null-conditional confuses the code analysis.")]
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2213:DisposableFieldsShouldBeDisposed", MessageId = "roomCleanupTimer", Justification = "Null-conditional confuses the code analysis.")]
        public void Dispose()
        {
            this.randomNumberGenerator?.Dispose();
            this.clientCleanupTimer?.Dispose();
            this.roomCleanupTimer?.Dispose();
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
            this.randomNumberGenerator.GetBytes(authenticationToken, 2, 2);
            var tokenAsString = authenticationToken.MakeDwordBigEndian(0).ToString();
            return tokenAsString;
        }

        private void ChatClientAccepting(object? sender, CancelEventArgs e)
        {
            if (this.settings.MaximumConnections == int.MaxValue)
            {
                return;
            }

            e.Cancel = this.CurrentConnections >= this.settings.MaximumConnections;
        }

        private void ChatClientAccepted(object? sender, ClientAcceptEventArgs e)
        {
            var chatClient = new ChatClient(e.AcceptedConnection, this.manager, this.loggerFactory.CreateLogger<ChatClient>());
            this.connectedClients.Add(chatClient);
            this.RaisePropertyChanged(nameof(this.CurrentConnections));
            chatClient.Disconnected += this.ChatClient_Disconnected;
        }

        private void ChatClient_Disconnected(object? sender, EventArgs e)
        {
            if (sender is IChatClient client)
            {
                this.connectedClients.Remove(client);
            }

            this.RaisePropertyChanged(nameof(this.CurrentConnections));
        }

        private void ClientCleanupInactiveClients(object? sender, ElapsedEventArgs e)
        {
            try
            {
                var bottomDateTimeMargin = DateTime.Now.Subtract(this.settings.ClientTimeout);
                for (int i = this.connectedClients.Count - 1; i >= 0; i--)
                {
                    var client = this.connectedClients[i];
                    if (client.LastActivity < bottomDateTimeMargin)
                    {
                        this.logger.LogDebug($"Disconnecting client {client}, because of activity timeout. LastActivity: {client.LastActivity}");
                        client.LogOff();
                    }
                }
            }
            catch (Exception ex)
            {
                this.logger.LogError(ex, "Error during checking for inactive clients");
            }
        }

        private void ClientCleanupUnusedRooms(object? sender, ElapsedEventArgs e)
        {
            try
            {
                var rooms = this.manager.OpenedRooms.Where(room => room.AuthenticationRequiredUntil < DateTime.Now && room.ConnectedClients.Count < 2).ToList();
                foreach (var room in rooms)
                {
                    this.logger.LogInformation($"Cleaning up room {room.RoomId}");
                    room.Close();
                }
            }
            catch (Exception ex)
            {
                this.logger.LogError(ex, "Error during cleanup of unused rooms");
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
}
