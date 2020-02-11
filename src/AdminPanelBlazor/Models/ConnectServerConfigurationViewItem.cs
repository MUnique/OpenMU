// <copyright file="ConnectServerConfigurationViewItem.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.AdminPanelBlazor.Models
{
    using System;
    using System.ComponentModel.DataAnnotations;
    using MUnique.OpenMU.DataModel.Configuration;

    /// <summary>
    /// A view item for the <see cref="ConnectServerDefinition"/>.
    /// </summary>
    public class ConnectServerConfigurationViewItem
    {
        private readonly ConnectServerDefinition config;

        /// <summary>
        /// Initializes a new instance of the <see cref="ConnectServerConfigurationViewItem"/> class.
        /// </summary>
        /// <param name="config">The configuration.</param>
        public ConnectServerConfigurationViewItem(ConnectServerDefinition config)
        {
            this.config = config;
        }

        /// <summary>
        /// Gets the internal id of the configuration.
        /// </summary>
        public Guid Id => this.config.GetId();

        /// <summary>
        /// Gets or sets the description of the server.
        /// </summary>
        /// <remarks>
        /// Will be displayed in the server list in the admin panel as <see cref="P:MUnique.OpenMU.Interfaces.IManageableServer.Description" />.
        /// </remarks>
        [Required]
        public string Description
        {
            get => this.config.Description;
            set => this.config.Description = value;
        }

        /// <summary>
        /// Gets or sets the game client definition.
        /// </summary>
        [Required]
        public GameClientDefinition GameClientDefinition
        {
            get => this.config.Client;
            set => this.config.Client = value;
        }

        /// <summary>
        /// Gets or sets a value indicating whether the client should get disconnected when a unknown packet is getting received.
        /// </summary>
        public bool DisconnectOnUnknownPacket
        {
            get => this.config.DisconnectOnUnknownPacket;
            set => this.config.DisconnectOnUnknownPacket = value;
        }

        /// <summary>
        /// Gets or sets the maximum size of the packets which should be received from the client. If this size is exceeded, the client will be disconnected.
        /// </summary>
        /// <remarks>
        /// DOS protection.
        /// </remarks>
        [Range(3, 255)]
        public int MaximumReceiveSize
        {
            get => this.config.MaximumReceiveSize;
            set => this.config.MaximumReceiveSize = (byte)value;
        }

        /// <summary>
        /// Gets or sets the network port on which the server is listening.
        /// </summary>
        [Range(1, 65535)]
        public int ClientListenerPort
        {
            get => this.config.ClientListenerPort;
            set => this.config.ClientListenerPort = (ushort)value;
        }

        /// <summary>
        /// Gets or sets the patch address.
        /// </summary>
        public string PatchAddress
        {
            get => this.config.PatchAddress;
            set => this.config.PatchAddress = value;
        }

        /// <summary>
        /// Gets or sets the maximum connections per ip.
        /// </summary>
        [Range(1, int.MaxValue)]
        public int MaxConnectionsPerAddress
        {
            get => this.config.MaxConnectionsPerAddress;
            set => this.config.MaxConnectionsPerAddress = value;
        }

        /// <summary>
        /// Gets or sets a value indicating whether the <see cref="P:MUnique.OpenMU.DataModel.Configuration.ConnectServerDefinition.MaxConnectionsPerAddress" /> should be checked.
        /// </summary>
        public bool CheckMaxConnectionsPerAddress
        {
            get => this.config.CheckMaxConnectionsPerAddress;
            set => this.config.CheckMaxConnectionsPerAddress = value;
        }

        /// <summary>
        /// Gets or sets the maximum connections the connect server should handle.
        /// </summary>
        [Range(1, int.MaxValue)]
        public int MaxConnections
        {
            get => this.config.MaxConnections;
            set => this.config.MaxConnections = value;
        }

        /// <summary>
        /// Gets or sets the listener backlog for the client listener.
        /// </summary>
        [Range(1, int.MaxValue)]
        public int ListenerBacklog
        {
            get => this.config.ListenerBacklog;
            set => this.config.ListenerBacklog = value;
        }

        /// <summary>
        /// Gets or sets the maximum FTP requests per connection.
        /// </summary>
        [Range(1, int.MaxValue)]
        public int MaxFtpRequests
        {
            get => this.config.MaxFtpRequests;
            set => this.config.MaxFtpRequests = value;
        }

        /// <summary>
        /// Gets or sets the maximum ip requests per connection.
        /// </summary>
        [Range(1, int.MaxValue)]
        public int MaxIpRequests
        {
            get => this.config.MaxIpRequests;
            set => this.config.MaxIpRequests = value;
        }

        /// <summary>
        /// Gets or sets the maximum server list requests per connection.
        /// </summary>
        [Range(1, int.MaxValue)]
        public int MaxServerListRequests
        {
            get => this.config.MaxServerListRequests;
            set => this.config.MaxServerListRequests = value;
        }

        /// <summary>
        /// Gets or sets the timeout in seconds.
        /// </summary>
        [Range(10, 3600)]
        public int TimeoutSeconds
        {
            get => (int)this.config.Timeout.TotalSeconds;
            set => this.config.Timeout = new TimeSpan(value * TimeSpan.TicksPerSecond);
        }

        /// <summary>
        /// Gets or sets the current version major.
        /// </summary>
        [Range(0, 255)]
        public int CurrentVersionMajor
        {
            get => this.config.CurrentPatchVersion[0];
            set => this.config.CurrentPatchVersion[0] = (byte)value;
        }

        /// <summary>
        /// Gets or sets the current version minor.
        /// </summary>
        [Range(0, 255)]
        public int CurrentVersionMinor
        {
            get => this.config.CurrentPatchVersion[1];
            set => this.config.CurrentPatchVersion[1] = (byte)value;
        }

        /// <summary>
        /// Gets or sets the current version patch.
        /// </summary>
        [Range(0, 255)]
        public int CurrentVersionPatch
        {
            get => this.config.CurrentPatchVersion[2];
            set => this.config.CurrentPatchVersion[2] = (byte)value;
        }
    }
}
