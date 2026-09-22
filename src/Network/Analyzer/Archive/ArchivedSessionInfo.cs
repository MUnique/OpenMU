// <copyright file="ArchivedSessionInfo.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.Network.Analyzer.Archive;

using System.Globalization;

/// <summary>
/// The information about one session in the archive.
/// </summary>
public sealed class ArchivedSessionInfo
{
    /// <summary>
    /// Initializes a new instance of the <see cref="ArchivedSessionInfo"/> class.
    /// </summary>
    /// <param name="id">The identifier of the session, which is its path relative to the
    /// archive.</param>
    /// <param name="directoryPath">The full path of the directory of the session.</param>
    /// <param name="metadata">The metadata of the session.</param>
    /// <param name="sizeInBytes">The size of the session on the file system.</param>
    /// <param name="isRunning">If set to <c>true</c>, the session is currently being written.</param>
    public ArchivedSessionInfo(string id, string directoryPath, ArchivedSessionMetadata metadata, long sizeInBytes, bool isRunning)
    {
        this.Id = id;
        this.DirectoryPath = directoryPath;
        this.Metadata = metadata;
        this.SizeInBytes = sizeInBytes;
        this.IsRunning = isRunning;
    }

    /// <summary>
    /// Gets the identifier of the session, which is its path relative to the archive.
    /// </summary>
    public string Id { get; }

    /// <summary>
    /// Gets the full path of the directory of the session.
    /// </summary>
    public string DirectoryPath { get; }

    /// <summary>
    /// Gets the metadata of the session.
    /// </summary>
    public ArchivedSessionMetadata Metadata { get; }

    /// <summary>
    /// Gets the size of the session on the file system, in bytes.
    /// </summary>
    public long SizeInBytes { get; }

    /// <summary>
    /// Gets a value indicating whether the session is currently being written, because the
    /// observed player is still online.
    /// </summary>
    public bool IsRunning { get; }

    /// <summary>
    /// Gets the duration of the session. For a running session, it's the duration so far.
    /// </summary>
    public TimeSpan Duration => (this.Metadata.EndTimestamp ?? DateTime.UtcNow) - this.Metadata.StartTimestamp;

    /// <summary>
    /// Gets the name which should be shown for this session.
    /// </summary>
    public string DisplayName => string.Create(
        CultureInfo.InvariantCulture,
        $"{this.Metadata.AccountName} ({this.Metadata.StartTimestamp:yyyy-MM-dd HH:mm:ss})");
}
