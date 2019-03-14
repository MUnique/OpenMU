// <copyright file="ConnectionTests.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.Network.Tests
{
    using System.Threading.Tasks;
    using NUnit.Framework;

    /// <summary>
    /// Tests for <see cref="Connection"/>.
    /// </summary>
    [TestFixture]
    public class ConnectionTests
    {
        /// <summary>
        /// Tests if the connection is disconnected after a malformed packet was received.
        /// </summary>
        /// <returns>The async task.</returns>
        [Test]
        public async Task DisconnectedByMalformedPacketReceived()
        {
            var malformedData = new byte[] { 0xDE, 0xAD, 0xBE, 0xEF };
            var duplexPipe = new DuplexPipe();
            using (var connection = new Connection(duplexPipe, null, null))
            {
                var disconnected = false;
                connection.Disconnected += (sender, args) => disconnected = true;
                _ = connection.BeginReceive();
                try
                {
                    await duplexPipe.ReceivePipe.Writer.WriteAsync(malformedData);
                }
                catch
                {
                    // we need to swallow the exception for this test, so we can check the connected flag afterwards.
                }

                for (int i = 0; i < 10 && !disconnected; i++)
                {
                    await Task.Delay(10).ConfigureAwait(false);
                }

                Assert.That(connection.Connected, Is.False);
            }
        }

        /// <summary>
        /// Tests if the connection is disconnected after a malformed packet was sent.
        /// </summary>
        /// <returns>The async task.</returns>
        [Test]
        public async Task DisconnectedByFailingEncryptionOfSentPacket()
        {
            var malformedData = new byte[] { 0xDE, 0xAD, 0xBE, 0xEF };
            var duplexPipe = new DuplexPipe();
            using (var connection = new Connection(duplexPipe, null, new Xor.PipelinedXor32Encryptor(duplexPipe.Output)))
            {
                var disconnected = false;
                connection.Disconnected += (sender, args) => disconnected = true;
                _ = connection.BeginReceive();

                try
                {
                    await connection.Output.WriteAsync(malformedData);
                    await connection.Output.FlushAsync();
                    await duplexPipe.SendPipe.Reader.ReadAsync();
                }
                catch
                {
                    // we need to swallow the exception for this test, so we can check the connected flag afterwards.
                }

                for (int i = 0; i < 10 && !disconnected; i++)
                {
                    await Task.Delay(10).ConfigureAwait(false);
                }

                Assert.That(disconnected, Is.True);
                Assert.That(connection.Connected, Is.False);
            }
        }

        /// <summary>
        /// Tests if the connection is initially connected.
        /// </summary>
        [Test]
        public void InitiallyConnected()
        {
            var duplexPipe = new DuplexPipe();
            using (var connection = new Connection(duplexPipe, null, null))
            {
                Assert.That(connection.Connected, Is.True);
            }
        }
    }
}
