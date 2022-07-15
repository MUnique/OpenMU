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
    public async ValueTask GuildMemberEnterGameAsync()
    {
        const byte serverId = 1;
        await this.GuildServer.PlayerEnteredGameAsync(this.GuildMaster.Id, this.GuildMaster.Name, serverId).ConfigureAwait(false);
        var guildId = await this.GuildServer.GetGuildIdByNameAsync(GuildName).ConfigureAwait(false);
        var guildMaster = (await this.GuildServer.GetGuildListAsync(guildId).ConfigureAwait(false)).First();
        Assert.That(guildMaster.ServerId, Is.EqualTo(serverId));
        this.GameServer1.Verify(g => g.AssignGuildToPlayerAsync(this.GuildMaster.Name, It.Is<GuildMemberStatus>(s => s.GuildId == guildId && s.Position == GuildPosition.GuildMaster)));
    }

    /// <summary>
    /// Tests if the exit of the last guild member removes (not deletes ;)) the guild from the guild server.
    /// </summary>
    [Test]
    public async ValueTask LastGuildMemberLeaveGameAsync()
    {
        const byte serverId = 1;
        await this.GuildServer.PlayerEnteredGameAsync(this.GuildMaster.Id, this.GuildMaster.Name, serverId).ConfigureAwait(false);
        var guildId = await this.GuildServer.GetGuildIdByNameAsync(GuildName).ConfigureAwait(false);
        await this.GuildServer.GuildMemberLeftGameAsync(guildId, this.GuildMaster.Id, serverId).ConfigureAwait(false);
        var guildList = await this.GuildServer.GetGuildListAsync(guildId).ConfigureAwait(false); // guild id is invalid now
        Assert.That(guildList, Is.Empty);
    }

    /// <summary>
    /// Tests if the exit of guild members is registered correctly in the guild member list.
    /// </summary>
    [Test]
    public async ValueTask GuildMemberLeaveGameAsync()
    {
        const byte serverId = 1;

        await this.GuildServer.PlayerEnteredGameAsync(this.GuildMaster.Id, this.GuildMaster.Name, serverId).ConfigureAwait(false);
        var guildId = await this.GuildServer.GetGuildIdByNameAsync(GuildName).ConfigureAwait(false);

        await this.GuildServer.CreateGuildMemberAsync(guildId, Guid.Empty, "TestMember", GuildPosition.NormalMember, serverId).ConfigureAwait(false);
        await this.GuildServer.GuildMemberLeftGameAsync(guildId, this.GuildMaster.Id, serverId).ConfigureAwait(false);
        var guildMaster = (await this.GuildServer.GetGuildListAsync(guildId).ConfigureAwait(false)).First(m => m.PlayerPosition == GuildPosition.GuildMaster);
        Assert.That(guildMaster.ServerId, Is.EqualTo(OpenMU.GuildServer.GuildServer.OfflineServerId));
    }

    /// <summary>
    /// Tests if the removal of the whole guild is forwarded to all game servers when the guild master kicks himself.
    /// </summary>
    [Test]
    public async ValueTask GuildPlayerKickDeletesGuildAsync()
    {
        const byte serverId = 1;
        await this.GuildServer.PlayerEnteredGameAsync(this.GuildMaster.Id, this.GuildMaster.Name, serverId).ConfigureAwait(false);
        var guildId = await this.GuildServer.GetGuildIdByNameAsync(GuildName).ConfigureAwait(false);
        await this.GuildServer.KickMemberAsync(guildId, this.GuildMaster.Name).ConfigureAwait(false);
        this.GameServer1.Verify(g => g.GuildDeletedAsync(guildId), Times.Once);
    }

    /// <summary>
    /// Tests if the removal of guild members is forwarded to all game servers.
    /// </summary>
    [Test]
    public async ValueTask GuildPlayerKickRemovesPlayerFromGuildAsync()
    {
        const byte serverId = 1;
        const string testMemberName = "TestMember";
        await this.GuildServer.PlayerEnteredGameAsync(this.GuildMaster.Id, this.GuildMaster.Name, serverId).ConfigureAwait(false);
        var guildId = await this.GuildServer.GetGuildIdByNameAsync(GuildName).ConfigureAwait(false);
        await this.GuildServer.CreateGuildMemberAsync(guildId, Guid.Empty, testMemberName, GuildPosition.NormalMember, serverId).ConfigureAwait(false);
        await this.GuildServer.KickMemberAsync(guildId, testMemberName).ConfigureAwait(false);
        this.GameServer1.Verify(g => g.GuildPlayerKickedAsync(testMemberName), Times.Once);
    }
}