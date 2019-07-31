// <copyright file="ClientSettingsDialog.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.ClientLauncher
{
    using System;
    using System.Drawing;
    using System.Windows.Forms;

    /// <summary>
    /// <see cref="ClientSettingsDialog"/>.
    /// </summary>
    internal partial class ClientSettingsDialog : Form
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="ClientSettingsDialog"/> class.
        /// </summary>
        public ClientSettingsDialog()
        {
            this.InitializeComponent();
            this.Icon = Icon.FromHandle(Properties.Resources.Settings_16x.GetHicon());
            var config = new ClientSettings();
            config.Load();
            this.ReadConfig(config);
        }

        private void SaveButtonClick(object sender, EventArgs e)
        {
            var config = new ClientSettings();

            this.SetConfig(config);
            config.Save();
        }

        private void ReadConfig(ClientSettings clientSettings)
        {
            this.colorDepthComboBox.SelectedIndex = clientSettings.ClientColorDepth == 0 ? 0 : 1;
            this.musicActiveCheckBox.Checked = clientSettings.IsMusicEnabled;
            this.soundActiveCheckBox.Checked = clientSettings.IsSoundEnabled;
            this.soundVolumeTrackBar.Value = clientSettings.VolumeLevel;
            this.windowModeCheckBox.Checked = clientSettings.IsWindowModeActive;
            this.clientResolutionComboBox.SelectedIndex = (int)clientSettings.Resolution;
            this.clientLanguageComboBox.SelectedIndex = (int)clientSettings.LangSelection;
        }

        private void SetConfig(ClientSettings clientSettings)
        {
            clientSettings.ClientColorDepth = (ClientColorDepth)this.colorDepthComboBox.SelectedIndex;
            clientSettings.IsMusicEnabled = this.musicActiveCheckBox.Checked;
            clientSettings.IsSoundEnabled = this.soundActiveCheckBox.Checked;
            clientSettings.VolumeLevel = this.soundVolumeTrackBar.Value;
            clientSettings.IsWindowModeActive = this.windowModeCheckBox.Checked;
            clientSettings.Resolution = (ClientResolution)this.clientResolutionComboBox.SelectedIndex;
            clientSettings.LangSelection = (ClientLanguage)this.clientLanguageComboBox.SelectedIndex;
        }
    }
}
