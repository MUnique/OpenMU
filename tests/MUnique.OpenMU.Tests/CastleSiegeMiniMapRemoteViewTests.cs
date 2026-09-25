// <copyright file="CastleSiegeMiniMapRemoteViewTests.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.Tests;

using MUnique.OpenMU.GameLogic.Views.CastleSiege;
using MUnique.OpenMU.GameServer.RemoteView.CastleSiege;
using MUnique.OpenMU.Network.Packets.ServerToClient;
using WireCommandType = MUnique.OpenMU.Network.Packets.ServerToClient.CastleSiegeGuildCommandType;

/// <summary>
/// Tests Castle Siege mini-map and guild-command remote-view packet serialization.
/// </summary>
[TestFixture]
public class CastleSiegeMiniMapRemoteViewTests
{
    /// <summary>
    /// Verifies the mini-map player- and NPC-position packets.
    /// </summary>
    [Test]
    public async ValueTask SerializeMiniMapPlayerAndNpcPositionsAsync()
    {
        var (player, output) = CastleSiegeRemoteViewTestHelper.CreatePlayer();

        await new CastleSiegeMiniMapPlugIn(player)
            .ShowPlayerPositionsAsync(
            [
                new CastleSiegeMiniMapPlayerInfo(10, 20),
                new CastleSiegeMiniMapPlayerInfo(30, 40),
            ])
            .ConfigureAwait(false);
        await new CastleSiegeMiniMapPlugIn(player)
            .ShowNpcPositionsAsync(
            [
                new CastleSiegeMiniMapNpcInfo(true, 50, 60),
                new CastleSiegeMiniMapNpcInfo(false, 70, 80),
            ])
            .ConfigureAwait(false);

        var data = output.ToArray().AsMemory();
        var playerPacketLength = CastleSiegeMiniMapPlayerPositions.GetRequiredSize(2);
        var npcPacketLength = CastleSiegeMiniMapNpcPositions.GetRequiredSize(2);
        Assert.That(data.Length, Is.EqualTo(playerPacketLength + npcPacketLength));

        var playerPositions = (CastleSiegeMiniMapPlayerPositions)data[..playerPacketLength];
        Assert.Multiple(() =>
        {
            Assert.That(playerPositions.PlayerCount, Is.EqualTo(2u));
            Assert.That(playerPositions[0].PositionX, Is.EqualTo(10));
            Assert.That(playerPositions[0].PositionY, Is.EqualTo(20));
            Assert.That(playerPositions[1].PositionX, Is.EqualTo(30));
            Assert.That(playerPositions[1].PositionY, Is.EqualTo(40));
        });

        var npcPositions = (CastleSiegeMiniMapNpcPositions)data.Slice(playerPacketLength, npcPacketLength);
        Assert.Multiple(() =>
        {
            Assert.That(npcPositions.NpcCount, Is.EqualTo((byte)2));
            Assert.That(npcPositions[0].NpcType, Is.EqualTo(CastleSiegeMiniMapNpcType.Gate));
            Assert.That(npcPositions[0].PositionX, Is.EqualTo(50));
            Assert.That(npcPositions[0].PositionY, Is.EqualTo(60));
            Assert.That(npcPositions[1].NpcType, Is.EqualTo(CastleSiegeMiniMapNpcType.GuardianStatue));
            Assert.That(npcPositions[1].PositionX, Is.EqualTo(70));
            Assert.That(npcPositions[1].PositionY, Is.EqualTo(80));
        });
    }

    /// <summary>
    /// Verifies that the mini-map player-position packet is still sent, with a zero count, when there are no
    /// players to report - the client clears its whole buffer on this packet, so skipping it on an empty list
    /// would leave stale entries on the client.
    /// </summary>
    [Test]
    public async ValueTask SerializeMiniMapPlayerPositionsSendsEmptyListAsync()
    {
        var (player, output) = CastleSiegeRemoteViewTestHelper.CreatePlayer();

        await new CastleSiegeMiniMapPlugIn(player)
            .ShowPlayerPositionsAsync([])
            .ConfigureAwait(false);

        var data = output.ToArray().AsMemory();
        Assert.That(data.Length, Is.EqualTo(CastleSiegeMiniMapPlayerPositions.GetRequiredSize(0)));
        var playerPositions = (CastleSiegeMiniMapPlayerPositions)data;
        Assert.That(playerPositions.PlayerCount, Is.EqualTo(0u));
    }

    /// <summary>
    /// Verifies that the guild-command packet relays the command-group slot ("Team") unchanged, rather than
    /// re-deriving it from the issuer's side - regression test for the wire semantics sven-n flagged in review.
    /// </summary>
    [Test]
    public async ValueTask SerializeGuildCommandRelaysTeamUnchangedAsync()
    {
        var (player, output) = CastleSiegeRemoteViewTestHelper.CreatePlayer();

        await new CastleSiegeCommandPlugIn(player)
            .ShowGuildCommandAsync(team: 5, positionX: 12, positionY: 34, CastleSiegeCommandType.Attack)
            .ConfigureAwait(false);

        var packet = (CastleSiegeGuildCommand)output.ToArray().AsMemory();
        Assert.Multiple(() =>
        {
            Assert.That(packet.Team, Is.EqualTo(5));
            Assert.That(packet.PositionX, Is.EqualTo(12));
            Assert.That(packet.PositionY, Is.EqualTo(34));
            Assert.That(packet.Command, Is.EqualTo(WireCommandType.Attack));
        });
    }

    /// <summary>
    /// Verifies the command-type mapping for the wire enum, including the fallback for the "wait" value.
    /// </summary>
    [Test]
    public async ValueTask SerializeGuildCommandMapsCommandTypesAsync()
    {
        var (player, output) = CastleSiegeRemoteViewTestHelper.CreatePlayer();

        await new CastleSiegeCommandPlugIn(player)
            .ShowGuildCommandAsync(team: 0, positionX: 0, positionY: 0, CastleSiegeCommandType.Defend)
            .ConfigureAwait(false);

        var packet = (CastleSiegeGuildCommand)output.ToArray().AsMemory();
        Assert.That(packet.Command, Is.EqualTo(WireCommandType.Defend));
    }
}
