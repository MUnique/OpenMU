// <copyright file="MainForm.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.ClientLauncher
{
    using System;
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
        /// Initializes a new instance of the <see cref="MainForm"/> class.
        /// </summary>
        public MainForm()
        {
            this.InitializeComponent();
            this.LoadOptions();
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
                var launcher = new Launcher
                {
                    HostAddress = this.ServerAddressTextBox.Text,
                    HostPort = (int)this.ServerPortControl.Value,
                    MainExePath = this.MainExePathTextBox.Text,
                };
                launcher.LaunchClient();

                this.SaveOptions(launcher);
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
            if (!File.Exists(ConfigFileName))
            {
                return;
            }

            var reader = new XmlSerializer(typeof(Launcher));
            using (var file = new StreamReader(ConfigFileName))
            {
                var launcher = (Launcher)reader.Deserialize(file);
                this.ServerAddressTextBox.Text = launcher.HostAddress;
                this.ServerPortControl.Value = launcher.HostPort;
                this.MainExePathTextBox.Text = launcher.MainExePath;
                file.Close();
            }
        }

        private void SaveOptions(Launcher launcher)
        {
            var writer = new XmlSerializer(typeof(Launcher));
            using (var file = File.Create(ConfigFileName))
            {
                writer.Serialize(file, launcher);
                file.Close();
            }
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
            using (var configDialog = new ClientSettingsDialog())
            {
                configDialog.ShowDialog(this);
            }
        }
    }
}
