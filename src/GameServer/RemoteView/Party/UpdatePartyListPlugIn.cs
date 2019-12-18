// <copyright file="UpdatePartyListPlugIn.cs" company="MUnique">
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

            var partyList = party.PartyList;
            var partyListCount = party.PartyList?.Count ?? 0;
            using var writer = this.player.Connection.StartSafeWrite(PartyList.HeaderType, PartyList.GetRequiredSize(partyListCount));
            var packet = new PartyList(writer.Span)
            {
                Count = (byte)partyListCount,
            };

            for (byte i = 0; i < partyListCount; i++)
            {
                var partyMember = partyList[i];
                var partyMemberBlock = packet[i];
                partyMemberBlock.Index = i;
                partyMemberBlock.Name = partyMember.Name;
                partyMemberBlock.MapId = (byte)partyMember.CurrentMap.MapId;
                partyMemberBlock.PositionX = partyMember.Position.X;
                partyMemberBlock.PositionY = partyMember.Position.Y;
                partyMemberBlock.CurrentHealth = partyMember.CurrentHealth;
                partyMemberBlock.MaximumHealth = partyMember.MaximumHealth;
            }

            writer.Commit();
        }
    }
}