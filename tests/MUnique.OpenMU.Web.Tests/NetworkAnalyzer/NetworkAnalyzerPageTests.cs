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
    /// Tests if the newest packets are shown on top while the traffic is followed, and in
    /// chronological order when it's not.
    /// </summary>
    [Test]
    public void FollowingShowsTheNewestPacketsOnTop()
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
        // default, so the newest packet comes first.
        component.WaitForState(
            () => component.FindComponent<PacketGrid>().Instance.Packets.Count == 2,
            TimeSpan.FromSeconds(10));

        var packets = component.FindComponent<PacketGrid>().Instance.Packets;
        Assert.That(packets[0].PacketData, Is.EqualTo("C1 04 F1 02"), "The newest packet comes first while following.");

        component.Find("button[title*='newest packets']").Click();

        var chronological = component.FindComponent<PacketGrid>().Instance.Packets;
        Assert.That(chronological[0].PacketData, Is.EqualTo("C1 04 F1 01"), "Without following, the packets are in chronological order.");
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
