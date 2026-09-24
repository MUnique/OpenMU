// <copyright file="RaklionRemoteViewTests.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.Tests;

using Moq;
using MUnique.OpenMU.GameLogic;
using MUnique.OpenMU.GameLogic.Raklion;
using MUnique.OpenMU.GameServer.RemoteView.World;
using MUnique.OpenMU.Network.Packets.ServerToClient;
using RaklionState = MUnique.OpenMU.GameLogic.Raklion.RaklionState;

/// <summary>
/// Tests the packets which are sent to the client during the raklion event.
/// </summary>
[TestFixture]
public class RaklionRemoteViewTests
{
    /// <summary>
    /// Tests the packets of the states of the event and of Selupan.
    /// </summary>
    [Test]
    public async Task StatePacketsAsync()
    {
        var (player, output) = CastleSiegeRemoteViewTestHelper.CreatePlayer();
        var view = new RaklionEventViewPlugIn(player);

        await view.ShowStateInfoAsync(RaklionState.StartBattle, SelupanState.Pattern3, true, TimeSpan.FromSeconds(125.7)).ConfigureAwait(false);
        await view.ShowCurrentStateAsync(RaklionState.CloseDoor).ConfigureAwait(false);
        await view.ShowStateChangeAsync(RaklionState.Notify4).ConfigureAwait(false);
        await view.ShowSelupanStateAsync(SelupanState.Pattern7).ConfigureAwait(false);
        await view.ShowBattleResultAsync(true).ConfigureAwait(false);

        var data = output.ToArray().AsMemory();
        Assert.That(
            data.Length,
            Is.EqualTo(RaklionStateInfo.Length + RaklionCurrentState.Length + (2 * RaklionStateChange.Length) + RaklionBattleResult.Length));

        var offset = 0;
        var stateInfoData = Next(RaklionStateInfo.Length);
        var stateInfo = (RaklionStateInfo)stateInfoData;
        Assert.That(stateInfoData.Span[..4].ToArray(), Is.EqualTo(new byte[] { 0xC1, 12, 0xD1, 0x10 }));
        Assert.That(stateInfo.State, Is.EqualTo(RaklionStateInfo.RaklionState.StartBattle));
        Assert.That(stateInfo.DetailState, Is.EqualTo((byte)SelupanState.Pattern3));
        Assert.That(stateInfo.CanEnter, Is.True);
        Assert.That(stateInfo.RemainingSeconds, Is.EqualTo(125));

        var currentState = (RaklionCurrentState)Next(RaklionCurrentState.Length);
        Assert.That(currentState.State, Is.EqualTo(RaklionCurrentState.RaklionState.CloseDoor));

        var stateChange = (RaklionStateChange)Next(RaklionStateChange.Length);
        Assert.That(stateChange.State, Is.EqualTo(RaklionStateChange.RaklionState.Notify4));

        var selupanState = (RaklionStateChange)Next(RaklionStateChange.Length);
        Assert.That(selupanState.State, Is.EqualTo(RaklionStateChange.RaklionState.DetailState));
        Assert.That(selupanState.DetailState, Is.EqualTo((byte)SelupanState.Pattern7));

        var result = (RaklionBattleResult)Next(RaklionBattleResult.Length);
        Assert.That(result.Result, Is.EqualTo(RaklionBattleResult.BattleResult.Success));

        Memory<byte> Next(int length)
        {
            var packet = data.Slice(offset, length);
            offset += length;
            return packet;
        }
    }

    /// <summary>
    /// Tests the packet of a skill of Selupan, which contains the ids at the aligned offsets.
    /// </summary>
    [Test]
    public async Task SelupanSkillPacketAsync()
    {
        var (player, output) = CastleSiegeRemoteViewTestHelper.CreatePlayer();
        var view = new RaklionEventViewPlugIn(player);
        var selupan = new Mock<IAttacker>();
        selupan.Setup(s => s.Id).Returns(0x1234);
        var target = new Mock<IAttackable>();
        target.Setup(t => t.Id).Returns(0x0567);

        await view.ShowSelupanSkillAsync(selupan.Object, target.Object, SelupanSkill.IceStorm).ConfigureAwait(false);
        await view.ShowSelupanSkillAsync(selupan.Object, null, SelupanSkill.Heal).ConfigureAwait(false);

        var data = output.ToArray().AsMemory();
        Assert.That(data.Length, Is.EqualTo(2 * MonsterSkillAnimation.Length));
        Assert.That(data.Span[..10].ToArray(), Is.EqualTo(new byte[] { 0xC1, 10, 0x69, 0x00, 35, 0, 0x34, 0x12, 0x67, 0x85 }));

        var heal = (MonsterSkillAnimation)data.Slice(MonsterSkillAnimation.Length, MonsterSkillAnimation.Length);
        Assert.That(heal.SkillNumber, Is.EqualTo((ushort)SelupanSkill.Heal));
        Assert.That(heal.AttackerId, Is.EqualTo(0x1234));
        Assert.That(heal.TargetId, Is.EqualTo(0x1234), "Without a target, Selupan is the target of its skill.");
    }
}
