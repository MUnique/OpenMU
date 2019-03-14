// <copyright file="PipelinedEncryptorTests.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.Network.Tests
{
    using System;
    using System.Buffers;
    using System.IO.Pipelines;
    using System.Threading.Tasks;
    using MUnique.OpenMU.Network.SimpleModulus;
    using NUnit.Framework;

    /// <summary>
    /// Tests for the pipelined encryptors.
    /// </summary>
    [TestFixture]
    public class PipelinedEncryptorTests
    {
        /// <summary>
        /// Tests the C1-Packet encryption. C1 packet to the client are not getting encrypted, so we check this.
        /// </summary>
        /// <returns>The task.</returns>
        [Test]
        public async Task C1NonEncrypt()
        {
            var packet = new byte[] { 0xC1, 0x05, 0xAB, 0xBC, 0xDE };
            var expected = new byte[] { 0xC1, 0x05, 0xAB, 0xBC, 0xDE };
            await this.Encrypt(packet, expected).ConfigureAwait(false);
        }

        /// <summary>
        /// Tests the C1-Packet encryption. C1 packet to the client are not getting encrypted, so we check this.
        /// </summary>
        /// <returns>The task.</returns>
        [Test]
        public async Task C2NonEncrypt()
        {
            var packet = new byte[] { 0xC2, 0x00, 0x05, 0xBC, 0xDE };
            var expected = new byte[] { 0xC2, 0x00, 0x05, 0xBC, 0xDE };
            await this.Encrypt(packet, expected).ConfigureAwait(false);
        }

        /// <summary>
        /// Tests the encryption of C3-packets with the <see cref="PipelinedEncryptor" />.
        /// </summary>
        /// <returns>The task.</returns>
        [Test]
        public async Task C3Encrypt()
        {
            var packet = Convert.FromBase64String("w7kxFgK8hYpGGLgdXe7ZpTZViB+r3sRI3YSqZs7/Mh5Vmh2mXqs+3dqkvURmXrL57ASs+FkJz/236Tl9ER67R+WZyMLRMkeLF6tEBiB/4X7SsXrKUznES8of73RxwMy76HZezJbvJ7m9IOGuxcjcNwe6q1+k8fOs1Hz3sULSGlbfiB6qIBXo4onADTNYFoYCQrdtthVsF/aDsvcZ93V36gaKzzyqMhby0sjV4+TAU7719W6LZWNAcnA=");
            var expected = Convert.FromBase64String("w/+eRi3xwRp1LdEdKA9lEFFECgL0vYm8siQHloDxwRXsKx4SdRNuBxgl3W3N+OcgJy/dTaThrSAVUhV1XkPLtCklDzoX4gjPXGPjVEJIfb0iY+wGCIFSZnZDKvYIh8GIF7CNZlOLxigIGPESgPY2Ax6WBoTZaDexFePWS8Q1i0Phk5XkZ1LqTRG5gwwxvCZzRi04HVRMTleEnUN2IOIF79s2xr8BXWhsbTIUx30ychj0wdeeAz8D2DCFUUdyB6kWoMG/4V7Mu44JrgM3mfiD0py6j145biJC/BDr9Ii9AsEokQX15FptGi+9/C64i7EBH7QPOm69cdjeNFBQPpms");

            await this.Encrypt(packet, expected).ConfigureAwait(false);
        }

        /// <summary>
        /// Tests the encryption of C4-packets with the <see cref="PipelinedEncryptor"/>.
        /// </summary>
        /// <returns>The task.</returns>
        [Test]
        public async Task C4Encrypt()
        {
            var packet = Convert.FromBase64String("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");
            var expected = Convert.FromBase64String("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");
            await this.Encrypt(packet, expected).ConfigureAwait(false);
        }

        /// <summary>
        /// Tests if the reader gets the information that the writer of the encryptor completed with the last read when the complete was called before.
        /// </summary>
        /// <returns>The task.</returns>
        [Test]
        public async Task CompletesWithLastRead()
        {
            var pipe = new Pipe();
            var encryptor = new PipelinedSimpleModulusEncryptor(pipe.Writer);
            encryptor.Writer.Write(new byte[] { 0xC1, 0x03, 0x01 });
            encryptor.Writer.Complete();
            var result = await pipe.Reader.ReadAsync().ConfigureAwait(false);
            Assert.That(result.IsCompleted, Is.True);
        }

        /// <summary>
        /// Tests if the reader gets the information that the writer of the encryptor completed after <see cref="PipeReader.ReadAsync"/> was called already.
        /// </summary>
        /// <returns>The task.</returns>
        [Test]
        public async Task CompleteAfterReadAsyncIsCalled()
        {
            var pipe = new Pipe();
            var encryptor = new PipelinedSimpleModulusEncryptor(pipe.Writer);
            var completed = false;
            pipe.Reader.OnWriterCompleted((e, o) => completed = true, null);
            var reading = pipe.Reader.ReadAsync();
            encryptor.Writer.Complete();
            var result = await reading;
            await Task.Delay(10).ConfigureAwait(false);
            Assert.That(completed && result.IsCompleted, Is.True);
        }

        private async Task Encrypt(byte[] decrypted, byte[] encrypted)
        {
            var pipe = new Pipe();
            var encryptor = new PipelinedSimpleModulusEncryptor(pipe.Writer);
            encryptor.Writer.Write(decrypted);
            await encryptor.Writer.FlushAsync().ConfigureAwait(false);
            var readResult = await pipe.Reader.ReadAsync().ConfigureAwait(false);
            var result = readResult.Buffer.ToArray();
            Assert.That(result, Is.EquivalentTo(encrypted));
        }
    }
}
