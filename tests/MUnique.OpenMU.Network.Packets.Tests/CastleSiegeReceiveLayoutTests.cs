// <copyright file="CastleSiegeReceiveLayoutTests.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.Network.Packets.Tests;

using System.Buffers.Binary;
using MUnique.OpenMU.Network.Packets.ServerToClient;

/// <summary>
/// Writes Castle Siege responses through the generated packet structs and checks that every value lands
/// at the byte offset where MuMain's receive structs read it. Those structs are not packed, so an integer
/// following the one-byte result starts at offset 8, and list entries are padded to a multiple of 4 bytes.
/// </summary>
[TestFixture]
public class CastleSiegeReceiveLayoutTests
{
    /// <summary>
    /// <c>PMSG_ANS_NPCBUY</c>: result at 4, NPC number at 8, NPC index at 12, 16 bytes.
    /// </summary>
    [Test]
    public void DefenseBuyResponse()
    {
        var data = new byte[CastleSiegeDefenseBuyResponse.Length];
        _ = new CastleSiegeDefenseBuyResponse(data) { Result = 1, NpcNumber = 277, NpcIndex = 3 };

        Assert.Multiple(() =>
        {
            Assert.That(data.Length, Is.EqualTo(16));
            Assert.That(data[4], Is.EqualTo(1));
            Assert.That(ReadInt(data, 8), Is.EqualTo(277));
            Assert.That(ReadInt(data, 12), Is.EqualTo(3));
        });
    }

    /// <summary>
    /// <c>PMSG_ANS_NPCREPAIR</c>: NPC number, index, current and maximum health at 8, 12, 16, 20; 24 bytes.
    /// </summary>
    [Test]
    public void DefenseRepairResponse()
    {
        var data = new byte[CastleSiegeDefenseRepairResponse.Length];
        _ = new CastleSiegeDefenseRepairResponse(data) { Result = 1, NpcNumber = 283, NpcIndex = 2, CurrentHp = 1500, MaxHp = 3000 };

        Assert.Multiple(() =>
        {
            Assert.That(data.Length, Is.EqualTo(24));
            Assert.That(ReadInt(data, 8), Is.EqualTo(283));
            Assert.That(ReadInt(data, 12), Is.EqualTo(2));
            Assert.That(ReadInt(data, 16), Is.EqualTo(1500));
            Assert.That(ReadInt(data, 20), Is.EqualTo(3000));
        });
    }

    /// <summary>
    /// <c>PMSG_ANS_NPCUPGRADE</c>: NPC number, index, upgrade type and value at 8, 12, 16, 20; 24 bytes.
    /// </summary>
    [Test]
    public void DefenseUpgradeResponse()
    {
        var data = new byte[CastleSiegeDefenseUpgradeResponse.Length];
        _ = new CastleSiegeDefenseUpgradeResponse(data) { Result = 1, NpcNumber = 277, NpcIndex = 1, NpcUpgradeType = 2, NpcUpgradeValue = 3 };

        Assert.Multiple(() =>
        {
            Assert.That(data.Length, Is.EqualTo(24));
            Assert.That(ReadInt(data, 8), Is.EqualTo(277));
            Assert.That(ReadInt(data, 12), Is.EqualTo(1));
            Assert.That(ReadInt(data, 16), Is.EqualTo(2));
            Assert.That(ReadInt(data, 20), Is.EqualTo(3));
        });
    }

    /// <summary>
    /// <c>PRECEIVE_CROWN_STATE</c>: the accumulated access time is a DWORD at 8; 12 bytes.
    /// </summary>
    [Test]
    public void CrownAccessState()
    {
        var data = new byte[CastleSiegeCrownAccessState.Length];
        _ = new CastleSiegeCrownAccessState(data) { State = CastleSiegeCrownAccessStateType.Started, AccumulatedTimeMs = 5000 };

        Assert.Multiple(() =>
        {
            Assert.That(data.Length, Is.EqualTo(12));
            Assert.That(data[4], Is.EqualTo(0));
            Assert.That(ReadInt(data, 8), Is.EqualTo(5000));
        });
    }

