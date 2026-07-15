// <copyright file="PartyTest.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.Tests;

using Microsoft.Extensions.Logging.Abstractions;
using Moq;
using MUnique.OpenMU.DataModel.Configuration;
using MUnique.OpenMU.GameLogic;
using MUnique.OpenMU.GameLogic.MuHelper;
using MUnique.OpenMU.GameLogic.PlayerActions.Party;
using MUnique.OpenMU.GameLogic.Views.Party;
using MUnique.OpenMU.GameServer;
using MUnique.OpenMU.Interfaces;
using MUnique.OpenMU.Pathfinding;
using MUnique.OpenMU.Persistence.InMemory;
using MUnique.OpenMU.PlugIns;

/// <summary>
/// Tests the party functions.
/// </summary>
[TestFixture]
public class PartyTest
{
    private readonly PartyKickAction _kickAction = new();

    /// <summary>
    /// Tests if an added party member gets added to the party list.
    /// </summary>
    [Test]
    public async ValueTask PartyMemberAddAsync()
    {
        var partyMember = await this.CreatePartyMemberAsync().ConfigureAwait(false);
        var party = new Party(new PartyManager(5, new NullLogger<Party>()), 5, new NullLogger<Party>());
        await party.AddAsync(partyMember).ConfigureAwait(false);

        Assert.That(party.PartyList, Contains.Item(partyMember));
    }

    /// <summary>
    /// Tests a kick request by a non-party-master for another player, which should fail.
    /// </summary>
    [Test]
    public async ValueTask PartyMemberKickFailByNonMasterAsync()
    {
        var party = await this.CreatePartyWithMembersAsync(3).ConfigureAwait(false);
        var partyMember2 = (Player)party.PartyList[1];
        var partyMember3 = party.PartyList[2];

        await this._kickAction.KickPlayerAsync(partyMember2, GetPartyMemberIndex(party, partyMember3)).ConfigureAwait(false);
        Assert.That(party.PartyList, Contains.Item(partyMember3));
    }

    /// <summary>
    /// Tests if the player is kicking himself works, even if the player is no party master.
    /// </summary>
    [Test]
    public async ValueTask PartyMemberKickHimselfAsync()
    {
        var party = await this.CreatePartyWithMembersAsync(3).ConfigureAwait(false);
        var partyMember2 = party.PartyList[1];

        await this._kickAction.KickPlayerAsync((Player)partyMember2, GetPartyMemberIndex(party, partyMember2)).ConfigureAwait(false);
        Assert.That(party.PartyList, Is.Not.Contains(partyMember2));
        Assert.That(party.PartyList, Has.Count.EqualTo(2));
    }

    /// <summary>
    /// Tests if another player can be kicked by the party master.
    /// </summary>
    [Test]
    public async ValueTask PartyMemberKickByMasterAsync()
    {
        var party = await this.CreatePartyWithMembersAsync(3).ConfigureAwait(false);
        var partyMaster = (Player)party.PartyList[0];
        var partyMember = (Player)party.PartyList[1];

        await this._kickAction.KickPlayerAsync(partyMaster, GetPartyMemberIndex(party, partyMember)).ConfigureAwait(false);
        Assert.That(party.PartyList, Is.Not.Contains(partyMember));
        Assert.That(party.PartyList, Has.Count.EqualTo(2));
    }

    /// <summary>
    /// Tests if the party disbands when the master kicks himself.
    /// </summary>
    [Test]
    public async ValueTask PartyMasterKicksHimselfAsync()
    {
        var party = await this.CreatePartyWithMembersAsync(3).ConfigureAwait(false);
        var partyMaster = party.PartyList[0];
        var partyMember = party.PartyList[1];

        await this._kickAction.KickPlayerAsync((Player)partyMaster, GetPartyMemberIndex(party, partyMaster)).ConfigureAwait(false);

        // Master leaves the party; the remaining 2 members stay.
        Assert.That(partyMaster.Party, Is.Null);
        Assert.That(party.PartyList, Does.Not.Contain(partyMaster));
        Assert.That(partyMember.Party, Is.SameAs(party));
        Assert.That(party.PartyList, Has.Count.EqualTo(2));

        // A new party master should have been assigned.
        Assert.That(party.PartyMaster, Is.Not.Null);
        Assert.That(party.PartyMaster, Is.SameAs(partyMember));
    }

