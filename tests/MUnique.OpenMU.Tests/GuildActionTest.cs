// <copyright file="GuildActionTest.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.Tests
{
    using System;
    using System.Linq;
    using MUnique.OpenMU.DataModel.Entities;
    using MUnique.OpenMU.GameLogic;
    using MUnique.OpenMU.GameLogic.NPC;
    using MUnique.OpenMU.GameLogic.PlayerActions.Guild;
    using MUnique.OpenMU.GameLogic.Views;
    using MUnique.OpenMU.GameServer.RemoteView;
    using MUnique.OpenMU.Interfaces;
    using NUnit.Framework;
    using Rhino.Mocks;

    /// <summary>
    /// Tests the guild player actions.
    /// </summary>
    [TestFixture]
    public class GuildActionTest : GuildTestBase
    {
        private Player guildMasterPlayer;
        private IGameServerContext gameServerContext;
        private Player player;

        /// <inheritdoc/>
        [SetUp]
        public override void Setup()
        {
            base.Setup();
            this.guildMasterPlayer = this.CreateGuildMasterPlayer();
            this.gameServerContext = this.CreateGameServer();
            this.guildMasterPlayer.GuildStatus = this.GuildServer.PlayerEnteredGame(this.GuildMaster.Id, this.GuildMaster.Name, 0);
            this.player = TestHelper.GetPlayer();
            this.player.CurrentMap.Add(this.guildMasterPlayer);
            this.player.SelectedCharacter.Name = "Player";
        }

        /// <summary>
        /// Tests if a guild request from a player to a guild master gets forwarded to the guild masters view.
        /// </summary>
        [Test]
        public void GuildRequest()
        {
            var guildRequestAction = new GuildRequestAction(this.gameServerContext);
            this.guildMasterPlayer.PlayerView.GuildView.Expect(v => v.ShowGuildJoinRequest(this.player));
            guildRequestAction.RequestGuild(this.player, this.guildMasterPlayer.Id);
            Assert.That(this.guildMasterPlayer.LastGuildRequester, Is.SameAs(this.player));
            this.guildMasterPlayer.PlayerView.GuildView.VerifyAllExpectations();
        }

        /// <summary>
        /// Tests if the guild member object gets created when the guild master accepts the request.
        /// </summary>
        [Test]
        public void GuildRequestAccept()
        {
            this.player.PlayerView.GuildView.Expect(v => v.GuildJoinResponse(GuildRequestAnswerResult.Accepted));
            this.RequestGuildAndRespond(true);

            Assert.That(this.player.GuildStatus, Is.Not.Null);
            Assert.That(this.player.GuildStatus.GuildId, Is.Not.EqualTo(0));
            this.player.PlayerView.GuildView.VerifyAllExpectations();
        }

        /// <summary>
        /// Tests if the guild member objects does not get created when the guild master refuses the request.
        /// </summary>
        [Test]
        public void GuildRequestRefuse()
        {
            this.player.PlayerView.GuildView.Expect(v => v.GuildJoinResponse(GuildRequestAnswerResult.Refused));
            this.RequestGuildAndRespond(false);
            Assert.That(this.player.GuildStatus, Is.Null);
            this.player.PlayerView.GuildView.VerifyAllExpectations();
        }

        /// <summary>
        /// Tests if the guild creation dialog gets displayed when a player requests it.
        /// </summary>
        [Test]
        public void GuildCreationDialog()
        {
            var action = new GuildMasterAnswerAction();
            this.player.OpenedNpc = new NonPlayerCharacter(null, null, null);
            this.player.PlayerView.GuildView.Expect(g => g.ShowGuildCreationDialog());
            action.ProcessAnswer(this.player, GuildMasterAnswerAction.Answer.ShowDialog);
            this.player.PlayerView.GuildView.VerifyAllExpectations();
        }

        /// <summary>
        /// Tests if a guild does get created correctly, when a player executes the creation action.
        /// </summary>
        [Test]
        public void GuildCreate()
        {
            var action = new GuildCreateAction(this.gameServerContext);
            action.CreateGuild(this.player, "Foobar2", new byte[0]);
            Assert.That(this.player.GuildStatus, Is.Not.Null);
            Assert.That(this.player.GuildStatus.Position, Is.EqualTo(GuildPosition.GuildMaster));
            var repository = this.RepositoryManager.GetRepository<DataModel.Entities.Guild>();
            var newGuild = repository.GetAll().First(g => g.Name == "Foobar2");
            Assert.That(newGuild.Members.Any(m => m.Id == this.player.SelectedCharacter.Id), Is.True);
        }

        /// <summary>
        /// Tests if the guild list request gets answered correctly.
        /// </summary>
        [Test]
        public void GetGuildList()
        {
            this.RequestGuildAndRespond(true);
            var action = new GuildListRequestAction(this.gameServerContext);
            this.player.PlayerView.GuildView.Expect(v => v.ShowGuildList(null)).IgnoreArguments();
            action.RequestGuildList(this.player);
            this.player.PlayerView.GuildView.VerifyAllExpectations();
            var guildList = this.GuildServer.GetGuildList(this.player.GuildStatus.GuildId);
            Assert.That(guildList.Any(entry => entry.PlayerName == this.player.SelectedCharacter.Name), Is.True);
        }

        private void RequestGuildAndRespond(bool acceptRequest)
        {
            var guildRequestAction = new GuildRequestAction(this.gameServerContext);
            guildRequestAction.RequestGuild(this.player, this.guildMasterPlayer.Id);
            var guildResponseAction = new GuildRequestAnswerAction(this.gameServerContext);
            guildResponseAction.AnswerRequest(this.guildMasterPlayer, acceptRequest);
        }

        private IGameServerContext CreateGameServer()
        {
            var gameServer = MockRepository.GenerateMock<IGameServerContext>();
            gameServer.Stub(g => g.Configuration).Return(new OpenMU.DataModel.Configuration.GameConfiguration());
            gameServer.Stub(g => g.GuildCache).Return(new GuildCache(gameServer, new GuildInfoSerializer()));
            gameServer.Stub(g => g.GuildServer).Return(this.GuildServer);

            return gameServer;
        }

        private Player CreateGuildMasterPlayer()
        {
            var masterPlayer = TestHelper.GetPlayer();
            this.GuildMaster = masterPlayer.SelectedCharacter;
            this.GuildMaster.Name = "GuildMaster";
            this.GuildMaster.Id = Guid.NewGuid();
            return masterPlayer;
        }
    }
}
