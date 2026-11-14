// <copyright file="BattleSoccerScoreboardTests.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.Tests;

using MUnique.OpenMU.GameLogic;
using MUnique.OpenMU.GameLogic.GuildWar;
using MUnique.OpenMU.GameServer.RemoteView.Guild;
using MUnique.OpenMU.Network.Packets.ServerToClient;

/// <summary>
/// Tests of the Battle Soccer scoreboard: end by time, draw decided by the next goal,
/// cancellation and the packets F3 23 (score) and F3 22 (clock).
/// </summary>
[TestFixture]
public class BattleSoccerScoreboardTests
{
    /// <summary>
    /// Tests that the team with more goals wins when the time is over.
    /// </summary>
    [Test]
    public void TimeOverTeamWithMoreGoalsWins()
    {
        var score = CreateScore();
        var ended = 0;
        score.PropertyChanged += (_, e) => ended += e.PropertyName == nameof(GuildWarScore.HasEnded) ? 1 : 0;
        score.IncreaseSecondGuildScore(SoccerGameMap.PointsPerGoal);

        score.EndByTime();

        Assert.That(score.HasEnded, Is.True);
        Assert.That(score.Winners, Is.EqualTo(GuildWarTeam.Second));
        Assert.That(ended, Is.EqualTo(1));
    }

    /// <summary>
    /// Tests that on a draw at the end of the time the match continues and the next goal decides.
    /// </summary>
    [Test]
    public void DrawIsDecidedByTheNextGoal()
    {
        var score = CreateScore();
        score.IncreaseFirstGuildScore(SoccerGameMap.PointsPerGoal);
        score.IncreaseSecondGuildScore(SoccerGameMap.PointsPerGoal);

        score.EndByTime();
        Assert.That(score.HasEnded, Is.False);
        Assert.That(score.IsSuddenDeath, Is.True);

        score.EndByTime();
        Assert.That(score.HasEnded, Is.False);

        score.IncreaseFirstGuildScore(SoccerGameMap.PointsPerGoal);
        Assert.That(score.HasEnded, Is.True);
        Assert.That(score.Winners, Is.EqualTo(GuildWarTeam.First));
    }

    /// <summary>
    /// Tests that without the time being over, the match only ends at 100 points (5 goals), as before.
    /// </summary>
    [Test]
    public void FiveGoalsEndTheMatch()
    {
        var score = CreateScore();
        for (var i = 0; i < 4; i++)
        {
            score.IncreaseFirstGuildScore(SoccerGameMap.PointsPerGoal);
        }

        Assert.That(score.HasEnded, Is.False);
        score.IncreaseFirstGuildScore(SoccerGameMap.PointsPerGoal);
        Assert.That(score.HasEnded, Is.True);
        Assert.That(score.Winners, Is.EqualTo(GuildWarTeam.First));
    }

    /// <summary>
    /// Tests that a cancelled match ends without a winner and doesn't accept goals anymore.
    /// </summary>
    [Test]
    public void CancelledMatchHasNoWinner()
    {
        var score = CreateScore();
        score.IncreaseFirstGuildScore(SoccerGameMap.PointsPerGoal);

        score.Cancel();
        score.IncreaseFirstGuildScore(SoccerGameMap.PointsPerGoal);

        Assert.That(score.HasEnded, Is.True);
        Assert.That(score.Winners, Is.Null);
        Assert.That(score.FirstGuildScore, Is.EqualTo(SoccerGameMap.PointsPerGoal));
    }

    /// <summary>
    /// Tests that the score is sent in packet F3 23 with names and goals; hiding it sends 255 as goals of the red team.
    /// </summary>
    /// <returns>The task.</returns>
    [Test]
    public async Task ScorePacketIsSentAsync()
    {
        var (player, output) = CastleSiegeRemoteViewTestHelper.CreatePlayer();
        var plugIn = new GuildSoccerScoreUpdatePlugIn(player);

        await plugIn.UpdateSoccerScoreAsync("RED", 2, "BLUE", 3).ConfigureAwait(false);
        await plugIn.HideSoccerScoreAsync().ConfigureAwait(false);
        await player.Connection!.Output.FlushAsync().ConfigureAwait(false);

        var bytes = output.ToArray();
        Assert.That(bytes, Has.Length.EqualTo(44));
        Assert.That(bytes[..4], Is.EqualTo(new byte[] { 0xC1, 22, 0xF3, 0x23 }));
        Assert.That(System.Text.Encoding.ASCII.GetString(bytes, 4, 3), Is.EqualTo("RED"));
        Assert.That(bytes[12], Is.EqualTo(2));
        Assert.That(System.Text.Encoding.ASCII.GetString(bytes, 13, 4), Is.EqualTo("BLUE"));
        Assert.That(bytes[21], Is.EqualTo(3));
        Assert.That(bytes[22 + 12], Is.EqualTo(255));
    }

    /// <summary>
    /// Tests that the clock is sent in packet F3 22 with the remaining seconds (little endian).
    /// </summary>
    /// <returns>The task.</returns>
    [Test]
    public async Task ClockPacketIsSentAsync()
    {
        var (player, output) = CastleSiegeRemoteViewTestHelper.CreatePlayer();

        await new GuildSoccerTimeUpdatePlugIn(player).UpdateSoccerTimeAsync(600).ConfigureAwait(false);
        await player.Connection!.Output.FlushAsync().ConfigureAwait(false);

        var bytes = output.ToArray();
        Assert.That(bytes, Has.Length.EqualTo(GuildSoccerTimeUpdate.Length));
        Assert.That(bytes[..4], Is.EqualTo(new byte[] { 0xC1, (byte)GuildSoccerTimeUpdate.Length, 0xF3, 0x22 }));
        GuildSoccerTimeUpdate packet = new Memory<byte>(bytes);
        Assert.That(packet.Seconds, Is.EqualTo(600));
    }

    private static GuildWarScore CreateScore() => new()
    {
        FirstGuildName = "RED",
        SecondGuildName = "BLUE",
        MaximumScore = 100,
    };
}
