// <copyright file="ServerListHandler.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.ConnectServer.PacketHandler;

using Microsoft.Extensions.Logging;
using MUnique.OpenMU.Interfaces;
using MUnique.OpenMU.Network;
using MUnique.OpenMU.Network.Xor;
using IConnectServer = MUnique.OpenMU.ConnectServer.IConnectServer;

/// <summary>
/// Handles the requests of server data.
/// </summary>
internal class ServerListHandler : IPacketHandler<Client>
{
    private readonly ILogger<ServerListHandler> _logger;
    private readonly IConnectServerSettings _connectServerSettings;
    private readonly IDictionary<byte, IPacketHandler<Client>> _packetHandlers = new Dictionary<byte, IPacketHandler<Client>>();

    /// <summary>
    /// Initializes a new instance of the <see cref="ServerListHandler" /> class.
    /// </summary>
    /// <param name="connectServer">The connect server.</param>
    /// <param name="loggerFactory">The logger factory.</param>
    public ServerListHandler(IConnectServer connectServer, ILoggerFactory loggerFactory)
    {
        this._logger = loggerFactory.CreateLogger<ServerListHandler>();
        this._connectServerSettings = connectServer.Settings;
        this._packetHandlers.Add(0x03, new ServerInfoRequestHandler(connectServer, loggerFactory.CreateLogger<ServerInfoRequestHandler>()));
        this._packetHandlers.Add(0x06, new ServerListRequestHandler(connectServer, loggerFactory.CreateLogger<ServerListRequestHandler>()));

        // old protocol:
        this._packetHandlers.Add(0x02, new ServerListRequestHandler(connectServer, loggerFactory.CreateLogger<ServerListRequestHandler>()));
    }

    /// <inheritdoc/>
    public async ValueTask HandlePacketAsync(Client client, Memory<byte> packet)
    {
        var headerSize = ArrayExtensions.GetPacketHeaderSize(packet.Span);
        if (headerSize == 0 || packet.Length <= headerSize + 1)
        {
            if (this._connectServerSettings.DisconnectOnUnknownPacket)
            {
                this._logger.LogInformation("Client {0}:{1} will be disconnected because it sent an unknown packet: {2}", client.Address, client.Port, packet.ToArray().ToHexString());
                await client.Connection.DisconnectAsync().ConfigureAwait(false);
            }

            return;
        }

        var subTypeIndex = headerSize + 1;
        var packetSubType = packet.Span[subTypeIndex];
        if (this._packetHandlers.TryGetValue(packetSubType, out var packetHandler))
        {
            await packetHandler.HandlePacketAsync(client, packet).ConfigureAwait(false);
            return;
        }

        if (TryGetXorDecryptedPacket(packet.Span, out var decryptedPacket, out var decryptedSubType))
        {
            if (this._packetHandlers.TryGetValue(decryptedSubType, out var decryptedHandler))
            {
                await decryptedHandler.HandlePacketAsync(client, decryptedPacket).ConfigureAwait(false);
                return;
            }
        }
        else if (this._connectServerSettings.DisconnectOnUnknownPacket)
        {
            this._logger.LogInformation("Client {0}:{1} will be disconnected because it sent an unknown packet: {2}", client.Address, client.Port, packet.ToArray().ToHexString());
            await client.Connection.DisconnectAsync().ConfigureAwait(false);
        }
        else
        {
            // do nothing
        }
    }

    private static bool TryGetXorDecryptedPacket(ReadOnlySpan<byte> packet, out Memory<byte> decryptedPacket, out byte subType)
    {
        decryptedPacket = default;
        subType = 0;

        if (packet.IsEmpty)
        {
            return false;
        }

        var headerSize = ArrayExtensions.GetPacketHeaderSize(packet[0]);
        if (headerSize == 0 || packet.Length <= headerSize + 1)
        {
            return false;
        }

        var buffer = packet.ToArray();
        for (var i = buffer.Length - 1; i > headerSize; i--)
        {
            buffer[i] = (byte)(buffer[i] ^ buffer[i - 1] ^ DefaultKeys.Xor32Key[i % 32]);
        }

        var subTypeIndex = headerSize + 1;
        if (subTypeIndex >= buffer.Length)
        {
            return false;
        }

        subType = buffer[subTypeIndex];
        decryptedPacket = buffer;
        return true;
    }
}
