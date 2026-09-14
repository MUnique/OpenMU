// <copyright file="ArchiveListTests.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.Web.Tests.NetworkAnalyzer;

using Bunit;
using MUnique.OpenMU.Interfaces;
using MUnique.OpenMU.Network.Analyzer.Archive;
using MUnique.OpenMU.Web.AdminPanel.Components.NetworkAnalyzer;

/// <summary>
/// Tests for the <see cref="ArchiveList"/>.
/// </summary>
[TestFixture]
public class ArchiveListTests
{
    /// <summary>
    /// Tests if a hint is shown when nothing is archived yet.
    /// </summary>
    [Test]
    public void ShowsHintWhenThereAreNoSessions()
    {
        using var context = CreateContext();

        var component = context.Render<ArchiveList>(parameters => parameters
            .Add(list => list.Sessions, []));

        Assert.That(component.Markup, Does.Contain("No archived sessions"));
    }

    /// <summary>
    /// Tests if the sessions are grouped by their account.
    /// </summary>
    [Test]
    public void SessionsAreGroupedByAccount()
    {
        using var context = CreateContext();
        IReadOnlyList<ArchivedSessionInfo> sessions =
        [
            CreateSession("FirstAccount"),
            CreateSession("SecondAccount"),
            CreateSession("SecondAccount"),
        ];

        var component = context.Render<ArchiveList>(parameters => parameters
            .Add(list => list.Sessions, sessions));

        var groups = component.FindAll(".accordion-item");
        Assert.That(groups, Has.Count.EqualTo(2));
        Assert.That(groups[0].TextContent, Does.Contain("FirstAccount"));
        Assert.That(groups[1].TextContent, Does.Contain("SecondAccount"));
        Assert.That(component.FindAll(".list-group-item"), Has.Count.EqualTo(3));
    }

    /// <summary>
    /// Tests if the download link points to the session, with a name which is escaped for the
    /// url.
    /// </summary>
    [Test]
    public void DownloadLinkPointsToTheSession()
    {
        using var context = CreateContext();
        var session = CreateSession("Account With Space");

        var component = context.Render<ArchiveList>(parameters => parameters
            .Add(list => list.Sessions, [session])
            .Add(list => list.DownloadRoute, "api/network-archive/"));

        var link = component.Find(".list-group-item a");
        Assert.That(link.GetAttribute("href"), Is.EqualTo("api/network-archive/Account%20With%20Space/2026-08-29_21-00-00_1"));
    }

    /// <summary>
    /// Tests if the selection of a session is reported.
    /// </summary>
    [Test]
    public void SelectionIsReported()
    {
        using var context = CreateContext();
        var session = CreateSession("TestAccount");
        ArchivedSessionInfo? selected = null;

        var component = context.Render<ArchiveList>(parameters => parameters
            .Add(list => list.Sessions, [session])
            .Add(list => list.OnSelect, info => selected = info));
        component.Find(".list-group-item").Click();

        Assert.That(selected, Is.EqualTo(session));
    }

    /// <summary>
    /// Tests if the deletion of a session is reported, without selecting it.
    /// </summary>
    [Test]
    public void DeletionIsReportedWithoutSelectingTheSession()
    {
        using var context = CreateContext();
        var session = CreateSession("TestAccount");
        ArchivedSessionInfo? deleted = null;
        ArchivedSessionInfo? selected = null;

        var component = context.Render<ArchiveList>(parameters => parameters
            .Add(list => list.Sessions, [session])
            .Add(list => list.OnSelect, info => selected = info)
            .Add(list => list.OnDelete, info => deleted = info));
        component.Find(".oi-trash").Click();

        Assert.That(deleted, Is.EqualTo(session));
        Assert.That(selected, Is.Null, "Deleting a session should not open it.");
    }

    /// <summary>
    /// Tests if a running session is marked as such, so that an admin sees that the player is
    /// still online.
    /// </summary>
    [Test]
    public void RunningSessionIsMarked()
    {
        using var context = CreateContext();

        var component = context.Render<ArchiveList>(parameters => parameters
            .Add(list => list.Sessions, [CreateSession("TestAccount", isRunning: true)]));

        Assert.That(component.FindAll(".oi-media-record"), Has.Count.EqualTo(1));
    }

    private static BunitContext CreateContext()
    {
        var context = new BunitContext();
        context.JSInterop.Mode = JSRuntimeMode.Loose;
        return context;
    }

    private static ArchivedSessionInfo CreateSession(string accountName, bool isRunning = false)
    {
        var metadata = new ArchivedSessionMetadata
        {
            AccountName = accountName,
            ServerType = ServerType.GameServer,
            ServerId = 1,
            StartTimestamp = new DateTime(2026, 8, 29, 21, 0, 0, DateTimeKind.Utc),
            PacketCount = 42,
        };

        var directory = $"{metadata.StartTimestamp:yyyy-MM-dd_HH-mm-ss}_{metadata.ServerId}";
        return new ArchivedSessionInfo(
            $"{accountName}/{directory}",
            $"/tmp/{accountName}/{directory}",
            metadata,
            1234,
            isRunning);
    }
}
