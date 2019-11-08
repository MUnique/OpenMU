// <copyright file="PlayerShopClosedPlugIn.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.GameServer.RemoteView.PlayerShop
{
    using System.Runtime.InteropServices;
    using MUnique.OpenMU.GameLogic;
    using MUnique.OpenMU.GameLogic.Views;
    using MUnique.OpenMU.GameLogic.Views.PlayerShop;
    using MUnique.OpenMU.Network;
    using MUnique.OpenMU.Network.Packets.ServerToClient;
    using MUnique.OpenMU.PlugIns;

    /// <summary>
    /// The default implementation of the <see cref="IPlayerShopClosedPlugIn"/> which is forwarding everything to the game client with specific data packets.
    /// </summary>
    [PlugIn("PlayerShopClosedPlugIn", "The default implementation of the IPlayerShopClosedPlugIn which is forwarding everything to the game client with specific data packets.")]
    [Guid("351cf726-5091-4995-9228-81c089d1da16")]
    public class PlayerShopClosedPlugIn : IPlayerShopClosedPlugIn
    {
        private readonly RemotePlayer player;

        /// <summary>
        /// Initializes a new instance of the <see cref="PlayerShopClosedPlugIn"/> class.
        /// </summary>
        /// <param name="player">The player.</param>
        public PlayerShopClosedPlugIn(RemotePlayer player) => this.player = player;

        /// <inheritdoc/>
        public void PlayerShopClosed(Player playerWithClosedShop)
        {
            var playerId = playerWithClosedShop.GetId(this.player);
            using (var writer = this.player.Connection.StartSafeWrite(
                Network.Packets.ServerToClient.PlayerShopClosed.HeaderType,
                Network.Packets.ServerToClient.PlayerShopClosed.Length))
            {
                _ = new PlayerShopClosed(writer.Span)
                {
                    PlayerId = playerId,
                };
                writer.Commit();
            }

            // The following usually just needs to be sent to all players which currently have the shop dialog open
            // For the sake of simplicity, we send it to all players.
            using (var writer = this.player.Connection.StartSafeWrite(ClosePlayerShopDialog.HeaderType, ClosePlayerShopDialog.Length))
            {
                _ = new ClosePlayerShopDialog(writer.Span)
                {
                    PlayerId = playerId,
                };
                writer.Commit();
            }
        }
    }
}
