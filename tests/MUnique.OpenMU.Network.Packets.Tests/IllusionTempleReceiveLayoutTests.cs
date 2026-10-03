// <copyright file="IllusionTempleReceiveLayoutTests.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.Network.Packets.Tests;

using System.Buffers.Binary;
using MUnique.OpenMU.Network.Packets.ServerToClient;

/// <summary>
/// Writes Illusion Temple packets through the generated packet structs and checks that every value lands
/// at the byte offset where MuMain's receive structs read it (<c>PMSG_CURSED_TAMPLE_STATE</c>,
/// <c>PMSG_CURSED_TAMPLE_PARTY_POS</c>, <c>PMSG_CURSED_TEMPLE_RESULT</c>,
/// <c>PMSG_CURSED_TEMPLE_USER_ADD_EXP</c>; not packed, so WORD and int fields are aligned).
/// </summary>
[TestFixture]
public class IllusionTempleReceiveLayoutTests
{
    /// <summary>
    /// The state: remaining seconds at 4, holy item player at 6, its position at 8/9, points at 10/11,
    /// team at 12, party count at 13, party entries of 6 bytes from 14 (map number as a byte at 2).
    /// </summary>
    [Test]
    public void State()
    {
        var data = new byte[IllusionTempleState.GetRequiredSize(2)];
        var packet = new IllusionTempleState(data)
        {
            RemainingSeconds = 300,
            PlayerIndex = 0x1234,
            PositionX = 140,
            PositionY = 60,
            Team1Points = 2,
            Team2Points = 1,
            MyTeam = 1,
            PartyCount = 2,
        };
        var second = packet[1];
        second.PlayerId = 0x0102;
        second.MapNumber = 45;
        second.PositionX = 150;
        second.PositionY = 70;

        Assert.Multiple(() =>
        {
            Assert.That(data.Length, Is.EqualTo(14 + (2 * 6)));
            Assert.That(BinaryPrimitives.ReadUInt16LittleEndian(data.AsSpan(4)), Is.EqualTo(300));
            Assert.That(BinaryPrimitives.ReadUInt16LittleEndian(data.AsSpan(6)), Is.EqualTo(0x1234));
            Assert.That(data[8..14], Is.EqualTo(new byte[] { 140, 60, 2, 1, 1, 2 }));
            Assert.That(BinaryPrimitives.ReadUInt16LittleEndian(data.AsSpan(14 + 6)), Is.EqualTo(0x0102));
            Assert.That(data[(14 + 6 + 2)..(14 + 6 + 5)], Is.EqualTo(new byte[] { 45, 150, 70 }));
        });
    }

    /// <summary>
    /// The result: player count at 6, player rows of 20 bytes from 7 (name 0..9, map 10, team 11, class 12,
    /// experience as an int at 16).
    /// </summary>
    [Test]
    public void Result()
    {
        var data = new byte[IllusionTempleResult.GetRequiredSize(2)];
        var packet = new IllusionTempleResult(data) { Team1Points = 5, Team2Points = 2, PlayerCount = 2 };
        var second = packet[1];
        second.Name = "Wwwwwwwwww";
        second.MapNumber = 45;
        second.Team = 1;
        second.Class = 7;
        second.AddedExperience = 123456;

        Assert.Multiple(() =>
        {
            Assert.That(data.Length, Is.EqualTo(7 + (2 * 20)));
            Assert.That(data[4..7], Is.EqualTo(new byte[] { 5, 2, 2 }));
            Assert.That(data[(7 + 20)..(7 + 20 + 10)], Is.EqualTo("Wwwwwwwwww"u8.ToArray()));
            Assert.That(data[(7 + 20 + 10)..(7 + 20 + 13)], Is.EqualTo(new byte[] { 45, 1, 7 }));
            Assert.That(BinaryPrimitives.ReadInt32LittleEndian(data.AsSpan(7 + 20 + 16)), Is.EqualTo(123456));
        });
    }
}
