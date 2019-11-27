// <copyright file="PlayerShopOpenedPlugIn.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.GameServer.RemoteView.PlayerShop
{
    using System.Collections.Generic;
    using System.Runtime.InteropServices;
    using MUnique.OpenMU.GameLogic;
    using MUnique.OpenMU.GameLogic.Views.PlayerShop;
    using MUnique.OpenMU.Network;
    using MUnique.OpenMU.Network.Packets.ServerToClient;
    using MUnique.OpenMU.PlugIns;

    /// <summary>
    /// The default implementation of the <see cref="IPlayerShopOpenedPlugIn"/> which is forwarding everything to the game client with specific data packets.
    /// </summary>
    [PlugIn("PlayerShopOpenedPlugIn", "The default implementation of the IPlayerShopOpenedPlugIn which is forwarding everything to the game client with specific data packets.")]
    [Guid("c61e9a96-57d0-4bf1-9667-62b0c2104314")]
    public class PlayerShopOpenedPlugIn : IPlayerShopOpenedPlugIn
    {
        private readonly RemotePlayer player;

        /// <summary>
        /// Initializes a new instance of the <see cref="PlayerShopOpenedPlugIn"/> class.
        /// </summary>
        /// <param name="player">The player.</param>
        public PlayerShopOpenedPlugIn(RemotePlayer player) => this.player = player;

        /// <inheritdoc/>
        public void PlayerShopOpened(Player playerWithShop)
        {
            this.player.ViewPlugIns.GetPlugIn<IShowShopsOfPlayersPlugIn>()?.ShowShopsOfPlayers(new List<Player>(1) { playerWithShop });
            if (this.player == playerWithShop)
            {
                // Success of opening the own shop
                using var writer = this.player.Connection.StartSafeWrite(PlayerShopOpenSuccessful.HeaderType, PlayerShopOpenSuccessful.Length);
                _ = new PlayerShopOpenSuccessful(writer.Span);
                writer.Commit();
            }
        }
    }
}