// <copyright file="PacketArchive.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.Network.Analyzer.Archive;

using System.Collections.Concurrent;
using System.Globalization;
using System.IO;
using System.Text.Json;
using Microsoft.Extensions.Logging;

/// <summary>
/// The implementation of the <see cref="IPacketArchive"/>, which keeps the sessions on the
/// file system.
/// </summary>
/// <remarks>
/// A session is a directory below the one of its account, which holds the captured packets in
/// one or more files of the analyzer tool format, plus the metadata as json. That way, a
/// session can be deleted, copied or opened as a whole.
/// </remarks>
public sealed class PacketArchive : IPacketArchive
{
    private readonly NetworkObservationOptions _options;

    private readonly ILogger<PacketArchive> _logger;

    private readonly ConcurrentDictionary<string, ArchivedSessionWriter> _runningSessions = new();

    /// <summary>
    /// Initializes a new instance of the <see cref="PacketArchive"/> class.
    /// </summary>
    /// <param name="options">The options of the network observation.</param>
    /// <param name="logger">The logger.</param>
    public PacketArchive(NetworkObservationOptions options, ILogger<PacketArchive> logger)
    {
        this._options = options;
        this._logger = logger;
    }

    /// <summary>
    /// Gets the full path of the archive.
    /// </summary>
    public string ArchivePath => this._options.GetFullArchivePath();

    /// <inheritdoc />
    public async ValueTask<ArchivedSessionWriter?> StartSessionAsync(ArchivedSessionMetadata metadata)
    {
        await this.ApplyHousekeepingAsync().ConfigureAwait(false);

        if (metadata.StartTimestamp == default)
        {
            metadata.StartTimestamp = DateTime.UtcNow;
        }

        string sessionId;
        string directoryPath;
        try
        {
            var accountDirectory = GetSafeName(metadata.AccountName);
            var sessionDirectory = string.Create(
                CultureInfo.InvariantCulture,
                $"{metadata.StartTimestamp:yyyy-MM-dd_HH-mm-ss}_{metadata.ServerId}");
            sessionId = $"{accountDirectory}/{sessionDirectory}";
            directoryPath = Path.Combine(this.ArchivePath, accountDirectory, sessionDirectory);
            Directory.CreateDirectory(directoryPath);
        }
        catch (Exception ex)
        {
            this._logger.LogError(ex, "Could not create the archive directory for the account {AccountName}.", metadata.AccountName);
            return null;
        }

        var writer = new ArchivedSessionWriter(
            directoryPath,
            metadata,
            this._options.GetMaximumSessionSizeInBytes(),
            this._logger,
            () => this.OnSessionClosedAsync(sessionId));
        this._runningSessions[sessionId] = writer;
        await writer.SaveMetadataAsync().ConfigureAwait(false);

        // Observing a player is an intrusion into their privacy, so it leaves a trace.
        this._logger.LogInformation(
            "Started to archive the traffic of the observed account {AccountName} at {SessionId}.",
            metadata.AccountName,
            sessionId);
        return writer;
    }

    /// <inheritdoc />
    public async ValueTask<IReadOnlyList<ArchivedSessionInfo>> GetSessionsAsync(string? accountName = null)
    {
        var archivePath = this.ArchivePath;
        if (!Directory.Exists(archivePath))
        {
            return [];
        }

        var accountDirectories = string.IsNullOrEmpty(accountName)
            ? Directory.EnumerateDirectories(archivePath)
            : [Path.Combine(archivePath, GetSafeName(accountName))];

        var result = new List<ArchivedSessionInfo>();
        foreach (var accountDirectory in accountDirectories)
        {
            if (!Directory.Exists(accountDirectory))
            {
                continue;
            }

            foreach (var sessionDirectory in Directory.EnumerateDirectories(accountDirectory))
            {
                if (await this.TryReadSessionAsync(sessionDirectory).ConfigureAwait(false) is { } session)
                {
                    result.Add(session);
                }
            }
        }

        result.Sort((left, right) => right.Metadata.StartTimestamp.CompareTo(left.Metadata.StartTimestamp));
        return result;
    }

    /// <inheritdoc />
    public async ValueTask<ArchivedSessionInfo?> GetSessionAsync(string sessionId)
    {
        if (this.TryGetSessionDirectory(sessionId) is not { } directoryPath)
        {
            return null;
        }

        return await this.TryReadSessionAsync(directoryPath).ConfigureAwait(false);
    }

