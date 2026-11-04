// <copyright file="PacketGridTests.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.Web.Tests.NetworkAnalyzer;

using Bunit;
using MUnique.OpenMU.Network.Analyzer;
using MUnique.OpenMU.Network.PlugIns;
using MUnique.OpenMU.Web.AdminPanel.Components.NetworkAnalyzer;

/// <summary>
/// Tests for the <see cref="PacketGrid"/>.
/// </summary>
[TestFixture]
public class PacketGridTests
{
    private static readonly ClientVersion Season6 = new(6, 3, ClientLanguage.English);

    private static readonly ClientVersion Version075 = new(0, 75, ClientLanguage.Invariant);

    /// <summary>
    /// Tests if a hint is shown when no packets have been captured yet.
    /// </summary>
    [Test]
    public void ShowsHintWhenThereAreNoPackets()
    {
        using var context = CreateContext();

        var component = context.Render<PacketGrid>(parameters => parameters
            .Add(grid => grid.Packets, []));

        Assert.That(component.Markup, Does.Contain("No packets captured yet"));
    }

    /// <summary>
    /// Tests if each packet is shown with its direction, size and code.
    /// </summary>
    [Test]
    public void EachPacketIsShownWithItsData()
    {
        using var context = CreateContext();
        IReadOnlyList<Packet> packets =
        [
            new(TimeSpan.FromSeconds(1), [0xC1, 0x05, 0x15, 0x01, 0x02], true),
            new(TimeSpan.FromSeconds(2), [0xC1, 0x08, 0x15, 0x00, 0x01, 0x02, 0x03, 0x04], false),
        ];

        var component = context.Render<PacketGrid>(parameters => parameters
            .Add(grid => grid.Packets, packets)
            .Add(grid => grid.ClientVersion, Season6));

        var rows = component.FindAll("tbody tr");
        Assert.That(rows, Has.Count.EqualTo(2));
        Assert.That(rows[0].TextContent, Does.Contain("C\u2192S"), "The first packet goes to the server.");
        Assert.That(rows[0].TextContent, Does.Contain("00:00:01"));
        Assert.That(rows[1].TextContent, Does.Contain("S\u2192C"), "The second packet goes to the client.");
        Assert.That(rows[1].TextContent, Does.Contain("00:00:02"));
    }

    /// <summary>
    /// Tests if the analyzer is used to show the message of a packet.
    /// </summary>
    [Test]
    public void MessageOfThePacketIsExtracted()
    {
        using var context = CreateContext();
        using var analyzer = new PacketAnalyzer();
        IReadOnlyList<Packet> packets = [new(TimeSpan.FromSeconds(1), [0xC1, 0x05, 0x15, 0x01, 0x02], true)];

        var component = context.Render<PacketGrid>(parameters => parameters
            .Add(grid => grid.Packets, packets)
            .Add(grid => grid.Analyzer, analyzer)
            .Add(grid => grid.ClientVersion, Season6));

        Assert.That(component.Markup, Does.Contain("InstantMoveRequest"));
    }

    /// <summary>
    /// Tests if the raw data is shown when no analyzer is available.
    /// </summary>
    [Test]
    public void RawDataIsShownWithoutAnalyzer()
    {
        using var context = CreateContext();
        IReadOnlyList<Packet> packets = [new(TimeSpan.FromSeconds(1), [0xC1, 0x05, 0x15, 0x01, 0x02], true)];

        var component = context.Render<PacketGrid>(parameters => parameters
            .Add(grid => grid.Packets, packets));

        Assert.That(component.Markup, Does.Contain("C1 05 15 01 02"));
    }

    /// <summary>
    /// Tests if the selection of a packet is reported.
    /// </summary>
    [Test]
    public void SelectionIsReported()
    {
        using var context = CreateContext();
        var packet = new Packet(TimeSpan.FromSeconds(1), [0xC1, 0x05, 0x15, 0x01, 0x02], true);
        Packet? selected = null;

        var component = context.Render<PacketGrid>(parameters => parameters
            .Add(grid => grid.Packets, [packet])
            .Add(grid => grid.OnSelect, p => selected = p));
        component.Find("tbody tr td:last-child span").Click();

        Assert.That(selected, Is.EqualTo(packet));
    }

