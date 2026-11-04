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
    public async ValueTask HandlePacketAsync(Client client, Memory<byte> packet)
    {
        this._logger.LogDebug("Client {0}:{1} requested Server List", client.Address, client.Port);
        if (client.ServerListRequestCount >= this._connectServer.Settings.MaxServerListRequests)
        {
            this._logger.LogDebug("Client {0}:{1} reached maxListRequests", client.Address, client.Port);
            await client.Connection.DisconnectAsync().ConfigureAwait(false);
        }

        client.ServerListRequestCount++;

        int WritePacket()
        {
            var serverList = this._connectServer.ServerList.Serialize();
            var span = client.Connection.Output.GetSpan(serverList.Length)[..serverList.Length];
            serverList.CopyTo(span);
            return span.Length;
        }

        await client.Connection.SendAsync(WritePacket).ConfigureAwait(false);
    }
}