// <copyright file="PacketArchiveTest.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.Network.Tests;

using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text.Json;
using Microsoft.Extensions.Logging.Abstractions;
using MUnique.OpenMU.Interfaces;
using MUnique.OpenMU.Network.Analyzer.Archive;
using MUnique.OpenMU.Network.PlugIns;

/// <summary>
/// Tests for the <see cref="PacketArchive"/>, which keeps the traffic of the observed accounts.
/// </summary>
[TestFixture]
public class PacketArchiveTest
{
    private static readonly byte[] LoginPacket = [0xC1, 0x06, 0xF1, 0x01, 0x01, 0x02];

    private static readonly byte[] ResponsePacket = [0xC1, 0x05, 0xF1, 0x00, 0x01];

    private string _archivePath = null!;

    /// <summary>
    /// Creates the archive directory of the test.
    /// </summary>
    [SetUp]
    public void SetUp()
    {
        this._archivePath = Path.Combine(Path.GetTempPath(), "openmu-archive-test-" + Guid.NewGuid().ToString("N"));
    }

    /// <summary>
    /// Removes the archive directory of the test.
    /// </summary>
    [TearDown]
    public void TearDown()
    {
        if (Directory.Exists(this._archivePath))
        {
            Directory.Delete(this._archivePath, true);
        }
    }

    /// <summary>
    /// Tests if the archived packets are the same as the captured ones, and that the metadata
    /// describes the finished session.
    /// </summary>
    /// <returns>The async task.</returns>
    [Test]
    public async Task ArchivedSessionContainsTheCapturedPacketsAsync()
    {
        var archive = this.CreateArchive();
        var writer = await archive.StartSessionAsync(CreateMetadata()).ConfigureAwait(false);
        Assert.That(writer, Is.Not.Null);

        writer!.PacketCaptured(LoginPacket, false);
        writer.PacketCaptured(ResponsePacket, true);
        await writer.DisposeAsync().ConfigureAwait(false);

        var sessions = await archive.GetSessionsAsync().ConfigureAwait(false);
        Assert.That(sessions, Has.Count.EqualTo(1));

        var session = await ArchivedSession.LoadAsync(sessions[0], 0).ConfigureAwait(false);
        Assert.That(session.PacketList, Has.Count.EqualTo(2));
        Assert.That(session.PacketList[0].PacketData, Is.EqualTo("C1 06 F1 01 01 02"));
        Assert.That(session.PacketList[0].ToServer, Is.True, "A received packet goes to the server.");
        Assert.That(session.PacketList[1].PacketData, Is.EqualTo("C1 05 F1 00 01"));
        Assert.That(session.PacketList[1].ToServer, Is.False, "A sent packet goes to the client.");

        Assert.That(sessions[0].Metadata.PacketCount, Is.EqualTo(2));
        Assert.That(sessions[0].Metadata.EndTimestamp, Is.Not.Null);
        Assert.That(sessions[0].Metadata.AccountName, Is.EqualTo("TestAccount"));
        Assert.That(sessions[0].Metadata.ClientVersion, Is.EqualTo(new ClientVersion(6, 3, ClientLanguage.English)));
        Assert.That(sessions[0].IsRunning, Is.False);
    }

    /// <summary>
    /// Tests if a session can be read while it's still being written - an admin should be able
    /// to look at the traffic of a player which is still online.
    /// </summary>
    /// <returns>The async task.</returns>
    [Test]
    public async Task RunningSessionCanBeReadAsync()
    {
        var archive = this.CreateArchive();
        var writer = await archive.StartSessionAsync(CreateMetadata()).ConfigureAwait(false);
        writer!.PacketCaptured(LoginPacket, false);

        var session = await this.WaitForPacketsAsync(archive, 1).ConfigureAwait(false);
        Assert.That(session.PacketList, Has.Count.EqualTo(1));
        Assert.That(session.Info.IsRunning, Is.True, "The session is still being written.");
        Assert.That(session.Info.Metadata.EndTimestamp, Is.Null);

        writer.PacketCaptured(ResponsePacket, true);
        var updated = await this.WaitForPacketsAsync(archive, 2).ConfigureAwait(false);
        Assert.That(updated.PacketList, Has.Count.EqualTo(2));

        await writer.DisposeAsync().ConfigureAwait(false);
    }