    /// <summary>
    /// Tests if the message is kept on one line and stays readable as a tooltip, so that a
    /// long one doesn't change the height of the row.
    /// </summary>
    [Test]
    public void LongMessageIsTruncatedButAvailableAsTooltip()
    {
        using var context = CreateContext();
        using var analyzer = new PacketAnalyzer();
        IReadOnlyList<Packet> packets = [new(TimeSpan.FromSeconds(1), [0xC1, 0x05, 0x15, 0x0A, 0x14], true)];

        var component = context.Render<PacketGrid>(parameters => parameters
            .Add(grid => grid.Packets, packets)
            .Add(grid => grid.Analyzer, analyzer)
            .Add(grid => grid.ClientVersion, Season6));

        var message = component.Find("tbody tr .packet-message");
        Assert.That(message.GetAttribute("title"), Does.Contain("InstantMoveRequest"));
    }

    /// <summary>
    /// Tests if the details of the selected packet are shown inside its row.
    /// </summary>
    [Test]
    public void DetailsOfTheSelectedPacketAreShownInItsRow()
    {
        using var context = CreateContext();
        using var analyzer = new PacketAnalyzer();
        var first = new Packet(TimeSpan.FromSeconds(1), [0xC1, 0x05, 0x15, 0x0A, 0x14], true);
        var second = new Packet(TimeSpan.FromSeconds(2), [0xC1, 0x05, 0x15, 0x0B, 0x15], true);

        var component = context.Render<PacketGrid>(parameters => parameters
            .Add(grid => grid.Packets, [first, second])
            .Add(grid => grid.Analyzer, analyzer)
            .Add(grid => grid.ClientVersion, Season6)
            .Add(grid => grid.SelectedPacket, second));

        var rows = component.FindAll("tbody tr");
        Assert.That(rows[0].QuerySelectorAll(".packet-detail"), Is.Empty, "The row of the other packet has no details.");
        var details = rows[1].QuerySelector(".packet-detail");
        Assert.That(details, Is.Not.Null, "The details are inside the row of the selected packet.");
        Assert.That(details!.TextContent, Does.Contain("C1 05 15 0B 15"));
        Assert.That(details.TextContent, Does.Contain("InstantMoveRequest"));
    }

    /// <summary>
    /// Tests if the height of the grid is limited to the number of visible rows, so that the
    /// rest is reachable by scrolling.
    /// </summary>
    [Test]
    public void HeightIsLimitedToTheVisibleRows()
    {
        using var context = CreateContext();
        IReadOnlyList<Packet> packets = [new(TimeSpan.FromSeconds(1), [0xC1, 0x05, 0x15, 0x01, 0x02], true)];

        var component = context.Render<PacketGrid>(parameters => parameters
            .Add(grid => grid.Packets, packets)
            .Add(grid => grid.VisibleRowCount, 10));

        var container = component.Find(".packet-grid");
        Assert.That(container.GetAttribute("style"), Is.EqualTo("height: 320px"), "10 rows of 28 pixels plus the header.");
    }

    /// <summary>
    /// Tests if the grid scrolls to the newest packet when auto scrolling is active.
    /// </summary>
    [Test]
    public void ScrollsToTheNewestPacketWhenAutoScrolling()
    {
        using var context = CreateContext();
        var scrollInvocation = context.JSInterop
            .SetupModule("./_content/MUnique.OpenMU.Web.AdminPanel/Components/NetworkAnalyzer/PacketGrid.razor.js")
            .SetupVoid("scrollToBottom", _ => true);
        IReadOnlyList<Packet> packets = [new(TimeSpan.FromSeconds(1), [0xC1, 0x05, 0x15, 0x01, 0x02], true)];

        context.Render<PacketGrid>(parameters => parameters
            .Add(grid => grid.Packets, packets)
            .Add(grid => grid.AutoScroll, true));

        Assert.That(scrollInvocation.Invocations, Is.Not.Empty);
    }

    /// <summary>
    /// Tests if the grid doesn't scroll when auto scrolling is not active.
    /// </summary>
    [Test]
    public void DoesNotScrollWithoutAutoScrolling()
    {
        using var context = CreateContext();
        var scrollInvocation = context.JSInterop
            .SetupModule("./_content/MUnique.OpenMU.Web.AdminPanel/Components/NetworkAnalyzer/PacketGrid.razor.js")
            .SetupVoid("scrollToBottom", _ => true);
        IReadOnlyList<Packet> packets = [new(TimeSpan.FromSeconds(1), [0xC1, 0x05, 0x15, 0x01, 0x02], true)];

        context.Render<PacketGrid>(parameters => parameters
            .Add(grid => grid.Packets, packets)
            .Add(grid => grid.AutoScroll, false));

        Assert.That(scrollInvocation.Invocations, Is.Empty);
    }

