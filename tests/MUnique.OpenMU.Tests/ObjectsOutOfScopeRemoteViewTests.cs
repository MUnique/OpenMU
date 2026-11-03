// <copyright file="ObjectsOutOfScopeRemoteViewTests.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.Tests;

using Moq;
using MUnique.OpenMU.GameLogic;
using MUnique.OpenMU.GameServer.RemoteView.World;
using MUnique.OpenMU.Network.Packets.ServerToClient;

/// <summary>
/// Tests for the <see cref="ObjectsOutOfScopePlugIn"/>.
/// </summary>
[TestFixture]
public class ObjectsOutOfScopeRemoteViewTests
{
    /// <summary>
    /// Tests that more objects than one packet can carry are sent in several packets, each with a length and
    /// a count which fit, and all the ids in their order.
    /// </summary>
    /// <param name="objectCount">The number of objects which leave the scope.</param>
    /// <param name="expectedPacketCount">The expected number of packets.</param>
    [TestCase(1, 1)]
    [TestCase(125, 1)]
    [TestCase(126, 2)]
    [TestCase(300, 3)]
    public async ValueTask ManyObjectsAreSplitIntoPacketsWhichFitAsync(int objectCount, int expectedPacketCount)
    {
        var (player, output) = CastleSiegeRemoteViewTestHelper.CreatePlayer();
        var objects = Enumerable.Range(1, objectCount).Select(id => CreateObject((ushort)id)).ToList();

        await new ObjectsOutOfScopePlugIn(player).ObjectsOutOfScopeAsync(objects).ConfigureAwait(false);

        var packets = SplitPackets(output.ToArray());
        var ids = new List<ushort>();
        foreach (var data in packets)
        {
            MapObjectOutOfScope packet = data.AsMemory();
            Assert.That(packet.Header.Length, Is.EqualTo(data.Length));
            Assert.That(data.Length, Is.EqualTo(MapObjectOutOfScope.GetRequiredSize(packet.ObjectCount)));
            for (var i = 0; i < packet.ObjectCount; i++)
            {
                ids.Add(packet[i].Id);
            }
        }

        Assert.That(packets, Has.Count.EqualTo(expectedPacketCount));
        Assert.That(ids, Is.EqualTo(Enumerable.Range(1, objectCount).Select(id => (ushort)id)));
    }

    private static IIdentifiable CreateObject(ushort id)
    {
        var identifiable = new Mock<IIdentifiable>();
        identifiable.Setup(o => o.Id).Returns(id);
        return identifiable.Object;
    }

    private static List<byte[]> SplitPackets(byte[] data)
    {
        var result = new List<byte[]>();
        var offset = 0;
        while (offset < data.Length)
        {
            var length = data[offset + 1];
            result.Add(data[offset..(offset + length)]);
            offset += length;
        }

        return result;
    }
}
