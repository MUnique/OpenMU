// <copyright file="ChatClientTests.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.ChatServer.Tests;

using System.Buffers;
using Microsoft.Extensions.Logging.Abstractions;
using MUnique.OpenMU.Interfaces;
using MUnique.OpenMU.Network;

/// <summary>
/// Unit tests for the <see cref="ChatClient"/>.
/// </summary>
[TestFixture]
public class ChatClientTests
{
    private const string ChatServerHost = "";

    /// <summary>
    /// Tests if the client joined the room when the authentication was successful.
    /// </summary>
    /// <returns>The async task.</returns>
    [Test]
    public async Task AuthenticationSuccessAsync()
    {
        var manager = new ChatRoomManager(new NullLoggerFactory());
        var roomId = manager.CreateChatRoom();
        var room = manager.GetChatRoom(roomId);
        var duplexPipe = new DuplexPipe();
        var connection = new Connection(duplexPipe, null, null, new NullLogger<Connection>());
        var client = new ChatClient(connection, manager, new NullLogger<ChatClient>());
        room!.RegisterClient(new ChatServerAuthenticationInfo(room.GetNextClientIndex(), roomId, "Bob", ChatServerHost, "128450673"));

        var authenticationPacket = new byte[] { 0xC1, 0x10, 0x00, 0x00, (byte)roomId, (byte)(roomId >> 8), 0xCD, 0xFD, 0x93, 0xC8, 0xFA, 0x9B, 0xCA, 0xF8, 0x98, 0xFC };
        await duplexPipe.ReceivePipe.Writer.WriteAndWaitForFlushAsync(authenticationPacket).ConfigureAwait(false);

        Assert.That(room.ConnectedClients, Contains.Item(client));
    }

    /// <summary>
    /// Tests if the authentication fails by providing the wrong token.
    /// </summary>
    /// <returns>The async task.</returns>
    [Test]
    public async Task AuthenticationFailedByWrongTokenAsync()
    {
        var manager = new ChatRoomManager(new NullLoggerFactory());
        var roomId = manager.CreateChatRoom();
        var room = manager.GetChatRoom(roomId);
        var duplexPipe = new DuplexPipe();
        var connection = new Connection(duplexPipe, null, null, new NullLogger<Connection>());
        var client = new ChatClient(connection, manager, new NullLogger<ChatClient>());
        room!.RegisterClient(new ChatServerAuthenticationInfo(room.GetNextClientIndex(), roomId, "Bob", ChatServerHost, "128450674"));
        var authenticationPacket = new byte[] { 0xC1, 0x10, 0x00, 0x00, (byte)roomId, (byte)(roomId >> 8), 0xCD, 0xFD, 0x93, 0xC8, 0xFA, 0x9B, 0xCA, 0xF8, 0x98, 0xFC };
        await duplexPipe.ReceivePipe.Writer.WriteAndWaitForFlushAsync(authenticationPacket).ConfigureAwait(false);

        Assert.That(room.ConnectedClients.Contains(client), Is.False);
        Assert.That(connection.Connected, Is.False);
    }

    /// <summary>
    /// Tests what happens if two clients try to authenticate with the same token. The first connection should be connected and authenticated, the second disconnected.
    /// </summary>
    /// <returns>The async task.</returns>
    [Test]
    public async Task AuthenticationFailedForSecondConnectionAsync()
    {
        var manager = new ChatRoomManager(new NullLoggerFactory());
        var roomId = manager.CreateChatRoom();
        var room = manager.GetChatRoom(roomId);
        room!.RegisterClient(new ChatServerAuthenticationInfo(room.GetNextClientIndex(), roomId, "Bob", ChatServerHost, "128450673"));
        var authenticationPacket = new byte[] { 0xC1, 0x10, 0x00, 0x00, (byte)roomId, (byte)(roomId >> 8), 0xCD, 0xFD, 0x93, 0xC8, 0xFA, 0x9B, 0xCA, 0xF8, 0x98, 0xFC };
        var duplexPipe1 = new DuplexPipe();
        var connection1 = new Connection(duplexPipe1, null, null, new NullLogger<Connection>());
        var client1 = new ChatClient(connection1, manager, new NullLogger<ChatClient>());
        await duplexPipe1.ReceivePipe.Writer.WriteAndWaitForFlushAsync(authenticationPacket).ConfigureAwait(false);

        var duplexPipe2 = new DuplexPipe();
        var connection2 = new Connection(duplexPipe2, null, null, new NullLogger<Connection>());
        var client2 = new ChatClient(connection2, manager, new NullLogger<ChatClient>());
        var disconnectedRaised = false;
        client2.Disconnected += (_, _) => disconnectedRaised = true;

        await duplexPipe2.ReceivePipe.Writer.WriteAndWaitForFlushAsync(authenticationPacket).ConfigureAwait(false);

        Assert.That(room.ConnectedClients, Has.Count.EqualTo(1));
        Assert.That(room.ConnectedClients, Contains.Item(client1));
        Assert.That(connection2.Connected, Is.False);
        Assert.That(disconnectedRaised, Is.True);
    }

