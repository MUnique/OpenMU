// <copyright file="CastleSiegeGuildSelectionRemoteViewTests.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.Tests;

using MUnique.OpenMU.GameLogic;
using MUnique.OpenMU.GameLogic.CastleSiege;
using MUnique.OpenMU.GameServer.RemoteView.CastleSiege;
using MUnique.OpenMU.Network.Packets.ServerToClient;
using BasicModel = MUnique.OpenMU.Persistence.BasicModel;
using DataJoinSide = MUnique.OpenMU.DataModel.Configuration.CastleSiegeJoinSide;
using PacketJoinSide = MUnique.OpenMU.Network.Packets.ServerToClient.CastleSiegeJoinSide;

/// <summary>
/// Tests Castle Siege guild-selection remote-view packet serialization.
/// </summary>
[TestFixture]
public class CastleSiegeGuildSelectionRemoteViewTests
{
    /// <summary>
    /// Verifies that a join-side notification is ignored after the player disconnected.
    /// </summary>
    [Test]
    public async ValueTask IgnoreJoinSideNotificationAfterDisconnectAsync()
    {
        var (player, output) = CastleSiegeRemoteViewTestHelper.CreatePlayer();
        await player.PlayerState.TryAdvanceToAsync(PlayerState.LoginScreen).ConfigureAwait(false);
        await player.DisconnectAsync().ConfigureAwait(false);
        var outputLengthAfterDisconnect = output.Length;
        Assert.That(player.Connection, Is.Null);

        await new CastleSiegeJoinSidePlugIn(player)
            .ShowJoinSideAsync(DataJoinSide.Attack1)
            .ConfigureAwait(false);

        Assert.That(output.Length, Is.EqualTo(outputLengthAfterDisconnect));
    }

    /// <summary>
    /// Verifies join-side, registration-list and selected-guild-list packets.
    /// </summary>
    [Test]
    public async ValueTask SerializeGuildSelectionResponsesAsync()
    {
        var (player, output) = CastleSiegeRemoteViewTestHelper.CreatePlayer();
        BasicModel.CastleSiegeGuildRegistration[] registrations =
        [
            new()
            {
                GuildName = "Alpha",
                Marks = 21,
                RegistrationOrder = 4,
            },
            new()
            {
                GuildName = "Bravo",
                Marks = 12,
                RegistrationOrder = 7,
            },
        ];
        CastleSiegeGuildParticipant[] guilds =
        [
            new()
            {
                GuildName = "Defend",
                Side = DataJoinSide.Defense,
                Score = 0,
                IsAllianceMaster = true,
            },
            new()
            {
                GuildName = "Alpha",
                Side = DataJoinSide.Attack1,
                Score = 177,
                IsAllianceMaster = true,
            },
        ];

        await new CastleSiegeJoinSidePlugIn(player)
            .ShowJoinSideAsync(DataJoinSide.Attack1)
            .ConfigureAwait(false);
        await new CastleSiegeRegisteredGuildListPlugIn(player)
            .ShowRegisteredGuildListAsync(registrations)
            .ConfigureAwait(false);
        await new CastleSiegeGuildListPlugIn(player)
            .ShowGuildListAsync(1, guilds)
            .ConfigureAwait(false);

        var data = output.ToArray().AsMemory();
        var registeredListLength = CastleSiegeRegisteredGuildListRef.GetRequiredSize(registrations.Length);
        var guildListLength = CastleSiegeGuildListRef.GetRequiredSize(guilds.Length);
        Assert.That(
            data.Length,
            Is.EqualTo(CastleSiegeJoinSideNotification.Length + registeredListLength + guildListLength));

        var joinSide = (CastleSiegeJoinSideNotification)data[..CastleSiegeJoinSideNotification.Length];
        Assert.That(joinSide.Side, Is.EqualTo(PacketJoinSide.Attack1));

        var offset = CastleSiegeJoinSideNotification.Length;
        var registeredList = (CastleSiegeRegisteredGuildList)data.Slice(offset, registeredListLength);
        Assert.Multiple(() =>
        {
            Assert.That(registeredList.Result, Is.EqualTo(1));
            Assert.That(registeredList.GuildCount, Is.EqualTo(2));
            Assert.That(registeredList[0].GuildName, Is.EqualTo("Alpha"));
            Assert.That(registeredList[0].GuildMarkCount, Is.EqualTo(21));
            Assert.That(registeredList[0].SequenceNumber, Is.EqualTo(4));
            Assert.That(registeredList[1].GuildName, Is.EqualTo("Bravo"));
            Assert.That(registeredList[1].GuildMarkCount, Is.EqualTo(12));
            Assert.That(registeredList[1].SequenceNumber, Is.EqualTo(7));
        });

        offset += registeredListLength;
        var guildList = (CastleSiegeGuildList)data.Slice(offset, guildListLength);
        Assert.Multiple(() =>
        {
            Assert.That(guildList.Result, Is.EqualTo(1));
            Assert.That(guildList.GuildCount, Is.EqualTo(2));
            Assert.That(guildList[0].Side, Is.EqualTo(PacketJoinSide.Defense));
            Assert.That(guildList[0].IsInvolved, Is.True);
            Assert.That(guildList[0].GuildName, Is.EqualTo("Defend"));
            Assert.That(guildList[0].Score, Is.Zero);
            Assert.That(guildList[1].Side, Is.EqualTo(PacketJoinSide.Attack1));
            Assert.That(guildList[1].IsInvolved, Is.True);
            Assert.That(guildList[1].GuildName, Is.EqualTo("Alpha"));
            Assert.That(guildList[1].Score, Is.EqualTo(177));
        });
    }
}
