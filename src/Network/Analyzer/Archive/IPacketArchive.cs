// <copyright file="IPacketArchive.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.Network.Analyzer.Archive;

/// <summary>
/// The archive which holds the traffic of the sessions of observed accounts.
/// </summary>
/// <remarks>
/// It's a feature of the game server: an observed account is archived as soon as it plays,
/// regardless of whether an admin panel is running somewhere.
/// </remarks>
public interface IPacketArchive
{
    /// <summary>
    /// Starts a new session in the archive.
    /// </summary>
    /// <param name="metadata">The metadata of the session. Its start timestamp is used as the
    /// reference for the timestamps of the packets.</param>
    /// <returns>The writer of the session, which is a sink of the connection; Or
    /// <see langword="null"/>, if the session couldn't be started.</returns>
    ValueTask<ArchivedSessionWriter?> StartSessionAsync(ArchivedSessionMetadata metadata);

    /// <summary>
    /// Gets the sessions which are in the archive, newest first.
    /// </summary>
    /// <param name="accountName">The name of the account, if only its sessions are wanted.</param>
    /// <returns>The sessions which are in the archive.</returns>
    ValueTask<IReadOnlyList<ArchivedSessionInfo>> GetSessionsAsync(string? accountName = null);

    /// <summary>
    /// Gets the information about the session with the specified identifier.
    /// </summary>
    /// <param name="sessionId">The identifier of the session.</param>
    /// <returns>The information about the session, if it exists; Otherwise,
    /// <see langword="null"/>.</returns>
    ValueTask<ArchivedSessionInfo?> GetSessionAsync(string sessionId);

    /// <summary>
    /// Deletes the session with the specified identifier.
    /// </summary>
    /// <param name="sessionId">The identifier of the session.</param>
    /// <returns><see langword="true"/>, if the session has been deleted.</returns>
    ValueTask<bool> DeleteSessionAsync(string sessionId);

    /// <summary>
    /// Applies the retention and the size limit of the archive, by removing the oldest
    /// sessions. A session which is currently written is never removed.
    /// </summary>
    /// <returns>The async task.</returns>
    ValueTask ApplyHousekeepingAsync();
}
