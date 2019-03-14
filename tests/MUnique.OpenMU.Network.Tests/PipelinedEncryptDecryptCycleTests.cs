// <copyright file="PipelinedEncryptDecryptCycleTests.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.Network.Tests
{
    using System;
    using System.Buffers;
    using System.IO.Pipelines;
    using System.Threading.Tasks;
    using MUnique.OpenMU.Network.SimpleModulus;
    using MUnique.OpenMU.Network.Xor;
    using NUnit.Framework;

    /// <summary>
    /// Tests the cycle of encrypting and decrypting a packet purely due pipes.
    /// </summary>
    [TestFixture]
    public class PipelinedEncryptDecryptCycleTests
    {
        /// <summary>
        /// Tests the encryption and decryption cycle of C3-packets from client to server.
        /// These packets get encrypted first by <see cref="PipelinedXor32Encryptor"/>, then by <see cref="PipelinedSimpleModulusEncryptor"/> using client-side keys.
        /// Then it gets decrypted by the <see cref="PipelinedSimpleModulusDecryptor"/> using server-side keys and finally by the <see cref="PipelinedXor32Decryptor"/>.
        /// </summary>
        /// <returns>The task.</returns>
        [Test]
        public async Task ClientToServerC3()
        {
            var packet = Convert.FromBase64String("w7kxFgK8hYpGGLgdXe7ZpTZViB+r3sRI3YSqZs7/Mh5Vmh2mXqs+3dqkvURmXrL57ASs+FkJz/236Tl9ER67R+WZyMLRMkeLF6tEBiB/4X7SsXrKUznES8of73RxwMy76HZezJbvJ7m9IOGuxcjcNwe6q1+k8fOs1Hz3sULSGlbfiB6qIBXo4onADTNYFoYCQrdtthVsF/aDsvcZ93V36gaKzzyqMhby0sjV4+TAU7719W6LZWNAcnA=");
            await this.EncryptDecryptFromClientToServer(packet).ConfigureAwait(false);
        }

        /// <summary>
        /// Tests the encryption of a packet where the final block doesn't have maximum size.
        /// </summary>
        /// <remarks>
        /// The test uses a real ping packet which was captured from a real game client.
        /// </remarks>
        /// <returns>The async task.</returns>
        [Test]
        public async Task ClientToServerC3WithNonMaximalFinalBlockSize()
        {
            var packet = new byte[] { 195, 12, 14, 0, 1, 51, 254, 39, 0, 0, 0, 0 };
            await this.EncryptDecryptFromClientToServer(packet).ConfigureAwait(false);
        }

        /// <summary>
        /// Tests the encryption of a C3 packet where the size is lower than the maximum block size.
        /// </summary>
        /// <remarks>
        /// The test uses a real ping packet which was captured from a real game client.
        /// </remarks>
        /// <returns>The async task.</returns>
        [Test]
        public async Task ClientToServerC3WithSmallPacket()
        {
            var packet = new byte[] { 195, 5, 14, 0, 1 };
            await this.EncryptDecryptFromClientToServer(packet).ConfigureAwait(false);
        }

        /// <summary>
        /// Tests the encryption and decryption cycle of C1-packets from client to server.
        /// These packets get encrypted first by <see cref="PipelinedXor32Encryptor"/>, then <see cref="PipelinedSimpleModulusEncryptor"/> just forwards then as-is.
        /// Then the <see cref="PipelinedSimpleModulusDecryptor"/> forwards them as well and finally it gets decrypted by the <see cref="PipelinedXor32Decryptor"/>.
        /// </summary>
        /// <returns>The task.</returns>
        [Test]
        public async Task ClientToServerC1()
        {
            var packet = new byte[] { 0xC1, 0x06, 0x11, 0x01, 0x02, 0x03 };
            await this.EncryptDecryptFromClientToServer(packet).ConfigureAwait(false);
        }

        /// <summary>
        /// Tests the encryption and decryption cycle of C3-packets from server to client.
        /// These packets get encrypted first by the <see cref="PipelinedSimpleModulusEncryptor"/> using server-side keys.
        /// On the client side it gets decrypted by the <see cref="PipelinedSimpleModulusDecryptor"/> using client-side keys.
        /// </summary>
        /// <returns>The task.</returns>
        [Test]
        public async Task ServerToClientC3()
        {
            var packet = Convert.FromBase64String("w7kxFgK8hYpGGLgdXe7ZpTZViB+r3sRI3YSqZs7/Mh5Vmh2mXqs+3dqkvURmXrL57ASs+FkJz/236Tl9ER67R+WZyMLRMkeLF6tEBiB/4X7SsXrKUznES8of73RxwMy76HZezJbvJ7m9IOGuxcjcNwe6q1+k8fOs1Hz3sULSGlbfiB6qIBXo4onADTNYFoYCQrdtthVsF/aDsvcZ93V36gaKzzyqMhby0sjV4+TAU7719W6LZWNAcnA=");
            await this.EncryptDecryptFromServerToClient(packet).ConfigureAwait(false);
        }

        /// <summary>
        /// Tests the encryption and decryption cycle of C1-packets from server to client.
        /// These packets are not encrypted at all, so all involved simple modulus encryptor/decryptors just forward them.
        /// </summary>
        /// <returns>The task.</returns>
        [Test]
        public async Task ServerToClientC1()
        {
            var packet = new byte[] { 0xC1, 0x06, 0x11, 0x01, 0x02, 0x03 };
            await this.EncryptDecryptFromServerToClient(packet).ConfigureAwait(false);
        }

        /// <summary>
        /// Tests the encryption-decryption cycle for the packet from server to client. The specified packet must be the same after the packet has passed this cycle.
        /// Packets from server to client are never encrypted by Xor32, so these encryptor/decryptors are not involved here.
        /// </summary>
        /// <param name="packet">The packet.</param>
        /// <returns>The task.</returns>
        private async Task EncryptDecryptFromServerToClient(byte[] packet)
        {
            // this pipe connects the encryptor with the decryptor. You can imagine this as the server-to-client pipe of a network socket, for example.
            var pipe = new Pipe();

            var encryptor = new PipelinedSimpleModulusEncryptor(pipe.Writer);
            var decryptor = new PipelinedSimpleModulusDecryptor(pipe.Reader, PipelinedSimpleModulusDecryptor.DefaultClientKey);
            encryptor.Writer.Write(packet);
            await encryptor.Writer.FlushAsync().ConfigureAwait(false);
            var readResult = await decryptor.Reader.ReadAsync().ConfigureAwait(false);

            var result = readResult.Buffer.ToArray();
            Assert.That(result, Is.EquivalentTo(packet));
        }

        /// <summary>
        /// Tests the encryption-decryption cycle for the packet. The specified packet must be the same after the packet has passed this cycle.
        /// </summary>
        /// <param name="packet">The packet.</param>
        /// <returns>The async task.</returns>
        private async Task EncryptDecryptFromClientToServer(byte[] packet)
        {
            // this pipe connects the encryptor with the decryptor. You can imagine this as the client-to-server pipe of a network socket, for example.
            var pipe = new Pipe();

            var encryptor = new PipelinedXor32Encryptor(new PipelinedSimpleModulusEncryptor(pipe.Writer, PipelinedSimpleModulusEncryptor.DefaultClientKey).Writer);
            var decryptor = new PipelinedXor32Decryptor(new PipelinedSimpleModulusDecryptor(pipe.Reader).Reader);
            encryptor.Writer.Write(packet);
            await encryptor.Writer.FlushAsync();
            var readResult = await decryptor.Reader.ReadAsync().ConfigureAwait(false);

            var result = readResult.Buffer.ToArray();
            Assert.That(result, Is.EquivalentTo(packet));
        }
    }
}