    /// <summary>
    /// Tests if the grid scrolls to the newest packet when the auto scrolling is switched on
    /// again - the view may have been left somewhere in the middle of the packets.
    /// </summary>
    [Test]
    public void ScrollsToTheNewestPacketWhenAutoScrollingIsSwitchedOn()
    {
        using var context = CreateContext();
        var scrollInvocation = context.JSInterop
            .SetupModule("./_content/MUnique.OpenMU.Web.AdminPanel/Components/NetworkAnalyzer/PacketGrid.razor.js")
            .SetupVoid("scrollToBottom", _ => true);
        IReadOnlyList<Packet> packets = [new(TimeSpan.FromSeconds(1), [0xC1, 0x05, 0x15, 0x01, 0x02], true)];

        var component = context.Render<PacketGrid>(parameters => parameters
            .Add(grid => grid.Packets, packets)
            .Add(grid => grid.AutoScroll, false));
        Assert.That(scrollInvocation.Invocations, Is.Empty);

        component.Render(parameters => parameters
            .Add(grid => grid.Packets, packets)
            .Add(grid => grid.AutoScroll, true));

        Assert.That(scrollInvocation.Invocations, Is.Not.Empty);
    }

    /// <summary>
    /// Tests if the scrolling which is reported by the javascript is passed on, so that the
    /// page can stop feeding new packets into a view which the user scrolled up.
    /// </summary>
    /// <returns>The async task.</returns>
    [Test]
    public async Task ScrollingAwayFromTheBottomIsReported()
    {
        using var context = CreateContext();
        IReadOnlyList<Packet> packets = [new(TimeSpan.FromSeconds(1), [0xC1, 0x05, 0x15, 0x01, 0x02], true)];
        bool? isAtBottom = null;

        var component = context.Render<PacketGrid>(parameters => parameters
            .Add(grid => grid.Packets, packets)
            .Add(grid => grid.OnAtBottomChanged, value => isAtBottom = value));
        await component.InvokeAsync(() => component.Instance.SetAtBottomAsync(false)).ConfigureAwait(false);

        Assert.That(isAtBottom, Is.False);
    }

    /// <summary>
    /// Tests if only the newest packets are rendered when there are more than the maximum.
    /// </summary>
    [Test]
    public void OnlyTheNewestPacketsAreRendered()
    {
        using var context = CreateContext();
        var packets = Enumerable.Range(0, 20)
            .Select(index => new Packet(TimeSpan.FromSeconds(index), [0xC1, 0x04, 0xF1, (byte)index], true))
            .ToList();

        var component = context.Render<PacketGrid>(parameters => parameters
            .Add(grid => grid.Packets, packets)
            .Add(grid => grid.MaximumRowCount, 5));

        var rows = component.FindAll("tbody tr");
        Assert.That(rows, Has.Count.EqualTo(5));
        Assert.That(rows[0].TextContent, Does.Contain("C1 04 F1 0F"), "The 16th packet is the first one which is still shown.");
        Assert.That(rows[4].TextContent, Does.Contain("C1 04 F1 13"), "The newest packet is the last row.");
    }

    /// <summary>
    /// Tests if the extracted messages are renewed when the client version changes - the
    /// messages are cached per packet, because a row is rendered again with each arriving
    /// packet.
    /// </summary>
    [Test]
    public void MessagesAreRenewedForAnotherClientVersion()
    {
        using var context = CreateContext();
        using var analyzer = new PacketAnalyzer();
        IReadOnlyList<Packet> packets = [new(TimeSpan.FromSeconds(1), [0xC2, 0x00, 0x06, 0x13, 0x01, 0x00], false)];

        var component = context.Render<PacketGrid>(parameters => parameters
            .Add(grid => grid.Packets, packets)
            .Add(grid => grid.Analyzer, analyzer)
            .Add(grid => grid.ClientVersion, Version075));
        Assert.That(component.Markup, Does.Contain("AddNpcsToScope075"));

        component.Render(parameters => parameters
            .Add(grid => grid.Packets, packets)
            .Add(grid => grid.Analyzer, analyzer)
            .Add(grid => grid.ClientVersion, Season6));

        Assert.That(component.Markup, Does.Contain("AddNpcsToScope").And.Not.Contain("AddNpcsToScope075"));
    }

    private static BunitContext CreateContext()
    {
        var context = new BunitContext();

        // The auto scrolling of the grid uses javascript, which is not available here.
        context.JSInterop.Mode = JSRuntimeMode.Loose;
        return context;
    }
}
