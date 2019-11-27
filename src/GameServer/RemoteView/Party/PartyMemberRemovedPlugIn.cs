// <copyright file="PartyMemberRemovedPlugIn.cs" company="MUnique">
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
    /// The default implementation of the <see cref="IPartyMemberRemovedPlugIn"/> which is forwarding everything to the game client with specific data packets.
    /// </summary>
    [PlugIn("PartyMemberRemovedPlugIn", "The default implementation of the IPartyMemberRemovedPlugIn which is forwarding everything to the game client with specific data packets.")]
    [Guid("d69b37b3-9e7f-40d1-9260-ba7a4f6369a2")]
    public class PartyMemberRemovedPlugIn : IPartyMemberRemovedPlugIn
    {
        private readonly RemotePlayer player;

        /// <summary>
        /// Initializes a new instance of the <see cref="PartyMemberRemovedPlugIn"/> class.
        /// </summary>
        /// <param name="player">The player.</param>
        public PartyMemberRemovedPlugIn(RemotePlayer player) => this.player = player;

        /// <inheritdoc/>
        public void PartyMemberRemoved(byte index)
        {
            using var writer = this.player.Connection.StartSafeWrite(RemovePartyMember.HeaderType, RemovePartyMember.Length);
            _ = new RemovePartyMember(writer.Span)
            {
                Index = index,
            };
            writer.Commit();
        }
    }
}