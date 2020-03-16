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
    using System.Timers;
    using log4net;
    using MUnique.OpenMU.Interfaces;
    using MUnique.OpenMU.Network;
    using MUnique.OpenMU.Network.PlugIns;
    using MUnique.OpenMU.PlugIns;

    /// <summary>
    /// Chat Server Listener, accepts incoming connections.
    /// </summary>
    public sealed class ChatServer : IChatServer, IDisposable
    {
        private static readonly ILog Log = LogManager.GetLogger(typeof(ChatServer));

        private readonly ChatRoomManager manager;

        private readonly ChatServerSettings settings;

        private readonly RandomNumberGenerator randomNumberGenerator;

        private readonly IList<IChatClient> connectedClients = new List<IChatClient>();

        private readonly IList<ChatServerListener> listeners = new List<ChatServerListener>();

        private readonly Timer clientCleanupTimer;

        private readonly Timer roomCleanupTimer;

        private readonly IIpAddressResolver addressResolver;

        private string publicIp;

        private ServerState serverState;

        /// <summary>
        /// Initializes a new instance of the <see cref="ChatServer" /> class.
        /// </summary>
        /// <param name="settings">The settings.</param>
        /// <param name="addressResolver">The address resolver which returns the address on which the listener will be bound to.</param>
        /// <param name="configurationId">The configuration identifier.</param>
        public ChatServer(ChatServerSettings settings, IIpAddressResolver addressResolver, Guid configurationId)
        {
            this.settings = settings;
            this.addressResolver = addressResolver;
            this.ConfigurationId = configurationId;
            this.manager = new ChatRoomManager();
            this.randomNumberGenerator = RandomNumberGenerator.Create();
            this.clientCleanupTimer = new Timer(this.settings.ClientCleanUpInterval.TotalMilliseconds);
            this.clientCleanupTimer.Elapsed += this.ClientCleanupInactiveClients;
            this.clientCleanupTimer.Start();
            this.roomCleanupTimer = new Timer(this.settings.RoomCleanUpInterval.TotalMilliseconds);
            this.roomCleanupTimer.Elapsed += this.ClientCleanupUnusedRooms;
            this.roomCleanupTimer.Start();
            var plugInManager = new PlugInManager();
            plugInManager.DiscoverAndRegisterPlugInsOf<INetworkEncryptionFactoryPlugIn>();
            foreach (var endpoint in this.settings.Endpoints)
            {
                var listener = new ChatServerListener(endpoint, plugInManager);
                listener.ClientAccepted += this.ChatClientAccepted;
                listener.ClientAccepting += this.ChatClientAccepting;
                this.listeners.Add(listener);
            }
        }

        /// <inheritdoc/>
        public event PropertyChangedEventHandler PropertyChanged;

        /// <inheritdoc/>
        public string Description => this.settings.Description;

        /// <inheritdoc/>
        public int Id => this.settings.ServerId;

        /// <inheritdoc />
        public Guid ConfigurationId { get; }

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

        /// <inheritdoc/>
        public ChatServerAuthenticationInfo RegisterClient(ushort roomId, string clientName)
        {
            var room = this.manager.GetChatRoom(roomId);
            if (room == null)
            {
                var errorMessage = $"RegisterClient: Could not find chat room with id {roomId} for '{clientName}'.";
                Log.Error(errorMessage);
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

        /// <summary>
        /// Starts the listener of this chat server instance.
        /// </summary>
        public void Start()
        {
            Log.Info("Begin starting");
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
                Log.Error("Error while starting", ex);
                this.ServerState = oldState;
            }

            Log.Info("Finished starting");
        }

        /// <inheritdoc/>
        public void Shutdown()
        {
            Log.Info("Begin shutdown");
            this.ServerState = OpenMU.Interfaces.ServerState.Stopping;
            this.clientCleanupTimer.Stop();
            this.roomCleanupTimer.Stop();
            foreach (var listener in this.listeners)
            {
                listener.Stop();
            }

            Log.Debug("Disconnecting all clients");
            var clients = this.connectedClients.ToList();
            foreach (var client in clients)
            {
                client.LogOff();
            }

            this.ServerState = OpenMU.Interfaces.ServerState.Stopped;
            Log.Info("Finished shutdown");
        }

        /// <summary>
        /// Gets the ip address of the server.
        /// </summary>
        /// <returns>The ip address of the server.</returns>
        public string GetIpAddress()
        {
            return this.publicIp ?? (this.publicIp = this.addressResolver.GetIPv4().ToString());
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

        private void ChatClientAccepting(object sender, CancelEventArgs e)
        {
            if (this.settings.MaximumConnections == int.MaxValue)
            {
                return;
            }

            e.Cancel = this.CurrentConnections >= this.settings.MaximumConnections;
        }

        private void ChatClientAccepted(object sender, ClientAcceptEventArgs e)
        {
            var chatClient = new ChatClient(e.AcceptedConnection, this.manager);
            this.connectedClients.Add(chatClient);
            this.RaisePropertyChanged(nameof(this.CurrentConnections));
            chatClient.Disconnected += this.ChatClient_Disconnected;
        }

        private void ChatClient_Disconnected(object sender, EventArgs e)
        {
            this.connectedClients.Remove(sender as IChatClient);
            this.RaisePropertyChanged(nameof(this.CurrentConnections));
        }

        private void ClientCleanupInactiveClients(object sender, ElapsedEventArgs e)
        {
            try
            {
                var bottomDateTimeMargin = DateTime.Now.Subtract(this.settings.ClientTimeout);
                for (int i = this.connectedClients.Count - 1; i >= 0; i--)
                {
                    var client = this.connectedClients[i];
                    if (client.LastActivity < bottomDateTimeMargin)
                    {
                        Log.Debug($"Disconnecting client {client}, because of activity timeout. LastActivity: {client.LastActivity}");
                        client.LogOff();
                    }
                }
            }
            catch (Exception ex)
            {
                Log.Error("Error during checking for inactive clients", ex);
            }
        }

        private void ClientCleanupUnusedRooms(object sender, ElapsedEventArgs e)
        {
            try
            {
                var rooms = this.manager.OpenedRooms.Where(room => room.AuthenticationRequiredUntil < DateTime.Now && room.ConnectedClients.Count < 2).ToList();
                foreach (var room in rooms)
                {
                    Log.Info($"Cleaning up room {room.RoomId}");
                    room.Close();
                }
            }
            catch (Exception ex)
            {
                Log.Error("Error during cleanup of unused rooms", ex);
            }
        }

        /// <summary>
        /// Called when a property changed.
        /// </summary>
        /// <param name="propertyName">Name of the property.</param>
        private void RaisePropertyChanged([CallerMemberName] string propertyName = null)
        {
            this.PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
