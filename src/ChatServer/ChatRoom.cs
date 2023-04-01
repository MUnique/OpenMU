// <copyright file="ChatRoom.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.ChatServer;

using System.Threading;
using Microsoft.Extensions.Logging;
using MUnique.OpenMU.Interfaces;
using Nito.AsyncEx.Synchronous;

/// <summary>
/// This class represents a Chat Room.
/// </summary>
internal sealed class ChatRoom : IDisposable
{
    private readonly ILogger<ChatRoom> _logger;

    /// <summary>
    /// Nicknames of the registered Clients.
    /// </summary>
    private readonly IList<ChatServerAuthenticationInfo> _registeredClients;

    /// <summary>
    /// Gets the <see cref="IChatClient"/>s which are currently connected to the ChatRoom.
    /// </summary>
    private readonly List<IChatClient> _connectedClients;

    private ReaderWriterLockSlim? _lockSlim = new ();

    private int _lastUsedClientIndex = -1;

    private bool _isClosing;

    /// <summary>
    /// Initializes a new instance of the <see cref="ChatRoom" /> class.
    /// </summary>
    /// <param name="roomId">The room identifier.</param>
    /// <param name="logger">The logger.</param>
    public ChatRoom(ushort roomId, ILogger<ChatRoom> logger)
    {
        this._logger = logger;
        this._logger.LogDebug("Creating room {RoomId}", roomId);
        this._connectedClients = new List<IChatClient>(2);
        this._registeredClients = new List<ChatServerAuthenticationInfo>(2);
        this.RoomId = roomId;
        this.AuthenticationRequiredUntil = DateTime.Now.AddSeconds(10);
    }

    /// <summary>
    /// Gets the currently connected clients.
    /// </summary>
    public IReadOnlyCollection<IChatClient> ConnectedClients => this._connectedClients;

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
        if (this._isClosing)
        {
            throw new ObjectDisposedException("Chat room is already disposed.");
        }

        if (authenticationInfo.RoomId != this.RoomId)
        {
            throw new ArgumentException(
                $"The RoomId of the authentication info ({authenticationInfo.RoomId}) does not match with this RoomId ({this.RoomId}).");
        }

