// <copyright file="MainForm.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.ClientLauncher
{
    using System;
    using System.Windows.Forms;

    /// <summary>
    /// The main form of the launcher.
    /// </summary>
    public partial class MainForm : Form
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="MainForm"/> class.
        /// </summary>
        public MainForm()
        {
            this.InitializeComponent();
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

        private void SearchMainExeButton_Click(object sender, EventArgs e)
        {
            var dialogResult = this.openFileDialog.ShowDialog(this);
            if (dialogResult == DialogResult.OK)
            {
                this.MainExePathTextBox.Text = this.openFileDialog.FileName;
            }
        }
    }
}
