// <copyright file="IConnectServerSettings.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.Interfaces
{
    using System;

    /// <summary>
    /// The connectServerSettings of the connect server.
    /// </summary>
    public interface IConnectServerSettings
    {
        /// <summary>
        /// Gets the server identifier.
        /// </summary>
        /// <remarks>Should be unique within all <see cref="IConnectServerSettings"/>.</remarks>
        byte ServerId { get; }

        /// <summary>
        /// Gets the description of the server.
        /// </summary>
        /// <remarks>
        /// Will be displayed in the server list in the admin panel as <see cref="IManageableServer.Description"/>.
        /// </remarks>
        string Description { get; }

        /// <summary>
        /// Gets a value indicating whether the client should get disconnected when a unknown packet is getting received.
        /// </summary>
        bool DisconnectOnUnknownPacket { get; }

        /// <summary>
        /// Gets the maximum size of the packets which should be received from the client. If this size is exceeded, the client will be disconnected.
        /// </summary>
        /// <remarks>DOS protection.</remarks>
        byte MaximumReceiveSize { get; }

        /// <summary>
        /// Gets the client listener port.
        /// </summary>
        int ClientListenerPort { get; }

        /// <summary>
        /// Gets the client which is expected to connect.
        /// </summary>
        IGameClientVersion Client { get; }

        /// <summary>
        /// Gets the timeout after which clients without activity get disconnected.
        /// </summary>
        TimeSpan Timeout { get; }

        /// <summary>
        /// Gets the current patch version.
        /// </summary>
        byte[] CurrentPatchVersion { get; }

        /// <summary>
        /// Gets the patch address.
        /// </summary>
        string PatchAddress { get; }

        /// <summary>
        /// Gets the maximum connections per ip.
        /// </summary>
        int MaxConnectionsPerAddress { get; }

        /// <summary>
        /// Gets a value indicating whether the <see cref="MaxConnectionsPerAddress"/> should be checked.
        /// </summary>
        bool CheckMaxConnectionsPerAddress { get; }

        /// <summary>
        /// Gets the maximum connections the connect server should handle.
        /// </summary>
        int MaxConnections { get; }

        /// <summary>
        /// Gets the listener backlog for the client listener.
        /// </summary>
        int ListenerBacklog { get; }

        /// <summary>
        /// Gets the maximum FTP requests per connection.
        /// </summary>
        int MaxFtpRequests { get; }

        /// <summary>
        /// Gets the maximum ip requests per connection.
        /// </summary>
        int MaxIpRequests { get; }

        /// <summary>
        /// Gets the maximum server list requests per connection.
        /// </summary>
        int MaxServerListRequests { get; }
    }
}
