// <copyright file="ChatClientConnectionInfo.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.ChatServer;

using MUnique.OpenMU.Interfaces;
using MUnique.OpenMU.Network;
using MUnique.OpenMU.Network.Analyzer;
using MUnique.OpenMU.Network.PlugIns;

/// <summary>
/// The <see cref="ICapturedConnectionInfo"/> of a client of the chat server.
/// </summary>
internal sealed class ChatClientConnectionInfo : ICapturedConnectionInfo
{
    private readonly ChatClient _client;

    private readonly IConnection _connection;

    /// <summary>
    /// Initializes a new instance of the <see cref="ChatClientConnectionInfo"/> class.
    /// </summary>
    /// <param name="client">The client.</param>
    /// <param name="connection">The connection of the client.</param>
    /// <param name="serverId">The identifier of the chat server.</param>
    /// <param name="serverDescription">The description of the chat server.</param>
    public ChatClientConnectionInfo(ChatClient client, IConnection connection, int serverId, string serverDescription)
    {
        this._client = client;
        this._connection = connection;
        this.ServerId = serverId;
        this.ServerDescription = serverDescription;
    }

    /// <inheritdoc />
    public Guid Id => this._connection.Id;

    /// <inheritdoc />
    public ServerType ServerType => ServerType.ChatServer;

    /// <inheritdoc />
    public int ServerId { get; }

    /// <inheritdoc />
    public string ServerDescription { get; }

    /// <summary>
    /// Gets the name of the account. The chat server doesn't know the account of its clients,
    /// so it's always <see langword="null"/>.
    /// </summary>
    public string? AccountName => null;

    /// <summary>
    /// Gets the name of the character. The chat clients authenticate with the name of the
    /// character which joined the chat room.
    /// </summary>
    public string? CharacterName => this._client.Nickname;

    /// <inheritdoc />
    public string? RemoteEndPoint => this._connection.EndPoint?.ToString();

    /// <summary>
    /// Gets the client version. The chat protocol doesn't depend on it, so the default
    /// version is used.
    /// </summary>
    public ClientVersion ClientVersion => default;

    /// <inheritdoc />
    public PacketDefinitionSet DefinitionSet => PacketDefinitionSet.ChatServer;

    /// <inheritdoc />
    public bool IsConnected => this._connection.Connected;

    /// <inheritdoc />
    public string DisplayName => this.CharacterName ?? this.RemoteEndPoint ?? this.Id.ToString();

    /// <inheritdoc />
    public void AddCaptureSink(IPacketCaptureSink sink) => this._connection.AddCaptureSink(sink);

    /// <inheritdoc />
    public void RemoveCaptureSink(IPacketCaptureSink sink) => this._connection.RemoveCaptureSink(sink);

    /// <inheritdoc />
    public ValueTask DisconnectAsync() => this._client.LogOffAsync();
}
