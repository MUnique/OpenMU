// <copyright file="PartyTest.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.Tests
{
    using Moq;
    using MUnique.OpenMU.DataModel.Configuration;
    using MUnique.OpenMU.GameLogic;
    using MUnique.OpenMU.GameLogic.PlayerActions.Party;
    using MUnique.OpenMU.GameLogic.Views.Party;
    using MUnique.OpenMU.Persistence.InMemory;
    using NUnit.Framework;

    /// <summary>
    /// Tests the party functions.
    /// </summary>
    [TestFixture]
    public class PartyTest
    {
        /// <summary>
        /// Tests if an added party member gets added to the party list.
        /// </summary>
        [Test]
        public void PartyMemberAdd()
        {
            var party = new Party(5);
            var partyMember = this.CreatePartyMember();
            party.Add(partyMember);
            Assert.That(party.PartyList, Contains.Item(partyMember));
        }

        /// <summary>
        /// Tests a kick request by a non-party-master for another player, which should fail.
        /// </summary>
        [Test]
        public void PartyMemberKickFailByNonMaster()
        {
            var party = this.CreatePartyWithMembers(3);
            var partyMember2 = party.PartyList[1];
            var partyMember3 = party.PartyList[2];
            party.KickPlayer(partyMember2, (byte)party.PartyList.IndexOf(partyMember3));
            Assert.That(party.PartyList, Contains.Item(partyMember3));
        }

        /// <summary>
        /// Tests if the player is kicking himself works, even if the player is no party master.
        /// </summary>
        [Test]
        public void PartyMemberKickHimself()
        {
            var party = this.CreatePartyWithMembers(3);
            var partyMember2 = party.PartyList[1];

            party.KickPlayer(partyMember2, (byte)party.PartyList.IndexOf(partyMember2));
            Assert.That(party.PartyList, Is.Not.Contains(partyMember2));
            Assert.That(party.PartyList, Has.Count.EqualTo(2));
        }

        /// <summary>
        /// Tests if another player can be kicked by the party master.
        /// </summary>
        [Test]
        public void PartyMemberKickByMaster()
        {
            var party = this.CreatePartyWithMembers(3);
            var partyMaster = party.PartyList[0] as Player;
            var partyMember = party.PartyList[1] as Player;

            party.KickPlayer(partyMaster, (byte)party.PartyList.IndexOf(partyMember));
            Assert.That(party.PartyList, Is.Not.Contains(partyMember));
            Assert.That(party.PartyList, Has.Count.EqualTo(2));
        }

        /// <summary>
        /// Tests if the party disbands when the master kicks himself.
        /// </summary>
        [Test]
        public void PartyMasterKicksHimself()
        {
            var party = this.CreatePartyWithMembers(3);
            var partyMaster = party.PartyList[0];
            var partyMember = party.PartyList[1];

            party.KickPlayer(partyMaster, (byte)party.PartyList.IndexOf(partyMaster));
            Assert.That(partyMember.Party, Is.Null);
            Assert.That(party.PartyList, Is.Null.Or.Empty);
        }

        /// <summary>
        /// Tests if the party automatically closes when one player is left.
        /// </summary>
        [Test]
        public void PartyAutoClose()
        {
            var party = new Party(5);
            var partyMember1 = this.CreatePartyMember();
            party.Add(partyMember1);
            var partyMember1Index = (byte)(party.PartyList.Count - 1);
            var partyMember2 = this.CreatePartyMember();
            party.Add(partyMember2);
            var partyMember2Index = (byte)(party.PartyList.Count - 1);

            party.KickPlayer(partyMember1, partyMember2Index);
            Assert.That(partyMember1.Party, Is.Null);
            Assert.That(partyMember2.Party, Is.Null);
            Assert.That(party.PartyList, Is.Null.Or.Empty);

            Mock.Get(partyMember1.ViewPlugIns.GetPlugIn<IPartyMemberRemovedPlugIn>()).Verify(v => v.PartyMemberRemoved(partyMember1Index), Times.Once);
            Mock.Get(partyMember2.ViewPlugIns.GetPlugIn<IPartyMemberRemovedPlugIn>()).Verify(v => v.PartyMemberRemoved(partyMember2Index), Times.Once);
        }

        /// <summary>
        /// Tests the adding of party members.
        /// </summary>
        [Test]
        public void PartyHandlerAdd()
        {
            var handler = new PartyRequestAction();
            var player = this.CreatePartyMember();
            var toRequest = this.CreatePartyMember();
            player.Observers.Add(toRequest);

            handler.HandlePartyRequest(player, toRequest);

            Mock.Get(toRequest.ViewPlugIns.GetPlugIn<IShowPartyRequestPlugIn>()).Verify(v => v.ShowPartyRequest(player), Times.Once);
            Assert.That(toRequest.LastPartyRequester, Is.SameAs(player));
        }

        /// <summary>
        /// Tests if the party gets created after the requested player responses with accepting the party.
        /// </summary>
        [Test]
        public void PartyResponseAcceptNewParty()
        {
            var handler = new PartyResponseAction();
            var player = this.CreatePartyMember();
            var requester = this.CreatePartyMember();
            player.LastPartyRequester = requester;
            player.PlayerState.TryAdvanceTo(PlayerState.PartyRequest);

            handler.HandleResponse(player, true);
            Assert.That(player.Party, Is.Not.Null);
            Assert.That(player.Party.PartyMaster, Is.SameAs(requester));
            Assert.That(player.LastPartyRequester, Is.Null);
            Assert.That(player.Party.PartyList, Contains.Item(player));
            Mock.Get(player.ViewPlugIns.GetPlugIn<IUpdatePartyListPlugIn>()).Verify(v => v.UpdatePartyList(), Times.AtLeastOnce);
            Mock.Get(requester.ViewPlugIns.GetPlugIn<IUpdatePartyListPlugIn>()).Verify(v => v.UpdatePartyList(), Times.AtLeastOnce);
        }

        /// <summary>
        /// Tests if a request to a player which is already in a party (however this happened...)
        /// does not cause the player to be added to the other party.
        /// </summary>
        [Test]
        public void PartyResponseAcceptExistingParty()
        {
            var handler = new PartyResponseAction();

            // first put the player in a party with another player
            var player = this.CreatePartyMember();
            player.LastPartyRequester = this.CreatePartyMember();
            player.PlayerState.TryAdvanceTo(PlayerState.PartyRequest);
            handler.HandleResponse(player, true);

            // now another player will try to request party from the player, which should fail
            var requester = this.CreatePartyMember();
            player.LastPartyRequester = requester;
            handler.HandleResponse(player, true);
            Assert.That(player.Party.PartyList, Is.Not.Contains(requester));
            Assert.That(player.LastPartyRequester, Is.Null);
            Assert.That(requester.Party, Is.Null);
            Mock.Get(player.ViewPlugIns.GetPlugIn<IShowPartyRequestPlugIn>()).Verify(v => v.ShowPartyRequest(requester), Times.Never);
        }

        private Player CreatePartyMember()
        {
            var result = TestHelper.GetPlayer(this.GetGameServer());
            result.PlayerState.TryAdvanceTo(PlayerState.EnteredWorld);
            return result;
        }

        private Party CreatePartyWithMembers(int numberOfMembers)
        {
            var party = new Party(5);
            for (ushort i = 0; i < numberOfMembers; i++)
            {
                var partyMember = this.CreatePartyMember();
                party.Add(partyMember);
            }

            return party;
        }

        private IGameContext GetGameServer()
        {
            var contextProvider = new InMemoryPersistenceContextProvider();
            var gameConfig = contextProvider.CreateNewContext().CreateNew<GameConfiguration>();
            gameConfig.Maps.Add(contextProvider.CreateNewContext().CreateNew<GameMapDefinition>());

            var mapInitializer = new MapInitializer(gameConfig);
            var gameServer = new GameContext(gameConfig, contextProvider, mapInitializer);
            gameServer.Configuration.MaximumPartySize = 5;

            return gameServer;
        }
    }
}
