// <copyright file="PartyView.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.GameServer.RemoteView
{
    using System.Text;
    using GameLogic.Views;
    using MUnique.OpenMU.GameLogic;
    using MUnique.OpenMU.Network;

    /// <summary>
    /// The default implementation of the party view which is forwarding everything to the game client which specific data packets.
    /// </summary>
    public class PartyView : ChatView, IPartyView
    {
        private readonly IConnection connection;
        private readonly IPartyMember player;
        private byte[] hpValues;

        /// <summary>
        /// Initializes a new instance of the <see cref="PartyView"/> class.
        /// </summary>
        /// <param name="connection">The connection.</param>
        /// <param name="player">The player.</param>
        public PartyView(IConnection connection, IPartyMember player)
            : base(connection)
        {
            this.connection = connection;
            this.player = player;
        }

        private byte[] HealthValues => this.hpValues ?? (this.hpValues = new byte[this.player.Party.MaxPartySize]);

        /// <inheritdoc/>
        public void ShowPartyRequest(IPartyMember requester)
        {
            this.connection.Send(new byte[] { 0xC1, 0x05, 0x40, requester.Id.GetHighByte(), requester.Id.GetLowByte() });
        }

        /// <inheritdoc/>
        public void PartyClosed()
        {
            this.connection.Send(new byte[] { 0xC1, 4, 0x42, 0 });
        }

        /// <inheritdoc/>
        public void UpdatePartyList()
        {
            var party = this.player.Party;
            if (party == null)
            {
                return;
            }

            const int serializedSizePerPlayer = 24;
            var partyList = party.PartyList;
            var partyListCount = party.PartyList?.Count ?? 0;
            var packet = new byte[5 + (serializedSizePerPlayer * partyListCount)];
            packet[0] = 0xC1;
            packet[1] = (byte)packet.Length;
            packet[2] = 0x42;
            packet[3] = 0x01;
            packet[4] = (byte)partyListCount;
            var offset = 5;
            for (int i = 0; i < partyListCount; i++)
            {
                var partyMember = partyList[i];
                Encoding.UTF8.GetBytes(partyMember.Name, 0, partyMember.Name.Length, packet, offset);
                packet[offset + 10] = (byte)i;
                packet[offset + 11] = (byte)partyMember.CurrentMap.MapId;
                packet[offset + 12] = partyMember.X;
                packet[offset + 13] = partyMember.Y;
                ////14 + 15 are unknown
                packet.SetIntegerSmallEndian(partyMember.CurrentHealth, offset + 16);
                packet.SetIntegerSmallEndian(partyMember.MaximumHealth, offset + 20);
                offset += serializedSizePerPlayer;
            }

            this.connection.Send(packet);
        }

        /// <inheritdoc/>
        public bool IsHealthUpdateNeeded()
        {
            return this.UpdateHealthValues();
        }

        /// <inheritdoc/>
        public void UpdatePartyHealth()
        {
            var partyListCount = this.player.Party.PartyList.Count;
            var packet = new byte[partyListCount + 4];
            packet[0] = 0xC1;
            packet[1] = (byte)packet.Length;
            packet[2] = 0x44;
            packet[3] = (byte)partyListCount;
            for (int i = 0; i < partyListCount; i++)
            {
               packet[4 + i] = (byte)((i << 4) + this.HealthValues[i]);
            }

            this.connection.Send(packet);
        }

        private bool UpdateHealthValues()
        {
            bool updated = false;
            for (int i = this.player.Party.PartyList.Count - 1; i >= 0; --i)
            {
                var partyMember = this.player.Party.PartyList[i];
                double value = (double)partyMember.CurrentHealth / partyMember.MaximumHealth;
                var newValue = (byte)(value * 10);
                if (this.HealthValues[i] != newValue)
                {
                    updated = true;
                    this.HealthValues[i] = newValue;
                }
            }

            return updated;
        }
    }
}
