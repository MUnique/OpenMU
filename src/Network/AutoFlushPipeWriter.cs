namespace MUnique.OpenMU.Network;

using System.Diagnostics.Metrics;
using System.IO.Pipelines;
using System.Threading;
using Microsoft.Extensions.Logging;

/// <summary>
/// A wrapper for an existing <see cref="PipeWriter"/>, which flushes the advanced bytes automatically.
/// </summary>
public class AutoFlushPipeWriter : PipeWriter
{
    private readonly PipeWriter _target;
    private readonly ILogger _logger;
    private readonly Counter<long> _writeCounter;
    private readonly SemaphoreSlim _outputLock;

    private int _isMarkedForFlushing;

    /// <summary>
    /// Initializes a new instance of the <see cref="AutoFlushPipeWriter"/> class.
    /// </summary>
    /// <param name="target">The target <see cref="PipeWriter"/>.</param>
    /// <param name="outputLock">The semaphore which is used to synchronize flushing with writing to the pipe.</param>
    /// <param name="logger">The logger.</param>
    /// <param name="writeCounter">A counter for the written bytes.</param>
    public AutoFlushPipeWriter(PipeWriter target, SemaphoreSlim outputLock, ILogger logger, Counter<long> writeCounter)
    {
        this._target = target;
        this._logger = logger;
        this._writeCounter = writeCounter;
        this._outputLock = outputLock;
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

        // Mark this instance that flushing is required. If that was successful, run the flush
        var wasMarkedForFlushBefore = Interlocked.CompareExchange(ref this._isMarkedForFlushing, 1, 0) == 1;
        if (!wasMarkedForFlushBefore)
        {
            _ = Task.Run(this.DoFlushAsync);
        }
    }

    /// <inheritdoc />
    public override Memory<byte> GetMemory(int sizeHint = 0)
    {
        return this._target.GetMemory(sizeHint);
    }

    /// <inheritdoc />
    public override Span<byte> GetSpan(int sizeHint = 0)
    {
        return this._target.GetSpan(sizeHint);
    }

    private async Task DoFlushAsync()
    {
        // We wait 10 milliseconds, maybe there will be more bytes to be flushed.
        await Task.Delay(10);

        if (!await this._outputLock.WaitAsync(10))
        {
            return;
        }

        try
        {
            await this._target.FlushAsync().ConfigureAwait(false);
        }
        catch (Exception ex)
        {
            this._logger.LogError(ex, "Unexpected error at flushing");
        }
        finally
        {
            this._isMarkedForFlushing = 0;
            this._outputLock.Release();
        }
    }
}