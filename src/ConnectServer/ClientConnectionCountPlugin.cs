// <copyright file="ClientConnectionCountPlugin.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.ConnectServer;

using System.Net;
using System.Net.Sockets;
using Microsoft.Extensions.Logging;
using MUnique.OpenMU.Interfaces;

/// <summary>
/// The client connection count plugin.
/// </summary>
internal class ClientConnectionCountPlugin : IAfterSocketAcceptPlugin, IAfterDisconnectPlugin
{
    private readonly ILogger<ClientConnectionCountPlugin> _logger;
    private readonly ClientConnectionCounter _clientCounter;
    private readonly IConnectServerSettings _connectServerSettings;

    /// <summary>
    /// Initializes a new instance of the <see cref="ClientConnectionCountPlugin" /> class.
    /// </summary>
    /// <param name="connectServerSettings">The settings.</param>
    /// <param name="logger">The logger.</param>
    public ClientConnectionCountPlugin(IConnectServerSettings connectServerSettings, ILogger<ClientConnectionCountPlugin> logger)
    {
        this._connectServerSettings = connectServerSettings;
        this._logger = logger;
        this._clientCounter = new ClientConnectionCounter();
    }

    /// <inheritdoc/>
    public bool OnAfterSocketAccept(Socket socket)
    {
        var ipAddress = (socket.RemoteEndPoint as IPEndPoint)?.Address;
        if (ipAddress is null)
        {
            // should never happen - but who knows. In this case, we allow the connection.
            this._logger.LogDebug($"Non-IPEndPoint connected: {socket.RemoteEndPoint}.");
            return true;
        }

        if (this._connectServerSettings.CheckMaxConnectionsPerAddress
            && this._clientCounter.GetConnectionCount(ipAddress) >= this._connectServerSettings.MaxConnectionsPerAddress)
        {
            this._logger.LogWarning("Maximum Connections per IP reached: {0}, Connection refused.", ipAddress);
            return false;
        }

        this._clientCounter.AddConnection(ipAddress);
        return true;
    }

    /// <inheritdoc/>
    public void OnAfterDisconnect(Client client)
    {
        this._clientCounter.RemoveConnection(client.Address);
    }
}