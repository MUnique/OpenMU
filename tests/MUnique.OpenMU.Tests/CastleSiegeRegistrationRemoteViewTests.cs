// <copyright file="CastleSiegeRegistrationRemoteViewTests.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.Tests;

using MUnique.OpenMU.GameLogic.Views.CastleSiege;
using MUnique.OpenMU.GameServer.RemoteView.CastleSiege;
using MUnique.OpenMU.Network.Packets.ServerToClient;

/// <summary>
/// Tests Castle Siege registration remote-view packet serialization.
/// </summary>
[TestFixture]
public class CastleSiegeRegistrationRemoteViewTests
{
    /// <summary>
    /// Verifies registration, unregistration, state and mark response packets.
    /// </summary>
    [Test]
    public async ValueTask SerializeRegistrationResponsesAsync()
    {
        var (player, output) = CastleSiegeRemoteViewTestHelper.CreatePlayer();

        await new CastleSiegeRegistrationResultPlugIn(player)
            .ShowRegistrationResultAsync(CastleSiegeRegistrationResult.AlreadyRegistered, "GuildA")
            .ConfigureAwait(false);
        await new CastleSiegeRegistrationResultPlugIn(player)
            .ShowUnregistrationResultAsync(CastleSiegeUnregistrationResult.Success, true, "GuildA")
            .ConfigureAwait(false);
        await new CastleSiegeRegistrationStatePlugIn(player)
            .ShowRegistrationStateAsync(
                CastleSiegeRegistrationStateResult.Registered,
                "GuildA",
                21,
                false,
                4)
            .ConfigureAwait(false);
        await new CastleSiegeMarkRegistrationResultPlugIn(player)
            .ShowMarkRegistrationResultAsync(CastleSiegeMarkRegistrationResult.IncorrectItem, "GuildA", 22)
            .ConfigureAwait(false);

        var data = output.ToArray().AsMemory();
        Assert.That(
            data.Length,
            Is.EqualTo(
                CastleSiegeRegistrationResponse.Length
                + CastleSiegeUnregisterResponse.Length
                + CastleSiegeRegistrationStateResponse.Length
                + CastleSiegeMarkRegistrationResponse.Length));

        var registration = (CastleSiegeRegistrationResponse)data[..CastleSiegeRegistrationResponse.Length];
        Assert.That(registration.Result, Is.EqualTo((byte)CastleSiegeRegistrationResult.AlreadyRegistered));
        Assert.That(registration.GuildName, Is.EqualTo("GuildA"));

        var offset = CastleSiegeRegistrationResponse.Length;
        var unregistration = (CastleSiegeUnregisterResponse)data.Slice(
            offset,
            CastleSiegeUnregisterResponse.Length);
        Assert.That(unregistration.Result, Is.EqualTo((byte)CastleSiegeUnregistrationResult.Success));
        Assert.That(unregistration.IsGivingUp, Is.True);
        Assert.That(unregistration.GuildName, Is.EqualTo("GuildA"));

        offset += CastleSiegeUnregisterResponse.Length;
        var state = (CastleSiegeRegistrationStateResponse)data.Slice(
            offset,
            CastleSiegeRegistrationStateResponse.Length);
        Assert.That(state.Result, Is.EqualTo((byte)CastleSiegeRegistrationStateResult.Registered));
        Assert.That(state.GuildName, Is.EqualTo("GuildA"));
        Assert.That(state.GuildMarkCount, Is.EqualTo(21));
        Assert.That(state.IsGivingUp, Is.False);
        Assert.That(state.RegistrationRank, Is.EqualTo(4));

        offset += CastleSiegeRegistrationStateResponse.Length;
        var mark = (CastleSiegeMarkRegistrationResponse)data.Slice(
            offset,
            CastleSiegeMarkRegistrationResponse.Length);
        Assert.That(mark.Result, Is.EqualTo((byte)CastleSiegeMarkRegistrationResult.IncorrectItem));
        Assert.That(mark.GuildName, Is.EqualTo("GuildA"));
        Assert.That(mark.GuildMarkCount, Is.EqualTo(22));
    }
}
