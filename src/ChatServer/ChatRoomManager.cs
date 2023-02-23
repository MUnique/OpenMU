// <copyright file="ChatRoomManager.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.ChatServer;

using System.Collections.Concurrent;
using Microsoft.Extensions.Logging;

/// <summary>
/// The Chat Room Manager manages the creation and destruction of chat rooms.
/// </summary>
internal class ChatRoomManager
{
    private readonly ILoggerFactory _loggerFactory;

    /// <summary>
    /// All currently used chat rooms.
    /// </summary>
    private readonly IDictionary<ushort, ChatRoom> _rooms = new ConcurrentDictionary<ushort, ChatRoom>();

    private readonly ConcurrentBag<ushort> _freeRoomIds = new ();

    /// <summary>
    /// Initializes a new instance of the <see cref="ChatRoomManager" /> class.
    /// </summary>
    /// <param name="loggerFactory">The logger factory.</param>
    public ChatRoomManager(ILoggerFactory loggerFactory)
    {
        this._loggerFactory = loggerFactory;
        for (ushort i = 0; i < ushort.MaxValue; ++i)
        {
            this._freeRoomIds.Add(i);
        }
    }

    /// <summary>
    /// Gets the opened rooms.
    /// </summary>
    public ICollection<ChatRoom> OpenedRooms => this._rooms.Values;

    /// <summary>
    /// Creates a new ChatRoom and returns its Room-ID.
    /// </summary>
    /// <returns>The Room-ID of the new room. Returns ushort.MaxValue, if there is no free chat room available.</returns>
    public ushort CreateChatRoom()
    {
        if (!this._freeRoomIds.TryTake(out ushort roomId))
        {
            throw new InvalidOperationException("There is no free room id, so the chat room couldn't be created.");
        }

        var room = new ChatRoom(roomId, this._loggerFactory.CreateLogger<ChatRoom>());
        room.RoomClosed += this.OnChatRoomClosed;
        this._rooms.Add(roomId, room);
        return roomId;
    }

    /// <summary>
    /// Returns the chat room with the corresponding Room-ID.
    /// Returns null, if ChatRoom wasn't found.
    /// </summary>
    /// <param name="roomId">Room-ID.</param>
    /// <returns>ChatRoom or null.</returns>
    internal ChatRoom? GetChatRoom(ushort roomId)
    {
        this._rooms.TryGetValue(roomId, out var room);
        return room;
    }

    private void OnChatRoomClosed(object? sender, ChatRoomClosedEventArgs eventArgs)
    {
        var room = eventArgs.ChatRoom;
        this._rooms.Remove(room.RoomId);
        this._freeRoomIds.Add(room.RoomId);
        room.Dispose();
    }
}