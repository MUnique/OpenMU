// <copyright file="GuildTestBase.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.Tests
{
    using System.Collections.Generic;
    using Moq;
    using MUnique.OpenMU.DataModel.Entities;
    using MUnique.OpenMU.Interfaces;
    using MUnique.OpenMU.Persistence;
    using MUnique.OpenMU.Persistence.InMemory;
    using NUnit.Framework;

    /// <summary>
    /// Base class for guild related tests.
    /// </summary>
    public class GuildTestBase
    {
        /// <summary>
        /// Gets or sets the first game server.
        /// </summary>
        protected Mock<IGameServer> GameServer0 { get; set; }

        /// <summary>
        /// Gets or sets the second game server.
        /// </summary>
        protected Mock<IGameServer> GameServer1 { get; set; }

        /// <summary>
        /// Gets or sets the repository manager.
        /// </summary>
        protected IPersistenceContextProvider PersistenceContextProvider { get; set; }

        /// <summary>
        /// Gets or sets the game servers.
        /// </summary>
        protected IDictionary<int, IGameServer> GameServers { get; set; }

        /// <summary>
        /// Gets or sets the guild server.
        /// </summary>
        protected IGuildServer GuildServer { get; set; }

        /// <summary>
        /// Gets or sets the guild master.
        /// </summary>
        protected Character GuildMaster { get; set; }

        /// <summary>
        /// Setups the test objects.
        /// </summary>
        [SetUp]
        public virtual void Setup()
        {
            this.GameServer0 = new Mock<IGameServer>();
            this.GameServer1 = new Mock<IGameServer>();
            this.PersistenceContextProvider = new InMemoryPersistenceContextProvider();

            this.GuildMaster = this.GetGuildMaster();
            this.GameServers = new Dictionary<int, IGameServer> { { 0, this.GameServer0.Object }, { 1, this.GameServer1.Object } };
            this.GuildServer = new OpenMU.GuildServer.GuildServer(this.GameServers, this.PersistenceContextProvider);
            var guildStatus = this.GuildServer.CreateGuild("Foobar", this.GuildMaster.Name, this.GuildMaster.Id, new byte[16], 0);
            this.GuildServer.GuildMemberLeftGame(guildStatus.GuildId, this.GuildMaster.Id, 0);
        }

        private Character GetGuildMaster()
        {
            var context = this.PersistenceContextProvider.CreateNewContext(null);
            var master = context.CreateNew<Character>();
            master.Name = "GuildMaster";
            return master;
        }
    }
}
