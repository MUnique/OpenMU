// <copyright file="ServerListItem.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.ConnectServer;

using System.Net;
using MUnique.OpenMU.Network.Packets.ConnectServer;

/// <summary>
/// A list item of an available server.
/// </summary>
internal class ServerListItem : IGameServerEntry
{
    private readonly ServerList _owner;

    private byte _serverLoadPercentage;
    private int _currentConnections;

    /// <summary>
    /// Initializes a new instance of the <see cref="ServerListItem"/> class.
    /// </summary>
    /// <param name="owner">The owner.</param>
    public ServerListItem(ServerList owner)
    {
        this.LoadIndex = -1;
        this._owner = owner;
        this.ConnectInfo = new byte[] { 0xC1, 0x16, 0xF4, 0x03, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0 };
    }

    /// <summary>
    /// Gets or sets the index of the server in the load (usage rate) array.
    /// </summary>
    /// <value>
    /// The index of the server in the load (usage rate) array.
    /// </value>
    public int LoadIndex { get; set; }

    /// <summary>
    /// Gets or sets the server identifier.
    /// </summary>
    /// <value>
    /// The server identifier.
    /// </value>
    public ushort ServerId { get; set; }

    /// <summary>
    /// Gets or sets the server load (usage rate).
    /// </summary>
    public byte ServerLoadPercentage
    {
        get => this._serverLoadPercentage;

        private set
        {
            this._serverLoadPercentage = value;
            var cache = this._owner.Cache;
            if (cache != null && this.LoadIndex != -1)
            {
                cache[this.LoadIndex] = value;
            }
        }
    }

    /// <summary>
    /// Gets the connect information.
    /// </summary>
    public byte[] ConnectInfo { get; }

    /// <summary>
    /// Gets or sets the ip end point.
    /// </summary>
    public IPEndPoint EndPoint
    {
        get
        {
            ConnectionInfoRef connectInfo = this.ConnectInfo.AsSpan();
            return new IPEndPoint(IPAddress.Parse(connectInfo.IpAddress), connectInfo.Port);
        }

        set
        {
            this.ConnectInfo.AsSpan().Clear();
            ConnectionInfo connectInfo = new ConnectionInfo(this.ConnectInfo);
            connectInfo.IpAddress = value.Address.ToString();
            connectInfo.Port = (ushort)value.Port;
        }
    }

    /// <summary>
    /// Gets or sets the maximum connection count.
    /// </summary>
    public int MaximumConnections { get; set; }

    /// <summary>
    /// Gets or sets the current connection count.
    /// </summary>
    public int CurrentConnections
    {
        get => this._currentConnections;
        set
        {
            this._currentConnections = value;
            this.ServerLoadPercentage = (byte)(this._currentConnections * 100f / this.MaximumConnections);
        }
    }

    /// <inheritdoc/>
    public override string ToString()
    {
        return $"ServerId={this.ServerId}, ServerLoadPercentage={this.ServerLoadPercentage}";
    }
}