    /// <summary>
    /// Tests if a session which grows over the configured size is continued in another file,
    /// so that a single one stays small enough to be opened by the analyzer tool.
    /// </summary>
    /// <returns>The async task.</returns>
    [Test]
    public async Task BigSessionIsSplitIntoSeveralFilesAsync()
    {
        var archive = this.CreateArchive();
        var metadata = CreateMetadata();
        var directoryPath = Path.Combine(this._archivePath, "TestAccount", $"{metadata.StartTimestamp:yyyy-MM-dd_HH-mm-ss}_1");
        Directory.CreateDirectory(directoryPath);

        // One packet is about 30 bytes as text, so each one of them exceeds this maximum.
        await using (var writer = new ArchivedSessionWriter(directoryPath, metadata, 10, new NullLogger<PacketArchive>()))
        {
            for (int i = 0; i < 5; i++)
            {
                writer.PacketCaptured(LoginPacket, false);
            }
        }

        var sessions = await archive.GetSessionsAsync().ConfigureAwait(false);
        var session = await ArchivedSession.LoadAsync(sessions[0], 0).ConfigureAwait(false);
        Assert.That(sessions[0].Metadata.Parts, Has.Count.EqualTo(5), "Each packet exceeds the maximum, so each one gets an own file.");
        Assert.That(session.PacketList, Has.Count.EqualTo(5), "All packets are read, over all files.");
    }

    /// <summary>
    /// Tests if only the newest packets are loaded when a maximum is specified.
    /// </summary>
    /// <returns>The async task.</returns>
    [Test]
    public async Task OnlyTheNewestPacketsAreLoadedAsync()
    {
        var archive = this.CreateArchive();
        var writer = await archive.StartSessionAsync(CreateMetadata()).ConfigureAwait(false);
        for (byte i = 0; i < 10; i++)
        {
            writer!.PacketCaptured([0xC1, 0x04, 0xF1, i], false);
        }

        await writer!.DisposeAsync().ConfigureAwait(false);

        var sessions = await archive.GetSessionsAsync().ConfigureAwait(false);
        var session = await ArchivedSession.LoadAsync(sessions[0], 3).ConfigureAwait(false);
        Assert.That(session.PacketList, Has.Count.EqualTo(3));
        Assert.That(session.PacketList[2].PacketData, Is.EqualTo("C1 04 F1 09"), "The newest packet is the last one.");
    }

    /// <summary>
    /// Tests if the sessions of one account can be requested, so that the page doesn't have to
    /// read the whole archive.
    /// </summary>
    /// <returns>The async task.</returns>
    [Test]
    public async Task SessionsCanBeFilteredByAccountAsync()
    {
        var archive = this.CreateArchive();
        await this.CreateFinishedSessionAsync(archive, "FirstAccount").ConfigureAwait(false);
        await this.CreateFinishedSessionAsync(archive, "SecondAccount").ConfigureAwait(false);

        var sessions = await archive.GetSessionsAsync("SecondAccount").ConfigureAwait(false);

        Assert.That(sessions, Has.Count.EqualTo(1));
        Assert.That(sessions[0].Metadata.AccountName, Is.EqualTo("SecondAccount"));
    }

    /// <summary>
    /// Tests if a session can be deleted again.
    /// </summary>
    /// <returns>The async task.</returns>
    [Test]
    public async Task SessionCanBeDeletedAsync()
    {
        var archive = this.CreateArchive();
        var session = await this.CreateFinishedSessionAsync(archive, "TestAccount").ConfigureAwait(false);

        Assert.That(await archive.DeleteSessionAsync(session.Id).ConfigureAwait(false), Is.True);
        Assert.That(await archive.GetSessionsAsync().ConfigureAwait(false), Is.Empty);
        Assert.That(Directory.Exists(session.DirectoryPath), Is.False);
    }

    /// <summary>
    /// Tests if a running session is not deleted, because its file is still written.
    /// </summary>
    /// <returns>The async task.</returns>
    [Test]
    public async Task RunningSessionIsNotDeletedAsync()
    {
        var archive = this.CreateArchive();
        var writer = await archive.StartSessionAsync(CreateMetadata()).ConfigureAwait(false);
        var sessions = await archive.GetSessionsAsync().ConfigureAwait(false);

        Assert.That(await archive.DeleteSessionAsync(sessions[0].Id).ConfigureAwait(false), Is.False);
        Assert.That(await archive.GetSessionsAsync().ConfigureAwait(false), Has.Count.EqualTo(1));

        await writer!.DisposeAsync().ConfigureAwait(false);
    }

    /// <summary>
    /// Tests if an identifier which points outside of the archive is refused - it comes from
    /// the outside, over the route of a page.
    /// </summary>
    /// <returns>The async task.</returns>
    [Test]
    public async Task SessionOutsideOfTheArchiveIsNotFoundAsync()
    {
        var archive = this.CreateArchive();
        await this.CreateFinishedSessionAsync(archive, "TestAccount").ConfigureAwait(false);

        Assert.That(await archive.GetSessionAsync("../../etc").ConfigureAwait(false), Is.Null);
        Assert.That(await archive.DeleteSessionAsync("../..").ConfigureAwait(false), Is.False);
    }

