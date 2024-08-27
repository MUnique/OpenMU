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
        this._target.Advance(bytes);
        this._writeCounter.Add(bytes);
    }

    /// <inheritdoc />
    public override Memory<byte> GetMemory(int sizeHint = 0)
    {
        return this._target.GetMemory(sizeHint);
    }

    /// <inheritdoc />
    public override Span<byte> GetSpan(int sizeHint = 0)
    {
        var span = this._target.GetSpan(sizeHint);
        span.Clear();
        return span;
    }
}