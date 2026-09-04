// <copyright file="CastleSiegeMachineRemoteViewTests.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.Tests;

using MUnique.OpenMU.GameServer.RemoteView.CastleSiege;
using MUnique.OpenMU.Network.Packets.ServerToClient;
using MUnique.OpenMU.Pathfinding;
using MachineType = MUnique.OpenMU.GameLogic.CastleSiege.CastleSiegeMachineType;
using MachinePacketType = MUnique.OpenMU.Network.Packets.ServerToClient.CastleSiegeMachineType;

/// <summary>
/// Tests serialization of Castle Siege warfare-machine views.
/// </summary>
[TestFixture]
public class CastleSiegeMachineRemoteViewTests
{
    /// <summary>
    /// Verifies machine-use, impact-region, and interface packets.
    /// </summary>
    [Test]
    public async ValueTask SerializeMachinePacketsAsync()
    {
        var (player, output) = CastleSiegeRemoteViewTestHelper.CreatePlayer();
        await new CastleSiegeMachineUseResultPlugIn(player)
            .ShowMachineUseResultAsync(true, 0x1234, MachineType.Attack, new Point(40, 41))
            .ConfigureAwait(false);
        await new CastleSiegeMachineRegionNotifyPlugIn(player)
            .ShowMachineRegionAsync(MachineType.Defense, new Point(42, 43))
            .ConfigureAwait(false);
        await new CastleSiegeMachineInterfacePlugIn(player)
            .ShowMachineInterfaceAsync(true, MachineType.Attack, 0x5678)
            .ConfigureAwait(false);

        var data = output.ToArray().AsMemory();
        Assert.That(
            data.Length,
            Is.EqualTo(
                CastleSiegeMachineUseResult.Length
                + CastleSiegeMachineRegionNotify.Length
                + CastleSiegeMachineInterface.Length));

        var useResult = (CastleSiegeMachineUseResult)data[..CastleSiegeMachineUseResult.Length];
        Assert.Multiple(() =>
        {
            Assert.That(useResult.Result, Is.EqualTo(1));
            Assert.That(useResult.NpcIndex, Is.EqualTo(0x1234));
            Assert.That(useResult.MachineType, Is.EqualTo(MachinePacketType.Attack));
            Assert.That(useResult.TargetX, Is.EqualTo(40));
            Assert.That(useResult.TargetY, Is.EqualTo(41));
        });

        var offset = CastleSiegeMachineUseResult.Length;
        var region = (CastleSiegeMachineRegionNotify)data.Slice(offset, CastleSiegeMachineRegionNotify.Length);
        Assert.Multiple(() =>
        {
            Assert.That(region.MachineType, Is.EqualTo(MachinePacketType.Defense));
            Assert.That(region.TargetX, Is.EqualTo(42));
            Assert.That(region.TargetY, Is.EqualTo(43));
        });

        offset += CastleSiegeMachineRegionNotify.Length;
        var machineInterface = (CastleSiegeMachineInterface)data.Slice(offset, CastleSiegeMachineInterface.Length);
        Assert.Multiple(() =>
        {
            Assert.That(machineInterface.Result, Is.EqualTo(1));
            Assert.That(machineInterface.MachineType, Is.EqualTo(MachinePacketType.Attack));
            Assert.That(machineInterface.NpcIndex, Is.EqualTo(0x5678));
        });
    }

    /// <summary>
    /// Verifies that rejected machine requests are sent as failure responses.
    /// </summary>
    [Test]
    public async ValueTask SerializeMachineFailurePacketsAsync()
    {
        var (player, output) = CastleSiegeRemoteViewTestHelper.CreatePlayer();
        await new CastleSiegeMachineUseResultPlugIn(player)
            .ShowMachineUseResultAsync(false, 0x1234, MachineType.Attack, default)
            .ConfigureAwait(false);
        await new CastleSiegeMachineInterfacePlugIn(player)
            .ShowMachineInterfaceAsync(false, MachineType.Defense, 0x5678)
            .ConfigureAwait(false);

        var data = output.ToArray().AsMemory();
        var useResult = (CastleSiegeMachineUseResult)data[..CastleSiegeMachineUseResult.Length];
        var machineInterface = (CastleSiegeMachineInterface)data.Slice(
            CastleSiegeMachineUseResult.Length,
            CastleSiegeMachineInterface.Length);
        Assert.Multiple(() =>
        {
            Assert.That(useResult.Result, Is.Zero);
            Assert.That(useResult.NpcIndex, Is.EqualTo(0x1234));
            Assert.That(machineInterface.Result, Is.Zero);
            Assert.That(machineInterface.NpcIndex, Is.EqualTo(0x5678));
        });
    }
}
