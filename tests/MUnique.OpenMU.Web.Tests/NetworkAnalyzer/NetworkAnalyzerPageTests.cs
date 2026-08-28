// <copyright file="NetworkAnalyzerPageTests.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.Web.Tests.NetworkAnalyzer;

using Bunit;
using Microsoft.Extensions.DependencyInjection;
using MUnique.OpenMU.Network.Analyzer;
using MUnique.OpenMU.Web.AdminPanel.Components.NetworkAnalyzer;
using MUnique.OpenMU.Web.AdminPanel.Pages;
using MUnique.OpenMU.Web.Shared.Services;

/// <summary>
/// Tests for the <see cref="NetworkAnalyzer"/> page.
/// </summary>
[TestFixture]
public class NetworkAnalyzerPageTests
{
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

    private static BunitContext CreateContext(IPacketCaptureService? captureService = null)
    {
        var context = new BunitContext();
        context.JSInterop.Mode = JSRuntimeMode.Loose;
        context.Services.AddSingleton<PacketAnalyzerProvider>();
        context.Services.AddSingleton<NavigationHistory>();
        if (captureService is not null)
        {
            context.Services.AddSingleton(captureService);
        }

        return context;
    }
}