    /// <summary>
    /// Tests if the party master role stays the same when a non-master is kicked.
    /// </summary>
    [Test]
    public async ValueTask PartyMemberKickByMasterMasterRemainsMasterAsync()
    {
        var party = await this.CreatePartyWithMembersAsync(3).ConfigureAwait(false);
        var partyMaster = party.PartyList[0];
        var partyMember = party.PartyList[1];

        await this._kickAction.KickPlayerAsync((Player)partyMaster, GetPartyMemberIndex(party, partyMember)).ConfigureAwait(false);
        Assert.That(party.PartyMaster, Is.SameAs(partyMaster));
        Assert.That(party.PartyList, Has.Count.EqualTo(2));
    }

    /// <summary>
    /// Tests if the first remaining member becomes the new party master when the master leaves.
    /// </summary>
    [Test]
    public async ValueTask PartyMasterLeavesAndFirstMemberBecomesNewMasterAsync()
    {
        var party = await this.CreatePartyWithMembersAsync(4).ConfigureAwait(false);
        var partyMaster = party.PartyList[0];
        var firstRemainingMember = party.PartyList[1];

        await this._kickAction.KickPlayerAsync((Player)partyMaster, GetPartyMemberIndex(party, partyMaster)).ConfigureAwait(false);

        Assert.That(partyMaster.Party, Is.Null);
        Assert.That(party.PartyList, Does.Not.Contain(partyMaster));
        Assert.That(party.PartyList, Has.Count.EqualTo(3));
        Assert.That(party.PartyList[0], Is.SameAs(firstRemainingMember));
        Assert.That(party.PartyMaster, Is.SameAs(firstRemainingMember));
    }

    /// <summary>
    /// Tests if the party automatically closes when one player is left.
    /// </summary>
    [Test]
    public async ValueTask PartyAutoCloseAsync()
    {
        var partyMember1 = await this.CreatePartyMemberAsync().ConfigureAwait(false);
        var party = new Party(new PartyManager(5, new NullLogger<Party>()), 5, new NullLogger<Party>());
        await party.AddAsync(partyMember1).ConfigureAwait(false);
        var partyMember1Index = (byte)(party.PartyList.Count - 1);
        var partyMember2 = await this.CreatePartyMemberAsync().ConfigureAwait(false);
        await party.AddAsync(partyMember2).ConfigureAwait(false);
        var partyMember2Index = (byte)(party.PartyList.Count - 1);

        await this._kickAction.KickPlayerAsync(partyMember1, partyMember2Index).ConfigureAwait(false);
        Assert.That(partyMember1.Party, Is.Null);
        Assert.That(partyMember2.Party, Is.Null);
        Assert.That(party.PartyList, Is.Null.Or.Empty);

        Mock.Get(partyMember1.ViewPlugIns.GetPlugIn<IPartyMemberRemovedPlugIn>()!).Verify(v => v!.PartyMemberRemovedAsync(partyMember1Index), Times.Once);
        Mock.Get(partyMember2.ViewPlugIns.GetPlugIn<IPartyMemberRemovedPlugIn>()!).Verify(v => v!.PartyMemberRemovedAsync(partyMember2Index), Times.Once);
    }

    /// <summary>
    /// Tests the adding of party members.
    /// </summary>
    [Test]
    public async ValueTask PartyHandlerAddAsync()
    {
        var handler = new PartyRequestAction();
        var player = await this.CreatePartyMemberAsync().ConfigureAwait(false);
        var toRequest = await this.CreatePartyMemberAsync().ConfigureAwait(false);
        player.Observers.Add(toRequest);

        await handler.HandlePartyRequestAsync(player, toRequest).ConfigureAwait(false);

        Mock.Get(toRequest.ViewPlugIns.GetPlugIn<IShowPartyRequestPlugIn>()!).Verify(v => v!.ShowPartyRequestAsync(player), Times.Once);
        Assert.That(toRequest.LastPartyRequester, Is.SameAs(player));
    }

