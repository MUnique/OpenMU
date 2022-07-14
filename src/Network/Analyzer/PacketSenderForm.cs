// <copyright file="PacketSenderForm.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.Network.Analyzer;

using System.ComponentModel;
using System.Diagnostics;
using System.Windows.Forms;

/// <summary>
/// A simple packet sender form which allows to send data packets to the client or the server.
/// </summary>
public partial class PacketSenderForm : Form
{
    private readonly LiveConnection _connection = null!;

    /// <summary>
    /// Initializes a new instance of the <see cref="PacketSenderForm"/> class.
    /// </summary>
    public PacketSenderForm()
    {
        this.InitializeComponent();
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="PacketSenderForm"/> class.
    /// </summary>
    /// <param name="connection">The connection.</param>
    public PacketSenderForm(LiveConnection connection)
    {
        this.InitializeComponent();
        this._connection = connection;

        this.SetText();

        this.sendButton.Click += this.SendButtonClick;
        connection.PropertyChanged += this.ConnectionOnPropertyChanged;
    }

    private void ConnectionOnPropertyChanged(object? sender, PropertyChangedEventArgs e)
    {
        if (!this.InvokeRequired)
        {
            this.SetText();
            return;
        }

        if (this.IsHandleCreated)
        {
            this.Invoke((Action)this.SetText);
        }
    }

    private void SetText()
    {
        this.Text = $"Packet Sender ({this._connection.Name})";
        if (!this._connection.IsConnected)
        {
            this.sendButton.Enabled = false;
        }
    }

    [System.Diagnostics.CodeAnalysis.SuppressMessage("Usage", "VSTHRD100:Avoid async void methods", Justification = "Catching all Exceptions.")]
    private async void SendButtonClick(object? sender, EventArgs e)
    {
        try
        {
            foreach (var line in this.packetTextBox.Lines.Where(l => !string.IsNullOrEmpty(l.Trim())))
            {
                if (!CapturedConnectionExtensions.TryParseArray(line.Trim(), out var data))
                {
                    MessageBox.Show("Wrong format. Please enter the packet in hex format with a space between each byte. \n Example: C1 03 30", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return;
                }

                if (!this._connection.IsConnected)
                {
                    return;
                }

                if (this.toClientRadioButton.Checked)
                {
                    await this._connection.SendToClientAsync(data);
                }
                else
                {
                    await this._connection.SendToServerAsync(data);
                }
            }
        }
        catch (Exception ex)
        {
            Debug.Fail(ex.Message, ex.StackTrace);
        }
    }
}