// <copyright file="AsyncConnectionTest.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.Network.Tests
{
    using System;
    using System.Net;
    using System.Net.Sockets;
    using System.Threading;

    using NUnit.Framework;

    /// <summary>
    /// Test of the async connection implementation.
    /// </summary>
    [TestFixture]
    public class AsyncConnectionTest
    {
        /// <summary>
        /// Tests the receive function with an async connection object.
        /// </summary>
        [Test]
        public void TestReceive()
        {
            this.TestReceive(socket => new Connection(socket, null, null));
        }

        /// <summary>
        /// Tests the receiving of data with any <see cref="IConnection"/> implementation.
        /// </summary>
        /// <param name="connectionCreator">The connection creator.</param>
        private void TestReceive(Func<Socket, IConnection> connectionCreator)
        {
            const int maximumPacketCount = 1000;

            IConnection connection = null;
            var server = new TcpListener(IPAddress.Any, 5000);
            server.Start();
            server.BeginAcceptSocket(
                asyncResult =>
                                     {
                                         var clientSocket = server.EndAcceptSocket(asyncResult);
                                         connection = connectionCreator(clientSocket);
                                     }, null);
            using (var client = new TcpClient("127.0.0.1", 5000))
            {
                while (connection == null)
                {
                    Thread.Sleep(10);
                }

                int packetCount = 0;
                connection.PacketReceived += (sender, p) => Interlocked.Increment(ref packetCount);
                connection.BeginReceive();

                var packet = new byte[] { 0xC1, 10, 0, 0, 0, 0, 0, 0, 0, 0 };
                for (int i = 0; i < maximumPacketCount; i++)
                {
                    client.Client.BeginSend(packet, 0, packet.Length, SocketFlags.None, null, null);
                }

                while (packetCount < maximumPacketCount)
                {
                    Thread.Sleep(1);
                }
            }

            server.Stop();
        }
    }
}
