// <copyright file="GuildServerTest.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.Tests;

using Moq;
using MUnique.OpenMU.Interfaces;

/// <summary>
/// Tests for the guild server.
/// </summary>
public class GuildServerTest : GuildTestBase
{
    /// <summary>
    /// Tests if the entrance of guild members is registered correctly in the guild member list.
    /// </summary>
    [Test]
    public void GuildMemberEnterGame()
    {
        const byte serverId = 1;
        this.GuildServer.PlayerEnteredGame(this.GuildMaster.Id, this.GuildMaster.Name, serverId);
        var guildId = this.GuildServer.GetGuildIdByName(GuildName);
        var guildMaster = this.GuildServer.GetGuildList(guildId).First();
        Assert.That(guildMaster.ServerId, Is.EqualTo(serverId));
        this.GameServer1.Verify(g => g.AssignGuildToPlayer(this.GuildMaster.Name, It.Is<GuildMemberStatus>(s => s.GuildId == guildId && s.Position == GuildPosition.GuildMaster)));
    }

    /// <summary>
    /// Tests if the exit of the last guild member removes (not deletes ;)) the guild from the guild server.
    /// </summary>
    [Test]
    public void LastGuildMemberLeaveGame()
    {
        const byte serverId = 1;
        this.GuildServer.PlayerEnteredGame(this.GuildMaster.Id, this.GuildMaster.Name, serverId);
        var guildId = this.GuildServer.GetGuildIdByName(GuildName);
        this.GuildServer.GuildMemberLeftGame(guildId, this.GuildMaster.Id, serverId);
        var guildList = this.GuildServer.GetGuildList(guildId); // guild id is invalid now
        Assert.That(guildList, Is.Empty);
    }

    /// <summary>
    /// Tests if the exit of guild members is registered correctly in the guild member list.
    /// </summary>
    [Test]
    public void GuildMemberLeaveGame()
    {
        const byte serverId = 1;

        this.GuildServer.PlayerEnteredGame(this.GuildMaster.Id, this.GuildMaster.Name, serverId);
        var guildId = this.GuildServer.GetGuildIdByName(GuildName);

        this.GuildServer.CreateGuildMember(guildId, Guid.Empty, "TestMember", GuildPosition.NormalMember, serverId);
        this.GuildServer.GuildMemberLeftGame(guildId, this.GuildMaster.Id, serverId);
        var guildMaster = this.GuildServer.GetGuildList(guildId).First(m => m.PlayerPosition == GuildPosition.GuildMaster);
        Assert.That(guildMaster.ServerId, Is.EqualTo(OpenMU.GuildServer.GuildServer.OfflineServerId));
    }

    /// <summary>
    /// Tests if the removal of the whole guild is forwarded to all game servers when the guild master kicks himself.
    /// </summary>
    [Test]
    public void GuildPlayerKickDeletesGuild()
    {
        const byte serverId = 1;
        this.GuildServer.PlayerEnteredGame(this.GuildMaster.Id, this.GuildMaster.Name, serverId);
        var guildId = this.GuildServer.GetGuildIdByName(GuildName);
        this.GuildServer.KickMember(guildId, this.GuildMaster.Name);
        this.GameServer1.Verify(g => g.GuildDeleted(guildId), Times.Once);
    }

    /// <summary>
    /// Tests if the removal of guild members is forwarded to all game servers.
    /// </summary>
    [Test]
    public void GuildPlayerKickRemovesPlayerFromGuild()
    {
        const byte serverId = 1;
        const string testMemberName = "TestMember";
        this.GuildServer.PlayerEnteredGame(this.GuildMaster.Id, this.GuildMaster.Name, serverId);
        var guildId = this.GuildServer.GetGuildIdByName(GuildName);
        this.GuildServer.CreateGuildMember(guildId, Guid.Empty, testMemberName, GuildPosition.NormalMember, serverId);
        this.GuildServer.KickMember(guildId, testMemberName);
        this.GameServer1.Verify(g => g.GuildPlayerKicked(testMemberName), Times.Once);
    }
}