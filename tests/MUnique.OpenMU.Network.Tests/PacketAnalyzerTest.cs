// <copyright file="PacketAnalyzerTest.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.Network.Tests;

using MUnique.OpenMU.Network.Analyzer;
using MUnique.OpenMU.Network.PlugIns;

/// <summary>
/// Tests for the <see cref="PacketAnalyzer"/>, especially for the selection of the packet
/// definition by direction, definition set and client version.
/// </summary>
[TestFixture]
public class PacketAnalyzerTest
{
    private static readonly ClientVersion Season6 = new(6, 3, ClientLanguage.English);

    private static readonly ClientVersion Version075 = new(0, 75, ClientLanguage.Invariant);

    /// <summary>
    /// Tests if the same packet code is resolved to a different definition, depending on the
    /// direction of the packet.
    /// </summary>
    [Test]
    public void DefinitionIsSelectedByDirection()
    {
        using var analyzer = new PacketAnalyzer();

        var toServer = new Packet(TimeSpan.Zero, [0xC1, 0x05, 0x15, 0x01, 0x02], true);
        var toClient = new Packet(TimeSpan.Zero, [0xC1, 0x08, 0x15, 0x00, 0x01, 0x02, 0x03, 0x04], false);

        Assert.That(analyzer.ExtractShortInformation(toServer, Season6).Definition?.Name, Is.EqualTo("InstantMoveRequest"));
        Assert.That(analyzer.ExtractShortInformation(toClient, Season6).Definition?.Name, Is.EqualTo("ObjectMoved"));
    }

    /// <summary>
    /// Tests if the client version decides which of the definitions of a packet code applies.
    /// The version is passed per call, so one instance can serve connections of different
    /// client versions.
    /// </summary>
    [Test]
    public void DefinitionIsSelectedByClientVersion()
    {
        using var analyzer = new PacketAnalyzer();
        var packet = new Packet(TimeSpan.Zero, [0xC2, 0x00, 0x06, 0x13, 0x01, 0x00], false);

        Assert.That(analyzer.ExtractShortInformation(packet, Version075).Definition?.Name, Is.EqualTo("AddNpcsToScope075"));
        Assert.That(analyzer.ExtractShortInformation(packet, Season6).Definition?.Name, Is.EqualTo("AddNpcsToScope"));
    }

    /// <summary>
    /// Tests if the definitions of the connect server are used when the corresponding
    /// definition set is selected.
    /// </summary>
    [Test]
    public void ConnectServerDefinitionsAreSelectable()
    {
        using var analyzer = new PacketAnalyzer(PacketDefinitionSet.ConnectServer);
        var packet = new Packet(TimeSpan.Zero, [0xC1, 0x04, 0xF4, 0x06], true);

        Assert.That(analyzer.ExtractShortInformation(packet, Season6).Definition?.Name, Is.EqualTo("ServerListRequest"));
    }

    /// <summary>
    /// Tests if a packet of the game server definitions is not found in the connect server
    /// definition set.
    /// </summary>
    [Test]
    public void DefinitionsOfOtherSetsAreNotUsed()
    {
        using var analyzer = new PacketAnalyzer(PacketDefinitionSet.ConnectServer);
        var packet = new Packet(TimeSpan.Zero, [0xC1, 0x05, 0x15, 0x01, 0x02], true);

        Assert.That(analyzer.ExtractShortInformation(packet, Season6).Definition, Is.Null);
    }

    /// <summary>
    /// Tests if a bidirectional packet definition is found for both directions.
    /// </summary>
    [Test]
    public void BidirectionalDefinitionIsFoundInBothDirections()
    {
        using var analyzer = new PacketAnalyzer(PacketDefinitionSet.ChatServer);
        var toServer = new Packet(TimeSpan.Zero, [0xC1, 0x05, 0x04, 0x00, 0x01], true);
        var toClient = new Packet(TimeSpan.Zero, [0xC1, 0x05, 0x04, 0x00, 0x01], false);

        Assert.That(analyzer.ExtractShortInformation(toServer, Season6).Definition?.Name, Is.EqualTo("ChatMessage"));
        Assert.That(analyzer.ExtractShortInformation(toClient, Season6).Definition?.Name, Is.EqualTo("ChatMessage"));
    }

    /// <summary>
    /// Tests if the extracted information contains the field values of the packet.
    /// </summary>
    [Test]
    public void InformationContainsFieldValues()
    {
        using var analyzer = new PacketAnalyzer();
        var packet = new Packet(TimeSpan.Zero, [0xC1, 0x05, 0x15, 0x0A, 0x14], true);

        var information = analyzer.ExtractInformation(packet, Season6);

        Assert.That(information, Does.Contain("InstantMoveRequest"));
        Assert.That(information, Does.Contain("10"));
        Assert.That(information, Does.Contain("20"));
    }
}
