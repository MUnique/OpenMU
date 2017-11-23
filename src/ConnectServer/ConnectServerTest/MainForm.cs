// <copyright file="MainForm.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace ConnectServerTest
{
    using System;
    using System.Collections.Generic;
    using System.Windows.Forms;

    /// <summary>
    /// The main form for testing.
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
        /// Gets or sets the connected clients.
        /// </summary>
        /// <value>
        /// The connected clients.
        /// </value>
        public static IList<TestClient> ConnectedClients { get; set; } = new List<TestClient>();

        private void BtnStart_Click(object sender, EventArgs e)
        {
            for (int i = 0; i < this.numClients.Value; ++i)
            {
                try
                {
                    var testClient = new TestClient(this.txtIP.Text, (int)this.numPort.Value, (int)this.numInterval.Value);
                    ConnectedClients.Add(testClient);
                }
                catch
                {
                    break;
                }
            }
        }

        private void BtnStop_Click(object sender, EventArgs e)
        {
            while (ConnectedClients.Count > 0)
            {
                ConnectedClients[ConnectedClients.Count - 1]?.Disconnect();
            }
        }

        private void Timer1_Tick(object sender, EventArgs e)
        {
            this.lblClients.Text = ConnectedClients.Count.ToString();
        }

        private void Button1_Click(object sender, EventArgs e)
        {
            var s = new TestClient(this.txtIP.Text, (int)this.numPort.Value, (int)this.numInterval.Value);
            var dummy = new byte[1000];
            s.Socket.Client.Receive(dummy);
            s.Socket.Client.Send(new byte[] { 0xC1, 0x04, 0xF4, 0x06 });
            var start = Environment.TickCount;
            s.Socket.Client.Receive(dummy);
            var end = Environment.TickCount - start;
            this.label6.Text = "Response Time: " + end.ToString() + "ms";
            s.Disconnect();
        }
    }
}
