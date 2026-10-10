// <copyright file="DuelSeason3RemoteViewTests.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.Tests;

using MUnique.OpenMU.GameLogic;
using MUnique.OpenMU.GameLogic.Views.Duel;
using MUnique.OpenMU.GameServer.RemoteView.Duel;

/// <summary>
/// Tests for the packets of the duel view plugins of the clients before Season 4, which use
/// their own packet codes instead of the sub codes of the duel packet.
/// </summary>
[TestFixture]
public class DuelSeason3RemoteViewTests
{
    private const ushort OpponentId = 0x1234;

    /// <summary>
    /// Tests that the duel request is sent with the code 0xAC, so the requested player can answer it.
    /// </summary>
    [Test]
    public async Task DuelRequestAsync()
    {
        var (player, output) = CastleSiegeRemoteViewTestHelper.CreatePlayer();
        var (requester, _) = CastleSiegeRemoteViewTestHelper.CreatePlayer();
        requester.Id = OpponentId;
        var view = new ShowDuelRequestPlugInSeason3(player);

        await view.ShowDuelRequestAsync(requester).ConfigureAwait(false);

        Assert.That(output.ToArray(), Is.EqualTo(new byte[]
        {
            0xC1, 15, 0xAC, 0x12, 0x34, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0,
        }));
    }

    /// <summary>
    /// Tests that the result is sent with the code 0xAA and tells the client whether the duel
    /// started; these clients don't know the result codes of the later protocol.
    /// </summary>
    [Test]
    public async Task DuelStartResultAsync()
    {
        var (player, output) = CastleSiegeRemoteViewTestHelper.CreatePlayer();
        var (opponent, _) = CastleSiegeRemoteViewTestHelper.CreatePlayer();
        opponent.Id = OpponentId;
        var view = new ShowDuelRequestResultPlugInSeason3(player);

        await view.ShowDuelRequestResultAsync(DuelStartResult.Success, opponent).ConfigureAwait(false);
        await view.ShowDuelRequestResultAsync(DuelStartResult.Refused, opponent).ConfigureAwait(false);
        await view.ShowDuelRequestResultAsync(DuelStartResult.FailedByNoFreeRoom, opponent).ConfigureAwait(false);

        Assert.That(output.ToArray(), Is.EqualTo(new byte[]
        {
            0xC1, 16, 0xAA, 1, 0x12, 0x34, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0,
            0xC1, 16, 0xAA, 0, 0x12, 0x34, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0,
            0xC1, 16, 0xAA, 0, 0x12, 0x34, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0,
        }));
    }

    /// <summary>
    /// Tests that the end of the duel is sent with the code 0xAB, with the data of the player
    /// which receives it.
    /// </summary>
    [Test]
    public async Task DuelEndAsync()
    {
        var (player, output) = CastleSiegeRemoteViewTestHelper.CreatePlayer();
        var view = new DuelEndedPlugInSeason3(player);

        await view.DuelEndedAsync().ConfigureAwait(false);

        Assert.That(output.ToArray(), Is.EqualTo(new byte[]
        {
            0xC1, 15, 0xAB, 0x02, 0x00, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0,
        }));
    }

    /// <summary>
    /// Tests that a finished duel also just ends the duel at these clients, because they don't
    /// know a packet which names the winner.
    /// </summary>
    [Test]
    public async Task DuelFinishedAsync()
    {
        var (player, output) = CastleSiegeRemoteViewTestHelper.CreatePlayer();
        var (opponent, _) = CastleSiegeRemoteViewTestHelper.CreatePlayer();
        var view = new DuelFinishedPlugInSeason3(player);

        await view.DuelFinishedAsync(player, opponent).ConfigureAwait(false);

        Assert.That(output.ToArray(), Is.EqualTo(new byte[]
        {
            0xC1, 15, 0xAB, 0x02, 0x00, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0,
        }));
    }

    /// <summary>
    /// Tests that the score is sent with the code 0xAD, with the score of the receiving player first.
    /// </summary>
    [Test]
    public async Task DuelScoreAsync()
    {
        var (requester, requesterOutput) = CastleSiegeRemoteViewTestHelper.CreatePlayer();
        var (opponent, opponentOutput) = CastleSiegeRemoteViewTestHelper.CreatePlayer();
        requester.Id = 0x1111;
        opponent.Id = OpponentId;
        var duelRoom = new DuelRoom(null, requester, opponent)
        {
            ScoreRequester = 3,
            ScoreOpponent = 5,
        };

        await new ShowDuelScoreUpdatePlugInSeason3(requester).UpdateScoreAsync(duelRoom).ConfigureAwait(false);
        await new ShowDuelScoreUpdatePlugInSeason3(opponent).UpdateScoreAsync(duelRoom).ConfigureAwait(false);

        Assert.Multiple(() =>
        {
            Assert.That(requesterOutput.ToArray(), Is.EqualTo(new byte[] { 0xC1, 9, 0xAD, 0x02, 0x00, 0x12, 0x34, 3, 5 }));
            Assert.That(opponentOutput.ToArray(), Is.EqualTo(new byte[] { 0xC1, 9, 0xAD, 0x02, 0x00, 0x11, 0x11, 5, 3 }));
        });
    }
}
