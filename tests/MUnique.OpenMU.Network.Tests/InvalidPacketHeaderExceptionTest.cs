// <copyright file="InvalidPacketHeaderExceptionTest.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.Network.Tests
{
    using System;
    using System.Threading.Tasks;
    using NUnit.Framework;

    /// <summary>
    /// Tests if <see cref="InvalidPacketHeaderException"/> are thrown when malformed data is read by a <see cref="PacketPipeReaderBase"/>.
    /// </summary>
    [TestFixture]
    public class InvalidPacketHeaderExceptionTest
    {
        private readonly byte[] malformedData = { 0xC1, 0x03, 0xFF, 0x00, 0x00, 0x00 };

        /// <summary>
        /// Tests if the exception is thrown.
        /// </summary>
        /// <returns>The async task.</returns>
        [Test]
        public async Task Thrown()
        {
            await this.TestException(e => { });
        }

        /// <summary>
        /// Tests if <see cref="InvalidPacketHeaderException.Header"/> is assigned correctly.
        /// </summary>
        /// <returns>The async task.</returns>
        [Test]
        public async Task TestHeader()
        {
            await this.TestException(e => Assert.That(e.Header, Is.EquivalentTo(new byte[] { 0x00, 0x00, 0x00 })));
        }

        /// <summary>
        /// Tests if <see cref="InvalidPacketHeaderException.Position"/> is assigned correctly.
        /// </summary>
        /// <returns>The async task.</returns>
        [Test]
        public async Task TestPosition()
        {
            await this.TestException(e => Assert.That(e.Position, Is.EqualTo(3)));
        }

        /// <summary>
        /// Tests if <see cref="InvalidPacketHeaderException.BufferContent"/> is assigned correctly.
        /// </summary>
        /// <returns>The async task.</returns>
        [Test]
        public async Task TestBufferContent()
        {
            await this.TestException(e => Assert.That(e.BufferContent, Is.EquivalentTo(this.malformedData)));
        }

        private async ValueTask TestException(Action<InvalidPacketHeaderException> check)
        {
            bool thrown = false;
            var duplexPipe = new DuplexPipe();
            using var connection = new Connection(duplexPipe, null, new Xor.PipelinedXor32Encryptor(duplexPipe.Output));
            _ = connection.BeginReceive();
            try
            {
                await duplexPipe.ReceivePipe.Writer.WriteAsync(this.malformedData);
            }
            catch (InvalidPacketHeaderException e)
            {
                thrown = true;
                check(e);
            }
            catch (Exception e)
            {
                Assert.Fail($"Wrong exception type {e}", e);
            }

            Assert.That(thrown);
        }
    }
}