    /// <summary>
    /// Tests if a successful authentication sets the <see cref="IChatClient.Nickname"/>.
    /// </summary>
    /// <returns>The async task.</returns>
    [Test]
    public async Task SetNicknameAsync()
    {
        var manager = new ChatRoomManager(new NullLoggerFactory());
        var roomId = manager.CreateChatRoom();
        var room = manager.GetChatRoom(roomId);
        var duplexPipe = new DuplexPipe();
        var connection = new Connection(duplexPipe, null, null, new NullLogger<Connection>());
        var client = new ChatClient(connection, manager, new NullLogger<ChatClient>());

        room!.RegisterClient(new ChatServerAuthenticationInfo(room.GetNextClientIndex(), roomId, "Bob", ChatServerHost, "128450673"));
        var authenticationPacket = new byte[] { 0xC1, 0x10, 0x00, 0x00, (byte)roomId, (byte)(roomId >> 8), 0xCD, 0xFD, 0x93, 0xC8, 0xFA, 0x9B, 0xCA, 0xF8, 0x98, 0xFC };
        await duplexPipe.ReceivePipe.Writer.WriteAndWaitForFlushAsync(authenticationPacket).ConfigureAwait(false);

        Assert.That(client.Nickname, Is.EqualTo("Bob"));
    }

    /// <summary>
    /// Tests if a successful authentication sets the <see cref="IChatClient.Index"/>.
    /// </summary>
    /// <returns>The async task.</returns>
    [Test]
    public async Task SetClientIndexAsync()
    {
        var manager = new ChatRoomManager(new NullLoggerFactory());
        var roomId = manager.CreateChatRoom();
        var room = manager.GetChatRoom(roomId);
        var duplexPipe = new DuplexPipe();
        var connection = new Connection(duplexPipe, null, null, new NullLogger<Connection>());
        var client = new ChatClient(connection, manager, new NullLogger<ChatClient>());

        var authInfo = new ChatServerAuthenticationInfo(3, roomId, "Bob", ChatServerHost, "128450673");
        room!.RegisterClient(authInfo);

        var authenticationPacket = new byte[] { 0xC1, 0x10, 0x00, 0x00, (byte)roomId, (byte)(roomId >> 8), 0xCD, 0xFD, 0x93, 0xC8, 0xFA, 0x9B, 0xCA, 0xF8, 0x98, 0xFC };
        await duplexPipe.ReceivePipe.Writer.WriteAndWaitForFlushAsync(authenticationPacket).ConfigureAwait(false);

        Assert.That(client.Index, Is.EqualTo(authInfo.Index));
    }

    /// <summary>
    /// Tests if a message sent to the client gets "encrypted" properly by the XOR-3 key.
    /// </summary>
    /// <returns>The async task.</returns>
    [Test]
    public async Task SentMessageEncryptedProperlyAsync()
    {
        var manager = new ChatRoomManager(new NullLoggerFactory());
        var duplexPipe = new DuplexPipe();
        var connection = new Connection(duplexPipe, null, null, new NullLogger<Connection>());
        var client = new ChatClient(connection, manager, new NullLogger<ChatClient>());
        var expectedPacket = new byte[] { 0xC1, 0x0B, 0x04, 0x01, 0x06, 0xBD, 0x8E, 0xEA, 0xBD, 0x8E, 0xEA };
        await client.SendMessageAsync(1, "AAAAAA").ConfigureAwait(false);
        var sendResult = await duplexPipe.SendPipe.Reader.ReadAsync().ConfigureAwait(false);
        var sentPacket = sendResult.Buffer.ToArray();
        Assert.That(sentPacket, Is.EqualTo(expectedPacket));
    }

    /// <summary>
    /// Tests if the room client list is sent property to the client. It should contain the joined client.
    /// </summary>
    /// <returns>The async task.</returns>
    [Test]
    public async Task RoomClientListSentAfterJoinAsync()
    {
        var manager = new ChatRoomManager(new NullLoggerFactory());
        var roomId = manager.CreateChatRoom();
        var room = manager.GetChatRoom(roomId);
        var duplexPipe = new DuplexPipe();
        var connection = new Connection(duplexPipe, null, null, new NullLogger<Connection>());
        var client = new ChatClient(connection, manager, new NullLogger<ChatClient>());
        room!.RegisterClient(new ChatServerAuthenticationInfo(room.GetNextClientIndex(), roomId, "Bob", ChatServerHost, "128450673"));

        var authenticationPacket = new byte[] { 0xC1, 0x10, 0x00, 0x00, (byte)roomId, (byte)(roomId >> 8), 0xCD, 0xFD, 0x93, 0xC8, 0xFA, 0x9B, 0xCA, 0xF8, 0x98, 0xFC };
        await duplexPipe.ReceivePipe.Writer.WriteAndWaitForFlushAsync(authenticationPacket).ConfigureAwait(false);

        var expectedPacket = new byte[] { 0xC2, 0x00, 0x13, 0x02, 0x00, 0x00, 0x01, 0x00, 0x00, 0x42, 0x6F, 0x62, 0, 0, 0, 0, 0, 0, 0 };
        var readResult = await duplexPipe.SendPipe.Reader.ReadAsync().ConfigureAwait(false);
        var result = readResult.Buffer.ToArray();
        Assert.That(result, Is.EquivalentTo(expectedPacket));
        Assert.That(client.Nickname, Is.EqualTo("Bob"));
    }