    /// <summary>
    /// Tests if the sessions which are older than the retention are removed.
    /// </summary>
    /// <returns>The async task.</returns>
    [Test]
    public async Task OldSessionsAreRemovedAsync()
    {
        var archive = this.CreateArchive(options => options.RetentionDays = 30);
        var oldId = await this.WriteSessionOnDiskAsync("OldAccount", DateTime.UtcNow.AddDays(-31)).ConfigureAwait(false);
        var recentId = await this.WriteSessionOnDiskAsync("RecentAccount", DateTime.UtcNow.AddDays(-1)).ConfigureAwait(false);

        await archive.ApplyHousekeepingAsync().ConfigureAwait(false);

        var sessions = await archive.GetSessionsAsync().ConfigureAwait(false);
        Assert.That(sessions.Select(session => session.Id), Is.EqualTo(new[] { recentId }));
        Assert.That(Directory.Exists(Path.Combine(this._archivePath, oldId)), Is.False);
    }

    /// <summary>
    /// Tests if the oldest sessions are removed when the archive gets too big.
    /// </summary>
    /// <returns>The async task.</returns>
    [Test]
    public async Task OldestSessionsAreRemovedWhenTheArchiveIsFullAsync()
    {
        var archive = this.CreateArchive(options =>
        {
            options.RetentionDays = 0;
            options.MaximumTotalSizeMb = 1;
        });

        // Together they are bigger than the maximum, so the older one has to go.
        var oldestId = await this.WriteSessionOnDiskAsync("FirstAccount", DateTime.UtcNow.AddHours(-2), 600 * 1024).ConfigureAwait(false);
        var newestId = await this.WriteSessionOnDiskAsync("SecondAccount", DateTime.UtcNow.AddHours(-1), 600 * 1024).ConfigureAwait(false);

        await archive.ApplyHousekeepingAsync().ConfigureAwait(false);

        var sessions = await archive.GetSessionsAsync().ConfigureAwait(false);
        Assert.That(sessions.Select(session => session.Id), Is.EqualTo(new[] { newestId }), "The oldest session should have been removed.");
        Assert.That(Directory.Exists(Path.Combine(this._archivePath, oldestId)), Is.False);
    }

    /// <summary>
    /// Tests if the housekeeping is applied when a session is finished, so that the archive
    /// doesn't grow between the logins of the observed players.
    /// </summary>
    /// <returns>The async task.</returns>
    [Test]
    public async Task HousekeepingIsAppliedWhenASessionEndsAsync()
    {
        var archive = this.CreateArchive(options => options.RetentionDays = 30);
        var oldId = await this.WriteSessionOnDiskAsync("OldAccount", DateTime.UtcNow.AddDays(-31)).ConfigureAwait(false);

        await this.CreateFinishedSessionAsync(archive, "TestAccount").ConfigureAwait(false);

        Assert.That(Directory.Exists(Path.Combine(this._archivePath, oldId)), Is.False);
    }

    /// <summary>
    /// Tests if an account name which can't be a directory name is still archived.
    /// </summary>
    /// <returns>The async task.</returns>
    [Test]
    public async Task AccountNameIsSanitizedAsync()
    {
        var archive = this.CreateArchive();
        var metadata = CreateMetadata();
        metadata.AccountName = "../etc/pass:wd";

        var writer = await archive.StartSessionAsync(metadata).ConfigureAwait(false);
        await writer!.DisposeAsync().ConfigureAwait(false);

        var sessions = await archive.GetSessionsAsync().ConfigureAwait(false);
        Assert.That(sessions, Has.Count.EqualTo(1));
        Assert.That(sessions[0].DirectoryPath, Does.StartWith(this._archivePath));
        Assert.That(sessions[0].Metadata.AccountName, Is.EqualTo("../etc/pass:wd"), "The real name is kept in the metadata.");
    }

    /// <summary>
    /// Tests if a file which contains an incomplete line - which happens when the process died
    /// while writing - is still readable.
    /// </summary>
    /// <returns>The async task.</returns>
    [Test]
    public async Task IncompleteLineIsSkippedAsync()
    {
        var archive = this.CreateArchive();
        var session = await this.CreateFinishedSessionAsync(archive, "TestAccount").ConfigureAwait(false);
        var partPath = Path.Combine(session.DirectoryPath, session.Metadata.Parts[0]);
        await File.AppendAllTextAsync(partPath, "1234567;True;6;C1 06 F1 01 01").ConfigureAwait(false);

        var loaded = await ArchivedSession.LoadAsync(session, 0).ConfigureAwait(false);

        Assert.That(loaded.PacketList, Has.Count.EqualTo(1), "Only the complete packet should be loaded.");
    }