    /// <summary>
    /// Tests if the party gets created after the requested player responses with accepting the party.
    /// </summary>
    [Test]
    public async ValueTask PartyResponseAcceptNewPartyAsync()
    {
        var handler = new PartyResponseAction();
        var player = await this.CreatePartyMemberAsync().ConfigureAwait(false);
        var requester = await this.CreatePartyMemberAsync().ConfigureAwait(false);
        player.LastPartyRequester = requester;
        await player.PlayerState.TryAdvanceToAsync(PlayerState.PartyRequest).ConfigureAwait(false);

        await handler.HandleResponseAsync(player, true).ConfigureAwait(false);
        Assert.That(player.Party, Is.Not.Null);
        Assert.That(player.Party!.PartyMaster, Is.SameAs(requester));
        Assert.That(player.LastPartyRequester, Is.Null);
        Assert.That(player.Party.PartyList, Contains.Item(player));
        Mock.Get(player.ViewPlugIns.GetPlugIn<IUpdatePartyListPlugIn>()!).Verify(v => v!.UpdatePartyListAsync(), Times.AtLeastOnce);
        Mock.Get(requester.ViewPlugIns.GetPlugIn<IUpdatePartyListPlugIn>()!).Verify(v => v!.UpdatePartyListAsync(), Times.AtLeastOnce);
    }

    /// <summary>
    /// Tests if a request to a player which is already in a party (however this happened...)
    /// does not cause the player to be added to the other party.
    /// </summary>
    [Test]
    public async ValueTask PartyResponseAcceptExistingPartyAsync()
    {
        var handler = new PartyResponseAction();

        // first put the player in a party with another player
        var player = await this.CreatePartyMemberAsync().ConfigureAwait(false);
        player.LastPartyRequester = await this.CreatePartyMemberAsync().ConfigureAwait(false);
        await player.PlayerState.TryAdvanceToAsync(PlayerState.PartyRequest).ConfigureAwait(false);
        await handler.HandleResponseAsync(player, true).ConfigureAwait(false);

        // now another player will try to request party from the player, which should fail
        var requester = await this.CreatePartyMemberAsync().ConfigureAwait(false);
        player.LastPartyRequester = requester;
        await handler.HandleResponseAsync(player, true).ConfigureAwait(false);
        Assert.That(player.Party!.PartyList, Is.Not.Contains(requester));
        Assert.That(player.LastPartyRequester, Is.Null);
        Assert.That(requester.Party, Is.Null);
        Mock.Get(player.ViewPlugIns.GetPlugIn<IShowPartyRequestPlugIn>()!).Verify(v => v!.ShowPartyRequestAsync(requester), Times.Never);
    }

    /// <summary>
    /// Tests if a party request is auto-accepted when <see cref="IMuHelperSettings.AutoAcceptFriend"/> is true
    /// and the requester is a friend.
    /// </summary>
    [Test]
    public async ValueTask PartyRequestAutoAcceptByFriendAsync()
    {
        var friendServer = new Mock<IFriendServer>();
        friendServer.Setup(f => f.IsFriendAsync(It.IsAny<string>(), It.IsAny<string>())).ReturnsAsync(true);
        var gameContext = PartyTest.CreateGameServerContext(friendServer.Object);
        var player = await PlayerTestHelper.CreatePlayerAsync(gameContext).ConfigureAwait(false);
        var toRequest = await PlayerTestHelper.CreatePlayerAsync(gameContext).ConfigureAwait(false);
        player.SelectedCharacter!.Name = "Requester";
        toRequest.SelectedCharacter!.Name = "Receiver";
        player.Observers.Add(toRequest);

        var settingsMock = new Mock<IMuHelperSettings>();
        settingsMock.Setup(s => s.AutoAcceptFriend).Returns(true);
        toRequest.MuHelperSettings = settingsMock.Object;

        var handler = new PartyRequestAction();
        await handler.HandlePartyRequestAsync(player, toRequest).ConfigureAwait(false);

        Assert.That(player.Party, Is.Not.Null);
        Assert.That(toRequest.Party, Is.SameAs(player.Party));
        Mock.Get(toRequest.ViewPlugIns.GetPlugIn<IShowPartyRequestPlugIn>()!).Verify(v => v!.ShowPartyRequestAsync(player), Times.Never);
    }

