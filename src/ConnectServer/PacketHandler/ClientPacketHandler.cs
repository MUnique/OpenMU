// <copyright file="ClientPacketHandler.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.ConnectServer.PacketHandler;

using System.Net.Sockets;

using Microsoft.Extensions.Logging;
using MUnique.OpenMU.Interfaces;
using MUnique.OpenMU.Network;
using IConnectServer = MUnique.OpenMU.ConnectServer.IConnectServer;

/// <summary>
/// The handler of packets coming from the client.
/// </summary>
internal class ClientPacketHandler : IPacketHandler<Client>
{
    private readonly ILogger<ClientPacketHandler> _logger;

    private readonly IDictionary<byte, IPacketHandler<Client>> _packetHandlers = new Dictionary<byte, IPacketHandler<Client>>();

    private readonly IConnectServerSettings _connectServerSettings;

    /// <summary>
    /// Initializes a new instance of the <see cref="ClientPacketHandler" /> class.
    /// </summary>
    /// <param name="connectServer">The connect server.</param>
    /// <param name="loggerFactory">The logger factory.</param>
    public ClientPacketHandler(IConnectServer connectServer, ILoggerFactory loggerFactory)
    {
        this._logger = loggerFactory.CreateLogger<ClientPacketHandler>();
        this._connectServerSettings = connectServer.Settings;

        // TODO: Is 0x05 correct? PatchCheckRequest has Code 0x02
        this._packetHandlers.Add(0x05, new FtpRequestHandler(connectServer.Settings, loggerFactory.CreateLogger<FtpRequestHandler>()));
        this._packetHandlers.Add(0xF4, new ServerListHandler(connectServer, loggerFactory));
    }

    /// <inheritdoc/>
    public async ValueTask HandlePacketAsync(Client client, Memory<byte> packet)
    {
        try
        {
            if (packet.Length > this._connectServerSettings.MaximumReceiveSize || packet.Length < 4)
            {
                await this.DisconnectClientUnknownPacketAsync(client, packet).ConfigureAwait(false);
                return;
            }

            var packetType = packet.Span[2];
            this._packetHandlers.TryGetValue(packetType, out var packetHandler);
            if (packetHandler is null)
            {
                var normalizedType = ArrayExtensions.NormalizePacketType(packet.Span[0], packetType);
                if (normalizedType != packetType)
                {
                    this._packetHandlers.TryGetValue(normalizedType, out packetHandler);
                }
            }

            if (packetHandler is not null)
            {
                await packetHandler.HandlePacketAsync(client, packet).ConfigureAwait(false);
            }
            else if (this._connectServerSettings.DisconnectOnUnknownPacket)
            {
                await this.DisconnectClientUnknownPacketAsync(client, packet).ConfigureAwait(false);
            }
            else
            {
                // do nothing.
            }
        }
        catch (SocketException ex)
        {
            if (this._logger.IsEnabled(LogLevel.Debug))
            {
                this._logger.LogDebug("SocketException occured in Client.ReceivePacket, Client Address: {0}:{1}, Packet: [{2}], Exception: {3}", client.Address, client.Port, packet.ToArray().ToHexString(), ex);
            }
        }
        catch (Exception ex)
        {
            this._logger.LogWarning("Exception occured in Client.ReceivePacket, Client Address: {0}:{1}, Packet: [{2}], Exception: {3}", client.Address, client.Port, packet.ToArray().ToHexString(), ex);
        }
    }

    private async ValueTask DisconnectClientUnknownPacketAsync(Client client, Memory<byte> packet)
    {
        this._logger.LogInformation("Client {0}:{1} will be disconnected because it sent an unknown packet: {2}", client.Address, client.Port, packet.ToArray().ToHexString());
        await client.Connection.DisconnectAsync().ConfigureAwait(false);
    }
}
