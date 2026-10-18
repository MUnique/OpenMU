// <copyright file="ServerListTest.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.ConnectServer.Tests;

using MUnique.OpenMU.Network.Packets.ConnectServer;
using MUnique.OpenMU.Network.PlugIns;

/// <summary>
/// Round-trip tests for <see cref="ServerList"/>, which verify that the load percentage
/// of the cached packet is updated through the generated packet accessors - not through
/// hard-coded byte offsets - and that cache invalidation works.
/// </summary>
[TestFixture]
internal class ServerListTest
{
    /// <summary>
    /// Tests that a connection count change is reflected in the cached packet of the new format,
    /// by reading it back through the generated <see cref="ServerListResponse"/> accessors.
    /// </summary>
    [Test]
    public void CurrentConnectionsChangeUpdatesCachedPacketNewFormat()
    {
        var serverList = CreateServerList(season: 1);
        var first = AddServer(serverList, serverId: 10, maximumConnections: 100);
        AddServer(serverList, serverId: 20, maximumConnections: 100);
        var packet = serverList.Serialize();

        first.CurrentConnections = 50;

        Assert.That(serverList.Cache, Is.SameAs(packet));
        AssertLoadPercentage(serverList.Cache, 10, 50);
        AssertLoadPercentage(serverList.Cache, 20, 0);
    }

    /// <summary>
    /// Tests that a connection count change is reflected in the cached packet of the old format,
    /// by reading it back through the generated <see cref="ServerListResponseOld"/> accessors.
    /// </summary>
    [Test]
    public void CurrentConnectionsChangeUpdatesCachedPacketOldFormat()
    {
        var serverList = CreateServerList(season: 0);
        var first = AddServer(serverList, serverId: 10, maximumConnections: 100);
        AddServer(serverList, serverId: 20, maximumConnections: 100);
        var packet = serverList.Serialize();

        first.CurrentConnections = 50;

        Assert.That(serverList.Cache, Is.SameAs(packet));
        AssertLoadPercentageOld(serverList.Cache, 10, 50);
        AssertLoadPercentageOld(serverList.Cache, 20, 0);
    }

    /// <summary>
    /// Tests that a connection count change made before the first serialization
    /// still shows up in the next serialized packet, in the new format.
    /// </summary>
    [Test]
    public void ConnectionChangeBeforeFirstSerializeIsIncludedNewFormat()
    {
        var serverList = CreateServerList(season: 1);
        var first = AddServer(serverList, serverId: 10, maximumConnections: 100);

        first.CurrentConnections = 25;
        var packet = serverList.Serialize();

        Assert.That(packet, Is.SameAs(serverList.Cache));
        AssertLoadPercentage(packet, 10, 25);
    }

    /// <summary>
    /// Tests that a connection count change made before the first serialization
    /// still shows up in the next serialized packet, in the old format.
    /// </summary>
    [Test]
    public void ConnectionChangeBeforeFirstSerializeIsIncludedOldFormat()
    {
        var serverList = CreateServerList(season: 0);
        var first = AddServer(serverList, serverId: 10, maximumConnections: 100);

        first.CurrentConnections = 25;
        var packet = serverList.Serialize();

        Assert.That(packet, Is.SameAs(serverList.Cache));
        AssertLoadPercentageOld(packet, 10, 25);
    }

    /// <summary>
    /// Tests that <see cref="ServerList.Serialize"/> returns the identical cached array
    /// on a subsequent call, in the new format.
    /// </summary>
    [Test]
    public void SerializeReturnsIdenticalCachedInstanceNewFormat()
    {
        var serverList = CreateServerList(season: 1);
        AddServer(serverList, serverId: 10, maximumConnections: 100);
        var packet = serverList.Serialize();

        Assert.That(serverList.Serialize(), Is.SameAs(packet));
    }

