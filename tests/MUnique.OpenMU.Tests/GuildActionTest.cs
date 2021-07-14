// <copyright file="GuildActionTest.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.Tests
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using Microsoft.Extensions.Logging.Abstractions;
    using Moq;
    using MUnique.OpenMU.GameLogic;
    using MUnique.OpenMU.GameLogic.Attributes;
    using MUnique.OpenMU.GameLogic.NPC;
    using MUnique.OpenMU.GameLogic.PlayerActions.Guild;
    using MUnique.OpenMU.GameLogic.Views.Guild;
    using MUnique.OpenMU.GameServer;
    using MUnique.OpenMU.Interfaces;
    using MUnique.OpenMU.Persistence.BasicModel;
    using MUnique.OpenMU.Persistence.InMemory;
    using MUnique.OpenMU.PlugIns;
    using NUnit.Framework;

    /// <summary>
    /// Tests the guild player actions.
    /// </summary>
    [TestFixture]
    public class GuildActionTest : GuildTestBase
    {
        private Player guildMasterPlayer = null!;
        private IGameServerContext gameServerContext = null!;
        private Player player = null!;

        /// <inheritdoc/>
        [SetUp]
        public override void Setup()
        {
            base.Setup();

            this.gameServerContext = this.CreateGameServer();
            this.guildMasterPlayer = TestHelper.CreatePlayer(this.gameServerContext);
            this.guildMasterPlayer.SelectedCharacter!.Id = this.GuildMaster.Id;
            this.guildMasterPlayer.SelectedCharacter.Name = this.GuildMaster.Name;
            this.guildMasterPlayer.GuildStatus = this.GuildServer.PlayerEnteredGame(this.GuildMaster.Id, this.GuildMaster.Name, 0);
            this.guildMasterPlayer.Attributes![Stats.Level] = 100;
            this.player = TestHelper.CreatePlayer(this.gameServerContext);
            this.player.CurrentMap!.Add(this.guildMasterPlayer);
            this.player.SelectedCharacter!.Name = "Player";
            this.player.SelectedCharacter.Id = Guid.NewGuid();
            this.player.Attributes![Stats.Level] = 20;
        }

        /// <summary>
        /// Tests if a guild request from a player to a guild master gets forwarded to the guild masters view.
        /// </summary>
        [Test]
        public void GuildRequest()
        {
            var guildRequestAction = new GuildRequestAction();
            guildRequestAction.RequestGuild(this.player, this.guildMasterPlayer.Id);
            Assert.That(this.guildMasterPlayer.LastGuildRequester, Is.SameAs(this.player));
            Mock.Get(this.guildMasterPlayer.ViewPlugIns.GetPlugIn<IShowGuildJoinRequestPlugIn>()).Verify(g => g!.ShowGuildJoinRequest(this.player), Times.Once);
        }

        /// <summary>
        /// Tests if the guild member object gets created when the guild master accepts the request.
        /// </summary>
        [Test]
        public void GuildRequestAccept()
        {
            this.RequestGuildAndRespond(true);

            Assert.That(this.player.GuildStatus, Is.Not.Null);
            Assert.That(this.player.GuildStatus!.GuildId, Is.Not.EqualTo(0));
            Mock.Get(this.player.ViewPlugIns.GetPlugIn<IGuildJoinResponsePlugIn>()).Verify(g => g!.ShowGuildJoinResponse(GuildRequestAnswerResult.Accepted), Times.Once);
        }

        /// <summary>
        /// Tests if the guild member objects does not get created when the guild master refuses the request.
        /// </summary>
        [Test]
        public void GuildRequestRefuse()
        {
            this.RequestGuildAndRespond(false);
            Assert.That(this.player.GuildStatus, Is.Null);
            Mock.Get(this.player.ViewPlugIns.GetPlugIn<IGuildJoinResponsePlugIn>()).Verify(g => g!.ShowGuildJoinResponse(GuildRequestAnswerResult.Refused), Times.Once);
        }

        /// <summary>
        /// Tests if the guild creation dialog gets displayed when a player requests it.
        /// </summary>
        [Test]
        public void GuildCreationDialog()
        {
            var action = new GuildMasterAnswerAction();
            this.player.OpenedNpc = new NonPlayerCharacter(null!, null!, null!);
            action.ProcessAnswer(this.player, GuildMasterAnswerAction.Answer.ShowDialog);
            Mock.Get(this.player.ViewPlugIns.GetPlugIn<IShowGuildCreationDialogPlugIn>()).Verify(g => g!.ShowGuildCreationDialog(), Times.Once());
        }

        /// <summary>
        /// Tests if a guild does get created correctly, when a player executes the creation action.
        /// </summary>
        [Test]
        public void GuildCreate()
        {
            var action = new GuildCreateAction();
            action.CreateGuild(this.player, "Foobar2", Array.Empty<byte>());
            Assert.That(this.player.GuildStatus, Is.Not.Null);
            Assert.That(this.player.GuildStatus!.Position, Is.EqualTo(GuildPosition.GuildMaster));
            var context = this.PersistenceContextProvider.CreateNewGuildContext();
            var newGuild = context.Get<DataModel.Entities.Guild>().First(g => g.Name == "Foobar2");
            Assert.That(newGuild.Members.Any(m => m.Id == this.player.SelectedCharacter!.Id), Is.True);
        }

        /// <summary>
        /// Tests if the guild list request gets answered correctly.
        /// </summary>
        [Test]
        public void GetGuildList()
        {
            this.RequestGuildAndRespond(true);
            var action = new GuildListRequestAction();
            action.RequestGuildList(this.player);
            var guildList = this.GuildServer.GetGuildList(this.player.GuildStatus!.GuildId);
            Mock.Get(this.player.ViewPlugIns.GetPlugIn<IShowGuildListPlugIn>()).Verify(v => v!.ShowGuildList(It.Is<IEnumerable<GuildListEntry>>(list => list.Any(entry => entry.PlayerName == this.player.SelectedCharacter!.Name))), Times.Once());
            Assert.That(guildList.Any(entry => entry.PlayerName == this.player.SelectedCharacter!.Name), Is.True);
        }

        private void RequestGuildAndRespond(bool acceptRequest)
        {
            var guildRequestAction = new GuildRequestAction();
            guildRequestAction.RequestGuild(this.player, this.guildMasterPlayer.Id);
            var guildResponseAction = new GuildRequestAnswerAction();
            guildResponseAction.AnswerRequest(this.guildMasterPlayer, acceptRequest);
        }

        private IGameServerContext CreateGameServer()
        {
            var gameConfiguration = new GameConfiguration();
            gameConfiguration.Maps.Add(new GameMapDefinition());
            var mapInitializer = new MapInitializer(gameConfiguration, new NullLogger<MapInitializer>());
            var gameServer = new GameServerContext(
                new GameServerDefinition { GameConfiguration = gameConfiguration, ServerConfiguration = new DataModel.Configuration.GameServerConfiguration() },
                this.GuildServer,
                new Mock<ILoginServer>().Object,
                new Mock<IFriendServer>().Object,
                new InMemoryPersistenceContextProvider(),
                mapInitializer,
                new NullLoggerFactory(),
                new PlugInManager(new List<PlugIns.PlugInConfiguration>(), new NullLoggerFactory(), null));
            mapInitializer.PlugInManager = gameServer.PlugInManager;
            return gameServer;
        }
    }
}
