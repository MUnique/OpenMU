// <copyright file="PacketSending.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.Network.Benchmarks
{
    using System;
    using BenchmarkDotNet.Attributes;

    /// <summary>
    /// A benchmark to compare <see cref="ConnectionExtensions.StartSafeWrite"/> with doing the same manually.
    /// The memory allocation should be the same.
    /// </summary>
    [CoreJob]
    [MemoryDiagnoser]
    [InvocationCount(100)]
    public class PacketSending
    {
        private readonly byte[] c1Packet = Convert.FromBase64String("wf8AudHEjjSP53H6Rkp3oXj7B9z+rVDR2f0Is4bvsIsUL3RM/aTDB2FX9YG3Hkboy1Z1JThot558MeDTvNuunzfl5RbWK6TTOP97prjPGbq3IOcweopTq3fVz8vD8EuFqVVJ0jgvEZ+xoe047RHmrRgmG5zzfSWtkTmeAVzZD0i09f1jhUeBiA5HfticGr5m7iGzndSvkSwvm0D/kRBD15GlhPgTgyfQpJONrP5NEHd7NxI6JnJzBWPQM+kHgvb+BKdH95bFUmv54vlBIeUt4ovIg1r9CLEfMX+UQk89yCKcj6dXBRjgteSmQUN5MuN9o1FePv6cAPv2KMUXMBAc");

        /// <summary>
        /// Gets or sets the packet count which will be tested.
        /// </summary>
        [Params(100, 500, 1000)]
        public int PacketCount { get; set; }

        /// <summary>
        /// Manually sends packets at the connection.
        /// </summary>
        [Benchmark]
        public void Manually()
        {
            var duplexPipe = new DuplexPipe();
            var connection = new Connection(duplexPipe, null, null);
            for (int i = 0; i < this.PacketCount; i++)
            {
                lock (connection)
                {
                    var span = connection.Output.GetSpan(this.c1Packet.Length);
                    this.c1Packet.CopyTo(span);
                    connection.Output.Advance(this.c1Packet.Length);
                }

                connection.Output.FlushAsync();
            }
        }

        /// <summary>
        /// Sends packets with using <see cref="ConnectionExtensions.StartSafeWrite"/>.
        /// </summary>
        [Benchmark]
        public void SafeWrite()
        {
            var duplexPipe = new DuplexPipe();
            var connection = new Connection(duplexPipe, null, null);
            for (int i = 0; i < this.PacketCount; i++)
            {
                using (var output = connection.StartSafeWrite(0xC1, this.c1Packet.Length))
                {
                    var span = output.Span;
                    this.c1Packet.CopyTo(span);
                    output.Commit();
                }
            }
        }
    }
}
