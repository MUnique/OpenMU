// <copyright file="PacketSending.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

using Nito.AsyncEx;

namespace MUnique.OpenMU.Network.Benchmarks;

using System.Threading;
using Microsoft.Extensions.Logging.Abstractions;

/// <summary>
/// A benchmark of sending network packets.
/// The memory allocation should be the same.
/// </summary>
[SimpleJob(RuntimeMoniker.NetCoreApp31)]
[MemoryDiagnoser]
[InvocationCount(100)]
public class PacketSending
{
    private readonly byte[] _c1Packet = Convert.FromBase64String("wf8AudHEjjSP53H6Rkp3oXj7B9z+rVDR2f0Is4bvsIsUL3RM/aTDB2FX9YG3Hkboy1Z1JThot558MeDTvNuunzfl5RbWK6TTOP97prjPGbq3IOcweopTq3fVz8vD8EuFqVVJ0jgvEZ+xoe047RHmrRgmG5zzfSWtkTmeAVzZD0i09f1jhUeBiA5HfticGr5m7iGzndSvkSwvm0D/kRBD15GlhPgTgyfQpJONrP5NEHd7NxI6JnJzBWPQM+kHgvb+BKdH95bFUmv54vlBIeUt4ovIg1r9CLEfMX+UQk89yCKcj6dXBRjgteSmQUN5MuN9o1FePv6cAPv2KMUXMBAc");

    /// <summary>
    /// Gets or sets the packet count which will be tested.
    /// </summary>
    [Params(100, 500, 1000)]
    public int PacketCount { get; set; }

    /// <summary>
    /// Sends packets at the connection with Spans.
    /// </summary>
    /// <param name="cancellationToken">The cancellation token.</param>
    [Benchmark]
    public async ValueTask SendSpanAsync(CancellationToken cancellationToken)
    {
        var duplexPipe = new DuplexPipe();
        using var connection = new Connection(duplexPipe, null, null, new NullLogger<Connection>());
        for (int i = 0; i < this.PacketCount && !cancellationToken.IsCancellationRequested; i++)
        {
            await connection.OutputLock.WaitAsync(cancellationToken);
            try
            {
                void Write()
                {
                    var span = connection.Output.GetSpan(this._c1Packet.Length);
                    this._c1Packet.CopyTo(span);
                    connection.Output.Advance(this._c1Packet.Length);
                }

                Write();
                await connection.Output.FlushAsync(cancellationToken);
            }
            finally
            {
                connection.OutputLock.Release();
            }
        }
    }

    /// <summary>
    /// Sends packets at the connection with Spans.
    /// </summary>
    /// <param name="cancellationToken">The cancellation token.</param>
    [Benchmark]
    public async ValueTask SendSpanInlineAsync(CancellationToken cancellationToken)
    {
        var duplexPipe = new DuplexPipe();
        using var connection = new Connection(duplexPipe, null, null, new NullLogger<Connection>());
        for (int i = 0; i < this.PacketCount && !cancellationToken.IsCancellationRequested; i++)
        {
            await connection.OutputLock.WaitAsync(cancellationToken);
            try
            {
                this._c1Packet.CopyTo(connection.Output.GetSpan(this._c1Packet.Length));
                connection.Output.Advance(this._c1Packet.Length);
                await connection.Output.FlushAsync(cancellationToken);
            }
            finally
            {
                connection.OutputLock.Release();
            }
        }
    }

    /// <summary>
    /// Sends packets at the connection with Spans.
    /// </summary>
    /// <param name="cancellationToken">The cancellation token.</param>
    [Benchmark]
    public async ValueTask SendSpanInlineWithAsyncLockAsync(CancellationToken cancellationToken)
    {
        var asyncLock = new AsyncLock();
        var duplexPipe = new DuplexPipe();
        using var connection = new Connection(duplexPipe, null, null, new NullLogger<Connection>());
        for (int i = 0; i < this.PacketCount && !cancellationToken.IsCancellationRequested; i++)
        {
            using var foo = await asyncLock.LockAsync(cancellationToken);
            this._c1Packet.CopyTo(connection.Output.GetSpan(this._c1Packet.Length));
            connection.Output.Advance(this._c1Packet.Length);
            await connection.Output.FlushAsync(cancellationToken);
        }
    }
}