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

    private PipeWriter? _activeCaptureWriter;

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
    /// Gets or sets the writer of the packet capture, into which the written data should be
    /// copied. If it's <see langword="null"/>, nothing should be captured.
    /// </summary>
    /// <remarks>
    /// It's only applied at a packet boundary, because the capture would start or end in the
    /// middle of a data packet otherwise. It can be set by any thread; the switch itself
    /// happens on the thread which writes to this instance.
    /// </remarks>
    internal PipeWriter? PendingCaptureWriter { get; set; }

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
        if (this._activeCaptureWriter is { } captureWriter)
        {
            return this.FlushWithCaptureAsync(captureWriter, cancellationToken);
        }

        // After a flush, the next written data starts a new packet, so a requested capture
        // can start here.
        this._activeCaptureWriter = this.PendingCaptureWriter;
        return this._target.FlushAsync(cancellationToken);
    }

    /// <inheritdoc />
    public override void Advance(int bytes)
    {
        if (this._activeCaptureWriter is { } captureWriter && bytes > 0 && this._lastBuffer.Length >= bytes)
        {
            // The data has to be copied before it's advanced, because the target may recycle
            // the buffer afterwards. The capturing must never break the connection, so a
            // failing capture is silently ignored here.
            try
            {
                this._lastBuffer.Span[..bytes].CopyTo(captureWriter.GetSpan(bytes));
                captureWriter.Advance(bytes);
            }
            catch (InvalidOperationException)
            {
                // The capture has been completed in the meantime.
            }
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
        this.ApplyPendingCaptureWriterAtPacketBoundary();
        var memory = this._target.GetMemory(sizeHint);
        this._lastBuffer = memory;
        return memory;
    }

    /// <inheritdoc />
    public override Span<byte> GetSpan(int sizeHint = 0)
    {
        this.ApplyPendingCaptureWriterAtPacketBoundary();
        if (this._activeCaptureWriter is null)
        {
            // We don't remember the buffer in this case. That way, a capture which gets
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

    /// <summary>
    /// Applies a requested change of the capture, when nothing is written to the target yet.
    /// In that case we're at a packet boundary, so a starting capture doesn't begin in the
    /// middle of a data packet.
    /// </summary>
    private void ApplyPendingCaptureWriterAtPacketBoundary()
    {
        if (!ReferenceEquals(this._activeCaptureWriter, this.PendingCaptureWriter)
            && this._target is { CanGetUnflushedBytes: true, UnflushedBytes: 0 })
        {
            this._activeCaptureWriter = this.PendingCaptureWriter;
        }
    }

    private async ValueTask<FlushResult> FlushWithCaptureAsync(PipeWriter captureWriter, CancellationToken cancellationToken)
    {
        try
        {
            await captureWriter.FlushAsync(cancellationToken).ConfigureAwait(false);
        }
        catch (InvalidOperationException)
        {
            // The capture has been completed in the meantime.
        }

        // After a flush, the next written data starts a new packet, so a requested change of
        // the capture can be applied here.
        this._activeCaptureWriter = this.PendingCaptureWriter;

        return await this._target.FlushAsync(cancellationToken).ConfigureAwait(false);
    }
}