// <copyright file="NetworkAnalyzerPageTests.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.Web.Tests.NetworkAnalyzer;

using Bunit;
using Microsoft.Extensions.DependencyInjection;
using MUnique.OpenMU.Network.Analyzer;
using MUnique.OpenMU.Network.Analyzer.Archive;
using MUnique.OpenMU.Network.PlugIns;
using MUnique.OpenMU.Web.AdminPanel.Components.NetworkAnalyzer;
using MUnique.OpenMU.Web.AdminPanel.Pages;
using MUnique.OpenMU.Web.Shared.Components.Modal;
using MUnique.OpenMU.Web.Shared.Services;

/// <summary>
/// Tests for the <see cref="NetworkAnalyzer"/> page.
/// </summary>
[TestFixture]
public class NetworkAnalyzerPageTests
{
    private static readonly byte[] TestPacket = [0xC1, 0x04, 0xF1, 0x01];

    private string _archivePath = null!;

    /// <summary>
    /// Creates the path of the archive of the test.
    /// </summary>
    [SetUp]
    public void SetUp()
    {
        this._archivePath = ArchiveTestHelper.CreateArchivePath();
    }

    /// <summary>
    /// Removes the archive of the test.
    /// </summary>
    [TearDown]
    public void TearDown()
    {
        ArchiveTestHelper.DeleteArchive(this._archivePath);
    }

    /// <summary>
    /// Tests if a note is shown when the capture service is not registered, which is the case
    /// in the distributed deployment.
    /// </summary>
    [Test]
    public void ShowsNoteWhenCaptureServiceIsNotAvailable()
    {
        using var context = CreateContext();

        var component = context.Render<NetworkAnalyzer>();

        Assert.That(component.Markup, Does.Contain("only available in the all-in-one deployment"));
        Assert.That(component.FindAll(".list-group-item"), Is.Empty);
    }

    /// <summary>
    /// Tests if the connections of the service are listed.
    /// </summary>
    [Test]
    public void ConnectionsOfTheServiceAreListed()
    {
        var connection = new TestConnectionInfo { CharacterName = "TestCharacter" };
        using var context = CreateContext(new TestCaptureService(connection));

        var component = context.Render<NetworkAnalyzer>();

        Assert.That(component.Markup, Does.Contain("TestCharacter"));
        Assert.That(component.Markup, Does.Contain("Select a connection"));
    }

    /// <summary>
    /// Tests if the selection of a connection starts its capture.
    /// </summary>
    [Test]
    public void SelectingAConnectionStartsItsCapture()
    {
        var connection = new TestConnectionInfo { CharacterName = "TestCharacter" };
        using var context = CreateContext(new TestCaptureService(connection));

        var component = context.Render<NetworkAnalyzer>();
        component.Find(".list-group-item").Click();

        Assert.That(connection.Sinks, Has.Count.EqualTo(1), "The capture should be registered at the connection.");
        Assert.That(component.Markup, Does.Contain("No packets captured yet"));
    }

    /// <summary>
    /// Tests if a preselected connection of the route is captured immediately.
    /// </summary>
    [Test]
    public void PreselectedConnectionIsCaptured()
    {
        var connection = new TestConnectionInfo { CharacterName = "TestCharacter" };
        using var context = CreateContext(new TestCaptureService(connection));

        context.Render<NetworkAnalyzer>(parameters => parameters
            .Add(page => page.ConnectionId, connection.Id));

        Assert.That(connection.Sinks, Has.Count.EqualTo(1));
    }

    /// <summary>
    /// Tests if the connection of a player is captured when the page is opened with the deep
    /// link of another page, e.g. the list of the online accounts.
    /// </summary>
    [Test]
    public void ConnectionOfAPlayerIsCaptured()
    {
        var connection = new TestConnectionInfo(serverId: 3) { CharacterName = "TestCharacter" };
        using var context = CreateContext(new TestCaptureService(connection));

        var component = context.Render<NetworkAnalyzer>(parameters => parameters
            .Add(page => page.ServerId, 3)
            .Add(page => page.PlayerName, "TestCharacter"));

        Assert.That(connection.Sinks, Has.Count.EqualTo(1), "The capture should be registered at the connection.");
        Assert.That(component.Markup, Does.Contain("No packets captured yet"));
    }