    /// <summary>
    /// Tests if the room client list is sent to the client which joins as second. It should contain both clients.
    /// </summary>
    /// <returns>The async task.</returns>
    [Test]
    public async Task RoomClientListSentForSecondClientAsync()
    {
        var manager = new ChatRoomManager(new NullLoggerFactory());
        var roomId = manager.CreateChatRoom();
        var room = manager.GetChatRoom(roomId);
        var duplexPipe1 = new DuplexPipe();
        var connection1 = new Connection(duplexPipe1, null, null, new NullLogger<Connection>());
        var client1 = new ChatClient(connection1, manager, new NullLogger<ChatClient>());
        room!.RegisterClient(new ChatServerAuthenticationInfo(room.GetNextClientIndex(), roomId, "Bob", ChatServerHost, "128450673"));
        room.RegisterClient(new ChatServerAuthenticationInfo(room.GetNextClientIndex(), roomId, "Alice", ChatServerHost, "94371960"));

        var authenticationPacket1 = new byte[] { 0xC1, 0x10, 0x00, 0x00, (byte)roomId, (byte)(roomId >> 8), 0xCD, 0xFD, 0x93, 0xC8, 0xFA, 0x9B, 0xCA, 0xF8, 0x98, 0xFC };
        await duplexPipe1.ReceivePipe.Writer.WriteAndWaitForFlushAsync(authenticationPacket1).ConfigureAwait(false);

        var duplexPipe2 = new DuplexPipe();
        var connection2 = new Connection(duplexPipe2, null, null, new NullLogger<Connection>());
        var client2 = new ChatClient(connection2, manager, new NullLogger<ChatClient>());
        var authenticationPacket2 = new byte[] { 0xC1, 0x10, 0x00, 0x00, (byte)roomId, (byte)(roomId >> 8), 0xC5, 0xFB, 0x98, 0xCB, 0xFE, 0x92, 0xCA, 0xFF, 0xAB, 0xFC };
        await duplexPipe2.ReceivePipe.Writer.WriteAndWaitForFlushAsync(authenticationPacket2).ConfigureAwait(false);

        var expectedPacket = new byte[]
        {
            0xC2, 0x00, 0x1E, 0x02, 0x00, 0x00, 0x02, 0x00,
            0x00, 0x42, 0x6F, 0x62, 0, 0, 0, 0, 0, 0, 0,
            0x01, 0x41, 0x6C, 0x69, 0x63, 0x65, 0, 0, 0, 0, 0,
        };

        await duplexPipe2.SendPipe.Writer.WaitForFlushAsync().ConfigureAwait(false);

        var readResult = await duplexPipe2.SendPipe.Reader.ReadAsync().ConfigureAwait(false);
        var result = readResult.Buffer.ToArray();
        Assert.That(result, Is.EquivalentTo(expectedPacket));
        Assert.That(client1.Nickname, Is.EqualTo("Bob"));
        Assert.That(client2.Nickname, Is.EqualTo("Alice"));
    }

