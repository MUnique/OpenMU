// <copyright file="Connection.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.Network;

using System.Diagnostics;
using System.Diagnostics.Metrics;
using System.Buffers;
using System.IO.Pipelines;
using System.Net;
using System.Threading;
using Microsoft.Extensions.Logging;
using Pipelines.Sockets.Unofficial;
using Nito.AsyncEx.Synchronous;
using MUnique.OpenMU.Network.SimpleModulus;
using MUnique.OpenMU.PlugIns;

/// <summary>
/// A connection which works on <see cref="IDuplexPipe"/>.
/// </summary>
/// <seealso cref="MUnique.OpenMU.Network.PacketPipeReaderBase" />
public sealed class Connection : PacketPipeReaderBase, IConnection
{
    private static readonly ActivitySource ActivitySource = new(typeof(Connection).FullName ?? nameof(Connection));
    private static readonly Meter ConnectionMeter = new (MeterName);
    private static readonly Counter<long> IncomingBytesCounter = ConnectionMeter.CreateCounter<long>("IncomingBytes", "bytes");
    private static readonly Counter<long> OutgoingBytesCounter = ConnectionMeter.CreateCounter<long>("OutgoingBytes", "bytes");
    private static readonly Counter<long> InvalidBlocksCounter = ConnectionMeter.CreateCounter<long>("InvalidBlocks");
    private static readonly Counter<long> ConnectionCounter = ConnectionMeter.CreateCounter<long>("ConnectionCount");

    private readonly IPipelinedEncryptor? _encryptionPipe;
    private readonly ILogger<Connection> _logger;
    private readonly EndPoint? _remoteEndPoint;

    private IDuplexPipe? _duplexPipe;
    private bool _disconnected;
    private PipeWriter? _outputWriter;

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
        this._remoteEndPoint = this.SocketConnection?.Socket.RemoteEndPoint;
        this.OutputLock = new SemaphoreSlim(1);
    }

    /// <inheritdoc />
    public event AsyncEventHandler<ReadOnlySequence<byte>>? PacketReceived;

    /// <inheritdoc />
    public event AsyncEventHandler? Disconnected;

    /// <inheritdoc />
    public bool Connected => this.SocketConnection != null ? this.SocketConnection.ShutdownKind == PipeShutdownKind.None && !this._disconnected : !this._disconnected;

    /// <inheritdoc />
    public EndPoint? EndPoint => this._remoteEndPoint;

    /// <inheritdoc />
    public PipeWriter Output => this._outputWriter ??= new AutoFlushPipeWriter(this._encryptionPipe?.Writer ?? this._duplexPipe!.Output, this.OutputLock, this._logger, OutgoingBytesCounter);

    /// <inheritdoc />
    public SemaphoreSlim OutputLock { get; }

    /// <summary>
    /// Gets the name of the meter.
    /// </summary>
    internal static string MeterName => typeof(Connection).FullName ?? nameof(Connection);

    /// <summary>
    /// Gets the socket connection, if the <see cref="_duplexPipe"/> is an instance of <see cref="SocketConnection"/>. Otherwise, it returns null.
    /// </summary>
    private SocketConnection? SocketConnection => this._duplexPipe as SocketConnection;

    /// <inheritdoc/>
    public override string ToString() => this._remoteEndPoint?.ToString() ?? $"{base.ToString()} {this.GetHashCode()}";

    /// <inheritdoc/>
    public async Task BeginReceive()
    {
        try
        {
            ConnectionCounter.Add(1);
            await this.ReadSource().ConfigureAwait(false);
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
            this.Source.Complete();
            this.Output.Complete();
            (this._duplexPipe as IDisposable)?.Dispose();
            this._duplexPipe = null;
        }

        this._logger.LogDebug("Disconnected");
        this._disconnected = true;

        await this.Disconnected.SafeInvoke().ConfigureAwait(false);
    }

    /// <inheritdoc/>
    public void Dispose()
    {
        this.DisconnectAsync().AsTask().WaitAndUnwrapException();
        this.PacketReceived = null;
        this.Disconnected = null;
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
    /// <returns>The async task.</returns>
    protected override async ValueTask ReadPacketAsync(ReadOnlySequence<byte> packet)
    {
        IncomingBytesCounter.Add(packet.Length);

        using var activity = ActivitySource.CreateActivity("Read Packet", ActivityKind.Server);
        activity?.SetTag("remoteEndPoint", this._remoteEndPoint)
                .SetTag("rawPacket", packet)
                .Start();
        try
        {
            await this.PacketReceived.SafeInvoke(packet);
        }
        finally
        {
            activity?.Stop();
        }
    }
}