        this.AuthenticationRequiredUntil = authenticationInfo.AuthenticationRequiredUntil;
        this._registeredClients.Add(authenticationInfo);
    }

    /// <summary>
    /// Gets the index of the next client.
    /// </summary>
    /// <returns>The index of the next client.</returns>
    public byte GetNextClientIndex()
    {
        var clientIndex = Interlocked.Increment(ref this._lastUsedClientIndex);
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
        var localLockSlim = this._lockSlim;
        if (this._isClosing || localLockSlim is null)
        {
            return;
        }

        this._isClosing = true;
        this._lockSlim = null;
        this._logger.LogDebug("Disposing room {RoomId}...", this.RoomId);
        this._registeredClients.Clear();
        localLockSlim.EnterWriteLock();
        try
        {
            Task.Run(async () =>
            {
                foreach (var connectedClient in this._connectedClients)
                {
                    await connectedClient.LogOffAsync().ConfigureAwait(false);
                }
            }).WaitAndUnwrapException();
            this._connectedClients.Clear();
            this.RoomClosed?.Invoke(this, new ChatRoomClosedEventArgs(this));
            this.RoomClosed = null;
        }
        finally
        {
            localLockSlim.ExitWriteLock();
        }

        localLockSlim.Dispose();
        this._logger.LogDebug("Room {RoomId} disposed.", this.RoomId);
    }

    /// <summary>
    /// The specified client will join the chatroom, if its registered. The Nickname is set to the clients object.
    /// </summary>
    /// <param name="chatClient">The chat client.</param>
    /// <returns>True, if the <paramref name="chatClient"/> provides the correct registered id with it's token.</returns>
    internal async ValueTask<bool> TryJoinAsync(IChatClient chatClient)
    {
        if (chatClient is null)
        {
            throw new ArgumentNullException(nameof(chatClient));
        }

        if (this._isClosing)
        {
            throw new ObjectDisposedException("Chat room is already disposed.");
        }

        this._logger.LogDebug(
            "Client {ChatClientIndex} is trying to join the room {RoomId} with token '{AuthenticationToken}'",
            chatClient.Index, this.RoomId, chatClient.AuthenticationToken);

        this._lockSlim?.EnterWriteLock();
        try
        {
            var authenticationInformation = this._registeredClients.FirstOrDefault(info => string.Equals(info.AuthenticationToken, chatClient.AuthenticationToken));
            if (authenticationInformation != null)
            {
                if (authenticationInformation.AuthenticationRequiredUntil < DateTime.Now)
                {
                    this._logger.LogInformation(
                        "Client {ChatClientIndex} has tried to join the room {RoomId} with token '{AuthenticationToken}', but was too late. It was valid until {AuthenticationRequiredUntil}.",
                        chatClient.Index, this.RoomId, chatClient.AuthenticationToken, authenticationInformation.AuthenticationRequiredUntil);
                }
                else
                {
                    chatClient.Nickname = authenticationInformation.ClientName;
                    chatClient.Index = authenticationInformation.Index;
                    this._registeredClients.Remove(authenticationInformation);
                    await this.SendChatRoomClientUpdateAsync(chatClient, ChatRoomClientUpdateType.Joined).ConfigureAwait(false);
                    this._connectedClients.Add(chatClient);
                    await chatClient.SendChatRoomClientListAsync(this._connectedClients).ConfigureAwait(false);
                    return true;
                }
            }
            else
            {
                this._logger.LogInformation(
                    "Client {ChatClientIndex} has tried to join the room {RoomId} with token '{AuthenticationToken}', but was not registered.",
                    chatClient.Index, this.RoomId, chatClient.AuthenticationToken);
            }
        }
        finally
        {
            this._lockSlim?.ExitWriteLock();
        }

        return false;
    }

    /// <summary>
    /// The specified client will leave the chatroom.
    /// If the chatroom is empty then, it will be removed from the manager.
    /// </summary>
    /// <param name="chatClient">The chat client.</param>
    internal async ValueTask LeaveAsync(IChatClient chatClient)
    {
        if (this._isClosing)
        {
            return;
        }

        this._logger.LogDebug($"Chat client ({chatClient}) is leaving.");
        this._lockSlim?.EnterWriteLock();
        try
        {
            this._connectedClients.Remove(chatClient);
        }
        finally
        {
            this._lockSlim?.ExitWriteLock();
        }

        bool roomIsEmpty;
        this._lockSlim?.EnterReadLock();
        try
        {
            roomIsEmpty = this._connectedClients.Count < 1;
            if (!roomIsEmpty)
            {
                await this.SendChatRoomClientUpdateAsync(chatClient, ChatRoomClientUpdateType.Left).ConfigureAwait(false);
            }
        }
        finally
        {
            this._lockSlim?.ExitReadLock();
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
    internal async ValueTask SendMessageAsync(byte senderId, string message)
    {
        if (this._isClosing)
        {
            return;
        }

        this._lockSlim?.EnterReadLock();
        try
        {
            foreach (var connectedClient in this._connectedClients)
            {
                await connectedClient.SendMessageAsync(senderId, message).ConfigureAwait(false);
            }
        }
        finally
        {
            this._lockSlim?.ExitReadLock();
        }
    }

    private async ValueTask SendChatRoomClientUpdateAsync(IChatClient updatedClient, ChatRoomClientUpdateType updateType)
    {
        foreach (var client in this._connectedClients)
        {
            await client.SendChatRoomClientUpdateAsync(updatedClient.Index, updatedClient.Nickname ?? string.Empty, updateType).ConfigureAwait(false);
        }
    }
}