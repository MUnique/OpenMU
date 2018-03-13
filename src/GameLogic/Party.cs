// <copyright file="Party.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.GameLogic
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading;

    using MUnique.OpenMU.GameLogic.Attributes;
    using MUnique.OpenMU.GameLogic.Views;

    /// <summary>
    /// The party object. Contains a group of players who can chat with each other, and get informations about the health status of their party mates.
    /// </summary>
    public sealed class Party : IDisposable
    {
        private readonly Timer healthUpdate;

        private readonly byte maxPartySize;

        /// <summary>
        /// Initializes a new instance of the <see cref="Party"/> class.
        /// </summary>
        /// <param name="maxPartySize">Maximum size of the party.</param>
        public Party(byte maxPartySize)
        {
            this.maxPartySize = maxPartySize;

            this.PartyList = new List<IPartyMember>(maxPartySize);
            var updateInterval = new TimeSpan(0, 0, 0, 0, 500);
            this.healthUpdate = new Timer(this.HealthUpdate_Elapsed, null, updateInterval, updateInterval);
        }

        /// <summary>
        /// Gets the party list.
        /// </summary>
        public IList<IPartyMember> PartyList { get; private set; }

        /// <summary>
        /// Gets the maximum size of the party.
        /// </summary>
        public byte MaxPartySize => this.maxPartySize;

        /// <summary>
        /// Gets the party master.
        /// </summary>
        public IPartyMember PartyMaster
        {
            get
            {
                if (this.PartyList.Count > 0)
                {
                    return this.PartyList[0];
                }
                else
                {
                    return null;
                }
            }
        }

        /// <summary>
        /// Kicks the player from the party.
        /// Only the party master is allowed to kick other players. However, players can kick themself out of the party.
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="index">The party list index of the member to kick.</param>
        public void KickPlayer(IPartyMember sender, byte index)
        {
            if (!Equals(sender, this.PartyList[0]) &&
                !Equals(sender, this.PartyList[index]))
            {
                // todo: maybe log wrong request as hack attempt
                return;
            }

            var toKick = this.PartyList[index];
            this.ExitParty(toKick);
        }

        /// <summary>
        /// Adds the specified new party mate.
        /// </summary>
        /// <param name="newPartyMate">The new party mate.</param>
        /// <returns><c>True</c>, if adding was successful; Otherwise, <c>false</c>.</returns>
        public bool Add(IPartyMember newPartyMate)
        {
            if (this.PartyList.Count >= this.maxPartySize)
            {
                return false;
            }

            this.PartyList.Add(newPartyMate);
            newPartyMate.Party = this;
            this.SendPartyList();
            return true;
        }

        /// <summary>
        /// Sends the chat message to all party members.
        /// </summary>
        /// <param name="message">The message.</param>
        /// <param name="senderCharacterName">The sender character name.</param>
        public void SendChatMessage(string message, string senderCharacterName)
        {
            for (int i = 0; i < this.PartyList.Count; i++)
            {
                this.PartyList[i].PartyView.ChatMessage(message, senderCharacterName, ChatMessageType.Party);
            }
        }

        /// <summary>
        /// Distributes the experience after kill.
        /// </summary>
        /// <param name="killedObject">The object which was killed.</param>
        /// <returns>The total distributed experience to all party members.</returns>
        public int DistributeExperienceAfterKill(IAttackable killedObject)
        {
            IList<Player> partyMembersInRange;
            if (killedObject is IObservable observable)
            {
                observable.ObserverLock.EnterReadLock();
                try
                {
                    partyMembersInRange = this.PartyList.OfType<Player>().Where(p => observable.Observers.Contains(p)).ToList();
                }
                finally
                {
                    observable.ObserverLock.ExitReadLock();
                }
            }
            else
            {
                partyMembersInRange = this.PartyList.OfType<Player>().ToList();
            }

            if (partyMembersInRange.Count == 0)
            {
                return 0;
            }

            var totalLevel = partyMembersInRange.Sum(p => (int)p.Attributes[Stats.Level]);
            var averageLevel = totalLevel / partyMembersInRange.Count;
            var averageExperience = killedObject.Attributes[Stats.Level] * 1000 / averageLevel;
            var totalAverageExperience = averageExperience * partyMembersInRange.Count * Math.Pow(1.2, partyMembersInRange.Count - 1);
            var randomizedTotalExperience = Rand.NextInt((int)(totalAverageExperience * 0.8), (int)(totalAverageExperience * 1.2));
            var randomizedTotalExperiencePerLevel = randomizedTotalExperience / totalLevel;
            foreach (var player in partyMembersInRange)
            {
                player.AddExperience((int)(randomizedTotalExperiencePerLevel * player.Attributes[Stats.Level]), killedObject);
            }

            return randomizedTotalExperience;
        }

        /// <inheritdoc/>
        public void Dispose()
        {
            if (this.PartyList != null)
            {
                for (byte i = 0; i < this.PartyList.Count; i++)
                {
                    this.PartyList[i].PartyView.PartyClosed();
                    this.PartyList[i].Party = null;
                }

                this.PartyList.Clear();
                this.PartyList = null;
                this.healthUpdate.Dispose();
            }
        }

        private void ExitParty(IPartyMember player)
        {
            if (this.PartyList.Count < 3 || Equals(this.PartyMaster, player))
            {
                this.Dispose();
                return;
            }

            this.PartyList.Remove(player);
            player.Party = null;
            player.PartyView.PartyClosed();
            this.SendPartyList();
        }

        private void HealthUpdate_Elapsed(object state)
        {
            var partyMaster = this.PartyList.FirstOrDefault();
            if (partyMaster == null)
            {
                return;
            }

            bool updateNeeded = partyMaster.PartyView.IsHealthUpdateNeeded();
            if (updateNeeded)
            {
                partyMaster.PartyView.UpdatePartyHealth();
                for (var i = this.PartyList.Count - 1; i >= 1; i--)
                {
                    var member = this.PartyList[i];
                    if (member.PartyView.IsHealthUpdateNeeded())
                    {
                        member.PartyView.UpdatePartyHealth();
                    }
                }
            }
        }

        private void SendPartyList()
        {
            if (this.PartyList.Count == 0)
            {
                return;
            }

            for (byte i = 0; i < this.PartyList.Count; i++)
            {
                this.PartyList[i].PartyView.UpdatePartyList();
            }
        }
    }
}
