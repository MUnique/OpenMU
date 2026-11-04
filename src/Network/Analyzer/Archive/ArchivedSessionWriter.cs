// <copyright file="ArchivedSessionWriter.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.Network.Analyzer.Archive;

using System.Globalization;
using System.IO;
using System.Text.Json;
using System.Threading;
using System.Threading.Channels;
using Microsoft.Extensions.Logging;

/// <summary>
/// Writes the captured packets of an observed session into the archive, while it's running.
/// </summary>
/// <remarks>
/// The packets are written by an own task, so that the network thread which captured a packet
/// is never waiting for the file system. They are appended and flushed as soon as the queue
/// ran empty, so that a running session can be read - and survives a crash of the process.
/// </remarks>
public sealed class ArchivedSessionWriter : IPacketCaptureSink, IAsyncDisposable
{
    /// <summary>
    /// The name of the file which contains the metadata of the session.
    /// </summary>
    public const string MetadataFileName = "session.json";

    /// <summary>
    /// The extension of the files which contain the packets. It's the one of the analyzer
    /// tool, so that an archived session can be opened with it.
    /// </summary>
    public const string PartFileExtension = ".mucap";

    /// <summary>
    /// The maximum number of packets which are waiting to be written. When the file system
    /// can't keep up with the traffic, the packets above it are dropped - the connection of
    /// the player must not be slowed down by the observation.
    /// </summary>
    private const int QueueCapacity = 10000;

    private static readonly JsonSerializerOptions JsonOptions = new() { WriteIndented = true };

    private readonly string _directoryPath;

    private readonly ArchivedSessionMetadata _metadata;

    private readonly long _maximumPartSize;

    private readonly ILogger _logger;

    private readonly Func<ValueTask>? _onClosedAsync;

    private readonly Channel<CapturedPacket> _channel = Channel.CreateBounded<CapturedPacket>(
        new BoundedChannelOptions(QueueCapacity) { SingleReader = true, FullMode = BoundedChannelFullMode.Wait });

    private readonly SemaphoreSlim _metadataSemaphore = new(1);

    private readonly Task _writeLoop;

    private StreamWriter? _currentPart;

    private long _currentPartSize;

    private long _sequence;

    private long _droppedPacketCount;

    private bool _isDisposed;

    /// <summary>
    /// Initializes a new instance of the <see cref="ArchivedSessionWriter"/> class.
    /// </summary>
    /// <param name="directoryPath">The path of the directory of this session.</param>
    /// <param name="metadata">The metadata of the session.</param>
    /// <param name="maximumPartSize">The maximum size of one file of the session, in bytes.</param>
    /// <param name="logger">The logger.</param>
    /// <param name="onClosedAsync">The callback which is invoked when the session is closed.</param>
    public ArchivedSessionWriter(
        string directoryPath,
        ArchivedSessionMetadata metadata,
        long maximumPartSize,
        ILogger logger,
        Func<ValueTask>? onClosedAsync = null)
    {
        this._directoryPath = directoryPath;
        this._metadata = metadata;
        this._maximumPartSize = maximumPartSize;
        this._logger = logger;
        this._onClosedAsync = onClosedAsync;
        this._writeLoop = Task.Run(this.WriteLoopAsync);
    }

    /// <summary>
    /// Gets the metadata of the session.
    /// </summary>
    public ArchivedSessionMetadata Metadata => this._metadata;

    /// <summary>
    /// Adds the name of a character which is played in this session.
    /// </summary>
    /// <param name="characterName">The name of the character.</param>
    /// <returns>The async task.</returns>
    public async ValueTask AddCharacterNameAsync(string characterName)
    {
        if (string.IsNullOrWhiteSpace(characterName))
        {
            return;
        }

        await this._metadataSemaphore.WaitAsync().ConfigureAwait(false);
        try
        {
            if (this._metadata.CharacterNames.Contains(characterName))
            {
                return;
            }

            this._metadata.CharacterNames.Add(characterName);
        }
        finally
        {
            this._metadataSemaphore.Release();
        }

        await this.SaveMetadataAsync().ConfigureAwait(false);
    }

    /// <summary>
    /// Saves the current state of the metadata, so that the session is visible in the archive
    /// while it's still running.
    /// </summary>
    /// <returns>The async task.</returns>
    public async ValueTask SaveMetadataAsync()
    {
        await this._metadataSemaphore.WaitAsync().ConfigureAwait(false);
        try
        {
            this._metadata.PacketCount = Interlocked.Read(ref this._sequence);
            this._metadata.DroppedPacketCount = Interlocked.Read(ref this._droppedPacketCount);
            var json = JsonSerializer.Serialize(this._metadata, JsonOptions);
            var path = Path.Combine(this._directoryPath, MetadataFileName);
            await File.WriteAllTextAsync(path, json).ConfigureAwait(false);
        }
        catch (Exception ex)
        {
            this._logger.LogWarning(ex, "Could not save the metadata of the archived session at {Path}.", this._directoryPath);
        }
        finally
        {
            this._metadataSemaphore.Release();
        }
    }