    /// <summary>
    /// Tests if the first client gets notified correctly when the second client joins the chat room.
    /// </summary>
    /// <returns>The async task.</returns>
    [Test]
    public async Task ClientJoinedPacketSentAsync()
    {
        var manager = new ChatRoomManager(new NullLoggerFactory());
        var roomId = manager.CreateChatRoom();
        var room = manager.GetChatRoom(roomId);
        var duplexPipe1 = new DuplexPipe();
        var connection1 = new Connection(duplexPipe1, null, null, new NullLogger<Connection>());
        var client1 = new ChatClient(connection1, manager, new NullLogger<ChatClient>());
        room!.RegisterClient(new ChatServerAuthenticationInfo(room.GetNextClientIndex(), roomId, "Bob", ChatServerHost, "128450673"));
        room.RegisterClient(new ChatServerAuthenticationInfo(room.GetNextClientIndex(), roomId, "Alice", ChatServerHost, "94371960"));

        var authenticationPacket1 = new byte[] { 0xC1, 0x10, 0x00, 0x00, (byte)roomId, (byte)(roomId >> 8), 0xCD, 0xFD, 0x93, 0xC8, 0xFA, 0x9B, 0xCA, 0xF8, 0x98, 0xFC };
        await duplexPipe1.ReceivePipe.Writer.WriteAndWaitForFlushAsync(authenticationPacket1).ConfigureAwait(false);

        var duplexPipe2 = new DuplexPipe();
        var connection2 = new Connection(duplexPipe2, null, null, new NullLogger<Connection>());
        var client2 = new ChatClient(connection2, manager, new NullLogger<ChatClient>());
        var authenticationPacket2 = new byte[] { 0xC1, 0x10, 0x00, 0x00, (byte)roomId, (byte)(roomId >> 8), 0xC5, 0xFB, 0x98, 0xCB, 0xFE, 0x92, 0xCA, 0xFF, 0xAB, 0xFC };
        await duplexPipe2.ReceivePipe.Writer.WriteAndWaitForFlushAsync(authenticationPacket2).ConfigureAwait(false);

        var expectedPacket = new byte[]
        {
            0xC1, 0x0F, 0x01, 0x00, 0x01, 0x41, 0x6C, 0x69, 0x63, 0x65, 0, 0, 0, 0, 0,
        }.AsString();

        var readResult = await duplexPipe1.SendPipe.Reader.ReadAsync().ConfigureAwait(false);
        var received = readResult.Buffer.ToArray().AsString();

        Assert.That(received, Contains.Substring(expectedPacket));
        Assert.That(client1.Nickname, Is.EqualTo("Bob"));
        Assert.That(client2.Nickname, Is.EqualTo("Alice"));
    }

    /// <summary>
    /// Tests if a call to <see cref="ChatClient.LogOffAsync" /> removes it from the chatroom,
    /// and the other remaining client gets notified about it.
    /// </summary>
    /// <returns>The async task.</returns>
    [Test]
    public async Task ClientLoggedOffAsync()
    {
        var manager = new ChatRoomManager(new NullLoggerFactory());
        var roomId = manager.CreateChatRoom();
        var room = manager.GetChatRoom(roomId);
        var bobsPipe = new DuplexPipe();
        var bobsClient = new ChatClient(new Connection(bobsPipe, null, null, new NullLogger<Connection>()), manager, new NullLogger<ChatClient>());
        room!.RegisterClient(new ChatServerAuthenticationInfo(room.GetNextClientIndex(), roomId, "Bob", ChatServerHost, "128450673"));
        room.RegisterClient(new ChatServerAuthenticationInfo(room.GetNextClientIndex(), roomId, "Alice", ChatServerHost, "94371960"));

        var bobsAuthPacket = new byte[] { 0xC1, 0x10, 0x00, 0x00, (byte)roomId, (byte)(roomId >> 8), 0xCD, 0xFD, 0x93, 0xC8, 0xFA, 0x9B, 0xCA, 0xF8, 0x98, 0xFC };
        await bobsPipe.ReceivePipe.Writer.WriteAndWaitForFlushAsync(bobsAuthPacket).ConfigureAwait(false);
        await bobsPipe.ReceivePipe.Writer.WaitForFlushAsync().ConfigureAwait(false);

        var alicePipe = new DuplexPipe();
        var aliceConnection = new Connection(alicePipe, null, null, new NullLogger<Connection>());
        var aliceClient = new ChatClient(aliceConnection, manager, new NullLogger<ChatClient>());
        var aliceAuthPacket = new byte[] { 0xC1, 0x10, 0x00, 0x00, (byte)roomId, (byte)(roomId >> 8), 0xC5, 0xFB, 0x98, 0xCB, 0xFE, 0x92, 0xCA, 0xFF, 0xAB, 0xFC };
        await alicePipe.ReceivePipe.Writer.WriteAndWaitForFlushAsync(aliceAuthPacket).ConfigureAwait(false);

        await bobsClient.LogOffAsync().ConfigureAwait(false);

        await Task.Delay(100).ConfigureAwait(false);

        var expectedPacket = new byte[]
        {
            0xC1, 0x0F, 0x01, 0x01, 0x00, 0x42, 0x6F, 0x62, 0, 0, 0, 0, 0, 0, 0,
        }.AsString();

        await alicePipe.SendPipe.Writer.WaitForFlushAsync().ConfigureAwait(false);

        var readResult = await alicePipe.SendPipe.Reader.ReadAsync().ConfigureAwait(false);
        var packets = readResult.Buffer.ToArray().AsString();
        Assert.That(packets, Contains.Substring(expectedPacket));
        Assert.That(room.ConnectedClients, Has.Count.EqualTo(1));
        Assert.That(room.ConnectedClients, Contains.Item(aliceClient));
    }
}