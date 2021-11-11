// <copyright file="ChatRoomManagerTests.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.ChatServer.Tests;

using Microsoft.Extensions.Logging.Abstractions;

/// <summary>
/// Unit tests for the <see cref="ChatRoomManager"/>.
/// </summary>
[TestFixture]
public class ChatRoomManagerTests
{
    /// <summary>
    /// Tests if the returned roomId by <see cref="ChatRoomManager.CreateChatRoom"/> actually returns a roomId with which the room can be retrieved by <see cref="ChatRoomManager.GetChatRoom"/>.
    /// </summary>
    [Test]
    public void RoomCreation()
    {
        var manager = new ChatRoomManager(new NullLoggerFactory());
        var roomId = manager.CreateChatRoom();
        var room = manager.GetChatRoom(roomId);
        Assert.That(room, Is.Not.Null);
    }

    /// <summary>
    /// Tests if recurring calls to <see cref="ChatRoomManager.CreateChatRoom"/> return different room ids.
    /// </summary>
    [Test]
    public void RoomCreationUniqueIds()
    {
        var manager = new ChatRoomManager(new NullLoggerFactory());
        var roomId1 = manager.CreateChatRoom();
        var roomId2 = manager.CreateChatRoom();

        Assert.That(roomId1, Is.Not.EqualTo(roomId2));
    }

    /// <summary>
    /// Tests if a call to <see cref="ChatRoomManager.GetChatRoom"/> with a random room id does not return a room.
    /// </summary>
    [Test]
    public void GetChatRoomNull()
    {
        var manager = new ChatRoomManager(new NullLoggerFactory());
        var room = manager.GetChatRoom(9999);
        Assert.That(room, Is.Null);
    }
}