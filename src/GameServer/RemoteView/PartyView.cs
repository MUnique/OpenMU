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
            using (var writer = this.connection.StartSafeWrite(0xC1, 5))
            {
                var packet = writer.Span;
                packet[2] = 0x40;
                packet[3] = requester.Id.GetHighByte();
                packet[4] = requester.Id.GetLowByte();
                writer.Commit();
            }
        }

        /// <inheritdoc/>
        public void PartyClosed()
        {
            using (var writer = this.connection.StartSafeWrite(0xC1, 4))
            {
                var packet = writer.Span;
                packet[2] = 0x42;
                packet[3] = 0x00;
                writer.Commit();
            }
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
            using (var writer = this.connection.StartSafeWrite(0xC1, 5 + (serializedSizePerPlayer * partyListCount)))
            {
                var packet = writer.Span;
                packet[2] = 0x42;
                packet[3] = 0x01;
                packet[4] = (byte)partyListCount;
                var offset = 5;
                for (int i = 0; i < partyListCount; i++)
                {
                    var partyMember = partyList[i];
                    packet.Slice(offset, 10).WriteString(partyMember.Name, Encoding.UTF8);
                    packet[offset + 10] = (byte)i;
                    packet[offset + 11] = (byte)partyMember.CurrentMap.MapId;
                    packet[offset + 12] = partyMember.X;
                    packet[offset + 13] = partyMember.Y;
                    ////14 + 15 are unknown
                    packet.Slice(offset + 16).SetIntegerBigEndian(partyMember.CurrentHealth);
                    packet.Slice(offset + 20).SetIntegerBigEndian(partyMember.MaximumHealth);
                    offset += serializedSizePerPlayer;
                }

                writer.Commit();
            }
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
            using (var writer = this.connection.StartSafeWrite(0xC1, partyListCount + 4))
            {
                var packet = writer.Span;
                packet[2] = 0x44;
                packet[3] = (byte)partyListCount;
                for (int i = 0; i < partyListCount; i++)
                {
                    packet[4 + i] = (byte)((i << 4) + this.HealthValues[i]);
                }

                writer.Commit();
            }
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
