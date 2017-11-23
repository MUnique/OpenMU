// <copyright file="GuildTestBase.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.Tests
{
    using System;
    using System.Collections.Generic;
    using MUnique.OpenMU.DataModel.Entities;
    using MUnique.OpenMU.Interfaces;
    using MUnique.OpenMU.Persistence;
    using NUnit.Framework;
    using Rhino.Mocks;

    /// <summary>
    /// Base class for guild related tests.
    /// </summary>
    public class GuildTestBase
    {
        /// <summary>
        /// Gets or sets the first game server.
        /// </summary>
        protected IGameServer GameServer0 { get; set; }

        /// <summary>
        /// Gets or sets the second game server.
        /// </summary>
        protected IGameServer GameServer1 { get; set; }

        /// <summary>
        /// Gets or sets the repository manager.
        /// </summary>
        protected IRepositoryManager RepositoryManager { get; set; }

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
        /// Gets or sets the guild.
        /// </summary>
        protected Guild Guild { get; set; }

        /// <summary>
        /// Setups the test objects.
        /// </summary>
        [SetUp]
        public virtual void Setup()
        {
            this.GameServer0 = MockRepository.GenerateMock<IGameServer>();
            this.GameServer1 = MockRepository.GenerateMock<IGameServer>();
            this.RepositoryManager = new TestRepositoryManager();

            this.GuildMaster = this.GetGuildMaster();
            this.GameServers = new Dictionary<int, IGameServer> { { 0, this.GameServer0 }, { 1, this.GameServer1 } };
            this.GuildServer = new OpenMU.GuildServer.GuildServer(this.GameServers, this.RepositoryManager);
            this.Guild = this.GuildServer.CreateGuild("Foobar", this.GuildMaster.Name, this.GuildMaster.Id, new byte[0], out ushort _, out GuildMemberInfo masterGuildMemberInfo);
            this.GuildMaster.GuildMemberInfo = masterGuildMemberInfo;

            this.RepositoryManager.GetRepository<Guild>().Stub(r => r.GetById(this.Guild.Id)).Return(this.Guild);
        }

        private Character GetGuildMaster()
        {
            var result = MockRepository.GenerateStub<Character>();
            result.Name = "GuildMaster";
            return result;
        }

        private class TestRepositoryManager : BaseRepositoryManager
        {
            public TestRepositoryManager()
            {
                var guildRepository = MockRepository.GenerateStub<IGuildRepository<Guild>>();

                this.RegisterRepository(guildRepository);
            }

            public override IContext CreateNewContext()
            {
                var context = MockRepository.GenerateStub<IContext>();
                context.Stub(c => c.SaveChanges()).Return(true);
                return context;
            }

            public override T CreateNew<T>(params object[] args)
            {
                if (typeof(T) == typeof(Guild))
                {
                    var guild = MockRepository.GenerateStub<Guild>();
                    guild.Id = Guid.NewGuid();
                    guild.Stub(g => g.Members).Return(new List<GuildMemberInfo>());
                    this.GetRepository<Guild>().Stub(r => r.GetById(guild.Id)).Return(guild);
                    return guild as T;
                }

                return base.CreateNew<T>(args);
            }
        }
    }
}
