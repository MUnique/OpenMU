// <copyright file="ServerListRequestHandler.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.ConnectServer.PacketHandler;

using Microsoft.Extensions.Logging;
using MUnique.OpenMU.Network;

/// <summary>
/// Handles the request of the server list.
/// </summary>
internal class ServerListRequestHandler : IPacketHandler<Client>
{
    private readonly IConnectServer _connectServer;
    private readonly ILogger<ServerListRequestHandler> _logger;

    /// <summary>
    /// Initializes a new instance of the <see cref="ServerListRequestHandler" /> class.
    /// </summary>
    /// <param name="connectServer">The connect server.</param>
    /// <param name="logger">The logger.</param>
    public ServerListRequestHandler(IConnectServer connectServer, ILogger<ServerListRequestHandler> logger)
    {
        this._connectServer = connectServer;
        this._logger = logger;
    }

    /// <inheritdoc/>
    public void HandlePacket(Client client, Span<byte> packet)
    {
        this._logger.LogDebug("Client {0}:{1} requested Server List", client.Address, client.Port);
        if (client.ServerListRequestCount >= this._connectServer.Settings.MaxServerListRequests)
        {
            this._logger.LogDebug("Client {0}:{1} reached maxListRequests", client.Address, client.Port);
            client.Connection.Disconnect();
        }

        client.ServerListRequestCount++;
        var serverList = this._connectServer.ServerList.Serialize();
        using var writer = client.Connection.StartSafeWrite(serverList[0], serverList.Length);
        serverList.CopyTo(writer.Span);
        writer.Commit();
    }
}