// <copyright file="MiniGameScoreRemoteViewTests.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.Tests;

using MUnique.OpenMU.GameServer.RemoteView.MiniGames;
using MUnique.OpenMU.Network.Packets.ServerToClient;

/// <summary>
/// Tests the mini game results which are sent to the client on the shared code C1 93.
/// The client chooses the result window by the byte at index 4: 0xFF blood castle, 0xFE chaos castle,
/// and a count below 200 for the devil square rank table.
/// </summary>
[TestFixture]
public class MiniGameScoreRemoteViewTests
{
    /// <summary>
    /// Tests that the blood castle result is sent with the blood castle type and the experience and money in their fields.
    /// </summary>
    [Test]
    public async Task BloodCastleResultAsync()
    {
        var (player, output) = CastleSiegeRemoteViewTestHelper.CreatePlayer();
        var view = new BloodCastleScoreTableViewPlugin(player);

        await view.ShowScoreTableAsync(true, "Winner", 1234, 5000, 70000).ConfigureAwait(false);

        var data = output.ToArray();
        Assert.That(data, Has.Length.EqualTo(BloodCastleScore.Length));
        AssertRawHeader(data, 0xFF);
        var result = new BloodCastleScore(data);
        Assert.Multiple(() =>
        {
            Assert.That(result.Type, Is.EqualTo(0xFF));
            Assert.That(result.Success, Is.True);
            Assert.That(result.PlayerName, Is.EqualTo("Winner"));
            Assert.That(result.TotalScore, Is.EqualTo(1234));
            Assert.That(result.BonusExperience, Is.EqualTo(5000));
            Assert.That(result.BonusMoney, Is.EqualTo(70000));
        });
    }

    /// <summary>
    /// Tests that the chaos castle result is sent with the chaos castle type, so that the client shows it on the
    /// chaos castle map, with the kill counts where the client reads them.
    /// </summary>
    [Test]
    public async Task ChaosCastleResultAsync()
    {
        var (player, output) = CastleSiegeRemoteViewTestHelper.CreatePlayer();
        var view = new ChaosCastleScoreTableViewPlugin(player);

        await view.ShowScoreTableAsync(true, "Winner", 12, 3, 5000).ConfigureAwait(false);

        var data = output.ToArray();
        Assert.That(data, Has.Length.EqualTo(ChaosCastleScore.Length));
        AssertRawHeader(data, 0xFE);
        var result = new ChaosCastleScore(data);
        Assert.Multiple(() =>
        {
            Assert.That(result.Type, Is.EqualTo(0xFE));
            Assert.That(result.Success, Is.True);
            Assert.That(result.PlayerName, Is.EqualTo("Winner"));
            Assert.That(result.MonsterKillCount, Is.EqualTo(12));
            Assert.That(result.PlayerKillCount, Is.EqualTo(3));
            Assert.That(result.BonusExperience, Is.EqualTo(5000));
        });
    }

    /// <summary>
    /// Tests that the devil square rank table sends the count of the rows it contains, when there are
    /// more scores than the table can hold.
    /// </summary>
    [Test]
    public async Task DevilSquareRankTableIsLimitedAsync()
    {
        var (player, output) = CastleSiegeRemoteViewTestHelper.CreatePlayer();
        var view = new MiniGameScoreTableViewPlugin(player);
        var scores = Enumerable.Range(1, 12).Select(i => ($"Player{i}", 100 - i, i * 10, i * 100)).ToList();

        await view.ShowScoreTableAsync(12, scores).ConfigureAwait(false);

        var data = output.ToArray();
        Assert.That(data, Has.Length.EqualTo(MiniGameScoreTableRef.GetRequiredSize(10)));
        AssertRawHeader(data, 10);
        var table = new MiniGameScoreTable(data);
        Assert.Multiple(() =>
        {
            Assert.That(table.PlayerRank, Is.EqualTo(12));
            Assert.That(table.ResultCount, Is.EqualTo(10));
            Assert.That(table[9].PlayerName, Is.EqualTo("Player10"));
            Assert.That(table[9].TotalScore, Is.EqualTo(90));
            Assert.That(table[9].BonusExperience, Is.EqualTo(100));
            Assert.That(table[9].BonusMoney, Is.EqualTo(1000));
        });
    }

    /// <summary>
    /// Asserts the header and the discriminating byte as they were sent, because the constructors of the
    /// packet structs initialize the header and the type again.
    /// </summary>
    /// <param name="data">The sent data.</param>
    /// <param name="expectedDiscriminator">The expected byte at index 4, which the client uses to choose the result window.</param>
    private static void AssertRawHeader(byte[] data, byte expectedDiscriminator)
    {
        Assert.Multiple(() =>
        {
            Assert.That(data[0], Is.EqualTo(0xC1));
            Assert.That(data[1], Is.EqualTo(data.Length));
            Assert.That(data[2], Is.EqualTo(0x93));
            Assert.That(data[4], Is.EqualTo(expectedDiscriminator));
        });
    }
}
