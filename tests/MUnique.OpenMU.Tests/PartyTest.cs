// <copyright file="PartyTest.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.Tests;

using Microsoft.Extensions.Logging.Abstractions;
using Moq;
using MUnique.OpenMU.DataModel.Configuration;
using MUnique.OpenMU.GameLogic;
using MUnique.OpenMU.GameLogic.PlayerActions.Party;
using MUnique.OpenMU.GameLogic.Views.Party;
using MUnique.OpenMU.Persistence.InMemory;
using MUnique.OpenMU.PlugIns;

/// <summary>
/// Tests the party functions.
/// </summary>
[TestFixture]
public class PartyTest
{
    private readonly PartyKickAction _kickAction = new ();

    /// <summary>
    /// Tests if an added party member gets added to the party list.
    /// </summary>
    [Test]
    public async ValueTask PartyMemberAddAsync()
    {
        var partyMember = await this.CreatePartyMemberAsync().ConfigureAwait(false);
        var party = new Party(5, new NullLogger<Party>());
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

        await this._kickAction.KickPlayerAsync(partyMember2, (byte)party.PartyList.IndexOf(partyMember3)).ConfigureAwait(false);
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

        await this._kickAction.KickPlayerAsync((Player)partyMember2, (byte)party.PartyList.IndexOf(partyMember2)).ConfigureAwait(false);
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

        await this._kickAction.KickPlayerAsync(partyMaster, (byte)party.PartyList.IndexOf(partyMember)).ConfigureAwait(false);
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

        await this._kickAction.KickPlayerAsync((Player)partyMaster, (byte)party.PartyList.IndexOf(partyMaster)).ConfigureAwait(false);
        Assert.That(partyMember.Party, Is.Null);
        Assert.That(party.PartyList, Is.Null.Or.Empty);
    }

    /// <summary>
    /// Tests if the party automatically closes when one player is left.
    /// </summary>
    [Test]
    public async ValueTask PartyAutoCloseAsync()
    {
        var partyMember1 = await this.CreatePartyMemberAsync().ConfigureAwait(false);
        var party = new Party(5, new NullLogger<Party>());
        await party.AddAsync(partyMember1).ConfigureAwait(false);
        var partyMember1Index = (byte)(party.PartyList.Count - 1);
        var partyMember2 = await this.CreatePartyMemberAsync().ConfigureAwait(false);
        await party.AddAsync(partyMember2).ConfigureAwait(false);
        var partyMember2Index = (byte)(party.PartyList.Count - 1);

        await this._kickAction.KickPlayerAsync(partyMember1, partyMember2Index).ConfigureAwait(false);
        Assert.That(partyMember1.Party, Is.Null);
        Assert.That(partyMember2.Party, Is.Null);
        Assert.That(party.PartyList, Is.Null.Or.Empty);

        Mock.Get(partyMember1.ViewPlugIns.GetPlugIn<IPartyMemberRemovedPlugIn>()).Verify(v => v!.PartyMemberRemovedAsync(partyMember1Index), Times.Once);
        Mock.Get(partyMember2.ViewPlugIns.GetPlugIn<IPartyMemberRemovedPlugIn>()).Verify(v => v!.PartyMemberRemovedAsync(partyMember2Index), Times.Once);
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

        Mock.Get(toRequest.ViewPlugIns.GetPlugIn<IShowPartyRequestPlugIn>()).Verify(v => v!.ShowPartyRequestAsync(player), Times.Once);
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
        Mock.Get(player.ViewPlugIns.GetPlugIn<IUpdatePartyListPlugIn>()).Verify(v => v!.UpdatePartyListAsync(), Times.AtLeastOnce);
        Mock.Get(requester.ViewPlugIns.GetPlugIn<IUpdatePartyListPlugIn>()).Verify(v => v!.UpdatePartyListAsync(), Times.AtLeastOnce);
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
        Mock.Get(player.ViewPlugIns.GetPlugIn<IShowPartyRequestPlugIn>()).Verify(v => v!.ShowPartyRequestAsync(requester), Times.Never);
    }

    private async ValueTask<Player> CreatePartyMemberAsync()
    {
        var result = await TestHelper.CreatePlayerAsync(this.GetGameContext()).ConfigureAwait(false);
        await result.PlayerState.TryAdvanceToAsync(PlayerState.EnteredWorld).ConfigureAwait(false);
        return result;
    }

    private async ValueTask<Party> CreatePartyWithMembersAsync(int numberOfMembers)
    {
        var party = new Party(5, new NullLogger<Party>());
        for (ushort i = 0; i < numberOfMembers; i++)
        {
            var partyMember = await this.CreatePartyMemberAsync().ConfigureAwait(false);
            await party.AddAsync(partyMember).ConfigureAwait(false);
        }

        return party;
    }

    private IGameContext GetGameContext()
    {
        var contextProvider = new InMemoryPersistenceContextProvider();
        var gameConfig = contextProvider.CreateNewContext().CreateNew<GameConfiguration>();
        gameConfig.Maps.Add(contextProvider.CreateNewContext().CreateNew<GameMapDefinition>());

        var mapInitializer = new MapInitializer(gameConfig, new NullLogger<MapInitializer>(), NullDropGenerator.Instance, null);
        var gameContext = new GameContext(gameConfig, contextProvider, mapInitializer, new NullLoggerFactory(), new PlugInManager(new List<PlugInConfiguration>(), new NullLoggerFactory(), null, null), NullDropGenerator.Instance, new ConfigurationChangeMediator());
        gameContext.Configuration.MaximumPartySize = 5;
        mapInitializer.PlugInManager = gameContext.PlugInManager;
        mapInitializer.PathFinderPool = gameContext.PathFinderPool;

        return gameContext;
    }
}