// <copyright file="CastleSiegeNpcRemoteViewTests.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.Tests;

using MUnique.OpenMU.DataModel.Configuration;
using MUnique.OpenMU.GameLogic.Views.CastleSiege;
using MUnique.OpenMU.GameServer.RemoteView.CastleSiege;
using MUnique.OpenMU.Network.Packets.ServerToClient;

/// <summary>
/// Tests Castle Siege NPC remote-view packet serialization.
/// </summary>
[TestFixture]
public class CastleSiegeNpcRemoteViewTests
{
    /// <summary>
    /// Verifies NPC list and defense-structure operation response packets.
    /// </summary>
    [Test]
    public async ValueTask SerializeNpcListsAndOperationResultsAsync()
    {
        var (player, output) = CastleSiegeRemoteViewTestHelper.CreatePlayer();
        await new CastleSiegeNpcListPlugIn(player)
            .ShowNpcListAsync(
                [
                    new CastleSiegeNpcInfo(
                        277,
                        9,
                        2,
                        1,
                        100_000,
                        75_000,
                        81,
                        59,
                        true),
                ])
            .ConfigureAwait(false);

        var operationView = new CastleSiegeNpcOperationResultPlugIn(player);
        await operationView.ShowBuyResultAsync(CastleSiegeNpcOperationResult.Success, 277, 9)
            .ConfigureAwait(false);
        await operationView.ShowRepairResultAsync(
                CastleSiegeNpcOperationResult.Success,
                277,
                9,
                75_000,
                100_000)
            .ConfigureAwait(false);
        await operationView.ShowUpgradeResultAsync(
                CastleSiegeNpcOperationResult.Success,
                277,
                9,
                CastleSiegeUpgradeType.Defense,
                2)
            .ConfigureAwait(false);
        await operationView.ShowGateInterfaceAsync(CastleSiegeNpcOperationResult.Success, 9)
            .ConfigureAwait(false);
        await operationView.ShowGateOperationResultAsync(CastleSiegeNpcOperationResult.Success, true, 9)
            .ConfigureAwait(false);
        await operationView.ShowGateStateAsync(true, 9).ConfigureAwait(false);

        var data = output.ToArray().AsMemory();
        var npcListLength = CastleSiegeNpcList.GetRequiredSize(1);
        Assert.That(
            data.Length,
            Is.EqualTo(
                npcListLength
                + CastleSiegeDefenseBuyResponse.Length
                + CastleSiegeDefenseRepairResponse.Length
                + CastleSiegeDefenseUpgradeResponse.Length
                + CastleSiegeGateInterfaceResponse.Length
                + CastleSiegeGateOperateResponse.Length
                + CastleSiegeGateStateNotification.Length));

        var npcPacket = (CastleSiegeNpcList)data[..npcListLength];
        Assert.That(npcPacket.Result, Is.EqualTo(1));
        Assert.That(npcPacket.NpcCount, Is.EqualTo(1));
        Assert.That(npcPacket[0].NpcNumber, Is.EqualTo(277));
        Assert.That(npcPacket[0].NpcIndex, Is.EqualTo(9));
        Assert.That(npcPacket[0].DefenseUpgradeLevel, Is.EqualTo(2));
        Assert.That(npcPacket[0].RegenerationLevel, Is.EqualTo(1));
        Assert.That(npcPacket[0].MaxHp, Is.EqualTo(100_000));
        Assert.That(npcPacket[0].CurrentHp, Is.EqualTo(75_000));
        Assert.That(npcPacket[0].PositionX, Is.EqualTo(81));
        Assert.That(npcPacket[0].PositionY, Is.EqualTo(59));
        Assert.That(npcPacket[0].IsAlive, Is.True);

        var offset = npcListLength;
        var buy = (CastleSiegeDefenseBuyResponse)data.Slice(
            offset,
            CastleSiegeDefenseBuyResponse.Length);
        Assert.That(buy.Result, Is.EqualTo((byte)CastleSiegeNpcOperationResult.Success));
        Assert.That(buy.NpcNumber, Is.EqualTo(277));
        Assert.That(buy.NpcIndex, Is.EqualTo(9));

        offset += CastleSiegeDefenseBuyResponse.Length;
        var repair = (CastleSiegeDefenseRepairResponse)data.Slice(
            offset,
            CastleSiegeDefenseRepairResponse.Length);
        Assert.That(repair.Result, Is.EqualTo((byte)CastleSiegeNpcOperationResult.Success));
        Assert.That(repair.CurrentHp, Is.EqualTo(75_000));
        Assert.That(repair.MaxHp, Is.EqualTo(100_000));

        offset += CastleSiegeDefenseRepairResponse.Length;
        var upgrade = (CastleSiegeDefenseUpgradeResponse)data.Slice(
            offset,
            CastleSiegeDefenseUpgradeResponse.Length);
        Assert.That(upgrade.Result, Is.EqualTo((byte)CastleSiegeNpcOperationResult.Success));
        Assert.That(upgrade.NpcUpgradeType, Is.EqualTo((uint)CastleSiegeUpgradeType.Defense));
        Assert.That(upgrade.NpcUpgradeValue, Is.EqualTo(2));

        offset += CastleSiegeDefenseUpgradeResponse.Length;
        var gateInterface = (CastleSiegeGateInterfaceResponse)data.Slice(
            offset,
            CastleSiegeGateInterfaceResponse.Length);
        Assert.That(gateInterface.Result, Is.EqualTo((byte)CastleSiegeNpcOperationResult.Success));
        Assert.That(gateInterface.GateIndex, Is.EqualTo(9));

        offset += CastleSiegeGateInterfaceResponse.Length;
        var gate = (CastleSiegeGateOperateResponse)data.Slice(
            offset,
            CastleSiegeGateOperateResponse.Length);
        Assert.That(gate.Result, Is.EqualTo((byte)CastleSiegeNpcOperationResult.Success));
        Assert.That(gate.IsOpen, Is.True);
        Assert.That(gate.GateIndex, Is.EqualTo(9));

        offset += CastleSiegeGateOperateResponse.Length;
        var gateState = (CastleSiegeGateStateNotification)data.Slice(
            offset,
            CastleSiegeGateStateNotification.Length);
        Assert.That(gateState.IsOpen, Is.True);
        Assert.That(gateState.GateIndex, Is.EqualTo(9));
    }
}
