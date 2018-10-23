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
    using MUnique.OpenMU.Network.Xor;

    /// <summary>
    /// Benchmarks for the simple modulus encryption.
    /// </summary>
    [CoreJob]
    [MemoryDiagnoser]
    [InvocationCount(100)]
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

        private readonly byte[] c1Packet = Convert.FromBase64String("wf8AudHEjjSP53H6Rkp3oXj7B9z+rVDR2f0Is4bvsIsUL3RM/aTDB2FX9YG3Hkboy1Z1JThot558MeDTvNuunzfl5RbWK6TTOP97prjPGbq3IOcweopTq3fVz8vD8EuFqVVJ0jgvEZ+xoe047RHmrRgmG5zzfSWtkTmeAVzZD0i09f1jhUeBiA5HfticGr5m7iGzndSvkSwvm0D/kRBD15GlhPgTgyfQpJONrP5NEHd7NxI6JnJzBWPQM+kHgvb+BKdH95bFUmv54vlBIeUt4ovIg1r9CLEfMX+UQk89yCKcj6dXBRjgteSmQUN5MuN9o1FePv6cAPv2KMUXMBAc");

        /// <summary>
        /// Benchmarks the performance of the <see cref="SimpleModulusEncryption"/>.
        /// </summary>
        /// <returns>The value task.</returns>
        [Benchmark]
        public async ValueTask SimpleModulusEncryption()
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
        /// Benchmarks the performance of the <see cref="MUnique.OpenMU.Network.Xor.PipelinedXor32Encryptor"/>.
        /// </summary>
        /// <returns>The value task.</returns>
        [Benchmark]
        public async ValueTask Xor32Encryption()
        {
            var pipe = new Pipe();
            var pipelinedEncryptor = new PipelinedXor32Encryptor(pipe.Writer);
            var readBuffer = new byte[256];
            for (int i = 0; i < PacketCount; i++)
            {
                await pipelinedEncryptor.Writer.WriteAsync(this.c1Packet);
                await pipelinedEncryptor.Writer.FlushAsync();
                var readResult = await pipe.Reader.ReadAsync();
                readResult.Buffer.CopyTo(readBuffer);
                //// In the client/server, I would process the readBuffer here
                pipe.Reader.AdvanceTo(readResult.Buffer.Start, readResult.Buffer.End);
            }

            pipelinedEncryptor.Writer.Complete();
        }
    }
}