    /// <summary>
    /// Tests if a note is shown when the player of a deep link isn't connected anymore. Such a
    /// link is rendered in another page, which may not be up to date anymore when it's used.
    /// </summary>
    [Test]
    public void NoteIsShownWhenThePlayerOfTheLinkIsGone()
    {
        var connection = new TestConnectionInfo(serverId: 3) { CharacterName = "TestCharacter" };
        using var context = CreateContext(new TestCaptureService(connection));

        var component = context.Render<NetworkAnalyzer>(parameters => parameters
            .Add(page => page.ServerId, 3)
            .Add(page => page.PlayerName, "AnotherCharacter"));

        Assert.That(component.Markup, Does.Contain("isn't available"));
        Assert.That(connection.Sinks, Is.Empty, "Nothing should be captured.");
    }

    /// <summary>
    /// Tests if the note about a missing connection disappears when another connection is
    /// selected.
    /// </summary>
    [Test]
    public void NoteAboutAMissingConnectionDisappearsOnSelection()
    {
        var connection = new TestConnectionInfo(serverId: 3) { CharacterName = "TestCharacter" };
        using var context = CreateContext(new TestCaptureService(connection));

        var component = context.Render<NetworkAnalyzer>(parameters => parameters
            .Add(page => page.ServerId, 3)
            .Add(page => page.PlayerName, "AnotherCharacter"));
        component.Find(".list-group-item").Click();

        Assert.That(component.Markup, Does.Not.Contain("isn't available"));
    }

    /// <summary>
    /// Tests if the packets are shown in chronological order, and that the grid scrolls to
    /// the newest one while the traffic is followed.
    /// </summary>
    [Test]
    public void PacketsAreShownInChronologicalOrder()
    {
        var connection = new TestConnectionInfo { CharacterName = "TestCharacter" };
        var service = new TestCaptureService(connection);
        using var context = CreateContext(service);

        var component = context.Render<NetworkAnalyzer>();
        component.Find(".list-group-item").Click();

        var capture = (LiveCapturedConnection)service.GetRunningCapture(connection.Id)!;
        capture.PacketCaptured(new byte[] { 0xC1, 0x04, 0xF1, 0x01 }, false);
        capture.PacketCaptured(new byte[] { 0xC1, 0x04, 0xF1, 0x02 }, false);

        // The view is updated by the refresh timer of the page. Following is active by
        // default, so it takes the new packets automatically.
        component.WaitForState(
            () => component.FindComponent<PacketGrid>().Instance.Packets.Count == 2,
            TimeSpan.FromSeconds(10));

        var grid = component.FindComponent<PacketGrid>().Instance;
        Assert.That(grid.Packets[0].PacketData, Is.EqualTo("C1 04 F1 01"), "The oldest packet comes first.");
        Assert.That(grid.Packets[1].PacketData, Is.EqualTo("C1 04 F1 02"), "The newest packet comes last.");
        Assert.That(grid.AutoScroll, Is.True, "The traffic is followed by default.");

        component.Find("button[title*='newest packet']").Click();

        Assert.That(component.FindComponent<PacketGrid>().Instance.AutoScroll, Is.False);
    }

    /// <summary>
    /// Tests if a view which doesn't follow the traffic anymore keeps its packets, and takes
    /// the missed ones when it follows again.
    /// </summary>
    /// <returns>The async task.</returns>
    [Test]
    public async Task ViewWhichDoesNotFollowDoesNotTakeNewPackets()
    {
        var connection = new TestConnectionInfo { CharacterName = "TestCharacter" };
        var service = new TestCaptureService(connection);
        using var context = CreateContext(service);

        var component = context.Render<NetworkAnalyzer>();
        component.Find(".list-group-item").Click();
        component.Find("button[title*='newest packet']").Click(); // stop following

        var capture = (LiveCapturedConnection)service.GetRunningCapture(connection.Id)!;
        var renderCount = component.RenderCount;
        capture.PacketCaptured(new byte[] { 0xC1, 0x04, 0xF1, 0x01 }, false);
        await Task.Delay(1000).ConfigureAwait(false);

        Assert.That(component.FindComponent<PacketGrid>().Instance.Packets, Is.Empty, "A view which doesn't follow should not take the new packet.");
        Assert.That(component.RenderCount, Is.EqualTo(renderCount), "Such a view should not be rendered again, it would just flicker.");

        component.Find("button[title*='newest packet']").Click(); // follow again
        component.WaitForState(
            () => component.FindComponent<PacketGrid>().Instance.Packets.Count == 1,
            TimeSpan.FromSeconds(10));
    }

