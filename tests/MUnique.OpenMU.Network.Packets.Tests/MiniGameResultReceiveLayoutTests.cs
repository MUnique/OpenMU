// <copyright file="MiniGameResultReceiveLayoutTests.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.Network.Packets.Tests;

using System.Buffers.Binary;
using MUnique.OpenMU.Network.Packets.ServerToClient;

/// <summary>
/// Writes the mini game results, which share the code C1 93, through the generated packet structs and checks
/// that every value lands where the client reads it. The client reads all of them through one struct
/// (<c>PDEVILRANK</c>: rank at 3, count at 4, rows from 5) with rows of <c>MatchResult</c> (name 0..9, then
/// score, experience and money as aligned integers at 12, 16 and 20), and chooses the result window by the
/// count byte: 0xFF is the blood castle result, 0xFE the chaos castle result, a count below 200 the devil
/// square rank table.
/// </summary>
[TestFixture]
public class MiniGameResultReceiveLayoutTests
{
    private const int FirstRowOffset = 5;

    private const int RowSize = 24;

    /// <summary>
    /// The blood castle result: success at 3, 0xFF at 4, one row with score, experience and money.
    /// </summary>
    [Test]
    public void BloodCastleResult()
    {
        var data = new byte[BloodCastleScore.Length];
        _ = new BloodCastleScore(data)
        {
            Success = true,
            PlayerName = "Wwwwwwwwww",
            TotalScore = 1234,
            BonusExperience = 56789,
            BonusMoney = 100000,
        };

        Assert.Multiple(() =>
        {
            Assert.That(data, Has.Length.EqualTo(FirstRowOffset + RowSize));
            Assert.That(data[..5], Is.EqualTo(new byte[] { 0xC1, 29, 0x93, 1, 0xFF }));
            Assert.That(data[FirstRowOffset..(FirstRowOffset + 10)], Is.EqualTo("Wwwwwwwwww"u8.ToArray()));
            Assert.That(BinaryPrimitives.ReadUInt32LittleEndian(data.AsSpan(FirstRowOffset + 12)), Is.EqualTo(1234));
            Assert.That(BinaryPrimitives.ReadUInt32LittleEndian(data.AsSpan(FirstRowOffset + 16)), Is.EqualTo(56789));
            Assert.That(BinaryPrimitives.ReadUInt32LittleEndian(data.AsSpan(FirstRowOffset + 20)), Is.EqualTo(100000));
        });
    }

    /// <summary>
    /// The chaos castle result: success at 3, 0xFE at 4, one row with the killed monsters in the score,
    /// the experience, and the killed players in the money field.
    /// </summary>
    [Test]
    public void ChaosCastleResult()
    {
        var data = new byte[ChaosCastleScore.Length];
        _ = new ChaosCastleScore(data)
        {
            Success = true,
            PlayerName = "Wwwwwwwwww",
            MonsterKillCount = 12,
            BonusExperience = 56789,
            PlayerKillCount = 3,
        };

        Assert.Multiple(() =>
        {
            Assert.That(data, Has.Length.EqualTo(FirstRowOffset + RowSize));
            Assert.That(data[..5], Is.EqualTo(new byte[] { 0xC1, 29, 0x93, 1, 0xFE }));
            Assert.That(data[FirstRowOffset..(FirstRowOffset + 10)], Is.EqualTo("Wwwwwwwwww"u8.ToArray()));
            Assert.That(BinaryPrimitives.ReadUInt32LittleEndian(data.AsSpan(FirstRowOffset + 12)), Is.EqualTo(12));
            Assert.That(BinaryPrimitives.ReadUInt32LittleEndian(data.AsSpan(FirstRowOffset + 16)), Is.EqualTo(56789));
            Assert.That(BinaryPrimitives.ReadUInt32LittleEndian(data.AsSpan(FirstRowOffset + 20)), Is.EqualTo(3));
        });
    }

    /// <summary>
    /// The devil square rank table: the player's rank at 3, the row count at 4, rows of 24 bytes from 5.
    /// </summary>
    [Test]
    public void DevilSquareRankTable()
    {
        var data = new byte[MiniGameScoreTableRef.GetRequiredSize(2)];
        var packet = new MiniGameScoreTable(data) { PlayerRank = 2, ResultCount = 2 };
        var second = packet[1];
        second.PlayerName = "Wwwwwwwwww";
        second.TotalScore = 1234;
        second.BonusExperience = 56789;
        second.BonusMoney = 100000;

        var secondRow = FirstRowOffset + RowSize;
        Assert.Multiple(() =>
        {
            Assert.That(data, Has.Length.EqualTo(FirstRowOffset + (2 * RowSize)));
            Assert.That(data[..5], Is.EqualTo(new byte[] { 0xC1, (byte)data.Length, 0x93, 2, 2 }));
            Assert.That(data[secondRow..(secondRow + 10)], Is.EqualTo("Wwwwwwwwww"u8.ToArray()));
            Assert.That(BinaryPrimitives.ReadUInt32LittleEndian(data.AsSpan(secondRow + 12)), Is.EqualTo(1234));
            Assert.That(BinaryPrimitives.ReadUInt32LittleEndian(data.AsSpan(secondRow + 16)), Is.EqualTo(56789));
            Assert.That(BinaryPrimitives.ReadUInt32LittleEndian(data.AsSpan(secondRow + 20)), Is.EqualTo(100000));
        });
    }
}
