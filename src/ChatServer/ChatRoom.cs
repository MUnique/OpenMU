// <copyright file="ChatRoom.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.ChatServer
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading;
    using Microsoft.Extensions.Logging;
    using MUnique.OpenMU.Interfaces;

    /// <summary>
    /// This class represents a Chat Room.
    /// </summary>
    internal sealed class ChatRoom : IDisposable
    {
        private readonly ILogger<ChatRoom> logger;

        /// <summary>
        /// Nicknames of the registered Clients.
        /// </summary>
        private readonly IList<ChatServerAuthenticationInfo> registeredClients;

        /// <summary>
        /// Gets the <see cref="IChatClient"/>s which are currently connected to the ChatRoom.
        /// </summary>
        private readonly List<IChatClient> connectedClients;

        private ReaderWriterLockSlim? lockSlim = new ();

        private int lastUsedClientIndex = -1;

        private bool isClosing;

        /// <summary>
        /// Initializes a new instance of the <see cref="ChatRoom" /> class.
        /// </summary>
        /// <param name="roomId">The room identifier.</param>
        /// <param name="logger">The logger.</param>
        public ChatRoom(ushort roomId, ILogger<ChatRoom> logger)
        {
            this.logger = logger;
            this.logger.LogDebug($"Creating room {roomId}");
            this.connectedClients = new List<IChatClient>(2);
            this.registeredClients = new List<ChatServerAuthenticationInfo>(2);
            this.RoomId = roomId;
            this.AuthenticationRequiredUntil = DateTime.Now.AddSeconds(10);
        }

        /// <summary>
        /// Gets the currently connected clients.
        /// </summary>
        public IReadOnlyCollection<IChatClient> ConnectedClients => this.connectedClients;

        /// <summary>
        /// Gets the id of the Chat Room.
        /// </summary>
        public ushort RoomId { get; }

        /// <summary>
        /// Gets a datetime indicating until a authentication is required.
        /// </summary>
        public DateTime AuthenticationRequiredUntil { get; private set; }

        /// <summary>
        /// Gets or sets the room closed event handler.
        /// </summary>
        public EventHandler<ChatRoomClosedEventArgs>? RoomClosed { get; set; }

        /// <summary>
        /// Registers a chat client to the chatroom. this is only called
        /// by the game server which will send id to the participants
        /// over the games connection.
        /// </summary>
        /// <param name="authenticationInfo">Authentication information of the participant.</param>
        public void RegisterClient(ChatServerAuthenticationInfo authenticationInfo)
        {
            if (this.isClosing)
            {
                throw new ObjectDisposedException("Chat room is already disposed.");
            }

            if (authenticationInfo.RoomId != this.RoomId)
            {
                throw new ArgumentException(
                    $"The RoomId of the authentication info ({authenticationInfo.RoomId}) does not match with this RoomId ({this.RoomId}).");
            }

            this.AuthenticationRequiredUntil = authenticationInfo.AuthenticationRequiredUntil;
            this.registeredClients.Add(authenticationInfo);
        }

        /// <summary>
        /// Gets the index of the next client.
        /// </summary>
        /// <returns>The index of the next client.</returns>
        public byte GetNextClientIndex()
        {
            var clientIndex = Interlocked.Increment(ref this.lastUsedClientIndex);
            return (byte)clientIndex;
        }

        /// <summary>
        /// Closes this chat room by disconnecting all clients.
        /// </summary>
        public void Close()
        {
           this.Dispose();
        }

        /// <summary>
        /// Performs application-defined tasks associated with freeing, releasing, or resetting unmanaged resources.
        /// </summary>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2213:DisposableFieldsShouldBeDisposed", MessageId = "lockSlim", Justification = "Null-conditional confuses the code analysis.")]
        public void Dispose()
        {
            var localLockSlim = this.lockSlim;
            if (this.isClosing || localLockSlim is null)
            {
                return;
            }

            this.isClosing = true;
            this.lockSlim = null;
            this.logger.LogDebug($"Disposing room {this.RoomId}...");
            this.registeredClients.Clear();
            localLockSlim.EnterWriteLock();
            try
            {
                foreach (var client in this.connectedClients)
                {
                    client.LogOff();
                }

                this.connectedClients.Clear();
                this.RoomClosed?.Invoke(this, new ChatRoomClosedEventArgs(this));
                this.RoomClosed = null;
            }
            finally
            {
                localLockSlim.ExitWriteLock();
            }

            localLockSlim.Dispose();
            this.logger.LogDebug($"Room {this.RoomId} disposed.");
        }

        /// <summary>
        /// The specified client will join the chatroom, if its registered. The Nickname is set to the clients object.
        /// </summary>
        /// <param name="chatClient">The chat client.</param>
        /// <returns>True, if the <paramref name="chatClient"/> provides the correct registered id with it's token.</returns>
        internal bool TryJoin(IChatClient chatClient)
        {
            if (chatClient is null)
            {
                throw new ArgumentNullException(nameof(chatClient));
            }

            if (this.isClosing)
            {
                throw new ObjectDisposedException("Chat room is already disposed.");
            }

            this.logger.LogDebug($"Client {chatClient.Index} is trying to join the room {this.RoomId} with token '{chatClient.AuthenticationToken}'");

            this.lockSlim?.EnterWriteLock();
            try
            {
                var authenticationInformation = this.registeredClients.FirstOrDefault(info => string.Equals(info.AuthenticationToken, chatClient.AuthenticationToken));
                if (authenticationInformation != null)
                {
                    if (authenticationInformation.AuthenticationRequiredUntil < DateTime.Now)
                    {
                        this.logger.LogInformation($"Client {chatClient.Index} has tried to join the room {this.RoomId} with token '{chatClient.AuthenticationToken}', but was too late. It was valid until {authenticationInformation.AuthenticationRequiredUntil}.");
                    }
                    else
                    {
                        chatClient.Nickname = authenticationInformation.ClientName;
                        chatClient.Index = authenticationInformation.Index;
                        this.registeredClients.Remove(authenticationInformation);
                        this.SendChatRoomClientUpdate(chatClient, ChatRoomClientUpdateType.Joined);
                        this.connectedClients.Add(chatClient);
                        chatClient.SendChatRoomClientList(this.connectedClients);
                        return true;
                    }
                }
                else
                {
                    this.logger.LogInformation($"Client {chatClient.Index} has tried to join the room {this.RoomId} with token '{chatClient.AuthenticationToken}', but was not registered.");
                }
            }
            finally
            {
                this.lockSlim?.ExitWriteLock();
            }

            return false;
        }

        /// <summary>
        /// The specified client will leave the chatroom.
        /// If the chatroom is empty then, it will be removed from the manager.
        /// </summary>
        /// <param name="chatClient">The chat client.</param>
        internal void Leave(IChatClient chatClient)
        {
            if (this.isClosing)
            {
                return;
            }

            this.logger.LogDebug($"Chat client ({chatClient}) is leaving.");
            this.lockSlim?.EnterWriteLock();
            try
            {
                this.connectedClients.Remove(chatClient);
            }
            finally
            {
                this.lockSlim?.ExitWriteLock();
            }

            bool roomIsEmpty;
            this.lockSlim?.EnterReadLock();
            try
            {
                roomIsEmpty = this.connectedClients.Count < 1;
                if (!roomIsEmpty)
                {
                    this.SendChatRoomClientUpdate(chatClient, ChatRoomClientUpdateType.Left);
                }
            }
            finally
            {
                this.lockSlim?.ExitReadLock();
            }

            if (roomIsEmpty)
            {
                this.Close();
            }
        }

        /// <summary>
        /// Sends a Message to all chat clients.
        /// </summary>
        /// <param name="senderId">The sender identifier.</param>
        /// <param name="message">The message.</param>
        internal void SendMessage(byte senderId, string message)
        {
            if (this.isClosing)
            {
                return;
            }

            this.lockSlim?.EnterReadLock();
            try
            {
                this.connectedClients.ForEach(c => c.SendMessage(senderId, message));
            }
            finally
            {
                this.lockSlim?.ExitReadLock();
            }
        }

        private void SendChatRoomClientUpdate(IChatClient updatedClient, ChatRoomClientUpdateType updateType)
        {
            foreach (var client in this.connectedClients)
            {
                client.SendChatRoomClientUpdate(updatedClient.Index, updatedClient.Nickname ?? string.Empty, updateType);
            }
        }
    }
}
