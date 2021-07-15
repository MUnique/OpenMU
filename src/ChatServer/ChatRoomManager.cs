// <copyright file="ChatRoomManager.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.ChatServer
{
    using System;
    using System.Collections.Concurrent;
    using System.Collections.Generic;
    using Microsoft.Extensions.Logging;

    /// <summary>
    /// The Chat Room Manager manages the creation and destruction of chat rooms.
    /// </summary>
    internal class ChatRoomManager
    {
        private readonly ILoggerFactory loggerFactory;

        /// <summary>
        /// All currently used chat rooms.
        /// </summary>
        private readonly IDictionary<ushort, ChatRoom> rooms = new ConcurrentDictionary<ushort, ChatRoom>();

        private readonly ConcurrentBag<ushort> freeRoomIds = new ();

        /// <summary>
        /// Initializes a new instance of the <see cref="ChatRoomManager" /> class.
        /// </summary>
        /// <param name="loggerFactory">The logger factory.</param>
        public ChatRoomManager(ILoggerFactory loggerFactory)
        {
            this.loggerFactory = loggerFactory;
            for (ushort i = 0; i < ushort.MaxValue; ++i)
            {
                this.freeRoomIds.Add(i);
            }
        }

        /// <summary>
        /// Gets the opened rooms.
        /// </summary>
        public ICollection<ChatRoom> OpenedRooms => this.rooms.Values;

        /// <summary>
        /// Creates a new ChatRoom and returns its Room-ID.
        /// </summary>
        /// <returns>The Room-ID of the new room. Returns ushort.MaxValue, if there is no free chat room available.</returns>
        public ushort CreateChatRoom()
        {
            if (this.freeRoomIds.TryTake(out ushort roomId))
            {
                var room = new ChatRoom(roomId, this.loggerFactory.CreateLogger<ChatRoom>());
                room.RoomClosed += this.OnChatRoomClosed;
                this.rooms.Add(roomId, room);
                return roomId;
            }

            throw new InvalidOperationException("There is no free room id, so the chat room couldn't be created.");
        }

        /// <summary>
        /// Returns the chat room with the corresponding Room-ID.
        /// Returns null, if ChatRoom wasn't found.
        /// </summary>
        /// <param name="roomId">Room-ID.</param>
        /// <returns>ChatRoom or null.</returns>
        internal ChatRoom? GetChatRoom(ushort roomId)
        {
            this.rooms.TryGetValue(roomId, out var room);
            return room;
        }

        private void OnChatRoomClosed(object? sender, ChatRoomClosedEventArgs eventArgs)
        {
            var room = eventArgs.ChatRoom;
            this.rooms.Remove(room.RoomId);
            this.freeRoomIds.Add(room.RoomId);
            room.Dispose();
        }
    }
}
