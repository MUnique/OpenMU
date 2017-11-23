// <copyright file="TestClient.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace ConnectServerTest
{
    using System;
    using System.Net.Sockets;
    using System.Timers;

    /// <summary>
    /// A test client, which sends server list request in an interval.
    /// </summary>
    public sealed class TestClient : IDisposable
    {
        private static readonly byte[] RequestServerListPacket = { 0xC1, 0x04, 0xF4, 0x06 };
        private readonly Timer timer;

        /// <summary>
        /// Initializes a new instance of the <see cref="TestClient"/> class.
        /// </summary>
        /// <param name="ip">The ip.</param>
        /// <param name="port">The port.</param>
        /// <param name="interval">The interval.</param>
        public TestClient(string ip, int port, int interval)
        {
            this.Socket = this.CreateConnection(ip, port);
            this.timer = new Timer(interval);
            this.timer.Elapsed += this.TimerElapsed;
            this.timer.Start();
        }

        /// <summary>
        /// Gets or sets the socket.
        /// </summary>
        public TcpClient Socket { get; set; }

        /// <summary>
        /// Disconnects this instance.
        /// </summary>
        public void Disconnect()
        {
            this.timer.Enabled = false;
            this.Dispose();
        }

        /// <summary>
        /// Performs application-defined tasks associated with freeing, releasing, or resetting unmanaged resources.
        /// </summary>
        public void Dispose()
        {
            this.Socket.Client.Close();
            this.timer.Dispose();
            MainForm.ConnectedClients.Remove(this);
        }

        /// <summary>
        /// Sets the timer interval.
        /// </summary>
        /// <param name="interval">The interval.</param>
        public void SetTimerInterval(int interval)
        {
            this.timer.Interval = interval;
        }

        private TcpClient CreateConnection(string ip, int port)
        {
            try
            {
                return new TcpClient(ip, port);
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
                throw;
            }
        }

        private void TimerElapsed(object sender, ElapsedEventArgs e)
        {
            try
            {
                if (this.Socket != null && this.Socket.Connected)
                {
                    this.Socket.Client.Send(RequestServerListPacket); // Request server list
                }
            }
            catch
            {
                this.Disconnect();
            }
        }
    }
}
