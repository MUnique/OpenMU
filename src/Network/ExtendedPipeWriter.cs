// <copyright file="ExtendedPipeWriter.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.Network;

using System.Diagnostics.Metrics;
using System.IO.Pipelines;
using System.Threading;

/// <summary>
/// A wrapper for an existing <see cref="PipeWriter"/>, which has metrics about the written bytes.
/// </summary>
public class ExtendedPipeWriter : PipeWriter
{
    private readonly PipeWriter _target;
    private readonly Counter<long> _writeCounter;

    private Memory<byte> _lastBuffer;

    /// <summary>
    /// Initializes a new instance of the <see cref="ExtendedPipeWriter"/> class.
    /// </summary>
    /// <param name="target">The target <see cref="PipeWriter"/>.</param>
    /// <param name="writeCounter">A counter for the written bytes.</param>
    public ExtendedPipeWriter(PipeWriter target, Counter<long> writeCounter)
    {
        this._target = target;
        this._writeCounter = writeCounter;
    }

    /// <summary>
    /// Gets or sets the collector which gets the written data, to capture the outgoing data
    /// packets. If it's <see langword="null"/>, nothing is captured.
    /// </summary>
    internal OutgoingPacketCollector? PacketCollector { get; set; }

    /// <inheritdoc />
    public override void Complete(Exception? exception = null)
    {
        this._target.Complete(exception);
    }

    /// <inheritdoc />
    public override void CancelPendingFlush()
    {
        this._target.CancelPendingFlush();
    }

    /// <inheritdoc />
    public override ValueTask<FlushResult> FlushAsync(CancellationToken cancellationToken = default)
    {
        return this._target.FlushAsync(cancellationToken);
    }

    /// <inheritdoc />
    public override void Advance(int bytes)
    {
        if (this.PacketCollector is { } collector && bytes > 0 && this._lastBuffer.Length >= bytes)
        {
            // The data has to be collected before it's advanced, because the target may
            // recycle the buffer afterwards.
            collector.DataWritten(this._lastBuffer.Span[..bytes]);
        }

        // A new buffer has to be requested before writing again, so we forget this one.
        // That way, the same data can't be reported twice.
        this._lastBuffer = default;

        this._target.Advance(bytes);
        this._writeCounter.Add(bytes);
    }

    /// <inheritdoc />
    public override Memory<byte> GetMemory(int sizeHint = 0)
    {
        var memory = this._target.GetMemory(sizeHint);
        this._lastBuffer = memory;
        return memory;
    }

    /// <inheritdoc />
    public override Span<byte> GetSpan(int sizeHint = 0)
    {
        if (this.PacketCollector is null)
        {
            // We don't remember the buffer in this case. That way, a collector which gets
            // attached between this call and the next Advance doesn't report stale data.
            this._lastBuffer = default;
            var span = this._target.GetSpan(sizeHint);
            span.Clear();
            return span;
        }

        var memory = this._target.GetMemory(sizeHint);
        this._lastBuffer = memory;
        var memorySpan = memory.Span;
        memorySpan.Clear();
        return memorySpan;
    }
}