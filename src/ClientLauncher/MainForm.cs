// <copyright file="MainForm.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

#pragma warning disable CA1416 This project is compiled for windows
namespace MUnique.OpenMU.ClientLauncher;

using System.ComponentModel;
using System.IO;
using System.Windows.Forms;
using System.Xml.Serialization;

/// <summary>
/// The main form of the launcher.
/// </summary>
public partial class MainForm : Form
{
    private const string ConfigFileName = "launcher.config";

    /// <summary>
    /// Gets or sets the binding list for the configured hosts.
    /// </summary>
    private BindingList<ServerHostSettings> _hostsBindingList = new();

    /// <summary>
    /// Initializes a new instance of the <see cref="MainForm"/> class.
    /// </summary>
    public MainForm()
    {
        this.InitializeComponent();
        this.LoadOptions();
        this.UpdateButtonStates();
    }

    private BindingList<ServerHostSettings> Hosts
    {
        get => this._hostsBindingList;
        set
        {
            this._hostsBindingList = value;
            this._serversComboBox.DataSource = value;
        }
    }

    /// <summary>
    /// Launches the MU Online client (main.exe) to connect to the configured address.
    /// </summary>
    /// <param name="sender">The sender.</param>
    /// <param name="e">The <see cref="EventArgs"/> instance containing the event data.</param>
    private void LaunchClick(object sender, EventArgs e)
    {
        try
        {
            var selectedHost = (ServerHostSettings)this._serversComboBox.SelectedItem!;
            var launcher = new Launcher
            {
                HostAddress = selectedHost.Address,
                HostPort = selectedHost.Port,
                MainExePath = this.MainExePathTextBox.Text,
            };
            launcher.LaunchClient();

            this.SaveCurrentOptions();
        }
        catch (UnauthorizedAccessException)
        {
            MessageBox.Show("Can't access Windows Registry. To use the launcher, run it as Administrator.");
        }
        catch (Exception ex)
        {
            MessageBox.Show("Error Starting MU. Path correct?" + Environment.NewLine + ex.Message);
        }
    }

    private void LoadOptions()
    {
        this._serversComboBox.DataSource = this.Hosts;
        if (!File.Exists(ConfigFileName))
        {
            return;
        }

        try
        {
            var reader = new XmlSerializer(typeof(LauncherSettings));
            using var file = new StreamReader(ConfigFileName);
            if (reader.Deserialize(file) is LauncherSettings launcherSettings)
            {
                this.MainExePathTextBox.Text = launcherSettings.MainExePath;
                this.Hosts = new BindingList<ServerHostSettings>(launcherSettings.Hosts);
            }

            file.Close();
        }
        catch
        {
            this.Hosts.Clear();
            this.Hosts.Add(new ServerHostSettings { Description = "Local ConnectServer", Address = "localhost", Port = 44405});
            this.Hosts.Add(new ServerHostSettings { Description = "Local GameServer 1", Address = "localhost", Port = 55901 });
        }
    }

    private void SaveCurrentOptions()
    {
        var settings = new LauncherSettings
        {
            Hosts = this._hostsBindingList.ToList(),
            MainExePath = this.MainExePathTextBox.Text,
        };

        var writer = new XmlSerializer(typeof(LauncherSettings));
        using var file = File.Create(ConfigFileName);
        writer.Serialize(file, settings);
        file.Close();
    }

    private void SearchMainExeButtonClick(object sender, EventArgs e)
    {
        var dialogResult = this.openFileDialog.ShowDialog(this);
        if (dialogResult == DialogResult.OK)
        {
            this.MainExePathTextBox.Text = this.openFileDialog.FileName;
        }
    }

    private void ConfigurationDialogButtonClick(object sender, EventArgs e)
    {
        if (OperatingSystem.IsWindows())
        {
            using var configDialog = new ClientSettingsDialog();
            configDialog.ShowDialog(this);
        }
        else
        {
            MessageBox.Show("Changing the configuration of the MU game client is only supported on windows.");
        }
    }

    private void OnAddHostButtonClick(object sender, EventArgs e)
    {
        using var dialog = new HostConfigurationDialog();
        if (dialog.ShowDialog(this) == DialogResult.OK)
        {
            var settings = dialog.Settings;
            this._hostsBindingList.Add(settings);
            this.SaveCurrentOptions();
            this.UpdateButtonStates();
        }
    }

    private void OnEditHostButtonClick(object sender, EventArgs e)
    {
        var selectedConfiguration = (ServerHostSettings)this._serversComboBox.SelectedItem!;
        using var dialog = new HostConfigurationDialog();
        dialog.Settings = selectedConfiguration;

        if (dialog.ShowDialog(this) == DialogResult.OK)
        {
            var editedConfiguration = dialog.Settings;
            selectedConfiguration.Port = editedConfiguration.Port;
            selectedConfiguration.Address = editedConfiguration.Address;
            selectedConfiguration.Description = editedConfiguration.Description;
            this.SaveCurrentOptions();
        }
    }

    private void OnRemoveHostButtonClick(object sender, EventArgs e)
    {
        this._hostsBindingList.RemoveAt(this._serversComboBox.SelectedIndex);
        this.SaveCurrentOptions();
    }

    private void OnServersComboBoxSelectedIndexChanged(object sender, EventArgs e)
    {
        this.UpdateButtonStates();
    }

    private void UpdateButtonStates()
    {
        var isServerSelected = this._serversComboBox.SelectedItem is ServerHostSettings;
        this._editHostButton.Enabled = isServerSelected;
        this._removeHostButton.Enabled = isServerSelected;
        this._launchButton.Enabled = isServerSelected;
    }
}