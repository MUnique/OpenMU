// <copyright file="LiveConnection.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.Network.Analyzer
{
    using System;
    using System.Buffers;
    using System.ComponentModel;
    using System.Runtime.CompilerServices;
    using log4net;

    /// <summary>
    /// A proxy which is a man-in-the-middle between a client and server connection.
    /// It allows to capture the unencrypted network traffic.
    /// </summary>
    public class LiveConnection : INotifyPropertyChanged, ICapturedConnection
    {
        /// <summary>
        /// The connection to the client.
        /// </summary>
        private readonly IConnection clientConnection;

        /// <summary>
        /// The connection to the server.
        /// </summary>
        private readonly IConnection serverConnection;

        /// <summary>
        /// The invoke action to run an action on the UI thread.
        /// </summary>
        private readonly Action<Delegate> invokeAction;

        /// <summary>
        /// The logger for this proxied connection.
        /// </summary>
        private readonly ILog log;

        private readonly string clientName;

        private string name;

        /// <summary>
        /// Initializes a new instance of the <see cref="LiveConnection"/> class.
        /// </summary>
        /// <param name="clientConnection">The client connection.</param>
        /// <param name="serverConnection">The server connection.</param>
        /// <param name="invokeAction">The invoke action to run an action on the UI thread.</param>
        public LiveConnection(IConnection clientConnection, IConnection serverConnection, Action<Delegate> invokeAction)
        {
            this.clientConnection = clientConnection;
            this.serverConnection = serverConnection;
            this.invokeAction = invokeAction;
            this.log = LogManager.GetLogger(this.GetType().Assembly, "Proxy_" + this.clientConnection);
            this.clientName = this.clientConnection.ToString();
            this.Name = this.clientName;
            this.clientConnection.PacketReceived += this.ClientPacketReceived;
            this.serverConnection.PacketReceived += this.ServerPacketReceived;
            this.clientConnection.Disconnected += this.ClientDisconnected;
            this.serverConnection.Disconnected += this.ServerDisconnected;
            this.log.Info("LiveConnection initialized.");
            this.clientConnection.BeginReceive();
            this.serverConnection.BeginReceive();
        }

        /// <summary>
        /// Occurs when a property value changes.
        /// </summary>
        public event PropertyChangedEventHandler PropertyChanged;

        /// <summary>
        /// Gets the packet list of all captured packets.
        /// </summary>
        public BindingList<Packet> PacketList { get; } = new BindingList<Packet>();

        /// <summary>
        /// Gets or sets the name of the proxied connection.
        /// </summary>
        public string Name
        {
            get => this.name;

            set
            {
                if (this.name != value)
                {
                    this.name = value;
                    this.OnPropertyChanged();
                }
            }
        }

        /// <summary>
        /// Gets a value indicating whether this instance is connected to the client and server.
        /// </summary>
        /// <value>
        ///   <c>true</c> if this instance is connected; otherwise, <c>false</c>.
        /// </value>
        public bool IsConnected => this.clientConnection.Connected && this.serverConnection.Connected;

        /// <summary>
        /// Disconnects the connection to the server and therefore indirectly also to the client.
        /// </summary>
        public void Disconnect()
        {
            this.serverConnection.Disconnect();
        }

        /// <summary>
        /// Sends data to the server.
        /// </summary>
        /// <param name="data">The data.</param>
        public void SendToServer(byte[] data)
        {
            var packet = new Packet(data, true);
            this.log.Info(packet.ToString());
            this.serverConnection.Output.Write(data);
            this.serverConnection.Output.FlushAsync();
            this.invokeAction((Action)(() => this.PacketList.Add(packet)));
        }

        /// <summary>
        /// Sends data to the client.
        /// </summary>
        /// <param name="data">The data.</param>
        public void SendToClient(byte[] data)
        {
            this.clientConnection.Output.Write(data);
            this.clientConnection.Output.FlushAsync();
            var packet = new Packet(data, false);
            this.log.Info(packet.ToString());
            this.invokeAction((Action)(() => this.PacketList.Add(packet)));
        }

        /// <summary>
        /// Called when a property value changed.
        /// </summary>
        /// <param name="propertyName">Name of the property.</param>
        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            this.invokeAction((Action)(() => this.PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName))));
        }

        private void ServerDisconnected(object sender, EventArgs e)
        {
            this.log.Info("The server connection closed.");
            this.clientConnection.Disconnect();
            this.Name = this.clientName + " [Disconnected]";
        }

        private void ClientDisconnected(object sender, EventArgs e)
        {
            this.log.Info("The client connected closed");
            this.serverConnection.Disconnect();
            this.Name = this.clientName + " [Disconnected]";
        }

        private void ServerPacketReceived(object sender, ReadOnlySequence<byte> data)
        {
            var dataAsArray = data.ToArray();
            this.SendToClient(dataAsArray);
        }

        private void ClientPacketReceived(object sender, ReadOnlySequence<byte> data)
        {
            var dataAsArray = data.ToArray();
            this.SendToServer(dataAsArray);
        }
    }
}