    /// <summary>
    /// Tests if a party request still shows the dialog when <see cref="IMuHelperSettings.AutoAcceptFriend"/>
    /// is true but the requester is not a friend.
    /// </summary>
    [Test]
    public async ValueTask PartyRequestAutoAcceptByFriendNotFriendAsync()
    {
        var friendServer = new Mock<IFriendServer>();
        friendServer.Setup(f => f.IsFriendAsync(It.IsAny<string>(), It.IsAny<string>())).ReturnsAsync(false);
        var gameContext = PartyTest.CreateGameServerContext(friendServer.Object);
        var player = await PlayerTestHelper.CreatePlayerAsync(gameContext).ConfigureAwait(false);
        var toRequest = await PlayerTestHelper.CreatePlayerAsync(gameContext).ConfigureAwait(false);
        player.SelectedCharacter!.Name = "Requester";
        toRequest.SelectedCharacter!.Name = "Receiver";
        player.Observers.Add(toRequest);

        var settingsMock = new Mock<IMuHelperSettings>();
        settingsMock.Setup(s => s.AutoAcceptFriend).Returns(true);
        toRequest.MuHelperSettings = settingsMock.Object;

        var handler = new PartyRequestAction();
        await handler.HandlePartyRequestAsync(player, toRequest).ConfigureAwait(false);

        Assert.That(player.Party, Is.Null);
        Mock.Get(toRequest.ViewPlugIns.GetPlugIn<IShowPartyRequestPlugIn>()!).Verify(v => v!.ShowPartyRequestAsync(player), Times.Once);
    }

    /// <summary>
    /// Tests if a party request is auto-accepted when <see cref="IMuHelperSettings.AutoAcceptGuild"/> is true
    /// and both players are members of the same guild.
    /// </summary>
    [Test]
    public async ValueTask PartyRequestAutoAcceptByGuildAsync()
    {
        var gameContext = GameContextTestHelper.CreateGameContext();
        var player = await PlayerTestHelper.CreatePlayerAsync(gameContext).ConfigureAwait(false);
        var toRequest = await PlayerTestHelper.CreatePlayerAsync(gameContext).ConfigureAwait(false);
        player.Observers.Add(toRequest);

        var guildId = 1u;
        player.GuildStatus = new GuildMemberStatus(guildId, GuildPosition.GuildMaster);
        toRequest.GuildStatus = new GuildMemberStatus(guildId, GuildPosition.NormalMember);

        var settingsMock = new Mock<IMuHelperSettings>();
        settingsMock.Setup(s => s.AutoAcceptGuild).Returns(true);
        toRequest.MuHelperSettings = settingsMock.Object;

        var handler = new PartyRequestAction();
        await handler.HandlePartyRequestAsync(player, toRequest).ConfigureAwait(false);

        Assert.That(player.Party, Is.Not.Null);
        Assert.That(toRequest.Party, Is.SameAs(player.Party));
        Mock.Get(toRequest.ViewPlugIns.GetPlugIn<IShowPartyRequestPlugIn>()!).Verify(v => v!.ShowPartyRequestAsync(player), Times.Never);
    }

    /// <summary>
    /// Tests if a party request still shows the dialog when <see cref="IMuHelperSettings.AutoAcceptGuild"/>
    /// is true but the players are in different guilds.
    /// </summary>
    [Test]
    public async ValueTask PartyRequestAutoAcceptByGuildDifferentGuildAsync()
    {
        var gameContext = GameContextTestHelper.CreateGameContext();
        var player = await PlayerTestHelper.CreatePlayerAsync(gameContext).ConfigureAwait(false);
        var toRequest = await PlayerTestHelper.CreatePlayerAsync(gameContext).ConfigureAwait(false);
        player.Observers.Add(toRequest);

        player.GuildStatus = new GuildMemberStatus(1u, GuildPosition.GuildMaster);
        toRequest.GuildStatus = new GuildMemberStatus(2u, GuildPosition.NormalMember);

        var settingsMock = new Mock<IMuHelperSettings>();
        settingsMock.Setup(s => s.AutoAcceptGuild).Returns(true);
        toRequest.MuHelperSettings = settingsMock.Object;

        var handler = new PartyRequestAction();
        await handler.HandlePartyRequestAsync(player, toRequest).ConfigureAwait(false);

        Assert.That(player.Party, Is.Null);
        Mock.Get(toRequest.ViewPlugIns.GetPlugIn<IShowPartyRequestPlugIn>()!).Verify(v => v!.ShowPartyRequestAsync(player), Times.Once);
    }

    /// <summary>
    /// Tests that a party request is not auto-accepted when no relevant flags are set.
    /// </summary>
    [Test]
    public async ValueTask PartyRequestAutoAcceptNoFlagsAsync()
    {
        var friendServer = new Mock<IFriendServer>();
        var gameContext = PartyTest.CreateGameServerContext(friendServer.Object);
        var player = await PlayerTestHelper.CreatePlayerAsync(gameContext).ConfigureAwait(false);
        var toRequest = await PlayerTestHelper.CreatePlayerAsync(gameContext).ConfigureAwait(false);
        player.SelectedCharacter!.Name = "Requester";
        toRequest.SelectedCharacter!.Name = "Receiver";
        player.Observers.Add(toRequest);

        var settingsMock = new Mock<IMuHelperSettings>();
        toRequest.MuHelperSettings = settingsMock.Object;

        var handler = new PartyRequestAction();
        await handler.HandlePartyRequestAsync(player, toRequest).ConfigureAwait(false);

        Assert.That(player.Party, Is.Null);
        Mock.Get(toRequest.ViewPlugIns.GetPlugIn<IShowPartyRequestPlugIn>()!).Verify(v => v!.ShowPartyRequestAsync(player), Times.Once);
    }