    /// <summary>
    /// <c>PMSG_ANS_NPCDBLIST</c> + <c>PMSG_NPCDBLIST</c>: count at 8, entries of 28 bytes from 12.
    /// </summary>
    [Test]
    public void NpcList()
    {
        var data = new byte[CastleSiegeNpcList.GetRequiredSize(2)];
        var packet = new CastleSiegeNpcList(data) { Result = 1, NpcCount = 2 };
        var second = packet[1];
        second.NpcNumber = 283;
        second.NpcIndex = 4;
        second.CurrentHp = 700;
        second.PositionX = 90;
        second.PositionY = 110;
        second.IsAlive = true;

        Assert.Multiple(() =>
        {
            Assert.That(data.Length, Is.EqualTo(12 + (2 * 28)));
            Assert.That(ReadInt(data, 8), Is.EqualTo(2));
            Assert.That(ReadInt(data, 12 + 28), Is.EqualTo(283));
            Assert.That(ReadInt(data, 12 + 28 + 4), Is.EqualTo(4));
            Assert.That(ReadInt(data, 12 + 28 + 20), Is.EqualTo(700));
            Assert.That(data[12 + 28 + 24], Is.EqualTo(90));
            Assert.That(data[12 + 28 + 25], Is.EqualTo(110));
            Assert.That(data[12 + 28 + 26], Is.EqualTo(1));
        });
    }

    /// <summary>
    /// <c>PMSG_ANS_CSREGGUILDLIST</c> + <c>PMSG_CSREGGUILDLIST</c>: count at 8, entries of 14 bytes from 12.
    /// </summary>
    [Test]
    public void RegisteredGuildList()
    {
        var data = new byte[CastleSiegeRegisteredGuildList.GetRequiredSize(2)];
        var packet = new CastleSiegeRegisteredGuildList(data) { Result = 1, GuildCount = 2 };
        var second = packet[1];
        second.GuildName = "Beta";
        second.GuildMarkCount = 42;
        second.SequenceNumber = 2;

        Assert.Multiple(() =>
        {
            Assert.That(data.Length, Is.EqualTo(12 + (2 * 14)));
            Assert.That(ReadInt(data, 8), Is.EqualTo(2));
            Assert.That(data[12 + 14], Is.EqualTo((byte)'B'));
            Assert.That(BinaryPrimitives.ReadUInt32BigEndian(data.AsSpan(12 + 14 + 8)), Is.EqualTo(42));
            Assert.That(data[12 + 14 + 13], Is.EqualTo(2));
        });
    }

    /// <summary>
    /// <c>PMSG_ANS_CSATTKGUILDLIST</c> + <c>PMSG_CSATTKGUILDLIST</c>: count at 8, entries of 16 bytes from 12
    /// with the score at entry offset 12.
    /// </summary>
    [Test]
    public void GuildList()
    {
        var data = new byte[CastleSiegeGuildList.GetRequiredSize(2)];
        var packet = new CastleSiegeGuildList(data) { Result = 1, GuildCount = 2 };
        var second = packet[1];
        second.Side = CastleSiegeJoinSide.Attack1;
        second.IsInvolved = true;
        second.GuildName = "Alpha";
        second.Score = 177;

        Assert.Multiple(() =>
        {
            Assert.That(data.Length, Is.EqualTo(12 + (2 * 16)));
            Assert.That(ReadInt(data, 8), Is.EqualTo(2));
            Assert.That(data[12 + 16], Is.EqualTo((byte)CastleSiegeJoinSide.Attack1));
            Assert.That(data[12 + 16 + 1], Is.EqualTo(1));
            Assert.That(data[12 + 16 + 2], Is.EqualTo((byte)'A'));
            Assert.That(ReadInt(data, 12 + 16 + 12), Is.EqualTo(177));
        });
    }

    /// <summary>
    /// <c>PRECEIVE_CASTLE_HUNTZONE_INFO</c>: current, maximum and unit price at 8, 12, 16; 20 bytes.
    /// </summary>
    [Test]
    public void HuntingZoneGuardInfo()
    {
        var data = new byte[CastleSiegeHuntingZoneGuardInfo.Length];
        _ = new CastleSiegeHuntingZoneGuardInfo(data) { Result = 1, IsEnabled = true, CurrentPrice = 1000, MaxPrice = 100000, UnitPrice = 100 };

        Assert.Multiple(() =>
        {
            Assert.That(data.Length, Is.EqualTo(20));
            Assert.That(data[5], Is.EqualTo(1));
            Assert.That(ReadInt(data, 8), Is.EqualTo(1000));
            Assert.That(ReadInt(data, 12), Is.EqualTo(100000));
            Assert.That(ReadInt(data, 16), Is.EqualTo(100));
        });
    }

    private static int ReadInt(byte[] data, int offset) => BinaryPrimitives.ReadInt32LittleEndian(data.AsSpan(offset));
}
