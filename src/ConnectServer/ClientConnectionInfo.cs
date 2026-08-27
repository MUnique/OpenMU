// <copyright file="ClientConnectionInfo.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.ConnectServer;

using MUnique.OpenMU.Interfaces;
using MUnique.OpenMU.Network;
using MUnique.OpenMU.Network.Analyzer;
using MUnique.OpenMU.Network.PlugIns;

/// <summary>
/// The <see cref="ICapturedConnectionInfo"/> of a client of the connect server.
/// </summary>
internal sealed class ClientConnectionInfo : ICapturedConnectionInfo
{
    private readonly Client _client;

    /// <summary>
    /// Initializes a new instance of the <see cref="ClientConnectionInfo"/> class.
    /// </summary>
    /// <param name="client">The client.</param>
    /// <param name="serverId">The identifier of the connect server.</param>
    /// <param name="clientVersion">The client version of the connect server.</param>
    /// <param name="serverDescription">The description of the connect server.</param>
    public ClientConnectionInfo(Client client, int serverId, ClientVersion clientVersion, string serverDescription)
    {
        this._client = client;
        this.ServerId = serverId;
        this.ClientVersion = clientVersion;
        this.ServerDescription = serverDescription;
    }

    /// <inheritdoc />
    public Guid Id => this._client.Connection.Id;

    /// <inheritdoc />
    public ServerType ServerType => ServerType.ConnectServer;

    /// <inheritdoc />
    public int ServerId { get; }

    /// <inheritdoc />
    public string ServerDescription { get; }

    /// <summary>
    /// Gets the name of the account. The clients of a connect server are never logged in, so
    /// it's always <see langword="null"/>.
    /// </summary>
    public string? AccountName => null;

    /// <summary>
    /// Gets the name of the character. The clients of a connect server never selected one, so
    /// it's always <see langword="null"/>.
    /// </summary>
    public string? CharacterName => null;

    /// <inheritdoc />
    public string? RemoteEndPoint => this._client.Connection.EndPoint?.ToString();

    /// <inheritdoc />
    public ClientVersion ClientVersion { get; }

    /// <inheritdoc />
    public PacketDefinitionSet DefinitionSet => PacketDefinitionSet.ConnectServer;

    /// <inheritdoc />
    public bool IsConnected => this._client.Connection.Connected;

    /// <inheritdoc />
    public string DisplayName => this.RemoteEndPoint ?? this.Id.ToString();

    /// <inheritdoc />
    public void AddCaptureSink(IPacketCaptureSink sink) => this._client.Connection.AddCaptureSink(sink);

    /// <inheritdoc />
    public void RemoveCaptureSink(IPacketCaptureSink sink) => this._client.Connection.RemoveCaptureSink(sink);

    /// <inheritdoc />
    public ValueTask DisconnectAsync() => this._client.Connection.DisconnectAsync();
}
