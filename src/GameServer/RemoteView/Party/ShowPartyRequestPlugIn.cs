// <copyright file="ShowPartyRequestPlugIn.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.GameServer.RemoteView.Party
{
    using System.Runtime.InteropServices;
    using MUnique.OpenMU.GameLogic;
    using MUnique.OpenMU.GameLogic.Views.Party;
    using MUnique.OpenMU.Network;
    using MUnique.OpenMU.Network.Packets.ServerToClient;
    using MUnique.OpenMU.PlugIns;

    /// <summary>
    /// The default implementation of the <see cref="IShowPartyRequestPlugIn"/> which is forwarding everything to the game client with specific data packets.
    /// </summary>
    [PlugIn("ShowPartyRequestPlugIn", "The default implementation of the IShowPartyRequestPlugIn which is forwarding everything to the game client with specific data packets.")]
    [Guid("d25e8b80-3128-4102-8916-98f3d68aa065")]
    public class ShowPartyRequestPlugIn : IShowPartyRequestPlugIn
    {
        private readonly RemotePlayer player;

        /// <summary>
        /// Initializes a new instance of the <see cref="ShowPartyRequestPlugIn"/> class.
        /// </summary>
        /// <param name="player">The player.</param>
        public ShowPartyRequestPlugIn(RemotePlayer player) => this.player = player;

        /// <inheritdoc/>
        public void ShowPartyRequest(IPartyMember requester)
        {
            using var writer = this.player.Connection.StartSafeWrite(PartyRequest.HeaderType, PartyRequest.Length);
            _ = new PartyRequest(writer.Span)
            {
                RequesterId = requester.Id,
            };
            writer.Commit();
        }
    }
}