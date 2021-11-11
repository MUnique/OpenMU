﻿// <copyright file="ServerListHandler.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.ConnectServer.PacketHandler;

using Microsoft.Extensions.Logging;
using MUnique.OpenMU.Interfaces;
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
    public void HandlePacket(Client client, Span<byte> packet)
    {
        var packetSubType = packet[3];
        if (this._packetHandlers.TryGetValue(packetSubType, out var packetHandler))
        {
            packetHandler.HandlePacket(client, packet);
        }
        else if (this._connectServerSettings.DisconnectOnUnknownPacket)
        {
            this._logger.LogInformation("Client {0}:{1} will be disconnected because it sent an unknown packet: {2}", client.Address, client.Port, packet.ToArray().ToHexString());
            client.Connection.Disconnect();
        }
        else
        {
            // do nothing
        }
    }
}