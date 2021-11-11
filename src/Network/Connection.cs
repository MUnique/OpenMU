// <copyright file="Connection.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.Network;

using System.Buffers;
using System.IO.Pipelines;
using System.Net;
using Microsoft.Extensions.Logging;
using Pipelines.Sockets.Unofficial;

/// <summary>
/// A connection which works on <see cref="IDuplexPipe"/>.
/// </summary>
/// <seealso cref="MUnique.OpenMU.Network.PacketPipeReaderBase" />
public sealed class Connection : PacketPipeReaderBase, IConnection
{
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
    }

    /// <inheritdoc />
    public event PipedPacketReceivedHandler? PacketReceived;

    /// <inheritdoc />
    public event DisconnectedHandler? Disconnected;

    /// <inheritdoc />
    public bool Connected => this.SocketConnection != null ? this.SocketConnection.ShutdownKind == PipeShutdownKind.None && !this._disconnected : !this._disconnected;

    /// <inheritdoc />
    public EndPoint? EndPoint => this._remoteEndPoint;

    /// <inheritdoc />
    public PipeWriter Output => this._outputWriter ??= this._encryptionPipe?.Writer ?? this._duplexPipe!.Output;

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
            await this.ReadSource().ConfigureAwait(false);
        }
        catch (Exception e)
        {
            this.OnComplete(e);
            return;
        }

        this.OnComplete(null);
    }

    /// <inheritdoc />
    public void Disconnect()
    {
        using var scope = this._logger.BeginScope(this._remoteEndPoint);
        if (this._disconnected)
        {
            this._logger.LogDebug("Connection already disconnected.");
            return;
        }

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

        this.Disconnected?.Invoke(this, EventArgs.Empty);
    }

    /// <inheritdoc/>
    public void Dispose()
    {
        this.Disconnect();
        this.PacketReceived = null;
        this.Disconnected = null;
    }

    /// <inheritdoc />
    protected override void OnComplete(Exception? exception)
    {
        using var scope = this._logger.BeginScope(this._remoteEndPoint);
        if (exception != null)
        {
            this._logger.LogError(exception, "Connection will be disconnected, because of an exception");
        }

        this.Output.Complete(exception);
        this.Disconnect();
    }

    /// <summary>
    /// Reads the mu online packet by raising <see cref="PacketReceived" />.
    /// </summary>
    /// <param name="packet">The mu online packet.</param>
    /// <returns>The async task.</returns>
    protected override Task ReadPacket(ReadOnlySequence<byte> packet)
    {
        this.PacketReceived?.Invoke(this, packet);
        return Task.CompletedTask;
    }
}