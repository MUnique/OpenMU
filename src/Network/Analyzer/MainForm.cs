// <copyright file="MainForm.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.Network.Analyzer
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel;
    using System.Windows.Forms;
    using MUnique.OpenMU.Network.PlugIns;

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

        private readonly PacketAnalyzer analyzer;

        private LiveConnectionListener clientListener;

        /// <summary>
        /// Initializes a new instance of the <see cref="MainForm"/> class.
        /// </summary>
        public MainForm()
        {
            this.InitializeComponent();
            this.clientBindingSource.DataSource = this.proxiedConnections;
            this.connectedClientsListBox.DisplayMember = nameof(ICapturedConnection.Name);
            this.connectedClientsListBox.Update();

            this.clientVersionComboBox.SelectedIndexChanged += (_, __) =>
            {
                var listener = this.clientListener;
                if (listener != null)
                {
                    listener.ClientVersion = this.SelectedClientVersion;
                }
            };
            this.clientVersionComboBox.DataSource = new BindingSource(this.clientVersions, null);
            this.clientVersionComboBox.DisplayMember = "Value";
            this.clientVersionComboBox.ValueMember = "Key";

            this.analyzer = new PacketAnalyzer();
            this.Disposed += (_, __) => this.analyzer.Dispose();

            this.targetHostTextBox.TextChanged += (_, __) =>
            {
                var listener = this.clientListener;
                if (listener != null)
                {
                    listener.TargetHost = this.targetHostTextBox.Text;
                }
            };
            this.targetPortNumericUpDown.ValueChanged += (_, __) =>
            {
                var listener = this.clientListener;
                if (listener != null)
                {
                    listener.TargetPort = (int)this.targetPortNumericUpDown.Value;
                }
            };
        }

        private ClientVersion SelectedClientVersion
        {
            get
            {
                if (this.clientVersionComboBox.SelectedItem is KeyValuePair<ClientVersion, string> selectedItem)
                {
                    return selectedItem.Key;
                }

                return default;
            }
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

        private void StartProxy(object sender, System.EventArgs e)
        {
            if (this.clientListener != null)
            {
                this.clientListener.Stop();
                this.clientListener = null;
                this.btnStartProxy.Text = "Start Proxy";
                return;
            }

            this.clientListener = new LiveConnectionListener((int)this.listenerPortNumericUpDown.Value, this.targetHostTextBox.Text, (int)this.targetPortNumericUpDown.Value, this.InvokeByProxy);
            this.clientListener.ClientVersion = this.SelectedClientVersion;
            this.clientListener.ClientConnected += this.ClientListenerOnClientConnected;
            this.clientListener.Start();
            this.btnStartProxy.Text = "Stop Proxy";
        }

        private void ClientListenerOnClientConnected(object sender, ClientConnectedEventArgs e)
        {
            this.InvokeByProxy(new Action(() =>
            {
                this.proxiedConnections.Add(e.Connection);
                if (this.proxiedConnections.Count == 1)
                {
                    this.connectedClientsListBox.SelectedItem = e.Connection;
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

        private void SendPacket(object sender, EventArgs e)
        {
            var index = this.connectedClientsListBox.SelectedIndex;
            if (index < 0
                || !(this.proxiedConnections[index] is LiveConnection liveConnection)
                || !liveConnection.IsConnected)
            {
                return;
            }

            var packetSender = new PacketSenderForm(liveConnection);
            packetSender.Show(this);
        }

        private void BeforeContextMenuOpens(object sender, CancelEventArgs e)
        {
            var index = this.connectedClientsListBox.SelectedIndex;
            var selectedConnection = index < 0 ? null : this.proxiedConnections[index];
            this.disconnectToolStripMenuItem.Enabled = selectedConnection is LiveConnection;
            this.saveToolStripMenuItem.Enabled = selectedConnection != null;
            this.openPacketSenderStripMenuItem.Enabled = selectedConnection is LiveConnection liveConnection && liveConnection.IsConnected;
        }
    }
}