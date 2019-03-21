// <copyright file="UpdatePartyListPlugIn.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.GameServer.RemoteView.Party
{
    using System.Runtime.InteropServices;
    using System.Text;
    using MUnique.OpenMU.GameLogic.Views.Party;
    using MUnique.OpenMU.Network;
    using MUnique.OpenMU.PlugIns;

    /// <summary>
    /// The default implementation of the <see cref="IUpdatePartyListPlugIn"/> which is forwarding everything to the game client with specific data packets.
    /// </summary>
    [PlugIn("UpdatePartyListPlugIn", "The default implementation of the IUpdatePartyListPlugIn which is forwarding everything to the game client with specific data packets.")]
    [Guid("bf880a4b-f4f6-41f0-adff-6eab0e99d985")]
    public class UpdatePartyListPlugIn : IUpdatePartyListPlugIn
    {
        private readonly RemotePlayer player;

        /// <summary>
        /// Initializes a new instance of the <see cref="UpdatePartyListPlugIn"/> class.
        /// </summary>
        /// <param name="player">The player.</param>
        public UpdatePartyListPlugIn(RemotePlayer player) => this.player = player;

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
            using (var writer = this.player.Connection.StartSafeWrite(0xC1, 5 + (serializedSizePerPlayer * partyListCount)))
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
                    packet[offset + 12] = partyMember.Position.X;
                    packet[offset + 13] = partyMember.Position.Y;
                    ////14 + 15 are unknown
                    packet.Slice(offset + 16).SetIntegerBigEndian(partyMember.CurrentHealth);
                    packet.Slice(offset + 20).SetIntegerBigEndian(partyMember.MaximumHealth);
                    offset += serializedSizePerPlayer;
                }

                writer.Commit();
            }
        }
    }
}