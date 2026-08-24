// <copyright file="Connection.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.Network;

using System.Buffers;
using System.Diagnostics;
using System.Diagnostics.Metrics;
using System.IO.Pipelines;
using System.Net;
using System.Threading;
using Microsoft.Extensions.Logging;
using MUnique.OpenMU.Network.SimpleModulus;
using MUnique.OpenMU.PlugIns;
using Nito.AsyncEx;
using Nito.AsyncEx.Synchronous;
using Pipelines.Sockets.Unofficial;

/// <summary>
/// A connection which works on <see cref="IDuplexPipe"/>.
/// </summary>
/// <seealso cref="MUnique.OpenMU.Network.PacketPipeReaderBase" />
public sealed class Connection : PacketPipeReaderBase, IConnection
{
    private static readonly ActivitySource ActivitySource = new(typeof(Connection).FullName ?? nameof(Connection));
    private static readonly Meter ConnectionMeter = new(MeterName);
    private static readonly Counter<long> IncomingBytesCounter = ConnectionMeter.CreateCounter<long>("IncomingBytes", "bytes");
    private static readonly Counter<long> OutgoingBytesCounter = ConnectionMeter.CreateCounter<long>("OutgoingBytes", "bytes");
    private static readonly Counter<long> InvalidBlocksCounter = ConnectionMeter.CreateCounter<long>("InvalidBlocks");
    private static readonly Counter<long> ConnectionCounter = ConnectionMeter.CreateCounter<long>("ConnectionCount");

    /// <summary>
    /// The start of an RDP connection attempt.
    /// </summary>
    /// <remarks>
    /// Some hackers try to break into servers.
    /// The packet mostly contains the content "Cookie: mstshash=Administr".
    /// </remarks>
    private static readonly byte[] RdpConnectionAttemptHeader = { 0x03, 0x00, 0x00 };

    private readonly IPipelinedEncryptor? _encryptionPipe;
    private readonly ILogger<Connection> _logger;
    private readonly EndPoint _remoteEndPoint;

    private readonly object _captureLock = new();

    private IDuplexPipe? _duplexPipe;
    private bool _disconnected;
    private ExtendedPipeWriter? _outputWriter;

    private volatile IPacketCaptureSink[]? _captureSinks;

    /// <summary>
    /// Initializes a new instance of the <see cref="Connection" /> class.
    /// </summary>
    /// <param name="duplexPipe">The duplex pipe of the (socket) connection.</param>
    /// <param name="decryptionPipe">The decryption pipe.</param>
    /// <param name="encryptionPipe">The encryption pipe.</param>
    /// <param name="logger">The logger.</param>
    public Connection(IDuplexPipe duplexPipe, IPipelinedDecryptor? decryptionPipe, IPipelinedEncryptor? encryptionPipe, ILogger<Connection> logger)
    {
        this._duplexPipe = duplexPipe;
        this._encryptionPipe = encryptionPipe;
        this._logger = logger;
        this.Source = decryptionPipe?.Reader ?? this._duplexPipe!.Input;
        this._remoteEndPoint = this.SocketConnection?.Socket.RemoteEndPoint ?? new IPEndPoint(IPAddress.Any, 0);
        this.LocalEndPoint = this.SocketConnection?.Socket.LocalEndPoint;
        this.OutputLock = new();
    }

    /// <inheritdoc />
    public event AsyncEventHandler<ReadOnlySequence<byte>>? PacketReceived;

    /// <inheritdoc />
    public event AsyncEventHandler? Disconnected;

    /// <inheritdoc />
    public Guid Id { get; } = Guid.NewGuid();

    /// <inheritdoc />
    public bool Connected => this.SocketConnection != null ? this.SocketConnection.ShutdownKind == PipeShutdownKind.None && !this._disconnected : !this._disconnected;

    /// <inheritdoc />
    public EndPoint? EndPoint => this._remoteEndPoint;

    /// <inheritdoc />
    public EndPoint? LocalEndPoint { get; }

