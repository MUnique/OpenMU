// <copyright file="PartyTest.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.Tests
{
    using MUnique.OpenMU.DataModel.Configuration;
    using MUnique.OpenMU.GameLogic;
    using MUnique.OpenMU.GameLogic.PlayerActions.Party;
    using NUnit.Framework;
    using Rhino.Mocks;

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
            var partyMember = CreatePartyMember(1);
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
            var partyMember1 = CreatePartyMember(1);
            party.Add(partyMember1);
            var partyMember2 = CreatePartyMember(2);
            party.Add(partyMember2);
            partyMember1.PlayerView.PartyView.Expect(v => v.PartyClosed());
            partyMember2.PlayerView.PartyView.Expect(v => v.PartyClosed());
            var partyMember2Index = (byte)(party.PartyList.Count - 1);
            party.KickPlayer(partyMember1, partyMember2Index);
            Assert.That(partyMember1.Party, Is.Null);
            Assert.That(partyMember2.Party, Is.Null);
            Assert.That(party.PartyList, Is.Null.Or.Empty);
            partyMember1.PlayerView.PartyView.VerifyAllExpectations();
            partyMember2.PlayerView.PartyView.VerifyAllExpectations();
        }

        /// <summary>
        /// Tests the adding of party members.
        /// </summary>
        [Test]
        public void PartyHandlerAdd()
        {
            var handler = new PartyRequestAction();
            var player = CreatePartyMember(1);
            var toRequest = CreatePartyMember(2);
            player.Observers.Add(toRequest);
            toRequest.PlayerView.PartyView.Expect(v => v.ShowPartyRequest(player));

            handler.HandlePartyRequest(player, toRequest);

            toRequest.PlayerView.PartyView.VerifyAllExpectations();
            Assert.That(toRequest.LastPartyRequester, Is.SameAs(player));
        }

        /// <summary>
        /// Tests if the party gets created after the requested player responses with accepting the party.
        /// </summary>
        [Test]
        public void PartyResponseAcceptNewParty()
        {
            var handler = new PartyResponseAction(this.GetGameServer());
            var player = CreatePartyMember(1);
            var requester = CreatePartyMember(2);
            player.LastPartyRequester = requester;
            player.PlayerState.TryAdvanceTo(PlayerState.PartyRequest);
            player.PlayerView.PartyView.Expect(v => v.UpdatePartyList());
            requester.PlayerView.PartyView.Expect(v => v.UpdatePartyList());
            handler.HandleResponse(player, true);
            Assert.That(player.Party, Is.Not.Null);
            Assert.That(player.Party.PartyMaster, Is.SameAs(requester));
            Assert.That(player.LastPartyRequester, Is.Null);
            Assert.That(player.Party.PartyList, Contains.Item(player));
            player.PlayerView.PartyView.VerifyAllExpectations();
            requester.PlayerView.PartyView.VerifyAllExpectations();
        }

        /// <summary>
        /// Tests if a request to a player which is already in a party (however this happened...)
        /// does not cause the player to be added to the other party.
        /// </summary>
        [Test]
        public void PartyResponseAcceptExistingParty()
        {
            var handler = new PartyResponseAction(this.GetGameServer());

            // first put the player in a party with another player
            var player = CreatePartyMember(1);
            player.LastPartyRequester = CreatePartyMember(2);
            player.PlayerState.TryAdvanceTo(PlayerState.PartyRequest);
            handler.HandleResponse(player, true);

            // now another player will try to request party from the player, which should fail
            var requester = CreatePartyMember(3);
            player.PlayerView.PartyView.Expect(v => v.ShowPartyRequest(requester)).Repeat.Never();
            player.LastPartyRequester = requester;

            handler.HandleResponse(player, true);
            Assert.That(player.Party.PartyList, Is.Not.Contains(requester));
            Assert.That(player.LastPartyRequester, Is.Null);
            Assert.That(requester.Party, Is.Null);
            requester.PlayerView.PartyView.VerifyAllExpectations();
            player.PlayerView.PartyView.VerifyAllExpectations();
        }

        private static Player CreatePartyMember(ushort id)
        {
            var result = TestHelper.GetPlayer(id);
            result.PlayerState.TryAdvanceTo(PlayerState.EnteredWorld);
            return result;
        }

        private Party CreatePartyWithMembers(int numberOfMembers)
        {
            var party = new Party(5);
            for (ushort i = 0; i < numberOfMembers; i++)
            {
                var partyMember = CreatePartyMember(i);
                party.Add(partyMember);
            }

            return party;
        }

        private IGameContext GetGameServer()
        {
            var gameServer = MockRepository.GenerateStub<IGameContext>();
            gameServer.Stub(g => g.Configuration).Return(new GameConfiguration());
            gameServer.Configuration.MaximumPartySize = 5;
            return gameServer;
        }
    }
}
