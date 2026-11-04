// <copyright file="DoppelgangerRemoteViewTests.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.Tests;

using MUnique.OpenMU.DataModel.Configuration;
using MUnique.OpenMU.GameLogic;
using MUnique.OpenMU.GameLogic.MiniGames;
using MUnique.OpenMU.GameLogic.MiniGames.Doppelganger;
using MUnique.OpenMU.GameLogic.PlayerActions.MiniGames;
using MUnique.OpenMU.GameServer.MessageHandler.MiniGames;
using MUnique.OpenMU.GameServer.RemoteView.MiniGames;
using MUnique.OpenMU.GameServer.RemoteView.NPC;
using MUnique.OpenMU.Network.Packets.ServerToClient;
using DoppelgangerResult = MUnique.OpenMU.GameLogic.MiniGames.Doppelganger.DoppelgangerResult;
using DoppelgangerResultPacket = MUnique.OpenMU.Network.Packets.ServerToClient.DoppelgangerResult;

/// <summary>
/// Tests the packets which are sent to the client during the doppelganger event.
/// </summary>
[TestFixture]
public class DoppelgangerRemoteViewTests
{
    /// <summary>
    /// Tests the packets of the event view, including the clamping of the path positions
    /// and the aligned reward experience of the result.
    /// </summary>
    [Test]
    public async Task EventViewSerializesPacketsAsync()
    {
        var (player, output) = CastleSiegeRemoteViewTestHelper.CreatePlayer();
        var view = new DoppelgangerEventViewPlugIn(player);

        await view.ShowStateAsync(DoppelgangerState.Playing).ConfigureAwait(false);
        await view.ShowMonsterPositionAsync(30).ConfigureAwait(false);
        await view.ShowIceWalkerAsync(5).ConfigureAwait(false);
        await view.HideIceWalkerAsync().ConfigureAwait(false);
        await view.ShowMonsterGoalAsync(2, 3).ConfigureAwait(false);
        await view.ShowResultAsync(DoppelgangerResult.MonstersReachedMagicCircle).ConfigureAwait(false);
        await view.ShowPlayInfoAsync(TimeSpan.FromSeconds(125), []).ConfigureAwait(false);

        var data = output.ToArray().AsMemory();
        var playInfoLength = DoppelgangerPlayInfoRef.GetRequiredSize(0);
        Assert.That(
            data.Length,
            Is.EqualTo(
                DoppelgangerStateUpdate.Length
                + DoppelgangerMonsterPosition.Length
                + (2 * DoppelgangerIceWalkerState.Length)
                + DoppelgangerMonsterGoal.Length
                + DoppelgangerResultPacket.Length
                + playInfoLength));

        var offset = 0;
        var state = (DoppelgangerStateUpdate)Next(DoppelgangerStateUpdate.Length);
        Assert.That(state.State, Is.EqualTo(DoppelgangerStateUpdate.DoppelgangerState.Playing));

        var monsterPosition = (DoppelgangerMonsterPosition)Next(DoppelgangerMonsterPosition.Length);
        Assert.That(monsterPosition.Position, Is.EqualTo(IDoppelgangerEventViewPlugIn.MaximumPathPosition), "The position is clamped to the magic circle.");

        var iceWalkerShown = (DoppelgangerIceWalkerState)Next(DoppelgangerIceWalkerState.Length);
        Assert.That(iceWalkerShown.State, Is.EqualTo(DoppelgangerIceWalkerState.IceWalkerState.Appeared));
        Assert.That(iceWalkerShown.Position, Is.EqualTo(5));

        var iceWalkerHidden = (DoppelgangerIceWalkerState)Next(DoppelgangerIceWalkerState.Length);
        Assert.That(iceWalkerHidden.State, Is.EqualTo(DoppelgangerIceWalkerState.IceWalkerState.Disappeared));

        var goal = (DoppelgangerMonsterGoal)Next(DoppelgangerMonsterGoal.Length);
        Assert.That(goal.GoalCount, Is.EqualTo(2));
        Assert.That(goal.MaximumGoalCount, Is.EqualTo(3));

        var resultData = Next(DoppelgangerResultPacket.Length);
        var result = (DoppelgangerResultPacket)resultData;
        Assert.That(result.Result, Is.EqualTo(DoppelgangerResultPacket.ResultType.MonstersReachedMagicCircle));
        Assert.That(result.RewardExperience, Is.Zero);
        Assert.That(resultData.Span[0], Is.EqualTo(0xC1));
        Assert.That(resultData.Span[1], Is.EqualTo(12), "The client structure isn't packed, so the experience is aligned at offset 8.");

        var playInfo = (DoppelgangerPlayInfo)Next(playInfoLength);
        Assert.That(playInfo.RemainingSeconds, Is.EqualTo(125));
        Assert.That(playInfo.PlayerCount, Is.Zero);

        Memory<byte> Next(int length)
        {
            var packet = data.Slice(offset, length);
            offset += length;
            return packet;
        }
    }