    /// <inheritdoc />
    public PipeWriter Output => this.OutputWriter;

    /// <inheritdoc />
    public AsyncLock OutputLock { get; }

    /// <summary>
    /// Gets the name of the meter.
    /// </summary>
    internal static string MeterName => typeof(Connection).FullName ?? nameof(Connection);

    /// <summary>
    /// Gets the socket connection, if the <see cref="_duplexPipe"/> is an instance of <see cref="SocketConnection"/>. Otherwise, it returns null.
    /// </summary>
    private SocketConnection? SocketConnection => this._duplexPipe as SocketConnection;

    /// <summary>
    /// Gets the <see cref="ExtendedPipeWriter"/> of the <see cref="Output"/>, which is also
    /// the place where the outgoing data packets are captured.
    /// </summary>
    private ExtendedPipeWriter OutputWriter => this._outputWriter ??= this.CreateOutputWriter(this._duplexPipe!);

    /// <inheritdoc/>
    public override string ToString() => this._remoteEndPoint?.ToString() ?? $"{base.ToString()} {this.GetHashCode()}";

    /// <inheritdoc/>
    public async Task BeginReceiveAsync()
    {
        try
        {
            ConnectionCounter.Add(1);
            await this.ReadSourceAsync().ConfigureAwait(false);
        }
        catch (OperationCanceledException)
        {
            // not an error which we need to handle.
        }
        catch (Exception ex)
        {
            await this.OnCompleteAsync(ex).ConfigureAwait(false);
            return;
        }

        await this.OnCompleteAsync(null).ConfigureAwait(false);
    }

    /// <inheritdoc />
    public async ValueTask DisconnectAsync()
    {
        using var scope = this._logger.BeginScope(this._remoteEndPoint);
        if (this._disconnected)
        {
            this._logger.LogDebug("Connection already disconnected.");
            return;
        }

        ConnectionCounter.Add(-1);
        this._logger.LogDebug("Disconnecting...");
        if (this._duplexPipe is not null)
        {
            await this.Source.CompleteAsync().ConfigureAwait(false);
            await this.Output.CompleteAsync().ConfigureAwait(false);
            (this._duplexPipe as IDisposable)?.Dispose();
            this._duplexPipe = null;
        }

        this._logger.LogDebug("Disconnected");
        this._disconnected = true;

        await this.Disconnected.SafeInvokeAsync().ConfigureAwait(false);
    }

    /// <inheritdoc />
    public void AddCaptureSink(IPacketCaptureSink sink)
    {
        lock (this._captureLock)
        {
            var current = this._captureSinks;
            if (current is null)
            {
                this._captureSinks = new[] { sink };
            }
            else
            {
                if (Array.IndexOf(current, sink) >= 0)
                {
                    return;
                }

                var updated = new IPacketCaptureSink[current.Length + 1];
                current.CopyTo(updated, 0);
                updated[^1] = sink;
                this._captureSinks = updated;
            }

            // We don't use the OutputWriter property here, because the connection may already
            // be disconnected - in that case there is no outgoing traffic to capture anymore.
            var outputWriter = this._outputWriter
                               ?? (this._duplexPipe is { } duplexPipe ? this._outputWriter = this.CreateOutputWriter(duplexPipe) : null);
            if (outputWriter is not null)
            {
                outputWriter.PacketCollector ??= new OutgoingPacketCollector(this.OnPacketSent);
            }
        }
    }

    /// <inheritdoc />
    public void RemoveCaptureSink(IPacketCaptureSink sink)
    {
        lock (this._captureLock)
        {
            var current = this._captureSinks;
            if (current is null)
            {
                return;
            }

            var index = Array.IndexOf(current, sink);
            if (index < 0)
            {
                return;
            }

            if (current.Length == 1)
            {
                this.StopCapturing();
                return;
            }

            var updated = new IPacketCaptureSink[current.Length - 1];
            Array.Copy(current, updated, index);
            Array.Copy(current, index + 1, updated, index, current.Length - index - 1);
            this._captureSinks = updated;
        }
    }

