// <copyright file="MainForm.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.Network.Analyzer
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel;
    using System.IO.Pipelines;
    using System.Net.Sockets;
    using System.Windows.Forms;
    using MUnique.OpenMU.Network.PlugIns;
    using MUnique.OpenMU.PlugIns;
    using Pipelines.Sockets.Unofficial;

    /// <summary>
    /// The main form of the analyzer.
    /// </summary>
    public partial class MainForm : Form
    {
        private readonly IList<ICapturedConnection> proxiedConnections = new BindingList<ICapturedConnection>();

        private readonly Dictionary<ClientVersion, string> clientVersions = new Dictionary<ClientVersion, string>
        {
            { new ClientVersion(6, 3, ClientLanguage.English), "S6E3 (1.04d)" },
            { new ClientVersion(0, 0, ClientLanguage.Invariant), "Season 0 - 6" },
            { new ClientVersion(0, 75, ClientLanguage.Invariant), "0.75" },
        };

        private readonly PlugInManager plugInManager;

        private readonly PacketAnalyzer analyzer;

        private Listener clientListener;

        private ClientVersion clientVersion;

        /// <summary>
        /// Initializes a new instance of the <see cref="MainForm"/> class.
        /// </summary>
        public MainForm()
        {
            this.InitializeComponent();
            this.clientBindingSource.DataSource = this.proxiedConnections;
            this.connectedClientsListBox.DisplayMember = nameof(ICapturedConnection.Name);
            this.connectedClientsListBox.Update();

            this.plugInManager = new PlugInManager();
            this.plugInManager.DiscoverAndRegisterPlugIns();
            this.clientVersionComboBox.SelectedIndexChanged += (sender, args) => this.clientVersion = ((KeyValuePair<ClientVersion, string>)this.clientVersionComboBox.SelectedItem).Key;
            this.clientVersionComboBox.DataSource = new BindingSource(this.clientVersions, null);
            this.clientVersionComboBox.DisplayMember = "Value";
            this.clientVersionComboBox.ValueMember = "Key";

            this.analyzer = new PacketAnalyzer();
            this.Disposed += (_, __) => this.analyzer.Dispose();
        }

        private INetworkEncryptionFactoryPlugIn NetworkEncryptionPlugIn =>
            this.plugInManager.GetStrategy<ClientVersion, INetworkEncryptionFactoryPlugIn>(this.clientVersion)
            ?? this.plugInManager.GetStrategy<ClientVersion, INetworkEncryptionFactoryPlugIn>(default);

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

        private IPipelinedEncryptor GetEncryptor(PipeWriter pipeWriter, DataDirection direction) => this.NetworkEncryptionPlugIn.CreateEncryptor(pipeWriter, direction);

        private IPipelinedDecryptor GetDecryptor(PipeReader pipeReader, DataDirection direction) => this.NetworkEncryptionPlugIn.CreateDecryptor(pipeReader, direction);

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
                reader => this.GetDecryptor(reader, DataDirection.ClientToServer),
                writer => this.GetEncryptor(writer, DataDirection.ServerToClient));
            this.clientListener.ClientAccepted += this.ClientConnected;
            this.clientListener.Start();
            this.btnStartProxy.Text = "Stop Proxy";
        }

        private void ClientConnected(object sender, ClientAcceptEventArgs e)
        {
            var clientConnection = e.AcceptedConnection;
            var serverSocket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
            serverSocket.Connect(this.txtOtherServer.Text, (int)this.numRealGSPort.Value);
            var socketConnection = SocketConnection.Create(serverSocket);

            var decryptor = this.GetDecryptor(socketConnection.Input, DataDirection.ServerToClient);
            var encryptor = this.GetEncryptor(socketConnection.Output, DataDirection.ClientToServer);
            var serverConnection = new Connection(socketConnection, decryptor, encryptor);
            var proxy = new LiveConnection(clientConnection, serverConnection, this.InvokeByProxy);
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
            this.SuspendLayout();
            try
            {
                var rows = this.packetGridView.SelectedRows;
                if (rows.Count > 0 && this.packetGridView.SelectedRows[0].DataBoundItem is Packet packet)
                {
                    this.rawDataTextBox.Text = packet.PacketData;
                    this.extractedInfoTextBox.Text = this.analyzer.ExtractInformation(packet);
                    this.packetInfoGroup.Enabled = true;
                }
                else
                {
                    this.packetInfoGroup.Enabled = false;
                    this.rawDataTextBox.Text = string.Empty;
                    this.extractedInfoTextBox.Text = string.Empty;
                }
            }
            finally
            {
                this.ResumeLayout();
            }
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

            var proxy = this.proxiedConnections[index] as LiveConnection;
            proxy?.Disconnect();
        }

        private void LoadFromFile(object sender, EventArgs e)
        {
            using (var loadFileDialog = new OpenFileDialog())
            {
                loadFileDialog.Filter = "Analyzer files (*.mucap)|*.mucap";
                if (loadFileDialog.ShowDialog(this) == DialogResult.OK)
                {
                    var loadedConnection = new SavedConnection(loadFileDialog.FileName);
                    if (loadedConnection.PacketList.Count == 0)
                    {
                        MessageBox.Show("The file couldn't be loaded. It was either empty or in a wrong format.", this.loadToolStripMenuItem.Text, MessageBoxButtons.OK, MessageBoxIcon.Error);
                    }
                    else
                    {
                        this.proxiedConnections.Add(loadedConnection);
                        this.connectedClientsListBox.SelectedItem = loadedConnection;
                        this.ConnectionSelected(this, EventArgs.Empty);
                    }
                }
            }
        }

        private void SaveToFile(object sender, EventArgs e)
        {
            var index = this.connectedClientsListBox.SelectedIndex;
            if (index < 0 || !(this.proxiedConnections[index] is ICapturedConnection capturedConnection))
            {
                return;
            }

            using (var saveFileDialog = new SaveFileDialog())
            {
                saveFileDialog.DefaultExt = "mucap";
                saveFileDialog.Filter = "Analyzer files (*.mucap)|*.mucap";
                saveFileDialog.AddExtension = true;
                if (saveFileDialog.ShowDialog(this) == DialogResult.OK)
                {
                    capturedConnection.SaveToFile(saveFileDialog.FileName);
                }
            }
        }

        private void BeforeContextMenuOpens(object sender, CancelEventArgs e)
        {
            var index = this.connectedClientsListBox.SelectedIndex;
            var selectedConnection = index < 0 ? null : this.proxiedConnections[index];
            this.disconnectToolStripMenuItem.Enabled = selectedConnection is LiveConnection;
            this.saveToolStripMenuItem.Enabled = selectedConnection != null;
        }
    }
}