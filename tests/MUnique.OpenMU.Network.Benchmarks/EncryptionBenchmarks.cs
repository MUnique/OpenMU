// <copyright file="EncryptionBenchmarks.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.Network.Benchmarks
{
    using System;
    using System.Buffers;
    using System.IO.Pipelines;
    using System.Threading.Tasks;
    using BenchmarkDotNet.Attributes;
    using MUnique.OpenMU.Network.SimpleModulus;

    /// <summary>
    /// Benchmarks for the simple modulus encryptors.
    /// </summary>
    [CoreJob]
    [MemoryDiagnoser]
    [InvocationCount(10)]
    public class EncryptionBenchmarks
    {
        /// <summary>
        /// The packet count for each benchmark run.
        /// </summary>
        private const int PacketCount = 100000;

        /// <summary>
        /// That's a 185 bytes unencrypted C3 packet. Encrypted it takes 255 bytes.
        /// </summary>
        private readonly byte[] c3Packet = Convert.FromBase64String("w7kxFgK8hYpGGLgdXe7ZpTZViB+r3sRI3YSqZs7/Mh5Vmh2mXqs+3dqkvURmXrL57ASs+FkJz/236Tl9ER67R+WZyMLRMkeLF6tEBiB/4X7SsXrKUznES8of73RxwMy76HZezJbvJ7m9IOGuxcjcNwe6q1+k8fOs1Hz3sULSGlbfiB6qIBXo4onADTNYFoYCQrdtthVsF/aDsvcZ93V36gaKzzyqMhby0sjV4+TAU7719W6LZWNAcnA=");

        /// <summary>
        /// Benchmarks the performance of the <see cref="PipelinedSimpleModulusEncryptor"/>.
        /// </summary>
        /// <returns>The value task.</returns>
        [Benchmark]
        public async ValueTask PipelinedSimpleModulusEncryptor()
        {
            var pipe = new Pipe();
            var pipelinedEncryptor = new PipelinedSimpleModulusEncryptor(pipe.Writer);
            var readBuffer = new byte[256];
            for (int i = 0; i < PacketCount; i++)
            {
                await pipelinedEncryptor.Writer.WriteAsync(this.c3Packet);
                await pipelinedEncryptor.Writer.FlushAsync();
                var readResult = await pipe.Reader.ReadAsync();
                readResult.Buffer.CopyTo(readBuffer);
                //// In the server, I would process the readBuffer here
                pipe.Reader.AdvanceTo(readResult.Buffer.Start, readResult.Buffer.End);
            }

            pipelinedEncryptor.Writer.Complete();
        }

        /// <summary>
        /// Benchmarks the performance of the <see cref="SimpleModulusEncryptor"/>.
        /// </summary>
        [Benchmark]
        public void SimpleModulusEncryptor()
        {
            var encryptor = new SimpleModulusEncryptor();
            for (int i = 0; i < PacketCount; i++)
            {
                var result = encryptor.Encrypt(this.c3Packet);
                //// In the server, I would process the result here
            }
        }
    }
}
