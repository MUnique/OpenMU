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
    using Guild = DataModel.Entities.Guild;

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
            var guildStatus = this.GuildServer.CreateGuild("Foobar", this.GuildMaster.Name, this.GuildMaster.Id, new byte[16], 0);
            this.GuildServer.GuildMemberLeftGame(guildStatus.GuildId, this.GuildMaster.Id, 0);
        }

        private Character GetGuildMaster()
        {
            var result = MockRepository.GenerateStub<Character>();
            result.Name = "GuildMaster";
            result.Id = new Guid("{32992D83-D3B4-444F-9652-BC0B5921EB64}");
            return result;
        }

        private sealed class TestRepositoryManager : BaseRepositoryManager
        {
            private readonly IList<Guild> guilds = new List<Guild>();
            private readonly IList<GuildMember> members = new List<GuildMember>();

            public TestRepositoryManager()
            {
                var guildRepository = MockRepository.GenerateStub<ITestGuildRepository>();
                guildRepository.Stub(r => r.GetAll()).WhenCalled(i => i.ReturnValue = this.guilds).Return(null);
                guildRepository.Stub(r => r.GetMemberNames(Guid.Empty)).IgnoreArguments()
                    .Return(new Dictionary<Guid, string>
                    {
                        { new Guid("{32992D83-D3B4-444F-9652-BC0B5921EB64}"), "GuildMaster" }
                    });
                this.RegisterRepository(guildRepository);
                this.RegisterRepository(MockRepository.GenerateStub<IRepository<GuildMember>>());
                this.GetRepository<GuildMember>().Stub(r => r.GetAll()).WhenCalled(i => i.ReturnValue = this.members).Return(null);
            }

            public override IContext CreateNewGuildContext()
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
                    guild.Stub(g => g.Members).Return(new List<GuildMember>());
                    this.GetRepository<Guild>().Stub(r => r.GetById(guild.Id)).IgnoreArguments().Return(guild);
                    this.guilds.Add(guild);
                    return guild as T;
                }

                if (typeof(T) == typeof(GuildMember))
                {
                    var member = MockRepository.GenerateStub<GuildMember>();
                    this.GetRepository<GuildMember>().Stub(r => r.GetById(member.Id)).IgnoreArguments().Return(member);
                    this.members.Add(member);
                    return member as T;
                }

                return base.CreateNew<T>(args);
            }

            /// <summary>
            /// An interface which inherits from <see cref="IGuildRepository"/> and <see cref="IRepository{Guild}"/> for easy stubbing of a guild repository.
            /// </summary>
            private interface ITestGuildRepository : IGuildRepository, IRepository<Guild>
            {
            }
        }
    }
}
