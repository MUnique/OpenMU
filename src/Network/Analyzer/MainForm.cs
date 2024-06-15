// <copyright file="MainForm.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.Network.Analyzer;

using System.ComponentModel;
using System.ComponentModel.Design;
using System.Diagnostics;
using System.Linq.Dynamic.Core;
using System.Linq.Expressions;
using System.Windows.Forms;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Logging.Abstractions;
using MUnique.OpenMU.Network.PlugIns;
using MUnique.OpenMU.PlugIns;
using Zuby.ADGV;

/// <summary>
/// The main form of the analyzer.
/// </summary>
public partial class MainForm : Form
{
    private readonly BindingList<ICapturedConnection> _proxiedConnections = new ();

    private readonly Dictionary<ClientVersion, string> _clientVersions = new ()
    {
        { new ClientVersion(6, 3, ClientLanguage.English), "S6E3 (1.04d)" },
        { new ClientVersion(1, 0, ClientLanguage.Invariant), "Season 1 - 6" },
        { new ClientVersion(0, 97, ClientLanguage.Invariant), "0.97" },
        { new ClientVersion(0, 95, ClientLanguage.English), "0.95d" },
        { new ClientVersion(0, 75, ClientLanguage.Invariant), "0.75" },
    };

    private readonly PacketAnalyzer _analyzer;

    private readonly PlugInManager _plugInManager;

    private LiveConnectionListener? _clientListener;
    private BindingList<Packet>? _unfilteredList;
    private Delegate? _filterMethod;
    private LambdaExpression? _filterExpression;

