// <copyright file="MainForm.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.Network.Analyzer
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel;
    using System.IO;
    using System.Linq;
    using System.Net.Sockets;
    using System.Text;
    using System.Windows.Forms;
    using MUnique.OpenMU.Network.SimpleModulus;
    using MUnique.OpenMU.Network.Xor;

    /// <summary>
    /// The main form of the analyzer.
    /// </summary>
    public partial class MainForm : Form
    {
        private const string ClientToServerPacketsFile = "ClientToServerPackets.xml";
        private const string ServerToClientPacketsFile = "ServerToClientPackets.xml";

        private readonly IList<Proxy> proxiedConnections = new BindingList<Proxy>();

        private PacketDefinitions clientPacketDefinitions;

        private PacketDefinitions serverPacketDefinitions;

        private Listener clientListener;

        /// <summary>
        /// Initializes a new instance of the <see cref="MainForm"/> class.
        /// </summary>
        public MainForm()
        {
            this.InitializeComponent();
            this.clientBindingSource.DataSource = this.proxiedConnections;
            this.connectedClientsListBox.DisplayMember = nameof(Proxy.Name);
            this.connectedClientsListBox.Update();

            this.LoadAndWatchConfiguration(def => this.serverPacketDefinitions = def, ServerToClientPacketsFile);
            this.LoadAndWatchConfiguration(def => this.clientPacketDefinitions = def, ClientToServerPacketsFile);
        }

        /// <inheritdoc />
        protected override void OnClosed(EventArgs e)
        {
            if (this.clientListener != null)
            {
                this.clientListener.Stop();
                this.clientListener = null;
            }

            base.OnClosed(e);
        }

        private void LoadAndWatchConfiguration(Action<PacketDefinitions> assignAction, string fileName)
        {
            assignAction(PacketDefinitions.Load(fileName));
            var watcher = new FileSystemWatcher(Environment.CurrentDirectory, fileName);

            watcher.Changed += (sender, args) =>
            {
                PacketDefinitions definitions;
                try
                {
                    definitions = PacketDefinitions.Load(fileName);
                }
                catch
                {
                    // I know, bad practise... but when it fails, because of some invalid xml file, we just don't assign it.
                    return;
                }

                if (definitions != null)
                {
                    assignAction(definitions);
                }
            };

            watcher.EnableRaisingEvents = true;

            this.components.Add(watcher);
        }

        private void StartProxy(object sender, System.EventArgs e)
        {
            if (this.clientListener != null)
            {
                this.clientListener.Stop();
                this.clientListener = null;
                this.btnStartProxy.Text = "Start Proxy";
                return;
            }

            this.clientListener = new Listener(
                (int)this.numGSPort.Value,
                () => new Decryptor(),
                () => new Encryptor());
            this.clientListener.ClientAccepted += this.ClientConnected;
            this.clientListener.Start();
            this.btnStartProxy.Text = "Stop Proxy";
        }

        private void ClientConnected(object sender, ClientAcceptEventArgs e)
        {
            var clientConnection = e.AcceptedConnection;
            var serverSocket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
            serverSocket.Connect(this.txtOtherServer.Text, (int)this.numRealGSPort.Value);
            var encryptor = new ComposableEncryptor().AddEncryptor(new Xor32Encryptor()).AddEncryptor(new SimpleModulusEncryptor(SimpleModulusEncryptor.DefaultClientKey));
            var decryptor = new ComposableDecryptor().AddDecryptor(new SimpleModulusDecryptor(SimpleModulusDecryptor.DefaultClientKey) { AcceptWrongBlockChecksum = true });
            var serverConnection = new Connection(serverSocket, encryptor, decryptor);
            var proxy = new Proxy(clientConnection, serverConnection, this.InvokeByProxy);
            this.InvokeByProxy(new Action(() =>
            {
                this.proxiedConnections.Add(proxy);
                if (this.proxiedConnections.Count == 1)
                {
                    this.connectedClientsListBox.SelectedItem = proxy;
                    this.ConnectionSelected(this, EventArgs.Empty);
                }
            }));
        }

        private void InvokeByProxy(Delegate action)
        {
            if (this.Disposing || this.IsDisposed)
            {
                return;
            }

            try
            {
                this.Invoke(action);
            }
            catch (ObjectDisposedException)
            {
                // the application is probably just closing down... so swallow this error.
            }
        }

        private void PacketSelected(object sender, System.EventArgs e)
        {
            var rows = this.packetGridView.SelectedRows;
            if (rows.Count > 0)
            {
                var packet = this.packetGridView.SelectedRows[0].DataBoundItem as Packet;
                this.rawDataTextBox.Text = packet?.PacketData;
                this.extractedInfoTextBox.Text = this.ExtractInformations(packet);
                this.packetInfoGroup.Enabled = true;
            }
            else
            {
                this.packetInfoGroup.Enabled = false;
                this.rawDataTextBox.Text = string.Empty;
                this.extractedInfoTextBox.Text = string.Empty;
            }
        }

        private string ExtractInformations(Packet packet)
        {
            var definitions = packet.ToServer ? this.clientPacketDefinitions : this.serverPacketDefinitions;
            var definition = definitions.Packets.FirstOrDefault(p => (byte)p.Type == packet.Type && p.Code == packet.Code && (!p.SubCodeSpecified || p.SubCode == packet.SubCode));
            if (definition != null)
            {
                var stringBuilder = new StringBuilder()
                    .Append(definition.Name);
                foreach (var field in definition.Fields)
                {
                    stringBuilder.Append(Environment.NewLine)
                        .Append(field.Name).Append(": ").Append(packet.ExtractFieldValue(field));
                }

                return stringBuilder.ToString();
            }

            return string.Empty;
        }

        private void ConnectionSelected(object sender, System.EventArgs e)
        {
            var index = this.connectedClientsListBox.SelectedIndex;
            if (index < 0)
            {
                this.packetBindingSource.DataSource = null;
                this.trafficGroup.Enabled = false;
                this.trafficGroup.Text = "Traffic";
            }
            else
            {
                var proxy = this.proxiedConnections[index];
                this.packetBindingSource.DataSource = proxy.PacketList;
                this.trafficGroup.Enabled = true;
                this.trafficGroup.Text = $"Traffic ({proxy.Name})";
            }
        }

        private void DisconnectClient(object sender, EventArgs e)
        {
            var index = this.connectedClientsListBox.SelectedIndex;
            if (index < 0)
            {
                return;
            }

            var proxy = this.proxiedConnections[index];
            proxy.Disconnect();
        }

        private void BeforeContextMenuOpens(object sender, CancelEventArgs e)
        {
            e.Cancel = this.connectedClientsListBox.SelectedIndex < 0;
        }
    }
}