    /// <summary>
    /// Tests that <see cref="ServerList.Serialize"/> returns the identical cached array
    /// on a subsequent call, in the old format.
    /// </summary>
    [Test]
    public void SerializeReturnsIdenticalCachedInstanceOldFormat()
    {
        var serverList = CreateServerList(season: 0);
        AddServer(serverList, serverId: 10, maximumConnections: 100);
        var packet = serverList.Serialize();

        Assert.That(serverList.Serialize(), Is.SameAs(packet));
    }

    /// <summary>
    /// Tests that adding a server invalidates the cache, so the next
    /// <see cref="ServerList.Serialize"/> builds a fresh array which contains it, in the new format.
    /// </summary>
    [Test]
    public void AddInvalidatesCacheNewFormat()
    {
        var serverList = CreateServerList(season: 1);
        AddServer(serverList, serverId: 10, maximumConnections: 100);
        var firstPacket = serverList.Serialize();

        AddServer(serverList, serverId: 20, maximumConnections: 100);
        var secondPacket = serverList.Serialize();

        Assert.That(secondPacket, Is.Not.SameAs(firstPacket));
        Assert.That(secondPacket.Length, Is.GreaterThan(firstPacket.Length));
        Assert.That(serverList.Cache, Is.SameAs(secondPacket));
    }

    /// <summary>
    /// Tests that removing a server invalidates the cache, so the next
    /// <see cref="ServerList.Serialize"/> builds a fresh array without it, in the new format.
    /// </summary>
    [Test]
    public void RemoveInvalidatesCacheNewFormat()
    {
        var serverList = CreateServerList(season: 1);
        var first = AddServer(serverList, serverId: 10, maximumConnections: 100);
        var second = AddServer(serverList, serverId: 20, maximumConnections: 100);
        serverList.Serialize();

        serverList.Remove(second);
        var packet = serverList.Serialize();

        AssertLoadPercentage(packet, 10, 0);
        Assert.That(
            () => ReadLoadPercentageNew(packet, 20),
            Throws.TypeOf<KeyNotFoundException>());
    }

    private static ServerList CreateServerList(byte season)
    {
        return new ServerList(new ClientVersion(season, season == 0 ? (byte)97 : (byte)6, ClientLanguage.Invariant));
    }

    private static ServerListItem AddServer(ServerList serverList, ushort serverId, int maximumConnections)
    {
        var item = new ServerListItem(serverList)
        {
            ServerId = serverId,
            MaximumConnections = maximumConnections,
        };
        serverList.Add(item);
        return item;
    }

    private static int GetServerIndexNew(byte[] packet, ushort serverId)
    {
        var response = new ServerListResponse(packet);
        for (int i = 0; i < response.ServerCount; i++)
        {
            if (response[i].ServerId == serverId)
            {
                return i;
            }
        }

        throw new KeyNotFoundException($"Server {serverId} not found in packet.");
    }

    private static int GetServerIndexOld(byte[] packet, byte serverId)
    {
        var response = new ServerListResponseOld(packet);
        for (int i = 0; i < response.ServerCount; i++)
        {
            if (response[i].ServerId == serverId)
            {
                return i;
            }
        }

        throw new KeyNotFoundException($"Server {serverId} not found in packet.");
    }

    private static byte ReadLoadPercentageNew(byte[] packet, ushort serverId)
    {
        var response = new ServerListResponse(packet);
        return response[GetServerIndexNew(packet, serverId)].LoadPercentage;
    }

    private static void AssertLoadPercentage(byte[]? packet, ushort serverId, byte expectedPercentage)
    {
        Assert.That(packet, Is.Not.Null);
        Assert.That(ReadLoadPercentageNew(packet!, serverId), Is.EqualTo(expectedPercentage));
    }

    private static void AssertLoadPercentageOld(byte[]? packet, byte serverId, byte expectedPercentage)
    {
        Assert.That(packet, Is.Not.Null);
        var response = new ServerListResponseOld(packet!);
        Assert.That(response[GetServerIndexOld(packet!, serverId)].LoadPercentage, Is.EqualTo(expectedPercentage));
    }
}