    /// <inheritdoc />
    public ValueTask<bool> DeleteSessionAsync(string sessionId)
    {
        if (this._runningSessions.ContainsKey(sessionId))
        {
            this._logger.LogInformation("The archived session {SessionId} is still running and was not deleted.", sessionId);
            return ValueTask.FromResult(false);
        }

        if (this.TryGetSessionDirectory(sessionId) is not { } directoryPath || !Directory.Exists(directoryPath))
        {
            return ValueTask.FromResult(false);
        }

        try
        {
            Directory.Delete(directoryPath, true);
            this.RemoveAccountDirectoryIfEmpty(Path.GetDirectoryName(directoryPath));
            this._logger.LogInformation("The archived session {SessionId} has been deleted.", sessionId);
            return ValueTask.FromResult(true);
        }
        catch (Exception ex)
        {
            this._logger.LogWarning(ex, "Could not delete the archived session {SessionId}.", sessionId);
            return ValueTask.FromResult(false);
        }
    }

    /// <inheritdoc />
    public async ValueTask ApplyHousekeepingAsync()
    {
        try
        {
            var sessions = await this.GetSessionsAsync().ConfigureAwait(false);
            var removable = sessions.Where(session => !session.IsRunning).ToList();

            if (this._options.RetentionDays > 0)
            {
                var oldestAllowedStart = DateTime.UtcNow.AddDays(-this._options.RetentionDays);
                foreach (var session in removable.Where(session => session.Metadata.StartTimestamp < oldestAllowedStart).ToList())
                {
                    await this.DeleteSessionAsync(session.Id).ConfigureAwait(false);
                    removable.Remove(session);
                }
            }

            if (this._options.MaximumTotalSizeMb <= 0)
            {
                return;
            }

            var maximumTotalSize = (long)this._options.MaximumTotalSizeMb * 1024 * 1024;
            var totalSize = sessions.Sum(session => session.SizeInBytes);

            // The oldest sessions are removed first - the newest traffic is the interesting one.
            foreach (var session in removable.OrderBy(session => session.Metadata.StartTimestamp))
            {
                if (totalSize <= maximumTotalSize)
                {
                    return;
                }

                if (await this.DeleteSessionAsync(session.Id).ConfigureAwait(false))
                {
                    totalSize -= session.SizeInBytes;
                }
            }
        }
        catch (Exception ex)
        {
            this._logger.LogWarning(ex, "Error during the housekeeping of the packet archive.");
        }
    }

    private static string GetSafeName(string name)
    {
        var safeName = string.Join('_', name.Split(Path.GetInvalidFileNameChars(), StringSplitOptions.RemoveEmptyEntries));
        return string.IsNullOrWhiteSpace(safeName) ? "unknown" : safeName;
    }

    private async ValueTask OnSessionClosedAsync(string sessionId)
    {
        this._runningSessions.TryRemove(sessionId, out _);
        this._logger.LogInformation("Finished the archived session {SessionId}.", sessionId);
        await this.ApplyHousekeepingAsync().ConfigureAwait(false);
    }

    private string? TryGetSessionDirectory(string sessionId)
    {
        if (string.IsNullOrWhiteSpace(sessionId))
        {
            return null;
        }

        var archivePath = this.ArchivePath;
        var directoryPath = Path.GetFullPath(Path.Combine(archivePath, sessionId.Replace('/', Path.DirectorySeparatorChar)));

        // The identifier comes from the outside, so it must not be able to point somewhere else.
        if (!directoryPath.StartsWith(archivePath + Path.DirectorySeparatorChar, StringComparison.Ordinal))
        {
            this._logger.LogWarning("The session identifier {SessionId} points outside of the archive.", sessionId);
            return null;
        }

        return directoryPath;
    }

    private async ValueTask<ArchivedSessionInfo?> TryReadSessionAsync(string directoryPath)
    {
        var metadataPath = Path.Combine(directoryPath, ArchivedSessionWriter.MetadataFileName);
        if (!File.Exists(metadataPath))
        {
            return null;
        }

        try
        {
            await using var stream = new FileStream(metadataPath, FileMode.Open, FileAccess.Read, FileShare.ReadWrite | FileShare.Delete);
            if (await JsonSerializer.DeserializeAsync<ArchivedSessionMetadata>(stream).ConfigureAwait(false) is not { } metadata)
            {
                return null;
            }

            var archivePath = this.ArchivePath;
            var sessionId = Path.GetRelativePath(archivePath, directoryPath).Replace(Path.DirectorySeparatorChar, '/');
            var size = new DirectoryInfo(directoryPath).EnumerateFiles().Sum(file => file.Length);
            return new ArchivedSessionInfo(sessionId, directoryPath, metadata, size, this._runningSessions.ContainsKey(sessionId));
        }
        catch (Exception ex)
        {
            this._logger.LogWarning(ex, "Could not read the archived session at {Path}.", directoryPath);
            return null;
        }
    }

    private void RemoveAccountDirectoryIfEmpty(string? accountDirectory)
    {
        if (accountDirectory is null
            || !Directory.Exists(accountDirectory)
            || Directory.EnumerateFileSystemEntries(accountDirectory).Any())
        {
            return;
        }

        Directory.Delete(accountDirectory);
    }
}