    /// <summary>
    /// Tests if the view stops taking new packets when the user scrolls up in the grid, so
    /// that the packets they're looking at don't move away under their mouse pointer.
    /// </summary>
    /// <returns>The async task.</returns>
    [Test]
    public async Task ScrollingUpStopsTheViewFromTakingNewPackets()
    {
        var connection = new TestConnectionInfo { CharacterName = "TestCharacter" };
        var service = new TestCaptureService(connection);
        using var context = CreateContext(service);

        var component = context.Render<NetworkAnalyzer>();
        component.Find(".list-group-item").Click();

        // The javascript of the grid reports the scrolling of the user like this.
        var grid = component.FindComponent<PacketGrid>().Instance;
        await component.InvokeAsync(() => grid.SetAtBottomAsync(false)).ConfigureAwait(false);
        Assert.That(component.FindComponent<PacketGrid>().Instance.AutoScroll, Is.False, "Scrolling up should stop the following.");

        var capture = (LiveCapturedConnection)service.GetRunningCapture(connection.Id)!;
        capture.PacketCaptured(new byte[] { 0xC1, 0x04, 0xF1, 0x01 }, false);
        await Task.Delay(1000).ConfigureAwait(false);
        Assert.That(component.FindComponent<PacketGrid>().Instance.Packets, Is.Empty, "The new packet should not move the view of the user.");

        await component.InvokeAsync(() => grid.SetAtBottomAsync(true)).ConfigureAwait(false);
        component.WaitForState(
            () => component.FindComponent<PacketGrid>().Instance.Packets.Count == 1,
            TimeSpan.FromSeconds(10));
        Assert.That(component.FindComponent<PacketGrid>().Instance.AutoScroll, Is.True, "Scrolling back to the bottom should follow the traffic again.");
    }

    /// <summary>
    /// Tests if the view follows the traffic again after it has been cleared - there is
    /// nothing left to look at, so it would just stay empty otherwise.
    /// </summary>
    [Test]
    public void ClearingTheViewFollowsTheTrafficAgain()
    {
        var connection = new TestConnectionInfo { CharacterName = "TestCharacter" };
        var service = new TestCaptureService(connection);
        using var context = CreateContext(service);

        var component = context.Render<NetworkAnalyzer>();
        component.Find(".list-group-item").Click();
        component.Find("button[title*='newest packet']").Click(); // stop following
        Assert.That(component.FindComponent<PacketGrid>().Instance.AutoScroll, Is.False);

        component.FindAll("button").First(button => button.TextContent.Contains("Clear")).Click();

        Assert.That(component.FindComponent<PacketGrid>().Instance.AutoScroll, Is.True);
    }

    /// <summary>
    /// Tests if the page is not rendered again when nothing changed. Rendering the grid
    /// without a change makes it flicker, especially while the user scrolls through it.
    /// </summary>
    /// <returns>The async task.</returns>
    [Test]
    public async Task NothingIsRenderedWithoutAChange()
    {
        var connection = new TestConnectionInfo { CharacterName = "TestCharacter" };
        var service = new TestCaptureService(connection);
        using var context = CreateContext(service);

        var component = context.Render<NetworkAnalyzer>();
        component.Find(".list-group-item").Click();

        var renderCount = component.RenderCount;
        await Task.Delay(1000).ConfigureAwait(false);

        Assert.That(component.RenderCount, Is.EqualTo(renderCount), "Without new packets, there is nothing to render.");
    }

    /// <summary>
    /// Tests if the periodic refresh of the connections doesn't render the page when the
    /// connections didn't change. The servers create the information about their connections
    /// on each request, so they are different objects every time.
    /// </summary>
    /// <returns>The async task.</returns>
    /// <remarks>
    /// This test takes a couple of seconds, because it has to wait for the refresh of the
    /// connections.
    /// </remarks>
    [Test]
    public async Task UnchangedConnectionsDoNotRenderThePage()
    {
        var connection = new TestConnectionInfo { CharacterName = "TestCharacter" };
        var service = new TestCaptureService(connection);
        using var context = CreateContext(service);

        var component = context.Render<NetworkAnalyzer>();
        component.Find(".list-group-item").Click();

        var renderCount = component.RenderCount;
        await Task.Delay(TimeSpan.FromSeconds(6.5)).ConfigureAwait(false);

        Assert.That(service.RequestedConnectionsCount, Is.GreaterThan(0), "The connections should have been refreshed.");
        Assert.That(component.RenderCount, Is.EqualTo(renderCount), "The unchanged connections should not render the page.");
    }

    /// <summary>
    /// Tests if the archived sessions of the observed accounts are listed in the sidebar.
    /// </summary>
    /// <returns>The async task.</returns>
    [Test]
    public async Task ArchivedSessionsAreListedAsync()
    {
        var archive = ArchiveTestHelper.CreateArchive(this._archivePath);
        await ArchiveTestHelper.AddSessionAsync(archive, "ObservedAccount", TestPacket).ConfigureAwait(false);
        using var context = CreateContext(new TestCaptureService(), archive);

        var component = context.Render<NetworkAnalyzer>();

        // The archive is read from the file system, so the list arrives with a later render.
        component.WaitForState(() => component.Markup.Contains("ObservedAccount"), TimeSpan.FromSeconds(10));
        Assert.That(component.Markup, Does.Contain("Archived sessions"));
    }

