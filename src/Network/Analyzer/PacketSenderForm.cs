// <copyright file="PacketSenderForm.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.Network.Analyzer
{
    using System;
    using System.ComponentModel;
    using System.Linq;
    using System.Windows.Forms;

    /// <summary>
    /// A simple packet sender form which allows to send data packets to the client or the server.
    /// </summary>
    /// <seealso cref="System.Windows.Forms.Form" />
    public class PacketSenderForm : Form
    {
        private readonly LiveConnection connection;

        /// <summary>
        /// Required designer variable.
        /// </summary>
#pragma warning disable 649
        private readonly IContainer components;
#pragma warning restore 649

        private TextBox packetTextBox;
        private Button sendButton;
        private RadioButton toClientRadioButton;
        private RadioButton toServerRadioButton;

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
            this.connection = connection;

            this.SetText();

            connection.PropertyChanged += this.ConnectionOnPropertyChanged;
        }

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                this.connection.PropertyChanged -= this.ConnectionOnPropertyChanged;
                this.components?.Dispose();
            }

            base.Dispose(disposing);
        }

        private void ConnectionOnPropertyChanged(object sender, PropertyChangedEventArgs e)
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
            this.Text = $"Packet Sender ({this.connection.Name})";
            if (!this.connection.IsConnected)
            {
                this.sendButton.Enabled = false;
            }
        }

        private void SendButtonClick(object sender, EventArgs e)
        {
            foreach (var line in this.packetTextBox.Lines.Where(l => !string.IsNullOrEmpty(l.Trim())))
            {
                if (!CapturedConnectionExtensions.TryParseArray(line.Trim(), out var data))
                {
                    MessageBox.Show("Wrong format. Please enter the packet in hex format with a space between each byte. \n Example: C1 03 30", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return;
                }

                if (!this.connection.IsConnected)
                {
                    return;
                }

                if (this.toClientRadioButton.Checked)
                {
                    this.connection.SendToClient(data);
                }
                else
                {
                    this.connection.SendToServer(data);
                }
            }
        }

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(PacketSenderForm));
            this.packetTextBox = new System.Windows.Forms.TextBox();
            this.sendButton = new System.Windows.Forms.Button();
            this.toClientRadioButton = new System.Windows.Forms.RadioButton();
            this.toServerRadioButton = new System.Windows.Forms.RadioButton();
            this.SuspendLayout();
            // 
            // packetTextBox
            // 
            this.packetTextBox.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.packetTextBox.ForeColor = System.Drawing.SystemColors.ControlText;
            this.packetTextBox.Location = new System.Drawing.Point(12, 12);
            this.packetTextBox.Multiline = true;
            this.packetTextBox.Name = "packetTextBox";
            this.packetTextBox.ScrollBars = System.Windows.Forms.ScrollBars.Both;
            this.packetTextBox.Size = new System.Drawing.Size(567, 76);
            this.packetTextBox.TabIndex = 4;
            // 
            // sendButton
            // 
            this.sendButton.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.sendButton.ForeColor = System.Drawing.SystemColors.ControlText;
            this.sendButton.Location = new System.Drawing.Point(504, 94);
            this.sendButton.Name = "sendButton";
            this.sendButton.Size = new System.Drawing.Size(75, 23);
            this.sendButton.TabIndex = 7;
            this.sendButton.Text = "Send";
            this.sendButton.UseVisualStyleBackColor = true;
            this.sendButton.Click += this.SendButtonClick;
            // 
            // toClientRadioButton
            // 
            this.toClientRadioButton.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.toClientRadioButton.AutoSize = true;
            this.toClientRadioButton.Checked = true;
            this.toClientRadioButton.ForeColor = System.Drawing.SystemColors.ControlText;
            this.toClientRadioButton.Location = new System.Drawing.Point(12, 100);
            this.toClientRadioButton.Name = "toClientRadioButton";
            this.toClientRadioButton.Size = new System.Drawing.Size(67, 17);
            this.toClientRadioButton.TabIndex = 6;
            this.toClientRadioButton.TabStop = true;
            this.toClientRadioButton.Text = "To Client";
            this.toClientRadioButton.UseVisualStyleBackColor = true;
            // 
            // toServerRadioButton
            // 
            this.toServerRadioButton.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.toServerRadioButton.AutoSize = true;
            this.toServerRadioButton.ForeColor = System.Drawing.SystemColors.ControlText;
            this.toServerRadioButton.Location = new System.Drawing.Point(84, 100);
            this.toServerRadioButton.Name = "toServerRadioButton";
            this.toServerRadioButton.Size = new System.Drawing.Size(72, 17);
            this.toServerRadioButton.TabIndex = 5;
            this.toServerRadioButton.Text = "To Server";
            this.toServerRadioButton.UseVisualStyleBackColor = true;
            // 
            // PacketSenderForm
            // 
            this.ClientSize = new System.Drawing.Size(591, 129);
            this.Controls.Add(this.sendButton);
            this.Controls.Add(this.toClientRadioButton);
            this.Controls.Add(this.toServerRadioButton);
            this.Controls.Add(this.packetTextBox);
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.Name = "PacketSenderForm";
            this.ResumeLayout(false);
            this.PerformLayout();
        }
    }
}