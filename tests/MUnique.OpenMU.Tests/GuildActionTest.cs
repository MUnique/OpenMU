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
using MUnique.OpenMU.GameServer.MessageHandler.Guild;
using MUnique.OpenMU.Interfaces;
using MUnique.OpenMU.Network.Packets.ClientToServer;
using MUnique.OpenMU.Network.Packets.ServerToClient;
using MUnique.OpenMU.Persistence.BasicModel;
using MUnique.OpenMU.Persistence.InMemory;
using MUnique.OpenMU.PlugIns;
using ViewGuildRelationshipRequestType = MUnique.OpenMU.GameLogic.Views.Guild.GuildRelationshipRequestType;
using ViewGuildRelationshipType = MUnique.OpenMU.GameLogic.Views.Guild.GuildRelationshipType;

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
        this._guildMasterPlayer = await PlayerTestHelper.CreatePlayerAsync(gameServerContext).ConfigureAwait(false);
        this._guildMasterPlayer.SelectedCharacter!.Id = this.GuildMaster.Id;
        this._guildMasterPlayer.SelectedCharacter.Name = this.GuildMaster.Name;
        await this.GuildServer.PlayerEnteredGameAsync(this.GuildMaster.Id, this.GuildMaster.Name, 0).ConfigureAwait(false);
        this._guildMasterPlayer.Attributes![Stats.Level] = 100;
        this._player = await PlayerTestHelper.CreatePlayerAsync(gameServerContext).ConfigureAwait(false);
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
        Mock.Get(this._guildMasterPlayer.ViewPlugIns.GetPlugIn<IShowGuildJoinRequestPlugIn>()!).Verify(g => g!.ShowGuildJoinRequestAsync(this._player), Times.Once);
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
        Mock.Get(this._player.ViewPlugIns.GetPlugIn<IGuildJoinResponsePlugIn>()!).Verify(g => g!.ShowGuildJoinResponseAsync(GuildRequestAnswerResult.Accepted), Times.Once);
    }

    /// <summary>
    /// Tests if the guild member objects does not get created when the guild master refuses the request.
    /// </summary>
    [Test]
    public async ValueTask GuildRequestRefuseAsync()
    {
        await this.RequestGuildAndRespondAsync(false).ConfigureAwait(false);
        Assert.That(this._player.GuildStatus, Is.Null);
        Mock.Get(this._player.ViewPlugIns.GetPlugIn<IGuildJoinResponsePlugIn>()!).Verify(g => g!.ShowGuildJoinResponseAsync(GuildRequestAnswerResult.Refused), Times.Once);
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
        Mock.Get(this._player.ViewPlugIns.GetPlugIn<IShowGuildCreationDialogPlugIn>()!).Verify(g => g!.ShowGuildCreationDialogAsync(), Times.Once());
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
        Mock.Get(this._player.ViewPlugIns.GetPlugIn<IShowGuildListPlugIn>()!)
            .Verify(v => v!.ShowGuildListAsync(
                It.Is<IReadOnlyCollection<GuildListEntry>>(list => list.Any(entry => entry.PlayerName == this._player.SelectedCharacter!.Name)),
                It.Is<Interfaces.Guild>(g => g.Name == GuildName)), Times.Once());
        Assert.That(guildList.Any(entry => entry.PlayerName == this._player.SelectedCharacter!.Name), Is.True);
    }

    private async ValueTask RequestGuildAndRespondAsync(bool acceptRequest)
    {
        var guildRequestAction = new GuildRequestAction();
        await guildRequestAction.RequestGuildAsync(this._player, this._guildMasterPlayer.Id).ConfigureAwait(false);
        var guildResponseAction = new GuildRequestAnswerAction();
        await guildResponseAction.AnswerRequestAsync(this._guildMasterPlayer, acceptRequest).ConfigureAwait(false);
    }

    /// <summary>
    /// Tests that the guild master can promote a member to battle master.
    /// </summary>
    [Test]
    public async ValueTask GuildRoleAssignPromoteToBattleMasterAsync()
    {
        await this.PrepareRoleAssignScenarioAsync().ConfigureAwait(false);

        var action = new GuildRoleAssignAction();
        await action.AssignRoleAsync(this._guildMasterPlayer, this._player.SelectedCharacter!.Name, GuildPosition.BattleMaster).ConfigureAwait(false);

        var guildList = await this.GuildServer.GetGuildListAsync(this._player.GuildStatus!.GuildId).ConfigureAwait(false);
        var entry = guildList.First(e => e.PlayerName == this._player.SelectedCharacter!.Name);
        Assert.Multiple(() =>
        {
            Assert.That(entry.PlayerPosition, Is.EqualTo(GuildPosition.BattleMaster));
            Assert.That(this._player.GuildStatus!.Position, Is.EqualTo(GuildPosition.BattleMaster));
        });
    }

    /// <summary>
    /// Tests that the guild master can promote a member to assistant master and demote back to normal member.
    /// </summary>
    [Test]
    public async ValueTask GuildRoleAssignPromoteAndDemoteAsync()
    {
        await this.PrepareRoleAssignScenarioAsync().ConfigureAwait(false);

        var action = new GuildRoleAssignAction();
        await action.AssignRoleAsync(this._guildMasterPlayer, this._player.SelectedCharacter!.Name, GuildPosition.AssistantMaster).ConfigureAwait(false);
        Assert.That(this._player.GuildStatus!.Position, Is.EqualTo(GuildPosition.AssistantMaster));

        await action.AssignRoleAsync(this._guildMasterPlayer, this._player.SelectedCharacter!.Name, GuildPosition.NormalMember).ConfigureAwait(false);
        var guildList = await this.GuildServer.GetGuildListAsync(this._player.GuildStatus!.GuildId).ConfigureAwait(false);
        var entry = guildList.First(e => e.PlayerName == this._player.SelectedCharacter!.Name);
        Assert.Multiple(() =>
        {
            Assert.That(entry.PlayerPosition, Is.EqualTo(GuildPosition.NormalMember));
            Assert.That(this._player.GuildStatus!.Position, Is.EqualTo(GuildPosition.NormalMember));
        });
    }

    /// <summary>
    /// Tests that a non-master cannot assign roles.
    /// </summary>
    [Test]
    public async ValueTask GuildRoleAssignNonMasterRejectedAsync()
    {
        await this.PrepareRoleAssignScenarioAsync().ConfigureAwait(false);

        var action = new GuildRoleAssignAction();
        await action.AssignRoleAsync(this._player, this._guildMasterPlayer.SelectedCharacter!.Name, GuildPosition.BattleMaster).ConfigureAwait(false);

        Assert.That(this._guildMasterPlayer.GuildStatus!.Position, Is.EqualTo(GuildPosition.GuildMaster));
    }

    /// <summary>
    /// Tests that promotion to guild master (leadership transfer) is rejected.
    /// </summary>
    [Test]
    public async ValueTask GuildRoleAssignMasterTransferRejectedAsync()
    {
        await this.PrepareRoleAssignScenarioAsync().ConfigureAwait(false);

        var action = new GuildRoleAssignAction();
        await action.AssignRoleAsync(this._guildMasterPlayer, this._player.SelectedCharacter!.Name, GuildPosition.GuildMaster).ConfigureAwait(false);

        Assert.That(this._player.GuildStatus!.Position, Is.EqualTo(GuildPosition.NormalMember));
    }

    /// <summary>
    /// Tests that an undefined position is rejected.
    /// </summary>
    [Test]
    public async ValueTask GuildRoleAssignInvalidPositionRejectedAsync()
    {
        await this.PrepareRoleAssignScenarioAsync().ConfigureAwait(false);

        var action = new GuildRoleAssignAction();
        await action.AssignRoleAsync(this._guildMasterPlayer, this._player.SelectedCharacter!.Name, GuildPosition.Undefined).ConfigureAwait(false);

        Assert.That(this._player.GuildStatus!.Position, Is.EqualTo(GuildPosition.NormalMember));
    }

    /// <summary>
    /// Tests that assigning a role to an offline player is rejected.
    /// </summary>
    [Test]
    public async ValueTask GuildRoleAssignOfflineTargetRejectedAsync()
    {
        await this.PrepareRoleAssignScenarioAsync().ConfigureAwait(false);

        var action = new GuildRoleAssignAction();
        await action.AssignRoleAsync(this._guildMasterPlayer, "NobodyOnline", GuildPosition.BattleMaster).ConfigureAwait(false);

        Assert.That(this._player.GuildStatus!.Position, Is.EqualTo(GuildPosition.NormalMember));
    }

    /// <summary>
    /// Tests that assigning a role to a player of another guild (or no guild) is rejected.
    /// </summary>
    [Test]
    public async ValueTask GuildRoleAssignDifferentGuildTargetRejectedAsync()
    {
        await this.PrepareRoleAssignScenarioAsync().ConfigureAwait(false);

        var outsider = await PlayerTestHelper.CreatePlayerAsync(this._guildMasterPlayer.GameContext).ConfigureAwait(false);
        outsider.SelectedCharacter!.Name = "Outsider";
        await outsider.PlayerState.TryAdvanceToAsync(PlayerState.EnteredWorld).ConfigureAwait(false);
        if (outsider.GameContext is GameContext gameContext)
        {
            gameContext.PlayersByCharacterName.TryAdd(outsider.SelectedCharacter!.Name, outsider);
        }

        var action = new GuildRoleAssignAction();
        await action.AssignRoleAsync(this._guildMasterPlayer, "Outsider", GuildPosition.BattleMaster).ConfigureAwait(false);

        Assert.That(outsider.GuildStatus, Is.Null);
        Assert.That(this._player.GuildStatus!.Position, Is.EqualTo(GuildPosition.NormalMember));
    }

    /// <summary>
    /// Tests that the guild master cannot change its own role.
    /// </summary>
    [Test]
    public async ValueTask GuildRoleAssignSelfRejectedAsync()
    {
        await this.PrepareRoleAssignScenarioAsync().ConfigureAwait(false);

        var action = new GuildRoleAssignAction();
        await action.AssignRoleAsync(this._guildMasterPlayer, this._guildMasterPlayer.SelectedCharacter!.Name, GuildPosition.BattleMaster).ConfigureAwait(false);

        Assert.That(this._guildMasterPlayer.GuildStatus!.Position, Is.EqualTo(GuildPosition.GuildMaster));
    }

    /// <summary>
    /// Tests that a relationship request from a non-master fails gracefully instead of throwing.
    /// </summary>
    [Test]
    public async ValueTask GuildRelationshipRequestNonMasterReturnsNoAuthorizationAsync()
    {
        await this.PrepareRoleAssignScenarioAsync().ConfigureAwait(false);

        var action = new GuildRelationshipChangeAction();
        await action.RequestAsync(this._player, this._guildMasterPlayer.Id, ViewGuildRelationshipType.Alliance, ViewGuildRelationshipRequestType.Join).ConfigureAwait(false);

        Mock.Get(this._player.ViewPlugIns.GetPlugIn<IGuildRelationshipChangeResultPlugIn>()!)
            .Verify(v => v!.ShowResultAsync(ViewGuildRelationshipType.Alliance, ViewGuildRelationshipRequestType.Join, GuildRelationshipChangeResultType.NoAuthorization, this._guildMasterPlayer.Id), Times.Once());
    }

    /// <summary>
    /// Tests that leaving an alliance without being in a guild fails gracefully instead of throwing.
    /// </summary>
    [Test]
    public async ValueTask GuildRelationshipLeaveWithoutGuildReturnsGracefullyAsync()
    {
        var action = new GuildRelationshipChangeAction();
        await action.RequestLeaveAllianceAsync(this._player).ConfigureAwait(false);

        Mock.Get(this._player.ViewPlugIns.GetPlugIn<IGuildRelationshipChangeResultPlugIn>()!)
            .Verify(v => v!.ShowResultAsync(ViewGuildRelationshipType.Alliance, ViewGuildRelationshipRequestType.Leave, GuildRelationshipChangeResultType.Failed, (ushort?)0), Times.Once());
    }

    /// <summary>
    /// Tests that the guild list is ordered by rank (master, assistant, battle master, normal) and then by name.
    /// </summary>
    [Test]
    public async ValueTask GuildListIsOrderedByRankAsync()
    {
        await this.PrepareRoleAssignScenarioAsync().ConfigureAwait(false);
        var guildId = this._player.GuildStatus!.GuildId;
        await this.GuildServer.CreateGuildMemberAsync(guildId, Guid.NewGuid(), "ZedNormal", GuildPosition.NormalMember, 0).ConfigureAwait(false);
        await this.GuildServer.CreateGuildMemberAsync(guildId, Guid.NewGuid(), "AmyAssistant", GuildPosition.AssistantMaster, 0).ConfigureAwait(false);

        var roleAssignAction = new GuildRoleAssignAction();
        await roleAssignAction.AssignRoleAsync(this._guildMasterPlayer, this._player.SelectedCharacter!.Name, GuildPosition.BattleMaster).ConfigureAwait(false);

        var listAction = new GuildListRequestAction();
        await listAction.RequestGuildListAsync(this._guildMasterPlayer).ConfigureAwait(false);

        Mock.Get(this._guildMasterPlayer.ViewPlugIns.GetPlugIn<IShowGuildListPlugIn>()!)
            .Verify(v => v!.ShowGuildListAsync(
                It.Is<IReadOnlyCollection<GuildListEntry>>(list => list.Select(e => e.PlayerName).SequenceEqual(new[] { "GuildMaster", "AmyAssistant", "Player", "ZedNormal" })),
                It.IsAny<Interfaces.Guild>()), Times.Once());
    }

    private async ValueTask PrepareRoleAssignScenarioAsync()
    {
        await this.RequestGuildAndRespondAsync(true).ConfigureAwait(false);
        await this._guildMasterPlayer.PlayerState.TryAdvanceToAsync(PlayerState.EnteredWorld).ConfigureAwait(false);
        await this._player.PlayerState.TryAdvanceToAsync(PlayerState.EnteredWorld).ConfigureAwait(false);
        if (this._guildMasterPlayer.GameContext is GameContext gameContext)
        {
            gameContext.PlayersByCharacterName.TryAdd(this._guildMasterPlayer.SelectedCharacter!.Name, this._guildMasterPlayer);
            gameContext.PlayersByCharacterName.TryAdd(this._player.SelectedCharacter!.Name, this._player);
        }
    }

    /// <summary>
    /// Tests that a raw C1 E1 packet with role value 32 promotes the member to battle master.
    /// </summary>
    [Test]
    public async ValueTask GuildRoleAssignHandlerPromotesViaRawPacketAsync()
    {
        await this.PrepareRoleAssignScenarioAsync().ConfigureAwait(false);

        var buffer = new byte[GuildRoleAssignRequest.Length];
        GuildRoleAssignRequest request = new(buffer);
        request.Role = GuildMemberRole.BattleMaster;
        request.PlayerName = this._player.SelectedCharacter!.Name;

        var handler = new GuildRoleAssignHandlerPlugIn();
        Assert.That(handler.Key, Is.EqualTo(0xE1));
        await handler.HandlePacketAsync(this._guildMasterPlayer, buffer).ConfigureAwait(false);

        Assert.That(this._player.GuildStatus!.Position, Is.EqualTo(GuildPosition.BattleMaster));
    }

    /// <summary>
    /// Tests that a space-padded name field still resolves the target (role value 64).
    /// </summary>
    [Test]
    public async ValueTask GuildRoleAssignHandlerSpacePaddedNameAsync()
    {
        await this.PrepareRoleAssignScenarioAsync().ConfigureAwait(false);

        var buffer = new byte[GuildRoleAssignRequest.Length];
        buffer[0] = 0xC1;
        buffer[1] = (byte)buffer.Length;
        buffer[2] = GuildRoleAssignRequest.Code;
        buffer[3] = 1; // Type byte, ignored by the handler.
        buffer[4] = (byte)GuildMemberRole.AssistantMaster;
        var nameBytes = System.Text.Encoding.UTF8.GetBytes(this._player.SelectedCharacter!.Name);
        Array.Copy(nameBytes, 0, buffer, 5, nameBytes.Length);
        for (int i = 5 + nameBytes.Length; i < buffer.Length; i++)
        {
            buffer[i] = 0x20; // space padding instead of zero padding.
        }

        var handler = new GuildRoleAssignHandlerPlugIn();
        await handler.HandlePacketAsync(this._guildMasterPlayer, buffer).ConfigureAwait(false);

        Assert.That(this._player.GuildStatus!.Position, Is.EqualTo(GuildPosition.AssistantMaster));
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