    /// <summary>
    /// Tests that a party request is not auto-accepted when MuHelperSettings is null.
    /// </summary>
    [Test]
    public async ValueTask PartyRequestAutoAcceptNoSettingsAsync()
    {
        var friendServer = new Mock<IFriendServer>();
        var gameContext = PartyTest.CreateGameServerContext(friendServer.Object);
        var player = await PlayerTestHelper.CreatePlayerAsync(gameContext).ConfigureAwait(false);
        var toRequest = await PlayerTestHelper.CreatePlayerAsync(gameContext).ConfigureAwait(false);
        player.Observers.Add(toRequest);

        var handler = new PartyRequestAction();
        await handler.HandlePartyRequestAsync(player, toRequest).ConfigureAwait(false);

        Assert.That(player.Party, Is.Null);
        Mock.Get(toRequest.ViewPlugIns.GetPlugIn<IShowPartyRequestPlugIn>()!).Verify(v => v!.ShowPartyRequestAsync(player), Times.Once);
    }

    private static IGameServerContext CreateGameServerContext(IFriendServer friendServer)
    {
        var contextProvider = new InMemoryPersistenceContextProvider();
        var context = contextProvider.CreateNewContext();
        var gameConfiguration = context.CreateNew<MUnique.OpenMU.Persistence.BasicModel.GameConfiguration>();
        gameConfiguration.MaximumPartySize = 5;
        gameConfiguration.RecoveryInterval = int.MaxValue;
        gameConfiguration.MaximumInventoryMoney = int.MaxValue;
        var mapDef = context.CreateNew<MUnique.OpenMU.Persistence.BasicModel.GameMapDefinition>();
        mapDef.Number = 0;
        mapDef.TerrainData = new byte[ushort.MaxValue + 3];
        gameConfiguration.Maps.Add(mapDef);

        var mapInitializer = new MapInitializer(gameConfiguration, new NullLogger<MapInitializer>(), NullDropGenerator.Instance, null);

        var gameServer = new GameServerContext(
            new GameServerDefinition
            {
                GameConfiguration = gameConfiguration,
                ServerConfiguration = new GameServerConfiguration(),
            },
            new Mock<IGuildServer>().Object,
            new Mock<IEventPublisher>().Object,
            new Mock<ILoginServer>().Object,
            friendServer,
            new InMemoryPersistenceContextProvider(),
            mapInitializer,
            new NullLoggerFactory(),
            new PlugInManager(new List<PlugInConfiguration>(), new NullLoggerFactory(), null, null),
            NullDropGenerator.Instance,
            new ConfigurationChangeMediator());
        mapInitializer.PlugInManager = gameServer.PlugInManager;
        mapInitializer.PathFinderPool = gameServer.PathFinderPool;
        return gameServer;
    }

    private async ValueTask<Player> CreatePartyMemberAsync()
    {
        var result = await PlayerTestHelper.CreatePlayerAsync(GameContextTestHelper.CreateGameContext()).ConfigureAwait(false);
        await result.PlayerState.TryAdvanceToAsync(PlayerState.EnteredWorld).ConfigureAwait(false);
        return result;
    }

    private static byte GetPartyMemberIndex(Party party, IPartyMember member)
    {
        for (byte index = 0; index < party.PartyList.Count; index++)
        {
            if (party.PartyList[index] == member)
            {
                return index;
            }
        }

        throw new ArgumentException("The member is not part of the party.", nameof(member));
    }

    private async ValueTask<Party> CreatePartyWithMembersAsync(int numberOfMembers)
    {
        var party = new Party(new PartyManager(5, new NullLogger<Party>()), 5, new NullLogger<Party>());
        for (ushort i = 0; i < numberOfMembers; i++)
        {
            var partyMember = await this.CreatePartyMemberAsync().ConfigureAwait(false);
            await party.AddAsync(partyMember).ConfigureAwait(false);
        }

        return party;
    }
}
