// <copyright file="ShowShopsOfPlayersPlugIn.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.GameServer.RemoteView.PlayerShop
{
    using System.Collections.Generic;
    using System.Runtime.InteropServices;
    using System.Text;
    using MUnique.OpenMU.GameLogic;
    using MUnique.OpenMU.GameLogic.Views;
    using MUnique.OpenMU.GameLogic.Views.PlayerShop;
    using MUnique.OpenMU.Network;
    using MUnique.OpenMU.Network.Packets;
    using MUnique.OpenMU.PlugIns;

    /// <summary>
    /// The default implementation of the <see cref="IShowShopsOfPlayersPlugIn"/> which is forwarding everything to the game client with specific data packets.
    /// </summary>
    [PlugIn("ShowShopsOfPlayersPlugIn", "The default implementation of the IShowShopsOfPlayersPlugIn which is forwarding everything to the game client with specific data packets.")]
    [Guid("619df3b3-6559-4336-975f-04a2f5867f38")]
    public class ShowShopsOfPlayersPlugIn : IShowShopsOfPlayersPlugIn
    {
        private readonly RemotePlayer player;

        /// <summary>
        /// Initializes a new instance of the <see cref="ShowShopsOfPlayersPlugIn"/> class.
        /// </summary>
        /// <param name="player">The player.</param>
        public ShowShopsOfPlayersPlugIn(RemotePlayer player) => this.player = player;

        /// <inheritdoc />
        public void ShowShopsOfPlayers(ICollection<Player> playersWithShop)
        {
            const int sizePerShop = 38;
            const int headerSize = 6;
            var shopCount = playersWithShop.Count;
            using (var writer = this.player.Connection.StartSafeWrite(0xC2, headerSize + (sizePerShop * shopCount)))
            {
                var packet = writer.Span;
                packet[3] = 0x3F;
                packet[5] = (byte)shopCount;
                int offset = headerSize;
                foreach (var shopPlayer in playersWithShop)
                {
                    var shopPlayerId = shopPlayer.GetId(this.player);
                    var shopBlock = packet.Slice(offset, sizePerShop);
                    shopBlock.SetShortLittleEndian(shopPlayerId);
                    shopBlock.Slice(2).WriteString(shopPlayer.ShopStorage.StoreName, Encoding.UTF8);
                    offset += sizePerShop;
                }

                writer.Commit();
            }
        }
    }
}