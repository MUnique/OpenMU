// <copyright file="PacketDetailTests.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.Web.Tests.NetworkAnalyzer;

using Bunit;
using MUnique.OpenMU.Network.Analyzer;
using MUnique.OpenMU.Network.PlugIns;
using MUnique.OpenMU.Web.AdminPanel.Components.NetworkAnalyzer;

/// <summary>
/// Tests for the <see cref="PacketDetail"/>.
/// </summary>
[TestFixture]
public class PacketDetailTests
{
    private static readonly ClientVersion Season6 = new(6, 3, ClientLanguage.English);

    /// <summary>
    /// Tests if a hint is shown when no packet is selected.
    /// </summary>
    [Test]
    public void ShowsHintWhenNoPacketIsSelected()
    {
        using var context = new BunitContext();

        var component = context.Render<PacketDetail>();

        Assert.That(component.Markup, Does.Contain("Select a packet"));
    }

    /// <summary>
    /// Tests if the raw data and the extracted fields of the packet are shown.
    /// </summary>
    [Test]
    public void ShowsRawDataAndExtractedFields()
    {
        using var context = new BunitContext();
        using var analyzer = new PacketAnalyzer();
        var packet = new Packet(TimeSpan.FromSeconds(1), [0xC1, 0x05, 0x15, 0x0A, 0x14], true);

        var component = context.Render<PacketDetail>(parameters => parameters
            .Add(detail => detail.Packet, packet)
            .Add(detail => detail.Analyzer, analyzer)
            .Add(detail => detail.ClientVersion, Season6));

        Assert.That(component.Markup, Does.Contain("C1 05 15 0A 14"));
        Assert.That(component.Markup, Does.Contain("InstantMoveRequest"));
        Assert.That(component.Markup, Does.Contain("10"), "The extracted x coordinate.");
        Assert.That(component.Markup, Does.Contain("20"), "The extracted y coordinate.");
    }

    /// <summary>
    /// Tests if only the raw data is shown when there is no analyzer.
    /// </summary>
    [Test]
    public void ShowsRawDataWithoutAnalyzer()
    {
        using var context = new BunitContext();
        var packet = new Packet(TimeSpan.FromSeconds(1), [0xC1, 0x05, 0x15, 0x0A, 0x14], true);

        var component = context.Render<PacketDetail>(parameters => parameters
            .Add(detail => detail.Packet, packet));

        Assert.That(component.Markup, Does.Contain("C1 05 15 0A 14"));
    }
}
