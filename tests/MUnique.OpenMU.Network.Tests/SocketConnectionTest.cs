// <copyright file="SocketConnectionTest.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.Network.Tests
{
    using System;
    using System.Net;
    using System.Net.Sockets;
    using System.Threading;
    using System.Threading.Tasks;
    using NUnit.Framework;
    using Pipelines.Sockets.Unofficial;

    /// <summary>
    /// Test of the async connection implementation.
    /// </summary>
    [TestFixture]
    [Ignore("It's using real sockets")]
    public class SocketConnectionTest
    {
        /// <summary>
        /// Tests the receive function with a pipelined connection object.
        /// </summary>
        [Test]
        public void TestReceivePipelined()
        {
            this.TestReceivePipelined(socket => new Connection(SocketConnection.Create(socket), null, null));
        }

        /// <summary>
        /// Tests the receive function with a pipelined connection object with encryptor/decryptor.
        /// </summary>
        [Test]
        public void TestReceivePipelinedWithEncryption()
        {
            this.TestReceivePipelined(socket =>
            {
                var socketConnection = SocketConnection.Create(socket);
                return new Connection(socketConnection, new PipelinedDecryptor(socketConnection.Input), new PipelinedEncryptor(socketConnection.Output));
            });
        }

        /// <summary>
        /// Tests the receiving of data with any <see cref="IConnection"/> implementation.
        /// </summary>
        /// <param name="connectionCreator">The connection creator.</param>
        private void TestReceivePipelined(Func<Socket, IConnection> connectionCreator)
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

        /// <summary>
        /// Tests if the connection is disconnected after sending invalid data.
        /// </summary>
        [Test]
        public async Task TestDisconnectOnInvalidHeaderSent()
        {
            IConnection connection = null;
            var server = new TcpListener(IPAddress.Any, 5000);
            server.Start();
            try
            {
                server.BeginAcceptSocket(
                    asyncResult =>
                    {
                        var clientSocket = server.EndAcceptSocket(asyncResult);
                        var socketConnection = SocketConnection.Create(clientSocket);
                        connection = new Connection(socketConnection, new PipelinedDecryptor(socketConnection.Input), new PipelinedEncryptor(socketConnection.Output));
                    }, null);

                using var client = new TcpClient("127.0.0.1", 5000);
                while (connection == null)
                {
                    Thread.Sleep(10);
                }

                connection.BeginReceive();

                var packet = new byte[22222];
                packet[0] = 0xDE;
                packet[1] = 0xAD;
                packet[2] = 0xBE;
                packet[3] = 0xAF;
                await connection.Output.WriteAsync(packet).ConfigureAwait(false);
                await Task.Delay(1000).ConfigureAwait(false);

                Assert.That(connection.Connected, Is.False);
            }
            finally
            {
                server.Stop();
            }
        }

        /// <summary>
        /// Tests if the connection is disconnected after receiving invalid data.
        /// </summary>
        [Test]
        public void TestDisconnectOnInvalidHeaderReceived()
        {
            var server = new TcpListener(IPAddress.Any, 5000);
            server.Start();
            try
            {
                server.BeginAcceptSocket(
                    asyncResult =>
                    {
                        var clientSocket = server.EndAcceptSocket(asyncResult);
                        var packet = new byte[] { 0xDE, 0xAD, 0xBE, 0xEF, 0, 0, 0, 0, 0, 0 };
                        clientSocket.BeginSend(packet, 0, packet.Length, SocketFlags.None, null, null);
                    }, null);

                using var client = new TcpClient("127.0.0.1", 5000);
                var socketConnection = SocketConnection.Create(client.Client);
                var connection = new Connection(socketConnection, new PipelinedDecryptor(socketConnection.Input), new PipelinedEncryptor(socketConnection.Output));

                connection.BeginReceive();

                Thread.Sleep(100);
                Assert.That(connection.Connected, Is.False);
            }
            finally
            {
                server.Stop();
            }
        }
    }
}
