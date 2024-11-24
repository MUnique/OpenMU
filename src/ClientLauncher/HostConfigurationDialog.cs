// <copyright file="HostConfigurationDialog.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.ClientLauncher;

using System.ComponentModel;
using System.Windows.Forms;

/// <summary>
/// Dialog for connection settings of a server.
/// </summary>
public partial class HostConfigurationDialog : Form
{
    /// <summary>
    /// Initializes a new instance of the <see cref="HostConfigurationDialog"/> class.
    /// </summary>
    public HostConfigurationDialog()
    {
        this.InitializeComponent();
    }

    /// <summary>
    /// Gets or sets the settings.
    /// </summary>
    [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
    public ServerHostSettings Settings
    {
        get
        {
            return new ServerHostSettings
            {
                Description = this._descriptionTextBox.Text,
                Address = this._serverAddressTextBox.Text,
                Port = (int)this._serverPortControl.Value,
            };
        }

        set
        {
            this._descriptionTextBox.Text = value.Description;
            this._serverAddressTextBox.Text = value.Address;
            this._serverPortControl.Value = value.Port;
        }
    }
}
