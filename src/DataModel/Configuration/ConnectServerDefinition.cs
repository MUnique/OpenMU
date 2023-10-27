// <copyright file="ConnectServerDefinition.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.DataModel.Configuration;

using MUnique.OpenMU.Annotations;
using MUnique.OpenMU.Interfaces;

/// <summary>
/// The definition of a connect server.
/// </summary>
[AggregateRoot]
[Cloneable]
public partial class ConnectServerDefinition : IConnectServerSettings
{
    /// <summary>
    /// Gets or sets the id of this definition.
    /// </summary>
    public Guid Id { get; set; }

    /// <inheritdoc />
    public Guid ConfigurationId => this.Id;

    /// <summary>
    /// Gets or sets the server identifier.
    /// </summary>
    public byte ServerId { get; set; }

    /// <summary>
    /// Gets or sets the description of the server.
    /// </summary>
    /// <remarks>
    /// Will be displayed in the server list in the admin panel as <see cref="IManageableServer.Description"/>.
    /// </remarks>
    public string Description { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets the client which is expected to connect.
    /// </summary>
    [Required]
    public virtual GameClientDefinition? Client { get; set; }

    /// <inheritdoc/>
    IGameClientVersion IConnectServerSettings.Client => this.Client ?? throw new InvalidOperationException("ConnectServerDefinition.Client not initialized.");

    /// <summary>
    /// Gets or sets a value indicating whether the client should get disconnected when a unknown packet is getting received.
    /// </summary>
    public bool DisconnectOnUnknownPacket { get; set; }

    /// <summary>
    /// Gets or sets the maximum size of the packets which should be received from the client. If this size is exceeded, the client will be disconnected.
    /// </summary>
    /// <remarks>DOS protection.</remarks>
    public byte MaximumReceiveSize { get; set; }

    /// <summary>
    /// Gets or sets the network port on which the server is listening.
    /// </summary>
    public int ClientListenerPort { get; set; }

    /// <summary>
    /// Gets or sets the timeout after which clients without activity get disconnected.
    /// </summary>
    public TimeSpan Timeout { get; set; }

    /// <summary>
    /// Gets or sets the current patch version.
    /// </summary>
    public byte[]? CurrentPatchVersion { get; set; }

    /// <inheritdoc />
    byte[] IConnectServerSettings.CurrentPatchVersion => this.CurrentPatchVersion ?? throw new InvalidOperationException("Patch Version is not initialized");

    /// <summary>
    /// Gets or sets the patch address.
    /// </summary>
    public string PatchAddress { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets the maximum connections per ip.
    /// </summary>
    public int MaxConnectionsPerAddress { get; set; }

    /// <summary>
    /// Gets or sets a value indicating whether the <see cref="MaxConnectionsPerAddress"/> should be checked.
    /// </summary>
    public bool CheckMaxConnectionsPerAddress { get; set; }

    /// <summary>
    /// Gets or sets the maximum connections the connect server should handle.
    /// </summary>
    public int MaxConnections { get; set; }

    /// <summary>
    /// Gets or sets the listener backlog for the client listener.
    /// </summary>
    public int ListenerBacklog { get; set; }

    /// <summary>
    /// Gets or sets the maximum FTP requests per connection.
    /// </summary>
    public int MaxFtpRequests { get; set; }

    /// <summary>
    /// Gets or sets the maximum ip requests per connection.
    /// </summary>
    public int MaxIpRequests { get; set; }

    /// <summary>
    /// Gets or sets the maximum server list requests per connection.
    /// </summary>
    public int MaxServerListRequests { get; set; }
}