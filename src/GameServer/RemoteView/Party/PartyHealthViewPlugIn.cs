// <copyright file="PartyHealthViewPlugIn.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.GameServer.RemoteView.Party
{
    using System.Runtime.InteropServices;
    using MUnique.OpenMU.GameLogic.Views.Party;
    using MUnique.OpenMU.Network;
    using MUnique.OpenMU.Network.Packets.ServerToClient;
    using MUnique.OpenMU.PlugIns;

    /// <summary>
    /// The default implementation of the party view which is forwarding everything to the game client which specific data packets.
    /// </summary>
    [PlugIn("Party View", "The default implementation of the party view which is forwarding everything to the game client which specific data packets.")]
    [Guid("CEE58BCB-FB8C-4AEB-9FC8-5D3A11FA7C03")]
    public class PartyHealthViewPlugIn : IPartyHealthViewPlugIn
    {
        private readonly RemotePlayer player;
        private byte[] healthValues;

        /// <summary>
        /// Initializes a new instance of the <see cref="PartyHealthViewPlugIn"/> class.
        /// </summary>
        /// <param name="player">The player.</param>
        public PartyHealthViewPlugIn(RemotePlayer player)
        {
            this.player = player;
        }

        private byte[] HealthValues => this.healthValues ??= new byte[this.player.Party.MaxPartySize];

        /// <inheritdoc/>
        public bool IsHealthUpdateNeeded()
        {
            return this.UpdateHealthValues();
        }

        /// <inheritdoc/>
        public void UpdatePartyHealth()
        {
            var partyListCount = this.player.Party.PartyList.Count;
            using var writer = this.player.Connection.StartSafeWrite(PartyHealthUpdate.HeaderType, PartyHealthUpdate.GetRequiredSize(partyListCount));
            var packet = new PartyHealthUpdate(writer.Span)
            {
                Count = (byte)partyListCount,
            };

            for (int i = 0; i < partyListCount; i++)
            {
                var member = packet[i];
                member.Index = (byte)i;
                member.Value = this.HealthValues[i];
            }

            writer.Commit();
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
