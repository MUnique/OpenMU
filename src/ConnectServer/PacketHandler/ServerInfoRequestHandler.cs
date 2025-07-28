// <copyright file="ServerInfoRequestHandler.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.ConnectServer.PacketHandler;

using System.Net;
using Microsoft.Extensions.Logging;
using MUnique.OpenMU.Network;
using MUnique.OpenMU.Network.Packets.ConnectServer;

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
    public async ValueTask HandlePacketAsync(Client client, Memory<byte> packet)
    {
        var serverId = GetServerId(packet.Span);
        this._logger.LogDebug("Client {0}:{1} requested Connection Info of ServerId {2}", client.Address, client.Port, serverId);
        if (client.ServerInfoRequestCount >= this._connectServer.Settings.MaxIpRequests)
        {
            this._logger.LogDebug($"Client {client.Address}:{client.Port} reached max ip requests.");
            await client.Connection.DisconnectAsync().ConfigureAwait(false);
        }

        // First we look, if we can just use the IP address which the client connected to.
        // If the game server is running on the same ip as the connect server, we can use that.
        // This way, we can be sure, that the client can connect to it, too.
        var localIpEndPoint = client.Connection.LocalEndPoint as IPEndPoint;
        var serverItem = this._connectServer.ServerList.GetItem(serverId);
        var isGameServerOnSameMachineAsConnectServer = (serverItem?.EndPoint.Address).IsOnSameHost();
        var isClientConnectedOnNonRegisteredAddress = !object.Equals(serverItem?.EndPoint.Address, localIpEndPoint?.Address);
        bool.TryParse(Environment.GetEnvironmentVariable("DOTNET_RUNNING_IN_CONTAINER"), out var isRunningOnDocker);
        if (isGameServerOnSameMachineAsConnectServer
            && !isRunningOnDocker
            && isClientConnectedOnNonRegisteredAddress) // only if we can't use the cached data
        {
            int WritePacket()
            {
                var data = client.Connection.Output.GetSpan(ConnectionInfoRef.Length)[..ConnectionInfoRef.Length];
                _ = new ConnectionInfoRef(data)
                {
                    IpAddress = localIpEndPoint!.Address.ToString(),
                    Port = (ushort)serverItem!.EndPoint.Port,
                };
                return data.Length;
            }

            await client.Connection.SendAsync(WritePacket).ConfigureAwait(false);
        }
        else if (this._connectServer.ConnectInfos.TryGetValue(serverId, out var connectInfo))
        {
            // more optimal way, because the serialized data was cached.

            int WritePacket()
            {
                var span = client.Connection.Output.GetSpan(connectInfo.Length)[..connectInfo.Length];
                connectInfo.CopyTo(span);
                return span.Length;
            }

            await client.Connection.SendAsync(WritePacket).ConfigureAwait(false);
        }
        else
        {
            this._logger.LogDebug($"Client {client.Address}:{client.Port}: Connection Info not found, sending Server List instead.");
            int WritePacket()
            {
                var serverList = this._connectServer.ServerList.Serialize();
                var span = client.Connection.Output.GetSpan(serverList.Length)[..serverList.Length];
                serverList.CopyTo(span);
                return span.Length;
            }

            await client.Connection.SendAsync(WritePacket).ConfigureAwait(false);
        }

        await client.SendHelloAsync().ConfigureAwait(false);
        client.ServerInfoRequestCount++;
    }

    private static ushort GetServerId(Span<byte> packet)
    {
        if (packet.Length == ConnectionInfoRequestRef.Length)
        {
            ConnectionInfoRequestRef data = packet;
            return data.ServerId;
        }

        if (packet.Length == ConnectionInfoRequest075Ref.Length)
        {
            ConnectionInfoRequest075Ref data = packet;
            return data.ServerId;
        }

        throw new ArgumentException($"Unknown packet length C1 {packet.Length} F4 03 ...");
    }
}