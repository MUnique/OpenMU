// <copyright file="GuildServerTest.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.Tests
{
    using System;
    using System.Linq;
    using NUnit.Framework;
    using Rhino.Mocks;

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
            this.GameServer0.Expect(g => g.GuildChatMessage(this.Guild.Id, sender, message));
            this.GameServer1.Expect(g => g.GuildChatMessage(this.Guild.Id, sender, message));
            this.GuildServer.GuildMessage(this.Guild.Id, sender, message);
            this.GameServer0.VerifyAllExpectations();
            this.GameServer1.VerifyAllExpectations();
        }

        /// <summary>
        /// Tests if the entrance of guild members is registered correctly in the guild member list.
        /// </summary>
        [Test]
        public void GuildMemberEnterGame()
        {
            const byte serverId = 1;
            var guildMaster = this.Guild.Members.First(member => member.Name == this.GuildMaster.Name);
            var shortGuildId = this.GuildServer.GuildMemberEnterGame(this.Guild.Id, this.GuildMaster.Name, serverId);
            Assert.That(guildMaster.ServerId, Is.EqualTo(serverId));
            Assert.That(shortGuildId, Is.Not.EqualTo(0));
        }

        /// <summary>
        /// Tests if the exit of guild members is registered correctly in the guild member list.
        /// </summary>
        [Test]
        public void GuildMemberLeaveGame()
        {
            const byte serverId = 1;
            this.GuildServer.GuildMemberEnterGame(new Guid(1, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0), this.GuildMaster.Name, serverId);
            this.GuildServer.GuildMemberLeaveGame(this.Guild.Id, this.GuildMaster.Name, serverId);
            Assert.That(this.Guild.Members.First().ServerId, Is.EqualTo(OpenMU.GuildServer.GuildServer.OfflineServerId));
        }

        /// <summary>
        /// Tests if the removal of guild members is forwarded to all game servers.
        /// </summary>
        [Test]
        public void GuildPlayerKick()
        {
            const byte serverId = 1;
            this.GuildServer.GuildMemberEnterGame(this.Guild.Id, this.GuildMaster.Name, serverId);
            this.GameServer1.Expect(g => g.GuildPlayerKicked(this.GuildMaster.Name));
            this.GuildServer.KickPlayer(this.Guild.Id, this.GuildMaster.Name);
            this.GameServer1.VerifyAllExpectations();
        }
    }
}
