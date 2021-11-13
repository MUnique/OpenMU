// <copyright file="ServerInfoRequestHandler.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.ConnectServer.PacketHandler;

using Microsoft.Extensions.Logging;
using MUnique.OpenMU.Network;

/// <summary>
/// Handles the server info request of a client, which means the client wants to know the connect data of the server it just clicked on.
/// </summary>
internal class ServerInfoRequestHandler : IPacketHandler<Client>
{
    private readonly IConnectServer _connectServer;
    private readonly ILogger<ServerInfoRequestHandler> _logger;

    /// <summary>
    /// Initializes a new instance of the <see cref="ServerInfoRequestHandler" /> class.
    /// </summary>
    /// <param name="connectServer">The connect server.</param>
    /// <param name="logger">The logger.</param>
    public ServerInfoRequestHandler(IConnectServer connectServer, ILogger<ServerInfoRequestHandler> logger)
    {
        this._connectServer = connectServer;
        this._logger = logger;
    }

    /// <inheritdoc/>
    public void HandlePacket(Client client, Span<byte> packet)
    {
        var serverId = packet.Length > 5 ? (ushort)(packet[4] | packet[5] << 8) : packet[4];
        this._logger.LogDebug("Client {0}:{1} requested Connection Info of ServerId {2}", client.Address, client.Port, serverId);
        if (client.ServerInfoRequestCount >= this._connectServer.Settings.MaxIpRequests)
        {
            this._logger.LogDebug($"Client {client.Address}:{client.Port} reached max ip requests.");
            client.Connection.Disconnect();
        }

        if (this._connectServer.ConnectInfos.TryGetValue(serverId, out var connectInfo))
        {
            using var writer = client.Connection.StartSafeWrite(connectInfo[0], connectInfo.Length);
            connectInfo.CopyTo(writer.Span);
            writer.Commit();
        }
        else
        {
            this._logger.LogDebug($"Client {client.Address}:{client.Port}: Connection Info not found, sending Server List instead.");
            var serverList = this._connectServer.ServerList.Serialize();
            using var writer = client.Connection.StartSafeWrite(serverList[0], serverList.Length);
            serverList.CopyTo(writer.Span);
            writer.Commit();
        }

        client.SendHello();
        client.ServerInfoRequestCount++;
    }
}