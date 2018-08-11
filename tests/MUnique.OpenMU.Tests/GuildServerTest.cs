// <copyright file="GuildServerTest.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.Tests
{
    using System;
    using System.Linq;
    using Moq;
    using MUnique.OpenMU.Interfaces;
    using NUnit.Framework;

    /// <summary>
    /// Tests for the guild server.
    /// </summary>
    public class GuildServerTest : GuildTestBase
    {
        /// <summary>
        /// Tests if guild chat messages get forwarded to all servers.
        /// </summary>
        [Test]
        public void GuildChatForwarding()
        {
            const string sender = "Sender";
            const string message = "The Message";

            this.GuildServer.GuildMessage(1, sender, message);
            this.GameServer0.Verify(g => g.GuildChatMessage(1, sender, message), Times.Once);
            this.GameServer1.Verify(g => g.GuildChatMessage(1, sender, message), Times.Once);
        }

        /// <summary>
        /// Tests if the entrance of guild members is registered correctly in the guild member list.
        /// </summary>
        [Test]
        public void GuildMemberEnterGame()
        {
            const byte serverId = 1;
            var guildStatus = this.GuildServer.PlayerEnteredGame(this.GuildMaster.Id, this.GuildMaster.Name, serverId);
            var guildMaster = this.GuildServer.GetGuildList(guildStatus.GuildId).First();
            Assert.That(guildMaster.ServerId, Is.EqualTo(serverId));
            Assert.That(guildStatus, Is.Not.Null);
        }

        /// <summary>
        /// Tests if the exit of the last guild member removes (not deletes ;)) the guild from the guild server.
        /// </summary>
        [Test]
        public void LastGuildMemberLeaveGame()
        {
            const byte serverId = 1;
            var guildStatus = this.GuildServer.PlayerEnteredGame(this.GuildMaster.Id, this.GuildMaster.Name, serverId);
            this.GuildServer.GuildMemberLeftGame(guildStatus.GuildId, this.GuildMaster.Id, serverId);
            var guildList = this.GuildServer.GetGuildList(guildStatus.GuildId); // guild id is invalid now
            Assert.That(guildList, Is.Empty);
        }

        /// <summary>
        /// Tests if the exit of guild members is registered correctly in the guild member list.
        /// </summary>
        [Test]
        public void GuildMemberLeaveGame()
        {
            const byte serverId = 1;

            var guildStatus = this.GuildServer.PlayerEnteredGame(this.GuildMaster.Id, this.GuildMaster.Name, serverId);

            this.GuildServer.CreateGuildMember(guildStatus.GuildId, Guid.Empty, "TestMember", GuildPosition.NormalMember, serverId);
            this.GuildServer.GuildMemberLeftGame(guildStatus.GuildId, this.GuildMaster.Id, serverId);
            var guildMaster = this.GuildServer.GetGuildList(guildStatus.GuildId).First(m => m.PlayerPosition == GuildPosition.GuildMaster);
            Assert.That(guildMaster.ServerId, Is.EqualTo(OpenMU.GuildServer.GuildServer.OfflineServerId));
        }

        /// <summary>
        /// Tests if the removal of the whole guild is forwarded to all game servers when the guild master kicks himself.
        /// </summary>
        [Test]
        public void GuildPlayerKickDeletesGuild()
        {
            const byte serverId = 1;
            var guildStatus = this.GuildServer.PlayerEnteredGame(this.GuildMaster.Id, this.GuildMaster.Name, serverId);
            this.GuildServer.KickMember(guildStatus.GuildId, this.GuildMaster.Name);
            this.GameServer1.Verify(g => g.GuildDeleted(guildStatus.GuildId), Times.Once);
        }

        /// <summary>
        /// Tests if the removal of guild members is forwarded to all game servers.
        /// </summary>
        [Test]
        public void GuildPlayerKickRemovesPlayerFromGuild()
        {
            const byte serverId = 1;
            const string testMemberName = "TestMember";
            var guildStatus = this.GuildServer.PlayerEnteredGame(this.GuildMaster.Id, this.GuildMaster.Name, serverId);
            this.GuildServer.CreateGuildMember(guildStatus.GuildId, Guid.Empty, testMemberName, GuildPosition.NormalMember, serverId);
            this.GuildServer.KickMember(guildStatus.GuildId, testMemberName);
            this.GameServer1.Verify(g => g.GuildPlayerKicked(testMemberName), Times.Once);
        }
    }
}
