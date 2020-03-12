// <copyright file="PipelinedDecryptorTests.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.Network.Tests
{
    using System;
    using System.Buffers;
    using System.IO.Pipelines;
    using System.Text;
    using System.Threading.Tasks;
    using MUnique.OpenMU.Network.PlugIns;
    using MUnique.OpenMU.Network.Xor;
    using NUnit.Framework;

    /// <summary>
    /// Tests for the pipelined decryptors.
    /// </summary>
    [TestFixture]
    public class PipelinedDecryptorTests
    {
        /// <summary>
        /// Tests the decryption of C1-packets with the <see cref="PipelinedDecryptor"/>.
        /// </summary>
        /// <returns>The task.</returns>
        [Test]
        public async Task C1Decrypt()
        {
            var expected = Convert.FromBase64String("wf8AudHEjjSP53H6Rkp3oXj7B9z+rVDR2f0Is4bvsIsUL3RM/aTDB2FX9YG3Hkboy1Z1JThot558MeDTvNuunzfl5RbWK6TTOP97prjPGbq3IOcweopTq3fVz8vD8EuFqVVJ0jgvEZ+xoe047RHmrRgmG5zzfSWtkTmeAVzZD0i09f1jhUeBiA5HfticGr5m7iGzndSvkSwvm0D/kRBD15GlhPgTgyfQpJONrP5NEHd7NxI6JnJzBWPQM+kHgvb+BKdH95bFUmv54vlBIeUt4ovIg1r9CLEfMX+UQk89yCKcj6dXBRjgteSmQUN5MuN9o1FePv6cAPv2KMUXMBAc");
            var packet = Convert.FromBase64String("wf8AR45pIrXwJJSigq/5q+EIGvMkdtgUiZterAzfIedYZt9tiA8JrQZiVhvKs9TPNnISAhOEzU52qIUXjWqKWMQwGPA+Nlcn1RmjyRe/h85LeYuO3agFs4C6uDLXGx7W1JAUOBgUwPyHFTnNRjD3qYO3uRDKSJMj9iBzM0ms7ej3EyO+I0cDKOyYJzPJtCu+YlH3X6LynazHsz6ANxsWjLYCS01G5gR3Gbn1lQ0nFpLb/vn2+XT54cb5B6+OMIg7lCKooS/JXpaldEzBhgQIGaB67INXoO/tmAhRUjs6vNPkehC5pJ+6rIIXlxgHUpAej8yHjFs4xiCSVV0LHTFj");
            await this.Decrypt(packet, expected).ConfigureAwait(false);
        }

        /// <summary>
        /// Tests the decryption of C2-packets with the <see cref="PipelinedDecryptor"/>.
        /// </summary>
        /// <returns>The task.</returns>
        [Test]
        public async Task C2Decrypt()
        {
            var expected = Convert.FromBase64String("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");
            var packet = Convert.FromBase64String("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");
            await this.Decrypt(packet, expected).ConfigureAwait(false);
        }

        /// <summary>
        /// Tests the pipelined decryption of C3-packets.
        /// </summary>
        /// <returns>The task.</returns>
        [Test]
        public async Task C3Decrypt()
        {
            var decryptedPacket = Convert.FromBase64String("w7gAudHEjjSP53H6Rkp3oXj7B9z+rVDR2f0Is4bvsIsUL3RM/aTDB2FX9YG3Hkboy1Z1JThot558MeDTvNuunzfl5RbWK6TTOP97prjPGbq3IOcweopTq3fVz8vD8EuFqVVJ0jgvEZ+xoe047RHmrRgmG5zzfSWtkTmeAVzZD0i09f1jhUeBiA5HfticGr5m7iGzndSvkSwvm0D/kRBD15GlhPgTgyfQpJONrP5NEHd7NxI6JnJzBQ==");
            var encryptedPacket = Convert.FromBase64String("w/+r1UUAxjcz8wQKPyQ8IsrH0PAGfEF0+VwSJx3GwXzcOw42Oj7OVZIU/EXp3FMYZIbCGlPGNTwJuaEKkEHWoaiNtoM8NwWwhcPBmITN+C2nH48ZhNDtNQYzOOd3stxbIPwkuI1LmgohXa1iHnUUIdLeQ3+h1oURffLHjnkZcpyuECtoLRjgYjaayychmMDG8wVPVPKokNIrLIeyyFJGLMv0pgH8FyIX5mCJqOog/B26j44cU0eKyQK6geDV7f5xMxtlYL1gy/4ZcyLexoEAdeGSp5QmHLhCb8L99cD1UVIul9IQkU35AzYhRRtU7yjTjYlhVLjCMiIVnjQfJKac");
            await this.Decrypt(encryptedPacket, decryptedPacket).ConfigureAwait(false);
        }

        /// <summary>
        /// Tests the pipelined decryption of C4-packets.
        /// </summary>
        /// <returns>The task.</returns>
        [Test]
        public async Task C4Decrypt()
        {
            var decryptedPacket = Convert.FromBase64String("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");
            var encryptedPacket = Convert.FromBase64String("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");
            await this.Decrypt(encryptedPacket, decryptedPacket).ConfigureAwait(false);
        }

        /// <summary>
        /// Tests that the decryption of a packet where the final block doesn't have maximum size.
        /// </summary>
        /// <remarks>
        /// The test uses a real ping packet which was captured from a real game client.
        /// </remarks>
        /// <returns>The async task.</returns>
        [Test]
        public async Task C3DecryptWithNonMaximalFinalBlockSize()
        {
            var decrypted = new byte[] { 195, 12, 14, 0, 1, 51, 254, 39, 0, 0, 0, 0 };
            var encrypted = new byte[] { 0xC3, 0x18, 0xBC, 0x9D, 0x35, 0xE2, 0xA2, 0x21, 0x22, 0x91, 0x5C, 0x2B, 0x1E, 0x18, 0x63, 0x47, 0x28, 0xC3, 0x10, 0x20, 0xF1, 0xC4, 0x2A, 0x14 };
            await this.Decrypt(encrypted, decrypted).ConfigureAwait(false);
        }

        /// <summary>
        /// Decrypts a login packet which was captured from a version 0.75 game client.
        /// </summary>
        /// <returns>The task.</returns>
        [Test]
        public async Task DecryptVersion075LoginPacket()
        {
            var encrypted = new byte[]
            {
                0xC3, 0x4E, 0x38, 0x08, 0x6C, 0x49, 0x4A, 0x27, 0xE7, 0x63, 0xC4, 0x55, 0xD1, 0x24, 0x44, 0x06, 0x0B, 0xF3, 0x0A, 0xD1, 0x79, 0xEC, 0x65, 0xA6, 0x8F, 0xD4, 0x51, 0x7C, 0xA0, 0x33, 0x2C, 0x24, 0xF0, 0xC6, 0x29, 0xC3, 0x28,
                0x15, 0x7A, 0x67, 0x0A, 0xF8, 0x3B, 0xFA, 0xD7, 0x50, 0xA7, 0x03, 0x84, 0x92, 0xC0, 0x57, 0xBD, 0xDA, 0x3D, 0x60, 0x30, 0xFD, 0xD7, 0xB5, 0x80, 0x43, 0x09, 0xDA, 0x74, 0x99, 0x74, 0xE1, 0xC9, 0x09, 0x5B, 0xA3, 0x23, 0x13,
                0x94, 0xEC, 0xFA, 0xC9,
            };

            var pipe = new Pipe();
            var plugin = new Version075NetworkEncryptionFactoryPlugIn();
            var decryptor = plugin.CreateDecryptor(pipe.Reader, DataDirection.ClientToServer);
            pipe.Writer.Write(encrypted);
            await pipe.Writer.FlushAsync().ConfigureAwait(false);
            var readResult = await decryptor.Reader.ReadAsync().ConfigureAwait(false);
            var result = readResult.Buffer.ToArray();

            var xor3Decryptor = new Xor3Decryptor(0);
            xor3Decryptor.Decrypt(result.AsSpan(4, 10));
            xor3Decryptor.Decrypt(result.AsSpan(14, 10));
            Assert.That(result[2], Is.EqualTo(0xF1));
            Assert.That(result[3], Is.EqualTo(0x01));
            Assert.That(result.ExtractString(4, 10, Encoding.ASCII), Is.EqualTo("test2"));
            Assert.That(result.ExtractString(14, 10, Encoding.ASCII), Is.EqualTo("test2"));
        }

        /// <summary>
        /// Tests if the reader of the decryptor gets the information that the writer of the pipe completed with the last read when the complete was called before.
        /// </summary>
        /// <returns>The task.</returns>
        [Test]
        public async Task CompletesWithLastRead()
        {
            var pipe = new Pipe();
            var decryptor = new PipelinedDecryptor(pipe.Reader);
            pipe.Writer.Write(new byte[] { 0xC1, 0x03, 0x01 });
            await pipe.Writer.CompleteAsync().ConfigureAwait(false);
            await Task.Delay(10).ConfigureAwait(false);
            var result = await decryptor.Reader.ReadAsync().ConfigureAwait(false);
            Assert.That(result.IsCompleted, Is.True);
        }

        /// <summary>
        /// Tests if the reader of the decryptor gets the information that the writer of the pipe completed after <see cref="PipeReader.ReadAsync"/> was called already.
        /// </summary>
        /// <returns>The task.</returns>
        [Test]
        public async Task CompletesAfterReadAsyncIsCalled()
        {
            var pipe = new Pipe();
            var decryptor = new PipelinedDecryptor(pipe.Reader);
            var reading = decryptor.Reader.ReadAsync().ConfigureAwait(false);
            await pipe.Writer.CompleteAsync().ConfigureAwait(false);
            var result = await reading;
            await Task.Delay(10).ConfigureAwait(false);
            Assert.That(result.IsCompleted, Is.True);
        }

        /// <summary>
        /// Decrypts the specified encrypted packet and compares the result it with the specified decrypted packet.
        /// </summary>
        /// <param name="encryptedPacket">The encrypted packet.</param>
        /// <param name="decryptedPacket">The expected decrypted packet.</param>
        /// <returns>The task.</returns>
        private async Task Decrypt(byte[] encryptedPacket, byte[] decryptedPacket)
        {
            var pipe = new Pipe();
            var decryptor = new PipelinedDecryptor(pipe.Reader);
            pipe.Writer.Write(encryptedPacket);
            await pipe.Writer.FlushAsync().ConfigureAwait(false);
            var readResult = await decryptor.Reader.ReadAsync().ConfigureAwait(false);
            var result = readResult.Buffer.ToArray();
            Assert.That(result, Is.EquivalentTo(decryptedPacket));
        }
    }
}
