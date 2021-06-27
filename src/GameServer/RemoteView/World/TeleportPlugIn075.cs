// <copyright file="TeleportPlugIn075.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.GameServer.RemoteView.World
{
    using System.Runtime.InteropServices;
    using MUnique.OpenMU.GameLogic.Views.World;
    using MUnique.OpenMU.Network.Packets.ServerToClient;
    using MUnique.OpenMU.Network.PlugIns;
    using MUnique.OpenMU.PlugIns;

    /// <summary>
    /// The default implementation of the <see cref="ITeleportPlugIn"/> which is forwarding everything to the game client with specific data packets.
    /// </summary>
    [PlugIn(nameof(TeleportPlugIn075), "The default implementation of the ITeleportPlugIn which is forwarding everything to the game client with specific data packets.")]
    [Guid("490DB5E5-9DB6-4068-9708-E7D69F82BF3B")]
    [MaximumClient(0, 89, ClientLanguage.Invariant)]
    public class TeleportPlugIn075 : ITeleportPlugIn
    {
        private readonly RemotePlayer player;

        /// <summary>
        /// Initializes a new instance of the <see cref="TeleportPlugIn075"/> class.
        /// </summary>
        /// <param name="player">The player.</param>
        public TeleportPlugIn075(RemotePlayer player) => this.player = player;

        /// <inheritdoc/>
        public void ShowTeleported()
        {
            if (this.player.SelectedCharacter?.CurrentMap is null)
            {
                return;
            }

            var mapNumber = (byte)this.player.SelectedCharacter.CurrentMap.Number;
            var position = this.player.Position;
            this.player.Connection?.SendMapChanged075(mapNumber, position.X, position.Y, this.player.Rotation.ToPacketByte(), isMapChange: false);
        }
    }
}