    /// <inheritdoc/>
    public void Dispose()
    {
        this.DisconnectAsync().AsTask().WaitAndUnwrapException();
        this.PacketReceived = null;
        this.Disconnected = null;
        lock (this._captureLock)
        {
            this.StopCapturing();
        }
    }

    /// <inheritdoc />
    protected override async ValueTask OnCompleteAsync(Exception? exception)
    {
        using var scope = this._logger.BeginScope(this._remoteEndPoint);
        if (exception is InvalidBlockChecksumException)
        {
            InvalidBlocksCounter.Add(1, new KeyValuePair<string, object?>("RemoteEndPoint", this._remoteEndPoint));
        }

        if (exception != null)
        {
            if (exception is ConnectionResetException)
            {
                this._logger.LogInformation(exception, "Connection was closed.");
            }
            else if (exception is ConnectionAbortedException)
            {
                this._logger.LogInformation(exception, "Connection was aborted by the server.");
            }
            else if (exception is InvalidOperationException && exception.Message == "Reading is not allowed after reader was completed.")
            {
                this._logger.LogInformation(exception, "Reader was completed.");
            }
            else if (exception is InvalidPacketHeaderException packetHeaderException
                     && packetHeaderException.Header.Take(3).SequenceEqual(RdpConnectionAttemptHeader))
            {
                this._logger.LogWarning("Connection will be closed, RDP connection attempt by {endPoint}", this.EndPoint);
            }
            else
            {
                this._logger.LogError(exception, "Connection will be disconnected, because of an exception");
            }
        }

        await this.Output.CompleteAsync(exception).ConfigureAwait(false);
        await this.DisconnectAsync().ConfigureAwait(false);
    }

    /// <summary>
    /// Reads the mu online packet by raising <see cref="PacketReceived" />.
    /// </summary>
    /// <param name="packet">The mu online packet.</param>
    /// <returns><see langword="true" />, if the flush was successful or not required.<see langword="false" />, if the pipe reader is completed and no longer reading data.</returns>
    protected override async ValueTask<bool> ReadPacketAsync(ReadOnlySequence<byte> packet)
    {
        IncomingBytesCounter.Add(packet.Length);

        using var activity = ActivitySource.CreateActivity("Read Packet", ActivityKind.Server);
        activity?.SetTag("remoteEndPoint", this._remoteEndPoint)
                .SetTag("rawPacket", packet)
                .Start();
        try
        {
            if (this._captureSinks is not null)
            {
                if (packet.IsSingleSegment)
                {
                    this.RaisePacketCaptured(packet.FirstSpan, false);
                }
                else
                {
                    this.RaisePacketCaptured(packet.ToArray(), false);
                }
            }

            await this.PacketReceived.SafeInvokeAsync(packet).ConfigureAwait(false);
            return true;
        }
        finally
        {
            activity?.Stop();
        }
    }

    private ExtendedPipeWriter CreateOutputWriter(IDuplexPipe duplexPipe)
    {
        return new ExtendedPipeWriter(this._encryptionPipe?.Writer ?? duplexPipe.Output, OutgoingBytesCounter);
    }

    /// <summary>
    /// Stops the capturing. Must be called within a lock of the <see cref="_captureLock"/>.
    /// </summary>
    private void StopCapturing()
    {
        this._captureSinks = null;
        if (this._outputWriter is { } outputWriter)
        {
            outputWriter.PacketCollector = null;
        }
    }

    private void OnPacketSent(ReadOnlySpan<byte> packet)
    {
        this.RaisePacketCaptured(packet, true);
    }

    private void RaisePacketCaptured(ReadOnlySpan<byte> packet, bool sent)
    {
        if (this._captureSinks is not { } sinks)
        {
            return;
        }

        foreach (var sink in sinks)
        {
            try
            {
                sink.PacketCaptured(packet, sent);
            }
            catch (Exception ex)
            {
                this._logger.LogWarning(ex, "Error in a packet capture sink of connection {connectionId}.", this.Id);
            }
        }
    }
}