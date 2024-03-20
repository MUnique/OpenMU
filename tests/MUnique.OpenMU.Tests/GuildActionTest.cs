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
    public override async ValueTask SetupAsync()
    {
        await base.SetupAsync().ConfigureAwait(false);

        var gameServerContext = this.CreateGameServer();
        this._guildMasterPlayer = await TestHelper.CreatePlayerAsync(gameServerContext).ConfigureAwait(false);
        this._guildMasterPlayer.SelectedCharacter!.Id = this.GuildMaster.Id;
        this._guildMasterPlayer.SelectedCharacter.Name = this.GuildMaster.Name;
        await this.GuildServer.PlayerEnteredGameAsync(this.GuildMaster.Id, this.GuildMaster.Name, 0).ConfigureAwait(false);
        this._guildMasterPlayer.Attributes![Stats.Level] = 100;
        this._player = await TestHelper.CreatePlayerAsync(gameServerContext).ConfigureAwait(false);
        await this._player.CurrentMap!.AddAsync(this._guildMasterPlayer).ConfigureAwait(false);
        this._player.SelectedCharacter!.Name = "Player";
        this._player.SelectedCharacter.Id = Guid.NewGuid();
        this._player.Attributes![Stats.Level] = 20;
    }

    /// <inheritdoc />
    protected override void SetupGameServer(Mock<IGameServer> gameServer)
    {
        base.SetupGameServer(gameServer);
        gameServer.Setup(gs => gs.AssignGuildToPlayerAsync(It.IsAny<string>(), It.IsAny<GuildMemberStatus>()))
            .Callback((string name, GuildMemberStatus status) =>
            {
                if (this._player?.Name == name)
                {
                    this._player.GuildStatus = status;
                }

                if (this._guildMasterPlayer?.Name == name)
                {
                    this._guildMasterPlayer.GuildStatus = status;
                }
            });
    }

    /// <summary>
    /// Tests if a guild request from a player to a guild master gets forwarded to the guild masters view.
    /// </summary>
    [Test]
    public async ValueTask GuildRequestAsync()
    {
        var guildRequestAction = new GuildRequestAction();
        await guildRequestAction.RequestGuildAsync(this._player, this._guildMasterPlayer.Id).ConfigureAwait(false);
        Assert.That(this._guildMasterPlayer.LastGuildRequester, Is.SameAs(this._player));
        Mock.Get(this._guildMasterPlayer.ViewPlugIns.GetPlugIn<IShowGuildJoinRequestPlugIn>()).Verify(g => g!.ShowGuildJoinRequestAsync(this._player), Times.Once);
    }

    /// <summary>
    /// Tests if the guild member object gets created when the guild master accepts the request.
    /// </summary>
    [Test]
    public async ValueTask GuildRequestAcceptAsync()
    {
        await this.RequestGuildAndRespondAsync(true).ConfigureAwait(false);

        Assert.That(this._player.GuildStatus, Is.Not.Null);
        Assert.That(this._player.GuildStatus!.GuildId, Is.Not.EqualTo(0));
        Mock.Get(this._player.ViewPlugIns.GetPlugIn<IGuildJoinResponsePlugIn>()).Verify(g => g!.ShowGuildJoinResponseAsync(GuildRequestAnswerResult.Accepted), Times.Once);
    }

    /// <summary>
    /// Tests if the guild member objects does not get created when the guild master refuses the request.
    /// </summary>
    [Test]
    public async ValueTask GuildRequestRefuseAsync()
    {
        await this.RequestGuildAndRespondAsync(false).ConfigureAwait(false);
        Assert.That(this._player.GuildStatus, Is.Null);
        Mock.Get(this._player.ViewPlugIns.GetPlugIn<IGuildJoinResponsePlugIn>()).Verify(g => g!.ShowGuildJoinResponseAsync(GuildRequestAnswerResult.Refused), Times.Once);
    }

    /// <summary>
    /// Tests if the guild creation dialog gets displayed when a player requests it.
    /// </summary>
    [Test]
    public async ValueTask GuildCreationDialogAsync()
    {
        var action = new GuildMasterAnswerAction();
        this._player.OpenedNpc = new NonPlayerCharacter(null!, null!, null!);
        await action.ProcessAnswerAsync(this._player, GuildMasterAnswerAction.Answer.ShowDialog).ConfigureAwait(false);
        Mock.Get(this._player.ViewPlugIns.GetPlugIn<IShowGuildCreationDialogPlugIn>()).Verify(g => g!.ShowGuildCreationDialogAsync(), Times.Once());
    }

    /// <summary>
    /// Tests if a guild does get created correctly, when a player executes the creation action.
    /// </summary>
    [Test]
    public async ValueTask GuildCreateAsync()
    {
        var action = new GuildCreateAction();
        await action.CreateGuildAsync(this._player, "Foobar2", []).ConfigureAwait(false);
        Assert.That(this._player.GuildStatus, Is.Not.Null);
        Assert.That(this._player.GuildStatus!.Position, Is.EqualTo(GuildPosition.GuildMaster));
        var context = this.PersistenceContextProvider.CreateNewGuildContext();
        var newGuild = (await context.GetAsync<DataModel.Entities.Guild>().ConfigureAwait(false)).First(g => g.Name == "Foobar2");
        Assert.That(newGuild.Members.Any(m => m.Id == this._player.SelectedCharacter!.Id), Is.True);
    }

    /// <summary>
    /// Tests if the guild list request gets answered correctly.
    /// </summary>
    [Test]
    public async ValueTask GetGuildListAsync()
    {
        await this.RequestGuildAndRespondAsync(true).ConfigureAwait(false);
        var action = new GuildListRequestAction();
        await action.RequestGuildListAsync(this._player).ConfigureAwait(false);
        var guildList = await this.GuildServer.GetGuildListAsync(this._player.GuildStatus!.GuildId).ConfigureAwait(false);
        Mock.Get(this._player.ViewPlugIns.GetPlugIn<IShowGuildListPlugIn>()).Verify(v => v!.ShowGuildListAsync(It.Is<IEnumerable<GuildListEntry>>(list => list.Any(entry => entry.PlayerName == this._player.SelectedCharacter!.Name))), Times.Once());
        Assert.That(guildList.Any(entry => entry.PlayerName == this._player.SelectedCharacter!.Name), Is.True);
    }

    private async ValueTask RequestGuildAndRespondAsync(bool acceptRequest)
    {
        var guildRequestAction = new GuildRequestAction();
        await guildRequestAction.RequestGuildAsync(this._player, this._guildMasterPlayer.Id).ConfigureAwait(false);
        var guildResponseAction = new GuildRequestAnswerAction();
        await guildResponseAction.AnswerRequestAsync(this._guildMasterPlayer, acceptRequest).ConfigureAwait(false);
    }

    private IGameServerContext CreateGameServer()
    {
        var gameConfiguration = new GameConfiguration();
        gameConfiguration.Maps.Add(new GameMapDefinition());
        var mapInitializer = new MapInitializer(gameConfiguration, new NullLogger<MapInitializer>(), NullDropGenerator.Instance, null);

        var gameServer = new GameServerContext(
            new GameServerDefinition { GameConfiguration = gameConfiguration, ServerConfiguration = new DataModel.Configuration.GameServerConfiguration() },
            this.GuildServer,
            new Mock<IEventPublisher>().Object,
            new Mock<ILoginServer>().Object,
            new Mock<IFriendServer>().Object,
            new InMemoryPersistenceContextProvider(),
            mapInitializer,
            new NullLoggerFactory(),
            new PlugInManager(new List<PlugIns.PlugInConfiguration>(), new NullLoggerFactory(), null, null),
            NullDropGenerator.Instance,
            new ConfigurationChangeMediator());
        mapInitializer.PlugInManager = gameServer.PlugInManager;
        mapInitializer.PathFinderPool = gameServer.PathFinderPool;
        return gameServer;
    }
}