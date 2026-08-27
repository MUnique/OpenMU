// <copyright file="CastleSiegeCrownRemoteViewTests.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.Tests;

using MUnique.OpenMU.DataModel.Configuration;
using MUnique.OpenMU.GameLogic;
using MUnique.OpenMU.GameServer.RemoteView.CastleSiege;
using MUnique.OpenMU.Network.Packets.ServerToClient;
using CrownAccessPacket = MUnique.OpenMU.Network.Packets.ServerToClient.CastleSiegeCrownAccessState;
using CrownAccessState = MUnique.OpenMU.GameLogic.Views.CastleSiege.CastleSiegeCrownAccessState;
using DataJoinSide = MUnique.OpenMU.DataModel.Configuration.CastleSiegeJoinSide;
using SwitchInfo = MUnique.OpenMU.GameLogic.Views.CastleSiege.CastleSiegeSwitchInfo;
using SwitchInfoPacket = MUnique.OpenMU.Network.Packets.ServerToClient.CastleSiegeSwitchInfo;

/// <summary>
/// Tests Castle Siege Crown remote-view packet serialization.
/// </summary>
[TestFixture]
public class CastleSiegeCrownRemoteViewTests
{
    /// <summary>
    /// Verifies that Crown notifications are ignored after the player disconnected.
    /// </summary>
    [Test]
    public async ValueTask IgnoreCrownNotificationsAfterDisconnectAsync()
    {
        var (player, output) = CastleSiegeRemoteViewTestHelper.CreatePlayer();
        await player.PlayerState.TryAdvanceToAsync(PlayerState.LoginScreen).ConfigureAwait(false);
        await player.DisconnectAsync().ConfigureAwait(false);
        var outputLengthAfterDisconnect = output.Length;
        Assert.That(player.Connection, Is.Null);

        await new CastleSiegeCrownStatePlugIn(player).ShowCrownStateAsync(true).ConfigureAwait(false);
        await new CastleSiegeCrownAccessStatePlugIn(player)
            .ShowCrownAccessStateAsync(CrownAccessState.Attempt, TimeSpan.Zero)
            .ConfigureAwait(false);
        await new CastleSiegeSwitchInfoPlugIn(player)
            .ShowSwitchInfoAsync(new(1, false, DataJoinSide.None, string.Empty, string.Empty))
            .ConfigureAwait(false);
        await new CastleSiegeOwnershipChangePlugIn(player)
            .ShowOwnershipChangeAsync("Owner")
            .ConfigureAwait(false);

        Assert.That(output.Length, Is.EqualTo(outputLengthAfterDisconnect));
    }

    /// <summary>
    /// Verifies Crown, switch, and ownership notification packets.
    /// </summary>
    [Test]
    public async ValueTask SerializeCrownAndSwitchNotificationsAsync()
    {
        var (player, output) = CastleSiegeRemoteViewTestHelper.CreatePlayer();

        await new CastleSiegeCrownStatePlugIn(player)
            .ShowCrownStateAsync(true)
            .ConfigureAwait(false);
        await new CastleSiegeCrownAccessStatePlugIn(player)
            .ShowCrownAccessStateAsync(CrownAccessState.Success, TimeSpan.FromMilliseconds(12_345))
            .ConfigureAwait(false);
        await new CastleSiegeSwitchInfoPlugIn(player)
            .ShowSwitchInfoAsync(
                new SwitchInfo(
                    0x1234,
                    true,
                    DataJoinSide.Attack2,
                    "Attackr",
                    "Attacker"))
            .ConfigureAwait(false);
        await new CastleSiegeOwnershipChangePlugIn(player)
            .ShowOwnershipChangeAsync("Defender")
            .ConfigureAwait(false);

        var data = output.ToArray().AsMemory();
        Assert.That(
            data.Length,
            Is.EqualTo(
                CastleSiegeCrownStateUpdate.Length
                + CrownAccessPacket.Length
                + SwitchInfoPacket.Length
                + CastleSiegeBattleProcess.Length));

        var crownState = (CastleSiegeCrownStateUpdate)data[..CastleSiegeCrownStateUpdate.Length];
        Assert.That(crownState.State, Is.EqualTo(CastleSiegeCrownState.Accessible));

        var offset = CastleSiegeCrownStateUpdate.Length;
        var crownAccess = (CrownAccessPacket)data.Slice(offset, CrownAccessPacket.Length);
        Assert.Multiple(() =>
        {
            Assert.That(crownAccess.State, Is.EqualTo(CastleSiegeCrownAccessStateType.Succeeded));
            Assert.That(crownAccess.AccumulatedTimeMs, Is.EqualTo(12_345));
        });

        offset += CrownAccessPacket.Length;
        var switchInfo = (SwitchInfoPacket)data.Slice(offset, SwitchInfoPacket.Length);
        Assert.Multiple(() =>
        {
            Assert.That(switchInfo.SwitchIndex, Is.EqualTo(0x1234));
            Assert.That(switchInfo.IsOccupied, Is.True);
            Assert.That(switchInfo.JoinSide, Is.EqualTo(MUnique.OpenMU.Network.Packets.ServerToClient.CastleSiegeJoinSide.Attack2));
            Assert.That(switchInfo.GuildName, Is.EqualTo("Attackr"));
            Assert.That(switchInfo.UserName, Is.EqualTo("Attacker"));
        });

        offset += SwitchInfoPacket.Length;
        var ownership = (CastleSiegeBattleProcess)data.Slice(offset, CastleSiegeBattleProcess.Length);
        Assert.Multiple(() =>
        {
            Assert.That(ownership.State, Is.EqualTo(CastleSiegeBattleProcessState.CrownRegistrationSucceeded));
            Assert.That(ownership.GuildName, Is.EqualTo("Defender"));
        });
    }
}
