// <copyright file="LiveConnection.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.Network.Analyzer;

using System.Buffers;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using Microsoft.Extensions.Logging;

/// <summary>
/// A proxy which is a man-in-the-middle between a client and server connection.
/// It allows to capture the unencrypted network traffic.
/// </summary>
public class LiveConnection : INotifyPropertyChanged, ICapturedConnection
{
    /// <summary>
    /// The connection to the client.
    /// </summary>
    private readonly IConnection _clientConnection;

    /// <summary>
    /// The connection to the server.
    /// </summary>
    private readonly IConnection _serverConnection;

    /// <summary>
    /// The invoke action to run an action on the UI thread.
    /// </summary>
    private readonly Action<Delegate> _invokeAction;

    /// <summary>
    /// The logger for this proxied connection.
    /// </summary>
    private readonly ILogger _logger;

    private readonly string _clientName;

    private string _name = null!;

    /// <summary>
    /// Initializes a new instance of the <see cref="LiveConnection" /> class.
    /// </summary>
    /// <param name="clientConnection">The client connection.</param>
    /// <param name="serverConnection">The server connection.</param>
    /// <param name="invokeAction">The invoke action to run an action on the UI thread.</param>
    /// <param name="loggerFactory">The logger factory.</param>
    public LiveConnection(IConnection clientConnection, IConnection serverConnection, Action<Delegate> invokeAction, ILoggerFactory loggerFactory)
    {
        this._clientConnection = clientConnection;
        this._serverConnection = serverConnection;
        this._invokeAction = invokeAction;
        this._logger = loggerFactory.CreateLogger("Proxy_" + this._clientConnection);
        this._clientName = this._clientConnection.ToString()!;
        this.Name = this._clientName;
        this._clientConnection.PacketReceived += this.ClientPacketReceived;
        this._serverConnection.PacketReceived += this.ServerPacketReceived;
        this._clientConnection.Disconnected += this.ClientDisconnected;
        this._serverConnection.Disconnected += this.ServerDisconnected;
        this._logger.LogInformation("LiveConnection initialized.");
        this._clientConnection.BeginReceive();
        this._serverConnection.BeginReceive();
    }

    /// <summary>
    /// Occurs when a property value changes.
    /// </summary>
    public event PropertyChangedEventHandler? PropertyChanged;

    /// <summary>
    /// Gets the packet list of all captured packets.
    /// </summary>
    public BindingList<Packet> PacketList { get; } = new ();

    /// <summary>
    /// Gets or sets the name of the proxied connection.
    /// </summary>
    public string Name
    {
        get => this._name;

        set
        {
            if (this._name != value)
            {
                this._name = value;
                this.OnPropertyChanged();
            }
        }
    }

    /// <inheritdoc />
    public DateTime StartTimestamp { get; } = DateTime.UtcNow;

    /// <summary>
    /// Gets a value indicating whether this instance is connected to the client and server.
    /// </summary>
    /// <value>
    ///   <c>true</c> if this instance is connected; otherwise, <c>false</c>.
    /// </value>
    public bool IsConnected => this._clientConnection.Connected && this._serverConnection.Connected;

    /// <summary>
    /// Disconnects the connection to the server and therefore indirectly also to the client.
    /// </summary>
    public ValueTask Disconnect()
    {
        return this._serverConnection.DisconnectAsync();
    }

    /// <summary>
    /// Sends data to the server.
    /// </summary>
    /// <param name="data">The data.</param>
    public async ValueTask SendToServerAsync(byte[] data)
    {
        var packet = new Packet(DateTime.UtcNow - this.StartTimestamp, data, true);
        this._logger.LogInformation(packet.ToString());
        this._serverConnection.Output.Write(data);
        await this._serverConnection.Output.FlushAsync();
        this._invokeAction((Action)(() => this.PacketList.Add(packet)));
    }

    /// <summary>
    /// Sends data to the client.
    /// </summary>
    /// <param name="data">The data.</param>
    public async ValueTask SendToClientAsync(byte[] data)
    {
        this._clientConnection.Output.Write(data);
        await this._clientConnection.Output.FlushAsync();
        var packet = new Packet(DateTime.UtcNow - this.StartTimestamp, data, false);
        this._logger.LogInformation(packet.ToString());
        this._invokeAction((Action)(() => this.PacketList.Add(packet)));
    }

    /// <summary>
    /// Called when a property value changed.
    /// </summary>
    /// <param name="propertyName">Name of the property.</param>
    protected virtual void OnPropertyChanged([CallerMemberName] string? propertyName = null)
    {
        this._invokeAction((Action)(() => this.PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName))));
    }

    private async ValueTask ServerDisconnected()
    {
        this._logger.LogInformation("The server connection closed.");
        await this._clientConnection.DisconnectAsync();
        this.Name = this._clientName + " [Disconnected]";
    }

    private async ValueTask ClientDisconnected()
    {
        this._logger.LogInformation("The client connected closed");
        await this._serverConnection.DisconnectAsync();
        this.Name = this._clientName + " [Disconnected]";
    }

    private async ValueTask ServerPacketReceived(ReadOnlySequence<byte> data)
    {
        var dataAsArray = data.ToArray();
        this.SendToClientAsync(dataAsArray);
    }

    private async ValueTask ClientPacketReceived(ReadOnlySequence<byte> data)
    {
        var dataAsArray = data.ToArray();
        this.SendToServerAsync(dataAsArray);
    }
}