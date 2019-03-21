// <copyright file="ShowGuildJoinRequestPlugIn.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.GameServer.RemoteView.Guild
{
    using System.Runtime.InteropServices;
    using MUnique.OpenMU.GameLogic;
    using MUnique.OpenMU.GameLogic.Views.Guild;
    using MUnique.OpenMU.Network;
    using MUnique.OpenMU.PlugIns;

    /// <summary>
    /// The default implementation of the <see cref="IShowGuildJoinRequestPlugIn"/> which is forwarding everything to the game client with specific data packets.
    /// </summary>
    [PlugIn("ShowGuildJoinRequestPlugIn", "The default implementation of the IShowGuildJoinRequestPlugIn which is forwarding everything to the game client with specific data packets.")]
    [Guid("521ff03d-c8ad-44d9-a23d-8f98c4c174ae")]
    public class ShowGuildJoinRequestPlugIn : IShowGuildJoinRequestPlugIn
    {
        private readonly RemotePlayer player;

        /// <summary>
        /// Initializes a new instance of the <see cref="ShowGuildJoinRequestPlugIn"/> class.
        /// </summary>
        /// <param name="player">The player.</param>
        public ShowGuildJoinRequestPlugIn(RemotePlayer player) => this.player = player;

        /// <inheritdoc/>
        public void ShowGuildJoinRequest(Player requester)
        {
            using (var writer = this.player.Connection.StartSafeWrite(0xC1, 5))
            {
                var packet = writer.Span;
                packet[2] = 0x50;
                packet[3] = requester.Id.GetHighByte();
                packet[4] = requester.Id.GetLowByte();
                writer.Commit();
            }
        }
    }
}