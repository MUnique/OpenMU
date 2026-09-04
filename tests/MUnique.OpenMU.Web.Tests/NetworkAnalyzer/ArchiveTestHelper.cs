// <copyright file="ArchiveTestHelper.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.Web.Tests.NetworkAnalyzer;

using System.IO;
using Microsoft.Extensions.Logging.Abstractions;
using MUnique.OpenMU.Interfaces;
using MUnique.OpenMU.Network.Analyzer.Archive;
using MUnique.OpenMU.Network.PlugIns;

/// <summary>
/// Creates archives with sessions for the tests of the network analyzer page.
/// </summary>
public static class ArchiveTestHelper
{
    /// <summary>
    /// Creates a path of an archive directory which doesn't exist yet.
    /// </summary>
    /// <returns>The path of the archive directory.</returns>
    public static string CreateArchivePath()
    {
        return Path.Combine(Path.GetTempPath(), "openmu-page-archive-" + Guid.NewGuid().ToString("N"));
    }

    /// <summary>
    /// Creates an archive at the specified path.
    /// </summary>
    /// <param name="archivePath">The path of the archive.</param>
    /// <returns>The created archive.</returns>
    public static PacketArchive CreateArchive(string archivePath)
    {
        return new PacketArchive(
            new NetworkObservationOptions { ArchivePath = archivePath },
            new NullLogger<PacketArchive>());
    }

    /// <summary>
    /// Adds a finished session of the specified account to the archive.
    /// </summary>
    /// <param name="archive">The archive.</param>
    /// <param name="accountName">The name of the account.</param>
    /// <param name="packet">The packet which is archived.</param>
    /// <returns>The information about the created session.</returns>
    public static async ValueTask<ArchivedSessionInfo> AddSessionAsync(PacketArchive archive, string accountName, byte[] packet)
    {
        var metadata = new ArchivedSessionMetadata
        {
            AccountName = accountName,
            ServerType = ServerType.GameServer,
            ServerId = 1,
            ServerDescription = "Test Server",
            RemoteEndPoint = "127.0.0.1:1234",
            ClientVersion = new ClientVersion(6, 3, ClientLanguage.English),
            StartTimestamp = DateTime.UtcNow,
        };

        var writer = await archive.StartSessionAsync(metadata).ConfigureAwait(false);
        writer!.PacketCaptured(packet, false);
        await writer.DisposeAsync().ConfigureAwait(false);

        var sessions = await archive.GetSessionsAsync(accountName).ConfigureAwait(false);
        return sessions.First(session => session.Metadata.StartTimestamp == metadata.StartTimestamp);
    }

    /// <summary>
    /// Removes the archive directory again.
    /// </summary>
    /// <param name="archivePath">The path of the archive.</param>
    public static void DeleteArchive(string archivePath)
    {
        if (Directory.Exists(archivePath))
        {
            Directory.Delete(archivePath, true);
        }
    }
}
