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
    using MUnique.OpenMU.GameLogic.Views.Party;

    /// <summary>
    /// The party object. Contains a group of players who can chat with each other, and get informations about the health status of their party mates.
    /// </summary>
    public sealed class Party : IDisposable
    {
        private static readonly log4net.ILog Log = log4net.LogManager.GetLogger(typeof(Party));

        private readonly Timer healthUpdate;

        private readonly byte maxPartySize;

        private readonly List<Player> experienceDistributionList;

        /// <summary>
        /// Initializes a new instance of the <see cref="Party"/> class.
        /// </summary>
        /// <param name="maxPartySize">Maximum size of the party.</param>
        public Party(byte maxPartySize)
        {
            this.maxPartySize = maxPartySize;

            this.PartyList = new List<IPartyMember>(maxPartySize);
            this.experienceDistributionList = new List<Player>(this.MaxPartySize);
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
        /// </summary>
        /// <param name="sender">The sender.</param>
        public void KickMySelf(IPartyMember sender)
        {
            for (int i = 0; i < this.PartyList.Count; i++)
            {
                if (this.PartyList[i].Id == sender.Id)
                {
                    this.ExitParty(this.PartyList[i], (byte)i);
                    return;
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
                Log.WarnFormat("Suspicious request for sender with name: {0}, could be hack attempt.", sender.Name);
                return;
            }

            var toKick = this.PartyList[index];
            this.ExitParty(toKick, index);
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
                this.PartyList[i].ViewPlugIns.GetPlugIn<IChatViewPlugIn>()?.ChatMessage(message, senderCharacterName, ChatMessageType.Party);
            }
        }

        /// <summary>
        /// Distributes the experience after kill.
        /// </summary>
        /// <param name="killedObject">The object which was killed.</param>
        /// <param name="killer">The killer which is member of the party. All players which observe the killer, get experience.</param>
        /// <returns>
        /// The total distributed experience to all party members.
        /// </returns>
        public int DistributeExperienceAfterKill(IAttackable killedObject, IObservable killer)
        {
            lock (this.experienceDistributionList)
            {
                try
                {
                    return this.InternalDistributeExperienceAfterKill(killedObject, killer);
                }
                finally
                {
                    this.experienceDistributionList.Clear();
                }
            }
        }

        /// <inheritdoc/>
        public void Dispose()
        {
            if (this.PartyList != null)
            {
                for (byte i = 0; i < this.PartyList.Count; i++)
                {
                    this.PartyList[i].ViewPlugIns.GetPlugIn<IPartyMemberRemovedPlugIn>()?.PartyMemberRemoved(i);
                    this.PartyList[i].Party = null;
                }

                this.PartyList.Clear();
                this.PartyList = null;
                this.healthUpdate.Dispose();
            }
        }

        private int InternalDistributeExperienceAfterKill(IAttackable killedObject, IObservable killer)
        {
            if (killedObject is IObservable observable)
            {
                observable.ObserverLock.EnterReadLock();
                try
                {
                    this.experienceDistributionList.AddRange(this.PartyList.OfType<Player>().Where(p => killer.Observers.Contains(p)));
                }
                finally
                {
                    observable.ObserverLock.ExitReadLock();
                }
            }
            else
            {
                this.experienceDistributionList.AddRange(this.PartyList.OfType<Player>());
            }

            var count = this.experienceDistributionList.Count;
            if (count == 0)
            {
                return count;
            }

            var totalLevel = this.experienceDistributionList.Sum(p => (int)p.Attributes[Stats.Level]);
            var averageLevel = totalLevel / count;
            var averageExperience = killedObject.Attributes[Stats.Level] * 1000 / averageLevel;
            var totalAverageExperience = averageExperience * count * Math.Pow(1.2, count - 1);
            var randomizedTotalExperience = Rand.NextInt((int)(totalAverageExperience * 0.8), (int)(totalAverageExperience * 1.2));
            var randomizedTotalExperiencePerLevel = randomizedTotalExperience / (float)totalLevel;
            foreach (var player in this.experienceDistributionList)
            {
                player.AddExperience((int)(randomizedTotalExperiencePerLevel * player.Attributes[Stats.Level] * player.Attributes[Stats.ExperienceRate]), killedObject);
            }

            return randomizedTotalExperience;
        }

        private void ExitParty(IPartyMember player, byte index)
        {
            if (this.PartyList.Count < 3 || Equals(this.PartyMaster, player))
            {
                this.Dispose();
                return;
            }

            this.PartyList.Remove(player);
            player.Party = null;
            player.ViewPlugIns.GetPlugIn<IPartyMemberRemovedPlugIn>()?.PartyMemberRemoved(index);
            this.SendPartyList();
        }

        private void HealthUpdate_Elapsed(object state)
        {
            var partyMaster = this.PartyList.FirstOrDefault();
            if (partyMaster == null)
            {
                return;
            }

            bool updateNeeded = partyMaster.ViewPlugIns.GetPlugIn<IPartyHealthViewPlugIn>()?.IsHealthUpdateNeeded() ?? false;
            if (updateNeeded)
            {
                partyMaster.ViewPlugIns.GetPlugIn<IPartyHealthViewPlugIn>()?.UpdatePartyHealth();
                for (var i = this.PartyList.Count - 1; i >= 1; i--)
                {
                    var member = this.PartyList[i];
                    var plugIn = member.ViewPlugIns.GetPlugIn<IPartyHealthViewPlugIn>();
                    if (plugIn?.IsHealthUpdateNeeded() ?? false)
                    {
                        plugIn.UpdatePartyHealth();
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
                this.PartyList[i].ViewPlugIns.GetPlugIn<IUpdatePartyListPlugIn>()?.UpdatePartyList();
            }
        }
    }
}
