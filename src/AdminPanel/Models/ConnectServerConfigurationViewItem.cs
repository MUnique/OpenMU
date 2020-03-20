// <copyright file="ConnectServerConfigurationViewItem.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.AdminPanel.Models
{
    using System;
    using System.ComponentModel;
    using System.ComponentModel.DataAnnotations;
    using MUnique.OpenMU.DataModel.Configuration;
    using MUnique.OpenMU.Persistence;

    /// <summary>
    /// A view item for the <see cref="ConnectServerDefinition"/>.
    /// </summary>
    public class ConnectServerConfigurationViewItem
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="ConnectServerConfigurationViewItem"/> class.
        /// </summary>
        /// <param name="config">The configuration.</param>
        public ConnectServerConfigurationViewItem(ConnectServerDefinition config)
        {
            this.Configuration = config;
        }

        /// <summary>
        /// Gets the internal id of the configuration.
        /// </summary>
        [Browsable(false)]
        public Guid Id => this.Configuration.GetId();

        /// <summary>
        /// Gets the underlying configuration object.
        /// </summary>
        [Browsable(false)]
        public ConnectServerDefinition Configuration { get; }

        /// <summary>
        /// Gets or sets the description of the server.
        /// </summary>
        /// <remarks>
        /// Will be displayed in the server list in the admin panel as <see cref="P:MUnique.OpenMU.Interfaces.IManageableServer.Description" />.
        /// </remarks>
        [Required]
        public string Description
        {
            get => this.Configuration.Description;
            set => this.Configuration.Description = value;
        }

        /// <summary>
        /// Gets or sets the game client definition.
        /// </summary>
        [Required]
        public GameClientDefinition GameClientDefinition
        {
            get => this.Configuration.Client;
            set => this.Configuration.Client = value;
        }

        /// <summary>
        /// Gets or sets a value indicating whether the client should get disconnected when a unknown packet is getting received.
        /// </summary>
        public bool DisconnectOnUnknownPacket
        {
            get => this.Configuration.DisconnectOnUnknownPacket;
            set => this.Configuration.DisconnectOnUnknownPacket = value;
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
            get => this.Configuration.MaximumReceiveSize;
            set => this.Configuration.MaximumReceiveSize = (byte)value;
        }

        /// <summary>
        /// Gets or sets the network port on which the server is listening.
        /// </summary>
        [Range(1, 65535)]
        public int ClientListenerPort
        {
            get => this.Configuration.ClientListenerPort;
            set => this.Configuration.ClientListenerPort = (ushort)value;
        }

        /// <summary>
        /// Gets or sets the patch address.
        /// </summary>
        public string PatchAddress
        {
            get => this.Configuration.PatchAddress;
            set => this.Configuration.PatchAddress = value;
        }

        /// <summary>
        /// Gets or sets the maximum connections per ip.
        /// </summary>
        [Range(1, int.MaxValue)]
        public int MaxConnectionsPerAddress
        {
            get => this.Configuration.MaxConnectionsPerAddress;
            set => this.Configuration.MaxConnectionsPerAddress = value;
        }

        /// <summary>
        /// Gets or sets a value indicating whether the <see cref="P:MUnique.OpenMU.DataModel.Configuration.ConnectServerDefinition.MaxConnectionsPerAddress" /> should be checked.
        /// </summary>
        public bool CheckMaxConnectionsPerAddress
        {
            get => this.Configuration.CheckMaxConnectionsPerAddress;
            set => this.Configuration.CheckMaxConnectionsPerAddress = value;
        }

        /// <summary>
        /// Gets or sets the maximum connections the connect server should handle.
        /// </summary>
        [Range(1, int.MaxValue)]
        public int MaxConnections
        {
            get => this.Configuration.MaxConnections;
            set => this.Configuration.MaxConnections = value;
        }

        /// <summary>
        /// Gets or sets the listener backlog for the client listener.
        /// </summary>
        [Range(1, int.MaxValue)]
        public int ListenerBacklog
        {
            get => this.Configuration.ListenerBacklog;
            set => this.Configuration.ListenerBacklog = value;
        }

        /// <summary>
        /// Gets or sets the maximum FTP requests per connection.
        /// </summary>
        [Range(1, int.MaxValue)]
        public int MaxFtpRequests
        {
            get => this.Configuration.MaxFtpRequests;
            set => this.Configuration.MaxFtpRequests = value;
        }

        /// <summary>
        /// Gets or sets the maximum ip requests per connection.
        /// </summary>
        [Range(1, int.MaxValue)]
        public int MaxIpRequests
        {
            get => this.Configuration.MaxIpRequests;
            set => this.Configuration.MaxIpRequests = value;
        }

        /// <summary>
        /// Gets or sets the maximum server list requests per connection.
        /// </summary>
        [Range(1, int.MaxValue)]
        public int MaxServerListRequests
        {
            get => this.Configuration.MaxServerListRequests;
            set => this.Configuration.MaxServerListRequests = value;
        }

        /// <summary>
        /// Gets or sets the timeout in seconds.
        /// </summary>
        [Range(10, 3600)]
        public int TimeoutSeconds
        {
            get => (int)this.Configuration.Timeout.TotalSeconds;
            set => this.Configuration.Timeout = new TimeSpan(value * TimeSpan.TicksPerSecond);
        }

        /// <summary>
        /// Gets or sets the current version major.
        /// </summary>
        [Range(0, 255)]
        public int CurrentVersionMajor
        {
            get => this.Configuration.CurrentPatchVersion[0];
            set => this.Configuration.CurrentPatchVersion[0] = (byte)value;
        }

        /// <summary>
        /// Gets or sets the current version minor.
        /// </summary>
        [Range(0, 255)]
        public int CurrentVersionMinor
        {
            get => this.Configuration.CurrentPatchVersion[1];
            set => this.Configuration.CurrentPatchVersion[1] = (byte)value;
        }

        /// <summary>
        /// Gets or sets the current version patch.
        /// </summary>
        [Range(0, 255)]
        public int CurrentVersionPatch
        {
            get => this.Configuration.CurrentPatchVersion[2];
            set => this.Configuration.CurrentPatchVersion[2] = (byte)value;
        }
    }
}