    /// <summary>
    /// Initializes a new instance of the <see cref="MainForm"/> class.
    /// </summary>
    public MainForm()
    {
        this.InitializeComponent();
        var serviceContainer = new ServiceContainer();
        serviceContainer.AddService(typeof(ILoggerFactory), new NullLoggerFactory());
        this._plugInManager = new PlugInManager(null, new NullLoggerFactory(), serviceContainer, null);
        this._plugInManager.DiscoverAndRegisterPlugIns();
        this.clientBindingSource.DataSource = this._proxiedConnections;
        this.connectedClientsListBox.DisplayMember = nameof(ICapturedConnection.Name);
        this.connectedClientsListBox.Update();

        this._analyzer = new PacketAnalyzer();
        this.Disposed += (_, _) => this._analyzer.Dispose();

        this.clientVersionComboBox.SelectedIndexChanged += (_, _) =>
        {
            if (this._clientListener is { } listener)
            {
                listener.ClientVersion = this.SelectedClientVersion;
            }

            this._analyzer.ClientVersion = this.SelectedClientVersion;
        };
        this.clientVersionComboBox.DataSource = new BindingSource(this._clientVersions, null);
        this.clientVersionComboBox.DisplayMember = "Value";
        this.clientVersionComboBox.ValueMember = "Key";

        this.targetHostTextBox.TextChanged += (_, _) =>
        {
            var listener = this._clientListener;
            if (listener != null)
            {
                listener.TargetHost = this.targetHostTextBox.Text;
            }
        };
        this.targetPortNumericUpDown.ValueChanged += (_, _) =>
        {
            var listener = this._clientListener;
            if (listener != null)
            {
                listener.TargetPort = (int)this.targetPortNumericUpDown.Value;
            }
        };

        this.packetGridView.FilterStringChanged += this.OnPacketFilterStringChanged;
        foreach (DataGridViewColumn column in this.packetGridView.Columns)
        {
            this.packetGridView.SetSortEnabled(column, false);
        }
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

    private ICapturedConnection? SelectedConnection
    {
        get
        {
            var index = this.connectedClientsListBox.SelectedIndex;
            if (index < 0)
            {
                return null;
            }

            return this._proxiedConnections[index];
        }
    }

    /// <inheritdoc />
    protected override void OnClosed(EventArgs e)
    {
        if (this._clientListener != null)
        {
            this._clientListener.Stop();
            this._clientListener = null;
        }

        base.OnClosed(e);
    }

    private static string ConvertFilterStringToExpressionString(string filter)
    {
        var result = new StringBuilder();

        filter = filter
            .Replace("(", string.Empty, StringComparison.InvariantCulture)
            .Replace(")", string.Empty, StringComparison.InvariantCulture);

        var andOperator = string.Empty;
        foreach (var columnFilter in filter.Split("AND"))
        {
            // Example: [Type] IN 'C1', 'C2'
            result.Append(andOperator);

            var temp1 = columnFilter.Trim().Split("IN");
            var columnName = temp1[0].Split('[', ']')[1].Trim();

            // prepare beginning of linq statement
            result.Append("(")
                .Append(columnName)
                .Append(" != null && (");

            string orOperator = string.Empty;

            var filterValues = temp1[1].Split(',').Select(v => v.Replace("\'", string.Empty).Trim());

            foreach (var filterValue in filterValues)
            {
                result.Append(orOperator).Append(columnName);
                if (double.TryParse(filterValue, out _))
                {
                    result.Append(" = ").Append(filterValue);
                }
                else
                {
                    result.Append(".Contains(\"").Append(filterValue).Append("\")");
                }

                orOperator = " OR ";
            }

            result.Append("))");

            andOperator = " AND ";
        }

        // replace all single quotes with double quotes
        return result.ToString();
    }

    private void UpdateFilterExpression()
    {
        var filterString = this.packetGridView.FilterString;
        if (string.IsNullOrEmpty(filterString))
        {
            this._filterExpression = null;
            this._filterMethod = null;
        }
        else
        {
            var filterExpressionString = ConvertFilterStringToExpressionString(filterString);
            this._filterExpression = DynamicExpressionParser.ParseLambda(ParsingConfig.Default, true, typeof(Packet), typeof(bool), filterExpressionString);
            this._filterMethod = this._filterExpression.Compile(false);
        }
    }

    private void SetPacketDataSource()
    {
        if (this._unfilteredList is { } oldList)
        {
            oldList.ListChanged -= this.OnUnfilteredListChanged;
        }

        this._unfilteredList = this.SelectedConnection?.PacketList;

        if (this._unfilteredList is null)
        {
            return;
        }

        if (this._filterExpression is null)
        {
            this.packetBindingSource.DataSource = this.SelectedConnection?.PacketList;
        }
        else
        {
            this.packetBindingSource.DataSource = this._unfilteredList.AsQueryable().Where(this._filterExpression).ToList();
            this._unfilteredList.ListChanged += this.OnUnfilteredListChanged;
        }
    }

    private void OnUnfilteredListChanged(object? sender, ListChangedEventArgs e)
    {
        if (e.ListChangedType != ListChangedType.ItemAdded
            || this._filterMethod is not { } filter
            || this._unfilteredList is not { } sourceList
            || this._unfilteredList?.Count < e.NewIndex
            || e.NewIndex < 0)
        {
            return;
        }

        var newPacket = sourceList[e.NewIndex];

        if (filter.DynamicInvoke(newPacket) is true)
        {
            this.packetBindingSource.Add(newPacket);
        }
    }

    private void OnPacketFilterStringChanged(object? sender, AdvancedDataGridView.FilterEventArgs e)
    {
        try
        {
            this.UpdateFilterExpression();
            this.SetPacketDataSource();
        }
        catch (Exception ex)
        {
            Debug.Fail(ex.Message, ex.StackTrace);
        }
    }

    private void StartProxy(object sender, System.EventArgs e)
    {
        if (this._clientListener != null)
        {
            this._clientListener.Stop();
            this._clientListener = null;
            this.btnStartProxy.Text = "Start Proxy";
            return;
        }

        this._clientListener = new LiveConnectionListener(
            (int)this.listenerPortNumericUpDown.Value,
            this.targetHostTextBox.Text,
            (int)this.targetPortNumericUpDown.Value,
            this._plugInManager,
            new NullLoggerFactory(),
            this.InvokeByProxy)
        {
            ClientVersion = this.SelectedClientVersion,
        };

        this._analyzer.ClientVersion = this.SelectedClientVersion;
        this._clientListener.ClientConnected += this.ClientListenerOnClientConnected;
        this._clientListener.Start();
        this.btnStartProxy.Text = "Stop Proxy";
    }

    private void ClientListenerOnClientConnected(object? sender, ClientConnectedEventArgs e)
    {
        this.InvokeByProxy(new Action(() =>
        {
            this._proxiedConnections.Add(e.Connection);
            if (this._proxiedConnections.Count == 1)
            {
                this.connectedClientsListBox.SelectedItem = e.Connection;
                this.OnConnectionSelected(this, EventArgs.Empty);
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

    private void OnPacketSelected(object sender, EventArgs e)
    {
        this.SuspendLayout();
        try
        {
            var rows = this.packetGridView.SelectedRows;
            if (rows.Count > 0 && this.packetGridView.SelectedRows[0].DataBoundItem is Packet packet)
            {
                this.rawDataTextBox.Text = packet.PacketData;
                this.extractedInfoTextBox.Text = this._analyzer.ExtractInformation(packet);
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

    private void OnConnectionSelected(object sender, EventArgs e)
    {
        this.SetPacketDataSource();
        if (this.SelectedConnection is { } connection)
        {
            this.trafficGroup.Enabled = true;
            this.trafficGroup.Text = $"Traffic ({connection.Name})";
        }
        else
        {
            this.trafficGroup.Enabled = false;
            this.trafficGroup.Text = "Traffic";
        }
    }

    [System.Diagnostics.CodeAnalysis.SuppressMessage("Usage", "VSTHRD100:Avoid async void methods", Justification = "Catching all Exceptions.")]
    private async void OnDisconnectClientClick(object sender, EventArgs e)
    {
        try
        {
            var index = this.connectedClientsListBox.SelectedIndex;
            if (index < 0)
            {
                return;
            }

            if (this._proxiedConnections[index] is LiveConnection proxy)
            {
                await proxy.DisconnectAsync().ConfigureAwait(false);
            }
        }
        catch
        {
            // Must be catched because the method is async void.
        }
    }

    private void OnLoadFromFileClick(object sender, EventArgs e)
    {
        using var loadFileDialog = new OpenFileDialog { Filter = "Analyzer files (*.mucap)|*.mucap" };
        if (loadFileDialog.ShowDialog(this) == DialogResult.OK)
        {
            var loadedConnection = new SavedConnection(loadFileDialog.FileName);
            if (loadedConnection.PacketList.Count == 0)
            {
                MessageBox.Show("The file couldn't be loaded. It was either empty or in a wrong format.", this.loadToolStripMenuItem.Text, MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            else
            {
                this._proxiedConnections.Add(loadedConnection);
                this.connectedClientsListBox.SelectedItem = loadedConnection;
                this.OnConnectionSelected(this, EventArgs.Empty);
            }
        }
    }

    private void OnSaveToFileClick(object sender, EventArgs e)
    {
        var index = this.connectedClientsListBox.SelectedIndex;
        if (index < 0 || this._proxiedConnections[index] is not { } capturedConnection)
        {
            return;
        }

        using var saveFileDialog = new SaveFileDialog
        {
            DefaultExt = "mucap",
            Filter = "Analyzer files (*.mucap)|*.mucap",
            AddExtension = true,
        };
        if (saveFileDialog.ShowDialog(this) == DialogResult.OK)
        {
            capturedConnection.SaveToFile(saveFileDialog.FileName);
        }
    }

    private void OnSendPacketClick(object sender, EventArgs e)
    {
        var index = this.connectedClientsListBox.SelectedIndex;
        if (index < 0
            || this._proxiedConnections[index] is not LiveConnection { IsConnected: true } liveConnection)
        {
            return;
        }

        var packetSender = new PacketSenderForm(liveConnection);
        packetSender.Show(this);
    }

    private void OnBeforeContextMenuOpens(object sender, CancelEventArgs e)
    {
        var index = this.connectedClientsListBox.SelectedIndex;
        var selectedConnection = index < 0 ? null : this._proxiedConnections[index];
        this.disconnectToolStripMenuItem.Enabled = selectedConnection is LiveConnection;
        this.saveToolStripMenuItem.Enabled = selectedConnection != null;
        this.openPacketSenderStripMenuItem.Enabled = selectedConnection is LiveConnection { IsConnected: true };
    }
}