    /// <summary>
    /// Tests that a failed entrance to the doppelganger event is shown with the event specific packet,
    /// instead of throwing an exception like for mini games which aren't supported by the view.
    /// </summary>
    [Test]
    public async Task EnterResultUsesDoppelgangerPacketAsync()
    {
        var (player, output) = CastleSiegeRemoteViewTestHelper.CreatePlayer();
        var view = new ShowMiniGameEnterResultViewPlugIn(player);

        await view.ShowResultAsync(MiniGameType.Doppelganger, EnterResult.PlayerKillerCantEnter).ConfigureAwait(false);

        var packet = (DoppelgangerEnterResult)output.ToArray().AsMemory();
        Assert.That(packet.Result, Is.EqualTo(DoppelgangerEnterResult.EnterResult.PlayerKiller));
    }

    /// <summary>
    /// Tests the countdowns of the event, which use the values the client expects on the doppelganger maps.
    /// </summary>
    [Test]
    public async Task CountdownsUseDoppelgangerValuesAsync()
    {
        var (player, output) = CastleSiegeRemoteViewTestHelper.CreatePlayer();
        var miniGameStateView = new UpdateMiniGameStateViewViewPlugIn(player);
        var eventView = new DoppelgangerEventViewPlugIn(player);

        await miniGameStateView.UpdateStateAsync(MiniGameType.Doppelganger, MiniGameState.Closed).ConfigureAwait(false);
        await eventView.ShowIceWalkerCountdownAsync().ConfigureAwait(false);
        await miniGameStateView.UpdateStateAsync(MiniGameType.Doppelganger, MiniGameState.Ended).ConfigureAwait(false);

        var data = output.ToArray().AsMemory();
        Assert.That(data.Length, Is.EqualTo(3 * UpdateMiniGameState.Length));
        var states = Enumerable.Range(0, 3)
            .Select(i => ((UpdateMiniGameState)data.Slice(i * UpdateMiniGameState.Length, UpdateMiniGameState.Length)).State)
            .ToList();
        Assert.That(states, Is.EqualTo(new[]
        {
            UpdateMiniGameState.MiniGameTypeState.DoppelgangerStarting,
            UpdateMiniGameState.MiniGameTypeState.DoppelgangerIceWalkerCountdown,
            UpdateMiniGameState.MiniGameTypeState.DoppelgangerEnding,
        }));
    }

    /// <summary>
    /// Tests that the entrance window of Lugard is opened with zero minutes until the entrance opens,
    /// so the client enables the enter button.
    /// </summary>
    [Test]
    public async Task LugardWindowAllowsEntranceAsync()
    {
        var (player, output) = CastleSiegeRemoteViewTestHelper.CreatePlayer();
        var view = new OpenNpcWindowPlugIn(player);

        await view.OpenNpcWindowAsync(NpcWindow.LugardDoppelgangerEntry).ConfigureAwait(false);

        var data = output.ToArray();
        Assert.That(data, Has.Length.EqualTo(NpcWindowResponse.Length));
        Assert.That(((NpcWindowResponse)data.AsMemory()).Window, Is.EqualTo(NpcWindowResponse.NpcWindow.LugardDoppelgangerEntry));
        Assert.That(data[4], Is.Zero, "The byte after the window contains the minutes until the entrance opens.");
    }

    /// <summary>
    /// Tests that the handler of the enter request belongs to the right sub code and ignores truncated packets.
    /// </summary>
    [Test]
    public async Task EnterHandlerIgnoresTruncatedPacketAsync()
    {
        var (player, output) = CastleSiegeRemoteViewTestHelper.CreatePlayer();
        var handler = new DoppelgangerEnterRequestHandlerPlugIn();

        Assert.That(handler.Key, Is.EqualTo(0x0E));

        await handler.HandlePacketAsync(player, new byte[] { 0xC1, 0x04, 0xBF, 0x0E }).ConfigureAwait(false);

        Assert.That(output.Length, Is.Zero);
        Assert.That(player.CurrentMiniGame, Is.Null);
    }
}