    /// <inheritdoc />
    public void PacketCaptured(ReadOnlySpan<byte> packet, bool sent)
    {
        // A packet which was sent to the remote endpoint of a server connection is a packet
        // which goes to the client; a received one goes to the server.
        var captured = new CapturedPacket(
            DateTime.UtcNow - this._metadata.StartTimestamp,
            packet.ToArray(),
            !sent,
            Interlocked.Increment(ref this._sequence));

        if (!this._channel.Writer.TryWrite(captured))
        {
            // The sequence numbers make such a gap visible in the archived session.
            Interlocked.Increment(ref this._droppedPacketCount);
        }
    }

    /// <inheritdoc />
    public async ValueTask DisposeAsync()
    {
        if (this._isDisposed)
        {
            return;
        }

        this._isDisposed = true;
        this._channel.Writer.TryComplete();
        try
        {
#pragma warning disable VSTHRD003 // The loop is started by this instance, so awaiting it here can't deadlock.
            await this._writeLoop.ConfigureAwait(false);
#pragma warning restore VSTHRD003
        }
        catch (Exception ex)
        {
            this._logger.LogWarning(ex, "Error while finishing the archived session at {Path}.", this._directoryPath);
        }

        await this.CloseCurrentPartAsync().ConfigureAwait(false);
        this._metadata.EndTimestamp = DateTime.UtcNow;
        await this.SaveMetadataAsync().ConfigureAwait(false);
        this._metadataSemaphore.Dispose();

        if (this._onClosedAsync is { } onClosedAsync)
        {
            await onClosedAsync().ConfigureAwait(false);
        }
    }

    private async Task WriteLoopAsync()
    {
        var reader = this._channel.Reader;
        while (await reader.WaitToReadAsync().ConfigureAwait(false))
        {
            while (reader.TryRead(out var packet))
            {
                await this.WritePacketAsync(packet).ConfigureAwait(false);
            }

            if (this._currentPart is { } currentPart)
            {
                try
                {
                    await currentPart.FlushAsync().ConfigureAwait(false);
                }
                catch (Exception ex)
                {
                    this._logger.LogWarning(ex, "Could not flush the archived session at {Path}.", this._directoryPath);
                }
            }
        }
    }

    private async ValueTask WritePacketAsync(CapturedPacket packet)
    {
        try
        {
            var writer = await this.GetCurrentPartAsync().ConfigureAwait(false);
            if (writer is null)
            {
                return;
            }

            var data = new Packet(packet.Timestamp, packet.Data, packet.ToServer);
            var line = string.Create(
                CultureInfo.InvariantCulture,
                $"{packet.Timestamp.Ticks};{packet.ToServer};{data.Size};{data.PacketData};{packet.Sequence}");
            await writer.WriteLineAsync(line).ConfigureAwait(false);
            this._currentPartSize += line.Length + Environment.NewLine.Length;
            if (this._currentPartSize >= this._maximumPartSize)
            {
                // The session continues in another file, so that a single one stays small
                // enough to be opened by the analyzer tool.
                await this.CloseCurrentPartAsync().ConfigureAwait(false);
            }
        }
        catch (Exception ex)
        {
            this._logger.LogWarning(ex, "Could not write a packet of the archived session at {Path}.", this._directoryPath);
        }
    }

    private async ValueTask<StreamWriter?> GetCurrentPartAsync()
    {
        if (this._currentPart is { } currentPart)
        {
            return currentPart;
        }

        var partName = string.Create(CultureInfo.InvariantCulture, $"part-{this._metadata.Parts.Count:D3}{PartFileExtension}");
        var stream = new FileStream(
            Path.Combine(this._directoryPath, partName),
            FileMode.Create,
            FileAccess.Write,
            FileShare.ReadWrite | FileShare.Delete);
        this._currentPart = new StreamWriter(stream);
        this._currentPartSize = 0;

        // Each part starts with the timestamp of the session, so that it can be loaded on its
        // own - the timestamps of the packets are relative to it.
        var startTimestamp = this._metadata.StartTimestamp.ToString("O", CultureInfo.InvariantCulture);
        await this._currentPart.WriteLineAsync(startTimestamp).ConfigureAwait(false);
        await this._currentPart.FlushAsync().ConfigureAwait(false);

        await this._metadataSemaphore.WaitAsync().ConfigureAwait(false);
        try
        {
            this._metadata.Parts.Add(partName);
        }
        finally
        {
            this._metadataSemaphore.Release();
        }

        await this.SaveMetadataAsync().ConfigureAwait(false);
        return this._currentPart;
    }

    private async ValueTask CloseCurrentPartAsync()
    {
        if (this._currentPart is not { } currentPart)
        {
            return;
        }

        this._currentPart = null;
        try
        {
            await currentPart.DisposeAsync().ConfigureAwait(false);
        }
        catch (Exception ex)
        {
            this._logger.LogWarning(ex, "Could not close a file of the archived session at {Path}.", this._directoryPath);
        }
    }

    private readonly record struct CapturedPacket(TimeSpan Timestamp, byte[] Data, bool ToServer, long Sequence);
}