    /// <summary>
    /// Tests if the packets of an archived session are shown when it's opened.
    /// </summary>
    /// <returns>The async task.</returns>
    [Test]
    public async Task ArchivedSessionShowsItsPacketsAsync()
    {
        var archive = ArchiveTestHelper.CreateArchive(this._archivePath);
        await ArchiveTestHelper.AddSessionAsync(archive, "ObservedAccount", TestPacket).ConfigureAwait(false);
        using var context = CreateContext(new TestCaptureService(), archive);

        var component = context.Render<NetworkAnalyzer>();
        component.WaitForElement(".archive-list .list-group-item", TimeSpan.FromSeconds(10)).Click();

        // The packets of the session are read from its file, which takes a moment.
        component.WaitForState(
            () => component.FindComponents<PacketGrid>().Count > 0 && component.FindComponent<PacketGrid>().Instance.Packets.Count > 0,
            TimeSpan.FromSeconds(10));
        var grid = component.FindComponent<PacketGrid>().Instance;
        Assert.That(grid.Packets, Has.Count.EqualTo(1));
        Assert.That(grid.Packets[0].PacketData, Is.EqualTo("C1 04 F1 01"));
        Assert.That(grid.ClientVersion, Is.EqualTo(new ClientVersion(6, 3, ClientLanguage.English)), "The version comes from the metadata of the session.");
    }

    /// <summary>
    /// Tests if an archived session is deleted after the user confirmed it.
    /// </summary>
    /// <returns>The async task.</returns>
    [Test]
    public async Task ArchivedSessionIsDeletedAfterConfirmationAsync()
    {
        var archive = ArchiveTestHelper.CreateArchive(this._archivePath);
        var session = await ArchiveTestHelper.AddSessionAsync(archive, "ObservedAccount", TestPacket).ConfigureAwait(false);
        var modalService = new TestModalService();
        using var context = CreateContext(new TestCaptureService(), archive, modalService);

        var component = context.Render<NetworkAnalyzer>();
        component.WaitForElement(".archive-list .oi-trash", TimeSpan.FromSeconds(10)).Click();

        // The deletion is confirmed by the user and applied on the file system, so it takes a
        // moment until the session is gone.
        component.WaitForState(() => modalService.ShownDialogs.Count == 1, TimeSpan.FromSeconds(10));
        component.WaitForState(() => component.Markup.Contains("No archived sessions"), TimeSpan.FromSeconds(10));
        Assert.That(await archive.GetSessionAsync(session.Id).ConfigureAwait(false), Is.Null);
    }

    /// <summary>
    /// Tests if the observation of the account of the selected connection can be toggled.
    /// </summary>
    [Test]
    public void ObservationOfAnAccountCanBeToggled()
    {
        var connection = new TestConnectionInfo { AccountName = "TestAccount", CharacterName = "TestCharacter" };
        var service = new TestCaptureService(connection);
        using var context = CreateContext(service);

        var component = context.Render<NetworkAnalyzer>();
        component.Find(".list-group-item").Click();
        component.Find("button[title*='Archives the traffic']").Click();

        Assert.That(service.ObservationChanges, Is.EqualTo(new[] { (connection.Id, true) }));
        Assert.That(connection.IsObserved, Is.True);
    }

    /// <summary>
    /// Tests if the archive isn't shown when it's not configured - it's registered by the
    /// game server, which isn't necessarily in the same process.
    /// </summary>
    [Test]
    public void ArchiveIsNotShownWhenItIsNotAvailable()
    {
        using var context = CreateContext(new TestCaptureService());

        var component = context.Render<NetworkAnalyzer>();

        Assert.That(component.Markup, Does.Not.Contain("Archived sessions"));
    }

    private static BunitContext CreateContext(
        IPacketCaptureService? captureService = null,
        IPacketArchive? archive = null,
        IModalService? modalService = null)
    {
        var context = new BunitContext();
        context.JSInterop.Mode = JSRuntimeMode.Loose;
        context.Services.AddSingleton<PacketAnalyzerProvider>();
        context.Services.AddSingleton<NavigationHistory>();
        context.Services.AddSingleton(modalService ?? new TestModalService());
        if (captureService is not null)
        {
            context.Services.AddSingleton(captureService);
        }

        if (archive is not null)
        {
            context.Services.AddSingleton(archive);
        }

        return context;
    }
}