    /// <summary>
    /// Tests if the metadata is written in a format which can be read again, so that the
    /// sessions of a previous run of the server are still described.
    /// </summary>
    /// <returns>The async task.</returns>
    [Test]
    public async Task MetadataIsReadableAsJsonAsync()
    {
        var archive = this.CreateArchive();
        var session = await this.CreateFinishedSessionAsync(archive, "TestAccount").ConfigureAwait(false);

        var json = await File.ReadAllTextAsync(Path.Combine(session.DirectoryPath, ArchivedSessionWriter.MetadataFileName)).ConfigureAwait(false);
        var metadata = JsonSerializer.Deserialize<ArchivedSessionMetadata>(json);

        Assert.That(metadata, Is.Not.Null);
        Assert.That(metadata!.AccountName, Is.EqualTo("TestAccount"));
        Assert.That(metadata.ServerType, Is.EqualTo(ServerType.GameServer));
        Assert.That(metadata.Parts, Has.Count.EqualTo(1));
    }

    private static ArchivedSessionMetadata CreateMetadata(string accountName = "TestAccount", DateTime? startTimestamp = null)
    {
        return new ArchivedSessionMetadata
        {
            AccountName = accountName,
            ServerType = ServerType.GameServer,
            ServerId = 1,
            ServerDescription = "Test Server",
            RemoteEndPoint = "127.0.0.1:1234",
            ClientVersion = new ClientVersion(6, 3, ClientLanguage.English),
            StartTimestamp = startTimestamp ?? DateTime.UtcNow,
        };
    }

    private PacketArchive CreateArchive(Action<NetworkObservationOptions>? configure = null)
    {
        var options = new NetworkObservationOptions { ArchivePath = this._archivePath };
        configure?.Invoke(options);
        return new PacketArchive(options, new NullLogger<PacketArchive>());
    }

    private async ValueTask<ArchivedSessionInfo> CreateFinishedSessionAsync(
        PacketArchive archive,
        string accountName,
        DateTime? startTimestamp = null,
        int packetCount = 1)
    {
        var metadata = CreateMetadata(accountName, startTimestamp);
        var writer = await archive.StartSessionAsync(metadata).ConfigureAwait(false);
        for (int i = 0; i < packetCount; i++)
        {
            writer!.PacketCaptured(LoginPacket, false);
        }

        await writer!.DisposeAsync().ConfigureAwait(false);

        var sessions = await archive.GetSessionsAsync(accountName).ConfigureAwait(false);
        return sessions.First(session => session.Metadata.StartTimestamp == metadata.StartTimestamp);
    }

    private async ValueTask<string> WriteSessionOnDiskAsync(string accountName, DateTime startTimestamp, int approximateSize = 0)
    {
        var sessionDirectory = $"{startTimestamp:yyyy-MM-dd_HH-mm-ss}_1";
        var sessionId = $"{accountName}/{sessionDirectory}";
        var directoryPath = Path.Combine(this._archivePath, accountName, sessionDirectory);
        Directory.CreateDirectory(directoryPath);

        var line = $"{TimeSpan.FromSeconds(1).Ticks};True;6;C1 06 F1 01 01 02;1";
        var content = new StringBuilder(startTimestamp.ToString("O")).AppendLine();
        do
        {
            content.AppendLine(line);
        }
        while (content.Length < approximateSize);

        await File.WriteAllTextAsync(Path.Combine(directoryPath, "part-000.mucap"), content.ToString()).ConfigureAwait(false);

        var metadata = CreateMetadata(accountName, startTimestamp);
        metadata.EndTimestamp = startTimestamp.AddMinutes(1);
        metadata.Parts.Add("part-000.mucap");
        await File.WriteAllTextAsync(
            Path.Combine(directoryPath, ArchivedSessionWriter.MetadataFileName),
            JsonSerializer.Serialize(metadata)).ConfigureAwait(false);
        return sessionId;
    }

    private async ValueTask<ArchivedSession> WaitForPacketsAsync(PacketArchive archive, int packetCount)
    {
        // The packets are written by another task, so the file needs a moment to catch up.
        var watch = Stopwatch.StartNew();
        ArchivedSession? session = null;
        while (watch.Elapsed < TimeSpan.FromSeconds(10))
        {
            var sessions = await archive.GetSessionsAsync().ConfigureAwait(false);
            session = await ArchivedSession.LoadAsync(sessions[0], 0).ConfigureAwait(false);
            if (session.PacketList.Count >= packetCount)
            {
                return session;
            }

            await Task.Delay(50).ConfigureAwait(false);
        }

        return session!;
    }
}
