// <copyright file="Settings.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.ConnectServer
{
    using System;

    /// <summary>
    /// The settings of the connect server.
    /// </summary>
    internal class Settings
    {
        /// <summary>
        /// Gets or sets a value indicating whether the client should get disconnected when a unknown packet is getting received.
        /// </summary>
        public bool DcOnUnknownPacket { get; set; } = true;

        /// <summary>
        /// Gets or sets the maximum size of the packets which should be received from the client. If this size is exceeded, the client will be disconnected.
        /// </summary>
        /// <remarks>DOS protection.</remarks>
        public byte MaxReceiveSize { get; set; } = 6;

        /// <summary>
        /// Gets or sets the client listener port.
        /// </summary>
        public ushort ClientListenerPort { get; set; } = 44405;

        /// <summary>
        /// Gets or sets the timeout after which clients without activity get disconnected.
        /// </summary>
        public TimeSpan Timeout { get; set; } = new TimeSpan(0, 1, 0);

        /// <summary>
        /// Gets or sets the current patch version.
        /// </summary>
        public byte[] CurrentPatchVersion { get; set; } = new byte[] { 1, 3, 0x2B };

        /// <summary>
        /// Gets or sets the patch address.
        /// </summary>
        public string PatchAddress { get; set; } = "patch.muonline.webzen.com";

        /// <summary>
        /// Gets or sets the maximum connections per ip.
        /// </summary>
        public int MaxConnectionsPerAddress { get; set; } = 30;

        /// <summary>
        /// Gets or sets a value indicating whether the <see cref="MaxConnectionsPerAddress"/> should be checked.
        /// </summary>
        public bool CheckMaxConnectionsPerAddress { get; set; } = true;

        /// <summary>
        /// Gets or sets the maximum connections the connect server should handle.
        /// </summary>
        public int MaxConnections { get; set; } = 10000;

        /// <summary>
        /// Gets or sets the listener backlog for the client listener.
        /// </summary>
        public int ListenerBacklog { get; set; } = 100;

        /// <summary>
        /// Gets or sets the maximum FTP requests per connection.
        /// </summary>
        public int MaxFtpRequests { get; set; } = 1;

        /// <summary>
        /// Gets or sets the maximum ip requests per connection.
        /// </summary>
        public int MaxIpRequests { get; set; } = 5;

        /// <summary>
        /// Gets or sets the maximum server list requests per connection.
        /// </summary>
        public int MaxServerListRequests { get; set; } = 20;

        /// <summary>
        /// Gets or sets the size of the client pool.
        /// </summary>
        public int ClientPoolSize { get; set; } = 10;
    }
}
