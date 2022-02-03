// <copyright file="GuildActionTest.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.Tests;

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

/// <summary>
/// Tests the guild player actions.
/// </summary>
[TestFixture]
public class GuildActionTest : GuildTestBase
{
    private Player _guildMasterPlayer = null!;
    private Player _player = null!;

    /// <inheritdoc/>
    [SetUp]
    public override void Setup()
    {
        base.Setup();

        var gameServerContext = this.CreateGameServer();
        this._guildMasterPlayer = TestHelper.CreatePlayer(gameServerContext);
        this._guildMasterPlayer.SelectedCharacter!.Id = this.GuildMaster.Id;
        this._guildMasterPlayer.SelectedCharacter.Name = this.GuildMaster.Name;
        this.GuildServer.PlayerEnteredGame(this.GuildMaster.Id, this.GuildMaster.Name, 0);
        this._guildMasterPlayer.Attributes![Stats.Level] = 100;
        this._player = TestHelper.CreatePlayer(gameServerContext);
        this._player.CurrentMap!.Add(this._guildMasterPlayer);
        this._player.SelectedCharacter!.Name = "Player";
        this._player.SelectedCharacter.Id = Guid.NewGuid();
        this._player.Attributes![Stats.Level] = 20;
    }

    /// <summary>
    /// Tests if a guild request from a player to a guild master gets forwarded to the guild masters view.
    /// </summary>
    [Test]
    public void GuildRequest()
    {
        var guildRequestAction = new GuildRequestAction();
        guildRequestAction.RequestGuild(this._player, this._guildMasterPlayer.Id);
        Assert.That(this._guildMasterPlayer.LastGuildRequester, Is.SameAs(this._player));
        Mock.Get(this._guildMasterPlayer.ViewPlugIns.GetPlugIn<IShowGuildJoinRequestPlugIn>()).Verify(g => g!.ShowGuildJoinRequest(this._player), Times.Once);
    }

    /// <summary>
    /// Tests if the guild member object gets created when the guild master accepts the request.
    /// </summary>
    [Test]
    public void GuildRequestAccept()
    {
        this.RequestGuildAndRespond(true);

        Assert.That(this._player.GuildStatus, Is.Not.Null);
        Assert.That(this._player.GuildStatus!.GuildId, Is.Not.EqualTo(0));
        Mock.Get(this._player.ViewPlugIns.GetPlugIn<IGuildJoinResponsePlugIn>()).Verify(g => g!.ShowGuildJoinResponse(GuildRequestAnswerResult.Accepted), Times.Once);
    }

    /// <summary>
    /// Tests if the guild member objects does not get created when the guild master refuses the request.
    /// </summary>
    [Test]
    public void GuildRequestRefuse()
    {
        this.RequestGuildAndRespond(false);
        Assert.That(this._player.GuildStatus, Is.Null);
        Mock.Get(this._player.ViewPlugIns.GetPlugIn<IGuildJoinResponsePlugIn>()).Verify(g => g!.ShowGuildJoinResponse(GuildRequestAnswerResult.Refused), Times.Once);
    }

    /// <summary>
    /// Tests if the guild creation dialog gets displayed when a player requests it.
    /// </summary>
    [Test]
    public void GuildCreationDialog()
    {
        var action = new GuildMasterAnswerAction();
        this._player.OpenedNpc = new NonPlayerCharacter(null!, null!, null!);
        action.ProcessAnswer(this._player, GuildMasterAnswerAction.Answer.ShowDialog);
        Mock.Get(this._player.ViewPlugIns.GetPlugIn<IShowGuildCreationDialogPlugIn>()).Verify(g => g!.ShowGuildCreationDialog(), Times.Once());
    }

    /// <summary>
    /// Tests if a guild does get created correctly, when a player executes the creation action.
    /// </summary>
    [Test]
    public void GuildCreate()
    {
        var action = new GuildCreateAction();
        action.CreateGuild(this._player, "Foobar2", Array.Empty<byte>());
        Assert.That(this._player.GuildStatus, Is.Not.Null);
        Assert.That(this._player.GuildStatus!.Position, Is.EqualTo(GuildPosition.GuildMaster));
        var context = this.PersistenceContextProvider.CreateNewGuildContext();
        var newGuild = context.Get<DataModel.Entities.Guild>().First(g => g.Name == "Foobar2");
        Assert.That(newGuild.Members.Any(m => m.Id == this._player.SelectedCharacter!.Id), Is.True);
    }

    /// <summary>
    /// Tests if the guild list request gets answered correctly.
    /// </summary>
    [Test]
    public void GetGuildList()
    {
        this.RequestGuildAndRespond(true);
        var action = new GuildListRequestAction();
        action.RequestGuildList(this._player);
        var guildList = this.GuildServer.GetGuildList(this._player.GuildStatus!.GuildId);
        Mock.Get(this._player.ViewPlugIns.GetPlugIn<IShowGuildListPlugIn>()).Verify(v => v!.ShowGuildList(It.Is<IEnumerable<GuildListEntry>>(list => list.Any(entry => entry.PlayerName == this._player.SelectedCharacter!.Name))), Times.Once());
        Assert.That(guildList.Any(entry => entry.PlayerName == this._player.SelectedCharacter!.Name), Is.True);
    }

    private void RequestGuildAndRespond(bool acceptRequest)
    {
        var guildRequestAction = new GuildRequestAction();
        guildRequestAction.RequestGuild(this._player, this._guildMasterPlayer.Id);
        var guildResponseAction = new GuildRequestAnswerAction();
        guildResponseAction.AnswerRequest(this._guildMasterPlayer, acceptRequest);
    }

    private IGameServerContext CreateGameServer()
    {
        var gameConfiguration = new GameConfiguration();
        gameConfiguration.Maps.Add(new GameMapDefinition());
        var mapInitializer = new MapInitializer(gameConfiguration, new NullLogger<MapInitializer>(), NullDropGenerator.Instance);
        
        var gameServer = new GameServerContext(
            new GameServerDefinition { GameConfiguration = gameConfiguration, ServerConfiguration = new DataModel.Configuration.GameServerConfiguration() },
            this.GuildServer,
            new Mock<IEventPublisher>().Object,
            new Mock<ILoginServer>().Object,
            new Mock<IFriendServer>().Object,
            new InMemoryPersistenceContextProvider(),
            mapInitializer,
            new NullLoggerFactory(),
            new PlugInManager(new List<PlugIns.PlugInConfiguration>(), new NullLoggerFactory(), null),
            NullDropGenerator.Instance);
        mapInitializer.PlugInManager = gameServer.PlugInManager;
        return gameServer;
    }
}